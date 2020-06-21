using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Queries
{
    public class NotaTipoConsultas
    {
        internal static string ObterPorCicloIdDataAvaliacao = @"select ntv.descricao from notas_tipo_valor ntv
                        inner join notas_conceitos_ciclos_parametos nccp
                        on nccp.tipo_nota = ntv.id
                        where nccp.ciclo = @cicloId and @dataReferencia >= nccp.inicio_vigencia
                        and (nccp.ativo = true or @dataReferencia <= nccp.fim_vigencia)
                        order by nccp.id asc";
    }
}
