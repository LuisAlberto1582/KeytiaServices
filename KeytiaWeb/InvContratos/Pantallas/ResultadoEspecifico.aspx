<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ResultadoEspecifico.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.ResultadoEspecifico" %>


<%@ Import Namespace="System.Web" %>
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

            <div runat="server" class="col-sm-12">
                <asp:Label ID="lblHeader" runat="server" Text="Búsqueda: " CssClass="title-form"></asp:Label>
                <asp:TextBox ID="txtContrato" runat="server" BorderStyle="None" ReadOnly="true" Enabled="false" BackColor="Transparent"></asp:TextBox>

                <h4>Contratos</h4>
                <div style="width: 100%; height: 200px; overflow: auto" class="contenedor">
                    <asp:GridView ID="gvContrato" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="Both" AllowSorting="true" runat="server" EmptyDataText="No se encontraron elementos que coincidan con los parámetros de  búsqueda" DataKeyNames="Folio, Activo, Id">
                        <Columns>

                            <%--                            <asp:HyperLinkField
                                DataTextField="Folio"
                                HeaderText="Folio2"
                                DataNavigateUrlFields="Folio, Activo, Id"
                                DataNavigateUrlFormatString="~\invContratos\Pantallas\DetallePrevio.aspx?Folio={0}&Estatus={1}&IdContrato={0}&IdAnexo=0&IdConv=0&Procedimiento=Contrato" />--%>

                            <asp:TemplateField HeaderText="Folio">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" Text='<%#Eval("Folio")%>' OnClick="lbtngvContrato_Click"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:BoundField DataField="Id" HeaderText="Id" Visible="false" />
                            <asp:BoundField DataField="Nombre" HeaderText="Proveedor" />
                            <asp:BoundField DataField="CategoriaConvenio" HeaderText="Categoria Convenio" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripcion" />
                            <asp:BoundField DataField="FechaInicioVigencia" HeaderText="Fecha Inicio" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="Fecha Fin" />
                            <asp:BoundField DataField="Activo" HeaderText="Estatus" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>
                <br />

                <h4>Anexos</h4>
                <div style="width: 100%; height: 210px; overflow: auto" class="contenedor">
                    <asp:GridView ID="gvAnexo" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="Both" AllowSorting="true" runat="server" EmptyDataText="No se encontraron elementos que coincidan con los parámetros de  búsqueda" DataKeyNames="Folio, Activo, Contrato, Id, ContratoId">
                        <Columns>
                            <%--<asp:HyperLinkField DataTextField="Folio" HeaderText="Folio" DataNavigateUrlFields="Folio, Activo, Contrato, Id, ContratoId" DataNavigateUrlFormatString="~\invContratos\Pantallas\DetallePrevio.aspx?Folio={0}&Estatus={1}&folioContrato={2}&IdAnexo={3}&IdContrato={4}&IdConv=0&Procedimiento=Anexo" />--%>

                            <asp:TemplateField HeaderText="Folio">
                                <ItemTemplate>
                                    <asp:LinkButton ID="gridAnexolkBtn" runat="server" Text='<%#Eval("Folio")%>' OnClick="gridAnexolkBtn_Click"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ContratoId" HeaderText="ContratoId" Visible="false" />
                            <asp:BoundField DataField="Id" HeaderText="Id" Visible="false" />
                            <asp:BoundField DataField="Contrato" HeaderText="Contrato" />
                            <asp:BoundField DataField="Nombre" HeaderText="Proveedor" />
                            <asp:BoundField DataField="CategoriaConvenio" HeaderText="Categoria Convenio" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripcion" />
                            <asp:BoundField DataField="FechaInicioVigencia" HeaderText="Fecha Inicio" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="Fecha Fin" />
                            <asp:BoundField DataField="Activo" HeaderText="Estatus" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>
                <br />
                <h4>Convenios</h4>
                <div style="width: 100%; height: 200px; overflow: auto" class="contenedor">
                    <asp:GridView ID="gvConvenio" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard" GridLines="Both" AllowSorting="true" runat="server" EmptyDataText="No se encontraron elementos que coincidan con los parámetros de  búsqueda" DataKeyNames="Folio, Activo, Contrato, Id, ContratoId">
                        <Columns>
                            <%--<asp:HyperLinkField DataTextField="Folio" HeaderText="Folio" DataNavigateUrlFields="Folio, Activo, Contrato, Id, ContratoId" DataNavigateUrlFormatString="~\invContratos\Pantallas\DetallePrevio.aspx?Folio={0}&Estatus={1}&folioContrato={2}&IdConv={3}&IdContrato={4}&IdAnexo=0&Procedimiento=ConvModificatorio" />--%>

                            <asp:TemplateField HeaderText="Folio">
                                <ItemTemplate>
                                    <asp:LinkButton ID="gridConveniolkBtn" runat="server" Text='<%#Eval("Folio")%>' OnClick="gridConveniolkBtn_Click"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ContratoId" HeaderText="ContratoId" Visible="false" />
                            <asp:BoundField DataField="Id" HeaderText="Id" Visible="false" />
                            <asp:BoundField DataField="Contrato" HeaderText="Contrato" />
                            <asp:BoundField DataField="Nombre" HeaderText="Proveedor" />
                            <asp:BoundField DataField="CategoriaConvenio" HeaderText="Categoria Convenio" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripcion" />
                            <asp:BoundField DataField="FechaInicioVigencia" HeaderText="Fecha Inicio" />
                            <asp:BoundField DataField="FechaFinVigencia" HeaderText="Fecha Fin" />
                            <asp:BoundField DataField="Activo" HeaderText="Estatus" />
                        </Columns>
                        <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
