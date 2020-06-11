using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAlunoQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAlunoQuery, RelatorioConselhoClasseBase>
    {
        private IMediator _mediator;

        public ObterRelatorioConselhoClasseAlunoQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<RelatorioConselhoClasseBase> Handle(ObterRelatorioConselhoClasseAlunoQuery request, CancellationToken cancellationToken)
        {
            var fechamentoTurma = await ObterFechamentoTurmaPorId(request.FechamentoTurmaId);

            RelatorioConselhoClasseBase relatorio;

            int? bimestre = null;

            if (fechamentoTurma.PeriodoEscolarId.HasValue)
            {
                relatorio = new RelatorioConselhoClasseBimestre();
                bimestre = fechamentoTurma.PeriodoEscolar.Bimestre;
                relatorio.Bimestre = fechamentoTurma.PeriodoEscolar.Bimestre.ToString();
            }
            else
            {
                relatorio = new RelatorioConselhoClasseFinal();
                relatorio.Bimestre = "Final";
            }

            relatorio.Titulo = "Conselho de Classe";
            relatorio.Turma = fechamentoTurma.Turma.Nome;
            relatorio.Data = DateTime.Now;

            var dreUe = await ObterDreUePorTurma(fechamentoTurma.Turma.CodigoTurma);

            relatorio.Dre = dreUe.Dre;
            relatorio.Ue = dreUe.Ue;

            var dadosAluno = await ObterDadosAluno(fechamentoTurma.Turma.CodigoTurma, request.CodigoAluno);

            relatorio.AlunoNome = dadosAluno.NomeAluno;
            relatorio.AlunoNumero = Convert.ToInt32(dadosAluno.NumeroAlunoChamada);
            relatorio.AlunoDataDeNascimento = dadosAluno.DataNascimento;
            relatorio.AlunoCodigoEol = dadosAluno.CodigoAluno.ToString();
            relatorio.AlunoSituacao = $"{dadosAluno.SituacaoMatricula} em {dadosAluno.DataSituacao:dd/MM/yyyy}";

            relatorio.AlunoFrequenciaGlobal = await ObterFrequenciaGlobalPorAluno(fechamentoTurma.Turma.CodigoTurma, request.CodigoAluno);

            if (bimestre.HasValue)
            {
                ((RelatorioConselhoClasseBimestre)relatorio).GruposMatrizComponentesComNota =
                    await _mediator.Send(new ObterDadosComponenteComNotaBimestreQuery()
                    {
                        FechamentoTurmaId = request.FechamentoTurmaId,
                        ConselhoClasseId = request.ConselhoClasseId,
                        Turma = fechamentoTurma.Turma,
                        CodigoAluno = request.CodigoAluno,
                        PeriodoEscolar = fechamentoTurma.PeriodoEscolar
                    });

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
                ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesComNota =
                    await _mediator.Send(new ObterDadosComponenteComNotaFinalQuery()
                    {
                        FechamentoTurmaId = request.FechamentoTurmaId,
                        ConselhoClasseId = request.ConselhoClasseId,
                        Turma = fechamentoTurma.Turma,
                        CodigoAluno = request.CodigoAluno,
                        PeriodoEscolar = fechamentoTurma.PeriodoEscolar
                    });

                ((RelatorioConselhoClasseFinal)relatorio).GruposMatrizComponentesSemNota =
                    await _mediator.Send(new ObterDadosComponenteSemNotaFinalQuery()
                    {
                        CodigoTurma = fechamentoTurma.Turma.CodigoTurma,
                        CodigoAluno = request.CodigoAluno,
                        Bimestre = bimestre
                    });
            }

            var recomendacoes = await ObterRecomendacoesPorFechamento(
               request.FechamentoTurmaId,
               request.CodigoAluno);

            relatorio.RecomendacaoAluno = recomendacoes.RecomendacoesAluno;
            relatorio.RecomendacaoFamilia = recomendacoes.RecomendacoesFamilia;
            relatorio.AnotacoesPedagogicas = recomendacoes.AnotacoesPedagogicas;

            var anotacoes = await ObterAnotacoesAluno(
               request.FechamentoTurmaId,
               request.CodigoAluno
            );

            relatorio.AnotacoesAluno = anotacoes;

            return relatorio;
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
