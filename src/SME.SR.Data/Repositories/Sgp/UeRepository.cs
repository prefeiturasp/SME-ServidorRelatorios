using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class UeRepository : IUeRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UeRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Ue> ObterPorCodigo(string UeCodigo)
        {
            var query = @"select Id, ue_id Codigo, Nome, tipo_escola TipoEscola from ue where ue_id = @ueCodigo";
            var parametros = new { UeCodigo };
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<Ue>(query, parametros);
        }

        public async Task<IEnumerable<UePorDresIdResultDto>> ObterPorDresId(long[] dreIds)
        {
            var query = @"select Id, ue.dre_id as dreId, ue_id Codigo, Nome, tipo_escola TipoEscola from ue ";
            if (dreIds != null && dreIds.Any())
                query = query += "where ue.dre_id = ANY(@dreIds) ";

            var parametros = new { dreIds = dreIds.ToList() };
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<UePorDresIdResultDto>(query, parametros);
        }

        public async Task<IEnumerable<Ue>> ObterPorDreSemestreModadalidadeAnoId(long dreId, int? semestre, int modalidadeId, string[] anos)
        {
            var query = new StringBuilder(@"select distinct u.Id, u.ue_id Codigo, u.Nome, u.tipo_escola TipoEscola 
                            from ue u 
                            inner join turma t on t.ue_id  = u.id
                            where u.dre_id = @dreId ");

            if (semestre.HasValue)
                query.AppendLine("and t.semestre = @semestre");

            if (modalidadeId > 0)
                query.AppendLine("and t.modalidade_codigo = @modalidadeId");

            if (anos != null && anos.Length > 0)
                query.AppendLine("and t.ano = ANY(@anos)");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<Ue>(query.ToString(), new
            {
                dreId,
                semestre = semestre ?? 0,
                modalidadeId,
                anos = anos.ToList()
            });

        }

        public async Task<IEnumerable<Ue>> ObterPorCodigos(string[] ueCodigos)
        {
            var query = @"select ue.Id, ue.ue_id Codigo, ue.Nome, ue.tipo_escola TipoEscola,
                          dre.id Id, dre.dre_id Codigo, dre.abreviacao, dre.nome
                          from ue
                          inner join dre on ue.dre_id = dre.id
                          where ue_id = any(@ueCodigos)";

            var parametros = new { ueCodigos };
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return (await conexao.QueryAsync<Ue, Dre, Ue>(query, (ue, dre) =>
            {
                ue.Dre = dre;
                return ue;
            }
            , parametros, splitOn: "Id,Id"));
        }

        public async Task<Ue> ObterPorId(long ueId)
        {
            var query = @"select Id, ue_id Codigo, Nome, tipo_escola TipoEscola, dre_id DreId from ue where id = @ueId";
            var parametros = new { ueId };
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<Ue>(query, parametros);
        }

        public async Task<Ue> ObterUeComDrePorId(long ueId)
        {
            var query = @"select
	                        ue.ue_id as UeCodigo,
	                        ue.tipo_escola as TipoEscola,
	                        ue.*,
	                        dre.*,
	                        dre.dre_id as DreCodigo
                        from
	                        ue
                        inner join dre on
	                        dre.id = ue.dre_id
                        where
	                        ue.id = @ueId";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return (await conexao.QueryAsync<Ue, Dre, Ue>(query, (ue, dre) =>
            {
                ue.AdicionarDre(dre);
                return ue;
            },
            new { ueId })).FirstOrDefault();

        }
        public async Task<Ue> ObterUeComDrePorCodigo(string codigoUe)
        {
            var query = @"select
	                        ue.ue_id as UeCodigo,
	                        ue.tipo_escola as TipoEscola,
	                        ue.*,
	                        dre.*,
	                        dre.dre_id as DreCodigo
                        from
	                        ue
                        inner join dre on
	                        dre.id = ue.dre_id
                        where
	                        ue.ue_id = @codigoUe";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return (await conexao.QueryAsync<Ue, Dre, Ue>(query, (ue, dre) =>
                {
                    ue.AdicionarDre(dre);
                    return ue;
                },
                new { codigoUe })).FirstOrDefault();

        }
    }
}
