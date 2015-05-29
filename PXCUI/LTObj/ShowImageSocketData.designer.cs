namespace PlusObj
{
    partial class ShowImageSocketDataForm
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
            this.Value_SendTime = new System.Windows.Forms.Label();
            this.Value_DataSize = new System.Windows.Forms.Label();
            this.Value_FPS = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Value_SendTime
            // 
            this.Value_SendTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Value_SendTime.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Value_SendTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Value_SendTime.Location = new System.Drawing.Point(12, 44);
            this.Value_SendTime.Name = "Value_SendTime";
            this.Value_SendTime.Size = new System.Drawing.Size(246, 23);
            this.Value_SendTime.TabIndex = 8;
            this.Value_SendTime.Text = "Delay_Net:  0";
            // 
            // Value_DataSize
            // 
            this.Value_DataSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Value_DataSize.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Value_DataSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Value_DataSize.Location = new System.Drawing.Point(12, 77);
            this.Value_DataSize.Name = "Value_DataSize";
            this.Value_DataSize.Size = new System.Drawing.Size(246, 23);
            this.Value_DataSize.TabIndex = 7;
            this.Value_DataSize.Text = "DataSize:  0.0 KB";
            // 
            // Value_FPS
            // 
            this.Value_FPS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Value_FPS.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Value_FPS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Value_FPS.Location = new System.Drawing.Point(12, 11);
            this.Value_FPS.Name = "Value_FPS";
            this.Value_FPS.Size = new System.Drawing.Size(91, 23);
            this.Value_FPS.TabIndex = 6;
            this.Value_FPS.Text = "FPS:  0";
            // 
            // ShowImageSocketData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 111);
            this.Controls.Add(this.Value_SendTime);
            this.Controls.Add(this.Value_DataSize);
            this.Controls.Add(this.Value_FPS);
            this.Name = "ShowImageSocketData";
            this.Text = "ShowImageSocketData";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Value_SendTime;
        private System.Windows.Forms.Label Value_DataSize;
        private System.Windows.Forms.Label Value_FPS;
    }
}