<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConfigSolicitudRepTernium.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepTernium.ConfigSolicitudRepTernium" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
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
                        <span class="caption-subject titlePortletKeytia">Reporte Ternium</span>
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
                                    <input id="txtClave" type="text" class="autosuggest placeholderstile form-control" name="txtClave" />
                                </div>
                            </div>
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
                                <span class="col-sm-2 control-label">Empresa:</span>
                                <div class="col-sm-8">
                                    <asp:DropDownList ID="ddlEmpresa" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <span class="col-sm-2 control-label">Archivo:</span>
                                <div class="col-sm-8">
                                    <asp:FileUpload runat="server" ID="fileArchivo" ClientIDMode="Static" />
                                </div>
                                <asp:Label runat="server" ID="lblEstatusFileInventarioUnidadNegocio" Text="" Visible="false"></asp:Label>
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
