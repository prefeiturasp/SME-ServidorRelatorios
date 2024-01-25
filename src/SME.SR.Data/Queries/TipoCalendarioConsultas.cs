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
            query.AppendLine(ObterFiltroSemestre(modalidade, semestre));
            if (semestre == 2)
                query.AppendLine("order by id desc");
            return query.ToString();
        }

        public static string ObterFiltroSemestre(ModalidadeTipoCalendario modalidade, int semestre)
        {
            return modalidade.EhEjaOuCelp() && semestre > 0
                ? "and t.semestre = @semestre"
                : string.Empty;
        }
    }
}
