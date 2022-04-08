namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPerguntasProficienciaQueryDto : RelatorioSondagemComponentesPorTurmaPerguntasQueryDto
    {
        public string SubPerguntaId { get; set; }
        public string SubPergunta { get; set; }
    }
}
