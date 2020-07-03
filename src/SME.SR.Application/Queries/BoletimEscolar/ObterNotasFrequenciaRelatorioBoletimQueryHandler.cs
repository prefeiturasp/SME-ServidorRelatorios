using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasFrequenciaRelatorioBoletimQueryHandler : IRequestHandler<ObterNotasFrequenciaRelatorioBoletimQuery, IEnumerable<IGrouping<string, NotasFrequenciaAlunoBimestre>>>
    {
        private INotasFrequenciaAlunoBimestreRepository notasFrequenciaRepository;

        public ObterNotasFrequenciaRelatorioBoletimQueryHandler(INotasFrequenciaAlunoBimestreRepository notasFrequenciaRepository)
        {
            this.notasFrequenciaRepository = notasFrequenciaRepository ?? throw new ArgumentException(nameof(notasFrequenciaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasFrequenciaAlunoBimestre>>> Handle(ObterNotasFrequenciaRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var notasFrequencia = await notasFrequenciaRepository.ObterNotasFrequenciaAlunosBimestre(request.CodigosTurma, request.CodigosAlunos);

            if (notasFrequencia == null || !notasFrequencia.Any())
                throw new NegocioException("Não foi possível obter as notas/frequência dos alunos");

            return notasFrequencia.GroupBy(nf => nf.CodigoTurma);
        }
    }
}
