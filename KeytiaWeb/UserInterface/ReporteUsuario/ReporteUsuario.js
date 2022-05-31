function ReporteUsuario(tabsId, id, fnPostBack) {
    var me = this;

    this.id = id;
    this.conf = {};
    this.lang = {};
    this.reports = {};

    this.tabsId = tabsId;

    this.tabContainerId = this.tabsId + "_Container";
    this.tabContainer = $("#" + this.tabContainerId);
    this.tabs = $("#" + this.tabContainerId + " > ul > li");

    this.prevTab = -1;
    this.currTab = -1;

    this.dataOrderFields = 3;
    this.groupFields = 5;
    this.groupMatFields = 2;

    this.graphChanged = false;

    this.doPostBack = fnPostBack;

    $(document).ready(function() {
        me.updateCurrTab();
    });
}

ReporteUsuario.prototype.initReport = function(initDataJSON, initDataLang) {
    this.conf = JSON.parse(initDataJSON);
    this.lang = JSON.parse(initDataLang);

    this.initFieldOrderControls();

    this.initTabs();

    this.initStart();
    this.initFields();
    this.initSelectedFields();
    this.initSummary()
    this.initFieldOrder();
    this.initCriterias();

    if (this.conf.groupTableId != "" || this.conf.groupMatTableId != "")
        this.initCriteriasGrp();

    this.initDataOrder();
    this.initMail();

    if (this.conf.groupTableId != "")
        this.initGroup();

    if (this.conf.groupMatTableId != "")
        this.initGroupMat();

    if (this.conf.graphTableId != "")
        this.initGraph();

    this.setModified();

    DSOControls.Button.Init();
}

ReporteUsuario.prototype.getField = function(fieldId) {
    var ret = null;

    for (var i = 0; i < this.conf.fields.length; i++)
        if (this.conf.fields[i].id == fieldId) {
            ret = this.conf.fields[i];
            break;
        }

    return ret;
}

ReporteUsuario.prototype.getCriteria = function(criteriaId) {
    var ret = null;

    for (var i = 0; i < this.conf.criterias.length; i++)
        if (this.conf.criterias[i].row == criteriaId) {
            ret = this.conf.criterias[i];
            break;
        }

    return ret;
}

ReporteUsuario.prototype.getCriteriaGrp = function(criteriaId) {
    var ret = null;

    for (var i = 0; i < this.conf.criteriasGrp.length; i++)
        if (this.conf.criteriasGrp[i].row == criteriaId) {
            ret = this.conf.criteriasGrp[i];
        break;
    }

    return ret;
}

ReporteUsuario.prototype.generate = function() {
    this.conf.gen = 1;
    $(".ReporteEstandar").hide();
    this.lastStep();
    this.setModified();
    this.doPostBack();
}

ReporteUsuario.prototype.save = function() {
    this.confirmed = undefined;

    if (this.validateSave())
        this.waitConfirm(this.id + ".saveConfirmed();");
}


ReporteUsuario.prototype.saveConfirmed = function() {
    if (this.confirmed) {
        this.conf.action = "save";
        //this.conf.gen = 0;
        this.setModified();
        this.doPostBack();
    }
}

ReporteUsuario.prototype.validateSave = function() {
    var me = this;

    if (this.conf.id != -1 && $("#" + this.conf.textBoxNameId).val() == "")
        jConfirm(this.lang.RUMsgConfSave, "KeytiaV", function(r) { me.confirmed = r; });
    else
        this.confirmed = true;

    return true;
}

ReporteUsuario.prototype.del = function() {
    this.confirmed = undefined;

    if (this.validateDel())
        this.waitConfirm(this.id + ".delConfirmed();");
}


ReporteUsuario.prototype.delConfirmed = function() {
    if (this.confirmed) {
        this.conf.action = "delete";
        this.conf.gen = 0;
        this.setModified();
        this.doPostBack();
    }
}

ReporteUsuario.prototype.validateDel = function() {
    var me = this;

    if (this.conf.id != -1 && $("#" + this.conf.textBoxNameId).val() == "")
        jConfirm(this.lang.RUMsgConfDel, "KeytiaV", function(r) { me.confirmed = r; });
    else
        this.confirmed = true;

    return true;
}

ReporteUsuario.prototype.waitConfirm = function(afterConfirm) {
    if (this.confirmed == undefined)
        setTimeout(this.id + ".waitConfirm('" + afterConfirm + "');", 250);
    else
        setTimeout(afterConfirm, 250)
}

ReporteUsuario.prototype.setModified = function(modified) {
    if (modified != undefined && modified != null)
        this.conf.modified = modified;

    $("#" + this.conf.saveButtonId).attr("disabled", !this.conf.modified);
    $("#" + this.conf.deleteButtonId).attr("disabled", this.conf.name == "");
    $("#" + this.conf.generateButtonId).attr("disabled", !(this.conf.modified || this.conf.name != ""));

    $("#" + this.conf.textBoxDataId).val(JSON.stringify(this.conf));
}



/* * * * * Tabs * * * * */

ReporteUsuario.prototype.initTabs = function() {
    for (i = 0; i < this.lang.tabs.length; i++)
        this.initTab(this.lang.tabs[i].index, this.lang.tabs[i].title, this.lang.tabs[i].contentId, this.lang.tabs[i].panelId);
}

ReporteUsuario.prototype.initTab = function(tabId, tabTitle, tabContentId, tabPanelId) {
    var me = this;
    var pnl;

    this.tabs.each(function() {
        if ($(this).attr("itemIndex") == tabId)
            $(this).children(0).text(tabTitle);

        $(this).children(0).click(function() {
            me.updateCurrTab();
            me.tabClicked($(this).parent());
        });
    });

    pnl = $("#" + tabPanelId).detach();
    $("#" + tabContentId).append(pnl);
}

ReporteUsuario.prototype.currentTab = function() {
    var state = JSON.parse($(this.tabContainer.attr("txtStateID")).val());
    return state.selectedIndex;
}

ReporteUsuario.prototype.prevStep = function() {
    var tab;

    tab = $("#" + this.tabContainerId + " li[itemIndex=\"" + (this.currentTab() - 1) + "\"] > a");
    tab.click();
}

ReporteUsuario.prototype.nextStep = function() {
    var tab;

    tab = $("#" + this.tabContainerId + " li[itemIndex=\"" + (this.currentTab() + 1) + "\"] > a");
    tab.click();
}

ReporteUsuario.prototype.lastStep = function() {
    var tab;

    tab = $("#" + this.tabContainerId + " li").last().find('a');
    tab.click();
}

ReporteUsuario.prototype.updateCurrTab = function() {
    this.prevTab = this.currTab;
    this.currTab = this.currentTab();
}

ReporteUsuario.prototype.tabClicked = function(tab) {
//    if (this.fieldsSelectedChanged)
//        this.doPostBack();
}




/* * * * * Inicio * * * * */
ReporteUsuario.prototype.initStart = function() {
    var me = this;
    var tabStart;
    var tr;
    var td;
    var lst;
    var divDesc;
    var i;

    tabStart = $("#" + this.conf.startTableId);
    //tabFields.attr("border", 1);

    tr = $(document.createElement("tr"));
    tabStart.prepend(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgRepDisp);
    tr.append(td);

    td = $(document.createElement("td"));
    tr.append(td);

    //lista de reportes
    lst = $(document.createElement("select"));
    lst.attr("id", "lstReports");
    lst.attr("size", "16");
    lst.attr("style", "width: 600px; height: 150px;");
    td.append(lst);

    td.append("<br/><br/>");

    divDesc = $(document.createElement("div"));
    divDesc.attr("id", "divDesc");
    divDesc.attr("style", "width: 570px; height: 150px; padding: 15px; overflow: auto;");
    divDesc.addClass("ui-corner-all ui-widget-content");
    divDesc.html("<div style=\"font-size: 130%; font-weight: bold;\">" + this.lang.RUMsgDesc + "</div><br/>");
    td.append(divDesc);

    $.ajax({
        type: "POST",
        url: "../../WebMethods.aspx/GetReports",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function(data, status, xhr) { me.listReports(data, status, xhr); }
    });
}

ReporteUsuario.prototype.listReports = function(data, status, xhr) {
    var me = this;
    var ogn = "";
    var og = undefined;
    var i;
    var row;
    var opt;

    var lst = $("#lstReports");

    this.reports = JSON.parse(data.d);

    for (i = 0; i < this.reports.length; i++) {
        row = $(this.reports[i]);

        if (row.attr("{RepUsuAcceso}.vchDescripcion") != ogn) {
            this.addSelGroup(lst, og);

            ogn = row.attr("{RepUsuAcceso}.vchDescripcion");
            og = $(document.createElement("optgroup"));
            og.attr("label", ogn);
        }

        opt = $(document.createElement("option"));
        opt.attr("value", row.attr("vchCodigo"));
        //opt.attr("text", row.attr("vchDescripcion"));
        opt.html(row.attr("vchDescripcion"));

        opt.appendTo(og);
    }

    this.addSelGroup(lst, og);

    var showDescr = function() {
        me.conf.vchCodigoOpen = lst.val();
        me.setModified();

        for (i = 0; i < me.reports.length; i++)
            if (me.reports[i].vchCodigo == lst.val()) {
            $("#divDesc").html("<div style=\"font-size: 130%; font-weight: bold;\">" + me.lang.RUMsgDesc + "</div><br/>" +
                    ($(me.reports[i]).attr("{Descripcion}") != null ? $(me.reports[i]).attr("{Descripcion}") : ""));
            break;
        }
    };

    lst.change(showDescr);
    lst.keypress(showDescr);
    lst.keydown(showDescr);
    lst.keyup(showDescr);
}

ReporteUsuario.prototype.addSelGroup = function(sel, group) {
    if (group != undefined)
        group.appendTo(sel);
}

/* * * * * Campos * * * * */

