<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepLlamNoContestadas.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.RepLlamNoContestadas" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="../InventarioTelecom/css/w2ui.css" rel="stylesheet" />
    <link href="../RepSevenEleven/CSS/StyleBody.css" rel="stylesheet" />
    <script type="text/javascript" src="../InventarioTelecom/js/jquery.js"></script>
    <script type="text/javascript" src="../InventarioTelecom/js/w2ui.js"></script>
    <script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
    <script type="text/javascript" src="../RepPentafon/JSPicker/jquery-1.12.4.js"></script>
    <script type="text/javascript" src="../RepPentafon/JSPicker/jquery-ui.js"></script>
    <script type="text/javascript" src="../RepPentafon/JSPicker/espanol.js"></script>

    <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;
        var dataDisp0 = [];
        var totalC = 0;

        function GetInfo(method) {
            $.ajax({
                url: pagePath + "/" + method,
                async: false,
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.d !== null) {
                        dataJSON = JSON.parse(result.d);
                    }
                    else { dataJSON = JSON.parse(errTxt); }
                },
                error: function (XMLHttpRequest, callStatus, errorThrown) {
                    dataJSON = JSON.parse(errTxt);
                }
            });
        }

        function RepLlamNoContestadas(method) {
            GetInfo(method);
            var obj = dataJSON;
            var dataDisp = obj.records;

            var cols = [
                { field: "recid", caption: 'Tipo destino', size: '150px', sortable: true, frozen: true },
                { field: "Fecha", caption: 'Fecha', size: '110px', sortable: true },
                { field: "Hora", caption: 'Hora', size: '100px', sortable: true },
                { field: "CallerID", caption: 'Caller ID', size: '90px', sortable: true },
                { field: "ExtOriginal", caption: 'Ext. Original', size: '90px', sortable: true },
                { field: "ExtDesvia", caption: 'Ext. Desvia', size: '90px', sortable: true },
                { field: "ExtFinal", caption: 'Ext. Final', size: '90px', sortable: true },
                { field: "Desconexion", caption: 'Desconexion', size: '350px', sortable: true },
                { field: "UltimoRedireccion", caption: 'Ultimo Redireccion', size: '300px', sortable: true },
                { field: "Duracion", caption: 'Duracion', size: '90px', sortable: true }
            ];

            $jQuery2_3('#repLlamadas').w2grid({
                name: 'repLlamadas',
                header: '',
                show: {
                    header: true,
                    toolbar: true,
                    lineNumbers: false,
                    footer: true
                },
                reorderColumns: true,
                searches: [
                    { type: 'text', field: 'recid', caption: 'Tipo destino', operator: 'contains' },
                    { type: 'text', field: 'Fecha', caption: 'Fecha', operator: 'contains' },
                    { type: 'text', field: 'Hora', caption: 'Hora', operator: 'contains' },
                    { type: 'text', field: 'CallerID', caption: 'Caller ID', operator: 'contains' },
                    { type: 'text', field: 'ExtOriginal', caption: 'Ext. Original', operator: 'contains' },
                    { type: 'text', field: 'ExtDesvia', caption: 'Ext. Desvia', operator: 'contains' },
                    { type: 'text', field: 'ExtFinal', caption: 'Ext. Final', operator: 'contains' },
                    { type: 'text', field: 'Desconexion', caption: 'Desconexion', operator: 'contains' }
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
                records: dataDisp
            });
            //w2ui['ConsumoLinea'].hideColumn('recid');
        }
    </script>
    <script type="text/javascript">
        var i = sessionStorage.getItem('FecIniC3');
        var f = sessionStorage.getItem('FecFinC3');
        var l = sessionStorage.getItem('FecFormatoIni3');
        var m = sessionStorage.getItem('FecFormatoFin3');
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

                        sessionStorage.setItem('FecIniC3', FecI);
                        var s = document.getElementById('from').value;
                        sessionStorage.setItem('FecFormatoIni3', s);
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
                        sessionStorage.setItem('FecFinC3', FecF);
                        document.getElementById("<%=finalFecha.ClientID%>").value = FecF;

                    var h = document.getElementById('to').value;
                    sessionStorage.setItem('FecFormatoFin3', h);
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
        var i = sessionStorage.getItem('FecIniC3');
        var f = sessionStorage.getItem('FecFinC3');
        var l = sessionStorage.getItem('FecFormatoIni3');
        var m = sessionStorage.getItem('FecFormatoFin3');
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
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
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
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
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
                                                <div class="col-xs-auto col-sm-auto col-md-auto col-lg-auto columna">
                                                    <div class="fechas-periodo pull-left">
                                                        <label class="col-sm-2 control-label1 applyDate1">Fecha Fin: </label>
                                                        <input type="text" id="to" name="to" class="form-control textDateRange" autocomplete="off">
                                                    </div>
                                                </div>
                                            </div>
                                            <input type="hidden" id="hfFechaInicio" />
                                            <input type="hidden" id="hfFechaFin1" />
                                            <asp:HiddenField runat="server" ID="inicioFecha" />
                                            <asp:HiddenField runat="server" ID="finalFecha" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="col-sm-4">
                                            </div>
                                            <div class="col-sm-6">
                                                <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                                    <div class="col-sm-6">
                                                        <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-lg" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-sm-8">
                                                            <div class="form-group">                                                                                                                            <asp:Label runat="server" CssClass="col-sm-6 control-label">DETALLE DE LLAMADAS: </asp:Label>
                                                            <asp:RadioButtonList runat="server" ID="rbtnList" RepeatDirection="Horizontal" AutoPostBack="true" 
                                                                OnSelectedIndexChanged="rbtnList_SelectedIndexChanged" RepeatColumns="2">
                                                                <asp:ListItem Selected="True" Value="1">&nbsp;No Contestadas&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                                                <asp:ListItem Value="0">&nbsp;Contestadas</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <div class="row">
                                                        <div class="col-sm-8">
                                                            <asp:Label runat="server" CssClass="col-sm-6 control-label">TIPO DE LLAMADAS: </asp:Label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnEnlace" GroupName="reportes" Checked="true" AutoPostBack="true" />Enlace
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnEntrada" GroupName="reportes" AutoPostBack="true" />
                                                                Entrada
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <asp:Panel runat="server" ID="line" Height="400px">
                                                        <div id="repLlamadas" style="height: 400px;"></div>
                                                    </asp:Panel>
                                                    <asp:Button ID="btnExportar" runat="server" CssClass="classButton" OnClick="btnExportar_Click" />
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
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnEnlace" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnEntrada" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnList" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
