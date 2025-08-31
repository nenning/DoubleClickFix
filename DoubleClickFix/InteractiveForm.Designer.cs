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
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InteractiveForm));
            logTextBox = new TextBox();
            delayLabel = new Label();
            delayTextBox = new TextBox();
            saveButton = new Button();
            runAtStartupCheckBox = new CheckBox();
            groupBox1 = new GroupBox();
            dragEndDelayTextBox = new TextBox();
            dragEndDelayLabel = new Label();
            dragStartDelayTextBox = new TextBox();
            dragStartDelayLabel = new Label();
            fixDraggingCheckBox = new CheckBox();
            useMinDelayCheckBox = new CheckBox();
            thresholdSlider = new TrackBar();
            buttonEnabledCheckBox = new CheckBox();
            mouseButtonComboBox = new ComboBox();
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
            toolTip1 = new ToolTip(components);
            versionLabel = new Label();
            gitLinkLabel = new LinkLabel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)thresholdSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            notifyMenuStrip.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
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
            // saveButton
            // 
            resources.ApplyResources(saveButton, "saveButton");
            saveButton.Name = "saveButton";
            toolTip1.SetToolTip(saveButton, resources.GetString("saveButton.ToolTip"));
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += OnSaveButtonClicked;
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
            groupBox1.Controls.Add(dragEndDelayTextBox);
            groupBox1.Controls.Add(dragEndDelayLabel);
            groupBox1.Controls.Add(dragStartDelayTextBox);
            groupBox1.Controls.Add(dragStartDelayLabel);
            groupBox1.Controls.Add(fixDraggingCheckBox);
            groupBox1.Controls.Add(saveButton);
            groupBox1.Controls.Add(useMinDelayCheckBox);
            groupBox1.Controls.Add(thresholdSlider);
            groupBox1.Controls.Add(buttonEnabledCheckBox);
            groupBox1.Controls.Add(mouseButtonComboBox);
            groupBox1.Controls.Add(delayTextBox);
            groupBox1.Controls.Add(runAtStartupCheckBox);
            groupBox1.Controls.Add(delayLabel);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // dragEndDelayTextBox
            // 
            resources.ApplyResources(dragEndDelayTextBox, "dragEndDelayTextBox");
            dragEndDelayTextBox.Name = "dragEndDelayTextBox";
            dragEndDelayTextBox.TextChanged += OnDragStopDelayTextChanged;
            // 
            // dragEndDelayLabel
            // 
            resources.ApplyResources(dragEndDelayLabel, "dragEndDelayLabel");
            dragEndDelayLabel.Name = "dragEndDelayLabel";
            toolTip1.SetToolTip(dragEndDelayLabel, resources.GetString("dragEndDelayLabel.ToolTip"));
            // 
            // dragStartDelayTextBox
            // 
            resources.ApplyResources(dragStartDelayTextBox, "dragStartDelayTextBox");
            dragStartDelayTextBox.Name = "dragStartDelayTextBox";
            dragStartDelayTextBox.TextChanged += OnDragStartDelayTextChanged;
            // 
            // dragStartDelayLabel
            // 
            resources.ApplyResources(dragStartDelayLabel, "dragStartDelayLabel");
            dragStartDelayLabel.Name = "dragStartDelayLabel";
            toolTip1.SetToolTip(dragStartDelayLabel, resources.GetString("dragStartDelayLabel.ToolTip"));
            // 
            // fixDraggingCheckBox
            // 
            resources.ApplyResources(fixDraggingCheckBox, "fixDraggingCheckBox");
            fixDraggingCheckBox.Name = "fixDraggingCheckBox";
            toolTip1.SetToolTip(fixDraggingCheckBox, resources.GetString("fixDraggingCheckBox.ToolTip"));
            fixDraggingCheckBox.UseVisualStyleBackColor = true;
            fixDraggingCheckBox.CheckedChanged += OnFixDraggingCheckBoxChanged;
            // 
            // useMinDelayCheckBox
            // 
            resources.ApplyResources(useMinDelayCheckBox, "useMinDelayCheckBox");
            useMinDelayCheckBox.Name = "useMinDelayCheckBox";
            toolTip1.SetToolTip(useMinDelayCheckBox, resources.GetString("useMinDelayCheckBox.ToolTip"));
            useMinDelayCheckBox.UseVisualStyleBackColor = true;
            useMinDelayCheckBox.CheckedChanged += UseMinDelayCheckBoxCheckedChanged;
            // 
            // thresholdSlider
            // 
            resources.ApplyResources(thresholdSlider, "thresholdSlider");
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
            mouseButtonComboBox.Name = "mouseButtonComboBox";
            mouseButtonComboBox.SelectedIndexChanged += OnSelectedMouseButtonChanged;
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
            resources.ApplyResources(descriptionTextBox, "descriptionTextBox");
            descriptionTextBox.BackColor = SystemColors.Control;
            descriptionTextBox.BorderStyle = BorderStyle.None;
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
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(descriptionTextBox);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(logTextBox);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // groupBox4
            // 
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Controls.Add(x2);
            groupBox4.Controls.Add(x1);
            groupBox4.Controls.Add(middle);
            groupBox4.Controls.Add(left);
            groupBox4.Controls.Add(right);
            groupBox4.Controls.Add(wheel);
            groupBox4.Controls.Add(richTextBox1);
            groupBox4.Controls.Add(pictureBox1);
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // versionLabel
            // 
            resources.ApplyResources(versionLabel, "versionLabel");
            versionLabel.Name = "versionLabel";
            // 
            // gitLinkLabel
            // 
            resources.ApplyResources(gitLinkLabel, "gitLinkLabel");
            gitLinkLabel.Name = "gitLinkLabel";
            gitLinkLabel.TabStop = true;
            gitLinkLabel.LinkClicked += OnGitLinkLabelClicked;
            // 
            // InteractiveForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(gitLinkLabel);
            Controls.Add(versionLabel);
            Controls.Add(groupBox3);
            Controls.Add(label1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(groupBox4);
            Name = "InteractiveForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)thresholdSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            notifyMenuStrip.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox logTextBox;
        private Label delayLabel;
        private TextBox delayTextBox;
        private Button saveButton;
        private CheckBox runAtStartupCheckBox;
        private GroupBox groupBox1;
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
        private TextBox dragStartDelayTextBox;
        private Label dragStartDelayLabel;
        private TextBox dragEndDelayTextBox;
        private Label dragEndDelayLabel;
        private ToolTip toolTip1;
        private Label versionLabel;
        private LinkLabel gitLinkLabel;
    }
}
