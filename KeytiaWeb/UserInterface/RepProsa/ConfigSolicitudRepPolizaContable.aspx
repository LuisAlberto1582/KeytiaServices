<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConfigSolicitudRepPolizaContable.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepProsa.ConfigSolicitudRepPolizaContable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        function VerificarCampos() {
            let flg = false;
            let txtFlg = "Debe llenar todos los campos.";
            const txtDescripcion = document.querySelector("#txtDescripcion").value;
            const txtPeriodo = document.querySelector("#txtPeriodo").value;
            const txtDestinatario = document.querySelector("#txtDestinatario").value;
            const chkRepPolizaContable = document.querySelector("#chkRepPolizaContable").checked;
            const chkRepExcedentes = document.querySelector("#chkRepExcedentes").checked;
            const chkRepGastosBAM = document.querySelector("#chkRepGastosBAM").checked;

            if (txtDescripcion != "" && txtPeriodo != "" && txtDestinatario != "") {
                flg = true;

                //Verificacion correo
                const regex = /^[-\w.%+]{1,64}@(?:[A-Z0-9-]{1,63}\.){1,125}[A-Z]{2,63}$/i;
                lstDestinatarios = txtDestinatario.split(",");
                for (let itm of lstDestinatarios) {
                    if (!regex.test(itm)) {
                        flg = false;
                        txtFlg = "Correo inválido.";
                        break;
                    }
                }

                //Seleccion de al menos un reporte
                if (chkRepPolizaContable == false && chkRepExcedentes == false && chkRepGastosBAM == false) {
                    flg = false;
                    txtFlg = "Debe seleccionar al menos un reporte.";
                }
            }

            if (flg == false) {
                alert(txtFlg);
            }
            return flg;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div class="row">
        <div class="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                        <span class="caption-subject titlePortletKeytia">Reportes prosa</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" role="form">
                        <form method="post" class="form-horizontal" role="form">
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Descripcion:</span>
                                <div class="col-sm-8">
                                    <input id="txtDescripcion" type="text" class="autosuggest placeholderstile form-control" name="txtDescripcion" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Periodo:</span>
                                <div class="col-sm-8">
                                    <input id="txtPeriodo" type="month" class="autosuggest placeholderstile form-control" name="txtPeriodo" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Generar:</span>
                                <div class="col-sm-8">
                                    <asp:CheckBox runat="server" ID="chkRepPolizaContable" Text="Reporte de póliza contable" CssClass="checkbox-inline" ClientIDMode="Static" />
                                    <asp:CheckBox runat="server" ID="chkRepExcedentes" Text="Reporte de excedenetes" CssClass="checkbox-inline" ClientIDMode="Static" />
                                    <asp:CheckBox runat="server" ID="chkRepGastosBAM" Text="Reporte de gastos BAM" CssClass="checkbox-inline" ClientIDMode="Static" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Destinatario:</span>
                                <div class="col-sm-8">
                                    <input id="txtDestinatario" type="text" class="autosuggest placeholderstile form-control" name="txtDestinatario" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Estatus:</span>
                                <div class="col-sm-8">
                                    <input type="text" class="autosuggest placeholderstile form-control" value="En espera de servicio" disabled />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-offset-5 col-sm-10">
                                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" OnClick="btnAceptar_Click" OnClientClick="return VerificarCampos();" />
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
