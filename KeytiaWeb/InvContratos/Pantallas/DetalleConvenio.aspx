<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DetalleConvenio.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.DetalleConvenio" %>

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
        <div class="formulario col-sm-12">

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

            <div class="container container col-lg-12 row" runat="server">
                <div class="row container col-lg-12">
                    <div class="row form-inline col-lg-10">
                        <asp:Label ID="lblFolioHead" runat="server" Text="Folio contrato:" CssClass="title-form control-label"></asp:Label>
                        <asp:TextBox ID="txtFolioContrato" runat="server" ReadOnly="true" BorderStyle="None" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="row form-inline col-lg-10">
                        <asp:Label ID="lblConvenioHead" runat="server" Text="Detalle Convenio: " CssClass="title-form control-label"></asp:Label>
                        <asp:TextBox ID="txtFolioConvenio" runat="server" ReadOnly="true" BorderStyle="None" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="row col-sm-12 col-m-12 col-lg-12">
                        <div class="col-sm-8 col-m-8 col-lg-8">
                            <h4>Contratos</h4>
                        </div>
                        <div class="co2-sm-2 col-m-2 col-lg-2">
                            <asp:Button runat="server" ID="btnEditar" Text="Editar" OnClick="btnEditar_Click" CssClass="form-control btn btn-default" />
                        </div>
                        <div class="co2-sm-2 col-m-2 col-lg-2">
                            <asp:Button runat="server" ID="btnEliminar" Text="Eliminar" OnClick="btnEliminar_Click" CssClass=" form-control btn-default" />
                        </div>
                    </div>
                </div>

                <div style="width: 100%; height: 200px; overflow: auto" class="contenedor col-lg-12">
                    <asp:GridView ID="gvConvenio" OnRowCommand="gvConvenio_RowCommand" runat="server" ShowHeader="false" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="None">
                        <Columns>
                            <asp:TemplateField ControlStyle-Width="20" HeaderStyle-Width="20">
                                <ItemTemplate>
                                    <%--<img alt="" width="20" id="image" style="cursor: pointer" src="../Content/images/plus.png" />--%>
                                    <asp:Panel ID="pnlDetalleConvenio" runat="server" Style="display: inline;">
                                        <div class="row">
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblFolioC" runat="server" Text="Folio:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtFolioC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                                <asp:TextBox ID="txtConvenioId" Visible="false" ReadOnly="true" runat="server" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblInicioC" runat="server" Text="Fecha Ini vigencia:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtInicioC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblFolioRelacionadoC" runat="server" Text="Folio con el cual se relaciona:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtFolioRelacionadoC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblFinC" runat="server" Text="Fecha fin vigencia:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtFinC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblClaveC" runat="server" Text="Clave:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtClaveC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblVigenciaC" runat="server" Text="Vigente:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtVigenciaC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblProveedorC" runat="server" Text="Proveedor:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtProveedorC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblSolicitanteC" runat="server" Text="Solicitante:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtSolicitanteC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblTipoContratoC" runat="server" Text="Tipo de contrato:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtTipoContratoC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblCompradorC" runat="server" Text="Comprador:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtCompradorC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 form-inline">
                                                <asp:Label ID="lblTipoServicioC" runat="server" Text="Tipo de servicio:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtTipoServicioC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-7 form-inline">
                                                <asp:Label ID="lblDescripcionC" runat="server" Text="Descripcion:" CssClass="control-label"></asp:Label>
                                                <asp:TextBox ID="txtDescripcionC" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField DataField="Folio" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="FechaFinVigencia" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="Encabezado" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" />--%>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="imgBtnCargar" runat="server" Text="Cargar archivos" CommandName="Cargar" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>

                <h4>Descarga de Archivos</h4>
                <div style="width: 100%; height: 200px; overflow: auto" class="contenedor col-lg-12">
                    <asp:GridView ID="gvArchivos" DataKeyNames="RutaArchivo" OnRowCommand="gvArchivos_RowCommand" AutoGenerateColumns="false" Font-Size="Small" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="Both" runat="server">
                        <Columns>
                            <asp:BoundField DataField="RutaArchivo" HeaderText="RutaArchivo" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:ButtonField ControlStyle-CssClass="alert-link" DataTextField="Nombre" HeaderText="Nombre" CommandName="RutaArchivo" />
                            <asp:BoundField DataField="FechaCarga" HeaderText="FechaCarga" />
                            <asp:BoundField DataField="EsVigente" HeaderText="Vigente" />
                            <asp:BoundField DataField="Usuar" HeaderText="Usuar" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>

                <h4>Elementos contratados</h4>
                <div class="col-lg-12 contenedor" style="width: 100%; height: 200px; overflow: auto">
                    <asp:GridView ID="gvElementos" DataKeyNames="Id" runat="server" AutoGenerateColumns="False" CssClass="table" EnableModelValidation="True" OnRowDeleting="gvElementos_RowDeleting">
                        <Columns>
                            <asp:BoundField DataField="Id" Visible="false" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="Elemento" HeaderText="Elemento" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="CostoUnitarioMXN" HeaderText="Importe MXN" />
                            <asp:BoundField DataField="CostoUnitarioMonedaOriginal" HeaderText="Importe moneda original" />
                            <asp:BoundField DataField="TipoDeCambio" HeaderText="Tipo de Cambio" />
                            <asp:BoundField DataField="MonedaOriginal" HeaderText="Moneda" />
                            <asp:CommandField ShowDeleteButton="true" ButtonType="Link" DeleteText="Eliminar" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>
                <div class="form-inline">
                    <asp:TextBox ID="txtCantidad" runat="server" Width="102" PlaceHolder="Cantidad" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control" />
                    <asp:DropDownList ID="DpdElemento" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" Width="140" AppendDataBoundItems="true" CssClass="form-control">
                        <asp:ListItem Value="0">Tipo..</asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="110" Enabled="false" CssClass="form-control" />
                    <asp:TextBox ID="txtCostoUnitarioMXN" runat="server" Width="90" PlaceHolder="Importe MXN" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control" />
                    <asp:TextBox ID="txtCostoUnitarioMonedaOriginal" runat="server" Width="190" PlaceHolder="Importe moneda orig." CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control" />
                    <asp:TextBox ID="txtTipodeCambio" runat="server" Width="145" PlaceHolder="Tipo de cambio" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control" />
                    <%--<asp:TextBox ID="txtMoneda" runat="server" Width="90" PlaceHolder="Moneda" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control" />--%>
                    <asp:DropDownList ID="DpMonedaElemCont" DataTextField="vchCodigo" DataValueField="Id" Width="170" runat="server" Font-Size="Larger" AppendDataBoundItems="true" CssClass="form-control">
                        <asp:ListItem Value="0">Moneda..</asp:ListItem>
                    </asp:DropDownList>


                    <asp:LinkButton ID="InsertaElemento" runat="server" CssClass="btn btn-default form-control" OnClick="InsertaElementos"> 
                            <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                    </asp:LinkButton>
                </div>

                <br />
                <h4>Relacion de Pagos</h4>
                <div class="col-lg-12 contenedor" style="width: 100%; height: 200px; overflow: auto">
                    <asp:GridView ID="gvRelacionPagos" DataKeyNames="Id" runat="server" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="Both" EnableModelValidation="True" OnRowDeleting="gvRelacionPagos_RowDeleting">
                        <Columns>
                            <asp:BoundField DataField="Id" Visible="false" />
                            <asp:BoundField DataField="PagoNumero" HeaderText="Pago Número" />
                            <asp:BoundField DataField="FechaPago" HeaderText="Fecha Pago" />
                            <asp:BoundField DataField="ImporteMXN" HeaderText="Importe MXN" />
                            <asp:BoundField DataField="ImporteMonedaOriginal" HeaderText="Importe moneda original" />
                            <asp:BoundField DataField="TipoDeCambio" HeaderText="Tipo de Cambio" />
                            <asp:BoundField DataField="MonedaOriginal" HeaderText="Moneda Original" />
                            <asp:CommandField ShowDeleteButton="true" ButtonType="Link" DeleteText="Eliminar" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>

                <div class="form-inline">
                    <asp:TextBox ID="txtPagoNumero" runat="server" Width="136" PlaceHolder="Pago número" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumerosInt(event)" CssClass="form-control" />
                    <asp:TextBox ID="txtFechaPago" runat="server" Width="140" PlaceHolder="aaaa/mm/dd" CssClass="form-control" />
                    <asp:TextBox ID="txtImporteMXN" runat="server" Width="130" PlaceHolder="Importe MXN" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control" />
                    <asp:TextBox ID="txtImporteMoneda" runat="server" Width="190" PlaceHolder="Importe moneda orig." CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control" />
                    <asp:TextBox ID="txtTipoCambio" runat="server" Width="145" PlaceHolder="Tipo de cambio" CausesValidation="true" ValidationGroup="okButton" onkeypress="javascript:return solonumeros(event)" CssClass="form-control" />
                    <asp:DropDownList ID="DpdMoneda" DataTextField="vchCodigo" DataValueField="Id" runat="server" Font-Size="Larger" Width="160" AppendDataBoundItems="true" CssClass="form-control">
                        <asp:ListItem Value="0">Moneda..</asp:ListItem>
                    </asp:DropDownList>

                    <asp:LinkButton ID="InsertaRelacionPago" runat="server" CssClass="btn btn-default form-control" OnClick="InsertaRelacionPagos"> 
                        <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                    </asp:LinkButton>
                </div>
                <br />
                <br />
            </div>
        </div>
    </asp:Panel>


    <%-- MODAL RM --%>
    <asp:Panel Style="display: none;" runat="server" CssClass="modal-Keytia" ID="myModalCargar" TabIndex="-1" role="dialog">
        <div class="rule"></div>
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label runat="server" ID="lblTituloEditHallazo" Text="Cargar archivos"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>

                <div class="modal-body">
                    <div id="RepCapturaCollapse" class="form-horizontal" role="form">
                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Label ID="lblFolio" runat="server" Text="Detalle Convenio: " CssClass="control-label"></asp:Label>
                                <asp:TextBox ID="txtFolioConvenio2" runat="server" ReadOnly="true" BorderStyle="None" CssClass="form-control" Width="200"></asp:TextBox>
                            </div>
                        </div>
                        <br />

                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Label ID="lblArchivo" runat="server" Text="Archivo: " CssClass="control-label"></asp:Label>
                                <asp:FileUpload ID="FileUploadControl" runat="server" />
                            </div>
                        </div>
                        <br />

                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Label ID="Label2" runat="server" Text="¿Es documento más reciente?" CssClass="control-label"></asp:Label>
                                <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                                <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                            </div>
                        </div>
                        <br />

                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Label ID="Label3" runat="server" Text="Comentarios: " CssClass="control-label"></asp:Label>
                                <asp:TextBox ID="txtComentario" runat="server" CssClass="form-control" Rows="10" />
                            </div>
                        </div>
                        <br />
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="Button1" runat="server" OnClick="btnCargar_Click" Text="Guardar" CssClass="btn btn-keytia-sm" />
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

    <%--<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>");
            $(this).attr("src", "../Content/images/minus.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "../Content/images/plus.png");
            $(this).closest("tr").next().remove();
        });


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
