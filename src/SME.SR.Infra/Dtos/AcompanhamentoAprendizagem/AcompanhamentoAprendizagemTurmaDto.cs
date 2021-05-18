using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemTurmaDto
    {
        public AcompanhamentoAprendizagemTurmaDto()
        {
            alunos = new List<AcompanhamentoAprendizagemAlunoDto>();
            PercursoTurmaImagens = new List<AcompanhamentoAprendizagemPercursoTurmaImagemDto>();
        }

        public long Id { get; set; }
        public string ApanhadoGeral { get; set; }
        public int Semestre { get; set; }
        public List<AcompanhamentoAprendizagemAlunoDto> Alunos { get { return alunos; } }
        public List<AcompanhamentoAprendizagemPercursoTurmaImagemDto> PercursoTurmaImagens { get; set; }

        private List<AcompanhamentoAprendizagemAlunoDto> alunos { get; set; }

        public void Add(AcompanhamentoAprendizagemAlunoDto acompanhamentoAprendizagemAluno)
        {
            if (!alunos.Any(a => a.Id == acompanhamentoAprendizagemAluno.Id))
                alunos.Add(acompanhamentoAprendizagemAluno);
        }
        public void AddFotoAluno(string AlunoCodigo, AcompanhamentoAprendizagemAlunoFotoDto foto)
        {
            var aluno = alunos.FirstOrDefault(a => a.AlunoCodigo == AlunoCodigo);

            if (aluno == null)
                throw new NegocioException($"Não foi possível localizar o nível de código {AlunoCodigo}");

            if (!aluno.Fotos.Any(a => a.Id == foto.Id))
                aluno.Add(foto);
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

            foreach (var img in imagens)
            {
                var numeroImagem = j++;
                var textoSemImagem = registrosemTag.Replace($"imagem {numeroImagem}", $"<b>imagem {numeroImagem}</b>");

                registrosemTag = textoSemImagem;
            }
            return registrosemTag;
        }
    }
}
