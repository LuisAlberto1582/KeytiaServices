<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DepuracionDetalleCDR.aspx.cs" Inherits="KeytiaWeb.UserInterface.DepuracionDetalleCDR.DepuracionDetalleCDR" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        #divCargando {
            position: fixed;
            top: 0px;
            left: 0px;
            z-index: 3200;
            background: rgba(255,255,255,.5);
            width: 100%;
            height: 100%;
            display: none;
            padding-top: 15%;
            text-align: center;
        }

        .loader {
            border: 16px solid #f3f3f3;
            border-radius: 50% !important;
            border-top: 16px solid #696CAC;
            width: 150px;
            height: 150px;
            -webkit-animation: spin 2s linear infinite; /* Safari */
            animation: spin 2s linear infinite;
            margin: 0 auto;
        }
        /* Safari */
        @-webkit-keyframes spin {
            0% {
                -webkit-transform: rotate(0deg);
            }

            100% {
                -webkit-transform: rotate(360deg);
            }
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }

        #txtCargando
        {
            color: #58697D;
            font-size: 18pt !important;
            font-weight: bold;
        }
        
        .tablaMsg 
         {
            border-collapse:collapse;
            border-spacing:0;
            margin: 0 auto;
        }

        .tablaMsg th
        {
            padding:3px;
            border-style:solid;
            border-width:1px;
            border-color:#cccccc;
            background-color:#eeeeee;
            text-align: center;
        }

        .tablaMsg td
        {
            padding:3px;
            border-style:solid;
            border-width:1px;
            border-color:#cccccc;
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        function MostrarCargando(texto) {
            document.getElementById('divCargando').style.display = 'block';
            document.getElementById('txtCargando').innerHTML = texto;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <div class="portlet-title">
        <div class="caption">
            <i class="icon-bar-chart font-dark hide"></i>
            <span class="caption-subject titlePortletKeytia">Depuración y respaldo de DetalleCDR</span>
        </div>
        <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
            EnableScriptGlobalization="true">
        </asp:ToolkitScriptManager>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div id="divCargando">
        <span id="txtCargando"></span>
        <div class="loader"></div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="pnlPrincipal" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet-body">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" style="text-align: center;">
                        <div class="row">
                            <div class="table-responsive" style="width: 95%; margin: 0 auto;">
                                <asp:GridView ID="gvMeses" HeaderStyle-CssClass="tableHeaderStyle"
                                    CssClass="table table-bordered tableDashboard" runat="server"
                                    AutoGenerateColumns="false" GridLines="None">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Seleccionar">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSel" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Anio" HeaderText="Año" />
                                        <asp:BoundField DataField="Mes" HeaderText="Mes" />
                                        <asp:BoundField DataField="CantidadLlamadas" HeaderText="Cantidad de llamadas" />
                                        <asp:BoundField DataField="CostoTotal" HeaderText="Costo Total" DataFormatString="{0:C}" />
                                        <asp:BoundField DataField="CantidadMinutos" HeaderText="Cantidad de minutos" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <br />
                        <asp:Button ID="btnRespaldo" runat="server" Text="Iniciar respaldo" CssClass="btn btn-keytia-lg" OnClick="btnRespaldo_Click" OnClientClick="MostrarCargando('Respaldando meses seleccionados...')" />
                        <asp:Button ID="btnDepuracion" runat="server" Text="Iniciar depuracion" CssClass="btn btn-keytia-lg" OnClick="btnDepuracion_Click" OnClientClick="MostrarCargando('Depurando meses seleccionados...')" />
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <%--Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMensaje" runat="server" Text=""></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensaje"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMensaje" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMensaje" runat="server" PopupControlID="pnlPopupMensaje"
        TargetControlID="lnkBtnMensaje" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensaje">
    </asp:ModalPopupExtender>
</asp:Content>
