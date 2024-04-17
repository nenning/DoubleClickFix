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
            SuspendLayout();
            // 
            // textBox
            // 
            logTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTextBox.Location = new Point(12, 124);
            logTextBox.Multiline = true;
            logTextBox.Name = "textBox";
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.Size = new Size(554, 445);
            logTextBox.TabIndex = 0;
            logTextBox.Text = "Elapsed time in milliseconds between two mouse clicks:\r\n";
            // 
            // label1
            // 
            delayLabel.AutoSize = true;
            delayLabel.Location = new Point(12, 48);
            delayLabel.Name = "label1";
            delayLabel.Size = new Size(268, 25);
            delayLabel.TabIndex = 1;
            delayLabel.Text = "Minimal double-click speed [ms]";
            // 
            // textBox1
            // 
            delayTextBox.Location = new Point(309, 45);
            delayTextBox.Name = "textBox1";
            delayTextBox.Size = new Size(82, 31);
            delayTextBox.TabIndex = 2;
            // 
            // button1
            // 
            saveButton.Location = new Point(427, 40);
            saveButton.Name = "button1";
            saveButton.Size = new Size(117, 41);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += OnSaveButtonClicked;
            // 
            // InteractiveForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(578, 590);
            Controls.Add(saveButton);
            Controls.Add(delayTextBox);
            Controls.Add(delayLabel);
            Controls.Add(logTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "InteractiveForm";
            Text = "Fix wrong double mouse clicks";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox logTextBox;
        private Label delayLabel;
        private TextBox delayTextBox;
        private Button saveButton;

    }
}
