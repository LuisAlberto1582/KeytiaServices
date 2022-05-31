<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="WorkflowMovEnPBXPendientes.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkflowMovEnPBXPendientes" %>

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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Listado de movimientos pendientes en el conmutador:</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
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
                            <table class="DSOGrid" cellspacing="0" border="0" style="height: 100%; width: 100%;
                                border-collapse: collapse;">
                                <tbody>
                                    <tr class="titulosReportes">
                                        <th colspan="9">
                                            <asp:Label ID="Label1" runat="server">Filtros</asp:Label>
                                        </th>
                                    </tr>
                                    <tr class="grvitemStyle">
                                        <td align="left" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblProveedor" runat="server">Proveedor:</asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="cboProveedor" runat="server" Width="200px" DataValueField="iCodCatalogo"
                                                DataTextField="Descripcion">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="iCodProveedor" runat="server" Value="0" />
                                        </td>
                                        <td align="left" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblSolicitud" runat="server">Solicitud:</asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="cboSolicitud" runat="server" Width="200px" DataValueField="iCodCatalogo"
                                                DataTextField="Descripcion">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="iCodSolicitud" runat="server" Value="0" />
                                        </td>
                                        <td align="left" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblSitio" runat="server">Sitio:</asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="cboSitio" runat="server" Width="150px" DataValueField="iCodCatalogo"
                                                DataTextField="Descripcion">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="iCodSitio" runat="server" Value="0" />
                                        </td>
                                        <td align="left" class="titulosFiltroDetallado">
                                            &nbsp;<asp:Label ID="lblTecnologia" runat="server">Tecnología:</asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="cboTecnologia" runat="server" Width="150px" DataValueField="iCodCatalogo"
                                                DataTextField="Descripcion">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="iCodTecnologia" runat="server" Value="0" />
                                        </td>
                                        <td align="left">
                                            &nbsp;<asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="buttonPlay"
                                                OnClick="btnFiltrar_Click" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlRep2" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep2" runat="server" CssClass="TopCenter Center" Width="98%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep2" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <div style="width: 100%; max-height: 400px; overflow: scroll">
                                <asp:GridView ID="gridMovEnPBX" runat="server" AutoGenerateColumns="false" CssClass="DSOGrid"
                                    HeaderStyle-CssClass="titulosReportes" ShowFooter="true" FooterStyle-CssClass="titulosReportes"
                                    FooterStyle-HorizontalAlign="Center" DataKeyNames="iCodRegistro,ProveedorID,SolicitudID,SitioID,RutaImgEvidencia"
                                    EmptyDataText="No se encontraron resultados">
                                    <Columns>
                                        <%--0--%><asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/pencilsmall.png" OnClick="gridMovEnPBX_EditRow"
                                                    runat="server" CommandArgument='<%# Eval("iCodRegistro") %>' RowIndex='<%# Container.DisplayIndex %>' />&nbsp;&nbsp;&nbsp;&nbsp;
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--1--%><asp:BoundField DataField="ProveedorNom" HeaderText="Proveedor" ItemStyle-HorizontalAlign="Center" />
                                        <%--2--%><asp:BoundField DataField="SolicitudID" HeaderText="Solicitud" ItemStyle-HorizontalAlign="Center" />
                                        <%--3--%><%--<asp:BoundField DataField="RespuestaPBX" HeaderText="Respuesta PBX" ItemStyle-HorizontalAlign="Center" />--%>
                                        <%--4--%><asp:BoundField DataField="FechaRegistro" HeaderText="Fecha Registro" ItemStyle-HorizontalAlign="Center" />
                                        <%--5--%><asp:BoundField DataField="SitioNom" HeaderText="Sitio" ItemStyle-HorizontalAlign="Center" />
                                        <%--6--%><asp:BoundField DataField="TecnologiaDesc" HeaderText="Tecnología" ItemStyle-HorizontalAlign="Center" />
                                        <%--7--%><asp:BoundField DataField="IPSitio" HeaderText="IP" ItemStyle-HorizontalAlign="Center" />
                                        <%--8--%><asp:BoundField DataField="MovimientoDesc" HeaderText="Movimiento" ItemStyle-HorizontalAlign="Center" />
                                        <%--9--%><asp:BoundField DataField="Recurso" HeaderText="Recurso" ItemStyle-HorizontalAlign="Center" />
                                        <%--10--%><asp:BoundField DataField="CosCod" HeaderText="Cos" ItemStyle-HorizontalAlign="Center" />
                                        <%--11--%><asp:BoundField DataField="CosDesc" HeaderText="Permisos" ItemStyle-HorizontalAlign="Center" />
                                        <%--12--%><asp:BoundField DataField="EmpleadoNom" HeaderText="Solicitante" ItemStyle-HorizontalAlign="Center" />
                                        <%--13--%><asp:BoundField DataField="Telefono" HeaderText="Teléfono" ItemStyle-HorizontalAlign="Center" />
                                        <%--14--%><%--<asp:BoundField DataField="Comentarios" HeaderText="Comentarios" ItemStyle-HorizontalAlign="Left" />--%>
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
    <%----%>
    <%--NZ: Modal para Editar el Mov en PBX.--%>
    <asp:Panel ID="pnlPopupEditMovEnPBX" runat="server" CssClass="modalPopupEtq" Style="display: none;
        width: 1200px">
        <div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px;
            font-size: 12px">
            <asp:Label runat="server" ID="lblTituloEditMovEnPBX"></asp:Label>
        </div>
        <div class="bodyEtq">
            <asp:Panel ID="pnlRepEditMovEnPBX" runat="server" CssClass="TopCenter divToCenter">
                <asp:Table ID="tblRepEditMovEnPBX" runat="server" CssClass="TopCenter Center" Width="98%">
                    <asp:TableRow>
                        <asp:TableCell Width="100%">
                            <asp:Panel ID="RepEditMovEnPBX" runat="server" CssClass="TopCenter divToCenter">
                                <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                    style="height: 10px; width: 100%;">
                                </div>
                                <asp:Table runat="server" ID="tablaCaptura" CssClass="DSOGrid" CellSpacing="0" BorderWidth="1"
                                    Style="height: 100%; width: 100%; border-collapse: collapse;">
                                    <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" CssClass="titulosReportes">
                                        <asp:TableHeaderCell ID="celdaTitulo" runat="server" ColumnSpan="5">
                                            <asp:Label ID="Label2" runat="server">Datos</asp:Label>
                                        </asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                    <asp:TableRow runat="server" align="left" ID="rowSolicitudRecurso">
                                        <asp:TableCell ID="cellLblSolicitudModal" runat="server" align="left" CssClass="titulosFiltroDetallado"
                                            Width="8%">
                                            <asp:Label ID="lblSolicitudModal" runat="server">Solicitud:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtSolicitudModal" runat="server" align="left" Width="50%"
                                            Enabled="false">
                                            <asp:TextBox ID="txtSolicitudModal" runat="server" Enabled="false"></asp:TextBox>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVaciaRow1" runat="server" align="left" Width="2%">
                                            <asp:HiddenField runat="server" ID="hfICodRegBitacora" Value="" />
                                            <asp:HiddenField runat="server" ID="hfRutaImgEvidencia" Value="" />
                                            <asp:HiddenField runat="server" ID="hfProveedorID" Value="" />
                                            <asp:HiddenField runat="server" ID="hfSolicitudID" Value="" />
                                            <asp:HiddenField runat="server" ID="hfSitioID" Value="" />
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellLblRecursoModal" runat="server" Width="8%" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblRecursoModal" runat="server">Recurso:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtRecurso" runat="server" align="left" Width="30%">
                                            <asp:TextBox ID="txtRecursoModal" runat="server" Enabled="false" Font-Bold="true"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow runat="server" ID="rowEmpleadoRegistro">
                                        <asp:TableCell ID="cellLblEmpleadoModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblEmpleadoModal" runat="server">Empleado:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtEmpleadoModal" runat="server" align="left">
                                            <asp:TextBox ID="txtEmpleadoModal" runat="server" Enabled="false" Font-Bold="true"
                                                Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVaciaRow2" runat="server" align="left">                                        
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellLblRegistroModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblRegistroModal" runat="server">Registro:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtRegistroModal" runat="server" align="left">
                                            <asp:TextBox ID="txtRegistroModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow runat="server" ID="rowSitioTecnologia">
                                        <asp:TableCell ID="cellLblSitioModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblSitioModal" runat="server">Sitio:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtSitioModal" runat="server" align="left">
                                            <asp:TextBox ID="txtSitioModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVaciaRow3" runat="server" align="left">                                        
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellLblTecnologiaModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblTecnologiaModal" runat="server">Tecnología:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtTecnologiaModal" runat="server" align="left">
                                            <asp:TextBox ID="txtTecnologiaModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow runat="server" ID="rowIPMovimiento">
                                        <asp:TableCell ID="cellLblIPModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblIPModal" runat="server">IP:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtIPModal" runat="server" align="left">
                                            <asp:TextBox ID="txtIPModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVaciaRow4" runat="server" align="left">                                        
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellLblMovimientoModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblMovimientoModal" runat="server">Movimiento:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtMovimientoModal" runat="server" align="left">
                                            <asp:TextBox ID="txtMovimientoModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow runat="server" ID="rowPermisoTel">
                                        <asp:TableCell ID="cellLblPermisoModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblPermisoModal" runat="server">Permiso:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtPermisoModal" runat="server" align="left">
                                            <asp:TextBox ID="txtPermisoModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVaciaRow5" runat="server" align="left">                                        
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellLblTelefonoModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblTelefonoModal" runat="server">Teléfono:</asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtRespuestaModal" runat="server" align="left">
                                            <asp:TextBox ID="txtTelefonoModal" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow runat="server" ID="rowEvidencia">
                                        <asp:TableCell ID="cellEvidenciaModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                            <asp:Label ID="lblEvidencia" runat="server">Evidencia:</asp:Label>
                                            <%--<asp:Button runat="server" ID="UploadButtonEvidencia" Text="Upload" OnClick="UploadButtonEvidencia_Click" />--%>
                                            <%--<asp:ImageButton ID="UploadButtonEvidencia" runat="server" ImageUrl="~/images/addsmall.png"
                                                CssClass="buttonAdd" OnClick="UploadButtonEvidencia_Click" />--%>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellTxtEvidenciaModal" runat="server" align="left">
                                            <asp:FileUpload ID="FileUploadEvidencia" runat="server" />
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVaciaRow6" runat="server" align="left">                                        
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellCancelarRegistro" runat="server" align="left">                                        
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellckbCancelarReg" runat="server" align="left">
                                           <%-- <asp:CheckBox runat="server" Text="¿Cancelar Movimiento?" />--%>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow runat="server" align="left" ID="rowComentarios">
                                        <asp:TableCell ID="cellComentarioModal" runat="server" ColumnSpan="5">
                                            <asp:Table ID="Table2" runat="server" Width="100%">
                                                <asp:TableRow ID="rowComentarioModal" runat="server">
                                                    <asp:TableCell ID="cellLblComentarioModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                                        &nbsp;<asp:Label ID="lblComentarioModal" runat="server">Comentario:</asp:Label>
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                                <asp:TableRow ID="TableRow4" runat="server" Width="98%">
                                                    <asp:TableCell ID="cellTxtComentarioModal" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                                        &nbsp;<asp:TextBox ID="txtComentarioModal" runat="server" TextMode="MultiLine" Height="50px"
                                                            MaxLength="8000" Width="98%" Font-Names="Arial"></asp:TextBox>
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                            </asp:Table>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="rowBotones" runat="server">
                                        <asp:TableCell ID="TableCellVacia2" runat="server" align="left">                                            
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellBtnCancelarModal" runat="server" align="center">
                                            <asp:Button ID="btnCancelarModal" runat="server" Text="Salir" CssClass="buttonPlay"
                                                OnClientClick="return Hidepopup()" />
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellBtnAceptarModal" runat="server" align="center" ColumnSpan="2">
                                            <asp:Button ID="btnGuardarCambiosHallazgoModal" runat="server" Text="Guardar" CssClass="buttonPlay"
                                                OnClick="btnGuardarCambiosBitacoraMovPBX_Click" />
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellVacia4" runat="server" align="left">                                        
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                    style="height: 10px; width: 100%;">
                                </div>
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
        </div>
        <div class="footerEtq" style="vertical-align: middle; text-align: center;" align="center">
            <%--<asp:Button ID="Button1" runat="server" Text="      CERRAR      " CssClass="yesEtq" />--%>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnEditMovEnPBX" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEditMovEnPBX" runat="server" PopupControlID="pnlPopupEditMovEnPBX"
        TargetControlID="lnkBtnEditMovEnPBX" OkControlID="btnYes" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
</asp:Content>
