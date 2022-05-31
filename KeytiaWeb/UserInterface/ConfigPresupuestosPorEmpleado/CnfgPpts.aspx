<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="CnfgPpts.aspx.cs" Inherits="KeytiaWeb.UserInterface.ConfigPresupuestosPorEmpleado.CnfgPpts"
    EnableEventValidation="false" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script type="text/javascript" language="javascript">
        function alerta(mensaje) {
            $(document).ready(function() { jAlert(mensaje, "Advertencia"); });
        }

        function confirm(mensaje) {
            $(document).ready(function() { jConfirm(mensaje, "Confirmación"); });
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
    <div>
        <br />
        <asp:Panel ID="pnlTitulos" runat="server">
            <div align="left" style="width: 50%; float: left;">
                <asp:Label ID="lblCenCos" runat="server" CssClass="tituloInicio" Text="Centros de costos">
                </asp:Label>
                <asp:Label ID="lblEmpleados" runat="server" CssClass="tituloInicio" Text="Empleados">
                </asp:Label>
            </div>
            <div align="right" style="width: 50%; float: right;">
                <asp:LinkButton ID="lbtnRegresarABusq" runat="server" Font-Bold="true" OnClick="lbtnRegresarABusq_Click">Volver a selección de centro de costos</asp:LinkButton>
            </div>
        </asp:Panel>
        <br />
        <br />
        <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix">
            &nbsp;<asp:Button ID="btnGuardar" runat="server" Text="Grabar" CssClass="buttonSave"
                OnClick="btnGuardar_Click" />&nbsp;
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="buttonCancel"
                OnClick="btnCancelar_Click" />
        </asp:Panel>
        <%--        <asp:Panel ID="pnlToolBar3" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
            HorizontalAlign="Right">
            <asp:Label ID="lblFiltrar" runat="server" Text="Label">Filtrar : 
            </asp:Label>
            &nbsp;<asp:TextBox ID="txtFiltrar" runat="server" OnTextChanged="txtFiltrar_TextChanged"></asp:TextBox>&nbsp;
        </asp:Panel>--%>
    </div>
    <br />
    <%--Seccion con seleccion de Centros de costos--%>
    <asp:Panel ID="pnlCenCostos" runat="server" HorizontalAlign="Center">
        <asp:Panel ID="titulosgvrCenCos" runat="server" CssClass="PanelTitulosYBordeReportes"
            Height="90%">
            <asp:Panel ID="pnlHeader" runat="server" CssClass="titulosReportes" HorizontalAlign="Left">
                <asp:Label ID="lblHeader" runat="server" Text="Configuración"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlContenedorgrvCenCos" runat="server" ScrollBars="Vertical" Height="400">
                <asp:Table ID="tblEnpnlContenedorgrvCenCos" runat="server" Width="100%" HorizontalAlign="Center">
                    <asp:TableRow ID="tr1" runat="server" Width="100%">
                        <asp:TableCell ID="tc1" runat="server" Width="100%">
                            <asp:Panel ID="pnl1" runat="server" Height="10" Width="100%" CssClass="g-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix">
                            </asp:Panel>
                            <asp:GridView ID="grvCenCos" runat="server" AutoGenerateColumns="false" HorizontalAlign="Center"
                                DataKeyNames="iCodCenCos" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="iCodCenCos" HeaderText="Codigo CenCos" ItemStyle-HorizontalAlign="Left" />
                                    <asp:TemplateField HeaderText="Consultar">
                                        <ItemTemplate>
                                            <div align="center">
                                                <asp:ImageButton ID="ibtnEditPresup" ImageUrl="~/images/search.png" OnClick="grvCenCos_EditRow"
                                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Clave" HeaderText="Clave" ItemStyle-HorizontalAlign="Left" />
                                    <asp:HyperLinkField HeaderText="Centro de costos" DataNavigateUrlFields="iCodCenCos"
                                        DataNavigateUrlFormatString="~/UserInterface/ConfigPresupuestosPorEmpleado/CnfgPpts.aspx?iCodCenCos={0}"
                                        DataTextField="Descripcion" ItemStyle-HorizontalAlign="Left" ControlStyle-ForeColor="Black" />
                                    <asp:BoundField DataField="Centro de Costos Responsable" HeaderText="Centro de Costos Responsable"
                                        ItemStyle-HorizontalAlign="Left" />
                                </Columns>
                                <RowStyle CssClass="grvitemStyle" />
                                <HeaderStyle CssClass="titulosReportes" />
                                <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                            </asp:GridView>
                            <asp:Panel ID="pnl2" runat="server" Height="10" Width="100%" CssClass="g-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix">
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <%--Seccion de Empleados--%>
    <asp:Panel ID="pnlEmpleados" runat="server" HorizontalAlign="Center">
        <asp:Panel ID="Panel2" runat="server" CssClass="PanelTitulosYBordeReportes" Height="90%">
            <asp:Panel ID="Panel3" runat="server" CssClass="titulosReportes" HorizontalAlign="Left">
                <asp:Label ID="Label1" runat="server" Text="Configuración"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="Panel4" runat="server" ScrollBars="Vertical" Height="400">
                <asp:Table ID="Table1" runat="server" Width="100%" HorizontalAlign="Center">
                    <asp:TableRow ID="TableRow1" runat="server" Width="100%">
                        <asp:TableCell ID="TableCell1" runat="server" Width="100%">
                            <asp:Panel ID="Panel5" runat="server" Height="10" Width="100%" CssClass="g-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix">
                            </asp:Panel>
                            <asp:GridView ID="grvEmpleados" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center"
                                Width="100%" EnableViewState="False">
                                <Columns>
                                    <asp:BoundField DataField="iCodRegEmple" HeaderText="iCodRegistroEmple" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="iCodRegPpto" HeaderText="iCodRegistroPpto" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Nombre del empleado" HeaderText="Nombre" ItemStyle-HorizontalAlign="Left" />
                                    <asp:TemplateField HeaderText="Presupuesto" ItemStyle-HorizontalAlign="center">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtPresupuesto" runat="server" Text='<%# Bind("Presupuesto") %>'></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Los campos marcados con * solo deben contener números"
                                                ControlToValidate="txtPresupuesto" Text="*" ValidationExpression="^\d*$">
                                            </asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="rfvPresupuesto" runat="server" Text="*" ErrorMessage="El campo presupuesto es requerido"
                                                ControlToValidate="txtPresupuesto"></asp:RequiredFieldValidator>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Centro de costos" HeaderText="Centro de costos" ItemStyle-HorizontalAlign="Left" />
                                </Columns>
                                <RowStyle CssClass="grvitemStyle" />
                                <HeaderStyle CssClass="titulosReportes" />
                                <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                            </asp:GridView>
                            <asp:Panel ID="Panel6" runat="server" Height="10" Width="100%" CssClass="g-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix">
                            </asp:Panel>
                            <br />
                            <br />
                            <div id="divLabelPptosYRegresarBusqueda" runat="server">
                                <div align="left" style="width: 50%; float: left;">
                                    <asp:Label ID="lblPptoActualText" runat="server" Text="El presupuesto actual de este centro de costos es de : $ "
                                        ForeColor="#365F91" Font-Bold="True" Font-Names="Tahoma" Font-Size="Small">
                                    </asp:Label>
                                    <asp:Label ID="lblPptoActualNum" runat="server" Text="" ForeColor="#365F91" Font-Bold="True"
                                        Font-Names="Tahoma" Font-Size="Small">
                                    </asp:Label>
                                    <asp:Label ID="lblSumaPptoGrid" runat="server" Text="" Visible="false"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblPptoActualTextDisponible" runat="server" Text="El presupuesto disponible de este centro de costos es de : $ "
                                        ForeColor="#365F91" Font-Bold="True" Font-Names="Tahoma" Font-Size="Small">
                                    </asp:Label>
                                    <asp:Label ID="lblPptoActualNumDisponible" runat="server" Text="" ForeColor="#365F91"
                                        Font-Bold="True" Font-Names="Tahoma" Font-Size="Small">
                                    </asp:Label>
                                </div>
                            </div>
                            <br />
                            <div align="center">
                                <asp:ValidationSummary ID="valSumGridEmple" runat="server" />
                            </div>
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
