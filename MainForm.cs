using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
        public MainForm()
        {
            InitializeComponent();

            UserInitialization();
        }

      
        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);
            
            tbData.AppendText(str);
            
           
            string[] needo = tbData.Lines;
            if ((needo.Length - 2)>0)
            {
                String theString = needo[needo.Length - 2];
                string[] words = theString.Split(',', ':');

                if (words[0] == "ax")
                {
                    textBox4.Text = words[1];
                    textBox5.Text = words[3];
                    textBox6.Text = words[5];
                }

                if (words[0] == "gx")
                {
                    textBox7.Text = words[1];
                    textBox8.Text = words[3];
                    textBox9.Text = words[5];
                }

                if (words[0] == "mx")
                {
                    textBox10.Text = words[1];
                    textBox11.Text = words[3];
                    textBox12.Text = words[5];
                }

                if (words[0] == "Yaw" && words[2] == " Roll")
                {
                    chart1.Series["Series1"].Color = Color.Red;
                    chart1.Series["Series1"].BorderWidth = 3;
                    chart2.Series["Series1"].Color = Color.LightGreen;
                    chart2.Series["Series1"].BorderWidth = 3;




                    textBox1.Text = words[3];
                    textBox2.Text = words[4];
                    textBox3.Text = words[5];
                    //progressBar1.Value = Convert.ToInt16(words[5]);
                

                    chart1.ChartAreas["ChartArea1"].AxisY.Title = "Degrees";
                    chart1.Series["Series1"].Points.AddY(Convert.ToDouble(words[3]));

                    if (chart1.Series["Series1"].Points.Count.CompareTo(100) == 0)
                        chart1.Series["Series1"].Points.Clear();

                    chart2.ChartAreas["ChartArea1"].AxisY.Title = "Degrees";
                    chart2.Series["Series1"].Points.AddY(Convert.ToDouble(words[4]));

                    if (chart2.Series["Series1"].Points.Count.CompareTo(100) == 0)
                        chart2.Series["Series1"].Points.Clear();

                    chart4.ChartAreas["ChartArea1"].AxisY.Title = "Degrees";
                    chart4.Series["Series1"].Points.AddY(Convert.ToDouble(words[5]));

                    if (chart4.Series["Series1"].Points.Count.CompareTo(100) == 0)
                        chart4.Series["Series1"].Points.Clear();

                    
                    label23.Text = words[3];
                    label26.Text = words[4];
                    label22.Text = words[5];

                    if (Convert.ToInt16(Convert.ToSingle(words[3]))<0)
                        aGauge1.Value = (Convert.ToInt16(Convert.ToSingle(words[3])))  +360;
                    else
                        aGauge1.Value = (Convert.ToInt16(Convert.ToSingle(words[3])));

                    if (Convert.ToInt16(Convert.ToSingle(words[4])) < 0)
                        aGauge3.Value = (Convert.ToInt16(Convert.ToSingle(words[4]))) + 360;
                    else
                        aGauge3.Value = (Convert.ToInt16(Convert.ToSingle(words[4])));

                    if (Convert.ToInt16(Convert.ToSingle(words[5])) < 0)
                        aGauge2.Value = (Convert.ToInt16(Convert.ToSingle(words[5]))) + 360;
                    else
                        aGauge2.Value = (Convert.ToInt16(Convert.ToSingle(words[5])));
                    

                }


            }
            
            tbData.ScrollToCaret();

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void aGauge1_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }
    }
}
