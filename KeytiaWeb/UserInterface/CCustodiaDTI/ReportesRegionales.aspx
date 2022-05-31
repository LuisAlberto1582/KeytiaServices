<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ReportesRegionales.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.ReportesRegionales" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 10999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            border-radius: 10px;
        }

        .center image {
            height: 128px;
            width: 128px;
        }

        .classButton {
            background-color: transparent;
            width: 0px;
            height: 0px;
            border: none;
            display: none;
        }
    </style>
    <script type="text/javascript">
        function Valida() {

            var fieldEmail = document.getElementById("<%=txtEmail.ClientID%>").value;
            var fieldaAsunto = document.getElementById("<%=txtAsunto.ClientID%>").value;

            if (fieldaAsunto != '' && fieldEmail != '') {
                var btn = document.getElementById("<%=btnYes.ClientID%>");
                btn.click();
            }
<%--            else {               
                if (fieldaAsunto == '') { document.getElementById("<%=rfvAsunto.ClientID%>").style.display = 'block'; }
                if (fieldEmail == '') { document.getElementById("<%=rfvEmail.ClientID%>").style.display = 'block' }
            }--%>

        }
    </script>
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);
            // Place here the first init of the autocomplete
            InitAutoCompl();
        });

        function InitializeRequest(sender, args) {
        }

        function EndRequest(sender, args) {
            // after update occur on UpdatePanel re-init the Autocomplete
            InitAutoCompl();
        }

        function InitAutoCompl() {

            $(function () {
                var idText = document.getElementById('<%=txtBuscar.ClientID%>');
                $(idText).keyup(function () {
                    var val = $(this).val().toUpperCase();
                    $('#RepDetallLineasMovil > tbody > tr').each(function (index, element) {
                        if ($(this).text().toUpperCase().indexOf(val) < 0)
                            $(this).hide();
                        else
                            $(this).show();
                    });
                });
            });

        };

    </script>
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
                    <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
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
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Reportes Regionales</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <br />
                                    <div class="row" runat="server" id="pnlInfo" Visible="false">
                                        <div style="width: 800px; float: none; margin: 0 auto;" >
                                            <asp:Panel CssClass="alert alert-info text-center" runat="server" role="alert" >
                                                <strong>
                                                    <asp:Label ID="lblMensajeInfo" runat="server"></asp:Label>
                                                </strong>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="row" id="row1" runat="server">
                                        <div class="col-sm-12">
                                            <div class="col-sm-6">
                                                <asp:Panel runat="server" ID="row" CssClass="form-group">
                                                    <asp:Label runat="server" CssClass="col-sm-5 control-label">Periodo Facturación: </asp:Label>
                                                    <div class="col-sm-4">
                                                        <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion"></asp:DropDownList>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <div class="col-sm-5">
                                                <asp:Label runat="server" CssClass="col-sm-2 control-label">Región: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:DropDownList runat="server" ID="cboRegion" CssClass="form-control" DataValueField="REGION" DataTextField="REGION"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row" runat="server" id="row2">
                                        <div class="col-sm-11">
                                            <div class="col-sm-3">
                                            </div>
                                            <div class="col-sm-3">
                                                <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                                    <div class="col-sm-6">
                                                        <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-lg" Text="Aceptar" CausesValidation="false" OnClick="btnAplicarFecha_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <div class="col-sm-3">
                                                <div class="col-sm-6">
                                                    <button type="button" id="btn2" data-toggle="modal" data-target="#exampleModal" class="btn btn-keytia-lg">solicitar</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row" id="row3" runat="server">
                                        <div class="col-sm-12">
                                            <div class="col-sm-6" runat="server" id="resumenGral">
                                            </div>

                                            <div class="col-sm-6" runat="server" id="telMovil">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <div class="col-sm-6" runat="server" id="telFija">
                                                </div>

                                                <div class="col-sm-6" runat="server" id="telInternet">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" runat="server" id="rowBuscarText" visible="false">
                                        <div class="col-sm-8">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                <div class="col-offset-2 col-sm-4">
                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="row4" runat="server" visible="false">
                                        <div class="col-sm-12">
                                            <div class="table-fixed-nz">
                                                <asp:Panel ID="pnl1" runat="server" CssClass="col-md-12 col-sm-12">
                                                </asp:Panel>
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
                            <div class="modal-footer">
                                <asp:Button ID="Button1" runat="server" Text="OK" CssClass="btn btn-keytia-sm" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
                    TargetControlID="lnkBtnMsn" OkControlID="Button1" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
                </asp:ModalPopupExtender>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnAplicarFecha" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnYes" />
        </Triggers>
    </asp:UpdatePanel>
    <div class="modal" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true"><i class="fas fa-times"></i></button>
                        <h4 class="modal-title" id="exampleModalLabel">Solicitud de Reporte Regional</h4>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <div class="col-sm-8">
                                        <asp:CheckBox runat="server" ID="chkTodos" Text="Generar Todos" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <asp:Label runat="server" ID="lblAsunto" CssClass="col-sm-3 control-label">Asunto: </asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox runat="server" ID="txtAsunto" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <asp:Label runat="server" ID="lblEmail" CssClass="col-sm-3 control-label">Destinatario: </asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer" id="footer">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-3">
                                </div>
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-5">
                                    <button type="button" class="btn btn-keytia-sm" onclick="Valida();">Guardar</button>
                                    &nbsp;&nbsp;&nbsp;
                                            <button type="button" class="btn btn-keytia-sm" data-dismiss="modal">cancelar</button>
                                    <asp:Button runat="server" ID="btnYes" CssClass="classButton" OnClick="btnYes_Click"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
