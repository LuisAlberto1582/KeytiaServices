<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="EtiqNumsPorExten.aspx.cs" Inherits="KeytiaWeb.UserInterface.EtiquetaNums.EtiqNumsPorExten" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
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
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server"></span>
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
                                        <div class="row" runat="server" id="DatosEmple">
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
                        <div class="row" runat="server" id="row2" visible="false">
                            <div class="col-sm-12">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <p id="parrafo1" runat="server" style="text-align:center;font-weight:bold;"></p>
                                    </div>
                                    <div class="panel-body">                                        
                                        <div id="DivllamLocales" runat="server">
                                            <asp:GridView runat="server" ID="grdLlamLocales" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                                 DataKeyNames="NumMarcado,Extension,Localidad,TablaConsulta">
                                                <Columns>
                                                    <asp:BoundField DataField= "Extension" HeaderText="Ext." HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "NumMarcado" HeaderText="Número Marcado" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Cantidad" HeaderText="Cantidad" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Localidad" HeaderText="Referencia" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "LocalidadKeytia" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Costo" HeaderText="Importe" DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:TemplateField HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small">
                                                        <ItemTemplate>
                                                            <asp:LinkButton CssClass="btn btn-primary btn-xs"  ID="lnkVerDetall" runat="server" Text="Ver detalle" OnClick="lnkVerDetall_Click"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>                                               
                                            </asp:GridView>
                                        </div>
                                        <div>
                                            <asp:GridView runat="server" ID="grdTotales" AutoGenerateColumns="false" ShowHeader="false" CssClass="table table-bordered lastRowTable tableFooter">
                                                <Columns>
                                                    <asp:BoundField DataField = "Llamadas"/>
                                                    <asp:BoundField DataField = "Minutos"/>
                                                    <asp:BoundField DataField = "CostoTotal"/>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div class="panel-footer" runat="server" id="footer">
                                 
                                        <p style="font-weight:bold;">Las llamadas locales por el momento no se cobrarán.</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" id="row3" runat="server" visible="false">
                            <div class="col-sm-12">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <p id="parrafo2" runat="server" style="text-align:center;font-weight:bold;"></p>
                                    </div>
                                    <div class="panel-body">
                                        <div class="col-sm-12">
                                            <div class="row" runat="server" id="rowTexto">
                                                <p style="font-size: 14px;"><span style="font-weight: bold; color: red;">Las llamadas de negocio deberán ser seleccionadas con un click </span> <i class="fas fa-check-square"></i></p>
                                            </div>                                            
                                            <div class="row">
                                                <asp:Panel ID="panel2" runat="server" CssClass="form-group">
                                                    <asp:Label ID="Label2" runat="server" CssClass="col-sm-2 control-label" Text="Aceptar Todas" Font-Bold="true"></asp:Label>
                                                    <div class="col-sm-4">                                                
                                                        <label class="switch">
                                                            <asp:CheckBox runat="server" ID="chkMostrar" OnCheckedChanged="chkMostrar_CheckedChanged" AutoPostBack="true"/>
                                                            <span class="slider" runat="server" id="slider"></span>
                                                        </label>                                                    
                                                   </div>                                            
                                                </asp:Panel> 
                                            </div>
                                            <div class="row">
                                                <div  runat="server" id="LlamadasDiv">
                                                    <asp:GridView runat="server" ID="grvLlamadas" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                                        DataKeyNames="Idf,NumMarcado,TipoEtiqueta,Costo,Extension,TablaConsulta,Localidad">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Negocio" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small">
                                                                <ItemTemplate>                                                                       
                                                                            <asp:CheckBox runat="server" ID="chkRow" OnCheckedChanged="chkRow_CheckedChanged" AutoPostBack="true"/>
                                                                                                                                 
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField= "Extension"  HeaderText="Ext." HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "NumMarcado" HeaderText="Número" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "Cantidad" HeaderText="Cantidad" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:TemplateField HeaderText="Referencia" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtRferencia" Text='<%# Eval("Localidad") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField= "LocalidadKeytia" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "Costo" HeaderText="Importe"  DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:TemplateField >
                                                                <ItemTemplate>
                                                                    <asp:LinkButton CssClass="btn btn-primary btn-xs"  ID="lnkVerDetalle" runat="server" Text="Ver detalle" OnClick="lnkVerDetalle_Click"></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <div runat="server" id="divgrdLlamCobrarTot">
                                                <asp:GridView runat="server" ID="grdLlamCobrarTot" AutoGenerateColumns="false" ShowHeader="false" CssClass="table table-bordered lastRowTable tableFooter">
                                                    <Columns>
                                                        <asp:BoundField DataField = "Llamadas"/>
                                                        <asp:BoundField DataField = "Minutos"/>
                                                        <asp:BoundField DataField = "CostoTotal"/>
                                                    </Columns>
                                                </asp:GridView>
                                                </div>
                                            </div>
                                        </div>                                       
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" runat="server" id="row4" visible="false">
                            <div class="col-sm-12">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <p runat="server" style="text-align:center;font-weight:bold;" id="parrafo3"></p>
                                    </div>
                                    <div class="panel-body">
                                        <div class="table-fixed-nz">
                                                    <asp:GridView runat="server" ID="grdMovil" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                                        DataKeyNames="Idf,NumMarcado,Etiqueta,Costo,Extension,Localidad">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Negocio" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small">
                                                                <ItemTemplate>                                                                       
                                                                            <asp:CheckBox runat="server" ID="chkRow" OnCheckedChanged="chkRow_CheckedChanged1" AutoPostBack="true"/>                                                                                                                                                    
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField= "Extension"  HeaderText="Ext." HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "NumMarcado" HeaderText="Número" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "Numero" HeaderText="Cantidad" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:TemplateField HeaderText="Referencia" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtRferencia" Text='<%# Eval("Localidad") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField= "LocalidadKeytia" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:BoundField DataField= "Costo" HeaderText="Importe"  DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                            <asp:TemplateField >
                                                                <ItemTemplate>
                                                                    <asp:LinkButton CssClass="btn btn-primary btn-xs"  ID="lnkVerDetalle" runat="server" Text="Ver detalle" OnClick="lnkVerDetalle_Click1"></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>

                                       <div runat="server" id="totMoviles">
                                                <asp:GridView runat="server" ID="grvTotalMovil" AutoGenerateColumns="false" ShowHeader="false" CssClass="table table-bordered lastRowTable tableFooter">
                                                    <Columns>
                                                        <asp:BoundField DataField = "Llamadas"/>
                                                        <asp:BoundField DataField = "Minutos"/>
                                                        <asp:BoundField DataField = "CostoTotal"/>
                                                    </Columns>
                                                </asp:GridView>
                                        </div>
                                     </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" id="row5" runat="server">
                            <div class="col-sm-12">
                                <div class="panel panel-default">
                                    <div class="panel-footer">
                                        <div class="row">
                                            <div class="col-sm-4">
                                                <div class="form-horizontal">
                                                    <asp:Panel runat="server" CssClass="form-group">                                                      
                                                        <div class="col-sm-8">
                                                            <asp:Label runat="server" ID="lblTotGlobalLlam" class="btn btn-primary">Total de llamadas Cel/LD: <span runat="server" id="spnCelLd" class="badge"></span></asp:Label>
                                                        </div>
                                                    </asp:Panel>
                                                    <asp:Panel runat="server" CssClass="form-group">                                                        
                                                        <div class="col-sm-8">
                                                            <asp:Label runat="server" ID="lblTiempoTotGlobal" class="btn btn-primary">Tiempo Total: <span runat="server" id="spnTiempoTot" class="badge"></span></asp:Label>
                                                        </div>                                                                                                                
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="col-sm-2">

                                            </div>
                                            <div class="col-sm-4">
                                                <div class="form-horizontal">
                                                    <asp:Panel runat="server" CssClass="form-group">
                                                         <asp:label runat="server" ID="lblPaquete" CssClass="col-sm-2 control-label">&nbsp;</asp:label>                                                     
                                                        <div class="col-sm-4">
                                                            <asp:Label runat="server" class="btn btn-danger">Importe de Llamadas Personales Aceptadas: $ <span runat="server" id="spnTotalLlam" class="badge">0.00</span></asp:Label>                                                
                                                        </div>
                                                    </asp:Panel>
                                                    <asp:Panel runat="server" CssClass="form-group">
                                                        <asp:label runat="server" ID="Label1" CssClass="col-sm-2 control-label">&nbsp;</asp:label>                                                      
                                                        <div class="col-sm-4">
                                                            <asp:Label runat="server" class="btn btn-primary">Importe Negocio: $ <span runat="server" id="SpanTotalNeg" class="badge"></span></asp:Label>                                                
                                                        </div>
                                                    </asp:Panel>                                                    
                                                    <asp:Panel runat="server" CssClass="form-group">
                                                        <asp:label runat="server" ID="Label3" CssClass="col-sm-2 control-label">&nbsp;</asp:label>                                                   
                                                        <div class="col-sm-4">
                                                            <asp:Label runat="server" class="btn btn-primary">Importe Total: $ <span runat="server" id="SpanTotal" class="badge"></span></asp:Label>
                                                        </div>
                                                    </asp:Panel>                                                    
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" id="row6" runat="server">
                            <div class="col-sm-12">
                                <div class="panel panel-default">
                                    <div class="panel-body">
                                        <div class="row">
                                            <div class="col-sm-12"> 
                                               <div class="form-inline">                                      
                                                <div class="form-group">
                                                    <div class="col-sm-offset-4 col-sm-6">
                                                        <asp:Button ID="btnDetalle" runat="server" Text="Ver Detalle" CssClass="btn btn-keytia-lg" OnClick="btnDetalle_Click"/>
                                                    </div>                                                    
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-sm-offset-4 col-sm-6">
                                                        <asp:Button ID="btnGuardar" runat="server" Text="Aceptar Importes personales" CssClass="btn btn-keytia-lg" OnClick="btnGuardar_Click"/>
                                                    </div>                                                     
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-sm-offset-10 col-sm-6">
                                                        <asp:Button ID="btnImprimir" runat="server" Text="Exportar" CssClass="btn btn-keytia-lg" OnClick="btnImprimir_Click" />
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
        </div>
    </div>
