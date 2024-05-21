using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAnaliticoSondagemQueryHandler : IRequestHandler<ObterRelatorioAnaliticoSondagemQuery, IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>
    {
        private readonly ISondagemAnaliticaRepository sondagemAnaliticaRepository;
        private readonly IFabricaDeServicoAnaliticoSondagem fabricaDeServicoAnaliticoSondagem;

        public ObterRelatorioAnaliticoSondagemQueryHandler(ISondagemAnaliticaRepository sondagemAnaliticaRepository,
                                                           IFabricaDeServicoAnaliticoSondagem fabricaDeServicoAnaliticoSondagem)
        {
            this.sondagemAnaliticaRepository = sondagemAnaliticaRepository ?? throw new System.ArgumentNullException(nameof(sondagemAnaliticaRepository));
            this.fabricaDeServicoAnaliticoSondagem = fabricaDeServicoAnaliticoSondagem ?? throw new System.ArgumentNullException(nameof(fabricaDeServicoAnaliticoSondagem));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> Handle(ObterRelatorioAnaliticoSondagemQuery request, CancellationToken cancellationToken)
        {
            var servico = fabricaDeServicoAnaliticoSondagem.CriarServico(request.Filtro);

            if (servico != null)
                return await servico.ObterRelatorio(request.Filtro);

            return Enumerable.Empty<RelatorioSondagemAnaliticoPorDreDto>();
        }

        //TODO: Comentário para ser retido a cada feature
        //private Dictionary<TipoSondagem, Func<FiltroRelatorioAnaliticoSondagemDto, Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>>> ObterDicionarioAcaoObterRelatorio()
        //{
        //    return new Dictionary<TipoSondagem, Func<FiltroRelatorioAnaliticoSondagemDto, Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>>>()
        //    {
        //        { TipoSondagem.LP_Leitura, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoLeitura },
        //        { TipoSondagem.LP_LeituraVozAlta, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoLeituraDeVozAlta },
        //        { TipoSondagem.LP_Escrita, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoEscrita },
        //        { TipoSondagem.LP_ProducaoTexto, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoProducaoDeTexto },
        //        { TipoSondagem.MAT_CampoAditivo, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoCampoAditivo },
        //        { TipoSondagem.MAT_CampoMultiplicativo, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoCampoMultiplicativo },
        //        { TipoSondagem.MAT_Numeros, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoNumero },
        //        { TipoSondagem.MAT_IAD, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoIAD }
        //    };
        //}
    }
}
