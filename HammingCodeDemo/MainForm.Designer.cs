namespace HammingCodeDemo
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PictBox = new System.Windows.Forms.PictureBox();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.NextBtn = new System.Windows.Forms.Button();
            this.PrevBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictBox)).BeginInit();
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PictBox
            // 
            this.PictBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PictBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictBox.Location = new System.Drawing.Point(0, 72);
            this.PictBox.Name = "PictBox";
            this.PictBox.Size = new System.Drawing.Size(1853, 666);
            this.PictBox.TabIndex = 0;
            this.PictBox.TabStop = false;
            this.PictBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PictBox_Paint);
            this.PictBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictBox_MouseClick);
            this.PictBox.Resize += new System.EventHandler(this.PictBox_Resize);
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.NextBtn);
            this.TopPanel.Controls.Add(this.PrevBtn);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(1853, 72);
            this.TopPanel.TabIndex = 1;
            // 
            // NextBtn
            // 
            this.NextBtn.Location = new System.Drawing.Point(93, 12);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(75, 46);
            this.NextBtn.TabIndex = 1;
            this.NextBtn.Text = "Next";
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // PrevBtn
            // 
            this.PrevBtn.Location = new System.Drawing.Point(12, 12);
            this.PrevBtn.Name = "PrevBtn";
            this.PrevBtn.Size = new System.Drawing.Size(75, 46);
            this.PrevBtn.TabIndex = 0;
            this.PrevBtn.Text = "Prev";
            this.PrevBtn.UseVisualStyleBackColor = true;
            this.PrevBtn.Click += new System.EventHandler(this.PrevBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1853, 738);
            this.Controls.Add(this.PictBox);
            this.Controls.Add(this.TopPanel);
            this.Name = "MainForm";
            this.Text = "Hamming Code";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictBox)).EndInit();
            this.TopPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PictBox;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.Button PrevBtn;
    }
}

