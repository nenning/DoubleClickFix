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
            pictureBox1 = new PictureBox();
            notifyIcon = new NotifyIcon(components);
            notifyMenuStrip = new ContextMenuStrip(components);
            showUiMenu = new ToolStripMenuItem();
            exitMenu = new ToolStripMenuItem();
            label1 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            notifyMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // logTextBox
            // 
            resources.ApplyResources(logTextBox, "logTextBox");
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.TabStop = false;
            logTextBox.TextChanged += LogTextBoxChanged;
            // 
            // delayLabel
            // 
            resources.ApplyResources(delayLabel, "delayLabel");
            delayLabel.Name = "delayLabel";
            // 
            // delayTextBox
            // 
            resources.ApplyResources(delayTextBox, "delayTextBox");
            delayTextBox.Name = "delayTextBox";
            // 
            // saveButton
            // 
            resources.ApplyResources(saveButton, "saveButton");
            saveButton.Name = "saveButton";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += OnSaveButtonClicked;
            // 
            // runAtStartupCheckBox
            // 
            resources.ApplyResources(runAtStartupCheckBox, "runAtStartupCheckBox");
            runAtStartupCheckBox.Name = "runAtStartupCheckBox";
            runAtStartupCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(delayTextBox);
            groupBox1.Controls.Add(saveButton);
            groupBox1.Controls.Add(runAtStartupCheckBox);
            groupBox1.Controls.Add(delayLabel);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.app;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = notifyMenuStrip;
            resources.ApplyResources(notifyIcon, "notifyIcon");
            notifyIcon.MouseDoubleClick += NotifyIconDoubleClick;
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
            showUiMenu.Click += ShowUiMenuClick;
            // 
            // exitMenu
            // 
            exitMenu.Name = "exitMenu";
            resources.ApplyResources(exitMenu, "exitMenu");
            exitMenu.Click += ExitMenuClick;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // InteractiveForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(logTextBox);
            Controls.Add(groupBox1);
            Name = "InteractiveForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            notifyMenuStrip.ResumeLayout(false);
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
    }
}
