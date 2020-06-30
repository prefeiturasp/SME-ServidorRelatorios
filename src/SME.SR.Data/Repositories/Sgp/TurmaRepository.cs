using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public TurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<string> ObterCicloAprendizagem(string turmaCodigo)
        {
            var query = TurmaConsultas.CicloAprendizagemPorTurma;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                var ciclo = await conexao.QueryFirstOrDefaultAsync<string>(query, new { turmaCodigo });
                await conexao.CloseAsync();

                return ciclo;
            }
        }

        public async Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma)
        {
            var query = TurmaConsultas.DadosAlunos;
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Aluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<AlunoSituacaoDto>> ObterDadosAlunosSituacao(string turmaCodigo)
        {
            var query = TurmaConsultas.DadosAlunosSituacao;

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<AlunoSituacaoDto>(query, new { turmaCodigo });
            }
        }

        public async Task<DreUe> ObterDreUe(string codigoTurma)
        {
            var query = TurmaConsultas.DadosCompletosDreUe;
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<DreUe>(query, parametros);
            }
        }

        public async Task<Turma> ObterPorCodigo(string codigoTurma)
        {
            var query = TurmaConsultas.TurmaPorCodigo;

            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<Turma>(query, parametros);
            }
        }

        public async Task<IEnumerable<Turma>> ObterPorFiltros(string codigoUe, Modalidade? modalidade, int? anoLetivo, int? semestre)
        {
            var query = TurmaConsultas.TurmaPorUe(modalidade, anoLetivo, semestre);

            var parametros = new
            {
                CodigoUe = codigoUe,
                Modalidade = modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<Turma>(query, parametros);
            }


        }
        public async Task<IEnumerable<Turma>> ObterPorAbrangenciaFiltros(string codigoUe, Modalidade? modalidade, int? anoLetivo, string login, Guid perfil, bool consideraHistorico, int? semestre)
        {
            try
            {
                
                var query = TurmaConsultas.TurmaPorAbrangenciaFiltros;                

                var parametros = new
                {
                    CodigoUe = codigoUe,
                    Modalidade = (int)modalidade,
                    AnoLetivo = anoLetivo,
                    Semestre = semestre,
                    Login = login,
                    Perfil = perfil,
                    ConsideraHistorico = consideraHistorico
                };

                using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
                {
                    return await conexao.QueryAsync<Turma>(query, parametros);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
