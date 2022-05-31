<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CargaArchivos.aspx.cs" Inherits="KeytiaWeb.InvContratos.Pantallas.Pantallas_CargaArchivos" %>


<html>
<head>
    <title></title>
    <%--<link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="~InvContratos/Scripts/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>--%>

    <style type="text/css">
        html, body {
            background-color: #E5E5E5;
        }

        .formulario {
            background-color: white;
            font-family: 'Poppins', sans-serif;
            padding: 10px;
            margin: 30px 15px;
            font-size: 14px;
        }

        .form-control {
            font-size: 15px;
        }

        .control-label {
            font-size: 14px;
            font-weight: bold;
        }

        .form-title {
            font-size: 14px;
        }

        .title-form, h4, h3 {
            font-weight: bold;
            font-size: 16px;
        }

        .title-modal {
            background-color: #808080;
            color: white;
            text-align: center;
        }


        .contenedor {
            width: 100%;
            height: 200px;
            overflow: auto;
            border: 1px solid #808080;
            background-color: #E9E9E9;
            border-bottom-left-radius: 6px;
            border-top-left-radius: 6px;
            border-top-right-radius: 6px;
            border-bottom-right-radius: 6px;
            margin-bottom: 30px;
        }
    </style>
</head>
<body>
    <form runat="server">
        <div class="container formulario">
            <div class="col-lg-10">
                <h4 class="title-modal">Cargar Archivos</h4>
                <br />
                <asp:Label ID="lblFolio" runat="server" CssClass="control-label"></asp:Label>
                <asp:TextBox ID="txtFolio" runat="server" ReadOnly="true" BorderStyle="None" CssClass="form-control"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="lblArchivo" runat="server" Text="Archivo: " CssClass="control-label"></asp:Label>
                <asp:FileUpload ID="FileUploadControl" runat="server" />
                <br />
                <asp:Label ID="Label2" runat="server" Text="¿Es documento más reciente?" CssClass="control-label"></asp:Label>
                <asp:RadioButton ID="rbSi" Text="Si" Checked="true" GroupName="RadioGroup1" runat="server" />
                <asp:RadioButton ID="rbNo" Text="No" Checked="false" GroupName="RadioGroup1" runat="server" />
                <br />
                <asp:Label ID="Label3" runat="server" Text="Comentarios: " CssClass="control-label"></asp:Label>
                <br />
                <div class="col-lg-12">
                    <asp:TextBox ID="txtComentario" runat="server" CssClass="form-control" Rows="10" />
                </div>
                <div class="col-lg-3" style="margin-top: 25px">
                    <asp:Button ID="btnCargar" runat="server" OnClick="CargarArchivos" Text="Guardar" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>



