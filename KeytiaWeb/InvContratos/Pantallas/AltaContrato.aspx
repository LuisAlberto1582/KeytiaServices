<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AltaContrato.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.Pantallas_AltaContacto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--  <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
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

    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%" class="row">
        <div class="formulario  col-sm-12 col-lg-12">
            <div class="row">
                <div class="col-lg-12 col-sm-12">
                    <h3 class="labelsize">Alta de contrato</h3>
                    <hr />
                </div>

                <div class="portlet-body col-md-12 col-sm-12 col-lg-12 col-xs-12">
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
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFolio" Text="Folio*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFolio" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- FOLIO RELACIONADO --%>
                                            <div class="form-group">
                                                <asp:Label ID="lblFolioRelacionado" Style="font-weight: bold; color: #58697D;" runat="server" Text="Folio relacionado:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFolioRelacionado" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- CLAVE --%>
                                            <div class="form-group">
                                                <asp:Label ID="lblClave" Style="font-weight: bold; color: #58697D;" runat="server" Text="Clave:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtClave" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- CATEGORÍA DE SERVICIO --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCategoriaServicio" Text="Categoría de servicio*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="dpdCategoriaServicio" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <%-- PROVEEDOR --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblProveedor" Text="Proveedor*:" CssClass="col-lg-4 control-label"></asp:Label>
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
                                            <%-- FECHA SOLICITUD --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechaSolicitud" Text="Fecha solicitud*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaSolicitud" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- FECHA EMISION --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechaEmision" Text="Fecha Emisión*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaEmision" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- FECHA INICIO VIGENCIA --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechaInicioV" Text="Fecha inicio vigencia*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaInicioV" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- FECHA FIN VIGENCIA --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechafinV" Text="Fecha fin vigencia*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaFinV" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- CATEGORÍA CONVENIO --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCategoriaConvenio" Text="Categoria Convenio*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="dpdCategoria" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <%-- DESCRIPCION --%>
                                            <div class="form-group">
                                                <asp:Label ID="lblDescripcion" Style="font-weight: bold; color: #58697D;" runat="server" Text="Descripción*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtDescripcion" CssClass="form-control"></asp:TextBox>
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
                                    <h4 class="labelsize">Solicitante y Comprador </h4>
                                </div>
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <%-- SOLICITANTE NOMBRE --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblSolicitanteNombre" Text="Nombre solicitante*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtSolicitanteNombre" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- TELEFONO SOLICITANTE --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblTelSolicitante" Text="Teléfono solicitante:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtTelSolicitante" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- PUESTO SOLICITANTE --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblPuestoSol" Text="Puesto solicitante*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="DpdPuestos" DataTextField="vchDescripcion" DataValueField="iCodCatalogo" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <%-- AREA --%>
                                            <div class="form-group">
                                                <asp:Label ID="lblArea" Style="font-weight: bold; color: #58697D;" runat="server" Text="Area*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="DpdArea" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <%-- REGION --%>
                                            <div class="form-group">
                                                <asp:Label ID="lblRegion" Style="font-weight: bold; color: #58697D;" runat="server" Text="Region*:" CssClass="col-lg-4 control-label"></asp:Label>
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
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblNombreComprador" Text="Nombre comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtNombreComprador" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- TELEFONO COMPRADOR --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblTelComprador" Text="Teléfono comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtTelComprador" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- PUESTO COMPRADOR --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblPuestoComprador" Text="Puesto Comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="DpdPuesto" DataTextField="vchDescripcion" DataValueField="iCodCatalogo" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <%-- EMAIL COMPRADOR --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblEmailComprador" Text="Email comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtEmailComprador" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- AREA COMPRADOR --%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblAreaComprador" Text="Area Comprador:" CssClass="col-lg-4 control-label"></asp:Label>
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
                                <div class="panel-heading">
                                    <h4 class="labelsize"></h4>
                                </div>
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
                                    <h4 class="labelsize">Configuración warnings y destinatarios</h4>
                                </div>
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <%-- WARNING ESTATUS --%>
                                            <div class="form-group" style="display: none">
                                                <asp:Label ID="lblWarningEstatus" Style="font-weight: bold; color: #58697D;" runat="server" Text="Estatus de Warning:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="dpdWarningEstatus" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="1">CONTRATO VIGENTE</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <%-- CANTIDAD DIAS WARNING--%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCantidadDias" Text="periodicidad de warnings*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtCantidadDias" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- FRECUENCIA REENVIO--%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFrecuenciaReenvio" Text="Frecuencia de reenvío (Días)*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFrecuenciaReenvio" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- ENVIAR WARNINGS--%>
                                          <%--  <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblWarningActivo" Text="Enviar warnings:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:RadioButton ID="rbWSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                                                    <asp:RadioButton ID="rbWNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                                                </div>
                                            </div>--%>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <%--DESTINATARIO--%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblDestinatario" Text="Destinatario*:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtDestinatario" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%-- CC--%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCC" Text="CC:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtCC" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <%--CCO--%>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCCO" Text="CCO:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtCCO" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="row" style="margin-top: 30px; text-align: center; margin-bottom: 30px">
                        <asp:Button runat="server" ID="btnAceptar" OnClick="btnAceptar_Click" Text="Aceptar" CssClass="btn btn-keytia-md" />
                    </div>
                </div>

                <div class="row" style="margin-bottom: 15px">
                    <%-- ESTATUS DEL CONVENIO --%>
                    <div class="col-lg-6" style="display: none">
                        <div class="col-lg-4">
                            <asp:Label runat="server" ID="lblEstatusConvenio" Text="Estatus convenio:"></asp:Label>
                        </div>
                        <div class="col-lg-2">
                            <asp:DropDownList ID="DpdEstatus" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" Width="170" AppendDataBoundItems="true">
                                <asp:ListItem Value="1">ACTIVO</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                </div>
                <div class="row" style="margin-bottom: 15px; display: none;">
                    <%-- SOCIEDAD --%>
                    <div class="col-lg-6">
                        <div class="col-lg-4">
                            <asp:Label runat="server" ID="lblSociedades" Text="Sociedades:"></asp:Label>
                        </div>
                        <asp:CheckBoxList ID="CListSociedades" DataTextField="Nombre" AppendDataBoundItems="true" DataValueField="ID" runat="server"></asp:CheckBoxList>
                    </div>

                </div>
                <div class="row" style="margin-bottom: 15px; display: none;">
                    <%-- RFP --%>
                    <div class="col-lg-6">
                        <div class="col-lg-4">
                            <asp:Label ID="lblRFP" runat="server" Text="Requiere RFP:"></asp:Label>
                        </div>
                        <div class="col-lg-2">
                            <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                            <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                        </div>
                    </div>

                </div>
                <div class="row" style="margin-bottom: 15px; display: none;">
                    <%-- COMENTARIOS --%>
                    <div class="col-lg-6">
                        <div class="col-lg-4">
                            <asp:Label runat="server" ID="lblComentarios" Text="Comentarios:"></asp:Label>
                        </div>
                        <div class="col-lg-2">
                            <asp:TextBox runat="server" ID="txtComentarios" Text=""></asp:TextBox>
                        </div>
                    </div>
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
