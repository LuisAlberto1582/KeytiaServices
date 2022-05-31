<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginWFA.aspx.cs" Inherits="KeytiaWeb.LoginWFA" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
                        <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
                        EnableScriptGlobalization="true">
                        </asp:ToolkitScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                     <ContentTemplate>
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
                            <div class="form-horizontal portlet" style="background-color:none;">
                                <div class="row" >
                                    <div class="form-horizontal">
                                        <asp:Panel ID="rowNomina" runat="server" CssClass="form-group">
                                        <asp:Label runat="server" ID="lblNomina" CssClass="col-sm-4 control-label" style="font-weight:bold; font-size:16px;">N° Empleado: </asp:Label>
                                        <div class="col-sm-6">
                                             <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="N° Empleado" style="height:35px;"></asp:TextBox>
                                        </div>
                                        </asp:Panel>
                                        <%-- %><asp:Panel ID="rowBtnBuscar" runat="server" CssClass="form-group">
                                            <div class="col-sm-offset-2 ">
                                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" style="box-shadow:none!important;outline:0!important;color:white;background-color:#009639;border:1px solid #6ad42f;line-height:30px !important;height:35px;font-size:18px !important;font-family:sans-serif;font-weight:100;padding:0px 60px;display:inline-block;text-align:center;vertical-align:middle;touch-action:manipulation;white-space:nowrap;user-select:none;border-radius:6px;" 
                                                    OnClick="btnBuscar_Click"/>
                                            </div>
                                        </asp:Panel>--%>
                                        <asp:Panel ID="rowEmpresa" runat="server" CssClass="form-group">
                                        <asp:Label runat="server" ID="lblEmpresa" CssClass="col-sm-4 control-label" style="font-weight:bold; font-size:16px;">Empresa: </asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList runat="server" ID="cboEmpresa" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control" style="height:35px" Enabled="true" >
                                            </asp:DropDownList>
                                       </div>
                                    </asp:Panel>
                                     </div> 
                                    </div>
                                <div class="row" runat="server" id="row1" visible="true">
                                   <asp:Panel ID="rowAceptar" runat="server" CssClass="form-group">
                                        <div class="col-sm-offset-2 col-sm-8">
                                            <asp:Button ID="BtnIngresar" runat="server" OnClick="BtnIngresar_Click" style="box-shadow:none!important;outline:0!important;color:white;background-color:#009639;border:1px solid #6ad42f;line-height:30px !important;height:35px;font-size:18px !important;font-family:sans-serif;font-weight:100;padding:0px 60px;display:inline-block;text-align:center;vertical-align:middle;touch-action:manipulation;white-space:nowrap;user-select:none;border-radius:6px;" Text="Ingresar" role="button" Enabled="true"  Visible="true"/>
                                        </div>
                                   </asp:Panel>
                                    <%--<asp:Panel ID="rowEmpleado" runat="server" CssClass="form-group">
                                        <asp:Label runat="server" ID="Label3" CssClass="col-sm-4 control-label" style="font-weight:bold; font-size:16px;">Empleado: </asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList runat="server" ID="cboEmpleado" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="NomCompleto" CssClass="col-sm-2 form-control" style="height:35px" Enabled="false">
                                            </asp:DropDownList>
                                       </div>
                                    </asp:Panel>--%>

                                    <%--<asp:Panel ID="rowDireccion" runat="server" CssClass="form-group">
                                        <asp:Label runat="server" ID="Label1" CssClass="col-sm-4 control-label" style="font-weight:bold; font-size:16px;">Dirección: </asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList runat="server" ID="cboDireccion" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control" style="height:35px" Enabled="false" OnSelectedIndexChanged="cboDireccion_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                       </div>
                                    </asp:Panel>
                                    <asp:Panel ID="rowCencos" runat="server" CssClass="form-group">
                                        <asp:Label runat="server" ID="Label2" CssClass="col-sm-4 control-label" style="font-weight:bold; font-size:16px;">Centro de Costos: </asp:Label>
                                        <div class="col-sm-6">
                                            <asp:DropDownList runat="server" ID="cboCenCos" AppendDataBoundItems="true" DataValueField="iCodCatalogo" DataTextField="Descripcion" CssClass="col-sm-2 form-control" style="height:35px" Enabled="false" AutoPostBack="true" OnSelectedIndexChanged="cboCenCos_SelectedIndexChanged">
                                            </asp:DropDownList>
                                       </div>
                                    </asp:Panel>--%>
                                </div>
                            </div>
                            <br />
                            <%--<asp:LinkButton ID="hlOlvidoPassword" runat="server" CssClass="forgotYourPassword" OnClick="hlOlvidoPassword_Click"></asp:LinkButton>--%>
                        </div>
                    </div>
                    </ContentTemplate>
                    </asp:UpdatePanel>
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
