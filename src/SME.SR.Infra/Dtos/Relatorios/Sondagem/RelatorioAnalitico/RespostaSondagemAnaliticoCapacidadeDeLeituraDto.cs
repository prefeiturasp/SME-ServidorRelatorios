namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoCapacidadeDeLeituraDto : RelatorioSondagemAnaliticoDto
    {
        public RespostaCapacidadeDeLeituraDto OrdemDoNarrar { get; set; }
        public RespostaCapacidadeDeLeituraDto OrdemDoRelatar { get; set; }
        public RespostaCapacidadeDeLeituraDto OrdemDoArgumentar { get; set; }
    }
}
