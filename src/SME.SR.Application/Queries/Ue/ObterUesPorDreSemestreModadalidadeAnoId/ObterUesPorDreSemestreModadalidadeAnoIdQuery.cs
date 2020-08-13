using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterUesPorDreSemestreModadalidadeAnoIdQuery : IRequest<IEnumerable<Ue>>
    {
        public ObterUesPorDreSemestreModadalidadeAnoIdQuery(long dreId, int? semestre, int modalidadeId, string[] ano)
        {
            DreId = dreId;
            Semestre = semestre;
            ModalidadeId = modalidadeId;
            Ano = ano;
        }

        public long DreId { get; set; }
        public int? Semestre { get; internal set; }
        public int ModalidadeId { get; internal set; }
        public string[] Ano { get; internal set; }
    }
}
