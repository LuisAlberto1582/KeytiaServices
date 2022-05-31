Historico.prototype.cambioPeriodoEtiquetaExt = function(inst, ctlOrigen, ctlDestino, monthAdd, month, year) {
    DSOControls.DateTimeBox.setMonthDay.call($(ctlOrigen), year, month, inst);
    var $ctrlOrigen = $(ctlOrigen);
    var $ctrlDestino = $(ctlDestino);
    var dateDestino = $ctrlOrigen.datetimepicker('getDate');
    dateDestino.setMonth(dateDestino.getMonth() + monthAdd);
    $ctrlDestino.datetimepicker('setDate', dateDestino);
}