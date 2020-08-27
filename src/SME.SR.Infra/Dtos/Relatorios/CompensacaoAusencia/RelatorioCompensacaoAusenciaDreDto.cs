namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaDreDto
    {
        public RelatorioCompensacaoAusenciaDreDto()
        {

        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public RelatorioCompensacaoAusenciaUeDto Ue { get; set; }
    }
}
