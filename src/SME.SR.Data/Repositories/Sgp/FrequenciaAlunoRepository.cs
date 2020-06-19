using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class FrequenciaAlunoRepository : IFrequenciaAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FrequenciaAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<FrequenciaAluno> ObterPorAlunoDataDisciplina(string codigoAluno, DateTime dataAtual, TipoFrequenciaAluno tipoFrequencia, string disciplinaId)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoDataDisciplina;
            var parametros = new
            {
                CodigoAluno = codigoAluno,
                DataAtual = dataAtual,
                TipoFrequencia = tipoFrequencia,
                DisciplinaId = disciplinaId
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaGlobal;
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<double>(query, parametros);

        }

        public async Task<FrequenciaAluno> ObterPorAlunoBimestreAsync(string codigoAluno, int bimestre, TipoFrequenciaAluno tipoFrequencia, string disciplinaId)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoBimestreDisciplina;
            var parametros = new
            {
                CodigoAluno = codigoAluno,
                Bimestre = bimestre,
                TipoFrequencia = tipoFrequencia,
                DisciplinaId = disciplinaId
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<FrequenciaAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorDisciplinaBimestres(string codigoTurma, string codigoAluno, int? bimestre)
        {
            var query = FrequenciaAlunoConsultas.FrequenciaPorAlunoTurmaBimestre(bimestre);
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno, Bimestre = bimestre };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<FrequenciaAluno>(query, parametros);
            }
        }
    }
}
