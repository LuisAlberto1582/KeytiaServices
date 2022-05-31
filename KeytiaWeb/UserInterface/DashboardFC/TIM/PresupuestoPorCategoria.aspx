<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="PresupuestoPorCategoria.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.PresupuestoPorCategoria" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $("[id$='hlkCatMeses']").click(function (e) {
                e.preventDefault();
                var NavigateUrl = $(this).attr("href");
                window.location = NavigateUrl;
            });
            $("[id$='hlkCatMeses']").first().trigger('click');
        });
    </script>

    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pToolBar" runat="server" CssClass="page-title-keytia">
        <asp:Label ID="Label1" runat="server">Control de Presupuestos Por Concepto</asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRow_1_2" runat="server" CssClass="row">
            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_3" runat="server" CssClass="row">
            <asp:Panel ID="Rep3" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_4_5" runat="server" CssClass="row">
            <asp:Panel ID="Rep4" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep5" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_6_7" runat="server" CssClass="row">
            <asp:Panel ID="Rep6" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep7" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_38" runat="server" CssClass="row">
            <asp:Panel ID="Rep8" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="ContenedorPorCategoriaSinCarrier" runat="server" Visible="false" CssClass="table-responsive">
        <asp:GridView ID="gridRepCategoriasSinCarrier" runat="server" AutoGenerateColumns="False" OnRowDataBound="OnRowDataBoundCatSinCarrier" CssClass="table  table-bordered tableDashboard">
            <Columns>
                <asp:BoundField DataField="TDest" InsertVisible="False" ReadOnly="True" Visible="False" />
                <asp:TemplateField HeaderText="Concepto">
                    <ItemTemplate>
                        <asp:HyperLink ID="hlkCatMeses" runat="server" Font-Bold="true" Text='<%# Eval("NombreTDest") %>'
                            NavigateUrl=<%# "javascript:DrillDownSinCarrierNombreTDestMeses('" + DataBinder.Eval(Container.DataItem, "NombreTDest").ToString() + "');"%>></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Presupuesto" HeaderText="Ppto. Anual" DataFormatString="{0:c}" />
                <asp:BoundField DataField="Consumo" HeaderText="Consumo Acum." DataFormatString="{0:c}" />
                <asp:BoundField DataField="Variacion" HeaderText="% Variación" DataFormatString="{0:n}" />
                <asp:BoundField DataField="AnioPresupuesto" HeaderText="Año Ppto." DataFormatString="{0:n}" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <tr>
                            <td colspan="100%" style="padding: 0px;">
                                <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" TargetControlID="pnlDetalleMes"
                                    CollapsedSize="0" Collapsed="true" ExpandControlID="hlkCatMeses" CollapseControlID="hlkCatMeses"
                                    AutoCollapse="false" AutoExpand="false" ScrollContents="true" ExpandDirection="Vertical">
                                </asp:CollapsiblePanelExtender>
                                <div style="text-align: center">
                                    <asp:Panel ID="pnlDetalleMes" runat="server">
                                    </asp:Panel>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>

    <asp:Panel ID="ContenedorPorCategoriaConCarrier" runat="server" Visible="false" CssClass="table-responsive">
        <asp:GridView ID="gridRepCategoriaConCarrier" runat="server" AutoGenerateColumns="False" OnRowDataBound="OnRowDataBoundCatConCarrier" CssClass="table  table-bordered tableDashboard">
            <Columns>
                <asp:BoundField DataField="Carrier" InsertVisible="False" ReadOnly="True" Visible="False" />
                <asp:TemplateField HeaderText="Conceptos por Carrier">
                    <ItemTemplate>
                        <asp:HyperLink ID="hlkCategorias" runat="server" Font-Bold="true" Text='<%# Eval("NombreCarrier") %>'
                            NavigateUrl=<%# "javascript:DrillDownConCarrierNombreTDest('" + DataBinder.Eval(Container.DataItem, "NombreCarrier").ToString() + "');"%>></asp:HyperLink>

                        <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" TargetControlID="pnlDetallCategorias"
                            CollapsedSize="0" Collapsed="true" ExpandControlID="hlkCategorias" CollapseControlID="hlkCategorias"
                            AutoCollapse="false" AutoExpand="false" ScrollContents="true" ExpandDirection="Vertical">
                        </asp:CollapsiblePanelExtender>

                        <asp:Panel ID="pnlDetallCategorias" runat="server" CssClass="table-responsive">
                            <asp:GridView ID="gridRepCategoriasConCarrier" runat="server" AutoGenerateColumns="False" OnRowDataBound="OnRowDataBoundCatConCarrierMes" CssClass="table  table-bordered tableDashboard">
                                <Columns>
                                    <asp:BoundField DataField="Carrier" InsertVisible="False" ReadOnly="True" Visible="False" />
                                    <asp:BoundField DataField="TDest" InsertVisible="False" ReadOnly="True" Visible="False" />
                                    <asp:TemplateField HeaderText="Concepto">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hlkconCarrierCatMeses" runat="server" Font-Bold="true" Text='<%# Eval("NombreTDest") %>'
                                                NavigateUrl=<%# "javascript:DrillDownConCarrierNombreTDestMeses('" + DataBinder.Eval(Container.DataItem, "NombreCarrier").ToString() + "-" + DataBinder.Eval(Container.DataItem, "NombreTDest").ToString() + "');"%>></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Presupuesto" HeaderText="Ppto. Anual" DataFormatString="{0:c}" />
                                    <asp:BoundField DataField="Consumo" HeaderText="Consumo Acum." DataFormatString="{0:c}" />
                                    <asp:BoundField DataField="Variacion" HeaderText="% Variación" DataFormatString="{0:n}" />
                                    <asp:BoundField DataField="AnioPresupuesto" HeaderText="Año Ppto." DataFormatString="{0:n}" />
                                    <asp:BoundField DataField="NombreUltimoMesCargado" HeaderText="Último mes" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <tr>
                                                <td colspan="100%" style="padding: 0px;">
                                                    <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" TargetControlID="pnlDetalleMes"
                                                        CollapsedSize="0" Collapsed="true" ExpandControlID="hlkconCarrierCatMeses" CollapseControlID="hlkconCarrierCatMeses"
                                                        AutoCollapse="false" AutoExpand="false" ScrollContents="true" ExpandDirection="Vertical">
                                                    </asp:CollapsiblePanelExtender>
                                                    <div style="text-align: center">
                                                        <asp:Panel ID="pnlDetalleMes" runat="server">
                                                        </asp:Panel>
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
