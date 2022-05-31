<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="EditarConvenio.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.Pantallas_EditarConvenio" %>

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

        .table tbody tr td {
            border-top: 0px;
            border-bottom: 0px;
        }

        .table tr td {
            color: #58697D;
            font-weight: bold;
            font-size: 15px;
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
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%" CssClass="row">
        <div class="formulario col-lg-12">
            <div style="display: none">
                <asp:Label ID="lblFolioHead" Text="Folio contrato: " runat="server" Font-Underline="true" Font-Size="X-Large"></asp:Label>
                <asp:TextBox ID="txtFolioHead" ReadOnly="true" runat="server" Font-Underline="true" Font-Size="X-Large" BorderStyle="None"></asp:TextBox>
                <asp:Label ID="lblFolioConv" Text="Edición convenio:" Font-Underline="true" Font-Size="X-Large" runat="server"></asp:Label>
                <asp:TextBox ID="txtFolioConv" ReadOnly="true" runat="server" Font-Underline="true" Font-Size="X-Large" BorderStyle="None"></asp:TextBox>
            </div>
            <div class="container  col-lg-12  col-md-12" runat="server">
                <br />
                <h3 class="labelsize">Edición Convenio</h3>
                <div class="row">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <asp:GridView ID="gvConvenio" runat="server" ShowHeader="false" AutoGenerateColumns="false" CssClass="table" GridLines="None" AllowPaging="True" PageSize="5">
                                <Columns>
                                    <asp:BoundField DataField="Folio" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                                    <asp:BoundField DataField="FechaFinVigencia" HeaderText="FechaFinVigencia" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                                    <asp:BoundField DataField="Encabezado" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" />
                                </Columns>
                            </asp:GridView>
                        </div>
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
                                            <div class="form-group">
                                                <asp:Label ID="lblFolioContrato" Style="font-weight: bold; color: #58697D;" runat="server" Text="Folio contrato:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFolioContrato" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="lblFolioAnexo" Style="font-weight: bold; color: #58697D;" runat="server" Text="Folio Anexo:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox ID="txtFolioAnexo" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCategoriaServicio" Text="Categoría de servicio:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="dpdCategoriaServicio" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechaEmision" Text="Fecha Emisión:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaEmision" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechaInicioV" Text="Fecha inicio vigencia:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaInicioV" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblFechafinV" Text="Fecha fin vigencia:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtFechaFinV" placeholder="aaaa/mm/dd" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblCategoriaConvenio" Text="Categoria Convenio:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="dpdCategoria" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="lblDescripcion" Style="font-weight: bold; color: #58697D;" runat="server" Text="Descripción:" CssClass="col-lg-4 control-label"></asp:Label>
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
                                    <h4 class="labelsize">Comprador</h4>
                                </div>
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <asp:Label ID="lblArea" Style="font-weight: bold; color: #58697D;" runat="server" Text="Area:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="DpdArea" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="lblRegion" Style="font-weight: bold; color: #58697D;" runat="server" Text="Region:" CssClass="col-lg-4 control-label"></asp:Label>
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
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblNombreComprador" Text="Nombre comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtNombreComprador" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblTelComprador" Text="Teléfono comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtTelComprador" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblPuestoComprador" Text="Puesto Comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="DpdPuesto" DataTextField="vchDescripcion" DataValueField="iCodCatalogo" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblEmailComprador" Text="Email comprador:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtEmailComprador" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
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
                                <div class="panel-heading"></div>
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <asp:Label ID="lblCuentaContable" Style="font-weight: bold; color: #58697D;" runat="server" Text="Cuenta contable:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtCuentaContable" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblMonedaOrig" Text="Moneda Original:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="DpdMoneda" DataTextField="vchCodigo" DataValueField="Id" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="col-lg-6 form-control">
                                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblTipoCambio" Text="Tipo de Cambio:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtTipoCambio" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblMesDuracion" Text="Meses duración:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-6">
                                                    <asp:TextBox runat="server" ID="txtMesDuracion" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <asp:Label ID="lblMontoTotalMO" Style="font-weight: bold; color: #58697D;" runat="server" Text="Monto total Moneda Original:" CssClass="col-lg-4 control-label"></asp:Label>
                                                <div class="col-lg-4">
                                                    <asp:TextBox runat="server" ID="txtMontoTotalMO" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" Style="font-weight: bold; color: #58697D;" ID="lblMontoMXN" Text="Monto Total MXN:" CssClass="col-lg-4 control-label"></asp:Label>
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
                        <asp:TextBox runat="server" ReadOnly="true" ID="txtIdConvenio" Visible="false"></asp:TextBox>
                        <div class="col-lg-12" style="margin-top: 20px; text-align: center; margin-bottom: 30px">
                            <asp:Button runat="server" ID="btnGuardar" Text="Guardar" CssClass="btn btn-keytia-md" OnClick="btnGuardar_Click" />
                        </div>
                    </div>
                </div>
                <div style="display: none;">
                    <asp:TextBox runat="server" ID="txtComentarios" Text=""></asp:TextBox>
                    <asp:DropDownList ID="DpdEstatus" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" Width="170" AppendDataBoundItems="true">
                        <asp:ListItem Value="1">ACTIVO</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                    <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                    <asp:CheckBoxList ID="CListSociedades" DataTextField="Nombre" AppendDataBoundItems="true" DataValueField="ID" runat="server"></asp:CheckBoxList>
                </div>
            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        function Guardar(result, folio, folioConvenio, Id) {
            alert(result);
            if (result == "Ok") {
                location.href = 'DetalleConvenio.aspx?Folio=' + folio + '&FolioConvenio=' + folioConvenio + '&Id=' + Id;
            }
        }
    </script>
    <script>
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
