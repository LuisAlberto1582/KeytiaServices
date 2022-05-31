namespace KeytiaWeb.UserInterface
{
    public class CnfgReportesEspecialesEdit : HistoricEdit
    {
        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("Asunto");
            }
        }
    }
}