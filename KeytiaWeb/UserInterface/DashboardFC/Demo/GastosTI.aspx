<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="GastosTI.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.Demo.GastosTI" %>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div id="main" style="width: 100%; height: 100%">
        <iframe src="https://datastudio.google.com/embed/reporting/23c1e1ae-3dac-4783-89bd-5ef1051c7316/page/wmSrC">
        </iframe>
    </div>
</asp:Content>
