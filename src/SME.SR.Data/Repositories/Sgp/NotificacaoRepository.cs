﻿using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            bool exibirDescricao = false, bool exibirExcluidas = false, string dreCodigo = "-99", string ueCodigo = "-99")
        {
            var query = new StringBuilder($@"select 
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
							,ue.nome as UeNome ");

            if (exibirDescricao)
                query.AppendLine(",n.mensagem as Mensagem ");
            query.AppendLine($@" from notificacao n 
								inner join usuario u  on n.usuario_id = u.id
								 left join dre on n.dre_id = dre.dre_id 
								 left join ue on n.ue_id = ue.ue_id 
								where ano = @ano ");
            if (tipos != null && tipos.Any())
                query.AppendLine(" and tipo = ANY(@tipos) ");
            if (categorias != null && categorias.Any())
                query.AppendLine(" and categoria = ANY(@categorias)");
            if (situacoes != null && situacoes.Any())
                query.AppendLine(" and n.status = ANY(@situacoes) ");
            if (!exibirExcluidas)
                query.AppendLine(" and n.excluida = false ");
            if (dreCodigo != "-99")
                query.AppendLine(" and n.dre_id = @dreCodigo ");
            if (ueCodigo != "-99")
                query.AppendLine($" and n.ue_id = @ueCodigo ");
            if (!string.IsNullOrEmpty(usuarioRf))
                query.AppendLine(" and u.rf_codigo = @usuarioRf ");

            query.AppendLine(" order by u.nome ");

            var parametros = new
            {
                ano,
                tipos,
                categorias,
                situacoes,
                dreCodigo,
                ueCodigo,
                usuarioRf
            };
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotificacaoDto>(query.ToString(), parametros);
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotificacaoDto>(query, new { ano });
            };
        }
    }
}
