<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="JerarquiaCencosto.aspx.cs" Inherits="KeytiaWeb.UserInterface.JerarquiaCencosto.JerarquiaCencosto" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;
        $(function () {
            $("#" + "<%=txtCencosto.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetCencos",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.idCencos };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    $("#" + "<%=txtCencosto.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtCencosId.ClientID %>").val(ui.item.description);
                }
            });
        });

    </script>
    <style>
        .node {
            cursor: pointer;
        }

            .node circle {
                fill: #fff;
                stroke: steelblue;
                stroke-width: 1.5px;
            }

        .found {
            fill: #ff4136;
            stroke: #ff4136;
        }

        .node text {
            font: 10px sans-serif;
        }

        .link {
            fill: none;
            stroke: #ccc;
            stroke-width: 1.5px;
        }

        .search {
            width: 100%;
        }
    </style>
    <script>
        function imprim1() {
            document.getElementById("CencosJer").contentWindow.print();
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
        <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <div id="pnlMainHolder" runat="server">
        <div id="pnlRow_0" runat="server" cssclass="row">
            <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Jerarquia Centro de Costos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body" id="divContenedor">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-horizontal" role="form" runat="server" id="formNomina">
                                                        <asp:Panel ID="Panel1" runat="server" CssClass="form-group">
                                                            <asp:Label ID="lblCencostos" runat="server" CssClass="col-sm-4 control-label">Centro de costos:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtCencosto" runat="server" CssClass="autosuggest placeholderstile form-control"
                                                                    onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Centro de Costos" />
                                                                <div style="display: none">
                                                                    <asp:TextBox ID="txtCencosId" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-horizontal">
                                                        <asp:Panel ID="rowBtnBuscar" runat="server" CssClass="form-group">
                                                            <div class="col-sm-6">
                                                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-keytia-sm" OnClick="btnBuscar_Click"/>
                                                                &nbsp;&nbsp;<asp:Button ID="btnLimpiar" runat="server" Text="Borrar Filtros" CssClass="btn btn-keytia-sm" OnClick="btnLimpiar_Click"/>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </div>
                                        <div class="row">
                                                <div class="col-sm-8">
                                                    <asp:Panel ID="rowGuardar" runat="server" CssClass="form-group">
                                                        <div class="col-sm-6">
                                                            <asp:Button ID="btnDescargar" runat="server" CssClass="btn btn-keytia-sm" Text="Descargar" OnClick="btnDescargar_Click" />
                                                            &nbsp;&nbsp;
                                                            <button type="button" onclick="javascript:imprim1();" class="btn btn-keytia-sm">Imprimir</button>
                                                        </div>                                                     
                                                    </asp:Panel>
                                                </div>
                                        </div>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:PlaceHolder ID="iframeDiv" runat="server" />
                                                    <div id="search"></div>
                                                    <asp:HiddenField runat="server" ID="hdfPath" />
                                                    <script type="text/javascript">                                                      
                                                        var fileJason = document.getElementById("<%=hdfPath.ClientID%>").value;
                                                        sessionStorage.clear();
                                                        sessionStorage.setItem('FileJson', fileJason);
                                                    </script>
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
                <%--NZ: Modal para mensajes--%>
            <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>                          
                        </div>
                        <div class="modal-footer">                             
                            <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm"/>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
            <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
                TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
            </asp:ModalPopupExtender>
</asp:Content>
