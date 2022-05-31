<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DashboardRepCFiltros.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.DashboardRepCFiltros" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .leftPanel {
            margin: 20px;
            padding: 20px;
            width: 50%;
            float: left;
            height: 330px;
        }

        .rightPanel {
            margin: 20px;
            padding: 20px;
            width: 50%;
            float: right;
            height: 330px;
        }

        .format {
            padding: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <button id="btnRegresar" runat="server" onserverclick="btnRegresar_Click" type="button" class="btn btn-default btn-circle btnBackDetail"><i class="far fa-arrow-alt-circle-left"></i></button>
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
<%--<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Inicio</asp:Label>
                <asp:LinkButton ID="btnDescargarManual" runat="server" CssClass="linkButtonHelpStyle"
                    Text="Guía rápida de Usuario" Visible="false"></asp:LinkButton>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                <asp:Button ID="btnRegresar" runat="server" CssClass="buttonBack" Text="< Regresar" OnClick="btnRegresar_Click" />
                &nbsp;
                <asp:Button ID="btnExportarXLS" runat="server" Text="Exportar XLS" CssClass="buttonSave"  OnClick="btnExportarXLS_Click"  />
            </asp:Panel>
            <div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%" Style="min-height: 800px;">
        <asp:Panel ID="pnlDash" runat="server" CssClass="divToRightRepConsumoGlobalFiltro">
            <asp:Panel ID="pnlLeft" runat="server" CssClass="divToLeft">
                <asp:Panel ID="Seccion1Rep1" runat="server" CssClass="TopCenter divToCenter">
                    <asp:Panel ID="panelFiltrosRep1" CssClass="format" runat="server">
                        <asp:DropDownList ID="ddlTiposCampaniaRep1" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" runat="server">
                        </asp:DropDownList>
                        <asp:Button ID="btnAplicarFiltrosRep1" Text="Apicar" runat="server" OnClick="btnAplicarFiltrosRep1_Click" CssClass=" ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                    </asp:Panel>
                    <asp:Panel ID="Rep1" runat="server" CssClass="TopCenter divToCenter">
                    </asp:Panel>
                </asp:Panel>
                <br />
                <asp:Panel ID="Rep3" runat="server" CssClass="TopCenter divToCenter">
                </asp:Panel>
                <br />
                <asp:Panel ID="Rep5" runat="server" CssClass="TopCenter divToCenter">
                </asp:Panel>
                <br />
                <asp:Panel ID="Rep7" runat="server" CssClass="TopCenter divToCenter">
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="pnlRight" runat="server" CssClass="divToRight">
                <asp:Panel ID="Seccion1Rep2" runat="server" CssClass="TopCenter divToCenter">
                    <asp:Panel ID="panelFiltrosRep2" CssClass="format" runat="server">
                        <asp:DropDownList ID="ddlTiposCampaniaRep2" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" runat="server">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlcampanias" runat="server" CssClass=" ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary">
                        </asp:DropDownList>
                        <asp:Button ID="btnAplicarFiltrosRep2" Text="Aplicar" runat="server" OnClick="btnAplicarFiltorsRep2_Click" CssClass=" ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                    </asp:Panel>
                    <asp:Panel ID="Rep2" runat="server" CssClass="TopCenter divToCenter">
                    </asp:Panel>
                </asp:Panel>
                <br />
                <asp:Panel ID="Rep4" runat="server" CssClass="TopCenter divToCenter">
                </asp:Panel>
                <br />
                <asp:Panel ID="Rep6" runat="server" CssClass="TopCenter divToCenter">
                </asp:Panel>
                <br />
                <asp:Panel ID="Rep8" runat="server" CssClass="TopCenter divToCenter">
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Content>--%>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <%--<div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Inicio</asp:Label>
                <asp:LinkButton ID="btnDescargarManual" runat="server" CssClass="linkButtonHelpStyle"
                    Text="Guía rápida de Usuario" Visible="false"></asp:LinkButton>
            </div>
        </div>
        <div>
        </div>
          </div>
    --%>
    <%--    <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
        <asp:Button ID="btnRegresar" runat="server" CssClass="buttonBack" Text="< Regresar" OnClick="btnRegresar_Click" />
        &nbsp;
                <asp:Button ID="btnExportarXLS" runat="server" Text="Exportar XLS" CssClass="buttonSave" OnClick="btnExportarXLS_Click" />
    </asp:Panel>--%>
    <%--    <div>
    </div>--%>





    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_1_2" runat="server" CssClass="row">
            <asp:Panel ID="R1" runat="server" CssClass="col-md-6 col-sm-6">
                <asp:Panel ID="panelFiltrosRep1" CssClass="format" runat="server">
                    <asp:DropDownList ID="ddlTiposCampaniaRep1" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" runat="server">
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicarFiltrosRep1" Text="Aplicar" runat="server" OnClick="btnAplicarFiltrosRep1_Click" CssClass=" ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                </asp:Panel>
                <asp:Panel ID="Rep1" runat="server">
                </asp:Panel>
            </asp:Panel>



            <asp:Panel ID="R2" runat="server" CssClass="col-md-6 col-sm-6">
                <asp:Panel ID="panelFiltrosRep2" CssClass="format" runat="server">
                    <asp:DropDownList ID="ddlTiposCampaniaRep2" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" runat="server">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlcampanias" runat="server" CssClass=" ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary">
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicarFiltrosRep2" Text="Aplicar" runat="server" OnClick="btnAplicarFiltorsRep2_Click" CssClass=" ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                </asp:Panel>
                <asp:Panel ID="Rep2" runat="server">
                </asp:Panel>
            </asp:Panel>


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

        <asp:Panel ID="pnlRow_9" runat="server" CssClass="row">
            <asp:Panel ID="Rep9" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
            <asp:Panel ID="Rep10" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

    </asp:Panel>
</asp:Content>

