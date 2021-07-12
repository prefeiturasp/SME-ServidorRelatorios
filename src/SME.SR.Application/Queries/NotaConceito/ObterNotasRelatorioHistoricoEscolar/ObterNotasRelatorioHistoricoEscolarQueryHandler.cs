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
    public class ObterNotasRelatorioHistoricoEscolarQueryHandler : IRequestHandler<ObterNotasRelatorioHistoricoEscolarQuery, IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        private INotaConceitoRepository notasConceitoRepository;

        public ObterNotasRelatorioHistoricoEscolarQueryHandler(INotaConceitoRepository notasConceitoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            //var notas = await notasConceitoRepository.ObterNotasTurmasAlunosParaHistoricoEscolasAsync(request.CodigosAlunos, request.AnoLetivo, request.Modalidade, request.Semestre);
            IEnumerable<NotasAlunoBimestre> notasRegulares = await notasConceitoRepository.ObterNotasRegularesTurmasAlunosParaHistoricoEscolasAsync(request.CodigosAlunos, request.AnoLetivo, request.Modalidade, request.Semestre);
            IEnumerable<NotasAlunoBimestre> notasComplementares = await notasConceitoRepository.ObterNotasComplementaresTurmasAlunosParaHistoricoEscolasAsync(request.CodigosAlunos, request.AnoLetivo, request.Modalidade, request.Semestre);
            var notas = (notasRegulares ?? Enumerable.Empty<NotasAlunoBimestre>()).Concat(notasComplementares ?? Enumerable.Empty<NotasAlunoBimestre>());

            if (notas == null || !notas.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");

            return notas.GroupBy(nf => nf.CodigoTurma);
        }
    }
}
