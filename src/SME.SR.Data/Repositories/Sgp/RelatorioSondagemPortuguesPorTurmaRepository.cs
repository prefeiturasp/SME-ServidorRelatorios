using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RelatorioSondagemPortuguesPorTurmaRepository : IRelatorioSondagemPortuguesPorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        public RelatorioSondagemPortuguesPorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre, ProficienciaSondagemEnum proficiencia, string nomeColunaBimestre, GrupoSondagemEnum grupo, int semestre)
        {
            string sql = String.Empty;

            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.Leitura:
                case ProficienciaSondagemEnum.Escrita:
                    sql = $"select \"{nomeColunaBimestre}\" Resposta, ";
                    sql += "'1' PerguntaId, '' Pergunta, ";
                    sql += "\"studentCodeEol\" AlunoEolCode, ";
                    sql += "\"studentNameEol\" AlunoNome, ";
                    sql += "\"schoolYear\" AnoLetivo, ";
                    sql += "\"yearClassroom\" AnoTurma, ";
                    sql += "\"classroomCodeEol\" TurmaEolCode ";
                    sql += "from \"PortuguesePolls\" ";
                    sql += "where 1 = 1 ";

                    if (!string.IsNullOrEmpty(dreCodigo) && int.Parse(dreCodigo) > 0)
                        sql += "and \"dreCodeEol\" = @dreCodigo ";

                    if (!string.IsNullOrEmpty(ueCodigo) && int.Parse(ueCodigo) > 0)
                        sql += "and \"schoolCodeEol\" = @ueCodigo ";

                    if (!string.IsNullOrEmpty(turmaCodigo))
                        sql += "and \"classroomCodeEol\" = @turmaCodigo ";

                    if (anoLetivo > 0)
                        sql += "and \"schoolYear\" = @anoLetivo ";

                    if (anoTurma > 0)
                        sql += "and \"yearClassroom\" = @anoTurma ";

                    break;
                case ProficienciaSondagemEnum.Autoral:
                    sql += "select distinct sa2.\"CodigoAluno\" AlunoEolCode, sa2.\"NomeAluno\" AlunoNome, sa.\"AnoLetivo\", sa.\"AnoTurma\", sa.\"CodigoTurma\" TurmaEolCode, p.\"Id\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Descricao\" Resposta ";

                    sql += "from \"Sondagem\" sa ";
                    sql += "inner join \"ComponenteCurricular\" cc on sa.\"ComponenteCurricularId\" = cc.\"Id\"  ";
                    sql += "inner join \"Periodo\" p2 on sa.\"PeriodoId\" = p2.\"Id\"  ";
                    sql += "inner join \"SondagemAluno\" sa2 on sa.\"Id\" = sa2.\"SondagemId\"  ";
                    sql += "inner join \"Pergunta\" p on p.\"ComponenteCurricularId\" = sa.\"ComponenteCurricularId\"  ";
                    sql += "inner join \"SondagemAlunoRespostas\" pr on pr.\"PerguntaId\" = p.\"Id\" and pr.\"SondagemAlunoId\" = sa2.\"Id\"  ";
                    sql += "inner join \"Resposta\" r on r.\"Id\" = pr.\"RespostaId\"  ";
                    sql += "inner join \"OrdemPergunta\" op on op.\"GrupoId\" = sa.\"GrupoId\" ";
                    sql += "where sa.\"GrupoId\" = @grupoId ";

                    if(!string.IsNullOrEmpty(dreCodigo) && int.Parse(dreCodigo) > 0)
                    {
                        sql += "and sa.\"CodigoDre\" = @dreCodigo  ";
                    }
                    if (!string.IsNullOrEmpty(ueCodigo) != null && int.Parse(ueCodigo) > 0)
                    {
                        sql += "and sa.\"CodigoUe\" = @ueCodigo  ";
                    }
                    sql += "and sa.\"CodigoTurma\" = @turmaCodigo ";
                    sql += "and sa.\"AnoLetivo\" = @anoLetivo  ";
                    sql += "and sa.\"AnoTurma\" = @anoTurma ";
                    sql += "and cc.\"Id\" = @componenteCurricular ";
                    sql += "and p2.\"Descricao\" = @periodo  ";
                    sql += "order by sa2.\"NomeAluno\" ";
                    break;
            }

            var periodo = $"{ Math.Max(bimestre, semestre) }° {(bimestre != 0 ? "Bimestre": "Semestre")}";

            var componenteCurricular = ComponenteCurricularSondagemEnum.Portugues.Name();

            var grupoId = grupo.Name();

            var parametros = new object();

            if (proficiencia == ProficienciaSondagemEnum.Autoral)
            {
                parametros = new { componenteCurricular, dreCodigo, grupoId, ueCodigo, periodo, turmaCodigo, anoLetivo, anoTurma };
            } else
            {
                parametros = new { componenteCurricular, dreCodigo, grupoId, ueCodigo, periodo, turmaCodigo, anoLetivo = anoLetivo.ToString(), anoTurma = anoTurma.ToString() };
            }

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>(sql, parametros);
        }

        public async Task<IEnumerable<SondagemAutoralPorAlunoDto>> ObterPorFiltros(string grupoId, string componenteCurricularId, string periodoId, int anoLetivo, string codigoTurma)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"
                    select
                       s.""OrdemId"",
                       o.""Descricao"" as ""OrdemDescricao"",
                       sa.""CodigoAluno"",
                       sa.""NomeAluno"",
                       p.""Id"" as ""PerguntaId"",
                       p.""Descricao"" as ""PerguntaDescricao"",
                       sar.""RespostaId"",
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
                inner join ""Ordem"" o on
                    o.""Id"" = s.""OrdemId""
                where 1 = 1");

            if (!string.IsNullOrEmpty(grupoId))
                query.AppendLine(" and s.\"GrupoId\" = @GrupoId");

            if (!string.IsNullOrEmpty(componenteCurricularId))
                query.AppendLine(" and s.\"ComponenteCurricularId\" = @ComponenteCurricularId");

            if (!string.IsNullOrEmpty(periodoId))
                query.AppendLine(" and s.\"PeriodoId\" = @PeriodoId");

            if(anoLetivo > 0)
                query.AppendLine(" and s.\"AnoLetivo\" = @AnoLetivo");

            if (!string.IsNullOrEmpty(codigoTurma))
                query.AppendLine(" and s.\"CodigoTurma\" = @CodigoTurma");

            var parametros = new { grupoId, componenteCurricularId, periodoId, anoLetivo, codigoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<SondagemAutoralPorAlunoDto>(query.ToString(), parametros);
        }
    }
}
