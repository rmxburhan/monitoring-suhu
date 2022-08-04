using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_suhu
{
    public partial class FrmHome : Form
    {

        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader dataReader;
        private SqlDataAdapter da;
        string dataIn;
        public FrmHome()
        {
            InitializeComponent();
        }
        void Load_Alat()
        {
            Koneksi();
            try
            {
                command = new SqlCommand($"SELECT * FROM tbl_alat ORDER BY serial_number_alat ASC", connection);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbxAlat.DataSource = dt;
                cbxAlat.DisplayMember = "serial_number_alat";
                cbxAlat.ValueMember = "id";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }
        void Koneksi()
        {
            connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=db_test_suhu;Integrated Security=True");
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
       
     
        private void ShowData(object sender, EventArgs e)
        {
            //string[] splittedinput = dataIn.Split(',');
            //for (int i = 0; i < splittedinput.Length; i++)
            //{
            //    if (i == 0)
            //    {
            //        label1.Text = splittedinput[i].ToString();
            //    }
            //    if (i == 1)
            //    {
            //        label2.Text = splittedinput[i].ToString();
            //    }
            //}
            string[] splittedinput = dataIn.Split(',');
            try
            {
                Koneksi();
                using (command = new SqlCommand($"SELECT TOP 1 * FROM tbl_alat WHERE serial_number_alat = '{splittedinput[0]}'", connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        string id_alat = "";
                        while (dataReader.Read())
                        {
                            id_alat = dataReader["id"].ToString();
                        }
                        connection.Close();
                        Koneksi();
                        using (SqlCommand cmz = new SqlCommand($"INSERT INTO tbl_monitoring values('{id_alat}', '{splittedinput[1]}', '{splittedinput[2]}', '{DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss")}')", connection))
                        {
                            cmz.ExecuteNonQuery();
                            load("all");
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void load(string tipe)
        {
            Koneksi();
            try
            {
                if (tipe == "all")
                {
                    command = new SqlCommand("SELECT * FROM tbl_monitoring WHERE id_alat = '" + cbxAlat.SelectedValue + "'", connection);
                    da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    chart1.Series["suhu"].Points.Clear();
                    chart1.Series["kelembaban"].Points.Clear();
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            chart1.Series["suhu"].Points.AddXY(dataReader["waktu"].ToString(), int.Parse(dataReader["suhu"].ToString()));
                            chart1.Series["kelembaban"].Points.AddXY(dataReader["waktu"].ToString(), int.Parse(dataReader["kelembaban"].ToString()));
                        }
                    }
                }
                //else if (tipe == "search_chart")
                //{
                //    command = new SqlCommand($"SELECT * FROM tbl_monitoring WHERE id_alat = '{cbxAlat.SelectedValue} ' AND waktu between '{dtpFrom.Value.ToString("yyyy-MM-dd")}' AND '{dateTimePicker2.Value.ToString("yyyy-MM-dd")} 23:59:59'", connection);

                //    chart1.Series["suhu"].Points.Clear();
                //    chart1.Series["kelembaban"].Points.Clear();
                //    dataReader = command.ExecuteReader();
                //    if (dataReader.HasRows)
                //    {
                //        while (dataReader.Read())
                //        {
                //            chart1.Series["suhu"].Points.AddXY(dataReader["waktu"].ToString(), int.Parse(dataReader["suhu"].ToString()));
                //            chart1.Series["kelembaban"].Points.AddXY(dataReader["waktu"].ToString(), int.Parse(dataReader["kelembaban"].ToString()));
                //        }
                //    }
                //}
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        private void cbxAlat_SelectionChangeCommitted_1(object sender, EventArgs e)
        {
            load("all");
        }

        private void FrmHome_Load(object sender, EventArgs e)
        {
            Load_Alat();
            reportControl1.Hide();
            profileControl1.Hide();
            settingsControl1.Hide();
            connectPort();
            //serialPort1.PortName = "COM3";
            //serialPort1.Open();
        }
        void connectPort()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            string status = Properties.Settings.Default.portStatus;
            string port = Properties.Settings.Default.port;
            if (port == "")
            {
                reportControl1.Show();
                reportControl1.BringToFront();
                profileControl1.Hide();
                settingsControl1.Hide();
                MessageBox.Show("Silahkan tambah port terlebih dahulu");
            }
            else
            {
                if (status == "closed" || status == "")
                {
                    reportControl1.Hide();
                    profileControl1.Hide();
                    settingsControl1.Show();
                    settingsControl1.BringToFront();
                    MessageBox.Show("Port belum terkoneksi");
                }
                else if (status == "open")
                {
                    try
                    {
                        serialPort1.PortName = Properties.Settings.Default.port;
                        serialPort1.Open();
                        reportControl1.Hide();
                        profileControl1.Hide();
                        settingsControl1.Hide();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,"Silahkan pilih port terlebih dahulu di menu settings");
                    }
                }
            }
        }
        private void serialPort1_DataReceived_1(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIn = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(ShowData));
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginStatus.id_user = "";
            LoginStatus.nama_user = "";
            LoginStatus.username = "";
            FrmLogin frm = new FrmLogin();
            this.Hide();
            frm.ShowDialog();
            this.Close();
        }


        private void btnMonitoring_Click_1(object sender, EventArgs e)
        {
            connectPort();
         
        }

        private void btnLaporan_Click(object sender, EventArgs e)
        {
            reportControl1.Show();
            reportControl1.BringToFront();
            profileControl1.Hide();
            settingsControl1.Hide();
            serialPort1.Close();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            reportControl1.Hide();
            profileControl1.Show();
            profileControl1.BringToFront();
            settingsControl1.Hide();
            settingsControl1.Hide();
            serialPort1.Close();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            reportControl1.Hide();
            profileControl1.Hide();
            settingsControl1.Show();
            settingsControl1.BringToFront();
            serialPort1.Close();
        }

        private void settingsControl1_Load(object sender, EventArgs e)
        {

        }
    }
}