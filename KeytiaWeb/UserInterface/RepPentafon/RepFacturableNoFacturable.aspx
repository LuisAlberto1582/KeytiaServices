<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="RepFacturableNoFacturable.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepPentafon.RepFacturableNoFacturable" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 125px;
            padding-right: 500px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }

        .table1 tr:nth-child(odd) td {
            background-color: #EEEEEE;
        }

        .table1 tr:nth-child(even) td {
            background-color: #EEEEEE;
        }
    </style>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

    <%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js"></script>--%>

    <%--    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/jquery.sumoselect/3.0.2/sumoselect.min.css">

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.sumoselect/3.0.2/jquery.sumoselect.min.js"></script>--%>

    <link rel="stylesheet" href="SumoSelect/sumoselect.min.css">
    <script type="text/javascript" src="SumoSelect/jquery.sumoselect.min.js"></script>
    <script type="text/javascript" src="SumoSelect/ExportExcel.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            <%--$(<%=lstBoxTest.ClientID%>).SumoSelect({ selectAll: true, okCancelInMulti: true });--%>
            $(<%=lstBoxCampanias.ClientID%>).SumoSelect({ selectAll: true });
        });
    </script>
    <script type="text/javascript">
        function exporta() {
            var value = document.getElementById("<%=cboAnio.ClientID%>");
            var anio = value.options[value.selectedIndex].text;
            var valueMes = document.getElementById("<%=cboMes.ClientID%>");
            var mes = valueMes.options[valueMes.selectedIndex].text;
            var periodo = mes + ' DE ' + anio;

            ExportToExcel(periodo);
        }
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
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Reporte Facturable y No Facturable</span>
                                </div>
                                <div class="actions">
                                    Exportar:&nbsp;<a class="exportExcel" onclick="exporta();" style="text-decoration: none"><i class="fas fa-file-excel"></i>&nbsp;Excel&nbsp;&nbsp;&nbsp;</a>
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
                                                        <div class="col-sm-12">
                                                            <div class="col-sm-4">
                                                                <asp:Panel runat="server" ID="Panel1" CssClass="form-group">
                                                                    <asp:Label runat="server" CssClass="col-sm-2 control-label">Periodo: </asp:Label>
                                                                    <div class="col-sm-6">
                                                                        <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="cboMes_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                    <div class="col-sm-4">
                                                                        <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion" AutoPostBack="true" OnSelectedIndexChanged="cboMes_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                            <div class="col-sm-8">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Label runat="server" CssClass="col-sm-2 control-label">Campañas:</asp:Label>
                                                                    <div class="col-sm-5">
                                                                        <asp:ListBox runat="server" ID="lstBoxCampanias" SelectionMode="Multiple" CssClass="form-control"></asp:ListBox>
                                                                    </div>
                                                                    <asp:Panel ID="pnlRangeFechas" runat="server" CssClass="form-group">
                                                                        <div class="col-sm-2">
                                                                            <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-sm" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-12" runat="server" id="dateFinal">
                                                            <div class="table-fixed-nz">
                                                                <table style="margin: 15px; color: black; font-size: 13px; font-family: 'Poppins',sans-serif" border="0" cellspacing="0" cellpadding="0" id="TableFacturable">
                                                                    <tr>
                                                                        <td>&nbsp;</td>
                                                                        <td>&nbsp;</td>
                                                                        <td>&nbsp;</td>
                                                                        <asp:PlaceHolder runat="server" ID="Campanias"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>&nbsp;</td>
                                                                        <td>&nbsp;</td>
                                                                        <td style="vertical-align: middle; text-align: right; border: 1px solid #CCCCCC; width: 10px; background-color: #EEEEEE;">
                                                                            Carrier&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="FacturableNoFacturable"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotalGeneral"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td rowspan="3" style="vertical-align: middle; text-align: center; padding: 4.5px; border: 1px solid #CCCCCC; background-color: #EEEEEE">
                                                                            <p>VARIABLE</p>
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center; border: 1px solid #CCCCCC; background-color: #696CAC; color: white;">
                                                                            <p>VOZ</p>
                                                                        </td>
                                                                        <td>
                                                                            <table style="border-collapse: collapse; padding: 10px;">
                                                                                <tr>
                                                                                    <td style="vertical-align: middle; text-align: center; padding:17px; background-color: #EEEEEE;">Inbound
                                                                                    </td>
                                                                                    <td>
                                                                                        <table style="border-collapse: collapse; width: 125px; height: 170px; text-align: center;" cellspacing="0" cellpadding="0">
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Bestel</td>
                                                                                            </tr>
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Marcatel</td>
                                                                                            </tr>
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Protel</td>
                                                                                            </tr>
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Alestra</td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="vertical-align: middle; text-align: center; border-top: 1px solid #CCCCCC; padding: 17px; background-color: #EEEEEE;">Outbound
                                                                                    </td>
                                                                                    <td>
                                                                                        <table style="border-collapse: collapse; width: 125px; height: 170px; text-align: center;" cellspacing="0" cellpadding="0">
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Bestel</td>
                                                                                            </tr>
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Marcatel</td>
                                                                                            </tr>
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Protel</td>
                                                                                            </tr>
                                                                                            <tr style="background-color: #EEEEEE;border: 1px solid #CCCCCC">
                                                                                                <td>Alestra</td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="vertical-align: middle; text-align: center; border-right: 1px solid #CCCCCC; border-top: 1px solid #CCCCCC; padding: 4.5px; background-color: #EEEEEE;">Buzon
                                                                                    </td>
                                                                                    <td style="border: 1px solid #CCCCCC; background-color: #EEEEEE; text-align: center;">Smarttelco</td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="datosFacturableNoFacturable"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotGeneralVoz"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding: 10px; vertical-align: middle; text-align: center; border: 1px solid #CCCCCC; background-color: #696CAC; color: white;" colspan="2">
                                                                            SMS
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="datosFacturableNoFacturableSMS"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotSMS"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding: 10px; vertical-align: middle; text-align: center; border: 1px solid #CCCCCC; background-color: #696CAC; color: white;" colspan="2">
                                                                            MAIL
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="datosFacturableNoFacturableMail"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotEmail"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td rowspan="4" style="vertical-align: middle; text-align: center; padding: 4.5px; border: 1px solid #CCCCCC; background-color: #EEEEEE">
                                                                            <p>FIJO</p>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="vertical-align: middle; text-align: right; background-color: #696CAC; color: white; border-bottom: 1px solid #CCCCCC">
                                                                            Enlaces,L2L,MPLS
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center; border-bottom: 1px solid #CCCCCC;">
                                                                            <table style="border-collapse: collapse; padding: 10px;">
                                                                                <tr>
                                                                                    <td style="vertical-align: middle; text-align: left; border-right: 1px solid #CCCCCC; padding:18px; color: #FFFFFF; color: #696CAC; background-color: #696CAC;">outbound
                                                                                    </td>
                                                                                    <td>
                                                                                        <table style="border-collapse: collapse; width: 125px; height: 170px; text-align: center;background-color: #EEEEEE;" cellspacing="0" cellpadding="0">
                                                                                            <asp:PlaceHolder runat="server" ID="CarriersEnlace"></asp:PlaceHolder>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="DatosFacturaEnlaces"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotalGeneralEnlace"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="vertical-align: middle; text-align: center; border-bottom: 1px solid #CCCCCC; background-color: #696CAC; color: white;">HO
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center; border-top: 1px solid  #CCCCCC; border-bottom: 1px solid #CCCCCC;">
                                                                            <table style="border-collapse: collapse; padding: 10px;">
                                                                                <tr>
                                                                                    <td style="vertical-align: middle; text-align: left; padding: 18.5px; color: #696CAC; background-color: #696CAC;">
                                                                                        outbound
                                                                                    </td>
                                                                                    <td>
                                                                                        <table style="border-collapse: collapse; width: 125px; height: 170px; text-align: center;background-color: #EEEEEE;" cellspacing="0" cellpadding="0">
                                                                                            <asp:PlaceHolder runat="server" ID="CarriersHO"></asp:PlaceHolder>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td >
                                                                        <asp:PlaceHolder runat="server" ID="DatosFacturaHo"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotGeneralHO"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="vertical-align: middle; text-align: left; padding: 4.5px; border-bottom: 1px solid #CCCCCC; background-color: #696CAC; color: white;">&nbsp;Servicios Adicionales
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center; border-bottom: 1px solid #CCCCCC;">
                                                                            <table style="border-collapse: collapse; padding: 10px;">
                                                                                <tr>
                                                                                    <td style="vertical-align: middle; text-align: center; padding: 18.5px; color: #696CAC; background-color: #696CAC;">
                                                                                        outbound
                                                                                    </td>
                                                                                    <td>
                                                                                        <table style="border-collapse: collapse; width: 125px; height: 170px; text-align: center;background-color: #EEEEEE;" cellspacing="0" cellpadding="0">
                                                                                            <asp:PlaceHolder runat="server" ID="CarriersServAdic"></asp:PlaceHolder>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="DatosServAdic"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotalGeneralServAdic"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" style="vertical-align: middle; text-align: center; padding: 4.5px; background-color: #696CAC; color: white;">Total
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center;">
                                                                            <table style="border-collapse: collapse; padding: 10px;">
                                                                                <tr>
                                                                                    <td>
                                                                                        <table style="border-collapse: collapse; width: 226px; height: 170px; text-align: center;background-color: #EEEEEE;" cellspacing="0" cellpadding="0">
                                                                                            <asp:PlaceHolder runat="server" ID="CarriersTotales"></asp:PlaceHolder>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="TotalCarriers"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotalGeneralCarriers"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" style="vertical-align: middle; text-align: center; padding: 4.5px; background-color: #696CAC">&nbsp;
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center; border: 1px solid #CCCCCC; padding: 4.5px; background-color: #696CAC; color: white;">Sub Total:
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="Datostotales"></asp:PlaceHolder>
                                                                        <asp:PlaceHolder runat="server" ID="TotalGeneralCamp"></asp:PlaceHolder>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" style="vertical-align: middle; text-align: center; padding: 4.5px; background-color: #696CAC">&nbsp;
                                                                        </td>
                                                                        <td style="vertical-align: middle; text-align: center; border: 1px solid #CCCCCC; padding: 4.5px; background-color: #696CAC; color: white;">Total:
                                                                        </td>
                                                                        <asp:PlaceHolder runat="server" ID="SumaFactNoFactCamp"></asp:PlaceHolder>
                                                                    </tr>
                                                                </table>
                                                                <!--final de la tabla principal--->
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
        <Triggers>
            <asp:PostBackTrigger ControlID="cboMes" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="cboAnio" />
        </Triggers>
        <%--        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportarXLS" />
        </Triggers>--%>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnAplicarFecha" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        window.onsubmit = function () {
            var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
            window.setTimeout(function () {
                updateProgress.set_visible(true);
            }, 100);

        }
    </script>
</asp:Content>
