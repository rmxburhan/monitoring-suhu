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
    public partial class FrmRegister : Form
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader dataReader;
        public FrmRegister()
        {
            InitializeComponent();
        }
        void Koneksi()
        {
            connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=db_test_suhu;Integrated Security=True");
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        void Clear()
        {
            txtNama.Clear();
            rbLk.Checked = false;
            rbPr.Checked = false;
            txtAlamat.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void FrmRegister_Load(object sender, EventArgs e)
        {
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Koneksi();
            if (txtNama.Text != "" && txtAlamat.Text != "" && txtUsername.Text != "" && txtPassword.Text != "" && (rbLk.Checked || rbPr.Checked))
            {
                try
                {
                    string jk = "";
                    if (rbLk.Checked)
                    {
                        jk = "lk";
                    }
                    else if (rbPr.Checked)
                    {
                        jk = "pr";
                    }
                    using (command = new SqlCommand($"INSERT INTO tbl_user values('{txtNama.Text}', '{jk}', '{txtAlamat.Text}', '{txtUsername.Text}', '{txtPassword.Text}')",connection))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Registrasi berhasil");
                        FrmLogin frm = new FrmLogin();
                        this.Hide();
                        frm.ShowDialog();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } 
            else
            {
                MessageBox.Show("Isi semua data", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clear();
            }
            connection.Close();
        }
    }
}
