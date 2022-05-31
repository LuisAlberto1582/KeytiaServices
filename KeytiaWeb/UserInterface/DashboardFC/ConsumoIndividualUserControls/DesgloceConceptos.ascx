<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DesgloceConceptos.ascx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividualUserControls.DesgloceConceptos" %>
 <div class="modal fade" id="modaldesgloce"
         role="dialog" aria-labelledby="myModalLabel"
         aria-hidden="true">
  
        <div class="modal-dialog" style="width:90%; margin-top:0 !important;">
            <div class="modal-content">
                <div class="m-header">
                    <button class="close" data-dismiss="modal">
                        ×
                    </button>
                     
                    <h2 class="myModalLabel"> Desgloce de Conceptos</h2>
                   <button runat="server" ID="btnDesgloce" value="Title">Excel</button>
                </div>
                <div id="contenido" style="max-height:500px; overflow-y:scroll; margin:5px 0;">

                </div>
            </div>
        </div>
    </div>