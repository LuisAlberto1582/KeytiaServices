<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ListadoReportes.aspx.cs" Inherits="KeytiaWeb.UserInterface.Administracion.CMP.ListadoReportes" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="../ETL/CSS/StyleFile.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
 <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="modalUpload">
                <div class="centerUpload">
                    <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
        <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
    <div id="pnlMainHolder" runat="server">
        <div id="pnlRow_0" runat="server" cssclass="row">
            <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Historial de Reportes Automáticos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body" id="divContenedor">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    &nbsp;&nbsp;&nbsp;
                                    <asp:Button OnClick="btnAgregar_Click" CssClass="btn btn-keytia-sm" runat="server" ID="btnAgregar" Text="Agregar" ></asp:Button>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-sm-8">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                <div class="col-offset-2 col-sm-4">
                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" runat="server" id="rowGrv">
                                        <div class="col-sm-12">
                                            <div class="clearfix"></div>
                                            <div class="table-responsive">
                                               <asp:GridView HeaderStyle-CssClass="tableHeaderStyle"
                                                    CssClass="table table-bordered tableDashboard" HeaderStyle-Font-Bold="true" runat="server" ID="grvCargas"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                   <%-- <div class="row">
                                        <div class="col-sm-12">
                                            <!-- Alerta Success -->
                                            <div style="width: 400px; float: none; margin: 0 auto;">
                                                <asp:Panel ID="InfoPanelSucces" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                                    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                                    <strong>
                                                        <asp:Label ID="lblMensajeSuccess" runat="server"></asp:Label>
                                                    </strong>
                                                </asp:Panel>
                                                <br />
                                            </div>
                                        </div>--%>

                                    <%--</div>--%>                                   
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>
                </div>
                <%--<div class="modal-footer">
                    <asp:Button ID="btnSi" runat="server" Text="SI" CssClass="btn btn-keytia-sm" OnClick="btnSi_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnNo" runat="server" Text="NO" CssClass="btn btn-keytia-sm" />
                </div>--%>
            </div>
        </div>
    </asp:Panel>
   <%-- <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
        TargetControlID="lnkBtnMsn" OkControlID="btnNo" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>--%>
            <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
    <div class="modal" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true"><i class="fas fa-times"></i></button>
                        <h4 class="modal-title" id="exampleModalLabel">!Atención¡</h4>
                    </div>
                    <div class="panel-body">
                        <div class="form-group">
                            <br />
                            <label for="recipient-name" class="control-label" id="lblMensaje" style="font-size: 15px;"></label>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-4">
                                    <button type="button" class="btn btn-keytia-sm" onclick="eliminaCarga();">Si</button>
                                    &nbsp;&nbsp;&nbsp;
                                    <button type="button" class="btn btn-keytia-sm" data-dismiss="modal">No</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>