ReporteUsuario.prototype.initFields = function() {
    var me = this;
    var fld;
    var tabFields;
    var tr;
    var td;
    var chkMarkAll;
    var lblMarkAll;

    tabFields = $("#" + this.conf.fieldTableId);
    //tabFields.attr("border", 1);

    tr = $(document.createElement("tr"));
    tabFields.prepend(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    tr.append(td);

    td = $(document.createElement("td"));
    tr.append(td);

    chkMarkAll = $(document.createElement("input"));
    chkMarkAll.attr("type", "checkbox");
    chkMarkAll.attr("id", "chkMarkAllFields");
    td.append(chkMarkAll);

    lblMarkAll = $(document.createElement("label"));
    lblMarkAll.attr("for", "chkMarkAllFields");
    lblMarkAll.html(this.lang.RUMsgSelTodo);
    td.append(lblMarkAll);

    for (i = 0; i < this.conf.categories.length; i++) {
        $("#" + this.conf.categories[i].controlId + "_wrapper span").each(function() {
            if ($(this).attr("data-value") != undefined) {
                fld = me.getField($(this).attr("data-value"));
                fld.itemId = $(this).find("input[type=\"checkbox\"]").attr("id");
                $("#" + fld.itemId).change(function() { me.setModified(true); });
            }
        });
    }

    chkMarkAll.change(function() {
        for (var f = 0; f < me.conf.fields.length; f++)
            $("#" + me.conf.fields[f].itemId).attr("checked", $("#chkMarkAllFields").attr("checked"));

        me.fieldClicked();
    });

    $("#" + this.conf.accessTypeRblId + "_rbl").find("input[type=\"radio\"]").change(function() { me.setModified(true); });
}

ReporteUsuario.prototype.initSelectedFields = function() {
    var me = this;

    for (i = 0; i < this.conf.categories.length; i++)
        $("#" + this.conf.categories[i].controlId + "_wrapper input[type=\"checkbox\"]").each(function() { me.updateFields(this) });
}

ReporteUsuario.prototype.fieldClicked = function (event, obj) {
    var j;

    for (var i = 0; i < this.conf.fields.length; i++) {
        if (this.conf.groupTableId != "") {
            for (j = 1; j <= this.groupFields; j++) {
                if (this.conf.fields[i].id == $("#selGroup" + j).val())
                    $("#" + this.conf.fields[i].itemId).attr("checked", true);
            }

            if ($("#chkSummary" + this.conf.fields[i].id).attr("checked"))
                $("#" + this.conf.fields[i].itemId).attr("checked", true);
        }
        else if (this.conf.groupMatTableId != "") {
            for (j = 1; j <= this.groupMatFields; j++) {
                if (this.conf.fields[i].id == $("#selGroupMatX" + j).val())
                    $("#" + this.conf.fields[i].itemId).attr("checked", true);

                if (this.conf.fields[i].id == $("#selGroupMatY" + j).val())
                    $("#" + this.conf.fields[i].itemId).attr("checked", true);
            }

            if ($("#chkSummary" + this.conf.fields[i].id).attr("checked"))
                $("#" + this.conf.fields[i].itemId).attr("checked", true);
        }

        this.conf.fields[i].isSelected = $("#" + this.conf.fields[i].itemId).attr("checked");
    }

    this.initFieldOrder();
    this.initSummary();
    this.initDataOrder();

    if (this.conf.groupTableId != "")
        this.initGroup();

    if (this.conf.graphTableId != "")
        this.initGraph();
}

ReporteUsuario.prototype.updateFields = function(field) {
//    if (field.checked && this.fieldsSelected.indexOf(field.id) == -1) {
//        this.fieldsSelected += field.id + ",";
//        this.fieldsSelectedChanged = true;
//    }
//    else if (!field.checked && this.fieldsSelected.indexOf(field.id) != -1) {
//        this.fieldsSelected = this.fieldsSelected.replace(field.id + ",", "");
//        this.fieldsSelectedChanged = true;
//    }
}



/* * * * * Orden de los campos * * * * */
ReporteUsuario.prototype.initFieldOrderControls = function() {
    var tabFieldOrder
    var tr;
    var td;
    var lst;
    var btn;
    var div;

    tabFieldOrder = $("#" + this.conf.fieldOrderTableId);
    tabFieldOrder.html("");

    tr = $(document.createElement("tr"));
    tabFieldOrder.append(tr);

    //Leyenda lista
    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgFldOrd);
    tr.append(td);

    //Celda lista
    td = $(document.createElement("td"));
    td.addClass("DSOTcCtl ColSpan1");
    tr.append(td);

    //lista de campos
    lst = $(document.createElement("select"));
    lst.attr("id", "lstFieldOrder");
    lst.attr("size", "4");
    lst.attr("style", "width: 200px; height: 200px; float: left");
    td.append(lst);

    div = $(document.createElement("div"));
    div.attr("style", "width: 30px; float: left");
    td.append(div);

    btn = $(document.createElement("button")); div.append(btn); btn.button({ text: false, icons: { primary: 'ui-icon-triangle-1-n'} }).css("width", "22px").css("height", "22px").css("margin", "2px"); btn.click(function(event) { event.returnValue = false; event.preventDefault(); oRepUsr.fieldMoveTop('lstFieldOrder') });
    btn = $(document.createElement("button")); div.append(btn); btn.button({ text: false, icons: { primary: 'ui-icon-carat-1-n'} }).css("width", "22px").css("height", "22px").css("margin", "2px"); btn.click(function(event) { event.returnValue = false; event.preventDefault(); oRepUsr.fieldMoveUp('lstFieldOrder') });
    btn = $(document.createElement("button")); div.append(btn); btn.button({ text: false, icons: { primary: 'ui-icon-carat-1-s'} }).css("width", "22px").css("height", "22px").css("margin", "2px"); btn.click(function(event) { event.returnValue = false; event.preventDefault(); oRepUsr.fieldMoveDown('lstFieldOrder') });
    btn = $(document.createElement("button")); div.append(btn); btn.button({ text: false, icons: { primary: "ui-icon-triangle-1-s"} }).css("width", "22px").css("height", "22px").css("margin", "2px"); btn.click(function(event) { event.returnValue = false; event.preventDefault(); oRepUsr.fieldMoveBottom('lstFieldOrder') });
}

ReporteUsuario.prototype.initFieldOrder = function() {
    var fieldOrderList;
    var fieldsAnt = [];
    var fields = [];
    var i;
    var ins;

    fieldOrderList = $("#lstFieldOrder");
    fieldOrderList.html("");

    for (i = 0; i < this.conf.fields.length; i++) {
        if (this.conf.fields[i].isSelected) {
            fields = [];
            ins = false;

            if (this.conf.fields[i].fieldOrder == -1)
                this.conf.fields[i].fieldOrder = 99999;

            for (var j = 0; j < fieldsAnt.length; j++) {
                if (!ins && this.conf.fields[i].fieldOrder < fieldsAnt[j].fieldOrder) {
                    fields.length++;
                    fields[fields.length - 1] = this.conf.fields[i];
                    ins = true
                }

                fields.length++;
                fields[fields.length - 1] = fieldsAnt[j];
            }

            if (!ins) {
                fields.length++;
                fields[fields.length - 1] = this.conf.fields[i];
            }

            fieldsAnt = fields;
        }
        else
            this.conf.fields[i].fieldOrder = -1;
    }

    for (i = 0; i < fields.length; i++) {
        fields[i].fieldOrder = i;
        this.addOption(fieldOrderList, null, fields[i].id, fields[i].name);
    }

    this.setModified();
}

ReporteUsuario.prototype.updateReportDataFieldOrder = function() {
    var me = this;

    fieldOrder = [];

    $("#lstFieldOrder option").each(function() {
        fieldOrder.length++;
        fieldOrder[fieldOrder.length - 1] = $(this).attr("value");

        me.getField($(this).attr("value")).fieldOrder = fieldOrder.length - 1;
    });

    this.setModified(true);
}

ReporteUsuario.prototype.fieldMoveTop = function(lstName) {
    var lst = $("#" + lstName);
    this.fieldMove(lst, 0);
    this.updateReportDataFieldOrder();
}

ReporteUsuario.prototype.fieldMoveUp = function(lstName) {
    var lst = $("#" + lstName);
    this.fieldMove(lst, lst.attr("selectedIndex") - 1);
    this.updateReportDataFieldOrder();
}

ReporteUsuario.prototype.fieldMoveDown = function(lstName) {
    var lst = $("#" + lstName);
    this.fieldMove(lst, lst.attr("selectedIndex") + 1);
    this.updateReportDataFieldOrder();
}

ReporteUsuario.prototype.fieldMoveBottom = function(lstName) {
    var lst = $("#" + lstName);
    this.fieldMove(lst, lst.attr("length") - 1);
    this.updateReportDataFieldOrder();
}

ReporteUsuario.prototype.fieldMove = function(lst, newIndex) {
    if (lst.attr("selectedIndex") != -1 && lst.attr("selectedIndex") < lst[0].options.length) {
        var itm = lst[0].options[lst.attr("selectedIndex")];

        lst[0].options.remove(lst.attr("selectedIndex"));
        lst[0].options.add(itm, newIndex);
    }
}




