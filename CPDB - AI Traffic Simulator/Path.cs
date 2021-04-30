using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator
{
    public class Path
    {
        /// <summary>
        ///  Positions of the point 1 and 2
        /// </summary>
        public Point P1 { get; private set; }
        public Point P2 { get; private set; }

        /// <summary>
        /// Parent PG
        /// </summary>
        public PathGroup ParentPathGroup { get; set; }

        /// <summary>
        /// Path Direction
        ///  - False = From P1 to P2
        ///  - True  = From P2 to P1
        /// </summary>
        public bool Direction { get; set; }

        /// <summary>
        /// Path angle based on P1 to P2 (in radians)
        /// </summary>
        public double Angle { get; private set; }

        /// <summary>
        /// The error margin used to calculate how near is the car by the path
        /// </summary>
        public double ErrorMargin = 2.00f;

        /// <summary>
        /// Street Speed
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The current path id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Path() { }
        public Path(Point p1, Point p2, bool dir)
        {
            SetPath(p1, p2, dir);
            Speed = 3.00f;
        }

        /// <summary>
        /// Calculates the path lenght
        /// </summary>
        /// <returns></returns>
        public float GetPathLenght()
        {
            return Calc.Magnitude(P1, P2);
        }

        /// <summary>
        /// Calculate the part of the path the car is running
        /// Returns -1 case is out of th epath
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public float PartOfPath(Car c)
        {
            bool isBetweenTheLine = Calc.DoesPointLineNormalizedIntersectionIsBetweenTheLine(c.Position, P1, P2);
            if (!isBetweenTheLine) return -1.00f;
            //
            PointF intersectionPoint = Calc.PointLineNormalizedIntersection(c.Position, P1, P2);
            float dist = Calc.Magnitude(c.Position, intersectionPoint);
            if (dist > ErrorMargin) return -1.00f;
            //
            return Calc.Magnitude(P1, c.Position);
        }

        public PointF GetNearestPathPoint(PointF pos)
        {
            bool isBetweenLineSegment = Calc.DoesPointLineNormalizedIntersectionIsBetweenTheLine(pos, P1, P2);
            if (isBetweenLineSegment)
            {
                return Calc.PointLineNormalizedIntersection(pos, P1, P2);
            }
            else
            {
                float distCP1 = Calc.Modulus(Calc.Magnitude(pos, P1));
                float distCP2 = Calc.Modulus(Calc.Magnitude(pos, P2));
                if (distCP1 < distCP2)
                {
                    return P1;
                }
                else
                {
                    return P2;
                }
            }
        }

        public PointF GetNearestCorner(PointF pos)
        {
            float distP1 = Calc.Modulus(Calc.Magnitude(pos, P1));
            float distP2 = Calc.Modulus(Calc.Magnitude(pos, P2));
            //
            if (distP1 < distP2)
            {
                return P1;
            }
            else
            {
                return P2;
            }
        }

        public bool IsInCorner(PointF pos)
        {
            if (Calc.Modulus(Calc.Magnitude(pos, P1)) <= ErrorMargin ||
                Calc.Modulus(Calc.Magnitude(pos, P2)) <= ErrorMargin)
            {
                return true;
            }
            return false;
        }

        public Car GetNearestCarInPath(Logic cl, PointF pos, int ignoreId)
        {
            Car cOut = null;
            float dist = 9999999999999.0f;
            //
            foreach (Car c in cl.Cars)
            {
                Path pTmp = c.GetBelongingPath();
                if (pTmp != null)
                {
                    if (pTmp.ID == ID)
                    {
                        if (Calc.Magnitude(pos, c.Position) < dist)
                        {
                            if (c.Id == ignoreId) continue;
                            cOut = c;
                            dist = Calc.Magnitude(pos, c.Position);
                        }
                    }
                }
            }
            //
            return cOut;
        }

        public Car GetNearestCarInPath(Logic cl, PointF pos)
        {
            Car cOut = null;
            float dist = 9999999999999.0f;
            //
            foreach (Car c in cl.Cars)
            {
                Path pTmp = c.GetBelongingPath();
                if (pTmp != null)
                {
                    if (pTmp.ID == ID)
                    {
                        if (Calc.Magnitude(pos, c.Position) < dist)
                        {
                            cOut = c;
                            dist = Calc.Magnitude(pos, c.Position);
                        }
                    }
                }
            }
            //
            return cOut;
        }

        /// <summary>
        /// 0 = None
        /// 1 = P1
        /// 2 = P2
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int IsInWhichCorner(PointF pos)
        {
            if (Calc.Modulus(Calc.Magnitude(pos, P1)) <= ErrorMargin)
            {
                return 1;
            }
            if (Calc.Modulus(Calc.Magnitude(pos, P2)) <= ErrorMargin)
            {
                return 2;
            }
            return 0;
        }

        /// <summary>
        /// Calculate the angle between the two path points
        /// </summary>
        /// <returns></returns>
        public double CalculateAngle()
        {
            Angle = Calc.GetAngleOfLineBetweenTwoPoints(P1, P2);
            return Angle;
        }

        /// <summary>
        /// Set the path values
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="dir"></param>
        public void SetPath(Point p1, Point p2, bool dir)
        {
            // Set the values
            P1 = p1;
            P2 = p2;
            Direction = dir;

            // Update Angle
            CalculateAngle();
        }

        /// <summary>
        /// Get the line from this line
        /// </summary>
        /// <returns></returns>
        public Line GetLine()
        {
            return new Line(P1, P2);
        }

        public PointF GetPathCornerBasedOnDirection()
        {
            if (Direction == false) return P2;
            return P1;
        }

        /// <summary>
        /// Calculate intersection with another path
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IntersectionData IntersectsWith(Path p)
        {
            // Get the path lines
            Line l1 = this.GetLine();
            Line l2 = p.GetLine();

            // Calc Intersections
            IntersectionData id = new IntersectionData();
            id.Intersects = Calc.DoLinesIntersect(l1, l2);
            id.Parallel = Calc.AreTwoLinesParallel(l1, l2);
            if (id.Intersects)
            {
                id.IntersectionPoint = Calc.IntersectionBetweenTwoLines(l1, l2);
                if (!Calc.DoesPointLineNormalizedIntersectionIsBetweenTheLine(id.IntersectionPoint, l1.P1, l1.P2) ||
                    !Calc.DoesPointLineNormalizedIntersectionIsBetweenTheLine(id.IntersectionPoint, l2.P1, l2.P2))
                {
                    /////////////////// TODO <-----------------------------------------------
                    id.Intersects = false;
                }
            }

            return id;
        }

    }
}
