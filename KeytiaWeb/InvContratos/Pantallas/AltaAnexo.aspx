﻿<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AltaAnexo.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.Pantallas_AltaAnexo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--    <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
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

        .labelsize {
            color: #58697D;
            font-weight: bold;
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


    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%" class="row">
        <div class="formulario  col-sm-12 col-lg-12">
            <div class="row">
                <div class="col-sm-12 col-lg-12">
                    <h3 class="labelsize">Alta de Anexo</h3>
                    <hr />
                </div>
            </div>

            <div class="portlet-body">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="labelsize">Datos Generales</h4>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-6">
                                    <div class="form-horizontal">
                                        <%-- FOLIO --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblFolio" Text="Folio*:" Style="font-weight: bold; color: #58697D;" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtFolio" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- FOLIO RELACIONADO --%>
                                        <div class="form-group text-left">
                                            <asp:Label ID="lblFolioRelacionado" runat="server" Style="font-weight: bold; color: #58697D;" Text="Folio contrato:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtFolioRelacionado" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- CLAVE --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblClave" runat="server" Style="font-weight: bold; color: #58697D;" Text="Clave:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtClave" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- CATEGORÍA DE SERVICIO --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblCategoriaServicio" Style="font-weight: bold; color: #58697D;" Text="Categoría de servicio:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="dpdCategoriaServicio" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- PROVEEDOR --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblProveedor" Style="font-weight: bold; color: #58697D;" Text="Proveedor:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="dpdProveedor" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-horizontal">
                                        <%-- FECHA EMISION --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblFechaEmision" Style="font-weight: bold; color: #58697D;" Text="Fecha Emisión*:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtFechaEmision" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- FECHA INICIO VIGENCIA --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblFechaInicioV" Style="font-weight: bold; color: #58697D;" Text="Fecha inicio vigencia*:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtFechaInicioV" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- FECHA FIN VIGENCIA --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblFechafinV" Style="font-weight: bold; color: #58697D;" Text="Fecha fin vigencia*:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtFechaFinV" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- CATEGORÍA CONVENIO --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblCategoriaConvenio" Style="font-weight: bold; color: #58697D;" Text="Categoria Convenio*:" CssClass="col-lg-4 control-label textlabel"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="dpdCategoria" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- DESCRIPCION --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblDescripcion" runat="server" Style="font-weight: bold; color: #58697D;" Text="Descripción*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtDescripcion" CssClass="form-control" Text=""></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="labelsize">Comprador</h4>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-6">
                                    <div class="form-horizontal">
                                        <%-- AREA --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblArea" runat="server" Style="font-weight: bold; color: #58697D;" Text="Area*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="DpdArea" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- REGION --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblRegion" runat="server" Style="font-weight: bold; color: #58697D;" Text="Region*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="DpdRegion" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-horizontal">
                                        <%-- NOMBRE COMPRADOR --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblNombreComprador" Style="font-weight: bold; color: #58697D;" Text="Nombre comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtNombreComprador" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- TELEFONO COMPRADOR --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblTelComprador" Style="font-weight: bold; color: #58697D;" Text="Teléfono comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtTelComprador" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- PUESTO COMPRADOR --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblPuestoComprador" Style="font-weight: bold; color: #58697D;" Text="Puesto Comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="DpdPuesto" DataTextField="vchDescripcion" DataValueField="iCodCatalogo" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- EMAIL COMPRADOR --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblEmailComprador" Style="font-weight: bold; color: #58697D;" Text="Email comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtEmailComprador" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- AREA COMPRADOR --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" ID="lblAreaComprador" Style="font-weight: bold; color: #58697D;" Text="Area Comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtAreaComprador" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-default">
                            <div class="panel-heading"></div>
                            <div class="panel-body">
                                <div class="col-lg-6">
                                    <div class="form-horizontal">
                                        <%-- CUENTA CONTABLE --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblCuentaContable" Style="font-weight: bold; color: #58697D;" runat="server" Text="Cuenta contable:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtCuentaContable" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- MONEDA ORIGINAL --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblMonedaOrig" Text="Moneda Original*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:DropDownList ID="DpdMoneda" DataTextField="vchCodigo" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- TIPO DE CAMBIO --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblTipoCambio" Text="Tipo de Cambio*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtTipoCambio" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- MESES DURACIÓN CONVENIO --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblMesDuracion" Text="Meses duración*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtMesDuracion" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-horizontal">
                                        <%-- MONTO TOTAL MONEDA ORIGINAL --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblMontoTotalMO" Style="font-weight: bold; color: #58697D;" runat="server" Text="Monto total Moneda Original*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtMontoTotalMO" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- MONTO TOTAL EN MXN --%>
                                        <div class="form-group">
                                            <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblMontoMXN" Text="Monto Total MXN*:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtMontoMXN" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%-- CANTIDAD PAGOS PROGRAMADOS --%>
                                        <div class="form-group">
                                            <asp:Label ID="lblCantidadPagos" Style="font-weight: bold; color: #58697D;" runat="server" Text="Cantidad pagos programados:" CssClass="col-lg-4 control-label"></asp:Label>
                                            <div class="col-lg-6">
                                                <asp:TextBox runat="server" ID="txtCantidadPagos" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-10">
                        <div class="form-horizontal">
                            <div class="col-lg-">
                                <div class="col-lg-offset-6 col-lg-4">
                                    <asp:Button runat="server" ID="btnAceptar" OnClick="btnAceptar_Click" Text="Aceptar" CssClass="btn btn-keytia-md" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="form-horizontal">

                    <%-- ESTATUS DEL CONVENIO --%>
                    <div class="form-group" style="display: none">
                        <asp:Label runat="server" ID="lblEstatusConvenio" Text="Estatus convenio*:" CssClass="col-lg-4 control-label"></asp:Label>
                        <div class="col-lg-2">
                            <asp:DropDownList ID="DpdEstatus" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" Width="170" AppendDataBoundItems="true" CssClass="col-lg-2 form-control">
                                <asp:ListItem Value="1">ACTIVO</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <%-- SOCIEDAD --%>
                    <div class="form-group" style="display: none;">
                        <asp:Label runat="server" ID="lblSociedades" Text="Sociedades:" CssClass="col-lg-4 control-label"></asp:Label>
                        <div class="col-lg-6">
                            <asp:CheckBoxList ID="CListSociedades" DataTextField="Nombre" AppendDataBoundItems="true" DataValueField="ID" runat="server"></asp:CheckBoxList>
                        </div>
                    </div>
                    <%-- RFP --%>
                    <div class="form-group" style="display: none;">
                        <asp:Label ID="lblRFP" runat="server" Text="Requiere RFP:" CssClass="col-lg-4 control-label"></asp:Label>
                        <div class="col-lg-4">
                            <div class="form-inline">
                                <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                                <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <%-- COMENTARIOS --%>
            <div class="col-lg-6" style="display: none">
                <div class="col-lg-4">
                    <asp:Label runat="server" ID="lblComentarios" Text="Comentarios:"></asp:Label>
                </div>
                <div class="col-lg-2">
                    <asp:TextBox runat="server" ID="txtComentarios" Text=""></asp:TextBox>
                </div>
            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        function solonumeros(e) {
            var charCode = (e.which) ? e.which : e.keyCode;
            if (charCode > 46 && charCode > 31
                && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }

        function solonumerosInt(e) {

            var key;

            if (window.event) // IE
            {
                key = e.keyCode;
            }
            else if (e.which) // Netscape/Firefox/Opera
            {
                key = e.which;
            }

            if (key < 48 || key > 57) {
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
