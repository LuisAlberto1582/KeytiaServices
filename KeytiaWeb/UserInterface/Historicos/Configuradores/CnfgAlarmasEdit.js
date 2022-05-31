var patt = /^[a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}$/i;
var $field;
var email;
var Alarmas = {
    Init: function() {
        $(".DSOTextBox").blur(Alarmas.ValidarEmail);
    },

    ValidarEmail: function() {
        $field = $(this);
        email = $field.val().split(";");
        if ($field.attr("email") == "EmailEmisor") {
            Alarmas.ValidarEmailEmisor();
        }
        if ($field.attr("email") == "EmailReceptor") {
            Alarmas.ValidarEmailReceptor();
        }
        if ($field.attr("email") == "EmailOpcional") {
            Alarmas.ValidarEmailOpcional();
        }
    },

    ValidarEmailEmisor: function() {
        if (email[0] != "" && email.length == 1) {
            Alarmas.Validar();
        }
        else {
            $field.focus();
        }
    },

    ValidarEmailReceptor: function() {
        if (email[0] != "") {
            Alarmas.Validar();
        }
        else {
            $field.focus();
        }
    },

    ValidarEmailOpcional: function() {
        if (email[0] != "") {
            Alarmas.Validar();
        }
    },

    Validar: function() {
        for (i = 0; i < email.length; i++) {
            if (!patt.test(email[i]) && email.length != 0) {
                $field.focus();
            }
        }
    }
};

//Inicializacion General-------------------------------------------------------------------------
$(document).ready(function() {
    Alarmas.Init();
});