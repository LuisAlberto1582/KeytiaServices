<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsumoDeDatos.ascx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividualUserControls.ConsumoDeDatos" %>

 <div class="modal fade" id="modaldatos"
         role="dialog" aria-labelledby="myModalLabel"
         aria-hidden="true">
  
        <div class="modal-dialog" style="width:90%; margin-top:0 !important;">
            <div class="modal-content">
                <div class="m-header">
                    <button class="close" data-dismiss="modal">
                        ×
                    </button>
                    <h2 class="myModalLabel"> Consumo De Datos </h2>
                </div>
                <table class="display" id="tabledatos">
                <thead>
            <tr>
                <th>Fecha</th>
                <th>Hora</th>
                <th>Tipo</th>
                <th>Tipo Consumo</th>
                <th>Localildad Destino</th>
                <th>Servicio</th>
                <th>Internet(MB)</th>
                <th>Internet($)</th>
            </tr>
        </thead>
            </table>
            </div>
        </div>
    </div>