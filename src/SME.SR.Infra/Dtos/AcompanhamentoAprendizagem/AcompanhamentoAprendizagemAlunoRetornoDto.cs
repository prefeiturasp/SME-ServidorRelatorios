using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoRetornoDto
    {
        public AcompanhamentoAprendizagemAlunoRetornoDto()
        {
            Fotos = new List<AcompanhamentoAprendizagemAlunoFotoDto>();
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
        public List<AcompanhamentoAprendizagemAlunoFotoDto> Fotos { get; set; }

        public void Add(AcompanhamentoAprendizagemAlunoFotoDto acompanhamentoAprendizagemAlunoFotoDto)
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

        public string PercusoTurmaFormatado()
        {
            if (string.IsNullOrEmpty(ApanhadoGeral))
                return string.Empty;
            
            var i = 1;

            var imagens = Regex.Matches(ApanhadoGeral, "<img.+?>");

            foreach (var imagem in imagens)
            {
                var textoSemImagem = ApanhadoGeral.Replace(imagem.ToString(), $"#{i++}");
                ApanhadoGeral = textoSemImagem;
            }

            i = 1;
            var registro = UtilRegex.RemoverTagsHtml(ApanhadoGeral);            

            foreach (var imagem in imagens)
            {
                var textoSemTagsComImagens = registro.Replace($"#{i++}", imagem.ToString());
                registro = textoSemTagsComImagens;
            }            
             return registro;
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