/* * * * * Resumen * * * * */
ReporteUsuario.prototype.initSummary = function() {
    var me = this;

    var tabSummary;
    var tr;
    var td;

    var i;
    var fld;
    var chk;
    var lbl;
    var sel;
    var opt;

    var fldId;
    var chkNumRecs;
    var lblNumRecs;

    tabSummary = $("#" + this.conf.summaryTableId);
    tabSummary.html("");

    if (this.conf.groupTableId != "" || this.conf.groupMatTableId != "") {
        tr = $(document.createElement("tr"));
        tabSummary.append(tr);

        td = $(document.createElement("td"));
        td.addClass("DSOTcLbl");
        tr.append(td);

        td = $(document.createElement("td"));
        tr.append(td);

        chkNumRecs = $(document.createElement("input"));
        chkNumRecs.attr("type", "checkbox");
        chkNumRecs.attr("id", "chkNumRecs");
        chkNumRecs.attr("checked", this.conf.showNumRecs != 0);
        chkNumRecs.change(function() { me.conf.showNumRecs = ($(this).attr("checked") ? 1 : 0) });
        td.append(chkNumRecs);

        lblNumRecs = $(document.createElement("label"));
        lblNumRecs.attr("for", "chkNumRecs");
        lblNumRecs.html(this.lang.RUMsgNumRecs);
        td.append(lblNumRecs);
    }

    for (i = 0; i < this.conf.fields.length; i++) {
        fld = this.conf.fields[i];

        if ((fld.isSelected || this.conf.groupTableId != "" || this.conf.groupMatTableId != "") &&
        (fld.type == "Int" || fld.type == "IntFormat" || fld.type == "Float" || fld.type == "Currency")) {
            fldId = fld.id;

            tr = $(document.createElement("tr"));
            tr.attr("id", "rowSummary" + fldId);
            tabSummary.append(tr);

            //Leyenda Checkbox
            td = $(document.createElement("td"));
            td.addClass("DSOTcLbl");
            td.html(this.lang.RUMsgTotales);
            tr.append(td);

            //Celda Checkbox
            td = $(document.createElement("td"));
            td.addClass("DSOTcCtl ColSpan1");
            tr.append(td);

            //checkbox
            chk = $(document.createElement("input"));
            chk.attr("id", "chkSummary" + fldId);
            chk.attr("type", "checkbox");
            chk.change({ fldId: fldId }, function(event) { me.setSummary(event.data.fldId); });
            td.append(chk);

            if (fld.aggregateFn != "")
                chk.attr("checked", true);

            //label
            lbl = $(document.createElement("label"));
            lbl.attr("for", "chkSummary" + fldId);
            lbl.html(fld.name);
            td.append(lbl);


            //Leyenda Funcion
            td = $(document.createElement("td"));
            td.addClass("DSOTcLbl");
            td.html(this.lang.RUMsgAggFn);
            tr.append(td);

            //Celda select
            td = $(document.createElement("td"));
            td.addClass("DSOTcCtl ColSpan1");
            tr.append(td);

            //select
            sel = $(document.createElement("select"));
            sel.attr("id", "selSummary" + fldId);
            sel.change({ fldId: fldId }, function(event) { me.setSummary(event.data.fldId); });
            td.append(sel);

            this.addOption(sel, fld.aggregateFn, "sum", "Suma"); //TODO: Language
            //this.addOption(sel, fld.aggregateFn, "avg", "Media"); //TODO: Language
            //this.addOption(sel, fld.aggregateFn, "max", "Máximo"); //TODO: Language
            //this.addOption(sel, fld.aggregateFn, "min", "Mínimo"); //TODO: Language
        }
    }

    this.setModified();
}

ReporteUsuario.prototype.setSummary = function(fldId) {
    var fld;
    var chk;
    var sel;

    fld = this.getField(fldId);
    chk = $("#chkSummary" + fldId);
    sel = $("#selSummary" + fldId);

    fld.aggregateFn = (chk.attr("checked") ? sel.val() : "");

    this.setModified(true);
    this.fieldClicked();

    if (this.conf.graphTableId != "") {
        this.initGraph();
        this.setGraph();
    }
}



/* * * * * Orden de los datos * * * * */
ReporteUsuario.prototype.initDataOrder = function() {
    var me = this;

    var tabDataOrder;
    var tr;
    var td;

    var i;
    var j;
    var k;
    var f;
    var sel;
    var opt;

    var sortType;

    var fldId;

    tabDataOrder = $("#" + this.conf.dataOrderTableId);
    tabDataOrder.html("");

    for (i = 1; i <= this.dataOrderFields; i++) {
        sortType = "";

        tr = $(document.createElement("tr"));
        tr.attr("id", "rowDataOrder" + i);
        tabDataOrder.append(tr);

        //Leyenda Funcion
        td = $(document.createElement("td"));
        td.addClass("DSOTcLbl");
        td.html(i == 1 ? this.lang.RUMsgDatOrd : this.lang.RUMsgThen);
        tr.append(td);

        //Celda select
        td = $(document.createElement("td"));
        td.addClass("DSOTcCtl ColSpan1");
        tr.append(td);

        //select Field
        sel = $(document.createElement("select"));
        sel.attr("id", "selDataOrder" + i);
        sel.width(200);
        sel.change({ rowId: i }, function(event) { me.setDataOrder(event.data.rowId); });
        td.append(sel);

        this.addOption(sel, null, "-1", "");

        for (f = 0; f < this.conf.fields.length; f++) {
            if (this.conf.fields[f].isSelected) {
                opt = this.addOption(sel, (this.conf.fields[f].dataOrder == i ? this.conf.fields[f].id : "-1"), this.conf.fields[f].id, this.conf.fields[f].name);

                if (opt.attr("selected")) {
                    this.conf.fields[f].dataOrderType = (this.conf.fields[f].dataOrderType != -1 ? this.conf.fields[f].dataOrderType : 1);
                    sortType = this.conf.fields[f].dataOrderType;
                }
            }
        }


        //select 
        sel = $(document.createElement("select"));
        sel.attr("id", "selDataOrder" + i + "Type");
        sel.change({ rowId: i }, function(event) { me.setDataOrder(event.data.rowId); });
        td.append(sel);

        for (j = 0; j < this.lang.orderTypes.length; j++)
            this.addOption(sel, sortType, this.lang.orderTypes[j].id, this.lang.orderTypes[j].name);
    }

    //Si un campo se queda en blanco, los siguientes se ponen en blanco
    for (i = 1; i <= this.dataOrderFields; i++) {
        if ($("#selDataOrder" + i).val() == -1) {
            for (j = i; j < this.conf.fields.length; j++) {
                $("#selDataOrder" + j).val("");
                $("#selDataOrder" + j + "Type").val(-1);
            }

            for (j = 0; j < this.conf.fields.length; j++) {
                if (this.conf.fields[j].dataOrder >= i) {
                    this.conf.fields[j].dataOrder = -1;
                    this.conf.fields[j].dataOrderType = -1;
                }
            }
        }
    }

    //Quita de los otros combos el campo seleccionado en el actual
    for (i = 1; i <= this.dataOrderFields; i++) {
        for (j = 1; j <= this.dataOrderFields; j++) {
            if (i != j) {
                for (k = 1; k < $("#selDataOrder" + j)[0].options.length; k++) {
                    if ($("#selDataOrder" + j)[0].options[k].value == $("#selDataOrder" + i).val()) {
                        $("#selDataOrder" + j)[0].remove(k--);
                    }
                }
            }
        }
    }

    this.setModified();
}

ReporteUsuario.prototype.setDataOrder = function(rowId) {
    var fld;
    var selFld;
    var selType;
    var i;

    selFld = $("#selDataOrder" + rowId);
    selType = $("#selDataOrder" + rowId + "Type");

    if (selFld.length > 0 && selType.length > 0) {
        for (i = 0; i < this.conf.fields.length; i++) {
            if (this.conf.fields[i].dataOrder == rowId) {
                this.conf.fields[i].dataOrder = -1;
                this.conf.fields[i].dataOrderType = -1;
                break;
            }
        }

        if (selFld.val() != -1) {
            fld = this.getField(selFld.val());
            fld.dataOrder = rowId;
            fld.dataOrderType = selType.val();

            if (fld.dataOrderType == null)
                fld.dataOrderType = 1;
        }

        this.setModified(true);
        this.initDataOrder();
    }
}



/* * * * * Agrupamiento de los datos * * * * */
ReporteUsuario.prototype.initGroup = function () {
    var me = this;

    var tabGroup;
    var tr;
    var td;

    var i;
    var j;
    var k;
    var f;
    var sel;
    var sel2;
    var opt;

    var sortType;

    var fld;
    var fldId;

    tabGroup = $("#" + this.conf.groupTableId);
    tabGroup.html("");

    for (i = 1; i <= this.groupFields; i++) {
        fld = null;

        tr = $(document.createElement("tr"));
        tr.attr("id", "rowGroup" + i);
        tabGroup.append(tr);

        //Leyenda Funcion
        td = $(document.createElement("td"));
        td.addClass("DSOTcLbl");
        td.html(i == 1 ? this.lang.RUMsgGrpBy : this.lang.RUMsgThen);
        tr.append(td);

        //Celda select
        td = $(document.createElement("td"));
        td.addClass("DSOTcCtl ColSpan1");
        tr.append(td);

        //select Field
        sel = $(document.createElement("select"));
        sel.attr("id", "selGroup" + i);
        sel.width(200);
        sel.change({ rowId: i }, function (event) { me.setGroup(event.data.rowId); });
        td.append(sel);

        this.addOption(sel, null, "-1", "");

        for (f = 0; f < this.conf.fields.length; f++) {
            opt = this.addOption(sel, (this.conf.fields[f].group == i ? this.conf.fields[f].id : "-1"), this.conf.fields[f].id, this.conf.fields[f].name);

            if (opt.attr("selected")) {
                sortType = this.conf.fields[f].groupType;
                fld = this.conf.fields[f];
            }
        }


        //select 
        sel = $(document.createElement("select"));
        sel.attr("id", "selGroup" + i + "Type");
        sel.change({ rowId: i }, function (event) { me.setGroup(event.data.rowId); });
        sel.attr("style", "display:none");
        td.append(sel);

        for (j = 0; j < this.lang.orderTypes.length; j++)
            this.addOption(sel, sortType, this.lang.orderTypes[j].id, this.lang.orderTypes[j].name);

        if (fld != null)
            if (fld.type == "Date" || fld.type == "DateTime") {
                td.append(" agrupar por ")

                sel2 = $(document.createElement("select"));
                sel2.attr("id", "selGroupPer" + i);
                sel2.change({ rowId: i }, function (event) { me.setGroup(event.data.rowId); });
                td.append(sel2);

                this.addOption(sel2, fld.tipoPeriodoGrp, "1", "Dia"); //TODO: Language
                this.addOption(sel2, fld.tipoPeriodoGrp, "2", "Mes"); //TODO: Language
                this.addOption(sel2, fld.tipoPeriodoGrp, "3", "Año"); //TODO: Language
            }
    }

    //Si un campo se queda en blanco, los siguientes se ponen en blanco
    for (i = 1; i <= this.groupFields; i++) {
        if ($("#selGroup" + i).val() == -1) {
            for (j = i; j < this.conf.fields.length; j++) {
                $("#selGroup" + j).val("");
                $("#selGroup" + j + "Type").val(-1);
            }

            for (j = 0; j < this.conf.fields.length; j++) {
                if (this.conf.fields[j].group >= i) {
                    this.conf.fields[j].group = -1;
                    this.conf.fields[j].groupType = -1;
                }
            }
        }
    }

    //Quita de los otros combos el campo seleccionado en el actual
    for (i = 1; i <= this.groupFields; i++) {
        for (j = 1; j <= this.groupFields; j++) {
            if (i != j) {
                for (k = 1; k < $("#selGroup" + j)[0].options.length; k++) {
                    if ($("#selGroup" + j)[0].options[k].value == $("#selGroup" + i).val()) {
                        $("#selGroup" + j)[0].remove(k--);
                    }
                }
            }
        }
    }

    this.setModified();
}

