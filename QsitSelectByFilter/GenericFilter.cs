using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QsitSelectByFilter
{
    public class GenericFilter:ISelectionFilter
    {
        public BuiltInCategory _category { get; set; }

        public GenericFilter(BuiltInCategory category) 
        {
            _category = category;
        }

        public bool AllowElement(Element elem)
        {
            if (elem == null || elem.Category == null)
                return false;

            BuiltInCategory elemCategory = (BuiltInCategory)elem.Category.Id.Value;
            return elemCategory == _category;
        }



        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }




    }
}
