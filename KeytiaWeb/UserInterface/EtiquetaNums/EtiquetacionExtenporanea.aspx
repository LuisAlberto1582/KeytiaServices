<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="EtiquetacionExtenporanea.aspx.cs" Inherits="KeytiaWeb.UserInterface.EtiquetaNums.EtiquetacionExtenporanea" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
     <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;
        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);

            // Place here the first init of the autocomplete
            InitAutoCompl();
        });

        function InitializeRequest(sender, args) {
        }

        function EndRequest(sender, args) {
            // after update occur on UpdatePanel re-init the Autocomplete
            InitAutoCompl();
        }

        function InitAutoCompl() {
            $("#" + "<%=txtBusqueda.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetEmploye",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Nombre, description: item.Clave };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#" + "<%=txtBusqueda.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtEmpleId.ClientID %>").val(ui.item.description);
                }
            });
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
        <style>
        .btn-primary:hover{
            background:#337ab7;
            border-color:#2e6da4;
            color:#fff;
        }
        .badge{
            font-size:14px !important;
        }
        .tableFooter{
            border: 0px;
            text-align:right;
        }
        .tableFooteer1{
            border: 0px;
            text-align:left;
        }
        .table {
            width: 100%;
            margin-bottom: 8px;
        }

        .table-fixed-nz {
            max-width: 100%;
            overflow-x: auto;
            border-right: 0px solid #e7ecf1;
            overflow-y: auto;
            max-height: 100%;
        }
   .switch {
    position: relative;
    display: inline-block;
    width: 47px;
    height: 20px;/*34*/
    bottom:-4px;
    }
    .switch input { 
    opacity: 0;
    width: 0;
    height: 0;
    }
    .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: #ccc;
    -webkit-transition: .4s;
    transition: .4s;
    margin: 0px 0px;
    }

    .slider:before {
    position: absolute;
    content: "";
    height: 14px;
    width: 14px;
    left: 4px;
    bottom: 3px;
    background-color: white;
    -webkit-transition: .4s;
    transition: .4s;
    }

    input:checked + .slider {
    background-color: #009639;
    right: -2px;
    }
    input:focus + .slider {
    box-shadow: 0 0 1px #009639;
    }
    input:checked + .slider:before {
    -webkit-transform: translateX(26px);
    -ms-transform: translateX(26px);
    transform: translateX(26px);
    }
    /* Rounded sliders */
    .slider.round {
    border-radius: 34px;
    }
    .slider.round:before {
    border-radius: 50%;
    }

/* The Modal (background) */
.modal1 {
  display: none; /* Hidden by default */
  position: fixed; /* Stay in place */
  z-index: 9000; /* Sit on top */
  padding-top: 100px; /* Location of the box */
  left: 0;
  top: 0;
  width: 100%; /* Full width */
  height: 100%; /* Full height */
  overflow: auto; /* Enable scroll if needed */
  background-color: rgb(0,0,0); /* Fallback color */
  background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
}

/* Modal Content */
.modal1-content1 {
  position: relative;
  background-color: #fefefe;
  margin: auto;
  padding: 0;
  border: 1px solid #888;
  width: 50%;
  box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2),0 6px 20px 0 rgba(0,0,0,0.19);
  -webkit-animation-name: animatetop;
  -webkit-animation-duration: 0.4s;
  animation-name: animatetop;
  animation-duration: 0.4s
}

/* Add Animation */
@-webkit-keyframes animatetop {
  from {top:-300px; opacity:0} 
  to {top:0; opacity:1}
}

@keyframes animatetop {
  from {top:-300px; opacity:0}
  to {top:0; opacity:1}
}

/* The Close Button */
.close1 {
  color: white;
  float: right;
  font-size: 28px;
  font-weight: bold;
}

.close1:hover,
.close1:focus {
  color: #000;
  text-decoration: none;
  cursor: pointer;
}
 .modal-dialog {
            width: 100%;
            margin: 30px auto;
        }
 .modalUpload
{
           position: fixed;
    z-index: 999;
    height: 100%;
    width: 100%;
    top: 0;
    /*background-color: Black;
    filter: alpha(opacity=60);
    opacity: 0.6;
    -moz-opacity: 0.8;*/
}
.centerUpload
{
  z-index: 1000;
    margin: 300px auto;
    padding: 10px;
    width: 130px;
    padding-right:500px;
    /*background-color: White;
    border-radius: 10px;
    filter: alpha(opacity=100);
    opacity: 1;
    -moz-opacity: 1;*/

}
.center img
{
    height: 120px;
    width: 120px;
}
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
    EnableScriptGlobalization="true">
</asp:ToolkitScriptManager>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
<ProgressTemplate>
    <div class="modalUpload">
        <div class="centerUpload">
             <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
        </div>
    </div>
</ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<div ID="pnlMainHolder" runat="server">
    <div ID="pnlRow_0" runat="server" CssClass="row">
        <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Etiquetación Extemporánea</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body" id="divContenedor">
                    <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form" >
                        <div class="row" runat="server" id="row1">
                            <div class="col-sm-12">
                               <div class="panel panel-default">
                                    <div class="panel-heading">
