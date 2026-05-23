using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using static Calendar.Form1;

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
        private Button btnEditEvent;
        private Button btnClearDay;
        private Button btnShowAllEvents;
        private Label lblSelectedDate;
        private System.Windows.Forms.Timer reminderTimer;
        private NotifyIcon notifyIcon;
        private Button btnComplain;

        private Dictionary<DateTime, List<CalendarEvent>> eventsStorage;

        public class CalendarEvent
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime ReminderTime { get; set; }
            public bool HasReminder { get; set; }

            public override string ToString()
            {
                if (HasReminder)
                {
                    return $"{Title} ({ReminderTime.ToShortTimeString()})";
                }
                return Title ?? "Без названия.";
            }
        }

        public Form1()
        {
            eventsStorage = new Dictionary<DateTime, List<CalendarEvent>>();

            LoadEventsFromFile();

            this.Text = "Календарь событий.";
            this.Size = new Size(360, 580);
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

            lblSelectedDate = new Label();
            lblSelectedDate.Location = new Point(10, 195);
            lblSelectedDate.Size = new Size(285, 25);
            lblSelectedDate.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSelectedDate.ForeColor = Color.White;
            lblSelectedDate.TextAlign = ContentAlignment.MiddleCenter;
            lblSelectedDate.Text = DateTime.Now.ToLongDateString();

            eventPanel = new Panel();
            eventPanel.Location = new Point(10, 225);
            eventPanel.Size = new Size(325, 310);
            eventPanel.BackColor = Color.FromArgb(45, 45, 48);
            eventPanel.BorderStyle = BorderStyle.FixedSingle;

            lblEventsTitle = new Label();
            lblEventsTitle.Text = "События на выбранную дату.";
            lblEventsTitle.Location = new Point(8, 8);
            lblEventsTitle.Size = new Size(300, 20);
            lblEventsTitle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblEventsTitle.ForeColor = Color.FromArgb(180, 180, 180);

            eventList = new ListBox();
            eventList.Location = new Point(8, 35);
            eventList.Size = new Size(300, 120);
            eventList.Font = new Font("Segoe UI", 9);
            eventList.BackColor = Color.FromArgb(55, 55, 58);
            eventList.ForeColor = Color.White;
            eventList.BorderStyle = BorderStyle.FixedSingle;

            lblNoEvents = new Label();
            lblNoEvents.Text = "Нет событий на этот день.";
            lblNoEvents.Location = new Point(8, 80);
            lblNoEvents.Size = new Size(300, 20);
            lblNoEvents.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblNoEvents.ForeColor = Color.Gray;
            lblNoEvents.TextAlign = ContentAlignment.MiddleCenter;
            lblNoEvents.Visible = false;

            btnAddEvent = new Button();
            btnAddEvent.Text = "Добавить событие.";
            btnAddEvent.Location = new Point(8, 165);
            btnAddEvent.Size = new Size(145, 30);
            btnAddEvent.BackColor = Color.FromArgb(70, 130, 200);
            btnAddEvent.ForeColor = Color.White;
            btnAddEvent.FlatStyle = FlatStyle.Flat;
            btnAddEvent.Click += OnAddEventClick;

            btnEditEvent = new Button();
            btnEditEvent.Text = "Редактирование.";
            btnEditEvent.Location = new Point(160, 165);
            btnEditEvent.Size = new Size(145, 30);
            btnEditEvent.BackColor = Color.FromArgb(200, 160, 60);
            btnEditEvent.ForeColor = Color.White;
            btnEditEvent.FlatStyle = FlatStyle.Flat;
            btnEditEvent.Click += OnEditEventClick;

            btnDeleteEvent = new Button();
            btnDeleteEvent.Text = "Удалить событие.";
            btnDeleteEvent.Location = new Point(8, 200);
            btnDeleteEvent.Size = new Size(145, 30);
            btnDeleteEvent.BackColor = Color.FromArgb(180, 80, 80);
            btnDeleteEvent.ForeColor = Color.White;
            btnDeleteEvent.FlatStyle = FlatStyle.Flat;
            btnDeleteEvent.Click += OnDeleteEventClick;

            btnClearDay = new Button();
            btnClearDay.Text = "Очистить день.";
            btnClearDay.Location = new Point(160, 200);
            btnClearDay.Size = new Size(145, 30);
            btnClearDay.BackColor = Color.FromArgb(100, 100, 100);
            btnClearDay.ForeColor = Color.White;
            btnClearDay.FlatStyle = FlatStyle.Flat;
            btnClearDay.Click += OnClearDayClick;

            btnShowAllEvents = new Button();
            btnShowAllEvents.Text = "Все события.";
            btnShowAllEvents.Location = new Point(8, 240);
            btnShowAllEvents.Size = new Size(300, 30);
            btnShowAllEvents.BackColor = Color.FromArgb(75, 75, 80);
            btnShowAllEvents.ForeColor = Color.White;
            btnShowAllEvents.FlatStyle = FlatStyle.Flat;
            btnShowAllEvents.Click += OnShowAllEventsClick;

            btnComplain = new Button();
            btnComplain.Text = "Пожаловаться";
            btnComplain.Location = new Point(8, 275);
            btnComplain.Size = new Size(300, 30);
            btnComplain.BackColor = Color.FromArgb(75, 75, 80);
            btnComplain.ForeColor = Color.White;
            btnComplain.FlatStyle = FlatStyle.Flat;
            btnComplain.Click += OnComplainClick;

            eventPanel.Controls.Add(btnComplain);

            eventPanel.Controls.Add(lblEventsTitle);
            eventPanel.Controls.Add(eventList);
            eventPanel.Controls.Add(lblNoEvents);
            eventPanel.Controls.Add(btnAddEvent);
            eventPanel.Controls.Add(btnEditEvent);
            eventPanel.Controls.Add(btnDeleteEvent);
            eventPanel.Controls.Add(btnClearDay);
            eventPanel.Controls.Add(btnShowAllEvents);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Information;
            notifyIcon.Visible = true;
            notifyIcon.Text = "Календарь событий";
            notifyIcon.BalloonTipTitle = "Напоминание";

            reminderTimer = new System.Windows.Forms.Timer();
            reminderTimer.Interval = 60000;
            reminderTimer.Tick += CheckReminders;
            reminderTimer.Start();

            this.Controls.Add(calendar);
            this.Controls.Add(lblSelectedDate);
            this.Controls.Add(eventPanel);

            UpdateEventList();
            UpdateBoldedDates();
        }

        private void UpdateSelectedDateLabel()
        {
            lblSelectedDate.Text = calendar.SelectionStart.ToLongDateString();
        }

        private void UpdateEventList()
        {
            eventList.Items.Clear();
            DateTime selectedDate = calendar.SelectionStart.Date;

            if (eventsStorage.ContainsKey(selectedDate))
            {
                foreach (var ev in eventsStorage[selectedDate])
                {
                    eventList.Items.Add(ev);
                }
            }

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

        private void UpdateBoldedDates()
        {
            calendar.RemoveAllBoldedDates();
            foreach (var date in eventsStorage.Keys)
            {
                calendar.AddBoldedDate(date);
            }
            calendar.UpdateBoldedDates();
        }

        private void OnDateSelected(object? sender, DateRangeEventArgs e)
        {
            UpdateSelectedDateLabel();
            UpdateEventList();
        }

        private void OnDateChanged(object? sender, DateRangeEventArgs e)
        {
            UpdateSelectedDateLabel();
            UpdateEventList();
        }

        private void OnAddEventClick(object? sender, EventArgs e)
        {
            using (EventDialog dialog = new EventDialog(calendar.SelectionStart))
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.NewEvent != null)
                {
                    DateTime selectedDate = calendar.SelectionStart.Date;

                    if (!eventsStorage.ContainsKey(selectedDate))
                    {
                        eventsStorage[selectedDate] = new List<CalendarEvent>();
                    }

                    string funnyTitle = CheckForSixSevenEasterEgg(dialog.NewEvent.Title ?? "");
                    dialog.NewEvent.Title = funnyTitle;
                    eventsStorage[selectedDate].Add(dialog.NewEvent);
                    SaveEventsToFile();
                    UpdateEventList();
                    UpdateBoldedDates();
                }
            }
        }

        private void OnEditEventClick(object? sender, EventArgs e)
        {
            if (eventList.SelectedItem != null)
            {
                CalendarEvent selectedEvent = (CalendarEvent)eventList.SelectedItem;
                DateTime selectedDate = calendar.SelectionStart.Date;

                using (EventDialog dialog = new EventDialog(selectedDate, selectedEvent))
                {
                    if (dialog.ShowDialog() == DialogResult.OK && dialog.NewEvent != null)
                    {
                        int index = eventsStorage[selectedDate].IndexOf(selectedEvent);
                        if (index >= 0)
                        {
                            string funnyTitle = CheckForSixSevenEasterEgg(dialog.NewEvent.Title ?? "");
                            dialog.NewEvent.Title = funnyTitle;
                            eventsStorage[selectedDate][index] = dialog.NewEvent;
                            SaveEventsToFile();
                            UpdateEventList();
                            UpdateBoldedDates();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите событие для редактирования", "Календарь",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnDeleteEventClick(object? sender, EventArgs e)
        {
            if (eventList.SelectedItem != null)
            {
                CalendarEvent selectedEvent = (CalendarEvent)eventList.SelectedItem;
                DateTime selectedDate = calendar.SelectionStart.Date;

                DialogResult result = MessageBox.Show($"Удалить событие \"{selectedEvent.Title}\"?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    eventsStorage[selectedDate].Remove(selectedEvent);

                    if (eventsStorage[selectedDate].Count == 0)
                    {
                        eventsStorage.Remove(selectedDate);
                    }

                    SaveEventsToFile();
                    UpdateEventList();
                    UpdateBoldedDates();
                }
            }
            else
            {
                MessageBox.Show("Выберите событие для удаления", "Календарь",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnClearDayClick(object? sender, EventArgs e)
        {
            DateTime selectedDate = calendar.SelectionStart.Date;

            if (eventsStorage.ContainsKey(selectedDate))
            {
                DialogResult result = MessageBox.Show($"Удалить все события за {selectedDate.ToShortDateString()}?",
                    "Подтверждение очистки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    eventsStorage.Remove(selectedDate);
                    SaveEventsToFile();
                    UpdateEventList();
                    UpdateBoldedDates();
                }
            }
            else
            {
                MessageBox.Show("На этот день нет событий", "Календарь",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnShowAllEventsClick(object? sender, EventArgs e)
        {
            if (eventsStorage.Count == 0)
            {
                MessageBox.Show("Нет сохранённых событий", "Календарь",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form allEventsForm = new Form();
            allEventsForm.Text = "Все события";
            allEventsForm.Size = new Size(500, 400);
            allEventsForm.StartPosition = FormStartPosition.CenterParent;
            allEventsForm.BackColor = Color.FromArgb(32, 32, 32);
            allEventsForm.ForeColor = Color.White;

            ListBox allEventsList = new ListBox();
            allEventsList.Location = new Point(10, 10);
            allEventsList.Size = new Size(465, 340);
            allEventsList.Font = new Font("Segoe UI", 9);
            allEventsList.BackColor = Color.FromArgb(45, 45, 48);
            allEventsList.ForeColor = Color.White;

            foreach (var kvp in eventsStorage.OrderBy(x => x.Key))
            {
                allEventsList.Items.Add($"--- {kvp.Key.ToLongDateString()} ---");
                foreach (var ev in kvp.Value)
                {
                    string reminder = ev.HasReminder ? $" [в {ev.ReminderTime.ToShortTimeString()}]" : "";
                    allEventsList.Items.Add($"   • {ev.Title}{reminder}");
                    if (!string.IsNullOrEmpty(ev.Description))
                    {
                        allEventsList.Items.Add($"     {ev.Description}");
                    }
                }
                allEventsList.Items.Add("");
            }

            Button btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Location = new Point(200, 360);
            btnClose.Size = new Size(80, 30);
            btnClose.BackColor = Color.FromArgb(55, 55, 58);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.DialogResult = DialogResult.OK;

            allEventsForm.Controls.Add(allEventsList);
            allEventsForm.Controls.Add(btnClose);
            allEventsForm.ShowDialog();
        }

        private void OnComplainClick(object? sender, EventArgs e)
        {
            Form complainForm = new Form();
            complainForm.Text = "На что жалуетесь?";
            complainForm.Size = new Size(400, 450);
            complainForm.StartPosition = FormStartPosition.CenterParent;
            complainForm.BackColor = Color.FromArgb(45, 45, 48);
            complainForm.ForeColor = Color.White;
            complainForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            complainForm.MaximizeBox = false;
            complainForm.MinimizeBox = false;

            ListBox complaintList = new ListBox();
            complaintList.Location = new Point(10, 10);
            complaintList.Size = new Size(365, 330);
            complaintList.Font = new Font("Segoe UI", 9);
            complaintList.BackColor = Color.FromArgb(55, 55, 58);
            complaintList.ForeColor = Color.White;
            complaintList.BorderStyle = BorderStyle.FixedSingle;

            string[] complaints = new string[]
            {
                "Календарь показывает неправильные числа!",
                "События пропадают!",
                "Всё работает, но мне скучно.",
                "Мне не нравятся цвета кнопок!",
                "Я не понимаю, как приложение работает!",
                "Слишком много кнопок!",
                "Слишком мало кнопок!",
                "Мне не нравится шрифт!",
                "Я нашёл баг!",
                "Всё работает, но я хотел(а) проверить на кнопку."
            };

            foreach (string complaint in complaints)
            {
                complaintList.Items.Add(complaint);
            }

            Button btnSend = new Button();
            btnSend.Text = "Отправить жалобу";
            btnSend.Location = new Point(10, 350);
            btnSend.Size = new Size(365, 35);
            btnSend.BackColor = Color.FromArgb(180, 80, 80);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Click += (s, ev) =>
            {
                if (complaintList.SelectedItem != null)
                {
                    string selectedComplaint = complaintList.SelectedItem.ToString();
                    string response = GetComplaintResponse(selectedComplaint);
                    MessageBox.Show(response, "Ответ на жалобу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    complainForm.Close();
                }
                else
                {
                    MessageBox.Show("Выберите жалобу из списка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            complainForm.Controls.Add(complaintList);
            complainForm.Controls.Add(btnSend);
            complainForm.ShowDialog();
        }

        private string GetComplaintResponse(string complaint)
        {
            switch (complaint)
            {
                case "Календарь показывает неправильные числа!":
                    return "Календарь не ошибается.";
                case "События пропадают!":
                    return "События не пропадают. Протрезвейте.";
                case "Всё работает, но мне скучно.":
                    return "Вы жалуетесь на то, что всё работает? Это новый уровень.";
                case "Мне не нравятся цвета кнопок!":
                    return "Цвета утверждены советом. В совете работают дальтоники.";
                case "Я не понимаю, как приложение работает!":
                    return "Никто не понимает. Даже разработчик. ОСОБЕННО разработчик...";
                case "Слишком много кнопок!":
                    return "Кнопок ровно столько нужно. Изучите интерфейс нормально.";
                case "Слишком мало кнопок!":
                    return "Вы первый, кто жалуется на малое количество кнопок.";
                case "Мне не нравится шрифт!":
                    return "Шрифт выбран рыбкой разработчика, а Жемчуг - хороший мальчик!";
                case "Я нашёл баг!":
                    return "Это такая неназванная функция. Гордитесь собой!";
                case "Всё работает, но я хотел(а) проверить на кнопку.":
                    return "Похвально, похвально. Исследуйте дальше.";
                default:
                    return "Жалоба принята. Ответ придёт через 'Никогда'.";
            }
        }

        private void CheckReminders(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            foreach (var kvp in eventsStorage)
            {
                foreach (var ev in kvp.Value)
                {
                    if (ev.HasReminder && ev.ReminderTime <= now && ev.ReminderTime > now.AddMinutes(-1))
                    {
                        notifyIcon.ShowBalloonTip(5000, "Напоминание",
                            $"Событие: {ev.Title}\nДата: {kvp.Key.ToShortDateString()}\nВремя: {ev.ReminderTime.ToShortTimeString()}",
                            ToolTipIcon.Info);

                        ev.HasReminder = false;
                        SaveEventsToFile();
                    }
                }
            }
        }

        private string CheckForSixSevenEasterEgg(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            string lowerInput = input.ToLower();

            bool isSixSeven = lowerInput == "67" ||
                              lowerInput == "six seven" ||
                              lowerInput == "sixseven";

            if (isSixSeven)
            {
                Random random = new Random();
                string[] funnyMessages = new string[]
                {
                    "67? Серьезно?",
                    "Личной жизни у тебя нет точно.",
                    "Поздравлем! Ты получил секрет: 67.",
                    "Умно, брат, умно.",
                    "'70 - 3' = 67.",
                    "Сделай большой 67.",
                    "Ты слишком много времени проводишь за компьютером. 67.",
                    "Nы разблокировал достижение: 67.",
                    "Событие 67?",
                };
                return funnyMessages[random.Next(funnyMessages.Length)];
            }

            return input;
        }

        private void SaveEventsToFile()
        {
            try
            {
                var saveData = new Dictionary<string, List<CalendarEvent>>();
                foreach (var kvp in eventsStorage)
                {
                    saveData[kvp.Key.ToString("yyyy-MM-dd")] = kvp.Value;
                }
                string json = JsonSerializer.Serialize(saveData);
                File.WriteAllText("events.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void LoadEventsFromFile()
        {
            try
            {
                if (File.Exists("events.json"))
                {
                    string json = File.ReadAllText("events.json");
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, List<CalendarEvent>>>(json);

                    if (loaded != null)
                    {
                        eventsStorage.Clear();
                        foreach (var kvp in loaded)
                        {
                            if (DateTime.TryParse(kvp.Key, out DateTime date))
                            {
                                eventsStorage[date] = kvp.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            UpdateSelectedDateLabel();
            UpdateEventList();
        }
    }

    public class EventDialog : Form
    {
        public CalendarEvent? NewEvent { get; private set; }

        private DateTimePicker dtpDate;
        private DateTimePicker dtpTime;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private CheckBox chkReminder;

        public EventDialog(DateTime defaultDate, CalendarEvent? editEvent = null)
        {
            this.Text = editEvent == null ? "Новое событие" : "Редактирование события";
            this.Size = new Size(380, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            Label lblDate = new Label();
            lblDate.Text = "Дата: ";
            lblDate.Location = new Point(10, 15);
            lblDate.Size = new Size(60, 23);
            lblDate.ForeColor = Color.White;

            dtpDate = new DateTimePicker();
            dtpDate.Location = new Point(80, 12);
            dtpDate.Size = new Size(130, 23);
            dtpDate.Value = defaultDate;
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.BackColor = Color.FromArgb(55, 55, 58);
            dtpDate.ForeColor = Color.White;

            Label lblTime = new Label();
            lblTime.Text = "Время: ";
            lblTime.Location = new Point(220, 15);
            lblTime.Size = new Size(50, 23);
            lblTime.ForeColor = Color.White;

            dtpTime = new DateTimePicker();
            dtpTime.Location = new Point(270, 12);
            dtpTime.Size = new Size(90, 23);
            dtpTime.Format = DateTimePickerFormat.Time;
            dtpTime.ShowUpDown = true;
            dtpTime.BackColor = Color.FromArgb(55, 55, 58);
            dtpTime.ForeColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = "Название: ";
            lblTitle.Location = new Point(10, 50);
            lblTitle.Size = new Size(70, 23);
            lblTitle.ForeColor = Color.White;

            txtTitle = new TextBox();
            txtTitle.Location = new Point(80, 47);
            txtTitle.Size = new Size(280, 23);
            txtTitle.BackColor = Color.FromArgb(55, 55, 58);
            txtTitle.ForeColor = Color.White;
            txtTitle.BorderStyle = BorderStyle.FixedSingle;

            Label lblDesc = new Label();
            lblDesc.Text = "Описание: ";
            lblDesc.Location = new Point(10, 85);
            lblDesc.Size = new Size(70, 23);
            lblDesc.ForeColor = Color.White;

            txtDescription = new TextBox();
            txtDescription.Location = new Point(80, 82);
            txtDescription.Size = new Size(280, 23);
            txtDescription.BackColor = Color.FromArgb(55, 55, 58);
            txtDescription.ForeColor = Color.White;
            txtDescription.BorderStyle = BorderStyle.FixedSingle;

            chkReminder = new CheckBox();
            chkReminder.Text = "Напомнить о событии";
            chkReminder.Location = new Point(10, 120);
            chkReminder.Size = new Size(200, 25);
            chkReminder.ForeColor = Color.White;
            chkReminder.CheckedChanged += (s, ev) => dtpTime.Enabled = chkReminder.Checked;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(200, 160);
            btnOk.Size = new Size(75, 30);
            btnOk.BackColor = Color.FromArgb(0, 120, 215);
            btnOk.ForeColor = Color.White;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Click += OnOkClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(285, 160);
            btnCancel.Size = new Size(75, 30);
            btnCancel.BackColor = Color.FromArgb(55, 55, 58);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(lblDate);
            Controls.Add(dtpDate);
            Controls.Add(lblTime);
            Controls.Add(dtpTime);
            Controls.Add(lblTitle);
            Controls.Add(txtTitle);
            Controls.Add(lblDesc);
            Controls.Add(txtDescription);
            Controls.Add(chkReminder);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            if (editEvent != null)
            {
                dtpDate.Value = defaultDate;
                dtpTime.Value = editEvent.ReminderTime;
                txtTitle.Text = editEvent.Title ?? "";
                txtDescription.Text = editEvent.Description ?? "";
                chkReminder.Checked = editEvent.HasReminder;
                dtpTime.Enabled = editEvent.HasReminder;
            }
            else
            {
                dtpTime.Value = DateTime.Now.AddHours(1);
                dtpTime.Enabled = false;
            }
        }

        private void OnOkClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название события: ", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime eventDate = dtpDate.Value.Date;
            DateTime reminderTime = eventDate + dtpTime.Value.TimeOfDay;

            NewEvent = new CalendarEvent
            {
                Title = txtTitle.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                ReminderTime = reminderTime,
                HasReminder = chkReminder.Checked
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}