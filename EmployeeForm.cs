using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class EmployeeForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private DataGridView dgvGuests;
        private Button btnSelectGuest;
        private string employeeUsername;
        private Button btnServiceCompleted;

        public EmployeeForm(string username)
        {
            InitializeComponent();
            employeeUsername = username;
            InitializeComponents();
            LoadGuestsWantsServices();
        }

        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void InitializeComponents()
        {
            dgvGuests = new DataGridView();
            btnSelectGuest = new Button();

            dgvGuests.Location = new System.Drawing.Point(50, 50);
            dgvGuests.Size = new System.Drawing.Size(400, 200);
            dgvGuests.ReadOnly = true;
            dgvGuests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGuests.CellClick += new DataGridViewCellEventHandler(dgvGuests_CellClick);

            btnSelectGuest.Location = new System.Drawing.Point(50, 270);
            btnSelectGuest.Size = new System.Drawing.Size(150, 30);
            btnSelectGuest.Text = "Выбрать гостя";
            btnSelectGuest.Click += new EventHandler(btnSelectGuest_Click);

            btnServiceCompleted = new Button();
            btnServiceCompleted.Location = new System.Drawing.Point(250, 270);
            btnServiceCompleted.Size = new System.Drawing.Size(150, 30);
            btnServiceCompleted.Text = "Обслужено";
            btnServiceCompleted.Click += new EventHandler(btnServiceCompleted_Click);

            this.Controls.Add(btnServiceCompleted);


            this.Controls.Add(dgvGuests);
            this.Controls.Add(btnSelectGuest);
        }

        private bool MarkServiceCompleted(string workerName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE workers SET status = 'свободен' WHERE worker_name = '{workerName}'", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        private void btnServiceCompleted_Click(object sender, EventArgs e)

        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                if (IsWorkerBusy(employeeUsername, connection))
                {
                    // Получите информацию о госте, которого обслуживает работник
                    string guestName = GetGuestNameServicedByWorker(employeeUsername, connection);

                    if (!string.IsNullOrEmpty(guestName))
                    {
                        // Установите статус обслуживаемого гостя как обслуженный
                        UpdateGuestServiceStatus(guestName, connection);

                        // Установите статус работника как свободный
                        SetWorkerStatusFree(employeeUsername, connection);

                        MessageBox.Show($"Обслуживание для гостя {guestName} завершено. Работник {employeeUsername} свободен.");

                        // Обновите DataGridView и добавьте номер гостя в строку обслуживаемых номеров
                        LoadGuestsWantsServices();
                    }
                    else
                    {
                        MessageBox.Show("Работник не обслуживает ни одного гостя.");
                    }
                }
                else
                {
                    MessageBox.Show("Работник свободен и не обслуживает ни одного гостя.");
                }
            }
        }

        private string GetGuestNameServicedByWorker(string workerName, NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT guest_name FROM guests WHERE worker_name = '{workerName}'", connection))
            {
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        private void UpdateGuestServiceStatus(string guestName, NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE guests SET wants_services = false WHERE guest_name = '{guestName}'", connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        private void SetWorkerStatusFree(string workerName, NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE workers SET status = 'свободен' WHERE worker_name = '{workerName}'", connection))
            {
                cmd.ExecuteNonQuery();
            }
        }




        private void LoadGuestsWantsServices()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                if (IsWorkerBusy(employeeUsername, connection))
                {
                    // Если работник занят, покажем только обслуживаемый им номер
                    LoadGuestsForBusyWorker(connection, employeeUsername);
                }
                else
                {
                    // Если работник свободен, покажем гостей, которые хотят сервиса и не назначены работнику
                    LoadGuestsForFreeWorker(connection);
                }
            }
        }

        private bool IsWorkerBusy(string workerName, NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT status FROM workers WHERE worker_name = '{workerName}'", connection))
            {
                string status = cmd.ExecuteScalar()?.ToString();
                return status == "занят";
            }
        }


        private void LoadGuestsForBusyWorker(NpgsqlConnection connection, string workerName)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT guest_name, room_number FROM guests WHERE worker_name = '{workerName}'", connection))
            {
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvGuests.DataSource = dt;
                }
            }
        }

        private void LoadGuestsForFreeWorker(NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT guest_name, room_number FROM guests WHERE wants_services = true AND worker_name IS NULL", connection))
            {
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvGuests.DataSource = dt;
                }
            }
        }

        private void btnSelectGuest_Click(object sender, EventArgs e)
        {
            if (dgvGuests.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvGuests.SelectedRows[0];

                string guestName = selectedRow.Cells["guest_name"].Value.ToString();
                int roomNumber = Convert.ToInt32(selectedRow.Cells["room_number"].Value);

                if (SetGuestStatusAndServiceRoom(guestName, roomNumber, employeeUsername))
                {
                    MessageBox.Show($"Гость {guestName} выбран и его статус установлен как 'занят'. Обслуживаемый номер: {roomNumber}.");
                    LoadGuestsWantsServices(); // Обновите DataGridView и добавьте номер гостя в строку обслуживаемых номеров
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при выборе гостя. Пожалуйста, повторите попытку.");
                }
            }
            else
            {
                MessageBox.Show("Выберите гостя из списка.");
            }
        }

        private void dgvGuests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvGuests.Rows.Count)
            {
                DataGridViewRow selectedRow = dgvGuests.Rows[e.RowIndex];

                string guestName = selectedRow.Cells["guest_name"].Value.ToString();
                int roomNumber = Convert.ToInt32(selectedRow.Cells["room_number"].Value);

                MessageBox.Show($"Выбран гость {guestName}. Номер комнаты: {roomNumber}");

                // Меняем статус работника на "занят"
                SetWorkerStatusBusy(employeeUsername);

                // Дополнительная логика, если необходимо
            }
        }


        private bool SetGuestStatusAndServiceRoom(string guestName, int roomNumber, string workerName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE guests SET status = 'занят', worker_name = '{workerName}' WHERE guest_name = '{guestName}' AND room_number = {roomNumber}", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }


        private bool SetWorkerStatusBusy(string workerName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmdCheck = new NpgsqlCommand($"SELECT COUNT(*) FROM workers WHERE worker_name = '{workerName}' AND status = 'свободен'", connection))
                {
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        using (NpgsqlCommand cmdUpdate = new NpgsqlCommand($"UPDATE workers SET status = 'занят' WHERE worker_name = '{workerName}'", connection))
                        {
                            int rowsAffected = cmdUpdate.ExecuteNonQuery();

                            return rowsAffected > 0;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Работник {workerName} уже занят или отсутствует в системе.");
                        return false;
                    }
                }
            }
        }
    }
}
