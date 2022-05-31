<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="CategoriaElemento.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.CategoriaElemento" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--      <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="~InvContratos/Scripts/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>--%>

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
    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <asp:LinkButton ID="btnRegresar" runat="server" OnClick="btnRegresar_Click" CssClass="btn btn-default btn-circle btnBackDetail"><i class="far fa-arrow-alt-circle-left"></i></asp:LinkButton>
                        <asp:Panel ID="pnlMapaNavegacion" runat="server">
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%" CssClass="row">


        <asp:Panel ID="pnlBody" runat="server">
            <div class="formulario" style="width: 100%;" id="divForm" runat="server">

                <%-- BUSQUEDA GENERAL --%>
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12 ">
                        <div class="col-sm-2 col-md-2 col-lg-2">
                        </div>
                        <div class="col-sm-8 col-md-8 col-lg-8 ">
                            <div class="input-group">
                                <asp:TextBox ID="txtBuscar" PlaceHolder="Búsqueda" runat="server" CssClass="form-control" Style="height: 33px;" />
                                <asp:LinkButton runat="server" ID="lnkBuscar" CssClass="btn btn-keytia-sm input-group-btn" OnClick="lnkBuscar_Click" Style="height: 33px;">
                               <span class="glyphicon glyphicon-search" ></span>
                                </asp:LinkButton>
                            </div>
                        </div>
                        <div class="col-sm-2 col-md-2 col-lg-2">
                        </div>
                    </div>
                </div>
                <br />
                <br />


                <div class="container">
                    <div class="panel-default">
                        <div class="panel-heading">
                            <h3>Categorías elementos</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row" style="width: 100%; height: 320px; padding: 10px; overflow: scroll;">
                                <div class="col-lg-12">
                                    <asp:GridView
                                        ID="gvCategoria"
                                        DataKeyNames="Id,Nombre,Activo,sActivo"
                                        AutoGenerateColumns="false"
                                        CssClass="table text-center"
                                        GridLines="None"
                                        AllowSorting="false"
                                        runat="server"
                                        OnRowCommand="gvCategoria_RowCommand"
                                        Style="width: 100%;">
                                        <Columns>
                                            <asp:BoundField DataField="Id" HeaderText="" Visible="false" />
                                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                            <asp:BoundField DataField="sActivo" HeaderText="Estatus" />
                                            <asp:BoundField DataField="DtIniVigencia" HeaderText="Inicio Vigencia" />
                                            <asp:BoundField DataField="DtIniVigencia" HeaderText="Fin Vigencia" />
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
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-inline">
                                <asp:TextBox ID="txtNombreCategoria" PlaceHolder="Nombre" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:DropDownList ID="dpdEstatus" DataValueField="Id" runat="server" Width="170" AppendDataBoundItems="true" CssClass="form-control">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                    <asp:ListItem Value="1">Activo</asp:ListItem>
                                    <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                </asp:DropDownList>
                                <asp:LinkButton ID="ImgInsertaElementoContratado" runat="server" CssClass="btn btn-default form-control" OnClick="ImgInsertaElementoContratado_Click"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </asp:Panel>
    </asp:Panel>




    <%-- MODAL RM --%>
    <asp:Panel Style="display: none;" runat="server" CssClass="modalPopupEtq" ID="myModalCargar" Width="700px">
        <%--<div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px; font-size: 12px">
            <asp:Label runat="server" ID="lblTituloEditHallazo" Text="Cargar archivos"></asp:Label>
        </div>--%>

        <div class="modal-body  formulario">
            <div class="panel-default">
                <div class="panel-heading">
                    <h3>Categoría elemento</h3>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblIdCategoriaElementoModal" runat="server" CssClass="control-label" Visible="false"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:TextBox ID="txtIdCategoriaElementoModal" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblNombreCategoriaModal" runat="server" Text="Nombre: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:TextBox ID="txtNombreCategoriaElementoIdModal" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblEstatusModal" runat="server" CssClass="control-label" Text="Estatus: "></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:DropDownList ID="dpdEstatusModal" DataValueField="Id" runat="server" Width="170" AppendDataBoundItems="true" CssClass="form-control">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                    <asp:ListItem Value="1">Activo</asp:ListItem>
                                    <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>
        </div>


        <div class="footerEtq">
            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-default" OnClick="btnAceptar_Click" />
            <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-secondary" Text="Cerrar" />
        </div>
    </asp:Panel>
    <asp:LinkButton runat="server" ID="lnkAgregarNZ" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeCategoriaModal" runat="server" PopupControlID="myModalCargar"
        TargetControlID="lnkAgregarNZ" OkControlID="btnCerrarCarga" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>

    <%-- MODAL RM FIN --%>
</asp:Content>
