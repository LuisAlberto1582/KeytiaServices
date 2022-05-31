namespace KeytiaServiceTester
{
    partial class CargasTIMTester
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
            this.btnIniciarCarga = new System.Windows.Forms.Button();
            this.txtEsquema = new System.Windows.Forms.TextBox();
            this.txtClaveCarga = new System.Windows.Forms.TextBox();
            this.lblEsquema = new System.Windows.Forms.Label();
            this.lblClaveCarga = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnIniciarCarga
            // 
            this.btnIniciarCarga.Location = new System.Drawing.Point(184, 109);
            this.btnIniciarCarga.Name = "btnIniciarCarga";
            this.btnIniciarCarga.Size = new System.Drawing.Size(142, 73);
            this.btnIniciarCarga.TabIndex = 0;
            this.btnIniciarCarga.Text = "Iniciar carga";
            this.btnIniciarCarga.UseVisualStyleBackColor = true;
            this.btnIniciarCarga.Click += new System.EventHandler(this.btnIniciarCarga_Click);
            // 
            // txtEsquema
            // 
            this.txtEsquema.Location = new System.Drawing.Point(139, 44);
            this.txtEsquema.Name = "txtEsquema";
            this.txtEsquema.Size = new System.Drawing.Size(259, 20);
            this.txtEsquema.TabIndex = 1;
            // 
            // txtClaveCarga
            // 
            this.txtClaveCarga.Location = new System.Drawing.Point(139, 71);
            this.txtClaveCarga.Name = "txtClaveCarga";
            this.txtClaveCarga.Size = new System.Drawing.Size(259, 20);
            this.txtClaveCarga.TabIndex = 2;
            // 
            // lblEsquema
            // 
            this.lblEsquema.AutoSize = true;
            this.lblEsquema.Location = new System.Drawing.Point(70, 47);
            this.lblEsquema.Name = "lblEsquema";
            this.lblEsquema.Size = new System.Drawing.Size(54, 13);
            this.lblEsquema.TabIndex = 3;
            this.lblEsquema.Text = "Esquema:";
            // 
            // lblClaveCarga
            // 
            this.lblClaveCarga.AutoSize = true;
            this.lblClaveCarga.Location = new System.Drawing.Point(70, 74);
            this.lblClaveCarga.Name = "lblClaveCarga";
            this.lblClaveCarga.Size = new System.Drawing.Size(67, 13);
            this.lblClaveCarga.TabIndex = 4;
            this.lblClaveCarga.Text = "Clave carga:";
            // 
            // CargasTIMTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 210);
            this.Controls.Add(this.lblClaveCarga);
            this.Controls.Add(this.lblEsquema);
            this.Controls.Add(this.txtClaveCarga);
            this.Controls.Add(this.txtEsquema);
            this.Controls.Add(this.btnIniciarCarga);
            this.Name = "CargasTIMTester";
            this.Text = "TIM - Carga conceptos factura";
            this.Load += new System.EventHandler(this.CargasTIMTester_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnIniciarCarga;
        private System.Windows.Forms.TextBox txtEsquema;
        private System.Windows.Forms.TextBox txtClaveCarga;
        private System.Windows.Forms.Label lblEsquema;
        private System.Windows.Forms.Label lblClaveCarga;
    }
}