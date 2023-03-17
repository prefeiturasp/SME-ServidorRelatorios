using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaGlobalRelatorioBoletimQueryHandler : IRequestHandler<ObterFrequenciaGlobalRelatorioBoletimQuery, IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciaGlobalRelatorioBoletimQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> Handle(ObterFrequenciaGlobalRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var frequenciasRetorno = new List<FrequenciaAluno>();
            var alunosCodigos = request.CodigosAluno;
            int alunosPorPagina = 100;

            int cont = 0;
            int i = 0;
            while (cont < alunosCodigos.Length)
            {
                var alunosPagina = alunosCodigos.Skip(alunosPorPagina * i).Take(alunosPorPagina).ToList();
                var frequenciasAlunos = await frequenciaRepository.ObterFrequenciaGlobalAlunos(request.CodigosAluno, request.AnoLetivo, (int)request.Modalidade,request.CodigoTurmas);
                frequenciasRetorno.AddRange(frequenciasAlunos.ToList());
                cont += alunosPagina.Count();
                i++;
            }

            return frequenciasRetorno.GroupBy(f => f.CodigoAluno);
        }
    }
}
