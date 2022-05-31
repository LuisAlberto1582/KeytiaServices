<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SesionExpirada.aspx.cs" Inherits="KeytiaWeb.SessionExpirada" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1
        {
            text-align: center;
        }
    </style>
<style type="text/css">
h1 {font-family:verdana; color:#5867a8; font-size:xx-small;}
body {font-family:verdana; color:#5867a8; font-size:xx-small;}
TD {font-family:verdana; color:#5867a8; font-size:small;}
A {text-decoration : none; font-family:verdana;}
A:active {text-decoration : none; font-family:verdana;}
A:visited {text-decoration : none; font-family:verdana;}
A:hover {text-decoration : underline; font-family:verdana;}	
table {text-decoration : none; font-family:verdana; font-weight : bold; color:#5867a8;}
td.claros {text-decoration : none; font-family:verdana; font-weight : bold; }
td.oscuros {text-decoration : none; font-family:verdana; font-weight : bold; }
td.prueba {text-transform: lowercase;}
.Estilo1 {color: #ff3300}
.Estilo2 {color: #ff6600}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%" border="0" cellspacing="1" cellpadding="1">
    <tr>
	    <td width="20%">&nbsp;</td>
	    <td width="*" class="Estilo1" align="center">
		    <table width="100%" border="0" cellspacing="1" cellpadding="1">
		    <tr>
			    <td><br/><br/><br/><br/><br/><br/></td>
		    </tr>
		    <tr>
			    <td class="Estilo1" align="center">
				    <font size="4">
					    <br/><b><asp:Label ID="lblTituloSesExp" runat="server" Text=""></asp:Label></b>
					    <br/><b><asp:Label ID="lblmsgSesExp01" runat="server" Text=""></asp:Label></b>
				    </font>
			    </td>
		    </tr>
		    <tr>
			    <td>&nbsp;</td>
		    </tr>
		    <tr>
			    <td align="center">
				    <asp:Button ID="btnAceptar" runat="server" onclick="btnAceptar_Click" />&nbsp;
				    <asp:Button ID="btnCancelar" runat="server" onclick="btnCancelar_Click" />
				</td>
		    </tr>
		    <tr>
			    <td>&nbsp;</td>
		    </tr>
		    <tr> 
			    <td class="Estilo1" align="center">
				    <font size="2" color="#ff3300">
					    <b><asp:Label ID="lblmsgSesExp02" runat="server" Text=""></asp:Label></b>
				    </font>
			    </td>
		    </tr>
		    <tr>
			    <td class="Estilo1" align="center"> 
				    <font size="3" color="#ff3300">
				    <b><asp:Label ID="lblmsgSesExp03" runat="server" Text=""></asp:Label></b>
				    </font>
			    </td>
		    </tr>
		    <tr>
			    <td><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/></td>
		    </tr>
		    <tr>
			    <td align="center">
			        <asp:Image ID="imgLogoKta" runat="server" ImageUrl="~/images/logokeytia.jpg" />
			    </td>
		    </tr>
		    <tr>
			    <td align="center">
				    <h1 align="center"><asp:Label ID="lblmsgSesExp04" runat="server" Text=""></asp:Label></h1>
			    </td>
		    </tr>
		    </table>
	    </td>
	    <td width="20%">&nbsp;</td>
    </tr>
    </table>
    </form>
</body>
</html>
