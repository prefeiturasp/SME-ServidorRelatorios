using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterUesPorDreSemestreModadalidadeAnoIdQuery : IRequest<IEnumerable<Ue>>
    {
        public ObterUesPorDreSemestreModadalidadeAnoIdQuery(long dreId, int? semestre, int modalidadeId, string[] ano, IEnumerable<int> tipoEscolas)
        {
            DreId = dreId;
            Semestre = semestre;
            ModalidadeId = modalidadeId;
            Ano = ano;
            TipoEscolas = tipoEscolas;
        }

        public long DreId { get; set; }
        public int? Semestre { get; internal set; }
        public int ModalidadeId { get; internal set; }
        public string[] Ano { get; internal set; }
        public IEnumerable<int> TipoEscolas { get; internal set; }
    }
}
