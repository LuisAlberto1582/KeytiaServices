<%@ Page Language="C#" MasterPageFile="~/KeytiaOV.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Relaciones.aspx.cs" Inherits="KeytiaWeb.UserInterface.Relaciones" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="ctHead" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../RegEdit.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="ctTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Label ID="lblTitle" runat="server" CssClass="page-title-keytia"></asp:Label>
</asp:Content>
<asp:Content ID="ctContent" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="Content" runat="server"></asp:Panel>
</asp:Content>