ReporteUsuario.prototype.setGroup = function (rowId) {
    var fld;
    var selFld;
    var selType;
    var i;

    selFld = $("#selGroup" + rowId);
    selType = $("#selGroup" + rowId + "Type");

    if (selFld.length > 0 && selType.length > 0) {
        for (i = 0; i < this.conf.fields.length; i++) {
            if (this.conf.fields[i].group == rowId) {
                this.conf.fields[i].group = -1;
                this.conf.fields[i].groupType = -1;
                break;
            }
        }

        if (selFld.val() != -1) {
            fld = this.getField(selFld.val());
            fld.group = rowId;
            fld.groupType = selType.val();

            if (fld.groupType == null)
                fld.groupType = 1;
        }

        if (fld.type == "Date" || fld.type == "DateTime")
            fld.tipoPeriodoGrp = $("#selGroupPer" + rowId).val();
        else
            fld.tipoPeriodoGrp = 0;

        this.setModified(true);

        this.initGroup();

        this.fieldClicked();

        this.initGraph();
        this.setGraph();
    }
}


/* * * * * Agrupamiento de los datos matriciales * * * * */
ReporteUsuario.prototype.initGroupMat = function() {
    var me = this;

    var tabGroupMat;
    var tr;
    var td;

    var i;
    var j;
    var k;
    var f;

    var fld;
    var sel;
    var opt;

    var fldId;

    tabGroupMat = $("#" + this.conf.groupMatTableId);
    tabGroupMat.html("");


    tr = $(document.createElement("tr"));
    tabGroupMat.append(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.attr("colspan", "2")
    td.html(this.lang.RUMsgRowHdr);
    tr.append(td);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.attr("colspan", "2")
    td.html(this.lang.RUMsgColHdr);
    tr.append(td);


    for (i = 1; i <= this.groupMatFields; i++) {
        tr = $(document.createElement("tr"));
        tr.attr("id", "rowGroupMat" + i);
        tabGroupMat.append(tr);

        //Leyenda Funcion
        td = $(document.createElement("td"));
        td.addClass("DSOTcLbl");
        td.html(i == 1 ? this.lang.RUMsgSubTot : this.lang.RUMsgThen);
        tr.append(td);

        //Celda select
        td = $(document.createElement("td"));
        td.addClass("DSOTcCtl ColSpan1");
        tr.append(td);

        //select Field
        sel = $(document.createElement("select"));
        sel.attr("id", "selGroupMatY" + i);
        sel.width(200);
        sel.change({ rowId: i }, function(event) { me.setGroupMat(event.data.rowId); });
        td.append(sel);

        this.addOption(sel, null, "-1", "");
        fld = null;

        for (f = 0; f < this.conf.fields.length; f++) {
            opt = this.addOption(sel, (this.conf.fields[f].groupMatY == i ? this.conf.fields[f].id : "-1"), this.conf.fields[f].id, this.conf.fields[f].name);

            if (opt.attr("selected")) {
                fld = this.conf.fields[f];
            }
        }

        if (fld != null)
            if (fld.type == "Date" || fld.type == "DateTime") {
                td.append(" " + this.lang.RUMsgGrpBy + " ")

            sel2 = $(document.createElement("select"));
            sel2.attr("id", "selGroupMatYPer" + i);
            sel2.change({ rowId: i }, function(event) { me.setGroupMat(event.data.rowId); });
            td.append(sel2);

            this.addOption(sel2, fld.tipoPeriodoGrpY, "1", "Dia"); //TODO: Language
            this.addOption(sel2, fld.tipoPeriodoGrpY, "2", "Mes"); //TODO: Language
            this.addOption(sel2, fld.tipoPeriodoGrpY, "3", "Año"); //TODO: Language
        }




        //Leyenda Funcion
        td = $(document.createElement("td"));
        td.addClass("DSOTcLbl");
        td.html(i == 1 ? this.lang.RUMsgSubTot : this.lang.RUMsgThen);
        tr.append(td);

        //Celda select
        td = $(document.createElement("td"));
        td.addClass("DSOTcCtl ColSpan1");
        tr.append(td);

        //select Field
        sel = $(document.createElement("select"));
        sel.attr("id", "selGroupMatX" + i);
        sel.width(200);
        sel.change({ rowId: i }, function(event) { me.setGroupMat(event.data.rowId); });
        td.append(sel);

        this.addOption(sel, null, "-1", "");
        fld = null;

        for (f = 0; f < this.conf.fields.length; f++) {
            opt = this.addOption(sel, (this.conf.fields[f].groupMatX == i ? this.conf.fields[f].id : "-1"), this.conf.fields[f].id, this.conf.fields[f].name);

            if (opt.attr("selected"))
                fld = this.conf.fields[f];
        }

        if (fld != null)
            if (fld.type == "Date" || fld.type == "DateTime") {
                td.append(" " + this.lang.RUMsgGrpBy + " ")

            sel2 = $(document.createElement("select"));
            sel2.attr("id", "selGroupMatXPer" + i);
            sel2.change({ rowId: i }, function(event) { me.setGroupMat(event.data.rowId); });
            td.append(sel2);

            this.addOption(sel2, fld.tipoPeriodoGrpX, "1", "Dia"); //TODO: Language
            this.addOption(sel2, fld.tipoPeriodoGrpX, "2", "Mes"); //TODO: Language
            this.addOption(sel2, fld.tipoPeriodoGrpX, "3", "Año"); //TODO: Language
        }
    }

    //Si un campo se queda en blanco, los siguientes se ponen en blanco
    for (i = 1; i <= this.groupMatFields; i++) {
        if ($("#selGroupMatY" + i).val() == -1) {
            for (j = i; j < this.conf.fields.length; j++)
                $("#selGroupMatY" + j).val("");

            for (j = 0; j < this.conf.fields.length; j++) {
                if (this.conf.fields[j].groupMatY >= i)
                    this.conf.fields[j].groupMatY = -1;
            }
        }

        if ($("#selGroupMatX" + i).val() == -1) {
            for (j = i; j < this.conf.fields.length; j++)
                $("#selGroupMatX" + j).val("");

            for (j = 0; j < this.conf.fields.length; j++) {
                if (this.conf.fields[j].groupMatX >= i)
                    this.conf.fields[j].groupMatX = -1;
            }
        }
    }

    //Quita de los otros combos el campo seleccionado en el actual
    for (i = 1; i <= this.groupMatFields; i++) {
        for (j = 1; j <= this.groupMatFields; j++) {
            if (i != j) {
                for (k = 1; k < $("#selGroupMatY" + j)[0].options.length; k++)
                    if ($("#selGroupMatY" + j)[0].options[k].value == $("#selGroupMatY" + i).val())
                    $("#selGroupMatY" + j)[0].remove(k--);

                for (k = 1; k < $("#selGroupMatX" + j)[0].options.length; k++)
                    if ($("#selGroupMatX" + j)[0].options[k].value == $("#selGroupMatX" + i).val())
                    $("#selGroupMatX" + j)[0].remove(k--);
            }

            for (k = 1; k < $("#selGroupMatX" + j)[0].options.length; k++)
                if ($("#selGroupMatX" + j)[0].options[k].value == $("#selGroupMatY" + i).val())
                $("#selGroupMatX" + j)[0].remove(k--);

            for (k = 1; k < $("#selGroupMatY" + j)[0].options.length; k++)
                if ($("#selGroupMatY" + j)[0].options[k].value == $("#selGroupMatX" + i).val())
                $("#selGroupMatY" + j)[0].remove(k--);
        }
    }

    this.setModified();
}

ReporteUsuario.prototype.setGroupMat = function(rowId) {
    var fldX;
    var fldY;
    var selFldX;
    var i;

    selFldX = $("#selGroupMatX" + rowId);
    selFldY = $("#selGroupMatY" + rowId);

    if (selFldX.length > 0 && selFldY.length > 0) {
        for (i = 0; i < this.conf.fields.length; i++) {
            if (this.conf.fields[i].groupMatX == rowId) {
                this.conf.fields[i].groupMatX = -1;
                break;
            }
        }

        if (selFldX.val() != -1) {
            fldX = this.getField(selFldX.val());
            if (fldX != null)
                fldX.groupMatX = rowId;
        }

        for (i = 0; i < this.conf.fields.length; i++) {
            if (this.conf.fields[i].groupMatY == rowId) {
                this.conf.fields[i].groupMatY = -1;
                break;
            }
        }

        if (selFldY.val() != -1) {
            fldY = this.getField(selFldY.val());
            if (fldY != null)
                fldY.groupMatY = rowId;
        }


        if (fldX != undefined && fldX != null) {
            if (fldX.type == "Date" || fldX.type == "DateTime")
                fldX.tipoPeriodoGrpX = $("#selGroupMatXPer" + rowId).val();
            else
                fldX.tipoPeriodoGrpX = 0;
        }

        if (fldY != undefined && fldY != null) {
            if (fldY.type == "Date" || fldY.type == "DateTime")
                fldY.tipoPeriodoGrpY = $("#selGroupMatYPer" + rowId).val();
            else
                fldY.tipoPeriodoGrpY = 0;
        }

        this.setModified(true);

        this.initGroupMat();

        this.fieldClicked();

        this.initGraph();
        this.setGraph();
    }
}



/* * * * * Grafica * * * * */
ReporteUsuario.prototype.initGraph = function() {
    var me = this;

    var tabGraph;
    var tr;
    var td;

    var i;
    var f;
    var sel;
    var opt;

    var chkGpraph;
    var lblGpraph;

    tabGraph = $("#" + this.conf.graphTableId);
    tabGraph.html("");


    tr = $(document.createElement("tr"));
    tabGraph.append(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    tr.append(td);

    td = $(document.createElement("td"));
    tr.append(td);

    chkGpraph = $(document.createElement("input"));
    chkGpraph.attr("type", "checkbox");
    chkGpraph.attr("id", "chkGpraph");
    chkGpraph.attr("checked", this.conf.showGraph != 0);
    td.append(chkGpraph);

    chkGpraph.change(function() {
        me.checkGraph();
    });

    lblGpraph = $(document.createElement("label"));
    lblGpraph.attr("for", "chkGpraph");
    lblGpraph.html(this.lang.RUMsgIncGrf);
    td.append(lblGpraph);


    tr = $(document.createElement("tr"));
    tabGraph.append(tr);

    //Leyenda
    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgGraphDsg);
    tr.append(td);

    //Celda select
    td = $(document.createElement("td"));
    td.addClass("DSOTcCtl ColSpan1");
    tr.append(td);

    //select tipo
    sel = $(document.createElement("select"));
    sel.attr("id", "selGraphType");
    sel.width(200);
    sel.change(function(event) { me.checkGraph(); });
    td.append(sel);

    for (i = 0; i < this.lang.graphTypes.length; i++)
        this.addOption(sel, this.conf.graphType, this.lang.graphTypes[i].id, this.lang.graphTypes[i].name);


    tr = $(document.createElement("tr"));
    tabGraph.append(tr);

    //Leyenda
    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgX);
    tr.append(td);

    //Celda select
    td = $(document.createElement("td"));
    td.addClass("DSOTcCtl ColSpan1");
    tr.append(td);

    //select tipo
    sel = $(document.createElement("select"));
    sel.attr("id", "selGraphFieldX");
    sel.width(200);
    sel.change(function(event) { me.setGraph(); });
    td.append(sel);

    for (f = 0; f < this.conf.fields.length; f++)
        if (this.conf.fields[f].group != -1 || this.conf.fields[f].groupMatY != -1)
        opt = this.addOption(sel, this.conf.graphX, this.conf.fields[f].id, this.conf.fields[f].name);


    tr = $(document.createElement("tr"));
    tabGraph.append(tr);

    //Leyenda
    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgVals);
    tr.append(td);

    //Celda select
    td = $(document.createElement("td"));
    td.addClass("DSOTcCtl ColSpan1");
    tr.append(td);

    //select tipo
    sel = $(document.createElement("select"));
    sel.attr("id", "selGraphFieldY");
    sel.width(200);
    sel.change(function(event) { me.setGraph(); });
    td.append(sel);

    for (f = 0; f < this.conf.fields.length; f++)
        if (this.conf.fields[f].aggregateFn != "")
        opt = this.addOption(sel, this.conf.graphY, this.conf.fields[f].id, this.conf.fields[f].name);


    this.setModified();
}

