using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class GuestRoomForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private Label lblRoomInfo;

        public GuestRoomForm(string guestName)
        {
            InitializeComponent();
            InitializeComponents(guestName);
        }

        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void InitializeComponents(string guestName)
        {
            lblRoomInfo = new Label();

            lblRoomInfo.Location = new System.Drawing.Point(50, 50);
            lblRoomInfo.Size = new System.Drawing.Size(300, 50);
            lblRoomInfo.Text = GetGuestRoomInfo(guestName);

            this.Controls.Add(lblRoomInfo);
        }

        private string GetGuestRoomInfo(string guestName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT room_number FROM guests WHERE guest_name = '{guestName}'", connection))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int roomNumber = Convert.ToInt32(result);
                        return $"Добро пожаловать, {guestName}! Ваш номер комнаты: {roomNumber}";
                    }
                    else
                    {
                        return $"Ошибка: Гость {guestName} не найден в базе данных.";
                    }
                }
            }
        }
    }
}
