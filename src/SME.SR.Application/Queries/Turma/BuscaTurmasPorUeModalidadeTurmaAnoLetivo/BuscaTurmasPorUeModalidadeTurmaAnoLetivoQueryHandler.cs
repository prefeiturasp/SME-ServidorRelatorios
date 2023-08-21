using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Infra.Dtos;

namespace SME.SR.Application.Queries
{
    public class BuscaTurmasPorUeModalidadeTurmaAnoLetivoQueryHandler : IRequestHandler<BuscaTurmasPorUeModalidadeTurmaAnoLetivoQuery, PaginacaoResultadoDto<RetornoConsultaListagemTurmaComponenteDto>>
    {
        private readonly IRepositorioElasticTurma repositorioElasticTurma;
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IFuncionarioRepository funcionarioRepository;
        private readonly IMediator mediator;

        public BuscaTurmasPorUeModalidadeTurmaAnoLetivoQueryHandler(IRepositorioElasticTurma repositorioElasticTurma, IComponenteCurricularRepository componenteCurricularRepository,IFuncionarioRepository funcionarioRepository, IMediator mediator)
        {
            this.repositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<PaginacaoResultadoDto<RetornoConsultaListagemTurmaComponenteDto>> Handle(BuscaTurmasPorUeModalidadeTurmaAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            int[] tiposEscolaModalidade = null;

            if (request.Modalidade > 0)
                tiposEscolaModalidade = await mediator.Send(new ObterParametrosTipoModalidadeQuery(request.Modalidade));

            var turmasCacheadas = await repositorioElasticTurma.ObterListaTurmasAsync(
                                                    request.CodigoUe,tiposEscolaModalidade, request.CodigoTurma, request.AnoLetivo, request.EhProfessor, 
                                                    request.CodigoRf, request.ConsideraHistorico, request.PeriodoEscolarInicio, request.Modalidade);

            if (!turmasCacheadas.Any())
                return default;

            var retorno = turmasCacheadas.SelectMany(a => a.Componentes, (turma, componente) => 
                new RetornoConsultaListagemTurmaComponenteDto()
                {
                    TurmaCodigo = turma.CodigoTurma.ToString(),
                    Modalidade = turma.Modalidade,
                    NomeTurma = turma.NomeTurma,
                    Ano = turma.AnoTurma,
                    ComplementoTurmaEJA = turma.ComplementoTurmaEJA,
                    NomeComponenteCurricular = componente.NomeComponenteCurricular,
                    Turno = turma.Turno,
                    ComponenteCurricularCodigo = componente.ComponenteCurricularCodigo
                }).GroupBy(g => new { g.TurmaCodigo, g.Modalidade, g.NomeTurma, g.NomeComponenteCurricular, g.Ano, g.ComplementoTurmaEJA, g.Turno, g.ComponenteCurricularCodigo })
                .Select(s => new RetornoConsultaListagemTurmaComponenteDto()
                {
                    TurmaCodigo = s.Key.TurmaCodigo,
                    Modalidade = s.Key.Modalidade,
                    NomeTurma = s.Key.NomeTurma,
                    Ano = s.Key.Ano,
                    ComplementoTurmaEJA = s.Key.ComplementoTurmaEJA,
                    NomeComponenteCurricular = s.Key.NomeComponenteCurricular,
                    Turno = s.Key.Turno,
                    ComponenteCurricularCodigo = s.Key.ComponenteCurricularCodigo
                });

            if (request.AnosInfantilDesconsiderar.Any(a => a != null))
                retorno = retorno.Where(w => !request.AnosInfantilDesconsiderar.Contains(w.Ano));

            var turmasComTerritorioSaber = await TratarComponentesTerritorioSaber(retorno);

            var totalRegistros = turmasComTerritorioSaber.Any() ? turmasComTerritorioSaber.Count() : 0;

            var retornoTurmas = new PaginacaoResultadoDto<RetornoConsultaListagemTurmaComponenteDto>()
            {
                Items = turmasComTerritorioSaber,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / request.QtdeRegistros)
            };

            return retornoTurmas;
        }
        
        private async Task<IEnumerable<RetornoConsultaListagemTurmaComponenteDto>> TratarComponentesTerritorioSaber(IEnumerable<RetornoConsultaListagemTurmaComponenteDto> listagemTurmasComponentes)
        {
            var componentesApiEol = await componenteCurricularRepository.ObterComponentesCurricularesAPIEol();

            var turmasComponentes = listagemTurmasComponentes.ToList();

            foreach (var turmaComponentes in turmasComponentes
                .Where(a => componentesApiEol.Any(c => c.IdComponenteCurricular == a.ComponenteCurricularCodigo && c.EhTerritorio))
                .GroupBy(a => a.TurmaCodigo))
            {
                var territoriosBanco = await funcionarioRepository.BuscarDisciplinaTerritorioDosSaberesAsync(turmaComponentes.Key.ToString(), turmaComponentes.Select(a => a.ComponenteCurricularCodigo));
                if (territoriosBanco != null && territoriosBanco.Any())
                {
                    var turma = turmaComponentes.First();
                    turmasComponentes.RemoveAll(c => territoriosBanco.Any(x => x.CodigoComponenteCurricular == c.ComponenteCurricularCodigo));

                    var territorios = territoriosBanco.GroupBy(c => new { c.CodigoTerritorioSaber, c.CodigoExperienciaPedagogica, c.DataInicio });

                    foreach (var componenteTerritorio in territorios)
                    {
                        var componenteCurricular = turmaComponentes.FirstOrDefault(c => c.ComponenteCurricularCodigo == componenteTerritorio.First().CodigoComponenteCurricular);

                        var componenteCurricularCodigo = componenteTerritorio.FirstOrDefault().ObterCodigoComponenteCurricular(turmaComponentes.Key.ToString());

                        turmasComponentes.Add(new RetornoConsultaListagemTurmaComponenteDto()
                        {
                            ComponenteCurricularCodigo = componenteCurricularCodigo,
                            ComponenteCurricularTerritorioSaberCodigo = componenteTerritorio.FirstOrDefault().CodigoComponenteCurricular,
                            NomeComponenteCurricular = componenteTerritorio.FirstOrDefault().ObterDescricaoComponenteCurricular(),
                            TerritorioSaber = true,
                            Ano = turma.Ano,
                            Id = (turmasComponentes.Where(a => a.TurmaCodigo == turmaComponentes.Key).Count() + 1).ToString(),
                            Modalidade = turma.Modalidade,
                            NomeTurma = turma.NomeTurma,
                            TurmaCodigo = turma.TurmaCodigo,
                            Turno = turma.Turno,
                            ComplementoTurmaEJA = componenteCurricular.ComplementoTurmaEJA
                        });
                    }
                }
            }
            listagemTurmasComponentes = turmasComponentes;
            return listagemTurmasComponentes;
        }
    }
}