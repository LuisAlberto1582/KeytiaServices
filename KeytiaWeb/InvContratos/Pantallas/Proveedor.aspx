<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Proveedor.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.Proveedor" %>

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
    <%--<asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
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

        <div class="panel-default formulario" style="width: 100%">

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


            <div class="panel-heading">
                <h3>Proveedores</h3>
            </div>
            <div class="panel-body">
                <div style="width: 100%; height: 320px; overflow: auto" class="row">
                    <asp:GridView
                        ID="gvProveedores"
                        DataKeyNames="
                                        IdProveedor,
                                        RazonSocial,
                                        Nombre,
                                        IdPais,
                                        NoProveedorSAP,
                                        IdContacto,
                                        NombreContacto,
                                        CorreoElectronico,
                                        TelefonoExtension
                                    "
                        AutoGenerateColumns="false"
                        CssClass="table text-center"
                        GridLines="None"
                        AllowPaging="false"
                        AllowSorting="true"
                        runat="server"
                        OnRowCommand="gvProveedores_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="IdProveedor" HeaderText="" Visible="false" />
                            <asp:BoundField DataField="RazonSocial" HeaderText="Razon Social" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="IdPais" HeaderText="" Visible="false" />
                            <asp:BoundField DataField="Pais" HeaderText="País" />
                            <asp:BoundField DataField="NoProveedorSAP" HeaderText="No. Proveedor SAP" />
                            <asp:BoundField DataField="IdContacto" HeaderText="" Visible="false" />
                            <asp:BoundField DataField="NombreContacto" HeaderText="Nombre Contacto" />
                            <asp:BoundField DataField="CorreoElectronico" HeaderText="Correo Electronico" />
                            <asp:BoundField DataField="TelefonoExtension" HeaderText="Telefono Extensión" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDetalle" runat="server" Text="Editar" CommandName="EditarProveedor">
                                        <span aria-hidden="true" class="glyphicon glyphicon-pencil"></span>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEliminar" runat="server" Text="Eliminar" CommandName="EliminarProveedor">
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
                <div style="width: 100%;" class="row form-inline">
                    <asp:TextBox ID="txtRazonSocial" runat="server" Width="156" CssClass="form-control" PlaceHolder="Razón Social"></asp:TextBox>
                    <asp:TextBox ID="txtNombre" runat="server" Width="135" CssClass="form-control" PlaceHolder="Nombre"></asp:TextBox>
                    <asp:DropDownList ID="DpdRegion" DataTextField="Nombre" DataValueField="Id" runat="server" Width="170" AppendDataBoundItems="true" CssClass="form-control">
                        <asp:ListItem Value="0">Pais</asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtNumeroProveedorSAP" runat="server" Width="155" CssClass="form-control" PlaceHolder="Proveedor SAP"></asp:TextBox>
                    <asp:TextBox ID="txtContacto" runat="server" Width="130" CssClass="form-control" PlaceHolder="Contacto"></asp:TextBox>
                    <asp:TextBox ID="txtCorreo" runat="server" Width="130" CssClass="form-control" PlaceHolder="Correo"></asp:TextBox>
                    <asp:TextBox ID="txtTelefono" runat="server" Width="160" CssClass="form-control" PlaceHolder="Telefono"></asp:TextBox>
                    <asp:LinkButton ID="InsertaProveedores" runat="server" CssClass="btn btn-default form-control" OnClick="InsertaProveedor"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                    </asp:LinkButton>
                </div>
            </div>

        </div>

    </asp:Panel>



    <%-- MODAL RM --%>
    <asp:Panel Style="display: none;" runat="server" CssClass="modal-Keytia" ID="myModalCargar" TabIndex="-1" role="dialog">
        <div class="rule"></div>
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>

                <div class="modal-body  formulario">
                    <div class="panel-default">
                        <div class="panel-heading">
                            <h3>Edición proveedor</h3>
                        </div>

                        <div id="formEdicionProveedor" class="panel-body row">
                            <asp:TextBox ID="txtIdProveedorModal" runat="server" Visible="false"></asp:TextBox>

                            <div class="row">
                                <div class="col-lg-12" style="margin-top: 20px;">
                                    <div class="col-lg-2">
                                        <asp:Label ID="lblNombreModal" runat="server" Text="Nombre: " CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="txtNombreModal" runat="server" CssClass="text-left pull-left form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-2">
                                        <asp:Label ID="lblRazonSocialModal" runat="server" Text="Razón Social: " CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="txtRazonSocialModal" runat="server" CssClass="text-left pull-left form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12" style="margin-top: 20px">
                                    <div class="col-lg-2">
                                        <asp:Label ID="lblNumeroProveedorSAPModal" runat="server" Text="No. de proveedor SAP:" CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="TextBoxtxtNomProvSapModal" runat="server" CssClass="text-left pull-left form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:Label ID="lblPaisModal" runat="server" Text="País:" CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList ID="DpdPaisModal" DataTextField="Nombre" DataValueField="Id" runat="server" Width="170" AppendDataBoundItems="true" CssClass="form-control">
                                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                            <asp:ListItem Value="1">México</asp:ListItem>
                                            <asp:ListItem Value="2">Estados Unidos</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel-default">
                        <div class="panel-heading">
                            <h3>Edición Contacto</h3>
                        </div>
                        <div id="formEdicionContacto" class="panel-body row">
                            <asp:TextBox ID="txtIdContactoModal" runat="server" CssClass="text-left pull-left form-control" Visible="false"></asp:TextBox>

                            <div class="row">
                                <div class="col-lg-12" style="margin-top: 20px">
                                    <div class="col-lg-2">
                                        <asp:Label ID="lblNombreContactoModal" runat="server" Text="Nombre" CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="txtNombrecontactoModal" runat="server" CssClass="text-left pull-left form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:Label ID="lblcorreoContactoModal" runat="server" Text="Correo:" CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="txtCorreoContactoModal" runat="server" CssClass="text-left pull-left form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12" style="margin-top: 20px">
                                    <div class="col-lg-2">
                                        <asp:Label ID="lblTelefonoContactoModal" runat="server" Text="Teléfono:" CssClass="text-right control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="txtTelefonoContactoModal" runat="server" CssClass="text-left pull-left form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-keytia-sm" OnClick="btnAceptar_Click" />
                    <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-keytia-sm" Text="Cerrar" />
                </div>
            </div>
        </div>



    </asp:Panel>
    <asp:LinkButton runat="server" ID="lnkAgregarNZ" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeCargaArchivo" runat="server" PopupControlID="myModalCargar"
        TargetControlID="lnkAgregarNZ" OkControlID="btnCerrarCarga" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrar">
    </asp:ModalPopupExtender>

    <%-- MODAL RM FIN --%>
</asp:Content>
