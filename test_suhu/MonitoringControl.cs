using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace test_suhu
{
    public partial class MonitoringControl : UserControl
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader dataReader;
        private SqlDataAdapter da;
        string dataIn;
        public MonitoringControl()
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
      

        private void cbxAlat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            load("all");
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
        private void MonitoringControl_Load(object sender, EventArgs e)
        {
            Load_Alat();
            serialPort1.PortName = "COM2";
            serialPort1.Open();
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

        private void serialPort1_DataReceived_1(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIn = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(ShowData));
        }
    }
}
