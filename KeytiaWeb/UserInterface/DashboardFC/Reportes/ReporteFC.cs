using System.Text;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Web.UI;
using KeytiaServiceBL;
using System.Collections.Generic;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public class ReporteFC : DashboardFC.Dashboard
    {

        #region Campos

        private string _repFechaInicio = string.Empty;
        private string _repFechaFin = string.Empty;
        private string _repRequestPath = string.Empty;

        private string _repTDest = string.Empty;
        private string _repCarrier = string.Empty;
        private string _repCenCos = string.Empty;
        private string _repNav = string.Empty;
        private string _repSitio = string.Empty;
        private string _repEmple = string.Empty;
        private string _repTipoLlam = string.Empty;
        private string _repMesAnio = string.Empty;
        private string _repNumMarc = string.Empty;
        private string _repExtension = string.Empty;
        private string _repCodAuto = string.Empty;
        private string _repConcepto = string.Empty;
        private string _repCampania = string.Empty;
        private string _repSitioDest = string.Empty;
        private string _repLocali = string.Empty;
        private string _repLinea = string.Empty;
        private string _repMiConsumo = string.Empty;


        //Genera un arreglo de strings con el formato que tendrá cada campo en el reporte web
        string[] formatoCampos;

        List<string> listaURLFields = new List<string>();
        List<int> listaIndicesCamposDeNavegacion = new List<int>();
        List<int> listaIndicesCamposBoundField = new List<int>();
        List<int> listaIndicesCamposHyperlinkField = new List<int>();


        string[] listadoURLFields;
        int[] indicesCamposDeNavegacion;
        int[] indicesCamposBoundField;
        int[] indicesCamposHyperlinkField;

        #endregion


        #region Propiedades

        protected string RepFechaInicio
        {
            get { return _repFechaInicio; }
            //set { _repFechaInicio = value; }
        }

        protected string RepFechaFin
        {
            get { return _repFechaFin; }
            //set { _repFechaFin = value; }
        }

        protected string RepRequestPath
        {
            get { return _repRequestPath; }
            //set { _repRequestPath = value; }
        }

        protected string RepNav
        {
            get { return _repNav; }
            //set { _repNav = value; }
        }

        protected string RepSitio
        {
            get { return _repSitio; }
            //set { _repSitio = value; }
        }


        protected string RepEmple
        {
            get { return _repEmple; }
            //set { _repEmple = value; }
        }

        protected string RepTipoLlam
        {
            get { return _repTipoLlam; }
            //set { _repTipoLlam = value; }
        }

        protected string RepMesAnio
        {
            get { return _repMesAnio; }
            //set { _repMesAnio = value; }
        }

        protected string RepNumMarc
        {
            get { return _repNumMarc; }
            //set { _repNumMarc = value; }
        }

        protected string RepExtension
        {
            get { return _repExtension; }
            //set { _repExtension = value; }
        }

        protected string RepCodAuto
        {
            get { return _repCodAuto; }
            //set { _repCodAuto = value; }
        }

        protected string RepConcepto
        {
            get { return _repConcepto; }
            //set { _repConcepto = value; }
        }

        protected string RepCampania
        {
            get { return _repCampania; }
            //set { _repCampania = value; }
        }

        protected string RepSitioDest
        {
            get { return _repSitioDest; }
            //set { _repSitioDest = value; }
        }

        protected string RepLocali
        {
            get { return _repLocali; }
            //set { _repLocali = value; }
        }

        protected string RepLinea
        {
            get { return _repLinea; }
            //set { _repLinea = value; }
        }

        protected string RepMiConsumo
        {
            get { return _repMiConsumo; }
            //set { _repMiConsumo = value; }
        }

        protected string RepTDest
        {
            get { return _repTDest; }
            //set { _repTDest = value; }
        }

        protected string RepCarrier
        {
            get { return _repCarrier; }
            //set { _repCarrier = value; }
        }

        protected string RepCenCos
        {
            get { return _repCenCos; }
            //set { _repCenCos = value; }
        }

        #endregion



        #region Constructores

        //Constructor base con parametros FechaInicio y FechaFin solamente
        public ReporteFC(string FechaInicio, string FechaFin)
        {
            _repFechaInicio = FechaInicio;
            _repFechaFin = FechaFin;
        }


        //Constructor base con parametros FechaInicio, FechaFin y RequestPath
        public ReporteFC(string FechaInicio, string FechaFin, string RequestPath)
        {
            _repFechaInicio = FechaInicio;
            _repFechaFin = FechaFin;
            _repRequestPath = RequestPath;
        }

        //Constructor parametros FechaInicio, FechaFin, RequestPath y Listado de parámetros que tiene el reporte
        public ReporteFC(string FechaInicio, string FechaFin, string RequestPath, Dictionary<string, string> listadoParametros) :
            this(FechaInicio, FechaFin, RequestPath)
        {

            //Recorre uno a uno cada parámetr recibido en el diccionario
            foreach (KeyValuePair<string, string> parametro in listadoParametros)
            {

                //Valida si la llave del elemento es igual al nombre del parámetro
                //de ser así asigna el valor a la variable que corresponda.
                switch (parametro.Key.ToLower())
                {
                    case "tdest":
                        _repTDest = parametro.Value;
                        break;
                    case "carrier":
                        _repCarrier = parametro.Value;
                        break;
                    case "cencos":
                        _repCenCos = parametro.Value;
                        break;
                    case "nav":
                        _repNav = parametro.Value;
                        break;
                    case "sitio":
                        _repSitio = parametro.Value;
                        break;
                    case "emple":
                        _repEmple = parametro.Value;
                        break;
                    case "tipollam":
                        _repTipoLlam = parametro.Value;
                        break;
                    case "mesanio":
                        _repMesAnio = parametro.Value;
                        break;
                    case "nummarc":
                        _repNumMarc = parametro.Value;
                        break;
                    case "extension":
                        _repExtension = parametro.Value;
                        break;
                    case "codauto":
                        _repCodAuto = parametro.Value;
                        break;
                    case "concepto":
                        _repConcepto = parametro.Value;
                        break;
                    case "campania":
                        _repCampania = parametro.Value;
                        break;
                    case "sitiodest":
                        _repSitioDest = parametro.Value;
                        break;
                    case "locali":
                        _repLocali = parametro.Value;
                        break;
                    case "linea":
                        _repLinea = parametro.Value;
                        break;
                    case "miconsumo":
                        _repMiConsumo = parametro.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion


        #region Metodos

        /// <summary>
        /// Obtiene la información necesaria para generar el reporte.
        /// </summary>
        /// <param name="queryString">Consulta a ejecutar</param>
        /// <param name="listadoCamposReporte">Listado de campos que se mostrarán en el reporte</param>
        /// <returns>DataTable con la información que aparecerá en el reporte</returns>
        public DataTable ObtieneDatosParaGrid(string queryString, List<CampoReporte> listadoCamposReporte)
        {
            //Obtiene el resultado de la consulta y llena un DataTable, 
            //este resultado puede obtener columnas que no se requieren en el reporte, 
            //por lo que más adelante se depura
            DataTable ldt = DSODataAccess.Execute(queryString);


            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                //Genera un arreglo de strings con los campos que se indiquen en el parámetro de listado de campos
                string[] camposReporte = new string[listadoCamposReporte.Count];
                int i = 0;
                foreach (CampoReporte campoRep in listadoCamposReporte)
                {
                    camposReporte[i] = campoRep.NombreEnConsulta;

                    i++;
                }




                //Del DataTable original, que contiene todas las columnas que regresa la consulta
                //solo deja aquellas que recibe como parámetro
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);


                //Reemplaza el nombre de las columnas del DataTable para que aparezcan de esta forma en el reporte
                i = 0;
                foreach (CampoReporte campoRep in listadoCamposReporte)
                {
                    ldt.Columns[i].ColumnName = campoRep.NombreEnWeb;

                    i++;
                }

                ldt.AcceptChanges();
            }

            return ldt;
        }


        /// <summary>
        /// Crea el reporte en el contenedor requerido, con los campos y parámetros indicados
        /// </summary>
        /// <param name="contenedorGrid"></param>
        /// <param name="ldtGrid"></param>
        /// <param name="idControl"></param>
        /// <param name="listadoCamposReporte"></param>
        /// <param name="tituloGrid"></param>
        /// <param name="URLSiguienteNivel"></param>
        public void GeneraReporteEnPagina(List<CampoReporte> listadoCamposReporte)
        {

            //Genera un arreglo de strings con el formato que tendrá cada campo en el reporte web
            formatoCampos = new string[listadoCamposReporte.Count];

            int i = 0;

            foreach (CampoReporte campoRep in listadoCamposReporte)
            {

                //Se llena el arreglo con la propiedad Formato del campo
                formatoCampos[i] = campoRep.Formato;

                //Valida si la propiedad EsURLField es verdadera, 
                //en caso de que así sea se agrega el campo al arreglo listadoURLFields
                if (campoRep.EsURLField)
                {
                    listaURLFields.Add(campoRep.NombreEnConsulta);
                }

                if (campoRep.EsDatoDeNavegacion)
                {
                    //Si la propiedad EsDatoDeNavegacion es verdadera, se agrega el índice
                    //del campo en el arreglo indicesCamposDeNavegacion

                    listaIndicesCamposDeNavegacion.Add(i);
                }

                if (campoRep.EsBoundField)
                {
                    //Si la propiedad EsCampoBoundField es verdadera, se agrega el índice
                    //del campo en el arreglo indicesCamposBoundField

                    listaIndicesCamposBoundField.Add(i);
                }

                if (campoRep.EsHyperlinkField)
                {
                    //Si la propiedad EsCampoHyperlinkField es verdadera, se agrega el índice
                    //del campo en el arreglo indicesCamposHyperlinkField

                    listaIndicesCamposHyperlinkField.Add(i);
                }

                i++;
            }


            //Se arman los arreglos que recibe el método DTIChartsAndControls.GridView
            listadoURLFields = new string[listadoCamposReporte.Count];
            indicesCamposDeNavegacion = new int[listaIndicesCamposDeNavegacion.Count];
            indicesCamposBoundField = new int[listaIndicesCamposBoundField.Count];
            indicesCamposHyperlinkField = new int[listaIndicesCamposHyperlinkField.Count];

            listadoURLFields = listaURLFields.ToArray();
            indicesCamposDeNavegacion = listaIndicesCamposDeNavegacion.ToArray();
            indicesCamposBoundField = listaIndicesCamposBoundField.ToArray();
            indicesCamposHyperlinkField = listaIndicesCamposHyperlinkField.ToArray();

        }

        public Control GeneraReporte1PnlEnPagina(DataTable ldtGrid, string idControl, List<CampoReporte> listadoCamposReporte, string tituloHeader, string tituloGrid, string URLSiguienteNivel, int indiceCampoParaTotales, string URLNavegacion)
        {
            GeneraReporteEnPagina(listadoCamposReporte);

            //Genera el control que contendrá el Grid
            Control controlGrid = DTIChartsAndControls.GridView(idControl, ldtGrid, true, "Totales", 
                       formatoCampos, URLSiguienteNivel, listadoURLFields, indiceCampoParaTotales, indicesCamposDeNavegacion, indicesCamposBoundField, indicesCamposHyperlinkField);

            //Crea el Grid
            return DTIChartsAndControls.tituloYBordesReporte(controlGrid, tituloHeader, tituloGrid, 0, URLNavegacion);
        }


        /// <summary>
        /// Genera un reporte de dos paneles con las características de estos (Grid y Gráfica)
        /// Este tipo de reportes se incluye en navegaciones de segundo nivel
        /// </summary>
        /// <param name="contenedorGrid"></param>
        /// <param name="ldtGrid"></param>
        /// <param name="idControl"></param>
        /// <param name="listadoCamposReporte"></param>
        /// <param name="tituloGrid"></param>
        /// <param name="URLSiguienteNivel"></param>
        /// <param name="indiceCampoParaTotales"></param>
        public void GeneraReporte2PnlsEnPagina(Control contenedorGrid, DataTable ldtGrid, string idControl, List<CampoReporte> listadoCamposReporte, string tituloGrid, string URLSiguienteNivel, int indiceCampoParaTotales)
        {


            GeneraReporteEnPagina(listadoCamposReporte);


            //Genera el control que contendrá el Grid
            Control controlGrid = DTIChartsAndControls.GridView(idControl, ldtGrid, true, "Totales", 
                       formatoCampos, URLSiguienteNivel, listadoURLFields, indiceCampoParaTotales, indicesCamposDeNavegacion, indicesCamposBoundField, indicesCamposHyperlinkField);

            //Crea el Grid
            contenedorGrid.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(controlGrid, tituloGrid, 0));
        }


        /// <summary>
        /// Elimina el campo "link" del listado de campos que recibe como parámetro. 
        /// Es utilizado generalmente para la exportación a Excel
        /// </summary>
        /// <param name="listado"></param>
        /// <param name="campoAQuitar">Nombre del campo que se desea omitir de la lista. Debe ingresarse en minusculas</param>
        /// <returns></returns>
        public List<CampoReporte> EliminaCampoDeLista(List<CampoReporte> listado, string campoAQuitar)
        {
            int idx = 0;
            int indiceCampoLink = 0;
            bool contieneCampoLink = false;


            //Busca el campo "link" en el listado de campos, si lo encuentra lo elimina de la lista
            foreach (CampoReporte cr in listado)
            {

                if (cr.ToString().ToLower() == campoAQuitar)
                {
                    indiceCampoLink = idx;
                    contieneCampoLink = true;
                    break;
                }

                idx++;
            }


            if (contieneCampoLink)
            {
                listado.RemoveAt(indiceCampoLink);
            }

            return listado;

        }



        /// <summary>
        /// Elimina el campo "link" del listado de campos que recibe como parámetro. 
        /// Es utilizado generalmente para la exportación a Excel
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        public List<CampoReporte> EliminaCampoDeLista(List<CampoReporte> listado, int indiceCampo)
        {


            listado.RemoveAt(indiceCampo);


            return listado;

        }
        #endregion
    }
}
