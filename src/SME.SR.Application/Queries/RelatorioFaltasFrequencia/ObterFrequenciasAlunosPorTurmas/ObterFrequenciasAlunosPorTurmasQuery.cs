using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasAlunosPorTurmasQuery : IRequest<IEnumerable<FrequenciaAlunoRetornoDto>>
    {
        public ObterFrequenciasAlunosPorTurmasQuery(string[] codigosturma)
        {
            Codigosturma = codigosturma;
        }

        public ObterFrequenciasAlunosPorTurmasQuery(string codigoTurma)
        {
            Codigosturma = new string[] { codigoTurma };
        }

        public string[] Codigosturma { get; set; }
    }
}
