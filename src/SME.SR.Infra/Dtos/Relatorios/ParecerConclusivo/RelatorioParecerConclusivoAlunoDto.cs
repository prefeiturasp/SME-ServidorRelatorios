namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoAlunoDto
    {
        public RelatorioParecerConclusivoAlunoDto()
        {

        }
        public string TurmaNome { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNumeroChamada { get; set; }
        public string AlunoNomeCompleto { get; set; }
        public string ParecerConclusivoDescricao { get; set; }
    }
}