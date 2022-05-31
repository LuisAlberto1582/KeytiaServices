<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AppCCustodiaFCA.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.AppCCustodiaFCA" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--<style type="text/css">
        .lblDatosEmple, .lblDatosEmpleClaveFAC {
            background: #686CAC;
            color: white;
            font-size: 16px;
        }

        .txtDatosEmple {
            float: right;
        }


        .txtDatosEmpleClaveFAC, .txtTipoAlta {
            margin-left: 5px;
            margin-right: 5px;
        }

        .panel {
            margin-bottom: 15px;
        }


        #lblDatosEmpleTitle {
            font-size: 20px;
        }

        .title1 {
            font-size: 25px;
            color: #686CAC;
        }

        hr {
            display: block;
            height: 1px;
            border: 0;
            border-top: 1px solid #686CAC;
            margin: 1em 0;
            padding: 0;
        }

        .panel {
            padding-left: 20px;
            padding-right: 20px;
        }

        .panelDiv {
            margin-top: 10px;
            margin-bottom: 10px;
        }

        .txtDatosEmpleCenCos {
            margin-left: 5px;
            margin-top: 5px;
        }

        .panelDiv {
        }
    </style>--%>
    <style>
        .lblDatosEmple, .lblDatosEmpleClaveFAC {
            /*background: #686CAC;
            color: white;*/
            font-size: 13.5px;
            font-weight: bold;
        }

        .btn-primary:hover {
            background: #337ab7;
            border-color: #2e6da4;
            color: #fff;
        }

        .badge {
            font-size: 14px !important;
        }

        .tableFooter {
            border: 0px;
            text-align: right;
        }

        .tableFooteer1 {
            border: 0px;
            text-align: left;
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
            height: 20px; /*34*/
            bottom: -4px;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider, .slider2 {
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

            .slider:before, .slider2:before {
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

        input:checked + .slider, input:checked + .slider2 {
            /*background-color: #009639;*/
            background-color: #2196F3;
            right: -2px;
        }

        input:focus + .slider, input:focus + .slider2 {
            /*box-shadow: 0 0 1px #009639;*/
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before, input:checked + .slider2:before {
            -webkit-transform: translateX(26px);
            -ms-transform: translateX(26px);
            transform: translateX(26px);
        }
        /* Rounded sliders */
        .slider.round, .slider2.round {
            border-radius: 34px;
        }

            .slider.round:before, .slider2.round:before {
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
            animation-duration: 0.4s;
        }

        /* Add Animation */
        @-webkit-keyframes animatetop {
            from {
                top: -300px;
                opacity: 0;
            }

            to {
                top: 0;
                opacity: 1;
            }
        }

        @keyframes animatetop {
            from {
                top: -300px;
                opacity: 0;
            }

            to {
                top: 0;
                opacity: 1;
            }
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

        .modalUpload {
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

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            padding-right: 500px;
            /*background-color: White;
    border-radius: 10px;
    filter: alpha(opacity=100);
    opacity: 1;
    -moz-opacity: 1;*/
        }

        .center img {
            height: 120px;
            width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
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
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
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
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row" id="row1">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h4><strong>Configuración de Extensiones</strong></h4>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="col-sm-12">
                                                        <asp:Label runat="server" Style="font-weight: bold; color: red; font-size: 14px;" ID="lnlDatosEmpleNota" Text="">*Nota: En caso de seleccionar "Externo" el número de nómina no será necesario ya que se generará de manera aleatoria. </asp:Label>
                                                        <br />
                                                        <br />
                                                        <div class="row">
                                                            <div class="col-sm-6">
                                                                <asp:Panel runat="server" ID="rowTipoBusqueda" CssClass="form-group">
                                                                    <asp:Label runat="server" ID="lblDatosEmpleTipoEmple1" CssClass="lblDatosEmple col-sm-4 control-label" Text="Tipo alta"></asp:Label>
                                                                    <div class="col-sm-8">
                                                                        <asp:DropDownList runat="server" ID="ddlTipoEmple" AutoPostBack="true" AppendDataBoundItems="true" CssClass="form-control" OnSelectedIndexChanged="ddlTipoEmple_SelectedIndexChanged" Style="text-transform: uppercase">
                                                                            <asp:ListItem Text="--Seleccionar--" Value="0"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="row2">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-body">
                                                    <div class="col-sm-6">
                                                        <div class="form-group" runat="server" id="rowNomina">
                                                            <asp:Label runat="server" ID="lblDatosEmpleNomina" CssClass="lblDatosEmple col-sm-4 control-label" Text="No. Nómina "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleNomina" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowDCID">
                                                            <asp:Label runat="server" ID="lblDatosEmpleDc_id" CssClass="lblDatosEmple col-sm-4 control-label" Text="DC-ID "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleDc_id" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowNombre">
                                                            <asp:Label runat="server" ID="lblDatosEmpleNombre" CssClass="lblDatosEmple col-sm-4 control-label " Text="Nombre* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleNombre" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowAP">
                                                            <asp:Label runat="server" ID="lblDatosEmpleApPaterno" CssClass="lblDatosEmple col-sm-4 control-label" Text="Ap. paterno* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtxDatosEmpleApPaterno" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowAM">
                                                            <asp:Label runat="server" ID="lblDatosEmpleApMaterno" CssClass="lblDatosEmple col-sm-4 control-label" Text="Ap materno* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleApMaterno" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowNickName">
                                                            <asp:Label runat="server" ID="lblDatosEmpleNickName" CssClass="lblDatosEmple col-sm-4 control-label" Text="Nick-Name "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleNickName" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <%--                                            <div class="form-group" runat="server" id="divLineaDirecta">
                                                <asp:Label runat="server" ID="lblDatosEmpleNumLineaDirecta" CssClass="lblDatosEmple col-sm-4 control-label" Text="Núm. Línea Directa* "></asp:Label>
                                                <div class="col-sm-8">                                             
                                                    <asp:TextBox runat="server" ID="txtDatosEmpleNumLineaDirecta" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                </div>
                                            </div>--%>
                                                        <div class="form-group" runat="server" id="rowEstacion">
                                                            <asp:Label runat="server" ID="lblDatosEmpleEstacion" CssClass="lblDatosEmple col-sm-4 control-label" Text="Estación "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleEstacion" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                         <div class="form-group">
                                                            <asp:Label ID="lblEmailEmple" runat="server" CssClass="lblDatosEmple col-sm-4 control-label">E-mail:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox ID="txtEmailEmple" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblDatosEmpleJefeDirecto" CssClass="lblDatosEmple col-sm-4 control-label" Text="Jefe Directo"></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" ID="ddlDatosEmpleJefeDirecto" CssClass="txtDatosEmpleCenCos form-controClaveFAC form-control" AppendDataBoundItems="true">
                                                                    <asp:ListItem Text="--Selecciona--" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblDatosEmpleJefeDepartamento" CssClass="lblDatosEmple col-sm-4 control-label" Text="Departamento* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" ID="ddlDatosEmpleDepartamento" CssClass="txtDatosEmpleCenCos form-controClaveFAC form-control" AppendDataBoundItems="true">
                                                                    <asp:ListItem Text="--Selecciona--" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="divCenCos" visible=" false">
                                                            <asp:Label runat="server" ID="lblDatosEmpleJefeCentroCostos" CssClass="lblDatosEmple col-sm-4 control-label" Text="Centro de Costos* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" ID="ddlDatosEmpleCentroCostos" CssClass="txtDatosEmpleCenCos form-controClaveFAC form-control" AppendDataBoundItems="true">
                                                                    <asp:ListItem Text="--Selecciona--" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-6">
                                                        <div class="form-group" runat="server" id="rowFechaIni">
                                                            <asp:Label runat="server" ID="lblDatoEmpleFechaInicio" CssClass="lblDatosEmple col-sm-4 control-label" Text="Fecha de Inicio (dd/mm/yyyy)* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleFechaInicio" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowPlanta">
                                                            <asp:Label runat="server" ID="lblDatosEmplePlanta" CssClass="lblDatosEmple col-sm-4 control-label" Text="Planta "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" ID="ddlDatosEmplePlanta" AppendDataBoundItems="true" CssClass="txtDatosEmple form-control" Enabled="false">
                                                                    <asp:ListItem Text="--Seleccionar--" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowTID">
                                                            <asp:Label runat="server" ID="lblDatosEmpleT_ID" CssClass="lblDatosEmple col-sm-4 control-label" Text="T-ID* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleT_ID" CssClass="txtDatosEmple form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowDirector">
                                                            <asp:Label runat="server" ID="lblDatosEmpleDirector" CssClass="lblDatosEmple col-sm-4 control-label" Text="Director "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" ID="ddlDatosEmpleDirector" AppendDataBoundItems="true" CssClass="txtDatosEmple form-control" Enabled="false">
                                                                    <asp:ListItem Text="--Seleccionar--" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowExtension">
                                                            <asp:Label runat="server" ID="lblDatosEmpleExtension" CssClass="lblDatosEmple col-sm-4 control-label" Text="Extensión "></asp:Label>
                                                            <div class="col-sm-3">
                                                                <asp:TextBox runat="server" ID="txtDatosEmpleExtension" CssClass="txtDatosEmpleClaveFAC form-control" Enabled="false"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group">
                                                                <div class="col-sm-5">
                                                                    <asp:Label runat="server" ID="lblDatosEmpleExtensionPendiente" CssClass="lblDatosEmpleClaveFAC col-sm-6 control-label" Text="Sin ext. "></asp:Label>
                                                                    <label class="switch">
                                                                        <asp:CheckBox runat="server" ID="cbSinExt" Enabled="false"></asp:CheckBox>
                                                                        <span class="slider" runat="server" id="slider2"></span>
                                                                    </label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="form-group" runat="server" id="rowSitio">
                                                            <asp:Label runat="server" ID="lblDatosEmpleSitio" CssClass="lblDatosEmple col-sm-4 control-label" Text="Sitio* "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:DropDownList runat="server" ID="ddlDatosEmpleSitio" AppendDataBoundItems="true" CssClass="txtDatosEmple form-control" Enabled="false">
                                                                    <asp:ListItem Text="--Seleccionar--" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label ID="lblGeneraClaveFac" runat="server" CssClass="lblDatosEmple col-sm-4 control-label" Text="¿Generar ClaveFac?"></asp:Label>
                                                            <div class="col-sm-8">
                                                                <label class="switch">
                                                                    <asp:CheckBox runat="server" ID="chkMostrar" Enabled="false" AutoPostBack="true" OnCheckedChanged="chkMostrar_CheckedChanged" />
                                                                    <span class="slider" runat="server" id="slider"></span>
                                                                </label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblDatosEmpleClaveFAC" CssClass="lblDatosEmple col-sm-3 control-label" Text="Cve. FAC "></asp:Label>
                                                                <div class="col-sm-3">
                                                                    <asp:TextBox runat="server" ID="txtDatosEmpleClaveFAC" CssClass="txtDatosEmpleClaveFAC form-control" Enabled="false"></asp:TextBox>
                                                                </div>
                                                                <asp:Label runat="server" ID="lblDatosEmpleCobertura" CssClass="lblDatosEmpleClaveFAC col-sm-2 control-label" Text="Cobertura"></asp:Label>
                                                                <div class="col-sm-4">
                                                                    <asp:DropDownList runat="server" ID="ddlDatosEmpleCobertura" CssClass="txtDatosEmpleClaveFAC form-control" Enabled="false" AppendDataBoundItems="true">
                                                                        <asp:ListItem Text="--Selecciona--" Value="0"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblDatosEmpleEsDirector" CssClass="lblDatosEmple col-sm-4 control-label" Text="¿Es director? "></asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:RadioButton runat="server" ID="rdDatosEmpleesDirectorSI" Text="Sí" Checked="false" GroupName="gpEsDirector" CssClass="txtDatosEmpleClaveFAC" Enabled="false" />
                                                                <asp:RadioButton runat="server" ID="rdDatosEmpleesDirectorNO" Text="No" Checked="true" GroupName="gpEsDirector" CssClass="txtDatosEmplelaveFAC" Enabled="false" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-footer">
                                                    <div class="form-group">
                                                        <br />
                                                        <div class="col-sm-offset-3 col-sm-8">
                                                            <asp:Button runat="server" ID="btnGrabar" CssClass="btn btn-keytia-lg" Text="Grabar" Enabled="false" OnClick="btnGrabar_Click" />
                                                            <asp:Button runat="server" ID="btnCancelar" CssClass="btn btn-keytia-lg" Text="Cancelar" Enabled="false" OnClick="btnCancelar_Click" />
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
            <asp:PostBackTrigger ControlID="btnGrabar" />
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
