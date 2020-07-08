using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciasRelatorioBoletimQueryHandler : IRequestHandler<ObterFrequenciasRelatorioBoletimQuery, IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasRelatorioBoletimQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> Handle(ObterFrequenciasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var frequencias = await frequenciaRepository.ObterFrequenciasPorTurmasAlunos(request.CodigosTurma, request.CodigosAluno);

            return frequencias.GroupBy(f => f.TurmaId);
        }
    }
}
