<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConsumoMovilRep.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoMovilRep" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .navegacionStyle {
            margin-top: 5px;
            font-size: 14px;
            font-weight: bold;
            display: inline-block;
        }

        .modalProgress {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 50%;
            left: 50%;
            filter: alpha(opacity=60);
            opacity: 0.95;
            -moz-opacity: 0.8;
        }

        input[type=text], select {
            border: 1px solid #7C7EB6;
            -moz-border-radius: 7px !important;
            -webkit-border-radius: 7px !important;
            border-radius: 7px;
            padding: 2px 0px;
            padding-left: 4px;
            outline: 0;
            -webkit-appearance: none;
        }

        .centerProgress {
            z-index: 1000;
            margin-left: -45px;
            margin-top: -45px;
            /*width: 40%;*/
            border-radius: 10px;
            filter: alpha(opacity=60);
            opacity: 1;
            -moz-opacity: 1;
        }

            .centerProgress img {
                height: 90px;
                width: 90px;
            }
    </style>
    <script type="text/javascript">

        function ActualizarRepFiltro() {

            var UpdPrincipal = '<%=UpdPrincipal.ClientID%>';

            //FILTROS
            var PeriodoInfo = $("#<%=pdtInicio.ClientID%>" + "_txt").val();
            var Carrier = $("#<%=cboCarrier.ClientID%>").val();

            var Linea = $("#<%=txtLineaId.ClientID%>").val();
            if ($("#<%=txtLinea.ClientID %>").val().length == 0) {
                Linea = "0";
            }

            var Emple = $("#<%=txtEmpleId.ClientID%>").val();
            if ($("#<%=txtNombre.ClientID %>").val().length == 0) {
                Emple = "0";
            }

            var CenCos = $("#<%=txtCenCosId.ClientID%>").val();
            if ($("#<%=txtCenCos.ClientID %>").val().length == 0) {
                CenCos = "0";
            }

            var Plan = $("#<%=txtPlanId.ClientID%>").val();
            if ($("#<%=txtPlan.ClientID %>").val().length == 0) {
                Plan = "0";
            }

            var FechaFinPlan = $("#<%=pdtFin.ClientID%>" + "_txt").val();

            if (UpdPrincipal != null) {
                __doPostBack(UpdPrincipal,
                    JSON.stringify(
                            {
                                PeriodoInfo: PeriodoInfo,
                                ICodCatCarrier: Carrier,
                                ICodCatLinea: Linea,
                                ICodCatEmple: Emple,
                                ICodCatCenCos: CenCos,
                                ICodCatPlan: Plan,
                                FechaFinPlan: FechaFinPlan,
                                ClaveNivelReporte: "ConsumoMovil",
                                TituloNav: "Consumo Móvil"
                            }));
            }
        }

        function updating(sender, args) {
            if (args.get_progressData() && args.get_progressData().OperationComplete == 'true')
                args.set_cancel(true);
        }

        function Navegar(titulo, nav, line, carr, cat) {

            var UpdPrincipal = '<%=UpdPrincipal.ClientID%>';
            var PeriodoInfo = $("#<%=pdtInicio.ClientID%>" + "_txt").val();

            if (UpdPrincipal != null) {
                __doPostBack(UpdPrincipal,
                    JSON.stringify(
                            {
                                PeriodoInfo: PeriodoInfo, ICodCatCarrier: carr, ICodCatLinea: line, ClaveNivelReporte: nav, Categoria: cat, TituloNav: titulo
                            }));
            }
        }


        <%-- function EndProgress() {
            var progress = $("#<%=ActualizandoUpdPrincipal.ClientID%>")
            progress._pageRequestManager.abortPostBack();
        }--%>
    </script>
    <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;

        $(function () {
            $("#" + "<%=txtNombre.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplateEmple",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d);
                            if (dataJSON.length == 0) {
                                $("#" + "<%=txtEmpleId.ClientID %>").val("0");
                            }
                            response($.map(dataJSON, function (item) {
                                return { label: item.Nomina + ' ' + item.Nombre, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) {
                            $("#" + "<%=txtEmpleId.ClientID %>").val("0");
                        }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#" + "<%=txtNombre.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtEmpleId.ClientID %>").val(ui.item.description);
                },
                change: function (event, ui) {
                    if (ui.item == null || $("#<%=txtNombre.ClientID %>").val().length == 0) {
                        $("#" + "<%=txtEmpleId.ClientID %>").val("0");
                    }
                }
            });
        });

        $(function () {
            $("#" + "<%=txtCenCos.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplateCenCos",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d);
                            if (dataJSON.length == 0) {
                                $("#" + "<%=txtCenCosId.ClientID %>").val("0");
                            }
                            response($.map(dataJSON, function (item) {
                                return { label: item.Clave + ' ' + item.Descripcion, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) {
                            $("#" + "<%=txtCenCosId.ClientID %>").val("0");
                        }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    $("#" + "<%=txtCenCos.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtCenCosId.ClientID %>").val(ui.item.description);
                },
                change: function (event, ui) {
                    if (ui.item == null || $("#<%=txtCenCos.ClientID %>").val().length == 0) {
                        $("#" + "<%=txtCenCosId.ClientID %>").val("0");
                    }
                }
            });
        });


        $(function () {
            $("#" + "<%=txtPlan.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplatePlan",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d);
                            if (dataJSON.length == 0) {
                                $("#" + "<%=txtPlanId.ClientID %>").val("0");
                            }
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) {
                            $("#" + "<%=txtPlanId.ClientID %>").val("0");
                        }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    $("#" + "<%=txtPlan.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtPlanId.ClientID %>").val(ui.item.description);
                },
                change: function (event, ui) {
                    if (ui.item == null || $("#<%=txtPlan.ClientID %>").val().length == 0) {
                        $("#" + "<%=txtPlanId.ClientID %>").val("0");
                    }
                }
            });
        });


        $(function () {
            $("#" + "<%=txtLinea.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplateLinea",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d);
                            if (dataJSON.length == 0) {
                                $("#" + "<%=txtLineaId.ClientID %>").val("0");
                            }
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) {
                            $("#" + "<%=txtLineaId.ClientID %>").val("0");
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    $("#" + "<%=txtLinea.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtLineaId.ClientID %>").val(ui.item.description);
                },
                change: function (event, ui) {
                    if (ui.item == null || $("#<%=txtLinea.ClientID %>").val().length == 0) {
                        $("#" + "<%=txtLineaId.ClientID %>").val("0");
                    }
                }
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="Panel2" runat="server" CssClass="page-title-keytia">
        <asp:Label ID="lblTitulo" runat="server">Consumo Móvil</asp:Label>
    </asp:Panel>
    <div>
        <asp:UpdatePanel ID="UpdAux" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlNavegacion" runat="server" Font-Bold="true">
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <br />
    <asp:Panel ID="pnlMainHolder" runat="server" CssClass="row">
        <asp:Panel ID="pnlLeft" runat="server" CssClass="col-md-3 col-sm-3">
            <asp:Panel ID="Rep1" runat="server">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Filtros</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblFechaInicio" runat="server" CssClass="control-label">Periodo:&nbsp&nbsp&nbsp&nbsp</asp:Label>
                                    <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                        ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                    </cc1:DSODateTimeBox>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblCarrier" runat="server" CssClass="control-label">Carrier:</asp:Label>
                                    <asp:DropDownList ID="cboCarrier" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblLinea" runat="server" CssClass="control-label">Linea:</asp:Label>
                                    <asp:TextBox ID="txtLinea" runat="server" CssClass="autosuggest placeholderstile form-control"
                                        onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Linea" />
                                    <div style="display: none">
                                        <asp:TextBox ID="txtLineaId" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblEmpleado" runat="server" CssClass="control-label">Empleado:</asp:Label>
                                    <asp:TextBox ID="txtNombre" runat="server" CssClass="autosuggest placeholderstile form-control"
                                        onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Empleado" />
                                    <div style="display: none">
                                        <asp:TextBox ID="txtEmpleId" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblCentroCostos" runat="server" CssClass="control-label">Centro de costos:</asp:Label>
                                    <asp:TextBox ID="txtCenCos" runat="server" CssClass="autosuggest placeholderstile form-control"
                                        onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Centro de Costos" />
                                    <div style="display: none">
                                        <asp:TextBox ID="txtCenCosId" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblPlan" runat="server" CssClass="control-label">Plan:</asp:Label>
                                    <asp:TextBox ID="txtPlan" runat="server" CssClass="autosuggest placeholderstile form-control"
                                        onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Plan Tarifario" />
                                    <div style="display: none">
                                        <asp:TextBox ID="txtPlanId" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblFechaFin" runat="server" CssClass="control-label">Fecha fin plan:&nbsp&nbsp&nbsp&nbsp</asp:Label>
                                    <cc1:DSODateTimeBox ID="pdtFin" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                        ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                    </cc1:DSODateTimeBox>
                                </div>
                            </div>
                            <div class="row">
                                <br />
                                <div class="col-md-12 col-sm-12">
                                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-md" OnClientClick="ActualizarRepFiltro();return false" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <br />
        </asp:Panel>
        <asp:Panel ID="pnlRight" runat="server" CssClass="col-md-9 col-sm-9">
            <asp:UpdatePanel ID="UpdPrincipal" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="Rep2" runat="server" CssClass="col-md-12 col-sm-12" Style="padding-left: 0px; padding-right: 0px;">
                        <asp:Panel ID="Rep2PanelLeft" runat="server" CssClass="col-md-6 col-sm-6">
                            <asp:Panel ID="Rep2_1" runat="server">
                            </asp:Panel>
                        </asp:Panel>

                        <asp:Panel ID="Rep2PanelRight" runat="server" CssClass="col-md-6 col-sm-6">
                            <asp:Panel ID="Rep2_2" runat="server">
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:Panel ID="Rep4" runat="server" CssClass="col-md-12 col-sm-12" Style="padding-left: 0px; padding-right: 0px;">
                        <asp:Panel ID="Rep42PanelLeft" runat="server">
                            <asp:Panel ID="Rep4_1" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>
                        </asp:Panel>

                        <asp:Panel ID="Rep42PanelRight" runat="server">
                            <asp:Panel ID="Rep4_2" runat="server" CssClass="col-md-6 col-sm-6">
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnExportarXLS" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="ActualizandoUpdPrincipal">
                <ProgressTemplate>
                    <div class="modalProgress">
                        <div class="centerProgress">
                            <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" />
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </asp:Panel>
    </asp:Panel>


    <!--Exportar a XLS-->
    <asp:ImageButton ID="btnExportarXLS" runat="server" ImageUrl="~/img/svg/Asset 26.svg" Width="30px" Height="48px" OnClick="btnExportarXLS_Click" />
    <asp:AlwaysVisibleControlExtender ID="avceXLSExport" runat="server" TargetControlID="btnExportarXLS"
        VerticalSide="Top" VerticalOffset="68" HorizontalSide="Right" HorizontalOffset="20" ScrollEffectDuration=".1" />
</asp:Content>
