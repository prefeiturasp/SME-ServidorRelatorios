using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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

            var notificacoes = await mediator.Send(new ObterNotificacoesFiltrosQuery(filtro.AnoLetivo, filtro.UsuarioBuscaRf,
                filtro.Categorias, filtro.Tipos, filtro.Situacoes, filtro.ExibirDescricao, filtro.ExibirNotificacoesExcluidas, filtro.DRE, filtro.UE, filtro.Turma));

            if (notificacoes == null || !notificacoes.Any())
                throw new NegocioException("<b>O relatório com o filtro solicitado não possui informações.</b>");

            var relatorioNotificacoes = new RelatorioNotificacoesDto()
            {
                Cabecalho = await MontarCabecalhoRelatorio(filtro),
                Usuarios = await MapearParaUsuarios(notificacoes),
            };           

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotificacoes", relatorioNotificacoes, request.CodigoCorrelacao, "", "Relatório de Notificações", true)));
        }

        private async Task<IEnumerable<RelatorioNotificacoesUsuarioDto>> MapearParaUsuarios(IEnumerable<NotificacaoRetornoDto> notificacoes)
        {
            var usuarios = new List<RelatorioNotificacoesUsuarioDto>();            

            foreach(var usuarioNotificacao in notificacoes.GroupBy(c => c.UsuarioRf).Distinct())
            {
                var usuario = new RelatorioNotificacoesUsuarioDto();
                var nome = usuarioNotificacao.FirstOrDefault(c => c.UsuarioRf == usuarioNotificacao.Key).UsuarioNome;

                if (string.IsNullOrEmpty(nome))
                {                    
                    var usuarioCoreSSO = await mediator.Send(new ObterDadosUsuarioCoreSSOPorRfQuery(usuarioNotificacao.Key));
                    if (usuarioCoreSSO != null)
                        nome = usuarioCoreSSO.Nome;
                }

                usuario.Nome = $"{nome} ({usuarioNotificacao.Key})";
                usuario.Notificacoes = MontarNotificacoes(usuarioNotificacao);                              

                usuarios.Add(usuario);
            }           

            return usuarios.OrderBy(u => u.Nome);
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

        private IEnumerable<NotificacaoRelatorioDto> MontarNotificacoes(IEnumerable<NotificacaoRetornoDto> notificacoes)
        {
            foreach(var notificacao in notificacoes.OrderByDescending(n => n.DataRecebimento))
            {
                yield return new NotificacaoRelatorioDto()
                {
                    Codigo = notificacao.Codigo,
                    Titulo = notificacao.Titulo,
                    Categoria = notificacao.Categoria.Name(),
                    Tipo = notificacao.Tipo.Name(),
                    Situacao = notificacao.Situacao.Name(),
                    DataRecebimento = notificacao.DataRecebimento.ToString("dd/MM/yyyy HH:mm:ss"),
                    DataLeitura = notificacao.DataLeitura == null ? "" : notificacao.DataLeitura?.ToString("dd/MM/yyyy HH:mm:ss"),
                };
            }            
        }
    }
}
