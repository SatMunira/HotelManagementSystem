using System;
using System.Windows.Forms;

namespace HotelManagementSystem
{
    public partial class NoGuestRoomForm : Form
    {
        private Label lblNoRoomInfo;

        public NoGuestRoomForm()
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
            lblNoRoomInfo = new Label();

            lblNoRoomInfo.Location = new System.Drawing.Point(50, 50);
            lblNoRoomInfo.Size = new System.Drawing.Size(300, 50);
            lblNoRoomInfo.Text = "Извините, у вас еще нет номера комнаты.";

            this.Controls.Add(lblNoRoomInfo);
        }
    }
}
