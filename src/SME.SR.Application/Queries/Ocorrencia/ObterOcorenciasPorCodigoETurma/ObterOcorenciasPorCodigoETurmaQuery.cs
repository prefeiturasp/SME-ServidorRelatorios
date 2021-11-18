using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.Ocorrencia.ObterOcorenciasPorCodigoETurma
{
    public class ObterOcorenciasPorCodigoETurmaQuery : IRequest<IEnumerable<OcorrenciasPorCodigoTurmaDto>>
    {
        public ObterOcorenciasPorCodigoETurmaQuery(string turmaId, string[] ocorenciaCodigo)
        {
            TurmaId=turmaId;
            OcorenciaCodigo=ocorenciaCodigo;
        }

        public string TurmaId { get; set; }
        public string[] OcorenciaCodigo { get; set; }
    }
}
