<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="LineasSinActividad.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoDatosMovil.LineasSinActividad" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        $(function () {
            var idText = document.getElementById('<%=txtBuscar.ClientID%>');
            $(idText).keyup(function () {
                var val = $(this).val().toUpperCase();
                $('#RepConsumoMovil_t > tbody > tr').each(function (index, element) {
                    if ($(this).text().toUpperCase().indexOf(val) < 0)
                        $(this).hide();
                    else
                        $(this).show();
                });
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="modalUpload">
                <div class="centerUpload">
                    <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
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
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12">
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="row" runat="server" id="rowBusqueda" visible="false">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                        <div class="col-offset-2 col-sm-4">
                                                            <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:Panel ID="Rep2" runat="server" CssClass="col-md-12 col-sm-12">
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
        </ContentTemplate>

        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportarXLS" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
