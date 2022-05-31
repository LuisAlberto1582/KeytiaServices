<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="UploadFiles.aspx.cs" Inherits="KeytiaWeb.UserInterface.UploadFiles" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
            /*background-color: Black;
    filter: alpha(opacity=60);
    opacity: 0.6;
    -moz-opacity: 0.8;*/
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            /*background-color: White;
    border-radius: 10px;
    filter: alpha(opacity=100);
    opacity: 1;
    -moz-opacity: 1;*/
        }

        .center img {
            height: 120px;
            width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
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
                                    <span class="caption-subject titlePortletKeytia">Carga de Archivos</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:Panel ID="rowArchivo" runat="server" CssClass="form-group">
                                                <asp:Label ID="lblArchivo" runat="server" CssClass="col-sm-2 control-label">Archivo:</asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:FileUpload ID="FileUploadControl" CssClass="" runat="server" />
                                                    <asp:RequiredFieldValidator ErrorMessage="Debe Seleccionar Un archivo" ControlToValidate="FileUploadControl"
                                                        runat="server" ForeColor="Red"></asp:RequiredFieldValidator>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="rowDescripcion" runat="server" CssClass="form-group">
                                                <asp:Label ID="lblDescripcion" runat="server" CssClass="col-sm-2 control-label">Notas:</asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control">
                                                    </asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <div runat="server" id="CargaArchivo">
                                                <asp:Panel ID="rowCategoria" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblCategoria" runat="server" CssClass="col-sm-2 control-label">Categoria: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList runat="server" ID="cboCategoria" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="NombreCategoria" CssClass="col-sm-2 form-control">
                                                            <asp:ListItem Value="0"> Seleciona una Categoria </asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowAnio" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblAnio" runat="server" CssClass="col-sm-2 control-label">Año: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList runat="server" ID="cboAnio" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control"></asp:DropDownList>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowMes" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblMes" runat="server" CssClass="col-sm-2 control-label">Mes: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList runat="server" ID="cboMes" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control"></asp:DropDownList>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                                <div class="col-sm-offset-5 col-sm-10">
                                                    <asp:Button ID="UploadButton" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" OnClick="UploadButton_Click" />
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="rowLabel" runat="server" CssClass="form-group">
                                                <div class="col-sm-offset-4 col-sm-10">
                                                    <asp:Label runat="server" ID="StatusLabel" Font-Size="Large" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        </di>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="UploadButton" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        window.onsubmit = function () {
            if (Page_IsValid) {
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            }
        }
    </script>
</asp:Content>
