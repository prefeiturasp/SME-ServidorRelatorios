using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterTipoCalendarioPorIdQuery : IRequest<TipoCalendarioDto>
    {
        public ObterTipoCalendarioPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}

