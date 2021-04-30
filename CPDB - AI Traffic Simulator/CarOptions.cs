using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrafficSimulator
{
    public partial class CarOptions : Form
    {
        public Car CCar = null;
        public Form1 F1 = null;
        public TrafficSimulator CTS = null;

        public CarOptions()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OutputViewer OV = new OutputViewer();
            OV.OD = CCar.OutputData;
            OV.OutputName = "car" + CCar.Id.ToString();
            OV.MdiParent = F1;
            OV.Show();
        }

        private void CarOptions_Load(object sender, EventArgs e)
        {
            if (CCar == null || F1 == null)
            {
                return;
            }
            //
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CCar.Kill();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CCar.Revive();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CCar.UserControl = checkBox2.Checked;
            groupBox1.Enabled = checkBox2.Checked;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            CCar.Control.Up = true;
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            CCar.Control.Up = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            CCar.Control.Down = true;
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            CCar.Control.Down = false;
        }

        private void button8_MouseDown(object sender, MouseEventArgs e)
        {
            CCar.Control.Right = true;
        }

        private void button8_MouseUp(object sender, MouseEventArgs e)
        {
            CCar.Control.Right = false;
        }

        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            CCar.Control.Left = true;
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            CCar.Control.Left = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CTS.FollowingCar = CCar;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (CCar == null) Close();
            CTS.CLogic.Cars.RemoveAt(CTS.CLogic.GetCarIndexBydId(CCar.Id));
            this.Close();
        }
    }
}
