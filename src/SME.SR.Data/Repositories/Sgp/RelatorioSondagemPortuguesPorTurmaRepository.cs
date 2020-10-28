using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre, ProficienciaSondagemEnum proficiencia, string nomeColunaBimestre, GrupoSondagemEnum grupo)
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
                    sql += "where \"dreCodeEol\" = @dreCodigo ";
                    sql += "and \"schoolCodeEol\" = @ueCodigo ";
                    sql += "and \"classroomCodeEol\" = @turmaCodigo ";
                    sql += "and \"schoolYear\" = @anoLetivo ";
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
                    sql += "and sa.\"CodigoDre\" = @dreCodigo  ";
                    sql += "and sa.\"CodigoUe\" = @ueCodigo  ";
                    sql += "and sa.\"CodigoTurma\" = @turmaCodigo ";
                    sql += "and sa.\"AnoLetivo\" = @anoLetivo  ";
                    sql += "and sa.\"AnoTurma\" = @anoTurma ";
                    sql += "and cc.\"Id\" = @componenteCurricular ";
                    sql += "and p2.\"Descricao\" = @periodo  ";
                    sql += "order by sa2.\"NomeAluno\" ";
                    break;
            }

            var periodo = $"{ bimestre }° Bimestre";

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
    }
}
