<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginBAT.aspx.cs" Inherits="KeytiaWeb.LoginBAT" CodeFileBaseClass="KeytiaWeb.Login" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Keytia</title>
		<meta content="True" name="vs_snapToGrid"/>
		<meta content="True" name="vs_showGrid"/>
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"/>

    <link rel="stylesheet" type="text/css" href="styles/azul/keytia.css" />
    <style type="text/css">
        .style1
        {
            height: 467px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

    <asp:Panel ID="Panel1" runat="server"  >
        <table width="966" align="center" cellpadding="3px" style="border-collapse:collapse;background-color :White">
        <tr><td align="center">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/KeytiaHeader.gif" Width="956px" />
             </td>
        </tr>
        </table>
    </asp:Panel>
    <br />
    
<table   width="966" align="center" cellpadding="3px" style="border-collapse:collapse; background-color :#F6F6F6;  " >
    <tr style = "width:86px; height: 90px ">
        <td valign ="top"  >
            <asp:Panel ID="Panel3" runat="server" Width="522px" Height="470px"  >
                 <table style="width: 100%; height: 462px;">
                    <tr><td align = "center" class="style1">                
                        &nbsp;<img alt="" src="images/telefonotrim.jpg" 
                            style="width: 500px; height: 458px" /></td>
                    </tr>
                </table>
            </asp:Panel>
        </td> 
        <td align = "center" style="background-color :#F6F6F6;">
            <asp:Panel ID="Panel4" runat="server" Width="406px" style="vertical-align:middle">
             <table width="90%">
                
            </table>

            <table width="90%" style="height: 98px; width: 305px;background-color :#F6F6F6;  "   >
                
                <tr>
                <td height="40" colspan="2"></td>
                </tr>
                
                <tr><td align = "center" colspan="2">
                    <asp:Label ID="lblIntDat" runat="server" style="width: 155px" 
                        Font-Bold="True" ForeColor="#000066"><strong><u>Introduzca sus datos</u></strong></asp:Label></td>
                </tr>
                <tr>
                <td height="10" colspan="2"></td>
                </tr>
                <tr style="display:none"><td width="50%" align="right"><strong>
                        <asp:Label ID="lblUsuario" runat="server" style="width: 106px;" Text="Usuario:" 
                            ForeColor="#000066" ></asp:Label></strong></td>
                    <td width="50%" align ="left" >
                        <asp:TextBox ID="txtUsuario" runat="server" MaxLength="40" Width="170px" Text=""></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                </tr>
                <tr><td width="50%" align="right"><strong>
                        <asp:Label ID="lblPassword" runat="server" style="width: 106px" Text="Contraseña:" 
                            ForeColor="#000066"></asp:Label></strong></td>
                    <td width="50%" align="left">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="32" 
                            Width="170px" ></asp:TextBox></td>
                </tr>
                <tr><td colspan="2" align="center"><strong>
                        <asp:Button ID="BtnIngresar" runat="server" onclick="BtnIngresar_Click" 
                            style="width: 90px" Text="Iniciar sesión" Width="90px" ForeColor="#000066" /></strong></td>
                </tr>
                <tr>
                <td height="20" colspan="2">
                </td>
                </tr>
                <tr style="display:none"><td align="center" colspan="2">
                    <asp:HyperLink ID="hlOlvidoPassword" runat="server" Font-Bold="True" 
                        Font-Italic="True" Font-Underline="True" ForeColor="#000066" 
                        NavigateUrl="~/RecordarPassword.aspx">Recuperar contraseña</asp:HyperLink>
                    </td>
                </tr>
                <tr>
                <td height="40" colspan="2"></td>
                </tr>
            </table>
            <table width="90%" style = "height:25px">
                <tr><td align = "center">
                    <asp:Label ID="lblMensaje" runat="server" 
                        style="width: 255px" ForeColor="#000066" Font-Bold="True" 
                        Font-Size="Medium"></asp:Label></td>
                </tr>
            </table>
            </asp:Panel>
        </td> 
    </tr>
    
    </table>
            <table width="966" align="center" cellpadding="3px" style="height:28px;border-collapse:collapse; background-color :White">
                <tr>
                <td height="10" style="background-color :#5A92E6;"></td>
                </tr>
            </table>

    </form>
</body>
</html>
