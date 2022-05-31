<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepCabecerasPrefijos.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepPentafon.RepCabecerasPrefijos" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
<style type="text/css">
.modalUpload
{
           position: fixed;
    z-index: 999;
    height: 100%;
    width: 100%;
    top: 0;
}
.centerUpload
{
  z-index: 1000;
    margin: 300px auto;
    padding: 10px;
    width: 130px;
    padding-right:500px;

}
.center img
{
    height: 120px;
    width: 120px;
}
.txtDate1{
   float: left;
    margin-top: -10px;
    height: 45px;
}

.applyDate1 {
    background-color: #F58426;
    border: 1px solid #F58426;
    font-family: "Poppins",sans-serif;
    font-weight: 100;
    width: 60px;
    height: 45px;
    margin-top: 10px;
    color: white;
    font-size: 14px;
}
    .control-label1 {
        text-align: right;
        margin-bottom: 0;
        padding-top: 4px;
        padding-left: 9px;
    }
    </style>

    <link href="../InventarioTelecom/css/w2ui.css" rel="stylesheet" />
    <script type="text/javascript" src="../InventarioTelecom/js/jquery.js"></script>
    <script type="text/javascript" src="../InventarioTelecom/js/w2ui.js"></script>
    <script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
    <script type="text/javascript">
        var ed;

        var ed = sessionStorage.getItem("datom");
        
        if (ed === null) {
            ed = 0;
        }
            var pagePath = window.location.pathname;
            var dataJSON;
            var t;
            function GetInfo(method) {
                $.ajax({
                    url: pagePath + "/" + method,
                    data: "{ 'sitioClave':'" + ed + "' }",
                    async: false,
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {
                        if (result.d != null) {
                            dataJSON = JSON.parse(result.d);
                        }
                        else { dataJSON = JSON.parse(errTxt); }
                    },
                    error: function (XMLHttpRequest, callStatus, errorThrown) {
                        dataJSON = JSON.parse(errTxt);
                    }
                });
            }

            $jQuery2_3(document).ready(function () {
                $jQuery2_3('#prefijCabeceras').w2grid.autoLoad = true;
                $jQuery2_3('#prefijCabeceras').w2grid.skip=true;
            });

            function Cabeceras(method) {
                GetInfo(method);
                var obj = dataJSON;
                var dataDisp = obj.records;
                t = dataDisp;
                var cols = [
                //{ field: "recid", caption: '#', size: '80px', sortable: true, frozen: true },
                { field: "sitio", caption: 'Campaña', size: '190px', sortable: true, frozen: true },
                { field: "Carrier", caption: 'Carrier', size: '150px', sortable: true, frozen: true },
                { field: "Exten", caption: 'Prefijo', size: '140px', sortable: true, frozen: true },
                { field: "cabecera", caption: 'Cabecera', size: '180px', sortable: true },
                { field: "marcaciones", caption: 'Marcaciones', size: '150px', sortable: true },
                { field: "contestadas", caption: 'Contestadas', size: '150px', sortable: true },
                { field: "contacto", caption: '% Contacto', size: '90px', sortable: true }
                ];

                $jQuery2_3('#prefijCabeceras').w2grid({
                        name: 'prefijCabeceras',
                        header: 'Conteo de Llamadas por Cabecera',
                        show: {
                            header: true,
                            toolbar: true,
                            lineNumbers: false,
                            footer: true
                        },
                        reorderColumns: true,
                        searches: [
                        { type: 'text', field: 'Exten', caption: 'Prefijo', operator: 'contains' },
                        { type: 'text', field: 'Carrier', caption: 'Carrier', operator: 'contains' },
                        { type: 'text', field: 'cabecera', caption: 'Cabecera', operator: 'contains' },
                        { type: 'text', field: 'marcaciones', caption: 'Marcaciones', operator: 'contains' },
                        { type: 'text', field: 'contestadas', caption: 'Contestadas', operator: 'contains' },
                        { type: 'text', field: 'contacto', caption: '% Contacto', operator: 'contains' }
                        ],
                        toolbar:{
                            items: [
                                { type: 'spacer' },
                                { type: 'break' },
                                {
                                    type: 'button', id: 'item7', text: 'Exportar', img: 'icon-page', checked: true,
                                    onClick: function (event) {
                                        downloadFile();
                                    }
                                }
                              ]
                        },
                        columns: cols,
                        sortData: [{ field: 'Exten', direction: 'asc' }],
                        records: dataDisp
                });
            };
