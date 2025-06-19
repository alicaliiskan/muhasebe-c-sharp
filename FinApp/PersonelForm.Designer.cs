
namespace muhasebe
{
    partial class PersonelForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPerEkle = new System.Windows.Forms.ToolStripButton();
            this.btnPerCikar = new System.Windows.Forms.ToolStripButton();
            this.btnPerVeriGuncelle = new System.Windows.Forms.ToolStripButton();
            this.dgvGuncelle = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(54)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(37)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Gold;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(17)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(100)))), ((int)(((byte)(141)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(848, 427);
            this.dataGridView1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(17)))), ((int)(((byte)(40)))));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPerEkle,
            this.btnPerCikar,
            this.btnPerVeriGuncelle,
            this.dgvGuncelle});
            this.toolStrip1.Location = new System.Drawing.Point(0, 427);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(848, 55);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnPerEkle
            // 
            this.btnPerEkle.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnPerEkle.Image = global::FinApp.Properties.Resources.add_user;
            this.btnPerEkle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPerEkle.Margin = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.btnPerEkle.Name = "btnPerEkle";
            this.btnPerEkle.Size = new System.Drawing.Size(129, 52);
            this.btnPerEkle.Text = "Personel Ekle";
            this.btnPerEkle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPerEkle.Click += new System.EventHandler(this.btnPerEkle_Click);
            // 
            // btnPerCikar
            // 
            this.btnPerCikar.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnPerCikar.Image = global::FinApp.Properties.Resources.delete;
            this.btnPerCikar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPerCikar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.btnPerCikar.Name = "btnPerCikar";
            this.btnPerCikar.Size = new System.Drawing.Size(138, 52);
            this.btnPerCikar.Text = "Personel Çıkar";
            this.btnPerCikar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPerCikar.Click += new System.EventHandler(this.btnPerCikar_Click);
            // 
            // btnPerVeriGuncelle
            // 
            this.btnPerVeriGuncelle.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnPerVeriGuncelle.Image = global::FinApp.Properties.Resources.employee;
            this.btnPerVeriGuncelle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPerVeriGuncelle.Margin = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.btnPerVeriGuncelle.Name = "btnPerVeriGuncelle";
            this.btnPerVeriGuncelle.Size = new System.Drawing.Size(220, 52);
            this.btnPerVeriGuncelle.Text = "Personel Verisi Güncelle";
            this.btnPerVeriGuncelle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPerVeriGuncelle.Click += new System.EventHandler(this.btnPerVeriGuncelle_Click);
            // 
            // dgvGuncelle
            // 
            this.dgvGuncelle.BackColor = System.Drawing.Color.LightSteelBlue;
            this.dgvGuncelle.Image = global::FinApp.Properties.Resources.refresh_1_;
            this.dgvGuncelle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dgvGuncelle.Name = "dgvGuncelle";
            this.dgvGuncelle.Size = new System.Drawing.Size(164, 52);
            this.dgvGuncelle.Text = "Tabloyu Güncelle";
            this.dgvGuncelle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.dgvGuncelle.Click += new System.EventHandler(this.dgvGuncelle_Click);
            // 
            // PersonelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 482);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PersonelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PersonelForm";
            this.Load += new System.EventHandler(this.PersonelForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPerCikar;
        private System.Windows.Forms.ToolStripButton btnPerVeriGuncelle;
        private System.Windows.Forms.ToolStripButton btnPerEkle;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripButton dgvGuncelle;
    }
}
