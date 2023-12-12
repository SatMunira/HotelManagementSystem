using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class AddWorkerForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private TextBox txtWorkerName;
        private Button btnAddWorker;

        public AddWorkerForm()
        {
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            txtWorkerName = new TextBox();
            btnAddWorker = new Button();

            txtWorkerName.Location = new System.Drawing.Point(120, 50);
            txtWorkerName.Size = new System.Drawing.Size(150, 20);

            btnAddWorker.Location = new System.Drawing.Point(120, 80);
            btnAddWorker.Size = new System.Drawing.Size(150, 30);
            btnAddWorker.Text = "Добавить работника";
            btnAddWorker.Click += new EventHandler(btnAddWorker_Click);

            this.Controls.Add(txtWorkerName);
            this.Controls.Add(btnAddWorker);
        }

         private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void btnAddWorker_Click(object sender, EventArgs e)
        {
            string workerName = txtWorkerName.Text;

            if (!string.IsNullOrWhiteSpace(workerName))
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO workers (worker_name) VALUES ('{workerName}')", connection))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Работник {workerName} успешно добавлен.");
                            // Устанавливаем результат диалога, чтобы сообщить родительской форме об успешном добавлении
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при добавлении работника. Пожалуйста, повторите попытку.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите имя работника.");
            }
        }
    }
}
