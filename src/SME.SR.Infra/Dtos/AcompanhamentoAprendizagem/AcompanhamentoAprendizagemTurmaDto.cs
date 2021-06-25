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
        public void AddFotoAluno(string AlunoCodigo, ArquivoDto foto)
        {
            var aluno = alunos.FirstOrDefault(a => a.AlunoCodigo == AlunoCodigo);

            if (aluno == null)
                throw new NegocioException($"Não foi possível localizar o nível de código {AlunoCodigo}");

            if (!aluno.Fotos.Any(a => a.Id == foto.Id))
                aluno.Add(foto);
        }
        public string PercursoTurmaFormatado(int quantidadeImagens)
        {
            if (string.IsNullOrEmpty(ApanhadoGeral))
                return string.Empty;
<<<<<<< HEAD
            var registroFormatado = UtilRegex.RemoverTagsHtmlVideo(ApanhadoGeral);
=======

            var registroFormatado = UtilRegex.RemoverTagsHtml(ApanhadoGeral);
            registroFormatado = UtilRegex.RemoverTagsHtmlVideo(registroFormatado);
>>>>>>> 919f53fc... 42072 - Tratar imagens Percurso turma
            var numeroImagem = 0;

            var imagens = Regex.Matches(ApanhadoGeral, "<img.+?>");
            string pattern = @"(https|http):.*(jpg|jpeg|gif|png|bmp)";

            foreach (var imagem in imagens)
            {
                numeroImagem++;
                if (numeroImagem <= quantidadeImagens)
                {
<<<<<<< HEAD
                    string input = imagem.ToString();
                    Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        PercursoTurmaImagens.Add(new AcompanhamentoAprendizagemPercursoTurmaImagemDto
                        {
                            NomeImagem = $"imagem {numeroImagem}",
                            Imagem = match.Value
                        });
                    }
=======
>>>>>>> 919f53fc... 42072 - Tratar imagens Percurso turma
                    registroFormatado = registroFormatado.Replace(imagem.ToString(), $"|imagem {numeroImagem}|");
                }
                else
                {
                    registroFormatado = registroFormatado.Replace(imagem.ToString(), $"");
<<<<<<< HEAD
                }                               
            }
            return UtilRegex.RemoverTagsHtml(registroFormatado);
=======
                }

                string pattern = @"(https|http):.*(jpg|jpeg|gif|png|bmp)";
                string input = imagem.ToString();
                Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    PercursoTurmaImagens.Add(new AcompanhamentoAprendizagemPercursoTurmaImagemDto
                    {
                        NomeImagem = $"imagem {numeroImagem}",
                        Imagem = match.Value
                    });
                }                
            }
            return registroFormatado;
>>>>>>> 919f53fc... 42072 - Tratar imagens Percurso turma
        }
    }
}
