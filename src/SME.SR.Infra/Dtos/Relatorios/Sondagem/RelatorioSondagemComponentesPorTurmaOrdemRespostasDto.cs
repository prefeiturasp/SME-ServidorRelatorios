namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaOrdemRespostasDto
    {
        private string _resposta { get; set; }
        public int OrdemId { get; set; }
        public int PerguntaId { get; set; }
        public string Resposta { get { return TransformarSiglaResposta(_resposta); } set { _resposta = value; } }
        private string TransformarSiglaResposta(string sigla)
        {
            return (sigla.ToUpper()) switch
            {
                "S" => "Escreve convencionalmente",
                "N" => "Não escreve convencionalmente",
                "A" => "Acertou",
                "E" => "Errou",
                "NR" => "Não resolveu",
                _ => sigla,
            };
        }
    }
}
