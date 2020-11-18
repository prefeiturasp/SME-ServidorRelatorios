using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioUsuariosCommandHandler : IRequestHandler<ObterDadosRelatorioUsuariosCommand, IEnumerable<DreUsuarioDto>>
    {
        private readonly IMediator mediator;

        public ObterDadosRelatorioUsuariosCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<DreUsuarioDto>> Handle(ObterDadosRelatorioUsuariosCommand request, CancellationToken cancellationToken)
        {
            var usuarios = await mediator.Send(new ObterUsuariosAbrangenciaPorAcessoQuery(request.FiltroRelatorio.CodigoDre,
                                                                                          request.FiltroRelatorio.CodigoUe,
                                                                                          request.FiltroRelatorio.UsuarioRf,
                                                                                          request.FiltroRelatorio.Perfis,
                                                                                          request.FiltroRelatorio.DiasSemAcesso));

            await ObterSituacaoUsuarios(usuarios);

            if (request.FiltroRelatorio.Situacoes.Any())
                FiltrarSituacoesUsuario(usuarios, request.FiltroRelatorio.Situacoes);

            return await ObterUsuariosPorDre(usuarios);
        }

        private async Task<IEnumerable<DreUsuarioDto>> ObterUsuariosPorDre(IEnumerable<DadosUsuarioDto> usuarios)
        {
            var listaDresDto = new List<DreUsuarioDto>();

            foreach (var grupoDre in usuarios.GroupBy(a => a.Dre))
            {
                var dreDto = new DreUsuarioDto();
                dreDto.Nome = grupoDre.Key;

                dreDto.Perfis = ObterUsuariosPorPerfil(ObterUsuariosDre(grupoDre));
                dreDto.Ues = await ObterUes(grupoDre);

                listaDresDto.Add(dreDto);
            }

            return listaDresDto;
        }

        private async Task<IEnumerable<UePorPerfilUsuarioDto>> ObterUes(IGrouping<string, DadosUsuarioDto> usuarios)
        {
            var listaUesDto = new List<UePorPerfilUsuarioDto>();
            foreach (var grupoUe in usuarios.GroupBy(c => c.Ue))
            {
                var ueDto = new UePorPerfilUsuarioDto();

                ueDto.Nome = grupoUe.Key;
                ueDto.Perfis = ObterUsuariosPorPerfil(ObterUsuariosUe(grupoUe));
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
                var ultimaAulaRegistrada = await mediator.Send(new ObterUltimaAulaCadastradaProfessorQuery(usuario.Rf));
                var ultimaFrequenciaRegistrada = await mediator.Send(new ObterUltimaFrequenciaRegistradaProfessorQuery(usuario.Rf));
                var ultimoPlanoAulaCadastrado = EhPerfilInfantil(usuario.PerfilGuid) ? 
                    await mediator.Send(new ObterUltimoDiarioBordoProfessorQuery(usuario.Rf)) :
                    await mediator.Send(new ObterUltimoPlanoAulaProfessorQuery(usuario.Rf));

                listaProfessoresDto.Add(new UsuarioProfessorDto()
                {
                    Rf = usuario.Rf,
                    Nome = usuario.Nome,
                    Situacao = usuario.Situacao.Name(),
                    UltimoAcesso = usuario.UltimoAcesso.ToString("dd/MM/yyyy HH:mm"),
                    UltimaAulaRegistrada = ultimaAulaRegistrada.ToString("dd/MM/yyyy HH:mm"),
                    UltimaFrequenciaRegistrada = ultimaFrequenciaRegistrada.ToString("dd/MM/yyyy HH:mm"),
                    UltimoPlanoAulaRegistrado = ultimoPlanoAulaCadastrado.ToString("dd/MM/yyyy HH:mm")
                }); ;
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
            return usuarios.Where(c => EhPerfilProfessor(c.PerfilGuid));
        }

        private IEnumerable<PerfilUsuarioDto> ObterUsuariosPorPerfil(IEnumerable<DadosUsuarioDto> usuarios)
        {
            foreach (var grupoPerfil in usuarios.GroupBy(c => c.Perfil))
            {
                yield return new PerfilUsuarioDto()
                {
                    Nome = grupoPerfil.Key,
                    Usuarios = ObterUsuariosDto(grupoPerfil)
                };
            }
        }

        private IEnumerable<UsuarioDto> ObterUsuariosDto(IGrouping<string, DadosUsuarioDto> usuarios)
        {
            foreach (var usuario in usuarios)
                yield return new UsuarioDto()
                {
                    Rf = usuario.Rf,
                    Nome = usuario.Nome,
                    Situacao = usuario.Situacao.Name(),
                    UltimoAcesso = usuario.UltimoAcesso.ToString("dd/MM/yyyy HH:mm")
                };
        }

        private IEnumerable<DadosUsuarioDto> ObterUsuariosDre(IGrouping<string, DadosUsuarioDto> usuariosDre)
        {
            return usuariosDre.Where(c => new[] { TipoPerfil.SME, TipoPerfil.DRE }.Contains(c.TipoPerfil));
        }

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
            foreach (var usuario in usuarios)
                usuario.Situacao = await mediator.Send(new ObterSituacaoUsuarioPorRfQuery(usuario.Rf));
        }
    }
}
