<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConferenceAdd.aspx.cs" Inherits="KeytiaWeb.UserInterface.SYO.admin.Conference.ConferenceAdd" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .CheckList {
            /*margin-top: 5px;
            font-size: 14px;
            font-weight: bold;
            display: inline-block;*/
        }

            .CheckList td {
                padding-right: 15px;
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

    <asp:Label ID="lblTitle" runat="server" Text="Administrar Conferencias" CssClass="page-title-keytia"></asp:Label>

    <!--Crear conferencia-->
    <asp:Panel ID="pHeaderDatosConferencia" runat="server" CssClass="collapsible-Keytia">
        <asp:Table ID="tblHeaderConferencia" runat="server" Width="100%">
            <asp:TableRow ID="tblHeaderConferenciaF1" runat="server">
                <asp:TableCell ID="tblHeaderConferenciaC1" runat="server">
                    <asp:Label ID="lblDatosConferencia" runat="server" Text="Programar Conferencia"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeaderConferenciaC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pDatosConferencia" runat="server" BackColor="White" CssClass="col-md-12 col-sm-12">
        <br />
        <br />
        <div class="form-horizontal" role="form">
            <div class="row">
                <div class="col-md-6 col-sm-6">
                    <div class="form-group">
                        <asp:Label ID="lblNombre" runat="server" CssClass="col-sm-3 control-label">Nombre:</asp:Label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblFechaT" runat="server" CssClass="col-sm-3 control-label">Fecha Inicio:</asp:Label>
                        <div class="col-sm-9">
                            <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="true" ShowMinute="true"
                                ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                            </cc1:DSODateTimeBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblParticipantes" runat="server" CssClass="col-sm-3 control-label">Participantes:</asp:Label>
                        <div class="col-sm-9">
                            <asp:CheckBoxList ID="chkListParticipantes" runat="server" CssClass="CheckList" RepeatColumns="3">
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>

                <div class="col-md-6 col-sm-6">
                    <div class="form-group">
                        <asp:Label ID="lblDescripcion" runat="server" CssClass="col-sm-3 control-label">Descripción:</asp:Label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" MaxLength="300"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblFechaFin" runat="server" CssClass="col-sm-3 control-label">Fecha Fin:</asp:Label>
                        <div class="col-sm-9">
                            <cc1:DSODateTimeBox ID="pdtFin" runat="server" Row="1" ShowHour="true" ShowMinute="true"
                                ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                            </cc1:DSODateTimeBox>
                        </div>
                    </div>                    
                </div>
            </div>
        </div>
        <br />
        <asp:Table ID="tblGuardar" runat="server" Width="100%">
            <asp:TableRow ID="tblEdiGuardar" runat="server">
                <asp:TableCell ID="tblEditGuardarC1" runat="server" HorizontalAlign="Center">
                    <asp:LinkButton ID="lbtnSaveConferencia" runat="server" Text="Agregar" OnClick="lbtnSaveConferencia_Click" CssClass="btn btn-keytia-sm"></asp:LinkButton>&nbsp&nbsp                           
                    <asp:LinkButton ID="lbtnCancelarAddConferencia" Text="Cancelar" runat="server" CssClass="btn btn-keytia-sm" OnClick="lbtnCancelarAddConferencia_Click"></asp:LinkButton>&nbsp&nbsp
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpeDatosConferenciaImg" runat="server" TargetControlID="pDatosConferencia"
        ExpandControlID="pHeaderDatosConferencia" CollapseControlID="pHeaderDatosConferencia" CollapsedText="Mostrar..."
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical">
    </asp:CollapsiblePanelExtender>

    <br />

    <!--Conferencia-->
    <asp:Panel ID="pHeaderDatosConferenciaProg" runat="server" CssClass="collapsible-Keytia">
        <asp:Table ID="tblHeaderConferenciaProg" runat="server" Width="100%">
            <asp:TableRow ID="tblHeaderConferenciaProgF1" runat="server">
                <asp:TableCell ID="tblHeaderConferenciaProgC1" runat="server">
                    <asp:Label ID="lblDatosConferenciaProg" runat="server" Text="Conferencias Programadas"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeaderConferenciaProgC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse2" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pDatosConferenciaProg" runat="server" BackColor="White" CssClass="col-md-12 col-sm-12">
        <div class="table-responsive scrollbar scrollbar-warning thin">
            <asp:GridView ID="grvConferencias" runat="server" DataKeyNames="Id,Nombre"
                AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                EmptyDataText="No existen confierencias programadas">
                <Columns>
                    <%--0--%><asp:BoundField DataField="Id" Visible="false" ReadOnly="true" />
                    <%--1--%><asp:BoundField DataField="NumberId" HeaderText="Conferencia" />
                    <%--2--%><asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <%--3--%><asp:BoundField DataField="FechaInicio" HeaderText="Fecha Inicial" DataFormatString="{0:g}" />
                    <%--4--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Final" DataFormatString="{0:g}" />
                    <%--5--%><asp:BoundField DataField="Estatus" HeaderText="Estatus" />
                    <%--6--%><asp:BoundField DataField="Participantes" HeaderText="Participantes" />
                </Columns>
            </asp:GridView>
            <br />
        </div>
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpeDatosConferenciaProgImg" runat="server" TargetControlID="pDatosConferenciaProg"
        ExpandControlID="pHeaderDatosConferenciaProg" CollapseControlID="pHeaderDatosConferenciaProg" CollapsedText="Mostrar..."
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse2" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical">
    </asp:CollapsiblePanelExtender>

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
</asp:Content>


