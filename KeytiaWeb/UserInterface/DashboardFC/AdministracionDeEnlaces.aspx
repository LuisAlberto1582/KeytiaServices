<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AdministracionDeEnlaces.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.AdministracionDeEnlaces" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.25/css/jquery.dataTables.min.css">
<script type="text/javascript" src="https://code.jquery.com/jquery-3.5.1.js"></script>
<script type="text/javascript" charset="utf8" src="https://cdn.datatables.net/1.10.25/js/jquery.dataTables.js"></script>

<script type="text/javascript">
    var obj;

    var originalLink = window.location.href;
    
    if (originalLink.includes("enlace")) {
        var strNueva = originalLink.split('&');
        originalLink = strNueva[0];
    }

    var pagePath = window.location.pathname;

    $(function () {
        $.ajax({
            type: "POST",
            url: pagePath + "/GetInfo2",
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess,
            failure: function (response) {
                alert(response.d);
            },
            error: function (response) {
                alert(response.d);
            }
        });
    });

    function OnSuccess(response) {
        obj = response.d;
        $("[id*=GridView1]").DataTable(
            {
                bLengthChange: true,
                lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                bFilter: true,
                bSort: true,
                bPaginate: true,
                data: response.d,
                columnDefs: [
                    {
                        "targets": [1],
                        "data": "NombreCajero",
                        render: function (data, type, row, meta) {
                            var stringWithoutExtraSpaces = data.replace(/\s+/g, ' ').trim();
                            var newString = stringWithoutExtraSpaces.replace(/ /g, "+");
                            return '<a href=' + originalLink + '&enlace=replace>'.replace("replace", newString) + data + '</a>'
                        }
                    }

                ],
                columns: [
                    { 'data': 'NombreCajero' },
                    { 'data': 'IdCajero' },
                    { 'data': 'TipoCajero' },
                    { 'data': 'FechaActivacion' },
                    { 'data': 'CantidadCajeros' },
                    { 'data': 'IpLookback' },
                    { 'data': 'IpGateway' },
                    { 'data': 'IpMasc' },
                    { 'data': 'IpWan' },
                    { 'data': 'IpAlarma' },
                    { 'data': 'IpCamara' },
                    { 'data': 'IpTunel' }
                ]
            });
    };

</script>

<script type="text/javascript">
    function MostrarTipoEnlace() {
        var elemento = document.getElementById("RegistroEnlace");
        elemento.style.visibility = "visible";
    }
</script>

<script type="text/javascript">
    function MostrarMensajesDeError(mensaje) {
        alert("Campos requeridos y no ingresados:\n" + mensaje);
    }
</script>

<script type="text/javascript">
    function GetConfirmation(movimiento, ipCajeros) {
        var nombreCajero = document.getElementById('<%=NombreDeCajero.ClientID %>').value;

        var reply = confirm("Deseas confirmar " + movimiento + " del cajero con nombre: " + nombreCajero);

        if (reply) {
            return true;
        }
        else {
            return false;
        }
    }
</script>

<script type="text/javascript">
    function AlertIpsErrones(ipCajeros) {
        var nombreCajero = document.getElementById('<%=NombreDeCajero.ClientID %>').value;

        alert("Errores en formato de IP's de cajeros. Registro dado de alta sin las siguientes direcciones:\n\n" + ipCajeros);
        
    }
</script>

