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
    public partial class MachineControl : Form
    {
        public TrafficSimulator TS = null;

        public MachineControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TS.FollowingCar = null;
        }

        private void MachineControl_Load(object sender, EventArgs e)
        {
            if (TS == null)
            {
                MessageBox.Show("Error");
                Close();
            }
        }
    }
}
