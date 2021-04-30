using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator
{
    public class TrafficLight
    {
        public PointF Position { get; set; }
        public SizeF Size { get; set; }
        public float Angle { get; set; }
        public float ErrorMargin { get; set; } // In Degree
        public int StateId { get; set; }
        public TrafficLightStatus Status { get; set; }

        public TrafficLight()
        {
            Init();
        }

        public TrafficLight(TrafficLight inheritStateId)
        {
            Init();
            StateId = inheritStateId.StateId;
        }

        public TrafficLight(int stateId)
        {
            Init();
            StateId = stateId;
        }

        private void Init()
        {
            Size = new SizeF(100, 10);
            ErrorMargin = 20.0f;
            Angle = 90.0f;
            Status = TrafficLightStatus.Red;
        }

        public RectangleF GetRectangle()
        {
            return new RectangleF(Position.X - (Size.Width / 2), Position.Y - (Size.Height / 2),
                Size.Width, Size.Height);
        }
        
        public Color GetColor()
        {
            if (Status == TrafficLightStatus.Green) return Color.Green;
            if (Status == TrafficLightStatus.Red) return Color.Red;
            if (Status == TrafficLightStatus.Yellow) return Color.Yellow;
            return Color.Cyan;
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

        public bool IsTowardTrafficLight(Car c)
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

    public class TrafficLightGroup
    {
        public List<TrafficLight> TLs { get; set; }
        public int CurrentIdIndex { get; set; }
        public int Interval { get; set; }
        public int YellowRemainingInterval { get; set; }
        public List<int> Ids { get; set; }
        public int GroupId { get; set; }
        private int _counter = 0;
        private int _idCounter = 0;

        public TrafficLightGroup()
        {
            TLs = new List<TrafficLight>();
            Ids = new List<int>();
            Interval = 5000;
            YellowRemainingInterval = 500;
            CurrentIdIndex = 0;
            GroupId = 0;
        }

        public void AddTrafficLight(TrafficLight tl)
        {
            if (!IdExists(tl.StateId))
            {
                Ids.Add(tl.StateId);
            }
            //
            TLs.Add(tl);
        }

        private bool IdExists(int id)
        {
            foreach (int cid in Ids)
            {
                if (cid == id) return true;
            }
            return false;
        }

        private void SetIdIndexStatus(int ind, TrafficLightStatus s)
        {
            if (ind < 0 || ind >= Ids.Count) return;
            int id = Ids[ind];
            foreach (TrafficLight tl in TLs)
            {
                if (tl.StateId == id)
                {
                    tl.Status = s;
                }
                else
                {
                    tl.Status = TrafficLightStatus.Red;
                }
            }
        }

        public void SetIdIndexToGreen(int ind)
        {
            SetIdIndexStatus(ind, TrafficLightStatus.Green);
        }

        public void SetIdIndexToYellow(int ind)
        {
            SetIdIndexStatus(ind, TrafficLightStatus.Yellow);
        }

        public void DoStep()
        {
            // Don't change the counter case there is no traffic light
            if (Ids.Count == 0) return;
            //
            _counter++;
            if (_counter >= Interval)
            {
                _counter = 0;
                // Switch Lights
                CurrentIdIndex++;
                if (CurrentIdIndex >= Ids.Count)
                {
                    CurrentIdIndex = 0;
                }
                //
                SetIdIndexToGreen(CurrentIdIndex);
            }
            else if (_counter == Interval - YellowRemainingInterval) // It Only Ocurres on one step!
            {
                // Change to Yellow
                SetIdIndexToYellow(CurrentIdIndex);
            }
        }

        public int GetNewId()
        {
            _idCounter++;
            return _idCounter - 1;
        }

    }

    public enum TrafficLightStatus
    {
        None, Red, Yellow, Green
    }
}
