using System;

namespace SME.SR.Infra
{
    public class SecaoRespostaTextoEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        private const int QDADE_CHARS_POR_LINHA = 100;

        public SecaoRespostaTextoEncaminhamentoNaapa(QuestaoEncaminhamentoNAAPADetalhadoDto questao, bool semTitulo = false)
        {
            Titulo = semTitulo ? string.Empty : questao.Questao.ToUpper();
            Resposta = questao.Resposta;
        }

        public string Titulo { get; set; }
        public string Resposta {  get; set; }

        public override int ObterLinhasDeQuebra()
        {
            var linha = string.IsNullOrEmpty(Titulo) ? 1 : 2;

            if (!string.IsNullOrEmpty(Resposta) && Resposta.Length > QDADE_CHARS_POR_LINHA)
                return 1 + (int)Math.Round((double)(Resposta.Length / QDADE_CHARS_POR_LINHA));

            return linha;
        }
    }
}