ReporteUsuario.prototype.setGraph = function() {
    this.conf.graphType = $("#selGraphType").val();
    this.conf.graphX = $("#selGraphFieldX").val();
    this.conf.graphY = $("#selGraphFieldY").val();

    if (this.conf.graphType == null)
        this.conf.graphType = -1;

    if (this.conf.graphX == null)
        this.conf.graphX = -1;

    if (this.conf.graphY == null)
        this.conf.graphY = -1;

    this.setModified(true);

    if (this.conf.graphTableId != "")
        this.initGraph();
}

ReporteUsuario.prototype.checkGraph = function() {
    var me = this;
    var chkGpraph = $("#chkGpraph");
    var selGraphType = $("#selGraphType");

    //check incluir
    if (chkGpraph.attr("checked") != (me.conf.showGraph != 0))
        me.graphChanged = true;

    me.conf.showGraph = (chkGpraph.attr("checked") ? 1 : 0);

    //tipo de grafica
    if (selGraphType.attr("value") != me.conf.graphType)
        me.graphChanged = true;

    me.conf.graphType = selGraphType.attr("value");

    me.setModified(true);

    if (me.graphChanged && me.conf.gen == 1) {
        me.conf.gen = 0;
        me.setModified(true);
        me.doPostBack();
    }
}


/* * * * * Criterios * * * * */
ReporteUsuario.prototype.initCriterias = function() {
    var cs = JSON.parse(JSON.stringify(this.conf.criterias));
    var tfoot;

    var sel;
    var chkTopBottom;
    var txtTopBottom;

    for (i = 0; i < cs.length; i++) {
        this.addCriteria(cs[i].row, cs[i].fieldId, cs[i].operator);
        this.setCriteriaValue(cs[i].row, cs[i].value)
        this.updateReportDataCriteria(cs[i].row);
    }

    var me = this;
    var tabCriteria;
    var tr, td, txtConditions;

    tabCriteria = $("#" + this.conf.criteriaTableId);

    tfoot = $(document.createElement("tfoot"));
    tabCriteria.append(tfoot);

    tr = $(document.createElement("tr"));
    tfoot.append(tr);

    td = $(document.createElement("td"));
    td.attr("colspan", "5");
    tr.append(td);

    txtConditions = $(document.createElement("textarea"));
    txtConditions.attr("id", "txtConditions");
    txtConditions.attr("style", "width:500px;display:" + (this.conf.conditions == "" ? "none" : "inline-block") + ";");
    td.append(txtConditions);

    txtConditions.change(function() {
        me.conf.conditions = txtConditions.val();
        me.setModified(true);
    });

    txtConditions.blur(function() {
        me.conf.conditions = txtConditions.val();
        me.setModified();
    });

    txtConditions.val(this.conf.conditions);


    tr = $(document.createElement("tr"));
    tfoot.append(tr);

    td = $(document.createElement("td"));
    td.attr("colspan", "5");
    tr.append(td);

    chkTopBottom = $(document.createElement("input"));
    chkTopBottom.attr("type", "checkbox");
    chkTopBottom.attr("id", "chkTopBottom");
    chkTopBottom.attr("style", "width:20px");
    chkTopBottom.attr("checked", this.conf.topDir != 0);
    chkTopBottom.change(function() {
        if ($(this).attr("checked"))
            $("#selTopBottom").val("1");
        else
            $("#selTopBottom").val("0");

        me.conf.topDir = $("#selTopBottom").val();
    });
    td.append(chkTopBottom);

    td.append(this.lang.RUMsgShw + " ");

    sel = $(document.createElement("select"));
    sel.attr("id", "selTopBottom");
    sel.change(function() {
        me.conf.topDir = $("#selTopBottom").val();
        $("#chkTopBottom").attr("checked", me.conf.topDir != 0)
    });
    td.append(sel);

    this.addOption(sel, this.conf.topDir, "0", ""); //TODO: Language
    this.addOption(sel, this.conf.topDir, "1", "primeros"); //TODO: Language
    this.addOption(sel, this.conf.topDir, "-1", "últimos"); //TODO: Language

    txtTopBottom = $(document.createElement("input"));
    txtTopBottom.attr("id", "txtTopBottom");
    txtTopBottom.addClass("DSONumberEdit");
    txtTopBottom.change(function() { me.conf.topN = $("#txtTopBottom").val(); });
    txtTopBottom.val(me.conf.topN);
    td.append(txtTopBottom);

    txtTopBottom.attr("pSign", "p");
    txtTopBottom.attr("aSep", "");
    txtTopBottom.attr("aSign", "");
    txtTopBottom.attr("dGroup", "3");
    txtTopBottom.attr("mRound", "U");
    txtTopBottom.attr("mNum", "10");
    txtTopBottom.attr("aNeg", "-");
    txtTopBottom.attr("mDec", "0");
    txtTopBottom.attr("aDec", ".");
    txtTopBottom.attr("aPad", "false");

    DSOControls.NumberEdit.Init();

    txtTopBottom.attr("style", "width:25px");

    td.append(" " + this.lang.RUMsgRegs + ".");
}

ReporteUsuario.prototype.nextRowId = function() {
    this.conf.lastCriteriaRow = this.incrementString(this.conf.lastCriteriaRow, 0);
    return this.conf.lastCriteriaRow;
}

ReporteUsuario.prototype.incrementString = function(string, index) {
    var ret;
    var wrk;

    wrk = string.split("").reverse().join("");

    while (wrk.length - 1 < index) {
        if (wrk.length - 1 == index - 1)
            wrk = wrk + "@";
        else
            wrk = wrk + "A";
    }

    wrk = wrk.substr(0, index) + String.fromCharCode(wrk.charCodeAt(index) + 1) + wrk.substr(index + 1, wrk.length);

    if (wrk.charCodeAt(index) < 65) {
        wrk = wrk.substr(0, index) + "A" + wrk.substr(index + 1, wrk.length);
        ret = wrk.split("").reverse().join("");
    }
    else if (wrk.charCodeAt(index) > 90) {
        wrk = wrk.substr(0, index) + "A" + wrk.substr(index + 1, wrk.length)
        ret = this.incrementString(wrk.split("").reverse().join(""), index + 1);
    }
    else
        ret = wrk.split("").reverse().join("");

    return ret;
}

ReporteUsuario.prototype.addCriteria = function(rowId, fieldId, operatorId) {
    var me = this;

    var tabCriteria;
    var tr;
    var td;

    var btnDrop;
    var selField;
    var optField;
    var selOperator;
    var optOperator;
    var selValue;
    var txtValue;

    tabCriteria = $("#" + this.conf.criteriaTableId);

    //Renglon
    if (rowId == undefined)
        rowId = this.nextRowId();

    tr = $(document.createElement("tr"));
    tr.attr("id", "rowCriteria" + rowId);
    tabCriteria.append(tr);

    //Identificador de renglón
    td = $(document.createElement("td"));
    td.attr("id", "rowCriteria" + rowId + "RowId");
    tr.append(td);
    td.html(rowId);

    //Selector de campo
    td = $(document.createElement("td"));
    tr.append(td);

    selField = $(document.createElement("select"));
    selField.attr("id", "rowCriteria" + rowId + "Field");
    selField.change(function() { me.criteriaFieldSelected(rowId); });
    td.append(selField);

    for (var i = 0; i < this.conf.fields.length; i++)
        this.addOption(selField, fieldId, this.conf.fields[i].id, this.conf.fields[i].name);

    //Selector de operador
    td = $(document.createElement("td"));
    tr.append(td);

    selOperator = $(document.createElement("select"));
    selOperator.attr("id", "rowCriteria" + rowId + "Operator");
    selOperator.change(function() { me.updateReportDataCriteria(rowId); });
    td.append(selOperator);

    for (j = 0; j < this.lang.operators.length; j++)
        this.addOption(selOperator, operatorId, this.lang.operators[j].id, this.lang.operators[j].name);

    //Editor del valor
    td = $(document.createElement("td"));
    td.attr("id", "rowCriteria" + rowId + "EditorTd");
    tr.append(td);

    //Boton para eliminar la condición
    td = $(document.createElement("td"));
    tr.append(td);

    btnDrop = $(document.createElement("input"));
    btnDrop.attr("type", "button");
    btnDrop.addClass("button");
    btnDrop.attr("value", "-");
    btnDrop.button();
    btnDrop.click(function() { me.delCriteria(rowId) });
    td.append(btnDrop);

    me.criteriaFieldSelected(rowId);
    DSOControls.Button.Init();
}

