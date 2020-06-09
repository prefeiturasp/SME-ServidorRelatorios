using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterAnotacoesAlunoQuery : IRequest<IEnumerable<FechamentoAlunoAnotacaoConselho>>
    {
        public string CodigoAluno { get; set; }
        public long FechamentoTurmaId { get; set; }
    }
}
