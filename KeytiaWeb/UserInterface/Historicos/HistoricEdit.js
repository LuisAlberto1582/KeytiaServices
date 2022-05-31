var HisMsgs = {
    confirm: "",
    confirmTitle: ""
}
var Historico = function(hisID, jsObj) {
    var self = this;
    this.$historic = $(hisID);
    this.$container = this.$historic;
    this.jsObj = jsObj;
    this.editing = false;
    this.confirm = false;
    this.fnVigArray = [];
    this.$ctlIniVigencia = $(hisID).find(".DSODateTimeBox[id$='" + jsObj + "_RegWrapper_dtIniVigencia_dt']");
    this.$ctlFinVigencia = $(hisID).find(".DSODateTimeBox[id$='" + jsObj + "_RegWrapper_dtFinVigencia_dt']");
    this.$ctlIniVigencia.change(function() { try { self.vigenciaChange(); } catch (ex) { } });
    this.$ctlFinVigencia.change(function() { try { self.vigenciaChange(); } catch (ex) { } });

    $(hisID).find("[dataField]").change(function() { self.confirm = true; });
}
Historico.prototype.cancel = function(doPostBack) {
    var self = this;
    if (self.editing && self.confirm) {
        jConfirm(HisMsgs.confirm, HisMsgs.confirmTitle, function(r) {
            if (r) {
                doPostBack();
            }
        });
    }
    else {
        doPostBack();
    }
}
Historico.prototype.catChange = function($ddl) {
    var value = $ddl.val();
    var $mae = this.$historic.find(".DSODropDownList[id$='iCodMaestro_ddl']");
    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/GetHisMaestros",
        data: JSON.stringify({ iCodCatalogo: value }),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $mae[0],
        success: DSOControls.DropDown.Fill,
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnSearchHisReg = function($auto, request, response) {
    var term = request.term;
    var iCodMaestro = this.$historic.find(".DSODropDownList[id$='iCodMaestro_ddl']").val();
    if (iCodMaestro === "null") return;

    var param = { term: term, iCodMaestro: iCodMaestro };
    var options = {
        type: "POST",
        url: $($auto.element).attr("source"),
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $auto.element[0],
        success: function(data, status, xhr) {
            var results = (typeof data.d) == 'string' ?
                            eval('(' + data.d + ')') :
                            data.d;
            response(results);
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnSearchCatReg = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodEntidad = $($auto.element).attr("iCodEntidad");
    var entidadField = $($auto.element).attr("entidadField");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');

    if (entidadField !== undefined) {
        var $searchVal = self.$historic.find(".DSOAutocomplete[keytiaField='" + entidadField + "']").next(".DSOAutocompleteVal");
        iCodEntidad = $searchVal.val();
    }

    if (iCodEntidad === undefined || iCodEntidad === "null") return;

    if (iniVigencia === undefined) {
        iniVigencia = null;
    }

    if (finVigencia === undefined) {
        finVigencia = null;
    }

    var param = { term: term, iCodEntidad: iCodEntidad, iniVigencia: iniVigencia, finVigencia: finVigencia };
    var options = {
        type: "POST",
        url: $($auto.element).attr("source"),
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $auto.element[0],
        success: function(data, status, xhr) {
            var results = (typeof data.d) == 'string' ?
                            eval('(' + data.d + ')') :
                            data.d;
            response(results);
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}

Historico.prototype.fnSearchCatRestricted = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodEntidad = $($auto.element).attr("iCodEntidad");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');

    if (iCodEntidad === undefined || iCodEntidad === "null") return;

    if (iniVigencia === undefined) {
        iniVigencia = null;
    }

    if (finVigencia === undefined) {
        finVigencia = null;
    }

    var param = { term: term, iCodEntidad: iCodEntidad, iniVigencia: iniVigencia, finVigencia: finVigencia };
    var options = {
        type: "POST",
        url: $($auto.element).attr("source"),
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $auto.element[0],
        success: function(data, status, xhr) {
            var results = (typeof data.d) == 'string' ?
                            eval('(' + data.d + ')') :
                            data.d;
            response(results);
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnSearchCatFiltered = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodEntidad = $($auto.element).attr("iCodEntidad");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');
	var keytiaFilter = $($auto.element).attr("keytiaFilter");
	
    if (iCodEntidad === undefined || iCodEntidad === "null") return;

    if (iniVigencia === undefined) {
        iniVigencia = null;
    }

    if (finVigencia === undefined) {
        finVigencia = null;
    }

    var param = { term: term, iCodEntidad: iCodEntidad, keytiaFilter: keytiaFilter, iniVigencia: iniVigencia, finVigencia: finVigencia };
    var options = {
        type: "POST",
        url: $($auto.element).attr("source"),
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $auto.element[0],
        success: function(data, status, xhr) {
            var results = (typeof data.d) == 'string' ?
                            eval('(' + data.d + ')') :
                            data.d;
            response(results);
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnInitGrids = function() {
    this.$historic.find(".DSOGrid[id]").each(function() {
        var $grid = $(this).dataTable({ bRetrieve: true });
        var wrapperID = "#" + $grid.attr("id") + "_wrapper";
        var $expandable = $grid.closest(".DSOExpandable");
        var maxwidth = $expandable.width();
        $(wrapperID).css("max-width", maxwidth);
        $grid.fnAdjustColumnSizing();
    });
}
Historico.prototype.fnInitGrid = function(grID) {
    var $grid = this.$historic.find(grID).dataTable({ bRetrieve: true });
    $grid.fnAdjustColumnSizing();
}
Historico.prototype.filtrar = function() {
    var $grid = this.$historic.find(".DSOGrid[id$='HisGrid_Grid']").dataTable({ bRetrieve: true });
    $grid.fnDraw();
}
Historico.prototype.limpiarFiltro = function() {
    this.$historic.find("[dataFilter]").each(function() {
        $(this).val("");
    });
    var $filterWrapper = this.$historic.find(".dataTables_filter[id$='HisGrid_Grid_filter']");
    var $globalFilter = $filterWrapper.find("input");
    $globalFilter.val("");
    $globalFilter.trigger("keyup.DT");
}
Historico.prototype.fnServerData = function($grid, sSource, aoData, fnCallback) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestro = this.$historic.find(".DSODropDownList[id$='iCodMaestro_ddl']").val();
    var iCodEntidad = this.$historic.find(".DSODropDownList[id$='iCodEntidad_ddl']").val();

    if (self.refreshTimeout !== undefined) {
        clearInterval(self.refreshTimeout);
    }

    if (!iCodMaestro) {
        iCodMaestro = self.iCodMaestro;
    }

    if (!iCodEntidad) {
        iCodEntidad = self.iCodEntidad;
    }

    var aoColumns = $grid.fnSettings().aoColumns;
    var idx;
    var maxIdx = aoColumns.length;
    var col;
    var $filtro;
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        $filtro = self.$historic.find("[dataFilter='" + col.sName + "']");
        if ($filtro.length > 0) {
            request.sSearch[idx] = $filtro.val();
        }
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        iCodEntidad: parseInt(iCodEntidad + ".0"),
        iCodMaestro: parseInt(iCodMaestro + ".0")
    };

    var options = {
        type: "POST",
        url: sSource,
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function(data, textStatus, jqXHR) {
            var response = data.d;
            fnCallback(response);

            $grid.css("width", $grid.css("width"));
            $header.css("width", $grid.css("width"));

            if (self.refreshGrid !== undefined && self.refreshGrid > 0) {
                self.refreshTimeout = setTimeout(function() { $grid.fnAdjustColumnSizing(); }, self.refreshGrid);
            }
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnServerVisHistoricoParam = function($grid, sSource, aoData, fnCallback, parametros) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestro = this.$historic.find(".DSODropDownList[id$='iCodMaestro_ddl']").val();
    var iCodEntidad = this.$historic.find(".DSODropDownList[id$='iCodEntidad_ddl']").val();

    if (self.refreshTimeout !== undefined) {
        clearInterval(self.refreshTimeout);
    }

    if (!iCodMaestro) {
        iCodMaestro = self.iCodMaestro;
    }

    if (!iCodEntidad) {
        iCodEntidad = self.iCodEntidad;
    }

    var aoColumns = $grid.fnSettings().aoColumns;
    var idx;
    var maxIdx = aoColumns.length;
    var col;
    var $filtro;
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        $filtro = self.$historic.find("[dataFilter='" + col.sName + "']");
        if ($filtro.length > 0) {
            request.sSearch[idx] = $filtro.val();
        }
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        iCodEntidad: parseInt(iCodEntidad + ".0"),
        iCodMaestro: parseInt(iCodMaestro + ".0"),
        parametros: parametros
    };

    var options = {
        type: "POST",
        url: sSource,
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function(data, textStatus, jqXHR) {
            var response = data.d;
            fnCallback(response);

            $grid.css("width", $grid.css("width"));
            $header.css("width", $grid.css("width"));

            if (self.refreshGrid !== undefined && self.refreshGrid > 0) {
                self.refreshTimeout = setTimeout(function() { $grid.fnAdjustColumnSizing(); }, self.refreshGrid);
            }
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnRender = function(obj, imgClass, imgSrc, sdoPostBack) {
    var iCodRegistro = obj.aData[0];
    sdoPostBack = sdoPostBack.replace("{0}", iCodRegistro);
    var ret = "<a href=\"javascript:" + sdoPostBack + ";\"><img class='" + imgClass + "' src='" + imgSrc + "'></a>";
    return ret;
}
Historico.prototype.fnServerHistoricos = function($grid, sSource, aoData, fnCallback, iCodEntidad, iCodMaestro, parametros, dataFilter) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);

    if (!dataFilter) {
        var aoColumns = $grid.fnSettings().aoColumns;
        var idx;
        var maxIdx = aoColumns.length;
        var col;
        var $filtro;
        for (idx = 0; idx < maxIdx; idx++) {
            col = aoColumns[idx];
            $filtro = self.$historic.find("[" + dataFilter + "='" + col.sName + "']");
            if ($filtro.length > 0) {
                request.sSearch[idx] = $filtro.val();
            }
        }
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        iCodEntidad: parseInt(iCodEntidad + ".0"),
        iCodMaestro: parseInt(iCodMaestro + ".0"),
        parametros: parametros
    };

    var options = {
        type: "POST",
        url: sSource,
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function(data, textStatus, jqXHR) {
            var response = data.d;
            fnCallback(response);

            $grid.css("width", $grid.css("width"));
            $header.css("width", $grid.css("width"));
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnServerHistoricosParam = function($grid, sSource, aoData, fnCallback, iCodEntidad, iCodMaestro) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");
    var parametros = [];
    self.$historic.find("[dataField][keytiaField]").each(function() {
        var $ctl = $(this);
        var parametro = {};
        parametro.name = $ctl.attr("keytiaField");
        if ($ctl.hasClass("DSOTextBox") || $ctl.hasClass("DSODropDownList")) {
            parametro.value = $ctl.val();
        }
        else if ($ctl.hasClass("DSODateTimeBox")) {
            parametro.value = $ctl.datetimepicker('getDate');
        }
        else if ($ctl.hasClass("DSOAutocomplete")) {
            parametro.value = $($ctl.attr('textValueID')).val();
        }
        else if ($ctl.hasClass("DSOFlags")) {
            parametro.value = $ctl.prev(".DSOFlagsVal").val(); ;
        }
        parametros.push(parametro);
    });

    var param = {
        gsRequest: request,
        iCodEntidad: parseInt(iCodEntidad + ".0"),
        iCodMaestro: parseInt(iCodMaestro + ".0"),
        parametros: parametros
    };

    var options = {
        type: "POST",
        url: sSource,
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function(data, textStatus, jqXHR) {
            var response = data.d;
            fnCallback(response);

            $grid.css("width", $grid.css("width"));
            $header.css("width", $grid.css("width"));
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
Historico.prototype.fnSubHisInitComplete = function($grid) {
    var $btnAdd = $($grid.attr("buttonAdd"));
    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $toolbar = $(wrapperID).find(".dataTables_length");
    $toolbar.prepend($btnAdd);

    var $expandable = $grid.closest(".DSOExpandable");
    var maxwidth = $expandable.width();
    if (maxwidth > 0) {
        $(wrapperID).css("max-width", maxwidth);
    }
    else {
        $(wrapperID).css("max-width", "100%");
    }
    $grid.fnAdjustColumnSizing();
    this.$grid = $grid;
}
Historico.prototype.vigenciaChange = function() {
    var self = this;
    if (self.vigenciaTimeout !== undefined) {
        clearInterval(self.vigenciaTimeout);
    }
    self.vigenciaTimeout = setTimeout(function() {
        var idX = 0;
        var maxidX = self.fnVigArray.length;
        for (idX = 0; idX < maxidX; idX++)
            self.fnVigArray[idX].call(self);
    }, 100);
}