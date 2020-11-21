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

            long[] situacoes = new long[] { };
            long[] categorias = new long[] { };
            long[] tipos = new long[] { };

            if (request.Filtros.Situacoes.Contains(-99))
            {
                situacoes = new long[] {
                        (long)NotificacaoStatus.Aceita,
                        (long)NotificacaoStatus.Lida,
                        (long)NotificacaoStatus.Pendente,
                        (long)NotificacaoStatus.Reprovada
                    };
            } else
            {
                situacoes = request.Filtros.Situacoes.ToArray();
            }

            if (request.Filtros.Categorias.Contains(-99))
            {
                categorias = new long[] {
                        (long)NotificacaoCategoria.Alerta,
                        (long)NotificacaoCategoria.Aviso,
                        (long)NotificacaoCategoria.Workflow_Aprovacao
                    };
            }
            else
            {
                categorias = request.Filtros.Categorias.ToArray();
            }

            if (request.Filtros.Tipos.Contains(-99))
            {
                tipos = new long[] {
                        (long)NotificacaoTipo.Calendario,
                        (long)NotificacaoTipo.Fechamento,
                        (long)NotificacaoTipo.Frequencia,
                        (long)NotificacaoTipo.Notas,
                        (long)NotificacaoTipo.Sondagem,
                        (long)NotificacaoTipo.PlanoDeAula,
                        (long)NotificacaoTipo.Relatorio,
                        (long)NotificacaoTipo.Worker,
                        (long)NotificacaoTipo.Planejamento
                    };
            }
            else
            {
                tipos = request.Filtros.Tipos.ToArray();
            }

            var notificacoes = await mediator.Send(new ObterNotificacoesFiltrosQuery(request.Filtros.AnoLetivo, request.Filtros.UsuarioBuscaRf,
                categorias, tipos, situacoes, request.Filtros.ExibirDescricao, request.Filtros.ExibirNotificacoesExcluidas, request.Filtros.DRE, request.Filtros.UE));

            if (!notificacoes.Any())
                throw new NegocioException("<b>O relatório com o filtro solicitado não possui informações.</b>");

            dto = MapearNotificacoesDto(notificacoes);

            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotificacoes", dto, request.CodigoCorrelacao, "", "Relatório de Notificações", true, "RELATÓRIO DE NOTIFICAÇÕES")));
        }

        private RelatorioNotificacaoDto MapearNotificacoesDto(IEnumerable<NotificacaoDto> notificacoes)
        {
            RelatorioNotificacaoDto relatorioNotificacaoDto = new RelatorioNotificacaoDto();
            var dres = notificacoes.Select(n => n.DreId).Distinct();

            foreach(var dre in dres)
            {
                DreNotificacaoDto dreNotificacaoDto = new DreNotificacaoDto();
                dreNotificacaoDto.Nome = notificacoes.FirstOrDefault(c => c.DreId == dre).DreNome;

                var ues = notificacoes.Where(c => c.DreId == dre).Select(c => c.UeId).Distinct();

                foreach(var ue in ues)
                {
                    UeNotificacaoDto ueNotificacaoDto = new UeNotificacaoDto();

                    ueNotificacaoDto.Nome = notificacoes.FirstOrDefault(c => c.UeId == ue).UeNome;

                    var usuarios = notificacoes.Where(c => c.UeId == ue && c.DreId == dre).Select(c => c.UsuarioRf).Distinct();

                    foreach(var usuario in usuarios)
                    {
                        UsuarioNotificacaoDto usuarioNotificacaoDto = new UsuarioNotificacaoDto();
                        var nome = notificacoes.FirstOrDefault(c => c.UsuarioRf == usuario).UsuarioNome;
                        usuarioNotificacaoDto.Nome = nome != null ? $"{nome} ({usuario})"  : usuario;
                        usuarioNotificacaoDto.Notificacoes.AddRange(notificacoes.Where(c => c.UeId == ue && c.DreId == dre && c.UsuarioRf == usuario).OrderBy(n => n.Codigo));
                        ueNotificacaoDto.Usuarios.Add(usuarioNotificacaoDto);
                    }

                    dreNotificacaoDto.UEs.Add(ueNotificacaoDto);
                }

                relatorioNotificacaoDto.DREs.Add(dreNotificacaoDto);
            }
            
            return relatorioNotificacaoDto;
        }

        private async Task MontarCabecalhoRelatorioDto(RelatorioNotificacaoDto dto, FiltroNotificacaoDto filtros)
        {
            var nomeDre = "TODAS";
            var nomeUe = "TODAS";
            if (filtros.DRE != "-99")
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery(filtros.DRE));
                nomeDre = dre.Abreviacao;
            }

            if (filtros.UE != "-99")
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UE));
                nomeUe = $"{ue.TipoEscola.ShortName()} {ue.Nome}";
            }

            dto.CabecalhoDRE = nomeDre;
            dto.CabecalhoUE = nomeUe;
            dto.CabecalhoUsuario = filtros.UsuarioNome;
            dto.CabecalhoUsuarioRF = filtros.UsuarioRf;
        }
    }
}
