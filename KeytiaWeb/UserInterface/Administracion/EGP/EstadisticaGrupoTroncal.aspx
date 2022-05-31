<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="EstadisticaGrupoTroncal.aspx.cs" Inherits="KeytiaWeb.UserInterface.Administracion.EGP.EstadisticaGrupoTroncal" %>
<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .modalUpload {
            position: fixed;
            z-index: 999;
            height: 100%;
            width: 100%;
            top: 0;
        }

        .centerUpload {
            z-index: 1000;
            margin: 300px auto;
            padding: 10px;
            width: 130px;
            padding-right: 500px;
        }

        .center img {
            height: 120px;
            width: 120px;
        }

        .table1 tr:nth-child(odd) td {
            background-color: #696CAC;
        }

        .table1 tr:nth-child(even) td {
            background-color: #696CAC;
        }
        .cal_Theme1 .ajax__calendar_container   {
        background-color: #696CAC;
        border:solid 1px #696CAC;
        }

        .cal_Theme1 .ajax__calendar_header  {
        background-color: #ffffff;
        margin-bottom: 4px;
        }

        .cal_Theme1 .ajax__calendar_title,
        .cal_Theme1 .ajax__calendar_next,
        .cal_Theme1 .ajax__calendar_prev    {
        color: #004080;
        padding-top: 3px;
        }

        .cal_Theme1 .ajax__calendar_body    {
        background-color: #ffffff;
        border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_dayname {
        text-align:center;
        font-weight:bold;
        margin-bottom: 4px;
        margin-top: 2px;
        color: #004080;
        }

        .cal_Theme1 .ajax__calendar_day {
        color: #004080;
        text-align:center;
        }

        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_day,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_month,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_year,
        .cal_Theme1 .ajax__calendar_active  {
        color: #004080;
        font-weight: bold;
        background-color: #DEF1F4;
        }


 
        .cal_Theme1 .ajax__calendar_today   {
        font-weight:bold;
        }

        .cal_Theme1 .ajax__calendar_other,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_today,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_title {
            color: #bbbbbb;
        }
    </style>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

    <%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js"></script>--%>

    <%--    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/jquery.sumoselect/3.0.2/sumoselect.min.css">

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.sumoselect/3.0.2/jquery.sumoselect.min.js"></script>--%>
    <link href="../../RepPentafon/SumoSelect/sumoselect.min.css" rel="stylesheet" />
    <script src="../../RepPentafon/SumoSelect/jquery.sumoselect.min.js"></script>
    <script src="../../RepPentafon/SumoSelect/ExportExcel.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            <%--$(<%=lstBoxTest.ClientID%>).SumoSelect({ selectAll: true, okCancelInMulti: true });--%>
            $(<%=lstBoxTroncales.ClientID%>).SumoSelect({ selectAll: true });
           $(<%=lstTipo.ClientID%>).SumoSelect({ selectAll: true });
            $(<%=lstCircuitos.ClientID%>).SumoSelect({ selectAll: true });
            $(<%=lstSitio.ClientID%>).SumoSelect({ selectAll: true });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
     <asp:Panel ID="pnlIndicadores" runat="server" CssClass="row">
    </asp:Panel>

    <div class="clearfix"></div>
    <asp:Panel ID="pnlConsumos" runat="server" CssClass="row" Visible="false">
    </asp:Panel>
    <asp:Panel ID="pnlMapaNav" runat="server" CssClass="row">
        <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
            <div class="portlet solid bordered viewDetailPortlet">
                <div class="portlet-title">
                    <%--<div class="caption col-md-10 col-sm-10 col-lg-10 col-xs-10">
                        <button id="btnRegresar" runat="server" onserverclick="btnRegresar_ServerClick" type="button" class="btn btn-default btn-circle btnBackDetail"><i class="far fa-arrow-alt-circle-left"></i></button>
                        <asp:Panel ID="pnlMapaNavegacion" runat="server">
                        </asp:Panel>
                    </div>--%>
                    <div class="actions col-md-2 col-sm-2 col-lg-2 col-xs-2">
                        <p style="text-align: center;">
                            <img src="../../img/svg/Asset 22.svg" alt="">
                            Exportar:&nbsp;<asp:LinkButton ID="btnExportarXLS" runat="server"  OnClick="btnExportarXLS_Click" CssClass="exportExcel"><i class="fas fa-file-excel"></i>&nbsp;Excel</asp:LinkButton>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
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
            <div id="Div8" runat="server">
                <div id="Div9" runat="server" cssclass="row">
                    <div id="Div10" runat="server" cssclass="col-md-12 col-sm-12">
                        <div class="portlet solid bordered">
                            <div class="portlet-title">
                                <%--<div class="caption">
                                    <i class="icon-bar-chart font-dark hide"></i>
                                    <span class="caption-subject titlePortletKeytia" id="Span1" runat="server">Reporte Facturable y No Facturable</span>
                                </div>--%>
                                <div class="actions">
                                    <%--Exportar:&nbsp;<a class="exportExcel" onclick="ExportToExcel();" style="text-decoration: none"><i class="fas fa-file-excel"></i>&nbsp;Excel&nbsp;&nbsp;&nbsp;</a>--%>
                                    <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                                </div>
                            </div>
                            <div class="portlet-body" id="divContenedor">
                                <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <div class="col-sm-12">
                                                                <asp:Panel runat="server" ID="Panel2" CssClass="form-group">
                                                                    <asp:Label runat="server" CssClass="col-sm-1 control-label">Tipo de LLamada:</asp:Label>
                                                                    <div class="col-sm-3">
                                                                        <asp:ListBox runat="server" ID="lstTipo" SelectionMode="Multiple" CssClass="form-control"></asp:ListBox>
                                                                    </div>
                                                                    <asp:Label runat="server" CssClass="col-sm-1 control-label">Circuitos:</asp:Label>
                                                                    <div class="col-sm-3">
                                                                        <asp:ListBox runat="server" ID="lstCircuitos" SelectionMode="Multiple" CssClass="form-control"></asp:ListBox>
                                                                    </div>
                                                                    <asp:Label runat="server" CssClass="col-sm-1 control-label">Troncales:</asp:Label>
                                                                    <div class="col-sm-3">
                                                                        <asp:ListBox runat="server" ID="lstBoxTroncales" SelectionMode="Multiple" CssClass="form-control"></asp:ListBox>
                                                                    </div>
                                                                 
                                                                    
                                                                </asp:Panel>
                                                            </div>
                                                           
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12  text-center">
                                                            <div class="col-sm-12">
                                                                <asp:Panel runat="server" ID="Panel4" CssClass="form-group">
                                                                    <asp:Label runat="server" CssClass="col-sm-1 control-label">Sitio:</asp:Label>
                                                                    <div class="col-sm-3">
                                                                        <asp:ListBox runat="server" ID="lstSitio" SelectionMode="Multiple" CssClass="form-control"></asp:ListBox>
                                                                    </div>
                                                                </asp:Panel>
                                                            </div>
                                                           
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                             <div class="col-sm-12">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                        <asp:Label runat="server" CssClass="col-sm-1 control-label">Fecha Inicio:</asp:Label>
                                                                        <asp:TextBox runat="server" ID="Date1" CssClass="col-sm-4"></asp:TextBox>
                                                                        <asp:CalendarExtender runat="server"
                                                                                    TargetControlID="Date1"
                                                                                    CssClass=" cal_Theme1 text-center"
                                                                                   />
                                                                 

                                                                     <asp:Label runat="server" CssClass="col-sm-1 control-label">Fecha Fin:</asp:Label>
                                                                        <asp:TextBox runat="server" ID="Date2" CssClass="col-sm-4"></asp:TextBox>
                                                                        <asp:CalendarExtender runat="server"
                                                                                    TargetControlID="Date2"
                                                                                    CssClass=" cal_Theme1 text-center"
                                                                                   />
                                                                </asp:Panel>

                                                                 <%--<asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Panel ID="Panel3" runat="server" CssClass="form-group">
                                                                        <div class="col-sm-5 text-center">
                                                                            <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-sm" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                     </asp:Panel>--%>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                        <div class="col-sm-12">
                                                             <div class="col-sm-12">
                                                                <asp:Panel runat="server" CssClass="form-group">
                                                                    <asp:Panel ID="Panel3" runat="server" CssClass="form-group">
                                                                        <div class="col-sm-12 text-center">
                                                                            <asp:Button ID="btnAplicarFecha" runat="server" CssClass="btn btn-keytia-sm" Text="Aceptar" OnClick="btnAplicarFecha_Click" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-12" runat="server" id="dateFinal">
                                                              <div class="row col-sm-12 " id="Div7" runat="server">
                                                                <asp:Panel runat="server" ID="paneles">
                                                                        <asp:Panel ID="pnlRow_0" runat="server" CssClass="row">
                                                                        <asp:Panel ClientIDMode="Static" ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="Panel1" runat="server" CssClass="row">
                                                                        <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12"></asp:Panel>
                                                                    </asp:Panel>
                                                                </asp:Panel>
                                                                
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnAplicarFecha" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        window.onsubmit = function () {
            var updateProgress = $find("<%=UpdateProgress1.ClientID%>");
            window.setTimeout(function () {
                updateProgress.set_visible(true);
            }, 100);
        }
    </script>
</asp:Content>
