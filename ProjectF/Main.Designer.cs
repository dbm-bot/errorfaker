
namespace ProjectF
{
    partial class Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.emoji = new System.Windows.Forms.Label();
            this.errText = new System.Windows.Forms.Label();
            this.winLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.winLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // emoji
            // 
            this.emoji.AutoSize = true;
            this.emoji.Font = new System.Drawing.Font("Microsoft YaHei UI", 90F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emoji.ForeColor = System.Drawing.Color.White;
            this.emoji.Location = new System.Drawing.Point(104, 109);
            this.emoji.Name = "emoji";
            this.emoji.Size = new System.Drawing.Size(134, 156);
            this.emoji.TabIndex = 0;
            this.emoji.Text = ":(";
            // 
            // errText
            // 
            this.errText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errText.AutoSize = true;
            this.errText.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errText.ForeColor = System.Drawing.Color.White;
            this.errText.Location = new System.Drawing.Point(124, 324);
            this.errText.Name = "errText";
            this.errText.Size = new System.Drawing.Size(656, 93);
            this.errText.TabIndex = 1;
            this.errText.Text = "Your PC ran into a problem and needs to restart. We\'re\r\njust collecting some erro" +
    "r info, and then we\'ll restart for \r\nyou.";
            // 
            // winLogo
            // 
            this.winLogo.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.winLogo.ErrorImage = null;
            this.winLogo.Image = ((System.Drawing.Image)(resources.GetObject("winLogo.Image")));
            this.winLogo.InitialImage = null;
            this.winLogo.Location = new System.Drawing.Point(840, 265);
            this.winLogo.Name = "winLogo";
            this.winLogo.Size = new System.Drawing.Size(339, 183);
            this.winLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.winLogo.TabIndex = 2;
            this.winLogo.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.winLogo);
            this.Controls.Add(this.errText);
            this.Controls.Add(this.emoji);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Main";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "System";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Main_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Main_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.winLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label emoji;
        private System.Windows.Forms.Label errText;
        private System.Windows.Forms.PictureBox winLogo;
    }
}

