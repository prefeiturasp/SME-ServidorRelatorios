using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAtribuicoesEsporadicasPorFiltroQuery : IRequest<IEnumerable<AtribuicaoEsporadica>>
    {
        public ObterAtribuicoesEsporadicasPorFiltroQuery(int anoLetivo, string codigoRF, string dreId, string professorRF, string ueId)
        {
            AnoLetivo = anoLetivo;
            CodigoRF = codigoRF;
            DreId = dreId;
            ProfessorRF = professorRF;
            UeId = ueId;
        }

        public int AnoLetivo { get; set; }

        public string CodigoRF { get; set; }

        public string DreId { get; set; }

        public string ProfessorRF { get; set; }

        public string UeId { get; set; }
    }
}