<%--                                       <div class="row" runat="server" id="DatosEmple">
                                         <div class="col-sm-8">
                                            <asp:Panel runat="server" ID="row" CssClass="form-group">
                                                <asp:Label runat="server" CssClass="col-sm-5 control-label">Con llamadas hechas en: </asp:Label>
                                                    <div class="col-sm-4">
                                                        <asp:DropDownList runat="server" ID="cboMes" DataValueField="vchCodigo" DataTextField="Descripcion" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-sm-3">
                                                    <asp:DropDownList runat="server" ID="cboAnio" CssClass="form-control" DataValueField="vchCodigo" DataTextField="Descripcion"></asp:DropDownList> 
                                                </div>
                                            </asp:Panel>
                                          </div>
                                        </div>--%>
                                       <div class="row">
                                            <div class="col-sm-8">
                                             <asp:Panel CssClass="form-group" runat="server">
                                                <asp:Label runat="server" CssClass="col-sm-6 control-label">Buscar por Nómina o Nombre:</asp:Label>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox runat="server" ID="txtBusqueda" CssClass="form-control" onfocus="javascript:$(this).autocomplete('search','');"></asp:TextBox>
                                                        <asp:HiddenField runat="server" id="txtEmpleId"/>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
<%--                                       <div class="row">
                                           <div class="col-sm-8">
                                               <asp:Label runat="server" CssClass="col-sm-4 control-label">Tipo de Reporte: </asp:Label>
                                               <label class="radio-inline">
                                                   <asp:RadioButton runat="server" ID="rbtResumen" GroupName="reportes" Checked="true"/>Resumen
                                                </label>
                                               <label class="radio-inline">
                                                   <asp:RadioButton runat="server" ID="rbtnDetall" GroupName="reportes"/> Detalle
                                                </label>
                                           </div>
                                       </div>--%>
                                       <div class="row">
                                           <br />
                                           <div class="col-sm-12">
                                               <div class ="col-sm-8">
                                                   <asp:Panel ID="rowBuscar" runat="server" CssClass="form-group">
                                                        <div class="col-sm-offset-6 col-sm-8">
                                                            <asp:Button runat="server" id="btnBuscar" CssClass="btn btn-keytia-lg" text="Buscar" OnClick="btnBuscar_Click"/>
                                                        </div>               
                                                    </asp:Panel>
                                               </div>
                                           </div>
                                       </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" runat="server" id="detalleEmpleados" visible="false">
                            <div class="col-sm-12">
                               <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <div class="row" runat="server" id="Div1">
                                        <div class="col-sm-6">
                                            <asp:Panel runat="server" ID="rowNomEmple" CssClass="form-group">
                                                <asp:Label runat="server" CssClass="col-sm-4 control-label">Nombre del Empleado: </asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtNomEmple" runat="server" CssClass="form-control" Enabled="false" ></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowDepartamento" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblDepartamento" CssClass="col-sm-4 control-label">Departamento: </asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtDepartamento" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowLocalidad" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblLocalidad" CssClass="col-sm-4 control-label">Localidad: </asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtLocalidad" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowCencos" CssClass="form-group" Visible="false">
                                                <asp:Label runat="server" ID="lblCencos" CssClass="col-sm-4 control-label">Centro de costos:</asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtCencos" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:Panel runat="server" ID="rowNumEmpleado" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblNumEmpleado" CssClass="col-sm-6 control-label">Número de Empleado: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox ID="txtNumEmpleado" runat="server" CssClass="form-control" Enabled="false" ></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:panel runat="server" ID="rowNumDepto" CssClass="form-group">
                                                <asp:Label ID="lblNumDepto" runat="server" CssClass="col-sm-6 control-label">Número de Departamento: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtNumDepto" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:panel>
                                            <asp:Panel runat="server" ID="rowNumLocali" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblNumLocalidad" CssClass="col-sm-6 control-label">Número de Localidad: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtNumLocali" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                                <asp:HiddenField runat="server" ID="hfLocaliId" />
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowIdCencos" CssClass="form-group" Visible="false">
                                                <asp:Label runat="server" ID="lblIdCencos" CssClass="col-sm-6 control-label">Número de Centro de Costos: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtNumCencos" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" runat="server" id="rowBotones" visible="false">
                            <div class ="col-sm-12">
                                <div class="panel panel-default">
                                    <div class="panel-body">
                                        <div class="col-sm-6">
                                            <div class="col-sm-offset-2">
                                                <asp:Label ID="lblFechaInicio" runat="server" CssClass="control-label">Periodo:&nbsp&nbsp&nbsp&nbsp</asp:Label>
                                                <cc1:DSODateTimeBox ID="pdtInicio" runat="server" Row="1" ShowHour="false" ShowMinute="false"
                                                    ShowSecond="false" DateFormat="dd/MM/yyyy" EnableViewState="true">
                                                </cc1:DSODateTimeBox>
                                            </div> 
                                        </div>
                                        <div class ="col-sm-6">
                                            <div class="col-sm-offset-2 col-sm-4">
                                              <asp:Button runat="server" id="btnGuardar" CssClass="btn btn-keytia-lg" text="Aceptar" OnClick="btnGuardar_Click"/>
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
    </div>
 </div>
<%--NZ: Modal para mensajes--%>
<asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display:none;">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
            </div>
            <div class="modal-body">
                <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
<asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
    TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
</asp:ModalPopupExtender>
</ContentTemplate>
<Triggers>
    <asp:PostBackTrigger ControlID="btnBuscar" />
</Triggers>
</asp:UpdatePanel>
<script type="text/javascript">
        window.onsubmit = function () {          
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            
        }
</script>
</asp:Content>

