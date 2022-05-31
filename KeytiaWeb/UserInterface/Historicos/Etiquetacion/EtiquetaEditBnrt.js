//RJ.20160620 Lo único que se cambió en este archivo, con respecto al original EtiquetaEdit.js
//es la función Historico.prototype.IniConsumo

Historico.prototype.fnServerDataEmple = function($grid, sSource, aoData, fnCallback, iCodEmpleado) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestro = this.iCodMaestro;
    var iCodEntidad = this.iCodEntidad;

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
        liCodEntidad: parseInt(iCodEntidad + ".0"),
        liCodMaestro: parseInt(iCodMaestro + ".0"),
        liCodEmpleado: parseInt(iCodEmpleado + ".0")
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

Historico.prototype.DeshabilitaScreen = function() {
    var self = this;
    $.alerts._overlay('show');
    self.$historic.find("[idInput='TextBoxEtiqueta']").each(function() {
        this.disabled = true;
    });
    self.$historic.find("[idSelect='DropDownEtiqueta']").each(function() {
        this.disabled = true;
    });
    self.$historic.find(".DSODropDownList").each(function() {
        this.disabled = true;
    });
    self.$historic.find(".DSOGrid").each(function() {
        this.disabled = true;
    });
    self.$historic.find(".DSOCheckBox").each(function() {
        this.disabled = true;
    });
    self.$historic.find("img").each(function() {
        this.disabled = true;
    });
    self.$historic.find("[id$='Grid_wrapper']").each(function() {
        this.disabled = true;
    });
    self.$historic.find(".buttonSave").button("disable");

    if (self.$historic.find(".buttonCancel").length > 0) {
        self.$historic.find(".buttonCancel").button("disable");
    }

    self.$historic.find(".buttonEdit").button("disable");

    self.$historic.find(".buttonSearch").button("disable");
}

Historico.prototype.grabarEtiquetas = function(doPostBack) {
    var self = this;
    if (self.editing && self.confirmGrabar) {
        jConfirm(self.confirmGrabar, self.confirmTitleGrabar, function(r) {
            if (r) {
                self.DeshabilitaScreen();
                doPostBack();
            }
        });
    }
    else {
        self.DeshabilitaScreen();
        doPostBack();
    }
}

