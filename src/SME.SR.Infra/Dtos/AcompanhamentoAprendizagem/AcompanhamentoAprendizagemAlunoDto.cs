using SME.SR.Infra.Utilitarios;
using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoDto
    {
        public string PercursoColetivoTurma { get; set; }
        public string Semestre { get; set; }
        public string AlunoCodigo { get; set; }
        public string Observacoes { get; set; }
        public string PercursoIndividual { get; set; }

        public string PercursoTurmaFormatado(int quantidadeImagens)
        {
            if (string.IsNullOrEmpty(PercursoColetivoTurma))
                return string.Empty;
            var registroFormatado = UtilRegex.RemoverTagsHtmlVideo(PercursoColetivoTurma);
            var numeroImagem = 0;

            var imagens = Regex.Matches(PercursoColetivoTurma, "<img.+?>");
            string pattern = @"(https|http):.*(jpg|jpeg|gif|png|bmp)";

            foreach (var imagem in imagens)
            {
                numeroImagem++;
                if (numeroImagem <= quantidadeImagens)
                {
                    string input = imagem.ToString();
                    Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        //PercursoTurmaImagens.Add(new AcompanhamentoAprendizagemPercursoTurmaImagemDto
                        //{
                        //    NomeImagem = $"imagem {numeroImagem}",
                        //    Imagem = match.Value
                        //});
                    }
                    registroFormatado = registroFormatado.Replace(imagem.ToString(), $"|imagem {numeroImagem}|");
                }
                else
                {
                    registroFormatado = registroFormatado.Replace(imagem.ToString(), $"");
                }
            }
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }

        public string PercursoIndividualFormatado()
        {
            if (string.IsNullOrEmpty(PercursoIndividual))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMultiMidia(PercursoIndividual);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }

        public string ObservacoesFormatado()
        {
            if (string.IsNullOrEmpty(Observacoes))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMultiMidia(Observacoes);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
    }
}
