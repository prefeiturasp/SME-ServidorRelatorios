using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class RelatorioSondagemComponentePorTurmaRepository : IRelatorioSondagemComponentePorTurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioSondagemComponentePorTurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public List<RelatorioSondagemComponentesPorTurmaOrdemDto> ObterOrdens()
        {
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return conexao.Query<RelatorioSondagemComponentesPorTurmaOrdemDto>(
                "select Id, Descricao from Ordem").ToList();
            }
        }

        public List<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto> ObterPlanilhaLinhas(int dreId, int turmaId, int ano, int semestre)
        {
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                string sql = @$"select
                        AlunoEolCode,
                        AlunoNome,
                        AnoLetivo,
                        AnoTurma,
                        Semestre,
                        Ordem1Ideia,
                        Ordem1Resultado,
                        Ordem2Ideia,
                        Ordem2Resultado,
                        Ordem3Ideia,
                        Ordem3Resultado,
                        Ordem4Ideia,
                        Ordem4Resultado
                        from MathPoolCAs
                        where
                        DreEolCode = { dreId }
                        and AnoLetivo = { ano }
                        and AnoTurma = { turmaId }
                        order by AlunoNome";

                return conexao.Query<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>(sql).ToList();
            }
        }
    }
}
