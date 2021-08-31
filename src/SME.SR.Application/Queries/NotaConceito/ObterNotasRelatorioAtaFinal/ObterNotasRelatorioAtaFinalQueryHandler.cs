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
    public class ObterNotasRelatorioAtaFinalQueryHandler : IRequestHandler<ObterNotasRelatorioAtaFinalQuery, IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        private INotaConceitoRepository notasConceitoRepository;

        public ObterNotasRelatorioAtaFinalQueryHandler(INotaConceitoRepository notasConceitoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioAtaFinalQuery request, CancellationToken cancellationToken)
        {
            var notas = await notasConceitoRepository.ObterNotasTurmasAlunosParaAtaFinalAsync(request.CodigosAlunos, request.CodigoTurma, request.AnoLetivo, request.Modalidade, request.Semestre, request.TiposTurma);

            if (notas == null || !notas.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");

            return notas.GroupBy(nf => nf.CodigoTurma);
        }
    }
}
