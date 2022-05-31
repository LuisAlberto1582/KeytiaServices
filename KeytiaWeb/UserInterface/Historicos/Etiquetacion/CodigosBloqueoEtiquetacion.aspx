<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodigosBloqueoEtiquetacion.aspx.cs"
    Inherits="KeytiaWeb.UserInterface.CodigosBloqueoEtiquetacion" MasterPageFile="~/KeytiaOH.Master" %>

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
                     />
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">    
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID ="tblDDls" runat="server" align="Center" CssClass="TopCenter Center" Width="70%">
              <asp:TableRow runat="server" align="Right" ID="rowTecnologia" >
                    <asp:TableCell ID="cellLblTecnologia" runat="server" align="left" CssClass="titulosFiltroDetallado">
                        &nbsp;<asp:Label ID="lblTecnoligia" runat="server" Text="Tecnología:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDDLTecnologia" runat="server" align="left" CssClass="titulosFiltroDetallado">                        
                        <asp:DropDownList ID="ddlTecnoligia" runat="server"></asp:DropDownList>
                    </asp:TableCell>
                    <asp:TableCell ID="cellChBx" runat="server" align="Right" CssClass="titulosFiltroDetallado" >
                        <asp:CheckBox ID ="chBoxSeleccionarTodos"  Text="Seleccionar Todos" runat="server"  AutoPostBack="true" OnCheckedChanged="CheckChanged"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server" align="Right" ID="rowSitio" >
                    <asp:TableCell ID="cellLblSitio" runat="server" align="left" CssClass="titulosFiltroDetallado">
                        &nbsp;<asp:Label ID="lblSitio" runat="server" Text="Sitio:"></asp:Label>                        
                    </asp:TableCell>
                    <asp:TableCell ID="cellDDLSitio" runat="server" align="left" CssClass="titulosFiltroDetallado">    
                        <asp:DropDownList ID="ddlSitio" runat="server"></asp:DropDownList>
                    </asp:TableCell>
                     <asp:TableCell ID="cellBtnAplicar" runat="server" align="Right" CssClass="titulosFiltroDetallado" >
                        <asp:Button ID="btnAplicar" runat="server" Text="Aplicar Filtros" CssClass="buttonPlay" OnClick="AplicaFiltros_Click"/>
                    </asp:TableCell>
                </asp:TableRow>               
            </asp:Table>
        
            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter Center" Width="70%">
                 
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <div style="width: 100%; max-height: 380px; overflow: auto;">                      
                            <asp:GridView ID="grdCodigosEnProceso" runat="server" AutoGenerateColumns="false" CssClass="DSOGrid" EmptyDataText="No hay codigos pendientes">
                                <Columns>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lbliCodRegistroCodigo" runat="server" Text='<%# Eval ("iCodRegistroCodigo") %>'
                                                Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lbliCodRegistroBitacora" runat="server" Text='<%# Eval ("iCodRegistroBitacora") %>'
                                                Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chBxSeleccionar" Checked='<%# Convert.ToBoolean(Eval("Seleccionar"))%>'
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCodigo" runat="server" Text='<%# Eval("codAutoCod") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sitio">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSitio" runat="server" Text='<%# Eval("sitioDesc") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tecnología">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTecnologia" runat="server" Text='<%# Eval("tecnologiaDesc") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IP">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIP" runat="server" Text='<%# Eval("ip") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comentario" ItemStyle-Width="182">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComentario" runat="server" Text='<%# Eval("comentario") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lbliCodCatEstatus" runat="server" Text='<%# Eval("estatusBloqueo") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acción" ItemStyle-Width="182">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAccion" runat="server" Text='<%# Eval("estatusBloqueoDesc") %>' />
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
                <asp:TableRow ID="rowBtns" runat="server">
                    <asp:TableCell ID="cellBtns" runat="server" ColumnSpan="2" align="Right">
                        <asp:Button ID="btnLanzar" runat="server" Text="Insertar" CssClass="buttonPlay" CommandArgument="INSERTAR" OnClick="MostrarAviso_Click"/>
                        <asp:Button ID="btnEnviar" runat="server" Text="Cambiar Resultado" CssClass="buttonPlay" CommandArgument="MODIFICAR" OnClick="MostrarAviso_Click"/>
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar Codigo" CssClass="buttonPlay" CommandArgument="CANCELAR" OnClick="MostrarAviso_Click" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>           
        </asp:Panel>        
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
                <asp:Label ID="lblBodyModalMsn" runat="server" Text="">
                </asp:Label>
            </div>
            <div class="footerEtq" align="right">
                <asp:Button ID="btnYes" runat="server" Text="Aceptar" CssClass="yesEtq" CommandArgument=""  OnClick="Modal_Aceptar_Click" UseSubmitBehavior="false"/>
                <asp:Button ID="btnNo" runat="server" Text="Cancelar" CssClass="yesEtq" />
            </div>
        </asp:Panel>
        <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none" UseSubmitBehavior="false"></asp:LinkButton>
        <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
            TargetControlID="lnkBtnMsn"   BackgroundCssClass="modalBackground"> <%--OkControlID="btnYes"--%>
        </asp:ModalPopupExtender>
        
        
   
    
</asp:Content>

