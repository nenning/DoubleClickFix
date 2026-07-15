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
			dragEndDelayTextBox = new TextBox();
			dragEndDelayLabel = new Label();
			dragStartDelayTextBox = new TextBox();
			dragStartDelayLabel = new Label();
			fixDraggingCheckBox = new CheckBox();
			useMinDelayCheckBox = new CheckBox();
			thresholdSlider = new TrackBar();
			buttonEnabledCheckBox = new CheckBox();
			mouseButtonComboBox = new ComboBox();
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
			logTextBox.Dock = DockStyle.Fill;
			logTextBox.Name = "logTextBox";
			logTextBox.ReadOnly = true;
			logTextBox.TabStop = false;
			logTextBox.TextChanged += OnLogTextBoxChanged;
			//
			// delayLabel
			//
			resources.ApplyResources(delayLabel, "delayLabel");
			delayLabel.AutoSize = true;
			delayLabel.Anchor = AnchorStyles.Left;
			delayLabel.Margin = new Padding(3, 4, 3, 4);
			delayLabel.Name = "delayLabel";
			toolTip1.SetToolTip(delayLabel, resources.GetString("delayLabel.ToolTip"));
			//
			// delayTextBox
			//
			resources.ApplyResources(delayTextBox, "delayTextBox");
			delayTextBox.Anchor = AnchorStyles.Left;
			delayTextBox.Margin = new Padding(9, 1, 3, 1);
			delayTextBox.Name = "delayTextBox";
			delayTextBox.Width = 70;
			toolTip1.SetToolTip(delayTextBox, resources.GetString("delayTextBox.ToolTip"));
			//
			// resetButton
			//
			resources.ApplyResources(resetButton, "resetButton");
			resetButton.Anchor = AnchorStyles.Right;
			resetButton.AutoSize = true;
			resetButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			resetButton.Margin = new Padding(9, 1, 3, 1);
			resetButton.Padding = new Padding(16, 4, 16, 4);
			resetButton.Name = "resetButton";
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += OnResetButtonClicked;
			//
			// runAtStartupCheckBox
			//
			resources.ApplyResources(runAtStartupCheckBox, "runAtStartupCheckBox");
			runAtStartupCheckBox.AutoSize = true;
			runAtStartupCheckBox.Anchor = AnchorStyles.Left;
			runAtStartupCheckBox.Margin = new Padding(3, 4, 3, 4);
			runAtStartupCheckBox.Name = "runAtStartupCheckBox";
			toolTip1.SetToolTip(runAtStartupCheckBox, resources.GetString("runAtStartupCheckBox.ToolTip"));
			runAtStartupCheckBox.UseVisualStyleBackColor = true;
			//
			// groupBox1
			//
			groupBox1.Controls.Add(groupBox1Layout);
			resources.ApplyResources(groupBox1, "groupBox1");
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBox1.Dock = DockStyle.Top;
			groupBox1.Margin = new Padding(0, 4, 0, 0);
			groupBox1.Name = "groupBox1";
			groupBox1.TabStop = false;
			//
			// groupBox1Layout
			//
			groupBox1Layout.AutoSize = true;
			groupBox1Layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBox1Layout.Dock = DockStyle.Fill;
			groupBox1Layout.ColumnCount = 3;
			groupBox1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			groupBox1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			groupBox1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			groupBox1Layout.RowCount = 5;
			groupBox1Layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBox1Layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBox1Layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBox1Layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBox1Layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBox1Layout.Controls.Add(mouseButtonComboBox, 0, 0);
			groupBox1Layout.Controls.Add(buttonEnabledCheckBox, 1, 0);
			groupBox1Layout.Controls.Add(delayLabel, 0, 1);
			groupBox1Layout.Controls.Add(thresholdSlider, 1, 1);
			groupBox1Layout.Controls.Add(delayTextBox, 2, 1);
			groupBox1Layout.Controls.Add(useMinDelayCheckBox, 0, 2);
			groupBox1Layout.SetColumnSpan(useMinDelayCheckBox, 3);
			groupBox1Layout.Controls.Add(fixDraggingCheckBox, 0, 3);
			groupBox1Layout.Controls.Add(dragStartDelayLabel, 1, 3);
			groupBox1Layout.Controls.Add(dragStartDelayTextBox, 2, 3);
			groupBox1Layout.Controls.Add(dragEndDelayLabel, 1, 4);
			groupBox1Layout.Controls.Add(dragEndDelayTextBox, 2, 4);
			groupBox1Layout.Name = "groupBox1Layout";
			groupBox1Layout.Padding = new Padding(4, 10, 4, 10);
			//
			// dragEndDelayTextBox
			//
			resources.ApplyResources(dragEndDelayTextBox, "dragEndDelayTextBox");
			dragEndDelayTextBox.Anchor = AnchorStyles.Left;
			dragEndDelayTextBox.Margin = new Padding(9, 1, 3, 3);
			dragEndDelayTextBox.Name = "dragEndDelayTextBox";
			dragEndDelayTextBox.Width = 70;
			dragEndDelayTextBox.TextChanged += OnDragStopDelayTextChanged;
			//
			// dragEndDelayLabel
			//
			resources.ApplyResources(dragEndDelayLabel, "dragEndDelayLabel");
			dragEndDelayLabel.AutoSize = true;
			dragEndDelayLabel.Anchor = AnchorStyles.Left;
			dragEndDelayLabel.Margin = new Padding(3, 1, 3, 4);
			dragEndDelayLabel.Name = "dragEndDelayLabel";
			toolTip1.SetToolTip(dragEndDelayLabel, resources.GetString("dragEndDelayLabel.ToolTip"));
			//
			// dragStartDelayTextBox
			//
			resources.ApplyResources(dragStartDelayTextBox, "dragStartDelayTextBox");
			dragStartDelayTextBox.Anchor = AnchorStyles.Left;
			dragStartDelayTextBox.Margin = new Padding(9, 3, 3, 1);
			dragStartDelayTextBox.Name = "dragStartDelayTextBox";
			dragStartDelayTextBox.Width = 70;
			dragStartDelayTextBox.TextChanged += OnDragStartDelayTextChanged;
			//
			// dragStartDelayLabel
			//
			resources.ApplyResources(dragStartDelayLabel, "dragStartDelayLabel");
			dragStartDelayLabel.AutoSize = true;
			dragStartDelayLabel.Anchor = AnchorStyles.Left;
			dragStartDelayLabel.Margin = new Padding(3, 4, 3, 1);
			dragStartDelayLabel.Name = "dragStartDelayLabel";
			toolTip1.SetToolTip(dragStartDelayLabel, resources.GetString("dragStartDelayLabel.ToolTip"));
			//
			// fixDraggingCheckBox
			//
			resources.ApplyResources(fixDraggingCheckBox, "fixDraggingCheckBox");
			fixDraggingCheckBox.AutoSize = true;
			fixDraggingCheckBox.Anchor = AnchorStyles.Left;
			fixDraggingCheckBox.Margin = new Padding(3, 4, 3, 4);
			fixDraggingCheckBox.Name = "fixDraggingCheckBox";
			toolTip1.SetToolTip(fixDraggingCheckBox, resources.GetString("fixDraggingCheckBox.ToolTip"));
			fixDraggingCheckBox.UseVisualStyleBackColor = true;
			fixDraggingCheckBox.CheckedChanged += OnFixDraggingCheckBoxChanged;
			//
			// useMinDelayCheckBox
			//
			resources.ApplyResources(useMinDelayCheckBox, "useMinDelayCheckBox");
			useMinDelayCheckBox.AutoSize = true;
			useMinDelayCheckBox.Anchor = AnchorStyles.Left;
			useMinDelayCheckBox.Margin = new Padding(3, 4, 3, 4);
			useMinDelayCheckBox.Name = "useMinDelayCheckBox";
			toolTip1.SetToolTip(useMinDelayCheckBox, resources.GetString("useMinDelayCheckBox.ToolTip"));
			useMinDelayCheckBox.UseVisualStyleBackColor = true;
			useMinDelayCheckBox.CheckedChanged += UseMinDelayCheckBoxCheckedChanged;
			//
			// thresholdSlider
			//
			resources.ApplyResources(thresholdSlider, "thresholdSlider");
			thresholdSlider.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			thresholdSlider.Margin = new Padding(9, 3, 9, 3);
			thresholdSlider.Height = 56;
			thresholdSlider.LargeChange = 20;
			thresholdSlider.Maximum = 200;
			thresholdSlider.Minimum = -1;
			thresholdSlider.Name = "thresholdSlider";
			thresholdSlider.TickFrequency = 10;
			thresholdSlider.ValueChanged += OnThresholdValueChanged;
			//
			// buttonEnabledCheckBox
			//
			resources.ApplyResources(buttonEnabledCheckBox, "buttonEnabledCheckBox");
			buttonEnabledCheckBox.AutoSize = true;
			buttonEnabledCheckBox.Anchor = AnchorStyles.Left;
			buttonEnabledCheckBox.Margin = new Padding(24, 4, 3, 4);
			buttonEnabledCheckBox.Name = "buttonEnabledCheckBox";
			toolTip1.SetToolTip(buttonEnabledCheckBox, resources.GetString("buttonEnabledCheckBox.ToolTip"));
			buttonEnabledCheckBox.UseVisualStyleBackColor = true;
			buttonEnabledCheckBox.CheckedChanged += OnButtonEnabledCheckedChanged;
			//
			// mouseButtonComboBox
			//
			mouseButtonComboBox.FormattingEnabled = true;
			mouseButtonComboBox.Items.AddRange(new object[] { resources.GetString("mouseButtonComboBox.Items"), resources.GetString("mouseButtonComboBox.Items1"), resources.GetString("mouseButtonComboBox.Items2"), resources.GetString("mouseButtonComboBox.Items3"), resources.GetString("mouseButtonComboBox.Items4"), resources.GetString("mouseButtonComboBox.Items5") });
			resources.ApplyResources(mouseButtonComboBox, "mouseButtonComboBox");
			mouseButtonComboBox.Anchor = AnchorStyles.Left;
			mouseButtonComboBox.Margin = new Padding(3, 3, 3, 3);
			mouseButtonComboBox.Width = 260;
			mouseButtonComboBox.Name = "mouseButtonComboBox";
			mouseButtonComboBox.SelectedIndexChanged += OnSelectedMouseButtonChanged;
			//
			// remoteDesktopCheckBox
			//
			resources.ApplyResources(remoteDesktopCheckBox, "remoteDesktopCheckBox");
			remoteDesktopCheckBox.AutoSize = true;
			remoteDesktopCheckBox.Anchor = AnchorStyles.Left;
			remoteDesktopCheckBox.Margin = new Padding(3, 4, 3, 4);
			remoteDesktopCheckBox.Name = "remoteDesktopCheckBox";
			toolTip1.SetToolTip(remoteDesktopCheckBox, resources.GetString("remoteDesktopCheckBox.ToolTip"));
			remoteDesktopCheckBox.UseVisualStyleBackColor = true;
			remoteDesktopCheckBox.CheckedChanged += OnRemoteDesktopCheckBoxChanged;
			//
			// currentDeviceLabel
			//
			resources.ApplyResources(currentDeviceLabel, "currentDeviceLabel");
			currentDeviceLabel.AutoSize = true;
			currentDeviceLabel.Anchor = AnchorStyles.Left;
			currentDeviceLabel.Margin = new Padding(3, 4, 3, 4);
			currentDeviceLabel.Name = "currentDeviceLabel";
			//
			// ignoreCurrentDeviceCheckBox
			//
			resources.ApplyResources(ignoreCurrentDeviceCheckBox, "ignoreCurrentDeviceCheckBox");
			ignoreCurrentDeviceCheckBox.AutoSize = true;
			ignoreCurrentDeviceCheckBox.Anchor = AnchorStyles.Right;
			ignoreCurrentDeviceCheckBox.Margin = new Padding(24, 4, 3, 4);
			ignoreCurrentDeviceCheckBox.Name = "ignoreCurrentDeviceCheckBox";
			ignoreCurrentDeviceCheckBox.UseVisualStyleBackColor = true;
			ignoreCurrentDeviceCheckBox.CheckedChanged += OnIgnoreCurrentDeviceCheckBoxChanged;
			//
			// x2
			//
			resources.ApplyResources(x2, "x2");
			x2.AutoSize = true;
			x2.Location = new Point(28, 152);
			x2.Name = "x2";
			x2.UseVisualStyleBackColor = true;
			//
			// x1
			//
			resources.ApplyResources(x1, "x1");
			x1.AutoSize = true;
			x1.Location = new Point(28, 209);
			x1.Name = "x1";
			x1.UseVisualStyleBackColor = true;
			//
			// middle
			//
			resources.ApplyResources(middle, "middle");
			middle.AutoSize = true;
			middle.Location = new Point(101, 59);
			middle.Name = "middle";
			middle.UseVisualStyleBackColor = true;
			//
			// right
			//
			resources.ApplyResources(right, "right");
			right.AutoSize = true;
			right.Location = new Point(198, 97);
			right.Name = "right";
			right.UseVisualStyleBackColor = true;
			//
			// left
			//
			resources.ApplyResources(left, "left");
			left.AutoSize = true;
			left.Location = new Point(28, 97);
			left.Name = "left";
			left.UseVisualStyleBackColor = true;
			//
			// wheel
			//
			resources.ApplyResources(wheel, "wheel");
			wheel.AutoSize = true;
			wheel.Location = new Point(124, 179);
			wheel.Name = "wheel";
			wheel.UseVisualStyleBackColor = true;
			//
			// pictureBox1
			//
			resources.ApplyResources(pictureBox1, "pictureBox1");
			pictureBox1.Image = Properties.Resources.app;
			pictureBox1.Location = new Point(19, 42);
			pictureBox1.Size = new Size(259, 291);
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
			label1.AutoSize = true;
			label1.Anchor = AnchorStyles.Left;
			label1.Margin = new Padding(3, 0, 3, 2);
			label1.Name = "label1";
			//
			// descriptionTextBox
			//
			resources.ApplyResources(descriptionTextBox, "descriptionTextBox");
			descriptionTextBox.BackColor = SystemColors.Control;
			descriptionTextBox.Dock = DockStyle.Fill;
			descriptionTextBox.Name = "descriptionTextBox";
			descriptionTextBox.ReadOnly = true;
			descriptionTextBox.TabStop = false;
			//
			// richTextBox1
			//
			resources.ApplyResources(richTextBox1, "richTextBox1");
			richTextBox1.Dock = DockStyle.Fill;
			richTextBox1.Name = "richTextBox1";
			richTextBox1.ReadOnly = true;
			richTextBox1.MouseEnter += OnShowTestControls;
			richTextBox1.MouseLeave += OnHideTestControls;
			//
			// groupBox2
			//
			resources.ApplyResources(groupBox2, "groupBox2");
			groupBox2.Controls.Add(descriptionTextBox);
			groupBox2.Dock = DockStyle.Fill;
			groupBox2.Name = "groupBox2";
			groupBox2.TabStop = false;
			//
			// groupBox3
			//
			resources.ApplyResources(groupBox3, "groupBox3");
			groupBox3.Controls.Add(logTextBox);
			groupBox3.Dock = DockStyle.Fill;
			groupBox3.Name = "groupBox3";
			groupBox3.TabStop = false;
			//
			// groupBox4
			//
			resources.ApplyResources(groupBox4, "groupBox4");
			groupBox4.Controls.Add(richTextBox1);
			groupBox4.Controls.Add(testPicturePanel);
			groupBox4.Dock = DockStyle.Left;
			groupBox4.Margin = new Padding(0);
			groupBox4.Width = 301;
			groupBox4.Name = "groupBox4";
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
			testPicturePanel.Dock = DockStyle.Top;
			testPicturePanel.Height = 340;
			testPicturePanel.Name = "testPicturePanel";
			//
			// groupBoxDevice
			//
			groupBoxDevice.Controls.Add(groupBoxDeviceLayout);
			resources.ApplyResources(groupBoxDevice, "groupBoxDevice");
			groupBoxDevice.AutoSize = true;
			groupBoxDevice.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBoxDevice.Dock = DockStyle.Top;
			groupBoxDevice.Margin = new Padding(0, 4, 0, 0);
			groupBoxDevice.Name = "groupBoxDevice";
			groupBoxDevice.TabStop = false;
			//
			// groupBoxDeviceLayout
			//
			groupBoxDeviceLayout.AutoSize = true;
			groupBoxDeviceLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBoxDeviceLayout.Dock = DockStyle.Fill;
			groupBoxDeviceLayout.ColumnCount = 2;
			groupBoxDeviceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			groupBoxDeviceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			groupBoxDeviceLayout.RowCount = 1;
			groupBoxDeviceLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBoxDeviceLayout.Controls.Add(currentDeviceLabel, 0, 0);
			groupBoxDeviceLayout.Controls.Add(ignoreCurrentDeviceCheckBox, 1, 0);
			groupBoxDeviceLayout.Name = "groupBoxDeviceLayout";
			groupBoxDeviceLayout.Padding = new Padding(4, 7, 4, 7);
			//
			// groupBoxGeneral
			//
			groupBoxGeneral.Controls.Add(groupBoxGeneralLayout);
			resources.ApplyResources(groupBoxGeneral, "groupBoxGeneral");
			groupBoxGeneral.AutoSize = true;
			groupBoxGeneral.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBoxGeneral.Dock = DockStyle.Top;
			groupBoxGeneral.Margin = new Padding(0, 4, 0, 0);
			groupBoxGeneral.Name = "groupBoxGeneral";
			groupBoxGeneral.TabStop = false;
			//
			// groupBoxGeneralLayout
			//
			groupBoxGeneralLayout.AutoSize = true;
			groupBoxGeneralLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBoxGeneralLayout.Dock = DockStyle.Fill;
			groupBoxGeneralLayout.ColumnCount = 2;
			groupBoxGeneralLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			groupBoxGeneralLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			groupBoxGeneralLayout.RowCount = 2;
			groupBoxGeneralLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBoxGeneralLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			groupBoxGeneralLayout.Controls.Add(remoteDesktopCheckBox, 0, 0);
			groupBoxGeneralLayout.Controls.Add(resetButton, 1, 0);
			groupBoxGeneralLayout.SetRowSpan(resetButton, 2);
			groupBoxGeneralLayout.Controls.Add(runAtStartupCheckBox, 0, 1);
			groupBoxGeneralLayout.Name = "groupBoxGeneralLayout";
			groupBoxGeneralLayout.Padding = new Padding(4);
			//
			// versionLabel
			//
			resources.ApplyResources(versionLabel, "versionLabel");
			versionLabel.AutoSize = true;
			versionLabel.Anchor = AnchorStyles.Right;
			versionLabel.Margin = new Padding(9, 12, 12, 12);
			versionLabel.Name = "versionLabel";
			//
			// updateLinkLabel
			//
			resources.ApplyResources(updateLinkLabel, "updateLinkLabel");
			updateLinkLabel.AutoSize = true;
			updateLinkLabel.Anchor = AnchorStyles.Right;
			updateLinkLabel.Margin = new Padding(9, 12, 9, 12);
			updateLinkLabel.Name = "updateLinkLabel";
			updateLinkLabel.TabStop = true;
			//
			// gitLinkLabel
			//
			resources.ApplyResources(gitLinkLabel, "gitLinkLabel");
			gitLinkLabel.AutoSize = true;
			gitLinkLabel.Anchor = AnchorStyles.Left;
			gitLinkLabel.Margin = new Padding(12, 12, 9, 12);
			gitLinkLabel.Name = "gitLinkLabel";
			gitLinkLabel.TabStop = true;
			gitLinkLabel.LinkClicked += OnGitLinkLabelClicked;
			//
			// themeComboBox
			//
			resources.ApplyResources(themeComboBox, "themeComboBox");
			themeComboBox.Anchor = AnchorStyles.Right;
			themeComboBox.Margin = new Padding(9, 1, 3, 1);
			themeComboBox.Width = 130;
			themeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			themeComboBox.FormattingEnabled = true;
			themeComboBox.Name = "themeComboBox";
			//
			// languageComboBox
			//
			resources.ApplyResources(languageComboBox, "languageComboBox");
			languageComboBox.Anchor = AnchorStyles.Right;
			languageComboBox.Margin = new Padding(9, 1, 3, 1);
			languageComboBox.Width = 221;
			languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			languageComboBox.FormattingEnabled = true;
			languageComboBox.Name = "languageComboBox";
			languageComboBox.SelectedIndexChanged += OnLanguageChanged;
			//
			// bottomLayout
			//
			bottomLayout.AutoSize = true;
			bottomLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			bottomLayout.Dock = DockStyle.Fill;
			bottomLayout.ColumnCount = 3;
			bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			bottomLayout.RowCount = 1;
			bottomLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			bottomLayout.Controls.Add(gitLinkLabel, 0, 0);
			bottomLayout.Controls.Add(updateLinkLabel, 1, 0);
			bottomLayout.Controls.Add(versionLabel, 2, 0);
			bottomLayout.Name = "bottomLayout";
			bottomLayout.Padding = new Padding(12, 8, 12, 8);
			//
			// bottomPanel
			//
			bottomPanel.Controls.Add(bottomLayout);
			bottomPanel.Dock = DockStyle.Top;
			bottomPanel.Margin = new Padding(0, 4, 0, 0);
			bottomPanel.AutoSize = true;
			bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			bottomPanel.Name = "bottomPanel";
			//
			// headerLayout
			//
			headerLayout.AutoSize = true;
			headerLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			headerLayout.Dock = DockStyle.Top;
			headerLayout.ColumnCount = 3;
			headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			headerLayout.RowCount = 1;
			headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			headerLayout.Controls.Add(label1, 0, 0);
			headerLayout.Controls.Add(themeComboBox, 1, 0);
			headerLayout.Controls.Add(languageComboBox, 2, 0);
			headerLayout.Name = "headerLayout";
			headerLayout.Padding = new Padding(12, 0, 12, 0);
			headerLayout.Margin = new Padding(0);
			//
			// middleLayout
			//
			middleLayout.Dock = DockStyle.Fill;
			middleLayout.Margin = new Padding(0);
			middleLayout.ColumnCount = 2;
			middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
			middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
			middleLayout.RowCount = 1;
			middleLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			middleLayout.Controls.Add(groupBox2, 0, 0);
			middleLayout.Controls.Add(groupBox3, 1, 0);
			middleLayout.Name = "middleLayout";
			middleLayout.Padding = new Padding(0, 6, 0, 6);
			//
			// rootLayout
			//
			rootLayout.Dock = DockStyle.Fill;
			rootLayout.Padding = new Padding(13, 0, 13, 0);
			rootLayout.ColumnCount = 2;
			rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			rootLayout.RowCount = 6;
			rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			rootLayout.Controls.Add(headerLayout, 0, 0);
			rootLayout.SetColumnSpan(headerLayout, 2);
			rootLayout.Controls.Add(groupBoxDevice, 0, 1);
			rootLayout.Controls.Add(groupBox1, 0, 2);
			rootLayout.Controls.Add(groupBoxGeneral, 0, 3);
			rootLayout.Controls.Add(middleLayout, 0, 4);
			rootLayout.Controls.Add(bottomPanel, 0, 5);
			rootLayout.SetColumnSpan(bottomPanel, 2);
			rootLayout.Controls.Add(groupBox4, 1, 1);
			rootLayout.SetRowSpan(groupBox4, 4);
			rootLayout.Name = "rootLayout";
			//
			// InteractiveForm
			//
			resources.ApplyResources(this, "$this");
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1305, 1097);
			Controls.Add(rootLayout);
			Name = "InteractiveForm";
			Load += InteractiveForm_Load;
			groupBox1.ResumeLayout(false);
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
			groupBoxDeviceLayout.ResumeLayout(false);
			groupBoxDeviceLayout.PerformLayout();
			groupBoxGeneral.ResumeLayout(false);
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
