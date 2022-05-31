Historico.prototype.fnSearchEmpleByClient = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodCliente = $($auto.element).attr("clienteId");
    var iniVigencia = self.$ctlIniVigencia.datetimepicker('getDate');
    var finVigencia = self.$ctlFinVigencia.datetimepicker('getDate');

    if (iCodCliente === undefined || iCodCliente === "null") return;

    if (iniVigencia === undefined) {
        iniVigencia = null;
    }

    if (finVigencia === undefined) {
        finVigencia = null;
    }

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
