<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="ConsumoGeneralPorSitio.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.ConsumoGeneralPorSitio" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalProgress {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
            filter: alpha(opacity=60);
            opacity: 0.95;
            -moz-opacity: 0.8;
        }

        .centerProgress {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
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

        function ActualizarRepSitio(iCodCatSitioNombre) {

            var UpdPrincipal = '<%=UpdPrincipal.ClientID%>';
            if (UpdPrincipal != null) {
                __doPostBack(UpdPrincipal, JSON.stringify({ iCodCatSitioNombre: iCodCatSitioNombre, iCodCatClaveCar: '', OnlyFilterClaveCar: '0' }));
            }
        }

        function updating(sender, args) {
            if (args.get_progressData() && args.get_progressData().OperationComplete == 'true')
                args.set_cancel(true);
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pToolBar" runat="server" CssClass="page-title-keytia">
        <div class="form-row">
            <asp:Label ID="lblCarrierConsumo" runat="server"></asp:Label>
            <div>
                <div class="col-md-1 col-sm-1">
                    <asp:Label ID="Label1" runat="server">Filtros:</asp:Label>
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:DropDownList ID="ddlEmpre" runat="server" CssClass="dropdown-keytia form-control"
                        AppendDataBoundItems="true">
                        <asp:ListItem Value="-1">TODAS</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:DropDownList ID="ddlCarrier" runat="server" CssClass="dropdown-keytia form-control">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2 col-sm-2">
                <asp:LinkButton ID="btnAplicar" runat="server" Text="Aplicar" OnClick="btnAplicar_Click" CssClass="btn btn-keytia-md"></asp:LinkButton>
            </div>
        </div>
    </asp:Panel>
    <br />
    <br />
    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlLeft" runat="server" CssClass="row">
            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-6 col-sm-6">
            </asp:Panel>

            <asp:UpdatePanel ID="UpdPrincipal" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6">
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="ActualizandoUpdPrincipal" AssociatedUpdatePanelID="UpdPrincipal">
                <ProgressTemplate>
                    <div class="modalProgress">
                        <div class="centerProgress">
                            <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" />
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </asp:Panel>
        <asp:Panel ID="pnlRight" runat="server" CssClass="row">
            <asp:UpdatePanel ID="UpdAux" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="Rep3" runat="server" CssClass="col-md-6 col-sm-6">
                    </asp:Panel>

                    <asp:Panel ID="Rep4" runat="server" CssClass="col-md-6 col-sm-6">
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <asp:Panel ID="pnlGeneral" runat="server" CssClass="row">
            <asp:Panel ID="Rep5" runat="server" CssClass="col-md-6 col-sm-6">
            </asp:Panel>

            <asp:Panel ID="Rep6" runat="server" CssClass="col-md-6 col-sm-6">
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
