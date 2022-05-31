<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="DescargaDeRecursos.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ModuloKeytiaManagement.DescargaDeRecursos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <script language="javascript" type="text/javascript">

        function CheckBoxListSelect(cbControl, state) {
            var chkBoxList = document.getElementById(cbControl);
            var chkBoxCount = chkBoxList.getElementsByTagName("input");
            for (var i = 0; i < chkBoxCount.length; i++) {
                chkBoxCount[i].checked = state;
            }

            return false;
        }

        function ChangeCursorWait() {
            document.body.style.cursor = 'wait'
            setTimeout(function () { document.body.style.cursor = 'default' }, 8000);
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <style type="text/css">

        .formulario {
            background: #fff;
            padding-top: 20px;
            padding-bottom: 70px;
            padding-left: 20px;
        }

        a {
            padding-left: 20px;
        }

        #botones {
            padding-top: 20px;
            padding-left: 30px;
        }

        #checkBoxes{
            padding-top: 30px;
            padding-left: 30px;
        }

        .test td
        {
            margin-right:10px;
            padding-right:20px;
        }

        #botonDescarga {
            padding-top: 40px;
            padding-left: 550px;
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

    <div class ="formulario">
        <h3>
            Descarga masiva de recursos
        </h3>
        <br />
        <a href="#" onclick="javascript: CheckBoxListSelect ('<%= CheckBoxListRecursos.ClientID %>',true)">Seleccionar todos</a>
        <br />
        <a href="#" onclick="javascript: CheckBoxListSelect ('<%= CheckBoxListRecursos.ClientID %>',false)">Deseleccionar todos</a>
        <div id="checkBoxes"> 
            <asp:CheckBoxList ID="CheckBoxListRecursos" RepeatDirection="Horizontal" RepeatColumns="3" CssClass ="test" runat="server">
                <asp:ListItem Text="Cencos" />
                <asp:ListItem Text="Empleados" />
                <asp:ListItem Text="Codigos de Autorizacion" />
                <asp:ListItem Text="Extensiones Activas" />
                <asp:ListItem Text="Lineas Activas" />
                <asp:ListItem Text="Sitios" />
                <asp:ListItem Text="Tipo de Empleado" />
                <asp:ListItem Text="Puestos" />
                <asp:ListItem Text="Cos" />
                <asp:ListItem Text="Carriers" />
                <asp:ListItem Text="Cuentas Maestras" />
                <asp:ListItem Text="Tipo de Plan" />
                <asp:ListItem Text="Equipo de Celular" />
                <asp:ListItem Text="Plan Tarifario" />
                <asp:ListItem Text="CodAut sin Empleado" />
                <asp:ListItem Text="Extensiones sin Empleado" />
                <asp:ListItem Text="Lineas sin Empleado" />
                <asp:ListItem Text="Organizaciones" />
            </asp:CheckBoxList>
            <p>El proceso puede tomar varios minutos.</p>
        </div>
        <div id="botonDescarga">
            <asp:LinkButton ID="linkBtn" Text="<span>Descargar Excel</<span>" OnClientClick="ChangeCursorWait()" CssClass="btn_descarga" OnClick="SubmitBtn_Click" runat="server" />
        </div>
    </div>
</asp:Content>
