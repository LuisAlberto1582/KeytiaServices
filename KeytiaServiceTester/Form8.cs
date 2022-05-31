using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using KeytiaServiceBL;
using System.IO;
using System.Xml;

namespace KeytiaServiceTester
{
    public partial class Form8 : Form
    {
        string lsPath = "C:\\PruebasWordAccess\\";
        MailAccess mail;

        public Form8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            System.IO.Directory.CreateDirectory(lsPath);
            //Start Word and create a new document.
            WordAccess oWord = new WordAccess();
            //oWord.FilePath = lsPath + "Lista de Números Frecuentes.doc";
            oWord.Abrir();


            //Insert a paragraph at the beginning of the document.
            oWord.Parrafo.Range.Font.Bold = 1;
            oWord.Parrafo.Format.SpaceAfter = 24;//24 pt spacing after paragraph.
            oWord.InsertarTexto("Heading 1");

            //Insert a paragraph at the end of the document.
            oWord.NuevoParrafo();
            oWord.Parrafo.Range.Font.Bold = 1;
            oWord.Parrafo.Format.SpaceAfter = 6;
            oWord.InsertarTexto("Heading 2");

            //Insert another paragraph.
            oWord.NuevoParrafo();
            oWord.Parrafo.Range.Font.Bold = 0;
            oWord.Parrafo.Format.SpaceAfter = 24;//24 pt spacing after paragraph.
            oWord.InsertarTexto("This is a sentence of normal text. Now here is a table:");

            //Insert a 3 x 5 table, fill it with data, and make the first row
            //bold and italic.
            oWord.NuevoParrafo();
            System.Data.DataTable dt = new System.Data.DataTable();
            int r, c;

            for (c = 1; c <= 5; c++)
            {
                dt.Columns.Add("C" + c);
            }
            string strText;
            for (r = 1; r <= 3; r++)
            {
                System.Data.DataRow dr = dt.NewRow();
                for (c = 1; c <= 5; c++)
                {
                    strText = "r" + r + "c" + c;
                    dr[c - 1] = strText;
                }
                dt.Rows.Add(dr);
            }
            oWord.Parrafo.Format.SpaceAfter = 6;
            oWord.InsertarTabla(dt, false);
            oWord.Tabla.Rows[1].Range.Font.Bold = 1;
            oWord.Tabla.Rows[1].Range.Font.Italic = 1;

            //Add some text after the table.
            oWord.NuevoParrafo();
            oWord.Parrafo.Range.InsertParagraphBefore();
            oWord.Parrafo.Format.SpaceAfter = 24;
            oWord.InsertarTexto("And here's another table:");

            //Insert a 5 x 2 table, fill it with data, and change the column widths.
            dt = new System.Data.DataTable();

            for (c = 1; c <= 2; c++)
            {
                dt.Columns.Add("C" + c);
            }
            for (r = 1; r <= 5; r++)
            {
                System.Data.DataRow dr = dt.NewRow();
                for (c = 1; c <= 2; c++)
                {
                    strText = "r" + r + "c" + c;
                    dr[c - 1] = strText;
                }
                dt.Rows.Add(dr);
            }
            oWord.NuevoParrafo();
            //oWord.Parrafo.Format.SpaceAfter = 6;
            oWord.InsertarTabla(dt, false);
            oWord.Tabla.Range.ParagraphFormat.SpaceAfter = 6;
            oWord.Tabla.Columns[1].Width = oWord.App.InchesToPoints(2); //Change width of columns 1 & 2
            oWord.Tabla.Columns[2].Width = oWord.App.InchesToPoints(3);

            //Keep inserting text. When you get to 7 inches from top of the
            //document, insert a hard page break.
            int dPos = 0;
            do
            {
                oWord.NuevoParrafo();
                oWord.Parrafo.Format.SpaceAfter = 6;
                oWord.InsertarTexto("A line of text");
                dPos++;
            }
            while (dPos <= 6);
            // Replacing text with text
            oWord.ReemplazarTexto("text", "TEXTO");

            // Replacing text with images
            oWord.InsertarSaltoDePagina();
            oWord.InsertarTexto("We're now on page 2. Here's my chart:");

            // Preparing the text to replace
            dPos = 1;
            do
            {
                oWord.NuevoParrafo();
                oWord.Parrafo.Format.SpaceAfter = 6;
                oWord.InsertarTexto("Imagen " + dPos + ": " + dPos + ".png");
                dPos++;
            }
            while (dPos <= 6);

            // Replacing
            dPos = 1;
            do
            {
                oWord.ReemplazarTextoPorImagen(dPos.ToString() + ".png", "C:\\PruebasWordAccess\\" + dPos.ToString() + ".png");
                dPos++;
            }
            while (dPos <= 6);

            dPos = 1;
            do
            {
                oWord.NuevoParrafo();
                oWord.Parrafo.Format.SpaceAfter = 6;
                oWord.InsertarTexto("Imagen 1: 1.png");
                dPos++;
            }
            while (dPos <= 6);
            oWord.ReemplazarTextoPorImagen("1.png", "C:\\PruebasWordAccess\\1.png");


            oWord.InsertarSaltoDePagina();
            oWord.InsertarTexto("We're now on page 3. Here's my chart:");
            oWord.NuevoParrafo();

            //Insert a chart.
            dt = new System.Data.DataTable();

