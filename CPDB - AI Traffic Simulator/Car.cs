using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator
{
    public class Car
    {
        public int Id { get; set; }
        public Logic CLogic { get; set; }
        public PointF Position = new PointF(0, 0); //{ get; set; }
        public Size Size { get; set; }
        public Color Color { get; set; }
        public Stack<Task> ImmediateTasks { get; set; }
        public Queue<Task> TasksQueue { get; set; }
        public OutputData OutputData { get; set; }
        public float ErrorMargin = 2.00f;
        public float FOV { get; set; } // In Degree
        public double Angle { get; protected set; }
        public bool Working { get; set; }
        protected PointF LastPosition = new PointF(0, 0);

        public float ObjectiveSpeed { get; set; }
        public float Speed { get; set; }
        public float MaximumSpeed { get; set; }
        public float DefaultAccelerationForce { get; set; }
        public float SecureDistance { get; protected set; }
        public float SDRatio { get; set; } // Secure Distance Ratio
        public float MSecureDistance { get; set; } // Secure Distance Ratio
        public float EmergencyStopDistance { get; set; }

        public bool KeepInPath { get; set; }
        public bool Nomadic { get; set; }

        public bool UserControl { get; set; }
        public UserController Control { get; set; }

        public Car()
        {
            CLogic = null;
            Id = 0;
            Size = new Size(25, 25);
            Color = Color.Green;
            TasksQueue = new Queue<Task>();
            OutputData = new OutputData();
            ImmediateTasks = new Stack<Task>();
            Speed = 0.00f;
            ObjectiveSpeed = 0.00f;
            MaximumSpeed = 3.00f;
            DefaultAccelerationForce = 0.02f;
            SecureDistance = 100.00f;
            SDRatio = 30.0f;
            MSecureDistance = 60.0f;
            EmergencyStopDistance = 50.0f;
            KeepInPath = false;
            Nomadic = true;
            Angle = 0.00f;
            FOV = 90.00f;
            Working = true;
            UserControl = false;
            Control = new UserController();
        }

        public RectangleF GetRectangle()
        {
            return new RectangleF(Position, Size);
        }

        public Path GetBelongingPath()
        {
            //////////////////// TODO <------------- THE CAR NEED TO BE ALIGNED WITH THE STREET (SAME ANGLES)
            foreach (PathGroup pg in CLogic.PathGroups)
            {
                foreach (Path p in pg.Paths)
                {
                    if (Calc.Modulus(Calc.Magnitude(p.GetNearestPathPoint(Position), Position)) <= ErrorMargin)
                    {
                        return p;
                    }
                }
            }
            return null;
        }

        public void SpeedUp(float force)
        {
            Speed += force;
            if (Speed > MaximumSpeed) Speed = MaximumSpeed;
        }

        public void SpeedUp()
        {
            SpeedUp(DefaultAccelerationForce);
        }

        public void Break()
        {
            Speed -= 0.04f;
            if (Speed < 0.00f) Speed = 0.00f;
        }

        public Task CreateTask(PointF pos, TaskType taskType)
        {
            Task t = new Task();
            t.Objective = pos;
            t.InitialPosition = Position;
            t.Type = taskType;
            return t;
        }

        public void EnqueueTask(Task t)
        {
            TasksQueue.Enqueue(t);
        }

        public void PutInPath(Path p)
        {
            PointF np = p.GetNearestPathPoint(Position);
            EnqueueTask(CreateTask(np, TaskType.Queue));
        }

        public void PushOutput(string outStr)
        {
            OutputData.PushNewData("Car" + Id.ToString() + ": " + outStr);
        }

        public void SetObjectiveSpeed(float speed)
        {
            if (speed != ObjectiveSpeed)
            {
                PushOutput("Speed was set to " + speed.ToString("0.0000") + " at " + Logic.PointToString(Position));
            }
            //
            ObjectiveSpeed = speed;
        }

        public PointF GetEndOfPathPointToward()
        {
            Path np = GetBelongingPath();
            if (np == null) return new PointF(Position.X, Position.Y);
            //
            double streetAngle = np.CalculateAngle();
            double invStreetAngle = Calc.NormalizeRadian(streetAngle + Math.PI);
            //
            double diffStraight = Math.Abs(Angle - streetAngle);
            double diffInvStraight = Math.Abs(Angle - invStreetAngle);
            //
            if (diffStraight < diffInvStraight)
            {
                return np.P2;
            }
            else
            {
                return np.P1;
            }
        }

        public PointF DoTask(Task t)
        {
            PointF IPos = new PointF(0, 0);
            float dist = Calc.Magnitude(Position, t.Objective);
            //
            if (t.Unread == true)
            {
                t.Unread = false;
                //
                PushOutput("Started going from " + Logic.PointToString(Position) + " to " + Logic.PointToString(t.Objective));
            }
            //
            if (dist <= ErrorMargin)
            {
                if (t.Type == TaskType.Immediate)
                {
                    if (ImmediateTasks.Count > 0)
                    {
                        ImmediateTasks.Pop();
                        PushOutput(" The car reached the objetive " + Logic.PointToString(t.Objective));
                    }
                    else
                    {
                        // Error
                    }
                }
                else if (t.Type == TaskType.Queue)
                {
                    TasksQueue.Dequeue();
                }
            }
            else
            {
                double ang = Calc.GetAngleOfLineBetweenTwoPoints(Position, t.Objective);
                //
                IPos.X += (float)(Math.Cos(ang) * Speed);
                IPos.Y += (float)(Math.Sin(ang) * Speed);
                //
                if (dist > SecureDistance)
                {
                    //SpeedUp();
                    SetObjectiveSpeed(MaximumSpeed);
                }
                else
                {
                    SetObjectiveSpeed(0.5f);
                }
            }
            //
            return IPos;
        }

        public void Kill()
        {
            Color = Color.Red;
            Working = false;
        }

        public void Revive()
        {
            Color = Color.Blue;
            Working = true;
        }

        public bool GetYN()
        {
            Random r = new Random();
            int v = r.Next(0, 2);
            return v == 1;
        }

        public PointF DoAI()
        {
            // IPos
            PointF IPos = new PointF(0.00f, 0.00f);

            // Immediate Tasks
            bool hasImmediate = ImmediateTasks.Count != 0;
            if (hasImmediate)
            {
                Task t = ImmediateTasks.Peek();
                PointF diff = DoTask(t);
                IPos.X += diff.X;
                IPos.Y += diff.Y;
            }


            // Solve Tasks Queue
            if (TasksQueue.Count > 0 && !hasImmediate)
            {
                Task t = TasksQueue.Peek();
                PointF diff = DoTask(t);
                IPos.X += diff.X;
                IPos.Y += diff.Y;
            }

            // Nomadic Mode
            if (Nomadic)
            {
                Path cp = GetBelongingPath();
                if (cp != null)
                {
                    if (TasksQueue.Count == 0 && !hasImmediate)
                    {
                        PointF endOfPath = cp.GetPathCornerBasedOnDirection();
                        float dist = Calc.Modulus(Calc.Magnitude(endOfPath, Position));
                        if (dist < cp.ErrorMargin)
                        {
                            Path ip = cp.ParentPathGroup.GetWayWithDir(!cp.Direction);
                            PointF nearestCorner = ip.GetNearestCorner(Position);
                            Task t = CreateTask(nearestCorner, TaskType.Queue);
                            EnqueueTask(t);
                        }
                        else
                        {
                            //Task t = CreateTask(endOfPath, TaskType.Immediate);
                            //ImmediateTasks.Push(t);
                            //
                            Task t = CreateTask(endOfPath, TaskType.Queue);
                            EnqueueTask(t);
                        }
                    }
                }
            }

            // Path Info
            Path cCarPath = GetBelongingPath();
            int cCarPId = cCarPath != null ? cCarPath.ID : -1;

            // Speed Bump
            foreach (SpeedBump sb in CLogic.SpeedBumps)
            {
                if (sb.IsAlligned(this))
                {
                    if (sb.IsTowardSpeedBump(this))
                    {
                        float dist = sb.DistanceOfCar(this);
                        //
                        if (dist < SecureDistance)
                        {
                            SetObjectiveSpeed(sb.DesiredSpeed);
                        }
                    }
                }
            }

            ////////////////////////////////////////// IT'S NOT WORKING CORRECTLY! //////////////////////////////////////////////////////
            // Change Way (if there's another free way)
            if (cCarPath != null)
            {
                Car cNearest = cCarPath.GetNearestCarInPath(CLogic, Position, Id);
                if (cNearest != null)
                {
                    float dist = Calc.Modulus(Calc.Magnitude(Position, cNearest.Position));
                    if (dist < SecureDistance) // <--------------------------------------- TODO
                    {
                        List<Path> availablePaths = cCarPath.ParentPathGroup.GetWaysWithDir(cCarPath.Direction);
                        foreach (Path p in availablePaths)
                        {
                            if (p.ID == cCarPath.ID) continue;
                            //
                            PointF nLinePos = Calc.PointLineNormalizedIntersection(Position, p.P1, p.P2);
                            Car nearestAnotherPath = p.GetNearestCarInPath(CLogic, nLinePos);
                            if (nearestAnotherPath == null)
                            {
                                //
                                PointF futurePoint = new PointF(nLinePos.X + (float)((Math.Cos(Angle) * Speed)),
                                    nLinePos.Y + (float)((Math.Sin(Angle) * SecureDistance)));

                                // Go TO Another Path
                                Task task = CreateTask(futurePoint, TaskType.Immediate);
                                ImmediateTasks.Push(task);
                                TasksQueue.Clear();
                                break;
                            }
                            //
                            float distCarAnotherPath = Calc.Modulus(Calc.Magnitude(Position, nearestAnotherPath.Position));
                            if (distCarAnotherPath > SecureDistance)
                            {
                                //
                                PointF futurePoint = new PointF(nLinePos.X + (float)((Math.Cos(Angle) * Speed)),
                                    nLinePos.Y + (float)((Math.Sin(Angle) * SecureDistance)));
                                // Go TO Another Path
                                Task task = CreateTask(futurePoint, TaskType.Immediate);
                                ImmediateTasks.Push(task);
                                TasksQueue.Clear();
                                break;
                            }
                        }
                    }
                }
            }

            // Car Operations
            for (int C = 0; C < CLogic.Cars.Count; C++)
            {
                if (CLogic.Cars[C].Id != Id)
                {
                    // Current Car
                    Car cCar = CLogic.Cars[C];

                    // Error margin
                    const float errAngle = 30.0f;
                    double errAngleRad = Calc.NormalizeRadian(Calc.DegreeToRadians(errAngle));

                    // Vars
                    double angAc = Calc.NormalizeRadian(cCar.Angle);
                    double angAcInv = Calc.NormalizeRadian(cCar.Angle + Math.PI);
                    double angTc = Calc.NormalizeRadian(Angle);
                    double angAcTc = Calc.NormalizeRadian(Calc.GetAngleOfLineBetweenTwoPoints(cCar.Position, Position));
                    double angTcAc = Calc.NormalizeRadian(Calc.GetAngleOfLineBetweenTwoPoints(Position, cCar.Position));
                    float distBoth = Calc.Modulus(Calc.Magnitude(Position, cCar.Position));

                    // Test if the other car is going toward this car
                    if (Calc.IsInRange(angAcTc - (errAngleRad / 2.00d), angAcTc + (errAngleRad / 2.00d), angAc))
                    {
                        // Test if the other car is going in the same direction
                        if (Calc.IsInRange(angTc - (errAngleRad / 2.00d), angTc + (errAngleRad / 2.00d), angAcInv))
                        {
                            if (distBoth < SecureDistance)
                            {
                                //Break();
                                SetObjectiveSpeed(0.00f);
                            }
                        }
                    }

                    // Test if this car is going toward another car (few FOV to avoid the car speed down when the car is comming in the oposite way)
                    //if (Calc.IsInRange(angTcAc - (errAngleRad / 2.00d), angTcAc + (errAngleRad / 2.00d), angTc))
                    //{
                    //    if (distBoth < SecureDistance)
                    //    {
                    //        //Break();
                    //        SetObjectiveSpeed(0.00f);
                    //    }
                    //}

                    if (Calc.IsInRange(angTcAc - (Calc.DegreeToRadians(FOV) / 2.00d), angTcAc + (Calc.DegreeToRadians(FOV) / 2.00d), angTc))
                    {
                        Path bpcCar = cCar.GetBelongingPath();
                        if (bpcCar != null && GetBelongingPath() != null)
                        {
                            if (distBoth < SecureDistance && bpcCar.ID == GetBelongingPath().ID)
                            {
                                //Break();
                                SetObjectiveSpeed(0.00f);
                            }
                        }
                        else
                        {
                            if (distBoth < SecureDistance)
                            {
                                //Break();
                                SetObjectiveSpeed(0.00f);
                            }
                        }
                    }

                }
            }

            // Traffic Lights
            foreach (TrafficLightGroup tfg in CLogic.TrafficLights)
            {
                foreach (TrafficLight tl in tfg.TLs)
                {
                    if (tl.IsAlligned(this))
                    {
                        if (tl.IsTowardTrafficLight(this))
                        {
                            if (tl.Status == TrafficLightStatus.Red)
                            {
                                float dist = tl.DistanceOfCar(this);
                                //
                                if (dist < SecureDistance)
                                {
                                    SetObjectiveSpeed(0.00f);
                                }
                            }
                        }
                    }
                }
            }

            return IPos;
        }

        public virtual void Update()
        {
            // Decrease Car Speed
            Speed -= 0.01f;
            if (Speed < 0.00f)
            {
                Speed = 0.00f;
            }

            // Secure Speed Distance
            SecureDistance = (Speed * SDRatio) + MSecureDistance;

            // Position Changes
            PointF IPos = new PointF(0.00f, 0.00f);
            ObjectiveSpeed = MaximumSpeed;

            // AI
            if (!UserControl)
            {
                IPos = DoAI();
            }

            // User Control
            double UpAngleRad = Calc.DegreeToRadians(270);
            const double _90degreeRad = (Math.PI / 2);
            if (UserControl)
            {
                if (Control.IsSomeButtonPressed())
                {
                    SetObjectiveSpeed(MaximumSpeed);
                }
                else
                {
                    SetObjectiveSpeed(0.00f);
                }
                //
                if (Control.Up)
                {
                    double angNormalized = Calc.NormalizeRadian(UpAngleRad);
                    IPos.X += (float)(Math.Cos(angNormalized) * Speed);
                    IPos.Y += (float)(Math.Sin(angNormalized) * Speed);
                }
                if (Control.Right)
                {
                    double angNormalized = Calc.NormalizeRadian(UpAngleRad + _90degreeRad);
                    IPos.X += (float)(Math.Cos(angNormalized) * Speed);
                    IPos.Y += (float)(Math.Sin(angNormalized) * Speed);
                }
                if (Control.Down)
                {
                    double angNormalized = Calc.NormalizeRadian(UpAngleRad + (2 * _90degreeRad));
                    IPos.X += (float)(Math.Cos(angNormalized) * Speed);
                    IPos.Y += (float)(Math.Sin(angNormalized) * Speed);
                }
                if (Control.Left)
                {
                    double angNormalized = Calc.NormalizeRadian(UpAngleRad + (3 * _90degreeRad));
                    IPos.X += (float)(Math.Cos(angNormalized) * Speed);
                    IPos.Y += (float)(Math.Sin(angNormalized) * Speed);
                }
            }

            // Car Operations
            for (int C = 0; C < CLogic.Cars.Count; C++)
            {
                if (CLogic.Cars[C].Id != Id)
                {
                    // Current Car
                    Car cCar = CLogic.Cars[C];

                    // Test Colisions
                    if (GetRectangle().IntersectsWith(cCar.GetRectangle()))
                    {
                        Kill();
                    }
                }
            }

            // Speed Balance
            if (Speed < ObjectiveSpeed)
            {
                SpeedUp();
            }
            else if (Speed > ObjectiveSpeed)
            {
                Break();
            }

            // Work?
            if (!Working)
            {
                IPos = new PointF(0, 0);
            }

            // Increase Pos
            Position.X += IPos.X;
            Position.Y += IPos.Y;

            // Calc Angle
            if (Point.Round(LastPosition) != Point.Round(Position))
            {
                Angle = Calc.GetAngleOfLineBetweenTwoPoints(LastPosition, Position);
            }

            // Store the last position
            LastPosition = new PointF(Position.X, Position.Y);
        }

    }
}
