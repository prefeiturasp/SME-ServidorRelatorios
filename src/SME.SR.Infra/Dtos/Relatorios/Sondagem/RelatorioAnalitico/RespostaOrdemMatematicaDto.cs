namespace SME.SR.Infra
{
    public class RespostaOrdemMatematicaDto
    {
        public int Ordem { get; set; }
        public string Descricao { get; set; }
        public RespostaMatematicaDto Ideia { get; set; }
        public RespostaMatematicaDto Resultado { get; set; }
    }
}
