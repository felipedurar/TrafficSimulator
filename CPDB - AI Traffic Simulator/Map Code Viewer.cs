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
    public partial class Map_Code_Viewer : Form
    {
        public Form1 F1 = null;

        public Map_Code_Viewer()
        {
            InitializeComponent();
        }

        private void Map_Code_Viewer_Load(object sender, EventArgs e)
        {
            if (F1 == null)
            {
                MessageBox.Show("Internal Error!");
                return;
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            F1.RunCode(richTextBox1.Text);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        public bool OpenMapCode()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Map File|*.map*|All Files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(ofd.FileName);
                return true;
            }
            return false;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenMapCode();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "Map File|*.map*|All Files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(ofd.FileName)) File.Delete(ofd.FileName);
                //
                File.WriteAllText(ofd.FileName, richTextBox1.Text);
            }
        }
    }
}
