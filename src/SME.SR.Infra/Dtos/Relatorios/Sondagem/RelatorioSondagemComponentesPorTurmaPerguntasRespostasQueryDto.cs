namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPerguntasRespostasQueryDto
    {
        public string AlunoEolCode { get; set; }
        public string NomeAluno { get; set; }
        public int AnoLetivo { get; set; }
        public int AnoTurma { get; set; }
        public int PerguntaId { get; set; }
        public string Pergunta { get; set; }
        public string Resposta { get; set; }
        public int OrdenacaoResposta { get; set; }
    }
}
