using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaBIM.Models
{
   public class BeamColumnFloorSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category == null)
            {
                return false;
            }

            return elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralFraming) ||
                   elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns)||
                   elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