</div>

    <%--NZ: Modal para Editar el Hallazgo.--%>
    <asp:Panel id="pnlPopupAsignaLinea" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
        <div class="rule"></div>
        <div class="modal-dialog modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label runat="server" ID="lblTituloAsgnaLinea"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                    <div id="RepCapturaCollapse" class="form-horizontal" role="form">
                      <div class="row" runat="server" id="Div1">
                            <div class="col-sm-12">
                               <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <div class="row">
                                        <div class="col-sm-6">
                                            <asp:Panel runat="server" ID="rowNomEmpleado" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblNomEmpleado" CssClass="col-sm-4 control-label">Nombre del Empleado: </asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtNomEmpleado" runat="server" CssClass="form-control" Enabled="true" ></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowDep" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblDep" CssClass="col-sm-4 control-label">Departamento: </asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtDep" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowLocali" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblLocali" CssClass="col-sm-4 control-label">Localidad: </asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtLocali" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowCencosto" CssClass="form-group" Visible="false">
                                                <asp:Label runat="server" ID="lblCenCosto" CssClass="col-sm-4 control-label">Centro de costos:</asp:Label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtCencosto" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:Panel runat="server" ID="rowClaveEmple" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblClaveEmple" CssClass="col-sm-6 control-label">Número de Empleado: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox ID="txtClaveEmple" runat="server" CssClass="form-control" Enabled="false" ></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                            <asp:panel runat="server" ID="rowClaveDepto" CssClass="form-group">
                                                <asp:Label ID="lblClaveDepto" runat="server" CssClass="col-sm-6 control-label">Número de Departamento: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtClaveDepto" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:panel>
                                            <asp:Panel runat="server" ID="rowIdLocalidad" CssClass="form-group">
                                                <asp:Label runat="server" ID="lblIdLocalidad" CssClass="col-sm-6 control-label">Número de Localidad: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtLocalidadId" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                                <asp:HiddenField runat="server" ID="HiddenField1" />
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="rowNumCencosto" CssClass="form-group" Visible="false">
                                                <asp:Label runat="server" ID="lblNumCencosto" CssClass="col-sm-6 control-label">Número de Centro de Costos: </asp:Label>
                                                <div class="col-sm-6">
                                                    <asp:TextBox runat="server" ID="txtNumCencosto" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                      <div class="row">
                          <div class="col-sm-12">
                              <div class="panel panel-default">
                                  <div class="panel-heading">
                                      <h4><span runat="server" id="p1"></span></h4>
                                      <h4><span runat="server" id="p2"></span></h4>
                                  </div>
                                  <div class="panel-body">
                                      <!--Este titulo solo se muestra para el detalle de llamadas a moviles-->
                                      <h4><span runat="server" id="p3" visible="false"><strong>Llamadas de Entrada</strong> (Son llamadas recibidas estando en territorio Nacional e Internacional)</span></h4>                                     
                                     <div class="table-fixed-nz" runat="server" id="llamDetall">
                                      <asp:GridView runat="server" ID="grdDetalle" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard">
                                          <Columns>
                                             <asp:BoundField DataField= "Extension" HeaderText="Ext" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Fecha" HeaderText="Fecha(dd/mm/aaaa)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Hora" HeaderText="Hora" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "NumMarcado" HeaderText="Número Marcado" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField="Referencia" HeaderText="Referencia" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Localidad" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Costo" HeaderText="Importe" DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                          </Columns>            
                                      </asp:GridView>
                                    </div>
                                     <div runat="server" id="divViewTotal">
                                         <asp:GridView runat="server" ID="grdViewTotal" AutoGenerateColumns="false" ShowHeader="false" CssClass="table table-bordered lastRowTable tableFooter">
                                            <Columns>
                                              <asp:BoundField DataField = "Llamadas"/>
                                              <asp:BoundField DataField = "Minutos"/>
                                              <asp:BoundField DataField = "CostoTotal"/>
                                            </Columns>
                                         </asp:GridView>
                                     </div>
                                      <div runat="server" id="DetalleTelenet">
                                        <asp:GridView runat="server" ID="grdTelenet" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard">
                                          <Columns>
                                             <asp:BoundField DataField= "Extension" HeaderText="Ext" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>                                             
                                             <asp:BoundField DataField= "NumMarcado" HeaderText="Número Marcado" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField="Referencia" HeaderText="Referencia" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Localidad" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Costo" HeaderText="Importe" DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                          </Columns>            
                                        </asp:GridView>
                                      </div>
                                      <div id="totalTelenet">
                                          <asp:GridView runat="server" ID="grdTotalTelenet" AutoGenerateColumns="false" ShowHeader="false" CssClass="table table-bordered lastRowTable tableFooter">
                                            <Columns>
                                              <asp:BoundField DataField = "Llamadas"/>
                                              <asp:BoundField DataField = "Minutos"/>
                                              <asp:BoundField DataField = "CostoTotal"/>
                                            </Columns>
                                         </asp:GridView>
                                      </div>
                                      <div runat="server" id="DetalleGlobalMoviles">
                                       <asp:GridView runat="server" ID="grdGlobalDetallMoviles" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard">
                                          <Columns>
                                             <asp:BoundField DataField= "Extension" HeaderText="Ext" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>                                             
                                             <asp:BoundField DataField= "NumMarcado" HeaderText="Número Marcado" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField="Localidad" HeaderText="Referencia" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "LocalidadKeytia" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                             <asp:BoundField DataField= "Costo" HeaderText="Importe" DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                          </Columns>            
                                        </asp:GridView>
                                      </div>
                                      <!--Este titulo solo se muestra para el detalle de llamadas a moviles-->
                                     <h4><span runat="server" id="p4" visible="false"><strong>Llamadas de Salida</strong> (Son llamadas realizadas estando en territorio Nacional e Internacional)</span></h4>                                  
                                      <div class="table-fixed-nz" runat="server" id="divCel">
                                        <asp:GridView runat="server" ID="grdLlamCelSal" AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard">
                                          <Columns>
                                                    <asp:BoundField DataField= "Extension" HeaderText="Ext" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Fecha" HeaderText="Fecha(dd/mm/aaaa)" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Hora" HeaderText="Hora" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "NumMarcado" HeaderText="Número" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Duracion" HeaderText="Duración (minutos)" />                                                    
                                                    <asp:BoundField DataField= "Localidad" HeaderText="Lugar" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                    <asp:BoundField DataField= "Costo" HeaderText="Importe" DataFormatString="{0:c}" HeaderStyle-Font-Size="Small" ItemStyle-Font-Size="Small"/>
                                                </Columns>            
                                      </asp:GridView>
                                      </div>
                                      <div runat="server" id="divTotal2">
                                         <asp:GridView runat="server" ID="gridTotal2" AutoGenerateColumns="false" ShowHeader="false" CssClass="table table-bordered lastRowTable tableFooter">
                                            <Columns>
                                              <asp:BoundField DataField = "Llamadas"/>
                                              <asp:BoundField DataField = "Minutos"/>
                                              <asp:BoundField DataField = "CostoTotal"/>
                                            </Columns>
                                         </asp:GridView>
                                      </div>
                                      <h5><span runat="server" id="p5" visible="false"><strong>Informacíon mostrada por Telcel tal y como se muestra aqui</strong></span></h5>
                                  </div>
                              </div>
                          </div>
                      </div>
                    </div>
                </div>
                <div class="modal-footer">
                   <asp:Button ID="btnCancelarModal" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm"/>
                    <asp:Button ID="btnImprimirModal" runat="server" Text="Exportar" CssClass="btn btn-keytia-sm" Visible="false" OnClick="btnImprimirModal_Click"/>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnEditHallazo" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEditHallazo" runat="server" PopupControlID="pnlPopupAsignaLinea"
        TargetControlID="lnkBtnEditHallazo" CancelControlID="btnCerrar" BackgroundCssClass="modalPopupBackground" DropShadow="false">
    </asp:ModalPopupExtender>
    <%----%>
    <%--NZ: Modal para mensajes--%>
            <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
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
    <asp:PostBackTrigger ControlID="btnGuardar" />
</Triggers>
   <Triggers>
<asp:PostBackTrigger ControlID="btnImprimirModal"/>
</Triggers>
       <Triggers>
<asp:PostBackTrigger ControlID="btnImprimir"/>
</Triggers>
</asp:UpdatePanel>

<%--<script type="text/javascript">
        window.onsubmit = function () {          
                var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
                window.setTimeout(function () {
                    updateProgress.set_visible(true);
                }, 100);
            
        }
</script>--%>
</asp:Content>
