using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class DeleteWorkerConfirmationForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private Button btnConfirmDelete;

        private int workerId; // Идентификатор работника

        public DeleteWorkerConfirmationForm(int workerId)
        {
            this.workerId = workerId;
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            btnConfirmDelete = new Button();
            btnConfirmDelete.Location = new System.Drawing.Point(10, 10);
            btnConfirmDelete.Size = new System.Drawing.Size(150, 30);
            btnConfirmDelete.Text = "Подтвердить удаление";
            btnConfirmDelete.Click += new EventHandler(btnConfirmDelete_Click);

            this.Controls.Add(btnConfirmDelete);
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Получаем данные работника перед удалением
                string workerName;
                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT worker_name FROM workers WHERE id = {workerId}", connection))
                {
                    workerName = cmd.ExecuteScalar().ToString();
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand($"DELETE FROM workers WHERE id = {workerId}", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Работник {workerName} успешно удален.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при удалении работника. Пожалуйста, повторите попытку.");
                    }
                }
            }
        }
    }
}
