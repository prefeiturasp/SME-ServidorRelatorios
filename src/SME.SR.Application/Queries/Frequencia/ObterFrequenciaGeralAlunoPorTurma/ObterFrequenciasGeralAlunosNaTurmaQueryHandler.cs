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
    public class ObterFrequenciasGeralAlunosNaTurmaQueryHandler : IRequestHandler<ObterFrequenciasGeralAlunosNaTurmaQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasGeralAlunosNaTurmaQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciasGeralAlunosNaTurmaQuery request, CancellationToken cancellationToken)
        {
            var frequenciaTurma = await frequenciaRepository.ObterFrequenciaGeralAlunoPorAnoModalidadeSemestre(request.AnoTurma, request.TipoCalendarioId);

            if (frequenciaTurma == null || !frequenciaTurma.Any())
                return Enumerable.Empty<FrequenciaAluno>();

            return TratarFrequenciaAnualAluno(frequenciaTurma);
        }

        private IEnumerable<FrequenciaAluno> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaTurma)
        {
            var frequenciaGlobalAlunos = new List<FrequenciaAluno>();

            foreach (var aluno in frequenciaTurma.GroupBy(g => (g.CodigoAluno)))
            {
                frequenciaGlobalAlunos.Add(new FrequenciaAluno()
                {
                    CodigoAluno = aluno.Key,
                    TotalAulas = aluno.Sum(s => s.TotalAulas),
                    TotalAusencias = aluno.Sum(s => s.TotalAusencias),
                    TotalCompensacoes = aluno.Sum(s => s.TotalCompensacoes)
                });
            }
            return frequenciaGlobalAlunos;
        }
    }
}
