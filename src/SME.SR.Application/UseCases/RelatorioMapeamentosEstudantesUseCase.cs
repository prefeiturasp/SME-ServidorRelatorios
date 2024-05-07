using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioMapeamentosEstudantesUseCase : IRelatorioMapeamentosEstudantesUseCase
    {
        private readonly IMediator mediator;

        public RelatorioMapeamentosEstudantesUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioMapeamentoEstudantesDto>();
            var mapeamentos = await mediator.Send(new ObterMapeamentosEstudantesPorFiltroQuery(filtroRelatorio));

            if (mapeamentos == null || !mapeamentos.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

                    
            var relatorio = new RelatorioEncaminhamentosNAAPADto()
            {
                DreNome = !string.IsNullOrEmpty(filtroRelatorio.DreCodigo) && filtroRelatorio.DreCodigo.Equals("-99") || string.IsNullOrEmpty(filtroRelatorio.DreCodigo) ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().DreNome,
                UeNome = !string.IsNullOrEmpty(filtroRelatorio.UeCodigo) && filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().UeNome,
                UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
            };

            relatorio.EncaminhamentosDreUe = encaminhamentosAgrupados;
            await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad(relatorio, request.CodigoCorrelacao));
        }
    }
}
