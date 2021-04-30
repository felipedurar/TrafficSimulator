using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator
{
    public class Logic
    {
        public List<Car> Cars { get; set; }
        public List<PathGroup> PathGroups { get; set; }
        public List<PathIntersectionData> PathIntersections { get; set; }
        public List<SpeedBump> SpeedBumps { get; set; }
        public List<TrafficLightGroup> TrafficLights { get; set; }
        public CommandProcessor CProcessor = new CommandProcessor();
        private int PathIdCounter = 0;

        public Logic()
        {
            Cars = new List<Car>();
            PathGroups = new List<PathGroup>();
            PathIntersections = new List<PathIntersectionData>();
            SpeedBumps = new List<SpeedBump>();
            TrafficLights = new List<TrafficLightGroup>();
            CProcessor.CLogic = this;
        }

        public int GetNewPathId()
        {
            PathIdCounter++;
            return PathIdCounter - 1;
        }

        public void AddCar(Car c)
        {
            c.Id = Cars.Count;
            c.CLogic = this;
            c.PushOutput(" Respawned at " + Logic.PointToString(c.Position));
            Cars.Add(c);
        }

        public Car GetCarBydId(int id)
        {
            foreach (Car c in Cars)
            {
                if (c.Id == id)
                {
                    return c;
                }
            }
            return null;
        }


        public int GetCarIndexBydId(int id)
        {
            for (int C = 0; C < Cars.Count; C++)
            {
                if (Cars[C].Id == id)
                {
                    return C;
                }
            }
            return -1;
        }

        public PathGroup GetPathGroupBydId(int id)
        {
            foreach (PathGroup pg in PathGroups)
            {
                if (pg.Id == id)
                {
                    return pg;
                }
            }
            return null;
        }

        public TrafficLightGroup GetTrafficLightGroupById(int id)
        {
            foreach (TrafficLightGroup tlg in TrafficLights)
            {
                if (tlg.GroupId == id)
                {
                    return tlg;
                }
            }
            return null;
        }

        public Path GetNearestPath(PointF pos)
        {
            Path np = null;
            float dist = 0.00f;
            bool first = true;
            //
            foreach (PathGroup pg in PathGroups)
            {
                foreach (Path p in pg.Paths)
                {
                    PointF nearestPath = p.GetNearestPathPoint(pos);
                    float distTmp = Calc.Modulus(Calc.Magnitude(pos, nearestPath));
                    //
                    if (first)
                    {
                        first = false;
                        dist = distTmp;
                        np = p;
                    }
                    //
                    if (distTmp < dist)
                    {
                        dist = distTmp;
                        np = p;
                    }
                }
            }
            //
            return np;
        }

        public static string PointToString(PointF p)
        {
            return "(" + p.X.ToString("0.00") + ", " + p.Y.ToString("0.00") + ")";
        }

        //public void AddPathGroup(PathGroup pg)
        //{
        //    pg.Id = PathGroups.Count;
        //    PathGroups.Add(pg);
        //}

        public void CalcPathIntersections()
        {
            PathIntersections.Clear();
            // Test each path
            for (int C = 0; C < PathGroups.Count; C++)
            {
                for (int D = 0; D < PathGroups[C].Paths.Count; D++)
                {
                    //
                    for (int E = 0; E < PathGroups.Count; E++)
                    {
                        for (int F = 0; F < PathGroups[E].Paths.Count; F++)
                        {
                            // Verify if it's the same
                            if (C != E)
                            {
                                IntersectionData id = PathGroups[C].Paths[D].IntersectsWith(PathGroups[E].Paths[F]);
                                //
                                PathIntersectionData pid = new PathIntersectionData();
                                pid.IntersectionData = id;
                                pid.IntersectionPoint = id.IntersectionPoint;
                                pid.P1 = PathGroups[C].Paths[D];
                                pid.P2 = PathGroups[E].Paths[F];
                                //
                                PathIntersections.Add(pid);
                            }
                            //
                        }
                    }
                    //
                }
            }
        }

        public void Update()
        {
            foreach (Car c in Cars)
            {
                // Keep In Path
                if (c.KeepInPath)
                {
                    Path np = GetNearestPath(c.Position);
                    PointF pathNearestPoint = np.GetNearestPathPoint(c.Position);
                    //
                    float dist = Calc.Modulus(Calc.Magnitude(c.Position, pathNearestPoint));
                    if (dist > c.ErrorMargin)
                    {
                        if (c.ImmediateTasks.Count == 0)
                        {
                            Task t = c.CreateTask(pathNearestPoint, TaskType.Immediate);
                            c.ImmediateTasks.Push(t);
                        }
                    }
                }

                // Update the Traffic Light
                foreach (TrafficLightGroup tfg in TrafficLights)
                {
                    tfg.DoStep();
                }

                // Update the Car Behavior
                c.Update();
            }
        }

    }

    public enum TaskType
    {
        None, Immediate, Queue
    }

    public class Task
    {
        public PointF InitialPosition = new Point(0, 0);
        public PointF Objective = new Point(0, 0);
        public bool Unread = true;
        public TaskType Type = TaskType.None;
    }

    public class IntersectionData
    {
        public bool Intersects { get; set; }
        public bool Parallel { get; set; }
        public PointF IntersectionPoint { get; set; }
    }

    public class PathIntersectionData
    {
        public IntersectionData IntersectionData { get; set; }
        public PointF IntersectionPoint { get; set; }
        public Path P1 { get; set; }
        public Path P2 { get; set; }
    }

}
