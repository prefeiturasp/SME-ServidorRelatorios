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
            var qtdeLinha = string.IsNullOrEmpty(Titulo) ? 1 : 2;
            var quebra = Resposta.Split("\n");

            foreach(var linha in quebra)
            {
                if (!string.IsNullOrEmpty(linha) && linha.Length > QDADE_CHARS_POR_LINHA)
                    qtdeLinha += (int)Math.Round((double)(linha.Length / QDADE_CHARS_POR_LINHA));
                else
                    qtdeLinha += 1;
            }

            return qtdeLinha;
        }
    }
}
