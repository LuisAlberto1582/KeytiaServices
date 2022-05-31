<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConfigCargaLineaEmpleUnidadNegCenCos.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepProsa.ConfigCargaLineaEmpleUnidadNegCenCos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        function VerificarCampos() {
            let flg = false;
            let txtFlg = "Debe llenar todos los campos.";
            const txtClave = document.querySelector("#txtClave").value;
            const txtDescripcion = document.querySelector("#txtDescripcion").value;
            const txtPeriodo = document.querySelector("#txtPeriodo").value;
            const fileArchivo = document.querySelector("#fileInventarioLineas");
            const fileArchivo2 = document.querySelector("#fileInventarioUnidadNegocio");
            const fileArchivo3 = document.querySelector("#fileFTES");
            if (txtClave != "" && txtDescripcion != "" && txtPeriodo != "" && fileArchivo.files.length != 0 && fileArchivo2.files.length != 0 && fileArchivo3.files.length) {
                flg = true;
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
                        <span class="caption-subject titlePortletKeytia">Carga elementos y relaciones inventario líneas</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" role="form">
                        <form method="post" class="form-horizontal" role="form">
                            <asp:Panel ID="divMensaje" runat="server" Visible="false">
                                <div class="form-group">
                                    <div class="col-sm-offset-3 col-sm-5">
                                        <div id="" class="alert alert-danger text-center">
                                            <asp:Label ID="lblMensaje" runat="server" /></div>
                                    </div>
                                </div>
                            </asp:Panel>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Clave:</span>
                                <div class="col-sm-8">
                                    <input id="txtClave" type="text" class="autosuggest placeholderstile form-control" name="txtClave" maxlength="40" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Descripcion:</span>
                                <div class="col-sm-8">
                                    <input id="txtDescripcion" type="text" class="autosuggest placeholderstile form-control" name="txtDescripcion" maxlength="40" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Periodo:</span>
                                <div class="col-sm-8">
                                    <input id="txtPeriodo" type="month" class="autosuggest placeholderstile form-control" name="txtPeriodo" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Inventario líneas:</span>
                                <div class="col-sm-8">
                                    <asp:FileUpload runat="server" ID="fileInventarioLineas" ClientIDMode="Static" />
                                </div>
                                <asp:Label runat="server" ID="lblEstatusFileInventarioLineas" Text="" Visible="false"></asp:Label>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Inventario unidad de negocio:</span>
                                <div class="col-sm-8">
                                    <asp:FileUpload runat="server" ID="fileInventarioUnidadNegocio" ClientIDMode="Static" />
                                </div>
                                <asp:Label runat="server" ID="lblEstatusFileInventarioUnidadNegocio" Text="" Visible="false"></asp:Label>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Archivo FTEs:</span>
                                <div class="col-sm-8">
                                    <asp:FileUpload runat="server" ID="fileFTES" ClientIDMode="Static" />
                                </div>
                                <asp:Label runat="server" ID="lblEstatusFileFTES" Text="" Visible="false"></asp:Label>
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
