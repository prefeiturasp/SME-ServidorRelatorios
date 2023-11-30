using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SME.Pedagogico.Repository;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Data.Models.ElasticSearch;
using SME.SR.Data.Repositories.ElasticSearch.Base;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;

namespace SME.SR.Data.Repositories.ElasticSearch
{
    public class RepositorioElasticTurma : RepositorioElasticBase<DocumentoElasticTurma>, IRepositorioElasticTurma
    {
        public RepositorioElasticTurma(IElasticClient elasticClient, IServicoTelemetria servicoTelemetria) : base(
            elasticClient, servicoTelemetria)
        {
        }

        public async Task<IEnumerable<TurmaComponentesDto>> ObterListaTurmasAsync(string codigoUe,
            int[] tiposEscolaModalidade, long codigoTurma, int anoLetivo,
            bool ehProfessor, string codigoRf, bool consideraHistorico, DateTime periodoEscolarInicio, int modalidade)
        {
            QueryContainer query = new QueryContainerDescriptor<TurmaComponentesDto>();

            query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(p => p.CodigoEscola, codigoUe);
            query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Ano, anoLetivo);
            query = query &&
                    new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Modalidade, modalidade);

            if (codigoTurma > 0)
                query = query &&
                        new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.CodigoTurma,
                            codigoTurma);

            if (ehProfessor)
                query = query &&
                        new QueryContainerDescriptor<TurmaComponentesDto>().Term(
                            a => a.Componentes.First().RegistroFuncional, codigoRf);

            if (tiposEscolaModalidade != null)
            {
                query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Terms(c => c
                    .Name(IndicesElastic.INDICE_TURMA_COMPONENTES)
                    .Boost(1.1)
                    .Field(p => p.TipoEscola)
                    .Terms(tiposEscolaModalidade));
            }

            if (consideraHistorico)
            {
                query = query &&
                        (new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Historica,
                             consideraHistorico)
                         && new QueryContainerDescriptor<TurmaComponentesDto>().MatchPhrase(p =>
                             p.Field(f => f.SituacaoTurmaEscola.Equals("C")))
                         || (new QueryContainerDescriptor<TurmaComponentesDto>().MatchPhrase(p =>
                                 p.Field(f => f.SituacaoTurmaEscola.Equals("E")))
                             && new QueryContainerDescriptor<TurmaComponentesDto>().DateRange(termo => termo.Field(a =>
                                 a.DataStatusTurmaEscola).GreaterThanOrEquals(periodoEscolarInicio))));

            }
            else
                query = query &&
                        new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Historica,
                            consideraHistorico);

            var listagemTurmas = await ObterListaAsync<TurmaComponentesDto>(IndicesElastic.INDICE_TURMA_COMPONENTES,
                _ => query, "Buscar listagem de turmas");

            if (listagemTurmas == null)
                return default;

            ExecuteFiltroComponentePorRf(ehProfessor, codigoRf, listagemTurmas);

            return listagemTurmas;
        }

        private void ExecuteFiltroComponentePorRf(bool ehProfessor, string codigoRf,
            IEnumerable<TurmaComponentesDto> lista)
        {
            if (!ehProfessor)
                return;

            foreach (var turma in lista)
            {
                turma.Componentes = turma.Componentes.ToList()
                    .FindAll(componente => componente.RegistroFuncional == codigoRf);
            }
        }

        public async Task<IEnumerable<TurmaComponentesDto>> ObterTurmasAsync(int[] codigosTurmas)
        {
            QueryContainer query = new QueryContainerDescriptor<TurmaComponentesDto>();

            query = query &&
                    new QueryContainerDescriptor<AlunoNaTurmaDTO>().Terms(t =>
                        t.Field(f => f.CodigoTurma).Terms(codigosTurmas));

            var listagemTurmas = await ObterListaAsync<TurmaComponentesDto>(IndicesElastic.INDICE_TURMA_COMPONENTES,
                _ => query, "Buscar listagem de turmas");

            if (listagemTurmas == null)
                return default;

            return listagemTurmas;
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> ObterMatriculasAlunoNaTurma(int[] codigosTurmas)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Terms(t => t.Field(f => f.CodigoTurma).Terms(codigosTurmas));

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                                IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query,
                                "Busca matriculas aluno na turma",
                                new { codigosTurmas});

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoMatricula)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                    .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                    .First());

            return result?.ToList();
        }
    }
}