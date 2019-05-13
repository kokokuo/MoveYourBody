namespace KinectSimulation
{
    partial class KinectDisplay
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.kinectDisplayPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // kinectDisplayPanel
            // 
            this.kinectDisplayPanel.Location = new System.Drawing.Point(0, 0);
            this.kinectDisplayPanel.Name = "kinectDisplayPanel";
            this.kinectDisplayPanel.Size = new System.Drawing.Size(625, 444);
            this.kinectDisplayPanel.TabIndex = 0;
            this.kinectDisplayPanel.Visible = false;
            // 
            // KinectDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.kinectDisplayPanel);
            this.Name = "KinectDisplay";
            this.Text = "Display";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel kinectDisplayPanel;
    }
}

