using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TrafficSimulator
{
    public partial class TrafficSimulator : Form
    {
        public Form1 F1 = null;
        public Logic CLogic = new Logic();

        public Point CTranslation = new Point(0, 0);
        public float CScale = 1.0f;

        public Point MPosition = new Point(0, 0);
        public bool MLBD = false;
        public bool MRBD = false;

        public Car FollowingCar = null;

        public string MachineName = "machine001";

        public TrafficSimulator()
        {
            InitializeComponent();
            pictureBox1.MouseWheel += new MouseEventHandler(PicBox_MouseWheel);
        }

        private void TrafficSimulator_Load(object sender, EventArgs e)
        {
            Text = MachineName + " - Running";
        }

        public Point ProjectPoint(Point p)
        {
            return Point.Round(new PointF((p.X + CTranslation.X) * CScale, (p.Y + CTranslation.Y) * CScale));
        }

        public Rectangle ProjectRectangle(Rectangle r)
        {
            return new Rectangle(ProjectPoint(r.Location), ProjectSize(r.Size));
        }

        public Size ProjectSize(Size p)
        {
            return Size.Round(new SizeF(p.Width * CScale, p.Height * CScale));
        }

        public Point UnprojectPoint(Point p)
        {
            return Point.Round(new PointF((p.X / CScale) - CTranslation.X, (p.Y / CScale) - CTranslation.Y));
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics GOut = e.Graphics;

            GOut.Clear(Color.Black);

            if (CLogic == null) return;

            foreach (PathGroup pg in CLogic.PathGroups)
            {
                Point lastBegin = new Point(0, 0);
                Point lastEnd = new Point(0, 0);
                bool first = true;
                foreach (Path p in pg.Paths)
                {
                    // Draw the Path
                    GOut.DrawLine(new Pen(Color.White, 3), ProjectPoint(p.P1),
                        ProjectPoint(p.P2));

                    // Draw the direction
                    PointF cArrow = new PointF(p.P1.X, p.P1.Y);
                    double dirSE = Calc.GetAngleOfLineBetweenTwoPoints(p.P1, p.P2);
                    Color lineColor = Color.FromArgb(100, 255, 255, 255);
                    double inverseAngle = dirSE;
                    if (p.Direction == true)
                    {
                        inverseAngle -= Math.PI;
                    }
                    float diffAngle = 20.00f; // IN DEGREE
                    float lineSize = 20.00f;
                    float distSE = Calc.Modulus(Calc.Magnitude(p.P1, p.P2));
                    float distMagn = 30.0f;
                    while (true)
                    {
                        if (Calc.Modulus(Calc.Magnitude(cArrow, p.P1)) > distSE)
                        {
                            break;
                        }
                        //
                        double angleP1 = inverseAngle - Calc.DegreeToRadians(diffAngle);
                        double angleP2 = inverseAngle + Calc.DegreeToRadians(diffAngle);
                        PointF p1 = new PointF((float)(cArrow.X - (Math.Cos(angleP1) * lineSize)),
                            (float)(cArrow.Y - (Math.Sin(angleP1) * lineSize)));
                        PointF p2 = new PointF((float)(cArrow.X - (Math.Cos(angleP2) * lineSize)),
                            (float)(cArrow.Y - (Math.Sin(angleP2) * lineSize)));
                        //
                        GOut.DrawLine(new Pen(lineColor), ProjectPoint(Point.Round(cArrow)), ProjectPoint(Point.Round(p1)));
                        GOut.DrawLine(new Pen(lineColor), ProjectPoint(Point.Round(cArrow)), ProjectPoint(Point.Round(p2)));
                        //
                        cArrow.X += (float)(Math.Cos(dirSE) * distMagn);
                        cArrow.Y += (float)(Math.Sin(dirSE) * distMagn);
                    }

                    // Draw the line that belongs the pathgroup
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        GOut.DrawLine(new Pen(Color.Gray), ProjectPoint(lastBegin),
                            ProjectPoint(p.P1));
                        GOut.DrawLine(new Pen(Color.Gray), ProjectPoint(lastEnd),
                            ProjectPoint(p.P2));
                    }
                    //
                    lastBegin = p.P1;
                    lastEnd = p.P2;
                }
            }

            foreach (SpeedBump sb in CLogic.SpeedBumps)
            {
                GOut.FillRectangle(new SolidBrush(Color.Orange), ProjectRectangle(Rectangle.Round(sb.GetRectangle())));
            }

            foreach (TrafficLightGroup tlg in CLogic.TrafficLights)
            {
                foreach (TrafficLight tl in tlg.TLs)
                {
                    GOut.FillRectangle(new SolidBrush(tl.GetColor()), ProjectRectangle(Rectangle.Round(tl.GetRectangle())));
                }
            }

            foreach (PathIntersectionData pid in CLogic.PathIntersections)
            {
                if (pid.IntersectionData.Intersects)
                {
                    GOut.FillPie(new SolidBrush(Color.Red), new Rectangle(ProjectPoint(new Point((int)(pid.IntersectionPoint.X - 3), (int)(pid.IntersectionPoint.Y - 3))), new Size(6, 6)), 0, 360);
                }
            }

            foreach (Car c in CLogic.Cars)
            {
                // Draw the rectangle
                Point projectedPoint = ProjectPoint(Point.Round(c.Position));
                Size projectedSize = ProjectSize(c.Size);
                Rectangle carRect = new Rectangle((int)(projectedPoint.X - (projectedSize.Width / 2)), 
                    (int)(projectedPoint.Y - (projectedSize.Height / 2)),
                    projectedSize.Width, projectedSize.Height);
                GOut.FillRectangle(new SolidBrush(c.Color), carRect);

                // Draw the FOV
                float lineSize = 30.0f;
                double halfAngleRad = Calc.DegreeToRadians(c.FOV / 2);
                double p1Angle = c.Angle - halfAngleRad;
                double p2Angle = c.Angle + halfAngleRad;
                PointF p1Rad = new PointF((float)(c.Position.X + (Math.Cos(p1Angle) * lineSize)), (float)(c.Position.Y + (Math.Sin(p1Angle) * lineSize)));
                PointF p2Rad = new PointF((float)(c.Position.X + (Math.Cos(p2Angle) * lineSize)), (float)(c.Position.Y + (Math.Sin(p2Angle) * lineSize)));
                GOut.DrawLine(new Pen(Color.FromArgb(100, 255, 255, 255)), ProjectPoint(Point.Round(c.Position)), ProjectPoint(Point.Round(p1Rad)));
                GOut.DrawLine(new Pen(Color.FromArgb(100, 255, 255, 255)), ProjectPoint(Point.Round(c.Position)), ProjectPoint(Point.Round(p2Rad)));

                // Draw the Id
                Font idFont = new System.Drawing.Font("Consolas", 12);
                SizeF stringSize = GOut.MeasureString(c.Id.ToString(), idFont);
                GOut.DrawString(c.Id.ToString(), idFont, new SolidBrush(Color.White), 
                    new PointF(carRect.X + ((carRect.Width / 2) - (stringSize.Width / 2)), carRect.Y - stringSize.Height));
            }

        }

        public void PauseMachine()
        {
            timer1.Stop();
        }

        public void ResumeMachine()
        {
            timer1.Start();
        }

        private void PicBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                CScale -= 0.05f;
            }
            else
            {
                CScale += 0.05f;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CLogic.Update();
            //
            // Camera
            if (FollowingCar != null)
            {
                CTranslation.X = -1*(int)((FollowingCar.Position.X - (pictureBox1.Width / 2)) / CScale);
                CTranslation.Y = -1*(int)((FollowingCar.Position.Y - (pictureBox1.Height / 2)) / CScale);
            }
        }

        public Car PickCar(Point pos)
        {
            for (int C = 0; C < CLogic.Cars.Count; C++)
            {
                RectangleF carRect = new RectangleF(CLogic.Cars[C].Position.X - (CLogic.Cars[C].Size.Width / 2),
                    CLogic.Cars[C].Position.Y - (CLogic.Cars[C].Size.Height / 2), CLogic.Cars[C].Size.Width, CLogic.Cars[C].Size.Height);
                RectangleF cursorRect = new RectangleF(pos, new SizeF(1, 1));
                //
                if (carRect.IntersectsWith(cursorRect))
                {
                    return CLogic.Cars[C];
                }
            }
            return null;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MLBD = true;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MRBD = true;
            }
            //
            Point cPosition = UnprojectPoint(MPosition);
            Car c = PickCar(cPosition);
            if (c != null)
            {
                c.Speed = 0.00f;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MLBD = false;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MRBD = false;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point cPosition = UnprojectPoint(e.Location);
            Size diffUnprojected = new Size(UnprojectPoint(MPosition).X - UnprojectPoint(e.Location).X, UnprojectPoint(MPosition).Y - UnprojectPoint(e.Location).Y);
            //
            if (MLBD)
            {
                Car c = PickCar(cPosition);
                if (c != null)
                {
                    c.Speed = 0.00f;
                    c.Position.X -= diffUnprojected.Width;
                    c.Position.Y -= diffUnprojected.Height;
                }
                else
                {
                    CTranslation.X -= diffUnprojected.Width;
                    CTranslation.Y -= diffUnprojected.Height;
                }
            }
            //

            //
            MPosition = e.Location;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Car c = PickCar(UnprojectPoint(MPosition));
            if (c != null)
            {
                //c.Speed = 0.00f;
                //c.Position.X -= diff.Width;
                //c.Position.Y -= diff.Height;
                //
                CarOptions co = new CarOptions();
                co.CCar = c;
                co.F1 = F1;
                co.CTS = this;
                co.MdiParent = F1;
                co.Show();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void TrafficSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void respawnACarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Car c = new Car();
            c.Position = UnprojectPoint(MPosition);
            CLogic.AddCar(c);
        }

        private void spawnAStupidCarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cars.StupidCar c = new Cars.StupidCar();
            c.Position = UnprojectPoint(MPosition);
            CLogic.AddCar(c);
        }
    }
}
