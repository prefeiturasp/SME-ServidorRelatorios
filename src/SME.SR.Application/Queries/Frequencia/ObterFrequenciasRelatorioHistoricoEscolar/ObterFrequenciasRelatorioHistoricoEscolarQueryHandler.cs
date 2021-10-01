using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Frequencia.ObterFrequenciasRelatorioHistoricoEscolar
{
    public class ObterFrequenciasRelatorioHistoricoEscolarQueryHandler : IRequestHandler<ObterFrequenciasRelatorioHistoricoEscolarQuery, IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasRelatorioHistoricoEscolarQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> Handle(ObterFrequenciasRelatorioHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            var frequencias = await frequenciaRepository.ObterFrequenciasPorTurmasAlunosParaHistoricoEscolar(request.CodigosAluno, request.AnoLetivo, (int)request.Modalidade, request.Semestre);
            return frequencias.GroupBy(f => f.TurmaId);
        }
    }
}
