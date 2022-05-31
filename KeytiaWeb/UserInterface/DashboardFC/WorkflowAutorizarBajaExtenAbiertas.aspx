<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="WorkflowAutorizarBajaExtenAbiertas.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkflowAutorizarBajaExtenAbiertas" %>

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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Extensiones abiertas del periodo: </asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                &nbsp;&nbsp;<asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="buttonBack" OnClick="btnGuardar_Click" />
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep1" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep1" runat="server" CssClass="TopCenter Center" Width="98%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep1" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <div style="width: 100%; max-height: 400px; overflow: scroll">
                                <asp:GridView ID="gridExtenAbiertas" runat="server" AutoGenerateColumns="false" CssClass="DSOGrid"
                                    HeaderStyle-CssClass="titulosReportes" ShowFooter="true" FooterStyle-CssClass="titulosReportes"
                                    FooterStyle-HorizontalAlign="Center" DataKeyNames="iCodRegistro,ExtenID,EmpleID,SitioID,FechaMaxEdit,IsEnable"
                                    EmptyDataText="No se encontraron extensiones abiertas a autorizar">
                                    <Columns>
                                        <%--0--%><asp:TemplateField HeaderText="Autorizar Baja" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rdbAutorizar" runat="server" Checked='<%# !((int)Eval("AutorizoBaja") == 0) %>'
                                                    GroupName="Grupo1" Enabled='<%# !((int)Eval("IsEnable") == 0) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--0--%><asp:TemplateField HeaderText="Rechazar Baja" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rdbRechazar" runat="server" Checked='<%# !((int)Eval("PermanceEnSistema") == 0) %>'
                                                    GroupName="Grupo1" Enabled='<%# !((int)Eval("IsEnable") == 0) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--1--%><asp:BoundField DataField="Extension" HeaderText="Extensión" ItemStyle-HorizontalAlign="Center" />
                                        <%--2--%><asp:BoundField DataField="SitioDesc" HeaderText="Sitio" ItemStyle-HorizontalAlign="Center" />
                                        <%--4--%><asp:BoundField DataField="EmpleNombre" HeaderText="Empleado Responsable" ItemStyle-HorizontalAlign="Left" />
                                        <%--5--%><asp:BoundField DataField="CenCosDesc" HeaderText="Centro de Costos" ItemStyle-HorizontalAlign="Left" />
                                        <%--6--%><asp:BoundField DataField="CenCosDesc" HeaderText="Puesto" ItemStyle-HorizontalAlign="Center" />
                                        <%--7--%><asp:BoundField DataField="EmpleEmail" HeaderText="Email" ItemStyle-HorizontalAlign="Center" />
                                    </Columns>
                                    <RowStyle CssClass="grvitemStyle" />
                                    <AlternatingRowStyle CssClass="grvalternateItemStyle" />
                                </asp:GridView>
                            </div>
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <br />
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </asp:Panel>
    <%----%>
    <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" CssClass="modalPopupEtq" Style="display: none;
        width: 650px; height: auto">
        <div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px;
            font-size: 12px">
            <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
        </div>
        <div class="bodyEtq" style="margin-left: 10px; margin-right: 10px; font-weight: bold;">
            <asp:Label ID="lblBodyModalMsn" runat="server" Text="" Font-Size="Small"></asp:Label>
        </div>
        <div class="footerEtq" style="vertical-align: middle; text-align: center;" align="center">
            <asp:Button ID="btnYes" runat="server" Text="      OK      " CssClass="yesEtq" />
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
        TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
</asp:Content>
