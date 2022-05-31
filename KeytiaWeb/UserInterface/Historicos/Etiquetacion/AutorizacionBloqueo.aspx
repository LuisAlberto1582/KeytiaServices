<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutorizacionBloqueo.aspx.cs"
    Inherits="KeytiaWeb.UserInterface.AutorizacionBloqueo" MasterPageFile="~/KeytiaOH.Master" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--Barra con boton y fechas--%>
    <div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Empleados Listos para Bloquear</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                &nbsp;&nbsp;<asp:Button ID="btnRegresar" runat="server" Text="< Regresar" CssClass="buttonBack"
                    OnClick="btnRegresar_Click" />
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                    style="height: 10px; width: 100%;">
                </div>
                <div style="width: 100%; max-height: 380px; overflow: auto;">
                    <asp:GridView ID="grdEmpleSinEtiq" runat="server" AutoGenerateColumns="false" CssClass="DSOGrid"
                        EmptyDataText="No hay empleados pendientes de Autorización">
                        <Columns>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lbliCodCatalogoEmple" runat="server" Text='<%# Eval ("iCodCatalogoEmple") %>'
                                        Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Autorizado">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chbxAutorizado" Checked='<%# Convert.ToBoolean(Eval("AprobadoBloqueo"))%>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nombre">
                                <ItemTemplate>
                                    <asp:Label ID="lblNombre" runat="server" Text='<%# Eval("Nombre") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Centro de Costos">
                                <ItemTemplate>
                                    <asp:Label ID="lblCenCos" runat="server" Text='<%# Eval("CenCos") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sitio">
                                <ItemTemplate>
                                    <asp:Label ID="lblSitio" runat="server" Text='<%# Eval("Sitio") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Puesto" ItemStyle-Width="182">
                                <ItemTemplate>
                                    <asp:Label ID="lblPuesto" runat="server" Text='<%# Eval("Puesto") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cantidad de Llamadas sin etiqueta" ItemStyle-Width="232">
                                <ItemTemplate>
                                    <asp:Label ID="lblCantNums" runat="server" Text='<%# Eval("CantNums") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle CssClass="grvitemStyle" />
                        <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                    </asp:GridView>
                    <%--<asp:Button ID="GuardarPrevio" runat="server" Text="Guardar Previo" OnClick="GuardarPrevio_Click" />
                    <asp:Button ID="GuardarYEnviar" runat="server" Text="Guardar y enviar" OnClick="GuardarYEnviar_Click" />--%>
                </div>
                <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                    style="height: 10px; width: 100%;">
                </div>
            </asp:Panel>
            <asp:Button ID="GuardarPrevio" runat="server" Text="Guardar Previo" OnClick="GuardarPrevio_Click"
                Class="buttonPlay ui-button ui-widget ui-state-default ui-corner-all ui-state-hover" />
            <asp:Button ID="GuardarYEnviar" runat="server" Text="Guardar y enviar" OnClick="GuardarYEnviar_Click"
                Class="buttonPlay ui-button ui-widget ui-state-default ui-corner-all ui-state-hover" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
