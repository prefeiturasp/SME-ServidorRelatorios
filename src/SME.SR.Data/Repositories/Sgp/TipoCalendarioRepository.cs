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
            var parametros = new { AnoLetivo = anoLetivo, Modalidade = (int)modalidade, semestre };
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
            query.AppendLine(TipoCalendarioConsultas.ObterFiltroSemestre(modalidade, semestre));
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(query.ToString(), new { anoLetivo, modalidade = (int)modalidade, semestre });
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
            query.AppendLine(TipoCalendarioConsultas.ObterFiltroSemestre(turma.ModalidadeTipoCalendario, turma.Semestre));
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<TipoCalendario>(query.ToString(), new { turma.AnoLetivo, modalidade = (int)turma.ModalidadeTipoCalendario, turma.Semestre });
            }
        }
    }
}
