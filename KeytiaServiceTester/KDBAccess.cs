//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;

//namespace KeytiaServiceTester
//{
//    class KDBAccess
//    {
//        private DateTime pdtFechaVigencia;

//        public KDBAccess() { pdtFechaVigencia = DateTime.Today; }
//        public KDBAccess(DateTime ldtFechaVigencia) { pdtFechaVigencia = ldtFechaVigencia; }

//        public DateTime FechaVigencia
//        {
//            get { return pdtFechaVigencia; }
//            set { pdtFechaVigencia = value; }
//        }

//        /*
//         * Get [A] [B] By [C]
//         * [A] = [Rel,Cat,Mae,His]
//         * [B] = [Cod,Val,Ids,Reg]
//         * [C] = [Ids,Cod,Des,Ent]
//         */

//        #region GetHisRegByCod
//        public DataTable GetHisRegByCod(string lsEntidad, string[] lsCods)
//        {
//            return GetHisRegByCod(lsEntidad, lsCods, null);
//        }

//        public DataTable GetHisRegByCod(string lsEntidad, string[] lsCods, string[] lsCampos)
//        {
//            DataTable ldt = null;
//            Hashtable lhtCamposTodos = null;

//            lhtCamposTodos = CamposHis(lsEntidad, "");

//            ldt = SiFElDB.DSODataAccess.Execute(
//                    GetQuery(lhtCamposTodos, lsCampos,
//                        "his.iCodRegistro in (" + "\r\n" +
//                        "   select  distinct iCodMaestro " + "\r\n" +
//                        "   from    catalogos ent" + "\r\n" +
//                        "           inner join catalogos cat" + "\r\n" +
//                        "               on cat.iCodCatalogo = ent.iCodRegistro" + "\r\n" +
//                        "               and cat.vchCodigo in (" + ArrayToList(lsCods, ",", "'") + ")" + "\r\n" +
//                        "           inner join historicos his" + "\r\n" +
//                        "               on his.iCodCatalogo = cat.iCodRegistro" + "\r\n" +
//                        "   where   ent.vchCodigo = '" + lsEntidad + "')" + "\r\n",
//                        ""));

//            return ldt;
//        }
//        #endregion

//        #region GetHisRegByDes
//        public DataTable GetHisRegByDes(string lsEntidad, string[] lsDescripcion)
//        {
//            return GetHisRegByDes(lsEntidad, lsDescripcion, null);
//        }

//        public DataTable GetHisRegByDes(string lsEntidad, string[] lsDescripcion, string[] lsCampos)
//        {
//            DataTable ldt = null;
//            Hashtable lhtCamposTodos = null;

//            lhtCamposTodos = CamposHis(lsEntidad, "");

//            ldt = SiFElDB.DSODataAccess.Execute(
//                    GetQuery(lhtCamposTodos, lsCampos,
//                        "his.iCodRegistro in (" + "\r\n" +
//                        "   select  distinct iCodMaestro " + "\r\n" +
//                        "   from    catalogos ent" + "\r\n" +
//                        "           inner join catalogos cat" + "\r\n" +
//                        "               on cat.iCodCatalogo = ent.iCodRegistro" + "\r\n" +
//                        "               and cat.vchCodigo in (" + ArrayToList(lsDescripcion, ",", "'") + ")" + "\r\n" +
//                        "           inner join historicos his" + "\r\n" +
//                        "               on his.iCodCatalogo = cat.iCodRegistro" + "\r\n" +
//                        "   where   ent.vchCodigo = '" + lsEntidad + "')" + "\r\n",
//                        ""));

//            return ldt;
//        }
//        #endregion

//        #region GetHisRegByEnt
//        public DataTable GetHisRegByEnt(string lsEntidad, string lsMaestro)
//        {
//            return GetHisRegByEnt(lsEntidad, lsMaestro, null, "", "");
//        }

//        public DataTable GetHisRegByEnt(string lsEntidad, string lsMaestro, string lsWhere)
//        {
//            return GetHisRegByEnt(lsEntidad, lsMaestro, null, lsWhere, "");
//        }

//        public DataTable GetHisRegByEnt(string lsEntidad, string lsMaestro, string lsWhere, string lsOrder)
//        {
//            return GetHisRegByEnt(lsEntidad, lsMaestro, null, lsWhere, lsOrder);
//        }

