<%@ Page Title="" Language="C#" MasterPageFile="~/KeytiaOH.Master" AutoEventWireup="true" CodeBehind="WelcomePage.aspx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.WorkFlow.WelcomePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
        <div ID="pnlMainHolder" runat="server">
        <div ID="pnlRow_0" runat="server" CssClass="row">
            <div ID="Rep0" runat="server" CssClass="col-md-12 col-sm-12">
                <div class="portlet solid bordered">
                    <div class="portlet-title">
                        <div class="caption">
                            <i class="icon-bar-chart font-dark hide"></i>
                            <span class="caption-subject titlePortletKeytia"></span>
                        </div>
                        <div class="actions">
                            <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepDetallCollapse" aria-expanded="true" aria-controls="RepDetallCollapse"></button>
                        </div>
                    </div>
                     <div class="portlet-body">
                       <div class="collapse in form-horizontal" id="RepDetallCollapse" role="form">
                          <div class="row" runat="server" id="row1">
                            <div class="col-sm-12">
                             <div class="form-horizontal">
                                       <asp:Panel ID="rowbienvenida" runat="server" CssClass="form-group">
                                       <div class="col-sm-12">
                                       <div class="jumbotron" style="text-align:center;">
                                           <div class="container">
                                           <h1>¡BIENVENIDO!</h1>
                                           <br />
                                            <h3><label runat="server" id="lblNomEmple"></label></h3>
                                           <br />
                                           <br />
                                           <br />
                                           <br />
                                           <br />
                                           <br />
                                           <br />
                                           <h4>En caso de algún problema favor de reportarlo a atenciontelefoniamovil@afirme.com</h4>
                                          </div>
                                       </div>
                                        </div>
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
</asp:Content>
