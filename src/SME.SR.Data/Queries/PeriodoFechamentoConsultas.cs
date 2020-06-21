using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Queries
{
    public class PeriodoFechamentoConsultas
    {
        internal static string ObterPorTurma(int bimestre, long? periodoEscolarId)
        {
            var validacaoBimestre = bimestre == 0 ? "order by pe.bimestre desc limit 1" : "and pe.bimestre = @bimestre";
            var validacaoPeriodo = periodoEscolarId.HasValue ? "and pe.id = @periodoEscolarId" : "";

            return $@"select pfb.id, pfb.periodo_escolar_id PeriodoEscolarId, 
	                         pfb.periodo_fechamento_id PeriodoFechamentoId,
	                         pfb.inicio_fechamento InicioDoFechamento,
	                         pfb.final_fechamento FinalDoFechamento
                          from periodo_fechamento pf 
                         inner join periodo_fechamento_bimestre pfb on pfb.periodo_fechamento_id = pf.id
                         inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
                         inner join tipo_calendario tc on pe.tipo_calendario_id = tc.id 
                         where pf.ue_id = @ueId
                           and pf.dre_id = @dreId 
                           and tc.ano_letivo  = @anoLetivo
                            {validacaoPeriodo} 
                            {validacaoBimestre}";
        }
    }
}
