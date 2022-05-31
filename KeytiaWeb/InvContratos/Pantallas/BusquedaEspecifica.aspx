<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="BusquedaEspecifica.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.BusquedaEspecifica" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--    <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="~InvContratos/Scripts/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>--%>

    <style type="text/css">
        .mrgbot {
            margin-bottom: 15px;
        }

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
    <asp:Panel ID="pnlMainHolder" runat="server" CssClass="row">
        <div class="formulario  col-sm-12 col-md-12 col-lg-12">
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

            <div class="container col-sm-12 col-md-12 col-lg-12 " runat="server">
                <h4>Búsqueda de contrato, anexo y convenio</h4>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <!--FOLIO-->
                        <asp:Label ID="lblFolio" runat="server" Text="Folio:" Enabled="false" BackColor="Transparent" CssClass="control-label"></asp:Label>
                        <asp:TextBox ID="txtFolio" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- CLAVE --%>
                        <asp:Label ID="lblClave" runat="server" Text="Clave:" CssClass="control-label"></asp:Label>
                        <asp:TextBox ID="txtClave" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- CUENTA CONTABLE --%>
                        <asp:Label ID="lblCuentaContable" runat="server" Text="Cuenta contable:" CssClass="control-label"></asp:Label>
                        <asp:TextBox ID="txtCuentaContable" runat="server" CssClass="text-left form-control"> </asp:TextBox>
                    </div>

                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- PROVEEDOR --%>
                        <asp:Label ID="lblProveedor" runat="server" Text="Proveedor:" CssClass="control-label"></asp:Label>
                        <asp:DropDownList ID="dpdProveedor" DataTextField="Nombre" DataValueField="Id" runat="server" AutoPostBack="true" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- CATEGORIA --%>
                        <asp:Label ID="lblCategoria" runat="server" Text="Categoría:" CssClass="control-label"></asp:Label>
                        <asp:DropDownList ID="dpdCategoria" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- CATEGORIA SERVICIO --%>
                        <asp:Label ID="lblCategoriaServicio" runat="server" Text="Categoría de servicio:" CssClass="text-right control-label "></asp:Label>
                        <asp:DropDownList ID="dpdCategoriaServicio" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- ESTATUS --%>
                        <asp:Label ID="lblEstatus" runat="server" Text="Estatus:" CssClass="text-right control-label"></asp:Label>
                        <asp:DropDownList ID="DpdEstatus" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%--DESCRIPCION--%>
                        <asp:Label ID="lblDescripcion" runat="server" Text="Descripcion:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- NOMBRE DEL CONTRATO --%>
                        <asp:Label ID="lblContacto" runat="server" Text="Nombre del contacto:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtContacto" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- MAIL DEL CONTRATO --%>
                        <asp:Label ID="lblMail" runat="server" Text="Mail del contacto:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtMail" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- NOMBRE DEL ARCHIVO --%>
                        <asp:Label ID="lblArchivo" runat="server" Text="Nombre del archivo:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtArchivo" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- NOMBRE DEL COMPRADOR --%>
                        <asp:Label ID="lblComprador" runat="server" Text="Nombre del comprador:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtComprador" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- MAIL DEL COMPRADOR --%>
                        <asp:Label ID="lblMailComprador" runat="server" Text="Mail del comprador:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtMailComprador" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- NOMBRE DEL SOLICITANTE --%>
                        <asp:Label ID="lblSolicitante" runat="server" Text="Nombre del solicitante:" CssClass="control-label text-right"></asp:Label>
                        <asp:TextBox ID="txtSolicitante" runat="server" CssClass="text-left  form-control"> </asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- AREA --%>
                        <asp:Label ID="lblArea" runat="server" Text="Área:" CssClass="control-label text-right"></asp:Label>
                        <asp:DropDownList ID="DpdArea" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- REGION --%>
                        <asp:Label ID="lblRegion" runat="server" Text="Región:" CssClass="control-label text-right"></asp:Label>
                        <asp:DropDownList ID="DpdRegion" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- ELEMENTO CONTRATADO --%>
                        <asp:Label ID="lblElemento" runat="server" Text="Elemento contratado:" CssClass="control-label text-right "></asp:Label>
                        <asp:DropDownList ID="DpdElemento" DataTextField="Nombre" DataValueField="Id" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                            <asp:ListItem Value="0">Selecciona</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>
                <div class="row">
                    <div class="col-sm-3 col-md-3 col-lg-3 mrgbot"></div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot" style="margin: auto;">
                        <div class="col-sm-16 col-md-12 col-lg-12 mrgbot text-right">
                            <%-- ACEPTAR BUTON --%>
                            <div style="color: transparent">placeHolder</div>
                            <div class="text-center">
                                <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-keytia-md" OnClick="btnAceptar_Click1" />
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-3 col-md-3 col-lg-3 mrgbot"></div>
                </div>
                <div class="row" style="display: none">
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- VIGENTE ENTRE --%>
                        <div class="col-lg-3">
                            <asp:Label ID="lblFechas" runat="server" Text="Vigente entre:" CssClass="control-label text-right"></asp:Label>
                        </div>
                        <div class="col-lg-2">
                            <asp:TextBox runat="server" ID="txtFechaInicio" placeholder="aaaa/mm/dd" OnTextChanged="txtFechaInicio_TextChanged" CssClass="text-left  form-control"></asp:TextBox>
                            y
                        <asp:TextBox runat="server" ID="txtFechaFin" placeholder="aaaa/mm/dd" OnTextChanged="txtFechaFin_TextChanged" CssClass="text-left  form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-sm-6 col-md-6 col-lg-6 mrgbot">
                        <%-- VIGENTE --%>
                        <div class="col-lg-3">
                            <asp:Label ID="lblVigente" runat="server" Text="Vigente:" CssClass="control-label text-right"></asp:Label>
                        </div>
                        <div class="col-lg-2">
                            <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                            <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </asp:Panel>

</asp:Content>
