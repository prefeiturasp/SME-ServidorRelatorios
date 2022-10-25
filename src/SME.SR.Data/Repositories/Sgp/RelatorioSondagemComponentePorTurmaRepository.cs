using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemComponentePorTurmaRepository : IRelatorioSondagemComponentePorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private const int QUARTO_ANO = 4;
        private const int NONO_ANO = 9;
        private const int QUARTO_BIMESTRE = 4;
        private const int SEGUNDO_BIMESTRE = 2;

        public RelatorioSondagemComponentePorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdensAsync()
        {
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaOrdemDto>("select ROW_NUMBER () OVER (ORDER BY \"Id\") as Id, \"Descricao\" from public.\"Ordem\" ");
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasQueryDto>> ObterPerguntas(int anoLetivo, int anoTurma, int bimestre)
        {
            var numeracaoNaDescricaoDaQuestao = ExibirNumeroDaQuestao(anoTurma, bimestre) ? $@" 'Questão '|| pae.""Ordenacao""|| ': ' || p.""Descricao"" as ""Pergunta""  " : $@" p.""Descricao"" as ""Pergunta"" ";
            var sql = $@"select pae.""Ordenacao"" as ""PerguntaId"", {numeracaoNaDescricaoDaQuestao}
            from ""PerguntaAnoEscolar"" pae
                           inner join ""Pergunta"" p on p.""Id"" = pae.""PerguntaId""
                           left join  ""PerguntaAnoEscolarBimestre"" paeb ON paeb.""PerguntaAnoEscolarId"" = pae.""Id"" 
            where pae.""AnoEscolar"" = @anoTurma 
                           and(pae.""FimVigencia"" is null and extract(year from pae.""InicioVigencia"") <= @anoLetivo)";

            if (anoTurma <= 3)
                sql += "and pae.\"Grupo\" = @grupoNumero";

            sql += $@" and (paeb.""Id"" is null
                       and not exists(select 1 from ""PerguntaAnoEscolar"" pae 
                                      inner join  ""PerguntaAnoEscolarBimestre"" paeb ON paeb.""PerguntaAnoEscolarId"" = pae.""Id""
                                      where pae.""AnoEscolar"" = @anoTurma 
                                      and (pae.""FimVigencia"" is null and extract(year from pae.""InicioVigencia"") <= @anoLetivo) 
                                      and paeb.""Bimestre"" = @bimestre)
                        or paeb.""Bimestre"" = @bimestre)";
            sql += " order by pae.\"Ordenacao\"";

            var parametros = new { anoLetivo, anoTurma, bimestre, grupoNumero = (int)ProficienciaSondagemEnum.Numeros };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPerguntasQueryDto>(sql.ToString(), parametros);
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasProficienciaQueryDto>> ObterPerguntasProficiencia(int anoLetivo, int anoTurma, ProficienciaSondagemEnum proficiencia)
        {
            var sql = new StringBuilder();

            sql.AppendLine(" select pae.\"Ordenacao\" as PerguntaId, ");
            sql.AppendLine(" p_pai.\"Descricao\" as Pergunta, ");
            sql.AppendLine(" p_filho.\"Id\" as SubPerguntaId, ");
            sql.AppendLine(" p_filho.\"Descricao\" as SubPergunta ");
            sql.AppendLine(" from \"PerguntaAnoEscolar\" pae ");
            sql.AppendLine(" inner join \"Pergunta\" p_pai on p_pai.\"Id\" = pae.\"PerguntaId\" ");
            sql.AppendLine(" inner join \"Pergunta\" p_filho on p_filho.\"PerguntaId\" = pae.\"PerguntaId\" ");
            sql.AppendLine(" where pae.\"AnoEscolar\" = @anoTurma ");
            sql.AppendLine(" and ((pae.\"FimVigencia\" IS NULL AND EXTRACT (YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo) ");
            sql.AppendLine("  or (EXTRACT(YEAR FROM pae.\"FimVigencia\") >= @anoLetivo AND EXTRACT (YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo)) ");
            sql.AppendLine(" and pae.\"Grupo\" = @proficiencia ");

            var parametros = new { anoLetivo, anoTurma, proficiencia };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPerguntasProficienciaQueryDto>(sql.ToString(), parametros);
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasRespostasQueryDto>> ObterPerguntasRespostas(string dreCodigo, string turmaCodigo,
            int anoLetivo, int bimestre, int anoTurma, string componenteCurricularId, string periodoId = "")
        {
            var sql = new StringBuilder();

            sql.AppendLine("select distinct sa.\"CodigoAluno\" as AlunoEolCode, ");
            sql.AppendLine(" sa.\"NomeAluno\", ");
            sql.AppendLine(" s.\"AnoLetivo\", ");
            sql.AppendLine(" s.\"AnoTurma\", ");
            sql.AppendLine(" s.\"CodigoTurma\", ");
            sql.AppendLine(" pae.\"Ordenacao\" as PerguntaId, ");
            sql.AppendLine(" p.\"Descricao\" as Pergunta, ");
            sql.AppendLine(" r.\"Descricao\" as Resposta, ");
            sql.AppendLine(" pr.\"Ordenacao\" as OrdenacaoResposta ");
            sql.AppendLine(" from \"SondagemAlunoRespostas\" sar ");
            sql.AppendLine(" inner join \"SondagemAluno\" sa on sa.\"Id\" = sar.\"SondagemAlunoId\" ");
            sql.AppendLine(" inner join \"Sondagem\" s on s.\"Id\" = sa.\"SondagemId\" ");
            sql.AppendLine(" inner join \"Pergunta\" p on p.\"Id\" = sar.\"PerguntaId\" ");
            sql.AppendLine(" inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" ");
            sql.AppendLine(" inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p.\"Id\" and pr.\"RespostaId\" = sar.\"RespostaId\"");
            sql.AppendLine(" inner join \"Resposta\" r on r.\"Id\" = sar.\"RespostaId\" ");
            sql.AppendLine(" where s.\"AnoLetivo\" = @anoLetivo ");
            sql.AppendLine(" and s.\"CodigoTurma\" = @turmaCodigo");
            sql.AppendLine(" and s.\"ComponenteCurricularId\" = @componenteCurricularId");
            sql.AppendLine(" and sa.\"Bimestre\" = @bimestre ");
            sql.AppendLine(" and ((pae.\"FimVigencia\" IS NULL AND EXTRACT (YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo) ");
            sql.AppendLine("  or (EXTRACT(YEAR FROM pae.\"FimVigencia\") >= @anoLetivo AND EXTRACT (YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo)) ");

            if (anoTurma <= 3)
                sql.Append(" AND pae.\"Grupo\" = ").Append((int)ProficienciaSondagemEnum.Numeros).AppendLine();

            sql.AppendLine(" order by sa.\"NomeAluno\", pae.\"Ordenacao\", sa.\"CodigoAluno\" ");

            var parametros = new { anoLetivo, dreCodigo, anoTurma, turmaCodigo, periodoId, bimestre, componenteCurricularId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPerguntasRespostasQueryDto>(sql.ToString(), parametros);
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasRespostasProficienciaQueryDto>> ObterPerguntasRespostasProficiencia(string dreCodigo,
            string turmaCodigo, int anoLetivo, int bimestre, ProficienciaSondagemEnum proficiencia, int anoTurma, string componenteCurricularId, string periodoId = "")
        {
            var sql = new StringBuilder();

            sql.AppendLine(" select distinct sa.\"CodigoAluno\" as AlunoEolCode, ");
            sql.AppendLine(" sa.\"NomeAluno\", ");
            sql.AppendLine(" s.\"AnoLetivo\", ");
            sql.AppendLine(" s.\"AnoTurma\", ");
            sql.AppendLine(" s.\"CodigoTurma\", ");
            sql.AppendLine(" pae.\"Ordenacao\" as PerguntaId, ");
            sql.AppendLine(" p_pai.\"Descricao\" as Pergunta, ");
            sql.AppendLine(" r.\"Descricao\" as Resposta, ");
            sql.AppendLine(" pr.\"Ordenacao\" as OrdenacaoResposta, ");
            sql.AppendLine(" p_filho.\"Id\" as SubPerguntaId, ");
            sql.AppendLine(" p_filho.\"Descricao\" as SubPergunta ");
            sql.AppendLine(" from \"PerguntaAnoEscolar\" pae ");
            sql.AppendLine(" inner join \"Pergunta\" p_pai on p_pai.\"Id\" = pae.\"PerguntaId\" ");
            sql.AppendLine(" inner join \"Pergunta\" p_filho on p_filho.\"PerguntaId\" = pae.\"PerguntaId\" ");
            sql.AppendLine(" inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p_filho.\"Id\" ");
            sql.AppendLine(" left join \"Resposta\" r on r.\"Id\" = pr.\"RespostaId\" ");
            sql.AppendLine(" inner join \"SondagemAlunoRespostas\" sar on sar.\"PerguntaId\" = p_filho.\"Id\" and sar.\"RespostaId\" = r.\"Id\" ");
            sql.AppendLine(" inner join \"SondagemAluno\" sa on sa.\"Id\" = sar.\"SondagemAlunoId\" ");
            sql.AppendLine(" inner join \"Sondagem\" s on s.\"Id\" = sa.\"SondagemId\" ");
            sql.AppendLine(" where s.\"AnoLetivo\" = @anoLetivo ");
            sql.AppendLine(" and s.\"CodigoDre\" = @dreCodigo ");
            sql.AppendLine(" and s.\"AnoTurma\" = @anoTurma ");
            sql.AppendLine(" and s.\"CodigoTurma\" = @turmaCodigo ");
            sql.AppendLine(" and ((pae.\"FimVigencia\" IS NULL AND EXTRACT (YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo) ");
            sql.AppendLine("  or (EXTRACT(YEAR FROM pae.\"FimVigencia\") >= @anoLetivo AND EXTRACT (YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo)) ");
            sql.AppendLine(" and sa.\"Bimestre\" = @bimestre ");
            sql.AppendLine(" and pae.\"Grupo\" = @proficiencia ");
            sql.AppendLine("  and s.\"ComponenteCurricularId\" = @componenteCurricularId");

            if (!string.IsNullOrEmpty(periodoId))
                sql.AppendLine(" and s.\"PeriodoId\" = @periodoId ");

            sql.AppendLine(" order by sa.\"NomeAluno\", sa.\"CodigoAluno\", pae.\"Ordenacao\", p_filho.\"Descricao\" ");

            var parametros = new { anoLetivo, dreCodigo, anoTurma, turmaCodigo, periodoId, bimestre, proficiencia, componenteCurricularId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPerguntasRespostasProficienciaQueryDto>(sql.ToString(), parametros);
        }

        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int anoLetivo,
            int semestre, ProficienciaSondagemEnum proficiencia, int anoTurma, string periodoId = "")
        {
            StringBuilder sql = new StringBuilder();

            if (anoTurma >= 7)
            {
                sql.AppendLine("select \"CodigoAluno\" AlunoEolCode, \"NomeAluno\" AlunoNome, \"AnoLetivo\", \"AnoTurma\", \"CodigoTurma\", pae.\"Ordenacao\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Descricao\" Resposta, pr.\"Ordenacao\" OrdenacaoResposta ");
                sql.AppendLine(" from \"Sondagem\" s inner join \"SondagemAluno\" sa on sa.\"SondagemId\" = s.\"Id\" ");
                sql.AppendLine(" inner join \"SondagemAlunoRespostas\" sar on sar.\"SondagemAlunoId\" = sa.\"Id\"  ");
                sql.AppendLine(" inner join \"Pergunta\" p on sar.\"PerguntaId\" = p.\"Id\"  ");
                sql.AppendLine(" inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" and pae.\"AnoEscolar\" = s.\"AnoTurma\"");
                sql.AppendLine(" inner join \"Resposta\" r on sar.\"RespostaId\" = r.\"Id\" ");
                sql.AppendLine(" inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p.\"Id\" and pr.\"RespostaId\" = r.\"Id\" ");
                sql.AppendLine(" where s.\"AnoLetivo\" = @anoLetivo and \"CodigoDre\" = @dreCodigo and \"AnoTurma\" = @anoTurma and \"CodigoTurma\" = @turmaCodigo and \"PeriodoId\" = @periodoId order by \"NomeAluno\" , pr.\"Ordenacao\"");
            }
            else
            {
                if (proficiencia == ProficienciaSondagemEnum.CampoMultiplicativo)
                {
                    sql.AppendLine("select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\",\"Ordem5Ideia\",\"Ordem5Resultado\",\"Ordem6Ideia\",\"Ordem6Resultado\",\"Ordem7Ideia\",\"Ordem7Resultado\",\"Ordem8Ideia\",\"Ordem8Resultado\" from \"MathPoolCMs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"");
                }
                else if (proficiencia == ProficienciaSondagemEnum.Numeros)
                {
                    sql.AppendLine("select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Familiares\",\"Opacos\",\"Transparentes\",\"TerminamZero\",\"Algarismos\",\"Processo\",\"ZeroIntercalados\" from \"MathPoolNumbers\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"");
                }
                else
                {
                    sql.AppendLine("select \"AlunoEolCode\", \"AlunoNome\", \"AnoLetivo\", \"AnoTurma\",\"Semestre\",\"Ordem1Ideia\",\"Ordem1Resultado\",\"Ordem2Ideia\",\"Ordem2Resultado\",\"Ordem3Ideia\",\"Ordem3Resultado\",\"Ordem4Ideia\",\"Ordem4Resultado\" from \"MathPoolCAs\" where \"DreEolCode\" = @dreCodigo and \"AnoLetivo\" = @anoLetivo and \"TurmaEolCode\" = @turmaCodigo and \"Semestre\" = @semestre order by \"AlunoNome\"");
                }
            }

            var componenteCurricular = ComponenteCurricularSondagemEnum.Matematica.Name();

            var parametros = new { componenteCurricular, dreCodigo, anoLetivo, turmaCodigo, semestre, anoTurma, periodoId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql.ToString(), parametros);
        }

        private bool UtilizarPerguntaAnoEscolarBimestre(int anoEscolar, int bimestre)
        {
            return (anoEscolar >= QUARTO_ANO && anoEscolar <= NONO_ANO) && bimestre == QUARTO_BIMESTRE;
        }

        private bool ExibirNumeroDaQuestao(int anoEscolar, int bimestre)
        {
            return UtilizarPerguntaAnoEscolarBimestre(anoEscolar, bimestre) || bimestre == SEGUNDO_BIMESTRE;
        }
    }
}
