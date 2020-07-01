using MediatR;
using SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoEscolarQuery : IRequest<IEnumerable<HistoricoEscolarDTO>>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
