<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Autorizadores.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.Autorizadores1" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;
        $(function () {
            $("#" + "<%=txtCencosto.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetCencos",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.idCencos };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    $("#" + "<%=txtCencosto.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtCencosId.ClientID %>").val(ui.item.description);
                }
            });
        });

    </script>
    <style>
        .alinear {
            text-align: !important right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="modalUpload">
                <div class="centerUpload">
                    <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Autorizadores</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-sm-10">
                                                            <div class="form-horizontal" role="form" runat="server" id="formNomina">
                                                                <asp:Panel ID="Panel1" runat="server" CssClass="form-group">
                                                                    <asp:Label ID="lblCencostos" runat="server" CssClass="col-sm-6 control-label">Concepto Busqueda (Sociedad,Direccion,area,cecos): </asp:Label>
                                                                    <div class="col-sm-6">
                                                                        <asp:TextBox ID="txtCencosto" runat="server" CssClass="autosuggest placeholderstile form-control"
                                                                            onfocus="javascript:$(this).autocomplete('search','');" placeholder="Sociedad, Direccion, area, cecos" />
                                                                        <div style="display: none">
                                                                            <asp:TextBox ID="txtCencosId" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-2">
                                                            <div class="form-horizontal">
                                                                <asp:Panel ID="rowBtnBuscar" runat="server" CssClass="form-group">
                                                                    <div class="col-sm-4">
                                                                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-keytia-sm" OnClick="btnBuscar_Click" />
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-8">
                                                            <asp:Panel ID="rowGuardar" runat="server" CssClass="form-group">
                                                                <div class="col-sm-6">
                                                                    <asp:Button ID="btnDescargar" runat="server" CssClass="btn btn-keytia-sm" Text="Descargar" />
                                                                </div>
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <div class="col-sm-4">
                                                                <asp:Panel ID="rowSociedad" runat="server" CssClass="form-group">
                                                                    <asp:Label ID="lblSociedad" runat="server" CssClass="col-sm-3 control-label">Sociedad:</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox ID="txtSociedad" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="rowDirecccion" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblDireccion" CssClass="col-sm-3 control-label">Dirección:</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtDireccion" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="rowArea" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblArea" CssClass="alinear col-sm-3 control-label">Area:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtArea" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="rowCencos" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblCencos" CssClass="col-sm-3 control-label">CenCosto:</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtCencos" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:Panel runat="server" ID="rowRespSociedad" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblRespSociedad" CssClass="col-sm-4 control-label">Responsable: </asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtRespSociedad" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="rowRespDireccion" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblRespDire" CssClass="col-sm-4 control-label">Responsable: </asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtRespDir" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="rowRespArea" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblRespArea" CssClass="col-sm-4 control-label">Responsable: </asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtRespArea" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="rowRespCencos" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblRespCeCos" CssClass="col-sm-4 control-label">Responsable: </asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtRespCeCos" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <div class="col-sm-offset-4 col-sm-6">
                                                                        <asp:Button runat="server" ID="btnEditar" CssClass="btn btn-keytia-sm" Text="Editar" OnClick="btnEditar_Click" />
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <div class="col-sm-offset-4 col-sm-6">
                                                                        <asp:Button runat="server" ID="btnGuardar" CssClass="btn btn-keytia-sm" Text="Guardar" OnClick="btnGuardar_Click" Visible="false" />
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblEmailSoc" CssClass="col-sm-3 control-label">Email:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtEmailSoc" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblEmailDir" CssClass="col-sm-3 control-label">Email:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtEmailDir" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblEmailArea" CssClass="col-sm-3 control-label">Email:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtEmailArea" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblEmailCeCos" CssClass="col-sm-3 control-label">Email:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtEmailCeCos" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <div class="col-sm-4">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblAutN1" CssClass="col-sm-5 control-label">Autorizador N1:</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtAut1" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblAutN2" CssClass="col-sm-5 control-label">Autorizador N2:</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtAut2" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblAutN3" CssClass="col-sm-5 control-label">Autorizador N3:</asp:Label>
                                                                    <div class="col-sm-10">
                                                                        <asp:TextBox runat="server" ID="txtAut3" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>
            <%--NZ: Modal para mensajes--%>
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
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnBuscar" />
        </Triggers>

    </asp:UpdatePanel>
</asp:Content>
