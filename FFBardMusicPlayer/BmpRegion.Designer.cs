
namespace FFBardMusicPlayerInit
{
    partial class BmpRegion
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
            this.IconPicture = new System.Windows.Forms.PictureBox();
            this.region_global = new System.Windows.Forms.Button();
            this.region_kr = new System.Windows.Forms.Button();
            this.region_cn = new System.Windows.Forms.Button();
            this.game_region_text = new System.Windows.Forms.Label();
            this.progress_text = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.IconPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // IconPicture
            // 
            this.IconPicture.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.IconPicture.BackgroundImage = global::FFBardMusicPlayerInit.Properties.Resources.bmp_icon_3;
            this.IconPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.IconPicture.Location = new System.Drawing.Point(9, 9);
            this.IconPicture.Margin = new System.Windows.Forms.Padding(0);
            this.IconPicture.Name = "IconPicture";
            this.IconPicture.Size = new System.Drawing.Size(91, 88);
            this.IconPicture.TabIndex = 4;
            this.IconPicture.TabStop = false;
            // 
            // region_global
            // 
            this.region_global.Location = new System.Drawing.Point(103, 49);
            this.region_global.Name = "region_global";
            this.region_global.Size = new System.Drawing.Size(75, 23);
            this.region_global.TabIndex = 5;
            this.region_global.Text = "Global";
            this.region_global.UseVisualStyleBackColor = true;
            this.region_global.Click += new System.EventHandler(this.region_global_Click);
            // 
            // region_kr
            // 
            this.region_kr.Location = new System.Drawing.Point(223, 49);
            this.region_kr.Name = "region_kr";
            this.region_kr.Size = new System.Drawing.Size(36, 23);
            this.region_kr.TabIndex = 6;
            this.region_kr.Text = "KR";
            this.region_kr.UseVisualStyleBackColor = true;
            this.region_kr.Click += new System.EventHandler(this.region_kr_Click);
            // 
            // region_cn
            // 
            this.region_cn.Location = new System.Drawing.Point(184, 49);
            this.region_cn.Name = "region_cn";
            this.region_cn.Size = new System.Drawing.Size(36, 23);
            this.region_cn.TabIndex = 7;
            this.region_cn.Text = "CN";
            this.region_cn.UseVisualStyleBackColor = true;
            this.region_cn.Click += new System.EventHandler(this.region_cn_Click);
            // 
            // game_region_text
            // 
            this.game_region_text.AutoSize = true;
            this.game_region_text.Location = new System.Drawing.Point(103, 31);
            this.game_region_text.Name = "game_region_text";
            this.game_region_text.Size = new System.Drawing.Size(144, 15);
            this.game_region_text.TabIndex = 8;
            this.game_region_text.Text = "Select FFXIV Game Region";
            // 
            // progress_text
            // 
            this.progress_text.AutoSize = true;
            this.progress_text.Location = new System.Drawing.Point(103, 57);
            this.progress_text.Name = "progress_text";
            this.progress_text.Size = new System.Drawing.Size(0, 15);
            this.progress_text.TabIndex = 10;
            // 
            // BmpRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 110);
            this.Controls.Add(this.progress_text);
            this.Controls.Add(this.game_region_text);
            this.Controls.Add(this.region_cn);
            this.Controls.Add(this.region_kr);
            this.Controls.Add(this.region_global);
            this.Controls.Add(this.IconPicture);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BmpRegion";
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bard Music Player";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BmpRegion_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.IconPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox IconPicture;
        private System.Windows.Forms.Button region_global;
        private System.Windows.Forms.Button region_kr;
        private System.Windows.Forms.Button region_cn;
        private System.Windows.Forms.Label game_region_text;
        private System.Windows.Forms.Label progress_text;
    }
}