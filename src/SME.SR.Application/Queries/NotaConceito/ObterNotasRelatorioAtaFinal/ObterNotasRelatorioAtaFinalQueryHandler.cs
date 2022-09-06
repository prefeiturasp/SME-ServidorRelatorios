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
        private const int ANO_LETIVO_TURMAS_ED_FISICA_2020 = 2020;

        public ObterNotasRelatorioAtaFinalQueryHandler(INotaConceitoRepository notasConceitoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioAtaFinalQuery request, CancellationToken cancellationToken)
        {
            int[] modalidadesAtaFinal = VerificaModalidadeRelatorioAtaFinal(request.AnoLetivo, request.TiposTurma, request.Modalidade);

            var notas = await notasConceitoRepository.ObterNotasTurmasAlunosParaAtaFinalAsync(request.CodigosAlunos, request.CodigoTurma, request.AnoLetivo, modalidadesAtaFinal, request.Semestre, request.TiposTurma);

            if (notas == null || !notas.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");

            return notas.GroupBy(nf => nf.CodigoTurma);
        }

        public int[] VerificaModalidadeRelatorioAtaFinal(int anoLetivo, int[] tiposTurmas, int modalidade)
        {
            if (anoLetivo == ANO_LETIVO_TURMAS_ED_FISICA_2020 && tiposTurmas.Contains((int)TipoTurma.EdFisica) && (modalidade == (int)Modalidade.EJA || modalidade == (int)Modalidade.Medio))
                return new int[] { (int)Modalidade.Fundamental, modalidade};
            
            return new int[] { modalidade };
        }
    }
}
