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
        private readonly IRegistroFrequenciaRepository registroFrequenciaRepository;

        public ObterFrequenciasGeralPorAnoEAlunosQueryHandler(IFrequenciaAlunoRepository frequenciaRepository, IRegistroFrequenciaRepository registroFrequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
            this.registroFrequenciaRepository = registroFrequenciaRepository ?? throw new ArgumentNullException(nameof(registroFrequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciasGeralPorAnoEAlunosQuery request, CancellationToken cancellationToken)
        {
            var frequenciaTurma = await frequenciaRepository
                .ObterFrequenciaGeralPorAnoModalidadeSemestreEAlunos(request.AnoLetivo, request.TipoCalendarioId, request.AlunosCodigo, request.CodigoTurma);

            var totalAulasComComponentes = await registroFrequenciaRepository
                .ObterTotalAulasPorDisciplinaETurmaEBimestre(new string[] { request.CodigoTurma },
                                                             new string[] { }, 
                                                             request.TipoCalendarioId, 
                                                             new int[] { 1, 2, 3, 4 });

            int quantidadeTotalAulas = totalAulasComComponentes
                .Sum(t => t.AulasQuantidade);
;
            if (frequenciaTurma == null || !frequenciaTurma.Any())
                return Enumerable.Empty<FrequenciaAluno>();

            return TratarFrequenciaAnualAluno(frequenciaTurma, quantidadeTotalAulas, request.CodigoTurma);
        }

        private IEnumerable<FrequenciaAluno> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaTurma, int totalAulas, string codigoTurma)
        {
            var frequenciaGlobalAlunos = new List<FrequenciaAluno>();

            var agrupamentoAluno = frequenciaTurma.GroupBy(g => (g.CodigoAluno));

            foreach (var aluno in agrupamentoAluno)
            {
                int totalAulasNaTurma = aluno.Where(a => a.TurmaId.Equals(codigoTurma)).Sum(s => s.TotalAulas);
                int totalAusencias = aluno.Where(a => a.TurmaId.Equals(codigoTurma)).Sum(s => s.TotalAusencias) > totalAulasNaTurma
                    ? totalAulasNaTurma
                    : aluno.Where(a => a.TurmaId.Equals(codigoTurma)).Sum(s => s.TotalAusencias);

                var frequenciaAluno = new FrequenciaAluno()
                {
                    CodigoAluno = aluno.Key,
                    TotalAulas = totalAulasNaTurma,
                    TotalAusencias = totalAusencias,
                    TotalCompensacoes = aluno.Where(a => a.TurmaId.Equals(codigoTurma)).Sum(s => s.TotalCompensacoes),
                };

                frequenciaGlobalAlunos.Add(frequenciaAluno);
            }

            return frequenciaGlobalAlunos;
        }

    }
}
