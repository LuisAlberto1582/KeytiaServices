Historico.prototype.fnSearchConsulta = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodAtributo = $($($auto.element).attr("atribId")).val();
    
    if (iCodAtributo === undefined || iCodAtributo === "null" || iCodAtributo === "") return;

    var param = { term: term, iCodAtributo: iCodAtributo};
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

Historico.prototype.fnServerDataValorConsulta = function($grid, sSource, aoData, fnCallback, iCodAtributo) {
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

   //public static DSOGridServerResponse GetValorConsultasData(DSOGridServerRequest gsRequest, int iCodAtributo, int iCodEntidad, int iCodMaestro)

    var param = {
        gsRequest: request,
        iCodEntidad: parseInt(iCodEntidad + ".0"),
        iCodMaestro: parseInt(iCodMaestro + ".0"),
        iCodAtributo: iCodAtributo
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

Historico.prototype.InitValorConsultasChange = function(idAtrib, idConsul) {
    var $ctlAtrib = $(idAtrib);
    var $ctlConsul = $(idConsul);
    $ctlAtrib.keypress(function() {
        $ctlConsul.val('');
        $($ctlConsul.attr('textValueID')).val("");
    });
    $ctlAtrib.change(function() {
        $ctlConsul.val('');
        $($ctlConsul.attr('textValueID')).val("");
    });
}