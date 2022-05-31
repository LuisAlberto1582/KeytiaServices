using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb
{
    public partial class CambioContrasenia : System.Web.UI.Page
    {
        string iCodCatUsuar = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            iCodCatUsuar = Session["iCodUsuario"] == null ? "" : Session["iCodUsuario"].ToString();
        }

        protected void submitPwd_Click(object sender, EventArgs e)
        {
            string pwd = KeytiaServiceBL.Util.Encrypt(confcontraseña.Text.Trim());
            
            // Obtener iCodRegistro de tabla Esquema con el valor Usuar (iCodCatalogo en tabla cliente) 
            string query = "SELECT MAX(iCodRegistro) AS iCodRegistro " +
                           "FROM [VisHistoricos('Usuar','Usuarios','Español')] " +
                           "WHERE iCodCatalogo = " + iCodCatUsuar + " AND dtFinVigencia >= GetDate()";

            int iCodRegistroUsuar = (Int32)KeytiaServiceBL.DSODataAccess.ExecuteScalar(query);

            //Actualiza el password en el esquema del cliente
            string cmdTablaCliente = "SET ROWCOUNT 1 UPDATE [VisHistoricos('Usuar','Usuarios','Español')]  " +
                                     " SET Password = '" + pwd + "', ConfPassword = '" + pwd + "', dtFecUltAct = getdate()" +
                                     " WHERE iCodCatalogo = " + iCodCatUsuar +
                                     " AND dtFinVigencia >= GetDate() " +
                                     "AND iCodRegistro = " + iCodRegistroUsuar.ToString();

            KeytiaServiceBL.DSODataAccess.ExecuteNonQuery(cmdTablaCliente);
            
            int contextEsquema = DSODataContext.GetContext();
            DSODataContext.SetContext(0); //Cambia el context utilizando el esquema Keytia

            //Actualiza el password en el esquema Keytia
            string cmdTablaKeytia = "SET ROWCOUNT 1 UPDATE keytia.[VisDetallados('Detall','Detallado Usuarios','Español')] " +
                                    "SET Password = '" + pwd + "', dtFecUltAct = getdate()" +
                                    " WHERE UsuarDb = " + Session["iCodUsuarioDB"].ToString() +
                                    " AND iNumRegistro = " + iCodRegistroUsuar.ToString() +
                                    " AND iNumCatalogo = " + iCodCatUsuar;

            KeytiaServiceBL.DSODataAccess.ExecuteNonQuery(cmdTablaKeytia);


            DSODataContext.SetContext(contextEsquema);  //Cambia el context utilizando el esquema del cliente

            //Redirect hacia la página inicial del usuario
            string redirectFinal = ((string)Session["HomePage"]).IndexOf('?', 0) != -1 ? ((string)Session["HomePage"]) + "&isFT=1" : ((string)Session["HomePage"]) + "?isFT=1";  //is First Time
            
            Response.Redirect(redirectFinal);
        }
    }
}