//        public DataTable GetHisRegByEnt(string lsEntidad, string lsMaestro, string[] lsCampos)
//        {
//            return GetHisRegByEnt(lsEntidad, lsMaestro, lsCampos, "", "");
//        }

//        public DataTable GetHisRegByEnt(string lsEntidad, string lsMaestro, string[] lsCampos, string lsWhere)
//        {
//            return GetHisRegByEnt(lsEntidad, lsMaestro, lsCampos, lsWhere, "");
//        }

//        public DataTable GetHisRegByEnt(string lsEntidad, string lsMaestro, string[] lsCampos, string lsWhere, string lsOrder)
//        {
//            DataTable ldt = null;
//            Hashtable lhtCamposTodos = null;

//            lhtCamposTodos = CamposHis(lsEntidad, lsMaestro);
//            ldt = SiFElDB.DSODataAccess.Execute(GetQuery(lhtCamposTodos, lsCampos, lsWhere, lsOrder));

//            return ldt;
//        }
//        #endregion

//        #region GetRelRegByEnt
//        public DataTable GetRelRegByEnt(string lsRelacion, string lsEntidadBuscada)
//        {
//            return null;
//        }

//        public DataTable GetRelRegByEnt(string lsRelacion, string lsEntidadBuscada, string lsWhere)
//        {
//            return null;
//        }

//        public DataTable GetRelRegByEnt(string lsRelacion, string lsEntidadBuscada, string lsWhere, Hashtable lhtEntidadesDisp)
//        {
//            return null;
//        }
//        #endregion

//        public DataTable GetCatRegByEnt(string lsEntidad)
//        {
//            DataTable ldt = null;

//            ldt = SiFElDB.DSODataAccess.Execute(
//                        "select	cat.*" + "\r\n" +
//                        "from   catalogos ent" + "\r\n" +
//                        "       inner join catalogos cat" + "\r\n" +
//                        "           on cat.iCodCatalogo = ent.iCodRegistro" + "\r\n" +
//                        "where  ent.vchCodigo = '" + lsEntidad + "'" + "\r\n");

//            return ldt;
//        }

//        public DataTable GetMaeRegByEnt(string lsEntidad)
//        {
//            DataTable ldt = null;

//            ldt = SiFElDB.DSODataAccess.Execute(
//                        "select	mae.*" + "\r\n" +
//                        "from   catalogos ent" + "\r\n" +
//                        "       inner join maestros mae" + "\r\n" +
//                        "           on mae. = ent.iCodRegistro" + "\r\n" +
//                        "where  ent.vchCodigo = '" + lsEntidad + "'" + "\r\n");

//            return ldt;
//        }

//        public DataTable ExecuteQuery(string lsEntidad, string lsMaestro, string lsQuery)
//        {
//            DataTable ldt = null;
//            Hashtable lhtCamposTodos = null;

//            lhtCamposTodos = CamposHis(lsEntidad, lsMaestro);
//            ldt = SiFElDB.DSODataAccess.Execute(CamposHisParse(lhtCamposTodos, lsQuery));

//            return ldt;
//        }

//        public Object ExecuteScalar(string lsEntidad, string lsMaestro, string lsQuery)
//        {
//            DataTable ldt = null;
//            Object loRet = null;

//            ldt = ExecuteQuery(lsEntidad, lsMaestro, lsQuery);

//            if (ldt != null && ldt.Rows.Count > 0)
//                loRet = ldt.Rows[0][0];

//            return loRet;
//        }

//        public void Insert(string lsTabla, string lsEntidad, string lsMaestro, Hashtable lhtValores)
//        {
//            Hashtable lhtCamposTodos = null;
//            Hashtable lhtCampos = null;

//            StringBuilder lsbCampos = null;
//            StringBuilder lsbValores = null;

//            lhtCamposTodos = CamposHis(lsEntidad, lsMaestro);

//            if (lhtCamposTodos != null)
//            {
//                foreach (string key in lhtCamposTodos.Keys)
//                {
//                    if (key.StartsWith(lsMaestro + ":"))
//                        lhtCampos = (Hashtable)lhtCamposTodos[key];
//                }
//            }

//            if (lhtCampos != null)
//            {
//                lsbCampos = new StringBuilder();
//                lsbValores = new StringBuilder();

