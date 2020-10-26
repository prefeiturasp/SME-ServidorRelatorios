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
    }
}
