using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoAprendizagemQuery, RelatorioAcompanhamentoAprendizagemDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioAcompanhamentoAprendizagemQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioAcompanhamentoAprendizagemDto> Handle(ObterRelatorioAcompanhamentoAprendizagemQuery request, CancellationToken cancellationToken)
        {

            var alunosEol = request.AlunosEol;
            var acompanhmentosAlunos = request.AcompanhamentosAlunos;
            var frequenciaAlunos = request.FrequenciaAlunos;
            var registrosIndividuais = request.RegistrosIndividuais;
            var ocorrencias = request.Ocorrencias;

            var relatorio = new RelatorioAcompanhamentoAprendizagemDto
            {
                Cabecalho = MontarCabecalho(acompanhmentosAlunos.FirstOrDefault()),
                Alunos = MontarAlunos(acompanhmentosAlunos, alunosEol, frequenciaAlunos, registrosIndividuais, ocorrencias),
            };            

            return relatorio;            
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto MontarCabecalho(AcompanhamentoAprendizagemAlunoRetornoDto acompanhamentoAluno)
        {
            var cabecalho = new RelatorioAcompanhamentoAprendizagemCabecalhoDto
            {
                Dre = acompanhamentoAluno.DreAbreviacao,
                Ue = acompanhamentoAluno.UeNomeFormatado(),
                Turma = acompanhamentoAluno.TurmaNome,
                Semestre = acompanhamentoAluno.SemestreFormatado(),
            };

            return cabecalho;
        }

        private List<RelatorioAcompanhamentoAprendizagemAlunoDto> MontarAlunos(IEnumerable<AcompanhamentoAprendizagemAlunoRetornoDto> alunosAcompanhamento, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> Ocorrencias)
        {
            var alunosRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoDto>();
            

            foreach (var aluno in alunosAcompanhamento)
            {                
                var alunoEol = alunosEol.FirstOrDefault(a => a.AlunoCodigo == long.Parse(aluno.AlunoCodigo));

                if (alunoEol == null)
                    throw new NegocioException("AlunoEol não encontrado");

                var alunoRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoDto();
                alunoRelatorio.Nome = alunoEol.NomeRelatorio;
                alunoRelatorio.DataNascimento = alunoEol.DataNascimentoFormatado();
                alunoRelatorio.CodigoEol = alunoEol.AlunoCodigo.ToString();
                alunoRelatorio.Situacao = alunoEol.SituacaoRelatorio;
                alunoRelatorio.Responsavel = alunoEol.ResponsavelFormatado();
                alunoRelatorio.Telefone = alunoEol.ResponsavelCelularFormatado();
                alunoRelatorio.RegistroPercursoTurma = aluno.PercusoTurmaFormatado();
                alunoRelatorio.Observacoes = aluno.ObservacoesFormatado();

                // TODO : Verificar como recuperar o caminho da foto
                foreach (var foto in aluno.Fotos)
                {
                    alunoRelatorio.Fotos.Add(new RelatorioAcompanhamentoAprendizagemAlunoFotoDto{ Caminho = "https://media.gazetadopovo.com.br/viver-bem/2017/03/criancadocumento-600x401-ce1bce00.jpg" });
                }

                alunoRelatorio.Frequencias = MontarFrequencias(alunoRelatorio.CodigoEol, frequenciasAlunos);
                alunoRelatorio.RegistrosIndividuais = MontarRegistrosIndividuais(alunoRelatorio.CodigoEol, registrosIndividuais);
                alunoRelatorio.Ocorrencias = MontarOcorrencias(alunoRelatorio.CodigoEol, Ocorrencias);

                alunosRelatorio.Add(alunoRelatorio);
            }
            return alunosRelatorio;
        }


        private List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto> MontarFrequencias(string alunoCodigo, IEnumerable<FrequenciaAluno> frequenciasAlunos)
        {
            var freqenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();           

            if (frequenciasAlunos == null || !frequenciasAlunos.Any())
                return null;

            var frequenciasFiltrada = frequenciasAlunos.Where(f => f.CodigoAluno == alunoCodigo);

            foreach (var frequencia in frequenciasFiltrada.OrderBy(b => b.Bimestre))
            {
                var freqenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto
                {
                    Bimestre = $"{frequencia.Bimestre}º",
                    Aulas = frequencia.TotalAulas,
                    Ausencias = frequencia.TotalAusencias,
                    Frequencia = $"{frequencia.PercentualFrequencia}%",
                };               

                freqenciasRelatorio.Add(freqenciaRelatorio);
            }
            return freqenciasRelatorio;
        }

        private List<RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto> MontarRegistrosIndividuais(string alunoCodigo, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais)
        {
            var registrosIndividuaisRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto>();            

            if (registrosIndividuais == null || !registrosIndividuais.Any())
                return null;

            var registrosIndividuaisFiltrados = registrosIndividuais.Where(r => r.AlunoCodigo == long.Parse(alunoCodigo));

            if (registrosIndividuaisFiltrados == null || !registrosIndividuaisFiltrados.Any())
                return null;

            foreach (var registro in registrosIndividuaisFiltrados.OrderByDescending(b => b.DataRegistro))
            {
                var registroIndividualRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto
                {
                    Data = registro.DataRelatorio,
                    Descricao = registro.RegistroFormatado(),
                };                

                registrosIndividuaisRelatorio.Add(registroIndividualRelatorio);
            }
            return registrosIndividuaisRelatorio;
        }
        private List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto> MontarOcorrencias(string alunoCodigo, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias)
        {
            var ocorrenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto>();            

            if (ocorrencias == null || !ocorrencias.Any())
                return null;

            var ocorrenciasFiltradas = ocorrencias.Where(r => r.AlunoCodigo == long.Parse(alunoCodigo));

            if (ocorrenciasFiltradas == null || !ocorrenciasFiltradas.Any())
                return null;

            foreach (var ocorrencia in ocorrenciasFiltradas.OrderByDescending(o => o.DataOcorrencia))
            {
                var ocorrenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto
                {
                    Data = ocorrencia.DataRelatorio(),
                    Tipo = ocorrencia.TipoOcorrencia,
                    Descricao = ocorrencia.DescricaoFormatada(),
                    Titulo = ocorrencia.TituloOcorrencia,
                };
                ocorrenciasRelatorio.Add(ocorrenciaRelatorio);
            }
            return ocorrenciasRelatorio;
        }

        private RelatorioAcompanhamentoAprendizagemAlunoDto GerarAluno()
        {
            var aluno = new RelatorioAcompanhamentoAprendizagemAlunoDto();
            aluno.CodigoEol = "4241513";
            aluno.DataNascimento = "02/02/2012";
            aluno.Nome = "Nº1 - ALANA FERREIRA DE OLIVEIRA";
            aluno.Responsavel = "JONATHAN DA SILVA PEREIRA (FILIAÇÃO1)";
            aluno.Telefone = "(11) 94596-3666 (Atualizado - 26/06/2018)";
            aluno.Situacao = "MATRICULADO EM 04/02/2019";

            aluno.Fotos.Add(new RelatorioAcompanhamentoAprendizagemAlunoFotoDto()
            {
                Caminho = "https://media.gazetadopovo.com.br/viver-bem/2017/03/criancadocumento-600x401-ce1bce00.jpg"
            });

            aluno.Frequencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto()
            {
                Aulas = 50,
                Ausencias = 5,
                Frequencia = "90%",
                Bimestre = "1º"
            });

            aluno.Frequencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto()
            {
                Aulas = 50,
                Ausencias = 10,
                Frequencia = "80%",
                Bimestre = "2º"
            });

            aluno.Ocorrencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto()
            {
                Data = "02/10/2020",
                Tipo = "LISTAS OS TIPOS",
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                Titulo = "Títutlo de teste"
            });

            aluno.Ocorrencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto()
            {
                Data = "02/10/2020",
                Tipo = "LISTAS OS TIPOS",
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                Titulo = "Títutlo de teste"
            });

            aluno.Ocorrencias.Add(new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto()
            {
                Data = "02/10/2020",
                Tipo = "LISTAS OS TIPOS",
                Descricao = "No mundo atual, a expansão dos mercados mundiais exige a precisão e a definição do retorno esperado a longo prazo.",
                Titulo = "Títutlo de teste"
            });

            aluno.Observacoes = "O cuidado em identificar pontos críticos no novo modelo estrutural aqui preconizado nos obriga à análise do sistema de formação de quadros que corresponde às necessidades.";

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "Desta maneira, o desenvolvimento contínuo de distintas formas de atuação garante a contribuição de um grupo importante na determinação das direções preferenciais no sentido do progresso."
            });

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "O cuidado em identificar pontos críticos na necessidade de renovação processual ainda não demonstrou convincentemente que vai participar na mudança das condições financeiras e administrativas exigidas."
            });

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "Todas estas questões, devidamente ponderadas, levantam dúvidas sobre se o novo modelo estrutural aqui preconizado desafia a capacidade de equalização dos procedimentos normalmente adotados."
            });

            aluno.RegistrosIndividuais.Add(new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto()
            {
                Data = "04/12/2020",
                Descricao = "O cuidado em identificar pontos críticos na necessidade de renovação processual ainda não demonstrou convincentemente que vai participar na mudança das condições financeiras e administrativas exigidas."
            });

            aluno.RegistroPercursoTurma = "Gostaria de enfatizar que o surgimento do comércio virtual apresenta tendências no sentido de aprovar a manutenção das condições financeiras e administrativas exigidas.";

            return aluno;
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto GerarCabecalho()
        {
            return new RelatorioAcompanhamentoAprendizagemCabecalhoDto()
            {
                Dre = "BUTANTA",
                Ue = "EMEI ANTONIO BENTO",
                Turma = "5B",
                Professores = "FRANCISCA DA SILTA MATA, JESSICA DE OLIVEIRA",
                Semestre = "1º SEMESTRE 2021"
            };

        }
    }
}
