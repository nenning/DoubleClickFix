using DoubleClickFix.Properties;
namespace DoubleClickFix;

internal partial class InteractiveForm : Form
{
    private const int WM_INPUT = 0x00FF;

    private readonly IStartupRegistry startup;
    private readonly ISettings settings;
    private readonly Logger logger;
    private readonly Action<nint> rawInputProcessor;
    private readonly System.Windows.Forms.Timer debounceTimer;

    public InteractiveForm(IStartupRegistry startup, ISettings settings, Logger logger, Action<IntPtr> rawInputProcessor)
    {
        this.startup = startup;
        this.settings = settings;
        this.logger = logger;
        this.rawInputProcessor = rawInputProcessor;
        InitializeComponent();

        debounceTimer = new()
        {
            Interval = 100
        };
        debounceTimer.Tick += OnDebounceTimerTick;
        this.FormClosing += OnFormClosing;
        this.runAtStartupCheckBox.Checked = startup.IsRegistered();
        this.useMinDelayCheckBox.Checked = settings.MinDelay >= 0;

        logger.AddGuiLogger(text => Log(text));
        SetupTestArea();
        this.mouseButtonComboBox.SelectedIndex = 0;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_INPUT)
        {
            rawInputProcessor(m.LParam);
        }
        base.WndProc(ref m);
    }
    private void SetupTestArea()
    {
        pictureBox1.MouseDown += OnTestMouseDown;
        pictureBox1.MouseUp += OnTestMouseUp;
        richTextBox1.MouseDown += OnTestMouseDown;
        richTextBox1.MouseUp += OnTestMouseUp;
        OnHideTestControls(this, EventArgs.Empty);
    }

    private void OnTestMouseDown(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                left.BackColor = Color.Red;
                break;
            case MouseButtons.Right:
                right.BackColor = Color.Red;
                break;
            case MouseButtons.Middle:
                middle.BackColor = Color.Red;
                break;
            case MouseButtons.XButton1:
                x1.BackColor = Color.Red;
                break;
            case MouseButtons.XButton2:
                x2.BackColor = Color.Red;
                break;
        }
    }

    private void OnTestMouseUp(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                left.BackColor = Color.Transparent;
                break;
            case MouseButtons.Right:
                right.BackColor = Color.Transparent;
                break;
            case MouseButtons.Middle:
                middle.BackColor = Color.Transparent;
                break;
            case MouseButtons.XButton1:
                x1.BackColor = Color.Transparent;
                break;
            case MouseButtons.XButton2:
                x2.BackColor = Color.Transparent;
                break;
        }
    }

    private void Log(string message)
    {
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

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
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

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        logger.IsAppVisible = this.Visible;
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
        int threshold = -1;
        var index = mouseButtonComboBox.SelectedIndex;
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

    private void OnDebounceTimerTick(object? sender, EventArgs e)
    {
        UpdateSettings();
        debounceTimer.Stop();
    }

    private void UpdateSettings()
    {
        int threshold = thresholdSlider.Value;
        switch (mouseButtonComboBox.SelectedIndex)
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
        else
        {
            if (buttonEnabledCheckBox.Focused)
            {
                thresholdSlider.Value = 50;
            }
        }
    }

    private void UseMinDelayCheckBoxCheckedChanged(object sender, EventArgs e)
    {
        settings.MinDelay = useMinDelayCheckBox.Checked ? 0 : -1;
    }
}
