using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RelatorioControleLivrosRepository : IRelatorioControleLivrosRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioControleLivrosRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AcervoSolicitacaoDto>> ObterRelatorioControleLivrosEmpresados(long[] tiposAcervosPermitidos, 
            string solicitante, string tombo, SituacaoEmprestimo? situacaoEmprestimo, bool? somenteDevolvidos)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                                SELECT
	                                a.codigo as Tombo,
	                                a.Titulo,
	                                ae.situacao as SituacaoEmprestimo,
	                                ae.dt_emprestimo as DataEmprestimo,
	                                ae.dt_devolucao as DataDevolucao,
	                                u.nome as solicitante
	
                                FROM acervo_solicitacao_item asi
                                    JOIN acervo_solicitacao aso ON aso.id = asi.acervo_solicitacao_id
                                    JOIN acervo a ON a.id = asi.acervo_id
                                    JOIN usuario u ON u.id = aso.usuario_id
                                    JOIN acervo_emprestimo ae ON ae.acervo_solicitacao_item_id = asi.id AND NOT ae.excluido 
    
                                WHERE
                                    NOT asi.excluido
                                    AND NOT aso.excluido
                                    AND NOT a.excluido
                                    AND NOT u.excluido
                                    AND a.tipo = ANY(@tiposAcervosPermitidos)
                            ");

            var parametros = new DynamicParameters();
            parametros.Add("tiposAcervosPermitidos", tiposAcervosPermitidos);

            AdicionarFiltroSolicitante(query, parametros, solicitante);
            AdicionarFiltroTombo(query, parametros, tombo);
            AdicionarFiltroSituacao(query, parametros, situacaoEmprestimo, somenteDevolvidos);

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringCDEP);
            return await conexao.QueryAsync<AcervoSolicitacaoDto>(query.ToString(), parametros);
        }

        private void AdicionarFiltroSolicitante(StringBuilder query, DynamicParameters parametros, string? solicitante)
        {
            if (!string.IsNullOrWhiteSpace(solicitante))
            {
                query.AppendLine(" AND u.login = @Solicitante");
                parametros.Add("Solicitante", $"{solicitante}");
            }
        }

        private void AdicionarFiltroTombo(StringBuilder query, DynamicParameters parametros, string? tombo)
        {
            if (!string.IsNullOrWhiteSpace(tombo))
            {
                query.AppendLine(" AND a.codigo ILIKE @Tombo");
                parametros.Add("Tombo", $"%{tombo}%");
            }
        }

        private void AdicionarFiltroSituacao(StringBuilder query, DynamicParameters parametros, SituacaoEmprestimo? situacaoEmprestimo, bool? somenteDevolvidos)
        {
            if (situacaoEmprestimo.HasValue && situacaoEmprestimo.Value > 0)
            {
                query.AppendLine(" AND ae.situacao = @SituacaoEmprestimo");
                parametros.Add("SituacaoEmprestimo", (int)situacaoEmprestimo.Value);
                return;
            }

            if (somenteDevolvidos.HasValue)
            {
                if (somenteDevolvidos.Value)
                {
                    query.AppendLine(" AND ae.situacao = @SituacaoDevolvido");
                    parametros.Add("SituacaoDevolvido", (int)SituacaoEmprestimo.DEVOLVIDO);
                }
                else
                {
                    query.AppendLine(" AND ae.situacao <> @SituacaoDevolvido");
                    parametros.Add("SituacaoDevolvido", (int)SituacaoEmprestimo.DEVOLVIDO);
                }
                return;
            }

            query.AppendLine(" AND ae.situacao <> @SituacaoDevolvido");
            parametros.Add("SituacaoDevolvido", (int)SituacaoEmprestimo.DEVOLVIDO);
        }

        public async Task<IEnumerable<ControleAcervoDTO>> ObterRelatorioControleAcervos(long[] tiposAcervosPermitidos, TipoAcervo? tipoAcervo, SituacaoAcervo? situacaoAcervo)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                                SELECT
                                    a.Tipo as TipoAcervo,
	                                a.Titulo,
	                                a.codigo as Tombo,
	                                ae.situacao as SituacaoEmprestimo
                                FROM acervo_solicitacao_item asi
                                    JOIN acervo_solicitacao aso ON aso.id = asi.acervo_solicitacao_id
                                    JOIN acervo a ON a.id = asi.acervo_id
                                    JOIN usuario u ON u.id = aso.usuario_id
                                    JOIN acervo_emprestimo ae ON ae.acervo_solicitacao_item_id = asi.id AND NOT ae.excluido 
                                WHERE
                                    NOT asi.excluido
                                    AND NOT aso.excluido
                                    AND NOT a.excluido
                                    AND NOT u.excluido
                                    AND a.tipo = ANY(@tiposAcervosPermitidos)
                            ");

            var parametros = new DynamicParameters();
            parametros.Add("tiposAcervosPermitidos", tiposAcervosPermitidos);

            if (situacaoAcervo.HasValue && situacaoAcervo.Value > 0)
            {
                query.AppendLine(" AND COALESCE(a.situacao, 1) = @Situacao");
                parametros.Add("Situacao", (int)situacaoAcervo.Value);
            }

            if (tipoAcervo.HasValue && tipoAcervo.Value > 0)
            {
                query.AppendLine(" AND a.Tipo = @TipoAcervo");
                parametros.Add("TipoAcervo", tipoAcervo);
            }

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringCDEP);
            return await conexao.QueryAsync<ControleAcervoDTO>(query.ToString(), parametros);
        }
    }
}
