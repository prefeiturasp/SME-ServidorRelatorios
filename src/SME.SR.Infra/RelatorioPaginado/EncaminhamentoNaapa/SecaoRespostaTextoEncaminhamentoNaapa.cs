using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class SecaoRespostaTextoEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        private const int QDADE_CHARS_POR_LINHA = 110;
        private const int QTDE_TOTAL_CHARS_POR_PAG = 6820;
        private const int QTDE_TOTAL_LINHAS = 62;

        public SecaoRespostaTextoEncaminhamentoNaapa(QuestaoEncaminhamentoNAAPADetalhadoDto questao, bool semTitulo = false)
        {
            Titulo = semTitulo ? string.Empty : questao.Questao.ToUpper();
            Resposta = RemoveHTMLTags(questao.Resposta);
        }

        public SecaoRespostaTextoEncaminhamentoNaapa(string titulo)
        {
            Titulo = titulo;
        }

        public string Titulo { get; set; }
        public string Resposta { get; set; }

        public override int ObterLinhasDeQuebra()
        {
            var qtdeLinha = string.IsNullOrEmpty(Titulo) ? 0 : 1;
            var quebra = Resposta.Split("\n");

            foreach (var linha in quebra)
            {
                if (!string.IsNullOrEmpty(linha) && linha.Length > QDADE_CHARS_POR_LINHA)
                    qtdeLinha += (int)Math.Round((double)(linha.Length / QDADE_CHARS_POR_LINHA));
                else
                    qtdeLinha += 1;
            }

            return qtdeLinha;
        }

        public List<SecaoRespostaTextoEncaminhamentoNaapa> ObterSecaoTextoPaginada(int totalLinhas, int totalLinhaAtual)
        {
            var paginas = new List<SecaoRespostaTextoEncaminhamentoNaapa>();
            totalLinhas -= 1;
            var totalLinhasRestante = totalLinhas - totalLinhaAtual - 1;

            if (Resposta.Length > QTDE_TOTAL_CHARS_POR_PAG)
            {
                var totalPagina = ObterLinhasDeQuebra() / QTDE_TOTAL_LINHAS;
                int finalCaracteres = totalLinhasRestante > 3 ? (totalLinhasRestante  * QDADE_CHARS_POR_LINHA) : QTDE_TOTAL_CHARS_POR_PAG;
                var inicioCaracteres = 0;

                for (int pagina = 1; pagina <= totalPagina; pagina++)
                {
                    var paginaAtual = new SecaoRespostaTextoEncaminhamentoNaapa(this.Titulo);
                    var texto = ObterTexto(Resposta, inicioCaracteres, finalCaracteres);
                    if (string.IsNullOrEmpty(texto))
                        break;
                    var indiceUltimoEspaco = texto.LastIndexOf(" ");

                    paginaAtual.Resposta = indiceUltimoEspaco > -1 ? texto.Substring(0, indiceUltimoEspaco) : texto;
                    inicioCaracteres = indiceUltimoEspaco > -1 ? inicioCaracteres + indiceUltimoEspaco + 1 : inicioCaracteres + paginaAtual.Resposta.Length;
                    finalCaracteres = QTDE_TOTAL_CHARS_POR_PAG;
                    paginas.Add(paginaAtual);
                }
            }
            else
                paginas.Add(this);

            return paginas;
        }

        public static string RemoveHTMLTags(string texto)
        {
            return Regex.Replace(texto, "<.*?>", string.Empty);
        }

        private string ObterTexto(string texto, int inicioCaracteres, int finalCaracteres)
        {
            if (texto.Length < inicioCaracteres)
                return string.Empty;

            return texto.Substring(inicioCaracteres, finalCaracteres);
        }
    }
}
