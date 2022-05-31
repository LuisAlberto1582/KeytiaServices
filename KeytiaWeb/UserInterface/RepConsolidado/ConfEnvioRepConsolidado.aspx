<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="ConfEnvioRepConsolidado.aspx.cs" Inherits="KeytiaWeb.UserInterface.RepConsolidado.ConfEnvioRepConsolidado" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            padding-right: 500px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }

        .txtDate1 {
            float: left;
            margin-top: -10px;
            height: 45px;
        }

        .applyDate1 {
            background-color: #F58426;
            border: 1px solid #F58426;
            font-family: "Poppins",sans-serif;
            font-weight: 100;
            width: 60px;
            height: 45px;
            margin-top: 10px;
            color: white;
            font-size: 14px;
        }

        .control-label1 {
            text-align: right;
            margin-bottom: 0;
            padding-top: 4px;
            padding-left: 9px;
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
    </style>
    <script type="text/javascript">var $jQuery2_3 = $.noConflict(true);</script>
    <script type="text/javascript" src="../RepPentafon/JSPicker/jquery-1.12.4.js"></script>
    <script type="text/javascript" src="../RepPentafon/JSPicker/jquery-ui.js"></script>
    <script type="text/javascript" src="../RepPentafon/JSPicker/espanol.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            var dateFormat = "dd/mm/yy",
                from = $("#from")
                    .datepicker({
                        changeMonth: true,
                        changeYear: true,
                        numberOfMonths: 1,
                        altField: "#hfFechaInicio",
                        altFormat: "yy-mm-dd 00:00:00",
                        yearRange: "-1y:+1y"
                    })
                    .on("change", function () {
                        to.datepicker("option", "minDate", getDate(this));
                        $("#from").datepicker("option", "dateFormat", 'DD, d MM, yy');
                        var FecI = document.getElementById('hfFechaInicio').value;
                        document.getElementById("<%=inicioFecha.ClientID%>").value = FecI;

                        sessionStorage.setItem('FecIniC', FecI);
                        var s = document.getElementById('from').value;
                        sessionStorage.setItem('FecFormatoIni', s);
                    }),

                to = $("#to").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    numberOfMonths: 1,
                    altField: "#hfFechaFin1",
                    altFormat: "yy-mm-dd 23:59:59",
                    yearRange: "-1y:+1y"
                })
                    .on("change", function () {
                        from.datepicker("option", "maxDate", getDate(this));
                        $("#to").datepicker("option", "dateFormat", 'DD, d MM, yy');
                        $("#from").datepicker({ minDate: to });

                        var FecF = document.getElementById('hfFechaFin1').value;
                        sessionStorage.setItem('FecFinC', FecF);
                        document.getElementById("<%=finalFecha.ClientID%>").value = FecF;

                        var h = document.getElementById('to').value;
                        sessionStorage.setItem('FecFormatoFin', h);
                    });

            function getDate(element) {
                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }
                return date;
            }

        });
    </script>
    <script type="text/javascript">
        var i = sessionStorage.getItem('FecIniC');
        var f = sessionStorage.getItem('FecFinC');
        var l = sessionStorage.getItem('FecFormatoIni');
        var m = sessionStorage.getItem('FecFormatoFin');
        if (f != null && i != null) {
            $(document).ready(function () {
                document.getElementById('from').value = l;
                $("#hfFechaInicio").text(i);
                document.getElementById('to').value = m;
                $("#hfFechaFin1").text(f);
                document.getElementById("<%=inicioFecha.ClientID%>").value = i;
                document.getElementById("<%=finalFecha.ClientID%>").value = f;
            });
        }
    </script>
    
    <script type="text/javascript">
        function insertaJsPostback() {
            var dateFormat = "dd/mm/yy",
                from = $("#from")
                    .datepicker({
                        changeMonth: true,
                        changeYear: true,
                        numberOfMonths: 1,
                        altField: "#hfFechaInicio",
                        altFormat: "yy-mm-dd 00:00:00",
                        yearRange: "-1y:+1y"
                    })
                    .on("change", function () {
                        to.datepicker("option", "minDate", getDate(this));
                        $("#from").datepicker("option", "dateFormat", 'DD, d MM, yy');
                        var FecI = document.getElementById('hfFechaInicio').value;
                        document.getElementById("<%=inicioFecha.ClientID%>").value = FecI;

                            sessionStorage.setItem('FecIniC', FecI);
                            var s = document.getElementById('from').value;
                            sessionStorage.setItem('FecFormatoIni', s);
                        }),

                    to = $("#to").datepicker({
                        changeMonth: true,
                        changeYear: true,
                        numberOfMonths: 1,
                        altField: "#hfFechaFin1",
                        altFormat: "yy-mm-dd 23:59:59",
                        yearRange: "-1y:+1y"
                    })
                        .on("change", function () {
                            from.datepicker("option", "maxDate", getDate(this));
                            $("#to").datepicker("option", "dateFormat", 'DD, d MM, yy');
                            $("#from").datepicker({ minDate: to });

                            var FecF = document.getElementById('hfFechaFin1').value;
                            sessionStorage.setItem('FecFinC', FecF);
                            document.getElementById("<%=finalFecha.ClientID%>").value = FecF;

                            var h = document.getElementById('to').value;
                            sessionStorage.setItem('FecFormatoFin', h);
                        });

            function getDate(element) {
                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }
                return date;
            }

            var i = sessionStorage.getItem('FecIniC');
            var f = sessionStorage.getItem('FecFinC');
            var l = sessionStorage.getItem('FecFormatoIni');
            var m = sessionStorage.getItem('FecFormatoFin');
            if (f != null && i != null) {
                $(document).ready(function () {
                    document.getElementById('from').value = l;
                    $("#hfFechaInicio").text(i);
                    document.getElementById('to').value = m;
                    $("#hfFechaFin1").text(f);
                    document.getElementById("<%=inicioFecha.ClientID%>").value = i;
                document.getElementById("<%=finalFecha.ClientID%>").value = f;
                });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
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
                                    <div class="row" runat="server" id="row1">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h4>Configuración Reporte Consolidado</h4>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-6">
                                                            <asp:Label runat="server" CssClass="col-sm-4 control-label">&nbsp;</asp:Label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtPruebas" GroupName="reportes" Checked="true" AutoPostBack="true" />Envío de prueba
                                                            </label>
                                                            <label class="radio-inline">
                                                                <asp:RadioButton runat="server" ID="rbtnEnvioAut" GroupName="reportes" AutoPostBack="true" />
                                                                Envío automático
                                                            </label>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <br />
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-6">
                                                            <asp:Panel ID="rowEmailDest" runat="server" CssClass="form-group">
                                                                <asp:Label ID="lblEmailDest" runat="server" CssClass="col-sm-4 control-label">Email Destinatario: </asp:Label>
                                                                <div class="col-sm-8">
                                                                    <asp:TextBox ID="txtEmailDestinatario" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </asp:Panel>
                                                            <asp:Panel ID="rowAsunto" runat="server" CssClass="form-group">
                                                                <asp:Label ID="lblAsunto" runat="server" CssClass="col-sm-4 control-label">Asunto Correo:</asp:Label>
                                                                <div class="col-sm-8">
                                                                    <asp:TextBox ID="txtAsunto" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </asp:Panel>
                                                            <asp:Panel ID="rowFecIni" runat="server" CssClass="form-group">
                                                                <label class="col-sm-4 control-label">Fecha Inicio: </label>
                                                                <div class="col-sm-8">
                                                                    <input type="text" id="from" name="from" class="form-control" autocomplete="off">
                                                                </div>
                                                            </asp:Panel>

                                                            <!--EJEMPLO-->
                                                            <asp:Panel ID="rowOrganizacion" runat="server" CssClass="form-group" Visible="false">
                                                                <label class="col-sm-4 control-label">Organizacion: </label>
                                                                <div class="col-sm-8">
                                                                    <asp:DropDownList ID="listaOrganizacion" runat="server" AutoPostBack="true" OnSelectedIndexChanged="listaOrganizacion_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </asp:Panel>

                                                            <asp:Panel ID="rowSitio" runat="server" CssClass="form-group" Visible="false">
                                                                <label class="col-sm-4 control-label">Sitio: </label>
                                                                <div class="col-sm-8">
                                                                    <asp:DropDownList ID="listaSitios" runat="server">
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </asp:Panel>
                                                            <!--EJEMPLO-->

                                                            <asp:Panel ID="rowAdjuntarDetalle" runat="server" CssClass="form-group" Visible="false">
                                                                <asp:Label ID="Label2" runat="server" CssClass="col-sm-4 control-label" Text="Adjuntar detalle"></asp:Label>
                                                                <div class="col-sm-4">
                                                                    <label class="switch">
                                                                        <asp:CheckBox runat="server" ID="chkMostrar" />
                                                                        <span class="slider" runat="server" id="slider"></span>
                                                                    </label>
                                                                </div>
                                                            </asp:Panel>
                                                            <asp:Panel ID="rowCantEnvios" runat="server" CssClass="form-group" Visible="false">
                                                                <asp:Label ID="lblCantEnvios" runat="server" CssClass="col-sm-4 control-label">Enviar después de:</asp:Label>
                                                                <div class="col-sm-2">
                                                                    <asp:TextBox ID="txtCantEnvios" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <label class="control-label">dia(s)</label>
                                                            </asp:Panel>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <asp:Panel ID="rowEmailCC" runat="server" CssClass="form-group">
                                                                <asp:Label ID="lblEmailCC" runat="server" CssClass="col-sm-4 control-label">Email CC: </asp:Label>
                                                                <div class="col-sm-8">
                                                                    <asp:TextBox ID="txtEmailCC" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </asp:Panel>
                                                            <asp:Panel ID="rowEmailCCO" runat="server" CssClass="form-group">
                                                                <asp:Label ID="lblEmailCCO" runat="server" CssClass="col-sm-4 control-label">Email CCO:</asp:Label>
                                                                <div class="col-sm-8">
                                                                    <asp:TextBox ID="txtEmailCCo" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </asp:Panel>
                                                            <asp:Panel ID="rowFecFin" runat="server" CssClass="form-group">
                                                                <label class="col-sm-4 control-label">Fecha Fin: </label>
                                                                <div class="col-sm-8">
                                                                    <input type="text" id="to" name="to" class="form-control" autocomplete="off">
                                                                </div>
                                                            </asp:Panel>
                                                            <input type="hidden" id="hfFechaInicio" />
                                                            <input type="hidden" id="hfFechaFin1" />
                                                            <asp:HiddenField runat="server" ID="inicioFecha" />
                                                            <asp:HiddenField runat="server" ID="finalFecha" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <div class="col-sm-4">
                                                                <br />
                                                            </div>
                                                            <div class="col-sm-6">
                                                                <asp:Panel ID="rowGuardar" runat="server" CssClass="form-group">
                                                                    <div class="col-sm-6">
                                                                        <br />
                                                                        <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-keytia-lg" Text="Guardar" OnClick="btnGuardar_Click" />
                                                                    </div>
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
            <asp:PostBackTrigger ControlID="btnGuardar" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtPruebas" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="rbtnEnvioAut" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
