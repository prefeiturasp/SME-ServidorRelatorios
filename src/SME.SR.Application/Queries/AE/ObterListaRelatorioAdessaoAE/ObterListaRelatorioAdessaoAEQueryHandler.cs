using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.AE.Adesao;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterListaRelatorioAdessaoAEQueryHandler : IRequestHandler<ObterListaRelatorioAdessaoAEQuery, AdesaoAERetornoDto>
    {
        private readonly IMediator mediator;

        public ObterListaRelatorioAdessaoAEQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<AdesaoAERetornoDto> Handle(ObterListaRelatorioAdessaoAEQuery request, CancellationToken cancellationToken)
        {
            var retorno = new AdesaoAERetornoDto();

            if (!string.IsNullOrEmpty(request.RelatorioFiltros.DreCodigo))
            {
                if (!string.IsNullOrEmpty(request.RelatorioFiltros.UeCodigo))
                {
                    await TrataUe(request, retorno);
                }
                else
                {
                    var dreParaAdicionar = await TrataDre(request.RelatorioFiltros.DreCodigo, request.ListaConsolida);
                    retorno.MostraDRE = true;
                    retorno.DRE = dreParaAdicionar;
                }
            }
            else
            {
                await TrataSME(request, retorno);
            }

            await MontarCabecalho(request.RelatorioFiltros, retorno);

            return retorno;
        }

        private async Task MontarCabecalho(AdesaoAEFiltroDto relatorioFiltros, AdesaoAERetornoDto retorno)
        {
            if (!string.IsNullOrEmpty(relatorioFiltros.UeCodigo))
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(relatorioFiltros.UeCodigo));
                retorno.UeNome = $"{ue.Codigo} - {ue.NomeComTipoEscola}";
            }
            else retorno.UeNome = "Todas";

            if (!string.IsNullOrEmpty(relatorioFiltros.DreCodigo))
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery(relatorioFiltros.DreCodigo));
                retorno.DRENome = $"{dre.Abreviacao}";
            }
            else retorno.DRENome = "Todas";

            retorno.UsuarioNome = relatorioFiltros.UsuarioNome;
            retorno.UsuarioRF = relatorioFiltros.UsuarioRF;
            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");

        }

        private async Task TrataSME(ObterListaRelatorioAdessaoAEQuery request, AdesaoAERetornoDto retorno)
        {
            retorno.MostraSME = true;

            var registroSme = request.ListaConsolida.FirstOrDefault(a => string.IsNullOrEmpty(a.DreCodigo.Trim()));
            if (registroSme == null)
                throw new NegocioException("Não foi possível obter o registro consolidado da SME.");

            var valoresSme = new AdesaoAEValoresDto()
            {
                NaoRealizaram = registroSme.SemAppInstalado,
                Nome = "SME",
                PrimeiroAcessoIncompleto = registroSme.PrimeiroAcessoIncompleto,
                SemCpfOuCpfInvalido = registroSme.CpfsInvalidos,
                Validos = registroSme.Validos
            };

            var registroSMEParaInserir = new AdesaoAESmeDto() { Valores = valoresSme };

            retorno.SME = registroSMEParaInserir;

            var dresCodigosParaTratar = request.ListaConsolida
                .Where(a => string.IsNullOrEmpty(a.UeCodigo.Trim()) && !string.IsNullOrEmpty(a.DreCodigo.Trim()))
                .Select(a => a.DreCodigo)
                .Distinct();

            foreach (var dreCodigosParaTratar in dresCodigosParaTratar)
            {
                var dreParaAdicionar = await TrataDre(dreCodigosParaTratar, request.ListaConsolida.Where(a => a.DreCodigo == dreCodigosParaTratar).ToList());
                retorno.SME.Dres.Add(dreParaAdicionar);

            }
        }

        private async Task<AdesaoAEDreDto> TrataDre(string dreCodigoParaTratar, IEnumerable<AdesaoAEQueryConsolidadoRetornoDto> listaConsolida)
        {


            var uesCodigos = listaConsolida.Where(a => !string.IsNullOrEmpty(a.UeCodigo.Trim()))
                .Select(a => a.UeCodigo.ToString()).Distinct().ToArray();


            var Ues = await mediator.Send(new ObterUePorCodigosQuery(uesCodigos));

            var registroDreParaTratar = listaConsolida.FirstOrDefault(a => a.DreCodigo == dreCodigoParaTratar && string.IsNullOrEmpty(a.UeCodigo.Trim()));
            if (registroDreParaTratar == null)
                throw new NegocioException($"Não foi possível obter o registro consolidado da Dre {dreCodigoParaTratar}.");


            var dreValoresParaAdicionar = new AdesaoAEValoresDto()
            {
                Nome = registroDreParaTratar.DreNome,
                NaoRealizaram = registroDreParaTratar.SemAppInstalado,
                PrimeiroAcessoIncompleto = registroDreParaTratar.PrimeiroAcessoIncompleto,
                SemCpfOuCpfInvalido = registroDreParaTratar.CpfsInvalidos,
                Validos = registroDreParaTratar.Validos
            };

            var registroDre = new AdesaoAEDreDto() { Valores = dreValoresParaAdicionar };
            //Tratar as Ues \\ 

            foreach (var ueParaTratar in listaConsolida.Where(a => !string.IsNullOrEmpty(a.UeCodigo) && a.TurmaCodigo == 0))
            {
                var ue = Ues.FirstOrDefault(a => a.Codigo == ueParaTratar.UeCodigo);

                if (ue != null)
                {
                    var ueParaAdicionar = new AdesaoAEValoresDto()
                    {
                        Nome = ue.NomeComTipoEscola,
                        NaoRealizaram = ueParaTratar.SemAppInstalado,
                        PrimeiroAcessoIncompleto = ueParaTratar.PrimeiroAcessoIncompleto,
                        SemCpfOuCpfInvalido = ueParaTratar.CpfsInvalidos,
                        Validos = ueParaTratar.Validos
                    };

                    registroDre.Ues.Add(ueParaAdicionar);
                }

            }

            registroDre.Ues = registroDre
                .Ues
                .OrderBy(a => a.Nome).ToList();
            return registroDre;
        }

        private async Task TrataUe(ObterListaRelatorioAdessaoAEQuery request, AdesaoAERetornoDto retorno)
        {
            retorno.MostraUe = true;

            var registroUeParaTratar = request.ListaConsolida.FirstOrDefault(a => a.TurmaCodigo == 0 && !string.IsNullOrEmpty(a.UeCodigo));

            var nomeDaUe = await mediator.Send(new ObterUePorCodigoQuery(request.RelatorioFiltros.UeCodigo));

            var valoresDaUe = new AdesaoAEValoresDto()
            {
                NaoRealizaram = registroUeParaTratar.SemAppInstalado,
                SemCpfOuCpfInvalido = registroUeParaTratar.CpfsInvalidos,
                PrimeiroAcessoIncompleto = registroUeParaTratar.PrimeiroAcessoIncompleto,
                Validos = registroUeParaTratar.Validos,
                Nome = nomeDaUe?.NomeComTipoEscola
            };

            var UeParaAdicionar = new AdesaoAeUeRetornoDto()
            {
                Valores = valoresDaUe
            };

            var turmasCodigos = request.ListaConsolida.Select(a => a.TurmaCodigo.ToString()).Distinct().ToArray();

            var turmasEModalidades = await mediator.Send(new ObterTurmasEModalidadesPorCodigoTurmasQuery(turmasCodigos));

            var turmasAgrupadasPorModalidade = turmasEModalidades
                .GroupBy(a => a.Modalidade)
                .ToList();

            var devePreencherModalidade = turmasAgrupadasPorModalidade.Count > 1;

            foreach (var turmaAgrupadasPorModalidade in turmasAgrupadasPorModalidade)
            {
                var valoresDaMolidade = new AdesaoAEValoresDto();

                var codigosTurmasDaModalidade = turmaAgrupadasPorModalidade.Select(a => long.Parse(a.Codigo)).Distinct();


                var turmasEValoresDaModalidade = request.ListaConsolida
                    .Where(a => codigosTurmasDaModalidade.Contains(a.TurmaCodigo))
                    .ToList();

                if (devePreencherModalidade)
                {
                    valoresDaMolidade.NaoRealizaram = turmasEValoresDaModalidade.Sum(a => a.SemAppInstalado);
                    valoresDaMolidade.Nome = turmaAgrupadasPorModalidade.Key.Name();
                    valoresDaMolidade.PrimeiroAcessoIncompleto = turmasEValoresDaModalidade.Sum(a => a.PrimeiroAcessoIncompleto);
                    valoresDaMolidade.SemCpfOuCpfInvalido = turmasEValoresDaModalidade.Sum(a => a.CpfsInvalidos);
                    valoresDaMolidade.Validos = turmasEValoresDaModalidade.Sum(a => a.Validos);
                }

                var modalidadeParaAdicionar = new AdesaoAEModalidadeDto() { Valores = valoresDaMolidade };

                var alunosResponsaveisParaTratar = await mediator.Send(new ObterAlunosResponsaveisPorTurmasCodigoRelatorioAdesaoQuery(codigosTurmasDaModalidade.ToArray()));

                var cpfsDosResponsaveis = alunosResponsaveisParaTratar.Select(a => a.ResponsavelCpf).Distinct().ToArray();

                var usuariosDoApp = await mediator.Send(new ObterUsuariosAePorCpfsQuery(cpfsDosResponsaveis));

                foreach (var turma in turmaAgrupadasPorModalidade)
                {

                    var turmaParaTratar = turmasEValoresDaModalidade.FirstOrDefault(a => a.TurmaCodigo == long.Parse(turma.Codigo));
                    var valoresDaTurmaParaTratar = new AdesaoAEValoresDto();

                    valoresDaTurmaParaTratar.NaoRealizaram = turmaParaTratar.SemAppInstalado;
                    valoresDaTurmaParaTratar.Nome = $"{turmaAgrupadasPorModalidade.Key.ShortName()}-{turma.Nome}";
                    valoresDaTurmaParaTratar.PrimeiroAcessoIncompleto = turmaParaTratar.PrimeiroAcessoIncompleto;
                    valoresDaTurmaParaTratar.SemCpfOuCpfInvalido = turmaParaTratar.CpfsInvalidos;
                    valoresDaTurmaParaTratar.Validos = turmaParaTratar.Validos;

                    var turmaParaAdicionar = new AdesaoAETurmaDto() { Valores = valoresDaTurmaParaTratar };

                    switch (request.RelatorioFiltros.OpcaoListaUsuarios)
                    {
                        case FiltroRelatorioAEAdesaoEnum.ListarUsuariosNao:
                            UeParaAdicionar.MostraColunaSituacao = false;
                            break;
                        case FiltroRelatorioAEAdesaoEnum.ListarUsuariosValidos:
                            UeParaAdicionar.MostraColunaSituacao = false;
                            TrataListarCpfValidos(alunosResponsaveisParaTratar, usuariosDoApp, turma, turmaParaAdicionar);
                            break;
                        case FiltroRelatorioAEAdesaoEnum.ListarUsuariosCPFIrregular:
                            TrataListarCpfIrregular(alunosResponsaveisParaTratar, usuariosDoApp, turma, turmaParaAdicionar);
                            UeParaAdicionar.MostraColunaSituacao = false;
                            break;
                        case FiltroRelatorioAEAdesaoEnum.ListarUsuariosCPFTodos:
                            UeParaAdicionar.MostraColunaSituacao = true;
                            TrataListarTodosCpf(alunosResponsaveisParaTratar, usuariosDoApp, turma, turmaParaAdicionar);
                            break;
                        default:
                            break;
                    }
                    modalidadeParaAdicionar.Turmas.Add(turmaParaAdicionar);

                }
                modalidadeParaAdicionar.Turmas = modalidadeParaAdicionar.Turmas.OrderBy(a => a.Valores.Nome).ToList();

                UeParaAdicionar.Modalidades.Add(modalidadeParaAdicionar);
            }

            UeParaAdicionar.Modalidades = UeParaAdicionar.Modalidades.OrderBy(a => a.Valores.Nome).ToList();


            retorno.UE = UeParaAdicionar;
        }

        private void TrataListarCpfIrregular(IEnumerable<AlunoResponsavelAdesaoAEDto> alunosResponsaveisParaTratar, IEnumerable<UsuarioAEDto> usuariosDoApp, TurmaResumoDto turma, AdesaoAETurmaDto turmaParaAdicionar)
        {
            var alunosResponsaveisDaTurma = alunosResponsaveisParaTratar.Where(a => a.TurmaCodigo == long.Parse(turma.Codigo) &&
            !UtilCPF.Valida(a.ResponsavelCpf)).OrderBy(a => a.NomeAlunoParaVisualizar());

            foreach (var alunoResponsaveisDaTurma in alunosResponsaveisDaTurma)
            {

                var alunoResponsavelParaAdicionar = new AdesaoAEUeAlunoDto()
                {
                    Contato = alunoResponsaveisDaTurma.ResponsavelCelularFormatado(),
                    CpfResponsavel = alunoResponsaveisDaTurma.ResponsavelCpf?.ToString(),
                    Responsavel = alunoResponsaveisDaTurma.ResponsavelNome?.Trim(),
                    Estudante = alunoResponsaveisDaTurma.NomeAlunoParaVisualizar(),
                    Numero = alunoResponsaveisDaTurma.AlunoNumeroChamada
                };
                turmaParaAdicionar.Alunos.Add(alunoResponsavelParaAdicionar);

            }
        }

        private void TrataListarCpfValidos(IEnumerable<AlunoResponsavelAdesaoAEDto> alunosResponsaveisParaTratar, IEnumerable<UsuarioAEDto> usuariosDoApp, TurmaResumoDto turma, AdesaoAETurmaDto turmaParaAdicionar)
        {
            var alunosResponsaveisDaTurma = alunosResponsaveisParaTratar.Where(a => a.TurmaCodigo == long.Parse(turma.Codigo)).OrderBy(a => a.NomeAlunoParaVisualizar());


            foreach (var alunoResponsaveisDaTurma in alunosResponsaveisDaTurma)
            {
                var usuarioApp = usuariosDoApp.FirstOrDefault(a => a.Cpf == alunoResponsaveisDaTurma.ResponsavelCpf && !a.PrimeiroAcesso && !a.Excluido);
                if (usuarioApp != null)
                {
                    var alunoResponsavelParaAdicionar = new AdesaoAEUeAlunoDto()
                    {
                        Contato = alunoResponsaveisDaTurma.ResponsavelCelularFormatado(),
                        CpfResponsavel = alunoResponsaveisDaTurma.ResponsavelCpf.ToString(),
                        Responsavel = alunoResponsaveisDaTurma.ResponsavelNome,
                        Estudante = alunoResponsaveisDaTurma.NomeAlunoParaVisualizar(),
                        Numero = alunoResponsaveisDaTurma.AlunoNumeroChamada,
                        UltimoAcesso = usuarioApp?.UltimoLogin.ToString("dd/MM/yyyy HH:mm"),
                        SituacaoNoApp = ObtemSituacaoApp(usuarioApp, alunoResponsaveisDaTurma)
                    };
                    turmaParaAdicionar.Alunos.Add(alunoResponsavelParaAdicionar);
                }
            }
        }

        private void TrataListarTodosCpf(IEnumerable<AlunoResponsavelAdesaoAEDto> alunosResponsaveisParaTratar, IEnumerable<UsuarioAEDto> usuariosDoApp, TurmaResumoDto turma, AdesaoAETurmaDto turmaParaAdicionar)
        {


            var alunosResponsaveisDaTurma = alunosResponsaveisParaTratar.Where(a => a.TurmaCodigo == long.Parse(turma.Codigo)).OrderBy(a => a.NomeAlunoParaVisualizar());

            foreach (var alunoResponsaveisDaTurma in alunosResponsaveisDaTurma)
            {

                var usuarioApp = usuariosDoApp.FirstOrDefault(a => a.Cpf == alunoResponsaveisDaTurma.ResponsavelCpf);

                var alunoResponsavelParaAdicionar = new AdesaoAEUeAlunoDto()
                {
                    Contato = alunoResponsaveisDaTurma.ResponsavelCelularFormatado(),
                    CpfResponsavel = alunoResponsaveisDaTurma.ResponsavelCpf?.ToString(),
                    Responsavel = alunoResponsaveisDaTurma.ResponsavelNome.Trim(),
                    Estudante = alunoResponsaveisDaTurma.NomeAlunoParaVisualizar(),
                    Numero = alunoResponsaveisDaTurma.AlunoNumeroChamada?.Trim(),
                    UltimoAcesso = usuarioApp?.UltimoLogin.ToString("dd/MM/yyyy HH:mm"),
                    SituacaoNoApp = ObtemSituacaoApp(usuarioApp, alunoResponsaveisDaTurma)
                };

                turmaParaAdicionar.Alunos.Add(alunoResponsavelParaAdicionar);

            }

        }

        private string ObtemSituacaoApp(UsuarioAEDto usuarioApp, AlunoResponsavelAdesaoAEDto alunoResponsavelAdesaoAEDto)
        {
            if (string.IsNullOrEmpty(alunoResponsavelAdesaoAEDto.ResponsavelCpf) || !UtilCPF.Valida(alunoResponsavelAdesaoAEDto.ResponsavelCpf))
                return "CPF inválido";

            if (usuarioApp == null || usuarioApp.Excluido)
                return "Não tem";
            else
            {
                if (usuarioApp.PrimeiroAcesso)
                    return "Primeiro acesso incompleto";
            }
            return "Tem";
        }
    }
}


