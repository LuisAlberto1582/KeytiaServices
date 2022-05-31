<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="DescargaReportes.aspx.cs" Inherits="KeytiaWeb.UserInterface.Entregables.DescargaReportes" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <asp:Panel ID="pnlRow_1_2" runat="server" CssClass="row">
        <asp:Panel ID="pnlReportesDisp" runat="server" CssClass="col-md-6 col-sm-6"></asp:Panel>

        <asp:Panel ID="Rep2" runat="server" CssClass="col-md-6 col-sm-6">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                        <span class="caption-subject titlePortletKeytia">Descarga</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                        <div class="form-group">
                            <asp:Label runat="server" CssClass="col-sm-4 control-label">Año:</asp:Label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="cmbAnio" runat="server" CssClass="form-control" >
                                </asp:DropDownList>
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" CssClass="col-sm-4 control-label">Mes:</asp:Label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="cmbMes" runat="server" CssClass="form-control" >
                                </asp:DropDownList>
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" CssClass="col-sm-4 control-label">Tipo Reporte:</asp:Label>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="cmbTipoReporte" runat="server" OnSelectedIndexChanged="cmbTipoReporte_SelectedIndexChanged"
                                    AutoPostBack="true" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                        </div>
                         <div class="modal-footer">
                             <asp:Button ID="btnAceptar" runat="server" Text="Descargar" OnClick="btnAceptar_Click" CssClass="btn btn-keytia-md" />
                        </div>
                        
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>

    <%--<asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <table>
            <tr>
                <td width="55%" valign="top">
                    <table>
                        <tr>
                            <td>
                                <asp:Panel ID="" runat="server">
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="10%"></td>
                <td width="30%" valign="top">
                    <table>
                        <tr>
                            <td>Año:
                            </td>
                            <td>
                                
                            </td>
                        </tr>
                        <tr>
                            <td>Mes:
                            </td>
                            <td>
                                
                            </td>
                        </tr>
                        <tr>
                            <td>Tipo de reporte:
                            </td>
                            <td>
                                
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                               
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblnombreArchivo" runat="server" EnableViewState="False"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>--%>
</asp:Content>
