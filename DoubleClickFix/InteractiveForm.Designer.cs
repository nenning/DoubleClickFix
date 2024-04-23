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
            x2 = new CheckBox();
            x1 = new CheckBox();
            middle = new CheckBox();
            right = new CheckBox();
            left = new CheckBox();
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
            groupBox1.SuspendLayout();
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
            groupBox1.Controls.Add(x2);
            groupBox1.Controls.Add(x1);
            groupBox1.Controls.Add(middle);
            groupBox1.Controls.Add(right);
            groupBox1.Controls.Add(left);
            groupBox1.Controls.Add(delayTextBox);
            groupBox1.Controls.Add(saveButton);
            groupBox1.Controls.Add(runAtStartupCheckBox);
            groupBox1.Controls.Add(delayLabel);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
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
            // descriptionTextBox
            // 
            descriptionTextBox.BackColor = SystemColors.Control;
            descriptionTextBox.BorderStyle = BorderStyle.None;
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
            groupBox4.Controls.Add(pictureBox1);
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // InteractiveForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox3);
            Controls.Add(label1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(groupBox4);
            Name = "InteractiveForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            notifyMenuStrip.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
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
    }
}