<style type="text/css">
    #wrapper, #RegistroEnlace, #ConfiguracionFormulario {
        background-color: white;
        margin-bottom: 20px;
    }

    #firstTable {
        padding: 20px;
    }

    #tituloDocumento, #RegistroEnlace {
        padding-top: 20px;
    }

    #actualizarForm {
        padding-left: 1202px;
    }

    #ConfiguracionForm {
        padding: 20px;
    }

    .clientInfo {
        padding: 20px;
    }

    table {
        border-collapse: collapse;
        table-layout: fixed;
        /*width: 100%;*/
        font-size:12px;
        border: 1px solid black;
    }

    table td {
        padding: 5px;
        /*border: 1px solid black;*/
    }

    .wrapperForm {
        display: grid;
        grid-template-columns: 500px 500px;
        grid-row-gap: 15px;
        grid-column-gap: 150px;
        padding: 20px;
    }


    .gridInside {
        display: grid;
        grid-template-columns: 1fr 1fr;
    }

    .gridInside input {
        width: 327px;
    }

    .gridInsideV2 {
        display: grid;
        grid-template-columns: 1fr 1fr;
        grid-column-gap: 20px;
        grid-row-gap: 10px;
    }

    .grindInside input {
        width: 200px;
    }

    .contenedorForm {
        background-color: white;
        margin-bottom: 20px;
    }

    .cellColor {
        background-color: #f0f0f5;
    }

    #tituloFormEnlaces {
        padding-left: 20px;
        padding-top: 20px;
    }

    #btnAgregarEquiposDeSeguridad {
        padding-left: 20px;
    }

    .gridTest {
        display: grid;
        grid-template-columns: 300px 400px 400px 100px 100px;
        padding-left: 20px;
        grid-row-gap: 20px;
        grid-column-gap: 20px;
    }

    .gridTest div label {
        margin-right: 25px;
        color:red;
    }

    .gridTest select {
        width: 150px;
    }

    .gridTest input {
        width: 280px;
    }

    .gridTest input[type=submit] {
        width: 100px;
    }

</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
<div id="wrapper">
    <div id="tituloDocumento">
        <span class="caption-subject titlePortletKeytia">Administración de enlaces de cajeros</span>
    </div>
    <div style="padding-left:1535px;">
        <asp:LinkButton runat="server" id="LinkButton1" href="AdministracionDeCajeros.aspx" CssClass="btn btn-primary btn-sm">Regresar</asp:LinkButton>
    </div>

    <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered" style="border: 0px;">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server"></span>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12">
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:Panel ID="Rep2" runat="server" CssClass="col-md-12 col-sm-12">
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>



    <!-- PRIMERA TABLA PRUEBA -->
    
    <div style="width: 100%" id="firstTable">
        <asp:GridView ID="GridView1" runat="server" CssClass="display compact" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="NombreCajero" HeaderText="Nombre del enlace" />
                <asp:BoundField DataField="IdCajero" HeaderText="ID de enlace" />
                <asp:BoundField DataField="TipoCajero" HeaderText="Tipo de Enlace" />
                <asp:BoundField DataField="FechaActivacion" HeaderText="Fecha de Activacion" />
                <asp:BoundField DataField="CantidadCajeros" HeaderText="Cantidad de Cajeros" />
                <asp:BoundField DataField="IpLookback" HeaderText="IP Lookback" />
                <asp:BoundField DataField="IpGateway" HeaderText="IP Gateway" />
                <asp:BoundField DataField="IpMasc" HeaderText="IP Mascara" />
                <asp:BoundField DataField="IpWan" HeaderText="IP Wan" />
                <asp:BoundField DataField="IpAlarma" HeaderText="IP Alarma" />
                <asp:BoundField DataField="IpCamara" HeaderText="IP Camara" />
                <asp:BoundField DataField="IpTunel" HeaderText="IP Tunel" />
            </Columns>
        </asp:GridView>
    </div>
    <div style="padding-left:20px; padding-bottom:20px;">
        <%--<asp:LinkButton runat="server" id="LinkButton1" href="AdministracionDeCajeros.aspx" CssClass="btn btn-primary btn-sm">Regresar</asp:LinkButton>--%>
        <asp:Button ID="BtnAgregarEnlace" runat="server" onclick="AgregarEnlace_Click" CssClass="btn btn-primary btn-sm" Text="Agregar nuevo enlace" /> 
    </div>
</div>

