<%@ Page Language="C#" MasterPageFile="~/KeytiaOV.Master" Async="true" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Busquedas.aspx.cs" Inherits="KeytiaWeb.UserInterface.Busquedas" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="ctHead" ContentPlaceHolderID="cphHead" runat="server">
    <script src="Busquedas.aspx.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="ctTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Label ID="lblTitle" runat="server" CssClass="page-title-keytia"></asp:Label>
</asp:Content>
<asp:Content ID="ctContent" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="Content" runat="server">
        <asp:Panel ID="pnlBusqueda" runat ="server" >
            <asp:Panel ID="pnlToolBar" runat ="server" >
                <asp:Table ID="Table1" runat="server" Style="width:100%;" >
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell Style="width:100%">
                        </asp:TableHeaderCell>
                        <asp:TableHeaderCell>
                        </asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:Panel>
            <asp:Table ID="Table2" runat="server" Style="width:100%;" >
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlConsulta" runat ="server" >
        </asp:Panel>
    </asp:Panel>
</asp:Content>
