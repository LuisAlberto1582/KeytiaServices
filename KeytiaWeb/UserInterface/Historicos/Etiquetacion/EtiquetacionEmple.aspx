<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="EtiquetacionEmple.aspx.cs" Inherits="KeytiaWeb.UserInterface.EtiquetacionEmple" %>

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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Números no etiquetados</asp:Label>
            </div>
            
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
            &nbsp;&nbsp;<asp:Button ID="btnRegresar" runat="server" Text="< Regresar" CssClass="buttonBack" OnClick="btnRegresar_Click" />            
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter Center" Width="70%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <div style="width: 100%; max-height: 380px; overflow: auto">
                                <asp:GridView ID="gridResumenNums" runat="server" AutoGenerateColumns="false" CssClass="DSOGrid"
                                    HeaderStyle-CssClass="titulosReportes" ShowFooter="true" FooterStyle-CssClass="titulosReportes"
                                    FooterStyle-HorizontalAlign="Center" OnRowDataBound="OnRowDataBound" EmptyDataText="No cuenta con números por Etiquetar">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Número">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnLinkNumero" runat="server" Text='<%# Eval("Numero") %>' CommandArgument='<%# Eval("Numero") %>'
                                                    OnClick="btnLinkNumero_Click" Style="font-weight: bold;"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cantidad">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCantidad" runat="server" Text='<%# Eval("Cantidad") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Duración">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDuracion" runat="server" Text='<%# Eval("Duracion") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Costo">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCosto" runat="server" Text='<%# Eval("Costo") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Grupo" ItemStyle-Width="182">
                                            <ItemTemplate>
                                                <asp:Label ID="lblGrupo" runat="server" Text='<%# Eval("Grupo") %>' Visible="false" />
                                                <asp:DropDownList ID="ddlGrupoEtiqueta" runat="server" Width="180">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Etiqueta" ItemStyle-Width="232">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtEtiqueta" runat="server" Width="230" Text='<%# Eval("Etiqueta") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <RowStyle CssClass="grvitemStyle" />
                                    <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                                </asp:GridView>
                            </div>
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <br />
                            <asp:Panel runat="server" HorizontalAlign="Center">
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click"
                                    CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click"
                                    CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <%----%>
        <%--NZ: Modal para mensajes de Error--%>
        <asp:Panel ID="pnlPopupMensaje" runat="server" CssClass="modalPopupEtq" Style="display: none;
            width: 600px">
            <div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px;
                font-size: 12px">
                <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
            </div>
            <div class="bodyEtq" style="margin-left: 10px; margin-right: 10px; font-weight: bold;">
                <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>
            </div>
            <div class="footerEtq" align="right">
                <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="yesEtq" />
            </div>
        </asp:Panel>
        <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
        <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
            TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
        <%----%>
        <%--NZ: Modal para mostrar el Detalle de Llamadas de un número.--%>
        <asp:Panel ID="pnlPopupDetalleLlams" runat="server" CssClass="modalPopupEtq" Style="display: none;
            width: 600px">
            <div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px;
                font-size: 12px">
                <asp:Label runat="server" ID="lblTituloDetallLlams"></asp:Label>
            </div>
            <div class="bodyEtq">
                <asp:Panel ID="pnlRepDetallLlams" runat="server" CssClass="TopCenter divToCenter">
                    <asp:Table ID="tblRepDetallLlams" runat="server" CssClass="TopCenter Center" Width="90%">
                        <asp:TableRow>
                            <asp:TableCell Width="100%">
                                <asp:Panel ID="RepDetallLlams" runat="server" CssClass="TopCenter divToCenter">
                                    <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                        style="height: 10px; width: 100%;">
                                    </div>
                                    <div style="width: 100%; max-height: 300px; overflow: auto">
                                        <asp:GridView ID="gridDetallLlams" runat="server" AutoGenerateColumns="false" CssClass="DSOGrid"
                                            HeaderStyle-CssClass="titulosReportes" ShowFooter="true" FooterStyle-CssClass="titulosReportes"
                                            FooterStyle-HorizontalAlign="Center" Style="height: 100%;">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Fecha">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFechaPopup" runat="server" Text='<%# Eval("Fecha") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duración">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDuracionPopup" runat="server" Text='<%# Eval("Duracion") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Costo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCostoPopup" runat="server" Text='<%# Eval("Costo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Extensión">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblExtensionPopup" runat="server" Text='<%# Eval("Extension") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <RowStyle CssClass="grvitemStyle" />
                                            <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                                        </asp:GridView>
                                    </div>
                                    <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                        style="height: 10px; width: 100%;">
                                    </div>
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
            </div>
            <div class="footerEtq" align="right">
                <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="yesEtq" />
            </div>
        </asp:Panel>
        <asp:LinkButton ID="lnkBtnDetallLlams" runat="server" Style="display: none"></asp:LinkButton>
        <asp:ModalPopupExtender ID="mpeEtqDetallLlams" runat="server" PopupControlID="pnlPopupDetalleLlams"
            TargetControlID="lnkBtnDetallLlams" OkControlID="btnYes" BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
    </asp:Panel>
</asp:Content>
