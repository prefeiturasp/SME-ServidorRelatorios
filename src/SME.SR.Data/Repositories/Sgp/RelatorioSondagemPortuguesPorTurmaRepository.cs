using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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
        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre, ProficienciaSondagemEnum proficiencia)
        {
            string sql = String.Empty;

            string nomeColunaBimestre = ObterNomeColunaBimestre(bimestre, proficiencia);

            if (nomeColunaBimestre == String.Empty)
                throw new Exception($"Nome da coluna do bimestre não pode ser vazio.");

            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.Leitura:
                case ProficienciaSondagemEnum.Escrita:
                    sql = $"select {nomeColunaBimestre} Resposta, ";
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
                    sql += " inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" and pae.\"AnoEscolar\" = sa.\"AnoTurma\"";
                    sql += " inner join \"Resposta\" r on sa.\"RespostaId\" = r.\"Id\"";
                    sql += " where \"CodigoDre\" = @dreCodigo and \"CodigoUe\" = @ueCodigo and \"CodigoTurma\" = @turmaCodigo and sa.\"AnoLetivo\" = @anoLetivo and \"AnoTurma\" = @anoTurma order by \"NomeAluno\"";
                    break;
            }

            if (sql == String.Empty)
                throw new Exception($"{ proficiencia } fora do esperado.");

            var parametros = new { dreCodigo, ueCodigo, turmaCodigo, anoLetivo, anoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryAsync<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>(sql, parametros);
        }

        private String ObterNomeColunaBimestre(int bimestre, ProficienciaSondagemEnum proficiencia)
        {
            string nomeColunaBimestre = String.Empty;

            if (proficiencia == ProficienciaSondagemEnum.Leitura)
                nomeColunaBimestre = $"reading{bimestre}B";

            if (proficiencia == ProficienciaSondagemEnum.Leitura)
                nomeColunaBimestre = $"writing{bimestre}B";

            return nomeColunaBimestre;
        }
    }
}
