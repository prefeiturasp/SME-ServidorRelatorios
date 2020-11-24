using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public NotificacaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotificacaoDto>> ObterComFiltros(long ano, string usuarioRf, long[] categorias, long[] tipos, long[] situacoes, 
			bool exibirDescricao = false, bool exibirExcluidas = false, string dre = "-99", string ue = "-99")
        {
			var query = $@"select 
							n.codigo as Codigo
							,titulo
							,categoria
							,tipo
							,status as Situacao
							,u.nome as UsuarioNome
							,u.rf_codigo as UsuarioRf
							,n.criado_em as DataRecebimento
							,n.alterado_em as DataLeitura
							,dre.id as DreId
							,dre.nome as DreNome
							,ue.id as UeId
							,ue.nome as UeNome";

			if (exibirDescricao)
				query += ",n.mensagem as Mensagem";
			query += $@" from notificacao n 
						inner join usuario u  on n.usuario_id = u.id
						left join dre on n.dre_id = dre.dre_id 
						left join ue on n.ue_id = ue.ue_id 
						where (ano = @ano or ano = 0) 
						and tipo = ANY(@tipos) 
						and categoria = ANY(@categorias)
						and n.status = ANY(@situacoes) ";
			if (!exibirExcluidas)
				query += " and n.excluida = false";
			if (dre != "-99")
				query += $" and (n.dre_id = '{dre}' or n.dre_id = null or n.dre_id = '')";
			if (ue != "-99")
				query += $" and (n.ue_id = '{ue}' or n.ue_id = null or n.ue_id = '')";
			if (!String.IsNullOrEmpty(usuarioRf))
				query += $" and u.rf_codigo = '{usuarioRf}'";

			query += " order by u.nome";

			using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
			{
				return await conexao.QueryAsync<NotificacaoDto>(query, new { ano, tipos, categorias, situacoes });
			};
		}

        public async Task<IEnumerable<NotificacaoDto>> ObterPorAnoEUsuarioRf(long ano, string usuarioRf)
		{
            var query = $@"select 
							n.id as Codigo,
							titulo,
							categoria,
							tipo,
							status,
							u.nome,
							u.rf_codigo ,
							n.criado_em as DataRecebimento,
							n.alterado_em as DataLeitura,
							dre.id as DreId,
							dre.nome as DreNome,
							ue.id as UeId,
							ue.nome as UeNome	
						from notificacao n 
						inner join usuario u  on n.usuario_id = u.id
						inner join dre dre on n.dre_id = dre.dre_id 
						left join ue on dre.id = ue.dre_id 
						where ano = @ano";
			if (usuarioRf != "")
				query += $" and u.rf_codigo = '{usuarioRf}'";

			using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
			{
				return await conexao.QueryAsync<NotificacaoDto>(query, new { ano });
			};
		}
    }
}
