<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Mantenimiento.aspx.cs" Inherits="KeytiaWeb.Mantenimiento" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            font-family: Arial, Helvetica, sans-serif;
            color: #666699;
        }
        .style2
        {
            font-family: Arial, Helvetica, sans-serif;
            font-weight: bold;
            color: #666699;
        }
        .style3
        {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="style3">
    
        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/logokeytia.jpg" />
        <br />
        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/mantenimiento.jpg" 
            Width="50%" />
        <br />
        <span class="style2">El sistema estará en Mantenimiento hasta las 00:00 horas 
        del lunes 1 de Septiembre.</span><b><br class="style1" />
        <br class="style1" />
        </b><span class="style2">Si requiere algún reporte favor de comunicarse con 
        Ernesto Flores al teléfono (044)8180882508</span></div>
    </form>
</body>
</html>
