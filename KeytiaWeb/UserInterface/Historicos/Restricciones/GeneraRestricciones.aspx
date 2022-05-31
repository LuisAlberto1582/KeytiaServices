<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneraRestricciones.aspx.cs"
    Inherits="KeytiaWeb.UserInterface.GeneraRestricciones" MasterPageFile="~/KeytiaOH.Master" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">    
    <style type="text/css">
        .modalProgress {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.65;
            -moz-opacity: 0.8;
        }

        .centerProgress {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            background-color: White;
            border-radius: 10px;
            filter: alpha(opacity=100);
            opacity: 1;
            -moz-opacity: 1;
        }

            .centerProgress img {
                height: 128px;
                width: 128px;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:UpdatePanel ID="updContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlMainHolder" runat="server" CssClass="row">
                <asp:Panel ID="pnlRep0" runat="server" CssClass="col-md-12 col-sm-12">
                    <div class="portlet solid bordered">
                        <div class="portlet-title">
                            <div class="caption">
                                <i class="icon-bar-chart font-dark hide"></i>
                                <span class="caption-subject titlePortletKeytia">Restricciones de Usuario</span>
                            </div>
                            <div class="actions">
                                <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                            </div>
                        </div>
                        <div class="portlet-body">
                            <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="col-sm-3 control-label">Perfil:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlPerfiles" OnSelectedIndexChanged="OnSelectedIndexChanged_ddlPerfiles"
                                            AutoPostBack="true" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="col-sm-3 control-label">Usuario:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlUsuarios" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="col-sm-3 control-label">Restricciones De:</asp:Label>
                                    <div class="col-sm-2">
                                        <asp:CheckBox ID="chbxSitio" Checked="false" runat="server" Text="Sitio" CssClass="checkbox-inline" />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:CheckBox ID="chbxCenCos" Checked="false" runat="server" Text="CenCos" CssClass="checkbox-inline" />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:CheckBox ID="chbxEmple" Checked="false" runat="server" Text="Empleado" CssClass="checkbox-inline" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="col-sm-3 control-label">Mensaje:</asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtbxMsg" runat="server" TextMode="MultiLine" Height="60px" Enabled="false" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="btnGenerar" OnClick="OnClick_btnGenerar" Text="Generar restricciones" runat="server" CssClass="btn btn-keytia-md" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" ID="ProcesandoupdContent" AssociatedUpdatePanelID="updContent">
        <ProgressTemplate>
            <div class="modalProgress">
                <div class="centerProgress">
                    <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <%----%>
    <%--NZ: Modal para mensajes de Error--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMsn" runat="server" Text="No se puede generar restricciones"></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblBodyModalMsn" runat="server" Text="Se debe de seleccionar un usuario o un Perfil para proceder con la generación de restricciones"></asp:Label>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
        TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>
    <%----%>
</asp:Content>
