<%@ Page Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="Detallado.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.Detallado" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script type="text/javascript">
        var pagePath = window.location.pathname;
        var dataJSON;

        $(function () {
            $("#" + "<%=txtNombre.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplateEmple",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Nomina + ' ' + item.Nombre, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#" + "<%=txtNombre.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtEmpleId.ClientID %>").val(ui.item.description);
                }
            });
        });

        $(function () {
            $("#" + "<%=txtCenCos.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplateCenCos",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Clave + ' ' + item.Descripcion, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#" + "<%=txtCenCos.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtCenCosId.ClientID %>").val(ui.item.description);
                }
            });
        });

        $(function () {
            $("#" + "<%=txtLocali.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/ConsultaAutoComplateLocali",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.Id };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#" + "<%=txtLocali.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtLocaliId.ClientID %>").val(ui.item.description);
                }
            });
        });

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <div class="clearfix"></div>

    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <button id="btnRegresar" runat="server" onserverclick="btnRegresar_Click" type="button" class="btn btn-default btn-circle btnBackDetail"><i class="far fa-arrow-alt-circle-left"></i></button>
                        <asp:Panel ID="pnlMapaNavegacion" runat="server">
                        </asp:Panel>
                    </div>
                    <div class="actions col-md-2 col-sm-2 col-lg-2 col-xs-2">
                        <p style="text-align: center;">
                            <img src="../../img/svg/Asset 22.svg" alt="">
                            Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server" OnClick="btnExportarXLS_Click" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                        </p>
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

    <asp:Panel ID="pnlMainHolder" runat="server">
        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
            <asp:Panel ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia">Búsquedas Especiales</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                            <form name="FR_Busquedas" method="post" class="form-horizontal" role="form">
                                <asp:Panel ID="rowTelefonia" runat="server" CssClass="form-group" Visible="false">
                                    <asp:Label ID="lblTelefonia" runat="server" CssClass="col-sm-2 control-label">Telefonía:</asp:Label>
                                    <div class="col-sm-10">
                                        <asp:RadioButton ID="rbtnFija" runat="server" Text="Fija" Checked="true" AutoPostBack="true" GroupName="gpoTelefonia"
                                            OnCheckedChanged="rbtnFija_OnCheckedChanged" CssClass="checkbox-inline"/>
                                        <asp:RadioButton ID="rbtnFijaEntrada" runat="server" Text="Llams. Entrada (Fija)" GroupName="gpoTelefonia" AutoPostBack="true"
                                            OnCheckedChanged="rbtnFija_OnCheckedChanged" CssClass="checkbox-inline" Visible="false" />
                                        <asp:RadioButton ID="rbtnFijaEnlace" runat="server" Text="Llams. Enlace (Fija)" GroupName="gpoTelefonia" AutoPostBack="true"
                                            OnCheckedChanged="rbtnFija_OnCheckedChanged" CssClass="checkbox-inline" Visible="false" />
                                        <asp:RadioButton ID="rbtnMovil" runat="server" Text="Móvil" GroupName="gpoTelefonia" AutoPostBack="true"
                                            OnCheckedChanged="rbtnFija_OnCheckedChanged" CssClass="checkbox-inline"/>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowUbicacion" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblUbicacion" runat="server" CssClass="col-sm-2 control-label">Ubicación:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="cboUbicacion" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowEmple" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblEmpleado" runat="server" CssClass="col-sm-2 control-label">Empleado:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtNombre" runat="server" CssClass="autosuggest placeholderstile form-control"
                                            onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Empleado" />
                                        <div style="display: none">
                                            <asp:TextBox ID="txtEmpleId" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowCenCos" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblCentroCostos" runat="server" CssClass="col-sm-2 control-label">Centro de costos:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtCenCos" runat="server" CssClass="autosuggest placeholderstile form-control"
                                            onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Centro de Costos" />
                                        <div style="display: none">
                                            <asp:TextBox ID="txtCenCosId" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowCatLlam" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblTipoLlamada" runat="server" CssClass="col-sm-2 control-label">Categoría Llamada:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="cboTipoLlamada" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowCarrier" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblCarrier" runat="server" CssClass="col-sm-2 control-label">Carrier:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="cboCarrier" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowTDest" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblTipoDestino" runat="server" CssClass="col-sm-2 control-label">Tipo Destino:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="cboTipoDestino" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowLocali" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblLocalidad" runat="server" CssClass="col-sm-2 control-label">Localidad:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtLocali" runat="server" CssClass="autosuggest placeholderstile form-control"
                                            onfocus="javascript:$(this).autocomplete('search','');" placeholder="Buscar Localidad" />
                                        <div style="display: none">
                                            <asp:TextBox ID="txtLocaliId" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowNumMarcado" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblNumMarcado" runat="server" CssClass="col-sm-2 control-label">Núm. Marcado:</asp:Label>
                                    <div class="col-sm-5">
                                        <asp:TextBox ID="txtNumMarcado" runat="server" MaxLength="32" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:CheckBox ID="banderaNumMarcado" runat="server" Text="Búsqueda exacta" CssClass="checkbox-inline" />
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowExten" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblExtension" runat="server" CssClass="col-sm-2 control-label">Extensión:</asp:Label>
                                    <div class="col-sm-5">
                                        <asp:TextBox ID="txtExtension" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:CheckBox ID="banderaExtensionExacta" runat="server" Text="Búsqueda exacta" CssClass="checkbox-inline" />
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowLinea" runat="server" CssClass="form-group" Visible="false">
                                    <asp:Label ID="lblLinea" runat="server" CssClass="col-sm-2 control-label">Linea:</asp:Label>
                                    <div class="col-sm-5">
                                        <asp:TextBox ID="txtLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:CheckBox ID="banderaLineaExacta" runat="server" Text="Búsqueda exacta" CssClass="checkbox-inline" />
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowCodAuto" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblCodigo" runat="server" CssClass="col-sm-2 control-label">Código:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtCodigo" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowCosto" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblCosto" runat="server" CssClass="col-sm-2 control-label">Costo:</asp:Label>
                                    <div class="col-sm-1">
                                        <asp:DropDownList ID="cboCriterioCosto" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="=">=</asp:ListItem>
                                            <asp:ListItem Value="<">&lt;</asp:ListItem>
                                            <asp:ListItem Value="<=">&lt;=</asp:ListItem>
                                            <asp:ListItem Value=">">&gt;</asp:ListItem>
                                            <asp:ListItem Value=">=">&gt;=</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-7">
                                        <asp:TextBox ID="txtCosto" runat="server" MaxLength="5" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="rowDuracion" runat="server" CssClass="form-group">
                                    <asp:Label ID="lblDuracion" runat="server" CssClass="col-sm-2 control-label">Duración:</asp:Label>
                                    <div class="col-sm-1">
                                        <asp:DropDownList ID="cboCriteriosDuracion" runat="server" CssClass="col-sm-2 form-control">
                                            <asp:ListItem Value="=">=</asp:ListItem>
                                            <asp:ListItem Value="<">&lt;</asp:ListItem>
                                            <asp:ListItem Value="<=">&lt;=</asp:ListItem>
                                            <asp:ListItem Value=">">&gt;</asp:ListItem>
                                            <asp:ListItem Value=">=">&gt;=</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-7">
                                        <asp:TextBox ID="txtDuracion" runat="server" MaxLength="4" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                            
                                <asp:Panel ID="rowdirLlamada" runat="server" CssClass="form-group"  Visible="false">
                                    <asp:Label ID="lbldirLlamada" runat="server" CssClass="col-sm-2 control-label">Dirección de Llamada:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ClientIDMode="Static" ID="ddlDirLLam" runat="server" CssClass="form-control" >
                                             <asp:ListItem  Value="0">--TODAS--</asp:ListItem>
                                            <asp:ListItem  Value="1">Entrada</asp:ListItem>
                                            <asp:ListItem Value="2">Salida</asp:ListItem>
                                            
                                        </asp:DropDownList>
                                    </div>
                                 
                                </asp:Panel>
                                <asp:Panel ID="rowOrganizacion" runat="server" CssClass="form-group" Visible="false">
                                    <asp:Label ID="lblOrganizacion" runat="server" CssClass="col-sm-2 control-label">Organización:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ClientIDMode="Static" ID="ddlOrganizacion" runat="server" CssClass="form-control" >
                                            <asp:ListItem  Value="0">--TODAS--</asp:ListItem>
                                            <asp:ListItem  Value="1">UVM</asp:ListItem>
                                            <asp:ListItem Value="2">UNITEC</asp:ListItem>
                                            
                                        </asp:DropDownList>
                                    </div>
                                 
                                </asp:Panel>
                                <asp:Panel ID="rowLlamadasFueraDeHorario" runat="server" CssClass="form-group" Visible="false">
                                    <asp:Label  runat="server" CssClass="col-sm-2 control-label">Sólo Llamadas Fuera de Horario:</asp:Label>
                                    <div class="col-sm-8">
                                        <asp:CheckBox ID="chkLlamadasFueraDeHorario" runat="server" Text="Sólo Llamadas Fuera de Horario" />
                                    </div>
                                 
                                </asp:Panel>
                                <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                    <div class="col-sm-offset-5 col-sm-10">
                                        <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="btn btn-keytia-lg" OnClick="btnAceptar_Click" />
                                    </div>
                                </asp:Panel>
                            </form>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlRow_9" runat="server" CssClass="row">
            <asp:Panel ID="Rep9" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <%--NZ: Modal para mensajes--%>
    <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" style="display: none;">
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
    <%--NZ: Modal para e-mail--%>
    <asp:Panel ID="pnlPopupMail" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:Label ID="lblTituloModalMail" runat="server" Text="Demasiados registros para mostrar"></asp:Label>
                </div>
                <div class="modal-body">
                    Por favor ingresa los siguientes datos para recibir el archivo:<br /><br />
                    <table border="0" style="width: 95%;">
                        <tr>
                            <td>Nombre de reporte: </td>
                            <td><asp:TextBox ID="txtNombreRep" runat="server" CssClass="form-control" MaxLength="50" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="txtNombreRepValidator" ControlToValidate="txtNombreRep" validationgroup="MailGroup" Text="El nombre de reporte no puede estar vacío" ErrorMessage="Falta nombre de reporte" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="El nombre no puede contener los siguientes caracteres: <br>\ / : * ? &quot; < > |" ControlToValidate="txtNombreRep"
                                Display="Dynamic" ForeColor="#FF3300" SetFocusOnError="True" ValidationExpression="^[^\\\./:\*\?\&quot;<>\|]{1}[^\\/:\*\?\&quot;<>\|]{0,254}$" validationgroup="MailGroup">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:CustomValidator ID="NombreRepValidator" ControlToValidate="txtNombreRep" validationgroup="MailGroup" Text="El nombre ya existe, favor de ingresar otro" ErrorMessage="Nombre de reporte ya existe"  runat="server" 
                                OnServerValidate="NombreRepValidator_ServerValidate" Display="Static" ForeColor="#FF3300" SetFocusOnError="True"></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>Correo electrónico: </td>
                            <td><asp:TextBox ID="txtMail" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="txtEmailValidator" ControlToValidate="txtMail" validationgroup="MailGroup" Text="El correo electrónico no puede estar vacío" ErrorMessage="Falta correo electrónico" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Correo electrónico invalido" ControlToValidate="txtMail"
                            Display="Dynamic" ForeColor="#FF3300" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" validationgroup="MailGroup">
                            </asp:RegularExpressionValidator></td>
                        </tr>
                    </table>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnOkMail" runat="server" Text="OK" validationgroup="MailGroup" CssClass="btn btn-keytia-sm" OnClick="btnOkMail_Click" />
                    <asp:Button ID="btnRegMail" runat="server" Text="Regresar" CssClass="btn btn-keytia-sm" OnClick="btnRegMail_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkBtnMail" runat="server" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeEtqMail" runat="server" PopupControlID="pnlPopupMail"
        TargetControlID="lnkBtnMail" BackgroundCssClass="modalPopupBackground">
    </asp:ModalPopupExtender>
</asp:Content>
