namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoCapacidadeDeLeituraDto : RelatorioSondagemAnaliticoDto
    {
        public RespostaSondagemAnaliticoCapacidadeDeLeituraDto()
        {
            OrdemDoNarrar = new RespostaCapacidadeDeLeituraDto();
            OrdemDoRelatar = new RespostaCapacidadeDeLeituraDto();
            OrdemDoArgumentar = new RespostaCapacidadeDeLeituraDto();
        }
        public RespostaCapacidadeDeLeituraDto OrdemDoNarrar { get; set; }
        public RespostaCapacidadeDeLeituraDto OrdemDoRelatar { get; set; }
        public RespostaCapacidadeDeLeituraDto OrdemDoArgumentar { get; set; }
    }
}
