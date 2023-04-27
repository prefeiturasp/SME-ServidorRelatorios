using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAlunoQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAlunoQuery, RelatorioConselhoClasseArray>
    {
        private readonly IMediator mediator;
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ObterRelatorioConselhoClasseAlunoQueryHandler(IMediator mediator, VariaveisAmbiente variaveisAmbiente)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
                    relatorio.EhBimestreFinal = false;
                    SentrySdk.AddBreadcrumb($"Obtive o bimestre {bimestre}", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                }
                else
                {
                    relatorio = new RelatorioConselhoClasseFinal();
                    relatorio.Bimestre = "Final";
                    relatorio.EhBimestreFinal = true;
                    SentrySdk.AddBreadcrumb("Obtive o bimestre FINAL", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                }                
                relatorio.Titulo = "Ata de conselho de classe";
                relatorio.Turma = fechamentoTurma.Turma.NomeRelatorio;
                relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");

                SentrySdk.AddBreadcrumb("Obtendo a turma..", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                var turma = await ObterDadosTurma(fechamentoTurma.Turma.Codigo);

                relatorio.Dre = turma.Dre.Abreviacao;
                relatorio.Ue = turma.Ue.NomeRelatorio;

                SentrySdk.AddBreadcrumb($"Obtendo dados do aluno {request.CodigoAluno}", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                var dadosAluno = await ObterDadosAluno(fechamentoTurma.Turma.Codigo, request.CodigoAluno);

                if (dadosAluno == null)
                    SentrySdk.AddBreadcrumb("Não foi possível obter os dados do aluno!!!!!!", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                relatorio.AlunoNome = dadosAluno.NomeRelatorio;
                relatorio.AlunoNumero = Convert.ToInt32(dadosAluno.NumeroAlunoChamada);
                relatorio.AlunoDataDeNascimento = dadosAluno.DataNascimento.ToString("dd/MM/yyyy");
                relatorio.AlunoCodigoEol = dadosAluno.CodigoAluno.ToString();
                relatorio.AlunoSituacao = dadosAluno.SituacaoRelatorio;

                SentrySdk.AddBreadcrumb($"Obtendo frequencia global do aluno {request.CodigoAluno}", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                if (!fechamentoTurma.Turma.AnoLetivo.Equals(2020))
                    relatorio.AlunoFrequenciaGlobal = !string.IsNullOrEmpty(request.FrequenciaGlobal) ? request.FrequenciaGlobal : (await ObterFrequenciaGlobalPorAluno(fechamentoTurma.Turma.Codigo, request.CodigoAluno));

                if (bimestre.HasValue)
                {
                    SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesComNota Com Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");

                    ((RelatorioConselhoClasseBimestre)relatorio).GruposMatrizComponentesComNota =
                        await mediator.Send(new ObterDadosComponenteComNotaBimestreQuery()
                        {
                            FechamentoTurmaId = request.FechamentoTurmaId,
                            ConselhoClasseId = request.ConselhoClasseId,
                            Turma = turma,
                            CodigoAluno = request.CodigoAluno,
                            PeriodoEscolar = fechamentoTurma.PeriodoEscolar,
                            Usuario = request.Usuario
                        });

                    if (fechamentoTurma.Turma.AnoLetivo.Equals(2020))
                        DefinirFrequenciaGlobalBimestre2020(relatorio, ((RelatorioConselhoClasseBimestre)relatorio).GruposMatrizComponentesComNota);

                    SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesSemNota Com Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    ((RelatorioConselhoClasseBimestre)relatorio).GruposMatrizComponentesSemNota =
                        await mediator.Send(new ObterDadosComponenteSemNotaBimestreQuery()
                        {
                            CodigoTurma = fechamentoTurma.Turma.Codigo,
                            CodigoAluno = request.CodigoAluno,
                            Bimestre = bimestre
                        });
                }
                else
                {
                    relatorio.AlunoParecerConclusivo = 
                        await ObterParecerConclusivoPorAluno(request.CodigoAluno, request.ConselhoClasseId);

                    SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesComNota Sem Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesComNota =
                        await mediator.Send(new ObterDadosComponenteComNotaFinalQuery()
                        {
                            FechamentoTurmaId = request.FechamentoTurmaId,
                            ConselhoClasseId = request.ConselhoClasseId,
                            Turma = turma,
                            CodigoAluno = request.CodigoAluno,
                            PeriodoEscolar = fechamentoTurma.PeriodoEscolar,
                            Usuario = request.Usuario
                        });

                    if (fechamentoTurma.Turma.AnoLetivo.Equals(2020))
                        DefinirFrequenciaGlobalFinal2020(relatorio, ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesComNota);

                    SentrySdk.AddBreadcrumb("Obtendo GruposMatrizComponentesSemNota Sem Bimestre", "4.1 - ObterRelatorioConselhoClasseAlunoQueryHandler");
                    ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesSemNota =
                        await mediator.Send(new ObterDadosComponenteSemNotaFinalQuery()
                        {
                            CodigoTurma = fechamentoTurma.Turma.Codigo,
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

        private void DefinirFrequenciaGlobalFinal2020(RelatorioConselhoClasseBase relatorio, IEnumerable<GrupoMatrizComponenteComNotaFinal> listaGrupoMatrizComponenteComNotaFinal)
        {
            var somaPercentuaisFrequencia = 0.0;
            var totalDisciplinas = 0;
            listaGrupoMatrizComponenteComNotaFinal.ToList().ForEach(gmc =>
            {
                somaPercentuaisFrequencia += gmc.ComponentesComNota.Where(cn => !string.IsNullOrEmpty(cn.Frequencia)).Sum(gn => double.Parse(gn.Frequencia));
                var percentualFrequencia = gmc.ComponentesComNotaRegencia?.Frequencia;
                somaPercentuaisFrequencia += string.IsNullOrEmpty(percentualFrequencia) ? 0 : double.Parse(percentualFrequencia);
                totalDisciplinas += gmc.ComponentesComNota.Count();
                totalDisciplinas += gmc.ComponentesComNotaRegencia?.ComponentesCurriculares.Count ?? 0;
            });
            relatorio.AlunoFrequenciaGlobal = Math.Round(somaPercentuaisFrequencia / totalDisciplinas, 2).ToString();
        }

        private void DefinirFrequenciaGlobalBimestre2020(RelatorioConselhoClasseBase relatorio, IEnumerable<GrupoMatrizComponenteComNotaBimestre> listaGrupoMatrizComponenteComNotaBimestre)
        {
            var somaPercentuaisFrequencia = 0.0;
            var totalDisciplinas = 0;
            listaGrupoMatrizComponenteComNotaBimestre.ToList().ForEach(gmc =>
            {
                somaPercentuaisFrequencia += gmc.ComponentesComNota.Where(cn => !string.IsNullOrEmpty(cn.Frequencia)).Sum(gn => double.Parse(gn.Frequencia));
                var percentualFrequencia = gmc.ComponenteComNotaRegencia?.Frequencia;
                somaPercentuaisFrequencia += string.IsNullOrEmpty(percentualFrequencia) ? 0 : double.Parse(percentualFrequencia);
                totalDisciplinas += gmc.ComponentesComNota.Count();
                totalDisciplinas += gmc.ComponenteComNotaRegencia?.ComponentesCurriculares.Count ?? 0;
            });
            relatorio.AlunoFrequenciaGlobal = Math.Round(somaPercentuaisFrequencia / totalDisciplinas, 2).ToString();
        }

        private async Task<string> ObterParecerConclusivoPorAluno(string codigoAluno, long conselhoClasseId)
        {
            return await mediator.Send(new ObterParecerConclusivoPorAlunoQuery()
            {
                 CodigoAluno = codigoAluno,
                  ConselhoClasseId = conselhoClasseId
            }); 
        }

        private async Task<FechamentoTurma> ObterFechamentoTurmaPorId(long fechamentoTurmaId)
        {
            return await mediator.Send(new ObterFechamentoTurmaPorIdQuery()
            {
                FechamentoTurmaId = fechamentoTurmaId
            });
        }

        private async Task<Turma> ObterDadosTurma(string codigoTurma)
        {
            return await mediator.Send(new ObterTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private async Task<Aluno> ObterDadosAluno(string codigoTurma, string codigoAluno)
        {
            return await mediator.Send(new ObterDadosAlunoQuery()
            {
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno
            });
        }

        private async Task<string> ObterFrequenciaGlobalPorAluno(string codigoTurma, string codigoAluno)
        {
            var frequenciaGlobal = await mediator.Send(new ObterFrequenciaGlobalPorAlunoQuery()
            {
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno
            });
            return FrequenciaAluno.FormatarPercentual(frequenciaGlobal);
        }

        private async Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno)
        {
            return await mediator.Send(new ObterRecomendacoesPorFechamentoQuery()
            {
                CodigoAluno = codigoAluno,
                FechamentoTurmaId = fechamentoTurmaId
            });
        }

        private async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> ObterAnotacoesAluno(long fechamentoTurmaId, string codigoAluno)
        {
            return await mediator.Send(new ObterAnotacoesAlunoQuery()
            {
                CodigoAluno = codigoAluno,
                FechamentoTurmaId = fechamentoTurmaId
            });
        }
    }
}
