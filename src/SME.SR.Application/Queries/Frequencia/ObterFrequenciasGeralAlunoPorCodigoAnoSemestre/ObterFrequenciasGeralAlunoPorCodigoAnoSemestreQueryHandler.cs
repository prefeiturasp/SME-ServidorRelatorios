using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SME.SR.Data.Interfaces;

namespace SME.SR.Application
{
    internal class ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQueryHandler : IRequestHandler<ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IRegistroFrequenciaAlunoRepository repositorioFrequenciaAluno;

        public ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQueryHandler(IRegistroFrequenciaAlunoRepository repositorioFrequenciaAluno)
        {
            this.repositorioFrequenciaAluno = repositorioFrequenciaAluno ?? throw new ArgumentNullException(nameof(repositorioFrequenciaAluno));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQuery request, CancellationToken cancellationToken)
            => await repositorioFrequenciaAluno.ObterFrequenciaGeralAlunoPorAnoModalidadeSemestre(request.CodigoAluno, request.AnoTurma, request.TipoCalendarioId);
    }
}
