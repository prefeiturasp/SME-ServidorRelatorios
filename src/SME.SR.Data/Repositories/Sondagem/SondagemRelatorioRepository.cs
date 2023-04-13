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
                            @$" LEFT JOIN  ""PerguntaAnoEscolarBimestre"" paeb  on pae.""Id"" = paeb.""PerguntaAnoEscolarId"" and pae.""AnoEscolar"" >= {QUARTO_ANO} and pae.""AnoEscolar"" <= {NONO_ANO}" : "")}
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
            if (filtro.CodigoUe != "-99")
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

        public async Task<IEnumerable<PerguntaRespostaOrdemDto>> MatematicaIADNumeroBimestre(int anoLetivo, string componenteCurricularId, int bimestre, string codigoUe, string codigoDre, ProficienciaSondagemEnum proeficienciaSondagem = ProficienciaSondagemEnum.IAD)
        {
            var listaRetorno = new List<PerguntaRespostaOrdemDto>();
            var bimestreLike = bimestre + "%";
            var sql = new StringBuilder();
            sql.AppendLine(@" SELECT pa.""AnoEscolar"" AS AnoTurma,
                                   p.""Id"" AS PerguntaId,
                                   p.""Descricao"" AS PerguntaDescricao,
                                   r.""Id"" AS RespostaId,
                                   r.""Descricao"" AS RespostaDescricao,
                                   pa.""Ordenacao"" AS OrdermPergunta,
                                   pr.""Ordenacao"" AS OrdemResposta,
                                   tabela.""QtdRespostas"" as QtdRespostas,
                                   tabela.""CodigoDre"" as CodigoDre,
                                   tabela.""CodigoUe"" as CodigoUe,
                                   tabela.""CodigoTurma"" as CodigoTurma
                            FROM ""Pergunta"" p
                            INNER JOIN ""PerguntaAnoEscolar"" pa ON pa.""PerguntaId"" = p.""Id""
                            LEFT JOIN ""PerguntaAnoEscolarBimestre"" paeb ON pa.""Id"" = paeb.""PerguntaAnoEscolarId""
                            INNER JOIN ""PerguntaResposta"" pr ON pr.""PerguntaId"" = p.""Id""
                            INNER JOIN ""Resposta"" r ON r.""Id"" = pr.""RespostaId""
                            LEFT JOIN
                              (SELECT p.""Id"" AS ""PerguntaId"",
                                      r.""Id"" AS ""RespostaId"",
                                      COUNT(DISTINCT sa.""CodigoAluno"") AS ""QtdRespostas"",
                                      s.""CodigoDre"",
                                      s.""CodigoUe"",
                                      s.""AnoTurma"",
                                      s.""CodigoTurma""
                               FROM ""SondagemAlunoRespostas"" sar
                               INNER JOIN ""SondagemAluno"" sa ON sa.""Id"" = sar.""SondagemAlunoId""
                               INNER JOIN ""Sondagem"" s ON s.""Id"" = sa.""SondagemId""
                               INNER JOIN ""Pergunta"" p ON p.""Id"" = sar.""PerguntaId""
                               INNER JOIN ""Resposta"" r ON r.""Id"" = sar.""RespostaId""
                               left join ""Periodo"" p2 on p2.""Id"" = s.""PeriodoId""
                               WHERE s.""ComponenteCurricularId"" = @componenteCurricularId
                                 AND s.""AnoLetivo"" = @anoLetivo
                                 AND (s.""Bimestre"" = @bimestre or p2.""Descricao"" like @bimestreLike )
                            ");

            if (!string.IsNullOrWhiteSpace(codigoDre) && codigoDre != "-99")
                sql.AppendLine(@" AND s.""CodigoDre""=@codigoDre ");
            if (!string.IsNullOrWhiteSpace(codigoUe) && codigoUe != "-99")
                sql.AppendLine(@" AND s.""CodigoUe""=@codigoUe ");

            sql.AppendLine(@" GROUP BY p.""Id"",
                                    r.""Id"",
                                    s.""CodigoDre"",
                                    s.""CodigoUe"",
                                    s.""AnoTurma"",
                                    s.""CodigoTurma"") AS tabela ON p.""Id"" = tabela.""PerguntaId""
                        AND r.""Id""= tabela.""RespostaId""
                        AND pa.""AnoEscolar"" = tabela.""AnoTurma""
                        WHERE ((pa.""FimVigencia"" IS NULL
                                AND EXTRACT (YEAR
                                             FROM pa.""InicioVigencia"") <= @anoLetivo)
                               OR (EXTRACT(YEAR
                                           FROM pa.""FimVigencia"") >= @anoLetivo
                                   AND EXTRACT (YEAR
                                                FROM pa.""InicioVigencia"") <= @anoLetivo))
                          AND (paeb.""Id"" IS NULL
                               AND NOT EXISTS
                                 (SELECT 1
                                  FROM ""PerguntaAnoEscolar"" pae
                                  INNER JOIN ""PerguntaAnoEscolarBimestre"" paeb ON paeb.""PerguntaAnoEscolarId"" = pae.""Id""
                                  WHERE pae.""AnoEscolar"" = pa.""AnoEscolar""
                                    AND (pae.""FimVigencia"" IS NULL
                                         AND EXTRACT(YEAR
                                                     FROM pae.""InicioVigencia"") <= @anoLetivo)
                                    AND paeb.""Bimestre"" = @bimestre )
                               OR paeb.""Bimestre"" = @bimestre ) ");
            if (proeficienciaSondagem == ProficienciaSondagemEnum.Numeros)
                sql.AppendLine(@" AND pa.""Grupo"" = @proeficiencia ");
            else sql.Append(@" pa.""AnoEscolar"" >= 4 and pa.""AnoEscolar"" <= 9 ");

            sql.AppendLine(@"   ORDER BY tabela.""CodigoDre"",
                                     tabela.""CodigoUe"",
                                     tabela.""CodigoTurma"",
                                     pa.""AnoEscolar"",
                                     pa.""Ordenacao"",
                                     pr.""Ordenacao"" ");

            var parametros = new
            {
                anoLetivo,
                componenteCurricularId,
                bimestre,
                codigoDre,
                codigoUe,
                bimestreLike,
                proeficiencia = (int)proeficienciaSondagem
            };

                using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
                {
                    var consulta = await conexao.QueryAsync<PerguntaRespostaOrdemDto>(sql.ToString(), parametros);
                    if (consulta != null)
                        listaRetorno = consulta.ToList();
                }
            
            return listaRetorno;
        }

        public async Task<IEnumerable<PerguntaRespostaOrdemDto>> MatematicaIADAntes2022(int anoLetivo, string componenteCurricularId, int bimestre, string codigoUe, string codigoDre, string periodoId)
        {
            var listaRetorno = new List<PerguntaRespostaOrdemDto>();
            var bimestreLike = bimestre+"%";
            var sql = new StringBuilder(@"SELECT  pae.""AnoEscolar"" as ""AnoTurma"",
                                            pae.""Ordenacao"" AS ""OrdemPergunta"",
                                            p.""Id"" AS ""PerguntaId"",
                                            p.""Descricao"" AS ""PerguntaDescricao"",
                                            pr.""Ordenacao"" AS ""OrdemResposta"",
                                            r.""Id"" AS ""RespostaId"",
                                            r.""Descricao"" AS ""RespostaDescricao"",
                                            tabela.""QtdRespostas"",
                                            tabela.""CodigoDre"",
                                            tabela.""CodigoUe"", 	
                                            tabela.""CodigoTurma""
                                    FROM ""PerguntaAnoEscolar"" pae
                                    INNER JOIN ""Pergunta"" p ON p.""Id"" = pae.""PerguntaId"" 
                                    INNER JOIN ""PerguntaResposta"" pr ON pr.""PerguntaId"" = p.""Id""
                                    LEFT JOIN ""Resposta"" r ON r.""Id"" = pr.""RespostaId"" ");

            if (bimestre == QUARTO_BIMESTRE)
                sql.AppendLine(@$" LEFT JOIN  ""PerguntaAnoEscolarBimestre"" paeb  on pae.""Id"" = paeb.""PerguntaAnoEscolarId"" and pae.""AnoEscolar"" >= {QUARTO_ANO} and pae.""AnoEscolar"" <= {NONO_ANO}"" : "") }} ");

                             sql.AppendLine(@"    LEFT JOIN ( SELECT p.""Id"" AS ""PerguntaId"",
                                                        r.""Id"" AS ""RespostaId"", COUNT(1) AS ""QtdRespostas"",
                                                        s.""CodigoDre"", s.""CodigoUe"", s.""AnoTurma"", s.""CodigoTurma""
                                                FROM ""SondagemAlunoRespostas"" sar
                                                INNER JOIN ""SondagemAluno"" sa ON sa.""Id"" = sar.""SondagemAlunoId""
                                                INNER JOIN ""Sondagem"" s ON s.""Id"" = sa.""SondagemId""
                                                INNER JOIN ""Pergunta"" p ON p.""Id"" = sar.""PerguntaId""
                                                INNER JOIN ""Resposta"" r ON r.""Id"" = sar.""RespostaId""
                                                inner join ""Periodo"" p2 on p2.""Id"" = s.""PeriodoId"" 
                                                WHERE s.""ComponenteCurricularId"" =  @componenteCurricularId
                                                    AND s.""AnoLetivo"" = @anoLetivo
                                                    and p2.""Descricao"" like @bimestreLike ");

                                        if (!string.IsNullOrWhiteSpace(codigoDre) && codigoUe != "-99")
                                              sql.AppendLine(@$"AND s.""CodigoDre"" =  @codigoDre");                                        
                                        if (!string.IsNullOrWhiteSpace(codigoUe) && codigoUe != "-99")
                                              sql.AppendLine(@$"AND s.""CodigoUe"" =  @codigoUe");

                sql.AppendLine(@$"   GROUP BY p.""Id"", r.""Id"", s.""CodigoDre"", s.""CodigoUe"", s.""AnoTurma"", s.""CodigoTurma"") 
                                            AS tabela ON p.""Id"" = tabela.""PerguntaId"" AND r.""Id"" = tabela.""RespostaId"" AND pae.""AnoEscolar"" = tabela.""AnoTurma""
                                    WHERE pae.""AnoEscolar"" >= 7 and pae.""AnoEscolar"" <= 9
                                    AND ((pae.""FimVigencia"" IS NULL AND EXTRACT (YEAR FROM pae.""InicioVigencia"") <= @anoLetivo)
                                    OR (EXTRACT(YEAR FROM pae.""FimVigencia"") >= @anoLetivo AND EXTRACT (YEAR FROM pae.""InicioVigencia"") <= @anoLetivo)) ");


            if (bimestre == QUARTO_BIMESTRE)
                sql.AppendLine($@" AND (paeb.""Id"" is null or paeb.""Bimestre"" = {QUARTO_BIMESTRE} ) ");


            sql.AppendLine(@$"     ORDER BY tabela.""CodigoDre"", tabela.""CodigoUe"", tabela.""CodigoTurma"", pae.""AnoEscolar"", pae.""Ordenacao"", pr.""Ordenacao"";
		


                                    select p.""Descricao"", pae.""AnoEscolar"", pae.""InicioVigencia"", pae.""Grupo"", paeb.""Bimestre"" from ""PerguntaAnoEscolar"" pae 
                                    inner join ""PerguntaAnoEscolarBimestre"" paeb on paeb.""PerguntaAnoEscolarId"" = pae.""Id"" 
                                    inner join ""Pergunta"" p on p.""Id"" = pae.""PerguntaId"" ;");


            var parametros = new
            {
                anoLetivo,
                bimestre,
                bimestreLike,
                codigoDre,
                codigoUe,
                componenteCurricularId,
                periodoId,
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                var consulta = await conexao.QueryAsync<PerguntaRespostaOrdemDto>(sql.ToString(), parametros);
                if (consulta != null)
                    listaRetorno = consulta.ToList();
            }
            return listaRetorno;
        }

        public async Task<IEnumerable<PerguntaRespostaOrdemDto>> MatematicaNumerosAntes2022(int anoLetivo, int semestre, string codigoUe, string codigoDre, string periodoId)
        {
            var listaRetorno = new List<PerguntaRespostaOrdemDto>();
            var sql = new StringBuilder(@"SELECT *
                                            FROM
                                              (SELECT *,
                                                      count(tabela.""RespostaDescricao"") AS ""QtdRespostas""
                                               FROM
                                                 (SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                         mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                         mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                         mpc.""AnoTurma"",
                                                         CASE
                                                             WHEN mpc.""Familiares"" = 'S' THEN 'Sim'
                                                             WHEN mpc.""Familiares"" = 'N' THEN 'Não'
                                                             ELSE 'Sem Preenchimento'
                                                         END AS ""RespostaDescricao"",
                                                         'Familiares ou Frequentes' AS ""PerguntaDescricao"",
                                                         CASE
                                                             WHEN mpc.""Familiares"" = 'S' THEN 0
                                                             WHEN mpc.""Familiares"" = 'N' THEN 1
                                                             ELSE 2
                                                         END AS ""OrdemResposta"",
                                                         1 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre ");

            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");



            sql.AppendLine(@"  UNION ALL SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                                   mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                                   mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                                   mpc.""AnoTurma"",
                                                                   CASE
                                                                       WHEN mpc.""Opacos"" = 'S' THEN 'Sim'
                                                                       WHEN mpc.""Opacos"" = 'N' THEN 'Não'
                                                                       ELSE 'Sem Preenchimento'
                                                                   END AS ""RespostaDescricao"",
                                                                   'Opacos' AS ""PerguntaDescricao"",
                                                                    CASE
                                                                         WHEN mpc.""Opacos"" = 'S' THEN 0
                                                                         WHEN mpc.""Opacos"" = 'N' THEN 1
                                                                         ELSE 2
                                                                     END AS ""OrdemResposta"",
                                                                   2 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre ");

            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");

            sql.AppendLine(@"           UNION ALL SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                                   mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                                   mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                                   mpc.""AnoTurma"",
                                                                   CASE
                                                                       WHEN mpc.""Transparentes"" = 'S' THEN 'Sim'
                                                                       WHEN mpc.""Transparentes"" = 'N' THEN 'Não'
                                                                       ELSE 'Sem Preenchimento'
                                                                   END AS ""RespostaDescricao"",
                                                                   'Transparentes' AS ""PerguntaDescricao"",
                                                                    CASE
                                                                       WHEN mpc.""Transparentes"" = 'S' THEN 0
                                                                       WHEN mpc.""Transparentes"" = 'N' THEN 1
                                                                       ELSE 2
                                                                    END AS ""OrdemResposta"",
                                                                   3 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre ");

            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");

            sql.AppendLine(@"       UNION ALL SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                                   mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                                   mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                                   mpc.""AnoTurma"",
                                                                   CASE
                                                                       WHEN mpc.""TerminamZero"" = 'S' THEN 'Sim'
                                                                       WHEN mpc.""TerminamZero"" = 'N' THEN 'Não'
                                                                       ELSE 'Sem Preenchimento'
                                                                   END AS ""RespostaDescricao"",
                                                                   'Terminam em Zero' AS ""PerguntaDescricao"",
                                                                    CASE
                                                                       WHEN mpc.""TerminamZero"" = 'S' THEN 0
                                                                       WHEN mpc.""TerminamZero"" = 'N' THEN 1
                                                                       ELSE 2
                                                                    END AS ""OrdemResposta"",
                                                                   4 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre ");


            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");


            sql.AppendLine(@"               UNION ALL SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                                   mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                                   mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                                   mpc.""AnoTurma"",
                                                                   CASE
                                                                       WHEN mpc.""Algarismos"" = 'S' THEN 'Sim'
                                                                       WHEN mpc.""Algarismos"" = 'N' THEN 'Não'
                                                                       ELSE 'Sem Preenchimento'
                                                                   END AS ""RespostaDescricao"",
                                                                   'Algarismos Iguais' AS ""PerguntaDescricao"",
                                                                    CASE
                                                                        WHEN mpc.""Algarismos"" = 'S' THEN 0
                                                                        WHEN mpc.""Algarismos"" = 'N' THEN 1
                                                                        ELSE 2
                                                                     END AS ""OrdemResposta"",
                                                                   5 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre  ");
            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");

            sql.AppendLine(@"                        UNION ALL SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                                   mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                                   mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                                   mpc.""AnoTurma"",
                                                                   CASE
                                                                       WHEN mpc.""Processo"" = 'S' THEN 'Sim'
                                                                       WHEN mpc.""Processo"" = 'N' THEN 'Não'
                                                                       ELSE 'Sem Preenchimento'
                                                                   END AS ""RespostaDescricao"",
                                                                   'Processo de Generalização' AS ""PerguntaDescricao"",
                                                                    CASE
                                                                        WHEN mpc.""Processo"" = 'S' THEN 0
                                                                        WHEN mpc.""Processo"" = 'N' THEN 1
                                                                        ELSE 2
                                                                     END AS ""OrdemResposta"",
                                                                   6 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre  ");

            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");

            sql.AppendLine(@"                UNION ALL SELECT mpc.""DreEolCode"" AS ""CodigoDre"",
                                                                   mpc.""EscolaEolCode"" AS ""CodigoUe"",
                                                                   mpc.""TurmaEolCode"" AS ""CodigoTurma"",
                                                                   mpc.""AnoTurma"",
                                                                   CASE
                                                                       WHEN mpc.""ZeroIntercalados"" = 'S' THEN 'Sim'
                                                                       WHEN mpc.""ZeroIntercalados"" = 'N' THEN 'Não'
                                                                       ELSE 'Sem Preenchimento'
                                                                   END AS ""RespostaDescricao"",
                                                                   'Zero Intercalado' AS ""PerguntaDescricao"",
                                                                    CASE
                                                                         WHEN mpc.""ZeroIntercalados"" = 'S' THEN 0
                                                                         WHEN mpc.""ZeroIntercalados"" = 'N' THEN 1
                                                                         ELSE 2
                                                                   END AS ""OrdemResposta"",
                                                                   7 AS ""OrdemPergunta""
                                                  FROM ""MathPoolNumbers"" mpc
                                                  WHERE mpc.""AnoLetivo"" = @anoLetivo
                                                    AND mpc.""Semestre"" = @semestre  ");

            if (!string.IsNullOrWhiteSpace(codigoDre))
                sql.AppendLine(@" AND mpc.""DreEolCode"" = @codigoDre  ");
            if (!string.IsNullOrWhiteSpace(codigoUe))
                sql.AppendLine(@" AND mpc.""EscolaEolCode"" = @codigoUe  ");

            sql.AppendLine(@") AS tabela
                                               GROUP BY tabela.""CodigoDre"",
                                                        tabela.""CodigoUe"",
                                                        tabela.""CodigoTurma"",
                                                        tabela.""AnoTurma"",
                                                        tabela.""OrdemPergunta"",
                                                        tabela.""PerguntaDescricao"",
                                                        tabela.""RespostaDescricao"",
                                                        tabela.""OrdemResposta""
                                               ORDER BY tabela.""CodigoDre"",
                                                        tabela.""CodigoUe"",
                                                        tabela.""CodigoTurma"",
                                                        tabela.""AnoTurma"",
                                                        tabela.""OrdemPergunta"",
                                                        tabela.""PerguntaDescricao"",
                                                        tabela.""RespostaDescricao"",
                                                        tabela.""OrdemResposta""
                                                        ) AS tabela  ");

            var parametros = new
            {
                anoLetivo,
                semestre,
                codigoDre,
                codigoUe,
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                var consulta = await conexao.QueryAsync<PerguntaRespostaOrdemDto>(sql.ToString(), parametros);
                if (consulta != null)
                    listaRetorno = consulta.ToList();
            }
            return listaRetorno;

        }

        public async Task<IEnumerable<PerguntaRespostaOrdemDto>> ConsolidacaoCampoAditivoMultiplicativoAntes2022(RelatorioMatematicaFiltroDto filtro)
        {
            var retorno = new List<PerguntaRespostaOrdemDto>();
            var ehCampoAditivo = filtro.Proficiencia == ProficienciaSondagemEnum.CampoAditivo;
            var tabela = ehCampoAditivo ? "MathPoolCAs" : "MathPoolCMs";

            var filtroUeDre = "";
            if (!string.IsNullOrEmpty(filtro.CodigoDre) && filtro.CodigoDre != "-99")
                filtroUeDre += @" AND mpc.""DreEolCode"" =  @CodigoDRE";
            if (!string.IsNullOrEmpty(filtro.CodigoUe) && filtro.CodigoUe != "-99")
                filtroUeDre += @" AND mpc.""EscolaEolCode"" =  @CodigoUe";

            var query = @$"	select *, count(tabela.""RespostaDescricao"") as ""QtdRespostas"" from ( ";

            for (int i = (ehCampoAditivo ? 1 : 3);  
                     i <= (ehCampoAditivo ? 4 : 8); i++)
            {
                query += @$"{(i != (ehCampoAditivo ? 1 : 3) ? " union all " : "")} select 'Ideia' AS ""SubPerguntaDescricao"",
	                       mpc.""DreEolCode"" as ""CodigoDre"", 
	                       mpc.""EscolaEolCode"" as ""CodigoUe"", 
	                       mpc.""TurmaEolCode"" as ""CodigoTurma"", 
	                       mpc.""AnoTurma"",
	                       case when mpc.""Ordem{i}Ideia"" = 'A' then 'Acertou'
	                            when mpc.""Ordem{i}Ideia"" = 'E' then 'Errou'
	                            when mpc.""Ordem{i}Ideia"" = 'NR' then 'Não resolveu'
	                            else 'Sem preenchimento' end AS ""RespostaDescricao"",
	                       {i} as ""OrdemPergunta""
	                       from ""{tabela}"" mpc
                    where mpc.""AnoLetivo"" = @AnoLetivo and mpc.""Semestre"" = @Bimestre {filtroUeDre}
                    union all
                    select 'Resultado' AS ""SubPerguntaDescricao"",
	                       mpc.""DreEolCode"" as ""CodigoDre"", 
	                       mpc.""EscolaEolCode"" as ""CodigoUe"", 
	                       mpc.""TurmaEolCode"" as ""CodigoTurma"", 
	                       mpc.""AnoTurma"",
	                       case when mpc.""Ordem{i}Resultado"" = 'A' then 'Acertou'
	                            when mpc.""Ordem{i}Resultado"" = 'E' then 'Errou'
	                            when mpc.""Ordem{i}Resultado"" = 'NR' then 'Não resolveu'
	                            else 'Sem preenchimento' end AS ""RespostaDescricao"",
	                       {i} as ""OrdemPergunta""       
	                       from ""{tabela}"" mpc
                    where mpc.""AnoLetivo"" = @AnoLetivo and mpc.""Semestre"" = @Bimestre  {filtroUeDre}";
            }

            query += @") as tabela
                        group by tabela.""CodigoDre"", 
	                           tabela.""CodigoUe"", 
	                           tabela.""CodigoTurma"", 
	                           tabela.""AnoTurma"",
	                           tabela.""OrdemPergunta"",
	                           tabela.""SubPerguntaDescricao"",
	                           tabela.""RespostaDescricao""
                        order by tabela.""CodigoDre"", 
	                           tabela.""CodigoUe"", 
	                           tabela.""CodigoTurma"", 
	                           tabela.""AnoTurma"",
	                           tabela.""OrdemPergunta"",
	                           tabela.""SubPerguntaDescricao"",
	                           tabela.""RespostaDescricao""";
            var parametros = new
            {
                filtro.CodigoUe,
                filtro.CodigoDre,
                filtro.AnoLetivo,
                filtro.Bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                var consulta = await conexao.QueryAsync<PerguntaRespostaOrdemDto>(query.ToString(), parametros);
                if (consulta != null)
                    retorno = consulta.ToList();
            }
            return retorno;
        }
    }
}