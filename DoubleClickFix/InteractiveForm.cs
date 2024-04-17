using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace DoubleClickFix
{
    public partial class InteractiveForm : Form
    {
        public event Action<int>? OnSave;
        private int minDelay;

        public InteractiveForm()
        {
            InitializeComponent();
            SetupTrayIcon();
        }

        private void SetupTrayIcon()
        {
            this.FormClosing += HideFormInsteadOfClosing;
            NotifyIcon notifyIcon = new()
            {
                Icon = new Icon("app.ico"),
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
            ToolStripMenuItem debugMenuItem = new("Show Debug View");
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
            int minValue;
            if (this.OnSave != null && int.TryParse(delayTextBox.Text, out minValue))
            {
                this.OnSave(minValue);
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
                this.delayTextBox.Text = value.ToString();
            }
        }
        public void Log(string text)
        {
            logTextBox.AppendText(text + Environment.NewLine);
        }
    }
}