ReporteUsuario.prototype.delCriteria = function(rowId) {
    $("#rowCriteria" + rowId).remove();
    this.updateReportDataCriteria(rowId);
}

ReporteUsuario.prototype.criteriaFieldSelected = function(rowId) {
    var me = this;
    var selField;
    var field;
    var editorTd;
    var editor;
    var editorAux;

    selField = $("#rowCriteria" + rowId + "Field");
    field = this.getField(selField.val());

    editorTd = $("#rowCriteria" + rowId + "EditorTd");
    editorTd.html("");

    editor = $(document.createElement("input")).appendTo(editorTd);
    editor.attr("id", "rowCriteria" + rowId + "Editor");
    editor.change(function() {
        me.updateReportDataCriteria(rowId);
    });
    editor.blur(function() {
        me.updateReportDataCriteria(rowId);
    });

    //alert(field.type);

    if (field.type == "Int") {
        editor.attr("pSign", "p");
        editor.attr("aSep", "");
        editor.attr("aSign", "");
        editor.attr("dGroup", "3");
        editor.attr("mRound", "U");
        editor.attr("mNum", "10");
        editor.attr("aNeg", "-");
        editor.attr("mDec", "0");
        editor.attr("aDec", ".");
        editor.attr("aPad", "false");
        editor.addClass("DSONumberEdit");

        DSOControls.NumberEdit.Init();
    }
    else if (field.type == "IntFormat") {
        editor.attr("pSign", "p");
        editor.attr("aSep", ",");
        editor.attr("aSign", "");
        editor.attr("dGroup", "3");
        editor.attr("mRound", "U");
        editor.attr("mNum", "10");
        editor.attr("aNeg", "-");
        editor.attr("mDec", "0");
        editor.attr("aDec", ".");
        editor.attr("aPad", "false");
        editor.addClass("DSONumberEdit");

        DSOControls.NumberEdit.Init();
    }
    else if (field.type == "Float" || field.type == "Currency") {
        editor.attr("pSign", "p");
        editor.attr("aSep", ",");
        editor.attr("aSign", "");
        editor.attr("dGroup", "3");
        editor.attr("mRound", "U");
        editor.attr("mNum", "10");
        editor.attr("aNeg", "-");
        editor.attr("mDec", "2");
        editor.attr("aDec", ".");
        editor.attr("aPad", "false");
        editor.addClass("DSONumberEdit");

        DSOControls.NumberEdit.Init();
    }
    else if (field.type == "Date") {
        editor.attr("showCalendar", "true");
        editor.attr("ampm", "true");
        editor.attr("stepMinute", "0.05");
        editor.attr("stepSecond", "0.05");
        editor.attr("isRTL", "false");
        editor.attr("timeOnly", "false");
        editor.attr("stepHour", "0.05");
        editor.attr("showMinute", "false");
        editor.attr("showHour", "false");
        editor.attr("showMonthAfterYear", "false");
        editor.attr("isDisabled", "false");
        editor.attr("autocomplete", "off");
        editor.attr("autoSize", "false");
        editor.attr("showCurrent", "true");
        editor.attr("showSecond", "false");
        editor.attr("showWeek", "false");
        //editor.attr("buttonImage", this.conf.calendarImageUrl);
        editor.attr("buttonImage", this.lang.CalImg);
        editor.attr("TextValue", "#" + editor.attr("id") + "Aux");
        editor.addClass("DSODateTimeBox");

        editorAux = $(document.createElement("input")).appendTo(editorTd);
        editorAux.attr("id", "rowCriteria" + rowId + "EditorAux");
        editorAux.attr("style", "display:none");

        DSOControls.DateTimeBox.Init();
    }
    else if (field.type == "DateTime") {
        editor.attr("showCalendar", "true");
        editor.attr("ampm", "true");
        editor.attr("stepMinute", "0.05");
        editor.attr("stepSecond", "0.05");
        editor.attr("isRTL", "false");
        editor.attr("timeOnly", "false");
        editor.attr("stepHour", "0.05");
        editor.attr("showMinute", "true");
        editor.attr("showHour", "true");
        editor.attr("showMonthAfterYear", "false");
        editor.attr("isDisabled", "false");
        editor.attr("autocomplete", "off");
        editor.attr("autoSize", "false");
        editor.attr("showCurrent", "true");
        editor.attr("showSecond", "true");
        editor.attr("showWeek", "false");
        //editor.attr("buttonImage", this.conf.calendarImageUrl);
        editor.attr("buttonImage", this.lang.CalImg);
        editor.attr("TextValue", "#" + editor.attr("id") + "Aux");
        editor.addClass("DSODateTimeBox");

        editorAux = $(document.createElement("input")).appendTo(editorTd);
        editorAux.attr("id", "rowCriteria" + rowId + "EditorAux");
        editorAux.attr("style", "display:none");

        DSOControls.DateTimeBox.Init();
    }

    me.updateReportDataCriteria(rowId);
}

ReporteUsuario.prototype.updateReportDataCriteria = function(rowId) {
    var rowCriteria;
    var criteria;
    var newCriterias;
    var fld;

    rowCriteria = $("#rowCriteria" + rowId);
    criteria = this.getCriteria(rowId);

    //criterio borrado
    if ((rowCriteria == null || rowCriteria.length == 0) && criteria != null) {
        newCriterias = [];

        for (var i = 0; i < this.conf.criterias.length; i++)
            if (this.conf.criterias[i].row != rowId)
            newCriterias[newCriterias.length] = this.conf.criterias[i];

        this.conf.criterias = newCriterias;
        criteria = null;
    }

    //criterio agregado
    if ((rowCriteria != null && rowCriteria.length > 0) && criteria == null) {
        criteria = {};
        this.conf.criterias[this.conf.criterias.length] = criteria;
    }

    if (criteria != null) {
        criteria.id = 0;
        criteria.row = rowId;
        criteria.fieldId = $("#rowCriteria" + rowId + "Field").val();
        criteria.operator = $("#rowCriteria" + rowId + "Operator").val();
        criteria.value = $("#rowCriteria" + rowId + "Editor").val();

        fld = this.getField(criteria.fieldId);
        if (fld != null)
            if (fld.type == 'Date' || fld.type == 'DateTime')
                criteria.value = $("#rowCriteria" + rowId + "EditorAux").val();
    }

    this.setModified(true);
}

ReporteUsuario.prototype.setCriteriaValue = function(rowId, value) {
    $("#rowCriteria" + rowId + "Editor").val(value);
    $("#rowCriteria" + rowId + "EditorAux").val(value);
}



/* * * * * Criterios Agrupados * * * * */
ReporteUsuario.prototype.initCriteriasGrp = function() {
    var cs = JSON.parse(JSON.stringify(this.conf.criteriasGrp));
    var tfoot;

    var sel;
    var chkTopBottom;
    var txtTopBottom;

    for (i = 0; i < cs.length; i++) {
        this.addCriteriaGrp(cs[i].row, cs[i].fieldId, cs[i].operator);
        this.setCriteriaGrpValue(cs[i].row, cs[i].value)
        this.updateReportDataCriteriaGrp(cs[i].row);
    }

    var me = this;
    var tabCriteriaGrp;
    var tr, td, txtConditions;

    tabCriteriaGrp = $("#" + this.conf.criteriaGrpTableId);

    tfoot = $(document.createElement("tfoot"));
    tabCriteriaGrp.append(tfoot);

    tr = $(document.createElement("tr"));
    tfoot.append(tr);

    td = $(document.createElement("td"));
    td.attr("colspan", "5");
    tr.append(td);

    txtConditions = $(document.createElement("textarea"));
    txtConditions.attr("id", "txtConditionsGrp");
    txtConditions.attr("style", "width:500px;display:" + (this.conf.conditionsGrp == "" ? "none" : "inline-block") + ";");
    td.append(txtConditions);

    txtConditions.change(function() {
        me.conf.conditionsGrp = txtConditions.val();
        me.setModified(true);
    });

    txtConditions.blur(function() {
        me.conf.conditionsGrp = txtConditions.val();
        this.setModified();
    });

    txtConditions.val(this.conf.conditionsGrp);
}

ReporteUsuario.prototype.nextRowIdGrp = function() {
    this.conf.lastCriteriaGrpRow = this.incrementString(this.conf.lastCriteriaGrpRow, 0);
    return this.conf.lastCriteriaGrpRow;
}

