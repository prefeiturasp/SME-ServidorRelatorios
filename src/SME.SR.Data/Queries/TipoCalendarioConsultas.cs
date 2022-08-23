using SME.SR.Infra;
using System.Text;

namespace SME.SR.Data
{
    public class TipoCalendarioConsultas
    {
        public static string ObterPorAnoLetivoEModalidade(ModalidadeTipoCalendario modalidade, int semestre = 0) {

            StringBuilder query = new StringBuilder();

            query.AppendLine("select *");
            query.AppendLine("from tipo_calendario t");
            query.AppendLine("where t.excluido = false");
            query.AppendLine("and t.ano_letivo = @anoLetivo");
            query.AppendLine("and t.modalidade = @modalidade");

            if (modalidade == ModalidadeTipoCalendario.EJA)
            {
                var periodoReferencia = $"periodo_inicio {(semestre == 1 ? "<" : ">")} @dataReferencia";
                query.AppendLine($"and exists(select 0 from periodo_escolar p where tipo_calendario_id = t.id and {periodoReferencia})");
                if (semestre == 2)
                    query.AppendLine("order by id desc");
            }
            return query.ToString();
        }
           
    }
}
