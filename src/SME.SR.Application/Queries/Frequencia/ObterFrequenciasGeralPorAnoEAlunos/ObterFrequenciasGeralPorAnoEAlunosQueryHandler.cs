using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciasGeralPorAnoEAlunosQueryHandler : IRequestHandler<ObterFrequenciasGeralPorAnoEAlunosQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasGeralPorAnoEAlunosQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciasGeralPorAnoEAlunosQuery request, CancellationToken cancellationToken)
        {
            var frequenciaTurma = await frequenciaRepository.ObterFrequenciaGeralPorAnoModalidadeSemestreEAlunos(request.AnoLetivo, request.TipoCalendarioId, request.AlunosCodigo);

            if (frequenciaTurma == null || !frequenciaTurma.Any())
                return Enumerable.Empty<FrequenciaAluno>();

            return TratarFrequenciaAnualAluno(frequenciaTurma);
        }

        private IEnumerable<FrequenciaAluno> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaTurma)
        {
            var frequenciaGlobalAlunos = new List<FrequenciaAluno>();

            var agrupamentoAluno = frequenciaTurma.GroupBy(g => (g.CodigoAluno));
            foreach (var aluno in agrupamentoAluno)
            {
                var frequenciaAluno = new FrequenciaAluno()
                {
                    CodigoAluno = aluno.Key,
                    TotalAulas = aluno.Sum(s => s.TotalAulas),
                    TotalAusencias = aluno.Sum(s => s.TotalAusencias),
                    TotalCompensacoes = aluno.Sum(s => s.TotalCompensacoes)
                };

                frequenciaGlobalAlunos.Add(frequenciaAluno);
            }

            return frequenciaGlobalAlunos;
        }
    }
}
