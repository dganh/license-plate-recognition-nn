using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;


namespace nhandangkitu
{
    public class NeuralNetwork
    {
               
        
        const int number_of_layers = 3;						//3
        const int number_of_input_nodes = 20;				//150
        const int number_of_output_nodes = 7;	     		//7
        const int maximum_layers = 30;				//250
        const int maximum_number_of_sets = 3600;			//3600
        public int numberPattern = 30;                 //số mẫu dạy cho mỗi kí tự.50
        public int epochs = 1500;	   					//300
        const float error_threshold = 0.001F;				//0.0002F
        public float learning_rate = 20F;					//150F          tỉ lệ thuận với tốc độ học và tốc độ giảm sai số.-> số vòng lặp
        float slope = 0.014F;								//0.014F	tỉ lệ thuận với tốc độ học và tốc độ giảm sai sô.nhưng cần nhiều tt để học hơn
        int weight_bias = 30;								//30        KN:sai số giảm nhanh-> học ít-> ko biết j


        public int number_of_input_sets;
        public int[] layers = new int[number_of_layers];
        public float[] current_input = new float[number_of_input_nodes];
        public float[,] input_set = new float[number_of_input_nodes, maximum_number_of_sets];
        public int[] desired_output = new int[number_of_output_nodes];
        public float[,] node_output = new float[number_of_layers, maximum_layers];
        float[, ,] weight = new float[number_of_layers, maximum_layers, maximum_layers];
        public float[,] error = new float[number_of_layers, maximum_layers];
        public int[] output_bit = new int[number_of_output_nodes];
        public int[] desired_output_bit = new int[number_of_output_nodes];
        public int[,] desired_output_set = new int[number_of_output_nodes, maximum_number_of_sets];

        public string output_string;
        public string trainer_string;
        public string character_trainer_set_file_path;
        Random rnd = new Random();
        Encoding ascii = Encoding.ASCII;

        System.IO.StreamReader character_trainer_set_file_stream;
        System.IO.StreamWriter network_save_file_stream;
        System.IO.StreamReader network_load_file_stream;

        public int[] F;    //biến đầu vào



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void form_network()
        {
            layers[0] = number_of_input_nodes;
            layers[number_of_layers - 1] = number_of_output_nodes;
            for (int i = 1; i < number_of_layers - 1; i++)
                layers[i] = maximum_layers;
        }
        public void initialize_weights()
        {
            for (int i = 1; i < number_of_layers; i++)
                for (int j = 0; j < layers[i]; j++)
                    for (int k = 0; k < layers[i - 1]; k++)
                        weight[i, j, k] = (float)(rnd.Next(-weight_bias, weight_bias));
        }
        public void form_input_set()    //ok!
        {
            for (int k = 0; k < number_of_input_sets; k++)
            {
                string path;
                path = character_trainer_set_file_path.ToString() + "\\" + (k + 1).ToString() + ".bmp";
                try
                {
                    Bitmap setsImg = new Bitmap(path);
                    F = processImage.ImgGetInput(setsImg);
                }
                catch 
                { MessageBox.Show(path); }

                
                

                int i = 0;
                for (int j = 0; j < 4; j++)
                {
                     input_set[i++, k] = F[j];
                }
               
                for (int j = 4; j < 16; j+=2)
                {
                    input_set[i++, k] = F[j];
                }
                
                for (int j = 16; j < 20; j++)
                {
                    input_set[i++, k] = F[j];
                }
                for (int j = 20; j < 32; j += 2)
                {
                    input_set[i++, k] = F[j];
                }
            }
        }
        public void form_desired_output_set()   //ok!
        {
            for (int k = 0; k < number_of_input_sets; k++)
            {
                character_to_ascii(trainer_string[k / numberPattern].ToString()); //MessageBox.Show(trainer_string[k/numberPattern].ToString()); // k:numberPattern sẽ ra thứ tự của kí tự có trong file sets
                for (int j = 0; j < number_of_output_nodes; j++)
                    desired_output_set[j, k] = desired_output_bit[j];
            }
        }
        public void get_inputs(int set_number)  //ok!
        {
            for (int i = 0; i < number_of_input_nodes; i++)
                current_input[i] = input_set[i, set_number];
        }
        public void get_input()
        {

            int i = 0;
            for (int j = 0; j < 4; j++)
            {
                current_input[i++] = F[j];
            }

            for (int j = 4; j < 16; j += 2)
            {
                current_input[i++] = F[j];
            }

            for (int j = 16; j < 20; j++)
            {
                current_input[i++] = F[j];
            }
            for (int j = 20; j < 32; j += 2)
            {
                current_input[i++] = F[j];
            }
        }

        public void get_desired_outputs(int set_number)
        {
            for (int i = 0; i < number_of_output_nodes; i++)
                desired_output[i] = desired_output_set[i, set_number];
        }

