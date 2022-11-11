using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PeriodoFechamentoRepository : IPeriodoFechamentoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PeriodoFechamentoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<int> ObterBimestrePeriodoFechamentoAtual(int anoLetivo)
        {
            var query = @"select pe.bimestre 
                          from periodo_fechamento pf 
                         inner join periodo_fechamento_bimestre pfb on pfb.periodo_fechamento_id = pf.id
                         inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
                         inner join tipo_calendario tc on pe.tipo_calendario_id = tc.id 
                         where tc.ano_letivo  = @anoLetivo
                           and now()::date >= pfb.inicio_fechamento::date
                           order by pfb.inicio_fechamento desc 
                           limit 1 ";

            var parametros = new
            {
                AnoLetivo = anoLetivo
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<int>(query, parametros);
            };
        }

        public async Task<PeriodoFechamentoBimestre> ObterPeriodoFechamentoTurmaAsync(long ueId, long dreId, int anoLetivo, int bimestre, long? periodoEscolarId)
        {
            var query = PeriodoFechamentoConsultas.ObterPorTurma(bimestre, periodoEscolarId);
            var parametros = new
            {
                UeId = ueId,
                DreId = dreId,
                AnoLetivo = anoLetivo,
                Bimestre = bimestre,
                PeriodoEscolarId =  periodoEscolarId
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<PeriodoFechamentoBimestre>(query, parametros);
            };
        }

        public async Task<IEnumerable<PeriodoFechamentoBimestre>> ObterPeriodosFechamento(long ueId, long dreId, int anoLetivo)
        {
            var query = @"select pfb.id, pfb.periodo_escolar_id PeriodoEscolarId, 
	                         pfb.periodo_fechamento_id PeriodoFechamentoId,
	                         pfb.inicio_fechamento InicioDoFechamento,
	                         pfb.final_fechamento FinalDoFechamento
                          from periodo_fechamento pf 
                         inner join periodo_fechamento_bimestre pfb on pfb.periodo_fechamento_id = pf.id
                         inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
                         inner join tipo_calendario tc on pe.tipo_calendario_id = tc.id 
                         where pf.ue_id = @ueId
                           and pf.dre_id = @dreId 
                           and tc.ano_letivo  = @anoLetivo";

            var parametros = new
            {
                UeId = ueId,
                DreId = dreId,
                AnoLetivo = anoLetivo
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<PeriodoFechamentoBimestre>(query, parametros);
            };
        }
    }
}
