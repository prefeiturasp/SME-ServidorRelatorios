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
        public List<AcompanhamentoAprendizagemAlunoFotoDto> Fotos { get; set; }
        public List<AcompanhamentoAprendizagemPercursoTurmaImagemDto> PercursoTurmaImagens { get; set; }

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

        public string PercursoTurmaFormatado()
        {
            if (string.IsNullOrEmpty(ApanhadoGeral))
                return string.Empty;
            
            var i = 1;

            var imagens = Regex.Matches(ApanhadoGeral, "<img.+?>");

            foreach (var imagem in imagens)
            {
                var numeroImagem = i++;
                var textoSemImagem = ApanhadoGeral.Replace(imagem.ToString(), $"<b>imagem {numeroImagem}</b>");
                PercursoTurmaImagens.Add(new AcompanhamentoAprendizagemPercursoTurmaImagemDto
                {
                    NomeImagem = $"imagem {numeroImagem}",
                    Imagem = imagem.ToString().Replace("<img", "<img height='328' width='328'")
                });
                ApanhadoGeral = textoSemImagem;
            }

            return ApanhadoGeral;           
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
