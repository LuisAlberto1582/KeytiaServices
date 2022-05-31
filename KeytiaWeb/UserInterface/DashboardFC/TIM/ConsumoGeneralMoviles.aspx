<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConsumoGeneralMoviles.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.ConsumoGeneralMoviles" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
        <asp:Panel ID="pToolBar" runat="server" CssClass="page-title-keytia">
        <div class="mt-radio-inline">
            <asp:Label ID="lblCarrierConsumo" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="ddlCarrier" runat="server" CssClass="selectpicker dropdown-keytia" AppendDataBoundItems="true">
                    <asp:ListItem Value="-1">TODOS</asp:ListItem>
                </asp:DropDownList>
            <asp:LinkButton ID="btnAplicar" runat="server" Text="Aplicar"  CssClass="btn btn-keytia-md"></asp:LinkButton>
            <!-- OnClick="btnAplicar_Click" -->
        </div>
    </asp:Panel>
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlMainHolder" runat="server" CssClass="row">
        <asp:Panel ID="pnlLeft" runat="server" CssClass="col-md-4 col-sm-4">

            <asp:Panel ID="Rep1" runat="server" >
                 <asp:TabContainer ID="TabContainerPrincipal3" runat="server" CssClass="MyTabStyle">
                    <asp:TabPanel ID="TabPanel3Uno" runat="server" HeaderText="Grafica" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="Panel2" runat="server">
                                <asp:Panel ID="Panel3" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab1Rep3" runat="server" CssClass="col-md-12 col-sm-12">
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabPanel3Dos" runat="server" HeaderText="Carriers" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="Panel5" runat="server">
                                <asp:Panel ID="Panel6" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab2Rep3" runat="server" CssClass="col-md-12 col-sm-12">
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>
            </asp:Panel>
            <asp:Panel ID="Rep4" runat="server" />
            <asp:Panel ID="Rep7" runat="server" />
            
            
        </asp:Panel>
       
        <asp:Panel ID="pnlCenter" runat="server" CssClass="col-md-4 col-sm-4">

            <asp:Panel ID="Rep2" runat="server" CssClass="Row" />
            <asp:Panel ID="Rep5" runat="server" />
            <asp:Panel ID="Rep8" runat="server" >
                <asp:TabContainer ID="TabContainerPrincipal2" runat="server" CssClass="MyTabStyle">
                    <asp:TabPanel ID="TabPanel2Uno" runat="server" HeaderText="Importes" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="Panel7" runat="server">
                                <asp:Panel ID="Panel8" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab1Rep2" runat="server" CssClass="col-md-12 col-sm-12">
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabPanel2Dos" runat="server" HeaderText="Tráfico (GB)" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="Panel10" runat="server">
                                <asp:Panel ID="Panel11" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab2Rep2" runat="server" CssClass="col-md-12 col-sm-12">
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabPanel2Tres" runat="server" HeaderText="Importe/Tráfico" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="Panel13" runat="server">
                                <asp:Panel ID="Panel14" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab3Rep2" CssClass="col-md-12 col-sm-12" runat="server"></asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>
           </asp:Panel>
            
           
        </asp:Panel>

        <asp:Panel ID="pnlRight" runat="server" CssClass="col-md-4 col-sm-4">
           <asp:Panel ID="Rep3" runat="server" />
           <asp:Panel ID="Rep6" runat="server" />
           
           <asp:Panel ID="Rep9" runat="server">
                <asp:TabContainer ID="TabContainerPrincipal" runat="server" CssClass="MyTabStyle">
                    <asp:TabPanel ID="TabPanelUno" runat="server" HeaderText="Importes" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="Panel22" runat="server">
                                <asp:Panel ID="Panel23" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab1Rep1" runat="server" CssClass="col-md-12 col-sm-12">
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabPanelDos" runat="server" HeaderText="Tráfico (GB)" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="pnlMainHolder2" runat="server">
                                <asp:Panel ID="pnlLeft22" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab2Rep1" runat="server" CssClass="col-md-12 col-sm-12">
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabPanelTres" runat="server" HeaderText="Importe/Tráfico" Visible="true">
                        <ContentTemplate>
                            <asp:Panel ID="pnlMainHolder3" runat="server">
                                <asp:Panel ID="pnlLeft23" runat="server" CssClass="row">
                                    <asp:Panel ID="Tab3Rep1" CssClass="col-md-12 col-sm-12" runat="server"></asp:Panel>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>
            </asp:Panel>
            
        </asp:Panel>

        <asp:Panel ID="Panel1" runat="server" CssClass="col-md-12 col-sm-12">
           <asp:Panel ID="Rep10" runat="server" CssClass="col-md-6 col-sm-6" />
           <asp:Panel ID="Rep11" runat="server"  CssClass="col-md-6 col-sm-6" />
         </asp:Panel>
    </asp:Panel>

    
</asp:Content>

   