using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQueryHandler : IRequestHandler<ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery, IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta>>
    {
        private readonly ICompensacaoAusenciaRepository compensacaoAusenciaRepository;

        public ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQueryHandler(ICompensacaoAusenciaRepository compensacaoAusenciaRepository)
        {
            this.compensacaoAusenciaRepository = compensacaoAusenciaRepository ?? throw new ArgumentNullException(nameof(compensacaoAusenciaRepository));
        }
        public async Task<IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta>> Handle(ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery request, CancellationToken cancellationToken)
        {
            return await compensacaoAusenciaRepository.ObterPorUeModalidadeSemestreComponenteBimestre(request.UeId, (int)request.Modalidade, request.Semestre, request.TurmaCodigo, request.ComponetesCurricularesIds, request.Bimestre);
        }
    }
}
