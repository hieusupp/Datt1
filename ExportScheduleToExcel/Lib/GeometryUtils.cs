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

     
    }

}