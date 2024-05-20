#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Quadrant = System.Int32;
#endregion

namespace AlphaBIM
{
    public static class CurveUtils
    {
        /// <summary>
        /// Sort a list of curves to make them correctly 
        /// ordered and oriented to form a closed loop.
        /// source: https://thebuildingcoder.typepad.com/blog/2013/03/sort-and-orient-curves-to-form-a-contiguous-loop.html
        /// </summary>
        public static void SortCurvesContiguous(
          IList<Curve> curves)
        {
            int n = curves.Count;

            // Walk through each curve (after the first) 
            // to match up the curves in order

            for (int i = 0; i < n; ++i)
            {
                Curve curve = curves[i];
                XYZ endPoint = curve.GetEndPoint(1);
                XYZ p;

                // Find curve with start point = end point

                bool found = (i + 1 >= n);

                for (int j = i + 1; j < n; ++j)
                {
                    p = curves[j].GetEndPoint(0);

                    // If there is a match end->start, 
                    // this is the next curve

                    if (0.001 > p.DistanceTo(endPoint))
                    {
                        if (i + 1 != j)
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }

                    p = curves[j].GetEndPoint(1);

                    // If there is a match end->end, 
                    // reverse the next curve

                    if (0.001 > p.DistanceTo(endPoint))
                    {
                        if (i + 1 == j)
                        {
                            curves[i + 1] = CreateReversedCurve(curves[j]);
                        }
                        else
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = CreateReversedCurve(curves[j]);
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("SortCurvesContiguous:"
                      + " non-contiguous input curves");
                }
            }
        }


        public static void SortCurvesContiguous(
            CurveArray curveArray)
        {
            IList<Curve> curves = new List<Curve>();
            foreach (Curve cv in curveArray) curves.Add(cv);

            int n = curves.Count;

            // Walk through each curve (after the first) 
            // to match up the curves in order

            for (int i = 0; i < n; ++i)
            {
                Curve curve = curves[i];
                XYZ endPoint = curve.GetEndPoint(1);
                XYZ p;

                // Find curve with start point = end point

                bool found = (i + 1 >= n);

                for (int j = i + 1; j < n; ++j)
                {
                    p = curves[j].GetEndPoint(0);

                    // If there is a match end->start, 
                    // this is the next curve

                    if (0.001 > p.DistanceTo(endPoint))
                    {
                        if (i + 1 != j)
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }

                    p = curves[j].GetEndPoint(1);

                    // If there is a match end->end, 
                    // reverse the next curve

                    if (0.001 > p.DistanceTo(endPoint))
                    {
                        if (i + 1 == j)
                        {
                            curves[i + 1] = CreateReversedCurve(curves[j]);
                        }
                        else
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = CreateReversedCurve(curves[j]);
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("SortCurvesContiguous:"
                      + " non-contiguous input curves");
                }
            }
        }

        /// <summary>
        /// Create a new curve with the same 
        /// geometry in the reverse direction.
        /// </summary>
        /// <param name="orig">The original curve.</param>
        /// <returns>The reversed curve.</returns>
        /// <throws cref="NotImplementedException">If the 
        /// curve type is not supported by this utility.</throws>
        static Curve CreateReversedCurve(
            Curve orig)
        {
            //if (!IsSupported(orig))
            //{
            //    throw new NotImplementedException(
            //        "CreateReversedCurve for type "
            //        + orig.GetType().Name);
            //}

            if (orig is Line)
            {
                return Line.CreateBound(
                    orig.GetEndPoint(1),
                    orig.GetEndPoint(0));
            }
            else if (orig is Arc)
            {
                return Arc.Create(orig.GetEndPoint(1),
                    orig.GetEndPoint(0),
                    orig.Evaluate(0.5, true));
            }
            else
            {
                throw new Exception(
                    "CreateReversedCurve - Unreachable");
            }
        }
    }
}