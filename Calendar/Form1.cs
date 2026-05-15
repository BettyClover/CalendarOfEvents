using System;
using System.Drawing;
using System.Windows.Forms;

namespace Calendar
{
    public partial class Form1 : Form
    {
        private MonthCalendar calendar;
        private Panel eventPanel;
        private Label lblEventsTitle;
        private Label lblNoEvents;
        private ListBox eventList;
        private Button btnAddEvent;
        private Button btnDeleteEvent;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Календарь";
            this.Size = new Size(320, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Padding = new Padding(10);

            calendar = new MonthCalendar();
            calendar.Location = new Point(10, 10);
            calendar.Size = new Size(285, 180);
            calendar.Font = new Font("Segoe UI", 9);
            calendar.TitleBackColor = Color.FromArgb(45, 45, 48);
            calendar.TitleForeColor = Color.White;
            calendar.TrailingForeColor = Color.FromArgb(128, 128, 128);
            calendar.ForeColor = Color.White;
            calendar.BackColor = Color.FromArgb(32, 32, 32);
            calendar.DateSelected += OnDateSelected;
            calendar.DateChanged += OnDateChanged;

            //панель событий
            eventPanel = new Panel();
            eventPanel.Location = new Point(10, 200);
            eventPanel.Size = new Size(285, 235);
            eventPanel.BackColor = Color.FromArgb(45, 45, 48);
            eventPanel.BorderStyle = BorderStyle.FixedSingle;

            //заголовок событий
            lblEventsTitle = new Label();
            lblEventsTitle.Text = "Добавить событие или напоминание";
            lblEventsTitle.Location = new Point(8, 8);
            lblEventsTitle.Size = new Size(269, 20);
            lblEventsTitle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            lblEventsTitle.ForeColor = Color.FromArgb(180, 180, 180);

            //список
            eventList = new ListBox();
            eventList.Location = new Point(8, 75);
            eventList.Size = new Size(269, 80);
            eventList.Font = new Font("Segoe UI", 9);
            eventList.BackColor = Color.FromArgb(55, 55, 58);
            eventList.ForeColor = Color.White;
            eventList.BorderStyle = BorderStyle.FixedSingle;

            //"нет событий"
            lblNoEvents = new Label();
            lblNoEvents.Text = "Нет событий";
            lblNoEvents.Location = new Point(8, 100);
            lblNoEvents.Size = new Size(269, 20);
            lblNoEvents.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblNoEvents.ForeColor = Color.Gray;
            lblNoEvents.TextAlign = ContentAlignment.MiddleCenter;
            lblNoEvents.Visible = false;

            //кнопка добавления события
            btnAddEvent = new Button();
            btnAddEvent.Text = "Добавить событие";
            btnAddEvent.Location = new Point(8, 160);
            btnAddEvent.Size = new Size(269, 27);
            btnAddEvent.FlatStyle = FlatStyle.Standard;
            btnAddEvent.BackColor = Color.FromArgb(55, 55, 58);
            btnAddEvent.ForeColor = Color.White;
            btnAddEvent.FlatStyle = FlatStyle.Flat;
            btnAddEvent.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
            btnAddEvent.TextAlign = ContentAlignment.MiddleLeft;
            btnAddEvent.Click += OnAddEventClick;

            //кнопка удаления события
            btnDeleteEvent = new Button();
            btnDeleteEvent.Text = "Удалить событие";
            btnDeleteEvent.Location = new Point(8, 192);
            btnDeleteEvent.Size = new Size(269, 27);
            btnDeleteEvent.FlatStyle = FlatStyle.Standard;
            btnDeleteEvent.BackColor = Color.FromArgb(55, 55, 58);
            btnDeleteEvent.ForeColor = Color.White;
            btnDeleteEvent.FlatStyle = FlatStyle.Flat;
            btnDeleteEvent.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
            btnDeleteEvent.TextAlign = ContentAlignment.MiddleLeft;

            eventPanel.Controls.Add(lblEventsTitle);
            eventPanel.Controls.Add(eventList);
            eventPanel.Controls.Add(lblNoEvents);
            eventPanel.Controls.Add(btnAddEvent);
            eventPanel.Controls.Add(btnDeleteEvent);


            this.Controls.Add(calendar);
            this.Controls.Add(eventPanel);
        }

        private void OnDateSelected(object sender, DateRangeEventArgs e)
        {
            UpdateEventList();
        }

        private void OnDateChanged(object sender, DateRangeEventArgs e)
        {
            UpdateEventList();
        }

        private void UpdateEventList()
        {
            eventList.Items.Clear();

            if (eventList.Items.Count == 0)
            {
                lblNoEvents.Visible = true;
                eventList.Visible = false;
            }
            else
            {
                lblNoEvents.Visible = false;
                eventList.Visible = true;
            }
        }

        private void OnAddEventClick(object sender, EventArgs e)
        {
            Form dialog = new Form();
            dialog.Text = "Новое событие";
            dialog.Size = new Size(320, 140);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;
            dialog.BackColor = Color.FromArgb(45, 45, 48);
            dialog.ForeColor = Color.White;

            Label lblText = new Label();
            lblText.Text = "Введите событие:";
            lblText.Location = new Point(10, 10);
            lblText.Size = new Size(280, 20);
            lblText.ForeColor = Color.White;

            TextBox txtEvent = new TextBox();
            txtEvent.Location = new Point(10, 35);
            txtEvent.Size = new Size(280, 23);
            txtEvent.BackColor = Color.FromArgb(55, 55, 58);
            txtEvent.ForeColor = Color.White;
            txtEvent.BorderStyle = BorderStyle.FixedSingle;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(225, 70);
            btnOk.Size = new Size(65, 25);
            btnOk.BackColor = Color.FromArgb(55, 55, 58);
            btnOk.ForeColor = Color.White;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.DialogResult = DialogResult.OK;

            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(150, 70);
            btnCancel.Size = new Size(65, 25);
            btnCancel.BackColor = Color.FromArgb(55, 55, 58);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.DialogResult = DialogResult.Cancel;

            dialog.Controls.Add(lblText);
            dialog.Controls.Add(txtEvent);
            dialog.Controls.Add(btnOk);
            dialog.Controls.Add(btnCancel);

            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtEvent.Text))
            {
                eventList.Items.Add(txtEvent.Text);
                eventList.Visible = true;
                lblNoEvents.Visible = false;
                calendar.AddBoldedDate(calendar.SelectionStart);
                calendar.UpdateBoldedDates();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateEventList();
        }
    }
}