</script>
    <script type="text/javascript">
    function convertToCSV(objArray) {
        var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
        var str = '';

        for (var i = 0; i < array.length; i++) {
            var line = '';
            for (var index in array[i]) {
                if (line != '') line += ','

                line += array[i][index];
            }

            str += line + '\r\n';
        }

        return str;
    }

    function exportCSVFile(headers, items, fileTitle) {
        if (headers) {
            items.unshift(headers);
        }

        // Convert Object to JSON
        var jsonObject = JSON.stringify(items);

        var csv = this.convertToCSV(jsonObject);

        var exportedFilenmae = fileTitle + '.csv' || 'ReporteCabeceras.csv';

        var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });

        if (navigator.msSaveBlob) { // IE 10+
            navigator.msSaveBlob(blob, exportedFilenmae);
        } else {
            var link = document.createElement("a");
            if (link.download !== undefined) { // feature detection
                // Browsers that support HTML5 download attribute
                var url = URL.createObjectURL(blob);
                link.setAttribute("href", url);
                link.setAttribute("download", exportedFilenmae);
                link.style.visibility = 'hidden';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }
    }

    function downloadFile() {
        var headers = {
            sitio: 'Campaña',
            Carrier:'Carrier',
            Exten: 'Prefijo',
            cabecera: 'Cabecera',
            marcaciones: 'Marcaciones',
            contestadas: 'Contestadas',
            contacto: '% Contacto',
        };

        itemsNotFormatted = t;
        var itemsFormatted = [];

        // format the data
        itemsNotFormatted.forEach((item) => {
            itemsFormatted.push({
                sitio: item.sitio,
                Carrier: item.Carrier,
                Exten: item.Exten,
                cabecera: item.cabecera,
                marcaciones: item.marcaciones,
                contestadas: item.contestadas,
                contacto: item.contacto
            });
        });

        var combo = document.getElementById('<%=cboCampaña.ClientID%>');
        var selected = combo.options[combo.selectedIndex].text;
        var fileTitle = 'ReporteCabecera_' + selected; // or 'my-unique-title'
        if (itemsNotFormatted.length > 0)
        {
            exportCSVFile(headers, itemsFormatted, fileTitle); // call the exportCSVFile() function to process the JSON and trigger the download
        }        
    }
