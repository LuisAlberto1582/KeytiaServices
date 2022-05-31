<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepTIMCarga.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepTIM.RepTIMCarga" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);
            // Place here the first init of the autocomplete
            InitAutoCompl();
        });

        function InitializeRequest(sender, args) {
        }

        function EndRequest(sender, args) {
            // after update occur on UpdatePanel re-init the Autocomplete
            InitAutoCompl();
        }

      <%--  function InitAutoCompl() {

            $(function () {
                var idText = document.getElementById('<%=txtBuscar.ClientID%>');
                $(idText).keyup(function () {
                    var val = $(this).val().toUpperCase();
                    $('#ReportePrincipal > tbody > tr').each(function (index, element) {
                        if ($(this).text().toUpperCase().indexOf(val) < 0)
                            $(this).hide();
                        else
                            $(this).show();
                    });
                });
            });

        };--%>

    </script>
    <style>
        .test {
            width: 250px
        }
        /* asi se crea una clase */
        .wrapper {
            background-color: white;
            width: 1075px;
            height: 100px;
            display: grid;
            grid-template-columns: 700px 700px;
        }

        .elementogrid {
            margin-top: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <%--<asp:Label runat="server" ID="Label1" CssClass="col-sm-2 control-label">Fecha Factura: </asp:Label>
    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control test"  AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged1">
    </asp:DropDownList>--%>
    <div class="row" runat="server" id="rowBusqueda">
        <div class="col-sm-6">
            <div class="form-group wrapper">
                <div class="elementogrid">
                    <asp:Label runat="server" ID="Label1" CssClass="col-sm-2 control-label">Fecha Factura: </asp:Label>
                    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control test" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged1">
                    </asp:DropDownList>
                </div>
              <%--  <div class="elementogrid">
                    <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                    <div class="col-offset-2 col-sm-4">
                        <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                    </div>
                </div>--%>
            </div>
        </div>
    </div>
    <div id="pnlMainHolder" runat="server">
        <div id="pnlRow_0" runat="server" cssclass="row">
            <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="caption">
                            <%--AQUI ESTA TOMANDO EL DE CARGA BAJAS --%>
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server"></span>
                        </div>
                        <div class="actions">
                            Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                </div>
                <div class="row" runat="server" id="pnlGrid">
                    <div class="col-sm-12">
                        <div style="overflow: auto;" runat="server" id="contenedor">
                        </div>
                    </div>
                </div>
                <div class="portlet-body" id="divContenedor">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12">
                                        </asp:Panel>
                                    </div>
                                </div>
                                <%--<div class="row" runat="server" id="rowBusqueda">--%>
                                <%--<div class="row" id="rowBusqueda">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                <div class="col-offset-2 col-sm-4">
                                                    <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>--%>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Panel ID="Rep2" runat="server" CssClass="col-md-12 col-sm-12">
                                        </asp:Panel>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Panel ID="Rep3" runat="server" CssClass="col-md-12 col-sm-12">
                                        </asp:Panel>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Panel ID="Rep4" runat="server" CssClass="col-md-12 col-sm-12">
                                        </asp:Panel>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Panel ID="Rep5" runat="server" CssClass="col-md-12 col-sm-12">
                                        </asp:Panel>
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
