using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPortuguesPorTurmaUseCase : IRelatorioSondagemPortuguesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemPortuguesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesPorTurmaFiltroDto>();

            var semestre = (filtros.Bimestre <= 2) ? 1 : 2;

            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, filtros.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(Int32.Parse(filtros.TurmaCodigo), dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var relatorioPerguntas = await ObterPerguntas(filtros);

            RelatorioSondagemPortuguesPorTurmaRelatorioDto relatorio = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros, relatorioPerguntas, dataDoPeriodo),
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = await ObterLinhas(filtros, alunosDaTurma)
                }
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            var mensagemDaNotificacao = $"Este é o relatório de Sondagem de Português ({relatorio.Cabecalho.Proficiencia}) da turma {relatorio.Cabecalho.Turma} da {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem (Português) - {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre}) - {relatorio.Cabecalho.Turma}";

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesPorTurma", relatorio, request.CodigoCorrelacao, mensagemDaNotificacao, mensagemTitulo));
        }

        private async Task<RelatorioSondagemPortuguesPorTurmaCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, List<RelatorioSondagemPortuguesPorTurmaPerguntaDto> perguntas, DateTime periodo)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });

            return await Task.FromResult(new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = filtros.DreCodigo,
                Periodo = periodo.ToString("dd/MM/yyyy"),
                Rf = filtros.UsuarioRF,
                Turma = filtros.TurmaCodigo.ToString(),
                Ue = filtros.UeCodigo,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                Perguntas = perguntas,
                AnoTurma = filtros.AnoTurma,
                ComponenteCurricular = "Português",
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>> ObterPerguntas(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros)
        {
            switch (filtros.ProficienciaId)
            {
                case ProficienciaSondagemEnum.Leitura:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                            {
                                Id = 1,
                                Nome = "Proficiência"
                            },
                        });
                case ProficienciaSondagemEnum.Escrita:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                        {
                            new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                            {
                                Id = 1,
                                Nome = ""
                            },
                        });
                case ProficienciaSondagemEnum.LeituraVozAlta:
                    return await Task.FromResult(
                        new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                            {
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 1,
                                    Nome = "Não conseguiu ou não quis ler"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 2,
                                    Nome = "Leu com muita dificuldade"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 3,
                                    Nome = "Leu com alguma fluência	"
                                },
                                new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                                {
                                    Id = 4,
                                    Nome = "Leu com fluência"
                                },
                            });
                default:
                    return await Task.FromResult(new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>());
            }
        }

        private async Task<List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>> ObterLinhas(RelatorioSondagemPortuguesPorTurmaFiltroDto filtros, IEnumerable<Aluno> alunos)
        {
            IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto> linhasSondagem = await mediator.Send(new ObterRelatorioSondagemPortuguesPorTurmaQuery()
            {
                DreCodigo = filtros.DreCodigo,
                UeCodigo = filtros.UeCodigo,
                TurmaCodigo = filtros.TurmaCodigo,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.AnoTurma,
                Bimestre = filtros.Bimestre,
                Proficiencia = filtros.ProficienciaId,
            });

            List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto> linhasPlanilha = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();
            foreach (Aluno aluno in alunos)
            {
                var sondagem = linhasSondagem.FirstOrDefault(a => a.AlunoEolCode == aluno.CodigoAluno.ToString());

                linhasPlanilha.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = aluno.CodigoAluno,
                        Nome = aluno.NomeRelatorio,
                        DataSituacao = aluno.DataSituacao.ToString("dd/MM/yyyy"),
                        SituacaoMatricula = aluno.SituacaoMatricula
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto()
                        {
                            PerguntaId = 1,
                            Resposta = sondagem?.Resposta
                        }
                    }
                });
            }
            return await Task.FromResult(linhasPlanilha);
        }
    }
}
