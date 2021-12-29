namespace StudentManagementSystem.Application
{
    partial class InstructorForm
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
            this.kayitGuncelleme = new System.Windows.Forms.Button();
            this.kayitListeleme = new System.Windows.Forms.Button();
            this.notGirme = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // kayitGuncelleme
            // 
            this.kayitGuncelleme.Location = new System.Drawing.Point(316, 89);
            this.kayitGuncelleme.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.kayitGuncelleme.Name = "kayitGuncelleme";
            this.kayitGuncelleme.Size = new System.Drawing.Size(221, 201);
            this.kayitGuncelleme.TabIndex = 1;
            this.kayitGuncelleme.Text = "button2";
            this.kayitGuncelleme.UseVisualStyleBackColor = true;
            // 
            // kayitListeleme
            // 
            this.kayitListeleme.Location = new System.Drawing.Point(599, 89);
            this.kayitListeleme.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.kayitListeleme.Name = "kayitListeleme";
            this.kayitListeleme.Size = new System.Drawing.Size(221, 201);
            this.kayitListeleme.TabIndex = 2;
            this.kayitListeleme.Text = "button3";
            this.kayitListeleme.UseVisualStyleBackColor = true;
            // 
            // notGirme
            // 
            this.notGirme.Location = new System.Drawing.Point(47, 89);
            this.notGirme.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.notGirme.Name = "notGirme";
            this.notGirme.Size = new System.Drawing.Size(221, 201);
            this.notGirme.TabIndex = 4;
            this.notGirme.Text = "button1";
            this.notGirme.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(338, 316);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Danışman İşlemleri";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(632, 316);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Profil Görüntüleme";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(78, 316);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Not Güncelleme";
            // 
            // InstructorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(882, 464);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.notGirme);
            this.Controls.Add(this.kayitListeleme);
            this.Controls.Add(this.kayitGuncelleme);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "InstructorForm";
            this.Text = "Öğretmen İşlemleri";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button kayitGuncelleme;
        private System.Windows.Forms.Button kayitListeleme;
        private System.Windows.Forms.Button notGirme;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
    }
}