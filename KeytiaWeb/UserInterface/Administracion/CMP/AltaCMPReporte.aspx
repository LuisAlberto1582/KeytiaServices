<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="AltaCMPReporte.aspx.cs" Inherits="KeytiaWeb.UserInterface.Administracion.CMP.AltaCMPReporte" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <link href="../ETL/CSS/StyleFile.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">


    <%-- <asp:ModalPopupExtender ID="mpeEtqMsn" runat="server" PopupControlID="pnlPopupMensaje"
                    TargetControlID="lnkBtnMsn" OkControlID="btnYes" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
                </asp:ModalPopupExtender>--%>
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="pnlMainHolder">
                <div id="pnlRow_0" runat="server" cssclass="row">
                    <div id="Rep0" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Alta de proceso Reporte Automatizado CMP</span>
                                </div>
                                <div class="actions">
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="panel panel-default">
                                        <div class="panel-body">

                                            <div class="row col-sm-12 text-center " id="dvMensaje" runat="server">
                                                <asp:Label CssClass="label label-danger p" runat="server" ID="lblMsj"></asp:Label>
                                            </div>


                                            <div class="row col-sm-12 " id="Div3" runat="server">

                                                <div class="form-group" runat="server" id="Div6">
                                                    <div class="col-sm-6">
                                                        <asp:Button runat="server" ClientIDMode="Static" Enabled="true" CssClass="btn btn-keytia-sm" ID="btnRegresar" Text="Regresar" OnClick="btnRegresar_Click" />
                                                        <asp:Button runat="server" ClientIDMode="Static" Enabled="true" CssClass="btn btn-keytia-sm" ID="btnAceptar" OnClick="btnAceptar_Click" Text="Enviar Reporte" />

                                                    </div>



                                                </div>



                                            </div>

                                            <div class="row col-sm-5">


                                                <div class="form-group" runat="server" id="Div4">
                                                    <asp:Label runat="server" CssClass="col-sm-8 control-label">Información de Reporte</asp:Label>


                                                </div>
                                                <div class="form-group mb-2" runat="server" id="rowEmpresa">
                                                    <asp:Label runat="server" CssClass="col-sm-4 control-label">Año:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList runat="server" ID="ddlYears"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group mx-sm-3 mb-2" runat="server" id="Div1">
                                                    <asp:Label runat="server" CssClass="col-sm-4 control-label">Mes:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:DropDownList CssClass="ddl" runat="server" ID="ddlMonths"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group mb-2" runat="server" id="Div2">
                                                    <asp:Label runat="server" CssClass="col-sm-4 control-label">Generar Reporte General:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:CheckBox CssClass="chkbox" onchange="txtCorreos()" ClientIDMode="Static" Text="Enviar Reporte General" Checked="true" RepeatDirection="Vertical" RepeatLayout="Flow" runat="server" ID="chkGeneral"></asp:CheckBox>
                                                    </div>
                                                </div>

                                            </div>



                                            <!--Correos-->
                                            <div class="row col-sm-7">


                                                <div class="form-group" runat="server" id="Div5">
                                                    <asp:Label runat="server" CssClass="col-sm-8 control-label">Configuración de Correo</asp:Label>
                                                </div>

                                                <div class="form-group row" runat="server">
                                                    <asp:Label for="txtCC" runat="server" class="col-sm-4 col-form-label">Adjuntar Copia Para Rep. Ind.</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox class="form-control-plaintext col-sm-12" runat="server" ToolTip="Adjuntar copia para los reportes individuales" ID="txtCC"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group row" runat="server">
                                                    <asp:Label for="txtCCO" runat="server" class="col-sm-4 col-form-label">Adjuntar Copia Oculta Para Rep. Ind.:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox class="form-control-plaintext col-sm-12" runat="server" ToolTip="Adjuntar Copia Oculta para los reportes individuales" ID="txtCCO"></asp:TextBox>
                                                    </div>
                                                </div>



                                                <div class="form-group row" runat="server" id="Ccgeneraldiv">
                                                    <asp:Label  for="txtCCGeneral" runat="server" class="col-sm-4 col-form-label">Correo(s) Destinatario(s) Para Reporte General:</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox  ClientIDMode="Static" class="form-control-plaintext col-sm-12" runat="server" ToolTip="Ingrese el o los correos destinatarios para el reporte general (separados por coma (,))" ID="txtCCGeneral"></asp:TextBox>
                                                    </div>
                                                </div>


                                                <div class="form-group row" runat="server" id="Ccogeneraldiv">
                                                    <asp:Label for="txtCCOGeneral" runat="server" class="col-sm-4 col-form-label"> Copia Oculta Para Reporte General</asp:Label>
                                                    <div class="col-sm-8">
                                                        <asp:TextBox ClientIDMode="Static" class="form-control-plaintext col-sm-12  " runat="server" ToolTip="Ingrese el o los correos para la copia oculta del reporte general (separados por coma (,))" ID="txtCCOGeneral"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>

                                            <!--Buscador-->
                                            <div class="row col-sm-12 text-center" style="margin-top: 10px; margin-bottom: 20px;">
                                                <div class="col-sm-12">

                                                    <asp:Label runat="server" ID="lblBuscar" CssClass="col-sm-2 control-label">Buscar CenCos: </asp:Label>
                                                    <div class="col-offset-2 col-sm-10">
                                                        <asp:TextBox runat="server" ClientIDMode="Static" onkeyup="functionBuscar()" placeholder="Busque un centro de costos" ID="txtBuscar" class=" form-control"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <!--CheckBox y Tabla-->
                                            <div class="row col-sm-12">
                                                <div class="col-sm-8">
                                                    <asp:Label runat="server" ID="Label1" CssClass="col-sm-7 text-center ">Listado de Centros de Costos </asp:Label>
                                                    <asp:CheckBox ID="chkPastConf" runat="server" CssClass="col-sm-5 chkbox" OnCheckedChanged="chkPastConf_CheckedChanged" Checked="false" AutoPostBack="true" Text="Cargar Configuración Anterior" />
                                                    <asp:CheckBoxList ClientIDMode="Static" AppendDataBoundItems="true" CssClass="chkbox col-sm-12"
                                                        RepeatColumns="2"
                                                        RepeatDirection="Horizontal"
                                                        RepeatLayout="Table" TextAlign="Right" ID="cblElementos" runat="server">
                                                    </asp:CheckBoxList>
                                                </div>
                                                <div class="col-sm-4">
                                                    <asp:Label runat="server" ID="Label2" CssClass="col-sm-12 ">CenCos Seleccionados </asp:Label>
                                                    <table id="tblSeleccionados" class="table-responsive table table-striped col-sm-12">
                                                        <tbody>
                                                        </tbody>
                                                    </table>

                                                </div>

                                            </div>

                                        </div>
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
                                <asp:Button ID="btnYes" runat="server" Text="OK" CssClass="btn btn-keytia-sm" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnAceptar" />
            <asp:PostBackTrigger ControlID="btnRegresar" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        $(document).ready(function () {
            document.getElementById("<%=chkPastConf.ClientID%>").checked = false;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);
            // Place here the first init of the autocomplete
            InitAutoCompl();
        });




        function functionBuscar() {
            var input, filter, ul, li, a, i, txtValue;
            input = document.getElementById('txtBuscar');
            filter = input.value.toUpperCase();
            ul = document.getElementsByTagName('Table');
            li = ul[0].getElementsByTagName('td');

            // Loop through all list items, and hide those who don't match the search query
            for (i = 0; i < li.length; i++) {
                a = li[i].getElementsByTagName("label")[0];
                txtValue = a.textContent || a.innerText;
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    li[i].style.display = "";
                } else {
                    li[i].style.display = "none";
                }
            }
        };
        function txtCorreos() {
            var chkbox = document.getElementById('chkGeneral');
            if (chkbox.checked) {
                $('#txtCCGeneral').parent().parent().fadeIn();
                $('#txtCCOGeneral').parent().parent().fadeIn();
            }
            else {
                $('#txtCCGeneral').parent().parent().hide();
                $('#txtCCOGeneral').parent().parent().hide();
            }
        }
        function PopulateTable() {
            var input = document.getElementById('cblElementos').getElementsByTagName("input");
            var labels = document.getElementById('cblElementos').getElementsByTagName("label");
            $("#tblSeleccionados > tbody").empty();
            var selected = 0;
            for (var i = 0; i < input.length; i++) {
                if (input[i].type == "checkbox") {
                    if (input[i].checked == true) {
                        selected++;
                        //alert(input[i].value);//Get all the checked checkboxes
                        $('#tblSeleccionados > tbody:first-child').append('<tr><td><label class="">' + labels[i].innerHTML + '</label></td></tr>');

                    }
                }
            }
            txtCorreos();
        }

        function InitializeRequest(sender, args) {
        }

        function EndRequest(sender, args) {
            // after update occur on UpdatePanel re-init the Autocomplete
            InitAutoCompl();
        }

        function InitAutoCompl() {
            
            document.getElementById("<%=cblElementos.ClientID%>").onchange = function () {
                var input = document.getElementById('cblElementos').getElementsByTagName("input");
                var labels = document.getElementById('cblElementos').getElementsByTagName("label");
                $("#tblSeleccionados > tbody").empty();
                var selected = 0;
                for (var i = 0; i < input.length; i++) {
                    if (input[i].type == "checkbox") {
                        if (input[i].checked == true) {
                            selected++;
                            //alert(input[i].value);//Get all the checked checkboxes
                            $('#tblSeleccionados > tbody:first-child').prepend('<tr><td><label class="">' + labels[i].innerHTML + '</label></td></tr>');

                        }
                    }
                };
            }
            function functionBuscar() {
                var input, filter, ul, li, a, i, txtValue;
                input = document.getElementById('txtBuscar');
                filter = input.value.toUpperCase();
                ul = document.getElementsByTagName('Table');
                li = ul[0].getElementsByTagName('td');

                // Loop through all list items, and hide those who don't match the search query
                for (i = 0; i < li.length; i++) {
                    a = li[i].getElementsByTagName("label")[0];
                    txtValue = a.textContent || a.innerText;
                    if (txtValue.toUpperCase().indexOf(filter) > -1) {
                        li[i].style.display = "";
                    } else {
                        li[i].style.display = "none";
                    }
                }
            };
            function txtCorreos() {
                var chkbox = document.getElementById('chkGeneral');
                if (chkbox.checked) {
                    $('#txtCCGeneral').parent().parent().fadeIn();
                    $('#txtCCOGeneral').parent().parent().fadeIn();
                }
                else {
                    $('#txtCCGeneral').parent().parent().hide();
                    $('#txtCCOGeneral').parent().parent().hide();
                }
            }
            function PopulateTable() {
                var input = document.getElementById('cblElementos').getElementsByTagName("input");
                var labels = document.getElementById('cblElementos').getElementsByTagName("label");
                $("#tblSeleccionados > tbody").empty();
                var selected = 0;
                for (var i = 0; i < input.length; i++) {
                    if (input[i].type == "checkbox") {
                        if (input[i].checked == true) {
                            selected++;
                            //alert(input[i].value);//Get all the checked checkboxes
                            $('#tblSeleccionados > tbody:last-child').prepend('<tr><td><label class="">' + labels[i].innerHTML + '</label></td></tr>');

                        }
                    }
                }
                txtCorreos();
            }


        };

    </script>
    <%--  <script type="text/javascript">
         var prm = Sys.WebForms.PageRequestManager.getInstance();
         prm.add_endRequest(function () {
             document.getElementById("<%=cblElementos.ClientID%>").onchange = function () {
                 var input = document.getElementById('cblElementos').getElementsByTagName("input");
                 var labels = document.getElementById('cblElementos').getElementsByTagName("label");
                 $("#tblSeleccionados > tbody").empty();
                 var selected = 0;
                 for (var i = 0; i < input.length; i++) {
                     if (input[i].type == "checkbox") {
                         if (input[i].checked == true) {
                             selected++;
                             //alert(input[i].value);//Get all the checked checkboxes
                             $('#tblSeleccionados > tbody:last-child').append('<tr><td><label class="">' + labels[i].innerHTML + '</label></td></tr>');

                         }
                     }
                 }
             };

         });
         prm.add_endRequest(txtCorreos());
         function functionBuscar() {
             var input, filter, ul, li, a, i, txtValue;
             input = document.getElementById('txtBuscar');
             filter = input.value.toUpperCase();
             ul = document.getElementsByTagName('Table');
             li = ul[0].getElementsByTagName('td');

             // Loop through all list items, and hide those who don't match the search query
             for (i = 0; i < li.length; i++) {
                 a = li[i].getElementsByTagName("label")[0];
                 txtValue = a.textContent || a.innerText;
                 if (txtValue.toUpperCase().indexOf(filter) > -1) {
                     li[i].style.display = "";
                 } else {
                     li[i].style.display = "none";
                 }
             }
         };
         function txtCorreos() {
             var chkbox = document.getElementById('chkGeneral');
             if (chkbox.checked) {
                 $('#txtCCGeneral').parent().parent().fadeIn();
                 $('#txtCCOGeneral').parent().parent().fadeIn();
             }
             else {
                 $('#txtCCGeneral').parent().parent().hide();
                 $('#txtCCOGeneral').parent().parent().hide();
             }
         }
         function PopulateTable() {
             var input = document.getElementById('cblElementos').getElementsByTagName("input");
             var labels = document.getElementById('cblElementos').getElementsByTagName("label");
             $("#tblSeleccionados > tbody").empty();
             var selected = 0;
             for (var i = 0; i < input.length; i++) {
                 if (input[i].type == "checkbox") {
                     if (input[i].checked == true) {
                         selected++;
                         //alert(input[i].value);//Get all the checked checkboxes
                         $('#tblSeleccionados > tbody:last-child').append('<tr><td><label class="">' + labels[i].innerHTML + '</label></td></tr>');

                     }
                 }
             }
             txtCorreos();
         }
         document.getElementById("<%=cblElementos.ClientID%>").onchange = function () {
             var input = document.getElementById('cblElementos').getElementsByTagName("input");
             var labels = document.getElementById('cblElementos').getElementsByTagName("label");
             $("#tblSeleccionados > tbody").empty();      
             for (var i = 0; i < input.length; i++) {
                 if (input[i].type == "checkbox") {
                     if (input[i].checked == true) {
                         $('#tblSeleccionados > tbody:last-child').append('<tr><td><label class="">' + labels[i].innerHTML + '</label></td></tr>');

                     }
                 }
             }
         };
    </script>--%>
</asp:Content>
