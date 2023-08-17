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
    public class ObterNotasRelatorioBoletimQueryHandler : IRequestHandler<ObterNotasRelatorioBoletimQuery, IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        private INotaConceitoRepository notasConceitoRepository;

        public ObterNotasRelatorioBoletimQueryHandler(INotaConceitoRepository notasConceitoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var notasRetorno = new List<NotasAlunoBimestre>();
            var alunosCodigos = request.CodigosAlunos;
            int alunosPorPagina = 200;

            foreach (string codTurma in request.CodigosTurmas)
            {
                var arrTurma = new string[] { codTurma };
                int cont = 0;
                int i = 0;
                while (cont < alunosCodigos.Length)
                {
                    var alunosPagina = alunosCodigos.Skip(alunosPorPagina * i).Take(alunosPorPagina).ToList();
                    var notasAlunosPagina = await notasConceitoRepository.ObterNotasTurmasAlunos(alunosPagina.ToArray(), arrTurma, request.AnoLetivo, request.Modalidade, request.Semestre);
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
