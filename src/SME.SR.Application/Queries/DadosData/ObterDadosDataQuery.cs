using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterDadosDataQuery : IRequest<DadosDataDto>
    {
        public bool PreencherData { get; set; }
    }
}
