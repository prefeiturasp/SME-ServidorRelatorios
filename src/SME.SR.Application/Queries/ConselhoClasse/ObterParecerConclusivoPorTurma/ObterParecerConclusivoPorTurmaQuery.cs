using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterParecerConclusivoPorTurmaQuery: IRequest<IEnumerable<ConselhoClasseParecerConclusivo>>
    {
        public ObterParecerConclusivoPorTurmaQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }
    }
}
