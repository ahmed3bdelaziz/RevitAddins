using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QsitColumnAutomator
{
    [Transaction(TransactionMode.Manual)]
    public class PreviewColumnsTaskDialog : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Step 1: Ask the user to select the column category (Architectural or Structural)
            TaskDialog categoryDialog = new TaskDialog("Select Column Category");
            categoryDialog.MainInstruction = "Choose the column category to preview:";
            categoryDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Architectural Columns");
            categoryDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Structural Columns");
            TaskDialogResult categoryResult = categoryDialog.Show();

            // Step 2: Based on the user's selection, collect the appropriate columns
            List<FamilyInstance> columns;
            BuiltInCategory category;
            if (categoryResult == TaskDialogResult.CommandLink1)
            {
                // Architectural Columns
                category = BuiltInCategory.OST_Columns;
            }
            else
            {
                // Structural Columns
                category = BuiltInCategory.OST_StructuralColumns;
            }

            // Collect columns from the selected category
            columns = new FilteredElementCollector(doc)
                .OfCategory(category)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            if (!columns.Any())
            {
                TaskDialog.Show("Column Preview", "No columns found in the selected category.");
                return Result.Succeeded;
            }

            // Step 3: Build a summary string for the selected columns
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{category.ToString()} Summary\n");

            int counter = 1;
            foreach (var col in columns)
            {
                string family = col.Symbol.Family.Name;
                string type = col.Name;
                string level = doc.GetElement(col.LevelId)?.Name ?? "N/A";
                string comment = col.LookupParameter("Comments")?.AsString() ?? "";

                sb.AppendLine($"{counter}. Family: {family}");
                sb.AppendLine($"   Type: {type}");
                sb.AppendLine($"   Level: {level}");
                sb.AppendLine($"   Comment: {comment}\n");

                counter++;
                if (counter > 30) // Limit to first 30 items to avoid overflow
                {
                    sb.AppendLine("... Only first 30 columns are shown.");
                    break;
                }
            }

            // Step 4: Display the summary in a TaskDialog
            TaskDialog td = new TaskDialog("Column Preview");
            td.MainInstruction = $"{category.ToString()} Summary";
            td.MainContent = sb.ToString();
            td.Show();

            return Result.Succeeded;
        }
    }
}
