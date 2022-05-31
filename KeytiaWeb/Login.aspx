<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="KeytiaWeb.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Keytia</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta content="" name="description" />
    <meta content="" name="author" />
    <!-- BEGIN GLOBAL MANDATORY STYLES -->
    <link href="https://fonts.googleapis.com/css?family=Poppins:300,300i,400,400i,500,500i,700,700i" rel="stylesheet" />
    <link href="scripts/assets/global/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="styles/default/css/keytia.css" rel="stylesheet" type="text/css" />
    <!--ga-->
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-44396842-1']);
        _gaq.push(['_setDomainName', 'dti.com.mx']);
        _gaq.push(['_setAllowLinker', true]);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>
</head>
<body>
    <video autoplay muted loop class="videoLogin">
        <source src="../img/analytics.mp4" type="video/mp4" />
    </video>


    <div class="container">
        <asp:Panel ID="pnl" runat="server" CssClass="row">
            <asp:Panel ID="pnlLogin" runat="server" CssClass="col-xs-10 col-sm-8 col-md-6 col-lg-6 col-xl-6 centeredLogin">
                <!-- BEGIN LOGIN -->
                <form id="form1" runat="server" action="">
                    <div class="thumbnail loginThumbnail">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/img/logo-keytia@2x.png" alt="..." />
                        <div class="caption text-center">
                            <h3>
                                <asp:Literal ID="lblIntDat" runat="server"></asp:Literal>
                            </h3>

                            <asp:Panel ID="pnlAlerta" runat="server" CssClass="alert alert-danger" Visible="false">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close" aria-hidden="true">
                                    x
                                </button>
                                <span>
                                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                                </span>
                            </asp:Panel>

                            <asp:TextBox ID="txtUsuario" runat="server" MaxLength="40" CssClass="form-control userLogin" placeholder="Usuario"></asp:TextBox>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="32" CssClass="form-control passLogin" placeholder="Contraseña"></asp:TextBox>

                            <p>
                                <asp:Button ID="BtnIngresar" runat="server" OnClick="BtnIngresar_Click" CssClass="btn btn-default btnLogin" Text="Login" role="button" />
                            </p>

                            <asp:LinkButton ID="hlOlvidoPassword" runat="server" CssClass="forgotYourPassword" OnClick="hlOlvidoPassword_Click"></asp:LinkButton>
                        </div>
                    </div>
                </form>
                <!-- END LOGIN -->
                <!-- Aqui va la recuperacion de la contraseña-->
            </asp:Panel>
            <asp:Panel ID="pnlRecuperacion" runat="server" CssClass="col-xs-10 col-sm-8 col-md-6 col-lg-6 col-xl-6 centeredLogin" Visible="false">
                <!-- BEGIN FORGOT PASSWORD FORM -->
                <form id="form2" runat="server" method="post">
                    <div class="thumbnail loginThumbnail">
                        <div class="caption text-center">
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/img/logo-keytia@2x.png" alt="..." />
                            <h3>
                                <asp:Literal ID="lblRecPsdUsr" runat="server"></asp:Literal>
                            </h3>

                            <asp:Panel ID="pnlAlertaRecuperacion" runat="server" CssClass="alert alert-danger" Visible="false">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close" aria-hidden="true">
                                    x
                                </button>
                                <span>
                                    <asp:Label ID="lblMensajeAlerta" runat="server"></asp:Label>
                                </span>
                            </asp:Panel>


                            <asp:TextBox ID="txtUsuarioRecuperacion" runat="server" MaxLength="40" CssClass="form-control userLogin" placeholder="Usuario"></asp:TextBox>
                            <asp:TextBox ID="txtEmailRecuperacion" runat="server" MaxLength="40" CssClass="form-control userLogin" placeholder="Email"></asp:TextBox>

                            <p>
                                <asp:Button ID="btnVolver" runat="server" CssClass="btn btn-default btnLogin" Text="Volver" role="button" OnClick="btnVolver_Click" />
                                <asp:Button ID="btnEnviar" runat="server" CssClass="btn btn-default btnLogin" Text="Enviar" role="button" OnClick="btnEnviar_Click" />
                            </p>
                        </div>
                    </div>
                </form>
                <!-- END FORGOT PASSWORD FORM -->
            </asp:Panel>
        </asp:Panel>
    </div>


    <footer class="footerLogin">
        <div class="container">
            <div class="text-center">
                <div class="copyright" style="margin-top: 15px;"><a href="http://www.dti.com.mx">Keytia® <span id="yearInCourse"><%=DateTime.Now.Year%></span> by DTI</a></div>
            </div>
        </div>
    </footer>

    <script src="scripts/assets/global/plugins/jquery.min.js" type="text/javascript"></script>
    <script src="scripts/assets/global/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>

</body>
</html>
