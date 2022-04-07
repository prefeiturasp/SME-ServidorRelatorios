namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPerguntasRespostasProficienciaQueryDto : RelatorioSondagemComponentesPorTurmaPerguntasRespostasQueryDto
    {
        public string SubPerguntaId { get; set; }
        public string SubPergunta { get; set; }
    }
}
