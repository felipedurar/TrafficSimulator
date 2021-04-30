using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator
{
    /// <summary>
    /// Calc Class
    /// Coded by: Felipe Durar
    /// </summary>
    public class Calc
    {
        /// <summary>
        /// Calculate the angle between two lines
        /// Returns the angle in radians
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff);
        }

        /// <summary>
        /// Calculates if two lines are parallel
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static bool AreTwoLinesParallel(Line l1, Line l2)
        {
            // Line Points
            Point ps1 = l1.P1;
            Point pe1 = l1.P2;
            Point ps2 = l2.P1;
            Point pe2 = l2.P2;

            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.Y - ps1.Y;
            float B1 = ps1.X - pe1.X;
            float C1 = A1 * ps1.X + B1 * ps1.Y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.Y - ps2.Y;
            float B2 = ps2.X - pe2.X;
            float C2 = A2 * ps2.X + B2 * ps2.Y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculate the instersection point of two lines
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static PointF IntersectionBetweenTwoLines(Line l1, Line l2)
        {
            // Line Points
            Point ps1 = l1.P1;
            Point pe1 = l1.P2;
            Point ps2 = l2.P1;
            Point pe2 = l2.P2;

            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.Y - ps1.Y;
            float B1 = ps1.X - pe1.X;
            float C1 = A1 * ps1.X + B1 * ps1.Y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.Y - ps2.Y;
            float B2 = ps2.X - pe2.X;
            float C2 = A2 * ps2.X + B2 * ps2.Y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
            {
                // Case you want throw the exception (that's not what we want)
                //throw new System.Exception("Lines are parallel");
                return new PointF(0, 0);
            }

            // now return the Vector2 intersection point
            return new PointF((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
        }

        /// <summary>
        /// Instersection between two SEGMENTS of lines
        /// In infinite lines intersections exists only two intersection cases:
        ///  - Intersection
        ///  - Parallel Lines
        /// In segment of lines exists the NON INTERSECTION case
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool DoLinesIntersect(Line line1, Line line2)
        {
            return CrossProduct(line1.P1, line1.P2, line2.P1) !=
                   CrossProduct(line1.P1, line1.P2, line2.P2) ||
                   CrossProduct(line2.P1, line2.P2, line1.P1) !=
                   CrossProduct(line2.P1, line2.P2, line1.P2);
        }

        /// <summary>
        /// Calculate the cross product of 3 points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double CrossProduct(PointF p1, PointF p2, PointF p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
        }

        /// <summary>
        /// Calculates the distance between two lines
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float Magnitude(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        /// <summary>
        /// Calculates the intersection point of a point to a segment of line going toward the line in a right angle
        /// 
        ///               |---Intersection Point
        ///              \/ 
        /// -------------.-----------------   <-- Line
        ///              |  <- Right Angle
        ///              |
        ///              |  
        ///              |
        ///              .  <--- Point
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="LineStart"></param>
        /// <param name="LineEnd"></param>
        /// <returns></returns>
        public static PointF PointLineNormalizedIntersection(PointF point, PointF LineStart, PointF LineEnd)
        {
            float LineMag;
            float U;
            PointF Intersection = new PointF(0, 0);

            LineMag = Magnitude(LineEnd, LineStart);

            U = (((point.X - LineStart.X) * (LineEnd.X - LineStart.X)) +
                ((point.Y - LineStart.Y) * (LineEnd.Y - LineStart.Y))) /
                (LineMag * LineMag);

            if (U < 0.0f || U > 1.0f)
                return new PointF(0, 0);   // closest point does not fall within the line segment

            Intersection.X = (float)(LineStart.X + U * (LineEnd.X - LineStart.X));
            Intersection.Y = (float)(LineStart.Y + U * (LineEnd.Y - LineStart.Y));

            return new PointF(Intersection.X, Intersection.Y);
        }

        /// <summary>
        ///  Calculates the intersection point of a point to a segment of line going toward the line in a right angle and test if it's
        /// between the line segment
        /// </summary>
        /// <param name="point"></param>
        /// <param name="LineStart"></param>
        /// <param name="LineEnd"></param>
        /// <returns></returns>
        public static bool DoesPointLineNormalizedIntersectionIsBetweenTheLine(PointF point, PointF LineStart, PointF LineEnd)
        {
            float LineMag;
            float U;

            LineMag = Magnitude(LineEnd, LineStart);

            U = (((point.X - LineStart.X) * (LineEnd.X - LineStart.X)) +
                ((point.Y - LineStart.Y) * (LineEnd.Y - LineStart.Y))) /
                (LineMag * LineMag);

            if (U < 0.0f || U > 1.0f)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calculates the distance from a point toward nearest intersection point of a segment of line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="LineStart"></param>
        /// <param name="LineEnd"></param>
        /// <returns></returns>
        public static float DistancePointLine(PointF point, PointF LineStart, PointF LineEnd)
        {
            PointF intersectionPoint = PointLineNormalizedIntersection(point, LineStart, LineEnd);
            return Magnitude(point, intersectionPoint);
        }

        /// <summary>
        /// Returns a value between 0 and 100 that represents a part of a total
        /// </summary>
        /// <param name="total"></param>
        /// <param name="portion"></param>
        /// <returns></returns>
        public static float Percent(float total, float portion)
        {
            return (portion * 100) / total;
        }

        /// <summary>
        /// Returns the absoulute value
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float Modulus(float val)
        {
            if (val < 0.00f) return val * (-1);
            return val;
        }

        /// <summary>
        /// Converts a degree angle to radians angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegreeToRadians(float angle)
        {
            return Math.PI * angle / 180.0;
        }

        //public float DistanceBetweenRectangles(Rectangle r1, Rectangle r2)
        //{
        //    // Case they colide
        //    if (r1.IntersectsWith(r2))
        //    {
        //        return 0.00f;
        //    }

            

        //}

        public static bool IsInRange(double min, double max, double val)
        {
            if (val >= min && val <= max) return true;
            return false;
        }

        /// <summary>
        /// Normalize any number to determined range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double Normalise(double value, double start, double end) 
        {
            double width = end - start;
            double offsetValue = value - start;

            return (offsetValue - (Math.Floor(offsetValue / width) * width )) + start;
        }

        public static double NormalizeRadian(double ang)
        {
            //double angTmp = ang;
            //double circRad = Math.PI * 2;
            //while (angTmp > circRad)
            //{
            //    angTmp -= circRad;
            //}
            //while (ang < 0)
            //{
            //    angTmp += circRad;
            //}
            //return angTmp;
            return Normalise(ang, 0.00f, 2 * Math.PI);
        }

        public static PointF RotatePoint(PointF ceterPoint, float angle, PointF p)
        {
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            // translate point back to origin:
            PointF nPointF = new PointF(p.X, p.Y);
            nPointF.X -= ceterPoint.X;
            nPointF.Y -= ceterPoint.Y;

            // rotate point
            double xnew = nPointF.X * c - nPointF.Y * s;
            double ynew = nPointF.X * s + nPointF.Y * c;

            // translate point back:
            nPointF.X = (float)xnew + ceterPoint.X;
            nPointF.Y = (float)ynew + ceterPoint.Y;
            return nPointF;
        }

    }

    /// <summary>
    /// Line Class
    /// Represents a rectangle formed by 4 points
    /// 
    ///  P1                       P2
    ///   ------------------------
    ///   |                      |
    ///   |                      |
    ///   |                      |
    ///   |                      |
    ///   |                      |
    ///   ------------------------
    ///  P4                       P3
    /// 
    /// Coded by: Felipe Durar
    /// </summary>
    public class Rectangle4P
    {
        public PointF P1 { get; set; }
        public PointF P2 { get; set; }
        public PointF P3 { get; set; }
        public PointF P4 { get; set; }

        public Rectangle4P() { }
        public Rectangle4P(RectangleF rect)
        {
            SetRectangle(rect);
        }

        public void SetRectangle(RectangleF rect)
        {
            P1 = rect.Location;
            P2 = new PointF(rect.X + rect.Width, rect.Y);
            P3 = new PointF(rect.X + rect.Width, rect.Y + rect.Height);
            P4 = new PointF(rect.X, rect.Y + rect.Height);
        }

        public PointF GetPointByNumber(int ind)
        {
            if (ind == 1) return P1;
            if (ind == 2) return P2;
            if (ind == 3) return P3;
            if (ind == 4) return P4;
            return new PointF(0, 0);
        }

        public PointF GetNearestPointBetweenTwoRectangles(Rectangle4P r4p)
        {
            float dist = Calc.Modulus(Calc.Magnitude(P1, r4p.P1));
            PointF np = P1;
            //
            for (int C = 1; C <= 4; C++)
            {
                PointF CP = GetPointByNumber(C);
                //
                for (int D = 1; D <= 4; D++)
                {
                    float cDist = Calc.Modulus(Calc.Magnitude(CP, r4p.GetPointByNumber(D)));
                    if (cDist < dist)
                    {
                        dist = cDist;
                        np = CP;
                    }
                }
            }
            //
            return np;
        }

        public PointF GetNearestPoint(PointF p)
        {
            float dist = Calc.Modulus(Calc.Magnitude(P1, p));
            PointF np = P1;
            //
            for (int C = 1; C <= 4; C++)
            {
                PointF CP = GetPointByNumber(C);
                //
                float cDist = Calc.Modulus(Calc.Magnitude(CP, p));
                if (cDist < dist)
                {
                    dist = cDist;
                    np = CP;
                }
            }
            //
            return np;
        }

        public PointF GetNearestPointInEdge(PointF p)
        {
            float dist = Calc.Modulus(Calc.Magnitude(P1, p));
            PointF np = P1;
            //
            for (int C = 1; C <= 4; C++)
            {
                //Point CP = GetPointByNumber(C);
                ////
                //float cDist = Calc.Modulus(Calc.Magnitude(CP, p));
                //if (cDist < dist)
                //{
                //    dist = cDist;
                //    np = CP;
                //}
                Line ln = new Line();
                ln.P1 = Point.Round(GetPointByNumber(C));
                ln.P2 = Point.Round(GetPointByNumber(C + 1 == 5 ? 1 : C + 1));
                //
                PointF intersectionPos = Calc.PointLineNormalizedIntersection(p, ln.P1, ln.P2);
                float cDist = Calc.Modulus(Calc.DistancePointLine(p, ln.P1, ln.P2));
                if (cDist < dist)
                {
                    dist = cDist;
                    np = intersectionPos;
                }
            }
            //
            return np;
        }

    }

    /// <summary>
    /// Line Class
    /// Represents a infinite line formed by two points
    /// Coded by: Felipe Durar
    /// </summary>
    public class Line
    {
        /// <summary>
        /// Line Points
        /// </summary>
        public Point P1 { get; set; }
        public Point P2 { get; set; }

        /// <summary>
        /// Constructors
        /// </summary>
        public Line() { }
        public Line(Point p1, Point p2)
        {
            P1 = new Point(p1.X, p1.Y);
            P2 = new Point(p2.X, p2.Y);
        }

    }
}
