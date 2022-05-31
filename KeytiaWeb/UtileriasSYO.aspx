<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UtileriasSYO.aspx.cs" Inherits="KeytiaWeb.UtileriasSYO" %>

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
    <h1>Utilerías SeeYouOn</h1>
    <table>
        <tr>
            <td>
				Conferencias
			</td>
            <td>
                <asp:Button ID="btnLimpiarConferencias" runat="server" Text="Limpiar Conferencias" onclick="btnLimpiarConferencias_Click" />
            </td>
            <td>
				<asp:Label ID="lblResultado" runat="server" Text=""></asp:Label>
			</td>
        </tr>
    </table>
    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
    </form>
</body>
</html>