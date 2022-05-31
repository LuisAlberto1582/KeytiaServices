
<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.Dashboard" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        #ctl00_cphTitle_pnlIndicadores{
            display:none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">

    <asp:Panel ID="pnlIndicadores" runat="server" CssClass="row">
    </asp:Panel>

    <div class="clearfix"></div>
    <asp:Panel ID="pnlDesvios" runat="server" CssClass="row" Visible="false">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                     <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <p style="margin-left:0px; font-weight: 500 !important; font-family: 'Poppins', sans-serif; font-size: 14pt;">Reportes de desvíos</p>
                    </div>
                    <div class="actions col-md-2 col-sm-2 col-lg-2 col-xs-2">
                        <p style="text-align:center;" runat="server" id="LlamPerd" visible="true">
                            <img src="../../img/svg/Asset 22.svg" alt="">
                            Exportar detalle:&nbsp;<asp:LinkButton ID="btnExortDetallP" runat="server" OnClick="btnExortDetallP_Click" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>                                  
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <asp:LinkButton ID="btnRegresar" runat="server" OnClick="btnRegresar_Click" CssClass="btn btn-default btn-circle btnBackDetail"><i class="far fa-arrow-alt-circle-left"></i></asp:LinkButton>
                        <asp:Panel ID="pnlMapaNavegacion" runat="server">
                        </asp:Panel>
                    </div>
                    <div class="actions col-md-2 col-sm-2 col-lg-2 col-xs-2">
                        <p style="text-align: center;">
                            <img src="../../img/svg/Asset 22.svg" alt="">
                            Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" OnClick="btnExportarXLS_Click" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                                                      
                        </p>
                        
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlHeaderContenido" runat="server" CssClass="row">
        <asp:Panel ID="pToolBar" runat="server">
        </asp:Panel>

        <asp:Panel ID="pnlLinkEtiquetacion" runat="server" Visible="false" CssClass="col-md-6 col-sm-6">
            <h4 style="text-align:right;">
                <asp:LinkButton ID="btnEtiquetacionEmple" runat="server" ForeColor="Red" OnClick="btnEtiquetacionEmple_Click" Visible="false" Text="">                                  
                </asp:LinkButton></h4>
            <h4>
                <asp:Label ID="lblSinEtiqPendiente" runat="server" ForeColor="Green" Text="Usted tiene todos los números etiquetados" Visible="false"></asp:Label></h4>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_1_2" runat="server" CssClass="row">
            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_3_4" runat="server" CssClass="row">
            <asp:Panel ID="Rep3" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep4" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_5_6" runat="server" CssClass="row">
            <asp:Panel ID="Rep5" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep6" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_7_8" runat="server" CssClass="row">
            <asp:Panel ID="Rep7" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep8" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_9_10" runat="server" CssClass="row">
            <asp:Panel ID="Rep9" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep10" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>
    </asp:Panel>

</asp:Content>
