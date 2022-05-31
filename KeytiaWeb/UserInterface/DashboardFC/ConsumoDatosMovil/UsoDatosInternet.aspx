<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="UsoDatosInternet.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoDatosMovil.ConsumoDatosNacional" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 9999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }
    </style>
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
        var pagePath = window.location.pathname;
        var dataJSON;
        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);
            // Place here the first init of the autocomplete
            InitAutoCompl();
        });

        function InitializeRequest(sender, args) {
        }

        function EndRequest(sender, args) {
            // after update occur on UpdatePanel re-init the Autocomplete
            InitAutoCompl();
        }

        function InitAutoCompl() {
            $(function () {
                var idText = document.getElementById('<%=txtLinea.ClientID%>');
                var grid = '<%=grvLineas.ClientID%>';

                var dato = document.getElementById('<%=txtLinea.ClientID%>').value;

                 $(idText).keyup(function () {
                     var val = $(this).val().toUpperCase();
                     $('#' + grid + ' > tbody > tr').each(function (index, element) {
                         if ($(this).text().toUpperCase().indexOf(val) < 0)
                             $(this).hide();
                         else
                             $(this).show();
                     });
                 });

                /*valida si la caja de texto tiene datos para poder filtrar en el grid despues del refresh del update panel*/
                if (dato != "") {
                    var val = dato.toUpperCase();
                    $('#' + grid + ' > tbody > tr').each(function (index, element) {
                        if ($(this).text().toUpperCase().indexOf(val) < 0)
                            $(this).hide();
                        else
                            $(this).show();
                    });
                }
             });
<%--            $("#" + "<%=txtLinea.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetLinea",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.idLinea };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    $("#" + "<%=txtLinea.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtLineaId.ClientID %>").val(ui.item.description);
                }
            });--%>
        };

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
                                    Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" CssClass="exportExcel" OnClick="btnExportarXLS_Click"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                                    &nbsp;&nbsp;&nbsp;
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="col-sm-12">
                                        <div class="col-sm-6">
                                            <asp:Panel ID="Panel1" runat="server" CssClass="form-group">
                                                <asp:Label ID="lblCencostos" runat="server" CssClass="col-sm-2 control-label">Línea: </asp:Label>
                                                <div class="col-sm-5">
                                                    <asp:TextBox ID="txtLinea" runat="server" CssClass="autosuggest placeholderstile form-control"
                                                        onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Línea" />
                                                    <div style="display: none">
                                                        <asp:TextBox ID="txtLineaId" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-sm-5">
                                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-keytia-sm" OnClick="btnBuscar_Click" Visible="false"/>
                                                    &nbsp;
                                                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-keytia-sm" OnClick="btnLimpiar_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:Panel runat="server" ID="row" CssClass="form-group">
                                                <asp:Label runat="server" CssClass="col-sm-2 control-label">Mostrar: </asp:Label>
                                                <div class="col-sm-5">
                                                    <asp:DropDownList runat="server" ID="cboTipo" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-sm-8">
                                                            <asp:Label runat="server" CssClass="col-sm-4 control-label">Consumo Internet: </asp:Label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtNcional" GroupName="reportes" Checked="true" AutoPostBack="true" OnCheckedChanged="rbtNcional_CheckedChanged" />Nacional
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnInternacional" GroupName="reportes" AutoPostBack="true" OnCheckedChanged="rbtnInternacional_CheckedChanged" />
                                                                Internacional
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div style="width: 800px; float: none; margin: 0 auto;">
                                                            <asp:Panel ID="pnlInfo" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                                                <strong>
                                                                    <asp:Label ID="lblMensajeInfo" runat="server"></asp:Label>
                                                                </strong>
                                                            </asp:Panel>
                                                        </div>
                                                        <asp:Panel ID="pnl1" runat="server" Visible="false" CssClass="col-md-12 col-sm-12">
                                                            <div class="table-fixed-nz">
                                                                <asp:GridView runat="server" ID="grvLineas"
                                                                    CssClass="fixed_header table table-bordered tableDashboard"
                                                                    AutoGenerateColumns="false" DataKeyNames="iCodCatalogo,Linea">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Linea">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="lnkVerConsumo" Text='<%# Eval("Linea") %>' RowIndex='<%# Container.DisplayIndex %>' OnClick="lnkVerConsumo_Click"></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                                                                        <asp:BoundField DataField="CenCos" HeaderText="CenCos" />
                                                                        <asp:BoundField DataField="GBIncluidos" HeaderText="GB Inlcuidos" />
                                                                        <asp:BoundField DataField="GBUtilizadosTotales" HeaderText="GB Utilizados Totales" />
                                                                        <asp:BoundField DataField="GBUtilizadosconCosto" HeaderText="GB Utilizados con Costo" />
                                                                        <asp:BoundField DataField="GBExcedido" HeaderText="GB Excedido" />
                                                                        <asp:BoundField DataField="ImporteTotalInternet" HeaderText="Importe Internet" />
                                                                        <asp:BoundField DataField="ImporteTotalFacturado" HeaderText="Importe Total Facturado" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-6 col-sm-6">
                                                            </asp:Panel>
                                                            <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6">
                                                            </asp:Panel>
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
                </div>
            </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtNcional" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnInternacional" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="cboTipo" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnBuscar" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnLimpiar" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportarXLS" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
