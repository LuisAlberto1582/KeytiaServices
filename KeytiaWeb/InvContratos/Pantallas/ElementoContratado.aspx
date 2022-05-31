<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ElementoContratado.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.ElementoContratado" %>


<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="~InvContratos/Scripts/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>

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
    <%--  <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
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
    </asp:Panel>--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">

    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%" CssClass="row">
        <div class="formulario col-sm-12 col-m-12 col-lg-12">
            <%-- BUSQUEDA GENERAL --%>
            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-12 ">
                    <div class="col-sm-2 col-md-2 col-lg-2">
                    </div>
                    <div class="col-sm-8 col-md-8 col-lg-8 ">
                        <div class="input-group">
                            <asp:TextBox ID="txtBuscar" PlaceHolder="Búsqueda" runat="server" CssClass="form-control" style="height:33px;"/>
                            <asp:LinkButton runat="server" ID="lnkBuscar" CssClass="btn btn-keytia-sm input-group-btn" OnClick="lnkBuscar_Click" style="height:33px;">
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
            <div class="col-sm-12 col-m-12 col-lg-12">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3>Elementos Contratados</h3>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:GridView
                                    ID="gvElementos"
                                    DataKeyNames="IdElemento,CategoriaElementoId,Nombre, Descripcion,ClaveCargo,sActivo,Activo,DtIniVigencia,DtFinVigencia,DtFecUltAct"
                                    AutoGenerateColumns="false"
                                    CssClass="table text-center"
                                    GridLines="None"
                                    AllowSorting="false"
                                    runat="server"
                                    OnRowCommand="gvElementos_RowCommand"
                                    Style="width: 100%; text-align: left;">
                                    <Columns>
                                        <asp:BoundField DataField="IdElemento" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="CategoriaElementoId" HeaderText="Categoria" Visible="false" />
                                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                        <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                        <asp:BoundField DataField="ClaveCargo" HeaderText="Clave Cargo" Visible="false" />
                                        <asp:BoundField DataField="SActivo" HeaderText="Estatus" />
                                        <asp:BoundField DataField="DtIniVigencia" HeaderText="Inicio Vigencia" />
                                        <asp:BoundField DataField="DtFinVigencia" HeaderText="Fin Vigencia" />
                                        <asp:BoundField DataField="DtFecUltAct" HeaderText="Fecha modificación" />
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
            </div>
            <br />
            <br />
            <hr />
            <div class="row">
                <div class="col-lg-12">
                    <div class="col-lg-12 form-inline">
                        <asp:DropDownList ID="DpdCategoria" DataTextField="Nombre" DataValueField="Id" runat="server" Width="160" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Categoria...</asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtNombreElemento" runat="server" PlaceHolder="Nombre" Width="135" CssClass="form-control"></asp:TextBox>
                        <asp:TextBox ID="txtDescripcionElemento" runat="server" PlaceHolder="Descripción" Width="135" CssClass="form-control"></asp:TextBox>
                        <asp:DropDownList ID="DpdClaveCargo" DataTextField="Nombre" DataValueField="Id" runat="server" Width="120" AppendDataBoundItems="true" CssClass="form-control" Visible="false">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="DpdActivo" DataTextField="Nombre" DataValueField="Id" runat="server" Width="120" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Estatus...</asp:ListItem>
                            <asp:ListItem Value="1">Activo</asp:ListItem>
                            <asp:ListItem Value="2">Inactivo</asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtxFechaInicio" runat="server" Width="100" PlaceHolder="aaaa/mm/dd" CssClass="form-control" Visible="false" />
                        <asp:TextBox ID="txtFechaFin" runat="server" Width="100" PlaceHolder="aaaa/mm/dd" CssClass="form-control" Visible="false" />
                        <asp:LinkButton ID="ImgInsertaElementoContratado" runat="server" CssClass="btn btn-default form-control" OnClick="ImgInsertaElementoContratado_Click"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>




    <%-- MODAL RM --%>
    <asp:Panel Style="display: none;" runat="server" CssClass="modal-Keytia" ID="myModalCargar" TabIndex="-1" role="dialog">

        <div class="modal-dialog modal-md">
            <div class="modal-content">

                <div class="modal-header">
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>

                <div class="rule"></div>
                <div class="modal-body">
                    <div class="panel-default">
                        <div class="panel-heading">
                            <h3>Elemento contratado</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lblIDElementoModal" runat="server" CssClass="control-label" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtIdElementoModal" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lblCategoriaModal" runat="server" CssClass="control-label" Text="Categoria: "></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:DropDownList ID="DpdCategoriaModal" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lblCategoriaElementoModal" runat="server" Text="Categoria: " CssClass="control-label" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="CategoriaElementoIdModal" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lblNombreModal" runat="server" Text="Nombre:" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtNombreModal" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lblDescripcionModal" runat="server" Text="Descripción: " CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtDescripcionModal" runat="server" CssClass="form-control"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lblClaveCargoModal" runat="server" CssClass="control-label" Text="clave crgo: " Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtClaveCargoModal" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-4">
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


                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-keytia-sm" OnClick="btnAceptar_Click" />
                            <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-keytia-sm" Text="Cerrar" />
                        </div>
                    </div>

                </div>
            </div>
        </div>


    </asp:Panel>
    <asp:LinkButton runat="server" ID="lnkAgregarNZ" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeInsertaSociedadModal" runat="server" PopupControlID="myModalCargar"
        TargetControlID="lnkAgregarNZ" OkControlID="btnCerrarCarga" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrar">
    </asp:ModalPopupExtender>

    <%-- MODAL RM FIN --%>
</asp:Content>
