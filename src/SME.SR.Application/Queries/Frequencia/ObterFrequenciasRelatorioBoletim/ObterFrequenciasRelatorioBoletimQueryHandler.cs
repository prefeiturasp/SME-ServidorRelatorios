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
    public class ObterFrequenciasRelatorioBoletimQueryHandler : IRequestHandler<ObterFrequenciasRelatorioBoletimQuery, IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasRelatorioBoletimQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> Handle(ObterFrequenciasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var frequenciasRetorno = new List<FrequenciaAluno>();
            var alunosCodigos = request.CodigosAluno;
            int alunosPorPagina = 100;

            int cont = 0;
            int i = 0;
            while (cont < alunosCodigos.Length)
            {
                var alunosPagina = alunosCodigos.Skip(alunosPorPagina * i).Take(alunosPorPagina).ToList();
                var frequenciasAlunos = await frequenciaRepository.ObterFrequenciasPorTurmasAlunos(alunosPagina.ToArray(), request.AnoLetivo, (int)request.Modalidade, request.Semestre, request.TurmaCodigos);
                frequenciasRetorno.AddRange(frequenciasAlunos.ToList());
                cont += alunosPagina.Count();
                i++;
            }           

            return frequenciasRetorno.GroupBy(f => f.CodigoAluno);
        }
    }
}
