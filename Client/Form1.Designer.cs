namespace Client
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.GameLabel = new System.Windows.Forms.Label();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.StepBar = new System.Windows.Forms.ProgressBar();
            this.TextBox = new System.Windows.Forms.RichTextBox();
            this.DownloadBar = new System.Windows.Forms.ProgressBar();
            this.ListGame = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // GameLabel
            // 
            this.GameLabel.AutoSize = true;
            this.GameLabel.BackColor = System.Drawing.Color.Transparent;
            this.GameLabel.Font = new System.Drawing.Font("Tahoma", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.GameLabel.Location = new System.Drawing.Point(319, 53);
            this.GameLabel.Name = "GameLabel";
            this.GameLabel.Size = new System.Drawing.Size(134, 35);
            this.GameLabel.TabIndex = 0;
            this.GameLabel.Text = "Valheim";
            this.GameLabel.Visible = false;
            // 
            // DownloadButton
            // 
            this.DownloadButton.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.DownloadButton.Location = new System.Drawing.Point(319, 330);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(147, 37);
            this.DownloadButton.TabIndex = 1;
            this.DownloadButton.Text = "Télécharger";
            this.DownloadButton.UseVisualStyleBackColor = true;
            this.DownloadButton.Visible = false;
            this.DownloadButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // StepBar
            // 
            this.StepBar.Location = new System.Drawing.Point(40, 406);
            this.StepBar.Name = "StepBar";
            this.StepBar.Size = new System.Drawing.Size(723, 23);
            this.StepBar.TabIndex = 2;
            this.StepBar.Visible = false;
            // 
            // TextBox
            // 
            this.TextBox.Location = new System.Drawing.Point(127, 146);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(560, 151);
            this.TextBox.TabIndex = 3;
            this.TextBox.Text = "";
            this.TextBox.Visible = false;
            // 
            // DownloadBar
            // 
            this.DownloadBar.Location = new System.Drawing.Point(40, 373);
            this.DownloadBar.Name = "DownloadBar";
            this.DownloadBar.Size = new System.Drawing.Size(723, 23);
            this.DownloadBar.TabIndex = 4;
            this.DownloadBar.Visible = false;
            // 
            // ListGame
            // 
            this.ListGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListGame.FormattingEnabled = true;
            this.ListGame.Location = new System.Drawing.Point(13, 13);
            this.ListGame.Name = "ListGame";
            this.ListGame.Size = new System.Drawing.Size(146, 23);
            this.ListGame.TabIndex = 5;
            this.ListGame.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ListGame);
            this.Controls.Add(this.DownloadBar);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.StepBar);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.GameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Slayer";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label GameLabel;
        private System.Windows.Forms.Button DownloadButton;
        private System.Windows.Forms.ProgressBar StepBar;
        private System.Windows.Forms.RichTextBox TextBox;
        private System.Windows.Forms.ProgressBar DownloadBar;
        private System.Windows.Forms.ComboBox ListGame;
    }
}

