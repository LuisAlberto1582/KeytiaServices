<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DetallePrevio.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.DetallePrevio" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%-- <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
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
            font-size: 16px;
            color: #58697D;
            font-weight: bold;
        }

        .labelsize2 {
            font-size: 16px;
            color: #58697D;
        }

        .labelsizegrid {
            font-size: 15px;
            color: #58697D;
            font-weight: bold;
        }

        .labelsizegrid1 {
            font-size: 14px;
            color: #58697D;
        }

        .linkgrid {
            font-size: 18px;
            color: #ffffff;
            font-weight: bold;
        }

        .table1 {
            border-color: #CCCCCC;
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
        <div class="formulario  col-sm-12 col-lg-12">
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

            <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
                <div class="row  col-md-12 col-sm-12 col-lg-12 col-xs-12">
                    <asp:Label ID="lblHeader" runat="server" Text="Folio Contrato:" CssClass="labelsize"></asp:Label>
                    <asp:TextBox ID="txtContrato" runat="server" CssClass="labelsize2" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent"></asp:TextBox>
                </div>
                <div class="row  col-md-12 col-sm-12 col-lg-12 col-xs-12">
                    <asp:Label ID="lblEstatus" runat="server" Text="Estatus: " CssClass="labelsize" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="txtEstatus" runat="server" CssClass="labelsize2" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent"></asp:TextBox>
                </div>

                <br />
                <br />
                <br />
                <h4 class="labelsize">Contratos</h4>
                <div class="contenedor col-md-12 col-sm-12 col-lg-12 col-xs-12" style="margin-bottom: 30px; padding: 0px;">
                    <asp:GridView ID="gvPrevioDetalle" runat="server" ShowHeader="false" AutoGenerateColumns="false" DataKeyNames="Folio, Id, Activo" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="None" OnRowDataBound="gvPrevioDetalle_RowDataBound" OnRowCommand="gvPrevioDetalle_RowCommand">
                        <Columns>
                            <asp:TemplateField ControlStyle-Width="20" HeaderStyle-Width="20">
                                <ItemTemplate>
                                    <img alt="" width="20" id="image" style="cursor: pointer" src="../Content/images/plus.png" />
                                    <asp:Panel ID="pnlDetalle" runat="server" Style="display: none">
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFolio" runat="server" Text="Folio:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFolio" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblInicio" runat="server" Text="Inicio Vigencia:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtInicio" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFolioRelacionado" runat="server" Text="Folio relacionado:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFolioRelacionado" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFin" runat="server" Text="Fin vigencia:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFin" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblClave" runat="server" Text="Clave:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtClave" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFechaSolicitud" runat="server" Text="Fecha de Solicitud:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFechaSolicitud" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblProveedor" runat="server" Text="Proveedor:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtProveedor" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblSolicitante" runat="server" Text="Solicitante:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtSolicitante" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblTipoContrato" runat="server" Text="Tipo de contrato:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtTipoContrato" Width="300" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblTipoServicio" runat="server" Text="Tipo de servicio:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtTipoServicio" runat="server" BorderStyle="None" ReadOnly="true" Width="300" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblComprador" runat="server" Text="Comprador:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtComprador" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblVigencia" runat="server" Text="Vigente:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtVigencia" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblDescripcion" runat="server" Text="Descripcion:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtDescripcion" runat="server" BorderStyle="None" ReadOnly="true" Width="250" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Folio" HeaderText="Folio" ItemStyle-CssClass="labelsize" Visible="false" ControlStyle-CssClass="labelsize" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="FechaFinVigencia" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="Encabezado" HeaderText="Encabezado" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" />
                            <asp:BoundField DataField="Activo" Visible="false" />
                            <asp:BoundField DataField="Id" Visible="false" />
                            <asp:TemplateField ItemStyle-CssClass="linkgrid">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDetalle" runat="server" Text="Ver Detalle" CommandName="VerDetalle" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-CssClass="linkgrid">
                                <ItemTemplate>
                                    <asp:LinkButton ID="imgBtnCargar" runat="server" Text="Cargar archivos" CommandName="Cargar" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>


                <h4 class="labelsize">Anexos</h4>
                <div class="contenedor  col-md-12 col-sm-12 col-lg-12 col-xs-12" style="margin-bottom: 30px; padding: 0px;">
                    <asp:GridView ID="gvAnexo" runat="server" ShowHeader="false" DataKeyNames="Folio, Id, FolioContrato, Activo" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="None" OnRowDataBound="gvAnexo_RowDataBound" OnRowCommand="gvAnexo_RowCommand">
                        <Columns>
                            <asp:TemplateField ControlStyle-Width="20" HeaderStyle-Width="20">
                                <ItemTemplate>
                                    <img alt="" width="20" id="image" style="cursor: pointer" src="../Content/images/plus.png" />
                                    <asp:Panel ID="pnlDetalleAnexo" runat="server" Style="display: none">
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFolio" runat="server" Text="Folio:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFolioA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                                <asp:TextBox ID="txtIdA" runat="server" BorderStyle="None" ReadOnly="true" Visible="false" BackColor="Transparent"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblInicioA" runat="server" Text="Fecha Ini vigencia:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtInicioA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFolioRelacionadoA" runat="server" Text="Folio relacionado:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFolioRelacionadoA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFinA" runat="server" Text="Fin vigencia:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFinA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblClaveA" runat="server" Text="Clave:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtClaveA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblDescripcionA" runat="server" Text="Descripcion:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtDescripcionA" runat="server" BorderStyle="None" ReadOnly="true" Width="250" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <<div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblProveedorA" runat="server" Text="Proveedor:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtProveedorA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblSolicitanteA" runat="server" Text="Solicitante:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtSolicitanteA" runat="server" BorderStyle="None" ReadOnly="true" Width="300" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblTipoContratoA" runat="server" Text="Tipo de contrato:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtTipoContratoA" runat="server" BorderStyle="None" ReadOnly="true" Width="300" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblCompradorA" runat="server" Text="Comprador:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtCompradorA" runat="server" BorderStyle="None" ReadOnly="true" Width="300" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <<div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblTipoServicioA" runat="server" Text="Tipo de servicio:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtTipoServicioA" runat="server" BorderStyle="None" ReadOnly="true" Width="300" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblVigenciaA" runat="server" Text="Vigente:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtVigenciaA" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Folio" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="FolioContrato" Visible="false" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="FechaFinVigencia" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="Encabezado" HeaderText="Encabezado" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" />
                            <asp:BoundField DataField="Activo" Visible="false" />
                            <asp:BoundField DataField="Id" Visible="false" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDetalle" runat="server" Text="Ver Detalle" CommandName="VerDetalle" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="imgBtnCargar" runat="server" Text="Cargar archivos" CommandName="Cargar" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>



                <h4 class="labelsize">Convenios modificatorios</h4>
                <div class="contenedor  col-md-12 col-sm-12 col-lg-12 col-xs-12" style="margin-bottom: 30px; padding: 0px;">
                    <asp:GridView ID="gvConvenio" runat="server" ShowHeader="false" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" DataKeyNames="Folio, Id, FolioContrato, Activo" GridLines="None" OnRowDataBound="gvConvenio_RowDataBound" OnRowCommand="gvConvenio_RowCommand">
                        <Columns>
                            <asp:TemplateField ControlStyle-Width="20" HeaderStyle-Width="20">
                                <ItemTemplate>
                                    <img alt="" width="20" id="image" style="cursor: pointer" src="../Content/images/plus.png" />
                                    <asp:Panel ID="pnlDetalleConvenio" runat="server" Style="display: none">
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFolioC" runat="server" Text="Folio:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFolioC" Width="250" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblInicioC" runat="server" Text="Fecha Ini vigencia:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtInicioC" Width="100" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFolioRelacionadoC" runat="server" Text="Folio relacionado:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFolioRelacionadoC" Width="50" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblFinC" runat="server" Text="Fin vigencia:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtFinC" Width="100" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblProveedorC" runat="server" Text="Proveedor:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtProveedorC" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblDescripcionC" runat="server" Text="Descripcion:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtDescripcionC" runat="server" BorderStyle="None" ReadOnly="true" Width="400" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblTipoContratoC" runat="server" Text="Tipo de contrato:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtTipoContratoC" runat="server" Width="300" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblSolicitanteC" runat="server" Text="Solicitante:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtSolicitanteC" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblTipoServicioC" runat="server" Text="Tipo de servicio:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtTipoServicioC" runat="server" Width="300" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblCompradorC" runat="server" Text="Comprador:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtCompradorC" Width="300" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6 col-sm-6 col-lg-6 col-xs-6">
                                                <asp:Label ID="lblVigenciaC" runat="server" Text="Vigente:" CssClass="labelsizegrid"></asp:Label>
                                                <asp:TextBox ID="txtVigenciaC" runat="server" BorderStyle="None" ReadOnly="true" BackColor="Transparent" CssClass="labelsizegrid2"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Folio" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="FolioContrato" Visible="false" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="FechaFinVigencia" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:BoundField DataField="Encabezado" HeaderText="Folio" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" />
                            <asp:BoundField DataField="Activo" Visible="false" />
                            <asp:BoundField DataField="Id" Visible="false" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnVerDetalle" Text="Ver Detalle" runat="server" CommandName="VerDetalle" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="imgBtnCargar" runat="server" Text="Cargar archivos" CommandName="Cargar" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:TextBox ID="url" Visible="false" runat="server"></asp:TextBox>

                <br />
                <br />
                <br />
                <div class="row">
                    <div class="col-lg-8 text-left">
                        <h4 class="labelsize">Documentos renovación</h4>
                    </div>
                    <div class="col-lg-4 text-right">
                        <asp:LinkButton runat="server" CssClass="btn btn-default" ID="lnkAgregar" OnClick="lnkAgregar_Click">
                    <span class="glyphicon glyphicon-plus"></span>
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="contenedor col-sm-12" style="margin-bottom: 30px;">
                    <asp:GridView ID="gvRenovacion" DataKeyNames="RutaArchivo" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="Both" runat="server" OnRowDataBound="gvRenovacion_RowDataBound" OnRowCommand="gvRenovacion_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="RutaArchivo" HeaderText="RutaArchivo" ControlStyle-CssClass="text-left" HeaderStyle-CssClass="text-left" Visible="false" />
                            <asp:ButtonField ControlStyle-CssClass="alert-link" DataTextField="FechaCarga" HeaderText="FechaCarga" CommandName="RutaArchivo" />
                            <asp:BoundField DataField="FaseDeRenovacion" HeaderText="FaseRenovacion" />
                            <asp:BoundField DataField="NombreArchivo" HeaderText="NombreArchivo" />
                            <asp:BoundField DataField="Comentarios" HeaderText="Vigente" />
                            <asp:BoundField DataField="UsuarioCarga" HeaderText="Usuario" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>
                <div class="col-lg-12 text-right">
                    <asp:CheckBox ID="CboxNotificacion" runat="server" Text="Desactivar notificaciones" OnCheckedChanged="CboxNotificacion_CheckedChanged" AutoPostBack="true" />
                </div>
            </div>

        </div>
        <%--    Modal carga Archivos--%>
        <%--<asp:Panel Style="display: none;" runat="server" CssClass="modalPopupEtq" ID="myModalCargarContrato" Width="700px">
                <div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px; font-size: 12px">
                    <asp:Label runat="server" ID="Label6" Text="Cargar archivos"></asp:Label>
                </div>

                <div class="modal-body">
                    <div class="row">
                        <div class="container">
                            <div class="col-lg-10">
                                <br />
                                <asp:Label ID="lblFolioContrato" runat="server"></asp:Label>
                                <asp:TextBox ID="txtFolioContrato" runat="server" ReadOnly="true" BorderStyle="None"></asp:TextBox>
                                <br />
                                <br />
                                <asp:Label ID="lblArchivoContrato" runat="server" Text="Archivo: "></asp:Label>
                                <asp:FileUpload ID="FileUploadControlContrato" runat="server" />
                                <br />
                                <asp:Label ID="Label2Contrato" runat="server" Text="¿Es documento más reciente?"></asp:Label>
                                <asp:RadioButton ID="rbSicntrato" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                                <asp:RadioButton ID="rbNoContrato" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                                <br />
                                <asp:Label ID="Label3Contrato" runat="server" Text="Comentarios: "></asp:Label>
                                <br />
                                <div class="col-lg-12">
                                    <asp:TextBox ID="txtComentarioContrato" runat="server" CssClass="form-control" Rows="10" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="footerEtq">
                    <asp:Button ID="btnCargarContrato" runat="server" OnClick="CargarArchivoscontrato" Text="Guardar" CssClass="btn btn-primary" />
                    <asp:Button runat="server" ID="btnCerrarCargaContrato" CssClass="btn btn-secondary" Text="Cerrar" />
                </div>
            </asp:Panel>
            <asp:LinkButton runat="server" ID="lnkAgregarNZContrato" Style="display: none"></asp:LinkButton>
            <asp:ModalPopupExtender ID="mpeCargaArchivoContrato" runat="server" PopupControlID="myModalCargarContrato"
                TargetControlID="lnkAgregarNZContrato" OkControlID="btnCerrarCarga" BackgroundCssClass="modalBackground">
            </asp:ModalPopupExtender>--%>


        <%-- Modal Cargar Archivos Fin --%>

        <%--    MODAL DE CARGAR DOCUMENTOS RENOVACIÓN--%>
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

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblFolio" runat="server" Text="Folio: "></asp:Label>
                                    <asp:TextBox ID="txtFolio" runat="server" ReadOnly="true" BorderStyle="None" BackColor="Transparent"></asp:TextBox>
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblArchivo" runat="server" Text="Archivo: "></asp:Label>
                                    <asp:FileUpload ID="FileUploadControl" runat="server" />
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="Label4" runat="server" Text="Fase: "></asp:Label>
                                    <asp:DropDownList ID="dpdFase" DataTextField="Nombre" DataValueField="Id" runat="server" Font-Size="Larger" Width="170" AppendDataBoundItems="true">
                                        <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="lblDias" runat="server" Text="Notificarme cada: "></asp:Label>
                                    <asp:TextBox ID="txtDias" runat="server" Width="50" />
                                    <asp:Label ID="Label5" runat="server" Text="días para darle seguimiento."></asp:Label>
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:CheckBox ID="chbxDeshabilitarNotificaciones" runat="server"></asp:CheckBox>
                                    <asp:Label ID="lblDeshabilitarNotificaciones" runat="server" Text="Deshabilitar notificaciones."></asp:Label>
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="Label3" runat="server" Text="Comentarios: "></asp:Label>
                                    <asp:TextBox ID="txtComentario" runat="server" CssClass="form-control" Rows="10" />
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-12 col-sm-12">
                                    <asp:Label ID="Label2" runat="server" Text="¿Es documento más reciente?"></asp:Label>
                                    <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                                    <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                                </div>
                            </div>


                        </div>
                    </div>

                    <div class="modal-footer">

                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <asp:Button ID="btnCargar" runat="server" OnClick="CargarArchivos" Text="Guardar" CssClass="btn btn-keytia-sm" />
                                <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-keytia-sm" Text="Cerrar" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:LinkButton runat="server" ID="lnkAgregarNZ" Style="display: none"></asp:LinkButton>
        <asp:ModalPopupExtender ID="mpeCargaArchivo" runat="server" PopupControlID="myModalCargar"
            TargetControlID="lnkAgregarNZ" OkControlID="btnCerrarCarga" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrar">
        </asp:ModalPopupExtender>



        <%--<script type="text/javascript" src="~/UserInterface/InvContratos/Scripts/jquery.min.js"></script>--%>
        <script type="text/javascript">
            $("[src*=plus]").live("click", function () {
                $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
                $(this).attr("src", "../Content/images/minus.png");
            });
            $("[src*=minus]").live("click", function () {
                $(this).attr("src", "../Content/images/plus.png");
                $(this).closest("tr").next().remove();
            });

            function actualizar() {
                var location = self.location;
            }
        </script>
    </asp:Panel>
</asp:Content>
