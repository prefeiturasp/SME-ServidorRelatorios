using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Excel.Sondagem;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAnaliticoSondagemExcelQueryHandler : IRequestHandler<ObterRelatorioAnaliticoSondagemExcelQuery, IEnumerable<RelatorioSondagemAnaliticoExcelDto>>
    {
        public async Task<IEnumerable<RelatorioSondagemAnaliticoExcelDto>> Handle(ObterRelatorioAnaliticoSondagemExcelQuery request, CancellationToken cancellationToken)
        {
            var dicionario = ObterDicionarioGeradorDataTable(request.RelatorioAnalitico);

            if (dicionario.ContainsKey(request.TipoSondagem))
                return await Task.FromResult(dicionario[request.TipoSondagem].ObterTabelaExcel());

            return await Task.FromResult(Enumerable.Empty<RelatorioSondagemAnaliticoExcelDto>());
        }

        private Dictionary<TipoSondagem, GeradorDeTabelaExcelAnaliticoSondagem> ObterDicionarioGeradorDataTable(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorios)
        {
            return new Dictionary<TipoSondagem, GeradorDeTabelaExcelAnaliticoSondagem>()
            {
                { TipoSondagem.LP_Leitura, new GeradorDeTabelaExcelAnaliticoSondagemLeitura(relatorios) },
                { TipoSondagem.LP_LeituraVozAlta, new GeradorDeTabelaExcelAnaliticoSondagemLeituraDeVozAlta(relatorios) },
                { TipoSondagem.LP_CapacidadeLeitura, new GeradorDeTabelaExcelAnaliticoSondagemCapacidadeDeLeitura(relatorios) },
                { TipoSondagem.LP_ProducaoTexto, new GeradorDeTabelaExcelAnaliticoSondagemProducaoDeTexto(relatorios) },
                { TipoSondagem.LP_Escrita, new GeradorDeTabelaExcelAnaliticoSondagemEscrita(relatorios) },
                { TipoSondagem.MAT_CampoAditivo, new GeradorDeTabelaExcelAnaliticoSondagemAditivoMultiplicativo(relatorios) },
                { TipoSondagem.MAT_CampoMultiplicativo, new GeradorDeTabelaExcelAnaliticoSondagemAditivoMultiplicativo(relatorios) },
            };
        }
    }
}
