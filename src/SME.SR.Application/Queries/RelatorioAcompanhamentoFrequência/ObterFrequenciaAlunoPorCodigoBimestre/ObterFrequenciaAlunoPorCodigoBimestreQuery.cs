using MediatR;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoPorCodigoBimestreQuery : IRequest<IEnumerable<FrequenciaAlunoConsolidadoDto>>
    {
        public ObterFrequenciaAlunoPorCodigoBimestreQuery(string bimestre, string[] codigosAlunos)
        {
            Bimestre = bimestre;
            CodigosAlunos = codigosAlunos;
        }

        public string Bimestre { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
