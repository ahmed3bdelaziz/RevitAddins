using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace QsitDeleteSelectedElement
{
    [Transaction(TransactionMode.Manual)]
    public class DeleteSelectedElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Prompt user to select one element
                Reference pickedRef = uiDoc.Selection.PickObject(ObjectType.Element, "Select an element to delete.");
                if (pickedRef == null) return Result.Cancelled;

                Element element = doc.GetElement(pickedRef);
                ElementId elementId = element.Id; // store ID before deletion

                // Show confirmation dialog
                TaskDialog confirmDialog = new TaskDialog("Confirm Deletion");
                confirmDialog.MainInstruction = "Do you want to delete this element?";
                confirmDialog.MainContent = $"Element ID: {elementId.Value}\nCategory: {element.Category?.Name}";
                confirmDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

                TaskDialogResult result = confirmDialog.Show();

                if (result == TaskDialogResult.Yes)
                {
                    using (Transaction tx = new Transaction(doc, "Delete Selected Element"))
                    {
                        tx.Start();
                        doc.Delete(elementId);
                        tx.Commit();
                    }

                    TaskDialog.Show("Deleted", $"Element {elementId.Value} was deleted.");
                }
                else
                {
                    TaskDialog.Show("Canceled", "Element was not deleted.");
                }
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled; // User canceled the selection
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
