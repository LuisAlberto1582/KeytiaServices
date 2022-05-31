<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Resultados.aspx.cs" MasterPageFile="~/KeytiaOH.Master" Inherits="KeytiaWeb.UserInterface.DirCorporativo.Resultados"  %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="ctHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="ctTitle" ContentPlaceHolderID="cphTitle" runat="server">
    
</asp:Content>
<asp:Content ID="ctContent" ContentPlaceHolderID="cphContent" runat="server">

    <div style="text-align:center" >
        <asp:PlaceHolder id="divDetallado" runat="server" Visible="true">
    
            <asp:Table ID="DETALLE" runat="server"  CellPadding="5" Width="70%" HorizontalAlign="Center" >
                <asp:TableHeaderRow >
                    <asp:TableCell ColumnSpan="4" HorizontalAlign ="Center" >
                        <asp:Label ID="Label2" runat="server" Text="DATOS DEL EMPLEADO"></asp:Label>
                    </asp:TableCell>
                </asp:TableHeaderRow>
                <asp:TableRow Width="100%" >
                    <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallNombre" runat="server">Nombre(s)</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallNombre" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>   
                    <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallTipoEmple" runat="server">Tipo de Empleado</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallTipoEmple" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>          
                </asp:TableRow>
                <asp:TableRow>
                   <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallPaterno" runat="server">Apellido Paterno</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallPaterno" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>   
                    <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallSitio" runat="server">Sitio</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallSitio" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>              
                </asp:TableRow>
                    <asp:TableRow>
                   <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallMaterno" runat="server">Apellido Materno</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallMaterno" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>   
                    <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallDepto" runat="server">Departamento</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallDepto" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>            
                </asp:TableRow>
                <asp:TableRow>
                   <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallEmail" runat="server">Email</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallEmail" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>   
                    <asp:TableCell HorizontalAlign ="Right" Width="20%">
                        <asp:Label ID="detallEmpresa" runat="server">Empresa</asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="30%">
                        <asp:Label Font-Bold="true" ID="txtDetallEmpresa" align="Center" runat="server" Width="35%"></asp:Label>
                    </asp:TableCell>
                          
                </asp:TableRow>
                </asp:Table>
                <br>
                
                <asp:Table ID="Table2" runat="server"  CellPadding="5" Width="70%" HorizontalAlign="Center">
                 <asp:TableHeaderRow BackColor="#990000" ForeColor="White">
                    <asp:TableCell HorizontalAlign="Center" Width="30%">Extensiones
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center" Width="40%">Sitios
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center" Width="30%">Radio
                    </asp:TableCell>
                 </asp:TableHeaderRow>
                 <asp:TableRow Visible ="false" ID = "r1">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r2">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r3">   <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r4">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r5">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r6">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r7">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r8">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r9">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow Visible ="false" ID = "r10">
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                 </asp:TableRow>
                 <asp:TableRow>
                    <asp:TableCell HorizontalAlign ="Center" ColumnSpan="3">
                        <asp:Button ID="btnBuscarOtro" runat="server" Text="Buscar Otro" OnClick="btnBuscarOtro_Click"  />
                    </asp:TableCell> 
                </asp:TableRow>
                
            </asp:Table> 
            
           
        </asp:PlaceHolder>
        </div>
    
    
</asp:Content>