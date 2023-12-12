using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class EditWorkerForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private TextBox txtWorkerName;
        private Button btnUpdate;

        private int workerId; // Идентификатор работника

        public EditWorkerForm(int workerId)
        {
            this.workerId = workerId;
            InitializeComponent();
            InitializeComponents();
            LoadWorkerData();
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void InitializeComponents()
        {
            txtWorkerName = new TextBox();
            btnUpdate = new Button();

            txtWorkerName.Location = new System.Drawing.Point(120, 50);
            txtWorkerName.Size = new System.Drawing.Size(150, 20);

            btnUpdate.Location = new System.Drawing.Point(120, 80);
            btnUpdate.Size = new System.Drawing.Size(150, 30);
            btnUpdate.Text = "Обновить данные";
            btnUpdate.Click += new EventHandler(btnUpdate_Click);

            this.Controls.Add(txtWorkerName);
            this.Controls.Add(btnUpdate);
        }

        private void LoadWorkerData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT worker_name FROM workers WHERE id = {workerId}", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtWorkerName.Text = reader["worker_name"].ToString();
                        }
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string updatedWorkerName = txtWorkerName.Text;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE workers SET worker_name = '{updatedWorkerName}' WHERE id = {workerId}", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные работника успешно обновлены.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при обновлении данных работника. Пожалуйста, повторите попытку.");
                    }
                }
            }
        }
    }
}
