using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.NotaConceito.ObterNotasRelatorioHistoricoEscolar
{
    public class ObterNotasRelatorioHistoricoEscolarOldQueryHandler : IRequestHandler<ObterNotasRelatorioHistoricoEscolarOldQuery, IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        private INotaConceitoRepository notasConceitoRepository;

        public ObterNotasRelatorioHistoricoEscolarOldQueryHandler(INotaConceitoRepository notasConceitoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioHistoricoEscolarOldQuery request, CancellationToken cancellationToken)
        {
            var notas = await notasConceitoRepository.ObterNotasTurmasAlunos(request.CodigosTurma, request.CodigosAlunos);

            if (notas == null || !notas.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");

            return notas.GroupBy(nf => nf.CodigoTurma);
        }
    }
}