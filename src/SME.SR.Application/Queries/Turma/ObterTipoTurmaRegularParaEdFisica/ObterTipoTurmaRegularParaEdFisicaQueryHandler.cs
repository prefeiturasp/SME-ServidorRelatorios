using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTipoTurmaRegularParaEdFisicaQueryHandler : IRequestHandler<ObterTipoTurmaRegularParaEdFisicaQuery, Dictionary<string, TipoNota?>>
    {
        private readonly IMediator mediator;
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private Dictionary<string, PeriodoEscolar> periodoUltimoBimestre;

        public ObterTipoTurmaRegularParaEdFisicaQueryHandler(IMediator mediator, IPeriodoEscolarRepository periodoEscolarRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.periodoUltimoBimestre = new Dictionary<string, PeriodoEscolar>();
        }

        public async Task<Dictionary<string, TipoNota?>> Handle(ObterTipoTurmaRegularParaEdFisicaQuery request, CancellationToken cancellationToken)
        {
            var retorno = new Dictionary<string, TipoNota?>();

            if (request.Turma.TipoTurma == TipoTurma.EdFisica)
            {
                var turmasCodigosParaConsulta = new List<int>();

                turmasCodigosParaConsulta.AddRange(request.Turma.ObterTiposRegularesDiferentes());

                var codigosTurmasAlunosRelacionadas = await mediator.Send(new ObterTurmaCodigosEAlunosPorAnoLetivoAlunoTipoTurmaQuery(request.Turma.AnoLetivo, request.CodigosAlunos, turmasCodigosParaConsulta, request.Turma.AnoLetivo < DateTimeExtension.HorarioBrasilia().Year));
                var codigoTurmas = codigosTurmasAlunosRelacionadas.GroupBy(g => g.CodigoAluno).Select(x => x.FirstOrDefault()).Select(x => x.CodigoTurma.ToString()).ToArray();
                var turmas = await mediator.Send(new ObterTurmasPorCodigoQuery(codigoTurmas));

                foreach(var turma in turmas)
                {
                    var periodoEscolarUltimoBimestre = await ObterPeriodoUltimoBimestre(turma);
                    var tipoNota = await mediator.Send(new ObterTipoNotaQuery() { PeriodoEscolar = periodoEscolarUltimoBimestre, Turma = turma });
                    var tipo = tipoNota == "Conceito" ? TipoNota.Conceito : TipoNota.Nota;
                    var codigosAlunos = codigosTurmasAlunosRelacionadas.Where(ta => ta.CodigoTurma.ToString() == turma.Codigo).Select(ta => ta.CodigoAluno).Distinct();

                    foreach(var codigoAluno in codigosAlunos)
                    {
                        retorno.Add(codigoAluno.ToString(), tipo);
                    }
                }
            }

            return retorno;
        }

        private async Task<PeriodoEscolar> ObterPeriodoUltimoBimestre(Turma turma)
        {
            var chave = $"{turma.AnoLetivo}_{turma.ModalidadeTipoCalendario}_{turma.Semestre}";

            if (periodoUltimoBimestre.ContainsKey(chave))
                return periodoUltimoBimestre[chave];

            var periodoEscolarUltimoBimestre = await periodoEscolarRepository.ObterUltimoPeriodoAsync(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre);
            if (periodoEscolarUltimoBimestre == null)
                throw new NegocioException("Não foi possível localizar o período escolar do ultimo bimestre da turma");

            periodoUltimoBimestre.Add(chave, periodoEscolarUltimoBimestre);

            return periodoEscolarUltimoBimestre;
        }
    }
}
