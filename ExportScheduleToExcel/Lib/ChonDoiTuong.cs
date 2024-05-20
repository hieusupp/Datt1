using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace AlphaBIM
{
    public static class ChonDoiTuong
    {

        /// <summary>
        /// Lấy về all textnote có ở view đụng với element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="doc"></param>
        /// <param name="saiSo"></param>
        /// <returns></returns>
        public static List<TextNote> GetTextNoteIntersectWithElement2(
            this Element element,
            Document doc, double saiSo)
        {
            BoundingBoxXYZ box = element.get_BoundingBox(doc.ActiveView);

            XYZ minPoint = new XYZ(box.Min.X - AlphaBIMUnitUtils.MmToFeet(saiSo),
                box.Min.Y - AlphaBIMUnitUtils.MmToFeet(saiSo), 0);

            XYZ maxPoint = new XYZ(box.Max.X + AlphaBIMUnitUtils.MmToFeet(saiSo),
                box.Max.Y + AlphaBIMUnitUtils.MmToFeet(saiSo), 0);

            Outline outlineElement = new Outline(minPoint, maxPoint);



            List<TextNote> allTextNote
                = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_TextNotes)
                    .Cast<TextNote>()
                    .ToList();



            List<TextNote> listTextNote = new List<TextNote>();
            foreach (TextNote text in allTextNote)
            {
                BoundingBoxXYZ box2 = text.get_BoundingBox(doc.ActiveView);
                minPoint = new XYZ(box2.Min.X, box2.Min.Y, 0);
                maxPoint = new XYZ(box2.Max.X, box2.Max.Y, 0);
                Outline outlineText = new Outline(minPoint, maxPoint);

                bool b = outlineElement.Intersects(outlineText, 0.0001);

                if (b) listTextNote.Add(text);
            }



            return listTextNote;
        }

    }
}