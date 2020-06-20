using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Queries
{
   public class CicloConsultas
    {
        internal static string ObterPorAnoModalidade = @"select tc.id from tipo_ciclo tc
                        inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                        where tca.ano = @ano and tca.modalidade = @modalidade";
    }
}
