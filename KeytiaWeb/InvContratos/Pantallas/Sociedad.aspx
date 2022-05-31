<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Sociedad.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.Sociedad" %>


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
    <%-- <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
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
                            <asp:TextBox ID="txtBuscar" PlaceHolder="Búsqueda" runat="server" CssClass="form-control" Style="height: 33px;" PlaceHoler="Busqueda"/>
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
                <h3>Sociedades</h3>
            </div>
            <div class="panel-body">
                <asp:TextBox ID="txtIdSociedad" Visible="false" runat="server"> </asp:TextBox>

                <div class="row col-lg-12">
                    <asp:GridView
                        ID="gvSociedad"
                        DataKeyNames="Id,Clave,Nombre,Activo,sActivo,DtIniVigencia,DtFinVigencia"
                        AutoGenerateColumns="false"
                        CssClass="table text-center"
                        GridLines="None"
                        AllowPaging="false"
                        AllowSorting="true"
                        runat="server"
                        OnRowCommand="gvSociedad_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="" Visible="false" />
                            <asp:BoundField DataField="Clave" HeaderText="Clave" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Activo" HeaderText="" Visible="false" />
                            <asp:BoundField DataField="sActivo" HeaderText="Estatus" />
                            <asp:BoundField DataField="DtIniVigencia" HeaderText="Inicio vigencia" />
                            <asp:BoundField DataField="DtFinVigencia" HeaderText="Fin vigencia" />
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
            <br />
            <br />
            <div class="form-inline">
                <asp:TextBox ID="txtClaveSociedad" runat="server" CssClass="form-control" PlaceHolder="Clave"></asp:TextBox>
                <asp:TextBox ID="txtNombreSociedad" runat="server" CssClass="form-control" PlaceHolder="Nombre"></asp:TextBox>
                <asp:DropDownList ID="DpdEstatusSociedad" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                    <asp:ListItem Value="0">Estatus..</asp:ListItem>
                    <asp:ListItem Value="1">Activo</asp:ListItem>
                    <asp:ListItem Value="2">Inactivo</asp:ListItem>
                </asp:DropDownList>
                <asp:LinkButton ID="InsertaSociedad" runat="server" CssClass="btn btn-default form-control" OnClick="InsertaSociedad_Click"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                </asp:LinkButton>
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
                            <h3>Sociedad</h3>
                        </div>
                        <div class="panel-body">
                            <asp:TextBox runat="server" ID="txtIdSociedadModal" Visible="false" CssClass="form-control"></asp:TextBox>
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-2">
                                        <asp:Label runat="server" ID="lblClaveSociedadModal" Text="Clave: " CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox runat="server" ID="txtClaveSociedadModal" CssClass="form-control"> </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-2">
                                        <asp:Label runat="server" ID="lblNombreSociedadModal" Text="Nombre: " CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:TextBox runat="server" ID="txtNombreSociedadModal" CssClass="form-control"> </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-2">
                                        <asp:Label ID="lblEstatusModal" Text="Estatus: " runat="server" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList ID="DpdEstatusModal" DataValueField="Id" runat="server" Width="170" AppendDataBoundItems="true" CssClass="form-control">
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
                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-keytia-sm" OnClick="btnAceptar_Click" />
                    <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-keytia-sm" Text="Cerrar" />
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
