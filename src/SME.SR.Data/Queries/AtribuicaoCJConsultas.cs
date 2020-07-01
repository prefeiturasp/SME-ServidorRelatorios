using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class AtribuicaoCJConsultas
    {
        internal static string ObterPorFiltros(Modalidade modalidade, string turmaId, string ueId, long componenteCurricularId,
            string usuarioRf, string usuarioNome, bool? substituir, string dreCodigo = "", string[] turmaIds = null, int? anoLetivo = null)
        {

            var query = new StringBuilder();

            query.AppendLine("select a.*, t.*");
            query.AppendLine("from");
            query.AppendLine("atribuicao_cj a");
            query.AppendLine("inner join turma t");
            query.AppendLine("on t.turma_id = a.turma_id");
            query.AppendLine("left join usuario u");
            query.AppendLine("on u.rf_codigo = a.professor_rf");
            query.AppendLine("where 1 = 1");

            if (modalidade != default)
                query.AppendLine("and a.modalidade = @modalidade");

            if (!string.IsNullOrEmpty(ueId))
                query.AppendLine("and a.ue_id = @ueId");

            if (!string.IsNullOrEmpty(turmaId))
                query.AppendLine("and a.turma_id = @turmaId");

            if (componenteCurricularId > 0)
                query.AppendLine("and a.disciplina_id = @componenteCurricularId");

            if (!string.IsNullOrEmpty(usuarioRf))
                query.AppendLine("and a.professor_rf = @usuarioRf");

            if (!string.IsNullOrEmpty(usuarioNome))
            {
                usuarioNome = $"%{usuarioNome.ToUpper()}%";
                query.AppendLine("and upper(f_unaccent(u.nome)) LIKE @usuarioNome");
            }

            if (substituir.HasValue)
                query.AppendLine("and a.substituir = @substituir");

            if (!string.IsNullOrEmpty(dreCodigo))
                query.AppendLine("and a.dre_id = @dreCodigo");

            if (turmaIds != null)
                query.AppendLine("and t.turma_id = ANY(@turmaIds)");

            if (anoLetivo != null)
                query.AppendLine("and t.ano_letivo = @anoLetivo");

            return query.ToString();

        }
    }
}
