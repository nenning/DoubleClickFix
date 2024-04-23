using DoubleClickFix.Properties;
using System.Diagnostics;
namespace DoubleClickFix
{
    public partial class InteractiveForm : Form
    {
        private readonly StartupRegistry startup;
        private readonly Settings settings;
        private readonly System.Windows.Forms.Timer debounceTimer;

        public InteractiveForm(StartupRegistry startup, Settings settings, Logger logger)
        {
            this.startup = startup;
            this.settings = settings;

            InitializeComponent();

            debounceTimer = new();
            debounceTimer.Interval = 100;
            debounceTimer.Tick += OnDebounceTimerTick;

            this.FormClosing += OnHideFormInsteadOfClosing;
            this.runAtStartupCheckBox.Checked = startup.IsRegistered();
            logger.AddLogger(text => Log(text));
            SetupPictureBox();
            this.comboBox1.SelectedIndex = 0;
        }

        public void SetupPictureBox()
        {
            pictureBox1.MouseDown += OnTestMouseDown;
            pictureBox1.MouseUp += OnTestMouseUp;
            richTextBox1.MouseDown += OnTestMouseDown;
            richTextBox1.MouseUp += OnTestMouseUp;
            OnHideTestControls(this, EventArgs.Empty);
        }

        private void OnTestMouseDown(object? sender, MouseEventArgs e)
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

        private void OnTestMouseUp(object? sender, MouseEventArgs e)
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

        private void OnHideFormInsteadOfClosing(object? sender, FormClosingEventArgs e)
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
            settings.Save();
        }

        private void OnLogTextBoxChanged(object? sender, EventArgs e)
        {
            if (logTextBox.TextLength > logTextBox.MaxLength - 1000)
            {
                logTextBox.Clear();
            }
        }

        private void OnNotifyIconDoubleClick(object? sender, MouseEventArgs e)
        {
            this.ShowForm();
        }

        private void OnShowUiMenuClick(object? sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void OnExitMenuClick(object? sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            // make sure the icon is removed from the system tray
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void OnSelectedMouseButtonChanged(object sender, EventArgs e)
        {
            thresholdSlider.Minimum = -1;
            thresholdSlider.Maximum = Math.Min(200, settings.WindowsDoubleClickTimeMilliseconds);
            int threshold = -1;
            var index = comboBox1.SelectedIndex;
            switch (index)
            {
                case 0:
                    threshold = settings.LeftThreshold;
                    break;
                case 1:
                    threshold = settings.RightThreshold;
                    break;
                case 2:
                    threshold = settings.MiddleThreshold;
                    break;
                case 3:
                    threshold = settings.X1Threshold;
                    break;
                case 4:
                    threshold = settings.X2Threshold;
                    break;
            }
            buttonEnabledCheckBox.Checked = threshold >= 0;
            thresholdSlider.Value = threshold;
        }

        private void OnShowTestControls(object sender, EventArgs e)
        {
            left.Show();
            right.Show();
            middle.Show();
            x1.Show();
            x2.Show();
            left.Checked = settings.LeftThreshold >= 0;
            right.Checked = settings.RightThreshold >= 0;
            middle.Checked = settings.MiddleThreshold >= 0;
            x1.Checked = settings.X1Threshold >= 0;
            x2.Checked = settings.X2Threshold >= 0;
        }

        private void OnHideTestControls(object sender, EventArgs e)
        {
            left.Hide();
            right.Hide();
            middle.Hide();
            x1.Hide();
            x2.Hide();
            pictureBox1.Invalidate();
        }

        private void OnThresholdValueChanged(object sender, EventArgs e)
        {
            this.delayTextBox.Text = thresholdSlider.Value.ToString();
            bool enabled = thresholdSlider.Value >= 0;
            if (enabled != buttonEnabledCheckBox.Checked)
            {
                buttonEnabledCheckBox.Checked = enabled;
            }
            ResetDebounceTimer();
        }

        private void ResetDebounceTimer()
        {
            // Restart the debounce timer whenever the TrackBar value changes
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void OnDebounceTimerTick(object sender, EventArgs e)
        {
            UpdateSettings();
            debounceTimer.Stop();
        }

        private void UpdateSettings()
        {
            int threshold = thresholdSlider.Value;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    settings.LeftThreshold = threshold;
                    break;
                case 1:
                    settings.RightThreshold = threshold;
                    break;
                case 2:
                    settings.MiddleThreshold = threshold;
                    break;
                case 3:
                    settings.X1Threshold = threshold;
                    break;
                case 4:
                    settings.X2Threshold = threshold;
                    break;
            }
        }

        private void OnButtonEnabledCheckedChanged(object sender, EventArgs e)
        {
            if (!buttonEnabledCheckBox.Checked)
            {
                thresholdSlider.Value = -1;
            }
        }
    }
}
