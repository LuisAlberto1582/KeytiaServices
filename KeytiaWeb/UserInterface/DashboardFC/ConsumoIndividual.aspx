<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConsumoIndividual.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividual" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
        var pagePath = window.location.pathname;
        var dataJSON;

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

            $("#" + "<%=txtEmple.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetEmple",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.ID };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=hdfIcodEmple.ClientID %>").val(ui.item.description);
                    $("#" + "<%=txtEmple.ClientID %>").val(ui.item.label);
                }
            });
        };
    </script>
    <style>
        .color {
            background-color: #00B1EB; 
        }
       /* asi se crea una clase 
        .wrapper {
            background-color: white;
            width: 1075px;
            height: 100px;
            display: grid;
            grid-template-columns: 700px 700px;
        }*/

        .elementogrid{
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
            <div id="pnlRow_0" runat="server" cssclass="row">
                <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet-body" id="divContenedor">
                            <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">                                
                                <div class="row" runat="server" id="rowFechas" visible="false">
                                    <br />
                                    <div class="col-sm-12">
                                        <div class="col-sm-8">
                                            <asp:Panel runat="server" ID="row" CssClass="form-group">
                                                <asp:Label runat="server" CssClass="col-sm-5 control-label">Periodo Facturación: </asp:Label>
                                                <div class="col-sm-4">
                                                    <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                                <div class="col-sm-3">
                                                    <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion"></asp:DropDownList>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </div>
                                <div class="row" runat="server" id="rowBusqueda" visible="false">
                                    <div class="col-sm-12">
                                        <div class="col-sm-2">
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <asp:Label ID="lblBuscar" runat="server" CssClass="col-sm-3 control-label">Buscar:</asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtEmple" CssClass="form-control"></asp:TextBox>
                                                    <asp:HiddenField runat="server" ID="hdfIcodEmple" />
                                                </div>
                                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-keytia-sm color " OnClick="btnBuscar_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:PlaceHolder ID="iframeDiv" runat="server" />
                                </div>
                            </div>
                        </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnBuscar" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
