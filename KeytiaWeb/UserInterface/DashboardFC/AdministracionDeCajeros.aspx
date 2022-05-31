<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AdministracionDeCajeros.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.AdministracionDeCajeros" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.25/css/jquery.dataTables.min.css">
<script type="text/javascript" src="https://code.jquery.com/jquery-3.5.1.js"></script>
<script type="text/javascript" charset="utf8" src="https://cdn.datatables.net/1.10.25/js/jquery.dataTables.js"></script>

<script type="text/javascript">
        var pagePath = window.location.pathname;
        var typeCheckBox = "";
        $(function () {
            $.ajax({
                type: "POST",
                url: pagePath + "/GetInfo" + typeCheckBox,
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
            $("[id*=example]").DataTable(
                {
                    bLengthChange: true,
                    lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                    bFilter: true,
                    bSort: true,
                    bPaginate: true,
                    data: response.d,
                    autoWidth: true,
                    columnDefs: [
                        {
                            "targets": [0],
                            "data": "NombreCajero", 
                            render: function (data, type, row, meta)
                            {
                                var stringWithoutExtraSpaces = data.replace(/\s+/g, ' ').trim();
                                var newString = stringWithoutExtraSpaces.replace(/ /g, "+");
                                return '<a href=AdministracionDeEnlaces.aspx?qry=replace>'.replace("replace", newString) + data + '</a>'
                            }
                        },
                        {
                            "targets": [1],
                            "data": "FolioCajero",
                            render: function (data, type, row, meta) {
                                return '<a href=ConfiguracionUbicacionesCajero.aspx?qry=replace>'.replace("replace", data) + data + '</a>'
                            }
                        },
                    ],
                    columns: [
                        { 'data': 'NombreCajero' },
                        { 'data': 'FolioCajero' },
                        { 'data': 'Domicilio' },
                        { 'data': 'EntreCalles' },
                        { 'data': 'Colonia' },
                        { 'data': 'Ciudad' },
                        { 'data': 'Municipio' },
                        { 'data': 'Estado' },
                        { 'data': 'CodigoPostal' },
                        { 'data': 'Latitud' },
                        { 'data': 'Longitud' },
                        { 'data': 'Telefono' },
                        { 'data': 'Contacto' },
                        { 'data': 'ContactoRegional' }
                    ]
                });
        };
</script>

<script type="text/javascript">
    function aspcheck1True() {
        if (document.getElementById('<%=CheckBox2.ClientID %>').checked == true) {
            typeCheckBox = "2";
            table.ajax.url(pagePath + "/GetInfo2").load();
            //var pagePath = window.location.pathname;
            //$(function () {
            //    $.ajax({
            //        type: "POST",
            //        url: pagePath + "/GetInfo2",
            //        data: '{}',
            //        contentType: "application/json; charset=utf-8",
            //        dataType: "json",
            //        success: OnSuccess,
            //        failure: function (response) {
            //            alert(response.d);
            //        },
            //        error: function (response) {
            //            alert(response.d);
            //        }
            //    });
            //});
        }
    }

    function aspcheck2True() {
        document.getElementById('<%=CheckBox1.ClientID %>').checked = false;
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
        padding-left: 20px;
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

</style>

<style type="text/css">
    #wrapper {
        background-color: white;
    }

    #firstTable {
        padding: 20px;
    }

    #tituloDocumento {
        padding-top: 20px;
        padding-left: 20px;
    }
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
<div id="wrapper">
    <label>
        <asp:CheckBox ID="CheckBox1" runat="server" onclick="aspcheck1True();" />
        <asp:CheckBox ID="CheckBox2" runat="server" onclick="aspcheck2True();" />
    </label>
    <div id="tituloDocumento">
        <span class="caption-subject titlePortletKeytia">Administración de ubicaciones de cajeros</span>
    </div>
    <div style="width: 100%" id="firstTable">
        <asp:GridView ID="example" runat="server" CssClass="display compact" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="NombreCajero" HeaderText="Ubicación" />
                <asp:BoundField DataField="Folio" HeaderText="Folio de ubicación" />
                <asp:BoundField DataField="Domicilio" HeaderText="Domicilio" />
                <asp:BoundField DataField="EntreCalles" HeaderText="Entre calles" />
                <asp:BoundField DataField="Colonia" HeaderText="Colonia" />
                <asp:BoundField DataField="Ciudad" HeaderText="Ciudad" />
                <asp:BoundField DataField="Municipio" HeaderText="Municipio" />
                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                <asp:BoundField DataField="CodigoPostal" HeaderText="Código Postal" />
                <asp:BoundField DataField="Latitud" HeaderText="Latitud" />
                <asp:BoundField DataField="Longitud" HeaderText="Longitud" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="Contacto" HeaderText="Contacto" />
                <asp:BoundField DataField="ContactoRegional" HeaderText="Contacto Regional" />
            </Columns>
        </asp:GridView>
    </div>
    <div style="padding-left:20px; padding-bottom:20px;">
        <asp:Button ID="BtnAgregarCajero" runat="server" onclick="AgregarCajero_Click" CssClass="btn btn-primary btn-sm" Text="Agregar Ubicación de Cajero" /> 
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
</asp:Content>
