<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="DashboardTop10CCustodia.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.DashboardTop10CCustodia"
    ValidateRequest="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        /*GridViews*/.grvitemStyle
        {
            background-color: #D6C7A5;
            color: black;
            border-style: solid;
            border-color: Gray;
            border-width: thin;
        }
        .grvalternateItemStyle
        {
            background-color: #FFFFFF;
            color: black;
            border-style: solid;
            border-color: Gray;
            border-width: thin;
        }
        .grvheaderStyle
        {
            background-color: #D66100;
            color: #4A3000;
            font-weight: bold;
        }
        /*Label*/.titulos
        {
            font-family: verdana;
            color: #2E6E9E;
            font-weight: bold;
            font-size: large;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div align="center" style="font-family: Verdana; font-size: 11px; color: #000000">
        <br />
        <br />
        <div align="left" style="text-indent: 5%">
            <label class="titulos">
                Estadisticas de Cartas Custodia
            </label>
        </div>
        <br />
        <br />
        <div align="center">
            <div id="headerCCustAceptadas" align="left" style="text-indent: 10%">
                <label class="titulos">
                    Cartas Custodia Aceptadas</label>
            </div>
            <br />
            <asp:GridView ID="grvCCustAceptadas" runat="server" AutoGenerateColumns="false" Width="80%"
                CssClass="GridView">
                <%-- <RowStyle CssClass="grvitemStyle" />
                <HeaderStyle CssClass="grvheaderStyle" />
                <AlternatingRowStyle CssClass="grvalternateItemStyle" />--%>
                <Columns>
                    <asp:HyperLinkField HeaderText="Folio" DataNavigateUrlFields="Codigo Empleado" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc=&busq=top10ccust"
                        DataTextField="Folio" />
                    <asp:BoundField DataField="Estatus" HeaderText="Estatus" />
                    <asp:HyperLinkField HeaderText="No. Nomina" DataNavigateUrlFields="Codigo Empleado"
                        DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc=&busq=top10ccust"
                        DataTextField="No. Nomina" />
                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                    <asp:BoundField DataField="Ubicación" HeaderText="Ubicación" />
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha de ultima actualización" />
                </Columns>
                <RowStyle CssClass="GridRowOdd" />
                <AlternatingRowStyle CssClass="GridRowEven" />
            </asp:GridView>
        </div>
        <br />
        <br />
        <div align="center">
            <div id="Div2" align="left" align="left" style="text-indent: 10%">
                <label class="titulos">
                    Cartas Custodia Pendientes</label>
            </div>
            <br />
            <asp:GridView ID="grvCCustPendientes" runat="server" AutoGenerateColumns="false"
                CssClass="GridView" Width="80%">
                <%--  <RowStyle CssClass="grvitemStyle" />
                <HeaderStyle CssClass="grvheaderStyle" />
                <AlternatingRowStyle CssClass="grvalternateItemStyle" />--%>
                <Columns>
                    <asp:HyperLinkField HeaderText="Folio" DataNavigateUrlFields="Codigo Empleado" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc=&busq=top10ccust"
                        DataTextField="Folio" />
                    <asp:BoundField DataField="Estatus" HeaderText="Estatus" />
                    <asp:HyperLinkField HeaderText="No. Nomina" DataNavigateUrlFields="Codigo Empleado"
                        DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc=&busq=top10ccust"
                        DataTextField="No. Nomina" />
                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                    <asp:BoundField DataField="Ubicación" HeaderText="Ubicación" />
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha de ultima actualización" />
                </Columns>
                <RowStyle CssClass="GridRowOdd" />
                <AlternatingRowStyle CssClass="GridRowEven" />
            </asp:GridView>
        </div>
        <br />
        <br />
        <div align="center">
            <div id="Div1" align="left" align="left" style="text-indent: 10%">
                <label class="titulos">
                    Cartas Custodia Rechazadas</label>
            </div>
            <br />
            <asp:GridView ID="grvCCustRechazadas" runat="server" AutoGenerateColumns="false"
                CssClass="GridView" Width="80%">
                <%--      <RowStyle CssClass="grvitemStyle" />
                <HeaderStyle CssClass="grvheaderStyle" />
                <AlternatingRowStyle CssClass="grvalternateItemStyle" />--%>
                <Columns>
                    <asp:HyperLinkField HeaderText="Folio" DataNavigateUrlFields="Codigo Empleado" DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc=&busq=top10ccust"
                        DataTextField="Folio" />
                    <asp:BoundField DataField="Estatus" HeaderText="Estatus" />
                    <asp:HyperLinkField HeaderText="No. Nomina" DataNavigateUrlFields="Codigo Empleado"
                        DataNavigateUrlFormatString="~/UserInterface/CCustodiaDTI/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=tLaJQx5zLrc=&busq=top10ccust"
                        DataTextField="No. Nomina" />
                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                    <asp:BoundField DataField="Ubicación" HeaderText="Ubicación" />
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha de ultima actualización" />
                </Columns>
                <RowStyle CssClass="GridRowOdd" />
                <AlternatingRowStyle CssClass="GridRowEven" />
            </asp:GridView>
        </div>
        <br />
        <br />
    </div>
</asp:Content>
