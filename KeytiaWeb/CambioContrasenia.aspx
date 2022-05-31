<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CambioContrasenia.aspx.cs" Inherits="KeytiaWeb.CambioContrasenia" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Lato:wght@300&display=swap" rel="stylesheet"/> 
    <title></title>
    <style type="text/css">

        html {
            background: url('images/background_cambiocontrasenia.png') no-repeat center fixed;     
            background-size: cover;
        }

        .cambio_contraseña {
            padding-top: 150px;
            display: block;
            width: 390px;
            font-family: 'Lato', sans-serif;
            border-radius: 20px;
            margin: 0 auto;
        }

        .form_input {
            border-radius: 25px;
            background: rgba(0, 29, 117, 0.6);
            box-shadow: 0 14px 30px rgba(0, 0, 0, 0.60), 0 7px 10px rgba(0, 0, 0, 0.60);
            border: 2px solid black;
        }

        td {
            padding: 10px;
        }

        tr {
            padding: 5px;
        }

        #message {
            padding-top: 10px;
            padding-left: 5px;
            width: 100%;
            font-size: 15px;
            font-weight: bold;
        }
        @keyframes fadeIn {
            0% {opacity:0;}
            50% {opacity:1;}
            100% {opacity:2;}
        }

        #message p {
            padding-bottom: 5px;
        }

        table {
            border-radius: 25px;
            padding: 5px;
            
        }

        input {
            padding: 5px 0;
            letter-spacing: .5px;
            border: none;
            border-bottom: 1px solid #fff;
            outline: none;
            background-color: transparent;
            transition: all 0.5s;
        }

        input:focus {
            transform: scale(1.05);
        }

        .smallButton {
            border-radius: 5px;
            border: none;
            outline: none; 
            background: #ff652f;
            color: #fff;
            font-size: 11px;
            font-weight: bold;
            padding: 9px 9px;
            letter-spacing: 1px;
            text-transform: uppercase;
            cursor: pointer;
            transition: all 0.5s;
        }

        .smallButton span {
            cursor: pointer;
            display: inline-block;
            position: relative;
            transition: 0.5s;
        }

        .smallButton span:after {
            content: '\00bb';
            position: absolute;
            opacity: 0;
            top: 0;
            right: -20px;
            transition: 0.5s;
        }

        .smallButton:hover span {
            padding-right: 25px;
        }

        .smallButton:hover span:after {
            opacity: 1;
            right: 0;
        }

        .claseUsuario {
            background: none;
            border: none;
        }

        label {
            color: #fff;
        }

        h2 {
            color: #ff652f;
        }

        h5 {
            color: #fff;
        }
    </style>
</head>

<!--- <input type="text" value="Things" readonly="readonly" /> --->

<body>
    <div class="cambio_contraseña">
        <div class="form_input">
            <form id="form_cambio" runat="server">
                <table>
                    <tr>
                        <td colspan="2"><h2><strong>Bienvenido</strong></h2><h5>Realizaremos el cambio de tu contraseña en este primer ingreso</h5></td>
                    </tr>
                    <tr>
                        <td><label for="contraseña">Contraseña</label></td>
                        <td><asp:TextBox ID="contraseña" runat="server" ReadOnly="false" TextMode="Password"/></td>
                    </tr>
                    <tr>
                        <td><label for="confirmacion">Confirmar contraseña</label></td>
                        <td><asp:TextBox ID="confcontraseña" runat="server" ReadOnly="false" TextMode="Password"/></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:LinkButton CssClass="smallButton" ID="submitPwd" runat="server" Text="<span>Continuar </span>" OnClientClick="return userValid();" OnClick="submitPwd_Click" />
                        </td>
                    </tr>
                </table>
            </form>
        </div>
        <div id="message">
        </div>
    </div>

    <script type="text/javascript">

        // Variables en el documento
        let buttonSubmit = document.getElementById("submitPwd");
        let msgError = document.getElementById("message");
        let button = document.getElementById('submitPwd');

        // Logica para modificar el estilo en requisitos y requisitos de contraseñas 

        function errorCheck() {
            let message = "";
            let contraseña = document.getElementById("contraseña").value;
            let confContraseña = document.getElementById("confcontraseña").value;

            if (!(contraseña == confContraseña)) {
                message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">Las contraseñas deben ser iguales</p>"
            }

            if (!(contraseña.length >= 8 && contraseña.length <= 40)) {
                message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">Las contraseñas deben tener una longitud mayor a 8</p>"
            }

            let numbers = /[0-9]/g;
            if (!contraseña.match(numbers) || !confContraseña.match(numbers)) {
                message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">La contraseña debe contener al menos un número</p>";
            }

            let upperLowerLetters = /[A-Za-z]/g;
            if (!contraseña.match(upperLowerLetters) || !confContraseña.match(upperLowerLetters)) {
                message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">La contraseña debe contener al menos una letra</p>";
            }

            msgError.innerHTML = message;
        }

        function userValid() {
            //let mediumRegex = new RegExp("^(((?=.*[a-z])(?=.*[A-Z]))|((?=.*[a-z])(?=.*[0-9]))|((?=.*[A-Z])(?=.*[0-9])))(?=.{6,})")

            //let lightRegex = new RegExp("^[A-Za-z0-9]{8,}$");
            let lightRegex = /^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$/;
            let contraseña = document.getElementById("contraseña").value;
            let confContraseña = document.getElementById("confcontraseña").value;

            if (confContraseña.match(lightRegex) && contraseña.match(lightRegex) && confContraseña == contraseña) {
                msgError.innerHTML = "";
                return true;
            }
            else {
                //alert("Formato incorrecto de contraseña");

                let message = "";
                //let contraseña = document.getElementById("contraseña").value;
                //let confContraseña = document.getElementById("confcontraseña").value;

                if (!(contraseña == confContraseña)) {
                    message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">Las contraseñas deben ser iguales</p>"
                }

                if (!(contraseña.length >= 8 && contraseña.length <= 40)) {
                    message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">Las contraseñas deben tener una longitud mayor a 8</p>"
                }

                let numbers = /[0-9]/g;
                if (!contraseña.match(numbers) || !confContraseña.match(numbers)) {
                    message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">La contraseña debe contener al menos un número</p>";
                }

                let upperLowerLetters = /[A-Za-z]/g;
                if (!contraseña.match(upperLowerLetters) || !confContraseña.match(upperLowerLetters)) {
                    message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">La contraseña debe contener al menos una letra</p>";
                }

                let specialCharacters = /[^\w\s]/g;
                if (contraseña.match(specialCharacters) || confContraseña.match(specialCharacters)) {
                    message += "<p style=\"animation: fadeIn ease 4s; padding-left: 15px;\">Favor de solo incluir letras y números</p>";
                }

                msgError.innerHTML = message;

                return false;
            }
        }
    </script>
</body>
</html>
