<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DispositivoMovil.aspx.cs" Inherits="KeytiaWeb.UserInterface.Inventario.DispositivoMovil" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--    <script type="text/javascript">
        function ShowSection(section) {
            var create = document.getElementById("SectionCreate");
            var update = document.getElementById("SectionUpdate");
            var list = document.getElementById("SectionList");


            var sec = document.getElementById(section);

            create.style.display = "none";
            update.style.display = "none";
            list.style.display = "none";

            sec.style.display = "block";
        }


    </script>--%>
    <style>
        .btnSection {
            float: right;
            margin: 5px;
            padding: 4px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlMainHolder" runat="server">
        <div class="page-title-keytia">
            <span>Inventario de dispositivos móviles</span>
        </div>
        <br />
        <asp:Panel ID="seccionAltaEquipos" runat="server" CssClass="row">
            <asp:Panel ID="pHeaderRegistroEquipo" runat="server" CssClass="collapsible-Keytia">
                <asp:Table ID="tblHeaderEmple" runat="server" Width="100%">
                    <asp:TableRow ID="tblHeaderEmpleF1" runat="server">
                        <asp:TableCell ID="tblHeaderEmpleC1" runat="server">
                            <asp:Label ID="lblDatosEmple" runat="server" Text="Registro de Equipos"></asp:Label>
                        </asp:TableCell>
                        <%--<asp:TableCell ID="tblHeaderEmpleC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>--%>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
            <asp:Panel ID="pregistroEquipo" runat="server" BackColor="White" CssClass="col-md-12 col-sm-12">
                <section id="SectionCreate" runat="server">
                    <asp:Panel ID="PnlCreate" runat="server" CssClass="row">
                        <br />
                        <br />
                        <div class="form-horizontal" role="form">
                            <div class="col-md-12 col-sm-12">
                                <div class="form-group">
                                    <asp:Label ID="Label3" runat="server" CssClass="col-sm-3 control-label">IMEI:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtIMEI" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblMarca" runat="server" CssClass="col-sm-3 control-label">Marca:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlMarca" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblModelo" runat="server" CssClass="col-sm-3 control-label">Modelo:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtModelo" runat="server" MaxLength="32" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblNoSerie" runat="server" CssClass="col-sm-3 control-label">No. Serie:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtNoSerie" runat="server" MaxLength="32" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblColor" runat="server" CssClass="col-sm-3 control-label">Color:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtColor" runat="server" MaxLength="32" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblFechaIng" runat="server" CssClass="col-sm-3 control-label">Fecha ingreso:</asp:Label>
                                    <div class="col-sm-6">
                                        <cc1:DSODateTimeBox ID="dtbFechaIng" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                        </cc1:DSODateTimeBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblEmpleado" runat="server" CssClass="col-sm-3 control-label">Empleado:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlEmpleado" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblFechaAsignacion" runat="server" CssClass="col-sm-3 control-label">Fecha asignación:</asp:Label>
                                    <div class="col-sm-6">
                                        <cc1:DSODateTimeBox ID="dtbFechaAsignacion" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                        </cc1:DSODateTimeBox>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="btnCreate" CssClass="btn btn-keytia-md" runat="server" Text="Aceptar" OnClick="btnCreate_Click" />
                                </div>
                            </div>
                        </div>

                    </asp:Panel>

                </section>
            </asp:Panel>


            <%--   <asp:CollapsiblePanelExtender ID="cpeRegistroEquipoImg" runat="server" TargetControlID="pregistroEquipo"
        ExpandControlID="pHeaderRegistroEquipo" CollapseControlID="pHeaderRegistroEquipo"
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical" >
    </asp:CollapsiblePanelExtender>--%>
        </asp:Panel>

        <asp:Panel ID="seccionBusquedaEuipos" runat="server" CssClass="row">
            <asp:Panel ID="PnlHeaderBusquedaEquipo" runat="server" CssClass="collapsible-Keytia">
                <asp:Table ID="Table3" runat="server" Width="100%">
                    <asp:TableRow ID="TableRow19" runat="server">
                        <asp:TableCell ID="TableCell35" runat="server">
                            <asp:Label ID="Label1" runat="server" Text="Busqueda de Equipos"></asp:Label>
                        </asp:TableCell>
                        <%--  <asp:TableCell ID="TableCell36" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse2" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>--%>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
            <asp:Panel ID="pBusquedaEquipo" runat="server" BackColor="White" CssClass="col-md-12 col-sm-12">
                <section id="SectionList" runat="server">
                    <asp:Panel ID="Rep9" runat="server" CssClass="row">

                        <div class="form-horizontal" role="form">
                            <br />
                            <br />

                            <div class="col-md-12 col-sm-12">
                                <div class="form-group">
                                    <asp:Label ID="lblIMEIBuscar" runat="server" CssClass="col-sm-3 control-label">IMEI:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtIMEIBuscar" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="form-group">
                                    <asp:Label ID="lblMarcaBuscar" runat="server" CssClass="col-sm-3 control-label">Marca:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlMarcaBuscar" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>



                                <div class="form-group">
                                    <asp:Label ID="lblModeloBuscar" runat="server" CssClass="col-sm-3 control-label">Modelo:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtModeloBuscar" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                    </div>
                                </div>



                                <div class="form-group">
                                    <asp:Label ID="lblNoSerieBuscar" runat="server" CssClass="col-sm-3 control-label">No. Serie:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtNoSerieBuscar" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                    </div>
                                </div>



                                <div class="form-group">
                                    <asp:Label ID="lblColorBuscar" runat="server" CssClass="col-sm-3 control-label">Color:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtColorBuscar" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                    </div>
                                </div>



                                <%--<div class="form-group">
                            <asp:Label ID="lblFechaIngresoBuscar" runat="server" CssClass="col-sm-3 control-label">Fecha ingreso:</asp:Label>
                            <div class="col-sm-6">
                                <cc1:DSODateTimeBox ID="dtbFechaIngresoBuscar" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                    ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                </cc1:DSODateTimeBox>
                            </div>
                        </div>--%>



                                <div class="form-group">
                                    <asp:Label ID="lblEmpleBuscar" runat="server" CssClass="col-sm-3 control-label">Empleado:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlEmpleBuscar" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>



                                <%--<div class="form-group">
                            <asp:Label ID="lblFehaAsignacionBuscar" runat="server" CssClass="col-sm-3 control-label">Fecha asignación:</asp:Label>
                            <div class="col-sm-6">
                                <cc1:DSODateTimeBox ID="dtBFechaAsignacionBuscar" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                    ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                </cc1:DSODateTimeBox>
                            </div>
                        </div>--%>


                                <div class="modal-footer">
                                    <asp:Button ID="btnBuscaEquipos" CssClass="btn btn-keytia-md" runat="server" Text="Buscar" OnClick="btnBuscaEquipos_Click" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlGRID" runat="server" CssClass="col-md-12 col-sm-12" BackColor="White">
                        <div class="table-responsive">
                            <asp:GridView ID="grvEquipo" runat="server" DataKeyNames="iCodCatEquipo"
                                AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                EmptyDataText="No se encuentran equipos registrados">
                                <Columns>
                                    <asp:TemplateField HeaderText="Editar" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnEditarEquipoMovilRow" ImageUrl="~/images/pencilsmall.png" OnClick="btnEditarEquipoMovilRow_Click"
                                                runat="server" RowIndex='<%# Container.DisplayIndex %>' Accion="Update" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Borrar" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnBorrarEquipoMovilRow" ImageUrl="~/images/deletesmall.png" OnClick="btnBorrarEquipoMovilRow_Click"
                                                runat="server" RowIndex='<%# Container.DisplayIndex %>' Accion="Delete" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="iCodCatEquipo" Visible="false" ReadOnly="true" />
                                    <asp:BoundField DataField="IMEI" HeaderText="IMEI" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Marca" HeaderText="Marca" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Modelo" HeaderText="Modelo" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="No. Serie" HeaderText="No. Serie" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Color" HeaderText="Color" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Fecha Registro" HeaderText="Fecha Registro" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Nomina" HeaderText="Nomina" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Colaborador" HeaderText="Colaborador" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />

                                </Columns>
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                </section>
            </asp:Panel>
            <%--  <asp:CollapsiblePanelExtender ID="cpeBusquedaEquipoImg" runat="server" TargetControlID="pBusquedaEquipo"
        ExpandControlID="PnlHeaderBusquedaEquipo" CollapseControlID="PnlHeaderBusquedaEquipo"
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse2" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical" >
    </asp:CollapsiblePanelExtender>--%>
        </asp:Panel>
        <br />




        <%--Modal DELETE--%>
        <asp:Panel ID="pnlModalDelete" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <asp:Label Font-Bold="true" ID="lblTituloModalDelete" runat="server" Text=""></asp:Label>
                        <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalDelete"><i class="fas fa-times"></i></button>
                    </div>

                    <div class="modal-body scrollbar scrollbar-warning thin">
                        <div class="form-horizontal" role="form">
                            <div class="row">

                                <div class="col-md-12 col-sm-12">

                                    <div class="form-group">
                                        <asp:Label ID="lblICodCatEquipoModalDelete" runat="server" CssClass="col-sm-3 control-label" Enabled="false" Visible="false" Text=""></asp:Label>
                                    </div>

                                    <div class="form-group">
                                        <asp:Label ID="lblIMEIModalDelete" runat="server" CssClass="col-sm-3 control-label">IMEI:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtIMEIModalDelete" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <asp:Label ID="lblEmpleModalDelete" runat="server" CssClass="col-sm-3 control-label">Empleado:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList ID="ddlEmpleModalDelete" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <asp:Label ID="lblFehcaRetiroModladelete" runat="server" CssClass="col-sm-3 control-label">Fecha de retiro:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtFechaRetiroModalDelete" runat="server" CssClass="form-control" ReadOnly="false" Enabled="true" MaxLength="10"></asp:TextBox>
                                            <asp:CalendarExtender ID="ceFecharetiroModalDelete" runat="server" TargetControlID="txtFechaRetiroModalDelete">
                                            </asp:CalendarExtender>
                                        </div>
                                    </div>


                                    <div class="form-group">
                                        <asp:Label ID="lblAlmacenResguardoModalDelete" runat="server" CssClass="col-sm-3 control-label">Almacén de resguardo:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtAlmacenResguardoModalDelete" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <asp:Label ID="lblMotivoModalDelete" runat="server" CssClass="col-sm-3 control-label">Motivo:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList ID="ddlMotivoModalDelete" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <div class="col-sm-6">
                                            <asp:RadioButton ID="rbtnDevueltoModalDelete" runat="server" Text="Devuelto al operador" CssClass="checkbox-inline" Checked="true" GroupName="gpoTelefonia" />
                                        </div>

                                        <div class="col-sm-6">
                                            <asp:RadioButton ID="rbtnBasuraModelDelete" runat="server" Text="Basura electrónica" CssClass="checkbox-inline" GroupName="gpoTelefonia" />
                                        </div>
                                    </div>

                                    <div class="modal-footer">
                                            <asp:Button ID="btnAceptarModalDelete" runat="server" Text="Aceptar" CssClass="btn btn-keytia-md" OnClick="btnAceptarModalDelete_Click" />
                                    </div>



                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:LinkButton ID="lnkbtnModalDelete" runat="server" Style="display: none"></asp:LinkButton>
        <asp:ModalPopupExtender ID="mpeModalDelete" runat="server" PopupControlID="pnlModalDelete" TargetControlID="lnkbtnModalDelete"
            BackgroundCssClass="modalPopupBackground" DropShadow="false" CancelControlID="btnCerrarModalDelete">
        </asp:ModalPopupExtender>




        <!--Modal Update-->
        <asp:Panel ID="pnlAccionEquipo" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
            <div class="modal-dialog modal-md">
                <div class="modal-content">
                    <div class="modal-header">
                        <asp:Label Font-Bold="true" ID="lblTituloPopUpAccion" runat="server" Text=""></asp:Label>
                        <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalEditAccion"><i class="fas fa-times"></i></button>
                    </div>

                    <div class="modal-body scrollbar scrollbar-warning thin">
                        <div class="form-horizontal" role="form">
                            <div class="row">

                                <div class="col-md-12 col-sm-12">

                                    <div class="form-group">
                                        <asp:Label ID="lblICodCatEquipoModal" runat="server" CssClass="col-sm-3 control-label" Enabled="false" Visible="false" Text=""></asp:Label>

                                    </div>


                                    <div class="form-group">
                                        <asp:Label ID="lblIMEIModal" runat="server" CssClass="col-sm-3 control-label">IMEI:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtIMEIModal" runat="server" CssClass="form-control" MaxLength="32" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>


                                    <div class="form-group">
                                        <asp:Label ID="lblMarcaMarcaModal" runat="server" CssClass="col-sm-3 control-label">Marca:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList ID="ddlMarcaModal" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </div>



                                    <div class="form-group">
                                        <asp:Label ID="lblModeloModal" runat="server" CssClass="col-sm-3 control-label">Modelo:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtModeloModal" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                        </div>
                                    </div>



                                    <div class="form-group">
                                        <asp:Label ID="lblNoSerieModal" runat="server" CssClass="col-sm-3 control-label">No. Serie:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtNoSerieModal" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                        </div>
                                    </div>



                                    <div class="form-group">
                                        <asp:Label ID="lblColorModal" runat="server" CssClass="col-sm-3 control-label">Color:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="txtColorModal" runat="server" CssClass="form-control" MaxLength="32"></asp:TextBox>
                                        </div>
                                    </div>



                                    <div class="form-group">
                                        <asp:Label ID="lblFechaIngresoModal" runat="server" CssClass="col-sm-3 control-label">Fecha ingreso:</asp:Label>
                                        <div class="col-sm-6">
                                            <%--     <cc1:DSODateTimeBox ID="dtbFechaIngModal" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                                        </cc1:DSODateTimeBox>--%>

                                            <asp:TextBox ID="txtFechaInicioModal" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                            <asp:CalendarExtender ID="ceSelectorFechaInicioModal" runat="server" TargetControlID="txtFechaInicioModal">
                                            </asp:CalendarExtender>
                                        </div>
                                    </div>



                                    <div class="form-group">
                                        <asp:Label ID="lblEmpleadoModal" runat="server" CssClass="col-sm-3 control-label">Empleado:</asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList ID="ddlEmpleadoModal" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>



                                    <div class="form-group">
                                        <asp:Label ID="lblFehaAsignacionModal" runat="server" CssClass="col-sm-3 control-label">Fecha asignación:</asp:Label>
                                        <div class="col-sm-6">
                                            <%--      <cc1:DSODateTimeBox ID="dtBFechaAsignacionModal" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                                        </cc1:DSODateTimeBox>--%>

                                            <asp:TextBox ID="txtFechaAsignacionModal" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                            <asp:CalendarExtender ID="ceSeleccionFechaAsignacionModal" runat="server" TargetControlID="txtFechaAsignacionModal">
                                            </asp:CalendarExtender>
                                        </div>
                                    </div>

                                    <br />
                                    <br />
                                    <div class="modal-footer">
                                        <asp:Button ID="btnAccionUpdateModal" CssClass="btn btn-keytia-md" runat="server" Text="Aceptar" OnClick="btnAccionUpdateModal_Click" />

                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:LinkButton ID="lnkBtnModalAccion" runat="server" Style="display: none"></asp:LinkButton>
        <asp:ModalPopupExtender ID="mpeAccionEquipo" runat="server" PopupControlID="pnlAccionEquipo" TargetControlID="lnkBtnModalAccion"
            BackgroundCssClass="modalPopupBackground" DropShadow="false" CancelControlID="btnCerrarModalEditAccion">
        </asp:ModalPopupExtender>


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
        <asp:ModalPopupExtender ID="mpeEquipoMsn" runat="server" PopupControlID="pnlPopupMensaje" TargetControlID="lnkBtnMsn"
            BackgroundCssClass="modalPopupBackground" DropShadow="false" CancelControlID="btnCerrarMensajes">
        </asp:ModalPopupExtender>

    </asp:Panel>
</asp:Content>



