Historico.prototype.deshabilitarAddButtons = function() {
    var self = this;
    var $addButtons = self.$historic.find(".buttonAdd");

    $addButtons.each(function() {
        var $addButton = $(this);
        $addButton.button("disable");
    });
}

Historico.prototype.filtrarTarifas = function() {
    var self = this;
    var $planServ = self.$historic.find(".DSOAutocomplete[keytiaField='PlanServ']");
    var $region = self.$historic.find(".DSOAutocomplete[keytiaField='Region']");
    var $addButtons = self.$historic.find(".buttonAdd");
    var $grids = this.$historic.find(".DSOGrid[id$='_Grid']");

    $grids.each(function() {
        var $grid = $(this).dataTable({ bRetrieve: true });
        $grid.fnDraw();
    });

    if ($planServ.val() == "" || $region.val() == "" || self.bPermisoAgregar != "True") {
        self.deshabilitarAddButtons()
        return;
    }

    $addButtons.each(function() {
        var $addButton = $(this);
        if (self.bPermisoAgregar == "True") {
            $addButton.button("enable");
        }
    });
}

Historico.prototype.deshabilitarRegistro = function() {
    var self = this;
    var $expRegistro = self.$historic.find(".DSOExpandable[id$='RegWrapper_pnl']");
    var $planServ = self.$historic.find(".DSOAutocomplete[keytiaField='PlanServ']");
    var $region = self.$historic.find(".DSOAutocomplete[keytiaField='Region']");
    var $addButtons = self.$historic.find(".buttonAdd");

    $expRegistro[0].style.visibility = "hidden";

    if ($planServ.val() == "" || $region.val() == "" || self.bPermisoAgregar != "True") {
        self.deshabilitarAddButtons()
        return;
    }
}

