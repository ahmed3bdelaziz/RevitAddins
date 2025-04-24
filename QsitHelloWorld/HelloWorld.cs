using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QsitHelloWorld
{
    [TransactionAttribute(TransactionMode.ReadOnly)]       
    public class HelloWorld : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                TaskDialog.Show("Revit", "Hello World from Revit!");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = "An error occurred: " + ex.Message;
                return Result.Failed;
            }
        }
    }
}
