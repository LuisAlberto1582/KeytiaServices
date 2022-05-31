<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DownLoadFiles.aspx.cs" Inherits="KeytiaWeb.UserInterface.DownLoadFiles" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">

        $(function () {
            var idText = document.getElementById('<%=txtBuscar.ClientID%>');
                var grvLineas = '<%=gvDetails.ClientID%>';
                    var grid;
                    if (grvLineas != null) {
                        grid = grvLineas;
                    }                   
            <%--var grid = '<%=grvLineasPlan.ClientID%>';--%>

                $(idText).keyup(function () {
                    var val = $(this).val().toUpperCase();
                    $('#' + grid + ' > tbody > tr').each(function (index, element) {
                        if ($(this).text().toUpperCase().indexOf(val) < 0)
                            $(this).hide();
                        else
                            $(this).show();
                    });
                });
            });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div id="pnlMainHolder" runat="server">
        <div id="pnlRow_0" runat="server" cssclass="row">
            <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Descarga de Archivos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="row" runat="server" id="row1">
                                <div class="col-sm-12">
                                    <div class="row">
                                        <asp:Panel ID="rowCategoria" runat="server" CssClass="form-group">
                                            <asp:Label ID="lblCategoria" runat="server" CssClass="col-sm-2 control-label">Categoria: </asp:Label>
                                            <div class="col-sm-8">
                                                <asp:DropDownList runat="server" ID="cboCategoria" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="NombreCategoria" CssClass="col-sm-2 form-control">
                                                    <asp:ListItem Value="0"> Seleciona una Categoria </asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowAnio" runat="server" CssClass="form-group">
                                            <asp:Label ID="lblAnio" runat="server" CssClass="col-sm-2 control-label">Año: </asp:Label>
                                            <div class="col-sm-8">
                                                <asp:DropDownList runat="server" ID="cboAnio" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control"></asp:DropDownList>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="rowMes" runat="server" CssClass="form-group">
                                            <asp:Label ID="lblMes" runat="server" CssClass="col-sm-2 control-label">Mes: </asp:Label>
                                            <div class="col-sm-8">
                                                <asp:DropDownList runat="server" ID="cboMes" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control"></asp:DropDownList>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div class="row">
                                        <asp:Panel ID="rowBuscar" runat="server" CssClass="form-group">
                                            <div class="col-sm-offset-4 col-sm-8">
                                                <asp:Button runat="server" ID="btnBuscar" CssClass="btn btn-keytia-lg" Text="Buscar" OnClick="btnBuscar_Click" />
                                            </div>
                                            <asp:Panel ID="rowLabel" runat="server" CssClass="form-group">
                                                <div class="col-sm-offset-2 col-sm-8">
                                                    <br />
                                                    <asp:Label runat="server" ID="lblMensaje" Font-Size="Large" />
                                                </div>
                                            </asp:Panel>
                                        </asp:Panel>
                                    </div>
                                    <div class="row" runat="server" visible="false" id="rowBusqueda">
                                        <div class="col-sm-8">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                <div class="col-offset-2 col-sm-4">
                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="table-responsive">
                                                <asp:GridView ID="gvDetails" CssClass="fixed_header table table-bordered tableDashboard" runat="server"
                                                    AutoGenerateColumns="false" DataKeyNames="RutaArchivo" OnRowDataBound="gvDetails_RowDataBound">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDownload" Text="Descargar" CssClass="btn btn-link btn-xs" runat="server" OnClick="lnkDownload_Click"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="vchDescripcion" HeaderText="Nombre del Archivo" />
                                                        <asp:BoundField DataField="FechaCarga" HeaderText="Fecha de Carga" />
                                                        <asp:BoundField DataField="Descripcion" HeaderText="Notas" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
