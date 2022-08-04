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
    public partial class ReportControl : UserControl
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader dataReader;
        private SqlDataAdapter da;
        public ReportControl()
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
        void load()
        {
            Koneksi();
            try
            {
                command = new SqlCommand($"SELECT * FROM tbl_monitoring WHERE id_alat = '{cbxAlat.SelectedValue} ' AND waktu between '{dtpFrom.Value.ToString("yyyy-MM-dd")}' AND '{dateTimePicker2.Value.ToString("yyyy-MM-dd")} 23:59:59'", connection);
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
            catch (Exception ez)
            {
                MessageBox.Show(ez.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }
        private void ReportControl_Load(object sender, EventArgs e)
        {
            Load_Alat();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            load();
        }
    }
}
