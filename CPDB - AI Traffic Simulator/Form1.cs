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
    public partial class Form1 : Form
    {
        //public TrafficSimulator TS = null;
        public List<TrafficSimulator> TSs = new List<TrafficSimulator>();

        public Form1()
        {
            InitializeComponent();
        }

        public TrafficSimulator GetMachineByName(string name)
        {
            for (int C = 0; C < TSs.Count; C++)
            {
                if (TSs[C].MachineName == name)
                {
                    return TSs[C];
                }
            }
            return null;
        }

        public string GetNewMachineName()
        {
            string initName = "machine";
            for (int C = 1; true; C++)
            {
                bool exists = GetMachineByName(initName + C.ToString()) != null;
                if (!exists)
                {
                    return initName + C.ToString();
                }
            }
        }

        public void NewMachine()
        {
            TrafficSimulator tsTmp = new TrafficSimulator();
            tsTmp.MachineName = GetNewMachineName();
            tsTmp.F1 = this;
            tsTmp.Show();
            TSs.Add(tsTmp);
            //
            UpdateMachineList();
            if (toolStripComboBox1.Items.Count > 0)
            {
                toolStripComboBox1.SelectedIndex = toolStripComboBox1.Items.Count - 1;
            }
        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //TS = new TrafficSimulator();
            //TS.Show();
            //
            NewMachine();
        }

        public void RunCode(string code)
        {
            if (TSs.Count == 0)
            {
                NoMachineRunning nmr = new NoMachineRunning();
                if (nmr.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    NewMachine();
                }
                else
                {
                    return;
                }
            }
            //
            if (toolStripComboBox1.SelectedItem == null)
            {
                if (toolStripComboBox1.Items.Count > 0)
                {
                    toolStripComboBox1.SelectedIndex = 0;
                }
            }
            //
            if (GetCurrentMachine() == null || GetCurrentMachine().IsDisposed)
            {
                // Do Some Error
                return;
            }
            //GetCurrentMachine().RunCode(code);
            GetCurrentMachine().CLogic.CProcessor.RunCode(code);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NewMapFile();
        }

        public Map_Code_Viewer NewMapFile()
        {
            Map_Code_Viewer mcv = new Map_Code_Viewer();
            mcv.F1 = this;
            mcv.MdiParent = this;
            mcv.Show();
            return mcv;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMapFile();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        public void UpdateMachineList()
        {
            toolStripComboBox1.Items.Clear();
            //
            for (int C = 0; C < TSs.Count; C++)
            {
                if (!TSs[C].IsDisposed)
                {
                    toolStripComboBox1.Items.Add(TSs[C].MachineName);
                }
            }

        }

        public TrafficSimulator GetCurrentMachine()
        {
            if (toolStripComboBox1.SelectedItem == null) return null;
            string selectedItem = toolStripComboBox1.SelectedItem.ToString();
            return GetMachineByName(selectedItem);
        }

        private void toolStripComboBox1_DropDown(object sender, EventArgs e)
        {
            UpdateMachineList();
        }

        private void viewGlobalOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void assignACarOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void viewCurrentMachineOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrafficSimulator ts = GetCurrentMachine();
            if (ts == null)
            {
                MessageBox.Show(" Select a machine first!");
                return;
            }
            OutputViewer ov = new OutputViewer();
            ov.OD = ts.CLogic.CProcessor.OD;
            ov.OutputName = ts.MachineName;
            ov.MdiParent = this;
            ov.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about ab = new about();
            ab.ShowDialog();
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrafficSimulator ts = GetCurrentMachine();
            if (ts == null)
            {
                MessageBox.Show(" Select a machine first!");
                return;
            }
            ts.ResumeMachine();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrafficSimulator ts = GetCurrentMachine();
            if (ts == null)
            {
                MessageBox.Show(" Select a machine first!");
                return;
            }
            ts.PauseMachine();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map_Code_Viewer mcv = NewMapFile();
            if (mcv != null)
            {
                if (!mcv.OpenMapCode())
                {
                    mcv.Close();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void currentMachineControlPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrafficSimulator ts = GetCurrentMachine();
            if (ts == null)
            {
                MessageBox.Show(" Select a machine first!");
                return;
            }
            MachineControl ov = new MachineControl();
            ov.TS = ts;
            ov.MdiParent = this;
            ov.Show();
        }
    }
}
