Historico.prototype.fnServerDetalle = function($grid, sSource, aoData, fnCallback) {
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestroDeta = this.$historic.find(".DSODropDownList[id$='iCodMaestroDeta_ddl']").val();
    var iCodCarga = this.iCodCatalogo;

    var aoColumns = $grid.fnSettings().aoColumns;
    var idx;
    var maxIdx = aoColumns.length;
    var col;
    var $filtro;
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        $filtro = this.$historic.find("[dataFilterDeta='" + col.sName + "']");
        if ($filtro.length > 0) {
            request.sSearch[idx] = $filtro.val();
        }
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        liTipoDeta: 0,
        liCodCarga: parseInt(iCodCarga + ".0"),
        liCodMaestro: parseInt(iCodMaestroDeta + ".0")
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


Historico.prototype.fnServerPendientes = function($grid, sSource, aoData, fnCallback) {
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestroPend = this.$historic.find(".DSODropDownList[id$='iCodMaestroPend_ddl']").val();
    var iCodCarga = this.iCodCatalogo;

    var aoColumns = $grid.fnSettings().aoColumns;
    var idx;
    var maxIdx = aoColumns.length;
    var col;
    var $filtro;
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        $filtro = this.$historic.find("[dataFilterPend='" + col.sName + "']");
        if ($filtro.length > 0) {
            request.sSearch[idx] = $filtro.val();
        }
    }


    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");
    $header.css("width", $grid.css("width"));

    var param = {
        gsRequest: request,
        liTipoDeta: 1,
        liCodCarga: parseInt(iCodCarga + ".0"),
        liCodMaestro: parseInt(iCodMaestroPend + ".0")
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
Historico.prototype.fnServerPendFacturas = function($grid, sSource, aoData, fnCallback) {
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestroPend = this.$historic.find(".DSODropDownList[id$='iCodMaestroPend_ddl']").val();
    var iCodCarga = this.iCodCatalogo;

    var aoColumns = $grid.fnSettings().aoColumns;
    var idx;
    var maxIdx = aoColumns.length;
    var col;
    var $filtro;
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        $filtro = this.$historic.find("[dataFilterPend='" + col.sName + "']");
        if ($filtro.length > 0) {
            request.sSearch[idx] = $filtro.val();
        }
    }


    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");
    $header.css("width", $grid.css("width"));

    var param = {
        gsRequest: request,
        liTipoDeta: 2,
        liCodCarga: parseInt(iCodCarga + ".0"),
        liCodMaestro: parseInt(iCodMaestroPend + ".0")
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
Historico.prototype.filtrarDeta = function() {
var $grid = this.$historic.find(".DSOGrid[id$='DetaGrid_GridDetaWrapper_Grid']").dataTable({ bRetrieve: true });
    $grid.fnDraw();
}
Historico.prototype.limpiarFiltroDeta = function() {
this.$historic.find("[dataFilterDeta]").each(function() {
        $(this).val("");
    });
    var $filterWrapper = this.$historic.find(".dataTables_filter[id$='DetaGrid_GridDetaWrapper_Grid_filter']");
    var $globalFilter = $filterWrapper.find("input");
    $globalFilter.val("");
    $globalFilter.trigger("keyup.DT");
}

Historico.prototype.filtrarPend = function() {
var $grid = this.$historic.find(".DSOGrid[id$='PendGrid_GridPendWrapper_Grid']").dataTable({ bRetrieve: true });
    $grid.fnDraw();
}
Historico.prototype.limpiarFiltroPend = function() {
    this.$historic.find("[dataFilterPend]").each(function() {
        $(this).val("");
    });
    var $filterWrapper = this.$historic.find(".dataTables_filter[id$='PendGrid_GridPendWrapper_Grid_filter']");
    var $globalFilter = $filterWrapper.find("input");
    $globalFilter.val("");
    $globalFilter.trigger("keyup.DT");
}
Historico.prototype.fnRenderPend = function(obj, imgClass, imgSrc, imgClassE, imgSrcE, sdoPostBack) {
    var iCodRegistro = obj.aData[1];
    var vchBoton = obj.aData[0];
    sdoPostBack = sdoPostBack.replace("{0}", iCodRegistro);
    if (vchBoton == "Edit") {
        var ret = "<a href=\"javascript:" + sdoPostBack + ";\"><img class='" + imgClassE + "' src='" + imgSrcE + "'></a>";
    } else {
        var ret = "<a href=\"javascript:" + sdoPostBack + ";\"><img class='" + imgClass + "' src='" + imgSrc + "'></a>";
    }

    return ret;
}
Historico.prototype.fnServerDetalleE = function($grid, sSource, aoData, fnCallback) {
    var request = DSOControls.Grid.GetRequest(aoData);
    var iCodMaestroDeta = this.$historic.find(".DSODropDownList[id$='iCodMaestroDeta_ddl']").val();
    var iCodCarga = this.iCodCatalogo;

    var aoColumns = $grid.fnSettings().aoColumns;
    var idx;
    var maxIdx = aoColumns.length;
    var col;
    var $filtro;
    for (idx = 0; idx < maxIdx; idx++) {
        col = aoColumns[idx];
        $filtro = this.$historic.find("[dataFilterDeta='" + col.sName + "']");
        if ($filtro.length > 0) {
            request.sSearch[idx] = $filtro.val();
        }
    }

    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $header = $(wrapperID).find(".dataTables_scrollHeadInner > .DSOGrid");

    var param = {
        gsRequest: request,
        liTipoDeta: 3,
        liCodCarga: parseInt(iCodCarga + ".0"),
        liCodMaestro: parseInt(iCodMaestroDeta + ".0")
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