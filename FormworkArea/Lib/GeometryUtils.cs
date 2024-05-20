#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;

#endregion

namespace AlphaBIM
{
    public static class GeometryUtils
    {
        public static PlanarFace GetPlanarFaceBottom(Solid solid)
        {
            foreach (Face f in solid.Faces)
            {
                if (f.ComputeNormal(new UV()).IsAlmostEqualTo(XYZ.BasisZ.Negate()))
                {
                    return f as PlanarFace;
                }
            }

            return null;
        }

        // Hàm từ soild lấy ra được các face 
        public static List<Face> GetFaces(this Solid solid)
        {
            List<Face> faces = new List<Face>();

            FaceArray faceArray = solid.Faces;

            List<Face> planarFaces = new List<Face>();
            foreach (Face face in faceArray)
            {
                faces.Add(face);
            }

            return faces;
        }
        public static double CalAreaNotTop(this List<Face> faces)
        {

            double result = 0;
            faces = faces
                .Where(x => !x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ)).ToList();

            foreach (Face face in faces)
            {
                result += face.Area;
            }

            return result;
        }
        public static double CalAreaOnlyBottom(this List<Face> faces)
        {

            double result = 0;
            faces = faces
                .Where(x => x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ.Negate())).ToList();// Lấy ra các face có z hướng xuog

            foreach (Face face in faces)
            {
                result += face.Area;
            }

            return result;
        }

        // Tìm ra face top và bottom 
        public static double CalAreaNotTopNotBottom(this List<Face> faces)
        {

            double result = 0;
            faces = faces
                .Where(x => !x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ))// trên (theo trục Z) 
                .Where(x => !x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ.Negate()))// dưới (theo trục Z)
                .ToList();
            foreach (Face face in faces)// 4 mặt 
            {
                result += face.Area;
            }

            return result;
        }
        public static double CalAreaAll(this List<Face> faces)
        {

            double result = 0;
            foreach (Face face in faces)// 4 mặt 
            {
                result += face.Area;
            }
            return result;
        }
        public static XYZ GetNormal(this Face face)
        {
            BoundingBoxUV uv = face.GetBoundingBox();
            UV min = uv.Min;
            UV max = uv.Max;

            UV uvCenter = (min + max) / 2;

            XYZ computeNormal = face.ComputeNormal(uvCenter);
            return computeNormal;
        }

        public static Plane GetPlaneOfFace(this Face face)
        {
            List<XYZ> origin = face.Triangulate().Vertices.ToList();
            XYZ faceNormal = face.ComputeNormal((UV.Zero));
            return Plane.CreateByNormalAndOrigin(faceNormal, origin[0]);
        }

    }

}