<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="HallazgosGA.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.HallazgosGA" %>

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
    <div>
        <asp:Label ID="lblInicio" runat="server" CssClass="page-title-keytia">Gestión Administrativa</asp:Label>
        <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header">
            &nbsp;&nbsp;<asp:Button ID="btnRegresar" runat="server" Text="< Regresar" CssClass="buttonBack"
                OnClick="btnRegresar_Click" />
            &nbsp;&nbsp;<asp:Button ID="btnExportarXLS" runat="server" Text="Exportar XLS" CssClass="buttonPlay"
                OnClick="btnExportarXLS_Click" />
        </asp:Panel>
    </div>
    <br />
    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="row divCenter" Visible="false">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-10 col-sm-10">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">
                                <asp:Label ID="lblTitulo" runat="server">Búsqueda</asp:Label>
                            </span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepBusquedaCollapse" aria-expanded="true" aria-controls="RepBusquedaCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div id="RepBusquedaCollapse" class="form-horizontal" role="form">
                            <asp:Panel ID="rowBusqueda" runat="server" CssClass="form-group">
                                <asp:Label ID="lblCarrier" runat="server" CssClass="col-sm-1 control-label">Carrier:</asp:Label>
                                <div class="col-sm-3">
                                    <asp:DropDownList ID="cboCarrier" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" OnSelectedIndexChanged="cboCarrierIndex_Changed"
                                        AutoPostBack="true" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodCarrier" runat="server" Value="0" />
                                </div>

                                <asp:Label ID="lblAnio" runat="server" CssClass="col-sm-1 control-label">Año:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="cboAnio" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" OnSelectedIndexChanged="cboCarrierIndex_Changed"
                                        AutoPostBack="true" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodAnio" runat="server" Value="0" />
                                </div>

                                <asp:Label ID="lblMes" runat="server" CssClass="col-sm-1 control-label">Mes:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="cboMes" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" OnSelectedIndexChanged="cboCarrierIndex_Changed"
                                        AutoPostBack="true" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodMes" runat="server" Value="0" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-sm" OnClick="btnAceptar_Click" />
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRep1" runat="server" Visible="false" CssClass="row divCenter">
            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Filtros</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepFiltrosCollapse" aria-expanded="true" aria-controls="RepFiltrosCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div id="RepFiltrosCollapse" class="form-horizontal" role="form">
                            <asp:Panel ID="rowFiltros" runat="server" CssClass="form-group">
                                <asp:Label ID="lblCuenta" runat="server" CssClass="col-sm-1 control-label">Cuenta:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="cboCtaMestra" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodCtaMaestra" runat="server" Value="0" />
                                </div>

                                <asp:Label ID="lblTDest" runat="server" CssClass="col-sm-1 control-label">Serivicio:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="cboTDest" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodTDest" runat="server" Value="0" />
                                </div>

                                <asp:Label ID="lblVariacion" runat="server" CssClass="col-sm-1 control-label">Variación:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="cboVariacion" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodVariacion" runat="server" Value="0" />
                                </div>

                                <asp:Label ID="lblEstatus" runat="server" CssClass="col-sm-1 control-label">Estatus:</asp:Label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="cboEstatus" runat="server" DataValueField="iCodCatalogo"
                                        DataTextField="Descripcion" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="iCodEstatus" runat="server" Value="0" />
                                </div>
                                <div class="col-sm-1">
                                    <br />
                                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-keytia-sm" OnClick="btnFiltrar_Click" />
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRep2" runat="server" Visible="false">
            <asp:Panel ID="Rep2" runat="server">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Resultado</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in" id="RepDetallCollapse">
                            <div class="table-responsive">
                                <asp:GridView ID="gridHallazgos" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered tableDashboard" UseAccessibleHeader="true"
                                    FooterStyle-HorizontalAlign="Center" DataKeyNames="HallazgoID,CarrierID,CtaMaestraID,TDestID,AnioID,MesID,VariacionID,EstatusID,CategoriaID"
                                    EmptyDataText="No se encontraron resultados" HeaderStyle-CssClass="tableHeaderStyle">
                                    <Columns>
                                        <%--0--%><asp:BoundField DataField="HallazgoID" Visible="false" />
                                        <%--1--%><asp:TemplateField HeaderText="Folio">
                                            <ItemTemplate>
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>
                                                                <asp:ImageButton ID="btnEditarHallazgoRow" ImageUrl="~/images/pencilsmall.png" OnClick="gridHallazgos_EditRow"
                                                                    runat="server" CommandArgument='<%# Eval("HallazgoID") %>' RowIndex='<%# Container.DisplayIndex %>' />
                                                            </td>
                                                            <td>
                                                                <asp:LinkButton ID="btnLinkFolio" runat="server" Text='<%# Eval("Folio") %>' CommandArgument='<%# Eval("HallazgoID") %>'
                                                                    Style="font-weight: bold; margin-left: 15px;" OnClick="btnLinkFolio_Click"></asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--2--%><asp:BoundField DataField="Hallazgo" HeaderText="Hallazgo" ItemStyle-HorizontalAlign="Left" />
                                        <%--3--%><asp:BoundField DataField="CarrierID" Visible="false" />
                                        <%--4--%><asp:BoundField DataField="Carrier" HeaderText="Carrier" ItemStyle-HorizontalAlign="Center" />
                                        <%--5--%><asp:BoundField DataField="CtaMaestraID" Visible="false" />
                                        <%--6--%><asp:TemplateField HeaderText="Cuenta" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCuenta" runat="server" Text='<%# Eval("Cuenta") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--7--%><asp:BoundField DataField="TDestID" Visible="false" />
                                        <%--8--%><asp:TemplateField HeaderText="Servicio" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTDest" runat="server" Text='<%# Eval("Servicio") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--9--%><asp:BoundField DataField="AnioID" Visible="false" />
                                        <%--10--%><asp:BoundField DataField="Anio" HeaderText="Año" ItemStyle-HorizontalAlign="Center" />
                                        <%--11--%><asp:BoundField DataField="MesID" Visible="false" />
                                        <%--12--%><asp:BoundField DataField="Mes" HeaderText="Mes" ItemStyle-HorizontalAlign="Center" />
                                        <%--13--%><asp:BoundField DataField="VariacionID" Visible="false" />
                                        <%--14--%><asp:TemplateField HeaderText="Variación" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblVariacion" runat="server" Text='<%# Eval("Variacion") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--15--%><asp:TemplateField HeaderText="Importe">
                                            <ItemTemplate>
                                                <asp:Label ID="lblImporte" runat="server" Text='<%# Eval("Importe", "{0:c}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--16--%><asp:BoundField DataField="EstatusID" Visible="false" />
                                        <%--17--%><asp:TemplateField HeaderText="Estatus" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblEstatus" runat="server" Text='<%# Eval("Estatus") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--18--%><asp:BoundField DataField="Descripcion" HeaderText="Descripción" ItemStyle-HorizontalAlign="Left" />
                                        <%--19--%><asp:BoundField DataField="CategoriaID" Visible="false" />
                                        <%--20--%><asp:TemplateField HeaderText="Categoría" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCategoria" runat="server" Text='<%# Eval("Categoria") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <%----%>
    <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <h5>
                        <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>
                    </h5>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje" TargetControlID="lnkBtnMsn"
        BackgroundCssClass="modalPopupBackground" DropShadow="false" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>
    <%----%>
    <%--NZ: Modal para Editar el Hallazgo.--%>
    <asp:Panel ID="pnlPopupEditHallazo" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="rule"></div>
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label runat="server" ID="lblTituloEditHallazo"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <div id="RepCapturaCollapse" class="form-horizontal" role="form">
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblFolioModal" runat="server" CssClass="control-label">Folio:</asp:Label>
                                <asp:TextBox ID="txtFolioModal" runat="server" MaxLength="10" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblCategoriaModal" runat="server" CssClass="control-label">Estatus:</asp:Label>
                                <asp:DropDownList ID="cboEstatusModal" runat="server" DataValueField="iCodCatalogo"
                                    DataTextField="Descripcion" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Label ID="lblHallazgoModal" runat="server" CssClass="control-label">Hallazgo:</asp:Label>
                                <asp:TextBox ID="txtHallazgoModal" runat="server" TextMode="MultiLine" Height="50px" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblCarrierModal" runat="server" CssClass="control-label">Carrier:</asp:Label>
                                <asp:TextBox ID="txtCarrierModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblCtaMaestraModal" runat="server" CssClass="control-label">Cuenta:</asp:Label>
                                <asp:TextBox ID="txtCtaMaestraModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblAnioModal" runat="server" CssClass="control-label">Año:</asp:Label>
                                <asp:TextBox ID="txtAnioModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblMesModal" runat="server" CssClass="control-label">Mes:</asp:Label>
                                <asp:TextBox ID="txtMesModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblTipoDestinoModal" runat="server" CssClass="control-label">Servicio:</asp:Label>
                                <asp:TextBox ID="txtTDestModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblVariacionModal" runat="server" CssClass="control-label">Variación:</asp:Label>
                                <asp:TextBox ID="txtVariacionModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 col-sm-6">
                                <asp:Label ID="lblImporteModal" runat="server" CssClass="control-label">Importe:</asp:Label>
                                <asp:TextBox ID="txtImporteModal" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 col-sm-6">
                                <asp:TextBox ID="txtIdHallazoModal" runat="server" Enabled="false" Visible="false"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Label ID="lblDescripcionModal" runat="server" CssClass="control-label">Descripción:</asp:Label>
                                <asp:TextBox ID="txtDescripcionModal" runat="server" TextMode="MultiLine" Height="50px" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancelarModal" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                    <asp:Button ID="btnGuardarCambiosHallazgoModal" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnGuardarCambiosHallazgoModal_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnEditHallazo" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEditHallazo" runat="server" PopupControlID="pnlPopupEditHallazo"
        TargetControlID="lnkBtnEditHallazo" CancelControlID="btnCerrar" BackgroundCssClass="modalPopupBackground" DropShadow="false">
    </asp:ModalPopupExtender>
    <%----%>
    <%--NZ: Modal para mostrar TODOS los comentarios.--%>
    <asp:Panel ID="pnlPopupComentarios" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="rule"></div>
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label runat="server" ID="lblTituloComentarios"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarComentarios"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <div class="container-fluid center-block">
                        <!-- Row 1 -->
                        <div class="col-sm-10">
                            <div class="input-group">
                                <asp:Label ID="lblAddComentarioModal" runat="server" CssClass="control-label">Agregar comentario al Hallazgo:</asp:Label>
                                <span class="input-group-btn">
                                    <asp:ImageButton ID="btnAgregarComentario" runat="server" ImageUrl="~/images/addsmall.png" OnClick="btnAgregarComentario_Click" />
                                </span>
                            </div>
                        </div>
                        <div class="col-sm-10">
                            <div class="form-group">
                                <asp:TextBox ID="txtAddComentarioModal" runat="server" TextMode="MultiLine" Height="50px" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gridComentarios" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered tableDashboard"
                            HeaderStyle-CssClass="tableHeaderStyle" DataKeyNames="HallazgoID" EmptyDataText="No hay Comentarios">
                            <Columns>
                                <asp:BoundField DataField="HallazgoID" Visible="false" />
                                <asp:BoundField DataField="Usuario" HeaderText="Usuario" ItemStyle-HorizontalAlign="Left" />
                                <asp:BoundField DataField="Comentario" HeaderText="Comentario" ItemStyle-HorizontalAlign="Left" />
                                <asp:BoundField DataField="Fecha" HeaderText="Fecha" ItemStyle-HorizontalAlign="Left" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnOKComentarios" runat="server" Text="CERRAR" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnComentarios" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeComentarios" runat="server" PopupControlID="pnlPopupComentarios" BackgroundCssClass="modalPopupBackground"
        TargetControlID="lnkBtnComentarios" DropShadow="false" CancelControlID="btnCerrarComentarios">
    </asp:ModalPopupExtender>
</asp:Content>
