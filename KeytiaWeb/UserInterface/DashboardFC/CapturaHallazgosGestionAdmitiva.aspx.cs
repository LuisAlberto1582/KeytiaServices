using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using System.Text;
using KeytiaServiceBL;
using System.Data;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class CapturaHallazgosGestionAdmitiva : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack)
                {
                    LlenarDropDownList();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void LlenarDropDownList()
        {
            cboCarrier.DataSource = GetDataDropDownList("CARRIER").DefaultView;
            cboCarrier.DataBind();

            cboCtaMaestra.DataSource = GetDataDropDownList("CTAMAESTRA").DefaultView;
            cboCtaMaestra.DataBind();

            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();

            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();

            cboTipoDestino.DataSource = GetDataDropDownList("TDEST").DefaultView;
            cboTipoDestino.DataBind();

            cboVariacion.DataSource = GetDataDropDownList("VARIACION").DefaultView;
            cboVariacion.DataBind();

            cboCategoria.DataSource = GetDataDropDownList("CATEGORIA").DefaultView;
            cboCategoria.DataBind();

            cboMoneda.DataSource = GetDataDropDownList("MONEDA").DefaultView;
            cboMoneda.DataBind();
        }

        private DataTable GetDataDropDownList(string clave)
        {
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");

            switch (clave.ToUpper())
            {
                case "CARRIER":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Carrier','Carriers','Español')]");
                    query.AppendLine("  ORDER BY vchDescripcion");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "CTAMAESTRA":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('CtaMaestra','Cuenta Maestra Carrier','Español')]");
                    query.AppendLine("  ORDER BY vchDescripcion");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "CTAMAESTRA-CARRIER":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('CtaMaestra','Cuenta Maestra Carrier','Español')]");
                    query.AppendLine("  AND Carrier = " + cboCarrier.SelectedValue);
                    query.AppendLine("  ORDER BY vchDescripcion");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "ANIO":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Anio','Años','Español')]");
                    query.AppendLine(" AND CONVERT(INT, vchDescripcion) >= 2016 AND CONVERT(INT, vchDescripcion) <= YEAR(GETDATE())");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "MES":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "TDEST":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('TDest','Tipo de Destino','Español')]");
                    query.AppendLine("  ORDER BY Español");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "VARIACION":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Descripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('VariacionGestionAdmitiva','Variacion Gestion Admitiva','Español')]");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()));
                case "CATEGORIA":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Descripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('CategoriaGestionAdmitiva','Categorias Gestion Admitiva','Español')]");
                    return DSODataAccess.Execute(query.ToString());
                case "MONEDA":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Moneda','Monedas','Español')]");
                    return DSODataAccess.Execute(query.ToString()); 
                default:
                    return new DataTable();
            }       
        }

        private DataTable AddRowDefault(DataTable dt) 
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["iCodCatalogo"] = 0;
                rowExtra["Descripcion"] = "- SELECCIONAR ELEMENTO -";
                dt.Rows.InsertAt(rowExtra, 0); 
            }
            return dt;
        }
        
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos())
            {
                return;
            }

            var dtResult = InsertarHallazgo();
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                int seRealizo = Convert.ToInt32(dtResult.Rows[0][0]);
                string mensaje = dtResult.Rows[0][1].ToString();

                if (seRealizo == 0)
                {
                    lblTituloModalMsn.Text = "Registro";
                    lblBodyModalMsn.Text = mensaje;
                    mpeEtqMsn.Show();
                }
                else
                {
                    txtFolio.Text = mensaje;
                    lblTituloModalMsn.Text = "Registro";
                    lblBodyModalMsn.Text = "El registro se inserto correctamente.";
                    mpeEtqMsn.Show();
                }
            }
            else
            {
                lblTituloModalMsn.Text = "Registro";
                lblBodyModalMsn.Text = "No fue posible insertar el registro.";
                mpeEtqMsn.Show();
            }
        }

        private DataTable InsertarHallazgo()
        {
            query.Length = 0;
            query.AppendLine("EXEC [AltaHallazgoGestionAdmitiva]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "'");
            query.AppendLine("  , @iCodCatCarrier = " + cboCarrier.SelectedValue);
            query.AppendLine("  , @iCodCatCuenta = " + cboCtaMaestra.SelectedValue);
            query.AppendLine("  , @iCodCatAnio = " + cboAnio.SelectedValue);
            query.AppendLine("  , @iCodCatMes = " + cboMes.SelectedValue);
            query.AppendLine("  , @iCodCatTDest = " + cboTipoDestino.SelectedValue);
            query.AppendLine("  , @iCodCatVariacion = " + cboVariacion.SelectedValue);
            query.AppendLine("  , @iCodCatCategoria = " + cboCategoria.SelectedValue);
            query.AppendLine("  , @iCodCatMoneda = " + cboMoneda.SelectedValue);
            query.AppendLine("  , @Hallazgo = '" + txtHallazgo.Text.Trim() + "'");
            query.AppendLine("  , @Importe = " + txtImporte.Text.Trim());
            query.AppendLine("  , @Descripcion = '" + txtDescripcion.Text.Trim() + "'");

            return DSODataAccess.Execute(query.ToString());
        }

        private bool ValidarDatos()
        {
            query.Length = 0;

            if (string.IsNullOrEmpty(txtHallazgo.Text.Trim()))
                query.AppendLine("- Hallazgo <br/>");

            if (string.IsNullOrEmpty(txtDescripcion.Text.Trim()))
                query.AppendLine("- Descripción <br/>");

            if (string.IsNullOrEmpty(txtImporte.Text.Trim()))
                query.AppendLine("- Importe <br/>");

            if (cboCarrier.SelectedIndex == 0)
                query.AppendLine("- Carrier <br/>");

            if (cboCtaMaestra.SelectedIndex == 0)
                query.AppendLine("- Cuenta <br/>");

            if (cboAnio.SelectedIndex == 0)
                query.AppendLine("- Año <br/>");

            if (cboMes.SelectedIndex == 0)
                query.AppendLine("- Mes <br/>");

            if (cboTipoDestino.SelectedIndex == 0)
                query.AppendLine("- Servicio <br/>");

            if (cboVariacion.SelectedIndex == 0)
                query.AppendLine("- Variación <br/>");

            if (cboCategoria.SelectedIndex == -1)
                query.AppendLine("- Categoría <br/>");

            if (cboMoneda.SelectedIndex == -1)
                query.AppendLine("- Moneda <br/>");

            if (query.Length > 0)
            {
                lblTituloModalMsn.Text = "Datos Requeridos";
                lblBodyModalMsn.Text = "Llene todos los campos de forma correcta: <br/>" + query.ToString();
                mpeEtqMsn.Show();
                return false;
            }

            float value = 0;
            if (!float.TryParse(txtImporte.Text, out value))
            {
                lblTituloModalMsn.Text = "Error en datos";
                lblBodyModalMsn.Text = string.Format("El campo 'Importe' debe ser un valor númerico");
                mpeEtqMsn.Show();
                return false;
            }

            return true;
        }       

        private void SetDefaultValues()
        {
            txtFolio.Text = string.Empty;
            txtHallazgo.Text = string.Empty;
            cboCarrier.SelectedIndex = 0;
            cboCtaMaestra.SelectedIndex = 0;
            cboAnio.SelectedIndex = 0;
            cboMes.SelectedIndex = 0;
            cboTipoDestino.SelectedIndex = 0;
            cboVariacion.SelectedIndex = 0;
            cboCategoria.SelectedIndex = 0;
            cboMoneda.SelectedIndex = 0;
            txtImporte.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
        }

        protected void cboCarrierIndex_Changed(Object sender, EventArgs e)
        {
            cboCtaMaestra.DataSource = GetDataDropDownList("CTAMAESTRA-CARRIER").DefaultView;
            cboCtaMaestra.DataBind();
        }

    }
}
