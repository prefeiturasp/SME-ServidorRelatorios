using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioNotificacaoCommandHandler : IRequestHandler<GerarRelatorioNotificacaoCommand, bool>
    {
        private readonly IMediator mediator;

        public GerarRelatorioNotificacaoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarRelatorioNotificacaoCommand request, CancellationToken cancellationToken)
        {
            var dto = new RelatorioNotificacaoDto();

            var notificacoes = await mediator.Send(new ObterNotificacoesPorAnoEUsuarioRfQuery(request.Filtros.AnoLetivo, request.Filtros.UsuarioBuscaRf));

            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotificacoes", dto, request.CodigoCorrelacao, "", "Relatório de Notificações", true, "RELATÓRIO DE NOTIFICAÇÕES")));
        }

        private async Task MontarCabecalhoRelatorioDto(RelatorioNotificacaoDto dto, FiltroNotificacaoDto filtros)
        {
            var nomeDre = "TODAS";
            var nomeUe = "TODAS";
            if (filtros.DRE != -99)
            {
                var dre = await mediator.Send(new ObterDrePorIdQuery(filtros.DRE));
                nomeDre = dre.Abreviacao;
            }

            if (filtros.UE != -99)
            {
                var ue = await mediator.Send(new ObterUePorIdQuery(filtros.UE));
                nomeUe = $"{ue.Codigo} - {ue.TipoEscola.ShortName()} {ue.Nome}";
            }

            dto.CabecalhoDRE = nomeDre;
            dto.CabecalhoUE = nomeUe;
            dto.CabecalhoUsuario = filtros.UsuarioNome;
            dto.CabecalhoUsuarioRF = filtros.UsuarioRf;
        }
    }
}
