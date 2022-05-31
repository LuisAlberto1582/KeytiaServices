var RepEstandar = function(repID, iCodReporte) {
    var self = this;
    this.$container = $(repID);
    this.$ctlIniVigencia = $(repID).find(".DSODateTimeBox[keytiaField='FechaIniRep']");
    this.$ctlFinVigencia = $(repID).find(".DSODateTimeBox[keytiaField='FechaFinRep']");
    this.$ctlIniVigencia.change(function() { self.vigenciaChange(); });
    this.$ctlFinVigencia.change(function() { self.vigenciaChange(); });
    this.iCodReporte = iCodReporte;
    this.parametros = [];
    this.fnVigArray = [];
}
RepEstandar.prototype.fnSearchCatReg = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodEntidad = $($auto.element).attr("iCodEntidad");
    var entidadField = $($auto.element).attr("entidadField");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');

    if (entidadField !== undefined) {
        var $searchVal = self.$container.find(".DSOAutocomplete[keytiaField='" + entidadField + "']").next(".DSOAutocompleteVal");
        iCodEntidad = $searchVal.val();
    }

    if (iCodEntidad === undefined || iCodEntidad === "null") return;

    if (iniVigencia === undefined || self.$ctlIniVigencia.length === 0) {
        iniVigencia = null;
    }

    if (finVigencia === undefined || self.$ctlFinVigencia.length === 0) {
        finVigencia = null;
    }

    if (finVigencia !== null && finVigencia.setDate !== undefined) {
        finVigencia.setDate(finVigencia.getDate() + 1);
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
RepEstandar.prototype.fnSearchCatRestricted = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodEntidad = $($auto.element).attr("iCodEntidad");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');

    if (iCodEntidad === undefined || iCodEntidad === "null") return;

    if (iniVigencia === undefined || self.$ctlIniVigencia.length === 0) {
        iniVigencia = null;
    }

    if (finVigencia === undefined || self.$ctlFinVigencia.length === 0) {
        finVigencia = null;
    }

    if (finVigencia !== null && finVigencia.setDate !== undefined) {
        finVigencia.setDate(finVigencia.getDate() + 1);
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
RepEstandar.prototype.fnSearchCatFiltered = function($auto, request, response) {
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
RepEstandar.prototype.fnReporte = function($grid, sSource, aoData, fnCallback) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);

    var param = {
        gsRequest: request,
        iCodReporte: self.iCodReporte,
        iNumeroRegistros: self.iNumeroRegistros,
        parametros: self.parametros
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
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
RepEstandar.prototype.fnRuta = function(obj, iCodCampoRuta, iCodCampoRutaAd, sdoPostBack) {
    var rows = [];
    var row = {};
    var idx;
    var aData = obj.aData;
    var aoColumns = obj.oSettings.aoColumns;
    var maxIdx = aoColumns.length;

    for (idx = 0; idx < maxIdx; idx++) {
        row[aoColumns[idx].sName] = aData[idx];
    }
    rows.push(row);
    var jsonData = JSON.stringify(rows);

    sdoPostBack = sdoPostBack.replace("{cmp}", iCodCampoRuta);
    sdoPostBack = sdoPostBack.replace("{cmpAd}", iCodCampoRutaAd);
    sdoPostBack = sdoPostBack.replace("{obj}", escape(jsonData));

    if (aData[aData.length - 2] === 1) {
        return aData[obj.iDataColumn];
    }
    else {
        return "<a href=\"javascript:" + sdoPostBack + ";\">" + aData[obj.iDataColumn] + "</a>";
    }
}
RepEstandar.prototype.fnRutaXY = function(obj, iCodCampoRuta, iCodCampoRutaAd, iRowX, sdoPostBack) {
    var rows = [];
    var row = {};
    var idx;
    var aData = obj.aData;
    var aoColumns = obj.oSettings.aoColumns;
    var maxIdx = aoColumns.length;

    for (idx = 0; idx < maxIdx; idx++) {
        row[aoColumns[idx].sName] = aData[idx];
    }
    rows.push(row);
    var jsonData = JSON.stringify(rows);

    sdoPostBack = sdoPostBack.replace("{cmp}", iCodCampoRuta);
    sdoPostBack = sdoPostBack.replace("{cmpAd}", iCodCampoRutaAd);
    sdoPostBack = sdoPostBack.replace("{rowX}", iRowX);
    sdoPostBack = sdoPostBack.replace("{obj}", escape(jsonData));

    if (aData[aData.length - 2] === 1) {
        return aData[obj.iDataColumn];
    }
    else {
        return "<a href=\"javascript:" + sdoPostBack + ";\">" + aData[obj.iDataColumn] + "</a>";
    }
}
RepEstandar.prototype.fnAdjustColumnSizing = function(grID) {
    var $grid = $(grID).dataTable({bRetrieve:true});
    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $expandable = $grid.closest(".DSOExpandable");
    var maxwidth = $expandable.width();
    $(wrapperID).css("max-width", maxwidth);
    $grid.fnAdjustColumnSizing();
}
RepEstandar.prototype.fnRowCallBack = function(nRow, aData, iDisplayIndex, iDisplayIndexFull) {
    if (aData[aData.length - 1] !== undefined
    && aData[aData.length - 1] !== "undefined" 
    && aData[aData.length - 1] !== "") {
        $(nRow).addClass(aData[aData.length - 1]);
    }
    return nRow;
}
RepEstandar.prototype.fnInitGridsParam = function() {
    var $param = this.$container.find(".ZonaParametros");
    $param.find(".DSOGrid[id]").each(function() {
        var $grid = $(this).dataTable({ bRetrieve: true });
        $grid.fnAdjustColumnSizing();
    });
}
RepEstandar.prototype.vigenciaChange = function() {
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
RepEstandar.prototype.cancelarCorreo = function() {
    var self = this;
    if (self.$btnCancelaCorreo !== undefined) {
        self.$btnCancelaCorreo.click();
    }
}