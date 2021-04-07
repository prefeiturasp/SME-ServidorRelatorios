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
    public class ObterFrequenciaGlobalRelatorioBoletimQueryHandler : IRequestHandler<ObterFrequenciaGlobalRelatorioBoletimQuery, IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciaGlobalRelatorioBoletimQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> Handle(ObterFrequenciaGlobalRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var frequencias = await frequenciaRepository.ObterFrequenciaGlobalAlunos(request.CodigosAluno, request.AnoLetivo, (int)request.Modalidade);

            return frequencias.GroupBy(f => f.CodigoAluno);
        }
    }
}
