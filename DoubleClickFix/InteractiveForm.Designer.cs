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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InteractiveForm));
            logTextBox = new TextBox();
            delayLabel = new Label();
            delayTextBox = new TextBox();
            saveButton = new Button();
            runAtStartupCheckBox = new CheckBox();
            groupBox1 = new GroupBox();
            pictureBox1 = new PictureBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // logTextBox
            // 
            logTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTextBox.Location = new Point(12, 277);
            logTextBox.Multiline = true;
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.Size = new Size(880, 607);
            logTextBox.TabIndex = 0;
            logTextBox.Text = resources.GetString("logTextBox.Text");
            logTextBox.TextChanged += LogTextBoxChanged;
            // 
            // delayLabel
            // 
            delayLabel.AutoSize = true;
            delayLabel.Location = new Point(23, 59);
            delayLabel.Name = "delayLabel";
            delayLabel.Size = new Size(268, 25);
            delayLabel.TabIndex = 1;
            delayLabel.Text = "Minimal double-click speed [ms]";
            // 
            // delayTextBox
            // 
            delayTextBox.Location = new Point(344, 56);
            delayTextBox.Name = "delayTextBox";
            delayTextBox.Size = new Size(82, 31);
            delayTextBox.TabIndex = 2;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(309, 174);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(117, 41);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += OnSaveButtonClicked;
            // 
            // runAtStartupCheckBox
            // 
            runAtStartupCheckBox.AutoSize = true;
            runAtStartupCheckBox.Location = new Point(23, 111);
            runAtStartupCheckBox.Name = "runAtStartupCheckBox";
            runAtStartupCheckBox.Size = new Size(150, 29);
            runAtStartupCheckBox.TabIndex = 4;
            runAtStartupCheckBox.Text = "Run at startup";
            runAtStartupCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(delayTextBox);
            groupBox1.Controls.Add(saveButton);
            groupBox1.Controls.Add(runAtStartupCheckBox);
            groupBox1.Controls.Add(delayLabel);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(457, 238);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Settings";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.None;
            pictureBox1.Enabled = false;
            pictureBox1.Image = Properties.Resources.app;
            pictureBox1.Location = new Point(653, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(239, 238);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // InteractiveForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(904, 905);
            Controls.Add(pictureBox1);
            Controls.Add(logTextBox);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "InteractiveForm";
            Text = "Fix wrong double mouse clicks";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
    }
}
