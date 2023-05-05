using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoGlobalPorComponentesBimestresETurmasQueryHandler : IRequestHandler<ObterFrequenciaAlunoGlobalPorComponentesBimestresETurmasQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IMediator mediator;
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ObterFrequenciaAlunoGlobalPorComponentesBimestresETurmasQueryHandler(IMediator mediator, IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaAlunoGlobalPorComponentesBimestresETurmasQuery request, CancellationToken cancellationToken)
        {
            var frequenciaAlunos = await frequenciaAlunoRepository.ObterFrequenciaPorComponentesBimestresTurmas(request.ComponentesCurriculares.Select(c => c.ToString()).ToArray()
                                                                                                             , new int[0]
                                                                                                             , request.TurmasCodigos.ToArray());


            return await TratarFrequenciaAnualAluno(frequenciaAlunos, request.TurmasCodigos, request.Modalidade, request.TipoCalendarioId);
        }

        private async Task<IEnumerable<FrequenciaAluno>> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<string> turmasCodigos, Modalidade modalidade, long tipoCalendarioId)
        {
            var frequenciaGlobalAlunos = new List<FrequenciaAluno>();

            foreach(var turmaCodigo in turmasCodigos)
            {

                var frequenciaAlunosTurma = frequenciaAlunos.Where(c => c.TurmaId == turmaCodigo);
                var agrupamentoAlunoComponente = frequenciaAlunosTurma.GroupBy(g => (g.CodigoAluno, g.DisciplinaId));
                foreach (var alunoComponente in agrupamentoAlunoComponente)
                {
                    var frequenciaAluno = new FrequenciaAluno()
                    {
                        CodigoAluno = alunoComponente.Key.CodigoAluno,
                        DisciplinaId = alunoComponente.Key.DisciplinaId
                    };

                    for (int bimestre = 1; bimestre <= (modalidade == Infra.Modalidade.EJA ? 2 : 4); bimestre++)
                    {
                        var frequenciaBimestre = alunoComponente.FirstOrDefault(c => c.Bimestre == bimestre);

                        frequenciaAluno.TotalAulas += frequenciaBimestre?.TotalAulas ??
                                            await mediator.Send(new ObterAulasDadasNoBimestreQuery(turmaCodigo, tipoCalendarioId, new long[] { long.Parse(frequenciaAluno.DisciplinaId) }, bimestre));

                        frequenciaAluno.TotalAusencias += frequenciaBimestre?.TotalAusencias ?? 0;
                        frequenciaAluno.TotalCompensacoes += frequenciaBimestre?.TotalCompensacoes ?? 0;
                    }

                    frequenciaGlobalAlunos.Add(frequenciaAluno);
                }
            }

            return frequenciaGlobalAlunos;
        }
    }
}
