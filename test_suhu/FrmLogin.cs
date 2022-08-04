using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace test_suhu
{
    public partial class FrmLogin : Form
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader dataReader;
        public FrmLogin()
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
            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Koneksi();
            if (txtUsername.Text != "" && txtPassword.Text != "")
            {
                using (command = new SqlCommand($"SELECT TOP 1 * FROM tbl_user WHERE username = '{txtUsername.Text}' AND password = '{txtPassword.Text}'", connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            LoginStatus.id_user = dataReader["id"].ToString();
                            LoginStatus.nama_user = dataReader["nama"].ToString();
                            LoginStatus.username = dataReader["username"].ToString();
                        }
                        FrmHome frm = new FrmHome();
                        this.Hide();
                        frm.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Username atau password salah", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Clear();
                    }
                }
            }
            else
            {
                MessageBox.Show("Field kolom username dan password tidak boleh kosong", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            connection.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            FrmRegister frm = new FrmRegister();
            this.Hide();
            frm.ShowDialog();
            this.Close();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
