<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="GestionDeServicios.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.TIM.GestionDeServicios" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .footer {
            display: none;
        }

        br {
            display: none;
        }

        iframe {
            border: none !important;
            height: 840px !important;
            width: 100% !important;
        }
    </style>
    <script type="text/javascript">
        //$(window).load(function () {
        //    $("body").find(".sidebar-toggler.btnTogglerBar").click();
        //});
        function showContent() {
        $(window).load(function () {
            $("body").find(".sidebar-toggler.btnTogglerBar").click();
        });
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
     <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

   <%-- <%--Barra
    <asp:Panel ID="pToolBar" runat="server" CssClass="navbar navbar-default">
        <div class="container-fluid">
            <div class="navbar-btn col-sm-3">
                <asp:DropDownList ID="ddlApps" runat="server" CssClass="form-control btn-sm" data-style="btn-default" Height="30px" AutoPostBack="true" OnSelectedIndexChanged="Page_Load">
                </asp:DropDownList>
            </div>
        </div>
    </asp:Panel>--%>

    <%--Reportes--%>
    <div id="main" style="width: 100%; height: 100%">
        <%--Reportes--%>
        <iframe src="https://servintory.herokuapp.com">

        </iframe>
    </div>

    <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMsn" runat="server" Text="Acceso Restringido"></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblBodyModalMsn" runat="server" Text="El usuario no tiene autorización de visualizar este contenido."></asp:Label>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
        TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
    </asp:ModalPopupExtender>
</asp:Content>
