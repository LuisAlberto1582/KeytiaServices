var Cat = {
    wdID: "",
    editando: false,
    confirmar: false,
    confirmarTitulo: "",
    confirmarCambios: "",
    finEditar: function() {
        $(Cat.wdID).closeWindow();
    },
    setConfirmar: function() {
        Cat.confirmar = true;
    },
    cancelar: function() {
        if (Cat.confirmar) {
            jConfirm(Cat.confirmarCambios, Cat.confirmarTitulo, function(r) {
                if (!r) {
                    DSOControls.Window.Init.call($(Cat.wdID)[0]);
                    $(Cat.wdID).showWindow();
                }
                else {
                    Cat.editando = false;
                }
            });
        }
        else {
            Cat.editando = false;
        }
    },
    initEditor: function(wdID, bAgregar) {
        Cat.wdID = wdID;
        Cat.editando = true;
        Cat.confirmar = true;
        var $btnBaja = $(wdID).find("button[btnBaja]");
        var $ctlCod = $(wdID).find(".DSOTextBox[dataField='vchCodigo']");
        var $ctlDesc = $(wdID).find(".DSOTextBox[dataField='vchDescripcion']");
        
        if (bAgregar) {
            $btnBaja.css("display", "none");
        }
        else {
            $btnBaja.css("display", "");
            $ctlCod.attr("disabled", true);
            $ctlDesc.attr("disabled", true);            
        }

        DSOControls.Window.Init.call($(wdID)[0]);
        $(wdID).showWindow();
    },
    editar: function(wdID, wdTitle, iCodReg) {
        if (Cat.editando) {
            return;
        }
        Cat.editando = true;
        Cat.wdID = wdID;
        $(wdID).attr("title", wdTitle);
        var $btnBaja = $(wdID).find("button[btnBaja]");
        var $ctlCod = $(wdID).find(".DSOTextBox[dataField='vchCodigo']");
        var $ctlDesc = $(wdID).find(".DSOTextBox[dataField='vchDescripcion']");

        if (iCodReg !== undefined) {
            var param = { iCodRegistro: iCodReg };
            $btnBaja.css("display", "");
            var options = {
                type: "POST",
                url: KeytiaMaster.appPath + "WebMethods.aspx/GetCatReg",
                data: JSON.stringify(param),
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                async: true,
                context: $(wdID)[0],
                success: function(data, textStatus, jqXHR) {
                    DSOControls.LoadContainerAjax.call(this, data, textStatus, jqXHR);
                    DSOControls.Window.Init.call(this);
                    $ctlCod.attr("disabled", true);
                    $ctlDesc.attr("disabled", true);
                    Cat.confirmar = false;
                    $(this).showWindow();
                },
                error: DSOControls.ErrorAjax
            };
            $.ajax(options);
        }
        else {
            $btnBaja.css("display", "none");
            DSOControls.CleanContainer.call($(wdID)[0]);
            DSOControls.Window.Init.call($(wdID)[0]);
            $ctlCod.removeAttr("disabled");
            $ctlDesc.removeAttr("disabled");
            Cat.confirmar = false;
            $(wdID).showWindow();
        }
    },
    borrar: function(msg, titulo, doPostBack) {
        jConfirm(msg, titulo, function(r) {
            if (r) {
                doPostBack();
            }
            else {
                Cat.editando = false;
                Cat.confirmar = false;
                Cat.finEditar();
            }
        });
    },
    fnServerData: function(sSource, aoData, fnCallback) {
        var request = DSOControls.Grid.GetRequest(aoData);
        var param = { gsRequest: request };
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
    },
    fnInitComplete: function() {
        var $btnAgregar = $("[id$='CatalogEdit']").find(".buttonAdd");
        var $toolbar = $("[id$='CatGrid_Grid_wrapper']").find(".dataTables_length");
        $toolbar.prepend($btnAgregar);
    }
}
