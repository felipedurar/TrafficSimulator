using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace TrafficSimulator
{
    public class CommandProcessor
    {
        public Logic CLogic = null;
        public OutputData OD = new OutputData();

        public void AddCar(int x, int y)
        {
            Car c = new Car();
            c.Position = new Point(x, y);
            CLogic.AddCar(c);
        }

        public void AddTask(int id, int xo, int yo)
        {
            Car c = CLogic.GetCarBydId(id);
            c.EnqueueTask(c.CreateTask(new PointF(xo, yo), TaskType.Queue));
        }

        public void SetCarPos(int id, int xo, int yo)
        {
            Car c = CLogic.GetCarBydId(id);
            c.Position = new PointF(xo, yo);
        }

        public void AddPathGroup(int id)
        {
            PathGroup pg = new PathGroup();
            pg.Id = id;
            //CLogic.AddPathGroup(pg);
            CLogic.PathGroups.Add(pg);
        }

        public void AddPath(int id, int x1, int y1, int x2, int y2, int dir)
        {
            Path p = new Path();
            p.SetPath(new Point(x1, y1), new Point(x2, y2), dir == 0 ? false : true);
            PathGroup pg = CLogic.GetPathGroupBydId(id);
            p.ParentPathGroup = pg;
            p.ID = CLogic.GetNewPathId();
            pg.Paths.Add(p);
        }

        public void PutInTrail(int carId, int pgId, int pathIndex)
        {
            Car c = CLogic.GetCarBydId(carId);
            PathGroup pg = CLogic.GetPathGroupBydId(pgId);
            c.PutInPath(pg.Paths[pathIndex]);
        }

        public void CalcPathIntersections(int n)
        {
            CLogic.CalcPathIntersections();
        }

        public void Clear(int n)
        {
            CLogic.PathGroups.Clear();
            CLogic.Cars.Clear();
            CLogic.PathIntersections.Clear();
            CLogic.SpeedBumps.Clear();
            CLogic.TrafficLights.Clear();
        }

        public void AddSpeedBump(int x, int y)
        {
            SpeedBump sb = new SpeedBump();
            sb.Position = new PointF(x, y);
            CLogic.SpeedBumps.Add(sb);
        }

        public void AddTrafficLightGroup(int id)
        {
            TrafficLightGroup tlg = new TrafficLightGroup();
            tlg.GroupId = id;
            CLogic.TrafficLights.Add(tlg);
        }

        public void AddTrafficLight(int idGroup, int idStatus, int x, int y, float angle)
        {
            TrafficLightGroup tlg = CLogic.GetTrafficLightGroupById(idGroup);
            TrafficLight tl = new TrafficLight(idStatus);
            tl.Angle = angle;
            tl.Position = new PointF(x, y);
            tlg.AddTrafficLight(tl);
        }

        public void RunCode(string code)
        {
            // execute the script
            ScriptEngine engine = Python.CreateEngine();
            // ScriptScope
            dynamic scope = engine.CreateScope();
            ScriptSource source = engine.CreateScriptSourceFromString(code, Microsoft.Scripting.SourceCodeKind.Statements);
            CompiledCode compiled;
            try
            {
                compiled = source.Compile();
            }
            catch (Exception ee)
            {
                //f1.richTextBox1.Text += " Compile Error: " + e.Message + "\n";
                //MessageBox.Show(" Compiler Error: " + ee.Message);
                OD.PushNewData("Compiler Error: " + ee.Message);
                return;
            }
            //scope.SetVariable("Out", workLinesS);
            //scope.FlushOut = new Func<int>(() => {
            //    string[] lns = ((string)scope.GetVariable("Out")).Split('\n');
            //    for (int C = 0; C < lns.Length; C++)
            //    {
            //        OutCode.Add(lns[C]);
            //    }
            //    return 0;
            //});
            scope.AddCar = new Action<int, int>(AddCar);
            scope.AddTask = new Action<int, int, int>(AddTask);
            scope.SetCarPos = new Action<int, int, int>(SetCarPos);
            scope.AddPathGroup = new Action<int>(AddPathGroup);
            scope.CalcPathIntersections = new Action<int>(CalcPathIntersections);
            scope.AddPath = new Action<int, int, int, int, int, int>(AddPath);
            scope.PutInTrail = new Action<int, int, int>(PutInTrail);
            scope.AddSpeedBump = new Action<int, int>(AddSpeedBump);
            scope.Clear = new Action<int>(Clear);
            scope.AddTrafficLightGroup = new Action<int>(AddTrafficLightGroup);
            scope.AddTrafficLight = new Action<int, int, int, int, float>(AddTrafficLight);
            //
            try
            {
                compiled.Execute(scope);
            }
            catch (Exception ee)
            {
                //MessageBox.Show(" Compiler Error: " + ee.Message);
                OD.PushNewData("Compiler Error: " + ee.Message);
                return;
            }
            //
            OD.PushNewData(" Compilation Finished!");
        }
    }
}
