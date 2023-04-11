using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAnaliticoSondagemQueryHandler : IRequestHandler<ObterRelatorioAnaliticoSondagemQuery, IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>
    {
        private readonly ISondagemAnaliticaRepository sondagemAnaliticaRepository;

        public ObterRelatorioAnaliticoSondagemQueryHandler(ISondagemAnaliticaRepository sondagemAnaliticaRepository)
        {
            this.sondagemAnaliticaRepository = sondagemAnaliticaRepository ?? throw new System.ArgumentNullException(nameof(sondagemAnaliticaRepository));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> Handle(ObterRelatorioAnaliticoSondagemQuery request, CancellationToken cancellationToken)
        {
            var dicionario = ObterDicionarioAcaoObterRelatorio();
    
            if (dicionario.ContainsKey(request.Filtro.TipoSondagem))
                return await dicionario[request.Filtro.TipoSondagem](request.Filtro);

            return Enumerable.Empty<RelatorioSondagemAnaliticoPorDreDto>();
        }

        private Dictionary<TipoSondagem, Func<FiltroRelatorioAnaliticoSondagemDto, Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>>> ObterDicionarioAcaoObterRelatorio()
        {
            return new Dictionary<TipoSondagem, Func<FiltroRelatorioAnaliticoSondagemDto, Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>>>()
            {
                { TipoSondagem.LP_CapacidadeLeitura, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoCapacidadeDeLeitura },
                { TipoSondagem.LP_Leitura, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoLeitura },
                { TipoSondagem.LP_LeituraVozAlta, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoLeituraDeVozAlta },
                { TipoSondagem.LP_Escrita, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoEscrita },
                { TipoSondagem.LP_ProducaoTexto, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoProducaoDeTexto },
                { TipoSondagem.MAT_CampoAditivo, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoCampoAditivo },
                { TipoSondagem.MAT_CampoMultiplicativo, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoCampoMultiplicativo },
            };
        }
    }
}
