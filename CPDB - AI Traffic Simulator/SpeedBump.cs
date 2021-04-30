using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator
{
    /// <summary>
    /// Speed Bump Class (Classe Quebra-Molas)
    /// Coded by: Felipe Durar
    /// </summary>
    public class SpeedBump
    {
        public PointF Position { get; set; }
        public SizeF Size { get; set; }
        public float Angle { get; set; } // In Degree
        public float ErrorMargin { get; set; } // In Degree
        public float DesiredSpeed { get; set; }
        public PointF P1 { get; private set; }
        public PointF P2 { get; private set; }

        public SpeedBump()
        {
            DesiredSpeed = 0.50f;
            Size = new SizeF(100, 30);
            ErrorMargin = 20.0f;
            Angle = 90.0f;
        }

        public RectangleF GetRectangle()
        {
            return new RectangleF(Position.X - (Size.Width / 2), Position.Y - (Size.Height / 2),
                Size.Width, Size.Height);
        }

        public bool IsAlligned(Car c)
        {
            RectangleF rect = GetRectangle();
            Rectangle4P r4p = new Rectangle4P(rect);
            PointF nearestPoint = r4p.GetNearestPointInEdge(c.Position);
            //
            double radErrMargin = Calc.DegreeToRadians(ErrorMargin);
            double angSB_C = Calc.NormalizeRadian(Calc.GetAngleOfLineBetweenTwoPoints(nearestPoint, c.Position));
            double tAngle = Calc.NormalizeRadian(Calc.DegreeToRadians(Angle));
            double iAngle = Calc.NormalizeRadian(Calc.DegreeToRadians(Angle) - Math.PI);
            //
            if (Calc.IsInRange(tAngle - (radErrMargin / 2), tAngle + (radErrMargin / 2), angSB_C) ||
                Calc.IsInRange(iAngle - (radErrMargin / 2), iAngle + (radErrMargin / 2), angSB_C))
            {
                return true;
            }
            return false;
        }

        public bool IsTowardSpeedBump(Car c)
        {
            RectangleF rect = GetRectangle();
            Rectangle4P r4p = new Rectangle4P(rect);
            PointF nearestPoint = r4p.GetNearestPointInEdge(c.Position);
            //
            double radErrMargin = Calc.DegreeToRadians(ErrorMargin);
            double angC_SB = Calc.NormalizeRadian(Calc.GetAngleOfLineBetweenTwoPoints(c.Position, nearestPoint));
            //
            if (Calc.IsInRange(angC_SB - (radErrMargin / 2), angC_SB + (radErrMargin / 2), Calc.NormalizeRadian(c.Angle)))
            {
                return true;
            }
            return false;
        }

        public float DistanceOfCar(Car c)
        {
            RectangleF rect = GetRectangle();
            Rectangle4P r4p = new Rectangle4P(rect);
            PointF nearestPoint = r4p.GetNearestPointInEdge(c.Position);
            //
            return Calc.Magnitude(c.Position, nearestPoint);
        }
    }
}
