﻿Historico.prototype.fnServerHistoricos = function($grid, sSource, aoData, fnCallback, iCodEntidad, iCodMaestro, parametros, dataFilter) {
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

Historico.prototype.fnSubHisInitComplete = function($grid) {
    var $btnAdd = $($grid.attr("buttonAdd"));
    var wrapperID = "#" + $grid.attr("id") + "_wrapper";
    var $toolbar = $(wrapperID).find(".dataTables_length");
    $toolbar.prepend($btnAdd);

    var $expandable = $grid.closest(".DSOExpandable");
    var maxwidth = $expandable.width();
    $(wrapperID).css("max-width", maxwidth);
    $grid.fnAdjustColumnSizing();
    this.$grid = $grid;
}