//                foreach (string lsCampo in lhtCampos.Keys)
//                {
//                    if (lhtValores.ContainsKey("{" + lsCampo + "}"))
//                    {
//                        if (lsbCampos.Length > 0)
//                        {
//                            lsbCampos.Append(", ");
//                            lsbValores.Append(", ");
//                        }

//                        lsbCampos.Append(lhtCampos[lsCampo]);
//                        lsbValores.Append(lhtValores["{" + lsCampo + "}"]);
//                    }
//                }

//                SiFElDB.DSODataAccess.ExecuteScalar (
//                    "declare @iCodRegistro\r\n" +
//                    "set @iCodRegistro = (select isnull(max(iCodRegistro), 0) + 1 from " + lsTabla + ")\r\n" +
//                    "insert into " + lsTabla + "\r\n" +
//                    "   (iCodRegistro, " + lsbCampos.ToString() + ")" + "\r\n" +
//                    "values" + "\r\n" +
//                    "   (@iCodRegistro, " + lsbValores.ToString() + ")" + "\r\n" + 
//                    "select @iCodRegistro" + "\r\n");
//            }
//        }
        
//        public void Update(string lsTabla, string lsEntidad, string lsMaestro, Hashtable lhtValores, int liCodRegistro)
//        {
//            Hashtable lhtCamposTodos = null;
//            Hashtable lhtCampos = null;
//            StringBuilder lsbValores = null;

//            lhtCamposTodos = CamposHis(lsEntidad, lsMaestro);

//            if (lhtCamposTodos != null)
//            {
//                foreach (string key in lhtCamposTodos.Keys)
//                {
//                    if (key.StartsWith(lsMaestro + ":"))
//                        lhtCampos = (Hashtable)lhtCamposTodos[key];
//                }
//            }

//            if (lhtCampos != null)
//            {
//                lsbValores = new StringBuilder();

//                foreach (string lsCampo in lhtCampos.Keys)
//                {
//                    if (lhtValores.ContainsKey("{" + lsCampo + "}"))
//                    {
//                        if (lsbValores.Length > 0)
//                            lsbValores.Append(", ");

//                        lsbValores.Append(lhtCampos[lsCampo] + " = " + lhtValores["{" + lsCampo + "}"]);
//                    }
//                }

//                SiFElDB.DSODataAccess.ExecuteNonQuery(
//                    "update " + lsTabla + "\r\n" +
//                    "set    " + lsbValores.ToString() + "\r\n" +
//                    "where  iCodRegistro = " + liCodRegistro + "\r\n");
//            }
//        }
        
//        private string GetQuery(Hashtable lhtCamposTodos, string[] lsCampos, string lsWhere, string lsOrder)
//        {
//            Hashtable lhtCampos = null;
//            Hashtable lhtSelect = null;
//            Hashtable lhtWhere = null;

//            StringBuilder lsbSelect = null;
//            StringBuilder lsbWhere = null;
//            StringBuilder lsbOrder = null;
//            StringBuilder lsbQuery = null;

//            lsbQuery = new StringBuilder();
//            lsbOrder = new StringBuilder(lsOrder);

//            //si hay {atributos} para reemplazar en el query a armar
//            if (lhtCamposTodos != null)
//            {
//                lhtSelect = new Hashtable();
//                lhtWhere = new Hashtable();

//                //inicializa los select y where para cada maestro
//                foreach (string kMae in lhtCamposTodos.Keys)
//                {
//                    if (kMae != "Todos")
//                    {
//                        lhtSelect.Add(kMae, new StringBuilder());
//                        lhtWhere.Add(kMae, new StringBuilder(lsWhere));
//                    }
//                }

//                foreach (string kCampo in ((Hashtable)lhtCamposTodos["Todos"]).Keys)
//                {
//                    lsbOrder.Replace("{" + kCampo + "}", "[{" + kCampo + "}]");

//                    foreach (string kMae in lhtCamposTodos.Keys)
//                    {
//                        if (kMae == "Todos")
//                            continue;

//                        lsbSelect = (StringBuilder)lhtSelect[kMae];
//                        lsbWhere = (StringBuilder)lhtWhere[kMae];

//                        lhtCampos = (Hashtable)lhtCamposTodos[kMae];

