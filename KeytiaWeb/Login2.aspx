<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login2.aspx.cs" Inherits="KeytiaWeb.Login2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Keytia</title>
		<meta content="True" name="vs_snapToGrid"/>
		<meta content="True" name="vs_showGrid"/>
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"/>

    <style type="text/css">
        BODY {   BACKGROUND-IMAGE: url(Images/backgroundAzul.jpg); 
                 MARGIN: 3px; 
                 BACKGROUND-COLOR: #7db4ec }                             
        .style1
        {
            width: 534px;
        }
        .style2
        {
            width: 499px;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
    <div>    </div>

    <table width="960" align="center" cellpadding="3px" style="border-collapse:collapse;background-color :White">
        <tr><td align="center" >
            <asp:Panel ID="Panel1" runat="server" >
                <img alt="" src="images/KeytiaHeader.gif" style="width:960px; height: 74px ; " />
            </asp:Panel>
            </td>
        </tr>
    </table>
    <table   width="966" align="center" cellpadding="3px" style="border-collapse:collapse; background-color :White" >
    <tr style = "width:86px; height: 90px ">
        <td valign ="top" class="style2" >
            <asp:Panel ID="Panel3" runat="server" Width="522px" Height="458px"  >
                 <table style="width: 100%; height: 457px;">
                    <tr><td align = "center" class="style1">                
                        &nbsp;<img alt="" src="images/telefonotrim.jpg" 
                            style="width: 500px; height: 423px" /></td>
                    </tr>
                </table>
            </asp:Panel>
        </td> 
        <td align = "center" >
            <asp:Panel ID="Panel4" runat="server" Width="406px" style="vertical-align:middle">
             <table width="90%">
                <tr><td align = "center">
                    <asp:Label ID="Label4" runat="server" style="width: 155px" 
                        Font-Bold="True" ForeColor="#000066">Introduzca sus datos</asp:Label></td>
                </tr>
            </table>

            <table width="90%" style="height: 98px; width: 305px;background-color :#99CCFF; border-style:solid   "   >
                <tr><td width="50%" align="right">
                        <asp:Label ID="Label2" runat="server" style="width: 106px;" Text="Nombre:" 
                            ForeColor="#000066" ></asp:Label></td>
                    <td width="50%" align ="left" >
                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="10" Width="170px"></asp:TextBox></td>
                </tr>
                <tr><td width="50%" align="right">
                        <asp:Label ID="Label3" runat="server" style="width: 106px" Text="Contraseña:" 
                            ForeColor="#000066"></asp:Label></td>
                    <td width="50%" align="left">
                        <asp:TextBox ID="TextBox2" runat="server" TextMode="Password" MaxLength="10" 
                            Width="170px" ></asp:TextBox></td>
                </tr>
                <tr><td colspan="2" align="right">
                        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                            style="width: 90px" Text="Ingresar" Width="90px" ForeColor="#000066" /></td>
                </tr>
            </table>
            <table width="90%" style = "height:25px">
                <tr><td align = "center">
                    <asp:Label ID="lblMensaje" runat="server" 
                        style="width: 255px" ForeColor="#000066"></asp:Label></td>
                </tr>
            </table>
            </asp:Panel>
        </td> 
    </tr>
    </table>
            <table width="966" align="center" cellpadding="3px" style="height:28px;border-collapse:collapse; background-color :White">
                <tr><td align="right">
                    <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="True" 
                        Font-Italic="True" Font-Underline="True" ForeColor="#000066">¿Olvidó su contraseña? Dé clic aquí</asp:LinkButton>
                    </td>
                </tr>
            </table>
    
    </form>
</body>
</html>
