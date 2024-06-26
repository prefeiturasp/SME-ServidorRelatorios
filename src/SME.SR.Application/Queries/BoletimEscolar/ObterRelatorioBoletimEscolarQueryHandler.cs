﻿using MediatR;
using SME.SR.Application.Queries.BoletimEscolar;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, List<RelatorioBoletimSimplesEscolarDto>>
    {
        private readonly IMediator mediator;
        private const string PRIMEIRO_ANO = "1";

        public ObterRelatorioBoletimEscolarQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<List<RelatorioBoletimSimplesEscolarDto>> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.TurmaCodigo))
                await ObterFechamentoTurma(request.TurmaCodigo);

            var dre = await ObterDrePorCodigo(request.DreCodigo);
            var ue = await ObterUePorCodigo(request.UeCodigo);
            var modalidadeCalendario = request.Modalidade.ObterModalidadeTipoCalendario();
            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(request.AnoLetivo, modalidadeCalendario, request.Semestre));
            var periodoEscolares = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));
            var dataInicioPeriodoEscolar = periodoEscolares?.OrderBy(p=> p.Bimestre).FirstOrDefault().PeriodoInicio ?? null;

            var turmas = await ObterTurmasRelatorio(request.TurmaCodigo, request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.ConsideraHistorico);
            var mediasFrequencia = await ObterMediasFrequencia();
            var alunosPorTurma = await ObterAlunosPorTurmasRelatorio(turmas.Select(t => t.Codigo).ToArray(), request.AlunosCodigo, request.ConsideraInativo, request.AnoLetivo, dataInicioPeriodoEscolar);
            var alunosAPesquisarTurmas = request.AlunosCodigo.Any() ? request.AlunosCodigo : alunosPorTurma.Select(a => a.Key);
            var turmasComplementaresEdFisica = await mediator.Send(new ObterTurmasComplementaresEdFisicaQuery(turmas.Select(t => t.Codigo).ToArray(), alunosAPesquisarTurmas.ToArray(), request.AnoLetivo));

            if (!string.IsNullOrEmpty(request.TurmaCodigo))
            {
                var turmasComplementaresEM = await RetornaPossibilidadeMatricula2TurmasRegularesNovoEM(Convert.ToInt32(request.TurmaCodigo), alunosAPesquisarTurmas.ToArray());

                if (request.Modalidade == Modalidade.Medio && turmasComplementaresEM.Any())
                    turmas = turmasComplementaresEM.OrderBy(b => b.Ano);
            }

            if (turmasComplementaresEdFisica != null && turmasComplementaresEdFisica.Any())
                turmas = turmas.Concat(turmasComplementaresEdFisica).Distinct();

            var codigosTurma = turmasComplementaresEdFisica != null && turmasComplementaresEdFisica.Any() ? turmas.Concat(turmasComplementaresEdFisica).Select(t => t.Codigo).Distinct().ToArray() : turmas.Select(t => t.Codigo).ToArray();
            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(turmas.Select(t => int.Parse(t.Codigo)).ToArray(), alunosPorTurma.SelectMany(t => t.Select(t => t.CodigoAluno)).Distinct().ToArray(), request.AnoLetivo, request.Semestre, request.UeCodigo, request.Modalidade, request.Usuario, request.ConsideraHistorico);
            var tiposNota = await ObterTiposNotaRelatorio(request.AnoLetivo, dre.Id, ue.Id, request.Semestre, request.Modalidade, turmas);
            string[] codigosAlunos = componentesCurriculares.Select(cc => cc.Key).ToArray();

            var notas = await ObterNotasAlunos(codigosAlunos, codigosTurma, request.AnoLetivo, request.Modalidade, request.Semestre);
            var pareceresConclusivos = await ObterPareceresConclusivos(dre.Codigo, ue.Codigo, turmas, request.AnoLetivo, request.Modalidade, request.Semestre);
            var frequencias = await ObterFrequenciasAlunos(codigosAlunos, request.AnoLetivo, request.Modalidade, request.Semestre, codigosTurma);
            var codigosDisciplinas = componentesCurriculares.SelectMany(cc => cc.Select(cc => cc.CodDisciplina.ToString())).Distinct().ToArray();
            var aulasPrevistas = await mediator.Send(new ObterAulasDadasTurmaBimestreComponenteCurricularQuery(codigosTurma, tipoCalendarioId, codigosDisciplinas));
            var frequenciaGlobal = await ObterFrequenciaGlobalAlunos(codigosAlunos, request.AnoLetivo, request.Modalidade, codigosTurma);

            return await MontarBoletins(dre, ue, turmas.Union(turmasComplementaresEdFisica).Distinct(), componentesCurriculares, alunosPorTurma, notas, pareceresConclusivos, frequencias, tiposNota, mediasFrequencia, frequenciaGlobal, aulasPrevistas);
        }

        private async Task<IEnumerable<Turma>> RetornaPossibilidadeMatricula2TurmasRegularesNovoEM(int codigoTurma, string[] alunosCodigos)
        {
            var dadosTurma = await mediator.Send(new ObterTurmaPorCodigoQuery(codigoTurma.ToString()));
            var turmasItinerarioEnsinoMedio = (await mediator.Send(new ObterTurmaItinerarioEnsinoMedioQuery())).ToList();

            if (dadosTurma.AnoLetivo >= 2021 && dadosTurma.Ano != PRIMEIRO_ANO)
            {
                var tiposTurmasParaConsulta = new List<int> { (int)dadosTurma.TipoTurma };
                tiposTurmasParaConsulta.AddRange(turmasItinerarioEnsinoMedio.Select(t => t.Id));

                var turmasEol = await mediator.Send(new ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery(dadosTurma.AnoLetivo, alunosCodigos,
                   tiposTurmasParaConsulta, dadosTurma.AnoLetivo < DateTimeExtension.HorarioBrasilia().Year, DateTimeExtension.HorarioBrasilia()));

                var turmasRetorno = await mediator.Send(new ObterTurmasPorCodigoQuery(turmasEol.Select(t => t.ToString()).ToArray()));

                if (turmasRetorno.Any())
                    return turmasRetorno;
            }

            return new List<Turma>() { };
        }

        private async Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> ObterPareceresConclusivos(string dreCodigo, string ueCodigo, IEnumerable<Turma> turmas, int anoLetivo, Modalidade modalidade, int semestre)
        {
            var turmasCodigo = turmas.Select(t => t.Codigo)?.ToArray();

            return await mediator.Send(new ObterPareceresFinaisConsolidadoQuery()
            {
                DreCodigo = dreCodigo,
                UeCodigo = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade,
                Semestre = semestre,
                TurmasCodigo = turmasCodigo
            });
        }

        private async Task ObterFechamentoTurma(string codigoTurma)
        {
            var obterFechamentosPorCodigosTurmaQuery = new ObterFehamentoPorCodigoTurmaQuery()
            {
                CodigoTurma = codigoTurma
            };

            await mediator.Send(obterFechamentosPorCodigosTurmaQuery);
        }

        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorio(string turmaCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico)
        {
            try
            {
                return await mediator.Send(new ObterTurmasRelatorioBoletimQuery()
                {
                    CodigoTurma = turmaCodigo,
                    CodigoUe = ueCodigo,
                    Modalidade = modalidade,
                    AnoLetivo = anoLetivo,
                    Semestre = semestre,
                    Usuario = usuario,
                    ConsideraHistorico = consideraHistorico
                });
            }
            catch (NegocioException)
            {
                throw new NegocioException("As turmas selecionadas não possuem fechamento.");
            }
        }
       
        private async Task<IEnumerable<IGrouping<string, Aluno>>> ObterAlunosPorTurmasRelatorio(string[] turmasCodigo, string[] alunosCodigo, bool trazerAlunosInativos, int anoLetivo, DateTime? dataInicioPeriodoEscolar = null)
        {
            return await mediator.Send(new ObterAlunosTurmasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo,
                TrazerAlunosInativos = trazerAlunosInativos,
                AnoLetivo = anoLetivo,
                DataInicioPeriodoEscolar = dataInicioPeriodoEscolar
            });
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(int[] codigosTurmas, int[] codigosAlunos, int anoLetivo, int semestre, string codigoUe, Modalidade modalidade, Usuario usuario, bool consideraHistorico = false)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorAlunosQuery(codigosTurmas, codigosAlunos, anoLetivo, semestre, codigoUe, modalidade, usuario, consideraHistorico));
        }

        private async Task<IDictionary<string, string>> ObterTiposNotaRelatorio(int anoLetivo, long dreId, long ueId, int semestre, Modalidade modalidade, IEnumerable<Turma> turmas)
        {
            return await mediator.Send(new ObterTiposNotaRelatorioBoletimQuery()
            {
                AnoLetivo = anoLetivo,
                DreId = dreId,
                UeId = ueId,
                Semestre = semestre,
                Modalidade = modalidade,
                Turmas = turmas
            });
        }

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestreBoletimSimplesDto>>> ObterNotasAlunos(string[] alunosCodigo, string[] codigosTurmas, int anoLetivo, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterNotasRelatorioBoletimSimplesQuery(alunosCodigo, codigosTurmas, anoLetivo, (int)modalidade, semestre));
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre, string[] turmaCodigo)
        {
            return await mediator.Send(new ObterFrequenciasRelatorioBoletimQuery(alunosCodigo, anoLetivo, modalidade, semestre, turmaCodigo));
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciaGlobalAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, string[] codigoTurmas)
        {
            return await mediator.Send(new ObterFrequenciaGlobalRelatorioBoletimQuery(alunosCodigo, anoLetivo, modalidade, codigoTurmas));
        }

        private async Task<List<RelatorioBoletimSimplesEscolarDto>> MontarBoletins(Dre dre, Ue ue, IEnumerable<Turma> turmas, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares,
                                                             IEnumerable<IGrouping<string, Aluno>> alunosPorTurma, IEnumerable<IGrouping<string, NotasAlunoBimestreBoletimSimplesDto>> notasAlunos,
                                                             IEnumerable<RelatorioParecerConclusivoRetornoDto> pareceresConclusivos, IEnumerable<IGrouping<string, FrequenciaAluno>> frequenciasAlunos,
                                                             IDictionary<string, string> tiposNota, IEnumerable<MediaFrequencia> mediasFrequencias, IEnumerable<IGrouping<string, FrequenciaAluno>> frequenciaGlobal,
                                                             IEnumerable<TurmaComponenteQuantidadeAulasDto> aulasPrevistas)
        {
            return await mediator.Send(new MontarBoletinsQuery()
            {
                Dre = dre,
                Ue = ue,
                ComponentesCurriculares = componentesCurriculares,
                Turmas = turmas,
                AlunosPorTuma = alunosPorTurma,
                Notas = notasAlunos,
                Frequencias = frequenciasAlunos,
                TiposNota = tiposNota,
                MediasFrequencia = mediasFrequencias,
                PareceresConclusivos = pareceresConclusivos,
                FrequenciasGlobal = frequenciaGlobal,
                AulasPrevistas = aulasPrevistas
            });
        }

        private async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            return await mediator.Send(new ObterParametrosMediaFrequenciaQuery());
        }        
    }

    internal class ComparadorFrequencia : IEqualityComparer<FrequenciaAluno>
    {
        public bool Equals([AllowNull] FrequenciaAluno x, [AllowNull] FrequenciaAluno y)
            => x.CodigoAluno.Equals(y.CodigoAluno)
            && x.TurmaId.Equals(y.TurmaId)
            && x.DisciplinaId.Equals(y.DisciplinaId)
            && x.Bimestre.Equals(y.Bimestre);

        public int GetHashCode([DisallowNull] FrequenciaAluno obj)
            => string.Concat(obj.TurmaId, obj.CodigoAluno, obj.DisciplinaId, obj.Bimestre.ToString()).GetHashCode();
    }
}