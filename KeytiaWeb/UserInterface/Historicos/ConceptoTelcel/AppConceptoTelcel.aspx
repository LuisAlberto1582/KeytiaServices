<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppConceptoTelcel.aspx.cs" Inherits="KeytiaWeb.UserInterface.AppConceptoTelcel"
    MasterPageFile="~/KeytiaOH.Master" ValidateRequest="true" %>

<%@ Register Assembly="DSOControls2008" Namespace="DSOControls2008" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        var pagePath = window.location.pathname;
        $(function () {

            $("#" + "<%=txtCampoOrigen.ClientID %>").autocomplete(
                             {
                                 source: function (request, response) {

                                     $.ajax({
                                         url: pagePath + "/GetCampoOrigen",
                                         data: "{ 'id': '" + request.term + "'}",
                                         dataType: "json",
                                         type: "POST",
                                         contentType: "application/json; charset=utf-8",
                                         dataFilter: function (data) { return data; },
                                         success: function (data) {

                                             if (data.d.length == 0)
                                                 $("#spCampoOrigen").show();
                                             else
                                                 $("#spCampoOrigen").hide();

                                             response($.map(data.d, function (item) {
                                                 {
                                                     value = item.vchDescripcion + " : " + item.vchCodigo + " : " + item.iCodCatalogo;
                                                     return value;
                                                 }
                                             }))
                                         },
                                         error: function (XMLHttpRequest, callStatus, errorThrown) {
                                             //alert(callStatus);
                                         }
                                     });
                                 },
                                 minLength: 0,
                                 select: function (event, ui) {
                                     var str = ui.item.label.split(":");
                                     $("#" + "<%=txtCampoOrigen.ClientID %>").val(str[0]);
                                     $("#" + "<%=txtCampoOrigenCod.ClientID %>").val(str[1]);
                                     $("#" + "<%=txtCampoOrigenID.ClientID %>").val(str[2]);
                                     $("#" + "<%=btnCampoOrigen.ClientID %>").click();
                                 }
                             });
        });

                         $(function () {
                             $("#" + "<%=txtCampoDestino.ClientID %>").autocomplete(
                              {
                                  source: function (request, response) {

                                      $.ajax({
                                          url: pagePath + "/GetCampoDestino",
                                          data: "{ 'id': '" + request.term + "'}",
                                          dataType: "json",
                                          type: "POST",
                                          contentType: "application/json; charset=utf-8",
                                          dataFilter: function (data) { return data; },
                                          success: function (data) {

                                              if (data.d.length == 0)
                                                  $("#spCampoDestino").show();
                                              else
                                                  $("#spCampoDestino").hide();
                                              response($.map(data.d, function (item) {
                                                  {
                                                      value = item.vchDescripcion + " : " + item.vchCodigo + " : " + item.iCodCatalogo;
                                                      return value;
                                                  }
                                              }))
                                          },
                                          error: function (XMLHttpRequest, callStatus, errorThrown) {
                                              //alert(callStatus);
                                          }
                                      });
                                  },
                                  minLength: 0,
                                  select: function (event, ui) {
                                      var str = ui.item.label.split(":");
                                      $("#" + "<%=txtCampoDestino.ClientID %>").val(str[0]);
                                      $("#" + "<%=txtCampoDestinoCod.ClientID %>").val(str[1]);
                                      $("#" + "<%=txtCampoDestinoID.ClientID %>").val(str[2]);
                                      $("#" + "<%=btnCampoDestino.ClientID %>").click();
                                  }
                              });
                         });

                          function Validate() {
                              var retVal = true;
                              if ($("#spCampoOrigen").is(":visible") ||
                             $("#spCampoDestino").is(":visible"))
                                  retVal = false;
                              if ($("#" + "<%=txtCampoDestino.ClientID %>").val() == "" ||
    $("#" + "<%=txtCampoOrigen.ClientID %>").val() == "")
                                  retVal = false;
                              if ($("#" + "<%=txtCampoDestinoID.ClientID %>").val() == "" ||
                                $("#" + "<%=txtCampoOrigenID.ClientID %>").val() == "")
                                  retVal = false;
                              if (!retVal)
                                  alerta("Datos no válidos para grabar");
                              return retVal;
                          }

                          function alerta(mensaje) {
                              $(document).ready(function () { jAlert(mensaje, "Advertencia"); });
                          }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--Script Manager para AjaxControlToolkit-->
    <asp:ToolkitScriptManager ID="tsmAjaxControls" runat="server" EnableScriptLocalization="true"
        EnableScriptGlobalization="true">
    </asp:ToolkitScriptManager>


    <asp:Panel ID="pnlRow_1" runat="server" CssClass="row">
        <asp:Panel ID="Rep1" runat="server" CssClass="col-md-12 col-sm-12">
            <div class="portlet solid bordered">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-bar-chart font-dark hide"></i>
                        <span class="caption-subject titlePortletKeytia">Conceptos de Telcel</span>
                    </div>
                    <div class="actions">
                        <button class="btn btn-light" type="button" data-toggle="collapse" data-target="#RepConceptosCollapse" aria-expanded="true" aria-controls="RepConceptosCollapse"><i class="far fa-minus-square"></i></button>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="collapse in" id="RepConceptosCollapse">
                        <div class="row form-horizontal divCenter">
                            <div class="col-sm-6">

                                <asp:Panel ID="trCampoOrigen" runat="server" CssClass="form-group">
                                    <asp:Label runat="server" ID="lblCampoOrigen" Text="Campo Origen" CssClass="col-sm-4 control-label"></asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtCampoOrigen" runat="server" class="autosuggest form-control" onfocus="javascript:$(this).autocomplete('search','');" />
                                        <span id="spCampoOrigen" style="display: none; color: Red;">No se encontraron elementos</span>

                                        <div style="display: none">
                                            <asp:Button ID="btnCampoOrigen" runat="server" OnClick="btnCampoOrigen_Click" />
                                            <asp:TextBox ID="txtCampoOrigenID" runat="server" />
                                            <asp:TextBox ID="txtCampoOrigenCod" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="trCampoDestino" runat="server" CssClass="form-group" Visible="false">
                                    <asp:Label runat="server" ID="lblCampoDestino" Text="Campo Destino" CssClass="col-sm-4 control-label"></asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtCampoDestino" runat="server" class="autosuggest form-control" onfocus="javascript:$(this).autocomplete('search','');"></asp:TextBox>
                                        <span id="spCampoDestino" style="display: none; color: Red;">No se encontraron elementos</span>

                                        <div style="display: none">
                                            <asp:Button ID="btnCampoDestino" runat="server" OnClick="btnCampoDestino_Click" />
                                            <asp:TextBox ID="txtCampoDestinoID" runat="server"></asp:TextBox>
                                            <asp:TextBox ID="txtCampoDestinoCod" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="trConceptoFiltro" runat="server" CssClass="form-group" Visible="false">
                                    <asp:Label runat="server" ID="lblConceptoFiltro" Text="Concepto a Filtrar" Enabled="false" CssClass="col-sm-4 control-label"></asp:Label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtConceptoFiltro" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                            </div>
                        </div>
                        <br />
                        <asp:Table ID="trGrabar" runat="server" Width="100%" Visible="false">
                            <asp:TableRow ID="tblBotones" runat="server">
                                <asp:TableCell ID="tblBotonesC1" runat="server" HorizontalAlign="Center">
                                    <asp:Button ID="btnGrabar" Text="Grabar" runat="server" OnClick="btnGrabar_Click" OnClientClick="return Validate();" CssClass="btn btn-keytia-sm" />
                                    &nbsp&nbsp                                
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" CssClass="btn btn-keytia-sm" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </div>
                </div>
                <br />
                <br />
                <asp:Panel ID="pDatosConceptosTelcel" runat="server" CssClass="table-responsive">
                    <asp:UpdatePanel ID="upConceptosTelcel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="grvConceptosTelcel" runat="server" CssClass="table table-bordered tableDashboard" HeaderStyle-CssClass="tableHeaderStyle"
                                DataKeyNames="iCodCatalogo,iCodCatAtribDestino,iCodCatAtribOrigen" AutoGenerateColumns="false" EmptyDataText="No existen conceptos">
                                <Columns>
                                    <asp:BoundField DataField="vchCodAtribOrigen" HeaderText="Clave Campo Origen F1" HtmlEncode="false" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="vchDescAtribOrigen" HeaderText="Campo Origen F1" HtmlEncode="false" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="vchCodAtribDestino" HeaderText="Clave Campo Destino" HtmlEncode="false" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="vchDescAtribDestino" HeaderText="Campo Destino" HtmlEncode="false" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="ConceptoFiltro" HeaderText="Concepto a Filtrar" HtmlEncode="false" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="iCodCatalogo" HtmlEncode="true" Visible="false" />
                                    <asp:BoundField DataField="iCodCatAtribDestino" HtmlEncode="true" Visible="false" />
                                    <asp:BoundField DataField="iCodCatAtribOrigen" HtmlEncode="true" Visible="false" />
                                    <asp:TemplateField HeaderText="Editar">
                                        <ItemTemplate>
                                            <div align="center">
                                                <asp:ImageButton ID="btnEditarRow" ImageUrl="~/images/pencilsmall.png" OnClick="grvConceptosTelcel_EditRow"
                                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Borrar">
                                        <ItemTemplate>
                                            <div align="center">
                                                <asp:ImageButton ID="btnBorrarRow" ImageUrl="~/images/deletesmall.png" OnClick="grvConceptosTelcel_DeleteRow"
                                                    runat="server" RowIndex='<%# Container.DisplayIndex %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <!--Modal PopUp baja-->
                            <asp:Panel ID="pnlBajaConceptoTelcel" runat="server" TabIndex="-1" role="dialog" CssClass="modal-Keytia" Style="display: none;">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <asp:Label ID="lblTituloModalMsn" runat="server" Text="">¿Esta seguro que desea dar de baja este registro?</asp:Label>
                                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" id="btnCerrarMensajes"><i class="fas fa-times"></i></button>
                                        </div>
                                        <div class="modal-body">
                                            <asp:Label ID="lblConceptoTelcel" runat="server" Text="Concepto Telcel"></asp:Label>
                                            <asp:HiddenField ID="hfdiCodCatalogo" runat="server" />
                                        </div>
                                        <div class="modal-footer">
                                            <asp:Button ID="btnEliminarConcepto" runat="server" Text="Eliminar" OnClick="btnEliminarConcepto_PopUp" CssClass="btn btn-keytia-sm" />
                                            <asp:Button ID="btnCancelarPopUp" runat="server" Text="Cancelar" OnClientClick="return Hidepopup()" CssClass="btn btn-keytia-sm" />
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>                    
                            <asp:LinkButton ID="lnkFake" runat="server"></asp:LinkButton>
                            <asp:ModalPopupExtender ID="mpeConceptoTelcel" runat="server" DropShadow="false" PopupControlID="pnlBajaConceptoTelcel"
                                TargetControlID="lnkFake" BackgroundCssClass="modalPopupBackground" CancelControlID="btnCerrarMensajes">
                            </asp:ModalPopupExtender>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="grvConceptosTelcel" />
                            <asp:PostBackTrigger ControlID="btnEliminarConcepto" />
                        </Triggers>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
