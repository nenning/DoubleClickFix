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
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            bool success;
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
            if (int.TryParse(delayTextBox.Text, out int minValue))
            {
                settings.UpdateAppSettings(minValue);
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
        private void LogTextBoxChanged(object sender, EventArgs e)
        {
            if (logTextBox.TextLength > logTextBox.MaxLength - 1000)
            {
                logTextBox.Clear();
            }
        }

        private void NotifyIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowForm();
        }

        private void ShowUiMenuClick(object sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void ExitMenuClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            // make sure the icon is removed from the system tray
            notifyIcon.Dispose();
            Application.Exit();
        }
    }
}
