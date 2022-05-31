<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepNumsOrigenNoContestadas.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepLaureate.RepNumsOrigenNoContestadas" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            padding-right: 500px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }
    </style>
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
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

            <%--$(function () {
                var idText = document.getElementById('<%=txtBuscar.ClientID%>');
                var grid = '<%=grvLlamas.ClientID%>';
                //var grid = 'RepNumOrigen';
                $(idText).keyup(function () {
                    var val = $(this).val().toUpperCase();
                    $('#' + grid + ' > tbody > tr').each(function (index, element) {
                        if ($(this).text().toUpperCase().indexOf(val) < 0)
                            $(this).hide();
                        else
                            $(this).show();
                    });
                });
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
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Números origen en llamadas no contestadas</span>
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
                                            <div class="col-sm-8">
                                                <asp:Label runat="server" CssClass="col-sm-4 control-label">Organización: </asp:Label>
                                                <label class="radio-inline">
                                                    <asp:RadioButton runat="server" ID="rbtnUVM" GroupName="reportes" Checked="true" AutoPostBack="true" />
                                                    UVM
                                                </label>
                                                <label class="radio-inline">
                                                    <asp:RadioButton runat="server" ID="rbtnUNITEC" GroupName="reportes" AutoPostBack="true" />
                                                    UNITEC
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row" runat="server" id="pnlInfo">
                                        <div style="width: 800px; float: none; margin: 0 auto;">
                                            <asp:Panel CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                                <strong>
                                                    <asp:Label ID="lblMensajeInfo" runat="server"></asp:Label>
                                                </strong>
                                            </asp:Panel>
                                        </div>
                                    </div>
<%--                                    <div class="row">
                                        <div class="col-sm-8">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                <div class="col-offset-2 col-sm-4">
                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>--%>
                                    <div class="row" runat="server" id="pnlGrid">
                                        <div class="col-sm-12" runat="server" id="resumenGral">
<%--                                            <div class="table-fixed-nz">
                                                <asp:GridView runat="server" ID="grvLlamas"
                                                    CssClass="fixed_header table table-bordered tableDashboard"
                                                    AutoGenerateColumns="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="Sitio de la llamada" HeaderText="Sitio de la llamada" />
                                                        <asp:BoundField DataField="Número telefónico origen" HeaderText="Número telefónico origen" />
                                                        <asp:BoundField DataField="Cantidad de llamadas perdidas" HeaderText="Cantidad de llamadas perdidas" />
                                                        <asp:BoundField DataField="Cantidad de llamadas contestadas" HeaderText="Cantidad de llamadas contestadas" />
                                                        <asp:BoundField DataField="TotalLlamadas" HeaderText="Total Llamadas" />
                                                    </Columns>
                                                </asp:GridView>--%>
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
            <asp:PostBackTrigger ControlID="rbtnUNITEC" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnUVM" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportarXLS" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
