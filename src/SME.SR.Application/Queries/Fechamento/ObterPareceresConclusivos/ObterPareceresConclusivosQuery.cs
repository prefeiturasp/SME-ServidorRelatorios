using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterPareceresConclusivosQuery : IRequest<long[]>
    {
        public ObterPareceresConclusivosQuery(bool aprovada, bool frequencia, bool conselho)
        {
            Aprovada = aprovada;
            Frequencia = frequencia;
            Conselho = conselho;
        }

        public bool Aprovada { get; set; }
        public bool Frequencia { get; set; }
        public bool Conselho { get; set; }


    }
}
