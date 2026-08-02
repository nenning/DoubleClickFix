namespace DoubleClickFix
{
    partial class InteractiveForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(InteractiveForm));
			logTextBox = new TextBox();
			delayLabel = new Label();
			delayTextBox = new TextBox();
			resetButton = new Button();
			runAtStartupCheckBox = new CheckBox();
			groupBox1 = new GroupBox();
			groupBox1Layout = new TableLayoutPanel();
			mouseButtonComboBox = new ComboBox();
			buttonEnabledCheckBox = new CheckBox();
			thresholdSlider = new TrackBar();
			useMinDelayCheckBox = new CheckBox();
			fixDraggingCheckBox = new CheckBox();
			dragStartDelayLabel = new Label();
			dragStartDelayTextBox = new TextBox();
			dragEndDelayLabel = new Label();
			dragEndDelayTextBox = new TextBox();
			remoteDesktopCheckBox = new CheckBox();
			currentDeviceLabel = new Label();
			ignoreCurrentDeviceCheckBox = new CheckBox();
			x2 = new CheckBox();
			x1 = new CheckBox();
			middle = new CheckBox();
			right = new CheckBox();
			left = new CheckBox();
			wheel = new CheckBox();
			pictureBox1 = new PictureBox();
			notifyIcon = new NotifyIcon(components);
			notifyMenuStrip = new ContextMenuStrip(components);
			showUiMenu = new ToolStripMenuItem();
			exitMenu = new ToolStripMenuItem();
			label1 = new Label();
			descriptionTextBox = new TextBox();
			richTextBox1 = new RichTextBox();
			groupBox2 = new GroupBox();
			groupBox3 = new GroupBox();
			groupBox4 = new GroupBox();
			testPicturePanel = new Panel();
			groupBoxDevice = new GroupBox();
			groupBoxDeviceLayout = new TableLayoutPanel();
			groupBoxGeneral = new GroupBox();
			groupBoxGeneralLayout = new TableLayoutPanel();
			toolTip1 = new ToolTip(components);
			versionLabel = new Label();
			updateLinkLabel = new LinkLabel();
			gitLinkLabel = new LinkLabel();
			themeComboBox = new ComboBox();
			languageComboBox = new ComboBox();
			bottomPanel = new Panel();
			bottomLayout = new TableLayoutPanel();
			rootLayout = new TableLayoutPanel();
			headerLayout = new TableLayoutPanel();
			middleLayout = new TableLayoutPanel();
			groupBox1.SuspendLayout();
			groupBox1Layout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)thresholdSlider).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			notifyMenuStrip.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			testPicturePanel.SuspendLayout();
			groupBoxDevice.SuspendLayout();
			groupBoxDeviceLayout.SuspendLayout();
			groupBoxGeneral.SuspendLayout();
			groupBoxGeneralLayout.SuspendLayout();
			bottomPanel.SuspendLayout();
			bottomLayout.SuspendLayout();
			rootLayout.SuspendLayout();
			headerLayout.SuspendLayout();
			middleLayout.SuspendLayout();
			SuspendLayout();
			// 
			// logTextBox
			// 
			resources.ApplyResources(logTextBox, "logTextBox");
			logTextBox.Name = "logTextBox";
			logTextBox.ReadOnly = true;
			logTextBox.TabStop = false;
			logTextBox.TextChanged += OnLogTextBoxChanged;
			// 
			// delayLabel
			// 
			resources.ApplyResources(delayLabel, "delayLabel");
			delayLabel.Name = "delayLabel";
			toolTip1.SetToolTip(delayLabel, resources.GetString("delayLabel.ToolTip"));
			// 
			// delayTextBox
			// 
			resources.ApplyResources(delayTextBox, "delayTextBox");
			delayTextBox.Name = "delayTextBox";
			toolTip1.SetToolTip(delayTextBox, resources.GetString("delayTextBox.ToolTip"));
			// 
			// resetButton
			// 
			resources.ApplyResources(resetButton, "resetButton");
			resetButton.Name = "resetButton";
			// 
			// runAtStartupCheckBox
			// 
			resources.ApplyResources(runAtStartupCheckBox, "runAtStartupCheckBox");
			runAtStartupCheckBox.Name = "runAtStartupCheckBox";
			toolTip1.SetToolTip(runAtStartupCheckBox, resources.GetString("runAtStartupCheckBox.ToolTip"));
			runAtStartupCheckBox.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			resources.ApplyResources(groupBox1, "groupBox1");
			groupBox1.Controls.Add(groupBox1Layout);
			groupBox1.Name = "groupBox1";
			groupBox1.TabStop = false;
			// 
			// groupBox1Layout
			// 
			resources.ApplyResources(groupBox1Layout, "groupBox1Layout");
			groupBox1Layout.Controls.Add(mouseButtonComboBox, 0, 0);
			groupBox1Layout.Controls.Add(buttonEnabledCheckBox, 1, 0);
			groupBox1Layout.Controls.Add(delayLabel, 0, 1);
			groupBox1Layout.Controls.Add(thresholdSlider, 1, 1);
			groupBox1Layout.Controls.Add(delayTextBox, 2, 1);
			groupBox1Layout.Controls.Add(useMinDelayCheckBox, 0, 2);
			groupBox1Layout.Controls.Add(fixDraggingCheckBox, 0, 3);
			groupBox1Layout.Controls.Add(dragStartDelayLabel, 1, 3);
			groupBox1Layout.Controls.Add(dragStartDelayTextBox, 2, 3);
			groupBox1Layout.Controls.Add(dragEndDelayLabel, 1, 4);
			groupBox1Layout.Controls.Add(dragEndDelayTextBox, 2, 4);
			groupBox1Layout.Name = "groupBox1Layout";
			// 
			// mouseButtonComboBox
			// 
			mouseButtonComboBox.FormattingEnabled = true;
			mouseButtonComboBox.Items.AddRange(new object[] { resources.GetString("mouseButtonComboBox.Items"), resources.GetString("mouseButtonComboBox.Items1"), resources.GetString("mouseButtonComboBox.Items2"), resources.GetString("mouseButtonComboBox.Items3"), resources.GetString("mouseButtonComboBox.Items4"), resources.GetString("mouseButtonComboBox.Items5") });
			resources.ApplyResources(mouseButtonComboBox, "mouseButtonComboBox");
			mouseButtonComboBox.Name = "mouseButtonComboBox";
			mouseButtonComboBox.SelectedIndexChanged += OnSelectedMouseButtonChanged;
			// 
			// buttonEnabledCheckBox
			// 
			resources.ApplyResources(buttonEnabledCheckBox, "buttonEnabledCheckBox");
			buttonEnabledCheckBox.Name = "buttonEnabledCheckBox";
			toolTip1.SetToolTip(buttonEnabledCheckBox, resources.GetString("buttonEnabledCheckBox.ToolTip"));
			buttonEnabledCheckBox.UseVisualStyleBackColor = true;
			buttonEnabledCheckBox.CheckedChanged += OnButtonEnabledCheckedChanged;
			// 
			// thresholdSlider
			// 
			resources.ApplyResources(thresholdSlider, "thresholdSlider");
			thresholdSlider.LargeChange = 20;
			thresholdSlider.Maximum = 200;
			thresholdSlider.Minimum = -1;
			thresholdSlider.Name = "thresholdSlider";
			groupBox1Layout.SetRowSpan(thresholdSlider, 2);
			thresholdSlider.TickFrequency = 10;
			thresholdSlider.ValueChanged += OnThresholdValueChanged;
			// 
			// useMinDelayCheckBox
			// 
			resources.ApplyResources(useMinDelayCheckBox, "useMinDelayCheckBox");
			useMinDelayCheckBox.Name = "useMinDelayCheckBox";
			toolTip1.SetToolTip(useMinDelayCheckBox, resources.GetString("useMinDelayCheckBox.ToolTip"));
			useMinDelayCheckBox.UseVisualStyleBackColor = true;
			useMinDelayCheckBox.CheckedChanged += UseMinDelayCheckBoxCheckedChanged;
			// 
			// fixDraggingCheckBox
			// 
			resources.ApplyResources(fixDraggingCheckBox, "fixDraggingCheckBox");
			fixDraggingCheckBox.Name = "fixDraggingCheckBox";
			toolTip1.SetToolTip(fixDraggingCheckBox, resources.GetString("fixDraggingCheckBox.ToolTip"));
			fixDraggingCheckBox.UseVisualStyleBackColor = true;
			fixDraggingCheckBox.CheckedChanged += OnFixDraggingCheckBoxChanged;
			// 
			// dragStartDelayLabel
			// 
			resources.ApplyResources(dragStartDelayLabel, "dragStartDelayLabel");
			dragStartDelayLabel.Name = "dragStartDelayLabel";
			toolTip1.SetToolTip(dragStartDelayLabel, resources.GetString("dragStartDelayLabel.ToolTip"));
			// 
			// dragStartDelayTextBox
			// 
			resources.ApplyResources(dragStartDelayTextBox, "dragStartDelayTextBox");
			dragStartDelayTextBox.Name = "dragStartDelayTextBox";
			dragStartDelayTextBox.TextChanged += OnDragStartDelayTextChanged;
			// 
			// dragEndDelayLabel
			// 
			resources.ApplyResources(dragEndDelayLabel, "dragEndDelayLabel");
			dragEndDelayLabel.Name = "dragEndDelayLabel";
			toolTip1.SetToolTip(dragEndDelayLabel, resources.GetString("dragEndDelayLabel.ToolTip"));
			// 
			// dragEndDelayTextBox
			// 
			resources.ApplyResources(dragEndDelayTextBox, "dragEndDelayTextBox");
			dragEndDelayTextBox.Name = "dragEndDelayTextBox";
			dragEndDelayTextBox.TextChanged += OnDragStopDelayTextChanged;
			// 
			// remoteDesktopCheckBox
			// 
			resources.ApplyResources(remoteDesktopCheckBox, "remoteDesktopCheckBox");
			remoteDesktopCheckBox.Name = "remoteDesktopCheckBox";
			toolTip1.SetToolTip(remoteDesktopCheckBox, resources.GetString("remoteDesktopCheckBox.ToolTip"));
			remoteDesktopCheckBox.UseVisualStyleBackColor = true;
			remoteDesktopCheckBox.CheckedChanged += OnRemoteDesktopCheckBoxChanged;
			// 
			// currentDeviceLabel
			// 
			resources.ApplyResources(currentDeviceLabel, "currentDeviceLabel");
			currentDeviceLabel.Name = "currentDeviceLabel";
			// 
			// ignoreCurrentDeviceCheckBox
			// 
			resources.ApplyResources(ignoreCurrentDeviceCheckBox, "ignoreCurrentDeviceCheckBox");
			ignoreCurrentDeviceCheckBox.Name = "ignoreCurrentDeviceCheckBox";
			ignoreCurrentDeviceCheckBox.UseVisualStyleBackColor = true;
			ignoreCurrentDeviceCheckBox.CheckedChanged += OnIgnoreCurrentDeviceCheckBoxChanged;
			// 
			// x2
			// 
			resources.ApplyResources(x2, "x2");
			x2.Name = "x2";
			x2.UseVisualStyleBackColor = true;
			// 
			// x1
			// 
			resources.ApplyResources(x1, "x1");
			x1.Name = "x1";
			x1.UseVisualStyleBackColor = true;
			// 
			// middle
			// 
			resources.ApplyResources(middle, "middle");
			middle.Name = "middle";
			middle.UseVisualStyleBackColor = true;
			// 
			// right
			// 
			resources.ApplyResources(right, "right");
			right.Name = "right";
			right.UseVisualStyleBackColor = true;
			// 
			// left
			// 
			resources.ApplyResources(left, "left");
			left.Name = "left";
			left.UseVisualStyleBackColor = true;
			// 
			// wheel
			// 
			resources.ApplyResources(wheel, "wheel");
			wheel.Name = "wheel";
			wheel.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(pictureBox1, "pictureBox1");
			pictureBox1.Image = Properties.Resources.app;
			pictureBox1.Name = "pictureBox1";
			pictureBox1.TabStop = false;
			pictureBox1.MouseEnter += OnShowTestControls;
			pictureBox1.MouseLeave += OnHideTestControls;
			// 
			// notifyIcon
			// 
			notifyIcon.ContextMenuStrip = notifyMenuStrip;
			resources.ApplyResources(notifyIcon, "notifyIcon");
			notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
			// 
			// notifyMenuStrip
			// 
			notifyMenuStrip.ImageScalingSize = new Size(24, 24);
			notifyMenuStrip.Items.AddRange(new ToolStripItem[] { showUiMenu, exitMenu });
			notifyMenuStrip.Name = "contextMenuStrip1";
			resources.ApplyResources(notifyMenuStrip, "notifyMenuStrip");
			// 
			// showUiMenu
			// 
			showUiMenu.Name = "showUiMenu";
			resources.ApplyResources(showUiMenu, "showUiMenu");
			showUiMenu.Click += OnShowUiMenuClick;
			// 
			// exitMenu
			// 
			exitMenu.Name = "exitMenu";
			resources.ApplyResources(exitMenu, "exitMenu");
			exitMenu.Click += OnExitMenuClick;
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			// 
			// descriptionTextBox
			// 
			descriptionTextBox.BackColor = SystemColors.Control;
			resources.ApplyResources(descriptionTextBox, "descriptionTextBox");
			descriptionTextBox.Name = "descriptionTextBox";
			descriptionTextBox.ReadOnly = true;
			descriptionTextBox.TabStop = false;
			// 
			// richTextBox1
			// 
			resources.ApplyResources(richTextBox1, "richTextBox1");
			richTextBox1.Name = "richTextBox1";
			richTextBox1.ReadOnly = true;
			richTextBox1.MouseEnter += OnShowTestControls;
			richTextBox1.MouseLeave += OnHideTestControls;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(descriptionTextBox);
			resources.ApplyResources(groupBox2, "groupBox2");
			groupBox2.Name = "groupBox2";
			groupBox2.TabStop = false;
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(logTextBox);
			resources.ApplyResources(groupBox3, "groupBox3");
			groupBox3.Name = "groupBox3";
			groupBox3.TabStop = false;
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(richTextBox1);
			groupBox4.Controls.Add(testPicturePanel);
			resources.ApplyResources(groupBox4, "groupBox4");
			groupBox4.Name = "groupBox4";
			rootLayout.SetRowSpan(groupBox4, 4);
			groupBox4.TabStop = false;
			// 
			// testPicturePanel
			// 
			testPicturePanel.Controls.Add(x2);
			testPicturePanel.Controls.Add(x1);
			testPicturePanel.Controls.Add(middle);
			testPicturePanel.Controls.Add(left);
			testPicturePanel.Controls.Add(right);
			testPicturePanel.Controls.Add(wheel);
			testPicturePanel.Controls.Add(pictureBox1);
			resources.ApplyResources(testPicturePanel, "testPicturePanel");
			testPicturePanel.Name = "testPicturePanel";
			// 
			// groupBoxDevice
			// 
			resources.ApplyResources(groupBoxDevice, "groupBoxDevice");
			groupBoxDevice.Controls.Add(groupBoxDeviceLayout);
			groupBoxDevice.Name = "groupBoxDevice";
			groupBoxDevice.TabStop = false;
			// 
			// groupBoxDeviceLayout
			// 
			resources.ApplyResources(groupBoxDeviceLayout, "groupBoxDeviceLayout");
			groupBoxDeviceLayout.Controls.Add(currentDeviceLabel, 0, 0);
			groupBoxDeviceLayout.Controls.Add(ignoreCurrentDeviceCheckBox, 1, 0);
			groupBoxDeviceLayout.Name = "groupBoxDeviceLayout";
			// 
			// groupBoxGeneral
			// 
			resources.ApplyResources(groupBoxGeneral, "groupBoxGeneral");
			groupBoxGeneral.Controls.Add(groupBoxGeneralLayout);
			groupBoxGeneral.Name = "groupBoxGeneral";
			groupBoxGeneral.TabStop = false;
			// 
			// groupBoxGeneralLayout
			// 
			resources.ApplyResources(groupBoxGeneralLayout, "groupBoxGeneralLayout");
			groupBoxGeneralLayout.Controls.Add(remoteDesktopCheckBox, 0, 0);
			groupBoxGeneralLayout.Controls.Add(resetButton, 1, 0);
			groupBoxGeneralLayout.Controls.Add(runAtStartupCheckBox, 0, 1);
			groupBoxGeneralLayout.Name = "groupBoxGeneralLayout";
			// 
			// versionLabel
			// 
			resources.ApplyResources(versionLabel, "versionLabel");
			versionLabel.Name = "versionLabel";
			// 
			// updateLinkLabel
			// 
			resources.ApplyResources(updateLinkLabel, "updateLinkLabel");
			updateLinkLabel.Name = "updateLinkLabel";
			updateLinkLabel.TabStop = true;
			// 
			// gitLinkLabel
			// 
			resources.ApplyResources(gitLinkLabel, "gitLinkLabel");
			gitLinkLabel.Name = "gitLinkLabel";
			gitLinkLabel.TabStop = true;
			gitLinkLabel.LinkClicked += OnGitLinkLabelClicked;
			// 
			// themeComboBox
			// 
			resources.ApplyResources(themeComboBox, "themeComboBox");
			themeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			themeComboBox.FormattingEnabled = true;
			themeComboBox.Name = "themeComboBox";
			// 
			// languageComboBox
			// 
			resources.ApplyResources(languageComboBox, "languageComboBox");
			languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			languageComboBox.FormattingEnabled = true;
			languageComboBox.Name = "languageComboBox";
			languageComboBox.SelectedIndexChanged += OnLanguageChanged;
			// 
			// bottomPanel
			// 
			resources.ApplyResources(bottomPanel, "bottomPanel");
			rootLayout.SetColumnSpan(bottomPanel, 2);
			bottomPanel.Controls.Add(bottomLayout);
			bottomPanel.Name = "bottomPanel";
			// 
			// bottomLayout
			// 
			resources.ApplyResources(bottomLayout, "bottomLayout");
			bottomLayout.Controls.Add(gitLinkLabel, 0, 0);
			bottomLayout.Controls.Add(updateLinkLabel, 1, 0);
			bottomLayout.Controls.Add(versionLabel, 2, 0);
			bottomLayout.Name = "bottomLayout";
			// 
			// rootLayout
			// 
			resources.ApplyResources(rootLayout, "rootLayout");
			rootLayout.Controls.Add(headerLayout, 0, 0);
			rootLayout.Controls.Add(groupBoxDevice, 0, 1);
			rootLayout.Controls.Add(groupBox1, 0, 2);
			rootLayout.Controls.Add(groupBoxGeneral, 0, 3);
			rootLayout.Controls.Add(middleLayout, 0, 4);
			rootLayout.Controls.Add(bottomPanel, 0, 5);
			rootLayout.Controls.Add(groupBox4, 1, 1);
			rootLayout.Name = "rootLayout";
			// 
			// headerLayout
			// 
			resources.ApplyResources(headerLayout, "headerLayout");
			rootLayout.SetColumnSpan(headerLayout, 2);
			headerLayout.Controls.Add(label1, 0, 0);
			headerLayout.Controls.Add(themeComboBox, 1, 0);
			headerLayout.Controls.Add(languageComboBox, 2, 0);
			headerLayout.Name = "headerLayout";
			// 
			// middleLayout
			// 
			resources.ApplyResources(middleLayout, "middleLayout");
			middleLayout.Controls.Add(groupBox2, 0, 0);
			middleLayout.Controls.Add(groupBox3, 1, 0);
			middleLayout.Name = "middleLayout";
			// 
			// InteractiveForm
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(rootLayout);
			Name = "InteractiveForm";
			Load += InteractiveForm_Load;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox1Layout.ResumeLayout(false);
			groupBox1Layout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)thresholdSlider).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			notifyMenuStrip.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			groupBox4.ResumeLayout(false);
			testPicturePanel.ResumeLayout(false);
			testPicturePanel.PerformLayout();
			groupBoxDevice.ResumeLayout(false);
			groupBoxDevice.PerformLayout();
			groupBoxDeviceLayout.ResumeLayout(false);
			groupBoxDeviceLayout.PerformLayout();
			groupBoxGeneral.ResumeLayout(false);
			groupBoxGeneral.PerformLayout();
			groupBoxGeneralLayout.ResumeLayout(false);
			groupBoxGeneralLayout.PerformLayout();
			bottomPanel.ResumeLayout(false);
			bottomPanel.PerformLayout();
			bottomLayout.ResumeLayout(false);
			bottomLayout.PerformLayout();
			rootLayout.ResumeLayout(false);
			rootLayout.PerformLayout();
			headerLayout.ResumeLayout(false);
			headerLayout.PerformLayout();
			middleLayout.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private TextBox logTextBox;
        private Label delayLabel;
        private TextBox delayTextBox;
        private Button resetButton;
        private CheckBox runAtStartupCheckBox;
        private GroupBox groupBox1;
        private TableLayoutPanel groupBox1Layout;
        private PictureBox pictureBox1;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip notifyMenuStrip;
        private ToolStripMenuItem showUiMenu;
        private ToolStripMenuItem exitMenu;
        private Label label1;
        private TextBox descriptionTextBox;
        private RichTextBox richTextBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Panel testPicturePanel;
        private CheckBox right;
        private CheckBox left;
        private CheckBox x2;
        private CheckBox x1;
        private CheckBox middle;
        private CheckBox wheel;
        private ComboBox mouseButtonComboBox;
        private CheckBox buttonEnabledCheckBox;
        private TrackBar thresholdSlider;
        private CheckBox useMinDelayCheckBox;
        private CheckBox fixDraggingCheckBox;
        private CheckBox remoteDesktopCheckBox;
        private Label currentDeviceLabel;
        private CheckBox ignoreCurrentDeviceCheckBox;
        private TextBox dragStartDelayTextBox;
        private Label dragStartDelayLabel;
        private TextBox dragEndDelayTextBox;
        private Label dragEndDelayLabel;
        private ToolTip toolTip1;
        private Label versionLabel;
        private LinkLabel updateLinkLabel;
        private LinkLabel gitLinkLabel;
        private ComboBox themeComboBox;
        private ComboBox languageComboBox;
        private GroupBox groupBoxDevice;
        private TableLayoutPanel groupBoxDeviceLayout;
        private GroupBox groupBoxGeneral;
        private TableLayoutPanel groupBoxGeneralLayout;
        private Panel bottomPanel;
        private TableLayoutPanel bottomLayout;
        private TableLayoutPanel rootLayout;
        private TableLayoutPanel headerLayout;
        private TableLayoutPanel middleLayout;
    }
}
