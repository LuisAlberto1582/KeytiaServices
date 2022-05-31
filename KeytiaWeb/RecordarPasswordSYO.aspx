<%@ Page Language="C#"  AutoEventWireup="true" Async="true" CodeBehind="RecordarPasswordSYO.aspx.cs" Inherits="KeytiaWeb.RecordarPasswordSYO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Keytia</title>
		<meta content="True" name="vs_snapToGrid"/>
		<meta content="True" name="vs_showGrid"/>
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"/>

    <link rel="stylesheet" type="text/css" href="styles/azul/keytia.css" />
    </head>
<body>
    <form id="form1" runat="server">

    F<asp:Panel ID="Panel1" runat="server"  >
        <table width="966" align="center" cellpadding="3px" style="border-collapse:collapse;background-color :White">
        <tr><td align="center">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/SeeYouOnHeader.png" Width="956px" />
             </td>
        </tr>
        </table>
    </asp:Panel>
    <br />
<table   width="966" align="center" cellpadding="3px" style="height:500px;border-collapse:collapse; background-color :White" >
    <tr >
        <td align = "center" >
            <asp:Panel ID="Panel4" runat="server" Width="406px" 
                style="vertical-align:middle; margin-top: 0px;">
             <table width="90%" style="height: 30px">
                <tr><td align = "center">
                    <asp:Label ID="lblRecPsdUsr" runat="server" style="width: 155px" 
                        Font-Bold="True" ForeColor="#000066" Font-Size="Medium">Recuperación de contraseña </asp:Label></td>
                </tr>
            </table>

            <table width="90%" 
                    style="border-width: thick; height: 120px; width: 405px; border-style:solid" 
                    frame="border"   >
                <tr><td width="50%" align="right">
                        <asp:Label ID="lblUsuario" runat="server" style="width: 106px;" Text="Usuario:" 
                            ForeColor="#000066" Font-Size="Medium" ></asp:Label></td>
                    <td width="50%" align ="left" >
                        <asp:TextBox ID="txtUsuario" runat="server" MaxLength="40" Width="170px"></asp:TextBox></td>
                </tr>
                <tr><td width="50%" align="right">
                        <asp:Label ID="lblEmail" runat="server" style="width: 170px" Text="Cuenta de Correo:" 
                            ForeColor="#000066" Font-Size="Medium"></asp:Label></td>
                    <td width="50%" align="left">
                        <asp:TextBox ID="txtEmail" runat="server" 
                            Width="170px" ></asp:TextBox></td>
                </tr>
                <tr><td align="center">
                        <asp:Button ID="btnAceptar" runat="server" 
                            style="width: 90px" Text="Aceptar" Width="90px" ForeColor="#000066" 
                            Font-Size="Medium" onclick="btnAceptar_Click" /></td>
                            <td align="center">
                        <asp:Button ID="btnCancelar" runat="server" 
                            style="width: 90px" Text="Cancelar" Width="90px" ForeColor="#000066" 
                                    Font-Size="Medium" onclick="btnCancelar_Click" /></td>
                </tr>
            </table>
            </asp:Panel>
            <table width="90%" style = "height:25px">
                <tr><td align = "center">
                    <asp:Label ID="lblMensaje" runat="server" 
                        style="width: 355px" ForeColor="#000066" Font-Bold="True" 
                        Font-Size="Medium"></asp:Label></td>
                </tr>
            </table>
        </td> 
    </tr>
    
    </table>
    </form>
</body>
</html>
