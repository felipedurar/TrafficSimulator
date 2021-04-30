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
    public partial class OutputViewer : Form
    {
        public OutputData OD = null;
        public string OutputName = "Unknown";

        public OutputViewer()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Monitoring - " + timer1.Interval.ToString() + "ms";
            //
            if (OD.NewDataAvailable)
            {
                richTextBox1.Text += OD.GetNewData();
            }
        }

        private void runtimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            toolStripStatusLabel1.Text = "Monitor Stopped!";
        }

        private void existToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OutputViewer_Load(object sender, EventArgs e)
        {
            if (OD == null)
            {
                MessageBox.Show(" Fatal Internal Error!");
            }
            //
            foreach (string line in OD.Output)
            {
                richTextBox1.Text += line + "\n";
            }
            //
            Text = "OutputViewer - " + OutputName;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }
    }
}
