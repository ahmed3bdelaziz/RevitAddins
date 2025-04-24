using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QsitSelectByFilter
{
    [TransactionAttribute(TransactionMode.ReadOnly)] 
    public class SelectFiltered : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;

            try
            {
                //Reference selectedElement = uIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                //Element element = document.GetElement(selectedElement.ElementId);
                //ElementType elementType = document.GetElement(element.GetTypeId()) as ElementType;

                //TaskDialog.Show("QSIT ELEMENT DETAILS",
                //    $"ID: {element.Id}{Environment.NewLine}" +
                //    $"Instance: {element.Name}{Environment.NewLine}" +
                //    $"Type: {elementType.Name}{Environment.NewLine}" +
                //    $"Family: {elementType.FamilyName}{Environment.NewLine}" +
                //    $"Category: {element.Category.Name}");

                GenericFilter filter = new GenericFilter(BuiltInCategory.OST_Walls);
                IList<Element> elementsFiltered = uIDocument.Selection.PickElementsByRectangle(filter,"Only Pick Walls");
                TaskDialog.Show("QSIT ELEMENTS SELECTED",
                    $"Count of Selected Walls By Rectangle : {elementsFiltered.Count}" );
            }

            catch (Exception ex)
            {
                // Handle any exceptions that occur
                TaskDialog.Show("Error", $"An error occurred: {ex.Message}");
            }

            return Result.Succeeded;



        }











    }
    }

