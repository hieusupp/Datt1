#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
#endregion

namespace AlphaBIM
{
    public class SlabFoundationFilter : ISelectionFilter
    {
        public bool AllowElement(Element el)
        {
            // cho phép chọn el nếu kết quả dưới đây trả về true
            return el.Category.Name.Equals("Structural Foundations");
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}