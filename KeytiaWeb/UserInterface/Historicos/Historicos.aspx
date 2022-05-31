<%@ Page Title="" Language="C#" EnableEventValidation="false" Async="true" MasterPageFile="~/KeytiaOV.Master" AutoEventWireup="true" CodeBehind="Historicos.aspx.cs" Inherits="KeytiaWeb.UserInterface.Historicos" validateRequest="false" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="ctHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="ctTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Label ID="lblTitle" runat="server" CssClass="page-title-keytia"></asp:Label>
</asp:Content>
<asp:Content ID="ctContent" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="Content" runat="server" Width="99%"></asp:Panel>
</asp:Content>
