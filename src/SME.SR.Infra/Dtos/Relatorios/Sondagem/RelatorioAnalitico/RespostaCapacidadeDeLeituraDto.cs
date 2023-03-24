namespace SME.SR.Infra
{
    public class RespostaCapacidadeDeLeituraDto
    {
        public RespostaCapacidadeDeLeituraDto()
        {
            Localizacao = new ItemRespostaCapacidadeDeLeituraDto();
            Inferencia = new ItemRespostaCapacidadeDeLeituraDto();
            Reflexao = new ItemRespostaCapacidadeDeLeituraDto();
        }

        public ItemRespostaCapacidadeDeLeituraDto Localizacao { get; set; }
        public ItemRespostaCapacidadeDeLeituraDto Inferencia { get; set; }
        public ItemRespostaCapacidadeDeLeituraDto Reflexao { get; set; }
    }
}
