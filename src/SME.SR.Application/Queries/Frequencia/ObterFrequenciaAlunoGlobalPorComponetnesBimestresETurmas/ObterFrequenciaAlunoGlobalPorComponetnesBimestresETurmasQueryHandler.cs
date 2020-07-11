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
    public class ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQueryHandler : IRequestHandler<ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IMediator mediator;
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQueryHandler(IMediator mediator, IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery request, CancellationToken cancellationToken)
        {
            var frequenciaAlunos = await frequenciaAlunoRepository.ObterFrequenciaPorComponentesBimestresTurmas(request.ComponentesCurriculares.Select(c => c.ToString()).ToArray()
                                                                                                             , new int[0]
                                                                                                             , request.Turmas.Select(a => a.Codigo).ToArray());


            return await TratarFrequenciaAnualAluno(frequenciaAlunos, request.Turmas);
        }

        private async Task<IEnumerable<FrequenciaAluno>> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<Turma> turmas)
        {
            var frequenciaGlobalAlunos = new List<FrequenciaAluno>();

            foreach(var turma in turmas)
            {
                var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre));

                var frequenciaAlunosTurma = frequenciaAlunos.Where(c => c.TurmaId == turma.Codigo);
                var agrupamentoAlunoComponente = frequenciaAlunosTurma.GroupBy(g => (g.CodigoAluno, g.DisciplinaId));
                foreach (var alunoComponente in agrupamentoAlunoComponente)
                {
                    var frequenciaAluno = new FrequenciaAluno()
                    {
                        CodigoAluno = alunoComponente.Key.CodigoAluno,
                        DisciplinaId = alunoComponente.Key.DisciplinaId
                    };

                    for (int bimestre = 1; bimestre <= (turma.ModalidadeCodigo == Infra.Modalidade.EJA ? 2 : 4); bimestre++)
                    {
                        var frequenciaBimestre = alunoComponente.FirstOrDefault(c => c.Bimestre == bimestre);

                        frequenciaAluno.TotalAulas += frequenciaBimestre?.TotalAulas ??
                                            await mediator.Send(new ObterAulasDadasNoBimestreQuery(turma.Codigo, tipoCalendarioId, long.Parse(frequenciaAluno.DisciplinaId), bimestre));

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
