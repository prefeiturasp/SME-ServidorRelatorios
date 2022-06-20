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
    public class ObterNotasRelatorioBoletimSimplesQueryHandler : IRequestHandler<ObterNotasRelatorioBoletimSimplesQuery, IEnumerable<IGrouping<string, NotasAlunoBimestreBoletimSimplesDto>>>
    {
        private IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository;
        public ObterNotasRelatorioBoletimSimplesQueryHandler(IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository)
        {
            this.conselhoClasseConsolidadoRepository = conselhoClasseConsolidadoRepository ?? throw new ArgumentException(nameof(conselhoClasseConsolidadoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestreBoletimSimplesDto>>> Handle(ObterNotasRelatorioBoletimSimplesQuery request, CancellationToken cancellationToken)
        {
            var notasRetorno = new List<NotasAlunoBimestreBoletimSimplesDto>();
            var alunosCodigos = request.CodigosAlunos;
            int alunosPorPagina = 100;
            int anoAtual = DateTime.Now.Year;

            foreach (string codTurma in request.CodigosTurmas)
            {
                var arrTurma = new string[] { codTurma };
                int cont = 0;
                int i = 0;
                while (cont < alunosCodigos.Length)
                {
                    var alunosPagina = alunosCodigos.Skip(alunosPorPagina * i).Take(alunosPorPagina).ToList();
                    var notasAlunosPagina = await conselhoClasseConsolidadoRepository.ObterNotasBoletimPorAlunoTurma(alunosCodigos, request.CodigosTurmas, request.Semestre, anoAtual);
                    notasRetorno.AddRange(notasAlunosPagina.ToList());
                    cont += alunosPagina.Count();
                    i++;
                }
            }

            if (notasRetorno == null || !notasRetorno.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");

            return notasRetorno.GroupBy(nf => nf.CodigoAluno);
        }
    }
}
