<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepCencosJerOriginal.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepSevenEleven.RepCencosJerOriginal" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
<link href="../InventarioTelecom/css/w2ui.css" rel="stylesheet" />
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
.classButton{
    background-color:transparent;
    width:0px;
    height:0px;
    border:none;
}
.w2ui-grid .w2ui-grid-body div.w2ui-col-header {
    height: auto !important;
    width: 100%;
    overflow: hidden;
    padding-right: 10px !important;
    FONT-SIZE: 12px;
}
</style>
    <script type="text/javascript" src="../InventarioTelecom/js/jquery.js"></script>
    <script type="text/javascript" src="../InventarioTelecom/js/w2ui.js"></script>
    <script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
     <script type="text/javascript" src="../RepExcedentes/OrdenaDatos.js"></script>

   <script type="text/javascript">
       var pagePath = window.location.pathname;
       var dataJSON;
       var dataDisp1 = [];
       var total = 0;
       var totLlam = 0;
       var totMin = 0;
       var totSitios = 0;
       var totT = 0;

       function GetInfo(method) {
           $.ajax({
               url: pagePath + "/" + method,
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

       function ConsumoCencos(method) {
           GetInfo(method);
           var obj = dataJSON;
           var dataDisp = sortJSON(obj.records, 'TOTALREAL', 'desc');
           $jQuery2_3.each(dataDisp, function (i, item) {

                total += dataDisp[i].TOTALREAL;
                totLlam += dataDisp[i].LLAMADAS;
                totMin += dataDisp[i].MINUTOS;
               totSitios += dataDisp[i].TOTALSITIOS;
               totT += 1;
               var rf = { "recid": dataDisp[i].recid, "Plazas": dataDisp[i].Plazas, "TOTALREAL": dataDisp[i].TOTALREAL, "LLAMADAS": dataDisp[i].LLAMADAS, "MINUTOS": dataDisp[i].MINUTOS, "TOTALSITIOS": dataDisp[i].TOTALSITIOS };
                dataDisp1.push(rf);
                
            });
           var sb = { w2ui: { summary: true }, recid: "", Plazas: '<span style="float:right;font-weight:bold;font-size:14px;">Total:</span>', TOTALREAL: total, LLAMADAS: totLlam, MINUTOS: totMin, TOTALSITIOS: totSitios};
            dataDisp1.push(sb);
           var st = { w2ui: { summary: true }, recid: "S-2",Plazas: '<span style="float:right;font-weight:bold;font-size:13px;">Total: ' + totT + "</span>" };
           dataDisp1.push(st);
           var cols = [
               { field: "recid", caption: '#', size: '0px', sortable: true, frozen: true },
               { field: "Plazas", caption: 'Plazas', size: '250px', sortable: true, frozen: true },
               { field: "TOTALREAL", caption: 'Total', size: '140px', render: 'money', sortable: true, frozen: true },
               { field: "LLAMADAS", caption: 'Cantidad de Llamadas', size: '150px', render: 'float', sortable: true },
               { field: "MINUTOS", caption: 'Cantidad de Minutos', size: '140px', render: 'float', sortable: true },
               { field: "TOTALSITIOS", caption: 'Cantidad de Sitios', size: '140px', render: 'float', sortable: true }
           ];

           $jQuery2_3('#ConsumoCencos').w2grid({
               name: 'ConsumoCencos',
               header: 'CONSUMO POR PLAZAS',
               show: {
                   header: true,
                   toolbar: true,
                   lineNumbers: false,
                   footer: false
               },
               reorderColumns: true,
               searches: [
                   { type: 'text', field: 'Plazas', caption: 'Plazas', operator: 'contains' }
               ],
               toolbar: {
                   items: [
                       { type: 'spacer' },
                       { type: 'break' },
                       {
                           type: 'button', id: 'item7', text: 'Exportar', img: 'icon-page', checked: true,
                           onClick: function (event) {
                               downloadFile();
                           }
                       },
                       { type: 'spacer' },
                       { type: 'break' }
                   ]
               },
               columns: cols,
               records: dataDisp1
           });

           w2ui['ConsumoCencos'].hideColumn('recid');
           w2ui.ConsumoCencos.on('click', function (event) {
               var grid = this;
               event.onComplete = function () {
                   var record = this.get(event.recid);
                   var e = record.recid;
                   var p = record.Plazas;
                   if (e != null) {
                       sessionStorage.clear();
                       sessionStorage.setItem('recid', e);
                       sessionStorage.setItem('Plazas', p);
                       var ed = sessionStorage.getItem("recid");
                       if (ed != null) {
                           window.location.href = 'RepCencosJerN2.aspx';
                       }
                   }
               }
           });
       };
</script>

<script type="text/javascript" src="../RepPentafon/JSPicker/jquery-1.12.4.js"></script>
<script type="text/javascript" src="../RepPentafon/JSPicker/jquery-ui.js"></script>
<script type="text/javascript" src="../RepPentafon/JSPicker/espanol.js"></script>
  <script type="text/javascript">
        var i = sessionStorage.getItem('FecIniC2');
        var f = sessionStorage.getItem('FecFinC2');
        var l = sessionStorage.getItem('FecFormatoIni2');
        var m = sessionStorage.getItem('FecFormatoFin2');
        if (f != null && i != null) {
            $(document).ready(function () {
                document.getElementById('from').value = l;
                $("#hfFechaInicio").text(i);
                document.getElementById('to').value = m;
                $("#hfFechaFin").text(f);
                document.getElementById("<%=inicioFecha.ClientID%>").value = i;
                document.getElementById("<%=finalFecha.ClientID%>").value = f;
            });
        }
    </script>
  <script type="text/javascript">
      $(document).ready(function () {
          var dateFormat = "dd/mm/yy",
              from = $("#from")
                  .datepicker({
                      changeMonth: true,
                      changeYear: true,
                      numberOfMonths: 1,
                      altField: "#hfFechaInicio",
                      altFormat: "yy-mm-dd 00:00:00",
                      yearRange: "-1y:+1y"
                  })
                  .on("change", function () {
                      to.datepicker("option", "minDate", getDate(this));
                      $("#from").datepicker("option", "dateFormat", 'DD, d MM, yy');

                      var FecI = document.getElementById('hfFechaInicio').value;
                      document.getElementById("<%=inicioFecha.ClientID%>").value = FecI;

                      sessionStorage.setItem('FecIniC2', FecI);
                      var s = document.getElementById('from').value;
                      sessionStorage.setItem('FecFormatoIni2', s);
                  }),

              to = $("#to").datepicker({
                  changeMonth: true,
                  changeYear: true,
                  numberOfMonths: 1,
                  altField: "#hfFechaFin1",
                  altFormat: "yy-mm-dd 23:59:59",
                  yearRange: "-1y:+1y"
              })
                  .on("change", function () {
                      from.datepicker("option", "maxDate", getDate(this));
                      $("#to").datepicker("option", "dateFormat", 'DD, d MM, yy');
                      $("#from").datepicker({ minDate: to });

                      var FecF = document.getElementById('hfFechaFin1').value;
                      sessionStorage.setItem('FecFinC2', FecF);
                      document.getElementById("<%=finalFecha.ClientID%>").value = FecF;

                      var h = document.getElementById('to').value;
                      sessionStorage.setItem('FecFormatoFin2', h);
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
         function downloadFile() {
             var btn = '<%=btnExportar.ClientID%>';
             var obj = document.getElementById(btn);
             obj.click();
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
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server"></span>
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
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-6">
                                   <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                        <div class="col-sm-6">                          
                                            <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-lg" Text="Buscar" OnClick="btnAplicarFecha_Click"/>
                                        </div>
                                    </asp:Panel>
                                </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default">
                              <div class="panel-heading"><strong>Periodo de facturación, Plaza/Mercado: </strong><asp:Label runat="server" ID="lblFechaFact" Font-Bold="true"></asp:Label></div>
                              <div class="panel-body">                                                                               
                                <div id="ConsumoCencos" style="height:280px;" ></div>
                                    <asp:Button ID="btnExportar" runat="server" CssClass="classButton" OnClick="btnExportar_Click"/>                            
                              </div>
                            </div>
                        </div>
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
    <Triggers>
 <asp:PostBackTrigger ControlID="btnExportar" />
</Triggers>
</asp:UpdatePanel>
</asp:Content>
