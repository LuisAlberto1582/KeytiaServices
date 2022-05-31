var RelMsgs = {
    confirm: "",
    confirmTitle: ""
}

var Relacion = function(hisID, grID, wdAddID, wdEditID, wdDelID) {
    var self = this;
    this.$container = $(hisID);
    this.$grid = $(grID);
    this.$gridData = $(grID + "__hidden");
    this.$wdAdd = $(wdAddID);
    this.$wdEdit = $(wdEditID);
    this.$wdDel = $(wdDelID);
    this.name = "";
    this.row = null;
    this.editing = false;
    this.confirm = false;
    this.addedRows = 0;

    $(wdEditID).find("[dataField]").change(function() { self.confirm = true; });
    $(wdDelID).find("[dataField]").change(function() { self.confirm = true; });
    $(wdAddID).find("[dataField]").change(function() { self.confirm = true; });
}
Relacion.prototype.initWindow = function($wd, offset) {
    $wd.attr("posy", offset.top);
    $wd.attr("posx", offset.left);
    DSOControls.LoadContainer.call($wd, this.row);
    DSOControls.Window.Init.call($wd);

    this.confirm = false;
    $wd.showWindow();
}
Relacion.prototype.End = function($wd) {
    $wd.closeWindow();
}
Relacion.prototype.cancel = function($wd) {
    var self = this;
    self.editing = false;
    if (self.confirm) {
        jConfirm(RelMsgs.confirm, RelMsgs.confirmTitle, function(r) {
            if (!r) {
                self.editing = true;
                DSOControls.Window.Init.call($wd);
                $wd.showWindow();
            }
        });
    }
}
Relacion.prototype.setRow = function(aData, aoColumns) {
    var self = this;
    var maxIdx = aoColumns.length;
    var idx;
    var col;
    self.row = {};
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        self.row[col.sName] = aData[idx];
    }
    var date = (typeof self.row["dtFecUltAct"] == 'string') && self.row["dtFecUltAct"] !== "" ?
                            new Date(parseInt(self.row["dtFecUltAct"].substr(6))) :
                            self.row["dtFecUltAct"];
    self.row["dtFecUltAct"] = date;
    self.row["iCodRegistro"] = parseInt(self.row["iCodRegistro"]);
}
Relacion.prototype.updateRow = function($wd) {
    var row = this.row;
    $wd.find("[dataField]").each(function() {
        var $ctl = $(this);
        var field = $ctl.attr("dataField");
        var fieldDisplay = field + "Display";

        if ($ctl.hasClass("DSOTextBox")) {
            row[field] = $ctl.val();
        }
        else if ($ctl.hasClass("DSODropDownList")) {
            row[field] = $ctl.val();
            row[fieldDisplay] = $ctl.find("option:selected").text();
        }
        else if ($ctl.hasClass("DSODateTimeBox")) {
            row[field] = $ctl.datetimepicker('getDate');
            row[fieldDisplay] = row[field];
        }
        else if ($ctl.hasClass("DSOAutocomplete")) {
            row[field] = $($ctl.attr("textValueID")).val();
            if (!row[field] || row[field] === "null") {
                row[field] = null;
                row[fieldDisplay] = null;
            }
            else {
                row[fieldDisplay] = $ctl.val();
            }
        }
        else if ($ctl.hasClass("DSOFlags")) {
            row[field] = $ctl.prev(".DSOFlagsVal").val();
        }
    });
    if (row["iCodRegistro"] !== undefined
    && row["iCodRegistro"] !== null
    && row["iCodRegistro"] !== "") {
        row["iCodRegistro"] = parseInt(row["iCodRegistro"]);
    }
}
Relacion.prototype.getGridData = function() {
    var self = this;
    var rows;
    var $gridData = self.$gridData;

    try {
        rows = JSON.parse($gridData.val());
    }
    catch (e) {
        rows = [];
    }
    return rows;
}
Relacion.prototype.updateGridData = function() {
    var self = this;
    var row = self.row;
    var rows = self.getGridData();
    var $gridData = self.$gridData;

    if (self.addedRows === 0) {
        self.addedRows = -rows.length;
    }
    if (!row.iCodRegistro) {
        row["iCodRegistro"] = --self.addedRows;
        rows.push(row);
    }
    else {
        var maxRow = rows.length;
        var idx;
        var bPush = true;
        for (idx = 0; idx < maxRow; idx++) {
            if (rows[idx].iCodRegistro === row.iCodRegistro) {
                rows[idx] = row;
                bPush = false;
                break;
            }
        }
        if (bPush) {
            rows.push(row);
        }
    }
    $gridData.val(JSON.stringify(rows));
}
Relacion.prototype.edit = function(aData) {
    var self = this;
    if (self.editing) return;
    self.editing = true;
    var $grid = self.$grid;
    var offset = $grid.offset();
    var aoColumns = $grid.fnSettings().aoColumns;
    var maxIdx = aoColumns.length;
    var idx;
    var $ctl;
    var $wd;

    if (aData) {
        self.setRow(aData, aoColumns);
        if (self.row["iCodRegistro"] < 0) {
            $wd = self.$wdAdd;
        }
        else {
            $wd = self.$wdEdit;
        }

        for (var field in self.row) {
            $ctl = $wd.find(".DSOAutocomplete[dataField='" + field + "']");
            if (self.row["iCodRegistro"] < 0) {
                $ctl.attr("disabled", false);
            }
            else {
                $ctl.attr("disabled", true);
            }
        }
    }
    else {
        $wd = self.$wdAdd;
        self.row = {};
        for (idx = 0; idx < maxIdx; idx++) {
            col = aoColumns[idx];
            if (col.sName.indexOf("iFlags") === 0) {
                $ctl = $wd.find(".DSOFlags[dataField='" + col.sName + "']");
                self.row[col.sName] = parseInt($ctl.attr("valorDefault"));
            }
            else {
                self.row[col.sName] = null;
            }
        }
    }

    var colEntidad = $grid.attr("colEntidad");
    if ($grid.attr("iCodCatalogo") !== undefined) {
        var $ctlDes = self.$container.find(".DSOTextBox[dataField='vchDescripcion']:first");
        self.row[colEntidad] = parseInt($grid.attr("iCodCatalogo"));
        self.row[colEntidad + "Display"] = $ctlDes.val();
    }
    $ctl = $wd.find(".DSOAutocomplete[dataField='" + colEntidad + "']");
    $ctl.attr("disabled", true);

    self.initWindow($wd, offset);
}
Relacion.prototype.deleteEdit = function(aData) {
    var self = this;
    if (self.editing) return;

    self.editing = true;
    var $grid = self.$grid;
    var offset = $grid.offset();
    var aoColumns = $grid.fnSettings().aoColumns;
    var $wd = self.$wdDel;
    self.setRow(aData, aoColumns);

    self.initWindow($wd, offset);

    var $ctl = $wd.find(".DSODateTimeBox[dataField='dtFinVigencia']");
    var date = $ctl.datetimepicker("getDate");
    var defaultDate = new Date(2079, 00, 01);
    if ((date > defaultDate) - (date < defaultDate) == 0) {
        date = new Date();
        $ctl.datetimepicker("setDate", date);
    }
}
Relacion.prototype.save = function() {
    var self = this;
    var $wd = self.$wdAdd;
    var $grid = self.$grid;

    self.updateRow($wd);
    self.row["Editar"] = 1;
    self.row["Eliminar"] = 1;
    self.updateGridData();

    self.confirm = false;
    self.End($wd);
    $grid.fnDraw();
}
Relacion.prototype.saveEdit = function() {
    var self = this;
    var $wd = self.$wdEdit;
    var $grid = self.$grid;

    self.updateRow($wd);
    self.updateGridData();

    self.confirm = false;
    self.End($wd);
    $grid.fnDraw();
}
Relacion.prototype.saveDelete = function() {
    var self = this;
    var $wd = self.$wdDel;
    var $grid = self.$grid;
    var $gridData = self.$gridData;

    self.updateRow($wd);
    self.updateGridData();

    var row = self.row;
    var rows = self.getGridData();
    if (row.iCodRegistro < 0 &&
    (!row.dtFinVigencia
    || row.dtFinVigencia < new Date())) {
        var maxRow = rows.length;
        var idx;
        for (idx = 0; idx < maxRow; idx++) {
            if (rows[idx].iCodRegistro === row.iCodRegistro) {
                rows.splice(idx, 1);
                break;
            }
        }
        $gridData.val(JSON.stringify(rows));
    }

    self.confirm = false;
    self.End($wd);
    $grid.fnDraw();
}
Relacion.prototype.fnRender = function(obj, colName, func, imgClass, imgSrc) {
    var jsonData = JSON.stringify(obj.aData);

    var maxIdx = obj.oSettings.aoColumns.length;
    var idx;
    var col;

    for (idx = 0; idx < maxIdx; idx++) {
        if (obj.oSettings.aoColumns[idx].sName === colName) {
            col = idx;
            break;
        }
    }

    var ret = "";

    if (parseInt(obj.aData[col]) === 1) {
        ret = "<a href=\"#nogo\"><img class='" + imgClass + "' src='" + imgSrc + "' onclick=\"" + func + "(eval(unescape('" + escape(jsonData) + "')));\" ></a>";
    }
    return ret;
}
Relacion.prototype.fnInitComplete = function($grid) {
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
Relacion.prototype.fnServerData = function($grid, sSource, aoData, fnCallback) {
    var request = DSOControls.Grid.GetRequest(aoData);
    var $gridData = this.$gridData;
    var $ctlDes = this.$container.find(".DSOTextBox[dataField='vchDescripcion']:first");
    var vchDescripcion = $ctlDes.val();
    if (vchDescripcion === undefined) {
        vchDescripcion = "";
    }

    var param = { gsRequest: request, iCodEntidad: 0, iCodRelacion: 0, iCodCatalogo: -1, jsonData: null, vchDescripcion: vchDescripcion };
    if ($grid.attr("iCodEntidad") !== undefined) {
        param.iCodEntidad = parseInt($grid.attr("iCodEntidad"));
    }
    if ($grid.attr("iCodRelacion") !== undefined) {
        param.iCodRelacion = parseInt($grid.attr("iCodRelacion"));
    }
    if ($grid.attr("iCodCatalogo") !== undefined) {
        param.iCodCatalogo = parseInt($grid.attr("iCodCatalogo"));
    }
    if ($gridData.val() !== undefined && $gridData.val() !== "") {
        param.jsonData = $gridData.val();
    }

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
Relacion.prototype.fnAdjustColumnSizing = function() {
    var $grid = this.$grid;
    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $expandable = $grid.closest(".DSOExpandable");
    var maxwidth = $expandable.width();
    $(wrapperID).css("max-width", maxwidth);
    $grid.fnAdjustColumnSizing();
}
Relacion.prototype.InitAdd = function() {
    var self = this;

    self.editing = true;
    var $grid = self.$grid.dataTable({ bRetrieve: true });
    var aoColumns = $grid.fnSettings().aoColumns;
    var maxIdx = aoColumns.length;
    var idx;
    var $ctl;
    var $wd = self.$wdAdd;

    self.row = {};
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        self.row[col.sName] = null;
    }
    $ctl = $wd.find(".DSOTextBox[dataField='iCodRegistro']");
    if ($ctl.val() !== undefined
    && $ctl.val() !== null
    && $ctl.val() !== "") {
        self.row["iCodRegistro"] = parseInt($ctl.val());
    }

    DSOControls.Window.Init.call($wd);
    $wd.showWindow();
}

var MultiSelect = function(contenedor, grID, chkID) {
    var self = this;
    this.$container = contenedor.$container;
    this.$grid = $(grID);
    this.$gridData = $(grID + "__hidden");
    this.$ctlIniVigencia = contenedor.$ctlIniVigencia;
    this.$ctlFinVigencia = contenedor.$ctlFinVigencia;
    this.$chk = $(chkID);
    this.deSelAll = false;
    this.enableField = parseInt($(grID).attr("enableField")) == 1 ? true : false;
}
MultiSelect.prototype.getGridData = Relacion.prototype.getGridData;

MultiSelect.prototype.updateGridData = function(rows) {
    var self = this;
    var $gridData = self.$gridData;
    if (rows === null || rows === undefined) {
        $gridData.val('');
    }
    else {
        $gridData.val(JSON.stringify(rows));
    }
}
MultiSelect.prototype.fnServerData = function($grid, sSource, aoData, fnCallback) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);
    var $gridData = this.$gridData;
    var $chk = this.$chk;
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');

    if (iniVigencia === undefined || self.$ctlIniVigencia.length === 0) {
        iniVigencia = null;
    }

    if (finVigencia === undefined || self.$ctlFinVigencia.length === 0) {
        finVigencia = null;
    }

    if (self.$container.hasClass("ReporteEstandar")
    && finVigencia !== null && finVigencia.setDate !== undefined) {
        finVigencia.setDate(finVigencia.getDate() + 1);
    }

    var param = { gsRequest: request, iCodEntidad: 0, bSelTodos: 0, jsonSeleccionados: null, enableField: (self.enableField ? 1 : 0), iniVigencia: iniVigencia, finVigencia: finVigencia };
    if ($grid.attr("iCodEntidad") !== undefined) {
        param.iCodEntidad = parseInt($grid.attr("iCodEntidad"));
    }
    if ($chk.length > 0 && $chk[0].checked) {
        param.bSelTodos = 1;
    }
    if ($gridData.val() !== undefined && $gridData.val() !== "") {
        param.jsonSeleccionados = $gridData.val();
    }

    var options = {
        type: "POST",
        url: sSource,
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function(data, textStatus, jqXHR) {
            var response = data.d;
            if (response.sDSOTag !== null) {
                rows = DSOControls.LoadFunction(response.sDSOTag);
                self.updateGridData(rows);
            }
            if (self.deSelAll) {
                self.updateGridData([]);
                self.deSelAll = false;
            }
            fnCallback(response);
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
MultiSelect.prototype.fnInitComplete = function($grid) {
    var $chkSpan = this.$chk.closest('span');
    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $toolbar = $(wrapperID).find(".dataTables_length");
    $toolbar.append($chkSpan);

    $grid.fnAdjustColumnSizing();
    this.$grid = $grid;
}
MultiSelect.prototype.fnRenderCheck = function(obj, funcionOnClick) {
    var self = this;
    var $grid = self.$grid;
    var iCodCatalogo = parseInt(obj.aData[0]);
    var bSeleccionado = parseInt(obj.aData[1]);
    var disabled = self.enableField ? "" : "disabled=\"true\"";
    funcionOnClick = funcionOnClick.replace("{0}", iCodCatalogo);
    var ret;
    if (bSeleccionado == 1) {
        ret = "<input type=\"checkbox\" checked=\"true\" " + disabled + " onclick=\"" + funcionOnClick + "\"\></input>";
    }
    else {
        ret = "<input type=\"checkbox\" " + disabled + " onclick=\"" + funcionOnClick + "\"\></input>";
    }
    return ret;
}
MultiSelect.prototype.chkOnClick = function(iCodCatalogo, checked) {
    var self = this;
    self.$chk[0].checked = false;
    var rows = self.getGridData();
    var row = { iCodCatalogo: iCodCatalogo, bSeleccionado: checked ? 1 : 0 };
    var idx;
    var idxRemove = -1;
    var maxIdx = rows.length;
    var bPush = true;
    for (idx = 0; idx < maxIdx; idx++) {
        if (rows[idx]["iCodCatalogo"] == row["iCodCatalogo"]) {
            if (!checked) {
                idxRemove = idx;
            }
            bPush = false;
            break;
        }
    }
    if (bPush) {
        rows.push(row);
    }
    if (idxRemove !== -1) {
        rows.splice(idxRemove, 1);
    }
    self.updateGridData(rows);
}
MultiSelect.prototype.chkTodosOnclick = function(checked) {
    var self = this;
    if (checked) {
        self.$grid = $("#" + self.$grid.attr("id")).dataTable({ bRetrieve: true });
        self.deSelAll = false;
    }
    else {
        self.updateGridData([]);
        self.deSelAll = true;
    }
    self.$grid.fnDraw();
}
var AutoComplete = {};

AutoComplete.validaVigencias = function($ctl, restricted) {
    var self = this;
    var iCodEntidad = $ctl.attr("iCodEntidad");
    var entidadField = $ctl.attr("entidadField");
    var restrictedValues = $ctl.attr("restrictedValues");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');
    var iCodCatalogo = $($ctl.attr("textValueID")).val();

    if (entidadField !== undefined) {
        var $searchVal = self.$container.find(".DSOAutocomplete[keytiaField='" + entidadField + "']").next(".DSOAutocompleteVal");
        iCodEntidad = $searchVal.val();
    }

    if (iCodEntidad === undefined || iCodEntidad === "null") return;

    if (iCodCatalogo === undefined || iCodCatalogo === "null" || iCodCatalogo === "") return;


    if (iniVigencia === undefined || self.$ctlIniVigencia.length === 0) {
        iniVigencia = null;
    }

    if (finVigencia === undefined || self.$ctlFinVigencia.length === 0) {
        finVigencia = null;
    }

    if (self.$container.hasClass("ReporteEstandar")
    && finVigencia !== null && finVigencia.setDate !== undefined) {
        finVigencia.setDate(finVigencia.getDate() + 1);
    }

    if (restrictedValues === undefined) {
        restrictedValues = "";
    }

    var param = { iCodCatalogo: iCodCatalogo, iCodEntidad: iCodEntidad, restrictedValues: restrictedValues, iniVigencia: iniVigencia, finVigencia: finVigencia };
    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/ValidaVigenciasAutoComplete",
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $ctl,
        success: function(data, status, xhr) {
            var resultado = (typeof data.d) == 'string' ?
                                eval('(' + data.d + ')') :
                                data.d;
            if (!resultado) {
                $ctl.val("");
                $($ctl.attr("textValueID")).val("");
            }
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}

AutoComplete.validaVigenciasAtrib = function($ctl, restricted) {
    var self = this;
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');
    var iCodCatalogo = $($ctl.attr("textValueID")).val();

    if (iCodCatalogo === undefined || iCodCatalogo === "null" || iCodCatalogo === "") return;

    if (iniVigencia === undefined || self.$ctlIniVigencia.length === 0) {
        iniVigencia = null;
    }

    if (finVigencia === undefined || self.$ctlFinVigencia.length === 0) {
        finVigencia = null;
    }

    var param = { iCodCatalogo: iCodCatalogo, iniVigencia: iniVigencia, finVigencia: finVigencia };
    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/ValidaVigenciasAtribAutoComplete",
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        context: $ctl,
        success: function(data, status, xhr) {
            var resultado = (typeof data.d) == 'string' ?
                                eval('(' + data.d + ')') :
                                data.d;
            if (!resultado) {
                $ctl.val("");
                $($ctl.attr("textValueID")).val("");
            }
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
AutoComplete.mostrarRenglon = function($ctl, valMostrar) {
    var $self = $(this);
    if ($ctl.length == 0 || $self.length == 0) return;
    var $tr = $ctl.closest("tr");

    var iCodCatalogo = $($self.attr("textValueID")).val();

    if (iCodCatalogo === undefined || iCodCatalogo === "null" || iCodCatalogo === "" || iCodCatalogo != valMostrar)
    {
		$tr.hide();
	}
	else if (iCodCatalogo == valMostrar)
	{
		$tr.show();
	}
}