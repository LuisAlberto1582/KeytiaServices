<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Loginbimbo.aspx.cs" Inherits="KeytiaWeb.Loginbimbo" CodeFileBaseClass="KeytiaWeb.Login" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Keytia</title>
    <link rel="SHORTCUT ICON" href="favicon.ico"/>
		<meta content="True" name="vs_snapToGrid"/>
		<meta content="True" name="vs_showGrid"/>
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"/>

    <!--<link rel="stylesheet" type="text/css" href="styles/azul/keytia.css" />!-->
    </head>
<body>
    <form id="form1" runat="server">

    <asp:Panel ID="Panel1" runat="server"  >
        <table width="966" align="center" cellpadding="3px">
        <tr><td align="center">
                <asp:Panel ID="Panel4" runat="server" style="vertical-align:middle" 
                    Width="404px">
                    <table width="90%">
                    </table>
                    <table style="height: 171px; width: 404px; background-image: url('images/fondobimbo.jpg');">
                        <tr>
                            <td colspan="2" height="43" valign="top" align="left">
                                <img alt="Bimbo" src="images/bimbo.jpg" height="43" width="156" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right" width="50%">
                                <strong>
                                <asp:Label ID="lblUsuario" runat="server" ForeColor="#660000" 
                                    style="width: 106px;" Text="Usuario:"></asp:Label>
                                </strong>
                            </td>
                            <td align="left" width="50%">
                                <asp:TextBox ID="txtUsuario" runat="server" MaxLength="40" Width="170px"></asp:TextBox>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right" width="50%">
                                <strong>
                                <asp:Label ID="lblPassword" runat="server" ForeColor="#660000" 
                                    style="width: 106px" Text="Contraseña:"></asp:Label>
                                </strong>
                            </td>
                            <td align="left" width="50%">
                                <asp:TextBox ID="txtPassword" runat="server" MaxLength="32" TextMode="Password" 
                                    Width="170px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <strong>
                                <asp:Button ID="BtnIngresar" runat="server" Font-Bold="True" 
                                    ForeColor="#660000" onclick="BtnIngresar_Click" style="width: 90px" 
                                    Text="Entrar" Width="90px" />
                                </strong>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" height="20">
                            <asp:Label ID="lblIntDat" runat="server" style="width: 155px" 
                        Font-Bold="True" ForeColor="#000066" Visible="false"><strong><u>Introduzca sus datos</u></strong></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:HyperLink ID="hlOlvidoPassword" runat="server" Font-Bold="True" 
                                    Font-Italic="True" Font-Underline="True" ForeColor="#660000" 
                                    NavigateUrl="~/RecordarPassword.aspx">Recuperar contraseña</asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" height="40">
                            </td>
                        </tr>
                    </table>
                    <table style="height:25px" width="90%">
                        <tr>
                            <td align="center">
                            <asp:Label ID="lblMensaje" runat="server" 
                        style="width: 255px" ForeColor="#000066" Font-Bold="True" 
                        Font-Size="Medium"></asp:Label></td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        </table>
    </asp:Panel>
            <table width="966" align="center" cellpadding="3px">
                <tr>
                <td height="10" align="center">
                    <img alt="Keytia 5" src="images/k3logo.jpg" 
                        style="width: 116px; height: 30px" /></td>
                </tr>
                <tr>
                    <td height="10" align="center">
                        <asp:Label ID="lblCopyright" runat="server" Text="Copyright © 2012 DTI" 
                            Font-Bold="True" Font-Names="Arial" Font-Size="Smaller" 
                            ForeColor="#6161CA"></asp:Label>
                    </td>
                </tr>
            </table>

    <br />
    </form>
</body>
</html>
