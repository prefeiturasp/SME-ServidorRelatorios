using MediatR;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPareceresConclusivosPorTipoAprovacaoQuery : IRequest<IEnumerable<long>>
    {
        public ObterPareceresConclusivosPorTipoAprovacaoQuery(bool aprovada, bool frequencia, bool conselho, bool nota)
        {
            Aprovada = aprovada;
            Frequencia = frequencia;
            Conselho = conselho;
            Nota = nota;
        }

        public bool Aprovada { get; set; }
        public bool Frequencia { get; set; }
        public bool Conselho { get; set; }
        public bool Nota { get; set; }


    }
}