</script>
  <script type="text/javascript" src="JSPicker/jquery-1.12.4.js"></script>
  <script type="text/javascript" src="JSPicker/jquery-ui.js"></script>
  <script type="text/javascript" src="JSPicker/espanol.js"></script>
  <script type="text/javascript">
          $(document).ready(function () {
              var dateFormat = "dd/mm/yy",
              from = $("#from")
                .datepicker({
                    changeMonth: true,
                    changeYear:true,
                    numberOfMonths: 1,
                    altField: "#hfFechaInicio",
                    altFormat: "yy-mm-dd 00:00:00",
                    yearRange:"-1y:+1y"
                })
                .on("change", function () {
                    to.datepicker("option", "minDate", getDate(this));
                    $("#from").datepicker("option", "dateFormat", 'DD, d MM, yy');
                    var FecI = document.getElementById('hfFechaInicio').value;
                    document.getElementById("<%=inicioFecha.ClientID%>").value = FecI;

                    sessionStorage.setItem('FecIniC', FecI);
                    var s = document.getElementById('from').value;
                    sessionStorage.setItem('FecFormatoIni',s);
                }),

              to = $("#to").datepicker({
                  changeMonth: true,
                  changeYear:true,
                  numberOfMonths: 1,
                  altField: "#hfFechaFin1",
                  altFormat: "yy-mm-dd 23:59:59",
                  yearRange: "-1y:+1y"
              })
              .on("change", function () {
                  from.datepicker("option", "maxDate", getDate(this));
                  $("#to").datepicker("option", "dateFormat", 'DD, d MM, yy');
                  $("#from").datepicker({ minDate:to});

                  var FecF = document.getElementById('hfFechaFin1').value;
                  sessionStorage.setItem('FecFinC', FecF);
                  document.getElementById("<%=finalFecha.ClientID%>").value = FecF;

                  var h = document.getElementById('to').value;
                  sessionStorage.setItem('FecFormatoFin', h);
              });

              function getDate(element) {
                  var date;
                  try {
                      date = $.datepicker.parseDate(dateFormat, element.value);
                  } catch (error) {
                      date = null;
                  }
                  return date;
              }

          });
          
  </script>
  <script type="text/javascript">
        var i = sessionStorage.getItem('FecIniC');
        var f = sessionStorage.getItem('FecFinC');
        var l = sessionStorage.getItem('FecFormatoIni');
        var m = sessionStorage.getItem('FecFormatoFin');
        if (f != null && i != null) {
            $(document).ready(function () {
                document.getElementById('from').value = l;
                $("#hfFechaInicio").text(i);
                document.getElementById('to').value = m;
                $("#hfFechaFin1").text(f);
                document.getElementById("<%=inicioFecha.ClientID%>").value = i;
                document.getElementById("<%=finalFecha.ClientID%>").value = f;
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
EnableScriptGlobalization="true">
</asp:ToolkitScriptManager>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
<ProgressTemplate>
    <div class="modalUpload">
        <div class="centerUpload">
             <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
        </div>
    </div>
</ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<div ID="pnlMainHolder" runat="server">
    <div ID="pnlRow_0" runat="server" CssClass="row">
        <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Conteo de Llamadas por Cabecera</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body" id="divContenedor">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" >
                       <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-6">
                                   <div class="col-xs-auto col-sm-auto col-md-auto col-lg-auto columna">
                                       <div class="fechas-periodo pull-left">
                                          <label class="col-sm-2 control-label1 applyDate1">Fecha Inicio: </label> 
                                          <input type="text" id="from" name="from" class="form-control textDateRange" autocomplete="off">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="col-xs-auto col-sm-auto col-md-auto col-lg-auto columna"">
                                        <div class="fechas-periodo pull-left">
                                            <label class="col-sm-2 control-label1 applyDate1">Fecha Fin: </label>
                                            <input type="text" id="to" name="to" class="form-control textDateRange" autocomplete="off">
                                        </div>                                
                                    </div>
                                </div>
                                <input type="hidden" id="hfFechaInicio"/>
                                <input type="hidden"id="hfFechaFin1" />
                                <asp:HiddenField runat="server" ID="inicioFecha"/>
                                <asp:HiddenField  runat="server" ID="finalFecha"/>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-6">
                                    <asp:Panel runat="server" ID="row" CssClass="form-group">
                                        <asp:Label runat="server" CssClass="col-sm-2 control-label">Campaña</asp:Label>
                                           <div class="col-sm-8">
                                               <asp:DropDownList runat="server" ID="cboCampaña" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="vchDescripcion" CssClass="form-control" OnSelectedIndexChanged="cboCampaña_SelectedIndexChanged">
                                                   <asp:ListItem Value="0"> Selecciona Una Campaña </asp:ListItem>
                                               </asp:DropDownList>                                       
                                           </div>                                 
                                        <asp:HiddenField  runat="server" ID="claveSit"/>
                                    </asp:Panel>
                                </div>
                                <div class="col-sm-4">
                                   <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                        <div class="col-sm-4">                          
                                            <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-lg" Text="Buscar" OnClick="btnBuscar_Click"/>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="row">                           
                            <div class="col-sm-12" id="detalle">                                               
                                <div id="prefijCabeceras" style="height:430px;" ></div>
                            </div>
                            <%--<a class="btn btn-primary btn-sm" href='javascript:;' onclick="downloadFile();" role="button">Exportar</a>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</ContentTemplate>
<Triggers>
    <asp:PostBackTrigger ControlID="btnAplicarFecha" />
</Triggers>
</asp:UpdatePanel>
<script type="text/javascript">
        window.onsubmit = function () {          
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            
        }
</script>
</asp:Content>
