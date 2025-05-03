using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QsitColumnAutomator
{
    [Transaction(TransactionMode.Manual)]
    public class AssignRandomComments : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;
                Document document = uiDoc.Document;

                // Step 1: Ask user to choose "Architectural Columns" or "Structural Columns"
                TaskDialog categoryDialog = new TaskDialog("Choose Category");
                categoryDialog.MainInstruction = "Choose the category to apply comments to:";
                categoryDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Architectural Columns");
                categoryDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Structural Columns");
                TaskDialogResult categoryResult = categoryDialog.Show();

                BuiltInCategory selectedCategory;

                if (categoryResult == TaskDialogResult.CommandLink1)
                {
                    selectedCategory = BuiltInCategory.OST_Columns; // Architectural Columns
                }
                else if (categoryResult == TaskDialogResult.CommandLink2)
                {
                    selectedCategory = BuiltInCategory.OST_StructuralColumns; // Structural Columns
                }
                else
                {
                    TaskDialog.Show("Cancelled", "No category selected.");
                    return Result.Cancelled;
                }

                // Step 2: Identify Family Types for placed instances in the selected category
                // FIXED: Changed to properly identify unique family types
                List<FamilySymbol> familySymbols = IdentifyFamilyTypes(document, selectedCategory);

                if (familySymbols.Count == 0)
                {
                    TaskDialog.Show("No Columns Found", "No placed columns found in the selected category.");
                    return Result.Cancelled;
                }

                // Step 3: Let user select a Family Type from the identified FamilySymbols
                FamilySymbol selectedFamilyType = SelectFamilyType(familySymbols);
                if (selectedFamilyType == null)
                {
                    TaskDialog.Show("Cancelled", "No valid family type selected.");
                    return Result.Cancelled;
                }

                // Step 4: Ask user to choose between entering a comment or randomizing comments
                TaskDialog commentDialog = new TaskDialog("Choose Comment Action");
                commentDialog.MainInstruction = "Do you want to enter a comment or randomize comments?";
                commentDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Enter a Comment");
                commentDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Randomize Comments");
                TaskDialogResult commentResult = commentDialog.Show();

                // Step 5: Apply selected comment action
                if (commentResult == TaskDialogResult.CommandLink1)
                {
                    // Let user enter a custom comment
                    string userComment = ShowInputDialog("Enter a Comment", "Enter the comment for the selected column:");
                    if (string.IsNullOrEmpty(userComment))
                    {
                        TaskDialog.Show("No Comment", "You must enter a valid comment.");
                        return Result.Cancelled;
                    }
                    // Apply the entered comment to all instances of the selected FamilyType
                    ApplyCommentToFamilyType(selectedFamilyType, userComment, false);
                }
                else if (commentResult == TaskDialogResult.CommandLink2)
                {
                    // Apply a random comment to all instances of the selected FamilyType
                    ApplyCommentToFamilyType(selectedFamilyType, null, true);
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private List<FamilySymbol> IdentifyFamilyTypes(Document document, BuiltInCategory category)
        {
            // FIXED: Changed method to get distinct family symbols
            // First get all placed instances in the selected category
            var placedColumns = new FilteredElementCollector(document)
                .OfCategory(category)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            // Get the family symbols, making sure to use distinct by ID to avoid duplicates
            var familySymbols = placedColumns
                .Select(f => f.Symbol)
                .GroupBy(s => s.Id.IntegerValue) // Group by ID to get unique symbols
                .Select(g => g.First())          // Take the first from each group
                .ToList();

            return familySymbols;
        }

        private FamilySymbol SelectFamilyType(List<FamilySymbol> familySymbols)
        {
            // Step 3: Present user with a list of FamilySymbols (Family Types) to choose from
            TaskDialog familyDialog = new TaskDialog("Select Column Family Type");
            familyDialog.MainInstruction = "Select a column family type to assign a comment:";

            Dictionary<TaskDialogResult, FamilySymbol> familyMap = new Dictionary<TaskDialogResult, FamilySymbol>();
            int baseId = 1000;

            // Display each family type with a unique ID
            foreach (var familySymbol in familySymbols)
            {
                string displayName = $"{familySymbol.Family.Name} - {familySymbol.Name}";
                TaskDialogResult resultId = (TaskDialogResult)(baseId++);
                familyDialog.AddCommandLink((TaskDialogCommandLinkId)resultId, displayName);
                familyMap[resultId] = familySymbol;
            }

            // Show the dialog and get the user's choice
            TaskDialogResult selectedFamilyResult = familyDialog.Show();

            return familyMap.ContainsKey(selectedFamilyResult) ? familyMap[selectedFamilyResult] : null;
        }

        private string ShowInputDialog(string title, string prompt)
        {
            using (System.Windows.Forms.Form inputForm = new System.Windows.Forms.Form())
            {
                inputForm.Text = title;
                System.Windows.Forms.Label promptLabel = new System.Windows.Forms.Label { Text = prompt, Dock = System.Windows.Forms.DockStyle.Top };
                System.Windows.Forms.TextBox inputTextBox = new System.Windows.Forms.TextBox { Dock = System.Windows.Forms.DockStyle.Top };
                System.Windows.Forms.Button submitButton = new System.Windows.Forms.Button { Text = "OK", Dock = System.Windows.Forms.DockStyle.Bottom };

                submitButton.Click += (sender, e) =>
                {
                    inputForm.DialogResult = System.Windows.Forms.DialogResult.OK;
                    inputForm.Close();
                };

                inputForm.Controls.Add(inputTextBox);
                inputForm.Controls.Add(promptLabel);
                inputForm.Controls.Add(submitButton);
                inputForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                inputForm.AcceptButton = submitButton;

                System.Windows.Forms.DialogResult result = inputForm.ShowDialog();
                return result == System.Windows.Forms.DialogResult.OK ? inputTextBox.Text : string.Empty;
            }
        }

        private void ApplyCommentToFamilyType(FamilySymbol selectedFamilyType, string baseComment, bool isRandom)
        {
            Random rand = new Random();

            // Step 6: Find all instances of the selected FamilyType
            var placedInstances = new FilteredElementCollector(selectedFamilyType.Document)
                .OfCategoryId(selectedFamilyType.Category.Id)  // Correct method to use Category.Id
                .WhereElementIsNotElementType() // Only placed instances
                .Cast<FamilyInstance>()
                .Where(f => f.Symbol.Id == selectedFamilyType.Id) // Only instances of the selected family type
                .ToList();

            using (Transaction trans = new Transaction(selectedFamilyType.Document, "Assign Comment"))
            {
                trans.Start();

                // Look up the Comments parameter and apply the comment
                foreach (var instance in placedInstances)
                {
                    Parameter param = instance.LookupParameter("Comments");
                    if (param != null && !param.IsReadOnly) // Ensure the parameter is not read-only
                    {
                        string finalComment = isRandom ? GenerateRandomComment(rand) : baseComment;
                        param.Set(finalComment); // Apply the comment
                    }
                }

                trans.Commit();
            }
        }

        private string GenerateRandomComment(Random rand)
        {
            char letter = (char)('A' + rand.Next(0, 26));
            int number = rand.Next(1, 100);
            return $"{letter}{number}";
        }
    }
}