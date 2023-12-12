using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class EditGuestForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private TextBox txtGuestName;
        private TextBox txtRoomNumber;
        private Button btnUpdate;

        private int guestId; // Идентификатор гостя

        public EditGuestForm(int guestId)
        {
            this.guestId = guestId;
            InitializeComponent();
            InitializeComponents();
            LoadGuestData();
        }

        private void InitializeComponents()
        {
            txtGuestName = new TextBox();
            txtRoomNumber = new TextBox();
            btnUpdate = new Button();

            txtGuestName.Location = new System.Drawing.Point(120, 50);
            txtGuestName.Size = new System.Drawing.Size(150, 20);

            txtRoomNumber.Location = new System.Drawing.Point(120, 80);
            txtRoomNumber.Size = new System.Drawing.Size(150, 20);

            btnUpdate.Location = new System.Drawing.Point(120, 110);
            btnUpdate.Size = new System.Drawing.Size(150, 30);
            btnUpdate.Text = "Обновить данные";
            btnUpdate.Click += new EventHandler(btnUpdate_Click);

            this.Controls.Add(txtGuestName);
            this.Controls.Add(txtRoomNumber);
            this.Controls.Add(btnUpdate);
        }

        private void LoadGuestData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT guest_name, room_number FROM guests WHERE id = {guestId}", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtGuestName.Text = reader["guest_name"].ToString();
                            txtRoomNumber.Text = reader["room_number"].ToString();
                        }
                    }
                }
            }
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string updatedGuestName = txtGuestName.Text;
            string updatedRoomNumber = txtRoomNumber.Text;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Получаем текущий номер комнаты гостя
                int currentRoomNumber;
                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT room_number FROM guests WHERE id = {guestId}", connection))
                {
                    currentRoomNumber = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE guests SET guest_name = '{updatedGuestName}', room_number = '{updatedRoomNumber}' WHERE id = {guestId}", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные гостя успешно обновлены.");

                        // Обновляем статус предыдущей комнаты на "свободен"
                        UpdateRoomStatus(currentRoomNumber, "Свободен");

                        // Обновляем статус новой комнаты на "занят"
                        UpdateRoomStatus(Convert.ToInt32(updatedRoomNumber), "занят");

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при обновлении данных гостя. Пожалуйста, повторите попытку.");
                    }
                }
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
