using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace QsitPointSelection
{
    [TransactionAttribute(TransactionMode.ReadOnly)]  
    public class PointSelection:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Midpoint selection
                XYZ midPoint = uiDoc.Selection.PickPoint(ObjectSnapTypes.Midpoints);
                TaskDialog.Show("Coordinate Point", $"MidPoint X: {midPoint.X} \n Y: {midPoint.Y} \n Z: {midPoint.Z}");

                // Endpoint selection
                XYZ endPoint = uiDoc.Selection.PickPoint(ObjectSnapTypes.Endpoints);
                TaskDialog.Show("Coordinate Point", $"EndPoint X: {endPoint.X} \n Y: {endPoint.Y} \n Z: {endPoint.Z}");

                // Nearest point selection
                XYZ nearestPoint = uiDoc.Selection.PickPoint(ObjectSnapTypes.Nearest);
                TaskDialog.Show("Coordinate Point", $"NearestPoint X: {nearestPoint.X} \n Y: {nearestPoint.Y} \n Z: {nearestPoint.Z}");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
