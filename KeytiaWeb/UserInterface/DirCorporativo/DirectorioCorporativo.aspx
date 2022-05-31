<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DirectorioCorporativo.aspx.cs" MasterPageFile="~/KeytiaOH.Master" Inherits="KeytiaWeb.UserInterface.DirCorporativo.DirectorioCorporativo" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<asp:Content ID="ctHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="ctTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Label ID="lblTitle" runat="server" CssClass="page-title-keytia"></asp:Label>
</asp:Content>
<asp:Content ID="ctContent" ContentPlaceHolderID="cphContent" runat="server">
        
        <asp:PlaceHolder id="divBusqueda" runat="server" Visible="true">
            <asp:Label ID="lblTitulo" runat="server" Text="En esta sección se podrá hacer una búsqueda de cualquier empleado"></asp:Label>
            
            
            <asp:Table ID="Table1" runat="server"  CellPadding="5" Width="80%" HorizontalAlign="Center">
                <asp:TableHeaderRow >
                    <asp:TableCell ColumnSpan="2" HorizontalAlign ="Center" >
                        <asp:Label ID="lblInstrucciones" runat="server" Text="DIRECTORIO"></asp:Label>
                    </asp:TableCell>
                </asp:TableHeaderRow>
                <asp:TableRow Width="100%" >
                    <asp:TableCell HorizontalAlign ="Right" Width="25%">
                        <asp:Label ID="lblNomina" runat="server" Text="Nómina"></asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left" Width="75%">
                        <asp:TextBox ID="txtNomina" align="Center" runat="server" Width="75%"></asp:TextBox>
                    </asp:TableCell>            
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign ="Right">
                        <asp:Label ID="lblNombre" runat="server" Text="Nombre(s)"></asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left">
                        <asp:TextBox ID="txtNombre" align="Center" runat="server" Width="75%"></asp:TextBox>
                    </asp:TableCell>              
                </asp:TableRow>
                    <asp:TableRow>
                    <asp:TableCell HorizontalAlign ="Right">
                        <asp:Label ID="lblApellidos" runat="server" Text="Apellidos (paterno y/o materno)"></asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left">
                        <asp:TextBox ID="txtApellidos" align="Center" runat="server" Width="75%"></asp:TextBox>
                    </asp:TableCell>            
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign ="Right">
                        <asp:Label ID="lblExtension" runat="server" Text="Extensión"></asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left">
                        <asp:TextBox ID="txtExtension" align="Center" runat="server" Width="75%"></asp:TextBox>
                    </asp:TableCell>              
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign ="Right">
                        <asp:Label ID="lblEmpresa" runat="server" Text="Empresa"></asp:Label>
                    </asp:TableCell> 
                    <asp:TableCell HorizontalAlign ="Left">
                        <asp:TextBox ID="txtEmpresa" align="Center" runat="server" Width="75%"></asp:TextBox>
                    </asp:TableCell>              
                </asp:TableRow>
                
                 <asp:TableRow>
                    <asp:TableCell HorizontalAlign ="Center" ColumnSpan="2">
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" UseSubmitBehavior="true"   />
                    </asp:TableCell> 
                </asp:TableRow>
                
            </asp:Table> 
        </asp:PlaceHolder>
        
        <div style="text-align:center" >
        <asp:Panel id="divResultados" runat="server" Visible="false">
    
            <asp:Label ID="lblInformacion" runat="server" >Seleccionar el registro apropiado de la siguiente lista</asp:Label>
             <br />
             <asp:Label ID="Label1" runat="server" ></asp:Label>
             <br />
             <asp:GridView  ID="gvAgrupado" runat="server" RowStyle-HorizontalAlign="Center" Width="80%" 
                AutoGenerateColumns="false" ShowFooter="true"
                HeaderStyle-BackColor="#990000" HeaderStyle-ForeColor="White"
                HorizontalAlign="Center" >
                    <Columns>
                    <asp:BoundField DataField="Nomina" HeaderText="Nómina" HtmlEncode = "true" />

                    <asp:HyperLinkField  DataNavigateUrlFields="iCodCatalogo"
                        DataNavigateUrlFormatString="~/UserInterface/DirCorporativo/Resultados.aspx?Empleado={0}"
                        DataTextField="Nombre"  HeaderText= "Nombre" NavigateUrl="~/UserInterface/DirCorporativo/Resultados.aspx"
                         />
                     <asp:BoundField DataField="Email" HeaderText="Email" HtmlEncode = "true" />
                     <asp:BoundField DataField="Empresa" HeaderText="Empresa" HtmlEncode = "true" />


                    
                    </Columns>
            </asp:GridView>
                
            <br />
            
            <asp:Button ID="btnBuscarOtro" runat="server" Text="Buscar Otro" OnClick="btnBuscarOtro_Click"  />
        </asp:Panel>
        </div>

        
        
    
</asp:Content>
