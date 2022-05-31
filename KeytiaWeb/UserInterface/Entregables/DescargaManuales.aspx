<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DescargaManuales.aspx.cs" Inherits="KeytiaWeb.UserInterface.Entregables.DescargaManuales" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <style type="text/css">
        .contenedor_principal {
            background: #fff;
            padding-bottom: 100px;
            padding-left: 20px;
        }

        .contenedor_header {
            padding-left: 10px;
            padding-top: 5px;
        }

        p {
            padding-top: 10px;
        }
        
        .contenedor_descargas {
            padding-top: 10px;
            padding-left: 40px;
        }

        btn_descarga {
            padding-left: 20px;
        }

        .manual_normal {
            padding-top: 10px;
        }

        .manual_estilo {
            padding-top: 30px;
        }

        .btn_descarga {
            border-radius: 5px;
            border: none;
            outline: none; 
            background: #ff652f;
            color: #fff;
            font-size: 14px;
            font-weight: bold;
            padding: 9px 11px;
            letter-spacing: 1px;
            text-transform: uppercase;
            cursor: pointer;
            transition: all 0.5s;
            margin-right: 15px;
        }

        .btn_descarga span {
            cursor: pointer;
            display: inline-block;
            position: relative;
            transition: 0.5s;
        }

        .btn_descarga span:after {
            content: '\2B73';
            position: absolute;
            opacity: 0;
            top: 0;
            right: -20px;
            transition: 0.5s;
        }

        .btn_descarga:hover span {
            padding-right: 25px;
        }

        .btn_descarga:hover span:after {
            opacity: 1;
            right: 0;
        }

    </style>
    <div class ="contenedor_principal">
        <div class ="contenedor_header">
            <h2>Descarga de manuales de usuario</h2>
            <p>En esta sección encontrarás a tu disposición el manual de usuario para la operación óptima de la aplicación.</p>
        </div>
        <div id="contenedorDescargas" class="contenedor_descargas" runat="server">
            <asp:PlaceHolder ID="botonesDescarga" runat="server">
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
