using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemPortuguesPorTurmaRepository : IRelatorioSondagemPortuguesPorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        public RelatorioSondagemPortuguesPorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre, ProficienciaSondagemEnum proficiencia, string nomeColunaBimestre)
        {
            string sql = String.Empty;

            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.Leitura:
                case ProficienciaSondagemEnum.Escrita:
                    sql = $"select \"{nomeColunaBimestre}\" Resposta, ";
                    sql += "1 Id, '' Pergunta, ";
                    sql += "\"studentCodeEol\" AlunoEolCode, ";
                    sql += "\"studentNameEol\" AlunoNome, ";
                    sql += "\"schoolYear\" AnoLetivo, ";
                    sql += "\"yearClassroom\" AnoTurma, ";
                    sql += "\"classroomCodeEol\" TurmaEolCode ";
                    sql += "from \"PortuguesePolls\" ";
                    sql += "where \"dreCodeEol\" = @dreCodigo ";
                    sql += "and \"schoolCodeEol\" = @ueCodigo ";
                    sql += "and \"classroomCodeEol\" = @turmaCodigo ";
                    sql += "and \"schoolYear\" = @anoLetivo ";
                    sql += "and \"yearClassroom\" = @anoTurma ";
                    break;
                case ProficienciaSondagemEnum.LeituraVozAlta:
                    sql = $"select \"CodigoAluno\" AlunoEolCode, \"NomeAluno\" AlunoNome, \"AnoLetivo\", \"AnoTurma\", \"CodigoTurma\" TurmaEolCode, pae.\"Ordenacao\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Descricao\" Resposta";
                    sql += " from \"SondagemAutoral\" sa inner join \"Pergunta\" p on sa.\"PerguntaId\" = p.\"Id\"";
                    sql += " inner join \"ComponenteCurricular\" cc on p.\"ComponenteCurricularId\" = cc.\"Id\"";
                    sql += " inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" and pae.\"AnoEscolar\" = sa.\"AnoTurma\"";
                    sql += " inner join \"Resposta\" r on sa.\"RespostaId\" = r.\"Id\"";
                    sql += " where cc.\"Id\" = @componenteCurricular and  \"CodigoDre\" = @dreCodigo and \"CodigoUe\" = @ueCodigo and \"CodigoTurma\" = @turmaCodigo and sa.\"AnoLetivo\" = @anoLetivo and \"AnoTurma\" = @anoTurma order by \"NomeAluno\"";
                    break;
            }

            var componenteCurricular = ComponenteCurricularSondagemEnum.Portugues.Name();

            var parametros = new { componenteCurricular, dreCodigo, ueCodigo, turmaCodigo, anoLetivo = anoLetivo.ToString(), anoTurma = anoTurma.ToString() };

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
                where
            		s.""Id"" in (
            		select
            			s.""Id""
            		from
            			""Sondagem"" s
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

            query.AppendLine(")");

            var parametros = new { grupoId, componenteCurricularId, periodoId, anoLetivo, codigoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<SondagemAutoralPorAlunoDto>(query.ToString(), parametros);
        }
    }
}
