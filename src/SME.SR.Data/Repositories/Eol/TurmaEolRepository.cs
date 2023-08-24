using SME.Pedagogico.Repository.Constantes;
using SME.SR.Data.Extensions;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class TurmaEolRepository : ITurmaEolRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public TurmaEolRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<Turma> ObterTurmaSondagemPorCodigo(long turmaCodigo)
        {
            var query = @"select cd_turma_escola as codigo, an_letivo as anoLetivo, dc_turma_escola as nome, 5 as ModalidadeCodigo from turma_escola where cd_turma_escola = @turmaCodigo";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryFirstOrDefaultAsync<Turma>(query, new { turmaCodigo });

        }

        private string QueryCompletaCodigosTurmasAnoAtual()
        {
            return $@"SELECT distinct turesc.cd_turma_escola as CodigoTurma
                        FROM turma_escola turesc
                       INNER JOIN matricula_turma_escola matrTurma ON turesc.cd_turma_escola = matrTurma.cd_turma_escola
                       INNER JOIN v_matricula_cotic matricula ON matrTurma.cd_matricula = matricula.cd_matricula
                       INNER JOIN v_aluno_cotic aluno ON matricula.cd_aluno = aluno.cd_aluno              
                       WHERE aluno.cd_aluno in (@alunos)
                         and turesc.cd_tipo_turma IN (@tiposTurmaNormalizado)
                         and turesc.an_letivo = @anoLetivo
                         and ((matrTurma.cd_situacao_aluno in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno <= @data)
                           or (matrTurma.cd_situacao_aluno not in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno > @data))";
        }

        private string QueryCompletaCodigosTurmasAlunosAnosAnteriores()
        {
            var query = new StringBuilder($@"
                       SELECT CodigoTurma FROM (
                       SELECT DISTINCT turesc.cd_turma_escola as CodigoTurma,
                       ROW_NUMBER() OVER (PARTITION BY matrTurma.cd_matricula ORDER BY matrTurma.dt_situacao_aluno desc) as sequencia
                       FROM turma_escola turesc
                       INNER JOIN historico_matricula_turma_escola matrTurma ON matrTurma.cd_turma_escola = turesc.cd_turma_escola
                       INNER JOIN v_historico_matricula_cotic matricula ON matricula.cd_matricula = matrTurma.cd_matricula
                       INNER JOIN v_aluno_cotic aluno ON matricula.cd_aluno = aluno.cd_aluno              
                       WHERE aluno.cd_aluno in (@alunos)
                         and turesc.cd_tipo_turma IN (@tiposTurmaNormalizado)
                         and turesc.an_letivo = @anoLetivo
                         and ((matrTurma.cd_situacao_aluno in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno <= @data)
                           or (matrTurma.cd_situacao_aluno not in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno > @data))
                        ) as turmas
                        WHERE sequencia = 1 ");
            query.AppendLine(" union all ");
            query.AppendLine(QueryCompletaCodigosTurmasAnoAtual());

            return query.ToString();
        }

        public async Task<IEnumerable<int>> BuscarCodigosTurmasAlunoPorAnoLetivoAlunoAsync(int anoLetivo, string[] codigoAlunos, IEnumerable<int> tiposTurma, bool consideraHistorico = false, DateTime? dataReferencia = null, string ueCodigo = null)
        {
            var query = "SELECT DISTINCT CodigoTurma FROM ( " + QueryCompletaCodigosTurmas(tiposTurma != null && tiposTurma.Any());

            var tipos = string.Join(',', tiposTurma);
            var alunos = string.Join(',', codigoAlunos.Distinct().Select(c => Convert.ToInt32(c)).ToArray());

            var parametros = new
            {
                anoLetivo,
                alunos,
                data = dataReferencia ?? DateTime.Today,
                ueCodigo = ueCodigo?.ToDbChar(DapperConstants.CODIGOUE_LENGTH)
            };

            query = query.Replace("@tiposTurmaNormalizado", tipos);
            query = query.Replace("@alunos", alunos);

            if (!string.IsNullOrWhiteSpace(ueCodigo) && !consideraHistorico)
                query = string.Concat(query, " and turesc.cd_escola = @ueCodigo");

            query += ") AS CONSULTA ";

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<int>(query.ToString(), parametros);
                return result;
            }
        }

        public async Task<IEnumerable<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto>> BuscarCodigosTurmasAlunosPorAnoLetivoAluno(int anoLetivo, string[] codigoAlunos, IEnumerable<int> tiposTurma, bool consideraHistorico = false, DateTime? dataReferencia = null, string ueCodigo = null)
        {
            var query = "SELECT CodigoTurma, CodigoAluno FROM ( " + QueryCompletaCodigosTurmas(tiposTurma != null && tiposTurma.Any());

            var tipos = string.Join(',', tiposTurma);
            var alunos = string.Join(',', codigoAlunos.Distinct().Select(c => Convert.ToInt32(c)).ToArray());

            var parametros = new
            {
                anoLetivo,
                alunos,
                data = dataReferencia ?? DateTime.Today,
                ueCodigo = ueCodigo?.ToDbChar(DapperConstants.CODIGOUE_LENGTH)
            };

            query = query.Replace("@tiposTurmaNormalizado", tipos);
            query = query.Replace("@alunos", alunos);

            if (!string.IsNullOrWhiteSpace(ueCodigo) && !consideraHistorico)
                query = string.Concat(query, " and turesc.cd_escola = @ueCodigo");

            query += ") AS CONSULTA ";

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto>(query.ToString(), parametros);
                return result;
            }
        }

        private string QueryCompletaCodigosTurmas(bool consideraTiposTurma = true, string ueCodigo = null)
        {
            var query = new StringBuilder($@"
                       SELECT CodigoTurma, cd_aluno CodigoAluno FROM (
                       SELECT DISTINCT turesc.cd_turma_escola as CodigoTurma,
                       aluno.cd_aluno,
                       ROW_NUMBER() OVER (PARTITION BY matrTurma.cd_matricula ORDER BY matrTurma.dt_situacao_aluno desc) as sequencia
                       FROM turma_escola turesc
                       INNER JOIN historico_matricula_turma_escola matrTurma ON matrTurma.cd_turma_escola = turesc.cd_turma_escola
                       INNER JOIN v_historico_matricula_cotic matricula ON matricula.cd_matricula = matrTurma.cd_matricula
                       INNER JOIN v_aluno_cotic aluno ON matricula.cd_aluno = aluno.cd_aluno              
                       WHERE aluno.cd_aluno in (@alunos)
                         {(consideraTiposTurma ? "and turesc.cd_tipo_turma IN (@tiposTurmaNormalizado)" : string.Empty)}
                         and turesc.an_letivo = @anoLetivo
                         and ((matrTurma.cd_situacao_aluno in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno <= @data)
                           or (matrTurma.cd_situacao_aluno not in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno > @data))
                        {(string.IsNullOrEmpty(ueCodigo) ? string.Empty : " and turesc.cd_escola = @ueCodigo")}
                        ) as turmas
                        WHERE sequencia = 1 ");
            query.AppendLine(" union all ");
            query.AppendLine(QueryCompletaCodigosTurmasAnoAtual(consideraTiposTurma));
            return query.ToString();
        }

        private string QueryCompletaCodigosTurmasAnoAtual(bool consideraTiposTurma = true)
        {
            return $@"SELECT distinct turesc.cd_turma_escola as CodigoTurma, aluno.cd_aluno
                        FROM turma_escola turesc
                       INNER JOIN matricula_turma_escola matrTurma ON turesc.cd_turma_escola = matrTurma.cd_turma_escola
                       INNER JOIN v_matricula_cotic matricula ON matrTurma.cd_matricula = matricula.cd_matricula
                       INNER JOIN v_aluno_cotic aluno ON matricula.cd_aluno = aluno.cd_aluno
                       INNER JOIN alunos_matriculas_norm amn on amn.CodigoAluno = aluno.cd_aluno and amn.CodigoMatricula = matrTurma.cd_matricula 
                       WHERE aluno.cd_aluno in (@alunos)
                         {(consideraTiposTurma ? "and turesc.cd_tipo_turma IN (@tiposTurmaNormalizado)" : string.Empty)}
                         and turesc.an_letivo = @anoLetivo
                         and ((matrTurma.cd_situacao_aluno in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno <= @data) 
                           or (matrTurma.cd_situacao_aluno not in (1, 6, 10, 13, 5) and matrTurma.dt_situacao_aluno > @data))";
        }
    }
}