        public void load_character_trainer_set(OpenFileDialog open)    //ok!
        {
            string line;
            open.Filter = "Character Trainer Set (*.txt)|*.txt";
            if (open.ShowDialog() == DialogResult.OK)
            {
                character_trainer_set_file_stream = new System.IO.StreamReader(open.FileName);
                trainer_string = "";
                while ((line = character_trainer_set_file_stream.ReadLine()) != null)
                    trainer_string = trainer_string + line; MessageBox.Show(trainer_string);
                number_of_input_sets = trainer_string.Length * numberPattern;
               
                character_trainer_set_file_path = Path.GetDirectoryName(open.FileName);
                character_trainer_set_file_stream.Close();
                output_string = "";
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public void train_network2(ProgressBar progressBar1)
        {
            int set_number;
            float average_error = 0.0F;
            progressBar1.Maximum = epochs;
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                average_error = 0.0F;
                for (int i = 0; i < number_of_input_sets; i++)
                {
                    set_number = rnd.Next(0, number_of_input_sets);
                    get_inputs(set_number);
                    get_desired_outputs(set_number); // setnumber ~ k
                    calculate_outputs();
                    calculate_errors();
                    calculate_weights();
                    average_error = average_error + get_average_error();
                }
                //progressBar1.PerformStep();
                progressBar1.Value += 1;
                average_error = average_error / number_of_input_sets;
                if (average_error < error_threshold)
                {
                    MessageBox.Show(average_error.ToString() + "-" + epoch.ToString());
                    epoch = epochs + 1;
                    progressBar1.Value = progressBar1.Maximum;

                }
                if (epoch == epochs - 1)
                {
                    MessageBox.Show(average_error.ToString() + "-" + epoch.ToString());
                    progressBar1.Value = progressBar1.Maximum;
                }

            }

        }
       
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public void recognization()
        {
            calculate_outputs();
            for (int i = 0; i < number_of_output_nodes; i++)
            {
                output_bit[i] = threshold(node_output[number_of_layers - 1, i]);
            }
            char character = ascii_to_character();
            output_string = character.ToString();
        }
        public void calculate_outputs()
        {
            float f_net;
            int number_of_weights;
            for (int i = 0; i < number_of_layers; i++)
                for (int j = 0; j < layers[i]; j++)
                {
                    f_net = 0.0F;
                    if (i == 0) number_of_weights = 1;
                    else number_of_weights = layers[i - 1];

                    for (int k = 0; k < number_of_weights; k++)
                        if (i == 0)
                            f_net = current_input[j];
                        else
                            f_net = f_net + node_output[i - 1, k] * weight[i, j, k];
                    node_output[i, j] = sigmoid(f_net);
                }
        }
        public float sigmoid(float f_net)
        {
            float result = (float)((2 / (1 + Math.Exp(-1 * slope * f_net))) - 1);		//Bipolar			
            return result;
        }
        public float sigmoid_derivative(float result)
        {
            float derivative = (float)(0.5F * (1 - Math.Pow(result, 2)));			//Bipolar			
            return derivative;
        }
        public int threshold(float val)
        {
            if (val < 0.5)
                return 0;
            else
                return 1;
        }
        public void calculate_errors()
        {
            float sum = 0.0F;
            for (int i = 0; i < number_of_output_nodes; i++)
                error[number_of_layers - 1, i] = (float)((desired_output[i] - node_output[number_of_layers - 1, i]) * sigmoid_derivative(node_output[number_of_layers - 1, i]));
            for (int i = number_of_layers - 2; i >= 0; i--)
                for (int j = 0; j < layers[i]; j++)
                {
                    sum = 0.0F;
                    for (int k = 0; k < layers[i + 1]; k++)
                        sum = sum + error[i + 1, k] * weight[i + 1, k, j];
                    error[i, j] = (float)(sigmoid_derivative(node_output[i, j]) * sum);
                }
        }
        public float get_average_error()
        {
            float average_error = 0.0F;
            for (int i = 0; i < number_of_output_nodes; i++)
                average_error = average_error + error[number_of_layers - 1, i];
            average_error = average_error / number_of_output_nodes;
            return Math.Abs(average_error);
        }

        public void calculate_weights()
        {
            for (int i = 1; i < number_of_layers; i++)
                for (int j = 0; j < layers[i]; j++)
                    for (int k = 0; k < layers[i - 1]; k++)
                    {
                        weight[i, j, k] = (float)(weight[i, j, k] + learning_rate * error[i, j] * node_output[i - 1, k]);
                    }
        }
        public void save_network(SaveFileDialog save)
        {
            save.Filter = "Artificial Neural Network Files (*.net)|*.net";
            if ((save.ShowDialog() == DialogResult.OK))
            {
                if (save.FileName != "")
                {
                    network_save_file_stream = new StreamWriter(save.FileName);
                    network_save_file_stream.WriteLine("Weight values");
                    network_save_file_stream.WriteLine("Hidden Layer Size	= " + maximum_layers.ToString());
                    network_save_file_stream.WriteLine("Number of Epochs	= " + epochs.ToString());
                    network_save_file_stream.WriteLine("Learning Rate	= " + learning_rate.ToString());
                    network_save_file_stream.WriteLine("Sigmoid Slope	= " + slope.ToString());
                    network_save_file_stream.WriteLine("Weight Bias	= " + weight_bias.ToString());
                    network_save_file_stream.WriteLine("Thresold Error= " + error_threshold.ToString());
                    network_save_file_stream.WriteLine("");
                    for (int i = 1; i < number_of_layers; i++)
                        for (int j = 0; j < layers[i]; j++)
                            for (int k = 0; k < layers[i - 1]; k++)
                            {
                                network_save_file_stream.Write("Weight[" + i.ToString() + " , " + j.ToString() + " , " + k.ToString() + "] = ");
                                network_save_file_stream.WriteLine(weight[i, j, k]);

                            }
                    network_save_file_stream.Close();
                }
            }
        }
        public void load_network(OpenFileDialog open)
        {
            form_network();
            open.Filter = "Neural Network Files (*.net)|*.net";
            string line;
            char[] weight_char = new char[20];
            string weight_text = "";
            int title_length, weight_length;
            if ((open.ShowDialog() == DialogResult.OK))
            {
                if (open.FileName != "")
                {
                    network_load_file_stream = new StreamReader(open.FileName);
                    for (int i = 0; i < 8; i++)
                        network_load_file_stream.ReadLine();
                    for (int i = 1; i < number_of_layers; i++)
                        for (int j = 0; j < layers[i]; j++)
                            for (int k = 0; k < layers[i - 1]; k++)
                            {
                                weight_text = "";
                                line = network_load_file_stream.ReadLine();
                                title_length = ("Weight[" + i.ToString() + " , " + j.ToString() + " , " + k.ToString() + "] = ").Length;
                                weight_length = line.Length - title_length;
                                line.CopyTo(title_length, weight_char, 0, weight_length);
                                for (int counter = 0; counter < weight_length; counter++)
                                    weight_text = weight_text + weight_char[counter].ToString();
                                weight[i, j, k] = (float)Convert.ChangeType(weight_text, typeof(float));
                            }
                    network_load_file_stream.Close();
                }
            }
        }
        public void load_network()
        {
            form_network();
            string line;
            char[] weight_char = new char[20];
            string weight_text = "";
            int title_length, weight_length;
            network_load_file_stream = new StreamReader(Application.StartupPath+"\\Data\\"+"data.net");
            for (int i = 0; i < 8; i++)
                   network_load_file_stream.ReadLine();
                   for (int i = 1; i < number_of_layers; i++)
                        for (int j = 0; j < layers[i]; j++)
                            for (int k = 0; k < layers[i - 1]; k++)
                            {
                                weight_text = "";
                                line = network_load_file_stream.ReadLine();
                                title_length = ("Weight[" + i.ToString() + " , " + j.ToString() + " , " + k.ToString() + "] = ").Length;
                                weight_length = line.Length - title_length;
                                line.CopyTo(title_length, weight_char, 0, weight_length);
                                for (int counter = 0; counter < weight_length; counter++)
                                    weight_text = weight_text + weight_char[counter].ToString();
                                weight[i, j, k] = (float)Convert.ChangeType(weight_text, typeof(float));
                            }
                    network_load_file_stream.Close();
                
            
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        public void character_to_ascii(string character)
        {
            int byteCount = ascii.GetByteCount(character.ToCharArray());
            byte[] bytes = new Byte[byteCount];
            bytes = ascii.GetBytes(character);
            BitArray bits = new BitArray(bytes);
            System.Collections.IEnumerator bit_enumerator = bits.GetEnumerator();
            //int bit_array_length = bits.Length;
            bit_enumerator.Reset();
            for (int i = 0; i < 7; i++)
            {
                bit_enumerator.MoveNext();
                if (bit_enumerator.Current.ToString() == "True")
                    desired_output_bit[i] = 1;
                else
                    desired_output_bit[i] = 0;

            }
        }
        public char ascii_to_character()
        {
            int dec = binary_to_decimal();
            Byte[] bytes = new Byte[1];
            bytes[0] = (byte)(dec);
            int charCount = ascii.GetCharCount(bytes);
            char[] chars = new Char[charCount];
            chars = ascii.GetChars(bytes);
            return chars[0];
        }
        public int binary_to_decimal()
        {
            int dec = 0;
            for (int i = 0; i < number_of_output_nodes; i++)
                dec = dec + output_bit[i] * (int)(Math.Pow(2, i));
            return dec;
        }


    
    }
}
