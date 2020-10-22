using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaRespostaDto
    {
        private string _resposta { get; set; }
        public int PerguntaId { get; set; }
        public string Resposta { get { return TransformarSiglaResposta(_resposta); } set { _resposta = value; } }
        private string TransformarSiglaResposta(string sigla)
        {
            if (sigla == null) return String.Empty;

            return (sigla.ToUpper()) switch
            {
                "PS" => "Pré-silábico",
                "SSV" => "Silábico sem valor sonoro",
                "SCV" => "Silábico com valor sonoro",
                "SA" => "Sílábico alfabético",
                "A" => "Alfabético",
                _ => sigla,
            };
        }
    }
}