ReporteUsuario.prototype.addCriteriaGrp = function(rowId, fieldId, operatorId) {
    var me = this;

    var tabCriteria;
    var tr;
    var td;

    var btnDrop;
    var selField;
    var optField;
    var selOperator;
    var optOperator;
    var selValue;
    var txtValue;

    tabCriteria = $("#" + this.conf.criteriaGrpTableId);

    //Renglon
    if (rowId == undefined)
        rowId = this.nextRowIdGrp();

    tr = $(document.createElement("tr"));
    tr.attr("id", "rowCriteriaGrp" + rowId);
    tabCriteria.append(tr);

    //Identificador de renglón
    td = $(document.createElement("td"));
    td.attr("id", "rowCriteriaGrp" + rowId + "RowId");
    tr.append(td);
    td.html(rowId);

    //Selector de campo
    td = $(document.createElement("td"));
    tr.append(td);

    selField = $(document.createElement("select"));
    selField.attr("id", "rowCriteriaGrp" + rowId + "Field");
    selField.change(function() { me.criteriaGrpFieldSelected(rowId); });
    td.append(selField);

    for (var i = 0; i < this.conf.fields.length; i++)
        this.addOption(selField, fieldId, this.conf.fields[i].id, this.conf.fields[i].name);

    //Selector de operador
    td = $(document.createElement("td"));
    tr.append(td);

    selOperator = $(document.createElement("select"));
    selOperator.attr("id", "rowCriteriaGrp" + rowId + "Operator");
    selOperator.change(function() { me.updateReportDataCriteriaGrp(rowId); });
    td.append(selOperator);

    for (j = 0; j < this.lang.operators.length; j++)
        this.addOption(selOperator, operatorId, this.lang.operators[j].id, this.lang.operators[j].name);

    //Editor del valor
    td = $(document.createElement("td"));
    td.attr("id", "rowCriteriaGrp" + rowId + "EditorTd");
    tr.append(td);

    //Boton para eliminar la condición
    td = $(document.createElement("td"));
    tr.append(td);

    btnDrop = $(document.createElement("input"));
    btnDrop.attr("type", "button");
    btnDrop.addClass("button");
    btnDrop.attr("value", "-");
    btnDrop.button();
    btnDrop.click(function() { me.delCriteriaGrp(rowId) });
    td.append(btnDrop);

    me.criteriaGrpFieldSelected(rowId);
    DSOControls.Button.Init();
}

ReporteUsuario.prototype.delCriteriaGrp = function(rowId) {
    $("#rowCriteriaGrp" + rowId).remove();
    this.updateReportDataCriteriaGrp(rowId);
}

ReporteUsuario.prototype.criteriaGrpFieldSelected = function(rowId) {
    var me = this;
    var selField;
    var field;
    var editorTd;
    var editor;
    var editorAux;

    selField = $("#rowCriteriaGrp" + rowId + "Field");
    field = this.getField(selField.val());

    editorTd = $("#rowCriteriaGrp" + rowId + "EditorTd");
    editorTd.html("");

    editor = $(document.createElement("input")).appendTo(editorTd);
    editor.attr("id", "rowCriteriaGrp" + rowId + "Editor");
    editor.change(function() {
        me.updateReportDataCriteriaGrp(rowId);
    });
    editor.blur(function() {
        me.updateReportDataCriteriaGrp(rowId);
    });

    //alert(field.type);

    if (field.type == "Int") {
        editor.attr("pSign", "p");
        editor.attr("aSep", "");
        editor.attr("aSign", "");
        editor.attr("dGroup", "3");
        editor.attr("mRound", "U");
        editor.attr("mNum", "10");
        editor.attr("aNeg", "-");
        editor.attr("mDec", "0");
        editor.attr("aDec", ".");
        editor.attr("aPad", "false");
        editor.addClass("DSONumberEdit");

        DSOControls.NumberEdit.Init();
    }
    else if (field.type == "IntFormat") {
        editor.attr("pSign", "p");
        editor.attr("aSep", ",");
        editor.attr("aSign", "");
        editor.attr("dGroup", "3");
        editor.attr("mRound", "U");
        editor.attr("mNum", "10");
        editor.attr("aNeg", "-");
        editor.attr("mDec", "0");
        editor.attr("aDec", ".");
        editor.attr("aPad", "false");
        editor.addClass("DSONumberEdit");

        DSOControls.NumberEdit.Init();
    }
    else if (field.type == "Float" || field.type == "Currency") {
        editor.attr("pSign", "p");
        editor.attr("aSep", ",");
        editor.attr("aSign", "");
        editor.attr("dGroup", "3");
        editor.attr("mRound", "U");
        editor.attr("mNum", "10");
        editor.attr("aNeg", "-");
        editor.attr("mDec", "2");
        editor.attr("aDec", ".");
        editor.attr("aPad", "false");
        editor.addClass("DSONumberEdit");

        DSOControls.NumberEdit.Init();
    }
    else if (field.type == "Date") {
        editor.attr("showCalendar", "true");
        editor.attr("ampm", "true");
        editor.attr("stepMinute", "0.05");
        editor.attr("stepSecond", "0.05");
        editor.attr("isRTL", "false");
        editor.attr("timeOnly", "false");
        editor.attr("stepHour", "0.05");
        editor.attr("showMinute", "false");
        editor.attr("showHour", "false");
        editor.attr("showMonthAfterYear", "false");
        editor.attr("isDisabled", "false");
        editor.attr("autocomplete", "off");
        editor.attr("autoSize", "false");
        editor.attr("showCurrent", "true");
        editor.attr("showSecond", "false");
        editor.attr("showWeek", "false");
        //editor.attr("buttonImage", this.conf.calendarImageUrl);
        editor.attr("buttonImage", this.lang.CalImg);
        editor.attr("TextValue", "#" + editor.attr("id") + "Aux");
        editor.addClass("DSODateTimeBox");

        editorAux = $(document.createElement("input")).appendTo(editorTd);
        editorAux.attr("id", "rowCriteriaGrp" + rowId + "EditorAux");
        editorAux.attr("style", "display:none");

        DSOControls.DateTimeBox.Init();
    }
    else if (field.type == "DateTime") {
        editor.attr("showCalendar", "true");
        editor.attr("ampm", "true");
        editor.attr("stepMinute", "0.05");
        editor.attr("stepSecond", "0.05");
        editor.attr("isRTL", "false");
        editor.attr("timeOnly", "false");
        editor.attr("stepHour", "0.05");
        editor.attr("showMinute", "true");
        editor.attr("showHour", "true");
        editor.attr("showMonthAfterYear", "false");
        editor.attr("isDisabled", "false");
        editor.attr("autocomplete", "off");
        editor.attr("autoSize", "false");
        editor.attr("showCurrent", "true");
        editor.attr("showSecond", "true");
        editor.attr("showWeek", "false");
        //editor.attr("buttonImage", this.conf.calendarImageUrl);
        editor.attr("buttonImage", this.lang.CalImg);
        editor.attr("TextValue", "#" + editor.attr("id") + "Aux");
        editor.addClass("DSODateTimeBox");

        editorAux = $(document.createElement("input")).appendTo(editorTd);
        editorAux.attr("id", "rowCriteriaGrp" + rowId + "EditorAux");
        editorAux.attr("style", "display:none");

        DSOControls.DateTimeBox.Init();
    }

    me.updateReportDataCriteriaGrp(rowId);
}

ReporteUsuario.prototype.updateReportDataCriteriaGrp = function(rowId) {
    var rowCriteria;
    var criteria;
    var newCriterias;
    var fld;

    rowCriteria = $("#rowCriteriaGrp" + rowId);
    criteria = this.getCriteriaGrp(rowId);

    //criterio borrado
    if ((rowCriteria == null || rowCriteria.length == 0) && criteria != null) {
        newCriterias = [];

        for (var i = 0; i < this.conf.criteriasGrp.length; i++)
            if (this.conf.criteriasGrp[i].row != rowId)
            newCriterias[newCriterias.length] = this.conf.criteriasGrp[i];

        this.conf.criteriasGrp = newCriterias;
        criteria = null;
    }

    //criterio agregado
    if ((rowCriteria != null && rowCriteria.length > 0) && criteria == null) {
        criteria = {};
        this.conf.criteriasGrp[this.conf.criteriasGrp.length] = criteria;
    }

    if (criteria != null) {
        criteria.id = 0;
        criteria.row = rowId;
        criteria.fieldId = $("#rowCriteriaGrp" + rowId + "Field").val();
        criteria.operator = $("#rowCriteriaGrp" + rowId + "Operator").val();
        criteria.value = $("#rowCriteriaGrp" + rowId + "Editor").val();

        fld = this.getField(criteria.fieldId);
        if (fld != null)
            if (fld.type == 'Date' || fld.type == 'DateTime')
            criteria.value = $("#rowCriteriaGrp" + rowId + "EditorAux").val();
    }

    this.setModified(true);
}

ReporteUsuario.prototype.setCriteriaGrpValue = function(rowId, value) {
    $("#rowCriteriaGrp" + rowId + "Editor").val(value);
    $("#rowCriteriaGrp" + rowId + "EditorAux").val(value);
}



ReporteUsuario.prototype.addOption = function(select, selectedValue, value, text) {
    var opt;

    opt = $(document.createElement("option"));
    opt.attr("value", value);
    //opt.text = text;
    opt.html(text);
    opt.attr("selected", (selectedValue != undefined && selectedValue != null && value == selectedValue));

    select.append(opt);

    return opt;
}



