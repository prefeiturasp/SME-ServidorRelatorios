using Nest;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Data.Models.ElasticSearch;
using SME.SR.Data.Repositories.ElasticSearch.Base;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.ElasticSearch
{
    public class RepositorioElasticComponenteCurricular : RepositorioElasticBase<DocumentoElasticTurma>, IRepositorioElasticComponenteCurricular
    {
        public RepositorioElasticComponenteCurricular(IElasticClient elasticClient, IServicoTelemetria servicoTelemetria) : base(
            elasticClient, servicoTelemetria)
        {
        }

        public async Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesAsync(string[] codigosTurma)
        {
            QueryContainer query = new QueryContainerDescriptor<TurmaComponentesDto>();

            query &= new QueryContainerDescriptor<TurmaComponentesDto>().Bool(b => b
                .Should(
                    s => s.MatchPhrase(m => m.Field(f => f.SituacaoTurmaEscola).Query("O")),
                    s => s.MatchPhrase(m => m.Field(f => f.SituacaoTurmaEscola).Query("A")),
                    s => s.MatchPhrase(m => m.Field(f => f.SituacaoTurmaEscola).Query("C"))
                )
                .MinimumShouldMatch(1)
            );

            if (codigosTurma != null && codigosTurma.Length > 0)
            {
                var codigosTurmaLong = codigosTurma.Where(codigo => long.TryParse(codigo, out _))
                                                   .Select(long.Parse)
                                                   .ToArray();

                if (codigosTurmaLong.Any())
                {
                    query = query &&
                            new QueryContainerDescriptor<TurmaComponentesDto>()
                                .Terms(termo => termo.Field(f => f.CodigoTurma).Terms(codigosTurmaLong));
                }
            }
            var listagemTurmas = await ObterListaAsync<TurmaComponentesDto>(IndicesElastic.INDICE_TURMA_COMPONENTES,
                _ => query, "Buscar listagem de componentes");

            if (listagemTurmas == null)
                return default;

            return ConverterParaComponenteCurricular(listagemTurmas);

        }

        private IEnumerable<ComponenteCurricular> ConverterParaComponenteCurricular(IEnumerable<TurmaComponentesDto> turmasDto)
        {
            var componentesCurriculares = new List<ComponenteCurricular>();

            foreach (var turma in turmasDto)
            {
                if (turma.Componentes != null && turma.Componentes.Any())
                {
                    foreach (var componente in turma.Componentes)
                    {
                        componentesCurriculares.Add(new ComponenteCurricular
                        {
                            Codigo = componente.ComponenteCurricularCodigo,
                            Descricao = componente.NomeComponenteCurricular,
                            CodigoTurma = turma.CodigoTurma.ToString(),
                            AnoTurma = turma.AnoTurma,
                            TurnoTurma = int.TryParse(turma.Turno, out var turno) ? turno : 0,
                            TipoEscola = turma.TipoEscola.ToString()
                        });
                    }
                }
            }

            return componentesCurriculares;
        }
    }
}