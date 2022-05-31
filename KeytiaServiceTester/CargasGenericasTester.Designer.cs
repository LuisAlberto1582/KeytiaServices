namespace KeytiaServiceTester
{
    partial class CargasGenericasTester
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
            this.SuspendLayout();
            // 
            // btnIniciarCarga
            // 
            this.btnIniciarCarga.Location = new System.Drawing.Point(167, 83);
            this.btnIniciarCarga.Name = "btnIniciarCarga";
            this.btnIniciarCarga.Size = new System.Drawing.Size(147, 76);
            this.btnIniciarCarga.TabIndex = 0;
            this.btnIniciarCarga.Text = "Iniciar carga";
            this.btnIniciarCarga.UseVisualStyleBackColor = true;
            this.btnIniciarCarga.Click += new System.EventHandler(this.btnIniciarCarga_Click);
            // 
            // CargasGenericasTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 263);
            this.Controls.Add(this.btnIniciarCarga);
            this.Name = "CargasGenericasTester";
            this.Text = "CargasGenericasTester";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnIniciarCarga;
    }
}