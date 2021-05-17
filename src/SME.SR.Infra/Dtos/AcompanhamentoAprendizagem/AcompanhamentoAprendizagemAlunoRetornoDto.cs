using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoRetornoDto
    {
        public AcompanhamentoAprendizagemAlunoRetornoDto()
        {
            Fotos = new List<ArquivoDto>();
            PercursoTurmaImagens = new List<AcompanhamentoAprendizagemPercursoTurmaImagemDto>();
        }

        public long Id { get; set; }
        public string AlunoCodigo { get; set; }
        public string ApanhadoGeral { get; set; }
        public string Observacoes { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string DreNome { get; set; }
        public string DreAbreviacao { get; set; }
        public string TurmaNome { get; set; }
        public int Semestre { get; set; }
        public List<ArquivoDto> Fotos { get; set; }
        public List<AcompanhamentoAprendizagemPercursoTurmaImagemDto> PercursoTurmaImagens { get; set; }

        public void Add(ArquivoDto acompanhamentoAprendizagemAlunoFotoDto)
        {
            if (!Fotos.Any(a => a.Codigo == acompanhamentoAprendizagemAlunoFotoDto.Codigo))
                Fotos.Add(acompanhamentoAprendizagemAlunoFotoDto);

        }

        public string ObservacoesFormatado()
        {
            if (string.IsNullOrEmpty(Observacoes))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMidia(Observacoes);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }

        public string PercursoTurmaFormatado()
        {
            if (string.IsNullOrEmpty(ApanhadoGeral))
                return string.Empty;
            
            var i = 1;
            var j = 1;

            var imagens = Regex.Matches(ApanhadoGeral, "<img.+?>");

            foreach (var imagem in imagens)
            {
                var numeroImagem = i++;
                var textoSemImagem = ApanhadoGeral.Replace(imagem.ToString(), $" imagem {numeroImagem} ");                

                string pattern = @"(https|http):.*(jpg|jpeg|gif|png|bmp)";
                string input = imagem.ToString();
                Match m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);                

                if (m.Success)
                {

                    PercursoTurmaImagens.Add(new AcompanhamentoAprendizagemPercursoTurmaImagemDto
                    {
                        NomeImagem = $"imagem {numeroImagem}",
                        Imagem = m.Value
                    });
                }                
                ApanhadoGeral = textoSemImagem;
            }

            var registroFormatado = UtilRegex.RemoverTagsHtmlMidia(ApanhadoGeral);
            var registrosemTag = UtilRegex.RemoverTagsHtml(registroFormatado);

            foreach(var img in imagens)
            {
                var numeroImagem = j++;
                var textoSemImagem = registrosemTag.Replace($"imagem {numeroImagem}", $"<b>imagem {numeroImagem}</b>");

                registrosemTag = textoSemImagem;
            }
            return registrosemTag;           
        }

        public string SemestreFormatado()
        {
            if (Semestre == 0)
                return string.Empty;

            return $"{Semestre}º SEMESTRE {DateTime.Now.Year}";
        }

        public string UeNomeFormatado()
        {
            if (UeNome == null)
                return string.Empty;

            return $"{TipoEscola.GetAttribute<DisplayAttribute>().ShortName} {UeNome}";
        }       
    }
}
