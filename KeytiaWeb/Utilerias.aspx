<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Utilerias.aspx.cs" Inherits="KeytiaWeb.Utilerias" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xshtml">
<head id="Head1" runat="server">
    <title>Keytia</title>
		<meta content="True" name="vs_snapToGrid"/>
		<meta content="True" name="vs_showGrid"/>
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"/>

    <link rel="stylesheet" type="text/css" href="styles/azul/keytia.css" />
</head>
<body>
    <form id="form1" runat="server">
    <h1>Utilerías</h1>
    <table>
        <tr>
            <td>Utilerías</td>
            <td><asp:Button ID="btnPostBack" runat="server" Text="Postback"  />
                <asp:Button ID="btnStaticTest" runat="server" Text="Static" onclick="btnStaticTest_Click" />
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Keytia Web</td>
            <td><asp:Button ID="btnWebClearCache" runat="server" Text="Limpiar Caché" onclick="btnWebClearCache_Click" />
                <asp:Button ID="btnWebClearKDB" runat="server" Text="Limpiar KDB" onclick="btnWebClearKDB_Click" />
                <asp:Button ID="btnWebViewCache" runat="server" Text="Ver Caché" onclick="btnWebViewCache_Click" />
            </td>
        </tr>
        <tr>
            <td>Keytia Service</td>
            <td><asp:Button ID="btnSvcClearCache" runat="server" Text="Limpiar Caché" onclick="btnSvcClearCache_Click" />
                <asp:Button ID="btnSvcClearKDB" runat="server" Text="Limpiar KDB" onclick="btnSvcClearKDB_Click" />
            </td>
        </tr>
        <tr>
            <td>Keytia COM</td>
            <td><asp:Button ID="btnComClearCache" runat="server" Text="Limpiar Caché" onclick="btnComClearCache_Click" />
                <asp:Button ID="btnComClearKDB" runat="server" Text="Limpiar KDB" onclick="btnComClearKDB_Click" />
            </td>
        </tr>
    </table>
    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
    </form>
</body>
</html>
