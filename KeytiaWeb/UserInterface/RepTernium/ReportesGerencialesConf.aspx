<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ReportesGerencialesConf.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepTernium.ReportesGerencialesConf" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        function SeleccionarTodo(chkSeleccionarTodo) {
            const chksDirecciones = document.querySelectorAll("#grdDirecciones input[type=checkbox]");
            if (chkSeleccionarTodo.checked) {
                for (let itm of chksDirecciones) {
                    itm.checked = true;
                }
            }
            else {
                for (let itm of chksDirecciones) {
                    itm.checked = false;
                }
            }
        }

        function ValidarCampos() {
            let flg = false;
            let txtFlg = "Debe llenar todos los campos.";
            const txtClave = document.querySelector("#txtClave").value;
            const txtDestinatario = document.querySelector("#txtDestinatario").value;
            const ddlAnio = document.querySelector("#ddlAnio").value;
            const ddlMes = document.querySelector("#ddlMes").value;
            const ddlPais = document.querySelector("#ddlPais").value;
            if (txtClave != "" && txtDestinatario != "" && ddlAnio != "" && ddlMes != "" && ddlPais != "") {
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
                        <span class="caption-subject titlePortletKeytia">Reportes gerenciales</span>
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
                                            <asp:Label ID="lblMensaje" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Clave:</span>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtClave" CssClass="form-control" runat="server" ClientIDMode="Static" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Año:</span>
                                <div class="col-sm-3">
                                    <asp:DropDownList ID="ddlAnio" runat="server" CssClass="form-control" ClientIDMode="Static" />
                                </div>
                                <span class="col-sm-2 control-label">Mes:</span>
                                <div class="col-sm-3">
                                    <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-control" ClientIDMode="Static" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Pais:</span>
                                <div class="col-sm-8">
                                    <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPais_SelectedIndexChanged" AutoPostBack="true" ClientIDMode="Static" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Direcciones:</span>
                                <div class="col-sm-8">
                                    <div style="width:100%; height:300px; overflow:auto;">
                                        <asp:GridView ID="grdDirecciones" runat="server" AutoGenerateColumns="false" ShowHeader="false" ClientIDMode="Static">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkDireccion" runat="server" />
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                <asp:BoundField DataField="iCodCatalogo" />
                                                <asp:BoundField DataField="vchDescripcion" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <asp:CheckBox ID="chkSeleccionatTodos" runat="server" Text="Seleccionar todos" ClientIDMode="Static" CssClass="checkbox-inline" onclick="return SeleccionarTodo(this);" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-6">
                                    <asp:CheckBox ID="chkGenerarConsolidado" runat="server" Text="Generar consolidado" ClientIDMode="Static" CssClass="checkbox-inline" />
                                </div>
                                <div class="col-sm-6">
                                    <span class="control-label">Moneda en que se desea generar el reporte:</span>
                                    <br />
                                    <asp:RadioButton runat="server" Text="Moneda local" GroupName="Moneda" ID="radMonedaLocal" />
                                    <asp:RadioButton runat="server" Text="Dólares" GroupName="Moneda" ID="radDolares" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Destinatario:</span>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDestinatario" CssClass="form-control" runat="server" name="txtDestinatario" ClientIDMode="Static" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-offset-4 col-sm-10">
                                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" OnClick="btnAceptar_Click" OnClientClick="return ValidarCampos();"/>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
