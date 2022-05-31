<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="BusquedaExternaCCustodia.aspx.cs" Inherits="KeytiaWeb.BusquedaExternaCCustodia.BusquedaExternaCCustodia" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">

<div align="center" >
    <asp:Label ID="lblBusquedaCCustodia" runat="server" Text="Búsqueda de Cartas Custodia" Font-Bold="true">
    </asp:Label>
</div> 

<br />
<br />

<div>
    <asp:Table ID="tblBusquedaCCustodia" runat="server" CaptionAlign="Top" 
        HorizontalAlign="Center">

        
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblFolio" runat="server" Text="Folio:">   
                </asp:Label>
            </asp:TableCell>
            
            <asp:TableCell>
                    <asp:TextBox ID="txtFolio" runat="server">
                    </asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblExtension" runat="server" Text="Extensión:">   
                </asp:Label>
            </asp:TableCell>
            
            <asp:TableCell>
                <asp:TextBox ID="txtExtension" runat="server">
                </asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblNombre" runat="server" Text="Nombre(s):">   
                </asp:Label>
            </asp:TableCell>
            
            <asp:TableCell>
                <asp:TextBox ID="txtNombre" runat="server">
                </asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblApellidos" runat="server" Text="Apellidos (paterno y/o materno):">   
                </asp:Label>
            </asp:TableCell>
            
            <asp:TableCell>
                <asp:TextBox ID="txtApellidos" runat="server">
                </asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>  
        
    </asp:Table>
 </div>
 
<br />
<br />

<div align="center">
    <asp:Button ID="btnBuscarCCustodia" runat="server" Text="Buscar" 
        onclick="btnBuscarCCustodia_Click" />
</div>   

<br />
<br />
<br />

<div>
    <asp:Table ID="tblResultados" runat="server" CaptionAlign="Left" 
        HorizontalAlign="Center">
            <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblCartasEncontradas" runat="server" Text="Cartas encontradas: " Font-Bold="true">
                </asp:Label>
            </asp:TableCell>
            
            <asp:TableCell>
                <asp:Label ID="lblCartasEncontradasCount" runat="server" Text="">
                </asp:Label>
            </asp:TableCell>
            </asp:TableRow>
    </asp:Table>    
</div>

<br />
<br />

<div>
        <asp:GridView ID="grvCCustodia" runat="server" CellPadding="4" GridLines="None" 
                      AutoGenerateColumns="false" ShowFooter="true" 
            HeaderStyle-BackColor="#FF6600" HeaderStyle-ForeColor="White" 
            HorizontalAlign="Center">
            <Columns>
                <asp:BoundField DataField="FolioCCustodia" HeaderText="Folio" HtmlEncode = "true" />                
                <asp:HyperLinkField  HeaderText="Nombre"
                                     DataNavigateUrlFields="EmpleCatalogo"
                                     DataNavigateUrlFormatString="~/UserInterface/CCustodia/AppCCustodia.aspx?Opc=OpcAppCCustodia&iCodEmple={0}&st=jq9g9FcOjmI="
                                     DataTextField="NomCompleto"  /> 
                                                                        
<%--                <asp:BoundField DataField="NomCompleto" HeaderText="Nombre" HtmlEncode = "true" />--%>               
                <asp:BoundField DataField="Email" HeaderText="Email" HtmlEncode = "true" />
            </Columns>
            <HeaderStyle BackColor="#999999" ForeColor="White" />
        </asp:GridView>  
</div>

<br />
<br />

<div align="center">
    <asp:Button ID="btnRegresar" runat="server" Text="Regresar" Visible="false" 
        onclick="btnRegresar_Click"/>
</div>  

</asp:Content>
