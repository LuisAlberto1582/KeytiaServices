<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConfigurarEmpleGerente.aspx.cs" Inherits="KeytiaWeb.UserInterface.Administracion.ConfigurarEmpleGerente" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 9999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }

        .classButton {
            background-color: transparent;
            width: 0px;
            height: 0px;
            border: none;
            display: none;
        }

        .checkBox {
            text-align: center;
        }

        .selected {
            background-color: #A1DCF2;
        }
    </style>
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

        function InitAutoCompl() {

            $("[id*=chkHeader]").live("click", function () {
                var chkHeader = $(this);
                var grid = $(this).closest("table");
                $("input[type=checkbox]", grid).each(function () {
                    if (chkHeader.is(":checked")) {
                        $(this).attr("checked", "checked");
                        $("td", $(this).closest("tr")).addClass("selected");
                    } else {
                        $(this).removeAttr("checked");
                        $("td", $(this).closest("tr")).removeClass("selected");
                    }
                });
            });

            $("[id*=chkRow]").live("click", function () {
                var grid = $(this).closest("table");
                var chkHeader = $("[id*=chkHeader]", grid);
                if (!$(this).is(":checked")) {
                    $("td", $(this).closest("tr")).removeClass("selected");
                    chkHeader.removeAttr("checked");
                } else {
                    $("td", $(this).closest("tr")).addClass("selected");
                    if ($("[id*=chkRow]", grid).length == $("[id*=chkRow]:checked", grid).length) {
                        chkHeader.attr("checked", "checked");
                    }
                }
            });

            $(function () {
                var idText = document.getElementById('<%=txtBuscar.ClientID%>');
                var grid = '<%=grvEmples.ClientID%>';
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

        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="modalUpload">
                <div class="centerUpload">
                    <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Crear Nuevo Cliente</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="col-sm-5">
                                                <div class="panel panel-default">
                                                    <div class="panel-heading">
                                                        <strong>Cambiar Empleado a Gerente </strong>
                                                    </div>
                                                    <div class="panel-body">
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-7">
                                                <div class="panel panel-default">
                                                    <div class="panel-heading">
                                                        <strong>Quitar Empleado de Gerente</strong>
                                                    </div>
                                                    <div class="panel-body">
                                                        <div class="row" runat="server" id="rowBusqueda" visible="false">
                                                            <div class="col-sm-12">
                                                                <div class="form-group">
                                                                    <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar: </asp:Label>
                                                                    <div class="col-offset-2 col-sm-8">
                                                                        <asp:TextBox runat="server" ID="txtBuscar" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row" runat="server" id="rowGrid" visible="false">
                                                            <div class="col-sm-12">
                                                                <div class="table-fixed-nz">
                                                                    <asp:GridView runat="server" ID="grvEmples" CssClass="fixed_header table table-bordered tableDashboard"
                                                                        AutoGenerateColumns="false" DataKeyNames="icodEmple">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small" HeaderStyle-Width="10" ItemStyle-Width="10" ItemStyle-CssClass="checkBox">
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="chkHeader" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox runat="server" ID="chkRow" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="NominaA" HeaderText="Nomina" />
                                                                            <asp:BoundField DataField="Nomcompleto" HeaderText="Nombre" />
                                                                            <asp:BoundField DataField="vchDescripcion" HeaderText="Puesto" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row" runat="server" id="rowBoton" visible="false">
                                                            <div class="col-sm-12">
                                                                <div class="col-offset-4 col-sm-8">
                                                                    <asp:Button runat="server" ID="btnAceptar" CssClass="btn btn-keytia-sm" Text="Aceptar" OnClick="btnAceptar_Click"/>
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
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