ReporteUsuario.prototype.initMail = function() {
    var me = this;

    var tabMail;
    var editor;
    var editorAux;

    tabMail = $("#" + this.conf.mailTableId);
    tabMail.html("");
    tabMail.attr("style", "width:650px");



    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgFreq);
    td.attr("style", "width:150px")
    td.attr("rowspan", 6);
    tr.append(td);


    //No enviar
    td = $(document.createElement("td"));
    td.attr("style", "width:150px")
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqNE");
    chk.attr("name", "radMailFreq");
    chk.attr("value", 0);
    chk.attr("checked", this.conf.mailFreq == chk.val())
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqNE");
    lbl.html(this.lang.RUMsgNoEnv);
    td.append(lbl);

    td = $(document.createElement("td"));
    td.attr("style", "width:300px")
    tr.append(td);



    //Una Vez
    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqU");
    chk.attr("name", "radMailFreq");
    chk.attr("value", 1);
    chk.attr("checked", this.conf.mailFreq == chk.val())
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqU");
    lbl.html(this.lang.RUMsgUnaVez);
    td.append(lbl);



    //Una Vez - Fecha
    td = $(document.createElement("td"));
    tr.append(td);

    editor = $(document.createElement("input")).appendTo(td);
    editor.change(function() { me.setMail(); });

    editor.attr("id", "txtFechaUnaVez");
    editor.attr("showCalendar", "true");
    editor.attr("ampm", "true");
    editor.attr("stepMinute", "0.05");
    editor.attr("stepSecond", "0.05");
    editor.attr("isRTL", "false");
    editor.attr("timeOnly", "false");
    editor.attr("stepHour", "0.05");
    editor.attr("showMinute", "false");
    editor.attr("showHour", "false");
    editor.attr("showMonthAfterYear", "false");

    try {
        editor.attr("isDisabled", "false");
    } catch (ex) { }

    editor.attr("autocomplete", "off");
    editor.attr("autoSize", "false");
    editor.attr("showCurrent", "true");
    editor.attr("showSecond", "false");
    editor.attr("showWeek", "false");
    //editor.attr("buttonImage", this.conf.calendarImageUrl);
    editor.attr("buttonImage", this.lang.CalImg);
    editor.attr("TextValue", "#" + editor.attr("id") + "Aux");
    editor.addClass("DSODateTimeBox");

    editorAux = $(document.createElement("input")).appendTo(td);
    editorAux.attr("id", "txtFechaUnaVezAux");
    editorAux.attr("style", "display:none");

    editorAux.val(this.conf.mailFechaUnaVez);



    //Diario
    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqD");
    chk.attr("name", "radMailFreq");
    chk.attr("value", 2);
    chk.attr("checked", this.conf.mailFreq == chk.val())
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqD");
    lbl.html(this.lang.RUMsgDiario);
    td.append(lbl);


    //Diario - dias habiles
    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "checkbox");
    chk.attr("id", "chkMailDiarioDH");
    chk.attr("checked", this.conf.mailDiasHabiles != 0);
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "chkMailDiarioDH");
    lbl.html(this.lang.RUMsgDiaHab);
    td.append(lbl);



    //Semanal
    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqS");
    chk.attr("name", "radMailFreq");
    chk.attr("value", 3);
    chk.attr("checked", this.conf.mailFreq == chk.val())
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqS");
    lbl.html(this.lang.RUMsgSem);
    td.append(lbl);


    //Semanal - Dia
    td = $(document.createElement("td"));
    tr.append(td);

    editor = $(document.createElement("select"));
    editor.attr("id", "selMailSemanalDia");
    editor.change(function() { me.setMail(); });
    td.append(editor);

    this.addOption(editor, this.conf.mailDiaSemana, 0, "Domingo");
    this.addOption(editor, this.conf.mailDiaSemana, 1, "Lunes");
    this.addOption(editor, this.conf.mailDiaSemana, 2, "Martes");
    this.addOption(editor, this.conf.mailDiaSemana, 3, "Miércoles");
    this.addOption(editor, this.conf.mailDiaSemana, 4, "Jueves");
    this.addOption(editor, this.conf.mailDiaSemana, 5, "Viernes");
    this.addOption(editor, this.conf.mailDiaSemana, 6, "Sábado");



    //Quincenal
    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqQ");
    chk.attr("name", "radMailFreq");
    chk.attr("value", 4);
    chk.attr("checked", this.conf.mailFreq == chk.val())
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqQ");
    lbl.html(this.lang.RUMsgQuin);
    td.append(lbl);


    //Mensual
    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqM");
    chk.attr("name", "radMailFreq");
    chk.attr("value", 5);
    chk.attr("checked", this.conf.mailFreq == chk.val())
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqM");
    lbl.html(this.lang.RUMsgMens);
    td.append(lbl);


    //Mensual - Dia
    td = $(document.createElement("td"));
    tr.append(td);

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqMDiaD");
    chk.attr("name", "radMailFreqMDia");
    chk.attr("checked", this.conf.mailDiaMes != 0)
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqMDiaD");
    lbl.html(this.lang.RUMsgDia + " ");
    td.append(lbl);

    editor = $(document.createElement("input"));
    editor.val(this.conf.mailDiaMes);
    editor.attr("id", "txtMailFreqMDia");
    editor.attr("style", "width:30px");
    editor.change(function() { me.setMail(); });

    editor.attr("pSign", "p");
    editor.attr("aSep", "");
    editor.attr("aSign", "");
    editor.attr("dGroup", "3");
    editor.attr("mRound", "U");
    editor.attr("mNum", "10");
    editor.attr("aNeg", "-");
    editor.attr("mDec", "0");
    editor.attr("aDec", ".");
    editor.attr("aPad", "false");
    editor.addClass("DSONumberEdit");
    td.append(editor);


    //Mensual - semana
    td.append("<br>");

    chk = $(document.createElement("input"));
    chk.attr("type", "radio");
    chk.attr("id", "radMailFreqMDiaS");
    chk.attr("name", "radMailFreqMDia");
    chk.attr("checked", this.conf.mailDiaMes == 0)
    chk.change(function() { me.setMail(); });
    td.append(chk);

    lbl = $(document.createElement("label"));
    lbl.attr("for", "radMailFreqMDiaS");
    lbl.html(this.lang.RUMsgSemana + " ");
    td.append(lbl);

    editor = $(document.createElement("select"));
    editor.attr("id", "selMailFreqMDiaS");
    editor.change(function() { me.setMail(); });
    td.append(editor);

    this.addOption(editor, this.conf.mailSemanaMes, 1, "Primera");
    this.addOption(editor, this.conf.mailSemanaMes, 2, "Segunda");
    this.addOption(editor, this.conf.mailSemanaMes, 3, "Tercera");
    this.addOption(editor, this.conf.mailSemanaMes, 4, "Cuarta");
    this.addOption(editor, this.conf.mailSemanaMes, 5, "Última");

    td.append(" " + this.lang.RUMsgDia + " ");


    //Mensual - semana - dia
    editor = $(document.createElement("select"));
    editor.attr("id", "selMailFreqMDiaS");
    editor.change(function() { me.setMail(); });
    td.append(editor);

    this.addOption(editor, this.conf.mailDiaSemanaMes, 0, "Domingo");
    this.addOption(editor, this.conf.mailDiaSemanaMes, 1, "Lunes");
    this.addOption(editor, this.conf.mailDiaSemanaMes, 2, "Martes");
    this.addOption(editor, this.conf.mailDiaSemanaMes, 3, "Miércoles");
    this.addOption(editor, this.conf.mailDiaSemanaMes, 4, "Jueves");
    this.addOption(editor, this.conf.mailDiaSemanaMes, 5, "Viernes");
    this.addOption(editor, this.conf.mailDiaSemanaMes, 6, "Sábado");




    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgDest);
    td.attr("rowspan", 3);
    tr.append(td);



    td = $(document.createElement("td"));
    td.html(this.lang.RUMsgTo);
    tr.append(td);

    td = $(document.createElement("td"));
    tr.append(td);

    editor = $(document.createElement("input")).appendTo(td);
    editor.attr("id", "txtMailTo");
    editor.attr("style", "width:250px");
    editor.val(this.conf.mailTo);
    editor.change(function() { me.setMail(); });
    td.append(editor);



    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    td.html(this.lang.RUMsgCC);
    tr.append(td);

    td = $(document.createElement("td"));
    tr.append(td);

    editor = $(document.createElement("input")).appendTo(td);
    editor.attr("id", "txtMailCC");
    editor.attr("style", "width:250px");
    editor.val(this.conf.mailCC);
    editor.change(function() { me.setMail(); });
    td.append(editor);



    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    td.html(this.lang.RUMsgBCC);
    tr.append(td);

    td = $(document.createElement("td"));
    tr.append(td);

    editor = $(document.createElement("input")).appendTo(td);
    editor.attr("id", "txtMailBCC");
    editor.attr("style", "width:250px");
    editor.val(this.conf.mailBCC);
    editor.change(function() { me.setMail(); });
    td.append(editor);



    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgSubject);
    tr.append(td);

    td = $(document.createElement("td"));
    td.attr("colspan", "2");
    tr.append(td);

    editor = $(document.createElement("input")).appendTo(td);
    editor.attr("id", "txtMailSubject");
    editor.attr("style", "width:450px");
    editor.val(this.conf.mailSubject);
    editor.change(function() { me.setMail(); });
    td.append(editor);



    tr = $(document.createElement("tr"));
    tabMail.append(tr);

    td = $(document.createElement("td"));
    td.addClass("DSOTcLbl");
    td.html(this.lang.RUMsgBody);
    tr.append(td);

    td = $(document.createElement("td"));
    td.attr("colspan", "2");
    tr.append(td);

    editor = $(document.createElement("textarea")).appendTo(td);
    editor.attr("id", "txtMailMessage");
    editor.attr("rows", "5");
    editor.attr("style", "width:450px");
    editor.val(this.conf.mailMessage);
    editor.change(function() { me.setMail(); });
    td.append(editor);

    DSOControls.DateTimeBox.Init();
    DSOControls.NumberEdit.Init();
}

ReporteUsuario.prototype.setMail = function() {
    if ($("#radMailFreqNE").attr("checked")) this.conf.mailFreq = $("#radMailFreqNE").val();
    else if ($("#radMailFreqU").attr("checked")) this.conf.mailFreq = $("#radMailFreqU").val();
    else if ($("#radMailFreqD").attr("checked")) this.conf.mailFreq = $("#radMailFreqD").val();
    else if ($("#radMailFreqS").attr("checked")) this.conf.mailFreq = $("#radMailFreqS").val();
    else if ($("#radMailFreqQ").attr("checked")) this.conf.mailFreq = $("#radMailFreqQ").val();
    else if ($("#radMailFreqM").attr("checked")) this.conf.mailFreq = $("#radMailFreqM").val();

    if (this.conf.mailFreq == 1 && $("#txtFechaUnaVezAux").val() != "")
        this.conf.mailFechaUnaVez = $("#txtFechaUnaVezAux").val();

    this.conf.mailDiasHabiles = $("#chkMailDiarioDH").attr("checked") ? 1 : 0;
    this.conf.mailDiaSemana = $("#selMailSemanalDia").val();

    if ($("#radMailFreqMDiaD").attr("checked"))
    {
        this.conf.mailDiaMes = $("#txtMailFreqMDia").val();
        this.conf.mailSemanaMes = -1;
        this.conf.mailDiaSemanaMes = -1;
    }
    else if ($("#radMailFreqMDiaS").attr("checked"))
    {
        this.conf.mailDiaMes = 0;
        this.conf.mailSemanaMes = $("#selMailFreqMDiaS").val();
        this.conf.mailDiaSemanaMes = $("#selMailFreqMDiaS").val();
    }

    this.conf.mailTo = $("#txtMailTo").val();
    this.conf.mailCC = $("#txtMailCC").val();
    this.conf.mailBCC = $("#txtMailBCC").val();
    this.conf.mailSubject = $("#txtMailSubject").val();
    this.conf.mailMessage = $("#txtMailMessage").val();

    this.setModified(true);
    //this.initMail();
}
