namespace WinFormsClient.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BackButton = new Button();
            LoginButton = new Button();
            PasswordTextBox = new TextBox();
            UsernameTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            ErrorLabel = new Label();
            label3 = new Label();
            RememberMeCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // BackButton
            // 
            BackButton.Location = new Point(12, 265);
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(75, 23);
            BackButton.TabIndex = 0;
            BackButton.Text = "Back";
            BackButton.UseVisualStyleBackColor = true;
            BackButton.Click += BackButton_Click;
            // 
            // LoginButton
            // 
            LoginButton.Location = new Point(241, 190);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(75, 23);
            LoginButton.TabIndex = 1;
            LoginButton.Text = "Login";
            LoginButton.UseVisualStyleBackColor = true;
            LoginButton.Click += LoginButton_Click;
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Location = new Point(85, 146);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.Size = new Size(231, 23);
            PasswordTextBox.TabIndex = 2;
            // 
            // UsernameTextBox
            // 
            UsernameTextBox.Location = new Point(85, 100);
            UsernameTextBox.Name = "UsernameTextBox";
            UsernameTextBox.Size = new Size(231, 23);
            UsernameTextBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(85, 128);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 4;
            label1.Text = "Password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(85, 82);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 5;
            label2.Text = "Username";
            // 
            // ErrorLabel
            // 
            ErrorLabel.AutoSize = true;
            ErrorLabel.Location = new Point(178, 243);
            ErrorLabel.Name = "ErrorLabel";
            ErrorLabel.Size = new Size(0, 15);
            ErrorLabel.TabIndex = 6;
            ErrorLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 9);
            label3.Name = "label3";
            label3.Size = new Size(37, 15);
            label3.TabIndex = 7;
            label3.Text = "Login";
            // 
            // RememberMeCheckBox
            // 
            RememberMeCheckBox.AutoSize = true;
            RememberMeCheckBox.Location = new Point(85, 190);
            RememberMeCheckBox.Name = "RememberMeCheckBox";
            RememberMeCheckBox.Size = new Size(104, 19);
            RememberMeCheckBox.TabIndex = 8;
            RememberMeCheckBox.Text = "Remember me";
            RememberMeCheckBox.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 300);
            Controls.Add(RememberMeCheckBox);
            Controls.Add(label3);
            Controls.Add(ErrorLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(UsernameTextBox);
            Controls.Add(PasswordTextBox);
            Controls.Add(LoginButton);
            Controls.Add(BackButton);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Shotgun - Login";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BackButton;
        private Button LoginButton;
        private TextBox PasswordTextBox;
        private TextBox UsernameTextBox;
        private Label label1;
        private Label label2;
        private Label ErrorLabel;
        private Label label3;
        private CheckBox RememberMeCheckBox;
    }
}