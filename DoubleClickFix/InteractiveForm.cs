using Microsoft.Win32;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace DoubleClickFix
{
    public partial class InteractiveForm : Form
    {
        private int minDelay;
        private readonly StartupRegistry startup;
        private readonly Settings settings;

        public InteractiveForm(StartupRegistry startup, Logger logger, Settings settings)
        {
            this.startup = startup;
            this.settings = settings;
            InitializeComponent();
            SetupTrayIcon();
            this.runAtStartupCheckBox.Checked = startup.IsRegistered();

            logger.AddLogger(text => Log(text));

            this.MinDelay = settings.MinimumDoubleClickDelayMilliseconds;

        }

        private void SetupTrayIcon()
        {
            this.FormClosing += HideFormInsteadOfClosing;
            NotifyIcon notifyIcon = new()
            {
                Icon = this.Icon,
                Text = "Double-click fix"
            };
            notifyIcon.DoubleClick += (sender, e) => this.Show();
            ContextMenuStrip contextMenuStrip = new();
            ToolStripMenuItem exitMenuItem = new("Exit");
            exitMenuItem.Click += (sender, e) =>
            {
                notifyIcon.Visible = false;
                Application.Exit();
            };
            contextMenuStrip.Items.Add(exitMenuItem);
            ToolStripMenuItem debugMenuItem = new("Show UI");
            debugMenuItem.Click += (sender, e) => this.Show();

            contextMenuStrip.Items.Add(debugMenuItem);
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Visible = true;
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
                Log("Failed to write to startup registry.");
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
        public void Log(string text)
        {
            if (!IsDisposed && Visible)
            {
                logTextBox.AppendText(text + Environment.NewLine);
            }
        }

        private void LogTextBoxChanged(object sender, EventArgs e)
        {
            if (logTextBox.TextLength > logTextBox.MaxLength - 1000)
            {
                logTextBox.Clear();
            }
        }
    }
}
