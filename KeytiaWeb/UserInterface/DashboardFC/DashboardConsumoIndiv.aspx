<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DashboardConsumoIndiv.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.DashboardConsumoIndiv" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UserInterface/DashboardFC/ConsumoIndividualUserControls/DetalleLlamadasMoviles.ascx" TagPrefix="uc1" TagName="WebUserControl1" %>
<%@ Register Src="~/UserInterface/DashboardFC/ConsumoIndividualUserControls/DetalleLlamadasFija.ascx" TagPrefix="uc1" TagName="DetalleLlamadasFija" %>
<%@ Register Src="~/UserInterface/DashboardFC/ConsumoIndividualUserControls/ConsumoDeDatos.ascx" TagPrefix="uc1" TagName="ConsumoDeDatos" %>
<%@ Register Src="~/UserInterface/DashboardFC/ConsumoIndividualUserControls/DesgloceConceptos.ascx" TagPrefix="uc1" TagName="DesgloceConceptos" %>




<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <link href="ConsumoIndividualUserControls/StyleSheet1.css" rel="stylesheet" />
    <script type="text/javascript" charset="utf8" src="https://code.jquery.com/jquery-3.5.1.js"></script>
    <script type="text/javascript" charset="utf8" src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/buttons/1.7.1/js/dataTables.buttons.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/buttons/1.7.1/js/buttons.html5.min.js"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/buttons/1.7.1/js/buttons.print.min.js"></script>
    <script src="https://cdn.datatables.net/plug-ins/1.10.20/api/sum().js"></script>


    <style>
        th {
            background-color: #191970 !important;
            color: white !important;
        }

        td {
            color: #000000
        }

        .dataTables_scrollFoot {
            background-color: #191970 !important;
        }
    </style>
    <script>
        function DesgloceLlamadasMovil(numlinea) {

            $('#tablemoviles').DataTable({
                dom: 'Bfrtip',

                buttons: [
                    'csv', 'excel'
                ],
                buttons: [
                    {
                        extend: 'excelHtml5',
                        customize: function (xlsx) {
                            $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '[$$-45C] #,##0.00_-');
                            var sheet = xlsx.xl.worksheets['sheet1.xml'];

                            $('row c[r^="B"]', sheet).attr('s', '63');
                        },
                        footer: true


                    },
                    {
                        extend: 'csv',

                        exportOptions: {
                            format: {
                                body: function (data, row, column, node) {
                                    // Strip $ from salary column to make it numeric
                                    return column === 5 ?
                                        data.replace(/[$,]/g, '') :
                                        data;
                                }
                            }
                        }

                    },
                ],
                //drawCallback: function () {
                //    var api = this.api();
                //    var suma = api.column(5, { page: 'current' }).data().sum();
                //    $(api.table().footer()).html(
                //        'Total: $' + suma
                //    );
                //},
                destroy: true,
                responsive: true,
                columnDefs: [{
                    "defaultContent": "-",
                    "targets": "_all"
                }],
                language: {
                    url: "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Spanish.json"
                },
                scrollY: "400px",
                scrollCollapse: true,
                pageLength: 50,

                ajax: {
                    method: "POST",
                    url: "DashboardConsumoIndiv.aspx/DetalleLLamadasMoviles",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: function (d) {
                        return JSON.stringify({ linea: numlinea })
                    },
                    dataSrc: "d.data"

                },
                autoWidth: true,
                columns: [
                    { data: "Fecha" },
                    { data: "Duración(seg)", render: $.fn.dataTable.render.number(',', '.', 0, '') },
                    { data: "NumMarcado" },
                    { data: "LugarLlamado" },
                    { data: "TipoLlamada" },
                    { data: "Importe", render: $.fn.dataTable.render.number(' ', '.', 0, '$') },
                ],
            });

            var table = $('#tablemoviles').DataTable();
            var suma = table.column(5).data().sum();
            $("#totalmovil").html(
                '$' + suma
            );
        }

        function DesgloceConceptos(numlinea, icodempleado, lineaDesc) {
            var texto = "<span class='red'>Hello <b>Again</b></span>";

            $.ajax({
                url: 'DashboardConsumoIndiv.aspx/RepDesgloceConceptos',
                dataType: 'json',
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify({ linea: numlinea, empleado: icodempleado }),
                processData: false,
                success: function (data) {

                    $("#contenido").html(data.d);
                },
                error: function (jqXhr, textStatus, errorThrown) {
                    console.log(errorThrown);
                }
            });

            $("#lineaDesgloce").html(lineaDesc);
        }


        function DesgloceLlamadasFija(empleado) {
            var table = $('#tablefija').DataTable({

                dom: 'Bfrtip',
                //buttons: [
                //    'csv', 'excel'
                //],
                buttons: [
                    {
                        extend: 'excel',
                        customize: function (xlsx) {
                            $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '[$$-45C] #,##0.00_-');
                            var sheet = xlsx.xl.worksheets['sheet1.xml'];

                            $('row c[r^="K"]', sheet).attr('s', '63');
                            $('row c[r^="L"]', sheet).attr('s', '63');
                        },
                        footer: true
                    },
                    {
                        extend: 'csv',
                        exportOptions: {
                            format: {
                                body: function (data, row, column, node) {
                                    // Strip $ from salary column to make it numeric
                                    return column === 11 ?
                                        data.replace(/[$,]/g, function (d) {
                                            return d;
                                        }) :
                                        data;
                                }
                            }
                        }
                    },
                ],
                destroy: true,
                responsive: false,
                //drawCallback: function (settings) {
                //    var api = this.api();
                //    var suma = api.column(12, { page: 'current' }).data().sum();
                //    $("#labelfija").html(
                //        'Total: $' + suma
                //    );

                //},  
                columnDefs: [
                    { "defaultContent": "-", "targets": "_all" }
                 
                ],
                //fixedColumns: true,
                pageLength: 50,
                language: {
                    url: "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Spanish.json"
                },
                scrollY: "320px",
                // scrollCollapse: true,
                // sScrollX: 100,
                scrollX: true,
                //scrollCollapse: true,
                //scrollY: true,
                ajax: {
                    method: "POST",
                    url: "DashboardConsumoIndiv.aspx/DetalleLLamadasFija",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: function (d) {
                        return JSON.stringify({ emple: empleado })
                    },

                    dataSrc: "d.data"

                },
                //autoWidth: false,
                columns: [
                    { data: "Centro de Costos" },
                    { data: "Colaborador" },
                    { data: "Nomina" },
                    { data: "Extension" },
                    { data: "CodAuto" },
                    { data: "Fecha" },
                    { data: "Numero Marcado" },
                    { data: "Hora" },
                    { data: "Fecha Fin" },
                    { data: "Hora Fin" },
                    { data: "Cantidad Minutos", render: $.fn.dataTable.render.number(',', '.', 0, '') },
                    { data: "Cantidad Llamadas", render: $.fn.dataTable.render.number(',', '.', 0, '') },
                    { data: "Total", render: $.fn.dataTable.render.number(' ', '.', 0, '$') },
                    { data: "Sitio" },
                    { data: "Carrier" },
                    { data: "Tipo de Llamada" },
                    { data: "Tipo Destino" }
                ],
            });
            //var table = $('#tablefija').DataTable();
            //var suma = table.column(12).data().sum();
            //$("#totalfija").html(
            //    '$' + suma
            //);
        }
        function DesgloceConsumoDeDatos(numlinea) {
            $('#tabledatos').DataTable({

                dom: 'Bfrtip',
                //buttons: [
                //    'csv', 'excel'
                //],
                buttons: [
                    {
                        extend: 'excel',
                        customize: function (xlsx) {
                            $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '[$$-45C] #,##0.00_-');
                        },
                    },
                    {
                        extend: 'csv',
                        exportOptions: {
                            format: {
                                body: function (data, row, column, node) {
                                    // Strip $ from salary column to make it numeric
                                    return column === 7 ?
                                        data.replace(/[$,]/g, '') :
                                        data;
                                }
                            }
                        }

                    },
                ],

                destroy: true,
                responsive: true,
                columnDefs: [{
                    "defaultContent": "-",
                    "targets": "_all"
                }
                ],
                language: {
                    url: "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Spanish.json"
                },
                scrollY: "400px",
                pageLength: 50,
                scrollCollapse: true,
                async: true,
                ajax: {
                    method: "POST",
                    url: "DashboardConsumoIndiv.aspx/RepConsumoDeDatos",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: function (d) {
                        return JSON.stringify({ linea: numlinea })
                    },
                    dataSrc: "d.data"

                },
                autoWidth: true,
                columns: [
                    { data: "Fecha" },
                    { data: "Hora" },
                    { data: "Tipo" },
                    { data: "Tipo Consumo" },
                    { data: "LocalidadDestino" },
                    { data: "Servicio" },
                    { data: "MBInternet", render: $.fn.dataTable.render.number(',', '.', 0, '') },
                    { data: "Importe", render: $.fn.dataTable.render.number(' ', '.', 0, '$') }
                ],
            });
        }

        $(document).ready(function () {
            // Handler for .ready() called.
            $("#lnkMoviles").click(function () {

                if ($('#Moviles').length) {
                    $('html, body').animate({
                        scrollTop: $('#Moviles').offset().top - 60
                    }, 'slow');
                }
                else {
                    alert("No cuenta con lineas móviles")
                }

            });

            $("#lnkExtensiones").click(function () {

                if ($('#Fija').length) {
                    $('html, body').animate({
                        scrollTop: $('#Fija').offset().top + 280
                    }, 'slow');
                }
                else {
                    alert("No cuenta con actividad de telefonía fija")
                }


            });

            $("#lnkClaves").click(function () {
                if ($('#Fija').length) {
                    $('html, body').animate({
                        scrollTop: $('#Fija').offset().top + 280
                    }, 'slow');
                }
                else {
                    alert("No cuenta con actividad de telefonía fija")
                }
            });

            $("#btnDesgloce").click(function (e) {
                let file = new Blob([$('#contenido').html()], { type: "application/vnd.ms-excel" });
                let url = URL.createObjectURL(file);
                let a = $("<a />", {
                    href: url,
                    download: "DesgloceConceptos.xls"
                }).appendTo("body").get(0).click();
                e.preventDefault();
            });
        });



    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">


    <div class="col-md-12 col-sm-12 rowback " style="margin-bottom: 20px; padding:0 10px 10px 10px; background-color:#0E2B63 !important">
        
           
            <div class="row text-center">
                <br />
                <div class="col-md-2 col-sm-2">
                    <h3 style="color:white">Bienvenido</h3>
                </div>
                <div class="col-md-10 col-sm-10">
                    <div class="col-md-4 col-sm-4">
                    <div class="col-md-12 col-sm-12" style="border: 1px solid white; margin: 0 2px; padding: 5px">
                        <asp:Label runat="server" ID="lblNombre" Style="font-size: 1em;color:white"></asp:Label>

                    </div>
                    <%--<span style="display: block;color:white"">Nombre</span>--%>
                </div>
                <div class="col-md-4 col-sm-4">
                    <div class="col-md-12 col-sm-12" style="border: 1px solid white; margin: 0 2px; padding: 5px">
                        <asp:Label runat="server" ID="lblCorreo" Style="font-size: 1em; color:white"></asp:Label>

                    </div>
                    <%--<span style="display: block; color:white">Correo</span>--%>
                </div>
                <div class="col-md-4 col-sm-4">
                    <div class="col-md-12 col-sm-12" style="border: 1px solid white; margin: 0 2px; padding: 5px">
                        <asp:Label runat="server" ID="lblPuesto" Style="font-size: 1em; color:white"></asp:Label>

                    </div>
                    <%--<span style="display: block; color:white">Puesto</span>--%>
                </div>
                </div>
                

            </div>
        <div class="col-sm-8" style="margin: 0 auto; float: none">
                <asp:Label style="color:white; text-align:center" runat="server" CssClass="col-sm-2 control-label">Seleccionar Periodo: </asp:Label>
                <div class="col-sm-4">
                    <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control" OnSelectedIndexChanged="cboMes_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-4">
                    <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion" OnSelectedIndexChanged="cboAnio_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-2">
                    <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-sm" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                </div>
            </div>
        </div>


  <%--  <asp:Panel ID="pnlIndicadores" runat="server" CssClass="row">
        <div class="col-sm-12 ">
            <div class="col-sm-8" style="margin: 0 auto; float: none">
                <asp:Label runat="server" CssClass="col-sm-2 control-label">Periodo: </asp:Label>
                <div class="col-sm-4">
                    <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control" OnSelectedIndexChanged="cboMes_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-4">
                    <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion" OnSelectedIndexChanged="cboAnio_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-2">
                    <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-sm" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                </div>
            </div>

        </div>
    </asp:Panel>  --%>
    

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" ClientIDMode="Static" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlMainHolder" ClientIDMode="Static" runat="server" style="margin-top: 20px !important">
      
        <asp:Panel ID="pnlRow_1_2" runat="server" CssClass="row">


            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-9 col-sm-9"></asp:Panel>
            <asp:Panel ID="Rep2" runat="server" CssClass="col-md-3 col-sm-3">
                <a runat="server" id="lnkMoviles" style="line-height: normal !important">
                    <div class=" column">

                        <div class="indicadores">
                            <asp:Label ID="lblMoviles" CssClass="MessageTitlesCards" runat="server">2</asp:Label>
                            <span><i class="fa fa-mobile fa-3x"></i></span>
                            <span style="display: block;">Lineas Móviles</span>
                        </div>
                    </div>
                </a>
                <a runat="server" id="lnkExtensiones" style="line-height: normal !important">
                    <div class="  column">
                        <div class="indicadores">
                            <asp:Label ID="lblExtensiones" CssClass="MessageTitlesCards" runat="server">5</asp:Label>
                            <span><i class=" fa fa-phone fa-3x"></i></span>
                            <asp:Label runat="server" ID="exten" Style="display: block; margin: 5px 0;">Extensiones</asp:Label>
                        </div>

                    </div>
                </a>
                <a runat="server" id="lnkClaves" style="line-height: normal !important">
                    <div class=" column">
                        <div class="indicadores">
                            <asp:Label ID="lblClavesFac" CssClass="MessageTitlesCards" runat="server">1</asp:Label>
                            <span><i class=" fa fa-lock fa-3x"></i></span>
                            <span style="display: block; margin: 5px 0;">Claves Fac</span>
                        </div>
                    </div>
                </a>
            </asp:Panel>
        </asp:Panel>

        <div class="col-md-12 col-sm-12">
            <uc1:ConsumoDeDatos runat="server" ID="ConsumoDeDatos" />
            <uc1:WebUserControl1 runat="server" />
            <uc1:DetalleLlamadasFija runat="server" ID="DetalleLlamadasFija" />
            <uc1:DesgloceConceptos runat="server" ID="DesgloceConceptos" />
        </div>
    </asp:Panel>
</asp:Content>

