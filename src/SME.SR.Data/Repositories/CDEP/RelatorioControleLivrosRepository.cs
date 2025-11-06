using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string solicitante, string tombo, List<SituacaoEmprestimo>? situacaoEmprestimo, bool? somenteDevolvidos)
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

        private void AdicionarFiltroSituacao(StringBuilder query, DynamicParameters parametros, List<SituacaoEmprestimo> situacaoEmprestimo, bool? somenteDevolvidos)
        {
            if (somenteDevolvidos.HasValue)
            {
                if (somenteDevolvidos.Value)
                {
                    query.AppendLine(" AND ae.situacao = @SituacaoDevolvido");
                    parametros.Add("SituacaoDevolvido", (int)SituacaoEmprestimo.DEVOLVIDO);
                    return;
                }
            }

            if (situacaoEmprestimo != null && situacaoEmprestimo.Any())
            {
                query.AppendLine(" AND ae.situacao = ANY(@SituacoesEmprestimo)");
                parametros.Add("SituacoesEmprestimo", situacaoEmprestimo.Select(s => (int)s).ToArray());
                return;
            }
        }

        public async Task<IEnumerable<ControleAcervoDTO>> ObterRelatorioControleAcervos(long[] tiposAcervosPermitidos, TipoAcervo? tipoAcervo, SituacaoAcervo? situacaoAcervo)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                                SELECT
                                    a.Tipo as TipoAcervo,
	                                a.Titulo,
	                                a.codigo as Tombo,
	                                a.situacao as SituacaoEmprestimo
                                FROM acervo a 
                                WHERE
                                    NOT a.excluido
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

        public async Task<IEnumerable<ControleEditoraDTO>> ObterRelatorioControleEditoras(List<int>? idEditoras)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                 SELECT e.nome as Editora,
                        coalesce(a.codigo_novo, a.codigo) as tombo,
                        a.titulo,
                        a.situacao as situacaoemprestimo
                 FROM acervo a
                 INNER JOIN acervo_bibliografico ab on ab.acervo_id = a.id
                 INNER JOIN editora e on e.id = ab.editora_id
                 WHERE e.excluido is not true
                 AND a.excluido is not true
            ");

            var parametros = new DynamicParameters();

            if (idEditoras != null && idEditoras.Any())
            {
                query.AppendLine(" AND ab.editora_id = ANY(@IdEditoras)");
                parametros.Add("IdEditoras", idEditoras);
            }

            query.AppendLine(" ORDER BY Editora");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringCDEP);
            return await conexao.QueryAsync<ControleEditoraDTO>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<ControleAcervoAutorDTO>> ObterRelatorioControleAcervosAutor(long[] tiposAcervosPermitidos, TipoAcervo? tipoAcervo, List<int> autores)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                                  SELECT
  	                                    ca.nome as Autor,
                                        a.Tipo as TipoAcervo,
                                        a.codigo as Tombo,
                                        a.Titulo  
                                    FROM acervo_solicitacao_item asi
                                        JOIN acervo_solicitacao aso ON aso.id = asi.acervo_solicitacao_id
                                        JOIN acervo a ON a.id = asi.acervo_id
                                        JOIN usuario u ON u.id = aso.usuario_id 
                                        LEFT JOIN acervo_credito_autor aca ON aca.acervo_id = a.id
	                                    LEFT JOIN credito_autor ca ON aca.credito_autor_id = ca.id
                                    WHERE
                                        NOT asi.excluido
                                        AND NOT aso.excluido
                                        AND NOT a.excluido
                                        AND NOT u.excluido	
                                    AND a.tipo = ANY(@tiposAcervosPermitidos)
                            ");

            var parametros = new DynamicParameters();
            parametros.Add("tiposAcervosPermitidos", tiposAcervosPermitidos);

            if (autores?.Count > 0)
            {
                query.AppendLine(" AND ca.Id = ANY(@autores)");
                parametros.Add("autores", autores);
            }

            if (tipoAcervo.HasValue && tipoAcervo.Value > 0)
            {
                query.AppendLine(" AND a.Tipo = @tipoAcervo");
                parametros.Add("tipoAcervo", tipoAcervo);
            }

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringCDEP);
            return await conexao.QueryAsync<ControleAcervoAutorDTO>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<AcervoDevolucaoDto>> ObterRelatorioControleDevolucaoLivros(long[] tiposAcervosPermitidos, string solicitante, bool? somenteAtrasados = false)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                                ;WITH ultimaMovimentacaoAcervos AS (
                                    SELECT DISTINCT ON (acervo_solicitacao_item_id)
                                        id,
                                        acervo_solicitacao_item_id,
                                        dt_emprestimo,
                                        dt_devolucao,
                                        situacao,
                                        criado_em,
                                        criado_por,
                                        criado_login,
                                        alterado_em,
                                        alterado_por,
                                        alterado_login
                                    FROM acervo_emprestimo
                                    WHERE NOT excluido
                                    ORDER BY acervo_solicitacao_item_id, id DESC
                                ),
                                movimentacoesComAtraso AS (
                                    SELECT
                                        ae.*,
                                        CASE
                                            WHEN ae.situacao = 4 THEN
                                                (
                                                    SELECT ae2.dt_devolucao::DATE
                                                    FROM acervo_emprestimo ae2
                                                    WHERE ae2.acervo_solicitacao_item_id = ae.acervo_solicitacao_item_id
                                                      AND ae2.situacao != 4
                                                    ORDER BY ae2.id DESC
                                                    LIMIT 1
                                                ) - ae.dt_devolucao::DATE
                                            ELSE
                                                (current_date - ae.dt_devolucao::DATE)
                                        END AS dias_atraso_bruto
                                    FROM ultimaMovimentacaoAcervos ae
                                )
                                SELECT
                                    u.nome AS solicitante,
                                    a.codigo AS Tombo,
                                    a.Titulo,
                                    u.Telefone,
                                    u.email,
                                    u.login,
                                    a.tipo,
                                    mca.dt_emprestimo AS DataEmprestimo,
                                    mca.dt_devolucao AS DataDevolucao,
                                    mca.situacao,
                                    CASE
                                        WHEN mca.dias_atraso_bruto < 0 THEN 0
                                        ELSE mca.dias_atraso_bruto
                                    END AS DiasAtraso
                                FROM movimentacoesComAtraso mca
                                    INNER JOIN acervo_solicitacao_item asi ON asi.id = mca.acervo_solicitacao_item_id
                                    INNER JOIN acervo_solicitacao aso ON aso.id = asi.acervo_solicitacao_id
                                    INNER JOIN usuario u ON u.id = aso.usuario_id
                                    INNER JOIN acervo a ON a.id = asi.acervo_id
                                WHERE
                                    NOT asi.excluido
                                    AND NOT aso.excluido
                                    AND NOT a.excluido
                                    AND NOT u.excluido");


            var parametros = new DynamicParameters();
            if (tiposAcervosPermitidos != null && tiposAcervosPermitidos.Any())
            {
                query.AppendLine("AND a.tipo = ANY(@tiposAcervosPermitidos)");
                parametros.Add("tiposAcervosPermitidos", tiposAcervosPermitidos);
            }
            if (!string.IsNullOrWhiteSpace(solicitante))
            {
                query.AppendLine("AND u.login = @solicitante");
                parametros.Add("solicitante", solicitante);
            }
            if (somenteAtrasados.HasValue && somenteAtrasados.Value)
            {
                query.AppendLine("AND mca.situacao = 2");
            }

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringCDEP);
            return await conexao.QueryAsync<AcervoDevolucaoDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<RelatorioTitulosMaisPesquisadosDto>> ObterRelatorioTitulosMaisPesquisados(DateTime dataInicio, DateTime dataFim, List<TipoAcervo> tiposAcervos)
        {
            var query = new StringBuilder();
            query.AppendLine(@"
                                SELECT TRIM(LOWER(unaccent(termo_pesquisado))) AS TermoNormalizado,
                                       COUNT(*) AS Quantidade
                                  FROM historico_consultas_acervos
                                 WHERE data_consulta between @dataInicio and @dataFim");

            if (tiposAcervos != null && tiposAcervos.Any())
            {
                query.AppendLine(" AND tipo_acervo = ANY(@tiposAcervos)");
            }
            query.AppendLine(@"
                              GROUP BY TermoNormalizado
                              ORDER BY Quantidade DESC, TermoNormalizado;");

            var parametros = new DynamicParameters();
            parametros.Add("dataInicio", dataInicio);
            parametros.Add("dataFim", dataFim);
            if (tiposAcervos != null && tiposAcervos.Any())
            {
                parametros.Add("tiposAcervos", tiposAcervos.Select(ta => (int)ta).ToArray());
            }
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringCDEP);
            return await conexao.QueryAsync<RelatorioTitulosMaisPesquisadosDto>(query.ToString(), parametros);
        }
    }
}
