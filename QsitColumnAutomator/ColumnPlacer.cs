using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QsitColumnAutomator
{
    [Transaction(TransactionMode.Manual)]
    public class ColumnPlacer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;
                Document document = uiDoc.Document;

                // Show category selection dialog (Architectural or Structural)
                TaskDialog categoryDialog = new TaskDialog("Choose Column Category");
                categoryDialog.MainInstruction = "Select a column category:";
                categoryDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Architectural Columns");
                categoryDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Structural Columns");

                TaskDialogResult categoryChoice = categoryDialog.Show();

                // Determine category based on user selection
                BuiltInCategory chosenCategory = categoryChoice == TaskDialogResult.CommandLink1
                    ? BuiltInCategory.OST_Columns // Architectural columns
                    : BuiltInCategory.OST_StructuralColumns; // Structural columns

                // Collect families from the chosen category
                List<Family> families = new FilteredElementCollector(document)
                    .OfClass(typeof(Family))
                    .Cast<Family>()
                    .Where(f => f.FamilyCategory != null && f.FamilyCategory.Id.IntegerValue == (int)chosenCategory)
                    .ToList();

                if (families.Count == 0)
                {
                    TaskDialog.Show("Error", "No families found in the selected category.");
                    return Result.Failed;
                }

                // Show family selection dialog
                TaskDialog familyDialog = new TaskDialog("Choose Family");
                familyDialog.MainInstruction = "Select a family:";
                Dictionary<TaskDialogResult, Family> familyMap = new Dictionary<TaskDialogResult, Family>();
                int baseId = 1000;
                for (int i = 0; i < families.Count; i++)
                {
                    TaskDialogResult resultId = (TaskDialogResult)(baseId + i);
                    familyDialog.AddCommandLink((TaskDialogCommandLinkId)resultId, families[i].Name);
                    familyMap[resultId] = families[i];
                }

                TaskDialogResult familyResult = familyDialog.Show();
                if (!familyMap.ContainsKey(familyResult))
                {
                    TaskDialog.Show("Cancelled", "No valid family selected.");
                    return Result.Cancelled;
                }
                Family selectedFamily = familyMap[familyResult];

                // Collect types (symbols) from the selected family
                IEnumerable<ElementId> symbolIds = selectedFamily.GetFamilySymbolIds();
                List<FamilySymbol> familySymbols = symbolIds
                    .Select(id => document.GetElement(id) as FamilySymbol)
                    .ToList();

                if (familySymbols.Count == 0)
                {
                    TaskDialog.Show("Error", "No types found for the selected family.");
                    return Result.Failed;
                }

                // Show type selection dialog
                TaskDialog typeDialog = new TaskDialog("Choose Column Type");
                typeDialog.MainInstruction = "Select a column type:";
                Dictionary<TaskDialogResult, FamilySymbol> typeMap = new Dictionary<TaskDialogResult, FamilySymbol>();
                for (int i = 0; i < familySymbols.Count; i++)
                {
                    string displayName = familySymbols[i].FamilyName + " - " + familySymbols[i].Name;
                    TaskDialogResult resultId = (TaskDialogResult)(baseId + i);
                    typeDialog.AddCommandLink((TaskDialogCommandLinkId)resultId, displayName);
                    typeMap[resultId] = familySymbols[i];
                }

                TaskDialogResult typeResult = typeDialog.Show();
                if (!typeMap.ContainsKey(typeResult))
                {
                    TaskDialog.Show("Cancelled", "No valid column type selected.");
                    return Result.Cancelled;
                }
                FamilySymbol selectedSymbol = typeMap[typeResult];

                // Prompt for number of columns
                TaskDialog columnCountDialog = new TaskDialog("Number of Columns");
                columnCountDialog.MainInstruction = "Choose number of columns to place (1–10):";
                Dictionary<TaskDialogResult, int> countMap = new Dictionary<TaskDialogResult, int>();
                for (int i = 1; i <= 10; i++)
                {
                    TaskDialogResult resultId = (TaskDialogResult)(baseId + i);
                    columnCountDialog.AddCommandLink((TaskDialogCommandLinkId)resultId, i.ToString());
                    countMap[resultId] = i;
                }

                TaskDialogResult countResult = columnCountDialog.Show();
                if (!countMap.ContainsKey(countResult))
                {
                    TaskDialog.Show("Cancelled", "No valid count selected.");
                    return Result.Cancelled;
                }
                int numberOfColumns = countMap[countResult];

                // Ask user for placement method
                TaskDialog methodDialog = new TaskDialog("Placement Method");
                methodDialog.MainInstruction = "Choose placement method:";
                methodDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Place Automatically (pattern)");
                methodDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Place Manually with Cursor");

                TaskDialogResult methodChoice = methodDialog.Show();

                // Get the available levels
                List<Level> levels = new FilteredElementCollector(document)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .OrderBy(l => l.Elevation)
                    .ToList();

                if (levels.Count == 0)
                {
                    TaskDialog.Show("Error", "No levels found in the project.");
                    return Result.Failed;
                }

                // Show level selection dialog
                TaskDialog levelDialog = new TaskDialog("Choose Level");
                levelDialog.MainInstruction = "Select a level to place the columns on.";
                Dictionary<TaskDialogResult, Level> levelMap = new Dictionary<TaskDialogResult, Level>();
                for (int i = 0; i < levels.Count; i++)
                {
                    TaskDialogResult resultId = (TaskDialogResult)(baseId + i);
                    levelDialog.AddCommandLink((TaskDialogCommandLinkId)resultId, levels[i].Name);
                    levelMap[resultId] = levels[i];
                }

                TaskDialogResult levelResult = levelDialog.Show();
                if (!levelMap.ContainsKey(levelResult))
                {
                    TaskDialog.Show("Cancelled", "No valid level selected.");
                    return Result.Cancelled;
                }
                Level selectedLevel = levelMap[levelResult];

                // Start the transaction to place the columns
                using (Transaction trans = new Transaction(document, "Place Columns"))
                {
                    trans.Start();

                    // Ensure the selected type is active
                    if (!selectedSymbol.IsActive)
                    {
                        selectedSymbol.Activate();
                        document.Regenerate();
                    }

                    if (methodChoice == TaskDialogResult.CommandLink1)
                    {
                        // Ask user to pick a center point for the pattern
                        XYZ center = uiDoc.Selection.PickPoint("Pick a center point for the pattern");

                        // Place columns in a geometric pattern
                        double radius = 10; // feet
                        List<XYZ> points = GeneratePointsPattern(center, radius, numberOfColumns);

                        foreach (XYZ point in points)
                        {
                            document.Create.NewFamilyInstance(point, selectedSymbol, selectedLevel, StructuralType.Column);
                        }
                    }
                    else if (methodChoice == TaskDialogResult.CommandLink2)
                    {
                        // Improved instruction for manual placement
                        TaskDialog.Show("Manual Placement",
                            "You will now place columns using the cursor.\n\n" +
                            "Please make sure you're viewing the selected level (a plan view has been activated if available).\n\n" +
                            "Click to place each column. Press ESC to cancel at any time.");

                        for (int i = 0; i < numberOfColumns; i++)
                        {
                            try
                            {
                                XYZ point = uiDoc.Selection.PickPoint("Pick point for column " + (i + 1));
                                document.Create.NewFamilyInstance(point, selectedSymbol, selectedLevel, StructuralType.Column);
                            }
                            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                            {
                                break; // User cancelled
                            }
                        }
                    }

                    trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        /// Generates evenly spaced points in a circular pattern for automatic placement.
        /// </summary>
        private List<XYZ> GeneratePointsPattern(XYZ center, double radius, int count)
        {
            List<XYZ> points = new List<XYZ>();

            if (count == 1)
            {
                points.Add(center);
            }
            else
            {
                double angleStep = 2 * Math.PI / count;
                for (int i = 0; i < count; i++)
                {
                    double angle = i * angleStep;
                    double x = center.X + radius * Math.Cos(angle);
                    double y = center.Y + radius * Math.Sin(angle);
                    points.Add(new XYZ(x, y, center.Z));
                }
            }
            return points;
        }
    }
}
