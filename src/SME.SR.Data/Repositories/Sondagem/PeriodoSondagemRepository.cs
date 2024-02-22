using Npgsql;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PeriodoSondagemRepository : IPeriodoSondagemRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PeriodoSondagemRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<DateTime> ObterPeriodoAberturaFimPorBimestreAnoLetivo(int bimestre, int anoLetivo)
        {
            var query = new StringBuilder("select \"DataFim\" ");
            query.AppendLine("from \"PeriodoDeAberturas\" ");
            query.AppendLine("where \"Ano\" = @anoLetivo");
            query.AppendLine("and \"Bimestre\" = @bimestre");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<DateTime>(query.ToString(), new { bimestre, anoLetivo = anoLetivo.ToString() });
        }

        public async Task<DateTime> ObterPeriodoFixoFimPorDescricaoAnoLetivo(string descricao, int anoLetivo)
        {
            var query = new StringBuilder("select \"DataFim\" ");
            query.AppendLine("from \"PeriodoFixoAnual\" ");
            query.AppendLine("where \"Ano\" = @anoLetivo");
            query.AppendLine("and \"Descricao\" = @descricao");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<DateTime>(query.ToString(), new { descricao, anoLetivo });
        }

        public async Task<PeriodoSondagem> ObterPeriodoPorTipo(int periodo, TipoPeriodoSondagem tipoPeriodo)
        {
            var query = new StringBuilder("select \"Id\", \"Descricao\" ");
            query.AppendLine("from \"Periodo\" ");
            query.AppendLine("where \"TipoPeriodo\" = @tipoPeriodo");
            query.AppendLine("and \"Descricao\" like @periodo");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<PeriodoSondagem>(query.ToString(), new { periodo = $"{periodo}%", tipoPeriodo = (int)tipoPeriodo });
        }

        public async Task<PeriodoCompletoSondagemDto> ObterPeriodoCompletoPorBimestreEAnoLetivo(int bimestre, string anoLetivo)
        {
            var query = new StringBuilder("select \"DataInicio\" as PeriodoInicio, \"DataFim\" as PeriodoFim ");
            query.AppendLine("from \"PeriodoDeAberturas\" ");
            query.AppendLine("where \"Bimestre\" = @bimestre");
            query.AppendLine($"and \"TipoPeriodicidade\" = {(int)TipoPeriodoSondagem.Bimestre}");
            query.AppendLine("and \"Ano\" = @anoLetivo");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<PeriodoCompletoSondagemDto>(query.ToString(), new { bimestre, anoLetivo });
        }

        public async Task<PeriodoCompletoSondagemDto> ObterPeriodoCompletoPorSemestreEAnoLetivo(int semestre, string anoLetivo)
        {
            var query = new StringBuilder("select \"DataInicio\" as PeriodoInicio, \"DataFim\" as PeriodoFim ");
            query.AppendLine("from \"PeriodoDeAberturas\" ");
            query.AppendLine("where \"Bimestre\" = @semestre");
            query.AppendLine($"and \"TipoPeriodicidade\" = {(int)TipoPeriodoSondagem.Semestre}");
            query.AppendLine("and \"Ano\" = @anoLetivo");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<PeriodoCompletoSondagemDto>(query.ToString(), new { semestre, anoLetivo });
        }

        public async Task<PeriodoCompletoSondagemDto> ObterPeriodoFixoCompletoPorDescricaoEAnoLetivo(string likeDescricao, int anoLetivo)
        {
            var query = new StringBuilder("select pfa.\"DataInicio\" as PeriodoInicio, pfa.\"DataFim\" as PeriodoFim ");
            query.AppendLine("from \"PeriodoFixoAnual\" pfa ");
            query.AppendLine("where pfa.\"Ano\" = @anoLetivo");
            query.AppendLine("and pfa.\"Descricao\" like @likeDescricao");
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);

            return await conexao.QueryFirstOrDefaultAsync<PeriodoCompletoSondagemDto>(query.ToString(), new
            {
                likeDescricao,
                anoLetivo
            });
        }
    }
}

