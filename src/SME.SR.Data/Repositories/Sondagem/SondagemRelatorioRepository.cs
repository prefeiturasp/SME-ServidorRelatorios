using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.Sondagem;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SME.SR.Data.Repositories.Sondagem
{
    public class SondagemRelatorioRepository : ISondagemRelatorioRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public const int QUARTO_ANO = 4;
        public const int NONO_ANO = 9;
        public const int QUARTO_BIMESTRE = 4;

        public SondagemRelatorioRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PerguntaRespostaOrdemDto>> ConsolidacaoCampoAditivoMultiplicativo(RelatorioMatematicaFiltroDto filtro)
        {
            var retorno = new List<PerguntaRespostaOrdemDto>();
            var filtroUeDre = "";
            if (!string.IsNullOrEmpty(filtro.CodigoDre) && filtro.CodigoDre != "-99")
                filtroUeDre += @" AND s.""CodigoDre"" =  @CodigoDRE";
            if (!string.IsNullOrEmpty(filtro.CodigoUe) && filtro.CodigoUe != "-99")
                filtroUeDre += @" AND s.""CodigoUe"" =  @CodigoUe";

            var query = @$"	SELECT      pae.""AnoEscolar"" as ""AnoTurma"",
                                    pae.""Ordenacao"" AS ""OrdemPergunta"",
                                    ppai.""Id"" AS ""PerguntaId"",
                                    ppai.""Descricao"" AS ""PerguntaDescricao"",
                                    pfilho.""Id"" AS ""SubPerguntaId"",
                                    pfilho.""Descricao"" AS ""SubPerguntaDescricao"",
                                    pr.""Ordenacao"" AS ""OrdemResposta"",
                                    r.""Id"" AS ""RespostaId"",
                                    r.""Descricao"" AS ""RespostaDescricao"",
                                    tabela.""QtdRespostas"",
                                    tabela.""CodigoDre"",
			                        tabela.""CodigoUe"", 	
			                        tabela.""CodigoTurma""
                        FROM ""PerguntaAnoEscolar"" pae
                        INNER JOIN ""Pergunta"" ppai ON ppai.""Id"" = pae.""PerguntaId"" 
                        INNER JOIN ""Pergunta"" pfilho ON pfilho.""PerguntaId"" = pae.""PerguntaId""
                        INNER JOIN ""PerguntaResposta"" pr ON pr.""PerguntaId"" = pfilho.""Id""
                        LEFT JOIN ""Resposta"" r ON r.""Id"" = pr.""RespostaId""
                        {(filtro.Bimestre == QUARTO_BIMESTRE ? 
                            @$" LEFT JOIN  ""PerguntaAnoEscolarBimestre"" paeb  on pae.""Id"" = paeb.""PerguntaAnoEscolarId"" and pae.""AnoEscolar"" >= {QUARTO_ANO} and pae.""AnoEscolar"" <= {NONO_ANO}" : "") }
                        LEFT JOIN ( SELECT p.""Id"" AS ""PerguntaId"",
                                            r.""Id"" AS ""RespostaId"", COUNT(1) AS ""QtdRespostas"",
                                            s.""CodigoDre"", s.""CodigoUe"", s.""AnoTurma"", s.""CodigoTurma""
                                    FROM ""SondagemAlunoRespostas"" sar
                                    INNER JOIN ""SondagemAluno"" sa ON sa.""Id"" = sar.""SondagemAlunoId""
                                    INNER JOIN ""Sondagem"" s ON s.""Id"" = sa.""SondagemId""
                                    INNER JOIN ""Pergunta"" p ON p.""Id"" = sar.""PerguntaId""
                                    INNER JOIN ""Resposta"" r ON r.""Id"" = sar.""RespostaId""
                                    WHERE s.""ComponenteCurricularId"" = @ComponenteCurricularId
                                        AND s.""AnoLetivo"" = @AnoLetivo
                                        AND s.""Bimestre"" = @Bimestre
                                    {filtroUeDre}    
                                    GROUP BY p.""Id"", r.""Id"", s.""CodigoDre"", s.""CodigoUe"", s.""AnoTurma"", s.""CodigoTurma"") 
                                AS tabela ON pfilho.""Id"" = tabela.""PerguntaId"" AND r.""Id"" = tabela.""RespostaId"" AND pae.""AnoEscolar"" = tabela.""AnoTurma""
                        WHERE pae.""Grupo"" = @Grupo
                        AND ((pae.""FimVigencia"" IS NULL AND EXTRACT (YEAR FROM pae.""InicioVigencia"") <= @AnoLetivo)
                        OR (EXTRACT(YEAR FROM pae.""FimVigencia"") >= @AnoLetivo AND EXTRACT (YEAR FROM pae.""InicioVigencia"") <= @AnoLetivo))
                        {(filtro.Bimestre == QUARTO_BIMESTRE ? $@" AND (paeb.""Id"" is null or paeb.""Bimestre"" = {QUARTO_BIMESTRE})" : "")}
                        ORDER BY tabela.""CodigoDre"", tabela.""CodigoUe"", tabela.""CodigoTurma"", pae.""AnoEscolar"", pae.""Ordenacao"",  pfilho.""Id"", pr.""Ordenacao"" ";

            var parametros = new
            {
                filtro.CodigoUe,
                filtro.CodigoDre,
                filtro.AnoLetivo,
                filtro.Bimestre,
                filtro.ComponenteCurricularId,
                Grupo = filtro.Proficiencia
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                var consulta = await conexao.QueryAsync<PerguntaRespostaOrdemDto>(query.ToString(), parametros);
                if (consulta != null)
                    retorno = consulta.ToList();
            }
            return retorno;
        }

        public async Task<IEnumerable<OrdemPerguntaRespostaDto>> ConsolidadoCapacidadeLeitura(RelatorioPortuguesFiltroDto filtro)
        {
            var query = new StringBuilder();
            var retorno = new List<OrdemPerguntaRespostaDto>();
            var queryRelatorio = @"
                                   select tabela.""CodigoTurma"",
                                   tabela.""CodigoDre"",tabela.""CodigoUe"",
                                   o.""Id"" as ""OrdermId"",
                                   o.""Descricao"" as ""Ordem"",
                                   p.""Id"" as ""PerguntaId"",
                                   p.""Descricao"" as ""PerguntaDescricao"",
                                   r.""Id"" as ""RespostaId"", 
                                   r.""Descricao"" as ""RespostaDescricao"" ,
                                   gp.""Ordenacao"",
                                   tabela.""AnoTurma"",
                                   op.""OrdenacaoNaTela"",
                                   count(tabela.""RespostaId"") as ""QtdRespostas""
                                   from
                                       ""Ordem""  as o 
                                      inner join ""GrupoOrdem"" gp on 
                                      gp.""OrdemId"" = o.""Id"" and 
                                      gp.""GrupoId"" = @GrupoId
                                      inner join ""OrdemPergunta"" op on
                                       op.""GrupoId"" = @GrupoId
                                      inner join ""Pergunta"" p on
                                   	 p.""Id"" = op.""PerguntaId"" 
                                   inner join ""PerguntaResposta"" pr on
                                   	pr.""PerguntaId"" = p.""Id""
                                   inner join ""Resposta"" r on
                                   	r.""Id"" = pr.""RespostaId""
                                   left join (
                                   	select
                                   	    s.""OrdemId"",
                                   		s.""AnoLetivo"",		                                s.""CodigoDre"",
		                                s.""CodigoUe"",
                                   		s.""AnoTurma"",
                                   		s.""CodigoTurma"",

                                   		per.""Descricao"",
                                   		c.""Descricao"",
                                   		sa.""NomeAluno"",
                                   		p.""Id"" as ""PerguntaId"",
                                   		p.""Descricao"" as ""PerguntaDescricao"",
                                   		r.""Id"" as ""RespostaId"",
                                   		r.""Descricao"" as ""RespostaDescricao""
                                   	from
                                   		""SondagemAlunoRespostas"" sar
                                   	inner join ""SondagemAluno"" sa on
                                   		sa.""Id"" = ""SondagemAlunoId""
                                   	inner join ""Sondagem"" s on
                                   		s.""Id"" = sa.""SondagemId""
                                   	inner join ""Pergunta"" p on
                                   		p.""Id"" = sar.""PerguntaId""
                                   	inner join ""Resposta"" r on
                                   		r.""Id"" = sar.""RespostaId""
                                   	inner join ""Periodo"" per on
                                   		per.""Id"" = s.""PeriodoId""
                                   	inner join ""ComponenteCurricular"" c on
                                   		c.""Id"" = s.""ComponenteCurricularId""
                                       
                                   	where
                                   		s.""Id"" in (
                                   		select
                                   			s.""Id""
                                   		from
                                   			""Sondagem"" s
                                   		where
                                   		        s.""GrupoId"" = @GrupoId
                                   		    and s.""ComponenteCurricularId"" = @ComponenteCurricularId";
            query.Append(queryRelatorio);
            if (!string.IsNullOrEmpty(filtro.CodigoDre))
	            query.AppendLine(@" and ""CodigoDre"" =  @CodigoDRE");
            if (!string.IsNullOrEmpty(filtro.CodigoUe))
	            query.AppendLine(@"and ""CodigoUe"" =  @CodigoEscola");
                                        
            query.Append(@"  and s.""PeriodoId"" = @PeriodoId
                                   			and s.""AnoLetivo"" = @AnoLetivo
                                   		        ) ) as tabela on
                                          	p.""Id"" = tabela.""PerguntaId"" and
                                          	r.""Id""= tabela.""RespostaId"" and
                                              o.""Id"" = tabela.""OrdemId""
                                          group by
                                          	o.""Id"",
                                              o.""Descricao"",
                                              r.""Id"",
                                          	r.""Descricao"",
                                          	p.""Id"",
                                          	p.""Descricao"",
                                          	gp.""Ordenacao"",
                                            op.""OrdenacaoNaTela"",tabela.""CodigoTurma"",tabela.""AnoTurma"",tabela.""CodigoDre"",tabela.""CodigoUe""
                                          order by
                                             gp.""Ordenacao"",
                                             o.""Descricao"",
                                             op.""OrdenacaoNaTela"",
                                              r.""Descricao""");

            var parametros = new
            {
	            CodigoEscola = filtro.CodigoUe,
	            CodigoDRE = filtro.CodigoDre,
	            AnoLetivo = filtro.AnoLetivo,
	            PeriodoId = filtro.PeriodoId,
	            filtro.ComponenteCurricularId,
	            GrupoId = filtro.GrupoId

            };
            
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
	            var consulta = await conexao.QueryAsync<OrdemPerguntaRespostaDto>(query.ToString(), parametros);
	            if (consulta != null)
		            retorno = consulta.ToList();
            }
            return retorno;
        }

        public async Task<IEnumerable<PerguntaRespostaProducaoTextoDto>> ObterDadosProducaoTexto(RelatorioPortuguesFiltroDto filtro)
        {
	        var retorno = new List<PerguntaRespostaProducaoTextoDto>();
	        var sql = new StringBuilder();

	        sql.AppendLine("select ");
	        sql.AppendLine("	s.\"CodigoTurma\",");
	        sql.AppendLine("	s.\"AnoTurma\",");
	        sql.AppendLine("	s.\"CodigoUe\",");
	        sql.AppendLine("	s.\"CodigoDre\",");
	        sql.AppendLine("	g.\"Descricao\" as \"Grupo\" ,");
	        sql.AppendLine("	p.\"Descricao\" as \"Pergunta\" ,");
	        sql.AppendLine("	r.\"Descricao\"  as \"Resposta\",");
	        sql.AppendLine("    sa.\"CodigoAluno\" ");
	        sql.AppendLine("from \"Sondagem\" s");
	        sql.AppendLine("inner join \"SondagemAluno\" sa on sa.\"SondagemId\" = s.\"Id\"");
	        sql.AppendLine("inner join \"SondagemAlunoRespostas\" sar on sar.\"SondagemAlunoId\" = sa.\"Id\"");
	        sql.AppendLine("inner join \"Pergunta\" p on sar.\"PerguntaId\" = p.\"Id\" ");
	        sql.AppendLine("inner join \"Resposta\" r on sar.\"RespostaId\" = r.\"Id\" ");
	        sql.AppendLine("inner join \"Grupo\" g on s.\"GrupoId\" = g.\"Id\" ");
	        sql.AppendLine("where s.\"AnoLetivo\" = @AnoLetivo ");
	        sql.AppendLine("and s.\"ComponenteCurricularId\" = @ComponenteCurricularId ");
	        sql.AppendLine("and s.\"PeriodoId\" = @PeriodoId ");
	        sql.AppendLine("and s.\"GrupoId\" = @GrupoId ");
            if(filtro.CodigoUe != "-99")
	            sql.AppendLine(" and s.\"CodigoUe\" = @CodigoEscola ");
            if (filtro.CodigoDre != "-99")
                sql.AppendLine(" and s.\"CodigoDre\" = @CodigoDRE ");

	        var parametros = new
	        {
		        CodigoEscola = filtro.CodigoUe,
		        CodigoDRE = filtro.CodigoDre,
		        AnoLetivo = filtro.AnoLetivo,
		        PeriodoId = filtro.PeriodoId,
		        filtro.ComponenteCurricularId,
		        GrupoId = filtro.GrupoId

	        };
	        using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
	        {
		        var consulta = await conexao.QueryAsync<PerguntaRespostaProducaoTextoDto>(sql.ToString(), parametros);
		        if (consulta != null)
			        retorno = consulta.ToList();
	        }

	        return retorno;

        }

        public async Task<IEnumerable<PerguntaRelatorioMatematicaNumerosDto>> ObterPerguntasMatematicaNumeros(RelatorioPortuguesFiltroDto filtro)
        {
            var listaRetorno = new List<PerguntaRelatorioMatematicaNumerosDto>();
            var sql = new StringBuilder();
            var parametros = new { };
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                var consulta = await conexao.QueryAsync<PerguntaRelatorioMatematicaNumerosDto>(sql.ToString(), parametros);
                if (consulta != null)
                    listaRetorno = consulta.ToList();
            }
            return listaRetorno;
        }
    }
}