//                        //Select
//                        //arma el select. si el atributo se encuentra en el maestro, lo mapea. si no, le asigna null
//                        if (lsCampos == null || lsCampos.Length == 0 || ExisteEnArray(lsCampos, kCampo))
//                        {
//                            if (lsbSelect.Length > 0)
//                                lsbSelect.Append(",");

//                            lsbSelect.Append("[{" + kCampo + "}] = ");

//                            if (lhtCampos.ContainsKey(kCampo))
//                                lsbSelect.Append("his." + lhtCampos[kCampo]);
//                            else
//                                lsbSelect.Append("null");
//                        }

//                        //Where
//                        lhtWhere[kMae] = new StringBuilder(CamposHisParse(lhtCampos, lsbWhere.ToString()));
//                    }
//                }

//                //arma el query para cada maestro y los junta por un UNION
//                foreach (string kMae in lhtCamposTodos.Keys)
//                {
//                    if (kMae == "Todos")
//                        continue;

//                    if (lsbQuery.Length > 0)
//                        lsbQuery.Append("union" + "\r\n");

//                    lsbSelect = (StringBuilder)lhtSelect[kMae];
//                    lsbWhere = (StringBuilder)lhtWhere[kMae];

//                    lsbQuery.Append("select" + "\r\n");
//                    if (lsCampos == null || lsCampos.Length == 0)
//                        lsbQuery.Append("       his.*," + "\r\n");

//                    lsbQuery.Append("       " + lsbSelect.ToString() + "\r\n");
//                    lsbQuery.Append("from   historicos his" + "\r\n");
//                    lsbQuery.Append("where  his.iCodCatalogo = " + kMae.Split(':')[1] + "\r\n");
//                    lsbQuery.Append("       and his.iCodMaestro = " + kMae.Split(':')[2] + "\r\n");
//                    lsbQuery.Append("       and '" + pdtFechaVigencia.ToString("yyyy-MM-dd") + "' between his.dtIniVigencia and his.dtFinVigencia" + "\r\n");

//                    if (lsbWhere.Length > 0)
//                        lsbQuery.Append("       and " + lsbWhere.ToString() + "\r\n");
//                }

//                if (lsbOrder.Length > 0)
//                    lsbQuery.Append("order by " + lsbOrder.ToString() + "\r\n");
//            }
//            else
//            {
//                //si no hay {atributos} a reemplazar
//                lsbQuery.Append(
//                    "select his.*" + "\r\n" +
//                    "from   historicos his" + "\r\n" +
//                    "where  '" + pdtFechaVigencia.ToString("yyyy-MM-dd") + "' between his.dtIniVigencia and his.dtFinVigencia" + "\r\n");

//                if (lsWhere.Length > 0)
//                    lsbQuery.Append("       and " + lsWhere.ToString() + "\r\n");

//                if (lsOrder.Length > 0)
//                    lsbQuery.Append("order by " + lsOrder.ToString() + "\r\n");
//            }

//            return lsbQuery.ToString();
//        }

//        #region CamposHis
//        private Hashtable CamposHis(string lsEntidad, string lsMaestro)
//        {
//            StringBuilder lsbQuery = null;
//            DataTable ldtMaestros = null;

//            lsbQuery = new StringBuilder();
//            lsbQuery.Append(
//                "select mae.*" + "\r\n" +
//                "from   catalogos ent" + "\r\n" +
//                "       inner join maestros mae" + "\r\n" +
//                "           on mae.iCodEntidad = ent.iCodRegistro" + "\r\n");

//            if (lsMaestro.Length > 0)
//                lsbQuery.Append("           and mae.vchDescripcion = '" + lsMaestro + "'" + "\r\n");

//            if (lsEntidad.Length > 0)
//                lsbQuery.Append("where  ent.vchCodigo = '" + lsEntidad + "'" + "\r\n");

//            ldtMaestros = SiFElDB.DSODataAccess.Execute(lsbQuery.ToString());
//            return CamposHis(ldtMaestros);
//        }

//        private Hashtable CamposHis(DataTable ldtMaestros)
//        {
//            Hashtable lhtMaestros = null;
//            Hashtable lhtCampos = null;
//            Hashtable lhtCamposTodos = null;

//            if (ldtMaestros != null)
//            {
//                lhtMaestros = new Hashtable();
//                lhtCamposTodos = new Hashtable();

//                foreach (DataRow dr in ldtMaestros.Rows)
//                {
//                    lhtCampos = CamposHisMae(dr);

