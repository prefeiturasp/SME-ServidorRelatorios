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
    public class AtribuicaoCJRepository : IAtribuicaoCJRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AtribuicaoCJRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AtribuicaoCJ>> ObterPorFiltros(Modalidade? modalidade, string turmaId, string ueId, long componenteCurricularId, string usuarioRf, string usuarioNome, bool? substituir, string dreCodigo = "", string[] turmaIds = null, int? anoLetivo = null)
        {
            var query = AtribuicaoCJConsultas.ObterPorFiltros(modalidade, turmaId, ueId, componenteCurricularId, 
                usuarioRf, usuarioNome, substituir, dreCodigo, turmaIds,anoLetivo);

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<AtribuicaoCJ, Turma, AtribuicaoCJ>(query.ToString(), (atribuicaoCJ, turma) =>
                {
                    atribuicaoCJ.Turma = turma;
                    return atribuicaoCJ;
                }, new
                {
                    modalidade = modalidade.HasValue ? (int)modalidade : 0,
                    ueId,
                    turmaId,
                    componenteCurricularId,
                    usuarioRf,
                    usuarioNome,
                    substituir,
                    dreCodigo,
                    turmaIds,
                    anoLetivo
                }, splitOn: "id,id"));
            }
        }
    }
}
