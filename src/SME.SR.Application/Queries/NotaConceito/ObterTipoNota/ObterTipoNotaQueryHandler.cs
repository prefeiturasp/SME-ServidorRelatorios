using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTipoNotaQueryHandler : IRequestHandler<ObterTipoNotaQuery, string>
    {
        private readonly IPeriodoFechamentoRepository periodoFechamentoRepository;
        private readonly ICicloRepository cicloRepository;
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private readonly INotaTipoRepository notaTipoRepository;

        public ObterTipoNotaQueryHandler(IPeriodoFechamentoRepository periodoFechamentoRepository,
                                        ICicloRepository cicloRepository,
                                        IPeriodoEscolarRepository periodoEscolarRepository,
                                        INotaTipoRepository notaTipoRepository)
        {
            this.periodoFechamentoRepository = periodoFechamentoRepository ?? throw new ArgumentNullException(nameof(periodoFechamentoRepository));
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository));
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.notaTipoRepository = notaTipoRepository ?? throw new ArgumentNullException(nameof(notaTipoRepository));
        }

        public async Task<string> Handle(ObterTipoNotaQuery request, CancellationToken cancellationToken)
        {
            var bimestreFechamento = request.PeriodoEscolar != null ? request.PeriodoEscolar.Bimestre : (await ObterPeriodoUltimoBimestre(request.Turma)).Bimestre;
            PeriodoFechamentoBimestre periodoFechamentoBimestre = await periodoFechamentoRepository.ObterPeriodoFechamentoTurmaAsync(request.Turma.Ue.Id, request.Turma.Dre.Id, request.Turma.AnoLetivo, bimestreFechamento, request.PeriodoEscolar?.Id);

            return await ObterTipoNota(request.Turma, periodoFechamentoBimestre);
        }

        private async Task<PeriodoEscolar> ObterPeriodoUltimoBimestre(Turma turma)
        {
            var periodoEscolarUltimoBimestre = await periodoEscolarRepository.ObterUltimoPeriodoAsync(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre);
            if (periodoEscolarUltimoBimestre == null)
                throw new NegocioException("Não foi possível localizar o período escolar do ultimo bimestre da turma");

            return periodoEscolarUltimoBimestre;
        }

        private async Task<string> ObterTipoNota(Turma turma, PeriodoFechamentoBimestre periodoFechamentoBimestre)
        {
            var dataReferencia = periodoFechamentoBimestre != null ?
                periodoFechamentoBimestre.FinalDoFechamento :
                (await ObterPeriodoUltimoBimestre(turma)).PeriodoFim;

            var tipoNota = await ObterNotaTipo(turma, dataReferencia);
            if (tipoNota == null)
                throw new NegocioException("Não foi possível identificar o tipo de nota da turma");

            return tipoNota;
        }

        private async Task<string> ObterNotaTipo(Turma turma, DateTime dataReferencia)
        {
            var cicloId = await cicloRepository.ObterCicloIdPorAnoModalidade(turma.Ano, turma.ModalidadeCodigo);

            if (cicloId == null)
                throw new NegocioException("Não foi encontrado o ciclo da turma informada");

            return await notaTipoRepository.ObterPorCicloIdDataAvalicacao(cicloId, dataReferencia);
        }
    }
}
