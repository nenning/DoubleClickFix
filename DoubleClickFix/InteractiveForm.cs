using DoubleClickFix.Properties;
using System.Diagnostics;

namespace DoubleClickFix
{
    public partial class InteractiveForm : Form
    {
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
            SetupPictureBox();
            this.comboBox1.SelectedIndex = 0;
        }

        public void SetupPictureBox()
        {
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            richTextBox1.MouseDown += PictureBox1_MouseDown;
            richTextBox1.MouseUp += PictureBox1_MouseUp;
            HideTestControls(this, EventArgs.Empty);
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
            // TODO set settings in the event handlers?
            if (int.TryParse(delayTextBox.Text, out int minValue))
            {
                settings.Save();
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

        private void SelectedMouseButtonChanged(object sender, EventArgs e)
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

        private void ShowTestControls(object sender, EventArgs e)
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

        private void HideTestControls(object sender, EventArgs e)
        {
            left.Hide();
            right.Hide();
            middle.Hide();
            x1.Hide();
            x2.Hide();
            pictureBox1.Invalidate();
        }

        private void ThresholdValueChanged(object sender, EventArgs e)
        {
            this.delayTextBox.Text = thresholdSlider.Value.ToString();
            bool enabled = thresholdSlider.Value >= 0;
            if (enabled != buttonEnabledCheckBox.Checked)
            {
                buttonEnabledCheckBox.Checked = enabled;
            }
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            int threshold = thresholdSlider.Value;
            // TODO debounce
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

        private void ButtonEnabledCheckedChanged(object sender, EventArgs e)
        {
            if (!buttonEnabledCheckBox.Checked)
            {
                thresholdSlider.Value = -1;
            }
        }
    }
}
