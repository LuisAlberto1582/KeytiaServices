Historico.prototype.fnSearchAtribControl = function($auto, request, response) {
    var self = this;
    var term = request.term;
    var iCodType = $($($auto.element).attr("tipoDatoId")).val();

    if (iCodType === undefined || iCodType === "null" || iCodType === "") return;

    var param = { term: term, iCodType: iCodType };
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

Historico.prototype.InitAtribTypeChange = function(idType, idControl) {
    var $ctlType = $(idType);
    var $ctlControl = $(idControl);
    $ctlType.keypress(function() {
        $ctlControl.val('');
        $($ctlControl.attr('textValueID')).val("");
    });
    $ctlType.change(function() {
        $ctlControl.val('');
        $($ctlControl.attr('textValueID')).val("");
    });
}