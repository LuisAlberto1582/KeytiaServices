<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginNextel.aspx.cs" Inherits="KeytiaWeb.LoginNextel"
    CodeFileBaseClass="KeytiaWeb.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        h1
        {
            font-family: verdana;
            color: #5867a8;
            font-size: xx-small;
        }
        body
        {
            font-family: verdana;
            color: #5867a8;
            font-size: xx-small;
        }
        TD
        {
            font-family: verdana;
            color: #5867a8;
            font-size: small;
        }
        A
        {
            text-decoration: none;
            font-family: verdana;
        }
        A:active
        {
            text-decoration: none;
            font-family: verdana;
        }
        A:visited
        {
            text-decoration: none;
            font-family: verdana;
        }
        A:hover
        {
            text-decoration: underline;
            font-family: verdana;
        }
        table
        {
            text-decoration: none;
            font-family: verdana;
            font-weight: bold;
            color: #5867a8;
        }
        td.claros
        {
            text-decoration: none;
            font-family: verdana;
            font-weight: bold;
        }
        td.oscuros
        {
            text-decoration: none;
            font-family: verdana;
            font-weight: bold;
        }
        td.prueba
        {
            text-transform: lowercase;
        }
        .Estilo1
        {
            color: #FFFFFF;
        }
        .Estilo2
        {
            color: #990033;
        }
        .style1
        {
            height: 166px;
        }
    </style>
</head>
<body bgcolor="white">
    <br />
    <br />
    <br />
    <br />
    <br />
    <p>
    </p>
    <p>
    </p>
    <form id="FR_Login" runat="server" target="_top">
    <center>
        <table width="100%">
            <tr>
                <td width="21%" class="style1">
                </td>
                <td width="58%" class="style1">
                    <table align="center" style="height: 171px; width: 404px; background-image: url('images/nextel.jpg');
                        background-position: center top; background-repeat: no-repeat;">
                        <tr>
                            <td height="68" valign="top" align="left" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td height="20" valign="top" colspan="2" align="center">
                                <font color="#2E6E9E" size="3" face="Verdana, Vrinda, Estrangelo Edessa, Gautami, Mangal, Latha, MV Boli, Raavi, Shruti, Tunga, Marlett, MS Outlook, Symbol">
                                    <asp:Label ID="lblIntDat" runat="server"><strong><u>Introduzca sus datos</u></strong></asp:Label><br />
                                </font>
                            </td>
                        </tr>
                        <tr>
                            <td width="151" align="right">
                                <div align="right" class="Estilo1">
                                    <font color="#2E6E9E" size="2" face="Arial, Helvetica, sans-serif">
                                        <asp:Label ID="lblUsuario" runat="server" Text="Usuario:"></asp:Label>
                                    </font>
                                </div>
                            </td>
                            <td width="243" height="32" align="right">
                                <div align="left">
                                    <font size="4" face="Arial, Helvetica, sans-serif">
                                        <asp:TextBox ID="txtUsuario" runat="server" MaxLength="40" Width="170px"></asp:TextBox>
                                    </font>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <div align="right" class="Estilo1">
                                    <font color="#2E6E9E" size="2" face="Arial, Helvetica, sans-serif">
                                        <asp:Label ID="lblPassword" runat="server" Text=""></asp:Label>
                                    </font>
                                </div>
                            </td>
                            <td height="33" align="right">
                                <div align="left">
                                    <asp:TextBox ID="txtPassword" runat="server" MaxLength="32" TextMode="Password" Width="170px"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button ID="BtnIngresar" runat="server" Text="" OnClick="BtnIngresar_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="21%" class="style1">
                </td>
            </tr>
            <tr>
                <td width="21%" height="72">
                </td>
                <td>
                    <table align="center">
                        <tr>
                            <td align="center">
                                <font color="#2E6E9E" size="3" face="Verdana, Vrinda, Estrangelo Edessa, Gautami, Mangal, Latha, MV Boli, Raavi, Shruti, Tunga, Marlett, MS Outlook, Symbol">
                                    <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                                </font>
                            </td>
                        </tr>
                        <tr>
                            <td width="415" align="center">
                                <div align="center">
                                    <font color="#660033" size="3" face="Verdana, Vrinda, Estrangelo Edessa, Gautami, Mangal, Latha, MV Boli, Raavi, Shruti, Tunga, Marlett, MS Outlook, Symbol">
                                    </font><font color="#2E6E9E" size="3" face="Verdana, Vrinda, Estrangelo Edessa, Gautami, Mangal, Latha, MV Boli, Raavi, Shruti, Tunga, Marlett, MS Outlook, Symbol">
                                        <strong>
                                            <asp:Label ID="lblmsgSesExp02" runat="server" Text=""></asp:Label><br />
                                            <font size="5">
                                                <asp:Label ID="lblmsgSesExp03" runat="server" Text=""></asp:Label>
                                            </font></strong></font>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="4">
                                <%--Este Link debe llevar a la pagina default.htm--%>
                                <%--<asp:HyperLink ID="hplDefault" runat="server" NavigateUrl="" class="Estilo2" Text = ""></asp:HyperLink>--%>
                                <a href="http://10.103.133.26/directorio" style="color:#2E6E9E">Directorio Corporativo</a>
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="21%">
                </td>
            </tr>
        </table>
        <br />
        <br />
        <font color="#FFFFFF">
            <br />
        </font>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
    </center>
    <center>
        <table width="100%" align="center">
            <tr>
                <td align="center">
                    <img alt="Keytia 5" src="images/k3logo.jpg" style="width: 116px; height: 30px" />
                </td>
            </tr>
            <tr>
                <td>
                    <h1 align="center">
                        <asp:Label ID="lblmsgSesExp04" runat="server" Text=""></asp:Label></h1>
                </td>
            </tr>
        </table>
    </center>
    </form>
</body>
</html>
