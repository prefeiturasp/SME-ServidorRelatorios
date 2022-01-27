using MediatR;
using Sentry;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioUsuariosCommandHandler : IRequestHandler<ObterDadosRelatorioUsuariosCommand, DadosRelatorioUsuariosDto>
    {
        private readonly IMediator mediator;

        public ObterDadosRelatorioUsuariosCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<DadosRelatorioUsuariosDto> Handle(ObterDadosRelatorioUsuariosCommand request, CancellationToken cancellationToken)
        {
            try
            {
                SentrySdk.CaptureMessage("1.0 Obtendo ObterUsuariosAbrangenciaPorAcessoQuery - ObterDadosRelatorioUsuariosCommandHandler");
                var usuarios = await mediator.Send(new ObterUsuariosAbrangenciaPorAcessoQuery(request.FiltroRelatorio.CodigoDre,
                                                                                              request.FiltroRelatorio.CodigoUe,
                                                                                              request.FiltroRelatorio.UsuarioRf,
                                                                                              request.FiltroRelatorio.Perfis,
                                                                                              request.FiltroRelatorio.DiasSemAcesso));

                SentrySdk.CaptureMessage("1.1 ObterSituacaoUsuarios - ObterDadosRelatorioUsuariosCommandHandler");
                await ObterSituacaoUsuarios(usuarios);

                if (request.FiltroRelatorio.Situacoes.Any())
                    FiltrarSituacoesUsuario(usuarios, request.FiltroRelatorio.Situacoes);

                var dadosRelatorio = new DadosRelatorioUsuariosDto();

                SentrySdk.CaptureMessage("1.2 ObterUsuariosPorPerfil - ObterDadosRelatorioUsuariosCommandHandler");
                dadosRelatorio.PerfisSme = ObterUsuariosPorPerfil(ObterUsuariosSme(usuarios));

                SentrySdk.CaptureMessage("1.3 ObterUsuariosPorDre - ObterDadosRelatorioUsuariosCommandHandler");
                dadosRelatorio.Dres = await ObterUsuariosPorDre(usuarios, request.FiltroRelatorio.ExibirHistorico);

                return dadosRelatorio;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }            
        }

        private async Task<IEnumerable<DreUsuarioDto>> ObterUsuariosPorDre(IEnumerable<DadosUsuarioDto> usuarios, bool exibirHistorico)
        {
            var listaDresDto = new List<DreUsuarioDto>();

            foreach (var grupoDre in usuarios.GroupBy(a => (a.DreCodigo, a.Dre)))
            {
                var dreDto = new DreUsuarioDto();
                dreDto.Nome = grupoDre.Key.Dre;

                dreDto.Perfis = ObterUsuariosPorPerfil(ObterUsuariosDre(grupoDre)) ??
                    new List<PerfilUsuarioDto>();

                dreDto.Ues = await ObterUes(grupoDre);

                dreDto.HistoricoReinicioSenha = exibirHistorico ? 
                    await ObterHistoricoReinicioSenhaDre(grupoDre.Key.DreCodigo) :
                    new List<HistoricoReinicioSenhaDto>();

                listaDresDto.Add(dreDto);
            }

            return listaDresDto;
        }

        private async Task<IEnumerable<HistoricoReinicioSenhaDto>> ObterHistoricoReinicioSenhaDre(string DreCodigo)
            => await mediator.Send(new ObterHistoricoReinicioSenhaUsuarioPorDreQuery(DreCodigo));

        private async Task<IEnumerable<UePorPerfilUsuarioDto>> ObterUes(IGrouping<(string DreCodigo, string Dre), DadosUsuarioDto> usuarios)
        {
            var listaUesDto = new List<UePorPerfilUsuarioDto>();
            foreach (var grupoUe in usuarios.GroupBy(c => c.Ue))
            {
                var ueDto = new UePorPerfilUsuarioDto();

                ueDto.Nome = $"{usuarios.FirstOrDefault(c => c.Ue == grupoUe.Key).TipoEscola.ShortName()} - {grupoUe.Key}";
                var usuariosUe = ObterUsuariosUe(grupoUe);
                var usuariosPerfil = ObterUsuariosPorPerfil(usuariosUe);
                ueDto.Perfis = usuariosPerfil;

                ueDto.Professores = await ObterProfessores(FiltrarProfessores(grupoUe));

                listaUesDto.Add(ueDto);
            }

            return listaUesDto;
        }

        private async Task<IEnumerable<UsuarioProfessorDto>> ObterProfessores(IEnumerable<DadosUsuarioDto> usuarios)
        {
            var listaProfessoresDto = new List<UsuarioProfessorDto>();

            foreach(var usuario in usuarios)
            {
                var ultimaAulaRegistrada = await mediator.Send(new ObterUltimaAulaCadastradaProfessorQuery(usuario.Login));
                var ultimaFrequenciaRegistrada = await mediator.Send(new ObterUltimaFrequenciaRegistradaProfessorQuery(usuario.Login));
                var ultimoPlanoAulaCadastrado = EhPerfilInfantil(usuario.PerfilGuid) ? 
                    await mediator.Send(new ObterUltimoDiarioBordoProfessorQuery(usuario.Login)) :
                    await mediator.Send(new ObterUltimoPlanoAulaProfessorQuery(usuario.Login));

                listaProfessoresDto.Add(new UsuarioProfessorDto()
                {
                    Login = usuario.Login,
                    Nome = usuario.Nome,
                    Situacao = usuario.Situacao.Name(),
                    UltimoAcesso = usuario.UltimoAcesso == null ? "" : usuario.UltimoAcesso?.ToString("dd/MM/yyyy HH:mm"),
                    UltimaAulaRegistrada = ultimaAulaRegistrada?.ToString("dd/MM/yyyy HH:mm"),
                    UltimaFrequenciaRegistrada = ultimaFrequenciaRegistrada?.ToString("dd/MM/yyyy HH:mm"),
                    UltimoPlanoAulaRegistrado = ultimoPlanoAulaCadastrado?.ToString("dd/MM/yyyy HH:mm") 
                }); 
            }

            return listaProfessoresDto;
        }

        private bool EhPerfilInfantil(Guid perfil)
            => new[] {
                Perfis.PERFIL_CJ_INFANTIL,
                Perfis.PERFIL_PROFESSOR_INFANTIL
            }.Contains(perfil);

        private IEnumerable<DadosUsuarioDto> FiltrarProfessores(IGrouping<string, DadosUsuarioDto> usuarios)
        {
            return usuarios.Where(c => EhPerfilProfessor(c.PerfilGuid))
                        .DistinctBy(a => a.Nome);
        }

        private IEnumerable<PerfilUsuarioDto> ObterUsuariosPorPerfil(IEnumerable<DadosUsuarioDto> usuarios)
        {
            foreach (var grupoPerfil in usuarios.GroupBy(c => c.Perfil))
            {
                yield return new PerfilUsuarioDto()
                {
                    Nome = grupoPerfil.Key,
                    Usuarios = ObterUsuariosDto(grupoPerfil.DistinctBy(c => c.Login))
                };
            }
        }

        private IEnumerable<UsuarioDto> ObterUsuariosDto(IEnumerable<DadosUsuarioDto> usuarios)
        {
            foreach (var usuario in usuarios)
                yield return new UsuarioDto()
                {
                    Login = usuario.Login,
                    Nome = usuario.Nome,
                    Situacao = usuario.Situacao.Name(),
                    UltimoAcesso = usuario.UltimoAcesso == null ? "" : usuario.UltimoAcesso?.ToString("dd/MM/yyyy HH:mm")
                };
        }

        private IEnumerable<DadosUsuarioDto> ObterUsuariosDre(IGrouping<(string DreCodigo, string Dre), DadosUsuarioDto> usuariosDre)
            => usuariosDre.Where(c => new[] { TipoPerfil.DRE }.Contains(c.TipoPerfil));


        private IEnumerable<DadosUsuarioDto> ObterUsuariosSme(IEnumerable<DadosUsuarioDto> usuarios)
            => usuarios.Where(c => new[] { TipoPerfil.SME }.Contains(c.TipoPerfil)).DistinctBy(d => d.Login);

        private IEnumerable<DadosUsuarioDto> ObterUsuariosUe(IGrouping<string, DadosUsuarioDto> usuariosDre)
        {
            return usuariosDre.Where(c => c.TipoPerfil == TipoPerfil.UE && !EhPerfilProfessor(c.PerfilGuid));
        }

        private bool EhPerfilProfessor(Guid perfil)
            => new[] {
                Perfis.PERFIL_PROFESSOR,
                Perfis.PERFIL_CJ,
                Perfis.PERFIL_PROFESSOR_INFANTIL,
                Perfis.PERFIL_CJ_INFANTIL,
                Perfis.PERFIL_POA,
                Perfis.PERFIL_PAEE,
                Perfis.PERFIL_PAP,
                Perfis.PERFIL_POEI,
                Perfis.PERFIL_POED,
                Perfis.PERFIL_POSL
            }.Contains(perfil);

        private void FiltrarSituacoesUsuario(IEnumerable<DadosUsuarioDto> usuarios, int[] situacoes)
        {
            usuarios = usuarios.Where(c => situacoes.Contains((int)c.Situacao));
        }

        private async Task ObterSituacaoUsuarios(IEnumerable<DadosUsuarioDto> usuarios)
        {
            foreach (var usuariosPorLogin in usuarios.GroupBy(c => c.Login))
            {
                var situacao = await mediator.Send(new ObterSituacaoUsuarioPorRfQuery(usuariosPorLogin.Key));

                foreach (var usuario in usuariosPorLogin)
                    usuario.Situacao = situacao;

            }
        }
    }
}
