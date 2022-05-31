var Maestros = {
    Autocomplete: {
        Source: function(request, response) {
            var iCodEntidad = $($(this.element[0]).attr("iCodEntidad")).val();
            if (iCodEntidad === "null") return;

            var term = request.term;

            var param = { term: term, iCodEntidad: iCodEntidad };
            var options = {
                type: "POST",
                url: $(this.element).attr("source"),
                data: JSON.stringify(param),
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                async: true,
                context: this.element[0],
                success: function(data, status, xhr) {
                    var results = (typeof data.d) == 'string' ?
                        eval('(' + data.d + ')') :
                        data.d;

                    if (xhr === this.lastXhr) {
                        response(results);
                    }
                },
                error: DSOControls.ErrorAjax
            };
            this.element[0].lastXhr = $.ajax(options);
        }
    },
    ClearRowCol: function() {
        if ($(this).val() === "null") {
            var $tr = $(this).closest("tr");
            var $others = $tr.find(".DSODropDownList:not([tipoCampo])");
            $others.val("null");
            var $nextDDL = $tr.next("tr").find(".DSODropDownList[tipoCampo=" + $(this).attr("tipoCampo") + "]");
            if ($nextDDL.length > 0)
                Maestros.ClearRowCol.call($nextDDL);
        }
    }
}
$(document).ready(function() {
    $(".DSODropDownList[tipoCampo]").change(Maestros.ClearRowCol);
});