            for (c = 1; c <= 5; c++)
            {
                dt.Columns.Add("C" + c);
            }
            //string strText;
            Random rnd = new Random();
            for (r = 1; r <= 3; r++)
            {
                System.Data.DataRow dr = dt.NewRow();
                for (c = 1; c <= 5; c++)
                {
                    //strText = "r" + r + "c" + c;
                    dr[c - 1] = rnd.Next() * 100;
                }
                dt.Rows.Add(dr);
            }

            oWord.InsertarTabla(dt, true);
            oWord.Parrafo.Format.SpaceAfter = 24;
            oWord.NuevoParrafo();
            //oWord.InsertarGrafico(dt, true, Microsoft.Office.Core.XlChartType.xlLine, "mi Título");

            //Add text after the chart.
            oWord.NuevoParrafo();
            oWord.InsertarTexto("THE END.");

            oWord.NuevoParrafo();
            if (System.IO.File.Exists(lsPath + "logo_symbol.jpg"))
            {
                oWord.InsertarImagen(lsPath + "logo_symbol.jpg");
            }
            oWord.Parrafo.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //Close this form.
            //this.Close();
            oWord.FilePath = lsPath + "Prueba.doc";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.docx";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.pdf";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.rtf";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.dot";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.dotx";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.html";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.mhtml";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.xml";
            oWord.SalvarComo();
            oWord.FilePath = lsPath + "Prueba.docm";
            oWord.SalvarComo();
            oWord.Salir();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Proceso terminado");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            mail = new MailAccess();
            mail.Asunto = "Prueba";
            mail.De = new System.Net.Mail.MailAddress("tsp.keytia@delaware-software.com", "Keytia");
            mail.Para.Add("daniel.medina@delaware-software.com");

            string lsNombreArchivo = @"C:\Documents and Settings\Delaware\Escritorio\PlantillasMail\PlantillaMail.doc";

            mail.IsHtml = true;
            mail.AgregarWord(lsNombreArchivo);
            mail.Enviar();
            Cursor.Current = Cursors.Default;
        }
        #region Alarmas
        KeytiaServiceBL.LanzadorAlarmas poLanzadorAlarmas;
        System.Threading.Thread poThreadAlarmas;

        private void button3_Click(object sender, EventArgs e)
        {
            OnStart(null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OnStop();
        }

        protected void OnStart(string[] args)
        {

            poLanzadorAlarmas = new KeytiaServiceBL.LanzadorAlarmas();

            poThreadAlarmas = new System.Threading.Thread(poLanzadorAlarmas.Start);
            poThreadAlarmas.Start();
        }

        protected void OnStop()
        {
            int liCount;

            if (poLanzadorAlarmas != null)
                poLanzadorAlarmas.Stop();

            if (poThreadAlarmas != null)
            {
                liCount = 0;

                while (poThreadAlarmas.IsAlive && liCount < 60)
                {
                    System.Threading.Thread.Sleep(1000);
                    liCount++;
                }

                if (poThreadAlarmas.IsAlive)
                    poThreadAlarmas.Interrupt();
            }

            poLanzadorAlarmas = null;
            poThreadAlarmas = null;
        }
        #endregion

        private void button5_Click(object sender, EventArgs e)
        {
            object[,] loValores = new object[2, 5];
            for (int liRow = 0; liRow < 2; liRow++)
                for (int liCol = 0; liCol < 5; liCol++)
                    loValores[liRow, liCol] = liRow.ToString() + ", " + liCol.ToString();

            ExcelAccess loExcel;
            
            //Prueba para insertar celdas
            //loExcel = new ExcelAccess();
            //loExcel.FilePath = @"C:\DSO-DMM\DTI\Keytia V\Ciclo II\Requerimientos\Plantillas\prueba.xls";
            //loExcel.Abrir();
            //loExcel.InsertarCeldas(loExcel.NombreHoja0(), 15, 1, loValores, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow, Excel.XlInsertShiftDirection.xlShiftDown);
            //loExcel.InsertarCeldas(loExcel.NombreHoja0(), 15, 1, loValores, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow, Excel.XlInsertShiftDirection.xlShiftToRight);
            //loExcel.FilePath = @"c:\pruebaInsertarCeldas.xls";
            //loExcel.SalvarComo();
            //loExcel.Cerrar();
            //loExcel.Salir();
            //loExcel = null;

            //Prueba para insertar imágenes
            //loExcel = new ExcelAccess();
            //loExcel.Abrir();
            //loExcel.InsertPicture(loExcel.NombreHoja0(), @"C:\Documents and Settings\Delaware\Mis documentos\Mis imágenes\z530.jpg", "D5", "G15", true, true);
            //loExcel.FilePath = @"c:\pruebaInsertPicture.xls";
            //loExcel.SalvarComo();
            //loExcel.Cerrar();
            //loExcel.Salir();
            //loExcel = null;

            //Prueba para reemplazar texto por imagen
            loExcel = new ExcelAccess();
            loExcel.FilePath = @"c:\pruebaReemplazaTextoPorImagen.xls";
            loExcel.Abrir();
            loExcel.ReemplazaTextoPorImagen(loExcel.NombreHoja0(), "pruebaReemplazaTextoPorImagen", false, @"C:\cerebro.jpg", false, false);
            loExcel.FilePath = @"c:\pruebaReemplazaTextoPorImagenYaHecha.xls";
            loExcel.SalvarComo();
            loExcel.Cerrar();
            loExcel.Salir();
            loExcel = null;


        }
    }
}
