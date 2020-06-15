using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAlunoQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAlunoQuery, RelatorioConselhoClasseArray>
    {
        private IMediator _mediator;
        private readonly IConfiguration configuration;
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ObterRelatorioConselhoClasseAlunoQueryHandler(IMediator mediator, IConfiguration configuration, VariaveisAmbiente variaveisAmbiente)
        {
            this._mediator = mediator;
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<RelatorioConselhoClasseArray> Handle(ObterRelatorioConselhoClasseAlunoQuery request, CancellationToken cancellationToken)
        {
                try
                {

                    SentrySdk.AddBreadcrumb("Iniciando obter Dados", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                    SentrySdk.AddBreadcrumb("CONNECTION STRING DO EOL" + variaveisAmbiente.ConnectionStringEol.ToLower().Replace("password", "tchetche"), "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");


                    var fechamentoTurma = await ObterFechamentoTurmaPorId(request.FechamentoTurmaId);

                    if (fechamentoTurma == null)
                        SentrySdk.AddBreadcrumb("Não foi possível obter o fechamento da turma!!!!!", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                    RelatorioConselhoClasseBase relatorio;

                    int? bimestre = null;

                    var relatorioParaEnviar = new RelatorioConselhoClasseArray();

                    if (fechamentoTurma.PeriodoEscolarId.HasValue)
                    {

                        relatorio = new RelatorioConselhoClasseBimestre();
                        bimestre = fechamentoTurma.PeriodoEscolar.Bimestre;
                        relatorio.Bimestre = fechamentoTurma.PeriodoEscolar.Bimestre.ToString();

                        SentrySdk.AddBreadcrumb($"Obtive o bimestre {bimestre}", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    }
                    else
                    {
                        relatorio = new RelatorioConselhoClasseFinal();
                        relatorio.Bimestre = "Final";
                        SentrySdk.AddBreadcrumb("Obtive o bimestre FINAL", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    }
                    
                    relatorio.Titulo = "Conselho de Classe";
                    relatorio.Turma = fechamentoTurma.Turma.Nome;
                    relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");


                    SentrySdk.AddBreadcrumb("Obtendo a turma..", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    var dreUe = await ObterDreUePorTurma(fechamentoTurma.Turma.CodigoTurma);

                    relatorio.Dre = dreUe.Dre;
                    relatorio.Ue = dreUe.Ue;

                    SentrySdk.AddBreadcrumb($"Obtendo dados do aluno {request.CodigoAluno}", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    var dadosAluno = await ObterDadosAluno(fechamentoTurma.Turma.CodigoTurma, request.CodigoAluno);

                    if (dadosAluno == null)
                        SentrySdk.AddBreadcrumb("Não foi possível obter os dados do aluno!!!!!!", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                    relatorio.AlunoNome = dadosAluno.NomeAluno;
                    relatorio.AlunoNumero = Convert.ToInt32(dadosAluno.NumeroAlunoChamada);
                    relatorio.AlunoDataDeNascimento = dadosAluno.DataNascimento.ToString("dd/MM/yyyy");
                    relatorio.AlunoCodigoEol = dadosAluno.CodigoAluno.ToString();
                    relatorio.AlunoSituacao = $"{dadosAluno.SituacaoMatricula} em {dadosAluno.DataSituacao:dd/MM/yyyy}";

                    SentrySdk.AddBreadcrumb($"Obtendo frequencia global do aluno {request.CodigoAluno}", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    relatorio.AlunoFrequenciaGlobal = await ObterFrequenciaGlobalPorAluno(fechamentoTurma.Turma.CodigoTurma, request.CodigoAluno);

                    if (bimestre.HasValue)
                    {
                        SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesComNota Com Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                        ((RelatorioConselhoClasseBimestre)relatorio).GruposMatrizComponentesComNota =
                            await _mediator.Send(new ObterDadosComponenteComNotaBimestreQuery()
                            {
                                FechamentoTurmaId = request.FechamentoTurmaId,
                                ConselhoClasseId = request.ConselhoClasseId,
                                Turma = fechamentoTurma.Turma,
                                CodigoAluno = request.CodigoAluno,
                                PeriodoEscolar = fechamentoTurma.PeriodoEscolar
                            });


                        SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesSemNota Com Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                        ((RelatorioConselhoClasseBimestre)relatorio).GruposMatrizComponentesSemNota =
                            await _mediator.Send(new ObterDadosComponenteSemNotaBimestreQuery()
                            {
                                CodigoTurma = fechamentoTurma.Turma.CodigoTurma,
                                CodigoAluno = request.CodigoAluno,
                                Bimestre = bimestre
                            });
                    }
                    else
                    {
                        SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesComNota Sem Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                        ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesComNota =
                            await _mediator.Send(new ObterDadosComponenteComNotaFinalQuery()
                            {
                                FechamentoTurmaId = request.FechamentoTurmaId,
                                ConselhoClasseId = request.ConselhoClasseId,
                                Turma = fechamentoTurma.Turma,
                                CodigoAluno = request.CodigoAluno,
                                PeriodoEscolar = fechamentoTurma.PeriodoEscolar
                            });

                        SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesSemNota Sem Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                        ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesSemNota =
                            await _mediator.Send(new ObterDadosComponenteSemNotaFinalQuery()
                            {
                                CodigoTurma = fechamentoTurma.Turma.CodigoTurma,
                                CodigoAluno = request.CodigoAluno,
                                Bimestre = bimestre
                            });
                    }

                    SentrySdk.AddBreadcrumb("Obtendo ObterRecomendacoesPorFechamento", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    var recomendacoes = await ObterRecomendacoesPorFechamento(
                       request.FechamentoTurmaId,
                       request.CodigoAluno);

                    relatorio.RecomendacaoAluno = recomendacoes.RecomendacoesAluno;
                    relatorio.RecomendacaoFamilia = recomendacoes.RecomendacoesFamilia;
                    relatorio.AnotacoesPedagogicas = recomendacoes.AnotacoesPedagogicas;


                    SentrySdk.AddBreadcrumb("Obtendo ObterAnotacoesAluno", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    var anotacoes = await ObterAnotacoesAluno(
                       request.FechamentoTurmaId,
                       request.CodigoAluno
                    );

                    relatorio.AnotacoesAluno = anotacoes;

                    relatorioParaEnviar.Relatorio.Add(relatorio);

                    SentrySdk.AddBreadcrumb("Relatório serializado -> " + JsonConvert.SerializeObject(relatorioParaEnviar), "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                    SentrySdk.CaptureMessage("4.1 FINALIZOU OK - ObterRelatorioConselhoClasseAlunoQueryHandler");

                    return await Task.FromResult(relatorioParaEnviar);
                }

                catch (Exception ex)
                {
                    SentrySdk.CaptureMessage("4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler ERRO");
                    SentrySdk.CaptureException(ex);
                    throw ex;
                }
            
        }

        private async Task<FechamentoTurma> ObterFechamentoTurmaPorId(long fechamentoTurmaId)
        {
            return await _mediator.Send(new ObterFechamentoTurmaPorIdQuery()
            {
                FechamentoTurmaId = fechamentoTurmaId
            });
        }

        private async Task<DreUe> ObterDreUePorTurma(string codigoTurma)
        {
            return await _mediator.Send(new ObterDreUePorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private async Task<Aluno> ObterDadosAluno(string codigoTurma, string codigoAluno)
        {
            return await _mediator.Send(new ObterDadosAlunoQuery()
            {
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno
            });
        }

        private async Task<double> ObterFrequenciaGlobalPorAluno(string codigoTurma, string codigoAluno)
        {
            return await _mediator.Send(new ObterFrequenciaGlobalPorAlunoQuery()
            {
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno
            });
        }

        private async Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno)
        {
            return await _mediator.Send(new ObterRecomendacoesPorFechamentoQuery()
            {
                CodigoAluno = codigoAluno,
                FechamentoTurmaId = fechamentoTurmaId
            });
        }

        private async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> ObterAnotacoesAluno(long fechamentoTurmaId, string codigoAluno)
        {
            return await _mediator.Send(new ObterAnotacoesAlunoQuery()
            {
                CodigoAluno = codigoAluno,
                FechamentoTurmaId = fechamentoTurmaId
            });
        }
    }
}
