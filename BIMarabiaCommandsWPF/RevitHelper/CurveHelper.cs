using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace BIMarabiaCommandsWPF.RevitHelper
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    /// <summary>
    /// Curve loop helper supporting resorting and 
    /// orientation of curves to form a contiguous 
    /// closed loop.
    /// </summary>
    public static class CurveHelper
    {
        // Constants to be used.
        const double Inch = 1.0 / 12.0;         // Inch = feet / 12.
        const double Tolerance = Inch / 16.0;   // Tolerance to be used for checking equality.

        /// <summary>
        /// Predicate to report whether the given curve 
        /// type is supported by this utility class.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <returns>True if the curve type is supported, 
        /// false otherwise.</returns>
        static bool IsSupported(Curve curve)
        {
            // return if the curve is line or arc.
            return curve is Line || curve is Arc;
        }

        /// <summary>
        /// Create a new curve with the same 
        /// geometry in the reverse direction.
        /// </summary>
        /// <param name="original">The original curve.</param>
        /// <returns>The reversed curve.</returns>
        /// <throws cref="NotImplementedException">If the 
        /// curve type is not supported by this utility.</throws>
        static Curve CreateReversedCurve(Curve original)
        {
            // Check if the curve is supported.
            if (!IsSupported(original))
            {
                throw new NotImplementedException(
                  "CreateReversedCurve for type "
                  + original.GetType().Name);
            }

            // The curve is a line => Create a reversed line.
            if (original is Line)
            {
                return Line.CreateBound(
                  original.GetEndPoint(1),
                  original.GetEndPoint(0));
            }
            // The curve is arc => Create a reverses arc.
            else if (original is Arc)
            {
                return Arc.Create(original.GetEndPoint(1),
                  original.GetEndPoint(0),
                  original.Evaluate(0.5, true));
            }
            // Something else! => There is an error!
            else
            {
                throw new Exception(
                  "CreateReversedCurve - Unreachable");
            }
        }

        /// <summary>
        /// Swap two list items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index1">The index of the first item.</param>
        /// <param name="index2">The index of the second item.</param>
        static void Swap<T>(IList<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// Swap two curves with reversing the second curve direction.
        /// </summary>
        /// <param name="list">The curves list.</param>
        /// <param name="index1">The index of the first curve.</param>
        /// <param name="index2">The index of the second curve.</param>
        /// <throws cref="NotImplementedException">If the 
        /// curve type is not supported by this utility.</throws>
        static void SwapAndReverse(IList<Curve> list, int index1, int index2)
        {
            Curve temp = list[index1];
            list[index1] = CreateReversedCurve(list[index2]);
            list[index2] = temp;
        }

        /// <summary>
        /// Sort a list of curves and a list of offsets to make them
        /// correctly ordered and oriented to form a closed loop.
        /// </summary>
        static void SortCurvesWithOffsetsContiguous(IList<Curve> curves, IList<double> offsets)
        {
            // The number of curves.
            int n = curves.Count;

            // Walk through curves to match up the curves
            // in correct ordering and directions.

            for (int i = 0; i < n - 1; ++i)
            {
                // Get the current curve.
                Curve curve = curves[i];

                // Get the end point of the current curve.
                XYZ endPoint = curve.GetEndPoint(1);

                // The point to used to find the curve.
                XYZ p;

                // Initialize the "found" flag.
                bool found = false;

                // Initialize the counter.
                int j = i + 1;

                // Find curve with start point = end point of the current curve.

                while (!found && j < n)
                {
                    // Get the start point of the next curve.
                    p = curves[j].GetEndPoint(0);

                    // If there is a match end->start, 
                    // this is the next curve,
                    // do nothing,
                    // if not the next curve,
                    // swap it to be the next curve.

                    if (p.DistanceTo(endPoint) <= Tolerance)
                    {
                        if (i + 1 != j) // Ordering issue => swap.
                        {
                            // Swap curves.
                            Swap(curves, i + 1, j);

                            // Swap offsets.
                            Swap(offsets, i + 1, j);
                        }

                        found = true;   // issue solved, curve is found.
                    }

                    if (!found) // issue not solved yet, curve is not found => Check the end point.
                    {
                        // Get the end point of the next curve.
                        p = curves[j].GetEndPoint(1);

                        // If there is a match end->end,
                        // this is the next curve but reversed,
                        // reverse the next curve,
                        // if not the next curve,
                        // swap and reverse.

                        if (p.DistanceTo(endPoint) <= Tolerance)
                        {
                            if (i + 1 == j) // Direction issue => reverse.
                            {
                                curves[i + 1] = CreateReversedCurve(curves[j]);
                            }
                            else // Ordering and Direction issue => swap & reverse.
                            {
                                // Swap & reverse curves.
                                SwapAndReverse(curves, i + 1, j);

                                // Swap offsets.
                                Swap(offsets, i + 1, j);
                            }

                            found = true;   // issue solved, curve is found.
                        }
                    }

                    // Increase the counter => go to next curve.
                    j++;
                }

                if (!found) // curve not found yet! there is an error!
                {
                    throw new Exception("SortCurvesContiguous:"
                      + " non-contiguous input curves");
                }
            }
        }

        /// <summary>
        /// Return a list of curves and a list of offsets which
        /// are correctly ordered and oriented to form a closed loop.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="originalCurves">The list of curves which are the boundaries.</param>
        /// <param name="originalOffsets">The list of double values which are the offsets.</param>
        /// <returns>Tuple containing the contiguous list of curves and the contiguous list of offsets.</returns>
        public static (List<Curve> curves, List<double> offsets) GetContiguousCurvesWithOffsets(IList<Curve> originalCurves, IList<double> originalOffsets)
        {
            // Build a list of curves from the original curves list.
            List<Curve> newCurves = new List<Curve>(originalCurves);

            // Build a list of offsets from the original offsets list.
            List<double> newOffsets = new List<double>(originalOffsets);

            // Sort and make contiguous for the new curves list.
            SortCurvesWithOffsetsContiguous(newCurves, newOffsets);

            // Create the list with offsets tuple.
            var curvesWithOffsetsTuple = (newCurves, newOffsets);

            // Return the tuple.
            return curvesWithOffsetsTuple;
        }
    }
}