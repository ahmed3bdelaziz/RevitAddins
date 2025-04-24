using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace QsitDeleteSelectedElement
{
    [Transaction(TransactionMode.Manual)]
    public class DeleteWindowsByRectangle : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Allow the user to select multiple elements
                IList<Element> selectedElements = uiDoc.Selection.PickElementsByRectangle("Select elements to consider for deletion");

                if (selectedElements.Count == 0)
                {
                    TaskDialog.Show("No Elements", "No elements were selected.");
                    return Result.Succeeded;
                }

                // Filter the selected elements to only windows
                IList<Element> windowsToDelete = selectedElements
                    .Where(e => e.Category != null && e.Category.Id.Value == (int)BuiltInCategory.OST_Windows) // Updated to use Value instead of IntegerValue
                    .ToList();

                if (windowsToDelete.Count == 0)
                {
                    TaskDialog.Show("No Windows", "No windows were selected for deletion.");
                    return Result.Succeeded;
                }

                // Build a list of windows with their types and ids for confirmation
                string elementList = string.Join(Environment.NewLine, windowsToDelete.Select(e =>
                {
                    string typeName = doc.GetElement(e.GetTypeId())?.Name ?? "Unknown Type";
                    return $"- {typeName} (Id: {e.Id.Value})"; // Updated to use Value instead of IntegerValue
                }));

                // Show confirmation dialog
                TaskDialog confirm = new TaskDialog("Confirm Deletion");
                confirm.MainInstruction = $"Delete {windowsToDelete.Count} window(s)?";
                confirm.MainContent = $"Selected Windows:\n{elementList}";
                confirm.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
                var result = confirm.Show();

                if (result == TaskDialogResult.Yes)
                {
                    // Start a transaction to delete the selected windows
                    using (Transaction tx = new Transaction(doc, "Delete Selected Windows"))
                    {
                        tx.Start();
                        foreach (var elem in windowsToDelete)
                        {
                            doc.Delete(elem.Id);
                        }
                        tx.Commit();
                    }

                    TaskDialog.Show("Deleted", $"{windowsToDelete.Count} window(s) were deleted.");
                }
                else
                {
                    TaskDialog.Show("Canceled", "No elements were deleted.");
                }
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
