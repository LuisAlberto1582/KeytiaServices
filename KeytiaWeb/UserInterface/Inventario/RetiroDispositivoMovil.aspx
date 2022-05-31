<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RetiroDispositivoMovil.aspx.cs" Inherits="KeytiaWeb.UserInterface.Inventario.RetiroDispositivoMovil" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">



    <table>
        <tr>
            <td class="pageTitle">
                <span>Inventario de dispositivos móviles</span>
            </td>
        </tr>
    </table>

    <br />
    <br />


    <asp:Panel ID="Panel1" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Panel ID="SeccionUpdate" runat="server">
                <asp:Panel ID="Panel2" runat="server" CssClass="row">

                    <div class="form-horizontal" role="form">
                        <br />
                        <div class="col-md-12 col-sm-12">
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
                                    <cc1:DSODateTimeBox ID="dtbFechaIngModal" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                        ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                    </cc1:DSODateTimeBox>

                                    <%--<asp:TextBox ID="txtFechaInicioModal" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                        <asp:CalendarExtender ID="ceSelectorFechaInicioModal" runat="server" TargetControlID="txtFechaInicioModal" PopupPosition ="Right">
                                                        </asp:CalendarExtender>--%>
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
                                    <cc1:DSODateTimeBox ID="dtBFechaAsignacionModal" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                        ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                    </cc1:DSODateTimeBox>

                                    <%-- <asp:TextBox ID="txtFechaAsignacionModal" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                        <asp:CalendarExtender ID="ceSeleccionFechaAsignacionModal" runat="server" TargetControlID="txtFechaAsignacionModal" PopupPosition ="Right">
                                                        </asp:CalendarExtender>--%>
                                </div>
                            </div>

                            <br />
                            <br />
                            <div class="modal-footer">
                                <asp:Button ID="btnAccionModal" CssClass="btn btn-keytia-md" runat="server" Text="Aceptar" OnClick="btnBuscaEquipos_Click" />

                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>











            <!--SECCION DELETE-->
            <asp:Panel ID="SeccionDelete" runat="server">

            </asp:Panel>










<%--            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter Center" Width="68%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                                <form name="FR_Busquedas" method="post">
                            
                            <asp:Table runat="server" ID="tablaFiltros" CssClass="DSOGrid" CellSpacing="0" BorderWidth="1"
                                Style="height: 100%; width: 100%; border-collapse: collapse;">
                                
                                <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" CssClass="titulosReportes">
                                    <asp:TableHeaderCell ID="TableHeaderCell1" runat="server" ColumnSpan="2">
                                        <asp:Label ID="Label1" runat="server">Retiro de equipo</asp:Label>
                                    </asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                
                                <asp:TableRow runat="server" ID="TableRow2">
                                    <asp:TableCell ID="TableCell1" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                        &nbsp;<asp:Label ID="Label3" runat="server">IMEI:</asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell ID="TableCell2" runat="server" align="left">
                                        <asp:TextBox ID="TextBox2" runat="server" Width="240" MaxLength="32"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                               
                                <asp:TableRow runat="server" ID="TableRow3">
                                    <asp:TableCell ID="TableCell3" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                        &nbsp;<asp:Label ID="Label4" runat="server">Empleado:</asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell ID="TableCell4" runat="server" align="left">
                                        <asp:TextBox ID="TextBox3" runat="server" Width="240" MaxLength="32"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                                <asp:TableRow runat="server" ID="TableRow5">
                                    <asp:TableCell ID="TableCell7" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                        &nbsp;<asp:Label ID="Label6" runat="server">Fecha de retiro:</asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell ID="TableCell8" runat="server" align="left" CssClass="imgCalendarDetallado">
                                        <cc1:DSODateTimeBox ID="DSODateTimeBox1" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                            ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                        </cc1:DSODateTimeBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                                <asp:TableRow runat="server" ID="TableRow1">
                                    <asp:TableCell ID="TableCell5" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                        &nbsp;<asp:Label ID="Label2" runat="server">Almacén de resguardo:</asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell ID="TableCell6" runat="server" align="left">
                                        <asp:TextBox ID="TextBox1" runat="server" Width="240" MaxLength="32"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                                <asp:TableRow runat="server" align="left" ID="rowUbicacion">
                                    <asp:TableCell ID="cellLblUbicacion" runat="server" align="left" CssClass="titulosFiltroDetallado">
                                        &nbsp;<asp:Label ID="lblUbicacion" runat="server">Motivo:</asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell ID="cellCboTelefonia" runat="server" align="left">
                                        <asp:DropDownList ID="cboUbicacion" runat="server">
                                            <asp:ListItem Text="Término de plazo" Value="1" Selected=True></asp:ListItem>
                                            <asp:ListItem Text="Equipo dañado" Value="2" Selected=False></asp:ListItem>
                                            <asp:ListItem Text="Baja empleado" Value="3" Selected=False></asp:ListItem>
                                        </asp:DropDownList>
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                                <asp:TableRow runat="server" align="left" ID="rowTelefonia">
                                    <asp:TableCell ID="cellLblTelefonia" runat="server" Width="20%" align="left" CssClass="titulosFiltroDetallado">
                                        &nbsp;<asp:Label ID="lblTelefonia" runat="server"></asp:Label>
                                    </asp:TableCell>
                                    
                                    <asp:TableCell ID="cellLblTiposReporte" runat="server" Width="66%" align="left">
                                        <asp:RadioButton ID="rbtnFija" runat="server" Text="Devuelto al operador" Checked="true" GroupName="gpoTelefonia"
                                             CssClass="titulosFiltroDetallado" />
                                        &nbsp;
                                        <asp:RadioButton ID="rbtnMovil" runat="server" Text="Basura electrónica" GroupName="gpoTelefonia" 
                                             CssClass="titulosFiltroDetallado" />                                       
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                                
                                <asp:TableRow ID="TableRow9" runat="server">
                                    <asp:TableCell ID="cellBtnAceptar" runat="server" ColumnSpan="2" align="center">
                                        <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="buttonPlay" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                
                            </asp:Table>
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            </form>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlRep9" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep9" runat="server" CssClass="TopCenter divToCenter" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep9" runat="server" CssClass="TopCenter divToCenter">
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </asp:Panel>--%>



</asp:Content>
