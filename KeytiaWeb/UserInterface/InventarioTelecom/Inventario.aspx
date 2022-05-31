<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="Inventario.aspx.cs" Inherits="KeytiaWeb.UserInterface.InventarioTelecom.Inventario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <link rel="stylesheet" type="text/css" href="css/w2ui.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/w2ui.js"></script>
    <script type="text/javascript">
        //NZ Se agrega esta linea para que no haya conflicto entre las dos versiones de jQuery que se tiene en el proyecto. Sin esta linea no funciona.
        var $jQuery2_2 = $.noConflict(true);

        var pagePath = window.location.pathname;
        var dataJSON;
        $(function () {

            $.ajax({
                url: pagePath + "/ConsultaInventarioTelecom",
                async: false,
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.d != null) {
                        dataJSON = JSON.parse(result.d);
                    }
                    else { dataJSON = JSON.parse(errTxt); }
                },
                error: function (XMLHttpRequest, callStatus, errorThrown) {
                    dataJSON = JSON.parse(errTxt);
                }
            });
        });

        $(function () {
            var obj;
            try {
                obj = dataJSON;
            }
            catch (err) {
                obj = JSON.parse(errTxt);
            }
            var dataDisp = obj.records;
            var totalRows = dataWithTotal(dataDisp);
            for (var i = 0; i < totalRows.length; i++) {
                dataDisp.push(totalRows[i]);
            };

            var cols = [
            { field: "CrTelecom", caption: 'Cr Telecom', size: '80px', sortable: true },
            { field: "Nombre", caption: 'Nombre', size: '140px', sortable: true },
            { field: "TipoEdificio", caption: 'Tipo Edificio', size: '140px', sortable: true },
            { field: "DirIP", caption: 'Dirección IP', size: '100px', sortable: true },
            { field: "Recurso", caption: 'Recurso', size: '80px', sortable: true },
            { field: "GIG-E_Ocupados", caption: 'GE Ocupados', size: '90px', sortable: true },
            { field: "GIG-E_OFF", caption: 'GE OFF', size: '90px', sortable: true },
            { field: "GIG-E_Abajo", caption: 'GE Abajo', size: '90px', sortable: true },
            { field: "Fa_Ocupados", caption: 'FE Ocupados', size: '90px', sortable: true },
            { field: "Fa_OFF", caption: 'FE OFF', size: '90px', sortable: true },
            { field: "Fa_Abajo", caption: 'FE Abajo', size: '90px', sortable: true },
            { field: "TotalPuertos", caption: 'No. de Puertos', size: '100px', sortable: true },
            { field: "Admin", caption: 'Administrador', size: '100px', sortable: true },
            { field: "Hostname", caption: 'Hostname', size: '120px', sortable: true },
            { field: "Estatus", caption: 'Estatus', size: '120px', sortable: true },
            { field: "Marca", caption: 'Marca', size: '60px', sortable: true },
            { field: "Modelo", caption: 'Modelo', size: '120px', sortable: true },
            { field: "NoSerie", caption: 'No. de Serie', size: '100px', sortable: true },
            { field: "Ver_IOS", caption: 'Versión de OS', size: '220px', sortable: true },
            { field: "Domicilio", caption: 'Domicilio', size: '500px', sortable: true },
            { field: "Comentarios", caption: 'Comentarios', size: '300px', sortable: true },
            { field: "Stackeados", caption: 'Stackeados', size: '80px', sortable: true },
            { field: "FechaPoleo", caption: 'Fecha Poleo', size: '135px', sortable: true },
            ];

            $jQuery2_2('#grid').w2grid({
                name: 'grid',
                header: 'Lista de Equipos',
                show: {
                    toolbar: true
                },
                disableCVS: true,
                searches: [
                { type: 'int', field: 'CrTelecom', caption: 'Cr Telecom' },
                { type: 'text', field: 'Nombre', caption: 'Nombre' },
                { type: 'text', field: 'TipoEdificio', caption: 'Tipo Edificio' },
                { type: 'text', field: 'DirIP', caption: 'Dirección IP' },
                { type: 'text', field: 'Modelo', caption: 'Modelo' },
                { type: 'text', field: 'Hostname', caption: 'Hostname' },
                { type: 'text', field: 'NoSerie', caption: 'Número de Serie' },
                { type: 'int', field: 'Total Puertos', caption: 'Número de Puertos' },
                { type: 'text', field: 'Estatus', caption: 'Estatus' },
                { type: 'text', field: 'Domicilio', caption: 'Domicilio' },
                { type: 'text', field: 'Admin', caption: 'Administrador' }
                ],
                columns: cols,
                records: dataDisp
            });
        });

        function dataWithTotal(completeData) {
            var totals = [];
            var c_puertos = countPuertos(completeData);
            var total =
            {
                "recid": 0,
                "CrTelecom": "TOTALES",
                "Nombre": "",
                "TipoEdificio": "",
                "DirIP": "",
                "Recurso": "",
                "GIG-E_Ocupados": c_puertos[0],
                "Fa_Ocupados": c_puertos[1],
                "GIG-E_OFF": c_puertos[2],
                "Fa_OFF": c_puertos[3],
                "GIG-E_Abajo": c_puertos[4],
                "Fa_Abajo": c_puertos[5],
                "TotalPuertos": c_puertos[6],
                "Admin": "",
                "Hostname": "",
                "Estatus": "",
                "Marca": "",
                "Modelo": "",
                "NoSerie": "",
                "Ver_IOS": "",
                "Domicilio": "",
                "Comentarios": "",
                "Stackeados": "",
                "FechaPoleo": ""
            };
            totals.push(total);
            total =
            {
                "recid": -1,
                "CrTelecom": "TOTALES",
                "Nombre": "POR TIPO",
                "TipoEdificio": "",
                "DirIP": "",
                "Recurso": "",
                "GIG-E_Ocupados": "GE",
                "Fa_Ocupados": "FE",
                "GIG-E_OFF": c_puertos[7],
                "Fa_OFF": c_puertos[8],
                "GIG-E_Abajo": "",
                "Fa_Abajo": "",
                "TotalPuertos": "",
                "Admin": "",
                "Hostname": "",
                "Estatus": "",
                "Marca": "",
                "Modelo": "",
                "NoSerie": "",
                "Ver_IOS": "",
                "Domicilio": "",
                "Comentarios": "",
                "Stackeados": "",
                "FechaPoleo": "",
                "style": "color: blue"
            };
            totals.push(total);
            total =
            {
                "recid": -2,
                "CrTelecom": "TOTALES",
                "Nombre": "POR EQUIPO",
                "TipoEdificio": "",
                "DirIP": "",
                "Recurso": "",
                "GIG-E_Ocupados": "SWITCH",
                "Fa_Ocupados": "ROUTER",
                "GIG-E_OFF": c_puertos[9],
                "Fa_OFF": c_puertos[10],
                "GIG-E_Abajo": "",
                "Fa_Abajo": "",
                "TotalPuertos": "",
                "Admin": "",
                "Hostname": "",
                "Estatus": "",
                "Marca": "",
                "Modelo": "",
                "NoSerie": "",
                "Ver_IOS": "",
                "Domicilio": "",
                "Comentarios": "",
                "Stackeados": "",
                "FechaPoleo": "",
                "style": "background-color: #C2F5B4"
            };
            totals.push(total);
            return totals;
        }

        function countPuertos(dataIn) {
            var numG_Oc = 0;
            var numFa_Oc = 0;
            var numG_Off = 0;
            var numFa_Off = 0;
            var numG_A = 0;
            var numFa_A = 0;
            var numTotal = 0;
            var numSwitch = 0;
            var numRouter = 0;
            var crT = dataIn[0]["CrTelecom"];
            var write = true;
            for (var i = 0; i < dataIn.length; i++) {
                if (dataIn[i]["CrTelecom"] == crT) {
                    numG_Oc += dataIn[i]["GIG-E_Ocupados"];
                    numFa_Oc += dataIn[i]["Fa_Ocupados"];
                    numG_Off += dataIn[i]["GIG-E_OFF"];
                    numFa_Off += dataIn[i]["Fa_OFF"];
                    numG_A += dataIn[i]["GIG-E_Abajo"];
                    numFa_A += dataIn[i]["Fa_Abajo"];
                    numTotal += dataIn[i]["TotalPuertos"];
                    if (dataIn[i]["Recurso"] == "SWITCH") {
                        numSwitch += dataIn[i]["TotalPuertos"];
                    }
                    else {
                        numRouter += dataIn[i]["TotalPuertos"];
                    }
                }
                if (dataIn[i + 1]) {
                    if (dataIn[i + 1]["CrTelecom"] != crT) {
                        crT = dataIn[i + 1]["CrTelecom"];
                    }
                }
            }
            var totalArr = [];
            totalArr.push(numG_Oc);
            totalArr.push(numFa_Oc);
            totalArr.push(numG_Off);
            totalArr.push(numFa_Off);
            totalArr.push(numG_A);
            totalArr.push(numFa_A);
            totalArr.push(numTotal);
            totalArr.push(numG_Oc + numG_Off + numG_A);
            totalArr.push(numFa_Oc + numFa_Off + numFa_A);
            totalArr.push(numSwitch);
            totalArr.push(numRouter);
            return totalArr;
        }
        var errTxt = '{"records": [{"recid": 0,"CrTelecom": 0,"Nombre": "Error de Lectura", "TipoEdificio": 0, "DirIP": "0","Recurso": "-","GIG-E_Ocupados": 0,"Fa_Ocupados": 0,"GIG-E_OFF": 0,"Fa_OFF": 0,"GIG-E_Abajo": 0,"Fa_Abajo": 0,"TotalPuertos": 0,"Admin": "-","Hostname": "-","Estatus": "-","Marca": "-","Modelo": "-","NoSerie": "-","Ver_IOS": "-","Domicilio": "-","Comentarios": "-","Stackeados": "-","FechaPoleo": "-"}]}';

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <%--Barra con titulos y botones--%>
    <div>
        <asp:Label ID="lblTitulo" runat="server" CssClass="page-title-keytia">Lista de Equipos Telecom</asp:Label>

        <asp:Panel ID="pnlBarra" runat="server" CssClass="fg-toolbar ui-toolbar ui-widget-header">
            &nbsp;&nbsp;<asp:Button ID="btnExportarXLS" runat="server" Text="Exportar XLS" CssClass="buttonPlay"
                OnClick="btnExportarXLS_Click" />
        </asp:Panel>
    </div>
    <div id="grid" style="width: 100%; height: 550px;">
    </div>
    <div id="grid_json" style="margin: auto;">
    </div>
</asp:Content>
