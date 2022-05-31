<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetalleLlamadasFija.ascx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividualUserControls.DetalleLlamadasFija" %>
<!-- Modal content -->

 



 <div class="modal fade" id="modalfija"
         role="dialog" aria-labelledby="myModalLabel"
         aria-hidden="true">
  
        <div class="modal-dialog" style="width:90%; margin-top:0 !important;">
            <div class="modal-content">
                <div class="m-header">
                    <button class="close" data-dismiss="modal">
                        ×
                    </button>
                    <h2 class="myModalLabel"> Detalle de Llamadas Telefonía Fija </h2>
                </div>
                 <table class="display" id="tablefija">
                <thead>
            <tr>
                <th>Centro de Costos</th>
                <th>Colaborador</th>
                <th>Nomina</th>
                <th>Extension</th>
                <th>Código De Autorización</th>
                <th>Fecha</th>
                <th>Numero Marcado</th>
                <th>Hora</th>
                <th>Fecha Fin</th>
                <th >Hora Fin</th>
                <th>Cantidad Minutos</th>
                <th>Cantidad Llamadas</th>
                <th>Total</th>
                <th>Sitio</th>
                <th>Carrier</th>
                <th>Tipo de Llamada</th>
                <th>Tipo Destino</th>
            </tr>
        </thead>
                <tbody>

                </tbody>
         <%-- <tfoot style="background-color: #191970 !important" >
                    <tr >

                        <td colspan="12" style="background-color: #191970 !important;color: white !important;" >Total</td>
                        <td colspan="5" style="background-color: #191970 !important;color: white !important;"> <asp:label runat="server" id="labelfija" >1000</asp:label></td>

                    </tr>
                </tfoot>--%>
            </table>
            </div>
        </div>
    </div>