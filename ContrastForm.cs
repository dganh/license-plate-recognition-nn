using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace nhandangkitu
{
    public partial class ContrastForm : Form
    {
        public Bitmap view, plate, result1, result2;
        public ContrastForm()
        {
            InitializeComponent();
            slider1.Minimum = 1;
            slider1.Maximum = 100;
            slider1.Value = 1;
            pictureBox1.Image = view;           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
           
        }

      

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) { pictureBox1.Image = view; }
            if (radioButton2.Checked) { pictureBox1.Image = plate; }
        }

        private void slider1_MouseUp(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked)
            {
                result1 = processImage.AdjustContrast(view, (float)(slider1.Value));
                pictureBox1.Image = result1;
            }
            if (radioButton2.Checked & pictureBox1.Image!=null)
            {
                result2 = processImage.AdjustContrast(plate, (float)(slider1.Value));
                pictureBox1.Image = result1;
            }
        }
    }
}