<div id="contenedorFormAgregar" class="contenedorForm" runat="server" >
    <h3 id="tituloFormEnlaces"> Configuración de enlace de cajero</h3>
    <asp:Button ID="btnAgregarEquiposDeSeguridad" runat="server" onclick="AgregarEquipoSeguridad_Click" CssClass="btn btn-primary btn-sm" Text="Agregar Equipos de seguridad" style="margin-left:20px; margin: 20px; display:none;"/>
    <asp:Button ID="btnEliminarEquiposDeSeguridad" runat="server" onclick="EliminarBotonEquiposSeguridad_Click" CssClass="btn btn-primary btn-sm" Text="Eliminar Equipos de seguridad" style="margin-left:20px; margin: 20px; display:none;"/>
    <div class="wrapperForm">
        <div class="gridInside">
            <asp:Label ID="Label5" runat="server">Tipo de enlace:</asp:Label>
            <asp:DropDownList ID="TipoDeEnlace" runat="server">
                <asp:listitem text="Enlace Terrestre" value=902></asp:listitem>
                <asp:listitem text="Enlace Satelital" value=102></asp:listitem>
                <asp:listitem text="Enlace Celular" value="506"></asp:listitem>
            </asp:DropDownList>
        </div>
   
        <div class="gridInside">
            <asp:Label ID="Label3" runat="server">Fecha de activación:</asp:Label>
            <asp:TextBox ID="FechaActCajero"  type="date" runat="server" placeholder="Seleccionar fecha" />
            <asp:RequiredFieldValidator ID="ValidacionFechaCajero" runat="server" ControlToValidate="FechaActCajero" EnableClientScript="false"></asp:RequiredFieldValidator>
        </div>

        <div class="gridInside">
            <asp:Label ID="Label1" runat="server">Nombre de enlace:</asp:Label>
            <asp:TextBox ID="NombreDeCajero" runat="server" placeholder="Nombre de cajero" />
            <asp:RequiredFieldValidator ID="ValidacionNombreCajero" runat="server" ControlToValidate="NombreDeCajero" EnableClientScript="false"></asp:RequiredFieldValidator>
        </div>

        <div class="gridInside">
            <asp:Label ID="Label6" runat="server">ID de enlace:</asp:Label>
            <asp:TextBox ID="IdDeCajero" runat="server" placeholder="ID de cajero" />
            <asp:RequiredFieldValidator ID="ValidacionIdCajero" runat="server" ControlToValidate="IdDeCajero" EnableClientScript="false"></asp:RequiredFieldValidator>
        </div>

        <div class="gridInside">
            <asp:Label ID="Label4" runat="server">IP Lookback:</asp:Label>
            <asp:TextBox ID="IpCajLookback" runat="server" placeholder="IP Lookback" />
        </div>

        <div class="gridInside">
            <asp:Label ID="Label2" runat="server" >IP Gateway:</asp:Label>
            <asp:TextBox ID="IpCajGateway" runat="server" placeholder="IP Gateway" />
        </div>

        <div class="gridInside">
            <asp:Label ID="Label7" runat="server">IP Masc:</asp:Label>
            <asp:TextBox ID="IpCajMasc" runat="server" placeholder="IP Masc" />
        </div>

        <div class="gridInside">
            <asp:Label ID="Label9" runat="server">IP Wan:</asp:Label>
            <asp:TextBox ID="IpCajWan" runat="server" placeholder="IP Wan" />
        </div>

        <div class="gridInside">
            <asp:Label ID="Label11" runat="server" >IP Tunel:</asp:Label>
            <asp:TextBox ID="IpCajTunel" runat="server" placeholder="IP Tunel" />
        </div>

        <div>

        </div>

        <div class="gridInside">
            <asp:Label ID="lblIpCajAlarma" runat="server" style="display:none;">IP Alarma:</asp:Label>
            <asp:TextBox ID="IpCajAlarma" runat="server" placeholder="IP Alarma" style="display:none;"/>
        </div>

        <div class="gridInside" display="none">
            <asp:Label ID="lblIpLanAlarma" runat="server" style="display:none;">IP LAN Alarma:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLanAlarma" runat="server" placeholder="IP LAN Alarma" style="display:none;"/>
        </div>

        <div class="gridInside">
            <asp:Label ID="lblIpCctv" runat="server" style="display:none;">IP CCTV:</asp:Label>
            <asp:TextBox ID="IpCajCamara" runat="server" placeholder="IP CCTV" style="display:none;"/>
        </div>

        <div class="gridInside">
            <asp:Label ID="lblIpCajLanCctv" runat="server" style="display:none;">IP LAN CCTV:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLanCamara" runat="server" placeholder="IP LAN CCTV" style="display:none;"/>
        </div>

    </div>

    <div class="gridTest">
        <div>
            <asp:Label ID="Label10" runat="server">Tipo de cajero:</asp:Label>
            <asp:DropDownList ID="TipoDeCajero1" runat="server">
                <asp:listitem text="Frontal" value=709></asp:listitem>
                <asp:listitem text="Posterior" value=155></asp:listitem>
            </asp:DropDownList>
        </div>
        <div>
            <asp:Label ID="Label12" runat="server" >IP Cajero 1:</asp:Label>
            <asp:TextBox ID="cajeroIp1" runat="server" placeholder="IP Cajero 1" />
        </div>
        <div>
            <asp:Label ID="Label13" runat="server" >IP LAN Cajero 1:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLan1" runat="server" placeholder="IP Cajero 1" />
        </div>
        <div ID="btnAgregarCajero1div" runat="server">
            <asp:Button ID="btnAgregarCajero1" runat="server" CssClass="btn btn-primary btn-sm" Text="Agregar cajero" onclick="BtnAgregarCajero1_Click"/>
        </div>
        <div>
        </div>

        <div ID="tipoCajero2div" runat="server" >
            <asp:Label ID="Label8" runat="server">Tipo de cajero:</asp:Label>
            <asp:DropDownList ID="TipoDeCajero2" runat="server">
                <asp:listitem text="Frontal" value=709></asp:listitem>
                <asp:listitem text="Posterior" value=155></asp:listitem>
            </asp:DropDownList>
        </div>
        <div ID="ipCajero2div" runat="server">
            <asp:Label ID="Label14" runat="server" >IP Cajero 2:</asp:Label>
            <asp:TextBox ID="cajeroIp2" runat="server" placeholder="IP Cajero 2" />
        </div>
        <div ID="ipLanCajero2div" runat="server">
            <asp:Label ID="Label15" runat="server" >IP LAN Cajero 2:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLan2" runat="server" placeholder="IP Cajero 2" />
        </div>
        <div ID="btnAgregarCajero2div" runat="server">
            <asp:Button ID="btnAgregarCajero2" runat="server" CssClass="btn btn-primary btn-sm" Text="Agregar cajero" onclick="BtnAgregarCajero2_Click"/>
        </div>
        <div ID="btnQuitarCajero2div" runat="server">
            <asp:Button ID="btnQuitarCajero2" runat="server" CssClass="btn btn-primary btn-sm" Text="Quitar cajero" onclick="BtnQuitarCajero2_Click"/>
        </div>

        <div ID="tipoCajero3div" runat="server">
            <asp:Label ID="Label16" runat="server">Tipo de cajero:</asp:Label>
            <asp:DropDownList ID="TipoDeCajero3" runat="server">
                <asp:listitem text="Frontal" value=709></asp:listitem>
                <asp:listitem text="Posterior" value=155></asp:listitem>
            </asp:DropDownList>
        </div>
        <div ID="ipCajero3div" runat="server">
            <asp:Label ID="Label17" runat="server" >IP Cajero 3:</asp:Label>
            <asp:TextBox ID="cajeroIp3" runat="server" placeholder="IP Cajero 3" />
        </div>
        <div ID="ipLanCajero3div" runat="server">
            <asp:Label ID="Label18" runat="server" >IP LAN Cajero 3:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLan3" runat="server" placeholder="IP Cajero 3" />
        </div>
        <div ID="btnAgregarCajero3div" runat="server">
            <asp:Button ID="btnAgregarCajero3" runat="server" CssClass="btn btn-primary btn-sm" Text="Agregar cajero" onclick="BtnAgregarCajero3_Click"/>
        </div>
        <div ID="btnQuitarCajero3div" runat="server">
            <asp:Button ID="btnQuitarCajero3" runat="server" CssClass="btn btn-primary btn-sm" Text="Quitar cajero" onclick="BtnQuitarCajero3_Click"/>
        </div>

        <div ID="tipoCajero4div" runat="server">
            <asp:Label ID="Label19" runat="server">Tipo de cajero:</asp:Label>
            <asp:DropDownList ID="TipoDeCajero4" runat="server">
                <asp:listitem text="Frontal" value=709></asp:listitem>
                <asp:listitem text="Posterior" value=155></asp:listitem>
            </asp:DropDownList>
        </div>
        <div ID="ipCajero4div" runat="server">
            <asp:Label ID="Label20" runat="server" >IP Cajero 4:</asp:Label>
            <asp:TextBox ID="cajeroIp4" runat="server" placeholder="IP Cajero 4" />
        </div>
        <div ID="ipLanCajero4div" runat="server">
            <asp:Label ID="Label21" runat="server" >IP LAN Cajero 4:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLan4" runat="server" placeholder="IP Cajero 4" />
        </div>
        <div ID="btnAgregarCajero4div" runat="server">
            <asp:Button ID="btnAgregarCajero4" runat="server" CssClass="btn btn-primary btn-sm" Text="Agregar cajero" onclick="BtnAgregarCajero4_Click"/>
        </div>
        <div ID="btnQuitarCajero4div" runat="server">
            <asp:Button ID="btnQuitarCajero4" runat="server" CssClass="btn btn-primary btn-sm" Text="Quitar cajero" onclick="BtnQuitarCajero4_Click"/>
        </div>

        <div ID="tipoCajero5div" runat="server">
            <asp:Label ID="Label22" runat="server">Tipo de cajero:</asp:Label>
            <asp:DropDownList ID="TipoDeCajero5" runat="server">
                <asp:listitem text="Frontal" value=709></asp:listitem>
                <asp:listitem text="Posterior" value=155></asp:listitem>
            </asp:DropDownList>
        </div>
        <div ID="ipCajero5div" runat="server">
            <asp:Label ID="Label23" runat="server" >IP Cajero 5:</asp:Label>
            <asp:TextBox ID="cajeroIp5" runat="server" placeholder="IP Cajero 5" />
        </div>
        <div ID="ipLanCajero5div" runat="server">
            <asp:Label ID="Label24" runat="server" >IP LAN Cajero 5:</asp:Label>
            <asp:TextBox ID="txtBoxCajIpLan5" runat="server" placeholder="IP Cajero 5" />
        </div>
        <div ID="btnQuitarCajero5div" runat="server">
            <asp:Button ID="btnQuitarCajero5" runat="server" CssClass="btn btn-primary btn-sm" Text="Quitar cajero" onclick="BtnQuitarCajero5_Click"/>
        </div>
        <div>
            <%--<asp:Button ID="Button7" runat="server" CssClass="btn btn-primary btn-sm" Text="Agregar cajero" onclick="BtnAgregarCajero5_Click"/>--%>
        </div>
    </div>


    <div style="padding-top:20px; padding-left:20px; padding-bottom:20px;">
        <asp:LinkButton runat="server" id="BtnEliminarEnlace" onclick="BtnEliminar_Click" CssClass="btn btn-primary btn-sm" display="none" OnClientClick="return GetConfirmation('la eliminación');">Eliminar</asp:LinkButton>
        <asp:Button ID="BtnEditarEnlace" runat="server" onclick="BtnEditar_Click" CssClass="btn btn-primary btn-sm" Text="Editar" display="none" /> 
        <asp:Button ID="BtnGuardarEnlace" runat="server" onclick="BtnGuardar_Click" CssClass="btn btn-primary btn-sm" Text="Guardar" display="none" OnClientClick="return GetConfirmation('el alta');"/>
        <asp:Button ID="BtnGuardarEdicion" runat="server" onclick="BtnGuardarEdicion_Click" CssClass="btn btn-primary btn-sm" Text="Guardar Edicion" display="none" OnClientClick="return GetConfirmation('la edición');"/>
        <asp:Button ID="BtnCancelarEdicion" runat="server" onclick="BtnCancelarEdicion_Click" CssClass="btn btn-primary btn-sm" Text="Cancelar" display="none"/>
        <asp:Button ID="BtnCancelarIngresoRegistro" runat="server" onclick="BtnCancelarIngresoRegistro_Click" CssClass="btn btn-primary btn-sm" Text="Cancelar" display="none"/>
    </div>

</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
</asp:Content>

