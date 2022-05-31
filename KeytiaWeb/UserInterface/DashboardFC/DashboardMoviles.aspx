<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="DashboardMoviles.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.DashboardMoviles" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script type="text/javascript">
        var pagePath = window.location.pathname;
        $(function () {

            $("#" + "<%=txtConcepto.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaParaAutoComplateConcepto",
                        data: "{ 'concepto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataFilter: function (data) { return data; },
                        success: function (data) {

                            if (data.d.length == 0)
                                $("#spSearch").show();
                            else
                                $("#spSearch").hide();

                            response($.map(data.d, function (item) {
                                {
                                    return { label: item.Concepto, description: item.Descripcion }
                                }
                            }))
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) {
                            //alert(callStatus);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=txtConcepto.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtConceptoDescripcion.ClientID %>").val(ui.item.description);
                    $("#" + "<%=btnConcepto.ClientID %>").click();
                }
            });
        });



        function Validate() {
            var retVal = true;
            if ($("#spSearch").is(":visible"))
                retVal = false;
            if (!retVal)
                alerta("Datos no válidos");
            return retVal;
        }

        function alerta(mensaje) {
            $(document).ready(function () { jAlert(mensaje, "Advertencia"); });
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Panel ID="pnlIndicadores" runat="server" CssClass="row">
    </asp:Panel>

    <div class="clearfix"></div>
    <asp:Panel ID="pnlConsumos" runat="server" CssClass="row" Visible="false">
    </asp:Panel>
    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <button id="btnRegresar" runat="server" onserverclick="btnRegresar_Click" type="button" class="btn btn-default btn-circle btnBackDetail"><i class="far fa-arrow-alt-circle-left"></i></button>
                        <asp:Panel ID="pnlMapaNavegacion" runat="server">
                        </asp:Panel>
                    </div>
                    <div class="actions col-md-2 col-sm-2 col-lg-2 col-xs-2">
                        <p style="text-align: center;">
                            <img src="../../img/svg/Asset 22.svg" alt="">
                            Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" OnClick="btnExportarXLS_Click" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:LinkButton ID="btnAyuda" runat="server" CssClass="linkButtonHelpStyle" OnClick="btnAyuda_Click" Visible="false" Style="display: none">
        Ayuda
        <asp:Image ID="imgAyuda" runat="server" ImageUrl="~/images/Icono_ayuda.png" CssClass="imgStyleHelp" />
    </asp:LinkButton>

    <asp:Panel ID="pnlBusquedaCon" runat="server" CssClass="form-inline">
        <asp:Panel ID="pnlBusqConcepto" runat="server" CssClass="form-group" Visible="false">
            <div class="input-group">
                <asp:TextBox ID="txtConcepto" runat="server" Width="250px" CssClass="autosuggest placeholderstile form-control" onfocus="javascript:$(this).autocomplete('search','');"
                    placeholder="Buscar lineas por concepto" />
                <span class="input-group-addon">
                    <span class="glyphicon glyphicon glyphicon-search" aria-hidden="true"></span>
                </span>
            </div>
        </asp:Panel>
        <span id="spSearch" style="display: none; color: Red;">No se encontraron elementos</span>
        <div style="display: none">
            <asp:Button ID="btnConcepto" runat="server" Width="250px" PostBackUrl="~/UserInterface/DashboardFC/DashboardMoviles.aspx?Nav=ConsultaConcepto" />
            <asp:TextBox ID="txtConceptoDescripcion" runat="server"></asp:TextBox>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlCantMaxReg" runat="server" CssClass="form-inline" Style="margin-top: 5px;">
        <div class="form-group">
            <asp:Label ID="lblCantMaxReg" runat="server" CssClass="control-label">Cantidad máxima: </asp:Label>
            <div class="input-group">
                <asp:TextBox ID="txtCantMaxReg" runat="server" EnableViewState="false" CssClass="form-control" Width="100px" MaxLength="6" Style="margin-top: 5px;"></asp:TextBox>
                <span class="input-group-btn">
                    <asp:LinkButton ID="btnBuscar" runat="server" class="btn btn-keytia-sm" Height="30px">
                                            <span class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </span>
                <asp:RegularExpressionValidator ID="regexValtxtCantMaxReg" runat="server" ErrorMessage="* Capture un número valido"
                    ValidationExpression="\d*" ControlToValidate="txtCantMaxReg" SetFocusOnError="True"></asp:RegularExpressionValidator>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlMainHolder" runat="server">

        <asp:Panel ID="pnlHeaderAltReporte" runat="server" Width="100%">
            <asp:Label ID="lblHeaderAltReporte" runat="server" Width="100%"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_1_2" runat="server" CssClass="row">
            <asp:Panel ID="Rep1" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_3_4" runat="server" CssClass="row">
            <asp:Panel ID="Rep3" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep4" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_5_6" runat="server" CssClass="row">
            <asp:Panel ID="Rep5" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep6" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlRow_7_8" runat="server" CssClass="row">
            <asp:Panel ID="Rep7" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep8" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRow_10_11" runat="server" CssClass="row">
            <asp:Panel ID="Rep10" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

            <asp:Panel ID="Rep11" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRow_9" runat="server" CssClass="row">
            <asp:Panel ID="Rep9" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>
    </asp:Panel>


</asp:Content>
