using DoubleClickFix.Properties;

namespace DoubleClickFix
{
    public partial class InteractiveForm : Form
    {
        private int minDelay;
        private readonly StartupRegistry startup;
        private readonly Settings settings;

        public InteractiveForm(StartupRegistry startup, Settings settings, Logger logger)
        {
            this.startup = startup;
            this.settings = settings;

            InitializeComponent();
            this.FormClosing += HideFormInsteadOfClosing;
            this.runAtStartupCheckBox.Checked = startup.IsRegistered();
            logger.AddLogger(text => Log(text));
            this.MinDelay = settings.MinimumDoubleClickDelayMilliseconds;
            SetupPictureBox();
        }

        public void SetupPictureBox()
        {
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            richTextBox1.MouseDown += PictureBox1_MouseDown;
            richTextBox1.MouseUp += PictureBox1_MouseUp;
        }

        private void PictureBox1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                left.BackColor = Color.Red;
            }
            else if (e.Button == MouseButtons.Right)
            {
                right.BackColor = Color.Red;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                middle.BackColor = Color.Red;
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                x1.BackColor = Color.Red;
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                x2.BackColor = Color.Red;
            }
        }

        private void PictureBox1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                left.BackColor = Color.Transparent;
            }
            else if (e.Button == MouseButtons.Right)
            {
                right.BackColor = Color.Transparent;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                middle.BackColor = Color.Transparent;
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                x1.BackColor = Color.Transparent;
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                x2.BackColor = Color.Transparent;
            }
        }

        private void Log(string message)
        {
            if (!IsDisposed && InvokeRequired)
            {
                Invoke(new Action<string>(Log), message);
                return;
            }
            if (!IsDisposed)
            {
                logTextBox.AppendText(message + Environment.NewLine);
            }
        }

        private void ShowForm()
        {
            this.Show();
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            this.BringToFront();
        }

        private void HideFormInsteadOfClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        private void OnSaveButtonClicked(object? sender, EventArgs e)
        {
            bool success;

            var buttons = (left.Checked ? MouseButtons.Left : 0)
                        | (right.Checked ? MouseButtons.Right : 0)
                        | (middle.Checked ? MouseButtons.Middle : 0)
                        | (x1.Checked ? MouseButtons.XButton1 : 0)
                        | (x2.Checked ? MouseButtons.XButton2 : 0);

            if (runAtStartupCheckBox.Checked)
            {
                success = startup.Register();
            }
            else
            {
                success = startup.Unregister();
            }
            if (!success)
            {
                Log(Resources.WritingRegistryFailed);
            }
            // TODO set settings here, or live in the event handlers?
            if (int.TryParse(delayTextBox.Text, out int minValue))
            {
                settings.MinimumDoubleClickDelayMilliseconds = minValue;
                settings.Save();
            }
        }

        public int MinDelay
        {
            get
            {
                return minDelay;
            }
            set
            {
                minDelay = value;
                delayTextBox.Text = value.ToString();
            }
        }
        private void LogTextBoxChanged(object? sender, EventArgs e)
        {
            if (logTextBox.TextLength > logTextBox.MaxLength - 1000)
            {
                logTextBox.Clear();
            }
        }

        private void NotifyIconDoubleClick(object? sender, MouseEventArgs e)
        {
            this.ShowForm();
        }

        private void ShowUiMenuClick(object? sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void ExitMenuClick(object? sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            // make sure the icon is removed from the system tray
            notifyIcon.Dispose();
            Application.Exit();
        }

    }
}
