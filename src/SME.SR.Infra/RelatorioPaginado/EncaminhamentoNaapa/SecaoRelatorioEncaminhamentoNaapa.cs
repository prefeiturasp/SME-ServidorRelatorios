using System;

namespace SME.SR.Infra
{
    public abstract class SecaoRelatorioEncaminhamentoNaapa
    {
        public abstract int ObterLinhasDeQuebra();

        protected string ObterValor(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto secaoQuestao, string componente)
        {
            var questao = secaoQuestao.Questoes.Find(q => q.NomeComponente == componente);

            return ObterValorFormatado(questao);
        }

        protected string ObterValorFormatado(QuestaoEncaminhamentoNAAPADetalhadoDto questao)
        {
            if (questao != null)
            {
                var resposta = ObterValorPorTipo(questao);

                if (!string.IsNullOrEmpty(resposta))
                    return $"{questao.Questao}: {ObterValorPorTipo(questao)}";
            }
                
            return string.Empty;
        }

        private string ObterValorPorTipo(QuestaoEncaminhamentoNAAPADetalhadoDto questao)
        {
            if (questao.TipoQuestao == TipoQuestao.Data && !string.IsNullOrEmpty(questao.Resposta))
            {
                DateTime dataQuestao;

                if (DateTime.TryParse(questao.Resposta, out dataQuestao))
                {
                    return dataQuestao.ToString("dd/MM/yyyy");
                }
            }
                
            return questao.Resposta;
        }
    }
}
