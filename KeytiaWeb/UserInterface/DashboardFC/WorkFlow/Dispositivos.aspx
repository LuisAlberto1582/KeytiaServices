<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="Dispositivos.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.Dispositivos" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .formulario {
            background-color: white;
            font-family: 'Poppins', sans-serif;
            padding: 10px;
            margin: 30px 15px;
            font-size: 14px;
        }

        .form-control {
            font-size: 15px;
        }

        .control-label {
            font-size: 14px;
            font-weight: bold;
        }

        .form-title {
            font-size: 14px;
        }

        .title-form, h4, h3 {
            font-weight: bold;
            font-size: 16px;
        }


        .contenedor {
            width: 100%;
            height: 200px;
            overflow: auto;
            border: 1px solid #808080;
            background-color: #E9E9E9;
            border-bottom-left-radius: 6px;
            border-top-left-radius: 6px;
            border-top-right-radius: 6px;
            border-bottom-right-radius: 6px;
            margin-bottom: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <div id="pnlMainHolder" runat="server" style="width: 100%;">
        <div id="pnlRow_0" style="width: 100%;" runat="server" cssclass="row">
            <div id="Rep0" style="width: 100%;" runat="server" cssclass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia" id="labelMensaje" runat="server">Dispositivos</span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"><i class="far fa-minus-square"></i></button>
                        </div>
                    </div>
                    <div class="portlet-body">
                        <div class="collapse in form-horizontal" style="width: 100%;" id="RepDetallCollapse" role="form">
                            <div style="height: 320px; overflow: auto;">
                                <asp:GridView
                                    ID="gridDispositivos"
                                    DataKeyNames="ID,MarcaDispositivoMovilID,MarcaClave,MarcaDesc,ModeloDispositivoMovilID,	ModeloClave,ModeloDesc,TipoRecursoId,TipoRecursoClave,TipoRecursoDesc,IMEI,	SIMCard,LineaID,NumeroTelefonico,dtIniVigencia,dtFinVigencia,dtFecUltAct"
                                    AutoGenerateColumns="false"
                                    CssClass="table text-left"
                                    GridLines="None"
                                    AllowPaging="false"
                                    AllowSorting="true"
                                    runat="server"
                                    OnRowCommand="gridMarcas_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="ID" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="MarcaDispositivoMovilID" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="MarcaClave" HeaderText="Marca" />
                                        <asp:BoundField DataField="MarcaDesc" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="ModeloDispositivoMovilID" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="ModeloClave" HeaderText="Modelo" />
                                        <asp:BoundField DataField="ModeloDesc" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="TipoRecursoId" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="TipoRecursoClave" HeaderText="Recurso" />
                                        <asp:BoundField DataField="TipoRecursoDesc" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="IMEI" HeaderText="IMEI" />
                                        <asp:BoundField DataField="SIMCard" HeaderText="SIM	" />
                                        <asp:BoundField DataField="LineaID" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="NumeroTelefonico" HeaderText="Teléfono" />
                                        <asp:BoundField DataField="dtIniVigencia" HeaderText="Fecha alta" />
                                        <asp:BoundField DataField="dtFinVigencia" HeaderText="" Visible="false" />
                                        <asp:BoundField DataField="dtFecUltAct" HeaderText="" Visible="false" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDetalle" runat="server" Text="Editar" CommandName="Editar">
                                                                <span aria-hidden="true" class="glyphicon glyphicon-pencil"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEliminar" runat="server" Text="Eliminar" CommandName="Eliminar">
                                                                <span aria-hidden="true" class="glyphicon glyphicon-remove"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle BackColor="#BABABA" Font-Bold="True" ForeColor="Black" />
                                </asp:GridView>
                            </div>

                            <br />
                            <br />
                            <div class="form-inline" runat="server" id="rowForm">
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlMarca" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id" OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList Width="200px" runat="server" CssClass="control-form" ID="ddlModelo" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlTipoRecurso" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox Width="130px" runat="server" PlaceHolder="IMEI" MaxLength="15" CssClass="control-form" ID="txtIMEI"></asp:TextBox>
                                <asp:TextBox Width="130px" runat="server" PlaceHolder="SIM" MaxLength="19" CssClass="control-form" ID="txtSIM"></asp:TextBox>
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlNumeroTelefonico" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                                <asp:LinkButton ID="InsertaDispositivo" runat="server" CssClass="btn btn-default form-control" OnClick="InsertaDispositivo_Click"> 
                                    <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <%-- MODAL RM --%>
    <asp:Panel Style="display: none;" runat="server" CssClass="modalPopupEtq formulario" ID="myModalCargar" Width="900px">
        <%--<div class="headerEtq" style="height: 30px; vertical-align: middle; line-height: 30px; font-size: 12px">
            <asp:Label runat="server" ID="lblTituloEditHallazo" Text="Cargar archivos"></asp:Label>
        </div>--%>

        <div class="modal-body  formulario">
            <div class="panel-default">
                <div class="panel-heading">
                    <h3>Dispositivos</h3>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12">
                            <asp:TextBox Visible="false" runat="server" ID="txtIDModal"></asp:TextBox>
                            <asp:TextBox Visible="false" runat="server" ID="txtMarcaDispositivoMovilIDModal"></asp:TextBox>
                            <asp:TextBox Visible="false" runat="server" ID="txtModeloDispositivoMovilIDModal"></asp:TextBox>
                            <asp:TextBox Visible="false" runat="server" ID="txtTipoRecursoIdModal"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblMarcaClaveModal" runat="server" Text="Marca: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlMarcaClaveModal" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id" OnSelectedIndexChanged="ddlMarcaClaveModal_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblModeloClaveModal" runat="server" Text="Modelo: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlModeloClaveModal" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="Label1" runat="server" Text="Tipo Recurso: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlTipoRecursoClaveModal" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblIMEIModal" runat="server" Text="IMEI: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:TextBox runat="server" ID="txtIMEIModal" MaxLength="15"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblSIMCardModal" runat="server" Text="SIMCard: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:TextBox runat="server" ID="txtSIMCardModal" MaxLength="19"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-2">
                                <asp:Label ID="lblLineaIDModal" runat="server" Text="Numero telefónico: " CssClass="control-label"></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:DropDownList Width="130px" runat="server" CssClass="control-form" ID="ddlLineaIDModal" AppendDataBoundItems="true" DataTextField="Clave" DataValueField="Id">
                                    <asp:ListItem Value="0">Selecciona</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>
        </div>


        <div class="footerEtq formulario">
            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" class="btn btn-default" OnClick="btnAceptar_Click" />
            <asp:Button runat="server" ID="btnCerrarCarga" CssClass="btn btn-secondary" Text="Cerrar" />
        </div>
    </asp:Panel>
    <asp:LinkButton runat="server" ID="lnkAgregarNZ" Style="display: none"></asp:LinkButton>
    <asp:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="myModalCargar"
        TargetControlID="lnkAgregarNZ" OkControlID="btnCerrarCarga" BackgroundCssClass="modalPopupBackground">
    </asp:ModalPopupExtender>

    <%-- MODAL RM FIN --%>
</asp:Content>
