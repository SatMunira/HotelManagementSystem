using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class ManagerForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private DataGridView dgvGuests;
        private Button btnCheckIn;
        private DataGridView dgvWorkers;
        private Button btnAddWorker;
        private Button btnEditWorker;
        private Button btnDeleteWorker;

        public ManagerForm()
        {
            InitializeComponent();
            InitializeComponents();
            LoadGuests();
            LoadWorkers();

        }

        private void InitializeComponents()
        {
            dgvGuests = new DataGridView();
            btnCheckIn = new Button();

            dgvGuests.Location = new System.Drawing.Point(10, 10);
            dgvGuests.Size = new System.Drawing.Size(400, 200);

            btnCheckIn.Location = new System.Drawing.Point(10, 220);
            btnCheckIn.Size = new System.Drawing.Size(150, 30);
            btnCheckIn.Text = "Заселить гостя";
            btnCheckIn.Click += new EventHandler(btnCheckIn_Click);

            Button btnEditGuest = new Button();
            btnEditGuest.Location = new System.Drawing.Point(180, 220);
            btnEditGuest.Size = new System.Drawing.Size(150, 30);
            btnEditGuest.Text = "Изменить данные гостя";
            btnEditGuest.Click += new EventHandler(btnEditGuest_Click);

            Button btnDeleteGuest = new Button();
            btnDeleteGuest.Location = new System.Drawing.Point(340, 220);
            btnDeleteGuest.Size = new System.Drawing.Size(150, 30);
            btnDeleteGuest.Text = "Удалить гостя";
            btnDeleteGuest.Click += new EventHandler(btnDeleteGuest_Click);

            dgvWorkers = new DataGridView();
            dgvWorkers.Location = new System.Drawing.Point(20, 270);
            dgvWorkers.Size = new System.Drawing.Size(500, 150);
            dgvWorkers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvWorkers.AllowUserToAddRows = false;
            dgvWorkers.ReadOnly = true;
            dgvWorkers.AllowUserToDeleteRows = false;

            btnAddWorker = new Button();
            btnAddWorker.Location = new System.Drawing.Point(20, 430);
            btnAddWorker.Size = new System.Drawing.Size(150, 30);
            btnAddWorker.Text = "Добавить работника";
            btnAddWorker.Click += new EventHandler(btnAddWorker_Click);

            btnEditWorker = new Button();
            btnEditWorker.Location = new System.Drawing.Point(180, 430);
            btnEditWorker.Size = new System.Drawing.Size(150, 30);
            btnEditWorker.Text = "Изменить данные работника";
            btnEditWorker.Click += new EventHandler(btnEditWorker_Click);

            btnDeleteWorker = new Button();
            btnDeleteWorker.Location = new System.Drawing.Point(340, 430);
            btnDeleteWorker.Size = new System.Drawing.Size(150, 30);
            btnDeleteWorker.Text = "Удалить работника";
            btnDeleteWorker.Click += new EventHandler(btnDeleteWorker_Click);

            this.Controls.Add(dgvWorkers);
            this.Controls.Add(btnAddWorker);
            this.Controls.Add(btnEditWorker);
            this.Controls.Add(btnDeleteWorker);

            this.Controls.Add(btnEditGuest);
            this.Controls.Add(btnDeleteGuest);

            this.Controls.Add(dgvGuests);
            this.Controls.Add(btnCheckIn);
        }

        private void LoadWorkers()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, worker_name, status FROM workers", connection))
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvWorkers.DataSource = dataTable;
                }
            }
        }

        private void LoadGuests()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM guests", connection))
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvGuests.DataSource = dataTable;
                }
            }
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            CheckInForm checkInForm = new CheckInForm();
            checkInForm.ShowDialog();

            // После заселения обновляем список гостей
            LoadGuests();
        }

        private void btnEditGuest_Click(object sender, EventArgs e)
        {
            if (dgvGuests.SelectedRows.Count > 0)
            {
                int guestId = Convert.ToInt32(dgvGuests.SelectedRows[0].Cells["id"].Value);
                EditGuestForm editGuestForm = new EditGuestForm(guestId);
                editGuestForm.ShowDialog();

                // После редактирования обновляем список гостей
                LoadGuests();
            }
            else
            {
                MessageBox.Show("Выберите гостя для редактирования.");
            }
        }

        private void btnDeleteGuest_Click(object sender, EventArgs e)
        {
            if (dgvGuests.SelectedRows.Count > 0)
            {
                int guestId = Convert.ToInt32(dgvGuests.SelectedRows[0].Cells["id"].Value);
                DeleteGuestConfirmationForm deleteConfirmationForm = new DeleteGuestConfirmationForm(guestId);
                deleteConfirmationForm.ShowDialog();

                // После удаления обновляем список гостей
                LoadGuests();
            }
            else
            {
                MessageBox.Show("Выберите гостя для удаления.");
            }
        }

        private void btnAddWorker_Click(object sender, EventArgs e)
        {
            // Создаем экземпляр формы для добавления работника
            AddWorkerForm addWorkerForm = new AddWorkerForm();

            // Открываем форму в диалоговом режиме
            DialogResult result = addWorkerForm.ShowDialog();

            // После закрытия формы, проверяем результат
            if (result == DialogResult.OK)
            {
                // Если результат OK, то обновляем таблицу работников
                LoadWorkers();
            }
        }


        private void btnEditWorker_Click(object sender, EventArgs e)
        {
            if (dgvWorkers.SelectedRows.Count > 0)
            {
                int workerId = Convert.ToInt32(dgvWorkers.SelectedRows[0].Cells["id"].Value);
                EditWorkerForm editWorkerForm = new EditWorkerForm(workerId);
                editWorkerForm.ShowDialog();

                // После редактирования обновите таблицу работников вызовом LoadWorkers()
            }
            else
            {
                MessageBox.Show("Выберите работника для редактирования.");
            }
        }

        private void btnDeleteWorker_Click(object sender, EventArgs e)
        {
            if (dgvWorkers.SelectedRows.Count > 0)
            {
                int workerId = Convert.ToInt32(dgvWorkers.SelectedRows[0].Cells["id"].Value);
                DeleteWorkerConfirmationForm deleteConfirmationForm = new DeleteWorkerConfirmationForm(workerId);
                deleteConfirmationForm.ShowDialog();

                // После удаления обновите таблицу работников вызовом LoadWorkers()
            }
            else
            {
                MessageBox.Show("Выберите работника для удаления.");
            }
        }

    }
}
