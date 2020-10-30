using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasGeralAlunosNaTurmaQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciasGeralAlunosNaTurmaQuery(string codigoTurma)
        {
            this.CodigoTurma = codigoTurma;
        }

        public string CodigoTurma { get; set; }


    }
}
