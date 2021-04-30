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
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void label4_Click(object sender, EventArgs e)
        {
   
        }

        private void about_Load(object sender, EventArgs e)
        {
            label1.Select();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
