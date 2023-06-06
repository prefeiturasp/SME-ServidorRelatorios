using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Text;
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
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 7, 1);

            var parametros = new { AnoLetivo = anoLetivo, Modalidade = (int)modalidade, DataReferencia = dataReferencia };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(query, parametros);
            }
        }

        public async Task<TipoCalendarioDto> ObterPorId(long id)
        {
            var query = "select tc.ano_letivo as anoLetivo, tc.id, tc.nome from tipo_calendario tc where tc.id = @id";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryFirstOrDefaultAsync<TipoCalendarioDto>(query, new { id });

        }

        public async Task<long> ObterIdPorAnoLetivoEModalidadeAsync(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre = 0)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select id");
            query.AppendLine("from tipo_calendario t");
            query.AppendLine("where t.excluido = false");
            query.AppendLine("and t.ano_letivo = @anoLetivo");
            query.AppendLine("and t.modalidade = @modalidade");
            query.AppendLine("and t.situacao ");

            DateTime dataReferencia = DateTime.MinValue;
            if (modalidade == ModalidadeTipoCalendario.EJA)
            {
                var periodoReferencia = semestre == 1 ? "periodo_inicio < @dataReferencia" : "periodo_fim > @dataReferencia";
                query.AppendLine($"and exists(select 0 from periodo_escolar p where tipo_calendario_id = t.id and {periodoReferencia})");

                // 1/6/ano ou 1/7/ano dependendo do semestre
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 8, 1);
            }

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(query.ToString(), new { anoLetivo, modalidade = (int)modalidade, dataReferencia });
            }
        }

        public async Task<TipoCalendario> ObterPorTurma(Turma turma)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select *");
            query.AppendLine("from tipo_calendario t");
            query.AppendLine("where t.excluido = false");
            query.AppendLine("and t.ano_letivo = @anoLetivo");
            query.AppendLine("and t.modalidade = @modalidade");

            DateTime dataReferencia = DateTime.MinValue;
            if (turma.ModalidadeTipoCalendario == ModalidadeTipoCalendario.EJA)
            {
                var periodoReferencia = turma.Semestre == 1 ? "periodo_inicio < @dataReferencia" : "periodo_fim > @dataReferencia";
                query.AppendLine($"and exists(select 0 from periodo_escolar p where tipo_calendario_id = t.id and {periodoReferencia})");

                dataReferencia = new DateTime(turma.AnoLetivo, turma.Semestre == 1 ? 6 : 8, 1);
            }
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<TipoCalendario>(query.ToString(), new { turma.AnoLetivo, modalidade = (int)turma.ModalidadeTipoCalendario, dataReferencia });
            }
        }
    }
}
