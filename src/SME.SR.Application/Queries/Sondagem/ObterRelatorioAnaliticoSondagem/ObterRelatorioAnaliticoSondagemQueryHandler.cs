using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SME.SR.Application.Interfaces;
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

        private readonly IServiceScopeFactory servicos;

        public ObterRelatorioAnaliticoSondagemQueryHandler(IServiceScopeFactory servicos)
        {
            this.servicos = servicos ?? throw new System.ArgumentNullException(nameof(servicos));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> Handle(ObterRelatorioAnaliticoSondagemQuery request, CancellationToken cancellationToken)
        {
            var servico = ObterServico(request.Filtro.TipoSondagem);

            if (servico != null)
                return await servico.ObterRelatorio(request.Filtro);

            return Enumerable.Empty<RelatorioSondagemAnaliticoPorDreDto>();
        }

        private IServicoRepositorioAnalitico ObterServico(TipoSondagem tipoSondagem)
        {
            var dicionarioServico = ObterDicionarioTipoServico();

            if (dicionarioServico.ContainsKey(tipoSondagem))
                return ObterInstancia(dicionarioServico[tipoSondagem]);

            return null;
        }

        private IServicoRepositorioAnalitico ObterInstancia(Type tipo)
        {
            using var scope = servicos.CreateScope();
            return (IServicoRepositorioAnalitico)scope.ServiceProvider.GetService(tipo);
        }

        private Dictionary<TipoSondagem, Type> ObterDicionarioTipoServico()
        {
            return new Dictionary<TipoSondagem, Type>()
            {
                { TipoSondagem.LP_CapacidadeLeitura, typeof(IServicoAnaliticoSondagemCapacidadeDeLeitura) },
                { TipoSondagem.LP_Leitura, typeof(IServicoAnaliticoSondagemLeitura) },
                { TipoSondagem.LP_LeituraVozAlta, typeof(IServicoAnaliticoSondagemLeituraVozAlta) },
                { TipoSondagem.LP_Escrita, typeof(IServicoAnaliticoSondagemEscrita) },
                { TipoSondagem.LP_ProducaoTexto, typeof(IServicoAnaliticoSondagemProducaoDeTexto) },
                { TipoSondagem.MAT_CampoAditivo, typeof(IServicoAnaliticoSondagemCampoAditivo) },
                { TipoSondagem.MAT_CampoMultiplicativo, typeof(IServicoAnaliticoSondagemCampoMultiplicativo) },
                //{ TipoSondagem.MAT_Numeros, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoNumero },
                //{ TipoSondagem.MAT_IAD, sondagemAnaliticaRepository.ObterRelatorioSondagemAnaliticoIAD }
            };
        }
    }
}
