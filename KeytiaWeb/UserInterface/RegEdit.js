var estado = false;
var RegEdit = {
    Confirmar: false,
    Botones: {
        setEstado: function() {
            var disabled = true;
            if ($(".DSOAutocompleteVal[id$=iCodRegistro_txt]").val() !== '') disabled = false;
            $(".buttonSearch").button(disabled ? "disable" : "enable");
            $(".buttonEdit").button(disabled ? "disable" : "enable");
            $(".buttonDelete").button(disabled ? "disable" : "enable");
            if ($(".buttonAdd").length > 0) {
                var $Entidad = $(".DSODropDownList[id$=iCodEntidad_ddl]");
                var iCodEntidad = "";
                if ($Entidad.length > 0) {
                    iCodEntidad = $Entidad.val();
                    disabled = !disabled || (iCodEntidad === "null");
                }
                else {
                    disabled = !disabled;
                }
                if (!estado) {
                    $(".buttonAdd").button("option", "disabled", estado);
                    estado = true;
                }
            }
        },
        btnBaja_Click: function(msg, titulo, doPostBack) {
            $(this).button("disable");
            jConfirm(msg, titulo,
            function(r) {
                if (r) {
                    doPostBack();
                }
                else {
                    $(this).button("enable");
                }
            });
            return false;
        },
        btnCancelar_Click: function(msg, titulo, doPostBack) {
            if (RegEdit.Confirmar) {
                jConfirm(msg, titulo,
                function(r) {
                    if (r) {
                        doPostBack();
                    }
                });
            }
            else {
                doPostBack();
            }
            return false;
        }
    },
    DSODropDownList: {
        Focus: function() {
            var value = $(this).val();
            var tipoCampo = $(this).attr("tipoCampo");
            var $Entidad = $(".DSODropDownList[id$=iCodEntidad_ddl]");
            var iCodEntidad = "null";
            if ($Entidad.length > 0)
                iCodEntidad = $Entidad.val();
            var Excluir = [];
            var $others = $(".DSODropDownList[tipoCampo=" + tipoCampo + "]:not(#" + $(this).attr("id") + ")");
            $others.each(function() {
                if ($(this).val() !== "null")
                    Excluir.push($(this).val());
            });

            var params = { lsNombreTabla: "Maestros", tipoCampo: tipoCampo, iCodEntidad: iCodEntidad, Excluir: Excluir.join(",") };
            var ajaxPage = KeytiaMaster.appPath + "WebMethods.aspx/GetDataSource";

            var options = {
                type: "POST",
                url: ajaxPage,
                data: JSON.stringify(params),
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                async: true,
                context: $(this)[0],
                success: DSOControls.DropDown.Fill,
                error: DSOControls.ErrorAjax
            };
            $.ajax(options);
            return false;
        }
    },
    ShowNextRow: function() {
        var $ddl = $(this);
        if ($ddl.length == 0) return;
        var recursive = false;
        var tipoCampo = $ddl.attr("tipoCampo");
        var $td = $ddl.closest("td");
        var $nexRen = $td.next("td").next("td").find(".DSODropDownList[datafield=" + $ddl.attr("datafield") + "Ren" + "]");
        var $nexCol = $td.next("td").next("td").next("td").next("td").find(".DSODropDownList[datafield=" + $ddl.attr("datafield") + "Col" + "]");
        var $nexReq = $td.next("td").next("td").next("td").next("td").next("td").next("td").find(".DSOCheckBox[datafield=" + $ddl.attr("datafield") + "Req" + "]").find("input");
        var $tr = $ddl.closest("tr");
        var $nextDDL = $tr.next("tr").find(".DSODropDownList[tipoCampo=" + tipoCampo + "]");
        if ($ddl.val() == "null") {
            $nexRen.attr("disabled", true);
            $nexCol.attr("disabled", true);
            $nexReq.attr("disabled", true);
            $nextDDL.closest("tr").hide();
            if ($nextDDL.val() !== "null") {
                $nextDDL.val("null");
                $nextDDL.change();
                $ddl.focus();
            }
        }
        else {
            $nexRen.removeAttr("disabled");
            $nexCol.removeAttr("disabled");
            $nexReq.removeAttr("disabled");
            $nextDDL.closest("tr").show();
        }
    },
    HideDetails: function() {
        if ($(".DSOAutocompleteVal[id$=iCodRegistro_txt]").val() == '') {
            $(".DSODropDownList[tipoCampo], .DSOCheckBox, .DSODateTimeBox, .DSOTextBox").closest("tr").hide();
        }
    },
    Init: function() {
        $(".DSOAutocompleteVal[id$=iCodRegistro_txt]").change(RegEdit.HideDetails);
        $(".DSOAutocomplete[id$=iCodRegistro_srch]").keydown(function() {
            RegEdit.Botones.setEstado();
            RegEdit.HideDetails();
        });
        $(".DSOAutocomplete[id$=iCodRegistro_srch]").bind('blur', function() {
            RegEdit.Botones.setEstado();
            RegEdit.HideDetails();
        });
        $(".DSODropDownList[id$=iCodEntidad_ddl]").change(function() {
            $(".DSOAutocompleteVal[id$=iCodRegistro_txt]").val('');
            RegEdit.Botones.setEstado();
            RegEdit.HideDetails();
        });
        RegEdit.Botones.setEstado();
        if ($(".buttonSave").length == 0) return;
        $(".DSODropDownList[tipoCampo]").change(RegEdit.ShowNextRow);
        $(".DSODropDownList[tipoCampo]").change();
        $(".DSODropDownList[tipoCampo], .DSODateTimeBox, .DSOCheckBox, .DSOTextBox").change(function() { RegEdit.Confirmar = true; });
        $(".DSODropDownList[tipoCampo]").focus(RegEdit.DSODropDownList.Focus);
    }
};

//Inicializacion General-------------------------------------------------------------------------
$(document).ready(function() {
    RegEdit.Init();
});