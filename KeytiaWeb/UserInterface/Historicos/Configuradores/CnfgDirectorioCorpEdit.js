Historico.prototype.aplicaEmpleParticular = function($ddl) {
    var $empleParticular = this.$historic.find("[id$=VarChar02_Grid]");
    var $chkTodos = this.$historic.find("[id$=VarChar02_todos_chck]");
    var bValor = false;

    if ($ddl.find("input[type='checkbox']")[1].checked == true) {
        bValor = true;
    }

    $empleParticular.find("input:checkbox").each(function() {
        this.disabled = bValor;
    });
    $empleParticular[0].disabled = bValor;
    $chkTodos[0].disabled = bValor;
}