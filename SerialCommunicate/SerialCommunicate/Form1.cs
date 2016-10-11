using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.ComponentModel;
using System.Data;
using System.Drawing;


namespace SerialCommunicate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;//Port Name.
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text,10);//Baud Rate. Decimalism to Hex.
                serialPort1.Open();//Open Port
                button1.Enabled = false;//When open the port, disable the button.
                button2.Enabled = true;//When open the port, Enable the close port button.
            }
            catch {//if try failed, catch the following
                MessageBox.Show("Port Error, Plase Check Port", "Error");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < 10; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());//Add Port Number 
            }
            comboBox1.Text = "COM1";//Port Number Default Number 
            comboBox2.Text = "9600";//Default Baud Rate
            
            /*****************Important************************/
            
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//Must add Event Handeler Manually

        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)//Port receive data event
        {
            if (!radioButton3.Checked)//If receives as ASCII
            {
                string str = serialPort1.ReadExisting();//Receive as string
                textBox1.AppendText(str);//Adding to receive area
            }
            else { //If reveice as value
                byte data;
                data = (byte)serialPort1.ReadByte();//Transfer (int) to (byte)
                string str = Convert.ToString(data, 16).ToUpper();//Transfer to Hex, Capitals string
                textBox1.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//Fill in empty with 0  
                //Last command equals to：if(str.Length == 1)
                //                              str = "0" + str;
                //                        else 
                //                              str = str;
                //                        textBox1.AppendText("0x" + str);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();//Close Port
                button1.Enabled = true;//Enable Open Port Button
                button2.Enabled = false;//Disable Close Port Button
            }
            catch (Exception err)//In general case, close port won't have problems.So there's no program.
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];//Single "Byte" Operation
            if (serialPort1.IsOpen)//Check whether the port is open, if it's open, finish the following opertaion
            {
                if (textBox2.Text != "")//Check whether the sending area is empty, if is't not, finish the following opertaion
                {
                    if (!radioButton1.Checked)//If not send Value, then send ASCII
                    { 
                        try
                        {
                            serialPort1.WriteLine(textBox2.Text);//Send out ASCII(based on sending area)
                        }
                        catch (Exception err)//If there's error
                        {
                            MessageBox.Show("Writing Error", "Error");//Error Indication
                            serialPort1.Close();//Close Port
                            button1.Enabled = true;//Enable Open Port Button
                            button2.Enabled = false;//Disable Close Port Button
                        }
                    }
                    else//Sending Data
                    {
                        for (int i = 0; i < (textBox2.Text.Length - textBox2.Text.Length % 2) / 2; i++)//取余3运算作用是防止用户输入的字符为奇数个
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(Data, 0, 1);//Offest 0, sending 1 byte.(If input is 0A0BB, then send 0A0B）
                        }
                        if (textBox2.Text.Length % 2 != 0)//If length is odd, then operate the last character.
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(textBox2.Text.Length-1, 1), 16);//The last B will be sent as（0B）
                            serialPort1.Write(Data, 0, 1);//Send
                        }
                   }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open_fd = new OpenFileDialog();
                open_fd.Filter = "TXTfile|*.txt";
                textBox2.Text = File.ReadAllText(open_fd.FileName);
            }
            catch
            {
                MessageBox.Show("File Error, Plase Check File", "Error");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)// Clean Receive Data
        {
            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
