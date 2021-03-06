﻿namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto
    {
        private string _resposta { get; set; }
        public string OrdemId { get; set; }
        public string PerguntaId { get; set; }
        public string Resposta { get { return TransformarSiglaResposta(_resposta); } set { _resposta = value; } }
        private string TransformarSiglaResposta(string sigla)
        {
            return (sigla?.ToUpper()) switch
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
