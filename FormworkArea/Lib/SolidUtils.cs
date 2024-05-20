#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.DB;

#endregion

namespace AlphaBIM
{
    public static class SolidUtils
    {
        /// <summary>
        /// Create and return a solid representing 
        /// the bounding box.
        /// Link tham khảo: https://github.com/jeremytammik/the_building_coder_samples/blob/master/BuildingCoder/BuildingCoder/Util.cs
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Solid CreateSolidFromBoundingBox(
            BoundingBoxXYZ bbox)
        {
            // Tọa độ 4 điểm ở đáy của BoundingBoxXYZ

            XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

            // Edges in BBox coords

            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);

            // Create loop, still in BBox coords

            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);

            double height = bbox.Max.Z - bbox.Min.Z;

            CurveLoop baseLoop = CurveLoop.Create(edges);

            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);

            Solid preTransformBox = GeometryCreationUtilities
                .CreateExtrusionGeometry(loopList, XYZ.BasisZ,
                    height);

            return preTransformBox;
        }
        
        
        //Hàm này sẽ lấy ra được các Solid 
        public static Solid GetSolid(this Element el)
        {
            GeometryElement geoElement = el.get_Geometry(new Options());

            foreach (GeometryObject geoObject in geoElement)
            {
                if (geoObject is Solid)
                {
                    Solid solid = geoObject as Solid;
                    if (solid.Volume > 0)
                        return solid;
                }
                else if (geoObject is GeometryInstance)
                {
                    // GeometryInstance có thể bao gồm  Arc, Line, PolyLine, Point, Solid và GeometryInstance khác được lồng vào
                    GeometryInstance geoInstance = geoObject as GeometryInstance;
                    GeometryElement geoElement2 = geoInstance.GetInstanceGeometry();
                    foreach (GeometryObject geoObject2 in geoElement2)
                    {
                        if (geoObject2 is Solid)
                        {
                            Solid solid = geoObject2 as Solid;
                            if (solid.Volume > 0)
                                return solid;
                        }
                    }
                }

            }

            return null;
        }


    }
}