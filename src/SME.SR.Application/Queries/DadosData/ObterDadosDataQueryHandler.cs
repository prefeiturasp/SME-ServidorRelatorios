using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosDataQueryHandler : IRequestHandler<ObterDadosDataQuery, DadosDataDto>
    {
        private readonly IParametroSistemaRepository _parametroSistemaRepository;

        public ObterDadosDataQueryHandler(IParametroSistemaRepository parametroSistemaRepository)
        {
            this._parametroSistemaRepository = parametroSistemaRepository ?? throw new ArgumentNullException(nameof(parametroSistemaRepository));
        }

        public async Task<DadosDataDto> Handle(ObterDadosDataQuery request, CancellationToken cancellationToken)
        {
            var parametroMunicipio = await _parametroSistemaRepository.ObterValorPorTipo(TipoParametroSistema.MunicipioAtendimento);

            if (string.IsNullOrEmpty(parametroMunicipio))
                throw new NegocioException("Não foi possível obter o parâmetro de município de atendimento");

            return new DadosDataDto()
            {
                Ano = DateTime.Now.ToString("yyyy"),
                Dia = DateTime.Now.ToString("dd"),
                Mes = DateTime.Now.ToString("MMMM"),
                Municipio = parametroMunicipio
            };
        }
    }
}
