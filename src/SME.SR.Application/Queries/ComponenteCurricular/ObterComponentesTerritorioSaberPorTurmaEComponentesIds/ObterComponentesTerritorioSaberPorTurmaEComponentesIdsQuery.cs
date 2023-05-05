using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application 
{ 
    public class ObterComponentesTerritorioSaberPorTurmaEComponentesIdsQuery : IRequest<IEnumerable<ComponenteCurricularTerritorioSaber>>
    {
        public string TurmaCodigo { get; set; }
        public long[] ComponentesIds { get; set; }

        public ObterComponentesTerritorioSaberPorTurmaEComponentesIdsQuery(string codigoTurma, long[] componentesIds)
        {
            TurmaCodigo = codigoTurma;
            ComponentesIds = componentesIds;
        }
    }
}
