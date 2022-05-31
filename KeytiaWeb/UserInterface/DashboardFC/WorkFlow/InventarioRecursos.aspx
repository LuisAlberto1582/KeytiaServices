<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="InventarioRecursos.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.InventarioRecursos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <link href="css/w2ui.css" rel="stylesheet" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/w2ui.js"></script>

<script type="text/javascript">


    var $jQuery2_2 = $.noConflict(true);

    var pagePath = window.location.pathname;
    var dataJSON;

    function GetInfo(method) {
        $jQuery2_2.ajax({
            url: pagePath + "/" + method,
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
    }

    function InventarioRecursos(method) {

        GetInfo(method);

        var obj = dataJSON;
        var dataDisp = obj.records;
        var cols = [
        { field: "recid", caption: '#', size: '50px', sortable: true, frozen: true },
        { field: "Tel", caption: 'Línea', size: '120px', sortable: true, frozen: true },
        { field: "Plan", caption: 'Plan', size: '200px', sortable: true },
        { field: "Marca", caption: 'Marca', size: '100px', sortable: true },
        { field: "Modelo", caption: 'Modelo', size: '100px', sortable: true },
        { field: "IMEI", caption: 'IMEI', size: '150px', sortable: true },
        { field: "SIMCard", caption: 'SIM', size: '200px', sortable: true },
        { field: "Estatus", caption: 'Estatus', size: '95px', sortable: true },
        { field: "Estado", caption: 'Estado', size: '95px', sortable: true },
        { field: "Empleado", caption: 'Empleado', size: '300px', sortable: true },
        { field: "Puesto", caption: 'Puesto', size: '300px', sortable: true },
        { field: "Empresa", caption: 'Empresa', size: '140px', sortable: true }
        ];

        $jQuery2_2('#invRecursosDiv').w2grid({
            name: 'invRecursosDiv',
            header: 'Inventario de Recursos',
            show: {
                header: true,
                toolbar: true,
                lineNumbers: false,
                footer: true
            },
            reorderColumns: true,
            searches: [
            { type: 'text', field: 'recid', caption: '#' },
            { type: 'text', field: 'Tel', caption: 'Línea' },
            { type: 'text', field: 'Plan', caption: 'Plan' },
            { type: 'text', field: 'Marca', caption: 'Marca' },
            { type: 'text', field: 'Modelo', caption: 'Modelo' },
            { type: 'text', field: 'IMEI', caption: 'IMEI' },
            { type: 'text', field: 'SIMCard', caption: 'SIM' },
            { type: 'text', field: 'Estatus', caption: 'Estatus' },
            { type: 'text', field: 'Estado', caption: 'Estado' },
            { type: 'text', field: 'Empleado', caption: 'Empleado' },
            { type: 'text', field: 'Puesto', caption: 'Puesto' },
            { type: 'text', field: 'Empresa', caption: 'Empresa' }
            ],
            columns: cols,
            records: dataDisp
        });
    };
  

</script>

 <div class="clearfix"></div>      
            <div class="portlet solid bordered viewDetailPortlet" runat="server" id="divExportar">
                <div class="portlet-title">
                    <div class="actions col-md-2 col-sm-2 col-lg-2 col-xs-2">
                        <p style="text-align: center;">
                            <img src="../../img/svg/Asset 22.svg" alt="">
                            Exportar:&nbsp;&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" OnClick="btnExportarXLS_Click" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                        </p>
                    </div>
                </div>
            </div>      
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
      <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Inventario de Recursos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <div class="row">
                                <div class ="col-sm-12" >
                                    <div id="invRecursosDiv" style="height:400px;"></div>
                                </div>
                            </div>
                       </div>
                    </div>
                    </div>
            </asp:Panel>
            </asp:Panel>
            </asp:Panel>
</asp:Content>
