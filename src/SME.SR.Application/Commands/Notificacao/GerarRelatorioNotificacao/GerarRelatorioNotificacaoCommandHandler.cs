using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var filtro = request.Filtros;

            var relatorioNotificacoes = new RelatorioNotificacoesDto();

            var situacoes = filtro.Situacoes.ToArray();
            var categorias = filtro.Categorias.ToArray();
            var tipos = filtro.Tipos.ToArray();

            if (filtro.Situacoes.Contains(-99))
            {
                situacoes = new long[] {
                        (long)NotificacaoStatus.Aceita,
                        (long)NotificacaoStatus.Lida,
                        (long)NotificacaoStatus.Pendente,
                        (long)NotificacaoStatus.Reprovada
                    };
            }
           

            if (filtro.Categorias.Contains(-99))
            {
                categorias = new long[] {
                        (long)NotificacaoCategoria.Alerta,
                        (long)NotificacaoCategoria.Aviso,
                        (long)NotificacaoCategoria.Workflow_Aprovacao
                    };
            }
            

            if (filtro.Tipos.Contains(-99))
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
            

            var notificacoes = await mediator.Send(new ObterNotificacoesFiltrosQuery(filtro.AnoLetivo, filtro.UsuarioBuscaRf,
                categorias, tipos, situacoes, filtro.ExibirDescricao, filtro.ExibirNotificacoesExcluidas, filtro.DRE, filtro.UE, filtro.Turma));

            if (notificacoes == null || !notificacoes.Any())
                throw new NegocioException("<b>O relatório com o filtro solicitado não possui informações.</b>");

            relatorioNotificacoes.Cabecalho = await MontarCabecalhoRelatorio(filtro);

            var relatorioNotificacao = await MapearNotificacoesDto(notificacoes);

            

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotificacoes", relatorioNotificacao, request.CodigoCorrelacao, "", "Relatório de Notificações", true)));
        }

        private async Task<RelatorioNotificacoesDto> MapearNotificacoesDto(IEnumerable<NotificacaoRetornoDto> notificacoes)
        {
            var relatorioNotificacaoDto = new RelatorioNotificacaoDto();
            var dresId = notificacoes.Select(n => n.DreId).Distinct();

            foreach (var dreId in dresId)
            {
                var dreNotificacaoDto = new DreNotificacaoDto();
                dreNotificacaoDto.Nome = notificacoes.FirstOrDefault(c => c.DreId == dreId).DreNome;

                var uesId = notificacoes.Where(c => c.DreId == dreId).Select(c => c.UeId).Distinct();

                foreach (var ueId in uesId)
                {
                    var ueNotificacaoDto = new UeNotificacaoDto()
                    {
                        Nome = notificacoes.FirstOrDefault(c => c.UeId == ueId).UeNome,
                    };

                    var usuarios = notificacoes.Where(c => c.UeId == ueId && c.DreId == dreId).Select(c => c.UsuarioRf).Distinct();

                    foreach (var usuario in usuarios)
                    {
                        var usuarioNotificacaoDto = new UsuarioNotificacaoDto();
                        var nome = notificacoes.FirstOrDefault(c => c.UsuarioRf == usuario).UsuarioNome;
                        if (string.IsNullOrEmpty(nome))
                        {
                            var usuarioCoreSSO = await mediator.Send(new ObterDadosUsuarioCoreSSOPorRfQuery(usuario));
                            if (usuarioCoreSSO != null)
                                nome = usuarioCoreSSO.Nome;
                        }
                        usuarioNotificacaoDto.Nome = nome != null ? $"{nome} ({usuario})" : usuario;
                        usuarioNotificacaoDto.Notificacoes.AddRange(notificacoes.Where(c => c.UeId == ueId && c.DreId == dreId && c.UsuarioRf == usuario).OrderBy(n => n.Codigo));
                        ueNotificacaoDto.Usuarios.Add(usuarioNotificacaoDto);
                    }

                    dreNotificacaoDto.UEs.Add(ueNotificacaoDto);
                }

                relatorioNotificacaoDto.DREs.Add(dreNotificacaoDto);
            }

            return relatorioNotificacaoDto;
        }

        private async Task<RelatorioNotificacoesCabecalhoDto> MontarCabecalhoRelatorio(FiltroNotificacaoDto filtros)
        {
            var dre = await mediator.Send(new ObterDrePorCodigoQuery(filtros.DRE));
            var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UE));
                      
            return new RelatorioNotificacoesCabecalhoDto
            {
                Dre = dre == null ? "" : dre.Abreviacao,
                Ue = ue == null ? "" : ue.NomeRelatorio,
                Usuario = filtros.UsuarioNome,
                RF = filtros.UsuarioRf
            };
        }
    }
}
