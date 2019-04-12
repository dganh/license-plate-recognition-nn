using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;



namespace nhandangkitu
{
    public partial class TrainerForm : DevComponents.DotNetBar.Office2007Form
    {
        public NeuralNetwork NN = new NeuralNetwork();

        
        public TrainerForm()
        {
            InitializeComponent();
            numericUpDown1.Maximum = 5000;
            numericUpDown1.Minimum = 200;
            numericUpDown1.Value = NN.epochs;
            numericUpDown3.Value = (int)(NN.learning_rate);
            numericUpDown4.Value = NN.numberPattern;
            NN.form_network();
        }
        
        private void buttonX1_Click(object sender, EventArgs e)
        {
            NN.load_character_trainer_set(openFileDialog1);
            labelX1.Text = NN.trainer_string;
        }

        //private void buttonX2_Click(object sender, EventArgs e)
        //{

        //    NN.learning_rate = (float)(numericUpDown3.Value);
        //    NN.numberPattern = (int)(numericUpDown4.Value);
        //    NN.epochs = (int)(numericUpDown1.Value);

        //    NN.initialize_weights();
        //    NN.form_input_set();
        //    NN.form_desired_output_set();
        //    NN.train_network2(progressBar1);
        //    label4.Text += " Done!";
           
        //}


        private void buttonX3_Click(object sender, EventArgs e)
        {
            NN.save_network(saveFileDialog1);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Increment =100;

        }

        private void buttonX2_Click(object sender, EventArgs e)
        {


            NN.learning_rate = (float)(numericUpDown3.Value);
            NN.numberPattern = (int)(numericUpDown4.Value);
            NN.epochs = (int)(numericUpDown1.Value);

            NN.initialize_weights();
            NN.form_input_set();
            NN.form_desired_output_set();
            NN.train_network2(progressBar1);
            label4.Text += " Done!";

            //Thread trainer_thread = new Thread(new ThreadStart(NN.train_network2));
            //trainer_thread.Start();		
            
        }

            

      
    }
}