//                    lhtMaestros.Add(dr["vchDescripcion"] + ":" + dr["iCodEntidad"] + ":" + dr["iCodRegistro"], lhtCampos);

//                    foreach (string k in lhtCampos.Keys)
//                    {
//                        if (lhtCamposTodos.ContainsKey(k))
//                            lhtCamposTodos[k] = lhtCamposTodos[k] +
//                                "|" + dr["vchDescripcion"] + ":" + dr["iCodEntidad"] + ":" + dr["iCodRegistro"];
//                        else
//                            lhtCamposTodos.Add(k,
//                                dr["vchDescripcion"] + ":" + dr["iCodEntidad"] + ":" + dr["iCodRegistro"]);
//                    }
//                }

//                lhtMaestros.Add("Todos", lhtCamposTodos);
//            }

//            return lhtMaestros;
//        }

//        private Hashtable CamposHisMae(DataRow ldrMae)
//        {
//            Hashtable lhtCampos = null;
//            DataTable ldtMae = null;
//            DataTable ldtCampos = null;
//            StringBuilder lsAttSql = null;

//            if (ldrMae != null)
//            {
//                ldtMae = ldrMae.Table;
//                lsAttSql = new StringBuilder();

//                //Arma el query para consultar el nombre del atributo correspondiente al campo
//                foreach (DataColumn dc in ldtMae.Columns)
//                {
//                    if (ldrMae[dc.ColumnName] != System.DBNull.Value &&
//                    (dc.ColumnName.StartsWith("Integer") ||
//                        dc.ColumnName.StartsWith("Float") ||
//                        dc.ColumnName.StartsWith("Date") ||
//                        dc.ColumnName.StartsWith("VarChar")))
//                    {
//                        if (lsAttSql.Length > 0)
//                            lsAttSql.Append(", ");

//                        lsAttSql.Append(
//                            dc.ColumnName + " = ( " + "\r\n" +
//                            "select vchDescripcion" + "\r\n" +
//                            "from   catalogos" + "\r\n" +
//                            "where  iCodRegistro = " + ldrMae[dc.ColumnName] + ")" + "\r\n");
//                    }
//                }

//                if (lsAttSql.Length > 0)
//                {
//                    ldtCampos = SiFElDB.DSODataAccess.Execute("select " + lsAttSql.ToString());

//                    if (ldtCampos != null && ldtCampos.Rows.Count > 0)
//                    {
//                        lhtCampos = new Hashtable();

//                        foreach (DataColumn dc in ldtCampos.Columns)
//                            lhtCampos.Add(ldtCampos.Rows[0][dc.ColumnName], dc.ColumnName);
//                    }
//                }
//            }

//            return lhtCampos;
//        }

//        private string CamposHisParse(Hashtable lhtCamposMae, string lsCadena)
//        {
//            StringBuilder lsParsed = null;

//            lsParsed = new StringBuilder();
//            lsParsed.Append(lsCadena);

//            if (lhtCamposMae != null)
//            {
//                foreach (string key in lhtCamposMae.Keys)
//                    lsParsed.Replace("{" + key + "}", (string)lhtCamposMae[key]);
//            }

//            return lsParsed.ToString();
//        }
//        #endregion

//        #region Arrays
//        public static string ArrayToList(Array laArray)
//        {
//            return ArrayToList(laArray, ",");
//        }

//        public static string ArrayToList(Array laArray, string lsSeparador)
//        {
//            return ArrayToList(laArray, lsSeparador, "");
//        }

//        public static string ArrayToList(Array laArray, string lsSeparador, string lsDelimitador)
//        {
//            StringBuilder lsRet = null;

//            lsRet = new StringBuilder();

//            foreach(Object elem in laArray)
//            {
//                if (lsRet.Length != 0)
//                    lsRet.Append(lsSeparador);

//                lsRet.Append(lsDelimitador + elem.ToString() + lsDelimitador);
//            }

//            return lsRet.ToString();
//        }

//        public static bool ExisteEnArray(Array laArray, Object loElem)
//        {
//            bool lsRet = false;
//            Array laArray2;

//            laArray2 = (Array) laArray.Clone();
//            Array.Sort(laArray2);
//            lsRet = Array.BinarySearch(laArray2, loElem) >= 0;

//            return lsRet;
//        }
//        #endregion
//    }
//}
