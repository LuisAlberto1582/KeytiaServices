<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true"
    CodeBehind="AppCCustodia.aspx.cs" Inherits="KeytiaWeb.UserInterface.CCustodiaDTI.AppCCustodia"
    ValidateRequest="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <link rel="stylesheet" href="/resources/demos/style.css">
    <style>
        .custom-combobox {
            position: relative;
            display: inline-block;
        }

        .custom-combobox-toggle {
            position: absolute;
            top: 0;
            bottom: 0;
            margin-left: -1px;
            padding: 0;
        }

        .custom-combobox-input {
            margin: 0;
            padding: 5px 10px;
            width: 250px;
        }

        .ui-front {
            z-index: 99999999;
        }
    </style>
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script>
        $(function () {
            $.widget("custom.combobox", {
                _create: function () {
                    this.wrapper = $("<span>")
                        .addClass("custom-combobox")
                        .insertAfter(this.element);

                    this.element.hide();
                    this._createAutocomplete();
                    this._createShowAllButton();
                },

                _createAutocomplete: function () {
                    var selected = this.element.children(":selected"),
                        value = selected.val() ? selected.text() : "";

                    this.input = $("<input>")
                        .appendTo(this.wrapper)
                        .val(value)
                        .attr("title", "")
                        .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
                        .autocomplete({
                            delay: 0,
                            minLength: 0,
                            source: $.proxy(this, "_source")
                        })
                        .tooltip({
                            classes: {
                                "ui-tooltip": "ui-state-highlight"
                            }
                        });

                    this._on(this.input, {
                        autocompleteselect: function (event, ui) {
                            ui.item.option.selected = true;
                            this._trigger("select", event, {
                                item: ui.item.option
                            });
                        },

                        autocompletechange: "_removeIfInvalid"
                    });
                },

                _createShowAllButton: function () {
                    var input = this.input,
                        wasOpen = false;

                    $("<a>")
                        .attr("tabIndex", -1)
                        .attr("title", "Mostrar todos")
                        .tooltip()
                        .appendTo(this.wrapper)
                        .button({
                            icons: {
                                primary: "ui-icon-triangle-1-s"
                            },
                            text: false
                        })
                        .removeClass("ui-corner-all")
                        .addClass("custom-combobox-toggle ui-corner-right")
                        .on("mousedown", function () {
                            wasOpen = input.autocomplete("widget").is(":visible");
                        })
                        .on("click", function () {
                            input.trigger("focus");

                            // Close if already visible
                            if (wasOpen) {
                                return;
                            }

                            // Pass empty string as value to search for, displaying all results
                            input.autocomplete("search", "");
                        });
                },

                _source: function (request, response) {
                    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                    response(this.element.children("option").map(function () {
                        var text = $(this).text();
                        if (this.value && (!request.term || matcher.test(text)))
                            return {
                                label: text,
                                value: text,
                                option: this
                            };
                    }));
                },

                _removeIfInvalid: function (event, ui) {

                    // Selected an item, nothing to do
                    if (ui.item) {
                        return;
                    }

                    // Search for a match (case-insensitive)
                    var value = this.input.val(),
                        valueLowerCase = value.toLowerCase(),
                        valid = false;
                    this.element.children("option").each(function () {
                        if ($(this).text().toLowerCase() === valueLowerCase) {
                            this.selected = valid = true;
                            return false;
                        }
                    });

                    // Found a match, nothing to do
                    if (valid) {
                        return;
                    }

                    // Remove invalid value
                    this.input
                        .val("")
                        .attr("title", value + " no se encontro ninguna coincidencia")
                        .tooltip("open");
                    this.element.val("");
                    this._delay(function () {
                        this.input.tooltip("close").attr("title", "");
                    }, 2500);
                    this.input.autocomplete("instance").term = "";
                },

                _destroy: function () {
                    this.wrapper.remove();
                    this.element.show();
                }
            });

            $("#ctl00_cphContent_drpCenCosEmple").combobox();
            $("#toggle").on("click", function () {
                $("#ctl00_cphContent_drpCenCosEmple").toggle();
            });

        });
    </script>
    <%--20140227AM. Se agrega estilo para evitar que el area de escritura en los textbox multi-line, pueda ser modificado. (testeado en chrome y en firefox)--%>
    <style type="text/css">
        textarea {
            resize: none;
        }

        .ajax__calendar_day_disabled {
            background-color: #ccc !important;
            color: #eee !important;
        }
    </style>

    <style type="text/css">
        .modalProgress {
            position: fixed;
            z-index: 10999;
            height: 100%;
            width: 100%;
            top: 0;
            /*            background-color: #666699;
            filter: alpha(opacity=20);
            opacity: 0.2;*/
            /*-moz-opacity: 0.2;*/
        }

        .centerProgress {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            /* background-color:white;*/
            border-radius: 10px;
            /*            filter: alpha(opacity=100);
            opacity: 0.5;*/
            /*-moz-opacity: 0.2;*/
        }

            .centerProgress img {
                height: 128px;
                width: 128px;
            }

        #ctl00_cphContent_CalendarExtender6_container {
            position: static !important;
        }

        .formInLine {
            margin-top: 7px;
            margin-bottom: 7px;
        }

        .asterisk_input:after {
            content: "*";
            color: #e32;
            /*position: absolute;
            margin: 0px 0px 0px -20px;*/
            font-size: 24px;
            padding: 0 5px 0 0;
        }
    </style>

    <script type="text/javascript">  
        function TextBoxModalGenerico(mensaje) {
            document.getElementById('<%= lblBodyMensajeGenerico.ClientID %>').innerHTML = mensaje;
        }
        function TextBoxChange() {
            var tbValue = document.getElementById('<%= txtUsuarRedEmple.ClientID %>').value;

            if (tbValue == "") {
                alerta("El campo de 'Usuario de red' está en blanco, se eliminará el usuario de red " +
                    "que tiene asignado el empleado actualmente al guardar los cambios.");
            }
        }
    </script>

    <script type="text/javascript">
        function checkDate(sender, args) {
            var toDate = new Date();
            toDate.setMinutes(0);
            toDate.setSeconds(0);
            toDate.setHours(0);
            toDate.setMilliseconds(0);
            if (sender._selectedDate > toDate) {
                alert("No se puede seleccionar una fecha mayor al dia de hoy!");
                sender._selectedDate = toDate;
                //set the date back to the current date
                sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
        }
    </script>
    <script type="text/javascript">/*Este script se utiliza para que funcione el autocomplete si se hace un postback mediante un boton u otro control asp*/
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
            $("#" + "<%=txtLocalidadEmple.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetLocalidad",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=txtLocalidadEmple.ClientID %>").val(ui.item.label);
                }
            });

            $("#" + "<%=txtRegion.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetRegionLinea",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=txtRegion.ClientID %>").val(ui.item.label);
                }
            });

            $("#" + "<%=txtJefeEmple.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetJefeEmple",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.ID, email: item.Email };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=hdfJefeEmple.ClientID %>").val(ui.item.description);
                    $("#" + "<%=txtJefeEmple.ClientID %>").val(ui.item.label);
                    $("#" + "<%=txtEmailJefeEmple.ClientID %>").val(ui.item.email);
                }
            });

            $("#" + "<%=txtAreaLinea.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetAreasLinea",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.iCodCatalogo };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=hdfAreaLinea.ClientID %>").val(ui.item.description);
                    $("#" + "<%=txtAreaLinea.ClientID %>").val(ui.item.label);
                }
            });

            $("#" + "<%=txtCentroCosto.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: pagePath + "/GetCencosRazon",
                        data: "{ 'texto': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            dataJSON = JSON.parse(data.d)
                            response($.map(dataJSON, function (item) {
                                return { label: item.Descripcion, description: item.iCodCatalogo };
                            }));
                        },
                        error: function (XMLHttpRequest, callStatus, errorThrown) { }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $("#" + "<%=hdfCentroCosto.ClientID %>").val(ui.item.description);
                    $("#" + "<%=txtCentroCosto.ClientID %>").val(ui.item.label);
                }
            });


            $(function () {
                $.widget("custom.combobox", {
                    _create: function () {
                        this.wrapper = $("<span>")
                            .addClass("custom-combobox")
                            .insertAfter(this.element);

                        this.element.hide();
                        this._createAutocomplete();
                        this._createShowAllButton();
                    },

                    _createAutocomplete: function () {
                        var selected = this.element.children(":selected"),
                            value = selected.val() ? selected.text() : "";

                        this.input = $("<input>")
                            .appendTo(this.wrapper)
                            .val(value)
                            .attr("title", "")
                            .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
                            .autocomplete({
                                delay: 0,
                                minLength: 0,
                                source: $.proxy(this, "_source")
                            })
                            .tooltip({
                                classes: {
                                    "ui-tooltip": "ui-state-highlight"
                                }
                            });

                        this._on(this.input, {
                            autocompleteselect: function (event, ui) {
                                ui.item.option.selected = true;
                                this._trigger("select", event, {
                                    item: ui.item.option
                                });
                            },

                            autocompletechange: "_removeIfInvalid"
                        });
                    },

                    _createShowAllButton: function () {
                        var input = this.input,
                            wasOpen = false;

                        $("<a>")
                            .attr("tabIndex", -1)
                            .attr("title", "Mostrar todos")
                            .tooltip()
                            .appendTo(this.wrapper)
                            .button({
                                icons: {
                                    primary: "ui-icon-triangle-1-s"
                                },
                                text: false
                            })
                            .removeClass("ui-corner-all")
                            .addClass("custom-combobox-toggle ui-corner-right")
                            .on("mousedown", function () {
                                wasOpen = input.autocomplete("widget").is(":visible");
                            })
                            .on("click", function () {
                                input.trigger("focus");

                                // Close if already visible
                                if (wasOpen) {
                                    return;
                                }

                                // Pass empty string as value to search for, displaying all results
                                input.autocomplete("search", "");
                            });
                    },

                    _source: function (request, response) {
                        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                        response(this.element.children("option").map(function () {
                            var text = $(this).text();
                            if (this.value && (!request.term || matcher.test(text)))
                                return {
                                    label: text,
                                    value: text,
                                    option: this
                                };
                        }));
                    },

                    _removeIfInvalid: function (event, ui) {

                        // Selected an item, nothing to do
                        if (ui.item) {
                            return;
                        }

                        // Search for a match (case-insensitive)
                        var value = this.input.val(),
                            valueLowerCase = value.toLowerCase(),
                            valid = false;
                        this.element.children("option").each(function () {
                            if ($(this).text().toLowerCase() === valueLowerCase) {
                                this.selected = valid = true;
                                return false;
                            }
                        });

                        // Found a match, nothing to do
                        if (valid) {
                            return;
                        }

                        // Remove invalid value
                        this.input
                            .val("")
                            .attr("title", value + " no se encontro ninguna coincidencia")
                            .tooltip("open");
                        this.element.val("");
                        this._delay(function () {
                            this.input.tooltip("close").attr("title", "");
                        }, 2500);
                        this.input.autocomplete("instance").term = "";
                    },

                    _destroy: function () {
                        this.wrapper.remove();
                        this.element.show();
                    }
                });

                $("#ctl00_cphContent_drpCenCosEmple").combobox();
                $("#toggle").on("click", function () {
                    $("#ctl00_cphContent_drpCenCosEmple").toggle();
                });

            });


        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <div id="pnlMainHolder" runat="server">
        <div id="pnlRow_0" runat="server" cssclass="row">
            <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">

                <div class="row">
                    <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
                        <div class="portlet solid bordered viewDetailPortlet">
                            <div class="portlet-title">
                                <div class="caption col-md-8 col-sm-8 col-lg-8 col-xs-8">
                                    <asp:Label ID="lblTitle" runat="server" Text="Administración de Empleados y Recursos" CssClass="page-title-keytia"></asp:Label>
                                </div>
                                <div class="actions col-md-4 col-sm-4 col-lg-4 col-xs-4">
                                    <asp:LinkButton ID="lbtnRegresarPaginaBusq" runat="server" Text="Volver a los resultados de la búsqueda"
                                        Font-Bold="true" Font-Size="17px" OnClick="lbtnRegresarPaginaBusq_Click"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnRegresarPagBusqExternaCCust" runat="server" Text="Volver a los resultados de la búsqueda"
                                        Font-Bold="true" Visible="False" OnClick="lbtnRegresarPagBusqExternaCCust_Click"></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Datos de Empleado-->
                <div class="row">
                    <div class="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia">Datos de Empleado</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#datosEmple" aria-expanded="true" aria-controls="datosEmple"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body">
                                <div class="collapse in">
                                    <div class="tabbable tabbable-tabdrop">
                                        <div class="tab-content">
                                            <div class="tab-pane active">
                                                <div id="datosEmple">
                                                    <asp:Panel ID="pDatosEmple" runat="server" BackColor="White" CssClass="col-md-12 col-sm-12">
                                                        <asp:UpdatePanel ID="upDatosEmple" UpdateMode="Conditional" runat="server">
                                                            <Triggers>
                                                                <asp:PostBackTrigger ControlID="lbtnEditEmple" />
                                                                <asp:PostBackTrigger ControlID="lbtnSaveEmple" />
                                                                <%--<asp:PostBackTrigger ControlID="drpJefeEmple" />--%>
                                                                <asp:PostBackTrigger ControlID="lbtnCancelarEmple" />
                                                                <asp:PostBackTrigger ControlID="lbtnDeleteEmple" />
                                                                <asp:PostBackTrigger ControlID="txtUsuarRedEmple" />
                                                                <asp:PostBackTrigger ControlID="drpTipoEmpleado" />
                                                                <asp:PostBackTrigger ControlID="lbtnAgregarPuesto" />
                                                                <asp:PostBackTrigger ControlID="btnGuardarPuesto" />
                                                                <asp:PostBackTrigger ControlID="btnGuardarCenCos" />
                                                                <asp:PostBackTrigger ControlID="lbtnAgregarCenCos"/>
                                                            </Triggers>
                                                            <ContentTemplate>
                                                                <br />
                                                                <div class="form-horizontal" role="form">
                                                                    <div class="row">
                                                                        <div class="col-md-6 col-sm-6">
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblFechaT" runat="server" CssClass="col-sm-4 control-label">Fecha:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtFecha" runat="server" ReadOnly="false" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                        <asp:HiddenField ID="hdnFechaFinEmple" runat="server" />
                                                                                        <asp:CalendarExtender ID="ceSelectorFecha1" runat="server" TargetControlID="txtFecha">
                                                                                        </asp:CalendarExtender>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk1" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>

                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblNominaT" runat="server" CssClass="col-sm-4 control-label">Nómina:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtNominaEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk2" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblNombreT" runat="server" CssClass="col-sm-4 control-label">Nombre:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtNombreEmple" Text="" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk3" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblPaternoT" runat="server" CssClass="col-sm-4 control-label">Apellido Paterno:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:TextBox ID="txtApPaternoEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>

                                                                            <div class="form-group" runat="server" visible="false">
                                                                                <asp:Label ID="lblUbicacionT" runat="server" CssClass="col-sm-4 control-label">Ubicación:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:DropDownList ID="drpSitioEmple" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control">
                                                                                        <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                    </asp:DropDownList>
                                                                                </div>
                                                                            </div>

                                                                            <div class="form-group" runat="server" visible="false">
                                                                                <asp:Label ID="lblEmpresa" runat="server" CssClass="col-sm-4 control-label">Empresa:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:DropDownList ID="drpEmpresaEmple" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control">
                                                                                        <asp:ListItem Text="-- Selecciona una --" Value="" />
                                                                                    </asp:DropDownList>
                                                                                </div>
                                                                            </div>
                                                                            <asp:Panel ID="pnlCencos" runat="server">
                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblCenCos" runat="server" CssClass="col-sm-4 control-label">Centro de costos:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <div class="input-group">
                                                                                            <asp:DropDownList ID="drpCenCosEmple" runat="server" Enabled="false" AppendDataBoundItems="true" CssClass="custom-combobox">
                                                                                                <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                            </asp:DropDownList>
                                                                                            <div class="input-group-btn">
                                                                                                <asp:ImageButton ID="lbtnAgregarCenCos" runat="server" OnClick="btnAgregarCenCos_Click" ImageUrl="~/images/addsmall.png" />
                                                                                                <asp:Label runat="server" ID="lblasterisk4" CssClass="asterisk_input"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </asp:Panel>


                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblLocalildadT" runat="server" CssClass="col-sm-4 control-label">Ubicación:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:DropDownList ID="drpLocalidadEmple" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                        </asp:DropDownList>
                                                                                        <asp:TextBox ID="txtLocalidadEmple" runat="server" CssClass="form-control" Visible="false" Enabled="false"></asp:TextBox>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk5" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblJefeInmT" runat="server" CssClass="col-sm-4 control-label">Jefe Inmediato:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox runat="server" ID="txtJefeEmple" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                                        <asp:HiddenField runat="server" ID="hdfJefeEmple" />
                                                                                        <%--                                                                                        <asp:DropDownList ID="drpJefeEmple" runat="server" Enabled="false" Visible="false" AppendDataBoundItems="true" AutoPostBack="true"
                                                                                            OnSelectedIndexChanged="drpJefeEmple_IndexChanged" CssClass="form-control">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                        </asp:DropDownList>--%>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk6" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblEmailJefe" runat="server" CssClass="col-sm-4 control-label">E-mail del jefe:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:TextBox ID="txtEmailJefeEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>

                                                                            <%--Campos FCA 1 Col--%>

                                                                            <asp:Panel ID="pnlDatosEmpleFCACol1" runat="server" Visible="false">
                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblDatosEmpleFCADC_ID" runat="server" CssClass="col-sm-4 control-label">Dc_id:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:TextBox ID="txtDatosEmpleFCADC_ID" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblDatosEmpleFCAT_ID" runat="server" CssClass="col-sm-4 control-label">T_id:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:TextBox ID="txtDatosEmpleFCAT_ID" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                    </div>
                                                                                </div>

                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblDatosEmpleFCAPlanta" runat="server" CssClass="col-sm-4 control-label">Planta:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:DropDownList ID="ddlDatosEmpleFCAPlanta" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                        </asp:DropDownList>
                                                                                    </div>
                                                                                </div>


                                                                            </asp:Panel>

                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblPrepmovil" runat="server" CssClass="col-sm-4 control-label">Presupuesto Móvil:</asp:Label>
                                                                                <div class="col-sm-4">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox runat="server" ID="txtPrepMovil" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk7" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblPrepFija" runat="server" CssClass="col-sm-4 control-label">Presupuesto Fija:</asp:Label>
                                                                                <div class="col-sm-4">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtPrepFija" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk8" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-md-6 col-sm-6">
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblFolioT" runat="server" CssClass="col-sm-4 control-label">No. de Folio:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:TextBox ID="txtFolioCCustodia" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <asp:Panel ID="pnlEstatus" runat="server" CssClass="form-group">
                                                                                <asp:Label ID="lblStatusT" runat="server" CssClass="col-sm-4 control-label">Estatus:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:TextBox ID="txtEstatusCCustodia" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </asp:Panel>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblSegundoNomT" runat="server" CssClass="col-sm-4 control-label">Segundo Nombre:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:TextBox ID="txtSegundoNombreEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblMaternoT" runat="server" CssClass="col-sm-4 control-label">Apellido Materno:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <asp:TextBox ID="txtApMaternoEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblTipoEmpleT" runat="server" CssClass="col-sm-4 control-label">Tipo de Empleado:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:DropDownList ID="drpTipoEmpleado" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpTipoEmpleado_SelectedIndexChanged">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                        </asp:DropDownList>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk9" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblPuestoT" runat="server" CssClass="col-sm-4 control-label">Puesto:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:DropDownList ID="drpPuestoEmple" runat="server" Enabled="false" AppendDataBoundItems="true" CssClass="form-control">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                        </asp:DropDownList>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:ImageButton ID="lbtnAgregarPuesto" runat="server" OnClick="btnAgregarPuesto_Click" ImageUrl="~/images/addsmall.png" />
                                                                                            <asp:Label runat="server" ID="lblasterisk10" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblEmailEmple" runat="server" CssClass="col-sm-4 control-label">E-mail:</asp:Label>
                                                                                <div class="col-sm-8">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtEmailEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                        <div class="input-group-btn">
                                                                                            <asp:Label runat="server" ID="lblasterisk11" CssClass="asterisk_input"></asp:Label>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <asp:Panel runat="server" ID="pnlDatosUsuar" Enabled="true">
                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblUusarT" runat="server" CssClass="col-sm-4 control-label">Usuario:</asp:Label>
                                                                                    <asp:UpdatePanel ID="upUsuRed" runat="server" UpdateMode="Conditional">
                                                                                        <ContentTemplate>
                                                                                            <div class="col-sm-5">
                                                                                                <asp:TextBox ID="txtUsuarRedEmple" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                            </div>
                                                                                            <div class="col-sm-3">
                                                                                                <asp:CheckBox ID="chkUsuarioPendiente" runat="server" Text="Usuario pendiente" OnCheckedChanged="chkUsuarioPendiente_OnCheckedChanged"
                                                                                                    AutoPostBack="true" Visible="false" CssClass="checkbox-inline" />
                                                                                            </div>
                                                                                        </ContentTemplate>
                                                                                    </asp:UpdatePanel>
                                                                                </div>

                                                                            </asp:Panel>


                                                                            <%--    Campos FCA 2 Col--%>

                                                                            <asp:Panel ID="pnlDatosEmpleFCAcol2" runat="server" Visible="false">
                                                                                <asp:Panel runat="server" ID="pnlNcknamegroup">
                                                                                    <div class="form-group">
                                                                                        <asp:Label ID="lblDatosEmpleFCANickName" runat="server" CssClass="col-sm-4 control-label">Nick-Name:</asp:Label>
                                                                                        <div class="col-sm-8">
                                                                                            <asp:TextBox ID="txtDatosEmpleFCANickName" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                        </div>
                                                                                    </div>
                                                                                </asp:Panel>

                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblDatosEmpleFCAEstacion" runat="server" CssClass="col-sm-4 control-label">Estación:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:TextBox ID="txtDatosEmpleFCAEstacion" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblDatosEmpleFCADirector" runat="server" CssClass="col-sm-4 control-label">Director:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:DropDownList ID="ddlDatosEmpleFCADirector" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                        </asp:DropDownList>
                                                                                    </div>
                                                                                </div>

                                                                                <div class="form-group">
                                                                                    <asp:Label ID="lblatosEmpleFCADpto" runat="server" CssClass="col-sm-4 control-label">Departamento:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:DropDownList ID="ddlDatosEmpleFCADpto" runat="server" AppendDataBoundItems="true" Enabled="false" CssClass="form-control">
                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                        </asp:DropDownList>
                                                                                    </div>
                                                                                </div>
                                                                            </asp:Panel>
                                                                            <div class="form-group">
                                                                                <div class=" col-sm-3">
                                                                                    <asp:CheckBox ID="cbDatosEmpleFCAEsDirector" runat="server" Text="Director" Enabled="false" Visible="false" CssClass="checkbox-inline" />
                                                                                </div>

                                                                                <div class="col-sm-3 col-md-3">
                                                                                    <asp:CheckBox ID="cbEsGerenteEmple" runat="server" Text="Gerente" Enabled="false" Checked="false" CssClass="checkbox-inline" />
                                                                                </div>
                                                                                <div class="col-sm-6 col-md-6">
                                                                                    <asp:CheckBox ID="cbVisibleDirEmple" runat="server" Text="Visible en directorio" Enabled="false" Checked="false" CssClass="checkbox-inline" />
                                                                                </div>
                                                                            </div>

                                                                            <asp:Panel ID="trOpcSincronizacion" runat="server" CssClass="form-group">
                                                                                <asp:UpdatePanel ID="upOmiteSincro" runat="server" UpdateMode="Conditional">
                                                                                    <ContentTemplate>
                                                                                        <div class="col-sm-5">
                                                                                            <asp:CheckBox ID="ckbOmiteSincro" runat="server" OnCheckedChanged="ckbOmiteSincro_OnCheckedChanged" AutoPostBack="true"
                                                                                                CssClass="checkbox-inline" Text="Omitir de Sincronización" />
                                                                                        </div>
                                                                                        <div class="col-sm-7">
                                                                                            <asp:TextBox ID="txtComentariosSincro" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"
                                                                                                Visible="false" TextMode="MultiLine" Height="42" placeholder="Escriba un comentario"></asp:TextBox>
                                                                                        </div>
                                                                                    </ContentTemplate>
                                                                                </asp:UpdatePanel>
                                                                            </asp:Panel>
                                                                            <div class="form-group">
                                                                                <asp:Label ID="lblPrepTemporal" runat="server" CssClass="col-sm-4 control-label">Presupuesto Temporal</asp:Label>
                                                                                <div class="col-sm-4">
                                                                                    <asp:TextBox ID="txtPrepTemporal" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <br />
                                                                <asp:Table ID="tblEditDeleteEmple" runat="server" Width="100%">
                                                                    <asp:TableRow ID="tblEditDeleteEmpleF1" runat="server">
                                                                        <asp:TableCell ID="tblEditDeleteEmpleC1" runat="server" HorizontalAlign="Center">
                                                                            <asp:LinkButton ID="lbtnEditEmple" runat="server" Text="Editar" OnClick="lbtnEditEmple_Click" CssClass="btn btn-keytia-sm"></asp:LinkButton>&nbsp&nbsp
                                                                            <asp:LinkButton ID="lbtnSaveEmple" runat="server" Text="Guardar" OnClick="lbtnSaveEmple_Click" CssClass="btn btn-keytia-sm" Visible="false" Enabled="false"></asp:LinkButton>&nbsp&nbsp
                                                                            <asp:LinkButton ID="lbtnDeleteEmple" runat="server" Text="Borrar" OnClick="lbtnDeleteEmple_Click" CssClass="btn btn-keytia-sm"></asp:LinkButton>&nbsp&nbsp
                                                                            <asp:LinkButton ID="lbtnCancelarEmple" Text="Cancelar" runat="server" Enabled="false" CssClass="btn btn-keytia-sm" Visible="false" OnClick="lbtnCancelarEmple_Click"></asp:LinkButton>&nbsp&nbsp
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                </asp:Table>
                                                                <br />
                                                                <!--Modal PopUp para agregar un nuevo centro de costos-->
                                                                <asp:Panel ID="pnlAddCenCos" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                    <div class="rule"></div>
                                                                    <div class="modal-dialog modal-lg">
                                                                        <div class="modal-content">
                                                                            <div class="modal-header">
                                                                                <asp:Label ID="lblCenCosTitle" runat="server" Text="Centros de Costos"></asp:Label>
                                                                                <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalCenCos"><i class="fas fa-times"></i></button>
                                                                            </div>
                                                                            <div class="modal-body">
                                                                                <div class="row">
                                                                                    <div id="NuevoCenCos" class="form-horizontal" role="form">
                                                                                        <div class="col-md-12 col-sm-12">
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblClaveCenCos" runat="server" CssClass="col-sm-4 control-label">Número:</asp:Label>
                                                                                                <div class="col-sm-8">
                                                                                                    <asp:TextBox ID="txtClaveCenCos" runat="server" MaxLength="40" CssClass="form-control"></asp:TextBox>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblCenCosDesc" runat="server" CssClass="col-sm-4 control-label">Nombre:</asp:Label>
                                                                                                <div class="col-sm-8">
                                                                                                    <asp:TextBox ID="txtCenCosDesc" runat="server" MaxLength="160" CssClass="form-control"></asp:TextBox>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblFechaIniCenCos" runat="server" CssClass="col-sm-4 control-label">Fecha Inicio:</asp:Label>
                                                                                                <div class="col-sm-8">
                                                                                                    <asp:TextBox ID="txtFechaInicioCenCos" runat="server" Enabled="true" ReadOnly="false" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                    <asp:CalendarExtender ID="ceFechaInicioCenCos" runat="server" TargetControlID="txtFechaInicioCenCos">
                                                                                                    </asp:CalendarExtender>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblEmpleRespCenCos" runat="server" CssClass="col-sm-4 control-label">Empleado Responsable:</asp:Label>
                                                                                                <div class="col-sm-8">
                                                                                                    <asp:DropDownList ID="drpEmpleRespCenCos" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                        <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                                    </asp:DropDownList>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblCenCosResponsable" runat="server" CssClass="col-sm-4 control-label">Centro de Costos Responsable:</asp:Label>
                                                                                                <div class="col-sm-8">
                                                                                                    <asp:DropDownList ID="drpCenCosResponsable" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                        <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                                    </asp:DropDownList>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblCenCosEmpresa" runat="server" CssClass="col-sm-4 control-label">Empresa:</asp:Label>
                                                                                                <div class="col-sm-8">
                                                                                                    <asp:DropDownList ID="drpCenCosEmpresa" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                        <asp:ListItem Text="-- Selecciona uno --" Value="0" />
                                                                                                    </asp:DropDownList>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="modal-footer">
                                                                                <asp:Button ID="btnCancelarCenCos" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                                                                <asp:Button ID="btnGuardarCenCos" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnGuardarCenCos_Click" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:LinkButton ID="lnkBtnFakeAddCenCos" runat="server"></asp:LinkButton>
                                                                <asp:ModalPopupExtender ID="mpeAddCenCos" runat="server" DropShadow="false" PopupControlID="pnlAddCenCos"
                                                                    TargetControlID="lnkBtnFakeAddCenCos" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalCenCos">
                                                                </asp:ModalPopupExtender>

                                                                <!--Modal PopUp para agregar un nuevo Puesto de Empleado-->
                                                                <asp:Panel ID="pnlAddPuesto" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                    <div class="rule"></div>
                                                                    <div class="modal-dialog modal-md">
                                                                        <div class="modal-content">
                                                                            <div class="modal-header">
                                                                                <asp:Label ID="lblTituloPopUpPuesto" runat="server" Text="Detalle de puesto"></asp:Label>
                                                                                <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalNewPuesto"><i class="fas fa-times"></i></button>
                                                                            </div>
                                                                            <div class="modal-body">
                                                                                <div class="row">
                                                                                    <div id="NuevoPuesto" class="form-horizontal" role="form">
                                                                                        <div class="col-md-12 col-sm-12">
                                                                                            <div class="form-group">
                                                                                                <asp:Label ID="lblPuestoDesc" runat="server" Text="Puesto:" CssClass="col-sm-2 control-label"></asp:Label>
                                                                                                <div class="col-sm-10">
                                                                                                    <asp:TextBox ID="txtPuestoDesc" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    <asp:RequiredFieldValidator ID="rfvPuestoDesc" runat="server" ErrorMessage="Capture la descripcion del puesto"
                                                                                                        ControlToValidate="txtPuestoDesc" Display="Dynamic" ValidationGroup="upDatosEmple">*</asp:RequiredFieldValidator>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="modal-footer">
                                                                                <asp:Button ID="btnGuardarPuesto" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnGuardarPuesto_Click" />
                                                                                <asp:Button ID="btnCancelarPuesto" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <asp:ValidationSummary ID="ValSumAddPuesto" runat="server" />
                                                                </asp:Panel>
                                                                <asp:LinkButton ID="lnkBtnFakeAddPuesto" runat="server"></asp:LinkButton>
                                                                <asp:ModalPopupExtender ID="mpeAddPuesto" runat="server" DropShadow="false" PopupControlID="pnlAddPuesto"
                                                                    TargetControlID="lnkBtnFakeAddPuesto" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalNewPuesto">
                                                                </asp:ModalPopupExtender>


                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        <asp:UpdateProgress runat="server" ID="upUsuarioDeRed" AssociatedUpdatePanelID="upDatosEmple">
                                                            <ProgressTemplate>
                                                                <div class="modalProgress">
                                                                    <div class="centerProgress">
                                                                        <asp:Image runat="server" ID="imgUsuarRed" ImageUrl="~/images/loader2.gif" />
                                                                    </div>
                                                                </div>
                                                            </ProgressTemplate>
                                                        </asp:UpdateProgress>
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
                <!--Datos de códigos, extensiones y lineas-->
                <div class="row">
                    <asp:Panel ID="pHeaderCodAutoExten" runat="server">
                        <div class="col-md-12 col-sm-12">
                            <div class="portlet solid bordered">
                                <div class="portlet-title">
                                    <div class="caption">
                                        <i class="icon-bar-chart font-dark hide"></i>
                                        <span class="caption-subject titlePortletKeytia">Recursos asignados</span>
                                    </div>
                                    <div class="actions">
                                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#recursosEmple" aria-expanded="true" aria-controls="recursosEmple"><i class="far fa-minus-square"></i></button>
                                    </div>
                                </div>
                                <div class="portlet-body">
                                    <div class="collapse in">
                                        <div class="tabbable tabbable-tabdrop">
                                            <div class="tab-content">
                                                <div class="tab-pane active">
                                                    <div id="recursosEmple">
                                                        <asp:Panel ID="pDatosCodAutoExten" runat="server" CssClass="col-md-12 col-sm-12" BackColor="White">
                                                            <!--Extensiones-->
                                                            <asp:UpdatePanel ID="upDatosCodAutoExten" runat="server" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <div class="table-responsive">
                                                                        <asp:GridView ID="grvExten" runat="server" DataKeyNames="Exten,Sitio,Cos,TipoExten,iCodRegRelEmpExt"
                                                                            AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                                                            EmptyDataText="No existen extensiones asignadas a este empleado">
                                                                            <Columns>
                                                                                <%--AM. 20130717 Se agregaron las columnas de FechaFin y numero de registro de la relación--%>
                                                                                <%--0--%><asp:BoundField DataField="Exten" Visible="false" ReadOnly="true" />
                                                                                <%--1--%><asp:BoundField DataField="Sitio" Visible="false" ReadOnly="true" />
                                                                                <%--2--%><asp:BoundField DataField="Cos" HtmlEncode="false" Visible="false" />
                                                                                <%--3--%><asp:BoundField DataField="TipoExten" Visible="false" ReadOnly="true" />
                                                                                <%--4--%><asp:BoundField DataField="ExtenCod" HeaderText="Extensión" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--5--%><asp:BoundField DataField="SitioDesc" HeaderText="Sitio" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--6--%><asp:BoundField DataField="CosDesc" HeaderText="Cos" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--7--%><asp:BoundField DataField="FechaIni" HeaderText="Fecha Inicial" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--8--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Final" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--9--%><asp:BoundField DataField="TipoExtenDesc" HeaderText="Tipo" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--10--%><asp:CheckBoxField DataField="VisibleDir" HeaderText="Visible en Directorio" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--11--%><asp:BoundField DataField="ComentarioExten" HeaderText="Comentarios" HtmlEncode="true" />
                                                                                <%--12--%><asp:BoundField DataField="iCodRegRelEmpExt" Visible="false" ReadOnly="true" />
                                                                                <%--13--%><asp:TemplateField HeaderText="Editar" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnEditarExtenRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvExten_EditRow"
                                                                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <%--14--%><asp:TemplateField HeaderText="Borrar" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnBorrarExtenRow" ImageUrl="~/images/deletesmall.png" OnClick="grvExten_DeleteRow"
                                                                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                        <br />
                                                                    </div>
                                                                    <asp:Panel ID="pnlAltaDeExtensiones" runat="server" CssClass="form-horizontal" role="form">
                                                                        <div class="row">
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblExtenReg" runat="server" CssClass="control-label">Extensión</asp:Label>
                                                                                <asp:TextBox ID="txtExtensionNoPopUp" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                            </div>
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblSitioExtenReg" runat="server" CssClass="control-label">Sitio</asp:Label>
                                                                                <asp:DropDownList ID="drpSitioNoPopUp" runat="server" AppendDataBoundItems="true" AutoPostBack="true"
                                                                                    CssClass="form-control" OnSelectedIndexChanged="drpSitioCodAutoNoPopUp_Changed">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblCosExtenReg" runat="server" CssClass="control-label">Cos</asp:Label>
                                                                                <asp:DropDownList ID="drpCosExtenNoPopUp" runat="server" DataValueField="iCodCatalogo"
                                                                                    DataTextField="vchDescripcion" CssClass="form-control">
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                        </div>
                                                                        <br />
                                                                        <div class="row">
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblFechaInicialExtenReg" runat="server" CssClass="control-label">Fecha Inicial</asp:Label>
                                                                                <asp:TextBox ID="txtFechaInicioNoPopUp" runat="server" ReadOnly="false" Enabled="true"
                                                                                    MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                <asp:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtFechaInicioNoPopUp">
                                                                                </asp:CalendarExtender>
                                                                            </div>
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblTipoExtenReg" runat="server" CssClass="control-label">Tipo</asp:Label>
                                                                                <asp:DropDownList ID="drpTipoExtenNoPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblVisibleExtenReg" runat="server" CssClass="control-label">Visible en directorio</asp:Label>
                                                                                <asp:DropDownList ID="drpVisibleDirNoPopUp" runat="server" CssClass="form-control">
                                                                                    <asp:ListItem Value="1">Si</asp:ListItem>
                                                                                    <asp:ListItem Value="0">No</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                        </div>
                                                                        <br />
                                                                        <div class="row">
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label ID="lblComentariosExtenReg" runat="server" CssClass="control-label">Comentarios</asp:Label>
                                                                                <asp:TextBox ID="txtComentariosExtenNoPopUp" runat="server" TextMode="MultiLine" Height="50" CssClass="form-control"></asp:TextBox>
                                                                            </div>
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <asp:Label runat="server" CssClass="control-label" Font-Bold="true" Text=" '" ForeColor="White"></asp:Label>
                                                                                <asp:CheckBox ID="cbRangoExtenNoPopUp" runat="server" Text="Dar de alta nuevo rango de extensión" CssClass="checkbox-inline" />
                                                                            </div>
                                                                            <div class="col-md-4 col-sm-4">
                                                                                <br />
                                                                                <div class="form-group">
                                                                                    <div class="col-sm-12">
                                                                                        <asp:LinkButton ID="lbtnGuardarExtenNoPopUp" runat="server" OnClick="lbtnGuardar_ExtenNoPopUp"
                                                                                            CssClass="btn btn-keytia-md" UseSubmitBehavior="false" OnClientClick="this.disabled='true';">Agregar</asp:LinkButton>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <br />
                                                                    <!--Modal PopUp para Extensiones-->
                                                                    <asp:Panel ID="pnlAddEditExten" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-md">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label Font-Bold="true" ID="lblTituloPopUpExten" runat="server" Text="Detalle de extensión"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalEditExten"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body">
                                                                                    <div class="row">
                                                                                        <div class="form-horizontal" role="form">
                                                                                            <div class="col-md-12 col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblExtenCod" runat="server" Text="Extensión" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtExtension" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvExtension" runat="server" ControlToValidate="txtExtension"
                                                                                                            ErrorMessage="*" SetFocusOnError="True" ValidationGroup="upDatosCodAutoExten">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblSitioExten" runat="server" Text="Sitio" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpSitio" runat="server" AppendDataBoundItems="true" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpSitioCodAutoNoPopUp_Changed">
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvSitio" runat="server" ControlToValidate="drpSitio"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaIniExten" runat="server" Text="Fecha Inicial" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaInicio" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtFechaInicio">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvFechaInicio" runat="server" ControlToValidate="txtFechaInicio"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaFinExten" runat="server" Text="Fecha Final" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaFinExten" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtFechaFinExten"
                                                                                                            OnClientDateSelectionChanged="checkDate" Format="dd/MM/yyyy">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvFechaFinExten" runat="server" ControlToValidate="txtFechaFinExten"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCosExten" runat="server" Text="Cos" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpCosExten" runat="server" DataValueField="iCodCatalogo" CssClass="form-control" DataTextField="vchDescripcion">
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblTipoExten" runat="server" Text="Tipo" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpTipoExten" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvTipoExten" runat="server" ControlToValidate="drpTipoExten" ErrorMessage="*" ValidationGroup="upDatosCodAutoExten">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblVisibleDirExten" runat="server" Text="Visible en Directorio" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpVisibleDir" runat="server" CssClass="form-control">
                                                                                                            <asp:ListItem Value="1">Si</asp:ListItem>
                                                                                                            <asp:ListItem Value="0">No</asp:ListItem>
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvVisibleDir" runat="server" ControlToValidate="drpVisibleDir"
                                                                                                            ErrorMessage="*" InitialValue="Si" ValidationGroup="upDatosCodAutoExten">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblComentarioExten" runat="server" Text="Comentarios" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtComentariosExten" runat="server" TextMode="MultiLine" Height="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none">
                                                                                                    <asp:CheckBox ID="cbEditarExtension" runat="server" Visible="false" Checked="False" />
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none">
                                                                                                    <asp:CheckBox ID="cbBajaExtension" runat="server" Visible="false" Checked="False" />
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none">
                                                                                                    <asp:TextBox ID="txtRegistroRelacion" runat="server" Visible="False"></asp:TextBox>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnCancelarExten" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" OnClientClick="return Hidepopup()" />
                                                                                    <asp:Button ID="btnGuardarExten" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnGuardar_PopUpExten" UseSubmitBehavior="false" OnClientClick="this.disabled='true';" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkFakeExten" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpeExten" runat="server" DropShadow="false" PopupControlID="pnlAddEditExten"
                                                                        TargetControlID="lnkFakeExten" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalEditExten">
                                                                    </asp:ModalPopupExtender>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <asp:UpdateProgress runat="server" ID="ProcesandoAltaExtension" AssociatedUpdatePanelID="upDatosCodAutoExten">
                                                                <ProgressTemplate>
                                                                    <div class="modalProgress">
                                                                        <div class="centerProgress">
                                                                            <asp:Image runat="server" ID="imgExten" ImageUrl="~/images/loader2.gif" />
                                                                        </div>
                                                                    </div>
                                                                </ProgressTemplate>
                                                            </asp:UpdateProgress>

                                                            <!--Codigos-->
                                                            <asp:UpdatePanel ID="upDatosCodAutoExten2" runat="server" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <div class="table-responsive">
                                                                        <asp:GridView ID="grvCodAuto" runat="server" DataKeyNames="CodAuto,Sitio,Cos,iCodRegRelEmpCodAuto"
                                                                            AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                                                            EmptyDataText="No existen códigos asignados a este empleado">
                                                                            <Columns>
                                                                                <%--0--%><asp:BoundField DataField="CodAutoCod" HeaderText="Código de Llamadas" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--1--%><asp:BoundField DataField="SitioDesc" HeaderText="Sitio" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--2--%><asp:BoundField DataField="CosDesc" HeaderText="Cos" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--3--%><asp:BoundField DataField="FechaIni" HeaderText="Fecha Inicial" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--4--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Fin" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--RZ.20131227 Se retira campo "Visible en Directorio"--%>
                                                                                <%--4<asp:CheckBoxField DataField="VisibleDir" HeaderText="Visible en Directorio" />--%>
                                                                                <%--5--%><asp:BoundField DataField="CodAuto" HtmlEncode="true" Visible="false" />
                                                                                <%--6--%><asp:BoundField DataField="Sitio" HtmlEncode="true" Visible="false" />
                                                                                <%--7--%><asp:BoundField DataField="Cos" HtmlEncode="true" Visible="false" />
                                                                                <%--8--%><asp:BoundField DataField="iCodRegRelEmpCodAuto" Visible="false" ReadOnly="true" />
                                                                                <%--9--%><asp:TemplateField HeaderText="Editar" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnEditarCodAutoRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvCodAuto_EditRow"
                                                                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <%--10--%><asp:TemplateField HeaderText="Borrar" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnBorrarCodAutoRow" ImageUrl="~/images/deletesmall.png" OnClick="grvCodAuto_DeleteRow"
                                                                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <br />
                                                                    <asp:Panel ID="pnlAltaDeCodigosAut" runat="server" CssClass="form-horizontal" role="form">
                                                                        <div class="">
                                                                            <div class="col-md-2 col-sm-2">
                                                                                <asp:Label ID="lblCodigoReg" runat="server" CssClass="control-label">Código</asp:Label>
                                                                                <div class="form-group">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtCodAutoNoPopUp" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                        <span class="input-group-btn">
                                                                                            <asp:LinkButton ID="btnAutoGenerarCodigoNoPopUp" runat="server" OnClick="btnAutoGenerarCodigoNoPopUp_Click" Height="30px" CssClass="btn-keytia-sm">Generar</asp:LinkButton>
                                                                                        </span>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="col-md-3 col-sm-3">
                                                                                <asp:Label ID="lblSitioCodigoReg" runat="server" CssClass="control-label">Sitio</asp:Label>
                                                                                <asp:DropDownList ID="drpSitioCodAutoNoPopUp" runat="server" AppendDataBoundItems="true"
                                                                                    AutoPostBack="true" OnSelectedIndexChanged="drpSitioCodAutoNoPopUp_Changed" DataValueField="iCodCatalogo"
                                                                                    DataTextField="vchDescripcion" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                            <div class="col-md-3 col-sm-3">
                                                                                <asp:Label ID="lblCosCodigoReg" runat="server" CssClass="control-label">Cos</asp:Label>
                                                                                <asp:DropDownList ID="drpCosCodAutoNoPopUp" runat="server" DataValueField="iCodCatalogo"
                                                                                    DataTextField="vchDescripcion" CssClass="form-control">
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                            <div class="col-md-2 col-sm-2">
                                                                                <asp:Label ID="lblfechaIninCodigo" runat="server" CssClass="control-label">Fecha Inicial</asp:Label>
                                                                                <asp:TextBox ID="txtFechaInicioCodAutoNoPopUp" runat="server" ReadOnly="false" Enabled="true" MaxLength="10" CssClass="form-control">
                                                                                </asp:TextBox>
                                                                                <asp:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtFechaInicioCodAutoNoPopUp">
                                                                                </asp:CalendarExtender>
                                                                            </div>
                                                                            <div class="col-md-2 col-sm-2">
                                                                                <br />
                                                                                <div class="form-group">
                                                                                    <div class="col-sm-12">
                                                                                        <asp:LinkButton ID="lbtnGuardarCodAutoNoPopUp" runat="server" Text="Guardar" OnClick="lbtnGuardar_CodAutoNoPopUp" Height="30px"
                                                                                            CssClass="btn btn-keytia-md" UseSubmitBehavior="false" OnClientClick="this.disabled='true';">Agregar</asp:LinkButton>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <br />
                                                                    <br />
                                                                    <br />
                                                                    <br />
                                                                    <!--Modal PopUp para Codigos de Autorizacion-->
                                                                    <asp:Panel ID="pnlAddEditCodAuto" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-md">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label Font-Bold="true" ID="lblTituloPopUpCodAuto" runat="server" Text="Detalle de códigos"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalEditCodAuto"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body">
                                                                                    <div class="row">
                                                                                        <div class="form-horizontal" role="form">
                                                                                            <div class="col-md-12 col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCodAuto" runat="server" Text="Código" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:TextBox ID="txtCodAuto" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvCodAuto" runat="server" ControlToValidate="txtCodAuto"
                                                                                                            ErrorMessage="*" SetFocusOnError="True" ValidationGroup="upDatosCodAutoExten2">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblSitioCodAuto" runat="server" Text="Sitio" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:DropDownList ID="drpSitioCodAuto" runat="server" AppendDataBoundItems="true" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpSitioCodAutoNoPopUp_Changed">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaIniCodAuto" runat="server" Text="Fecha Inicial" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:TextBox ID="txtFechaInicioCodAuto" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="ceSelectorFecha2" runat="server" TargetControlID="txtFechaInicioCodAuto">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvFechaInicioCodAuto" runat="server" ControlToValidate="txtFechaInicioCodAuto"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten2">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaFinCodAuto" runat="server" Text="Fecha Fin" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:TextBox ID="txtFechaFinCodAuto" runat="server" ReadOnly="false" Enabled="true" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtFechaFinCodAuto"
                                                                                                            OnClientDateSelectionChanged="checkDate" Format="dd/MM/yyyy">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvFechaFinCodAuto" runat="server" ControlToValidate="txtFechaFinCodAuto"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosCodAutoExten2">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCosCodAuto" runat="server" Text="Cos" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:DropDownList ID="drpCosCodAuto" runat="server" CssClass="form-control" DataValueField="iCodCatalogo"
                                                                                                            DataTextField="vchDescripcion">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none;">
                                                                                                    <asp:CheckBox ID="cbEditarCodAuto" runat="server" Visible="false" Checked="False" />
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none;">
                                                                                                    <asp:CheckBox ID="cbBajaCodAuto" runat="server" Visible="false" Checked="False" />
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none;">
                                                                                                    <asp:TextBox ID="txtRegistroRelacionCodAuto" runat="server" Visible="False"></asp:TextBox>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnCancelarCodAuto" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" OnClientClick="return Hidepopup()" />
                                                                                    <asp:Button ID="btnGuardarCodAuto" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm"
                                                                                        OnClick="btnGuardar_PopUpCodAuto" UseSubmitBehavior="false" OnClientClick="this.disabled='true';" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkFakeCodAuto" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpeCodAuto" runat="server" DropShadow="false" PopupControlID="pnlAddEditCodAuto"
                                                                        TargetControlID="lnkFakeCodAuto" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalEditCodAuto">
                                                                    </asp:ModalPopupExtender>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <asp:UpdateProgress runat="server" ID="ProcesandoAltaCodAuto" AssociatedUpdatePanelID="upDatosCodAutoExten2">
                                                                <ProgressTemplate>
                                                                    <div class="modalProgress">
                                                                        <div class="centerProgress">
                                                                            <asp:Image runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" />
                                                                        </div>
                                                                    </div>
                                                                </ProgressTemplate>
                                                            </asp:UpdateProgress>

                                                            <!--Lineas-->
                                                            <%--NZ: 20160620 Se Agrega Sección de Lineas--%>
                                                            <asp:UpdatePanel ID="UpDatosLinea" runat="server" UpdateMode="Conditional">
                                                                <Triggers>
                                                                    <asp:PostBackTrigger ControlID="btnGuardarLinea" />
                                                                </Triggers>
                                                                <ContentTemplate>
                                                                    <div class="table-responsive">
                                                                        <asp:GridView ID="grvLinea" runat="server" DataKeyNames="Linea,Carrier,Sitio,iCodRegRelEmpLinea,PentaSAPCCDescription,PentaSAPAccount,PentaSAPProfitCenter,PentaSAPCostCenter,PentaSAPFA"
                                                                            AutoGenerateColumns="false" HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard"
                                                                            EmptyDataText="No existen lineas asignadas a este empleado" UseAccessibleHeader="true">
                                                                            <Columns>
                                                                                <%--0--%><asp:BoundField DataField="LineaCod" HeaderText="Línea Móvil/Fija" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--1--%><asp:BoundField DataField="CarrierDesc" HeaderText="Carrier" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--2--%><asp:BoundField DataField="SitioDesc" HeaderText="Sitio" HtmlEncode="true" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--3--%><asp:BoundField DataField="FechaIni" HeaderText="Fecha Inicial" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--4--%><asp:BoundField DataField="FechaFin" HeaderText="Fecha Fin" HtmlEncode="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                                                                <%--5--%><asp:BoundField DataField="Linea" HtmlEncode="true" Visible="false" />
                                                                                <%--6--%><asp:BoundField DataField="Carrier" HtmlEncode="true" Visible="false" />
                                                                                <%--7--%><asp:BoundField DataField="Sitio" HtmlEncode="true" Visible="false" />
                                                                                <%--8--%><asp:BoundField DataField="iCodRegRelEmpLinea" Visible="false" ReadOnly="true" />
                                                                                <%--9--%><asp:BoundField DataField="PentaSAPCCDescription" Visible="false" ReadOnly="true" />
                                                                                <%--10--%><asp:BoundField DataField="PentaSAPCCDescriptionDesc" HeaderText="Penta SAP CC Description" Visible="false" ReadOnly="true" />
                                                                                <%--11--%><asp:BoundField DataField="PentaSAPAccount" Visible="false" ReadOnly="true" />
                                                                                <%--12--%><asp:BoundField DataField="PentaSAPAccountDesc" HeaderText="Penta SAP Account" Visible="false" ReadOnly="true" />
                                                                                <%--13--%><asp:BoundField DataField="PentaSAPProfitCenter" Visible="false" ReadOnly="true" />
                                                                                <%--14--%><asp:BoundField DataField="PentaSAPProfitCenterDesc" HeaderText="Penta SAP Profit Center" Visible="false" ReadOnly="true" />
                                                                                <%--15--%><asp:BoundField DataField="PentaSAPCostCenter" Visible="false" ReadOnly="true" />
                                                                                <%--16--%><asp:BoundField DataField="PentaSAPCostCenterDesc" HeaderText="Penta SAP Cost Center" Visible="false" ReadOnly="true" />
                                                                                <%--17--%><asp:BoundField DataField="PentaSAPFA" Visible="false" ReadOnly="true" />
                                                                                <%--18--%><asp:BoundField DataField="PentaSAPFADesc" HeaderText="Penta SAP FA" Visible="false" ReadOnly="true" />

                                                                                <%--19--%><asp:TemplateField HeaderText="Editar" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnEditarLineaRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvLinea_EditRow"
                                                                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <%--20--%><asp:TemplateField HeaderText="Borrar" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnBorrarLineaRow" ImageUrl="~/images/deletesmall.png" OnClick="grvLinea_DeleteRow"
                                                                                            runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <br />
                                                                    <asp:Panel ID="pnlAltaDeLinea" runat="server" CssClass="form-horizontal" role="form">
                                                                        <div class="">
                                                                            <div class="row">
                                                                                <div class="col-sm-12">
                                                                                    <label class="radio-inline">
                                                                                        <asp:RadioButton runat="server" ID="rbtnAlta" GroupName="reportes" Checked="true" AutoPostBack="true" />Linea Nueva
                                                                                    </label>
                                                                                    <label class="radio-inline">
                                                                                        <asp:RadioButton runat="server" ID="rbtnAsignar" GroupName="reportes" AutoPostBack="true" />
                                                                                        Asignar Linea
                                                                                    </label>
                                                                                </div>
                                                                            </div>
                                                                            <div id="divLinea" runat="server" class="col-md-2 col-sm-2 formInLine">
                                                                                <asp:Label ID="lblLineaReg" runat="server" CssClass="control-label">Línea Móvil/Fija</asp:Label>
                                                                                <asp:TextBox ID="txtLineaNoPopUp" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                            </div>
                                                                            <div id="divICCID" runat="server" class="col-md-2 col-sm-2 formInLine">
                                                                                <asp:Label ID="lblICCIDAlta" runat="server" CssClass="control-label">ICCID</asp:Label>
                                                                                <asp:TextBox runat="server" ID="txtICCIDAlta" CssClass="form-control"></asp:TextBox>
                                                                            </div>
                                                                            <div runat="server" id="divCarrierLineaReg" class="col-md-3 col-sm-3 formInLine">
                                                                                <asp:Label ID="lblCarrierLineaReg" runat="server" CssClass="control-label">Carrier</asp:Label>
                                                                                <asp:DropDownList ID="drpCarrierLineaNoPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                            <div runat="server" id="divSitioLineaReg" class="col-md-3 col-sm-3 formInLine">
                                                                                <asp:Label ID="lblSitioLineaReg" runat="server" CssClass="control-label">Sitio</asp:Label>
                                                                                <asp:DropDownList ID="drpSitioLineaNoPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>

                                                                            <!--RM 20190328 Campos Adicionales para FCA -->


                                                                            <div id="divPentaSAPAccount" runat="server" class="col-sm-3 col-md-3 formInLine FCAFormControl" visible="false">
                                                                                <asp:Label ID="lblPentaSAPAccount" runat="server" CssClass="control-label">PSAP Account</asp:Label>
                                                                                <asp:DropDownList ID="ddlPentaSAPAccount" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>

                                                                            <div id="divPentaSAPProfitCenter" runat="server" class="col-sm3 col-md-3 formInLine FCAFormControl" visible="false">
                                                                                <asp:Label ID="lblPentaSAPProfitCenter" runat="server" CssClass="control-label">PentaSAP Profit Center</asp:Label>
                                                                                <asp:DropDownList ID="ddlPentaSAPProfitCenter" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>

                                                                            <div id="divPentaSAPCostCenter" runat="server" class="col-sm-3 col-md-3 formInLine FCAFormControl" visible="false">
                                                                                <asp:Label ID="lblPentaSAPCostCenter" runat="server" CssClass="control-label">PentaSAP Cost Center</asp:Label>
                                                                                <asp:DropDownList ID="ddlPentaSAPCostCenter" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>

                                                                            <div id="divPentaSAPCCDesc" runat="server" class="col-sm-3 col-md-3 formInLine FCAFormControl" visible="false">
                                                                                <asp:Label ID="lblPentaSAPCCDesc" runat="server" CssClass="control-label">PentaSAP CC Desc.</asp:Label>
                                                                                <asp:DropDownList ID="ddlPentaSAPCCDesc" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>

                                                                            <div id="divPentaSAPFAFCA" runat="server" class="col-sm-3 col-md-3 formInLine FCAFormControl" visible="false">
                                                                                <asp:Label ID="lblPentaSAPFAFCA" runat="server" CssClass="control-label">PentaSAP FA</asp:Label>
                                                                                <asp:DropDownList ID="ddlPentaSAPFAFCA" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                    <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                </asp:DropDownList>
                                                                            </div>
                                                                            <!---->

                                                                            <div id="divFechaIni" runat="server" class="col-md-2 col-sm-2 formInLine">
                                                                                <asp:Label ID="lblFechaIniLineaReg" runat="server" CssClass="control-label">Fecha Inicial</asp:Label>
                                                                                <asp:TextBox ID="txtFechaInicioLineaNoPopUp" runat="server" ReadOnly="false" Enabled="true" MaxLength="10" CssClass="form-control">
                                                                                </asp:TextBox>
                                                                                <asp:CalendarExtender ID="CalendarExtender6" runat="server" TargetControlID="txtFechaInicioLineaNoPopUp">
                                                                                </asp:CalendarExtender>
                                                                            </div>


                                                                            <div class="col-md-2 col-sm-2 formInLine">
                                                                                <div class="form-group">
                                                                                    <br />
                                                                                    <div class="col-sm-12">
                                                                                        <asp:LinkButton ID="lbtnGuardarLineaNoPopUp" runat="server" Text="Guardar" OnClick="lbtnGuardar_LineaNoPopUp"
                                                                                            CssClass="btn btn-keytia-md" Height="30px" UseSubmitBehavior="false" OnClientClick="this.disabled='true';">Agregar</asp:LinkButton>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                    </asp:Panel>

                                                                    <br />
                                                                    <br />
                                                                    <br />
                                                                    <br />
                                                                    <br />
                                                                    <!--Modal PopUp para Líneas-->
                                                                    <asp:Panel ID="pnlAddEditLinea" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-lg">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label Font-Bold="true" ID="lblTituloPopUpLinea" runat="server" Text="Detalle de líneas"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalEditLinea"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body scrollbar scrollbar-warning thin">
                                                                                    <div class="form-horizontal" role="form">
                                                                                        <div class="row">
                                                                                            <asp:HiddenField runat="server" ID="Index" />
                                                                                            <asp:HiddenField runat="server" ID="idLinea" />
                                                                                            <div class="col-md-6 col-sm-6">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblLinea" runat="server" Text="Línea" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvLinea" runat="server" ControlToValidate="txtLinea"
                                                                                                            ErrorMessage="*" SetFocusOnError="True" ValidationGroup="upDatosLinea">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCarrierLinea" runat="server" Text="Carrier" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpCarrierLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaIniLinea" runat="server" Text="Fecha Inicial" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaInicioLinea" runat="server" ReadOnly="false" Enabled="true" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                        <%--                                                                                                    <asp:CalendarExtender ID="ceSelectorFechaLinea" runat="server" TargetControlID="txtFechaInicioLinea">
                                                                                                    </asp:CalendarExtender>--%>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <%--                                                                                                    <asp:RequiredFieldValidator ID="rfvFechaInicioLinea" runat="server" ControlToValidate="txtFechaInicioLinea"
                                                                                                        ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosLinea">
                                                                                                    </asp:RequiredFieldValidator>--%>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCtaMaestraLinea" runat="server" Text="Cuenta Maestra" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpCtaMaestraLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control" Enabled="false">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblTipoPlanLinea" runat="server" Text="Tipo de Plan" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpTipoPlanLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblPlanTarifarioLinea" runat="server" Text="Plan Tarifario" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpPlanTarifarioLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblGbPlan" runat="server" CssClass="col-sm-4 control-label" Text="GB Datos:"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtGBPlan" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label runat="server" ID="lblMontoRenta" Text="Monto Renta" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox runat="server" ID="txtMontoRenta" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblFechaLimiteLinea" runat="server" Text="Fecha Limite" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaLimiteLinea" runat="server" ReadOnly="false" Enabled="true" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="ceFechaLimiteLinea" runat="server" TargetControlID="txtFechaLimiteLinea">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblEtiquetaLinea" runat="server" Text="Etiqueta" CssClass="col-sm-4 control-label" Visible="false"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtEtiquetaLinea" runat="server" MaxLength="50" CssClass="form-control" Visible="false"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblIMEILinea" runat="server" Text="IMEI" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtIMEILinea" runat="server" MaxLength="50" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label runat="server" ID="lblICCID" CssClass="col-sm-4 control-label" Text="ICCID"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtICCID" runat="server" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label runat="server" ID="lblArea" Text="Area" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox runat="server" ID="txtAreaLinea" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:HiddenField runat="server" ID="hdfAreaLinea" />
                                                                                                        <asp:DropDownList ID="drpAreaLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control" Visible="false">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                    <div class="form-group">
                                                                                                        <asp:ImageButton ID="btnAgregarAreaLinea" runat="server" OnClick="btnAgregarAreaLinea_Click" ImageUrl="~/images/addsmall.png" />
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label runat="server" ID="lblRegion" CssClass="col-sm-4 control-label" Text="Region (Cliente)"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox runat="server" ID="txtRegion" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>

                                                                                                </div>
                                                                                                <div class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblNumOrdenLinea" runat="server" Text="Núm. de Orden" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtNumOrdenLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--RM 20190407 Elementos Penta FCA-->
                                                                                                <div id="divPentaSAPCCDescriptionPopUp" class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblPentaSAPCCDescriptionPopUp" runat="server" Text="PentaSAP Cost Center Desc." CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="ddlPentaSAPCCDescriptionPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div id="divPentaSAPAccountPopUp" class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblPentaSAPAccountPopUp" runat="server" Text="PSAP Account" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="ddlPentaSAPAccountPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div id="divPentaSAPProfitCenterPopUp" class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblPentaSAPProfitCenterPopUp" runat="server" Text="PentaSAP Profit Center" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="ddlPentaSAPProfitCenterPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!---->

                                                                                                <div class="form-group" style="display: none;">
                                                                                                    <asp:CheckBox ID="cbEditarLinea" runat="server" Visible="false" Checked="False" />
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none;">
                                                                                                    <asp:CheckBox ID="cbBajaLinea" runat="server" Visible="false" Checked="False" />
                                                                                                </div>
                                                                                                <div class="form-group" style="display: none;">
                                                                                                    <asp:TextBox ID="txtRegistroRelacionLinea" runat="server" Visible="False"></asp:TextBox>
                                                                                                </div>
                                                                                            </div>

                                                                                            <div class="col-md-6 col-sm-6">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label runat="server" Text="Sitio" CssClass="col-sm-4 control-label" Visible="false"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblSitioLinea" runat="server" Text="Sitio - Linea" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <div class="input-group">
                                                                                                            <asp:DropDownList runat="server" ID="drpLineaSitio" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                            </asp:DropDownList>
                                                                                                            <asp:DropDownList ID="drpSitioLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control" Visible="false">
                                                                                                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                            </asp:DropDownList>
                                                                                                            <div class="input-group-btn">
                                                                                                                <asp:ImageButton ID="btnAgregarSitioLinea" runat="server" OnClick="btnAgregarSitioLinea_Click" ImageUrl="~/images/addsmall.png" />
                                                                                                            </div>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaFinLinea" runat="server" Text="Fecha Fin" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaFinLinea" runat="server" ReadOnly="false" Enabled="true" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="ceFechaFinLinea" runat="server" TargetControlID="txtFechaFinLinea">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="rfvFechaFinLinea" runat="server" ControlToValidate="txtFechaFinLinea"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosLinea">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblRazonSocialLinea" runat="server" Text="Razón Social" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-14">
                                                                                                        <asp:DropDownList ID="drpRazonSocialLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCentroCosto" runat="server" Text="Centro de Costos" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:HiddenField ID="hdfRazonId" runat="server" ClientIDMode="Static" />
                                                                                                        <asp:TextBox runat="server" ID="txtCentroCosto" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:HiddenField runat="server" ID="hdfCentroCosto" />
                                                                                                        <asp:DropDownList ID="drpCentroCosto" runat="server" AppendDataBoundItems="true" CssClass="form-control" Visible="false">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblFechaRelCenCosto" runat="server" Text="Fecha Relacion-CentroCosto" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaRelacion" runat="server" ReadOnly="false" Enabled="true" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="ceFechaRelacion" runat="server" TargetControlID="txtFechaRelacion">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFechaRelacion"
                                                                                                            ErrorMessage="*" InitialValue="Seleccionar" ValidationGroup="upDatosLinea">
                                                                                                        </asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblTipoEquipoLinea" runat="server" Text="Tipo Equipo" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList ID="drpTipoEquipoLinea" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label runat="server" ID="lblModelo" Text="Modelo Equipo" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:DropDownList runat="server" ID="drpModeloEquipo" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                            <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFinPlanLinea" runat="server" Text="Fin Plan" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFinPlanLinea" runat="server" ReadOnly="false" Enabled="false" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="ceFinPlanLinea" runat="server" TargetControlID="txtFinPlanLinea">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblFechaActivacionLinea" runat="server" Text="Fecha Activación" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtFechaActivacionLinea" runat="server" ReadOnly="false" Enabled="false" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:CalendarExtender ID="ceFechaActivacionLinea" runat="server" TargetControlID="txtFechaActivacionLinea">
                                                                                                        </asp:CalendarExtender>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblPlanLinea" runat="server" Text="Plan (Factura)" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtPlanLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group" runat="server" visible="false">
                                                                                                    <asp:Label ID="lblModeloEqLinea" runat="server" Text="Modelo Equipo" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtModeloEqLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCargoFijoLinea" runat="server" Text="Cargo fijo" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <asp:TextBox ID="txtCargoFijo" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                    </div>
                                                                                                    <div class="col-sm-1">
                                                                                                        <asp:RegularExpressionValidator ID="revCargoFijo" runat="server" ControlToValidate="txtCargoFijo"
                                                                                                            ValidationExpression="^(-)?\d+(\.\d).{0,6}?$" ErrorMessage="*" InitialValue="Seleccionar"
                                                                                                            ValidationGroup="upDatosLinea"></asp:RegularExpressionValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblCategoriaAsignacion" runat="server" Text="Categoría Asignación" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <div class="input-group">
                                                                                                            <asp:DropDownList runat="server" ID="drpCategoriaAsignacion" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                            </asp:DropDownList>
                                                                                                            <div class="input-group-btn">
                                                                                                                <asp:ImageButton ID="btnAgregarCatAsig" runat="server" OnClick="ImageButton1_Click" ImageUrl="~/images/addsmall.png" />
                                                                                                            </div>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblTipoAsignacion" runat="server" Text="Tipo Asignación" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                    <div class="col-sm-7">
                                                                                                        <div class="input-group">
                                                                                                            <asp:DropDownList runat="server" ID="drpTipoAsignacion" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                                <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                            </asp:DropDownList>
                                                                                                            <div class="input-group-btn">
                                                                                                                <asp:ImageButton ID="btnAgregaTipoAsig" runat="server" OnClick="btnAgregaTipoAsig_Click" ImageUrl="~/images/addsmall.png" />
                                                                                                            </div>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>


                                                                                            <!--RM 20190407 Elementos Penta FCA-->
                                                                                            <div id="divPentaSAPCostCenterPopUp" class="form-group" runat="server" visible="false">
                                                                                                <asp:Label ID="lblPentaSAPCostCenterPopUp" runat="server" Text="PentaSAP Cost Center" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                <div class="col-sm-7">
                                                                                                    <asp:DropDownList ID="ddlPentaSAPCostCenterPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                        <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                    </asp:DropDownList>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div id="divPentaSAPFAPopUp" class="form-group" runat="server" visible="false">
                                                                                                <asp:Label ID="lblPentaSAPFAPopUp" runat="server" Text="PentaSAP FA" CssClass="col-sm-4 control-label"></asp:Label>
                                                                                                <div class="col-sm-7">
                                                                                                    <asp:DropDownList ID="ddlPentaSAPFAPopUp" runat="server" AppendDataBoundItems="true" CssClass="form-control">
                                                                                                        <asp:ListItem Text="-- Selecciona uno --" Value="" />
                                                                                                    </asp:DropDownList>
                                                                                                </div>
                                                                                                <div id="divPopUpBR" runat="server" visible="false">
                                                                                                    <br />
                                                                                                    <br />
                                                                                                    <br />
                                                                                                    <br />
                                                                                                </div>
                                                                                                <!---->


                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="row">
                                                                                            <div class="col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblComentarios" runat="server" Text="Comentarios:" CssClass="col-sm-2 control-label"></asp:Label>
                                                                                                    <div class="col-sm-10">
                                                                                                        <asp:TextBox runat="server" ID="txtComentarios" TextMode="multiline" class="form-control" Rows="5"></asp:TextBox>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>

                                                                                        </div>
                                                                                        <div class="row">
                                                                                            <div class="col-md-6 col-sm-6">
                                                                                                <div class="form-group">
                                                                                                    <div class="col-sm-4">
                                                                                                        <asp:CheckBox ID="cbTelularLinea" runat="server" Checked="False" Text="Telular" CssClass="checkbox-inline" />
                                                                                                    </div>
                                                                                                    <div class="col-sm-4">
                                                                                                        <asp:CheckBox ID="cbTarjetaVPNetLinea" runat="server" Checked="False" Text="Tarjeta VPNet" CssClass="checkbox-inline" />
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="col-md-6 col-sm-6">
                                                                                                <div class="form-group">
                                                                                                    <div class="col-sm-4">
                                                                                                        <asp:CheckBox ID="cbNoPublicableLinea" runat="server" Checked="False" Text="No Publicable" CssClass="checkbox-inline" />
                                                                                                    </div>
                                                                                                    <div class="col-sm-4">
                                                                                                        <asp:CheckBox ID="cbConmutadaLinea" runat="server" Checked="False" Text="Conmutada" CssClass="checkbox-inline" />
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnGuardarLinea" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnGuardar_PopUpLinea" UseSubmitBehavior="false" OnClientClick="this.disabled='true';" />
                                                                                    <asp:Button ID="btnCancelarLinea" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" OnClientClick="return Hidepopup()" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkFakeLinea" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpeLinea" runat="server" DropShadow="false" PopupControlID="pnlAddEditLinea"
                                                                        TargetControlID="lnkFakeLinea" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalEditLinea">
                                                                    </asp:ModalPopupExtender>

                                                                    <!--Modal PopUp para agregar un nuevo Tipo -->
                                                                    <asp:Panel ID="pnlAddTipoAsignacion" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-md">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label ID="lblTituloPopUpTipo" runat="server" Text="Tipo Asignación"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalNewTipo"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body">
                                                                                    <div class="row">
                                                                                        <div id="NuevoTipoAsignacion" class="form-horizontal" role="form">
                                                                                            <div class="col-md-12 col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="lblAsignacionTipo" runat="server" Text="Tipo Asignación:" CssClass="col-sm-2 control-label"></asp:Label>
                                                                                                    <div class="col-sm-10">
                                                                                                        <asp:TextBox ID="txtTipoAsigacion" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Capture el Tipo Asignación"
                                                                                                            ControlToValidate="txtTipoAsigacion" Display="Dynamic" ValidationGroup="upDatosLinea">*</asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnAgregarTipo" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnAgregarTipo_Click" />
                                                                                    <asp:Button ID="btnCancelarTipo" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <asp:ValidationSummary ID="ValSumAddTipo" runat="server" />
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkBtnFakeAddTipo" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpeAddTipo" runat="server" DropShadow="false" PopupControlID="pnlAddTipoAsignacion"
                                                                        TargetControlID="lnkBtnFakeAddTipo" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalNewTipo">
                                                                    </asp:ModalPopupExtender>

                                                                    <!--Modal PopUp para agregar una nuevo Categoria -->
                                                                    <asp:Panel ID="pnlAddCategoriaAsignacion" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-md">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label ID="lblCategoria" runat="server" Text="Categoría Asignación"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalNewCategoria"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body">
                                                                                    <div class="row">
                                                                                        <div id="NuevaCategoria" class="form-horizontal" role="form">
                                                                                            <div class="col-md-12 col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="Label6" runat="server" Text="Categoría Asignació:" CssClass="col-sm-2 control-label"></asp:Label>
                                                                                                    <div class="col-sm-10">
                                                                                                        <asp:TextBox ID="txtCategoriaAsig" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Capture la descripcion de la Categoria"
                                                                                                            ControlToValidate="txtCategoriaAsig" Display="Dynamic" ValidationGroup="upDatosLinea">*</asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnAgregarCategoria" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnAgregarCategoria_Click" />
                                                                                    <asp:Button ID="btnCancelarCategoria" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <asp:ValidationSummary ID="ValSumAddCategoria" runat="server" />
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkBtnFakeAddCategoria" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpAddCategoria" runat="server" DropShadow="false" PopupControlID="pnlAddCategoriaAsignacion"
                                                                        TargetControlID="lnkBtnFakeAddCategoria" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalNewCategoria">
                                                                    </asp:ModalPopupExtender>

                                                                    <!-- Agregar Un nuev sitio para la linea.-->
                                                                    <asp:Panel ID="pnlAddUbicacionLinea" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-md">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label ID="lblSitioLineas" runat="server" Text="Sitio - Linea"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalNewSitioLinea"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body">
                                                                                    <div class="row">
                                                                                        <div id="SitioLineas" class="form-horizontal" role="form">
                                                                                            <div class="col-md-12 col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="Label4" runat="server" Text="Sitio - Linea:" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:TextBox ID="txtSitioLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Capture la descripcion del Sitio"
                                                                                                            ControlToValidate="txtSitioLinea" Display="Dynamic" ValidationGroup="upDatosLinea">*</asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnAgregarSitioLine" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnAgregarSitioLine_Click" />
                                                                                    <asp:Button ID="btnCancelarSitioLine" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <asp:ValidationSummary ID="ValSumAddSitioLinea" runat="server" />
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkBtnFakeAddSitioLinea" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpAddSitioLinea" runat="server" DropShadow="false" PopupControlID="pnlAddUbicacionLinea"
                                                                        TargetControlID="lnkBtnFakeAddSitioLinea" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarModalNewSitioLinea">
                                                                    </asp:ModalPopupExtender>


                                                                    <!-- Agregar un area Nueva.-->
                                                                    <asp:Panel ID="pnlAddAreaLinea" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                                                        <div class="rule"></div>
                                                                        <div class="modal-dialog modal-md">
                                                                            <div class="modal-content">
                                                                                <div class="modal-header">
                                                                                    <asp:Label ID="lblAreaLinea" runat="server" Text="Area"></asp:Label>
                                                                                    <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarModalNewAreaLinea"><i class="fas fa-times"></i></button>
                                                                                </div>
                                                                                <div class="modal-body">
                                                                                    <div class="row">
                                                                                        <div id="AreaLinea" class="form-horizontal" role="form">
                                                                                            <div class="col-md-12 col-sm-12">
                                                                                                <div class="form-group">
                                                                                                    <asp:Label ID="Label5" runat="server" Text="Area" CssClass="col-sm-3 control-label"></asp:Label>
                                                                                                    <div class="col-sm-8">
                                                                                                        <asp:TextBox ID="txtAddAreaLinea" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Capture la descripcion del Area"
                                                                                                            ControlToValidate="txtAddAreaLinea" Display="Dynamic" ValidationGroup="upDatosLinea">*</asp:RequiredFieldValidator>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="modal-footer">
                                                                                    <asp:Button ID="btnAgregarAreaLine" runat="server" Text="Guardar" CssClass="btn btn-keytia-sm" OnClick="btnAgregarAreaLine_Click" />
                                                                                    <asp:Button ID="btnCancelarAreaLine" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <asp:ValidationSummary ID="ValSumAddAreaLinea" runat="server" />
                                                                    </asp:Panel>
                                                                    <asp:LinkButton ID="lnkBtnFakeAddAreaLinea" runat="server"></asp:LinkButton>
                                                                    <asp:ModalPopupExtender ID="mpAddAreaLinea" runat="server" DropShadow="false" PopupControlID="pnlAddAreaLinea"
                                                                        TargetControlID="lnkBtnFakeAddAreaLinea" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCancelarAreaLine">
                                                                    </asp:ModalPopupExtender>

                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <asp:UpdateProgress runat="server" ID="ProcesandoAltaLinea" AssociatedUpdatePanelID="UpDatosLinea">
                                                                <ProgressTemplate>
                                                                    <div class="modalProgress">
                                                                        <div class="centerProgress">
                                                                            <asp:Image runat="server" ID="imgLinea" ImageUrl="~/images/loader2.gif" />
                                                                        </div>
                                                                    </div>
                                                                </ProgressTemplate>
                                                            </asp:UpdateProgress>
                                                        </asp:Panel>

                                                        <!--Comentarios-->
                                                        <asp:Panel ID="pComentarios" runat="server" CssClass="row">
                                                            <div class="col-md-12 col-sm-12">
                                                                <div class="portlet solid bordered">
                                                                    <div class="portlet-title">
                                                                        <div class="caption">
                                                                            <i class="icon-bar-chart font-dark hide"></i>
                                                                            <span class="caption-subject titlePortletKeytia">Cometarios</span>
                                                                        </div>
                                                                        <div class="actions">
                                                                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepComentariosCollapse" aria-expanded="true" aria-controls="RepComentariosCollapse"><i class="far fa-minus-square"></i></button>
                                                                        </div>
                                                                    </div>
                                                                    <div class="portlet-body">
                                                                        <div id="RepComentariosCollapse" class="form-horizontal" role="form">
                                                                            <div class="row">
                                                                                <div class="col-md-6 col-sm-6">
                                                                                    <asp:Label runat="server" CssClass="control-label">Comentarios del administrador:</asp:Label>
                                                                                    <asp:TextBox ID="txtComentariosAdmin" runat="server" TextMode="MultiLine" Height="50px" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                                <div class="col-md-6 col-sm-6">
                                                                                    <asp:Label runat="server" CssClass="control-label">Comentarios del empleado:</asp:Label>
                                                                                    <asp:TextBox ID="txtComenariosEmple" runat="server" TextMode="MultiLine" Height="50px" CssClass="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <div class="row">
                                                                                <div class="col-md-12 col-sm-12">
                                                                                    <br />
                                                                                    <asp:LinkButton ID="lbtnGuardarComentAdmin" runat="server" OnClick="lbtnGuardarComentAdmin_Click" Text="Guardar Comentarios" CssClass="btn btn-keytia-sm" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Panel ID="tblFechasCC" runat="server" CssClass="row">
                                                            <div class="col-md-12 col-sm-12">
                                                                <div class="portlet solid bordered">
                                                                    <br />
                                                                    <div class="form-horizontal" role="form">
                                                                        <div class="row">
                                                                            <div class="col-md-6 col-sm-6">
                                                                                <div class="form-group">
                                                                                    <asp:Label runat="server" CssClass="col-sm-4 control-label">Última modificación:</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:TextBox ID="txtUltimaMod" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="form-group">
                                                                                    <asp:Label runat="server" CssClass="col-sm-4 control-label">Último envío</asp:Label>
                                                                                    <div class="col-sm-8">
                                                                                        <asp:TextBox ID="txtUltimoEnvio" runat="server" ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="col-md-6 col-sm-6">
                                                                                <div class="form-group">
                                                                                    <div class="col-sm-12">
                                                                                        <asp:LinkButton ID="btnEnviarCCustodiaEmple" runat="server" OnClick="btnEnviarCCustodiaEmple_Click" Text="Enviar carta custodia al empleado" CssClass="btn btn-keytia-sm pull-right" />
                                                                                    </div>
                                                                                </div>
                                                                                <div class="form-group">
                                                                                    <div class="col-sm-12">
                                                                                        <asp:LinkButton ID="btnCambiarEstatusPte" runat="server" OnClick="btnCambiarEstatusPte_Click" Text="Cambiar a estatus PENDIENTE" CssClass="btn btn-keytia-sm pull-right" />
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>

                                                        <!--Carta Custodia Emple-->
                                                        <asp:Table ID="tblEmpleCCust" runat="server" Width="100%" Visible="False">
                                                            <asp:TableRow ID="trEmpleCCust1" runat="server">
                                                                <asp:TableCell ID="tcEmpleCCust1" runat="server" HorizontalAlign="Right">
                                                                    <asp:Button ID="btnAceptarCCust" runat="server" Text="Aceptar" OnClick="btnAceptarCCust_Click" CssClass="btn btn-keytia-sm" />
                                                                </asp:TableCell>
                                                                <asp:TableCell ID="tcEmpleCCust2" runat="server" HorizontalAlign="Left">
                                                                    <asp:Button ID="btnRechazarCCust" runat="server" Text="Rechazar" OnClick="btnRechazarCCust_Click" CssClass="btn btn-keytia-sm" />
                                                                </asp:TableCell>
                                                            </asp:TableRow>
                                                        </asp:Table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>

                <!--Exportar a PDF-->
                <asp:ImageButton ID="imgbPDFExport" runat="server" ImageUrl="~/images/adobe-pdf-logo.png"
                    Width="4%" OnClick="imgbPDFExport_Click" />
                <asp:AlwaysVisibleControlExtender ID="avcePDFExport" runat="server" TargetControlID="imgbPDFExport"
                    VerticalSide="Top" VerticalOffset="68" HorizontalSide="Right" HorizontalOffset="20" ScrollEffectDuration=".1" />

                <!--Modal PopUp para confirmar envio-->
                <asp:Panel ID="pnlConfirmarEnvio" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lblTituloModalMsn" runat="server" Text="Confirmación de envío"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarConfirmaEnvio"><i class="fas fa-times"></i></button>
                            </div>
                            <div class="modal-body">
                                <asp:Label Font-Bold="true" ID="Label1" runat="server" Text="Esta carta custodia ya ha sido enviada. ¿Desea volver a enviarla?"></asp:Label>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnCancelarEnvioCCust" runat="server" Text="Cancelar" CssClass="btn btn-keytia-sm" OnClientClick="return Hidepopup()" />
                                <asp:Button ID="btnAceptarEnvioCCust" runat="server" Text="Aceptar" CssClass="btn btn-keytia-sm" OnClick="btnAceptarEnvioCCust_ConfEnvio" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeConfirmPopup" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeConfirmEnvio" runat="server" DropShadow="false" PopupControlID="pnlConfirmarEnvio"
                    TargetControlID="lnkFakeConfirmPopup" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarConfirmaEnvio">
                </asp:ModalPopupExtender>

                <!--Modal PopUp para aviso de empleado en aceptar o rechazar ccustodia-->
                <asp:Panel ID="pnlNotificaEmple" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="Label2" runat="server" Text="Carta custodia"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarAceptaORechaza"><i class="fas fa-times"></i></button>
                            </div>
                            <div class="modal-body">
                                <asp:Label Font-Bold="true" ID="lblMensajeNotificaEmple1" runat="server"></asp:Label>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnNotificaEmpleCCust" runat="server" Text="Aceptar" CssClass="btn btn-keytia-sm" OnClientClick="return Hidepopup()" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeNotificaEmple" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeNotificaEmple" runat="server" DropShadow="false" PopupControlID="pnlNotificaEmple"
                    TargetControlID="lnkFakeNotificaEmple" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarAceptaORechaza">
                </asp:ModalPopupExtender>

                <!--Modal Popup para la confirmación de la baja dele empleado-->
                <asp:Panel ID="pnlBajaEmple" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label runat="server" Text="Baja de Empleado"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarBajaEmple"><i class="fas fa-times"></i></button>
                            </div>
                            <div class="modal-body">
                                <div class="row">
                                    <div class="form-horizontal" role="form">
                                        <div class="col-md-12 col-sm-12">
                                            <asp:Label ID="lcEmpleEnBajaMsj" runat="server"></asp:Label>
                                            <div class="form-group">
                                                <asp:Label ID="lblFechaBajaEmple" runat="server" Text="Fecha Fin:" CssClass="col-sm-4 control-label"></asp:Label>
                                                <div class="col-sm-5">
                                                    <asp:TextBox ID="txtFechaBajaEmpleado" runat="server" MaxLength="10" ReadOnly="false" CssClass="form-control"></asp:TextBox>
                                                    <asp:CalendarExtender ID="ceFechaBajaEmple" TargetControlID="txtFechaBajaEmpleado" runat="server">
                                                    </asp:CalendarExtender>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnAceptarBajaEmple" Text="Borrar" runat="server" CssClass="btn btn-keytia-sm" OnClick="btnAceptarBajaEmple_Click" />
                                <asp:Button ID="btnCancelarBajaEmple" Text="Cancelar" runat="server" CssClass="btn btn-keytia-sm" OnClientClick="return Hidepopup()" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeBajaEmple" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeBajaEmple" runat="server" DropShadow="false" PopupControlID="pnlBajaEmple"
                    TargetControlID="lnkFakeBajaEmple" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarBajaEmple">
                </asp:ModalPopupExtender>

                <!--Modal Popup para la reasingacion del jefe inmediato-->
                <asp:Panel ID="pnlReasignaEmple" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                    <div class="rule"></div>
                    <div class="modal-dialog modal-ld">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lblElimEmpleReasignaHeader" runat="server" Text="Eliminar empleado"></asp:Label>
                                <button type="button" runat="server" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarReasignaJefe"><i class="fas fa-times"></i></button>
                            </div>
                            <div class="modal-body">
                                <div class="row">
                                    <div class="col-md-12 col-sm-12">
                                        <asp:Literal ID="lcEmpleReasigna" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="row scrollbar scrollbar-warning thin" style="height: 150px;">
                                    <div class="col-md-12 col-sm-12">
                                        <div class="table-responsive">
                                            <asp:GridView ID="grvEmpleDepende" runat="server" AutoGenerateColumns="false"
                                                HeaderStyle-CssClass="tableHeaderStyle" CssClass="table table-bordered tableDashboard">
                                                <Columns>
                                                    <asp:BoundField HeaderText="No. nómina" DataField="NominaA" HtmlEncode="true" />
                                                    <asp:BoundField HeaderText="Nombre" DataField="NomCompleto" HtmlEncode="true" />
                                                    <asp:BoundField HeaderText="Puesto" DataField="PuestoDesc" HtmlEncode="true" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-horizontal" role="form">
                                        <div class="col-md-12 col-sm-12">
                                            <asp:Label ID="lcEmpleNuevoJefe" runat="server"></asp:Label>
                                            <div class="form-group">
                                                <div class="col-sm-8">
                                                    <asp:DropDownList ID="drpNuevoEmpleResp" AppendDataBoundItems="true" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="-- Seleccione un nuevo jefe --" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnCancelarReasignaEmple" Text="Cancelar" runat="server" OnClientClick="return Hidepopup()"
                                    CssClass="btn btn-keytia-sm" />
                                <asp:Button ID="btnReasignarBajaEmple" Text="Asignar" runat="server" CssClass="btn btn-keytia-sm"
                                    OnClick="btnReasignarBajaEmple_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:LinkButton ID="lnkButtonFakeReasignaEmple" runat="server"></asp:LinkButton>
                <asp:ModalPopupExtender ID="mpeReasingarBajaEmple" runat="server" DropShadow="false"
                    PopupControlID="pnlReasignaEmple" TargetControlID="lnkButtonFakeReasignaEmple"
                    BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarReasignaJefe">
                </asp:ModalPopupExtender>

                <%--NZ: Modal para mensajes--%>
                <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none; z-index: 15001;">
                    <div class="rule"></div>
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lblTituloMensajeGenerico" runat="server" Text="Mensaje"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                            </div>
                            <div class="modal-body">
                                <asp:Label ID="lblBodyMensajeGenerico" runat="server"></asp:Label>
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
            </div>
        </div>
    </div>
</asp:Content>
