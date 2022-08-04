using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_suhu
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        private void SettingsControl_Load(object sender, EventArgs e)
        {
            string[] port = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(port);
            comboBox1.SelectedIndex = 0;
            label3.Text = "Port yang digunakan adalah : " + Properties.Settings.Default.port;
            if (Properties.Settings.Default.port != "")
            {
                serialPort1.PortName = Properties.Settings.Default.port;
                if (Properties.Settings.Default.portStatus == "open")
                {
                    btnConnect.Enabled = false;
                    btnDisconect.Enabled = true;
                }
                else if (Properties.Settings.Default.portStatus == "closed" || Properties.Settings.Default.portStatus == "")
                {
                    btnConnect.Enabled = true;
                    btnDisconect.Enabled = false;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.Open();
                Properties.Settings.Default.port = comboBox1.SelectedItem.ToString();
                Properties.Settings.Default.portStatus = "open";
                Properties.Settings.Default.Save();
                btnConnect.Enabled = false;
                btnDisconect.Enabled = true;
                label3.Text = "Port yang digunakan adalah : " + Properties.Settings.Default.port;
                serialPort1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Akses ditolak port sedang digunakan", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconect_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.portStatus = "closed";
            Properties.Settings.Default.Save();
            btnConnect.Enabled = true;
            btnDisconect.Enabled = false;
        }
    }
}
