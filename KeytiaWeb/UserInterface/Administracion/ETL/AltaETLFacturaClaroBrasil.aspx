<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AltaETLFacturaClaroBrasil.aspx.cs" Inherits="KeytiaWeb.UserInterface.Administracion.ETL.AltaETLFacturaClaroBrasil" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="CSS/StyleFile.css" rel="stylesheet" />
    <script type="text/javascript">
        function redirect() {
            sessionStorage.clear();
            window.location.replace("ListadoCargasETLClaroBrasil.aspx");
            return false;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            var icodCarga = sessionStorage.getItem("icodCargaClaro");
            var claveCarga = sessionStorage.getItem("claveCargaClaro");
            if (icodCarga !== null && claveCarga !== null) {
                var field = document.getElementById("<%=txtClave.ClientID%>");
                document.getElementById("<%=icodCarga.ClientID%>").value = icodCarga;
                field.value = claveCarga;
                field.disabled = true;
                document.getElementById("<%=rowEmpresa.ClientID%>").style.display = "none";
                document.getElementById("<%=rowMoneda.ClientID%>").style.display = "none";
                document.getElementById("<%=rowMes.ClientID%>").style.display = "none";
                document.getElementById("<%=rowAnio.ClientID%>").style.display = "none";
                document.getElementById("<%=colFiles.ClientID%>").style.display = "none";
            }
        });
    </script>
    <script type="text/javascript">
        function deshabilitaBtn() {
            var boton = document.getElementById("<%=btnAceptar.ClientID%>");
            boton.disabled = true;
            document.getElementById("<%=btnBack.ClientID%>").disabled =true
        }
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
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Alta de proceso ETL Factura Claro Brasil</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="panel panel-default">
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <div class="col-sm-2">
                                                    </div>
                                                    <div class="col-sm-6">
                                                        <div class="form-group">
                                                            <asp:Label ID="lblClave" runat="server" CssClass="col-sm-4 control-label">Clave:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" class="form-control" ID="txtClave" value="" MaxLength="40"></asp:TextBox>
                                                                <asp:HiddenField runat="server" ID="icodCarga" />
                                                            </div>
                                                        </div>

                                                        <div class="form-group" runat="server" id="rowEmpresa">
                                                            <asp:Label for="lblEmpresa" runat="server" CssClass="col-sm-4 control-label">Empresa:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" class="dropdown form-control" ID="ddlEmpresa" value=""></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="form-group" runat="server" id="rowMoneda">
                                                            <asp:Label for="lblMoneda" runat="server" CssClass="col-sm-4 control-label">Moneda:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" class="dropdown form-control" ID="ddlMoneda" value=""></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="form-group" runat="server" id="rowAnio">
                                                            <asp:Label for="lblAnio" runat="server" CssClass="col-sm-4 control-label">Año publicación:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" class="dropdown form-control" ID="ddlAnio" value=""></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="form-group" runat="server" id="rowMes">
                                                            <asp:Label for="lblMes" runat="server" CssClass="col-sm-4 control-label">Mes publicación:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" class="dropdown form-control" ID="ddlMes" value=""></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <div class="col-sm-4" runat="server" id="colFiles">
                                                        <asp:Label ID="lblSeleccionArchivos" runat="server" CssClass="control-label"><strong>Selección de archivos:</strong></asp:Label>
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblArchivo1" Text="Archivo 1"></asp:Label><asp:FileUpload runat="server" ID="fuClaroFile1" /><asp:Label runat="server" ID="lblEstatusArchivo01" Text="" Visible="false"></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <asp:Label ID="lblOpcionesChk01" runat="server" CssClass="control-label"><strong>Configuración de carga:</strong></asp:Label>
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div>
                                                                    <asp:CheckBox runat="server" ID="chkCargaLineasNoReg" Text="Publica líneas sin identificar" Checked="true" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <asp:Label ID="lblSeleccionProcesos" runat="server" CssClass="control-label"><strong>Selección de procesos:</strong></asp:Label>
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div>
                                                                    <asp:CheckBox runat="server" ID="chkSubirInfo" Text="Subir información hacia BD" Checked="true" />
                                                                </div>
                                                                <div>
                                                                    <asp:CheckBox runat="server" ID="chkActualizaLineas" Text="Actualizar líneas y atributos" Checked="true" />
                                                                </div>
                                                                <div>
                                                                    <asp:CheckBox runat="server" ID="chkGeneraDetalle" Text="Generar información de detalle" Checked="true" />
                                                                </div>
                                                                <div>
                                                                    <asp:CheckBox runat="server" ID="chkGeneraResumenes" Text="Generar información de resumenes" Checked="true" />
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Table ID="trGrabar" runat="server" Width="100%" Visible="true">
                                            <asp:TableRow ID="tblBotones" runat="server">
                                                <asp:TableCell ID="tblBotonesC2" runat="server" HorizontalAlign="Center">
                                                    <asp:Panel ID="pnlAlerta" runat="server" CssClass="alert alert-success" Visible="false">
                                                        <button type="button" class="close" data-dismiss="alert" aria-label="Close" aria-hidden="true">
                                                            x
                                                        </button>
                                                        <span>
                                                            <asp:Label runat="server" ID="lblMensajeConfirma" Text="Elemento registrado correctamente!" Visible="false"></asp:Label>
                                                        </span>
                                                    </asp:Panel>
                                                    <br />
                                                    <div class="col-sm-6">
                                                        <asp:Panel ID="rowRegresar" runat="server" CssClass="form-group" Visible="true">
                                                            <div class="col-sm-offset-6 col-sm-8">
                                                                <asp:Button runat="server" ID="btnBack" OnClientClick="redirect();" class="btn btn-keytia-sm" Text="Regresar"></asp:Button>
                                                                &nbsp;&nbsp;&nbsp;
                                                            <asp:Button CssClass="btn btn-keytia-sm" runat="server" ID="btnAceptar" Text="Aceptar" OnClientClick="deshabilitaBtn();" UseSubmitBehavior="false" OnClick="btnAceptar_Click"></asp:Button>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
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
            <asp:PostBackTrigger ControlID="btnAceptar" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>