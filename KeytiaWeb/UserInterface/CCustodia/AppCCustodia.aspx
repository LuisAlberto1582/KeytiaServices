<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="AppCCustodia.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodia.AppCCustodia"
    ValidateRequest="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <!--Hoja de estilos para Cartas Custodia-->
    <!--<link href="../../styles/OrangeNextel/CCustodia.css" rel="stylesheet" type="text/css" />-->
    <!--Scripts en jQuery-->
    <%--RZ.20131205 Se retiran estos scripts servian para funcionalidad del autocomplete en los no. series
        function GetModeloId(modeloid) {
            $find('<%=acNoSerie.ClientID%>').set_contextKey(modeloid);
        }

        function OnNSerieSelected(source, eventArgs) {

            var hdnValorNSerieID = "<%= hdnValorNSerie.ClientID %>";

            document.getElementById(hdnValorNSerieID).value = eventArgs.get_value();
            __doPostBack(hdnValorNSerieID, "");
        }
    --%>
    <%--20140227AM. Se agrega estilo para evitar que el area de escritura en los textbox multi-line, pueda ser modificado. (testeado en chrome y en firefox)--%>
    <style type="text/css">
        textarea
        {
            resize: none;
        }
    </style>

    <script src="../../scripts/jquery.blockUI.js" type="text/javascript"></script>

    <script src="../../scripts/jQueryBlockUIPopUp.js" type="text/javascript"></script>

     <style type="text/css">
        .modalProgress
        {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.65;
            -moz-opacity: 0.8;
        }
        .centerProgress
        {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            background-color: White;
            border-radius: 10px;
            filter: alpha(opacity=100);
            opacity: 1;
            -moz-opacity: 1;
        }
        .centerProgress img
        {
            height: 128px;
            width: 128px;
        }
    </style>

    <script type="text/javascript" language="javascript">
        function alerta(mensaje) {
            //alert('Mensaje: \n' + mensaje);
            $(document).ready(function() { jAlert(mensaje, "Advertencia"); });
        }

        function TextBoxChange() {
            var tbValue = document.getElementById('<%= txtUsuarRedEmple.ClientID %>').value;

            if (tbValue == "") {
                alerta("El campo de 'Usuario de red' está en blanco, se eliminará el usuario de red " +
                "que tiene asignado el empleado actualmente al guardar los cambios.");
            }
        }  
        
    </script>

    <script type="text/javascript" language="javascript">
        //20150330 NZ Se agrego para que este modal tome estilos.
        $(document).ready(function() {

            BlockUI("<%=pnlAddEditInventario.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <asp:Table ID="tblHeaderCCustodia" runat="server" Width="100%">
        <asp:TableRow ID="tblrHeaderCCustodia" runat="server">
            <asp:TableCell ID="tblcTitulo" runat="server" HorizontalAlign="Left">
                <asp:Label ID="lblTitle" runat="server" Text="Cartas Custodia Servicios de Voz" CssClass="titleCCustodia"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Right">
                <asp:LinkButton ID="lbtnRegresarPaginaBusq" runat="server" Text="Volver a los resultados de la búsqueda"
                    Font-Bold="true" OnClick="lbtnRegresarPaginaBusq_Click"></asp:LinkButton>
                <asp:LinkButton ID="lbtnRegresarPagBusqExternaCCust" runat="server" Text="Volver a los resultados de la búsqueda"
                    Font-Bold="true" Visible="False" OnClick="lbtnRegresarPagBusqExternaCCust_Click"></asp:LinkButton>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <!--Datos de Empleado-->
    <asp:Panel ID="pHeaderDatosEmple" runat="server" CssClass="headerCCustodia">
        <asp:Table ID="tblHeaderEmple" runat="server" Width="100%">
            <asp:TableRow ID="tblHeaderEmpleF1" runat="server">
                <asp:TableCell ID="tblHeaderEmpleC1" runat="server">
                    <asp:Label ID="lblDatosEmple" runat="server" CssClass="titleSeccionCCustodia" Text="Datos de Empleado"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeaderEmpleC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse" runat="server" ImageAlign="Middle" Style="cursor: pointer" /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pDatosEmple" runat="server">
        <asp:UpdatePanel ID="upDatosEmple" UpdateMode="Conditional" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="lbtnEditEmple" />
                <asp:PostBackTrigger ControlID="lbtnSaveEmple" />
                <asp:PostBackTrigger ControlID="drpJefeEmple" />
                <asp:PostBackTrigger ControlID="lbtnCancelarEmple" />
                <asp:PostBackTrigger ControlID="lbtnDeleteEmple" />
                <asp:PostBackTrigger ControlID="txtUsuarRedEmple" />
            </Triggers>
            <ContentTemplate>
                <br />
                <asp:Table ID="tblDatosEmple" runat="server" Width="100%">
                    <asp:TableRow ID="trFila0" runat="server">
                        <asp:TableCell ID="tcCelda01" runat="server">
Fecha:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda02" runat="server">
                            <asp:TextBox ID="txtFecha" runat="server" ReadOnly="false" Enabled="false" Width="200"></asp:TextBox>
                            <asp:HiddenField ID="hdnFechaFinEmple" runat="server" />
                            <asp:CalendarExtender ID="ceSelectorFecha1" runat="server" TargetControlID="txtFecha">
                            </asp:CalendarExtender>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda03" runat="server">
No. de Folio:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda04" runat="server">
                            <asp:TextBox ID="txtFolioCCustodia" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila1" runat="server">
                        <asp:TableCell ID="tcCelda11" runat="server">
Estatus:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda12" runat="server">
                            <asp:TextBox ID="txtEstatusCCustodia" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda13" runat="server">
Nómina:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda14" runat="server">
                            <asp:TextBox ID="txtNominaEmple" runat="server" ReadOnly="true" Enabled="false" Width="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila2" runat="server">
                        <asp:TableCell ID="tcCelda21" runat="server">
Nombre:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda22" runat="server">
                            <asp:TextBox ID="txtNombreEmple" runat="server" ReadOnly="true" Enabled="false" Width="200"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tcSegundoNombreC1" runat="server">
Segundo Nombre:
                        </asp:TableCell>
                        <asp:TableCell ID="tcSegundoNombreC2" runat="server">
                            <asp:TextBox ID="txtSegundoNombreEmple" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFilaEmple" runat="server">
                        <asp:TableCell ID="tcApPaternoC1" runat="server">
Apellido Paterno:
                        </asp:TableCell>
                        <asp:TableCell ID="tcApPaternoC2" runat="server">
                            <asp:TextBox ID="txtApPaternoEmple" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tcApMaternoEmpleC1" runat="server">
Apellido Materno:
                        </asp:TableCell>
                        <asp:TableCell ID="tcApMaternoEmpleC2" runat="server">
                            <asp:TextBox ID="txtApMaternoEmple" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFilaEmple2" runat="server">
                        <asp:TableCell ID="tcCelda23" runat="server">
Ubicación:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda24" runat="server">
                            <asp:DropDownList ID="drpSitioEmple" runat="server" AppendDataBoundItems="true" Enabled="false" Width="450">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda81" runat="server">
Visible en directorio:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda82" runat="server">
                            <asp:CheckBox ID="cbVisibleDirEmple" Checked="false" runat="server" Enabled="false" /></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFilaEmpreUbica" runat="server">
                        <asp:TableCell ID="tcEmpresaC1" runat="server">
Empresa:
                        </asp:TableCell>
                        <asp:TableCell ID="tcEmpresaC2" runat="server">
                            <asp:DropDownList ID="drpEmpresaEmple" runat="server" AppendDataBoundItems="true"
                                Enabled="false" Width="450">
                                <asp:ListItem Text="-- Selecciona una --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell ID="tcTipoEmpleadoC1" runat="server">
Tipo de Empleado:
                        </asp:TableCell>
                        <asp:TableCell ID="tcTipoEmpleadoC2" runat="server">
                            <asp:DropDownList ID="drpTipoEmpleado" runat="server" AppendDataBoundItems="true"
                                Enabled="false" Width="200">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila3" runat="server">
                        <asp:TableCell ID="tcCelda31" runat="server">
Centro de costos:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda32" runat="server">
                            <asp:DropDownList ID="drpCenCosEmple" runat="server" Enabled="false" AppendDataBoundItems="true" Width="450">
                                <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                            </asp:DropDownList>
                            <%--AM 20131106 Agrego PopUp para agregar Puesto--%>
                            <asp:LinkButton ID="lbtnAgregarCenCos" runat="server" OnClick="btnAgregarCenCos_Click">Agregar
                            CenCos</asp:LinkButton>
                            <asp:Panel ID="pnlAddCenCos" runat="server" CssClass="modalPopup" Style="display: none"
                                ScrollBars="None" Height="245px" Width="450px">
                                <div align="center">
                                    <br />
                                    <asp:Label ID="lblCenCosTitle" runat="server" Text="Centros de Costos" Font-Bold="True"></asp:Label>
                                    <br />
                                    <br />
                                    <table width="90%">
                                        <tr>
                                            <td align="right" width="50%">
                                                <asp:Label ID="lblClaveCenCos" runat="server" Text="Número:"></asp:Label>
                                            </td>
                                            <td align="left" width="50%">
                                                <asp:TextBox ID="txtClaveCenCos" runat="server" Width="100%" MaxLength="40"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" width="50%">
                                                <asp:Label ID="lblCenCosDesc" runat="server" Text="Nombre:"></asp:Label>
                                            </td>
                                            <td align="left" width="50%">
                                                <asp:TextBox ID="txtCenCosDesc" runat="server" Width="100%" MaxLength="160"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" width="50%">
                                                <asp:Label ID="lblFechaIniCenCos" runat="server" Text="Fecha Inicio:"></asp:Label>
                                            </td>
                                            <td align="left" width="50%">
                                                <asp:TextBox ID="txtFechaInicioCenCos" runat="server" Enabled="true" ReadOnly="false"
                                                    MaxLength="10" Width="100%"></asp:TextBox>
                                                <asp:CalendarExtender ID="ceFechaInicioCenCos" runat="server" TargetControlID="txtFechaInicioCenCos">
                                                </asp:CalendarExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" width="50%">
                                                <asp:Label ID="lblEmpleRespCenCos" runat="server" Text="Empleado Responsable:"></asp:Label>
                                            </td>
                                            <td align="left" width="50%">
                                                <asp:DropDownList ID="drpEmpleRespCenCos" runat="server" AppendDataBoundItems="true"
                                                    Width="100%">
                                                    <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" width="50%">
                                                <asp:Label ID="lblCenCosResponsable" runat="server" Text="Centro de Costos Responsable: "></asp:Label>
                                            </td>
                                            <td align="left" width="50%">
                                                <asp:DropDownList ID="drpCenCosResponsable" runat="server" AppendDataBoundItems="true"
                                                    Width="100%">
                                                    <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <br />
                                                <asp:Button ID="btnGuardarCenCos" runat="server" Text="Guardar" 
                                                 CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                                                 OnClick="btnGuardarCenCos_Click" />
                                                &nbsp;
                                                <asp:Button ID="btnCancelarCenCos" runat="server" Text="Cancelar" 
                                                 CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:Panel>
                            <asp:LinkButton ID="lnkBtnFakeAddCenCos" runat="server"></asp:LinkButton>
                            <asp:ModalPopupExtender ID="mpeAddCenCos" runat="server" DropShadow="false" PopupControlID="pnlAddCenCos"
                                TargetControlID="lnkBtnFakeAddCenCos" BackgroundCssClass="modalBackground">
                            </asp:ModalPopupExtender>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda33" runat="server">
Puesto:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda34" runat="server">
                            <asp:DropDownList ID="drpPuestoEmple" runat="server" Enabled="false" AppendDataBoundItems="true" Width="450">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                            <%--AM 20131105 Agrego PopUp para agregar Puesto--%>
                            <asp:LinkButton ID="lbtnAgregarPuesto" runat="server" OnClick="btnAgregarPuesto_Click">Agregar Puesto</asp:LinkButton>
                            <asp:Panel ID="pnlAddPuesto" runat="server" CssClass="modalPopup" Style="display: none"
                                ScrollBars="None" Height="130px" Width="350px">
                                <div align="center">
                                    <asp:Label Font-Bold="True" ID="lblTituloPopUpPuesto" runat="server" Text="Detalle de puesto"></asp:Label>
                                </div>
                                <br />
                                <table align="center">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPuestoDesc" runat="server" Text="Puesto:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPuestoDesc" runat="server" MaxLength="50" Width="200"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvPuestoDesc" runat="server" ErrorMessage="Capture la descripcion del puesto"
                                                ControlToValidate="txtPuestoDesc" Display="Dynamic" ValidationGroup="upDatosEmple">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                                <div align="center">
                                    <br />
                                    <asp:Button ID="btnGuardarPuesto" runat="server" Text="Guardar" 
                                     CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                                     OnClick="btnGuardarPuesto_Click" />
                                    &nbsp;
                                    <asp:Button ID="btnCancelarPuesto" runat="server" Text="Cancelar" 
                                     CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" />
                                </div>
                                <asp:ValidationSummary ID="ValSumAddPuesto" runat="server" />
                            </asp:Panel>
                            <asp:LinkButton ID="lnkBtnFakeAddPuesto" runat="server"></asp:LinkButton>
                            <asp:ModalPopupExtender ID="mpeAddPuesto" runat="server" DropShadow="false" PopupControlID="pnlAddPuesto"
                                TargetControlID="lnkBtnFakeAddPuesto" BackgroundCssClass="modalBackground">
                            </asp:ModalPopupExtender>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila4" runat="server">
                        <asp:TableCell ID="tcCelda41" runat="server">
Localidad:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda42" runat="server">
                            <asp:DropDownList ID="drpLocalidadEmple" runat="server" AppendDataBoundItems="true"
                                Enabled="false" Width="450">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda43" runat="server">
E-mail:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda44" runat="server">
                            <asp:TextBox ID="txtEmailEmple" runat="server" ReadOnly="true" Enabled="false" Width="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila5" runat="server">
                        <asp:TableCell ID="tcCelda51" runat="server">
Radio AT&T:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda52" runat="server">
                            <asp:TextBox ID="txtRadioNextelEmple" runat="server" ReadOnly="false" Enabled="false" Width="200"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda53" runat="server">
Attuid / Usuario de red:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda54" runat="server">
                            <!--20140606 AM Se agregan controles de ajax para informar al usuario que la información esta siendo procesada-->
                            <asp:UpdatePanel ID="upUsuRed" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtUsuarRedEmple" runat="server" ReadOnly="true" Enabled="false"
                                        Width="200"></asp:TextBox>&nbsp;&nbsp;
                                    <asp:CheckBox ID="chkUsuarioPendiente" runat="server" Text="Usuario pendiente" OnCheckedChanged="chkUsuarioPendiente_OnCheckedChanged"
                                        AutoPostBack="true" Visible="false" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress runat="server" ID="upUsuarioDeRed" AssociatedUpdatePanelID="upUsuRed">
                                <ProgressTemplate>
                                    Procesando...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila6" runat="server">
                        <asp:TableCell ID="tcCelda61" runat="server">
Número Celular:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda62" runat="server">
                            <asp:TextBox ID="txtNumCelularEmple" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda63" runat="server">
Gerente:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda64" runat="server">
                            <asp:CheckBox ID="cbEsGerenteEmple" Checked="false" Enabled="false" runat="server" /></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="trFila7" runat="server">
                        <asp:TableCell ID="tcCelda71" runat="server">
Jefe Inmediato:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda72" runat="server">
                            <asp:DropDownList ID="drpJefeEmple" runat="server" Enabled="false" AppendDataBoundItems="true"
                                AutoPostBack="true" OnSelectedIndexChanged="drpJefeEmple_IndexChanged" Width="450">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda73" runat="server">
E-mail del jefe:
                        </asp:TableCell>
                        <asp:TableCell ID="tcCelda74" runat="server">
                            <asp:TextBox ID="txtEmailJefeEmple" runat="server" ReadOnly="true" Enabled="false"
                                Width="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
                <!--RZ.20130718 Se agrega confirmbuttonextender y boton de cancelar-->
                <asp:Table ID="tblEditDeleteEmple" runat="server" Width="100%">
                    <asp:TableRow ID="tblEditDeleteEmpleF1" runat="server">
                        <asp:TableCell ID="tblEditDeleteEmpleC1" runat="server" HorizontalAlign="Center">
                            <asp:LinkButton ID="lbtnEditEmple" runat="server" Text="[ Modificar ]" OnClick="lbtnEditEmple_Click"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnSaveEmple" runat="server" Text="[  Guardar  ]" OnClick="lbtnSaveEmple_Click"
                                Visible="false" Enabled="false"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnDeleteEmple" runat="server" Text="[ Borrar ]" OnClick="lbtnDeleteEmple_Click"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnCancelarEmple" Text="[ Cancelar ]" runat="server" Enabled="false"
                                Visible="false" OnClick="lbtnCancelarEmple_Click"></asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="upUsuarioDeRed2" AssociatedUpdatePanelID="upUsuRed">
            <ProgressTemplate>
                <div class="modalProgress">
                    <div class="centerProgress">
                        <asp:Image runat="server" ID="imgUsuarRed" ImageUrl="~/images/loader.gif" />
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpeDatosEmpleImg" runat="server" TargetControlID="pDatosEmple"
        ExpandControlID="pHeaderDatosEmple" CollapseControlID="pHeaderDatosEmple" CollapsedText="Mostrar..."
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical">
    </asp:CollapsiblePanelExtender>
    <br />
    <!--Datos de Inventario-->
    <asp:Panel ID="pHeaderInventario" runat="server" CssClass="headerCCustodia">
        <asp:Table ID="tblHeaderInventario" runat="server" Width="100%">
            <asp:TableRow ID="tblHeaderInventarioF1" runat="server">
                <asp:TableCell ID="tblHeaderInventarioC1" runat="server">
                    <asp:Label ID="lblInventario" runat="server" CssClass="titleSeccionCCustodia" Text="Inventario asignado"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeaderInventarioC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse2" runat="server" ImageAlign="Middle" Style="cursor: pointer" /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pDatosInventario" runat="server">
        <asp:UpdatePanel ID="upDatosInventario" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="grvInventario" runat="server" CellPadding="4" CssClass="GridView"
                    DataKeyNames="iCodMarca,iCodModelo" GridLines="None" AutoGenerateColumns="false"
                    ShowFooter="true" Width="100%" Style="text-align: center; margin-top: 0px;" EmptyDataText="No existe inventario asignado a este empleado">
                    <Columns>
                        <asp:BoundField DataField="Marca" HeaderText="Marca" HtmlEncode="true" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" HtmlEncode="true" />
                        <asp:BoundField DataField="TipoAparato" HeaderText="Tipo de Aparato" HtmlEncode="true" />
                        <asp:BoundField DataField="NoSerie" HeaderText="No. de Serie" HtmlEncode="true" />
                        <asp:BoundField DataField="MACAddress" HeaderText="MAC Address" HtmlEncode="true" />
                        <asp:BoundField DataField="iCodMarca" HtmlEncode="true" Visible="false" />
                        <asp:BoundField DataField="iCodModelo" HtmlEncode="true" Visible="false" />
                        <%--RZ.20131204 Se retira la edición del inventario
                        <asp:TemplateField HeaderText="Editar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEditarRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvInventario_EditRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        --%>
                        <asp:TemplateField HeaderText="Borrar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnBorrarRow" ImageUrl="~/images/deletesmall.png" OnClick="grvInventario_DeleteRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="GridRowOdd" />
                    <AlternatingRowStyle CssClass="GridRowEven" />
                </asp:GridView>
                <!--RZ.2013120312 Nuevo modulo para asignacion de inventario-->
                <asp:Table ID="tblAddInventario" runat="server" HorizontalAlign="Center" CssClass="tableInv">
                    <asp:TableRow ID="tblAddInventarioR1" runat="server">
                        <asp:TableHeaderCell ID="tblAddInventarioR1HC1" runat="server">
                            Marca
                        </asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="tblAddInventarioR1HC2" runat="server">
                            Modelo
                        </asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="tblAddInventarioR1HC3" runat="server">
                            Tipo de Aparato
                        </asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="tblAddInventarioR1HC4" runat="server">
                            No. de serie
                        </asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="tblAddInventarioR1HC5" runat="server">
                            MAC Address
                        </asp:TableHeaderCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tblAddInventarioR2" runat="server">
                        <asp:TableCell ID="tblAddInventarioR2C1" runat="server">
                            <asp:DropDownList ID="ddlMarca" runat="server" Width="100%">
                            </asp:DropDownList>
                            <asp:CascadingDropDown UseContextKey="true" ContextKey='<%# Bind("iCodMarca") %>'
                                ID="cddMarca" runat="server" Category="Marca" TargetControlID="ddlMarca" ServiceMethod="ObtieneTodasMarcas"
                                ServicePath="~/UserInterface/CCustodia/CCustodia.asmx" PromptText="-- Seleccionar --">
                            </asp:CascadingDropDown>
                        </asp:TableCell>
                        <asp:TableCell ID="tblAddInventarioR2C2" runat="server">
                            <asp:DropDownList ID="ddlModelo" runat="server" Width="100%">
                            </asp:DropDownList>
                            <asp:CascadingDropDown UseContextKey="true" ContextKey='<%# Bind("iCodModelo") %>'
                                ID="cddModelo" runat="server" Category="Modelo" TargetControlID="ddlModelo" ParentControlID="ddlMarca"
                                ServiceMethod="ObtieneTodosModeloPorMarca" ServicePath="~/UserInterface/CCustodia/CCustodia.asmx"
                                PromptText="-- Seleccionar --">
                            </asp:CascadingDropDown>
                        </asp:TableCell>
                        <asp:TableCell ID="tblAddInventarioR2C3" runat="server">
                            <asp:TextBox ID="txtTipoAparato" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tblAddInventarioR2C4" runat="server">
                            <asp:TextBox ID="txtNoSerie" runat="server" Width="200px"></asp:TextBox>
                            <asp:LinkButton ID="lbtnBuscaNoSerie" runat="server" Text=" [ Buscar ]" OnClick="lbtnBuscaNoSerie_Click"></asp:LinkButton>
                            <asp:HiddenField ID="hdnfDispositivo" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell ID="tblAddInventarioR2C5" runat="server">
                            <asp:TextBox ID="txtMACAddress" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="tblAddInventarioR2C6" runat="server">
                            <asp:LinkButton ID="lbtnAsignaEquipo" runat="server" Text="[ Asignar Equipo ]" OnClick="lbtnAsignaEquipo_Click"></asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
                <!--RZ.2013120312 ModalPopUp para busqueda de numeros de series.-->
                <asp:Panel ID="pnlBuscaNoSerie" runat="server" CssClass="modalPopup" Style="display: none"
                    Width="500px" Height="500px">
                    <asp:Table ID="tblHeadBuscaNoSerie" runat="server" Width="100%" CellPadding="10">
                        <asp:TableRow ID="tblHeadBuscaNoSerieR1" runat="server">
                            <asp:TableCell ID="tblHeadBuscaNoSerieR1C1" runat="server" HorizontalAlign="Left">
                                <asp:Label ID="lbTitleBuscaNoSerie" runat="server" Text="No. de Serie"></asp:Label>
                                <asp:TextBox ID="txtBuscaNoSerie" runat="server"></asp:TextBox>
                                <asp:ImageButton ID="ibtnSearchNoSerie" runat="server" ImageUrl="~/images/search.png"
                                    OnClick="ibtnSearchNoSerie_Click" />
                            </asp:TableCell>
                            <asp:TableCell ID="tblHeadBuscaNoSerieR1C2" runat="server" HorizontalAlign="Right">
                                <asp:LinkButton ID="lbtnCerrarBusqNoSerie" runat="server" Text="[ Cerrar ]" OnClientClick="return Hidepopup()"></asp:LinkButton>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <br />
                    <div style="width: 100%; height: 400px; overflow: scroll">
                        <asp:GridView ID="grvResultInventario" runat="server" CellPadding="4" CssClass="GridView"
                            DataKeyNames="iCodDispositivo" GridLines="None" AutoGenerateColumns="false" ShowFooter="true"
                            Width="100%" Style="text-align: center; margin-top: 0px;" EmptyDataText="No existe inventario disponible para esta búsqueda">
                            <Columns>
                                <asp:BoundField DataField="NoSerie" HeaderText="No. de Serie" HtmlEncode="true" />
                                <asp:BoundField DataField="TipoAparato" HeaderText="Tipo de Aparato" HtmlEncode="true" />
                                <asp:BoundField DataField="MACAddress" HeaderText="MAC Address" HtmlEncode="true" />
                                <asp:BoundField DataField="iCodDispositivo" HtmlEncode="true" Visible="false" />
                                <asp:TemplateField HeaderText="Asignar">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnAsignaDispRow" ImageUrl="~/images/checkmarksmall.png" OnClick="grvResultInventario_AsignaRow"
                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle CssClass="GridRowOdd" />
                            <AlternatingRowStyle CssClass="GridRowEven" />
                        </asp:GridView>
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeBusqNoSerie" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeBuscaNoSerie" runat="server" DropShadow="false" PopupControlID="pnlBuscaNoSerie"
                    TargetControlID="lnkFakeBusqNoSerie" BackgroundCssClass="modalBackground">
                </asp:ModalPopupExtender>
                <%-- RZ.20131204 Se retira anteriro manera de agregar inventario 
                <table align="center">
                    <tr>
                        <td>
                            <asp:Button ID="btnAgregar" runat="server" Text="Agregar" OnClick="btnAgregar_Click" />
                        </td>
                    </tr>
                </table>
                --%>
                <%--RZ.20131204 Se cambian controles dropdowns y autocomplete por textbox's --%>
                <!--Modal PopUp para inventario (solo funciona para baja de inventario, se retira alta y edicion)-->
                <asp:Panel ID="pnlAddEditInventario" runat="server" CssClass="modalPopupEtq" Style="display: none"
                    Width="420px" Height="250">                    
                    <div align="center" class="headerEtq" style="height: 40px; vertical-align: middle;
                        line-height: 20px; font-size: 12px">
                        <asp:Label Font-Bold="true" ID="lblTituloPopUp" runat="server" Text="¿Esta seguro que desea dar de baja la relación del dispositivo con el empleado?"></asp:Label>
                    </div>
                    <br />
                    <div align="center">
                        <table align="center" width="95%">
                            <tr>
                                <td>
                                    <asp:Label ID="lblMarcaPopUp" runat="server" Text="Marca"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMarcaPopUp" runat="server" Visible="true" Enabled="false" Width="100%"></asp:TextBox>
                                    <%--
                                    <asp:DropDownList ID="drpMarcaPopUp" runat="server" Width="100%">
                                    </asp:DropDownList>
                                    <asp:CascadingDropDown UseContextKey="true" ContextKey='<%# Bind("iCodMarca") %>'
                                        ID="ccdMarcaPopUp" runat="server" Category="Marca" TargetControlID="drpMarcaPopUp"
                                        ServiceMethod="ObtieneTodasMarcas" ServicePath="~/UserInterface/CCustodia/CCustodia.asmx"
                                        PromptText="-- Seleccionar --">
                                    </asp:CascadingDropDown>
                                    --%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblModeloPopUp" runat="server" Text="Modelo"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtModeloPopUp" runat="server" Visible="true" Enabled="false" Width="100%"></asp:TextBox>
                                    <%--
                                    <asp:DropDownList ID="drpModeloPopUp" runat="server" Width="100%">
                                    </asp:DropDownList>
                                    <asp:CascadingDropDown UseContextKey="true" ContextKey='<%# Bind("iCodModelo") %>'
                                        ID="ccdModelo" runat="server" Category="Modelo" TargetControlID="drpModeloPopUp"
                                        ParentControlID="drpMarcaPopUp" ServiceMethod="ObtieneTodosModeloPorMarca" ServicePath="~/UserInterface/CCustodia/CCustodia.asmx"
                                        PromptText="-- Seleccionar --">
                                    </asp:CascadingDropDown>
                                    --%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblNoSeriePopUp" runat="server" Text="No. de Serie"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNoSeriePopUp" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    <%--
                                    <div id="ListDivisor">
                                    </div>
                                    <asp:AutoCompleteExtender ID="acNoSerie" runat="server" TargetControlID="txtNoSeriePopUp"
                                        UseContextKey="true" ContextKey="" EnableCaching="true" CompletionSetCount="50"
                                        MinimumPrefixLength="0" ServicePath="~/UserInterface/CCustodia/CCustodia.asmx"
                                        FirstRowSelected="true" ServiceMethod="ObtieneNoSeriePorModelo" OnClientItemSelected="OnNSerieSelected"
                                        ShowOnlyCurrentWordInCompletionListItem="true" CompletionListCssClass="CompletionListCssClass"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionListElementID="ListDivisor" />
                                     --%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbltMacAddressPopUp" runat="server" Text="MAC Address"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMacAddressPopUp" runat="server" Width="100%" Enabled="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblTipoAparatoPopUp" runat="server" Text="Tipo de Aparato"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTipoAparatoPopUp" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" class="footerEtq">
                                    <br />
                                    <asp:Button ID="btnGuardarPopUp" runat="server" Text="Eliminar" 
                                     CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                                     OnClick="btnGuardar_PopUpInventario" />
                                    &nbsp;
                                    <asp:Button ID="btnCancelarPopUp" runat="server" Text="Cancelar" 
                                     CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                                     OnClientClick="return Hidepopup()" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:HiddenField ID="hdnValorNSerie" OnValueChanged="hdnValorNSerie_ValueChanged"
                        runat="server" />
                </asp:Panel>
                <asp:LinkButton ID="lnkFake" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeInventario" runat="server" DropShadow="false" PopupControlID="pnlAddEditInventario"
                    TargetControlID="lnkFake" BackgroundCssClass="modalBackground">
                </asp:ModalPopupExtender>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="grvInventario" />
                <asp:PostBackTrigger ControlID="btnGuardarPopUp" />
                <asp:PostBackTrigger ControlID="hdnValorNSerie" />
            </Triggers>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpeInventario" runat="server" TargetControlID="pDatosInventario"
        ExpandControlID="pHeaderInventario" CollapseControlID="pHeaderInventario" CollapsedText="Mostrar..."
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse2" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical">
    </asp:CollapsiblePanelExtender>
    <br />
    <!--Datos de codigos y extensiones-->
    <asp:Panel ID="pHeaderCodAutoExten" runat="server" CssClass="headerCCustodia">
        <asp:Table ID="tblHeaderCodAutoExten" runat="server" Width="100%">
            <asp:TableRow ID="tblHeaderCodAutoExtenF1" runat="server">
                <asp:TableCell ID="tblHeaderCodAutoExtenC1" runat="server">
                    <asp:Label ID="lblCodAutoExten" runat="server" CssClass="titleSeccionCCustodia" Text="Recursos asignados"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeaderCodAutoExtenC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse3" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pDatosCodAutoExten" runat="server">
        <!--Extensiones-->
        <asp:UpdatePanel ID="upDatosCodAutoExten" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="grvExten" runat="server" CellPadding="4" CssClass="GridView" DataKeyNames="Exten,Sitio,TipoExten,iCodRegRelEmpExt"
                    GridLines="None" AutoGenerateColumns="false" ShowFooter="true" Width="100%" Style="text-align: center;
                    margin-top: 0px;" EmptyDataText="No existen extensiones asignadas a este empleado">
                    <Columns>
                        <%--AM. 20130717 Se agregaron las columnas de FechaFin y numero de registro de la relación--%>
                        <%--0--%><asp:BoundField DataField="Exten" Visible="false" ReadOnly="true" />
                        <%--1--%><asp:BoundField DataField="Sitio" Visible="false" ReadOnly="true" />
                        <%--2--%><asp:BoundField DataField="TipoExten" Visible="false" ReadOnly="true" />
                        <%--3--%><asp:BoundField DataField="ExtenCod" HeaderText="Extensión" HtmlEncode="true" />
                        <%--4--%><asp:BoundField DataField="SitioDesc" HeaderText="Sitio" HtmlEncode="true" />
                        <%--5--%><asp:BoundField DataField="FechaIni" HeaderText="Fecha Inicial" HtmlEncode="true"
                            DataFormatString="{0:d}" />
                        <%--6--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Final" HtmlEncode="true"
                            DataFormatString="{0:d}" />
                        <%--7--%><asp:BoundField DataField="TipoExtenDesc" HeaderText="Tipo" HtmlEncode="true" />
                        <%--8--%><asp:CheckBoxField DataField="VisibleDir" HeaderText="Visible en Directorio" />
                        <%--9--%><asp:BoundField DataField="ComentarioExten" HeaderText="Comentarios" HtmlEncode="true" />
                        <%--10--%><asp:BoundField DataField="iCodRegRelEmpExt" Visible="false" ReadOnly="true" />
                        <%--11--%><asp:TemplateField HeaderText="Editar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEditarExtenRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvExten_EditRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--12--%><asp:TemplateField HeaderText="Borrar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnBorrarExtenRow" ImageUrl="~/images/deletesmall.png" OnClick="grvExten_DeleteRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="GridRowOdd" />
                    <AlternatingRowStyle CssClass="GridRowEven" />
                </asp:GridView>
                <br />
                <table align="center" visible="false">
                    <tr>
                        <td>
                            <asp:Button ID="btnAgregarExten" runat="server" Text="Agregar" OnClick="btnAgregarExten_Click"
                                Visible="false" Enabled="false" />
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblAltaDeExtensiones" runat="server" HorizontalAlign="Center" CssClass="tableInv">
                    <asp:TableRow HorizontalAlign="Center" Font-Bold="true">
                        <asp:TableCell>Extensión
                        </asp:TableCell>
                        <asp:TableCell>Sitio
                        </asp:TableCell>
                        <asp:TableCell>Fecha Inicial
                        </asp:TableCell>
                        <asp:TableCell>Tipo
                        </asp:TableCell>
                        <asp:TableCell>Visible en directorio
                        </asp:TableCell>
                        <asp:TableCell>Comentarios
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell>
                            <asp:TextBox ID="txtExtensionNoPopUp" runat="server" MaxLength="10"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:DropDownList ID="drpSitioNoPopUp" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtFechaInicioNoPopUp" runat="server" ReadOnly="false" Enabled="true"
                                MaxLength="10"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtFechaInicioNoPopUp">
                            </asp:CalendarExtender>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:DropDownList ID="drpTipoExtenNoPopUp" runat="server" AppendDataBoundItems="true">
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:DropDownList ID="drpVisibleDirNoPopUp" runat="server">
                                <asp:ListItem Value="1">Si</asp:ListItem>
                                <asp:ListItem Value="0">No</asp:ListItem>
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtComentariosExtenNoPopUp" runat="server" TextMode="MultiLine"
                                Height="50"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <!--20140430 AM Se agregan controles de ajax para informar al usuario que la información esta siendo procesada-->
                            <asp:UpdateProgress runat="server" ID="ProcesandoAltaExtension">
                                <ProgressTemplate>
                                    Procesando...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <asp:LinkButton ID="lbtnGuardarExtenNoPopUp" runat="server" OnClick="lbtnGuardar_ExtenNoPopUp">[Agregar]</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ColumnSpan="7">
                            <asp:CheckBox ID="cbRangoExtenNoPopUp" runat="server" />Dar de alta nuevo rango
                            de extensión
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
                <br />
                <!--Modal PopUp para Extensiones-->
                <asp:Panel ID="pnlAddEditExten" runat="server" CssClass="modalPopupEtq" Style="display: none"
                    Width="475" ScrollBars="None" Height="425">                  
                    <div align="center" class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px; font-size: 12px">
                        <asp:Label Font-Bold="true" ID="lblTituloPopUpExten" runat="server" Text="Detalle de extensión"></asp:Label></div>
                    <br />
                    <table align="center" width="95%">
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblExtenCod" runat="server" Text="Extensión"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:TextBox ID="txtExtension" runat="server" MaxLength="10" Width="100%"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvExtension" runat="server" ControlToValidate="txtExtension"
                                    ErrorMessage="*" SetFocusOnError="True" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <%--AM.Se agrega bandera para dar de alta rango de extension nuevo--%>
                        <tr align="left">
                            <td width="30%">
                            </td>
                            <td width="70%">
                                <asp:CheckBox ID="cbRangoExten" runat="server" />&nbsp;
                                <asp:Label ID="lblRangoExten" runat="server" Text="Dar de alta nuevo rango de extensión" />
                            </td>
                        </tr>
                        <%--AM.20130701 CheckBox oculto para editar--%>
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="cbEditarExtension" runat="server" Visible="false" Checked="False" />
                            </td>
                        </tr>
                        <%--AM.20130701 CheckBox oculto para baja--%>
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="cbBajaExtension" runat="server" Visible="false" Checked="False" />
                            </td>
                        </tr>
                        <%--AM.20130701 text oculto para baja--%>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="txtRegistroRelacion" runat="server" Visible="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblSitioExten" runat="server" Text="Sitio"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:DropDownList ID="drpSitio" runat="server" AppendDataBoundItems="true" Width="100%">
                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvSitio" runat="server" ControlToValidate="drpSitio"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblFechaIniExten" runat="server" Text="Fecha Inicial"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:TextBox ID="txtFechaInicio" runat="server" ReadOnly="false" Enabled="true" Width="100%"
                                    MaxLength="10"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtFechaInicio">
                                </asp:CalendarExtender>
                                <asp:RequiredFieldValidator ID="rfvFechaInicio" runat="server" ControlToValidate="txtFechaInicio"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <%--AM. 20130715 Se agrega FechaFinExten--%>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblFechaFinExten" runat="server" Text="Fecha Final"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:TextBox ID="txtFechaFinExten" runat="server" ReadOnly="false" Enabled="true"
                                    Width="100%" MaxLength="10"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtFechaFinExten">
                                </asp:CalendarExtender>
                                <asp:RequiredFieldValidator ID="rfvFechaFinExten" runat="server" ControlToValidate="txtFechaInicio"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblTipoExten" runat="server" Text="Tipo"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:DropDownList ID="drpTipoExten" runat="server" AppendDataBoundItems="true" Width="100%">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTipoExten" runat="server" ControlToValidate="drpTipoExten"
                                    ErrorMessage="*" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblVisibleDirExten" runat="server" Text="Visible en Directorio"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:DropDownList ID="drpVisibleDir" runat="server">
                                    <asp:ListItem Value="1">Si</asp:ListItem>
                                    <asp:ListItem Value="0">No</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvVisibleDir" runat="server" ControlToValidate="drpVisibleDir"
                                    ErrorMessage="*" InitialValue="Si" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblComentarioExten" runat="server" Text="Comentarios"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:TextBox ID="txtComentariosExten" runat="server" TextMode="MultiLine" Width="100%"
                                    Height="50"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <div align="center" class="footerEtq">
                        <asp:Button ID="btnGuardarExten" runat="server" Text="Guardar"
                         CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                         OnClick="btnGuardar_PopUpExten" />&nbsp;
                        <asp:Button ID="btnCancelarExten" runat="server" Text="Cancelar" 
                        CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                         OnClientClick="return Hidepopup()" />
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeExten" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeExten" runat="server" DropShadow="false" PopupControlID="pnlAddEditExten"
                    TargetControlID="lnkFakeExten" BackgroundCssClass="modalBackground">
                </asp:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="ProcesandoAltaExtension2" AssociatedUpdatePanelID="upDatosCodAutoExten">
            <ProgressTemplate>
                <div class="modalProgress">
                    <div class="centerProgress">
                        <asp:Image runat="server" ID="imgExten" ImageUrl="~/images/loader.gif" />
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <!--Codigos-->
        <asp:UpdatePanel ID="upDatosCodAutoExten2" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="grvCodAuto" runat="server" CellPadding="4" CssClass="GridView"
                    DataKeyNames="CodAuto,Sitio,iCodRegRelEmpCodAuto" GridLines="None" AutoGenerateColumns="false"
                    ShowFooter="true" Width="100%" Style="text-align: center; margin-top: 0px;" EmptyDataText="No existen códigos asignados a este empleado">
                    <Columns>
                        <%--0--%><asp:BoundField DataField="CodAutoCod" HeaderText="Código de Llamadas" HtmlEncode="true" />
                        <%--1--%><asp:BoundField DataField="SitioDesc" HeaderText="Sitio" HtmlEncode="true" />
                        <%--2--%><asp:BoundField DataField="FechaIni" HeaderText="Fecha Inicial" HtmlEncode="true"
                            DataFormatString="{0:d}" />
                        <%--3--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Fin" HtmlEncode="true"
                            DataFormatString="{0:d}" />
                        <%--RZ.20131227 Se retira campo "Visible en Directorio"--%>
                        <%--4<asp:CheckBoxField DataField="VisibleDir" HeaderText="Visible en Directorio" />--%>
                        <%--5--%><asp:BoundField DataField="CodAuto" HtmlEncode="true" Visible="false" />
                        <%--6--%><asp:BoundField DataField="Sitio" HtmlEncode="true" Visible="false" />
                        <%--7--%><asp:BoundField DataField="iCodRegRelEmpCodAuto" Visible="false" ReadOnly="true" />
                        <%--8--%><asp:TemplateField HeaderText="Editar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEditarCodAutoRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvCodAuto_EditRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--9--%><asp:TemplateField HeaderText="Borrar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnBorrarCodAutoRow" ImageUrl="~/images/deletesmall.png" OnClick="grvCodAuto_DeleteRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="GridRowOdd" />
                    <AlternatingRowStyle CssClass="GridRowEven" />
                </asp:GridView>
                <br />
                <table align="center" visible="false">
                    <tr>
                        <td>
                            <asp:Button ID="btnAgregarCodAuto" runat="server" Text="Agregar" OnClick="btnAgregarCodAuto_Click"
                                Visible="false" Enabled="false" />
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblAltaDeCodigosAut" runat="server" HorizontalAlign="Center">
                    <asp:TableRow HorizontalAlign="Center" Font-Bold="true">
                        <asp:TableCell>Código
                        </asp:TableCell>
                        <asp:TableCell>Sitio
                        </asp:TableCell>
                        <asp:TableCell>Fecha Inicial
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell>
                            <asp:TextBox ID="txtCodAutoNoPopUp" runat="server" MaxLength="50"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:DropDownList ID="drpSitioCodAutoNoPopUp" runat="server" AppendDataBoundItems="true"
                                Width="200">
                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtFechaInicioCodAutoNoPopUp" runat="server" ReadOnly="false" Enabled="true"
                                Width="200" MaxLength="10">
                            </asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtFechaInicioCodAutoNoPopUp">
                            </asp:CalendarExtender>
                        </asp:TableCell>
                        <asp:TableCell>
                            <!--20140430 AM Se agregan controles de ajax para informar al usuario que la información esta siendo procesada-->
                            <asp:UpdateProgress runat="server" ID="ProcesandoAltaCodAuto">
                                <ProgressTemplate>
                                    Procesando...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <asp:LinkButton ID="lbtnGuardarCodAutoNoPopUp" runat="server" Text="Guardar" OnClick="lbtnGuardar_CodAutoNoPopUp">[Agregar]</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
                <!--Modal PopUp para Codigos de Autorizacion-->
                <asp:Panel ID="pnlAddEditCodAuto" runat="server" CssClass="modalPopupEtq" Style="display: none"
                    Width="350" Height="225">
                    <div align="center"class="headerEtq" style="height: 30px; vertical-align: middle;
                        line-height: 30px; font-size: 12px">
                        <asp:Label Font-Bold="true" ID="lblTituloPopUpCodAuto" runat="server" Text="Detalle de códigos"></asp:Label>
                    </div>
                    <br />
                    <table align="center">
                        <tr>
                            <td>
                                <asp:Label ID="lblCodAuto" runat="server" Text="Código"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCodAuto" runat="server" MaxLength="50" Width="200"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCodAuto" runat="server" ControlToValidate="txtCodAuto"
                                    ErrorMessage="*" SetFocusOnError="True" ValidationGroup="upDatosCodAutoExten2">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblSitioCodAuto" runat="server" Text="Sitio"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="drpSitioCodAuto" runat="server" AppendDataBoundItems="true"
                                    Width="200">
                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblFechaIniCodAuto" runat="server" Text="Fecha Inicial"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFechaInicioCodAuto" runat="server" ReadOnly="false" Enabled="true"
                                    Width="200" MaxLength="10"></asp:TextBox>
                                <asp:CalendarExtender ID="ceSelectorFecha2" runat="server" TargetControlID="txtFechaInicioCodAuto">
                                </asp:CalendarExtender>
                                <asp:RequiredFieldValidator ID="rfvFechaInicioCodAuto" runat="server" ControlToValidate="txtFechaInicioCodAuto"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten2">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <%--AM. 20130715 Se agrega FechaFinCodAuto--%>
                        <tr>
                            <td>
                                <asp:Label ID="lblFechaFinCodAuto" runat="server" Text="Fecha Fin"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFechaFinCodAuto" runat="server" ReadOnly="false" Enabled="true"
                                    Width="200" MaxLength="10"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtFechaFinCodAuto">
                                </asp:CalendarExtender>
                                <asp:RequiredFieldValidator ID="rfvFechaFinCodAuto" runat="server" ControlToValidate="txtFechaFinCodAuto"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten2">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <%--AM. Se agrega seccion para seleccionar si la extension sera visible en el directorio  20130628--%>
                        <%--RZ.20131227 Se retira la bandera "Visible en directorio"
                        <tr>
                            <td>
                                <asp:Label ID="lblVisibleDirCodAuto" runat="server" Text="Visible en Directorio"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="drpVisibleDirCodAuto" runat="server">
                                    <asp:ListItem Value="1">Si</asp:ListItem>
                                    <asp:ListItem Value="0">No</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="drpVisibleDir"
                                    ErrorMessage="*" InitialValue="Si" ValidationGroup="upDatosCodAutoExten">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>--%>
                        <%--AM.20130701 CheckBox oculto para editar--%>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbEditarCodAuto" runat="server" Visible="false" Checked="False" />
                            </td>
                        </tr>
                        <tr>
                            <%--AM.20130701 CheckBox oculto para baja--%>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="cbBajaCodAuto" runat="server" Visible="false" Checked="False" />
                                </td>
                            </tr>
                            <%--AM.20130701 text oculto para baja--%>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtRegistroRelacionCodAuto" runat="server" Visible="False"></asp:TextBox>
                                </td>
                            </tr>
                    </table>
                    <div align="center" class="footerEtq">
                        <asp:Button ID="btnGuardarCodAuto" runat="server" Text="Guardar" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                         OnClick="btnGuardar_PopUpCodAuto" 
                         />&nbsp;
                        <asp:Button ID="btnCancelarCodAuto" runat="server" Text="Cancelar" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                         OnClientClick="return Hidepopup()" 
                          />
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeCodAuto" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeCodAuto" runat="server" DropShadow="false" PopupControlID="pnlAddEditCodAuto"
                    TargetControlID="lnkFakeCodAuto" BackgroundCssClass="modalBackground">
                </asp:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="ProcesandoAltaCodAuto2" AssociatedUpdatePanelID="upDatosCodAutoExten2">
            <ProgressTemplate>
                <div class="modalProgress">
                    <div class="centerProgress">
                        <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader.gif" />
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpeCodAutoExten" runat="server" TargetControlID="pDatosCodAutoExten"
        ExpandControlID="pHeaderCodAutoExten" CollapseControlID="pHeaderCodAutoExten"
        CollapsedText="Mostrar..." ExpandedText="Ocultar" ImageControlID="imgExpandCollapse3"
        ExpandedImage="~/images/up-arrow-square-blue.png" CollapsedImage="~/images/down-arrow-square-blue.png"
        ExpandDirection="Vertical">
        <%--ExpandedSize="100%"--%>
    </asp:CollapsiblePanelExtender>
    <br />
    <!--NZ 20150826 Se agrega sección para agregar usuarios a empleado. -->
    <!--Datos ID Usuario y PIN -->
    <asp:Panel ID="pHeaderUsuarios" runat="server" CssClass="headerCCustodia">
        <asp:Table ID="tblHeaderUsuarios" runat="server" Width="100%">
            <asp:TableRow ID="tblHeaderUsuariosF1" runat="server">
                <asp:TableCell ID="tblHeaderUsuariosC1" runat="server">
                    <asp:Label ID="lblUsuarios" runat="server" CssClass="titleSeccionCCustodia" Text="ID's de Usuario asignados"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tblHeaderUsuariosC2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse5" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pDatosUsuarios" runat="server">
        <!--Datos ID's Usuario-->
        <asp:UpdatePanel ID="UpDatosUsuarios" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="grvUsuarios" runat="server" CellPadding="4" CssClass="GridView"
                    DataKeyNames="iCodRegUsuario" GridLines="None" AutoGenerateColumns="false" ShowFooter="true"
                    Width="100%" Style="text-align: center; margin-top: 0px;" EmptyDataText="No existen ID's de usuario asignados a este empleado">
                    <Columns>
                        <%--0--%><asp:BoundField DataField="IdUsuario" HeaderText="ID de Usuario" HtmlEncode="true" />
                        <%--1--%><asp:BoundField DataField="Pin" HeaderText="PIN" HtmlEncode="true" />
                        <%--2--%><asp:BoundField DataField="FechaIni" HeaderText="Fecha Inicial" HtmlEncode="true"
                            DataFormatString="{0:d}" />
                        <%--3--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Fin" HtmlEncode="true"
                            DataFormatString="{0:d}" />
                        <%--4--%><asp:BoundField DataField="ComentariosUsuarios" HeaderText="Comentarios"
                            HtmlEncode="true" />
                        <%--5--%><asp:BoundField DataField="iCodRegUsuario" Visible="false" ReadOnly="true"
                            HtmlEncode="true" />
                        <%--6--%><asp:TemplateField HeaderText="Editar">
                            <ItemTemplate>                           
                                <asp:ImageButton ID="btnEditarUsuariosRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvUsuarios_EditRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--7--%><asp:TemplateField HeaderText="Borrar">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnBorrarUsuariosRow" ImageUrl="~/images/deletesmall.png" OnClick="grvUsuarios_DeleteRow"
                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="GridRowOdd" />
                    <AlternatingRowStyle CssClass="GridRowEven" />
                </asp:GridView>
                <br />
                <table align="center" visible="false">
                    <tr>
                        <td>
                            <asp:Button ID="btnAgregarUsuario" runat="server" Text="Agregar" Visible="false"
                                Enabled="false" OnClick="btnAgregarUsuario_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblAltaDeUsuarios" runat="server" HorizontalAlign="Center">
                    <asp:TableRow HorizontalAlign="Center" Font-Bold="true">
                        <asp:TableCell>ID de Usuario
                        </asp:TableCell>
                        <asp:TableCell>PIN
                        </asp:TableCell>
                        <asp:TableCell>Fecha Inicial
                        </asp:TableCell>
                        <asp:TableCell>Fecha Fin
                        </asp:TableCell>
                        <asp:TableCell>Comentarios
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell>
                            <asp:TextBox ID="txtIDUsuarioNoPopUp" runat="server" MaxLength="15"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtPinNoPopUp" runat="server" MaxLength="15"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtFechaInicioUsuarioNoPopUp" runat="server" ReadOnly="false" Enabled="true"
                                Width="200" MaxLength="10">
                            </asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender6" runat="server" TargetControlID="txtFechaInicioUsuarioNoPopUp">
                            </asp:CalendarExtender>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtFechaFinUsuarioNoPopUp" runat="server" ReadOnly="false" Enabled="true"
                                Width="200" MaxLength="10">
                            </asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender7" runat="server" TargetControlID="txtFechaFinUsuarioNoPopUp">
                            </asp:CalendarExtender>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtComentariosUsuarioNoPopUp" runat="server" TextMode="MultiLine"
                                Height="50"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                         <!--20150907 Se agregan controles de ajax para informar al usuario que la información esta siendo procesada-->
                            <asp:UpdateProgress runat="server" ID="ProcesandoAltaIDUsuarios">
                                <ProgressTemplate>
                                    Procesando...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <asp:LinkButton ID="lbtnGuardarUsuarioNoPopUp" runat="server" Text="Guardar" OnClick="lbtnGuardarUsuarioNoPopUp_Click">[Agregar]</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
                <!--Modal PopUp para IDs de Usuarios del Empleado-->
                <asp:Panel ID="pnlAddEditUsuarios" runat="server" CssClass="modalPopupEtq" Style="display: none"
                    Width="420" Height="265" ScrollBars="None">
                    <div align="center" class="headerEtq" style="height: 30px; vertical-align: middle;
                        line-height: 30px; font-size: 12px">
                        <asp:Label Font-Bold="true" ID="lblTituloPopUpUsuarios" runat="server" Text="Detalle de usuarios"></asp:Label>
                    </div>
                    <br />
                    <table align="center">
                        <tr>
                            <td>
                                <asp:Label ID="lblIdUsuario" runat="server" Text="ID de Usuario"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtIdUsuario" runat="server" MaxLength="15" Width="200"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvIdUsuario" runat="server" ControlToValidate="txtIdUsuario"
                                    ErrorMessage="*" SetFocusOnError="True" ValidationGroup="upDatosUsuarios">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblPin" runat="server" Text="PIN"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPin" runat="server" MaxLength="15" Width="200"></asp:TextBox>
                                <%--<asp:RegularExpressionValidator ID="rev" runat="server" ErrorMessage="*" ValidationExpression="^\d*$"
                                    ControlToValidate="txtPin"></asp:RegularExpressionValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblFechaIniUsuario" runat="server" Text="Fecha Inicial"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFechaIniUsuario" runat="server" ReadOnly="false" Enabled="true"
                                    Width="200" MaxLength="10"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtenderPopUpUs" runat="server" TargetControlID="txtFechaIniUsuario">
                                </asp:CalendarExtender>
                                <asp:RequiredFieldValidator ID="rfvFechaIniUsuario" runat="server" ControlToValidate="txtFechaIniUsuario"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosUsuarios">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblFechaFinUsuario" runat="server" Text="Fecha Fin"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFechaFinUsuario" runat="server" ReadOnly="false" Enabled="true"
                                    Width="200" MaxLength="10"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtenderPopUpUs2" runat="server" TargetControlID="txtFechaFinUsuario">
                                </asp:CalendarExtender>
                                <asp:RequiredFieldValidator ID="rfvFechaFinUsuario" runat="server" ControlToValidate="txtFechaFinUsuario"
                                    ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosUsuarios">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td width="30%">
                                <asp:Label ID="lblComentarioUsuario" runat="server" Text="Comentarios"></asp:Label>
                            </td>
                            <td width="70%">
                                <asp:TextBox ID="txtComentariosUsuarios" runat="server" TextMode="MultiLine" Width="200"
                                    Height="50"></asp:TextBox>
                            </td>
                        </tr>
                        <%--AM.20130701 CheckBox oculto para editar--%>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbEditarUsuario" runat="server" Visible="false" Checked="False" />
                            </td>
                        </tr>
                        <tr>
                            <%--AM.20130701 CheckBox oculto para baja--%>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="cbBajaUsuario" runat="server" Visible="false" Checked="False" />
                                </td>
                            </tr>
                            <%--AM.20130701 text oculto para baja--%>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtRegistroRelacionUsuario" runat="server" Visible="False"></asp:TextBox>
                                </td>
                            </tr>
                    </table>
                    <div align="center" class="footerEtq">
                        <asp:Button ID="btnGuardarUsuario" runat="server" Text="Guardar" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                          OnClick="btnGuardarUsuario_Click"
                         />&nbsp;
                        <asp:Button ID="btnCancelarUsuario" runat="server" Text="Cancelar" CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                          OnClientClick="return Hidepopup()"
                         />
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeUsuarios" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeUsuarios" runat="server" DropShadow="false" PopupControlID="pnlAddEditUsuarios"
                    TargetControlID="lnkFakeUsuarios" BackgroundCssClass="modalBackground">
                </asp:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="ProcesandoAltaUserID" AssociatedUpdatePanelID="UpDatosUsuarios">
            <ProgressTemplate>
                <div class="modalProgress">
                    <div class="centerProgress">
                        <asp:Image runat="server" ID="imgUsuario" ImageUrl="~/images/loader.gif" />
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpeUsuarios" runat="server" TargetControlID="pDatosUsuarios"
        ExpandControlID="pHeaderUsuarios" CollapseControlID="pHeaderUsuarios" CollapsedText="Mostrar..."
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse5" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical">
    </asp:CollapsiblePanelExtender>
    <br />
    <br />
    <br />
    <!--Comentarios-->
    <asp:Panel ID="pComentarios" runat="server">
        <asp:Table ID="tblComentarios" runat="server" Width="100%">
            <asp:TableRow ID="tblComentariosF1" runat="server">
                <asp:TableCell ID="tblComentariosC1" runat="server">
                    Comentarios del administrador:
                </asp:TableCell>
                <asp:TableCell ID="tblComentariosC2" runat="server">
                    Comentarios del empleado:
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblComentariosF2" runat="server">
                <asp:TableCell ID="tblComentariosC3" runat="server">
                    <asp:TextBox ID="txtComentariosAdmin" runat="server" TextMode="MultiLine" Width="50%"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell ID="tblComentariosC4" runat="server">
                    <asp:TextBox ID="txtComenariosEmple" runat="server" TextMode="MultiLine" Width="50%"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:LinkButton ID="lbtnGuardarComentAdmin" runat="server" OnClick="lbtnGuardarComentAdmin_Click"
            Text="[ Guardar Comentarios ]" />
    </asp:Panel>
    <br />
    <!--Politicas de Uso-->
    <asp:Panel ID="pPoliticasUso" runat="server" CssClass="headerCCustodia">
        <asp:Table ID="tblPoliticasUso" runat="server" Width="100%">
            <asp:TableRow ID="trPoliticasUso1" runat="server">
                <asp:TableCell ID="tcPoliticasUso1" runat="server">
                    <asp:Label ID="lblHeaderPoliticasUso" runat="server" CssClass="titleSeccionCCustodia"
                        Text="Politicas de Uso"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="tcPoliticasUso2" runat="server" HorizontalAlign="Right">
                    <asp:Image ID="imgExpandCollapse4" runat="server" ImageAlign="Middle" Style="cursor: pointer" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pContentPoliticas" runat="server">
        <ul>
            <li>Es responsabilidad de la persona a la cual se le ha asignado un aparato telefónico
                con su código y extensión el buen uso de los mismos.</li>
            <li>Utilizar el teléfono como medio de comunicación para asuntos relacionados con lo
                laboral. </li>
            <li>La asignación del aparato telefónico queda bajo su custodia, siendo responsable
                del cuidado físico. </li>
            <li>Cualquier daño o pérdida de este equipo será cubierta por el usuario.</li>
            <li>El equipo no puede ser cambiado de lugar, modificar su configuración, instalar nuevo
                sw ni realizar cambio alguno sin la previa autorización del área de Telefonía Corporativa y/o Call Center.</li>
            <li>Telefonía Corporativa y/o Call Center estará encargado de controlar y vigilar el
                inventario de los equipos telefónicos y objetos asociados al área de telefonía por
                lo que cualquier actividad de mantenimiento preventivo o correctivo, cambios en
                la configuración o reporte de fallas en el funcionamiento deberá ser reportado en Service Desk.</li>
            <li>Está prohibido hacer uso de servicios de entretenimiento y cargos telefónicos.</li>
            <li>La asignación de las claves de acceso telefónico es personal e intransferible.</li>
            <li>El usuario tiene conocimiento que las llamadas que genere y reciba podrán ser monitoreadas.</li>
            <li>El usuario tiene conocimiento que los registros de las llamadas que generé y reciba
                podrán ser consultadas por él y por su jefe directo.</li>
            <li>El usuario tiene conocimiento que cualquier cambio o nuevo requerimiento de su servicio
                tendrá que ser a través de Service desk <asp:HyperLink runat="server" NavigateUrl="http://servicedesk.mx.att.com/SDPortal/" Text="http://servicedesk.mx.att.com/SDPortal/"></asp:HyperLink></li>
            <li>El no utilizar adecuadamente los recursos (teléfono, buzón, clave, etc.) será motivo
                de suspensión de los mismos.</li>
            <li>El usuario acepta que la información es correcta y aceptada por las políticas anteriores.</li>
        </ul>
        <asp:Table ID="tblFechasCC" runat="server">
            <asp:TableRow ID="trFechasCC1" runat="server">
                <asp:TableCell ID="tcFechasCC1" runat="server">
                Última modificación:
                </asp:TableCell>
                <asp:TableCell ID="tcFechasCC2" runat="server">
                    <asp:TextBox ID="txtUltimaMod" runat="server" ReadOnly="true" Enabled="false" Width="200"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="trFchasCC2" runat="server">
                <asp:TableCell ID="tcFechasCC3" runat="server">
                Último envío
                </asp:TableCell>
                <asp:TableCell ID="tcFechasCC4" runat="server">
                    <asp:TextBox ID="txtUltimoEnvio" runat="server" ReadOnly="true" Enabled="false" Width="200"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="cpePoliticasUso" runat="server" TargetControlID="pContentPoliticas"
        ExpandControlID="pPoliticasUso" CollapseControlID="pPoliticasUso" CollapsedText="Mostrar..."
        ExpandedText="Ocultar" ImageControlID="imgExpandCollapse4" ExpandedImage="~/images/up-arrow-square-blue.png"
        CollapsedImage="~/images/down-arrow-square-blue.png" ExpandDirection="Vertical">
    </asp:CollapsiblePanelExtender>
    <!--Editar Carta Custodia-->
    <asp:Table ID="tblEditCC" runat="server" Width="100%">
        <asp:TableRow ID="trEditCC1" runat="server">
            <asp:TableCell ID="tcEditCC1" runat="server" HorizontalAlign="Right">
                <asp:Button ID="btnEnviarCCustodiaEmple" runat="server" Text="Enviar carta custodia al empleado"
                  CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
                  OnClick="btnEnviarCCustodiaEmple_Click"  />
            </asp:TableCell>
            <asp:TableCell ID="tcEditCC2" runat="server" HorizontalAlign="Left">
                <asp:Button ID="btnCambiarEstatusPte" runat="server" Text="Cambiar a estatus PENDIENTE"
                    CssClass="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" 
                    OnClick="btnCambiarEstatusPte_Click"  />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <!--Carta Custodia Emple-->
    <asp:Table ID="tblEmpleCCust" runat="server" Width="100%" Visible="False">
        <asp:TableRow ID="trEmpleCCust1" runat="server">
            <asp:TableCell ID="tcEmpleCCust1" runat="server" HorizontalAlign="Right">
                <asp:Button ID="btnAceptarCCust" runat="server" Text="Aceptar" OnClick="btnAceptarCCust_Click" />
            </asp:TableCell>
            <asp:TableCell ID="tcEmpleCCust2" runat="server" HorizontalAlign="Left">
                <asp:Button ID="btnRechazarCCust" runat="server" Text="Rechazar" OnClick="btnRechazarCCust_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <!--Exportar a PDF-->
    <asp:ImageButton ID="imgbPDFExport" runat="server" ImageUrl="~/images/adobe-pdf-logo.png"
        Width="4%" OnClick="imgbPDFExport_Click" />
    <asp:AlwaysVisibleControlExtender ID="avcePDFExport" runat="server" TargetControlID="imgbPDFExport"
        VerticalSide="Top" VerticalOffset="20" HorizontalSide="Right" HorizontalOffset="20"
        ScrollEffectDuration=".1" />
    <%--    AM.20130712 Agrego popup para confirmar envio de CCust--%>
    <!--Modal PopUp para confirmar envio-->
    <asp:Panel ID="pnlConfirmarEnvio" runat="server" CssClass="modalPopup" Style="display: none"
        Width="300" Height="90">
        <asp:Label Font-Bold="true" ID="Label1" runat="server" Text="Esta carta custodia ya ha sido enviada. ¿Desea volver a enviarla?"></asp:Label>
        <br />
        <br />
        <table align="center">
            <tr>
                <td>
                    <asp:Button ID="btnAceptarEnvioCCust" runat="server" Text="Aceptar" OnClick="btnAceptarEnvioCCust_ConfEnvio" />
                </td>
                <td>
                    <asp:Button ID="btnCancelarEnvioCCust" runat="server" Text="Cancelar" OnClientClick="return Hidepopup()" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:LinkButton ID="lnkFakeConfirmPopup" runat="server"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeConfirmEnvio" runat="server" DropShadow="false" PopupControlID="pnlConfirmarEnvio"
        TargetControlID="lnkFakeConfirmPopup" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <!--Modal PopUp para aviso de empleado en aceptar o rechazar ccustodia-->
    <asp:Panel ID="pnlNotificaEmple" runat="server" CssClass="modalPopup" Style="display: none"
        Width="300" Height="100">
        <asp:Label Font-Bold="true" ID="lblMensajeNotificaEmple1" runat="server"></asp:Label>
        <asp:Label Font-Bold="true" ID="lblMensajeNotificaEmple2" runat="server"></asp:Label>
        <br />
        <br />
        <table align="center">
            <tr>
                <td>
                    <asp:Button ID="btnNotificaEmpleCCust" runat="server" Text="Aceptar" OnClientClick="return Hidepopup()" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:LinkButton ID="lnkFakeNotificaEmple" runat="server"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeNotificaEmple" runat="server" DropShadow="false" PopupControlID="pnlNotificaEmple"
        TargetControlID="lnkFakeNotificaEmple" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <!--Modal Popup para la confirmación de la baja dele empleado-->
    <asp:Panel ID="pnlBajaEmple" runat="server" CssClass="modalPopup" HorizontalAlign="Justify"
        Style="display: none" Width="500" Height="300">
        <asp:Literal ID="lcEmpleEnBajaMsj" runat="server"></asp:Literal>
        <asp:Panel ID="pnlBotonesBajaEmpleado" HorizontalAlign="Center" runat="server">
            <asp:Label ID="lblFechaBajaEmple" runat="server" Text="Fecha Fin: "></asp:Label>
            <asp:TextBox ID="txtFechaBajaEmpleado" runat="server"></asp:TextBox>
            <asp:CalendarExtender ID="ceFechaBajaEmple" TargetControlID="txtFechaBajaEmpleado"
                runat="server">
            </asp:CalendarExtender>
            <br />
            <br />
            <asp:Button ID="btnAceptarBajaEmple" Text="Borrar" runat="server" CssClass="buttonEdit"
                OnClick="btnAceptarBajaEmple_Click" />
            <asp:Button ID="btnCancelarBajaEmple" Text="Cancelar" runat="server" OnClientClick="return Hidepopup()"
                CssClass="buttonCancel" />
        </asp:Panel>
    </asp:Panel>
    <asp:LinkButton ID="lnkFakeBajaEmple" runat="server"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeBajaEmple" runat="server" DropShadow="false" PopupControlID="pnlBajaEmple"
        TargetControlID="lnkFakeBajaEmple" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <!--Modal Popup para la reasingacion del jefe inmediato-->
    <asp:Panel ID="pnlReasignaEmple" CssClass="modalPopup" HorizontalAlign="Justify"
        Style="display: none" runat="server">
        <asp:Label ID="lblElimEmpleReasignaHeader" runat="server" Text="Eliminar empleado"
            ForeColor="Red" Font-Bold="true"></asp:Label>
        <asp:Literal ID="lcEmpleReasigna" runat="server"></asp:Literal>
        <asp:Panel ID="pnlGridView" runat="server">
            <asp:GridView ID="grvEmpleDepende" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField HeaderText="No. nómina" DataField="NominaA" HtmlEncode="true" />
                    <asp:BoundField HeaderText="Nombre" DataField="NomCompleto" HtmlEncode="true" />
                    <asp:BoundField HeaderText="Puesto" DataField="PuestoDesc" HtmlEncode="true" />
                </Columns>
            </asp:GridView>
        </asp:Panel>
        <asp:Literal ID="lcEmpleNuevoJefe" runat="server"></asp:Literal>
        <asp:DropDownList ID="drpNuevoEmpleResp" AppendDataBoundItems="true" runat="server">
            <asp:ListItem Text="-- Seleccione un nuevo jefe --" Value="" />
        </asp:DropDownList>
        <br />
        <br />
        <asp:Table ID="tblBtnsReasignaBajaEmple" runat="server" HorizontalAlign="Center">
            <asp:TableRow ID="tblBtnsReasignaBajaEmpleR1" runat="server">
                <asp:TableCell ID="tblBtnsReasignaBajaEmpleC1" runat="server">
                    <asp:Button ID="btnReasignarBajaEmple" Text="Asignar" runat="server" CssClass="buttonEdit"
                        OnClick="btnReasignarBajaEmple_Click" />
                </asp:TableCell>
                <asp:TableCell ID="tblBtnsReasignaBajaEmpleC2" runat="server">
                    <asp:Button ID="btnCancelarReasignaEmple" Text="Cancelar" runat="server" OnClientClick="return Hidepopup()"
                        CssClass="buttonCancel" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:LinkButton ID="lnkButtonFakeReasignaEmple" runat="server"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeReasingarBajaEmple" runat="server" DropShadow="false"
        PopupControlID="pnlReasignaEmple" TargetControlID="lnkButtonFakeReasignaEmple"
        BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
</asp:Content>
