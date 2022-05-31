<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BajaCargas.aspx.cs" Inherits="KeytiaWeb.UserInterface.BajaCargas.BajaCargas"
    MasterPageFile="~/KeytiaOH.Master" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>

    <div>
        <asp:Label ID="lblInicio" runat="server" CssClass="page-title-keytia">Baja Cargas</asp:Label>
    </div>

    <asp:TabContainer ID="TabContainerPrincipal" runat="server" CssClass="MyTabStyle">
        <asp:TabPanel ID="TabPanelbtnBajaCargaUnica" runat="server" HeaderText="Baja carga unica">
            <ContentTemplate>
                <div class="form-horizontal" role="form">
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div class="form-group">
                                <asp:Label Text="Clave :" ID="lblClaveCU" runat="server" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtClaveCU" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label Text="Descripcion Sitio :" ID="lblDescSitioBajaCU" runat="server" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox ID="txtDescSitioBajaCU" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnAceptarBajaCU" Text="Aceptar" CssClass="btn btn-keytia-md" runat="server" OnClick="btnAceptarBajaCU_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanelbtnBajaMCarga" runat="server" HeaderText="Baja cargas multiples">
            <ContentTemplate>
                <div class="form-horizontal" role="form">
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div class="form-group">
                                <asp:Label Text="Descripcion Sitio: " ID="lblDescSitioBajaMC" runat="server" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtDescSitioBajaMC" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-sm-4">
                                    <asp:Button Text="Obtener Cargas" ID="btnObtenerCargas" CssClass="btn btn-keytia-md" runat="server" OnClick="btnObtenerCargas_Click" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="col-sm-1 control-label">Cargas:</asp:Label>
                                <div class="col-sm-10 scrollbar scrollbar-warning thin" style="height: 250px">
                                    <asp:CheckBoxList ID="chBxListCargasPorSitio" runat="server" CssClass="form-control">
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnAceptarBajaMC" Text="Aceptar" CssClass="btn btn-keytia-md" runat="server" OnClick="btnAceptarBajaMC_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanelbtnBajaPorFechas" runat="server" HeaderText="Baja cargas rango fechas">
            <ContentTemplate>
                <div class="form-horizontal" role="form">
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div class="form-group">
                                <asp:Label Text="Descripcion Sitio:" ID="lblDescSitioBajaCPorFechas" runat="server" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtDescSitioBajaCPorFechas" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label Text="Fecha Inicio:" ID="lblFechaInicioBajaCPorFechas" runat="server" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtFechaInicioBajaCPorFechas" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label Text="Fecha FIn: " ID="lblFechaFinBajaCPorFechas" runat="server" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtFechaFinBajaCPorFechas" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnAceptarBajaCPorFechas" runat="server" CssClass="btn btn-keytia-md" Text="Aceptar" OnClick="btnAceptarBajaCPorFechas_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
    </asp:TabContainer>
</asp:Content>
