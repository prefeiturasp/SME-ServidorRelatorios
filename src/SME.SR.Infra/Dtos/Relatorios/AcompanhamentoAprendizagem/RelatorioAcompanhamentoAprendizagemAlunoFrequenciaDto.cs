namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto
    {
        public string Bimestre { get; set; }
        public int Aulas { get; set; }
        public int Ausencias { get; set; }
        public string Frequencia { get; set; }
    }
}