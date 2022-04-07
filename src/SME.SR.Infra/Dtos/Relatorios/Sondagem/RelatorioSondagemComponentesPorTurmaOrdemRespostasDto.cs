namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaOrdemRespostasDto
    {
        private string _resposta;

        public int OrdemId { get; set; }
        public int PerguntaId { get; set; }
        public string PerguntaKey { get; set; }
        public int AnoLetivo { get; set; }

        public string Resposta
        {
            get
            {
                if (AnoLetivo < 2022)
                    return TransformarSiglaResposta(_resposta);

                return _resposta;
            }
            set =>  _resposta = value;
        }

        public int OrdenacaoResposta { get; set; }

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
