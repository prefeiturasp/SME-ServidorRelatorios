using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAtribuicaoCJUseCase : IRelatorioAtribuicaoCJUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAtribuicaoCJUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioAtribuicaoCJDto>();

            var relatorio = new RelatorioAtribuicaoCjDto();

            await MontarCabecalho(relatorio, filtros.DreCodigo, filtros.UeCodigo, filtros.Modalidade,
                                  filtros.Semestre, filtros.TurmaCodigo, filtros.UsuarioRf,
                                  request.UsuarioLogadoRF);

            var lstAtribuicaoEsporadica = await mediator.Send(new ObterAtribuicoesEsporadicasPorFiltroQuery(filtros.AnoLetivo,
                                                                                                               filtros.UsuarioRf,
                                                                                                               filtros.DreCodigo,
                                                                                                               filtros.UsuarioRf,
                                                                                                               filtros.UeCodigo));

            var lstAtribuicaoCJ = await mediator.Send(new ObterAtribuicoesCJPorFiltroQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                DreCodigo = filtros.DreCodigo,
                UeId = filtros.UeCodigo,
                UsuarioRf = filtros.UsuarioRf,
                TurmaId = filtros.TurmaCodigo,
                Modalidade = filtros.Modalidade,
                Semestre = filtros.Semestre
            });

            var lstServidores = new List<string>();

            lstServidores.AddRange(lstAtribuicaoEsporadica.Select(s => s.ProfessorRf));
            lstServidores.AddRange(lstAtribuicaoCJ.Select(cj => cj.ProfessorRf));

            var lstServidoresArray = lstServidores?.Distinct().ToArray();

            var cargosServidores = await mediator.Send(new ObterCargosAtividadesPorRfQuery(lstServidoresArray));

            var lstProfServidorTitulares = await mediator.Send(new ObterProfessorTitularComponenteCurricularPorCodigosRfQuery(lstServidoresArray));

            if (filtros.ExibirAtribuicoesExporadicas)
                AdicionarAtribuicoesEsporadicas(relatorio, lstAtribuicaoEsporadica, cargosServidores);

            var turmasId = lstAtribuicaoCJ.Select(t => t.Turma.Codigo)?.Distinct().ToArray();

            var componentesId = lstAtribuicaoCJ.Select(t => t.ComponenteCurricularId.ToString())?.Distinct().ToArray();

            var lstProfTitulares = await mediator.Send(new ObterProfessorTitularComponenteCurricularPorTurmaQuery(turmasId));

            var aulas = Enumerable.Empty<AulaVinculosDto>();

            if (filtros.ExibirAulas)
                aulas = await mediator.Send(new ObterAulaVinculosPorTurmaComponenteQuery(turmasId, componentesId, true));

            AdicionarAtribuicoesCJ(relatorio, lstAtribuicaoCJ, lstProfTitulares, lstProfServidorTitulares, lstAtribuicaoEsporadica, cargosServidores, aulas,
                                   filtros.TipoVisualizacao, filtros.ExibirAulas, filtros.Modalidade);

            OrdernarRelatorio(relatorio, filtros.TipoVisualizacao);

            if (string.IsNullOrEmpty(filtros.DreCodigo) && string.IsNullOrEmpty(filtros.UeCodigo))
            {
                relatorio.ExibirDre = true;
                //Chamar Query para realizar agrupamento e montagem;
            }

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAtribuioesCj", relatorio, request.CodigoCorrelacao));

        }

        private void OrdernarRelatorio(RelatorioAtribuicaoCjDto relatorio, TipoVisualizacaoRelatorioAtribuicaoCJ tipoVisualizacao)
        {
            if (relatorio.RelatorioVazio(tipoVisualizacao))
            {
                throw new NegocioException("Não foram encontrados dados a serem impressos");
            }
            else
            {
                if (relatorio.AtribuicoesEsporadicas.Any())
                    relatorio.AtribuicoesEsporadicas = relatorio.AtribuicoesEsporadicas.OrderBy(o => o.NomeUsuario).ToList();

                if (relatorio.AtribuicoesCjPorTurma.Any())
                {
                    relatorio.AtribuicoesCjPorTurma = relatorio.AtribuicoesCjPorTurma.OrderBy(o => o.NomeTurma).ToList();
                    relatorio.AtribuicoesCjPorTurma.ForEach(a =>
                    {
                        a.AtribuicoesCjProfessor = a.AtribuicoesCjProfessor.OrderBy(o => o.ComponenteCurricular).ThenBy(o => o.NomeProfessorCj).ToList();
                    });

                }

                if (relatorio.AtribuicoesCjPorProfessor.Any())
                {
                    relatorio.AtribuicoesCjPorProfessor = relatorio.AtribuicoesCjPorProfessor.OrderBy(o => o.NomeProfessor).ToList();
                    relatorio.AtribuicoesCjPorProfessor.ForEach(a =>
                    {
                        a.AtribuiicoesCjTurma = a.AtribuiicoesCjTurma.OrderBy(o => o.NomeTurma).ThenBy(o => o.ComponenteCurricular).ToList();
                    });

                }
            }
        }

        private void AdicionarAtribuicoesCJ(RelatorioAtribuicaoCjDto relatorio, IEnumerable<AtribuicaoCJ> lstAtribuicaoCJ,
                                            IEnumerable<ProfessorTitularComponenteCurricularDto> lstProfTitulares,
                                            IEnumerable<ProfessorTitularComponenteCurricularDto> lstProfServidorTitulares,
                                            IEnumerable<AtribuicaoEsporadica> lstAtribuicaoEsporadica, IEnumerable<ServidorCargoDto> cargosServidores,
                                            IEnumerable<AulaVinculosDto> aulas, TipoVisualizacaoRelatorioAtribuicaoCJ tipoVisualizacao, bool exibirAulas,
                                            Modalidade? filtroModalidade)
        {
            if (tipoVisualizacao == TipoVisualizacaoRelatorioAtribuicaoCJ.Professor)
            {
                var agrupamento = lstAtribuicaoCJ.GroupBy(cj => new { cj.ProfessorRf, cj.ProfessorNome });

                relatorio.AtribuicoesCjPorProfessor.AddRange(
                    agrupamento.Select(professor =>
                    {
                        var retorno = new AtribuicaoCjPorProfessorDto();

                        string tipoCJ = ObterTipoProfessorCJ(professor.Key.ProfessorRf, lstAtribuicaoEsporadica, lstProfServidorTitulares, cargosServidores);

                        retorno.NomeProfessor = $"{professor.Key.ProfessorNome} ({professor.Key.ProfessorRf}) - {tipoCJ}";
                        retorno.AtribuiicoesCjTurma.AddRange(
                             professor.Select(t =>
                             {
                                 var titular = lstProfTitulares.FirstOrDefault(p => p.TurmaCodigo == t.Turma.Codigo &&
                                                                                    p.ComponenteCurricularId == t.ComponenteCurricularId.ToString());



                                 var retorno = new AtribuicaoCjTurmaDto()
                                 {
                                     ComponenteCurricular = t.ComponenteCurricularNome,
                                     DataAtribuicao = t.CriadoEm.ToString("dd/MM/yyyy"),
                                     NomeProfessorTitular = titular != null ? titular.ProfessorNomeRf : string.Empty,
                                     NomeTurma = t.Turma.Nome,
                                     Aulas = exibirAulas ? ObterAulasDadas(t.ProfessorRf, t.Turma.Codigo, t.ComponenteCurricularId, aulas)?.ToList() : null
                                 };
                                 return retorno;
                             }));

                        return retorno;
                    }));
            }
            else
            {
                var agrupamento = lstAtribuicaoCJ.GroupBy(cj => new { cj.Turma });

                relatorio.AtribuicoesCjPorTurma.AddRange(
                   agrupamento.Select(turma =>
                   {
                       var retorno = new AtribuicaoCjPorTurmaDto();

                       retorno.NomeTurma = turma.Key.Turma.NomePorFiltroModalidade(filtroModalidade);
                       retorno.AtribuicoesCjProfessor.AddRange(
                            turma.Select(t =>
                            {
                                var titular = lstProfTitulares.FirstOrDefault(p => p.TurmaCodigo == t.Turma.Codigo &&
                                                                                   p.ComponenteCurricularId == t.ComponenteCurricularId.ToString());

                                var retorno = new AtribuicaoCjProfessorDto()
                                {
                                    ComponenteCurricular = t.ComponenteCurricularNome,
                                    DataAtribuicao = t.CriadoEm.ToString("dd/MM/yyyy"),
                                    NomeProfessorTitular = titular != null ? titular.ProfessorNomeRf : string.Empty,
                                    NomeProfessorCj = t.ProfessorNomeRf,
                                    TipoProfessorCj = ObterTipoProfessorCJ(t.ProfessorRf, lstAtribuicaoEsporadica, lstProfServidorTitulares, cargosServidores),
                                    Aulas = exibirAulas ? ObterAulasDadas(t.ProfessorRf, t.Turma.Codigo, t.ComponenteCurricularId, aulas)?.ToList() : null
                                };
                                return retorno;
                            }));

                       return retorno;
                   }));
            }
        }

        private IEnumerable<AtribuicaoCjAulaDto> ObterAulasDadas(string professorRf, string codigoTurma, long componenteCurricularId, IEnumerable<AulaVinculosDto> aulas)
        {
            return aulas.Where(aula => aula.ProfessorRf == professorRf && aula.TurmaCodigo == codigoTurma &&
                                       aula.ComponenteCurricularId == componenteCurricularId.ToString()).Select(aula =>
                {
                    return new AtribuicaoCjAulaDto()
                    {
                        AulaDada = aula.AulaDada(),
                        DataAula = aula.Data.ToString("dd/MM/yyyy"),
                        Observacoes = aula.Observacoes()
                    };
                });
        }

        private string ObterTipoProfessorCJ(string professorRf, IEnumerable<AtribuicaoEsporadica> lstAtribuicaoEsporadica,
                                            IEnumerable<ProfessorTitularComponenteCurricularDto> lstProfTitulares, IEnumerable<ServidorCargoDto> cargosServidores)
        {
            List<int> idsComponentesCurricularesCj = new List<int> { 514, 526, 527, 528, 529, 530, 531, 532, 533 };

            if (idsComponentesCurricularesCj.Any(componenteCJ => cargosServidores.Any(cargo => cargo.CodigoRF == professorRf &&
                                                                                               cargo.CodigoComponenteCurricular == componenteCJ.ToString())))
                return "CJ da UE";
            else if (lstAtribuicaoEsporadica.Any(esporadico => esporadico.ProfessorRf == professorRf))
                return "Esporádico";
            else if (cargosServidores.Any(cargo => cargo.CodigoRF == professorRf && cargo.PossuiCargoSobrepostoGestao()))
                return "Gestão da UE";
            else if (lstProfTitulares.Any(titular => titular.ProfessorRf == professorRf))
                return "Titular da UE";

            return string.Empty;

        }

        private void AdicionarAtribuicoesEsporadicas(RelatorioAtribuicaoCjDto relatorio, IEnumerable<AtribuicaoEsporadica> lstAtribuicaoEsporadica,
                                                     IEnumerable<ServidorCargoDto> cargosServidor)
        {
            relatorio.AtribuicoesEsporadicas.AddRange(
                lstAtribuicaoEsporadica.Select(atribuicao =>
                {
                    var cargo = cargosServidor.FirstOrDefault(cargo => cargo.CodigoRF == atribuicao.ProfessorRf);

                    var retorno = new AtribuicaoEsporadicaDto()
                    {
                        AtribuidoPor = atribuicao.CriadoPor,
                        DataAtribuicao = atribuicao.CriadoEm.ToString("dd/MM/yyyy"),
                        DataInicio = atribuicao.DataInicio.ToString("dd/MM/yyyy"),
                        DataFim = atribuicao.DataFim.ToString("dd/MM/yyyy"),
                        NomeUsuario = cargo.NomeRelatorio,
                        Cargo = cargo.CargoRelatorio
                    };

                    return retorno;
                }));
        }

        private async Task MontarCabecalho(RelatorioAtribuicaoCjDto relatorio, string dreCodigo, string ueCodigo,
                                           Modalidade? modalidade, int? semestre, string codigoTurma, string professorRf,
                                           string usuarioRf)
        {
            if (!string.IsNullOrEmpty(dreCodigo))
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));
                relatorio.DreNome = dre.Abreviacao;
            }
            else
                relatorio.DreNome = "Todas";

            if (!string.IsNullOrEmpty(ueCodigo))
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));

                relatorio.UeNome = ue.NomeRelatorio;
            }
            else
                relatorio.UeNome = "Todas";

            if (modalidade.HasValue)
            {
                relatorio.Modalidade = modalidade.Name();
            }
            else
                relatorio.Modalidade = "Todas";

            if (semestre.HasValue && semestre > 0)
                relatorio.Semestre = $"{semestre}º Semestre";

            if (!string.IsNullOrEmpty(codigoTurma))
            {
                var turma = await mediator.Send(new ObterTurmaQuery(codigoTurma));

                relatorio.Turma = turma.NomeRelatorio;
            }
            else
                relatorio.Turma = "Todas";

            if (!string.IsNullOrEmpty(professorRf))
            {
                var professor = await mediator.Send(new ObterUsuarioPorCodigoRfQuery(professorRf));

                relatorio.Professor = professor.Nome;
                relatorio.RfProfessor = professor.CodigoRf;
            }

            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery(usuarioRf));

            relatorio.Usuario = usuario.Nome;
            relatorio.RfUsuario = usuario.CodigoRf;
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorFiltro(string dreCodigo, string ueCodigo, int anoLetivo, Modalidade? modalidade, int? semestre, string login, Guid perfilAtual, bool consideraHistorico)
        {
            return await mediator.Send(new ObterTurmasPorAbrangenciaFiltroQuery()
            {
                CodigoDre = dreCodigo,
                CodigoUe = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade ?? default,
                Semestre = semestre ?? 0,
                Login = login,
                Perfil = perfilAtual,
                ConsideraHistorico = consideraHistorico
            });
        }
    }
}
