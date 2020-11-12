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


            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotificacoes", dto, request.CodigoCorrelacao, "", "Relatório de Notificações", true, "RELATÓRIO DE NOTIFICAÇÕES")));
        }

        private async Task MontarCabecalhoRelatorioDto(RelatorioNotificacaoDto dto, FiltroNotificacaoDto filtros)
        {
            var nomeDre = "TODAS";
            var nomeUe = "TODAS";
            if (filtros.DREs.Count() == 1)
            {
                var dre = await mediator.Send(new ObterDrePorIdQuery(filtros.DREs.FirstOrDefault()));
                nomeDre = dre.Abreviacao;
            }

            if (filtros.UEs.Count() == 1)
            {
                var ue = await mediator.Send(new ObterUePorIdQuery(filtros.UEs.FirstOrDefault()));
                nomeUe = $"{ue.Codigo} - {ue.TipoEscola.ShortName()} {ue.Nome}";
            }

            dto.CabecalhoDRE = nomeDre;
            dto.CabecalhoUE = nomeUe;
            dto.CabecalhoUsuario = filtros.Professor;
            dto.CabecalhoUsuarioRF = filtros.RF;
        }
    }
}
