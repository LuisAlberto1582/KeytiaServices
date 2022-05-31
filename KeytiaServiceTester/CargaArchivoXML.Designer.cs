namespace KeytiaServiceTester
{
    partial class CargaArchivoXML
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
            this.btnAceptar = new System.Windows.Forms.Button();
            this.lblEsquema = new System.Windows.Forms.Label();
            this.lblArchivoXML = new System.Windows.Forms.Label();
            this.txtEsquema = new System.Windows.Forms.TextBox();
            this.txtArchivo = new System.Windows.Forms.TextBox();
            this.txtClaveCarga = new System.Windows.Forms.TextBox();
            this.lblClaveCarga = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(216, 150);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(163, 64);
            this.btnAceptar.TabIndex = 0;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // lblEsquema
            // 
            this.lblEsquema.AutoSize = true;
            this.lblEsquema.Location = new System.Drawing.Point(75, 61);
            this.lblEsquema.Name = "lblEsquema";
            this.lblEsquema.Size = new System.Drawing.Size(54, 13);
            this.lblEsquema.TabIndex = 1;
            this.lblEsquema.Text = "Esquema:";
            // 
            // lblArchivoXML
            // 
            this.lblArchivoXML.AutoSize = true;
            this.lblArchivoXML.Location = new System.Drawing.Point(75, 114);
            this.lblArchivoXML.Name = "lblArchivoXML";
            this.lblArchivoXML.Size = new System.Drawing.Size(71, 13);
            this.lblArchivoXML.TabIndex = 2;
            this.lblArchivoXML.Text = "Archivo XML:";
            this.lblArchivoXML.Visible = false;
            // 
            // txtEsquema
            // 
            this.txtEsquema.Location = new System.Drawing.Point(152, 61);
            this.txtEsquema.Name = "txtEsquema";
            this.txtEsquema.Size = new System.Drawing.Size(353, 20);
            this.txtEsquema.TabIndex = 3;
            // 
            // txtArchivo
            // 
            this.txtArchivo.Location = new System.Drawing.Point(152, 111);
            this.txtArchivo.Name = "txtArchivo";
            this.txtArchivo.Size = new System.Drawing.Size(353, 20);
            this.txtArchivo.TabIndex = 4;
            this.txtArchivo.Visible = false;
            // 
            // txtClaveCarga
            // 
            this.txtClaveCarga.Location = new System.Drawing.Point(152, 87);
            this.txtClaveCarga.Name = "txtClaveCarga";
            this.txtClaveCarga.Size = new System.Drawing.Size(353, 20);
            this.txtClaveCarga.TabIndex = 6;
            // 
            // lblClaveCarga
            // 
            this.lblClaveCarga.AutoSize = true;
            this.lblClaveCarga.Location = new System.Drawing.Point(75, 87);
            this.lblClaveCarga.Name = "lblClaveCarga";
            this.lblClaveCarga.Size = new System.Drawing.Size(67, 13);
            this.lblClaveCarga.TabIndex = 5;
            this.lblClaveCarga.Text = "Clave carga:";
            // 
            // CargaArchivoXML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 270);
            this.Controls.Add(this.txtClaveCarga);
            this.Controls.Add(this.lblClaveCarga);
            this.Controls.Add(this.txtArchivo);
            this.Controls.Add(this.txtEsquema);
            this.Controls.Add(this.lblArchivoXML);
            this.Controls.Add(this.lblEsquema);
            this.Controls.Add(this.btnAceptar);
            this.Name = "CargaArchivoXML";
            this.Text = "Keytia Tester - Carga archivo XML";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Label lblEsquema;
        private System.Windows.Forms.Label lblArchivoXML;
        private System.Windows.Forms.TextBox txtEsquema;
        private System.Windows.Forms.TextBox txtArchivo;
        private System.Windows.Forms.TextBox txtClaveCarga;
        private System.Windows.Forms.Label lblClaveCarga;
    }
}