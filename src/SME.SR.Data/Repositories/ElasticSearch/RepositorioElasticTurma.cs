using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SME.Pedagogico.Repository;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Data.Models.Elastic;
using SME.SR.Data.Repositories.ElasticSearch.Base;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;

namespace SME.SR.Data.Repositories.ElasticSearch
{
    public class RepositorioElasticTurma : RepositorioElasticBase<DocumentoElasticTurma>, IRepositorioElasticTurma
    {
        public RepositorioElasticTurma(IElasticClient elasticClient, IServicoTelemetria servicoTelemetria) : base(elasticClient, servicoTelemetria)
        {
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> ObterAlunosAtivosNaTurmaAsync(int codigoTurma, DateTime dataAula)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoTurma, codigoTurma);
            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().DateRange(termo => termo.Field(a => a.DataSituacao).LessThanOrEquals(dataAula));

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                        IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query, 
                        "Buscar alunos ativos na turma", 
                        new { codigoTurma, DataSituacao = dataAula});

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoAluno)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                            .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                            .First())
                                                .Where(aluno => aluno.CodigoSituacaoMatricula != (int)TipoSituacaoMatricula.VinculoIndevido);

            return result?.ToList();
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> ObterMatriculasTurmaDoAlunoAsync(string codigoAluno, DateTime? dataAula, int? anoLetivo)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoAluno, codigoAluno);
            if (dataAula != null)
                query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().DateRange(termo => termo.Field(a => a.DataSituacao).LessThanOrEquals(dataAula));
            if ((anoLetivo ?? 0) > 0)
                query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.Ano, anoLetivo);

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                        IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query,
                        "Buscar turmas do aluno",
                        new { codigoAluno, DataSituacao = dataAula, AnoLetivo = anoLetivo });

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoMatricula)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                    .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                    .First());

            return result?.ToList();
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> ObterAlunosNaTurmaAsync(int codigoTurma, int? anoLetivo)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoTurma, codigoTurma);

            if ((anoLetivo ?? 0) > 0)
                query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.Ano, anoLetivo);

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                        IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query, 
                        "Busca alunos na turma",
                        new { codigoTurma });

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoAluno)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                            .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                            .First());

            return result?.ToList();
        }

        public async Task<IReadOnlyList<AlunoNaTurmaDTO>> ObterAlunosPorTurmaAsync(int codigoTurma, int codigoAluno, bool consideraInativos = false)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoTurma, codigoTurma);

            if (codigoAluno > 0)
                query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoAluno, codigoAluno);

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                                    IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query, 
                                    "Busca alunos por turma",
                                    new { codigoTurma, codigoAluno, consideraInativos });

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoAluno)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                            .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                            .First());
            if (!consideraInativos)
                result = result.Where(aluno => new List<int> { 1, 2, 3, 5, 6, 10, 13 }.Contains(aluno.CodigoSituacaoMatricula));
            return result?.ToList();
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> ObterAlunosPorTurmaMultiplexConnetionAsync(int codigoTurma)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoTurma, codigoTurma);

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                                IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query, 
                                "Busca alunos por turma multiplex",
                                new { codigoTurma });

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoAluno)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                            .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                            .First());

            return result?.ToList();
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> ObterMatriculasAlunoNaTurma(int codigoTurma, int codigoAluno)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoTurma, codigoTurma);
            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoAluno, codigoAluno);

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                                IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query, 
                                "Busca matricula aluno na turma",
                                new { codigoTurma, codigoAluno });

            var result = alunosTurma?.GroupBy(aluno => aluno.CodigoMatricula)
                                                .Select(agrupado =>
                                                    agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                    .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                    .First());

            return result?.ToList();
        }

        public async Task<IEnumerable<TurmaComponentesDto>> ObterListaTurmasAsync(
            string codigoUe, int[] tiposEscolaModalidade, long codigoTurma, int anoLetivo,
            bool ehProfessor, string codigoRf, bool consideraHistorico,
            DateTime periodoEscolarInicio, int modalidade)
        {
            QueryContainer query = new QueryContainerDescriptor<TurmaComponentesDto>();

            query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(p => p.CodigoEscola, codigoUe);
            query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Ano, anoLetivo);
            query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Modalidade, modalidade);

            if (codigoTurma > 0)
                query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.CodigoTurma, codigoTurma);

            if (ehProfessor)
                query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(a => a.Componentes.First().RegistroFuncional, codigoRf);

            if (tiposEscolaModalidade != null)
            {
                query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Terms(c => c.Name(IndicesElastic.INDICE_TURMA_COMPONENTES)
                    .Boost(1.1)
                    .Field(p => p.TipoEscola)
                    .Terms(tiposEscolaModalidade));
            }

            if (consideraHistorico)
            {
                query = query && (new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Historica, consideraHistorico)
                                && new QueryContainerDescriptor<TurmaComponentesDto>().MatchPhrase(p => p.Field(f => f.SituacaoTurmaEscola.Equals("C")))
                              || (new QueryContainerDescriptor<TurmaComponentesDto>().MatchPhrase(p => p.Field(f => f.SituacaoTurmaEscola.Equals("E")))
                                && new QueryContainerDescriptor<TurmaComponentesDto>().
                                    DateRange(termo => termo.Field(a =>
                                        a.DataStatusTurmaEscola).GreaterThanOrEquals(periodoEscolarInicio))));

            }
            else
                query = query && new QueryContainerDescriptor<TurmaComponentesDto>().Term(termo => termo.Historica, consideraHistorico);

            var listagemTurmas = await ObterListaAsync<TurmaComponentesDto>(IndicesElastic.INDICE_TURMA_COMPONENTES, _ => query, "Buscar listagem de turmas");

            if (listagemTurmas == null)
                return default;

            ExecuteFiltroComponentePorRf(ehProfessor, codigoRf, listagemTurmas);

            return listagemTurmas;
        }

        private void ExecuteFiltroComponentePorRf(bool ehProfessor, string codigoRf, IEnumerable<TurmaComponentesDto> lista)
        {
            if (!ehProfessor)
                return;

            foreach(var turma in lista)
            {
                turma.Componentes = turma.Componentes.ToList().FindAll(componente => componente.RegistroFuncional == codigoRf);
            }
        }

        public async Task<IEnumerable<TurmaComponentesDto>> ObterTurmasAsync(int[] codigosTurmas)
        {
            QueryContainer query = new QueryContainerDescriptor<TurmaComponentesDto>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Terms(t => t.Field(f => f.CodigoTurma).Terms(codigosTurmas));

            var listagemTurmas = await ObterListaAsync<TurmaComponentesDto>(IndicesElastic.INDICE_TURMA_COMPONENTES, _ => query, "Buscar listagem de turmas");

            if (listagemTurmas == null)
                return default;

            return listagemTurmas;
        }

        public async Task<IReadOnlyList<AlunoNaTurmaDTO>> ObterTodosAlunosNaTurmaAsync(int codigoTurma, int? codigoAluno)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoTurma, codigoTurma);

            if (codigoAluno.HasValue)
                query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Term(termo => termo.CodigoAluno, codigoAluno);

            var alunosTurma = await ObterListaAsync<AlunoNaTurmaDTO>(
                IndicesElastic.INDICE_ALUNO_TURMA_DRE, _ => query,
                "Busca matricula aluno na turma",
                new { codigoTurma, codigoAluno });

            var listaRetorno = new List<AlunoNaTurmaDTO>();

            foreach (var matriculaAlunoAtual in alunosTurma.OrderBy(a => a.CodigoAluno).ThenBy(a => a.DataSituacao))
            {
                var matriculaExistente = listaRetorno.LastOrDefault(m => m.CodigoMatricula == matriculaAlunoAtual.CodigoMatricula);
                if (matriculaExistente != null && (TipoSituacaoMatricula)matriculaExistente.CodigoSituacaoMatricula != TipoSituacaoMatricula.RemanejadoSaida)
                {
                    matriculaExistente.CodigoSituacaoMatricula = matriculaAlunoAtual.CodigoSituacaoMatricula;
                    matriculaExistente.SituacaoMatricula = matriculaAlunoAtual.SituacaoMatricula;
                    matriculaExistente.DataSituacao = matriculaAlunoAtual.DataSituacao;                   
                }
                else
                {
                    listaRetorno.Add(new AlunoNaTurmaDTO()
                    {
                        CodigoAluno = matriculaAlunoAtual.CodigoAluno,
                        NomeAluno = matriculaAlunoAtual.NomeAluno,
                        DataNascimento = matriculaAlunoAtual.DataNascimento,
                        NomeSocialAluno = matriculaAlunoAtual.NomeSocialAluno,
                        CodigoSituacaoMatricula = matriculaAlunoAtual.CodigoSituacaoMatricula,
                        SituacaoMatricula = matriculaAlunoAtual.SituacaoMatricula,
                        DataSituacao = matriculaAlunoAtual.DataSituacao,
                        DataMatricula = matriculaAlunoAtual.DataSituacao,
                        NumeroAlunoChamada = matriculaAlunoAtual.NumeroAlunoChamada,
                        PossuiDeficiencia = matriculaAlunoAtual.PossuiDeficiencia,
                        Transferencia_Interna = matriculaAlunoAtual.Transferencia_Interna,
                        Remanejado = matriculaAlunoAtual.Remanejado,
                        EscolaTransferencia = matriculaAlunoAtual.EscolaTransferencia,
                        TurmaTransferencia = matriculaAlunoAtual.TurmaTransferencia,
                        TurmaRemanejamento = matriculaAlunoAtual.TurmaRemanejamento,
                        ParecerConclusivo = matriculaAlunoAtual.ParecerConclusivo,
                        NomeResponsavel = matriculaAlunoAtual.NomeResponsavel,
                        TipoResponsavel = matriculaAlunoAtual.TipoResponsavel,
                        CelularResponsavel = matriculaAlunoAtual.CelularResponsavel,
                        DataAtualizacaoContato = matriculaAlunoAtual.DataAtualizacaoContato,
                        CodigoMatricula = matriculaAlunoAtual.CodigoMatricula,
                        Sequencia = matriculaAlunoAtual.Sequencia
                    });
                }
            }

            return listaRetorno;           
        }

        public async Task<long> ObterTotalTodosAlunosNasTurmasAsync(int[] codigosTurmas, DateTime dataInicio, DateTime dataFim)
        {
            QueryContainer query = new QueryContainerDescriptor<AlunoNaTurmaDTO>();

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Terms(t=> t.Field(f=> f.CodigoTurma).Terms(codigosTurmas));

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().DateRange(t => t.Field(f => f.DataMatricula).LessThanOrEquals(dataFim));

            query = query && new QueryContainerDescriptor<AlunoNaTurmaDTO>().Terms(t => t.Field(f => f.CodigoSituacaoMatricula).Terms(AlunoComSituacaoMatriculaAtiva()));

            long alunosTurma = await ObterTotalDeRegistroAPartirDeUmaCondicaoAsync<AlunoNaTurmaDTO>(
                IndicesElastic.INDICE_ALUNO_TURMA_DRE, 
                "Busca matricula aluno na turma", _ => query,
                new { codigosTurmas, dataInicio, dataFim});

            return alunosTurma;
        }

        private int[] AlunoComSituacaoMatriculaAtiva()
            => new int[] { (int)TipoSituacaoMatricula.Ativo,
                           (int)TipoSituacaoMatricula.PendenteRematricula,
                           (int)TipoSituacaoMatricula.Rematriculado,
                           (int)TipoSituacaoMatricula.SemContinuidade,
                           (int)TipoSituacaoMatricula.Concluido }; 
       
    }
}
