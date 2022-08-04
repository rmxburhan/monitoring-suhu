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
    public partial class ProfileControl : UserControl
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader dataReader;
        string oldPassword;
        public ProfileControl()
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
        void get_user_data()
        {
            Koneksi();
            try
            {
                command = new SqlCommand($"SELECT TOP 1 * FROM tbl_user WHERE id = '{LoginStatus.id_user}'", connection);
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        txtNama.Text = dataReader["nama"].ToString();
                        if (dataReader["jenis_kelamin"].ToString() == "lk")
                        {
                            rbLk.Checked = true;
                            rbPr.Checked = false;
                        }
                        else if (dataReader["jenis_kelamin"].ToString() == "pr")
                        {
                            rbLk.Checked = false;
                            rbPr.Checked = true;
                        }
                        txtAlamat.Text = dataReader["alamat"].ToString();
                        txtUsername.Text = dataReader["username"].ToString();
                        oldPassword = dataReader["password"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            panel2.Enabled = true;
        }

        private void ProfileControl_Load(object sender, EventArgs e)
        {
            get_user_data();
            panel2.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Koneksi();
            if (txtNama.Text != "" && txtAlamat.Text != "" && txtUsername.Text != "" && (rbLk.Checked || rbPr.Checked))
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
                    using (command = new SqlCommand($"UPDATE tbl_user SET nama = '{txtNama.Text}', jenis_kelamin = '{jk}', alamat = '{txtAlamat.Text}', username = '{txtUsername.Text}' WHERE id = '{LoginStatus.id_user}'", connection))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("update data berhasil");
                        LoginStatus.username = txtUsername.Text;
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
            }
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Koneksi();
            if (txtOldPassword.Text != "" && txtPassword.Text != "")
            {
                if (txtOldPassword.Text == oldPassword)
                {
                    try
                    {
                        using (command = new SqlCommand($"UPDATE tbl_user SET password = '{txtPassword.Text}' WHERE id = '{LoginStatus.id_user}'", connection))
                        {
                            command.ExecuteNonQuery();
                            panel2.Enabled = false;
                            txtOldPassword.Clear();
                            txtPassword.Clear();
                            MessageBox.Show("update password berhasil");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("old password salah", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Isi semua data", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            connection.Close();
        }
    }
}
