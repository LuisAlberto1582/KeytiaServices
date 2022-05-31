<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AltaNuevoEsquema.aspx.cs" Inherits="KeytiaWeb.UserInterface.Administracion.NuevoEsquema.AltaNuevoEsquema" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 9999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }

        .classButton {
            background-color: transparent;
            width: 0px;
            height: 0px;
            border: none;
            display: none;
        }
    </style>
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
   
    <script type="text/javascript">

        var pagePath = window.location.pathname;
        var dataJSON;
        var fieldDirCargas;

        $(window).load(function () {
            fieldDirCargas = document.getElementById("<%=txtDirCargas.ClientID%>").value;
        });

        function CreaEsquema() {

            var fieldEsquema = document.getElementById("<%=txtEsquema.ClientID%>").value;
            var fieldNameServer = document.getElementById("<%=txtServerCargas.ClientID%>").value;
            var fieldDirectorio = document.getElementById("<%=txtDirCargas.ClientID%>").value;
            var fieldClaveUser = document.getElementById("<%=txtClaveUser.ClientID%>").value;
            var fieldPassUser = document.getElementById("<%=txtPasswordUser.ClientID%>").value;
            var fieldLogoCliente = document.getElementById("<%=fUploadLogo.ClientID%>").value;
            var fieldClaveOper = document.getElementById("<%=txtClaveOper.ClientID%>").value;
            var fieldPassOper = document.getElementById("<%=txtPassOper.ClientID%>").value;
            var fieldEmail = document.getElementById("<%=txtEmailSyop.ClientID%>").value;

            if (fieldEsquema != '' && fieldNameServer != '' && fieldDirectorio != '' && fieldClaveUser != ''
                && fieldPassUser != '' && fieldLogoCliente != '' && fieldClaveOper != '' && fieldPassOper != '' && fieldEmail != '') {


                var btn = document.getElementById("btn2");
                btn.click();

            }
            else {

                if (fieldEsquema == '') { document.getElementById("<%=rfvtxtEsquema.ClientID%>").style.display = 'block'; }
                if (fieldNameServer == '') { document.getElementById("<%=rfvServerCargas.ClientID%>").style.display = 'block' }
                if (fieldDirectorio == '') { document.getElementById("<%=rfvDirCargas.ClientID%>").style.display = 'block' }
                if (fieldClaveUser == '') { document.getElementById("<%=rfvClaveUser.ClientID%>").style.display = 'block' }
                if (fieldPassUser == '') { document.getElementById("<%=rfvPasswordUser.ClientID%>").style.display = 'block' }
                if (fieldLogoCliente == '') { document.getElementById("<%=rfvUpload.ClientID%>").style.display = 'block' }
                if (fieldClaveOper == '') { document.getElementById("<%=rfvClaveOper.ClientID%>").style.display = 'block' }
                if (fieldPassOper == '') { document.getElementById("<%=rfvPassOper.ClientID%>").style.display = 'block' }
                if (fieldEmail == '') { document.getElementById("<%=rfvEmail.ClientID%>").style.display = 'block' }
            }

        }

        function copiar() {
            var fieldEsquema = document.getElementById("<%=txtEsquema.ClientID%>").value.replace(/ /g, "");
            document.getElementById("<%=txtEsquema.ClientID%>").value = fieldEsquema;
            if (fieldEsquema != "") {
                var newDir = fieldDirCargas + fieldEsquema;
                document.getElementById("<%=txtDirCargas.ClientID%>").value = newDir;
            }


        }

        function AltaEsquema() {

            var fieldEsquema = document.getElementById("<%=txtEsquema.ClientID%>").value;

            $.ajax({
                url: pagePath + "/GetEsquema",
                data: "{ 'texto': '" + fieldEsquema + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    dataJSON = JSON.parse(data.d)
                    $.map(dataJSON, function (item) {
                        sessionStorage.clear();
                        sessionStorage.setItem('existe', item.Descripcion);

                        var existeEsquema = item.Descripcion;

                        if (existeEsquema == 0) {
                            document.getElementById('footer').style.display = 'block'
                            document.getElementById('lblMensaje').innerHTML = "¿Esta seguro de Crear el Cliente: " + fieldEsquema + "?";

                        }
                        else {

                            document.getElementById('lblMensaje').innerHTML = "Ya existe un cliente con el nombre de Esquema: " + fieldEsquema + ", en la base de datos.";
                            document.getElementById('footer').style.display = 'none'
                        }

                    });
                },
                error: function (XMLHttpRequest, callStatus, errorThrown) {
                    document.getElementById('lblMensaje').innerHTML = "Ocurrio un error al validar el nuevo Cliente";
                    document.getElementById('footer').style.display = 'none'
                }
            });

        }
    </script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script type="text/javascript">
        function mostrarPassword() {
            var cambio = document.getElementById("<%=txtPasswordUser.ClientID%>");
            var cambio2 = document.getElementById("<%=txtPassOper.ClientID%>");

            if (cambio.type == "password") {
                cambio.type = "text";
                $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
            } else {
                cambio.type = "password";
                $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
            }

            if (cambio2.type == "password") {
                cambio2.type = "text";
                $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
            } else {
                cambio2.type = "password";
                $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="modalUpload">
                <div class="centerUpload">
                    <asp:Image class="center" runat="server" ID="imgCod" ImageUrl="~/images/loader2.gif" ToolTip="Procesando" />
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="pnlMainHolder" runat="server">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Crear Nuevo Cliente</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <strong>Datos del Esquema: </strong>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="col-sm-11">
                                                        <div class="form-group" id="frmName">
                                                            <asp:Label runat="server" CssClass="col-sm-3 control-label">Nombre:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtEsquema" CssClass="form-control" onkeyup="copiar();" autocomplete="off"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvtxtEsquema" runat="server" Display="Dynamic"
                                                                    ErrorMessage="Ingrese un nombre de Esquema." ControlToValidate="txtEsquema" CssClass="text-danger"
                                                                    SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                </asp:RequiredFieldValidator>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" CssClass="col-sm-3 control-label">Nombre del Server CargasCDR:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtServerCargas" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvServerCargas" runat="server" ErrorMessage="Ingrese el Nombre del Server de Cargas."
                                                                    ControlToValidate="txtServerCargas" Display="Dynamic" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                </asp:RequiredFieldValidator>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" CssClass="col-sm-3 control-label">Directorio de Cargas:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtDirCargas"  CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvDirCargas" runat="server" ErrorMessage="Ingrese el Directorio de Cargas."
                                                                    ControlToValidate="txtDirCargas" Display="Dynamic" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                </asp:RequiredFieldValidator>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" CssClass="col-sm-3 control-label">Logo Cliente:</asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:FileUpload runat="server" ID="fUploadLogo" ValidateRequestMode="Enabled" />
                                                                <asp:RequiredFieldValidator ID="rfvUpload" ErrorMessage="Debe Seleccionar Un archivo" ControlToValidate="fUploadLogo"
                                                                    runat="server" Display="Dynamic" CssClass="text-danger" ValidationGroup="UpdatePanel1"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:Label runat="server" CssClass="col-sm-3 control-label">Email Usuario: </asp:Label>
                                                            <div class="col-sm-8">
                                                                <asp:TextBox runat="server" ID="txtEmailSyop" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Ingrese un Email." 
                                                                    ControlToValidate="txtEmailSyop" Display="Dynamic" CssClass="text-danger" SetFocusOnError="false" ValidationGroup="UpdatePanel1">
                                                                </asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                                                    ControlToValidate="txtEmailSyop" ErrorMessage="Email con Formato Incorrrecto." ValidationGroup="UpdatePanel1">
                                                                </asp:RegularExpressionValidator>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="col-sm-6">
                                                <div class="panel panel-default">
                                                    <div class="panel-heading">
                                                        <strong>Datos del Usuario: </strong>
                                                    </div>
                                                    <div class="panel-body">
                                                        <div class="col-sm-12">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" CssClass="col-sm-3 control-label">Clave:</asp:Label>
                                                                <div class="col-sm-8">
                                                                    <asp:TextBox runat="server" ID="txtClaveUser" CssClass="form-control" ></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="rfvClaveUser" runat="server" ErrorMessage="Ingrese la Clave de Usuario."
                                                                        ControlToValidate="txtClaveUser" Display="Dynamic" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                    </asp:RequiredFieldValidator>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label runat="server" CssClass="col-sm-3 control-label">Contraseña:</asp:Label>
                                                                <div class="col-sm-8">
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtPasswordUser" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                                                        <div class="input-group-addon">
                                                                            <span class="fa fa-eye-slash icon" onclick="mostrarPassword()" style="cursor:hand;"></span>
                                                                        </div>
                                                                    </div>

                                                                    <asp:RequiredFieldValidator ID="rfvPasswordUser" runat="server" ErrorMessage="Ingrese el Password del Usuario."
                                                                        ControlToValidate="txtPasswordUser" Display="Dynamic" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                    </asp:RequiredFieldValidator>
                                                                </div>
                                                            </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            <div class="col-sm-6">
                                                <div class="panel panel-default">
                                                    <div class="panel-heading">
                                                        <strong>Datos del Usuario Operaciones: </strong>
                                                    </div>
                                                    <div class="panel-body">
                                                        <div class="col-sm-12">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" CssClass="col-sm-3 control-label">Clave:</asp:Label>
                                                                <div class="col-sm-8">                                                                    
                                                                        <asp:TextBox runat="server" ID="txtClaveOper" CssClass="form-control"></asp:TextBox>                                                               
                                                                    <asp:RequiredFieldValidator ID="rfvClaveOper" runat="server" ErrorMessage="Ingrese la Clave del Usuario Operaciones."
                                                                        ControlToValidate="txtClaveOper" Display="Dynamic" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                    </asp:RequiredFieldValidator>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label runat="server" CssClass="col-sm-3 control-label">Contraseña:</asp:Label>
                                                                <div class="col-sm-8">
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtPassOper" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                                                        <div class="input-group-addon">
                                                                            <span class="fa fa-eye-slash icon" onclick="mostrarPassword()" style="cursor:hand;"></span>
                                                                        </div>
                                                                    </div>
                                                                    
                                                                    <asp:RequiredFieldValidator ID="rfvPassOper" runat="server" ErrorMessage="Ingrese el Password del Usuario Operaciones."
                                                                        ControlToValidate="txtPassOper" Display="Dynamic" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="UpdatePanel1">
                                                                    </asp:RequiredFieldValidator>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                     </div>
                                    <div class="row">
                                        <div class="col-sm-10">
                                            <div class="col-sm-offset-6 col-sm-6">
                                                <button type="button" class="btn btn-keytia-sm" onclick="CreaEsquema();">Crear Cliente</button>
                                            </div>
                                        </div>
                                    </div>
                                    <button type="button" id="btn2" data-toggle="modal" data-target="#exampleModal" class="classButton" onclick="AltaEsquema();"></button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
                        <asp:Panel ID="pnlPopupMensaje" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <asp:Label ID="lblTituloModalMsn" runat="server" Text=""></asp:Label>
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblBodyModalMsn" runat="server" Text=""></asp:Label>                          
                        </div>
                        <div class="modal-footer">                             
                            <asp:Button ID="Button1" runat="server" Text="OK" CssClass="btn btn-keytia-sm" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="lnkBtnMsn" runat="server" Style="display: none"></asp:LinkButton>
            <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
                TargetControlID="lnkBtnMsn" OkControlID="Button1" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
            </asp:ModalPopupExtender>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnYes" />
        </Triggers>
    </asp:UpdatePanel>
    <div class="modal" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true"><i class="fas fa-times"></i></button>
                        <h4 class="modal-title" id="exampleModalLabel">Crear Nuevo Cliente</h4>
                    </div>
                    <div class="panel-body">
                        <div class="form-group">
                            <br />
                            <label for="recipient-name" class="control-label" id="lblMensaje" style="font-size: 15px;"></label>
                        </div>
                    </div>
                    <div class="panel-footer"" id="footer">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-4">
                                    <asp:Button runat="server" ID="btnYes" CssClass="btn btn-keytia-sm" Text="Si" OnClick="btnYes_Click" CausesValidation="false"></asp:Button>
                                    &nbsp;&nbsp;&nbsp;
                                    <button type="button" class="btn btn-keytia-sm" data-dismiss="modal">No</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
