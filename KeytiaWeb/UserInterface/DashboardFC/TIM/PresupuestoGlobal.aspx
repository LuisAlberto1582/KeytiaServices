<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="PresupuestoGlobal.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.PresupuestoGlobal" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $("[id$='hlkCarrierMeses']").click(function (e) {
                e.preventDefault();
                var NavigateUrl = $(this).attr("href");
                window.location = NavigateUrl;
            });
            $("[id$='hlkCarrierMeses']").first().trigger('click');
        });
    </script>

    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pToolBar" runat="server" CssClass="page-title-keytia">
        <asp:Label ID="lblInicio" runat="server">Control de Presupuestos Global</asp:Label>
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

        <asp:Panel ID="pnlRow_9" runat="server" CssClass="row">
            <asp:Panel ID="Rep9" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="ContenedorPorCarrier" runat="server" Visible="false" CssClass="table-responsive">
        <asp:GridView ID="gridRepCarries" runat="server" AutoGenerateColumns="False" OnRowDataBound="OnRowDataBound" CssClass="table  table-bordered tableDashboard">
            <Columns>
                <asp:BoundField DataField="Carrier" InsertVisible="False" ReadOnly="True" Visible="False" />
                <asp:TemplateField HeaderText="Carrier">
                    <ItemTemplate>
                        <asp:HyperLink ID="hlkCarrierMeses" runat="server" Font-Bold="true" Text='<%# Eval("NombreCarrier") %>'
                            NavigateUrl=<%# "javascript:DrillDownCarrierMeses('" + DataBinder.Eval(Container.DataItem, "NombreCarrier").ToString() + "');"%>></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Presupuesto" HeaderText="Ppto. Anual" DataFormatString="{0:c}" />
                <asp:BoundField DataField="Consumo" HeaderText="Consumo Acum." DataFormatString="{0:c}" />
                <asp:BoundField DataField="Variacion" HeaderText="% Variación" DataFormatString="{0:n}" />             
                <asp:BoundField DataField="AnioPresupuesto" HeaderText="Año Ppto." DataFormatString="" />
                <asp:BoundField DataField="NombreUltimoMesCargado" HeaderText="Último mes" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <tr>
                            <td colspan="100%" style="padding: 0px;">
                                <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" TargetControlID="pnlDetalleMes"
                                    CollapsedSize="0" Collapsed="true" ExpandControlID="hlkCarrierMeses" CollapseControlID="hlkCarrierMeses"
                                    AutoCollapse="false" AutoExpand="false" ScrollContents="true" ExpandDirection="Vertical">
                                </asp:CollapsiblePanelExtender>
                                <asp:Panel ID="pnlDetalleMes" runat="server">
                                </asp:Panel>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
