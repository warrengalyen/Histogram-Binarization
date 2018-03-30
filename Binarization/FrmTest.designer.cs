namespace Binarization
{
    partial class FrmTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTest));
            this.CmdSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CmbMethod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LblThreshold = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SrcPic = new System.Windows.Forms.PictureBox();
            this.DestPic = new System.Windows.Forms.PictureBox();
            this.PicHist = new System.Windows.Forms.PictureBox();
            this.CmdOpen = new System.Windows.Forms.Button();
            this.PicSmoothHist = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SrcPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DestPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicHist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicSmoothHist)).BeginInit();
            this.SuspendLayout();
            // 
            // CmdSave
            // 
            this.CmdSave.Location = new System.Drawing.Point(12, 68);
            this.CmdSave.Name = "CmdSave";
            this.CmdSave.Size = new System.Drawing.Size(77, 33);
            this.CmdSave.TabIndex = 4;
            this.CmdSave.Text = "Save image";
            this.CmdSave.UseVisualStyleBackColor = true;
            this.CmdSave.Click += new System.EventHandler(this.CmdSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Threshold Method:";
            // 
            // CmbMethod
            // 
            this.CmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbMethod.FormattingEnabled = true;
            this.CmbMethod.Location = new System.Drawing.Point(197, 25);
            this.CmbMethod.Name = "CmbMethod";
            this.CmbMethod.Size = new System.Drawing.Size(161, 21);
            this.CmbMethod.TabIndex = 29;
            this.CmbMethod.SelectedIndexChanged += new System.EventHandler(this.CmbMethod_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Global Binary Threshold:";
            // 
            // LblThreshold
            // 
            this.LblThreshold.AutoSize = true;
            this.LblThreshold.Location = new System.Drawing.Point(238, 78);
            this.LblThreshold.Name = "LblThreshold";
            this.LblThreshold.Size = new System.Drawing.Size(25, 13);
            this.LblThreshold.TabIndex = 31;
            this.LblThreshold.Text = "127";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(375, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Histogram";
            // 
            // SrcPic
            // 
            this.SrcPic.Location = new System.Drawing.Point(12, 128);
            this.SrcPic.Name = "SrcPic";
            this.SrcPic.Size = new System.Drawing.Size(521, 413);
            this.SrcPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SrcPic.TabIndex = 33;
            this.SrcPic.TabStop = false;
            // 
            // DestPic
            // 
            this.DestPic.Location = new System.Drawing.Point(551, 128);
            this.DestPic.Name = "DestPic";
            this.DestPic.Size = new System.Drawing.Size(521, 413);
            this.DestPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DestPic.TabIndex = 34;
            this.DestPic.TabStop = false;
            // 
            // PicHist
            // 
            this.PicHist.Location = new System.Drawing.Point(435, 14);
            this.PicHist.Name = "PicHist";
            this.PicHist.Size = new System.Drawing.Size(256, 108);
            this.PicHist.TabIndex = 35;
            this.PicHist.TabStop = false;
            // 
            // CmdOpen
            // 
            this.CmdOpen.Location = new System.Drawing.Point(12, 18);
            this.CmdOpen.Name = "CmdOpen";
            this.CmdOpen.Size = new System.Drawing.Size(77, 33);
            this.CmdOpen.TabIndex = 36;
            this.CmdOpen.Text = "Open image";
            this.CmdOpen.UseVisualStyleBackColor = true;
            this.CmdOpen.Click += new System.EventHandler(this.CmdOpen_Click);
            // 
            // PicSmoothHist
            // 
            this.PicSmoothHist.Location = new System.Drawing.Point(816, 13);
            this.PicSmoothHist.Name = "PicSmoothHist";
            this.PicSmoothHist.Size = new System.Drawing.Size(256, 108);
            this.PicSmoothHist.TabIndex = 37;
            this.PicSmoothHist.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(707, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Smoothed histogram";
            // 
            // FrmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1086, 558);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PicSmoothHist);
            this.Controls.Add(this.CmdOpen);
            this.Controls.Add(this.PicHist);
            this.Controls.Add(this.DestPic);
            this.Controls.Add(this.SrcPic);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LblThreshold);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CmbMethod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CmdSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Histogram-based global binarization";
            this.Load += new System.EventHandler(this.FrmTest_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SrcPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DestPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicHist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicSmoothHist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Button CmdSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CmbMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LblThreshold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox SrcPic;
        private System.Windows.Forms.PictureBox DestPic;
        private System.Windows.Forms.PictureBox PicHist;
        private System.Windows.Forms.Button CmdOpen;
        private System.Windows.Forms.PictureBox PicSmoothHist;
        private System.Windows.Forms.Label label3;
    }
}