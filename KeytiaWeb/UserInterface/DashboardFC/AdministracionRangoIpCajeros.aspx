<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AdministracionRangoIpCajeros.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.AdministracionRangoIpCajeros" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

<script type="text/javascript">
    function MostrarMensajesDeError(mensaje) {
        alert("El campo de IP es requerido.");
    }
</script>

<script type="text/javascript">
    function MostrarMensajesDeErrorIp(mensaje) {
        alert("Favor de ingresar una IP valida.");
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server"></span>
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
                                                    <div class="col-sm-offset-5 col-sm-10">
                                                        <asp:Button ID="btnAgregarRangoIp" runat="server" Text="Agregar nueva IP" CssClass="btn btn-keytia-lg"  onclick="AgregarRangoIp_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:Panel ID="Rep2" runat="server" CssClass="col-md-12 col-sm-12">
                                                        <div ID="agregadoRangoIpDiv" runat="server">

                                                            <asp:Panel ID="Panel1" runat="server">
                                                                <asp:Panel ID="Panel2" runat="server" CssClass="row">
                                                                    <asp:Panel ID="Panel3" runat="server" CssClass="col-md-12 col-sm-12">
                                                                        <div class="portlet solid bordered">
                                                                            <div class="portlet-title">
                                                                                <div class="caption">
                                                                                    <i class="icon-bar-chart font-dark hide"></i>
                                                                                    <span class="caption-subject titlePortletKeytia">Administración de direcciones IP</span>
                                                                                </div>
                                                                                <div class="actions">
                                                                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                                                                </div>
                                                                            </div>
                                                                            <div class="portlet-body">
                                                                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                                                                    <form name="FR_Busquedas" method="post" class="form-horizontal" role="form">

                                                                                        <asp:Panel ID="rowEmple" runat="server" CssClass="form-group">
                                                                                            <asp:Label ID="lblEmpleado" runat="server" CssClass="col-sm-2 control-label">Dirección IP:</asp:Label>
                                                                                            <div class="col-sm-8">
                                                                                                <asp:TextBox TextMode="MultiLine" ID="txtNombre" runat="server" CssClass="autosuggest placeholderstile form-control" placeholder="Dirección IP" />
                                                                                                <div style="display: none">
                                                                                                    <asp:TextBox ID="txtEmpleId" runat="server"></asp:TextBox>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:Panel>

                                                                                        <asp:Panel ID="rowUbicacion" runat="server" CssClass="form-group">
                                                                                            <asp:Label ID="lblUbicacion" runat="server" CssClass="col-sm-2 control-label">Tipo de enlace:</asp:Label>
                                                                                            <div class="col-sm-8">
                                                                                                <asp:DropDownList ID="cboTipoEnlace" runat="server" CssClass="form-control">
                                                                                                    <asp:listitem text="Enlace Terrestre" value="Enlace Terrestre"></asp:listitem>
                                                                                                    <asp:listitem text="Enlace Satelital" value="Enlace Satelital"></asp:listitem>
                                                                                                    <asp:listitem text="Enlace Celular" value="Enlace Celular"></asp:listitem>
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </asp:Panel>

                                                                                        <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                                                                            <div class="col-sm-offset-5 col-sm-10">
                                                                                                <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg"  onclick="AgregarIp_Click"/>
                                                                                                <asp:Button ID="Button3" runat="server" Text="Cancelar" CssClass="btn btn-keytia-lg"  onclick="CancelarAgregadoIp_Click" />
                                                                                            </div>
                                                                                        </asp:Panel>
                                                                                    </form>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                </asp:Panel>
                                                                <asp:Panel ID="pnlRow_9" runat="server" CssClass="row">
                                                                    <asp:Panel ID="Rep9" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
                                                                </asp:Panel>
                                                            </asp:Panel>

                                                            <%--<asp:Button ID="Button1" runat="server" onclick="AgregarIp_Click" CssClass="btn btn-primary btn-sm" Text="Guardar rango" />
                                                            <asp:Button ID="Button2" runat="server" onclick="CancelarAgregadoIp_Click" CssClass="btn btn-primary btn-sm" Text="Cancelar" />--%>
                                                        </div>
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
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--<div ID="agregadoRangoIpDiv" runat="server">
        <h1>AGREGADO DE RANGO IP</h1>
        <asp:Button ID="Button1" runat="server" onclick="AgregarIp_Click" CssClass="btn btn-primary btn-sm" Text="Agregar nuevo enlace" />
    </div>--%>
</asp:Content>

