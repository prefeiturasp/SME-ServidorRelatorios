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
                                  filtros.Usuario.CodigoRf);

            if (filtros.ExibirAtribuicoesExporadicas)
            {
                var lstAtribuicaoEsporadica = await mediator.Send(new ObterAtribuicoesEsporadicasPorFiltroQuery(filtros.AnoLetivo,
                                                                                                                filtros.UsuarioRf,
                                                                                                                filtros.DreCodigo,
                                                                                                                filtros.UsuarioRf,
                                                                                                                filtros.UeCodigo));



                var lstServidores = lstAtribuicaoEsporadica.Select(s => s.ProfessorRf);

                var cargosServidor = await mediator.Send(new ObterCargosServidoresPorAnoLetivoQuery(filtros.AnoLetivo, lstServidores.ToArray()));

                AdicionarAtribuicoesEsporadicas(relatorio, lstAtribuicaoEsporadica, cargosServidor);
            }

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

            var turmasId = lstAtribuicaoCJ.Select(t => t.TurmaId);

            var lstProfTitulares = await mediator.Send(new ObterProfessorTitularComponenteCurricularPorTurmaQuery()
            {
                 CodigosTurma = turmasId.ToArray()
            });

            AdicionarAtribuicoesCJ(relatorio, lstAtribuicaoCJ, lstProfTitulares, filtros.TipoVisualizacao);

        }

        private void AdicionarAtribuicoesCJ(RelatorioAtribuicaoCjDto relatorio, IEnumerable<AtribuicaoCJ> lstAtribuicaoCJ, IEnumerable<ProfessorTitularComponenteCurricularDto> lstProfTitulares, TipoVisualizacaoRelatorioAtribuicaoCJ tipoVisualizacao)
        {
            if (tipoVisualizacao == TipoVisualizacaoRelatorioAtribuicaoCJ.Professor)
            {
                var agrupamento = lstAtribuicaoCJ.GroupBy(cj => new { cj.ProfessorRf, cj.ProfessorNome });

                relatorio.AtribuicoesCjPorProfessor.AddRange(
                    agrupamento.Select(professor =>
                    {
                        var retorno = new AtribuicaoCjPorProfessorDto();

                        retorno.NomeProfessor = $"{professor.Key.ProfessorNome} ({professor.Key.ProfessorRf})";
                        retorno.AtribuiicoesCjTurma.AddRange(
                             professor.Select(t =>
                             {
                                 var titular = lstProfTitulares.FirstOrDefault(p => p.TurmaCodigo == t.TurmaId && 
                                                                                    p.ComponenteCurricularId == t.ComponenteCurricularId.ToString());

                                 var retorno = new AtribuicaoCjTurmaDto()
                                 {
                                     ComponenteCurricular = t.ComponenteCurricularNome,
                                     DataAtribuicao = t.CriadoEm.ToString("dd/MM/yyyy"),
                                     NomeProfessorTitular = titular.NomeProfessor,
                                     NomeTurma = t.Turma.Nome
                                 };
                                 return retorno;
                             }));

                        return retorno;
                    }));
            }
            else
            {
                var agrupamento = lstAtribuicaoCJ.GroupBy(cj => new { cj.TurmaId, cj.Turma.Nome});

                relatorio.AtribuicoesCjPorTurma.AddRange(
                   agrupamento.Select(turma =>
                   {
                       var retorno = new AtribuicaoCjPorTurmaDto();

                       retorno.NomeTurma = $"{turma.Key.Nome}";
                       retorno.AtribuicoesCjProfessor.AddRange(
                            turma.Select(t =>
                            {
                                var titular = lstProfTitulares.FirstOrDefault(p => p.TurmaCodigo == t.TurmaId &&
                                                                                   p.ComponenteCurricularId == t.ComponenteCurricularId.ToString());

                                var retorno = new AtribuicaoCjProfessorDto()
                                {
                                    ComponenteCurricular = t.ComponenteCurricularNome,
                                    DataAtribuicao = t.CriadoEm.ToString("dd/MM/yyyy"),
                                    NomeProfessorTitular = titular.NomeProfessor,
                                    NomeProfessorCj = t.ProfessorNome
                                };
                                return retorno;
                            }));

                       return retorno;
                   }));
            }
        }

        private void AdicionarAtribuicoesEsporadicas(RelatorioAtribuicaoCjDto relatorio, IEnumerable<AtribuicaoEsporadica> lstAtribuicaoEsporadica, IEnumerable<ServidorCargoDto> cargosServidor)
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
                                           Modalidade modalidade, int? semestre, string codigoTurma, string professorRf,
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

            relatorio.Modalidade = modalidade.Name();

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

                relatorio.Professor = professor.NomeRelatorio;
            }

            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery(usuarioRf));

            relatorio.Usuario = usuario.NomeRelatorio;
        }
    }
}
