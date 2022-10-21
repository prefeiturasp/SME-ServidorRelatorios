using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasComplementaresEdFisicaQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasComplementaresEdFisicaQuery(string[] codigosTurmas, string[] codigosAlunos)
        {
            CodigosTurmas = codigosTurmas;
            CodigosAlunos = codigosAlunos;
        }

        public string[] CodigosTurmas { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
