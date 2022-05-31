<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetalleLlamadasMoviles.ascx.cs" Inherits="KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividualUserControls.WebUserControl1" %>




<div class="modal fade" id="modalmovil"
    role="dialog" aria-labelledby="myModalLabel"
    aria-hidden="true">

    <div class="modal-dialog" style="width: 90%; margin-top: 0 !important;">
        <div class="modal-content">
            <div class="m-header">
                <button class="close" data-dismiss="modal">
                    ×
                </button>
                <h2 class="myModalLabel">Detalle de Llamadas Tel. Móvil</h2>
            </div>
            <table class="display" id="tablemoviles">
                <thead>
                    <tr>
                        <th>Fecha</th>
                        <th>Duración</th>
                        <th>Número Marcado</th>
                        <th>Lugar Llamado</th>
                        <th>Tipo Llamada</th>
                        <th>Importe</th>
                    </tr>
                </thead>
<%--                <tfoot style="background-color: #191970 !important" >
                    <tr >
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td style="background-color: #191970 !important;color: white !important;" >Total</td>
                        <td style="background-color: #191970 !important;color: white !important;"> <span id="totalmovil" ></span></td>
                    </tr>
                </tfoot>--%>
            </table>
        </div>
    </div>
</div>


