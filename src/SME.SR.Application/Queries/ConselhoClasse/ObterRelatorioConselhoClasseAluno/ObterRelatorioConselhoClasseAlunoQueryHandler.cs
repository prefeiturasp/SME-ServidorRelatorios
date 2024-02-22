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

        public ObterRelatorioConselhoClasseAlunoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioConselhoClasseArray> Handle(ObterRelatorioConselhoClasseAlunoQuery request, CancellationToken cancellationToken)
        {
            var fechamentoTurma = await ObterFechamentoTurmaPorId(request.FechamentoTurmaId);

            if (fechamentoTurma == null)
                await mediator.Send(new SalvarLogViaRabbitCommand("Não foi possível obter o fechamento da turma.", LogNivel.Critico, "ObterRelatorioConselhoClasseAlunoQueryHandler"));

            RelatorioConselhoClasseBase relatorio;
            int? bimestre = null;
            var relatorioParaEnviar = new RelatorioConselhoClasseArray();

            if (fechamentoTurma.PeriodoEscolarId.HasValue)
            {
                relatorio = new RelatorioConselhoClasseBimestre();
                bimestre = fechamentoTurma.PeriodoEscolar.Bimestre;
                relatorio.Bimestre = fechamentoTurma.PeriodoEscolar.Bimestre.ToString();
                relatorio.EhBimestreFinal = false;
            }
            else
            {
                relatorio = new RelatorioConselhoClasseFinal
                {
                    Bimestre = "Final",
                    EhBimestreFinal = true
                };
            }

            relatorio.Titulo = "Ata de conselho de classe";
            relatorio.Turma = fechamentoTurma.Turma.NomeRelatorio;
            relatorio.Data = DateTimeExtension.HorarioBrasilia().ToString("dd/MM/yyyy");

            var turma = await ObterDadosTurma(fechamentoTurma.Turma.Codigo);

            relatorio.Dre = turma.Dre.Abreviacao;
            relatorio.Ue = turma.Ue.Nome;

            var dadosAluno = await ObterDadosAluno(fechamentoTurma.Turma.Codigo, request.CodigoAluno);

            if (dadosAluno == null)
                await mediator.Send(new SalvarLogViaRabbitCommand("Não foi possível obter os dados do aluno.", LogNivel.Critico, "ObterRelatorioConselhoClasseAlunoQueryHandler"));

            relatorio.AlunoNome = dadosAluno.NomeRelatorio;
            relatorio.AlunoNumero = Convert.ToInt32(dadosAluno.NumeroAlunoChamada);
            relatorio.AlunoDataDeNascimento = dadosAluno.DataNascimento.ToString("dd/MM/yyyy");
            relatorio.AlunoCodigoEol = dadosAluno.CodigoAluno.ToString();
            relatorio.AlunoSituacao = dadosAluno.SituacaoRelatorio;

            if (!fechamentoTurma.Turma.AnoLetivo.Equals(2020))
                relatorio.AlunoFrequenciaGlobal = !string.IsNullOrEmpty(request.FrequenciaGlobal) ? request.FrequenciaGlobal : (await ObterFrequenciaGlobalPorAluno(fechamentoTurma.Turma.Codigo, request.CodigoAluno));

            if (bimestre.HasValue)
            {
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

                ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesSemNota =
                    await mediator.Send(new ObterDadosComponenteSemNotaFinalQuery()
                    {
                        CodigoTurma = fechamentoTurma.Turma.Codigo,
                        CodigoAluno = request.CodigoAluno,
                        Bimestre = bimestre
                    });
            }

            var recomendacoes = await ObterRecomendacoesPorFechamento(
               request.FechamentoTurmaId,
               request.CodigoAluno);

            relatorio.RecomendacaoAluno = recomendacoes?.RecomendacoesAluno;
            relatorio.RecomendacaoFamilia = recomendacoes?.RecomendacoesFamilia;
            relatorio.AnotacoesPedagogicas = recomendacoes?.AnotacoesPedagogicas;

            var anotacoes = await ObterAnotacoesAluno(
               request.FechamentoTurmaId,
               request.CodigoAluno
            );

            relatorio.AnotacoesAluno = anotacoes;

            relatorioParaEnviar.Relatorio.Add(relatorio);

            return await Task.FromResult(relatorioParaEnviar);
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
