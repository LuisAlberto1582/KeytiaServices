/*
 * Nombre:		    SCB
 * Fecha:		    20110607
 * Descripción:	    Clase para el manejo del usuario (Seguridad)
 * Modificación:	
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Usuarios
/// </summary>
/// 

namespace KeytiaWeb
{
    public class Usuarios
    {
        protected int piCodUsuario;
        protected String pvchCodUsuario;
        protected String pvchPwdUsuario;
        protected String pvchEmail;
        protected String pvchCodUsuarioDB;
        protected System.Data.DataRow pRow = null;

        public int iCodUsuario
        {
            get { return piCodUsuario; }
            set { piCodUsuario = value; }
        }

        public String vchCodUsuario
        {
            get { return pvchCodUsuario; }
            set { pvchCodUsuario = value; }
        }

        public String vchPwdUsuario
        {
            get { return pvchPwdUsuario; }
            set { pvchPwdUsuario = value; }
        }

        public String vchEmail
        {
            get { return pvchEmail; }
            set { pvchEmail = value; }
        }

        public String vchCodUsuarioDB
        {
            get { return pvchCodUsuarioDB; }
            set { pvchCodUsuarioDB = value; }
        }
        public System.Data.DataRow Row
        {
            get { return pRow; }
            set { pRow = value; }
        }

        public bool Consultar()
        {

            pRow = GetUsuario(pvchCodUsuario, pvchPwdUsuario, pvchEmail, pvchCodUsuarioDB);

            if (pRow != null)
                pvchCodUsuario = (string)pRow["vchCodigo"];

            return (pRow != null);

        }
        protected System.Data.DataRow GetUsuario(string lsCodUsuario, string lsPwdUsuario, string lsEmail, string lsCodUsuarioDB)
        {
            System.Data.DataTable ldtUsuario = null;
            KDBAccess kdb = new KDBAccess();
            string lsQuery = "";
            bool bUsrIdentificable = false;

            if (!String.IsNullOrEmpty(lsCodUsuario))
            {
                lsQuery += (lsQuery.Length > 0 ? " and " : "") + "vchCodigo = '" + lsCodUsuario + "'";
                bUsrIdentificable = true;
            }
            if (!String.IsNullOrEmpty(lsPwdUsuario))
            {
                lsPwdUsuario = KeytiaServiceBL.Util.Encrypt(lsPwdUsuario);
                lsQuery += (lsQuery.Length > 0 ? " and " : "") + "{Password} = '" + lsPwdUsuario + "'";
                bUsrIdentificable = true;
            }
            if (!String.IsNullOrEmpty(lsEmail))
            {
                lsQuery += (lsQuery.Length > 0 ? " and " : "") + "{Email} = '" + lsEmail + "'";
                bUsrIdentificable = true;
            }
            if (!String.IsNullOrEmpty(lsCodUsuarioDB))
            {
                lsQuery += (lsQuery.Length > 0 ? " and " : "") + "{UsuarDB} = '" + lsCodUsuarioDB + "'";
            }
            //ldtUsuario = kdb.GetHisRegByCod("Usuar", new string[] { lsCodUsuario }, new string[] { "iCodRegistro", "iCodCatalogo", "{Password}", "{UsuarDB}" });

            if (String.IsNullOrEmpty(lsQuery) || !bUsrIdentificable)
            {
                return null;
            }
            else
            {
                ldtUsuario = kdb.GetHisRegByEnt("Usuar", "Usuarios", new string[] { "iCodRegistro", "iCodCatalogo", "{Password}", "{Email}", "{UsuarDB}" }, lsQuery);
                if (ldtUsuario != null && ldtUsuario.Rows.Count == 0)
                {
                    return GetUsuarioDetallado(lsCodUsuario, lsPwdUsuario, lsEmail, lsCodUsuarioDB);
                }
            }
            return ldtUsuario.Rows[0];
        }
        protected System.Data.DataRow GetUsuarioDetallado(string lsCodUsuario, string lsPwdUsuario, string lsEmail, string lsCodUsuarioDB)
        {
            System.Data.DataTable ldtUsuario = null;
            KDBAccess kdb = new KDBAccess();
            string lsWhere = "";
            string lsColumnas = "";
            Hashtable phtColumns;
            Hashtable phtColumnsDetalado;
            StringBuilder psbQuery = new StringBuilder();

            // Obtener el Codigo de Maestro para buscar en detallados
            psbQuery.Length = 0;
            psbQuery.AppendLine("select iCodRegistro from Maestros where vchDescripcion  = 'Detallado Usuarios'");
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and iCodEntidad = (Select iCodRegistro from Catalogos where vchCodigo = 'Detall' ");
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and iCodCatalogo is null)");

            string iCodMaestro = DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString();
            string[] lsaColumnas = { "{iNumRegistro}", "{iNumCatalogo}", "{Password}", "{Email}", "{UsuarDB}", "{VchCodUsuario}" };
            string lsDetEmail = "", lsDetPassword = "", lsDetUsuarDB = "";

            foreach (string lsCol in lsaColumnas)
            {
                phtColumns = new Hashtable();
                phtColumns.Add(lsCol, "");
                phtColumnsDetalado = Util.TraducirHistoricos("Detall", "Detallado Usuarios", phtColumns);
                foreach (string lsColName in phtColumnsDetalado.Keys)
                {
                    if (lsColumnas.Length > 0)
                    {
                        lsColumnas += ",";
                    }
                    if (lsCol != "{VchCodUsuario}")
                    {
                        if (lsCol == "{iNumRegistro}")
                        {
                            lsColumnas += "iCodRegistro" + " = " + lsColName;
                        }
                        else if (lsCol == "{iNumCatalogo}")
                        {
                            lsColumnas += "iCodCatalogo" + " = " + lsColName;
                        }
                        else
                        {
                            lsColumnas += "[" + lsCol + "]" + " = " + lsColName;
                        }
                    }
                    else
                    {
                        lsColumnas += "vchCodigo" + " = " + lsColName;
                        if (!String.IsNullOrEmpty(lsCodUsuario))
                        {
                            lsWhere += " and " + lsColName + " = '" + lsCodUsuario + "'\n";
                        }
                    }

                    if (lsCol == "{Password}")
                    {
                        lsDetPassword = lsColName;
                    }
                    else if (lsCol == "{Email}")
                    {
                        lsDetEmail = lsColName;
                    }
                    else if (lsCol == "{UsuarDB}")
                    {
                        lsDetUsuarDB = lsColName;
                    }
                }
            }
            if (!String.IsNullOrEmpty(lsPwdUsuario))
            {
                lsWhere += " and " + lsDetPassword + " = '" + lsPwdUsuario + "'\n";
            }
            if (!String.IsNullOrEmpty(lsEmail))
            {
                lsWhere += " and " + lsDetEmail + " = '" + lsEmail + "'\n";
            }
            if (!String.IsNullOrEmpty(lsCodUsuarioDB))
            {
                lsWhere += " and " + lsDetUsuarDB + " = '" + lsCodUsuarioDB + "'\n";
            }
            psbQuery.Length = 0;
            psbQuery.AppendLine("select " + lsColumnas);
            psbQuery.AppendLine("from Detallados");
            psbQuery.AppendLine("where iCodMaestro = " + iCodMaestro);
            psbQuery.AppendLine(lsWhere);

            ldtUsuario = DSODataAccess.Execute(psbQuery.ToString());
            if (ldtUsuario != null && ldtUsuario.Rows.Count == 0)
            {
                return null;
            }
            return ldtUsuario.Rows[0];
        }

        public string GetPwdUsuario()
        {
            string lsPassword = "";

            //TripleDESWrapper DesPSW = new TripleDESWrapper();
            //lsPassword = DesPSW.Decrypt(pvchPwdUsuario);
            lsPassword = KeytiaServiceBL.Util.Decrypt(pvchPwdUsuario);

            return lsPassword;
        }
        public String CreaUsuario(string lsNombre, string lsPaterno, string lsMaterno)
        {
            //1.	En base al nombre: 
            //Para crear el username se tomará la primer letra del nombre y el primer apellido completo. 
            //Ejemplo: Juan Pérez Morales quedaría como: jperez
            //En caso que coincida con un usuario ya creado en sistema se deberán tomar las dos primeras 
            //letras del nombre y el primer apellido completo, entonces quedaría como: juperez
            //En caso de que ya exista un usuario con esa login, tomaríamos las primeras dos letras del 
            //nombre y el segundo apellido completo, quedaría entonces como: jumorales
            //En caso de que también ya exista un usuario con ese login, no deberá darse de alta 
            //el empleado, enviando una notificación de que no pudo ser creado porque ya existen 
            //usuarios con la condición seleccionada.

            //Para la creación del login se debe considerar que: en caso de que el nombre o los apellidos 
            //cuenten con alguno de los siguientes caracteres, se deberán reemplazar como se muestra a continuación:
            //á --> a
            //é --> e
            //í --> i
            //ó --> o
            //ú --> u
            //ñ --> n

            //Espacio en blanco --> eliminar espacios en blanco
            //# $ % & / ( ) = ¡ ! ¿ ? + - * ; . { } ´ _ < > \ | @  --> Eliminar character (\) No se puede replanzar

            lsNombre = lsNombre.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o")
                        .Replace("ú", "u").Replace("ñ", "n");
            lsNombre = lsNombre.Replace("#", "").Replace("$", "").Replace("%", "").Replace("&", "").Replace("/", "")
                        .Replace("(", "").Replace(")", "").Replace("=", "").Replace("¡", "").Replace("!", "")
                        .Replace("¿", "").Replace("?", "").Replace("+", "").Replace("-", "").Replace("*", "")
                        .Replace(";", "").Replace(".", "").Replace("{", "").Replace("}", "").Replace("´", "")
                        .Replace("_", "").Replace("<", "").Replace(">", "").Replace(" ", "").Replace("|", "")
                        .Replace("@", "");

            lsPaterno = lsPaterno.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o")
                        .Replace("ú", "u").Replace("ñ", "n");
            lsPaterno = lsPaterno.Replace("#", "").Replace("$", "").Replace("%", "").Replace("&", "").Replace("/", "")
                        .Replace("(", "").Replace(")", "").Replace("=", "").Replace("¡", "").Replace("!", "")
                        .Replace("¿", "").Replace("?", "").Replace("+", "").Replace("-", "").Replace("*", "")
                        .Replace(";", "").Replace(".", "").Replace("{", "").Replace("}", "").Replace("´", "")
                        .Replace("_", "").Replace("<", "").Replace(">", "").Replace(" ", "").Replace("|", "")
                        .Replace("@", "");
            lsMaterno = lsMaterno.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o")
                        .Replace("ú", "u").Replace("ñ", "n");
            lsMaterno = lsMaterno.Replace("#", "").Replace("$", "").Replace("%", "").Replace("&", "").Replace("/", "")
                        .Replace("(", "").Replace(")", "").Replace("=", "").Replace("¡", "").Replace("!", "")
                        .Replace("¿", "").Replace("?", "").Replace("+", "").Replace("-", "").Replace("*", "")
                        .Replace(";", "").Replace(".", "").Replace("{", "").Replace("}", "").Replace("´", "")
                        .Replace("_", "").Replace("<", "").Replace(">", "").Replace(" ", "").Replace("|", "")
                        .Replace("@", "");

            lsNombre = lsNombre.Trim();
            lsPaterno = lsPaterno.Trim();
            lsMaterno = lsMaterno.Trim();

            //'Para crear el username se tomará la primer letra del nombre y el primer apellido completo. 
            //'Ejemplo: Juan Pérez Morales quedaría como: jperez

            pvchCodUsuario = lsNombre.Substring(0, 1) + lsPaterno;
            if (!ExiUsuario())
            {
                return pvchCodUsuario;
            }

            //'En caso que coincida con un usuario ya creado en sistema se deberán tomar las dos primeras 
            //'letras del nombre y el primer apellido completo, entonces quedaría como: juperez

            pvchCodUsuario = lsNombre.Substring(0, 2) + lsPaterno;

            if (!ExiUsuario())
            {
                return pvchCodUsuario;
            }

            //'En caso de que ya exista un usuario con esa login, tomaríamos las primeras dos letras del 
            //'nombre y el segundo apellido completo, quedaría entonces como: jumorales

            if (lsMaterno.Length > 0)
            {
                pvchCodUsuario = lsNombre.Substring(0, 2) + lsMaterno;

                if (!ExiUsuario())
                {
                    return pvchCodUsuario;
                }
            }

            // AM 20131008.
            #region Crear usuario en base al nombre y un numero aleatorio
            // Se agrega bloque para crear usuario en base al nombre, 
            // Toma las 2 primeras letras del nombre, le concatena el ap. paterno y
            // le agrega un numero aleatorio al final que oscila entre los numeros 100 y 999.

            Random ranNum = new Random();
            int numeroRandom = ranNum.Next(100, 999);

            pvchCodUsuario = lsNombre.Substring(0, 2) + lsPaterno + numeroRandom.ToString();

            if (!ExiUsuario())
            {
                return pvchCodUsuario;
            }

            #endregion

            return "null";
        }

        public String CreaUsuario(string lsEmail, bool lbEmail)
        {
            //2.	En base al correo electrónico:
            //El username se formará en base al correo electrónico que se haya incluido como atributo del 
            //empleado, se tomará sólo el nombre de usuario (lo que viene antes de la @). 
            //Ejemplo: para el mail gramirez@dti.com.mx, el usuario deberá ser gramirez.
            //En caso de que el login ya exista, el empleado no será dado de alta, enviando un mensaje indicando 
            //que el usuario con ese login ya existe en sistema.
            //En el caso de que se intente dar de alta un empleado sin correo electrónico, se deberá considerar 
            //en automático lo descrito en el punto 1.  

            if (lsEmail.Length > 0)
            {
                string[] lstNombre = lsEmail.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                pvchCodUsuario = lstNombre[0].Trim();

                if (pvchCodUsuario.Length > 0 && !ExiUsuario())
                {
                    return pvchCodUsuario;
                }
            }

            return "null";
        }

        public String CreaUsuario(string lsNomina)
        {
            //3.	En base al número de nómina
            //El username será igual al número de nómina del empleado. Dado que la nómina no puede repetirse, 
            //no debería existir un usuario con ese mismo username, sin embargo, en caso de que se presente 
            //esa situación, el empleado no será dado de alta y se enviará un mensaje indicando que ya existe 
            //un usuario con ese login.

            pvchCodUsuario = lsNomina.Trim();
            if (!ExiUsuario())
            {
                return pvchCodUsuario;
            }

            return "null";
        }
        protected bool ExiUsuario()
        {
            bool lbExi = false;
            string lsCodUsuario = pvchCodUsuario;
            System.Data.DataTable ldtUsuario = null;
            KDBAccess kdb = new KDBAccess();

            ldtUsuario = kdb.GetHisRegByEnt("Usuar", "Usuarios", new string[] { "iCodRegistro", "iCodCatalogo", "{Password}", "{Email}", "{UsuarDB}" }, "vchCodigo = '" + lsCodUsuario + "'");
            if (ldtUsuario != null && ldtUsuario.Rows.Count > 0)
            {
                lbExi = true;
            }
            return (lbExi);
        }

        public String CreaPassword()
        {
            pvchPwdUsuario = "";

            return "null";

        }

        /*HD.20130530 Para uso en ventana de comentarios que solicita cliente Nextel 
         * Metodo para validar el Mail del empleado*/
        public static Boolean Validar_Email(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public String ValUsuarioEmailPassword()
        {
            String lbret = "";
            KDBAccess kdb = new KDBAccess();
            System.Data.DataTable ldtRow = null;
            int liCtx = DSODataContext.GetContext();

            try
            {
                //Obten la configuracion de usuario
                string lsPwdUsuario = KeytiaServiceBL.Util.Decrypt(pvchPwdUsuario);

                if (DSODataContext.Schema.Equals("Keytia"))
                {
                    ldtRow = GetUsuarioDetallado(pvchCodUsuario, "", pvchEmail);
                    if (ldtRow != null && ldtRow.Rows.Count > 0)
                    {
                        lbret = "ErrUsuarioEmail";
                    }
                    if (lbret == "")
                    {
                        ldtRow = GetUsuarioDetallado(pvchCodUsuario, lsPwdUsuario, "");
                        if (ldtRow != null && ldtRow.Rows.Count > 0)
                        {
                            lbret = "ErrUsuarioPwd";
                        }
                    }
                }
                else
                {

                    //Configuramos para el esquema KEYTIA
                    DSODataContext.SetContext();

                    ldtRow = GetUsuario(pvchCodUsuario, "", pvchEmail);

                    if (ldtRow != null && ldtRow.Rows.Count > 0)
                    {
                        int lUsuarDB = (int)ldtRow.Rows[0]["{UsuarDB}"];
                        if (ldtRow.Rows.Count > 1 || lUsuarDB != liCtx)
                        {
                            lbret = "ErrUsuarioEmail";
                        }
                    }
                    if (lbret == "")
                    {
                        ldtRow = GetUsuario(pvchCodUsuario, lsPwdUsuario, "");
                        if (ldtRow != null && ldtRow.Rows.Count > 0)
                        {
                            int lUsuarDB = (int)ldtRow.Rows[0]["{UsuarDB}"];
                            if (ldtRow.Rows.Count > 1 || lUsuarDB != liCtx)
                            {
                                lbret = "ErrUsuarioPwd";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Regresamos la configuración al cliente 
                throw new KeytiaWebException("ErrValUsuario", ex);
            }
            finally
            {
                //Regresamos la configuración al cliente 
                DSODataContext.SetContext(liCtx);
            }

            return lbret;

        }

        protected System.Data.DataTable GetUsuario(string lsCodUsuario, string lsPwdUsuario, string lsEmail)
        {
            System.Data.DataTable ldtUsuario = null;
            KDBAccess kdb = new KDBAccess();
            string lsQuery = "";
            string lsCodUsuarioDB = "";

            if (!String.IsNullOrEmpty(lsPwdUsuario))
            {
                lsPwdUsuario = KeytiaServiceBL.Util.Encrypt(lsPwdUsuario);
                lsQuery += " and {Password} = '" + lsPwdUsuario + "'";
            }
            if (!String.IsNullOrEmpty(lsEmail))
            {
                lsQuery += " and {Email} = '" + lsEmail + "'";
            }
            if (!String.IsNullOrEmpty(lsCodUsuarioDB))
            {
                lsQuery += " and {UsuarDB} = '" + lsCodUsuarioDB + "'";
            }
            //ldtUsuario = kdb.GetHisRegByCod("Usuar", new string[] { lsCodUsuario }, new string[] { "iCodRegistro", "iCodCatalogo", "{Password}", "{UsuarDB}" });

            ldtUsuario = kdb.GetHisRegByEnt("Usuar", "Usuarios", new string[] { "iCodRegistro", "iCodCatalogo", "{Password}", "{Email}", "{UsuarDB}" }, "vchCodigo = '" + lsCodUsuario + "'" + lsQuery);
            if (ldtUsuario != null && ldtUsuario.Rows.Count == 0)
            {
                if (String.IsNullOrEmpty(lsCodUsuario))
                {
                    return null;
                }
                else
                {
                    return GetUsuarioDetallado(lsCodUsuario, lsPwdUsuario, lsEmail);
                }
            }
            return ldtUsuario;
        }

        protected System.Data.DataTable GetUsuarioDetallado(string lsCodUsuario, string lsPwdUsuario, string lsEmail)
        {
            System.Data.DataTable ldtUsuario = null;
            KDBAccess kdb = new KDBAccess();
            string lsWhere = "";
            string lsColumnas = "";
            string lsCodUsuarioDB = "";
            Hashtable phtColumns;
            Hashtable phtColumnsDetalado;
            StringBuilder psbQuery = new StringBuilder();
            string[] lsaColumnas = { "{iNumRegistro}", "{iNumCatalogo}", "{Password}", "{Email}", "{UsuarDB}", "{VchCodUsuario}" };
            string lsDetEmail = "", lsDetPassword = "", lsDetUsuarDB = "";

            foreach (string lsCol in lsaColumnas)
            {
                phtColumns = new Hashtable();
                phtColumns.Add(lsCol, "");
                phtColumnsDetalado = Util.TraducirHistoricos("Detall", "Detallado Usuarios", phtColumns);
                foreach (string lsColName in phtColumnsDetalado.Keys)
                {
                    if (lsColumnas.Length > 0)
                    {
                        lsColumnas += ",";
                    }
                    if (lsCol != "{VchCodUsuario}")
                    {
                        if (lsCol == "{iNumRegistro}")
                        {
                            lsColumnas += "iCodRegistro" + " = " + lsColName;
                        }
                        else if (lsCol == "{iNumCatalogo}")
                        {
                            lsColumnas += "iCodCatalogo" + " = " + lsColName;
                        }
                        else
                        {
                            lsColumnas += "[" + lsCol + "]" + " = " + lsColName;
                        }
                    }
                    else
                    {
                        lsColumnas += "vchCodigo" + " = " + lsColName;
                        lsWhere += " and " + lsColName + " = '" + lsCodUsuario + "'\n";
                    }

                    if (lsCol == "{Password}")
                    {
                        lsDetPassword = lsColName;
                    }
                    else if (lsCol == "{Email}")
                    {
                        lsDetEmail = lsColName;
                    }
                    else if (lsCol == "{UsuarDB}")
                    {
                        lsDetUsuarDB = lsColName;
                    }
                }
            }
            if (!String.IsNullOrEmpty(lsPwdUsuario))
            {
                lsWhere += "and " + lsDetPassword + " = '" + lsPwdUsuario + "'\n";
            }
            if (!String.IsNullOrEmpty(lsEmail))
            {
                lsWhere += "and " + lsDetEmail + " = '" + lsEmail + "'\n";
            }
            if (!String.IsNullOrEmpty(lsCodUsuarioDB))
            {
                lsWhere += "and " + lsDetUsuarDB + " = '" + lsCodUsuarioDB + "'\n";
            }

            psbQuery.AppendLine("select " + lsColumnas);
            psbQuery.AppendLine("from Detallados");
            psbQuery.AppendLine("where 1 = 1");
            psbQuery.AppendLine(lsWhere);

            ldtUsuario = DSODataAccess.Execute(psbQuery.ToString());
            if (ldtUsuario != null && ldtUsuario.Rows.Count == 0)
            {
                return null;
            }
            return ldtUsuario;
        }

        protected System.Data.DataTable BuscarUsuario(string lsCurrentPassword)
        {
            KDBAccess kdb = new KDBAccess();
            System.Data.DataTable ldtUsuario = null;
            StringBuilder lsQuery = new StringBuilder();

            ldtUsuario = kdb.GetHisRegByEnt("Usuar", "Usuarios", new string[] { "iCodRegistro", "Tabla = 'Historicos'" },
                    "iCodCatalogo = " + HttpContext.Current.Session["iCodUsuario"] + " and {Password} = '" + KeytiaServiceBL.Util.Encrypt(lsCurrentPassword) + "'");
            if (ldtUsuario == null || ldtUsuario.Rows.Count == 0)
            {
                ldtUsuario = BuscarUsuarioDetallado(lsCurrentPassword);
                if (ldtUsuario == null || ldtUsuario.Rows.Count == 0)
                {
                    return null;
                }
            }

            return ldtUsuario;
        }

        protected System.Data.DataTable BuscarUsuarioDetallado(string lsCurrentPassword)
        {
            System.Data.DataTable ldtUsuario = null;
            KDBAccess kdb = new KDBAccess();
            string lsWhere = "";
            string lsColumnas = "iCodRegistro";
            Hashtable phtColumns;
            Hashtable phtColumnsDetalado;
            StringBuilder psbQuery = new StringBuilder();
            string[] lsaColumnas = { "{iNumCatalogo}", "{Password}", "{VchCodUsuario}" };
            string lsDetPassword = "";

            foreach (string lsCol in lsaColumnas)
            {
                phtColumns = new Hashtable();
                phtColumns.Add(lsCol, "");
                phtColumnsDetalado = Util.TraducirHistoricos("Detall", "Detallado Usuarios", phtColumns);
                foreach (string lsColName in phtColumnsDetalado.Keys)
                {
                    if (lsCol == "{VchCodUsuario}")
                    {
                        lsColumnas += ", vchCodigo" + " = " + lsColName;

                    }
                    if (lsCol == "{iNumCatalogo}")
                    {
                        lsWhere += " and " + lsColName + " = '" + HttpContext.Current.Session["iCodUsuario"] + "'\n";
                    }
                    if (lsCol == "{Password}")
                    {
                        lsDetPassword = lsColName;
                    }
                }
            }

            lsColumnas += ", Tabla = 'Detallados'";
            lsWhere += "and " + lsDetPassword + " = '" + KeytiaServiceBL.Util.Encrypt(lsCurrentPassword) + "'\n";

            psbQuery.AppendLine("select " + lsColumnas);
            psbQuery.AppendLine("from Detallados");
            psbQuery.AppendLine("where 1 = 1");
            psbQuery.AppendLine(lsWhere);

            ldtUsuario = DSODataAccess.Execute(psbQuery.ToString());
            if (ldtUsuario != null && ldtUsuario.Rows.Count == 0)
            {
                return null;
            }
            return ldtUsuario;
        }

        /// <summary>
        /// Desencripta un texto encriptado con el algoritmo simple, definido en DTI
        /// </summary>
        /// <param name="texto">Texto encriptado</param>
        /// <returns>Texto desencriptado</returns>
        public string DecryptSimpleAlgorithm(string textoEncriptado, int posicionesRestadas)
        {
            string textoDesencriptado = string.Empty;

            Dictionary<string, int> valores = new Dictionary<string, int>()
                {
                    {"a",1},
                    {"b",2},
                    {"c",3},
                    {"d",4},
                    {"e",5},
                    {"f",6},
                    {"g",7},
                    {"h",8},
                    {"i",9},
                    {"j",10},
                    {"k",11},
                    {"l",12},
                    {"m",13},
                    {"n",14},
                    {"ñ",15},
                    {"o",16},
                    {"p",17},
                    {"q",18},
                    {"r",19},
                    {"s",20},
                    {"t",21},
                    {"u",22},
                    {"v",23},
                    {"w",24},
                    {"x",25},
                    {"y",26},
                    {"z",27},
                    {".",28},
                    {"_",29},
                    {"-",30},
                    {"0",31},
                    {"1",32},
                    {"2",33},
                    {"3",34},
                    {"4",35},
                    {"5",36},
                    {"6",37},
                    {"7",38},
                    {"8",39},
                    {"9",40},
                };

            if (!string.IsNullOrEmpty(textoEncriptado))
            {


                for (int i = 0; i <= textoEncriptado.Length - 1; i++)
                {
                    string letra = textoEncriptado.Substring(i, 1);

                    if (valores.ContainsKey(letra))
                    {
                        int valorLetra = valores[letra];
                        int valorLetraOriginal = 0;

                        if (valores.Count >= (valorLetra + posicionesRestadas))
                        {
                            valorLetraOriginal = valorLetra + posicionesRestadas;
                        }
                        else
                        {
                            valorLetraOriginal = (valorLetra + posicionesRestadas) - valores.Count;
                        }


                        textoDesencriptado += valores.FirstOrDefault(x => x.Value == valorLetraOriginal).Key;

                    }
                }
            }
            return textoDesencriptado;
        }

        #region WebMethods

        public static string ChangePassword(string lsCurrentPassword, string lsNewPassword, string lsNewPasswordConf, string wnd)
        {
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
            string lsRet = "";
            System.Data.DataTable dt;
            bool lbHayError = false;

            if (lsCurrentPassword == "")
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetLangItem("CampoRequerido", Globals.GetLangItem("PwdActual")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            if (!lbHayError && lsNewPassword == "")
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetLangItem("CampoRequerido", Globals.GetLangItem("PwdNueva")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            if (!lbHayError && lsNewPasswordConf == "")
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetLangItem("CampoRequerido", Globals.GetLangItem("PwdNuevaConf")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            if (!lbHayError && lsNewPassword != lsNewPasswordConf)
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetMsgWeb("CampoNoIgual", Globals.GetLangItem("PwdNueva"), Globals.GetLangItem("PwdNuevaConf")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }
            //20131001.PT: Se cambio la longitd minima de 8 a 6 caracteres
            if (!lbHayError && (lsNewPassword.Length < 6 || lsNewPassword.Length > 32))
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetMsgWeb("ValLength", "6", "32", Globals.GetLangItem("PwdNueva")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            if (!lbHayError && (lsCurrentPassword == lsNewPassword))
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetMsgWeb("CampoIgual", Globals.GetLangItem("PwdActual"), Globals.GetLangItem("PwdNueva")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            Usuarios usr = new Usuarios();
            if (!lbHayError && !usr.ValUsuarioPassword(lsNewPassword))
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                        "message: '" + Globals.GetLangItem("USRep") + "', " +
                        "wnd: '" + wnd + "', " +
                        "error: 1 }";
                lbHayError = true;
            }


            if (!lbHayError)
            {
                dt = kdb.GetHisRegByEnt("Usuar", "Usuarios", new string[] { "iCodRegistro" },
                    "iCodCatalogo = " + HttpContext.Current.Session["iCodUsuario"] + " and {Password} = '" + KeytiaServiceBL.Util.Encrypt(lsCurrentPassword) + "'");

                if (dt == null || dt.Rows.Count <= 0)
                {
                    lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                        "message: '" + Globals.GetLangItem("PwInc") + "', " +
                        "wnd: '" + wnd + "', " +
                        "error: 1 }";

                    lbHayError = true;
                }
                else
                {
                    KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                    Hashtable htVal = new Hashtable();
                    htVal.Add("{Password}", KeytiaServiceBL.Util.Encrypt(lsNewPassword));

                    if (lCargasCOM.GuardaUsuarioEnKeytia(htVal, (int)dt.Rows[0]["iCodRegistro"], true, false, (int)HttpContext.Current.Session["iCodUsuarioDB"]))
                    {
                        kdb.Update("historicos", "Usuar", "Usuarios", htVal, (int)dt.Rows[0]["iCodRegistro"]);
                        lsRet = "{ title: 'Keytia', " +
                                "message: '" + Globals.GetLangItem("OpExito") + "', " +
                                "wnd: '" + wnd + "', " +
                                "error: 0 }";
                    }
                    else
                    {

                        lsRet = "{ title: 'Keytia', " +
                                "message: '" + Globals.GetLangItem("OpErr") + "', " +
                                "wnd: '" + wnd + "', " +
                                "error: 0 }";
                    }
                }
            }

            return lsRet;
        }

        /**++**HD Agrego este metodo para procesar los comentarios*/
        public static string UsrComment(string lsCommentName, string lsCommentCorreo, string lscboAsunto, string lsCommentComentario, string wnd)
        {
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
            string lsRet = "";
            bool lbHayError = false;

            //valida que el textbox lsCommentCorreo no este vacio
            if (lsCommentCorreo == "")
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetLangItem("CampoRequerido", Globals.GetLangItem("usrCommentMail")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            //valida que el combo cboAsunt no este vacio
            if (!lbHayError && lscboAsunto == "")
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetLangItem("CampoRequerido", Globals.GetLangItem("usrCommentSub")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            //valida que el textbox lsCommentComentario no este vacio
            if (!lbHayError && lsCommentComentario == "")
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetLangItem("CampoRequerido", Globals.GetLangItem("usrCommentCom")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            //Se valida que el textBox lsCommentCorreo tenga un correo valido
            if (!lbHayError && !Validar_Email(lsCommentCorreo))
            {
                lsRet = "{ title: '" + Globals.GetLangItem("ErrorTit") + "', " +
                    "message: '" + Globals.GetMsgWeb("ValEmplFormato", Globals.GetLangItem("usrCommentMail")) + "', " +
                    "wnd: '" + wnd + "', " +
                    "error: 1 }";

                lbHayError = true;
            }

            //Si todos los textbox contienen informacion y el textBox lsCommentCorreo tiene un correo valido entonces se guardaran los datos en la bd
            if (!lbHayError)
            {
                //Se insertan los Registros en la Base de Datos
                Hashtable phtValoresCampos = new Hashtable();
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                DateTime ldtUltAcc = DateTime.Now;


                /***++**HD ~~PT
                * El siguiente segmento consulta el Parametros de configuracion de envios de la empresa que esta relacionado con el usuario actual
                * para grabarlo en la tabla de comentarios de usuario, si la consulta no regrese datos se pondra en blanco
                */

                //Saca la empresa del usuario actual
                string qryEmpre = "select Empre from [VisHistoricos('Usuar','Usuarios','Español')] where iCodCatalogo = " + (int)HttpContext.Current.Session["iCodUsuario"];

                string empresa = null;
                DataRow drempre = null;
                drempre = KeytiaServiceBL.DSODataAccess.ExecuteDataRow(qryEmpre);

                if (drempre != null)
                    empresa = drempre["Empre"].ToString();


                DataRow drparamEnvio = null;
                string qry = "select CtaPara, CtaCC, CtaCCO from [VisHistoricos('ParamEnvios','Envio de Comentarios de Usuario','Español')] where dtIniVigencia<>dtFinVigencia and dtFinVigencia>=GETDATE() and Empre = " + empresa ;
                drparamEnvio = KeytiaServiceBL.DSODataAccess.ExecuteDataRow(qry);
                string ctaPara = "";
                string ctaCC = "";
                string ctaCCO = "";
                

                if (drparamEnvio != null)
                {
                    ctaPara = drparamEnvio["CtaPara"].ToString();
                    ctaCC = drparamEnvio["CtaCC"].ToString();
                    ctaCCO = drparamEnvio["CtaCCO"].ToString();
                  
                }


                /*Llenado del HashTable que contiene el nombre del campo y el valor que se insertara en la base de datos*/
                phtValoresCampos.Add("vchCodigo", "Coment " + ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                phtValoresCampos.Add("vchDescripcion", "Comentario" + " " + ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                phtValoresCampos.Add("{AsuntoCom}", lscboAsunto.ToString());
                phtValoresCampos.Add("{FechaAsignacion}", ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //phtValoresCampos.Add("{FechaEnvio}", ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                phtValoresCampos.Add("{NomCompleto}", lsCommentName.ToString());
                phtValoresCampos.Add("{CtaDe}", lsCommentCorreo.ToString());
                phtValoresCampos.Add("{CtaPara}", ctaPara);
                phtValoresCampos.Add("{CtaCC}", ctaCC);
                phtValoresCampos.Add("{CtaCCO}", ctaCCO);
                phtValoresCampos.Add("{Comentarios}", lsCommentComentario.ToString());
                phtValoresCampos.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"].ToString());

               

                if (((string)phtValoresCampos["vchCodigo"]).Length > 40)
                    phtValoresCampos["vchCodigo"] = ((string)phtValoresCampos["vchCodigo"]).Substring(0, 40);
                try
                {
                    int res = 0;
                    /*Insert a Base de Datos del Comentario*/
                    res = lCargasCOM.InsertaRegistro(phtValoresCampos, "Historicos", "Comentario", "Comentarios de Usuarios", (int)HttpContext.Current.Session["iCodUsuarioDB"]);


                }
                catch (Exception ex)
                {
                    KeytiaServiceBL.Util.LogException("No se pudo grabar el registro del comentario de usuario '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
                //Mensaje de Exito
                lsRet = "{ title: 'Keytia', " +
                                "message: '" + Globals.GetLangItem("OpExito") + "', " +
                                "wnd: '" + wnd + "', " +
                                "error: 0 }";

            }


            return lsRet;
        }

        protected bool ValUsuarioPassword(string lsNewPassword)
        {
            bool lbRet = true;
            DSODataContext.SetContext(0);

            System.Data.DataTable ldtUsuarioActualizar = BuscarUsuario(lsNewPassword);
            if (ldtUsuarioActualizar != null && ldtUsuarioActualizar.Rows.Count > 0)
            {
                lbRet = false;
            }

            DSODataContext.SetContext(int.Parse(HttpContext.Current.Session["iCodUsuarioDB"].ToString()));
            return lbRet;
        }

        #endregion
    }

    public class GeneradorPassword
    {

        /// <summary>
        /// Enumeración que permite conocer el tipo de juego de carácteres a emplear
        /// para cada carácter
        /// </summary>
        private enum TipoCaracterEnum { Minuscula, Mayuscula, Simbolo, Numero }

        #region Campos

        private int porcentajeMayusculas;
        private int porcentajeSimbolos;
        private int porcentajeNumeros;
        Random semilla;

        // Caracteres que pueden emplearse en la contraseña
        string caracteres = "abcdefghijklmnopqrstuvwxyz";
        string numeros = "0123456789";
        string simbolos = "%$#@+-=&";

        // Cadena que contiene el password generado
        private StringBuilder password;

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece la longitud en carácteres de la contraseña a obtener
        /// </summary>
        public int LongitudPassword { get; set; }

        /// <summary>
        /// Obtiene o establece el porcentaje de carácteres en mayúsculas que 
        /// contendrá la contraseña
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se produce al intentar introducir
        /// un valor que no coincida con un porcentaje</exception>
        public int PorcentajeMayusculas
        {
            get { return porcentajeMayusculas; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("El porcentaje es un número entre 0 y 100");
                porcentajeMayusculas = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el porcentaje de símbolos que contendrá la contraseña
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se produce al intentar introducir
        /// un valor que no coincida con un porcentaje</exception>
        public int PorcentajeSimbolos
        {
            get { return porcentajeSimbolos; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("El porcentaje es un número entre 0 y 100");
                porcentajeSimbolos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el número de caracteres numéricos que contendrá la contraseña
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se produce al intentar introducir
        /// un valor que no coincida con un porcentaje</exception>
        public int PorcentajeNumeros
        {
            get { return porcentajeNumeros; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("El porcentaje es un número entre 0 y 100");
                porcentajeNumeros = value;
            }
        }

        #endregion

        #region Constructores
        /// <summary>
        /// Constructor. La contraseña tendrá 8 caracteres, incluyendo una letra mayúscula, 
        /// un número y un símbolo
        /// 
        /// </summary>
        public GeneradorPassword()
            : this((new Random()).Next(6, 16))
        { }

        /// <summary>
        /// Constructor. La contraseña tendrá un 20% de caracteres en mayúsculas y otro tanto de 
        /// símbolos
        /// </summary>
        /// <param name="longitudCaracteres">Longitud en carácteres de la contraseña a obtener</param>
        /// <exception cref="ArgumentOutOfRangeException">Se produce al intentar introducir
        /// un porcentaje de caracteres especiales mayor de 100</exception>
        public GeneradorPassword(int longitudCaracteres)
            : this(longitudCaracteres, 20, 20, 20)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="longitudCaracteres">Longitud en carácteres de la contraseña a obtener</param>
        /// <param name="porcentajeMayusculas">Porcentaje a aplicar de caracteres en mayúscula</param>
        /// <param name="porcentajeSimbolos">Porcenta a aplicar de símbolos</param>
        /// <param name="porcentajeNumeros">Porcentaje de caracteres numéricos</param>
        /// <exception cref="ArgumentOutOfRangeException">Se produce al intentar introducir
        /// un porcentaje de caracteres especiales mayor de 100</exception>
        public GeneradorPassword(int longitudCaracteres, int porcentajeMayusculas, int porcentajeSimbolos, int porcentajeNumeros)
        {
            LongitudPassword = longitudCaracteres;
            PorcentajeMayusculas = porcentajeMayusculas;
            PorcentajeSimbolos = porcentajeSimbolos;
            PorcentajeNumeros = porcentajeNumeros;

            if (PorcentajeMayusculas + porcentajeSimbolos + PorcentajeNumeros > 100)
                throw new ArgumentOutOfRangeException(
                "La suma de los porcentajes de caracteres especiales no puede superar el " +
                "100%, es decir, no puede ser superior a la longitud de la contraseña");
            semilla = new Random(DateTime.Now.Millisecond);
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Obtiene el password
        /// </summary>
        /// <returns></returns>
        public string GetNewPassword()
        {
            GeneraPassword();
            return password.ToString();
        }

        /// <summary>
        /// Permite establecer el número de caracteres especiales que se quieren obtener
        /// </summary>
        /// <param name="numeroCaracteresMayuscula">Número de caracteres en mayúscula</param>
        /// <param name="numeroCaracteresNumericos">Número de caracteres numéricos</param>
        /// <param name="numeroCaracteresSimbolos">Número de caracteres de símbolos</param>
        public void SetCaracteresEspeciales(
            int numeroCaracteresMayuscula
            , int numeroCaracteresNumericos
            , int numeroCaracteresSimbolos)
        {
            // Comprobación de errores
            if (numeroCaracteresMayuscula
                    + numeroCaracteresNumericos
                    + numeroCaracteresSimbolos > LongitudPassword)
                throw new ArgumentOutOfRangeException(
                    "El número de caracteres especiales no puede superar la longitud del password");

            PorcentajeMayusculas = numeroCaracteresMayuscula * 100 / LongitudPassword;
            PorcentajeNumeros = numeroCaracteresNumericos * 100 / LongitudPassword;
            PorcentajeSimbolos = numeroCaracteresSimbolos * 100 / LongitudPassword;
        }

        /// <summary>
        /// Constructor. La contraseña tendrá 8 caracteres, incluyendo una letra mayúscula, 
        /// un número y un símbolo
        /// </summary>
        public static string GetPassword()
        {
            // Se crea un método estático para facilitar el uso
            GeneradorPassword gp = new GeneradorPassword();
            return gp.GetNewPassword();
        }

        #endregion

        #region Métodos de cálculo

        /// <summary>
        /// Método que genera el password. Primero crea una cadena de caracteres 
        /// en minúscula y va sustituyendo los caracteres especiales
        /// </summary>
        private void GeneraPassword()
        {
            // Se genera una cadena de caracteres en minúscula con la longitud del 
            // password seleccionado
            password = new StringBuilder(LongitudPassword);
            for (int i = 0; i < LongitudPassword; i++)
            {
                password.Append(GetCaracterAleatorio(TipoCaracterEnum.Minuscula));
            }

            // Se obtiene el número de caracteres especiales (Mayúsculas y caracteres) 
            int numMayusculas = (int)(LongitudPassword * (PorcentajeMayusculas / 100d));
            int numSimbolos = (int)(LongitudPassword * (PorcentajeSimbolos / 100d));
            int numNumeros = (int)(LongitudPassword * (PorcentajeNumeros / 100d));

            // Se obtienen las posiciones en las que irán los caracteres especiales
            int[] caracteresEspeciales =
                    GetPosicionesCaracteresEspeciales(numMayusculas + numSimbolos + numNumeros);
            int posicionInicial = 0;
            int posicionFinal = 0;

            // Se reemplazan las mayúsculas
            posicionFinal += numMayusculas;
            ReemplazaCaracteresEspeciales(caracteresEspeciales,
                 posicionInicial, posicionFinal, TipoCaracterEnum.Mayuscula);

            // Se reemplazan los símbolos
            posicionInicial = posicionFinal;
            posicionFinal += numSimbolos;
            ReemplazaCaracteresEspeciales(caracteresEspeciales,
                 posicionInicial, posicionFinal, TipoCaracterEnum.Simbolo);

            // Se reemplazan los Números
            posicionInicial = posicionFinal;
            posicionFinal += numNumeros;
            ReemplazaCaracteresEspeciales(caracteresEspeciales,
                 posicionInicial, posicionFinal, TipoCaracterEnum.Numero);
        }

        /// <summary>
        /// Reemplaza un caracter especial en la cadena Password
        /// </summary>
        private void ReemplazaCaracteresEspeciales(
                                        int[] posiciones
                                        , int posicionInicial
                                        , int posicionFinal
                                        , TipoCaracterEnum tipoCaracter)
        {
            for (int i = posicionInicial; i < posicionFinal; i++)
            {
                password[posiciones[i]] = GetCaracterAleatorio(tipoCaracter);
            }
        }

        /// <summary>
        /// Obtiene un array con las posiciones en las que deberán colocarse los caracteres
        /// especiales (Mayúsculas o Símbolos). Es importante que no se repitan los números
        /// de posición para poder mantener el porcentaje de dichos carácteres
        /// </summary>
        /// <param name="numeroPosiciones">Valor que representa el número de posiciones
        /// que deberán crearse sin repetir</param>
        private int[] GetPosicionesCaracteresEspeciales(int numeroPosiciones)
        {
            List<int> lista = new List<int>();
            while (lista.Count < numeroPosiciones)
            {
                int posicion = semilla.Next(0, LongitudPassword);
                if (!lista.Contains(posicion))
                {
                    lista.Add(posicion);
                }
            }
            return lista.ToArray();
        }

        /// <summary>
        /// Obtiene un carácter aleatorio en base a la "matriz" del tipo de caracteres
        /// </summary>
        private char GetCaracterAleatorio(TipoCaracterEnum tipoCaracter)
        {
            string juegoCaracteres;
            switch (tipoCaracter)
            {
                case TipoCaracterEnum.Mayuscula:
                    juegoCaracteres = caracteres.ToUpper();
                    break;
                case TipoCaracterEnum.Minuscula:
                    juegoCaracteres = caracteres.ToLower();
                    break;
                case TipoCaracterEnum.Numero:
                    juegoCaracteres = numeros;
                    break;
                default:
                    juegoCaracteres = simbolos;
                    break;
            }

            // índice máximo de la matriz char de caracteres
            int longitudJuegoCaracteres = juegoCaracteres.Length;

            // Obtención de un número aletorio para obtener la posición del carácter
            int numeroAleatorio = semilla.Next(0, longitudJuegoCaracteres);

            // Se devuelve una posición obtenida aleatoriamente
            return juegoCaracteres[numeroAleatorio];
        }

        #endregion

    }

}