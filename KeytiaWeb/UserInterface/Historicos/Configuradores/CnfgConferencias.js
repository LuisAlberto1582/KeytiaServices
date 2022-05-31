Historico.prototype.fnSearchSeeYouOnServicioConferencia = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodCliente = $($($auto.element).attr("clienteId")).val();
    var iniVigencia = self.$container.find(".DSODateTimeBox[dataField='FechaInicioReservacion']").datetimepicker('getDate');
    var finVigencia = self.$container.find(".DSODateTimeBox[dataField='FechaFinReservacion']").datetimepicker('getDate');

    if (iCodCliente === undefined || iCodCliente === "null" || iCodCliente === "")
        iCodCliente = 0;

    var param = { term: term, iCodCliente: iCodCliente, iniVigencia: iniVigencia, finVigencia: finVigencia };
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

Historico.prototype.InitValorClienteChange = function(idCliente, idServicio) {
    var $ctlCliente = $(idCliente);
    var $ctlServicio = $(idServicio);
    $ctlCliente.keypress(function() {
        $ctlServicio.val('');
        $($ctlServicio.attr('textValueID')).val("");
    });
    $ctlCliente.change(function() {
        $ctlServicio.val('');
        $($ctlServicio.attr('textValueID')).val("");
    });
}

Historico.prototype.fnSearchPhoneBookContact = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodConferencia = $($($auto.element).attr("TMSConfId")).val();

    if (iCodConferencia === undefined || iCodConferencia === "null" || iCodConferencia === "") return;

    var param = { term: term, iCodConferencia: iCodConferencia };
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