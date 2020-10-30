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
            return (sigla?.ToUpper()) switch
            {
                "S" => "Escreve de forma convencional",
                "N" => "Não escreve de forma convencional",
                "A" => "Acertou",
                "E" => "Errou",
                "NR" => "Não resolveu",
                _ => sigla,
            };
        }
    }
}
