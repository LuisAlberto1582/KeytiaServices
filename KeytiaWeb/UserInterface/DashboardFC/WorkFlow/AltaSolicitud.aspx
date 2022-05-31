<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AltaSolicitud.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.AltaSolicitud" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;

        $(function () {
            $("#" + "<%=txtEmpleNom.ClientID %>").autocomplete({
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
                                return { label: item.Nomina + ' ' + item.Nombre, description: item.Nomina };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#" + "<%=txtEmpleNom.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtEmpleId.ClientID %>").val(ui.item.description);
                }
            });
        });

    </script>
    <style>
        .puesto {
            color: #58697D;
            font-size: 16px;
        }

        .tooltip1 {
            position: relative;
            display: inline-block;
        }

            .tooltip1 .tooltiptext1 {
                visibility: hidden;
                position: absolute;
                width: 250px;
                background-color: #555;
                color: #fff;
                text-align: left;
                padding: 6px 6px 6px 6px;
                border-radius: 6px;
                z-index: 1;
                opacity: 0;
                transition: opacity 0.3s;
            }

            .tooltip1:hover .tooltiptext1 {
                visibility: visible;
                opacity: 1;
            }

        .tooltip1-right {
            top: -10px;
            left: 126%;
        }

            .tooltip1-right::after {
                content: "";
                position: absolute;
                top: 50%;
                right: 100%;
                margin-top: -5px;
                border-width: 5px;
                border-style: solid;
                border-color: transparent #555 transparent transparent;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row" Visible="false">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12" runat="server" id="rowBusqueda">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <div class="col-sm-6">
                            <div class="form-horizontal" role="form" runat="server" id="formNomina">
                                <asp:Panel ID="Panel1" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblEmpleado" runat="server" CssClass="col-sm-4 control-label">Empleado:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtEmpleNom" runat="server" CssClass="autosuggest placeholderstile form-control"
                                            onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Empleado" />
                                        <div style="display: none">
                                            <asp:TextBox ID="txtEmpleId" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-horizontal">
                                <asp:Panel ID="rowBtnBuscar" runat="server" CssClass="form-group">
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" CssClass="btn btn-keytia-lg" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Solicitud (Recurso Voz y Datos)</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row" runat="server" id="row1">
                                        <div class="col-sm-6">
                                            <div class="form-horizontal" role="form">
                                                <asp:Panel ID="rowNomina" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblNomina" runat="server" CssClass="col-sm-4 control-label">No. Nomina: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtNomina" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="iCodCatEmple" />
                                                        <asp:HiddenField runat="server" ID="hfEmpleEspecial" />
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowDireccion" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblDireccion" runat="server" CssClass="col-sm-4 control-label">Dirección: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowEmpresa" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblEmpresa" runat="server" CssClass="col-sm-4 control-label">Empresa a Facturar: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtEmpresa" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hfSociedad" />
                                                        <asp:HiddenField runat="server" ID="hfDireccion" />
                                                        <asp:HiddenField runat="server" ID="hfCencos" />
                                                        <asp:HiddenField runat="server" ID="hfArea" />
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowTipoRecurso" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblTipoRecurso" runat="server" CssClass="col-sm-4 control-label">Tipo Recurso: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtTipoRecurso" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hfTipoRecurso" />
                                                        <%--<asp:DropDownList runat="server" ID="cboTipoRecurso" AppendDataBoundItems="true" DataValueField="id" DataTextField="Descripcion" CssClass="col-sm-2 form-control" AutoPostBack="true" OnSelectedIndexChanged="cboTipoRecurso_SelectedIndexChanged">
                                                        <asp:ListItem Value="0"> Seleciona un Tipo de Recurso </asp:ListItem>
                                                    </asp:DropDownList>--%>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowMonto" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblMonto" runat="server" CssClass="col-sm-6 control-label" Enabled="false">Monto Mensual Autorizado:</asp:Label>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox ID="txtMonto" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowEmail" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblEmail" runat="server" CssClass="col-sm-4 control-label">Email: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>

                                                <asp:Panel runat="server" CssClass="form-group" ID="rowUbicacion" Visible="false">
                                                   <asp:Label ID="lblUbicacion" runat="server" CssClass="col-sm-4 control-label">Ubicación</asp:Label>
                                                    <div class="col-sm-8">
                                                            <asp:DropDownList runat="server" ID="cboUbicacion" AppendDataBoundItems="true" DataValueField="id" DataTextField="Nombre"  CssClass="col-sm-2 form-control" AutoPostBack="true"  OnSelectedIndexChanged="cboUbicacion_SelectedIndexChanged" EnableViewState="true">                                                                                                           
                                                            </asp:DropDownList>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowCorreoDirector" runat="server" CssClass="form-group" Visible ="false">
                                                    <asp:Label ID="lblCorreoDirector" runat="server" CssClass="col-sm-4 control-label">Director: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtCorreoDirector" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-horizontal" role="form">
                                                <asp:Panel ID="rowNombre" runat="server" CssClass="form-group">
                                                    <asp:Label ID="Nombre" runat="server" CssClass="col-sm-4 control-label">Nombre: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowArea" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblArea" runat="server" CssClass="col-sm-4 control-label">Área: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowPuesto" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblPuesto" runat="server" CssClass="col-sm-4 control-label" Enabled="false">Puesto: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtPuesto" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowPerfil" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblPerfil" runat="server" CssClass="col-sm-4 control-label">Perfil:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <%--<asp:DropDownList runat="server" ID="cboPerfil" AppendDataBoundItems="true" DataValueField="id" DataTextField="Descripcion" OnSelectedIndexChanged="cboPerfil_SelectedIndexChanged" CssClass="col-sm-2 form-control" AutoPostBack="true">
                                                    <asp:ListItem Value="0"> Seleciona un Perfil </asp:ListItem>
                                                </asp:DropDownList>>--%>
                                                        <asp:TextBox runat="server" ID="txtPerfil" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hfPerfil" />
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowPlan" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblPlan" runat="server" CssClass="col-sm-4 control-label">Plan:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtPlan" runat="server" CssClass="form-control" Enabled="false">
                                                        </asp:TextBox>
                                                        <asp:DropDownList runat="server" ID="cboPlan" AppendDataBoundItems="true"
                                                            DataValueField="iCodCatalogo" DataTextField="RPLAN" CssClass="col-sm-2 form-control" OnSelectedIndexChanged="cboPlan_SelectedIndexChanged" Visible="false" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                        <asp:HiddenField ID="hfPlanTarif" runat="server" />
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowChk" runat="server" CssClass="form-group">
                                                    <asp:Label ID="Label2" runat="server" CssClass="col-sm-2 control-label"></asp:Label>
                                                    <div class="col-sm-10">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">
                                                                <asp:CheckBox runat="server" ID="chkPuesto" />
                                                            </span>
                                                            <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control puesto" Enabled="false" Text="Puesto Nueva Creación" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="rowCorreoAdminMoviles" runat="server" CssClass="form-group" Visible ="false">
                                                    <asp:Label ID="lblCorreoAdminMoviles" runat="server" CssClass="col-sm-4 control-label">Admin. de Móviles: </asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ID="txtCorreoAdminMoviles" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" runat="server" id="row2">
                                        <div class="col-sm-10">
                                            <div class="form-horizontal" role="form">
                                                <asp:Panel ID="rowObjetivo" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblObjetivo" runat="server" CssClass="col-sm-2 control-label" Font-Size="16px">Objetivo: <div class="tooltip1"><i class="fas fa-question-circle"></i><span class="tooltiptext1 tooltip1-right">Propósito de la asignación.<br /></span></div></asp:Label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" ID="objetivo"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="col-sm-10" runat="server" visible="true">
                                            <div class="form-horizontal" role="form">
                                                <asp:Panel ID="rowJustificacion" runat="server" CssClass="form-group">
                                                    <asp:Label ID="lblJustificacion" runat="server" CssClass="col-sm-2 control-label" Font-Size="16px">Justificación:<div class="tooltip1"><i class="fas fa-question-circle"></i></i><span class="tooltiptext1 tooltip1-right">Cuáles serán los beneficios de realizar esta asignación // En caso de ser reasignación colocar el numero de la línea anterior y el nombre del usuario anterior.<br /></span></div></asp:Label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox TextMode="multiline" class="form-control" Rows="5" ID="justificacion" runat="server"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="col-sm-12" runat="server" visible="true">
                                            <div class="form-horizontal" role="form">
                                                <asp:Label ID="lblDireccionEnvio" runat="server" CssClass="col-sm-6 control-label">
                                              Domicilio de la Sucursal o Edificio a donde se enviará el equipo:                  
                                                </asp:Label>
                                                <asp:Panel ID="rowDireccionEnvio" runat="server" CssClass="form-group">
                                                    <asp:Label ID="Label1" runat="server" CssClass="col-sm-4 control-label">
                                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    </asp:Label>
                                                    <div class="col-sm-offset-1 col-sm-10">
                                                        <asp:TextBox TextMode="MultiLine" CssClass="form-control" Rows="5" ID="txtDireccionEnvio" runat="server"></asp:TextBox>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                                <div class="col-sm-offset-6 col-sm-8">
                                                    <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" OnClick="btnAceptar_Click" CssClass="btn btn-keytia-lg" />
                                                </div>
                                            </asp:Panel>
                                        </div>

                                    </div>
                                    <!-- Alerta Success -->
                                    <div style="width: 800px; float: none; margin: 0 auto;">
                                        <asp:Panel ID="InfoPanelSucces" CssClass="alert alert-success text-center" runat="server" role="alert" Visible="false">
                                            <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <strong>
                                                <asp:Label ID="lblMensajeSuccess" runat="server"></asp:Label>
                                            </strong>
                                        </asp:Panel>
                                        <asp:Panel ID="rowNuevaSol" runat="server" CssClass="form-group" Visible="false">
                                            <div class="col-sm-offset-4 col-sm-6">
                                                <asp:Button ID="btnNuevaSolicitud" runat="server" Text="NuevaSolicitud" OnClick="btnNuevaSolicitud_Click" CssClass="btn btn-keytia-lg" />
                                            </div>
                                        </asp:Panel>
                                        <br />
                                    </div>
                                    <!-- Alerta Danger -->
                                    <div style="width: 800px; float: none; margin: 0 auto;">
                                        <asp:Panel ID="pnlError" CssClass="alert alert-danger text-center" runat="server" role="alert" Visible="false">
                                            <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <strong>
                                                <asp:Label ID="lblMensajeDanger" runat="server"></asp:Label>
                                            </strong>
                                        </asp:Panel>
                                        <br />
                                    </div>
                                    <!-- Alerta Info -->
                                    <div style="width: 800px; float: none; margin: 0 auto;">
                                        <asp:Panel ID="pnlInfo" CssClass="alert alert-info text-center" runat="server" role="alert" Visible="false">
                                            <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <strong>
                                                <asp:Label ID="lblMensajeInfo" runat="server"></asp:Label>
                                            </strong>
                                        </asp:Panel>
                                        <br />
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
                    <asp:Label runat="server" ID="lblTitulomensaje"></asp:Label>
                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrar"><i class="fas fa-times"></i></button>
                </div>
                <div class="modal-body">
                   <asp:Label ID="lblmensaje" runat="server" Text=""></asp:Label>
                </div>
                <div class="modal-footer">
                  <asp:Button ID="Button1" runat="server" Text="OK" CssClass="btn btn-keytia-sm" OnClick="btnYes_Click" />
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
    </asp:UpdatePanel>
</asp:Content>
