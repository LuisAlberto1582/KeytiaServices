<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="SYOClienteUpd.aspx.cs" Inherits="KeytiaWeb.UserInterface.SYO.admin.Cliente.SYOClienteUpd" %>
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
                <asp:Label ID="lblInicio" runat="server" CssClass="tituloInicio">Clientes</asp:Label>
            </div>
            
        </div>   
         <div>
            <asp:Panel ID="pToolBar" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix ">
            &nbsp;&nbsp;<asp:LinkButton ID="LinkButton1" Text="Regresar" runat="server" PostBackUrl="~/UserInterface/SYO/admin/Cliente/SYOCliente.aspx" CssClass="buttonBack"></asp:LinkButton>
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
                                            <asp:Label ID="lblTitulo" runat="server">Agregar Cliente</asp:Label>
                                        </th>
                                    </tr>
         <tr class="grvitemStyle">
            <td>Clave: </td>
            <td>
                <asp:Label ID="lblClave" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
         <tr class="grvalternateItemStyle">
            <td>Descripcion: </td>
            <td>
                <asp:Label ID="lblDescripcion" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
      <tr class="grvitemStyle">
            <td>Logo: </td>
            <td>
                <asp:TextBox ID="txtLogo" runat="server" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator
                    ID="RequiredFieldValidator1" runat="server" ErrorMessage="*Debe introducir la ruta del logo" ControlToValidate="txtLogo"
                    Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
            </td>
        </tr>
         <tr class="grvalternateItemStyle">
            <td>Logo de Exportacion: </td>
            <td>
                <asp:TextBox ID="txtLogoExportacion" runat="server" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator
                    ID="RequiredFieldValidator2" runat="server" ErrorMessage="*Debe introducir la ruta del logo de exportacion" ControlToValidate="txtLogoExportacion"
                    Display="Dynamic" ForeColor="Red" ValidationGroup="vlg1"></asp:RequiredFieldValidator>
            </td>
        </tr>
       
        <tr class="grvitemStyle">
            <td></td>
            <td>
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" ValidationGroup="vlg1" OnClick="btnGuardar_Click" 
                     />
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


</script>
</asp:Content>
