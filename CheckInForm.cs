using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class CheckInForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private TextBox txtGuestName;
        private ComboBox cmbRoomNumber;
        private Button btnCheckIn;

        public CheckInForm()
        {
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void InitializeComponents()
        {
            txtGuestName = new TextBox();
            cmbRoomNumber = new ComboBox();
            btnCheckIn = new Button();

            txtGuestName.Location = new System.Drawing.Point(120, 50);
            txtGuestName.Size = new System.Drawing.Size(150, 20);

            cmbRoomNumber.Location = new System.Drawing.Point(120, 80);
            cmbRoomNumber.Size = new System.Drawing.Size(150, 20);
            cmbRoomNumber.DropDownStyle = ComboBoxStyle.DropDownList;

            btnCheckIn.Location = new System.Drawing.Point(120, 110);
            btnCheckIn.Size = new System.Drawing.Size(150, 30);
            btnCheckIn.Text = "Заселить";
            btnCheckIn.Click += new EventHandler(btnCheckIn_Click);

            this.Controls.Add(txtGuestName);
            this.Controls.Add(cmbRoomNumber);
            this.Controls.Add(btnCheckIn);

            // Заполняем ComboBox номерами комнат из базы данных
            LoadRoomNumbers();
        }

        private void LoadRoomNumbers()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT room_number FROM rooms WHERE status = 'Свободен'", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbRoomNumber.Items.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            string guestName = txtGuestName.Text;
            int roomNumber;

            if (int.TryParse(cmbRoomNumber.SelectedItem.ToString(), out roomNumber))
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO guests (guest_name, room_number, status) VALUES ('{guestName}', {roomNumber}, 'занят')", connection))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Обновляем статус комнаты
                            UpdateRoomStatus(roomNumber, "занят");

                            MessageBox.Show($"Гость {guestName} успешно заселен в комнату {roomNumber}.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при заселении гостя. Пожалуйста, повторите попытку.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите номер комнаты.");
            }
        }

        private void UpdateRoomStatus(int roomNumber, string status)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE rooms SET status = '{status}' WHERE room_number = {roomNumber}", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
