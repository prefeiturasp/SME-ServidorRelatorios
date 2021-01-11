using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class TipoCalendarioRepository : ITipoCalendarioRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public TipoCalendarioRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<long> ObterPorAnoLetivoEModalidade(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre = 0)
        {
            var query = TipoCalendarioConsultas.ObterPorAnoLetivoEModalidade(modalidade, semestre);

            DateTime dataReferencia = DateTime.MinValue;
            if (modalidade == ModalidadeTipoCalendario.EJA)
            {
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 7, 1);
            }

            var parametros = new { AnoLetivo = anoLetivo, Modalidade = (int)modalidade, DataReferencia = dataReferencia };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(query, parametros);
            }
        }

        public async Task<TipoCalendarioDto> ObterPorId(long id)
        {
            var query = "select tc.ano_letivo as anoLetivo, tc.id, tc.nome from tipo_calendario tc where tc.id = @id";
                        
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<TipoCalendarioDto>(query, new { id });           

        }
    }
}
