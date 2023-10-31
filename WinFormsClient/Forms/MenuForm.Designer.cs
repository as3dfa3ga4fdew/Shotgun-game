namespace WinFormsClient.Forms
{
    partial class MenuForm
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
            LoginButton = new Button();
            RegisterButton = new Button();
            QuitButton = new Button();
            LogoutButton = new Button();
            JoinButton = new Button();
            CreateButton = new Button();
            label1 = new Label();
            UsernameLabel = new Label();
            GameListDataGridView = new DataGridView();
            WaitingLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)GameListDataGridView).BeginInit();
            SuspendLayout();
            // 
            // LoginButton
            // 
            LoginButton.Location = new Point(12, 12);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(75, 23);
            LoginButton.TabIndex = 0;
            LoginButton.Text = "Login";
            LoginButton.UseVisualStyleBackColor = true;
            LoginButton.Click += LoginButton_Click;
            // 
            // RegisterButton
            // 
            RegisterButton.Location = new Point(93, 12);
            RegisterButton.Name = "RegisterButton";
            RegisterButton.Size = new Size(75, 23);
            RegisterButton.TabIndex = 1;
            RegisterButton.Text = "Register";
            RegisterButton.UseVisualStyleBackColor = true;
            RegisterButton.Click += RegisterButton_Click;
            // 
            // QuitButton
            // 
            QuitButton.Location = new Point(1313, 765);
            QuitButton.Name = "QuitButton";
            QuitButton.Size = new Size(75, 23);
            QuitButton.TabIndex = 2;
            QuitButton.Text = "Quit";
            QuitButton.UseVisualStyleBackColor = true;
            QuitButton.Click += QuitButton_Click;
            // 
            // LogoutButton
            // 
            LogoutButton.Location = new Point(12, 12);
            LogoutButton.Name = "LogoutButton";
            LogoutButton.Size = new Size(75, 23);
            LogoutButton.TabIndex = 3;
            LogoutButton.Text = "Logout";
            LogoutButton.UseVisualStyleBackColor = true;
            LogoutButton.Click += LogoutButton_Click;
            // 
            // JoinButton
            // 
            JoinButton.Location = new Point(900, 558);
            JoinButton.Name = "JoinButton";
            JoinButton.Size = new Size(75, 23);
            JoinButton.TabIndex = 5;
            JoinButton.Text = "Join";
            JoinButton.UseVisualStyleBackColor = true;
            JoinButton.Click += JoinButton_Click;
            // 
            // CreateButton
            // 
            CreateButton.Location = new Point(451, 558);
            CreateButton.Name = "CreateButton";
            CreateButton.Size = new Size(75, 23);
            CreateButton.TabIndex = 6;
            CreateButton.Text = "Create";
            CreateButton.UseVisualStyleBackColor = true;
            CreateButton.Click += CreateButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 40F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(603, 180);
            label1.Name = "label1";
            label1.Size = new Size(234, 72);
            label1.TabIndex = 7;
            label1.Text = "Shotgun";
            // 
            // UsernameLabel
            // 
            UsernameLabel.AutoSize = true;
            UsernameLabel.Location = new Point(451, 265);
            UsernameLabel.Name = "UsernameLabel";
            UsernameLabel.Size = new Size(0, 15);
            UsernameLabel.TabIndex = 8;
            // 
            // GameListDataGridView
            // 
            GameListDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GameListDataGridView.Location = new Point(451, 283);
            GameListDataGridView.Name = "GameListDataGridView";
            GameListDataGridView.RowTemplate.Height = 25;
            GameListDataGridView.Size = new Size(524, 269);
            GameListDataGridView.TabIndex = 9;
            // 
            // WaitingLabel
            // 
            WaitingLabel.AutoSize = true;
            WaitingLabel.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            WaitingLabel.Location = new Point(533, 401);
            WaitingLabel.Name = "WaitingLabel";
            WaitingLabel.Size = new Size(331, 37);
            WaitingLabel.TabIndex = 11;
            WaitingLabel.Text = "Waiting for player to join...";
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 800);
            Controls.Add(WaitingLabel);
            Controls.Add(GameListDataGridView);
            Controls.Add(UsernameLabel);
            Controls.Add(label1);
            Controls.Add(CreateButton);
            Controls.Add(JoinButton);
            Controls.Add(LogoutButton);
            Controls.Add(QuitButton);
            Controls.Add(RegisterButton);
            Controls.Add(LoginButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shotgun - Menu";
            Load += MenuForm_Load;
            ((System.ComponentModel.ISupportInitialize)GameListDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Button LoginButton;
        private Button RegisterButton;
        private Button QuitButton;
        private Button LogoutButton;
        private Button JoinButton;
        private Button CreateButton;
        private Label label1;
        private Label UsernameLabel;
        private DataGridView GameListDataGridView;
        private Label WaitingLabel;
    }
}