Historico.prototype.IniConsumo = function() {
    var self = this;
    var $dtIniPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date01_dt']");
    var $dtFinPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date02_dt']");
    var lDateIni = new Date($dtIniPeriodo.datetimepicker('getDate'));
    var lDateFin = new Date($dtFinPeriodo.datetimepicker('getDate'));

    var param = {
        liCodEmpleado: parseInt(self.iCodEmpleado + ".0"),
        ldtIniPeriodo: lDateIni,
        ldtFinPeriodo: lDateFin,
        ldtCorte: self.dtCorte,
        ldtLimite: self.dtLimite
    };

    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/GetConsumoResumen",
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        context: self,
        async: true,
        success: function(data, textStatus, jqXHR) {
            if ((typeof data.d) == 'string' && data.d != '') {
                var $Consumos = DSOControls.LoadFunction(data.d);
                var maxIdx = $Consumos.length;
                var idx;
                var $totP = self.$historic.find(".DSONumberEdit[id$='ResumenWrapper_dTotPersonal_val']");
                var $totL = self.$historic.find(".DSONumberEdit[id$='ResumenWrapper_dTotLaboral_val']")
                var $totR = self.$historic.find(".DSONumberEdit[id$='ResumenWrapper_dTotProveedor_val']") //RJ.20160606
                var $totO = self.$historic.find(".DSONumberEdit[id$='ResumenWrapper_dTotOutsourcing_val']") //RJ.20160606
                var $totN = self.$historic.find(".DSONumberEdit[id$='ResumenWrapper_dTotNI_val']");
                var $tot = self.$historic.find(".DSONumberEdit[id$='ResumenWrapper_dTotal_val']");
                var $totPServ = self.$historic.find(".DSOTextBox[id$='sTotPServer_txt']");
                var $totLServ = self.$historic.find(".DSOTextBox[id$='sTotLServer_txt']")
                var $totRServ = self.$historic.find(".DSOTextBox[id$='sTotRServer_txt']") //RJ.20160606
                var $totOServ = self.$historic.find(".DSOTextBox[id$='sTotOServer_txt']") //RJ.20160606
                var $totNServ = self.$historic.find(".DSOTextBox[id$='sTotNServer_txt']");
                var $totServ = self.$historic.find(".DSOTextBox[id$='sTotServer_txt']");
                var totP = 0;
                var totL = 0;
                var totR = 0; //RJ.20160606
                var totO = 0; //RJ.20160606
                var totN = 0;
                $totN.val(0);
                $totP.val(0);
                $totL.val(0);
                $totR.val(0); //RJ.20160606
                $totO.val(0); //RJ.20160606
                $tot.val(0);

                for (idx = 0; idx < maxIdx; idx++) {
                    var ldConsumo = parseFloat($Consumos[idx].Consumo).toFixed(2);
                    switch ($Consumos[idx].Grupo) {
                        case 0:
                            $totN.val(ldConsumo);
                            totN = ldConsumo;
                            break;
                        case 1:
                            $totP.val(ldConsumo);
                            totP = ldConsumo;
                            break;
                        case 2:
                            $totL.val(ldConsumo);
                            totL = ldConsumo;
                            break;
                        case 3: //RJ.20160606
                            $totR.val(ldConsumo);
                            totR = ldConsumo;
                            break;
                        case 4: //RJ.20160606
                            $totO.val(ldConsumo);
                            totO = ldConsumo;
                            break;
                        default:
                    }
                }
                var tot = parseFloat(totN) + parseFloat(totP) + parseFloat(totL) + parseFloat(totR) + parseFloat(totO); //RJ.20160606
                tot = parseFloat(tot).toFixed(2);
                $tot.val(tot);

                $totPServ.val($totP[0].value);
                $totLServ.val($totL[0].value);
                $totRServ.val($totR[0].value); //RJ.20160606
                $totOServ.val($totO[0].value); //RJ.20160606
                $totNServ.val($totN[0].value);
                $totServ.val($tot[0].value);
            }
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}

Historico.prototype.perChange = function($ddl) {
    var self = this;
    var $dtIniPeriodo = self.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date01_dt']");
    var $dtFinPeriodo = self.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date02_dt']");

    var $grid = this.$historic.find(".DSOGrid[id$='ResumenGrid_Grid']").dataTable({ bRetrieve: true });

    if (self.ejecutandoPerChange === true) {
        self.ejecutandoPerChange = false;
        return;
    }

    var day = parseInt(self.liDiaCorte);
    var lDateIni = new Date($dtIniPeriodo.datetimepicker('getDate'));
    var lDateFin = new Date($dtFinPeriodo.datetimepicker('getDate'));

    $dtIniPeriodo.attr("monthDayValue", day);
    if ($dtFinPeriodo[0].id == $ddl[0].id) {
        //Se modificó Fin de Periodo
        if (day - 1 > 0) {
            lDateIni = new Date(lDateFin.getFullYear(), lDateFin.getMonth() - 1, 1);
        }
        else {
            lDateIni = new Date(lDateFin.getFullYear(), lDateFin.getMonth(), 1);
        }
    }

    lDateFin = new Date(lDateIni.getFullYear(), lDateIni.getMonth() + 1, day);
    if (lDateFin.getDate() !== day) {
        lDateFin.setDate(0);
    }
    lDateFin.setDate(lDateFin.getDate() - 1);
    $dtFinPeriodo.attr("monthDayValue", lDateFin.getDate());

    var param = {
        liCodEmpleado: parseInt(self.iCodEmpleado + ".0"),
        ldtIniPeriodo: lDateIni,
        ldtFinPeriodo: lDateFin,
        ldtCorte: self.dtCorte,
        ldtLimite: self.dtLimite
    };

    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/IsPeriodoValido",
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        context: self,
        async: true,
        success: function(data, textStatus, jqXHR) {
            var $chkPersonal = self.$historic.find(".DSOCheckBox[dataField='bPersonales']").find("input[type='checkbox']");
            var lbLaboral = 0;
            $chkPersonal[0].status = false;
            var $chkLaboral = self.$historic.find(".DSOCheckBox[dataField='bLaborales']").find("input[type='checkbox']");
            if ($chkLaboral.length > 0) {
                lbLaboral = 1;
                $chkLaboral[0].status = false;
            }
            if ((typeof data.d) == 'number' && (data.d == 1)) {
                self.$historic.find(".buttonSave").button("enable");
                $chkPersonal.removeAttr("disabled");
                if (lbLaboral > 0) {
                    $chkLaboral.removeAttr("disabled");
                }
            }
            else {
                self.$historic.find(".buttonSave").button("disable");
                $chkPersonal.attr("disabled", true);
                if (lbLaboral > 0) {
                    $chkLaboral.attr("disabled", true);
                }
            }
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);

    self.ejecutandoPerChange = true;
    DSOControls.DateTimeBox.setMonth($dtIniPeriodo, lDateIni.getFullYear(), lDateIni.getMonth() + 1);

    self.ejecutandoPerChange = true;
    DSOControls.DateTimeBox.setMonth($dtFinPeriodo, lDateFin.getFullYear(), lDateFin.getMonth() + 1);

    self.ejecutandoPerChange = true;

    self.bSetResumen = 1;
    $grid.fnDraw();
    return;
}

Historico.prototype.getGridData = function() {
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

Historico.prototype.aDataToRow = function(sColumns, aData) {
    var columns = sColumns.split(',');
    var idx = 0;
    var maxIdx = columns.length;
    var row = {};
    for (idx = 0; idx < maxIdx; idx++) {
        if (aData) {
            row[columns[idx]] = aData[idx];
        }
        else {
            row[columns[idx]] = null;
        }
    }
    return row;
}
Historico.prototype.rowToaData = function(sColumns, row) {
    var columns = sColumns.split(',');
    var idx = 0;
    var maxIdx = columns.length;
    var aData = [];
    for (idx = 0; idx < maxIdx; idx++) {
        aData.push(row[columns[idx]]);
    }
    return aData;
}
Historico.prototype.fnUpdateData = function(iRegistro, sCampo, sValor, bIniConsumo) {
    var self = this;
    var param = {
        liCodEmpleado: parseInt(self.iCodEmpleado + ".0"),
        liCodMaestroResum: parseInt(self.iCodMaestroResum + ".0"),
        liCodEntidadResum: parseInt(self.iCodEntidadResum + ".0"),
        liRegistro: parseInt(iRegistro + ".0"),
        lsCampo: sCampo,
        lsValor: sValor
    };

    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/UpdateRowResumen",
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        context: self,
        async: false,
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
    if (bIniConsumo == true) {
        self.IniConsumo();
    }
}

Historico.prototype.fnServerResumen = function($grid, sSource, aoData, fnCallback, iCodEntidadResum, iCodMaestroResum) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);
    var $dtIniPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date01_dt']");
    var $dtFinPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date02_dt']");
    var $filtro = this.$historic.find(".DSODropDownList[dataField='iCodGrupo']")[0];
    var lDateIni = new Date($dtIniPeriodo.datetimepicker('getDate'));
    var lDateFin = new Date($dtFinPeriodo.datetimepicker('getDate'));
    var idx;
    var maxidx = $filtro.length;
    var $gridData = self.$historic.find(".DSOGrid[id$='ResumenGrid_Grid']");
    var rows = self.getGridData();

    for (idx = 0; idx < maxidx; idx++) {
        if ($filtro[idx].selected == true && $filtro[idx].value != "-1") {
            request.sSearch[2] = $filtro[idx].value;
            break;
        }
    }

    var $filtroTDest = this.$historic.find(".DSODropDownList[dataField='iCodDestino']")[0];
    maxidx = $filtroTDest.length;

    for (idx = 0; idx < maxidx; idx++) {
        if ($filtroTDest[idx].selected == true && $filtroTDest[idx].value != "-1") {
            request.sSearch[17] = $filtroTDest[idx].value;
            break;
        }
    }

    if (self.bSetResumen == undefined) {
        self.bSetResumen = 0;
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        liCodEmpleado: parseInt(self.iCodEmpleado + ".0"),
        liCodMaestroResum: parseInt(iCodMaestroResum + ".0"),
        liCodEntidad: parseInt(iCodEntidadResum + ".0"),
        ldtIniPeriodo: lDateIni,
        ldtFinPeriodo: lDateFin,
        ldtCorte: self.dtCorte,
        ldtLimite: self.dtLimite,
        liHisPrevEtq: self.liHisPrevEtq,
        liSetResumen: self.bSetResumen
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

            self.fnAct = "";
            self.bFiltro = 0;
            self.bSetResumen = 0;
            self.IniConsumo();
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}


Historico.prototype.ResumenEtiquetaDrawTxt = function() {
    var self = this;
    var $gridData = self.$historic.find(".DSOGrid[id$='ResumenGrid_Grid']");
    $gridData.find("[idInput='TextBoxEtiqueta']").each(function() {
        $(this).val($(this).attr("valorData"));
        $(this).change(function() {
            self.fnUpdateData($(this).attr("iRegistro"), $(this).attr("sColName"), $(this).val(), false);
        });
    });
}

Historico.prototype.fnRenderTxt = function(obj, lNameCol) {
    var self = this;
    var maxIdx = obj.oSettings.aoColumns.length;
    var idx;
    var sColName;
    var col;
    var ddl = $("<a></a>");
    var input = $("<input></input>");
    var columns = [];
    var sColumns;

    for (idx = 0; idx < maxIdx; idx++) {
        columns.push(obj.oSettings.aoColumns[idx].sName);
        if (obj.oSettings.aoColumns[idx].sTitle === lNameCol) {
            sColName = obj.oSettings.aoColumns[idx].sName;
            col = idx;
        }
    }

    sColumns = columns.join(',');

    input.attr("id", "oInput" + obj.iDataRow + "_" + obj.iDataColumn);
    input.attr("idInput", "TextBoxEtiqueta");
    input.attr("sColName", sColName);

    var rowObj = self.aDataToRow(sColumns, obj.aData);
    //if (rowObj["EdodeReg"] != "0") {
    if (rowObj["EdodeReg"] == "0") {
        return obj.aData[col];
    }

    input.attr("iRegistro", rowObj["iCodRegistro"]);
    input.attr("valorData", obj.aData[col]);
    input.attr("maxLength", self.liLongEtq);

    ddl.append(input);
    var ret = ddl.html();
    return ret;
}

Historico.prototype.ResumenEtiquetaDrawDD = function() {
    var self = this;
    if (self.gpoEtiqueta == undefined) {
        return;
    }

    var $gridData = self.$historic.find(".DSOGrid[id$='ResumenGrid_Grid']");
    var max = self.gpoEtiqueta.length;
    var idx;
    $gridData.find("[idSelect='DropDownEtiqueta']").each(function() {
        for (idx = 0; idx < max; idx++) {
            if ($(this).attr("valorData") == self.gpoEtiqueta[idx].text) {

                $(this).val(self.gpoEtiqueta[idx].value);
                break;
            }
        }

        $(this).change(function() {
            self.fnUpdateData($(this).attr("iRegistro"), $(this).attr("sColName"), $(this).val(), true);
        });
    });
}

Historico.prototype.fnRenderCmb = function(obj, sSource, lNameColGpo, lNameColCos) {
    var self = this;
    var maxIdx = obj.oSettings.aoColumns.length;
    var idx;
    var option;
    var ddl = $("<a></a>");
    var select = $("<select></select>");
    var data;
    var columns = [];
    var sColumns;
    var sColName;
    self.gpoEtiqueta = sSource;

    for (idx = 0; idx < maxIdx; idx++) {
        columns.push(obj.oSettings.aoColumns[idx].sName);
        if (obj.oSettings.aoColumns[idx].sName === "GEtiqueta") {
            sColName = obj.oSettings.aoColumns[idx].sTitle;
        }
    }
    sColumns = columns.join(',');

    select.attr("id", "oSelect" + obj.iDataRow + "_" + obj.iDataColumn);
    select.attr("idSelect", "DropDownEtiqueta");
    select.attr("sColName", sColName);

    for (idx = 0; idx < maxIdx; idx++) {
        if (obj.oSettings.aoColumns[idx].sTitle != lNameColGpo) {
            continue;
        }

        var rowObj = self.aDataToRow(sColumns, obj.aData);
        //if (rowObj["EdodeReg"] != "0") {
        if (rowObj["EdodeReg"] == "0") {
            return obj.aData[idx];
        }
        select.attr("iRegistro", rowObj["iCodRegistro"]);
        select.attr("valorData", obj.aData[idx]);

        var maxIdS = sSource.length;
        var idS;
        var idSelected;
        for (idS = 0; idS < maxIdS; idS++) {
            option = $("<option></option>");
            option.val(sSource[idS].value);
            option.html(sSource[idS].text);
            select.append(option);
        }
        break;
    }

    ddl.append(select);
    var ret = ddl.html();
    return ret;
}

Historico.prototype.fnRenderLnk = function(obj, lNameColTel, $wd) {
    var self = this;
    var maxIdx = obj.oSettings.aoColumns.length;

    for (idx = 0; idx < maxIdx; idx++) {
        if (obj.oSettings.aoColumns[idx].sTitle == lNameColTel) {
            break;
        }
    }

    var jsonData = JSON.stringify(obj.aData);
    var sdoFunction = self.jsObj + ".consultaDetLinea({0},{1})";
    sdoFunction = sdoFunction.replace("{0}", "$('#" + $wd.attr("id") + "')");
    sdoFunction = sdoFunction.replace("{1}", escape(jsonData));
    return "<a href=\"javascript:" + sdoFunction + ";\">" + obj.aData[idx] + "</a>";
}


Historico.prototype.filtrarResumen = function() {
    var $grid = this.$historic.find(".DSOGrid[id$='ResumenGrid_Grid']").dataTable({ bRetrieve: true });
    $grid.fnDraw();
}

Historico.prototype.fillTodasXGrupo = function(obj) {
    var self = this;
    var $dtIniPeriodo = self.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date01_dt']");
    var $dtFinPeriodo = self.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date02_dt']");
    var lDateIni = new Date($dtIniPeriodo.datetimepicker('getDate'));
    var lDateFin = new Date($dtFinPeriodo.datetimepicker('getDate'));
    var iCheck;
    var iValor;

    if (obj[0].checked == true) {
        iCheck = 1;
    }
    else {
        iCheck = 0;
    }

    if (obj[0].id.toString().search("bPersonales") >= 0) {
        iValor = 1;
        var $chkLaboral = self.$historic.find(".DSOCheckBox[dataField='bLaborales']").find("input[type='checkbox']");
        if ($chkLaboral.length > 0) {
            $chkLaboral[0].status = false;
        }
    }
    else {
        iValor = 2;
        var $chkPersonal = self.$historic.find(".DSOCheckBox[dataField='bPersonales']").find("input[type='checkbox']");
        $chkPersonal[0].status = false;
    }

    var param = {
        liCodEmpleado: parseInt(self.iCodEmpleado + ".0"),
        ldtIniPeriodo: lDateIni,
        ldtFinPeriodo: lDateFin,
        liCodEntidadResum: parseInt(self.iCodEntidadResum + ".0"),
        liCodMaestroResum: parseInt(self.iCodMaestroResum + ".0"),
        liCheck: parseInt(iCheck + ".0"),
        liValor: parseInt(iValor + ".0")
    };

    var options = {
        type: "POST",
        url: KeytiaMaster.appPath + "WebMethods.aspx/SetAllGrupo",
        data: JSON.stringify(param),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        context: self,
        async: true,
        success: function(data, textStatus, jqXHR) {
            self.filtrarResumen();
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}

Historico.prototype.vigenciaTimeBox = function($ddl) {
    var self = this;
    if (self.vigenciaTimeout !== undefined) {
        clearInterval(self.vigenciaTimeout);
    }
    self.vigenciaTimeout = setTimeout(function() {
        self.perChange($ddl);
    }, 100);
}

//WINDOWS DETALLES
Historico.prototype.End = function($wd) {
    var $txtWndVisible = this.$historic.find(".DSOTextBox[id$='txtWdVisible_txt']");
    $txtWndVisible.val("");
}

Historico.prototype.consultaDetalle = function($wd) {
    var self = this;
    var lswd = "Det";
    var $txtWndVisible = this.$historic.find(".DSOTextBox[id$='txtWdVisible_txt']");
    self.fnShowWindow($wd, lswd);
    $txtWndVisible.val(lswd);
}

Historico.prototype.consultaDetLinea = function($wd, loRow) {
    var self = this;
    var lswd = "DetLin";
    var $txtLinea = $wd.find("[keytiaField='TelDest']");
    var $txtGpo = $wd.find("[keytiaField='GEtiqueta']");
    var $txtWndVisible = this.$historic.find(".DSOTextBox[id$='txtWdVisible_txt']");
    $wd = $($wd.selector);

    $txtLinea[0].value = loRow[1];
    $txtGpo[0].value = loRow[2];

    self.lsLinea = loRow[1];
    $txtWndVisible.val(lswd + "|" + loRow[1] + "|" + loRow[9]);
    self.fnShowWindow($wd, lswd);
}

Historico.prototype.fnShowWindow = function($wd, lswd) {
    var self = this;
    var $grid = $wd.find(".DSOGrid[id$='" + lswd + "Grid_Grid']").dataTable({ bRetrieve: true });
    var $wdIniPeriodo = $wd.find(".DSODateTimeBox[id$='Atrib" + lswd + "Wrapper_Date01_dt']");
    var $wdFinPeriodo = $wd.find(".DSODateTimeBox[id$='Atrib" + lswd + "Wrapper_Date02_dt']");
    var $dtIniPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date01_dt']");
    var $dtFinPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date02_dt']");
    var lDateIni = new Date($dtIniPeriodo.datetimepicker('getDate'));
    var lDateFin = new Date($dtFinPeriodo.datetimepicker('getDate'));


    $wdIniPeriodo.attr("monthDayValue", lDateIni.getDate());
    $wdIniPeriodo.datetimepicker('setDate', lDateIni);
    $wdFinPeriodo.attr("monthDayValue", lDateFin.getDate());
    $wdFinPeriodo.datetimepicker('setDate', lDateFin);

    $grid.fnDraw();
    self.$wdDetalle = $wd;
    DSOControls.Window.Init.call($wd);
    $wd.showWindow();
}

Historico.prototype.fnServerDetalle = function($grid, sSource, aoData, fnCallback, iCodEntidadApp, iCodMaestroResum) {
    var self = this;
    var request = DSOControls.Grid.GetRequest(aoData);
    var $dtIniPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date01_dt']");
    var $dtFinPeriodo = this.$historic.find(".DSODateTimeBox[id$='AtribWrapper_Date02_dt']");
    var lDateIni = new Date($dtIniPeriodo.datetimepicker('getDate'));
    var lDateFin = new Date($dtFinPeriodo.datetimepicker('getDate'));
    var lsLinea = "";


    if ($grid[0].id.toString().match("wdVerDetLinea")) {
        lsLinea = (self.lsLinea == undefined ? "" : self.lsLinea);
    }

    if (isNaN(lDateIni) || isNaN(lDateFin)) {
        return;
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        liCodEmpleado: parseInt(self.iCodEmpleado + ".0"),
        liCodMaestro: parseInt(iCodMaestroResum + ".0"),
        liCodEntidad: parseInt(iCodEntidadApp + ".0"),
        lsLinea: lsLinea,
        ldtIniPeriodo: lDateIni,
        ldtFinPeriodo: lDateFin
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
            //            self.lsLinea = "";            
        },
        error: DSOControls.ErrorAjax
    };
    $.ajax(options);
}
//WINDOW DETALLE