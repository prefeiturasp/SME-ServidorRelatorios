using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTiposNotaRelatorioBoletimQueryHandler : IRequestHandler<ObterTiposNotaRelatorioBoletimQuery, IDictionary<string, string>>
    {
        private readonly IPeriodoFechamentoRepository periodoFechamentoRepository;
        private readonly ICicloRepository cicloRepository;
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private readonly INotaTipoRepository notaTipoRepository;

        public ObterTiposNotaRelatorioBoletimQueryHandler(IPeriodoFechamentoRepository periodoFechamentoRepository,
                                        ICicloRepository cicloRepository,
                                        IPeriodoEscolarRepository periodoEscolarRepository,
                                        INotaTipoRepository notaTipoRepository)
        {
            this.periodoFechamentoRepository = periodoFechamentoRepository ?? throw new ArgumentNullException(nameof(periodoFechamentoRepository));
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository));
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.notaTipoRepository = notaTipoRepository ?? throw new ArgumentNullException(nameof(notaTipoRepository));
        }

        public async Task<IDictionary<string, string>> Handle(ObterTiposNotaRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var bimestreFechamento = (await ObterPeriodoUltimoBimestre(request.AnoLetivo, request.Modalidade.ObterModalidadeTipoCalendario(), request.Semestre)).Bimestre;
            PeriodoFechamentoBimestre periodoFechamentoBimestre = await periodoFechamentoRepository.ObterPeriodoFechamentoTurmaAsync(request.UeId, request.DreId, request.AnoLetivo, bimestreFechamento, null);

            return await ObterTiposNota(request.Turmas, periodoFechamentoBimestre, request.AnoLetivo, request.Modalidade, request.Semestre);
        }

        private async Task<PeriodoEscolar> ObterPeriodoUltimoBimestre(int anoLetivo, ModalidadeTipoCalendario modalidadeTipoCalendario, int semestre)
        {
            var periodoEscolarUltimoBimestre = await periodoEscolarRepository.ObterUltimoPeriodoAsync(anoLetivo, modalidadeTipoCalendario, semestre);
            if (periodoEscolarUltimoBimestre == null)
                throw new NegocioException("Não foi possível localizar o período escolar do ultimo bimestre da turma");

            return periodoEscolarUltimoBimestre;
        }

        private async Task<IDictionary<string, string>> ObterTiposNota(IEnumerable<Turma> turmas, PeriodoFechamentoBimestre periodoFechamentoBimestre,
                                                                                    int anoLetivo, Modalidade modalidade, int semestre)
        {
            var dataReferencia = periodoFechamentoBimestre != null ?
                periodoFechamentoBimestre.FinalDoFechamento :
                (await ObterPeriodoUltimoBimestre(anoLetivo, modalidade.ObterModalidadeTipoCalendario(), semestre)).PeriodoFim;

            var tiposNota = await ObterNotasTipo(turmas, modalidade, dataReferencia);
            if (tiposNota == null)
                throw new NegocioException("Não foi possível identificar o tipo de nota da turma");

            return tiposNota;
        }

        private async Task<IDictionary<string, string>> ObterNotasTipo(IEnumerable<Turma> turmas, Modalidade modalidade, DateTime dataReferencia)
        {
            var anos = turmas.Select(t => t.Ano.ToString()).Distinct();

            var tipoCiclos = await cicloRepository.ObterCiclosPorAnosModalidade(anos.ToArray(), modalidade);

            if (tipoCiclos == null)
                throw new NegocioException("Não foi encontrado o ciclo da turma informada");

            var tiposCicloId = tipoCiclos.Select(t => t.Id);

            var notasTipo = await notaTipoRepository.ObterPorCiclosIdDataAvalicacao(tiposCicloId.ToArray(), dataReferencia);

            var lstTurmasTipoNota = new Dictionary<string, string>();

            turmas = turmas.Any() ? turmas.DistinctBy(t => t.Codigo).ToList() : turmas;

            foreach (var turma in turmas)
                lstTurmasTipoNota.Add(turma.Codigo, notasTipo?.FirstOrDefault(nt => nt.Ciclo == tipoCiclos?.FirstOrDefault(tp => tp.Ano == turma.Ano)?.Id)?.TipoNota);

            return lstTurmasTipoNota;
        }
    }
}
