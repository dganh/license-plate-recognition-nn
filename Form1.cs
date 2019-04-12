using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;

namespace nhandangkitu
{
    public partial class Form1 : DevComponents.DotNetBar.Office2007Form
    {
        private FilterInfoCollection Devices;
        private VideoCaptureDevice usedDevice;
        public
        Bitmap plateImg;
        Bitmap charImg;
        Bitmap viewImg;
        NeuralNetwork NN = new NeuralNetwork();
        
        int[] F;
       
        

        public Form1()
        {
            InitializeComponent();
            NN.form_network();
            NN.load_network();
           
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////////////
        //*************************000000000000000000000*********************************************//
        /////////////////////////////////////////////////////////////////////////////////////////////
        public void load_image()
        {
            
            string path;
            OpenFileDialog od = new OpenFileDialog();

                od.ShowDialog();
                path = od.FileName;
                if (path == "")
                {
                    return;
                }
                viewImg = new Bitmap(path);  //selected Palmprint Image
               
        }
        
     
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// //////////////////////////////////-EVEN PROCESS-//////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        
        private void button1_Click(object sender, EventArgs e)
        {
            load_image();
        }

       
        private void buttonItem24_Click(object sender, EventArgs e)
        {
            NN.load_network(openFileDialog1);
        }

        private void buttonX2_Click(object sender, EventArgs e)  //recog
        {
            richTextBox1.Text = null;
            try
            {
            charImg = new Bitmap(pictureBox2.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
            
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string;
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox3.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
           
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string;
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox4.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
          
                NN.get_input();
                NN.recognization();
                
                richTextBox1.Text +="-";
                richTextBox1.Text += "*";
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox5.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
           
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string + " ";
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox6.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
            
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string;
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox7.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
            
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string;
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox8.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
            
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string;
            }
            catch { richTextBox1.Text += "?"; }
            try
            {
            charImg = new Bitmap(pictureBox9.Image);
            F = processImage.ImgGetInput(charImg);
            NN.F = F;
            
                NN.get_input();
                NN.recognization();
                richTextBox1.Text += NN.output_string;
            }
            catch { richTextBox1.Text += "?"; }



        }

        private void buttonItem26_Click(object sender, EventArgs e)
        {
            TrainerForm frm = new TrainerForm();
            frm.ShowDialog();
        }

       
       

        private void buttonItem19_Click(object sender, EventArgs e)
        {
            if (pictureBox10.Image != null)
            {
                
                plateImg = new Bitmap(pictureBox10.Image);
                plateImg = processImage.plateResize(plateImg);
                //pictureBox10.Image = plateImg;
                plateImg = processImage.AdjustContrast(plateImg, 50);
                try
                {
                    ExtractCharacter ex = new ExtractCharacter(plateImg);
                    ex.Execute();
                    pictureBox2.Image = ex.CutImg1();
                    pictureBox3.Image = ex.CutImg2();
                    pictureBox4.Image = ex.CutImg3();
                    pictureBox5.Image = ex.CutImg4();
                    pictureBox6.Image = ex.CutImg5();
                    pictureBox7.Image = ex.CutImg6();
                    pictureBox8.Image = ex.CutImg7();
                    pictureBox9.Image = ex.CutImg8();
                   
                }
                catch { MessageBox.Show("Chất lượng ảnh kém"); }
            }
            else { MessageBox.Show("Ảnh biển số xe= NULL"); }
            
        }

        private void buttonItem15_Click(object sender, EventArgs e)
        {
            if (pictureBox10.Image != null)
            {
                ExtractCharacter ex;
                plateImg = new Bitmap(pictureBox10.Image);
               try
                {
                    Bitmap a = plateImg;
                    plateImg = processImage.plateResize(plateImg);
                    ex = new ExtractCharacter(plateImg);
                    ex.Execute();
                    double angle = ex.GetAngle();
                    plateImg = RotateImg.RotateImage(a, angle);
                    pictureBox10.Image = processImage.plateResize(plateImg);
                    //plateImg = RotateImg.RotateImage(plateImg, angle);
                    MessageBox.Show((angle*360.0/Math.PI).ToString());
                   //////////////////////////////////////////////////////////////////////////////////////
                    
               }
                catch { MessageBox.Show("Chất lượng ảnh kém"); }
            }

            else { MessageBox.Show("Ảnh biển số xe= NULL"); }
        }

        private void buttonItem7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
        }

     
        private void buttonItem36_Click(object sender, EventArgs e)
        {
            
            if (pictureBox1.Image != null )
            {
                ContrastForm frm = new ContrastForm();
                frm.view = new Bitmap(pictureBox1.Image);
                if(pictureBox10.Image!=null)
                frm.plate = new Bitmap(pictureBox10.Image);
                frm.ShowDialog();
                pictureBox1.Image = frm.result1;
                pictureBox10.Image = frm.result2;
                               
            }
            else
            {
                MessageBox.Show("error!");
            }
        }

        private void buttonItem6_Click(object sender, EventArgs e)
        {
            if (pictureBox10.Image != null) 
            {
                Bitmap a = new Bitmap(pictureBox10.Image);
                pictureBox10.Image = processImage.Median_LockBits(a); 
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            usedDevice = new VideoCaptureDevice( Devices[comboBox1.SelectedIndex].MonikerString);
            usedDevice.NewFrame += new NewFrameEventHandler(usedDevice_NewFrame);
            usedDevice.Start();
        }

        void usedDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = img;
        }

       
        private void Form1_Load(object sender, EventArgs e)
        {
            Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo d in Devices)
            {
                comboBox1.Items.Add(d.Name);
            }
            //comboBox1.SelectedIndex = 0;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            load_image();
            pictureBox10.Image = viewImg;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            try
            {
                if (usedDevice.IsRunning)
                {
                    usedDevice.Stop();
                    viewImg = (Bitmap)pictureBox1.Image;
                }
                else
                {
                    usedDevice.Start();
                }
            }
            catch { MessageBox.Show("Chưa Kết Nối Camera.."); }
            
        }

        private void buttonItem4_Click(object sender, EventArgs e)
        {
            processImage.SaveImg(pictureBox1);
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
            AboutBox1 frm = new AboutBox1();
            frm.ShowDialog();
        }

      

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            load_image();
            pictureBox1.Image = viewImg;
        }

        private void buttonItem23_Click(object sender, EventArgs e)
        {
            viewImg = (Bitmap)pictureBox1.Image;
            catbienso cut = new catbienso(viewImg);
            pictureBox10.Image = cut.CatDoc(viewImg);
            plateImg = (Bitmap)pictureBox10.Image;
        }

       

      

        

      

    }
}