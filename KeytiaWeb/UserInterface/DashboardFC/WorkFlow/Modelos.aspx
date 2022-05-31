<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Modelos.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.Modelos" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .formulario {
            background-color: white;
            font-family: 'Poppins', sans-serif;
            padding: 10px;
            margin: 30px 15px;
            font-size: 14px;
        }

        .form-control {
            font-size: 15px;
        }

        .control-label {
            font-size: 14px;
            font-weight: bold;
        }

        .form-title {
            font-size: 14px;
        }

        .title-form, h4, h3 {
            font-weight: bold;
            font-size: 16px;
        }


        .contenedor {
            width: 100%;
            height: 200px;
            overflow: auto;
            border: 1px solid #808080;
            background-color: #E9E9E9;
            border-bottom-left-radius: 6px;
            border-top-left-radius: 6px;
            border-top-right-radius: 6px;
            border-bottom-right-radius: 6px;
            margin-bottom: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <div id="pnlMainHolder" runat="server" style="width: 100%;">
        <div id="pnlRow_0" style="width: 100%;" runat="server" cssclass="row">
            <div id="Rep0" style="width: 100%;" runat="server" cssclass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Modelos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" style="width: 100%;" id="RepDetallCollapse" role="form">
                            <div style="height: 320px; overflow: auto;">
                                <asp:GridView
                                    ID="gridModelos"
                                    DataKeyNames="MarcaID,MarcaClave,MarcaDesc,ModeloID,ModeloClave,ModeloDesc,ModeloIniVigencia,ModeloFinVigencia"
                                    AutoGenerateColumns="false"
                                    CssClass="table text-left"
                                    GridLines="None"
                                    AllowPaging="false"
                                    AllowSorting="true"
                                    runat="server"
                                    OnRowCommand="gridModelos_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="ModeloID" runat="Server" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="MarcaID" runat="Server" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="MarcaClave" runat="Server" HeaderText="Marca" />
                                        <asp:BoundField DataField="ModeloClave" runat="Server" HeaderText="Nombre" />
                                        <asp:BoundField DataField="ModeloDesc" runat="Server" HeaderText="Descripción" />
                                        <asp:BoundField DataField="ModeloIniVigencia" runat="Server" HeaderText="Fecha alta" />
                                        <asp:BoundField DataField="ModeloFinVigencia" runat="Server" HeaderText="Fin Vigencia" Visible="false" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDetalle" runat="server" Text="Editar" CommandName="Editar">
                                                                <span aria-hidden="true" class="glyphicon glyphicon-pencil"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEliminar" runat="server" Text="Eliminar" CommandName="Eliminar">
                                                                <span aria-hidden="true" class="glyphicon glyphicon-remove"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                                </asp:GridView>
                            </div>

                            <br />
                            <br />
                            <div class="form-inline" runat="server" id="rowForm">
                                <asp:DropDownList Width="200px" runat="server" CssClass="control-form" ID="ddlInsertMarca" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox runat="server" ID="txtInsertNombre" PlaceHolder="Nombre" CssClass="form-control"></asp:TextBox>
                                <asp:TextBox runat="server" ID="txtInsertDescripcion" PlaceHolder="Descripción" CssClass="form-control"></asp:TextBox>
                                <asp:LinkButton ID="ImgInsertaModelo" runat="server" CssClass="btn btn-default form-control" OnClick="ImgInsertaModelo_Click"> 
                                            <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


     <%-- MODAL RM --%>
    <asp:Panel Style="display: none;" runat="server" CssClass="modalPopupEtq formulario" ID="myModalCargar" Width="900px">
        <%--<div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px; font-size: 12px">
            <asp:Label runat="server" ID="lblTituloEditHallazo" Text="Cargar archivos"></asp:Label>
        </div>--%>

        <div class="modal-body  formulario">
            <div class="panel-default">
                <div class="panel-heading">
                    <h3>Modelos</h3>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12">
                            <asp:TextBox Visible="false" runat="server" ID="txtIDModeloModal"></asp:TextBox>
                            <asp:TextBox Visible="false" runat="server" ID="txtIDMarcaModal"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblMarcaClaveModal" runat="server" Text="Marca: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlMarcaModal" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblModeloClaveModal" runat="server" Text="Modelo: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:TextBox Width="130px" runat="server" CssClass="control-form"  ID="txtNombreModeloModal"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblDescModal" runat="server" Text="Descripcion: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:TextBox ID="txtDescModal" runat="server" Width="130px" CssClass="control-form"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>
        </div>


        <div class="footerEtq formulario">
            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-default" OnClick="btnAceptar_Click" />
            <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-secondary" Text="Cerrar" />
        </div>
    </asp:Panel>
    <asp:LinkButton runat="server" ID="lnkAgregarNZ" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="myModalCargar"
        TargetControlID="lnkAgregarNZ" OkControlID="btnCerrarCarga" BackgroundCssClass="modalPopupBackground">
    </asp:ModalPopupExtender>

    <%-- MODAL RM FIN --%>
</asp:Content>
