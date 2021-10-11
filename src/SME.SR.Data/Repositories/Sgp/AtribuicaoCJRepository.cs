using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AtribuicaoCJRepository : IAtribuicaoCJRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AtribuicaoCJRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AtribuicaoCJ>> ObterPorFiltros(Modalidade? modalidade, string turmaId, string ueId, long componenteCurricularId, string usuarioRf,
                                                                     string usuarioNome, bool? substituir, string dreCodigo = "", string[] turmaIds = null,
                                                                     long[] componentesCurricularresId = null, int? anoLetivo = null, int? semestre = null)
        {
            var query = new StringBuilder();

            query.AppendLine("select a.disciplina_id ComponenteCurricularId,");
            query.AppendLine("a.dre_id DreId, a.ue_id UeId, a.migrado, a.criado_em CriadoEm, ");
            query.AppendLine("cc.descricao ComponenteCurricularNome,");
            query.AppendLine("t.modalidade_codigo Modalidade, ");
            query.AppendLine("a.professor_rf professorrf, u.nome ProfessorNome, a.substituir, ");
            query.AppendLine("t.turma_id Codigo, t.nome, ");
            query.AppendLine("t.modalidade_codigo ModalidadeCodigo, ");
            query.AppendLine("t.semestre, t.ano, t.ano_letivo AnoLetivo");
            query.AppendLine("from");
            query.AppendLine("atribuicao_cj a");
            query.AppendLine("inner join turma t");
            query.AppendLine("on t.turma_id = a.turma_id");
            query.AppendLine("left join componente_curricular cc");
            query.AppendLine("on a.disciplina_id = cc.id");
            query.AppendLine("left join usuario u");
            query.AppendLine("on u.rf_codigo = a.professor_rf");
            query.AppendLine("where 1 = 1");

            if (modalidade.HasValue)
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

            if (componentesCurricularresId != null)
                query.AppendLine("and a.disciplina_id = ANY(@componentesCurricularresId)");

            if (anoLetivo != null)
                query.AppendLine("and t.ano_letivo = @anoLetivo");

            if (semestre.HasValue)
                query.AppendLine("and t.semestre = @semestre");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<AtribuicaoCJ, Turma, AtribuicaoCJ>(query.ToString(), (atribuicaoCJ, turma) =>
                {
                    atribuicaoCJ.Turma = turma;
                    return atribuicaoCJ;
                }, new
                {
                    modalidade = modalidade.HasValue ? (int)modalidade : 0,
                    ueId,
                    turmaId,
                    componenteCurricularId,
                    usuarioRf,
                    usuarioNome,
                    substituir,
                    dreCodigo,
                    turmaIds,
                    componentesCurricularresId,
                    anoLetivo,
                    semestre
                }, splitOn: "ComponenteCurricularId,Codigo"));
            }
        }
    }
}
