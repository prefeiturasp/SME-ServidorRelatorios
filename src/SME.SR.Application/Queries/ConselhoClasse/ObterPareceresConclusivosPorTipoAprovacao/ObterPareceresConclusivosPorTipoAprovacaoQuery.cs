using MediatR;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPareceresConclusivosPorTipoAprovacaoQuery : IRequest<IEnumerable<long>>
    {
        public ObterPareceresConclusivosPorTipoAprovacaoQuery(bool aprovado)
        {
            Aprovado = aprovado;            
        }

        public bool Aprovado { get; set; }
        public bool Frequencia { get; set; }
        public bool Conselho { get; set; }
        public bool Nota { get; set; }


    }
}
