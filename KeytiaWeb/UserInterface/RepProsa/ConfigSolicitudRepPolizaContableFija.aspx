<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConfigSolicitudRepPolizaContableFija.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepProsa.ConfigSolicitudRepPolizaContableFija" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        table caption {
            text-align:center;
        }
    </style>
    <script type="text/javascript">
        function VerificarCampos() {
            let flg = false;
            let txtFlg = "Debe llenar todos los campos.";
            //const txtDescripcion = document.querySelector("#txtDescripcion").value;
            const txtPeriodo = document.querySelector("#txtPeriodo").value;
            const txtDestinatario = document.querySelector("#txtDestinatario").value;

            if (txtPeriodo != "" && txtDestinatario != "") {
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
                        <span class="caption-subject titlePortletKeytia">Solicitud de póliza contable telefonia fija</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" role="form">
                        <form method="post" class="form-horizontal" role="form">
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Periodo:</span>
                                <div class="col-sm-8">
                                    <input id="txtPeriodo" type="month" class="form-control" name="txtPeriodo" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Destinatario(s):</span>
                                <div class="col-sm-8">
                                    <input id="txtDestinatario" type="text" class="form-control" name="txtDestinatario" />
                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="panel-body">
                        <div class="row" runat="server" id="rowGrv">
                            <div class="col-sm-12">
                                <div class="clearfix"></div>
                                <div class="table-responsive">
<%--                                    <asp:ScriptManager ID="ScriptManager1" runat="server" />
                                    <asp:UpdatePanel runat="server" ID="updPanel">
                                        <ContentTemplate>--%>
                                            <asp:GridView runat="server" ID="grvFacturas"
                                        HeaderStyle-CssClass="tableHeaderStyle" DataKeyNames="iCodRegistro"
                                        CssClass="table table-bordered tableDashboard" HeaderStyle-Font-Bold="true"
                                        AutoGenerateColumns="false" OnRowEditing="grvFacturas_RowEditing" OnRowCancelingEdit="grvFacturas_RowCancelingEdit" OnRowUpdating="grvFacturas_RowUpdating" Caption="Bestel" CaptionAlign="Top">
                                        <columns>
                                            <asp:TemplateField HeaderText="Prorrateo">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkProrrateo" runat="server" Checked='<% # Convert.ToBoolean(Eval("BanderasFacturaPolizaContableTelFija")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Descripción">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescripcion" runat="server" Text='<% # Bind("vchDescripcion")%>'></asp:Label>
                                                </ItemTemplate>
                                                <%--<EditItemTemplate>
                                                    <asp:TextBox ID="txtDescripcion" runat="server" Text='<% # Bind("vchDescripcion") %>' />
                                                </EditItemTemplate>--%>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="DEBE">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDebe" runat="server" Text='<% # Bind("ImpDebe")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtDebe" runat="server" Text='<% # Bind("ImpDebe") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="HABER">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHaber" runat="server" Text='<% # Bind("ImpHaber")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtHaber" runat="server" Text='<% # Bind("ImpHaber") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="CC">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCC" runat="server" Text='<% # Bind("ProsaClaveCCOtrosGastos")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtCC" runat="server" Text='<% # Bind("ProsaClaveCCOtrosGastos") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="CIA">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCia" runat="server" Text='<% # Bind("Cia")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtCia" runat="server" Text='<% # Eval("Cia") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cuenta">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCuenta" runat="server" Text='<% # Bind("CuentaContable")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtCuenta" runat="server" Text='<% # Bind("CuentaContable") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Producto">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblProducto" runat="server" Text='<% # Bind("Producto")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtProducto" runat="server" Text='<% # Bind("Producto") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Temporal">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTemp" runat="server" Text='<% # Bind("Temp")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtTemp" runat="server" Text='<% # Bind("Temp") %>' />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:CommandField ButtonType="Link" ShowEditButton="true" />
                                        </columns>
                                    </asp:GridView>
<%--                                        </ContentTemplate>
                                    </asp:UpdatePanel>--%>
                                    
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12">
                                <div style="width: 400px; float: none; margin: 0 auto;">
                                    <asp:Panel ID="InfoPanelSucces" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <strong>
                                            <asp:Label ID="lblMensajeSuccess" runat="server"></asp:Label>
                                        </strong>
                                    </asp:Panel>
                                    <br />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-offset-5 col-sm-10">
                            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" OnClick="btnAceptar_Click" OnClientClick="return VerificarCampos();" />
                        </div>
                    </div>
                    <br />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
