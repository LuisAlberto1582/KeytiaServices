<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="SYOUsuarioUpd.aspx.cs" Inherits="KeytiaWeb.UserInterface.SYO.admin.Usuario.SYOUsuarioUpd" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <div>
        <div>
            <div align="left" class="AutoHeight">
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Usuarios</asp:Label>
            </div>
        </div>
        <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
                &nbsp;&nbsp;<asp:LinkButton ID="LinkButton1" Text="Regresar" runat="server" PostBackUrl="~/UserInterface/SYO/admin/Usuario/SYOUsuario.aspx"
                    CssClass="buttonBack"></asp:LinkButton>
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlMainHolder" runat="server" Width="100%">
        <asp:Panel ID="pnlRep0" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep0" runat="server" CssClass="TopCenter Center" Width="45%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Rep0" runat="server" CssClass="TopCenter divToCenter">
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            <form name="FR_Busquedas" method="post">
                            <table class="DSOGrid" cellspacing="0" rules="all" border="1" id="RepPorSucursalMatGrid"
                                style="height: 100%; width: 100%; border-collapse: collapse;">
                                <tr class="titulosReportes">
                                    <th colspan="2">
                                        <asp:Label ID="lblTitulo" runat="server">Actualizar Usuarios</asp:Label>
                                    </th>
                                </tr>
                                
                                <tr class="grvitemStyle">
                                    <td>
                                        Uri:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblUri" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Descripcion:
                                    </td>
                                    <td>
                                        <asp:Label ID="lblDescripcion" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Nombre:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNombre" runat="server" MaxLength="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*Debe introducir el nombre"
                                            ControlToValidate="txtNombre" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Apellido Paterno:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtApellidoPaterno" runat="server" MaxLength="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*Debe introducir el apellido paterno"
                                            ControlToValidate="txtApellidoPaterno" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Apellido Materno:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtApellidoMaterno" runat="server" MaxLength="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*Debe introducir el apellido materno"
                                            ControlToValidate="txtApellidoMaterno" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                  <tr class="grvitemStyle">
                                   <td>Tipo de URI:</td>
                                   <td>
                                     <div style="text-align: left; width: 50%; margin: auto;">
                                       <asp:CheckBoxList ID="CheckBoxList1" runat="server">
                                           <asp:ListItem Value="1" onclick="UncheckOthers(this);">Es Movil</asp:ListItem>
                                           <asp:ListItem Value="2" onclick="UncheckOthers(this);">Es VR</asp:ListItem>
                                           <asp:ListItem Value="4" onclick="UncheckOthers(this);">Es Fijo</asp:ListItem>
                                       </asp:CheckBoxList>
                                         </div>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="ValidateCheckBoxList" ErrorMessage="Elija una casilla" ValidationGroup="vlg1" ></asp:CustomValidator>
                                       </td>
                                 </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Contraseña:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCon" runat="server" MaxLength="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*Debe introducir Contraseña"
                                            ControlToValidate="txtCon" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Confirmar contraseña:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCon2" runat="server" MaxLength="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="*Debe introducir Contraseña"
                                            ControlToValidate="txtCon2" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                        Perfil de Usuario:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPerfil" runat="server">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ErrorMessage="Debe introducir Facultad"
                                            ControlToValidate="ddlPerfil" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"
                                            InitialValue="0"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="grvitemStyle">
                                    <td>
                                        Empresa:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlEmpresa" runat="server">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ErrorMessage="Debe introducir Facultad"
                                            ControlToValidate="ddlEmpresa" Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"
                                            InitialValue="0"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="grvalternateItemStyle">
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" ValidationGroup="vlg1"
                                            OnClick="btnGuardar_Click" />
                                    </td>
                                </tr>
                            </table>
                            <div class="fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix"
                                style="height: 10px; width: 100%;">
                            </div>
                            </form>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlRep9" runat="server" CssClass="TopCenter divToCenter">
            <asp:Table ID="tblRep9" runat="server" CssClass="TopCenter divToCenter" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="100%">
                        <asp:Panel ID="Rep9" runat="server" CssClass="TopCenter divToCenter">
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </asp:Panel>

    <script type="text/javascript">
    function validarFechaNacimiento(source, arguments) {
        var fecha = arguments.Value;

        if (!/^(19|20)\d\d[\/](0[1-9]|1[012])[\/](0[1-9]|[12][0-9]|3[01])$/.test(fecha)) {
            arguments.IsValid = false;
        }
        else {
            var parts = fecha.split("/");
            var day = parseInt(parts[2], 10);
            var month = parseInt(parts[1], 10);
            var year = parseInt(parts[0], 10);

            var montLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

            if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0)) {
                montLength[1] = 29;
            }

            if (day > 0 && day <= montLength[month - 1]) {
                arguments.IsValid = true;
            }
            else {
                arguments.IsValid = false;
            }

        }
    }
    function vermail(source, arguments) {
        var correo = arguments.Value;
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(correo)) {
            arguments.IsValid = true;
        } else {
            arguments.IsValid = false;
        }
    }
    function ValidateCheckBoxList(sender, args) {
        var checkBoxList = document.getElementById("<%=CheckBoxList1.ClientID %>");
        var checkboxes = checkBoxList.getElementsByTagName("input");
        var isValid = false;
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                isValid = true;
                break;
            }
        }
        args.IsValid = isValid;
    }
    function UncheckOthers(objchkbox) {
        //Get the parent control of checkbox which is the checkbox list
        var objchkList = objchkbox.parentNode.parentNode.parentNode;
        //Get the checkbox controls in checkboxlist
        var chkboxControls = objchkList.getElementsByTagName("input");
        //Loop through each check box controls
        for (var i = 0; i < chkboxControls.length; i++) {
            //Check the current checkbox is not the one user selected
            if (chkboxControls[i] != objchkbox && objchkbox.checked) {
                //Uncheck all other checkboxes
                chkboxControls[i].checked = false;
            }
        }
    }

    </script>

</asp:Content>
