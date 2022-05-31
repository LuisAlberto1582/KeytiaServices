<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ReporteUsuario.aspx.cs" Inherits="KeytiaWeb.UserInterface.ReporteUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Label ID="lblTitulo" runat="server" Text="Label"></asp:Label>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="Content" runat="server" Width="99%" style="display:none">
        <asp:Panel ID="Buttons" runat="server" Width="100%" CssClass="fg-toolbar" style="height:30px">
        </asp:Panel>
        <asp:TextBox ID="txtReportData" runat="server" TextMode="MultiLine" Rows="10" Columns="150" style="display:none" EnableViewState="false"></asp:TextBox>
        <asp:TextBox ID="txtLangData" runat="server" TextMode="MultiLine" Rows="10" Columns="150" style="display:none" EnableViewState="false"></asp:TextBox>
        <asp:TextBox ID="txtPrevData" runat="server" TextMode="MultiLine" Rows="10" Columns="150" style="display:none" EnableViewState="false"></asp:TextBox>
        
    </asp:Panel>
</asp:Content>
