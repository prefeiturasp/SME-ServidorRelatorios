using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RelatorioFrequenciaRepository : IRelatorioFrequenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioFrequenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioFrequenciaDreDto>> ObterFrequenciaPorAno(int anoLetivo,
                                                                                        string dreId,
                                                                                        string ueId,
                                                                                        Modalidade modalidade,
                                                                                        IEnumerable<string> anosEscolares,
                                                                                        IEnumerable<string> componentesCurriculares,
                                                                                        IEnumerable<int> bimestres,
                                                                                        TipoRelatorioFaltasFrequencia tipoRelatorio,
                                                                                        List<string> codigosTurma)
        {
            var query = new StringBuilder(@"
                select
	                d.abreviacao as NomeDre,
	                d.dre_id as CodigoDre,
	                u.ue_id as CodigoUe,
	                u.tipo_escola as TipoUe,
	                u.nome as NomeUe,");

            if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
            {
                query.AppendLine(@"case t.modalidade_codigo
    	                             when 1 then coalesce(t.serie_ensino, t.ano)
    	                             else concat(t.ano, 'º ano') 
                                   end as Nome,");
            }
            else
            {
                query.AppendLine(@"t.nome,");
            }

            query.AppendLine(@"t.modalidade_codigo as ModalidadeCodigo,                    
                    t.ano,
                    fa.bimestre NomeBimestre, 
                    fa.bimestre Numero, 
                    fa.disciplina_id CodigoComponente,
	                fa.codigo_aluno as CodigoAluno,
	                fa.total_ausencias as TotalAusencias,
	                fa.total_compensacoes as TotalCompensacoes,
	                fa.total_aulas as TotalAulas,
                    t.nome as NomeTurma,
                    t.turma_id as CodigoTurma,
                    t.ano
                from
	                frequencia_aluno fa
                inner join turma t on
	                fa.turma_id = t.turma_id
                inner join ue u on
	                t.ue_id = u.id
                inner join dre d on
	                u.dre_id = d.id
                left join componente_curricular cc on 
                    cc.id = cast(fa.disciplina_id as bigint)
                where
	                not fa.excluido and t.ano_letivo = @anoLetivo
                    and t.modalidade_codigo = @modalidade ");

            if (!string.IsNullOrWhiteSpace(dreId) && dreId != "-99")
                query.AppendLine("and d.dre_id = @dreId ");

            if (!string.IsNullOrWhiteSpace(ueId) && ueId != "-99")
                query.AppendLine("and u.ue_id = @ueId ");

            if (codigosTurma != null && codigosTurma.Any(c => c != "-99"))
                query.AppendLine("and t.turma_id = any(@codigosTurma) ");

            if (anosEscolares != null && anosEscolares.Any(c => c != "-99"))
                query.AppendLine("and t.ano = any(@anosEscolares) ");

            if (componentesCurriculares != null && componentesCurriculares.Any(c => c != "-99"))
                query.AppendLine("and disciplina_id = any(@componentesCurriculares) ");
            else query.AppendLine("and fa.disciplina_id is not null and fa.disciplina_id <> '' ");

            if (bimestres != null && bimestres.Any(c => c != 0))
                query.AppendLine("and bimestre = any(@bimestres) ");

            if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
                query.AppendLine("order by t.ano, fa.bimestre");
            else
                query.AppendLine("order by t.nome , fa.bimestre");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var dres = new List<RelatorioFrequenciaDreDto>();

                var arrayComponentes = componentesCurriculares = componentesCurriculares.Any() ? componentesCurriculares.ToArray() : new string[0] { };

                await conexao.QueryAsync(query.ToString(), (Func<RelatorioFrequenciaDreDto, RelatorioFrequenciaUeDto, RelatorioFrequenciaTurmaAnoDto, RelatorioFrequenciaBimestreDto, RelatorioFrequenciaComponenteDto, RelatorioFrequenciaAlunoDto, RelatorioFrequenciaDreDto>)((dre, ue, ano, bimestre, componente, aluno) =>
                {
                    dre = ObterDre(dre, dres);
                    ue = ObterUe(dre, ue);
                    ano = ObterAno(ue, ano);
                    bimestre = ObterBimestre(ano, bimestre);
                    componente = ObterComponente(bimestre, componente);
                    componente.Alunos.Add(aluno);

                    return dre;
                }), splitOn: "CodigoDre, CodigoUe, Nome,NomeBimestre, CodigoComponente,CodigoAluno",
                param: new
                {
                    anoLetivo,
                    dreId,
                    ueId,
                    modalidade,
                    anosEscolares = anosEscolares != null ? anosEscolares.ToArray() : null,
                    componentesCurriculares = arrayComponentes,
                    bimestres = bimestres.ToArray(),
                    codigosTurma
                });

                return dres;
            };
        }

        private String QueryObterTotalAulasPorDisciplinaTurmaBimestre(int bimestre, string disciplinaId, string[] turmasId)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("select ");
            query.AppendLine("COALESCE(SUM(a.quantidade),0) AS total");
            query.AppendLine("from ");
            query.AppendLine("aula a ");
            query.AppendLine("inner join registro_frequencia rf on ");
            query.AppendLine("rf.aula_id = a.id ");
            query.AppendLine("inner join periodo_escolar p on ");
            query.AppendLine("a.tipo_calendario_id = p.tipo_calendario_id ");
            query.AppendLine("where not a.excluido");
            query.AppendLine("and p.bimestre = @bimestre ");
            query.AppendLine("and a.data_aula >= p.periodo_inicio");
            query.AppendLine("and a.data_aula <= p.periodo_fim ");

            if (!string.IsNullOrWhiteSpace(disciplinaId))
                query.AppendLine("and a.disciplina_id = @disciplinaId ");

            query.AppendLine("and a.turma_id = ANY(@turmasId) ");
            return query.ToString();
        }

        public async Task<int> ObterTotalAulasPorDisciplinaTurmaBimestre(int bimestre, string disciplinaId, string[] turmasId)
        {
            String query = QueryObterTotalAulasPorDisciplinaTurmaBimestre(bimestre, disciplinaId, turmasId);
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<int>(query.ToString(), new { bimestre, disciplinaId, turmasId });
            }
        }

        private static RelatorioFrequenciaComponenteDto ObterComponente(RelatorioFrequenciaBimestreDto bimestre, RelatorioFrequenciaComponenteDto componente)
        {
            var componenteSelecionado = bimestre.Componentes.FirstOrDefault(c => c.CodigoComponente == componente.CodigoComponente);
            if (componenteSelecionado != null)
                componente = componenteSelecionado;
            else
                bimestre.Componentes.Add(componente);
            return componente;
        }

        private static RelatorioFrequenciaBimestreDto ObterBimestre(RelatorioFrequenciaTurmaAnoDto ano, RelatorioFrequenciaBimestreDto bimestre)
        {
            var bimestreSelecionado = ano.Bimestres.FirstOrDefault(c => c.NomeBimestre == bimestre.NomeBimestre);
            if (bimestreSelecionado != null)
                bimestre = bimestreSelecionado;
            else
                ano.Bimestres.Add(bimestre);
            return bimestre;
        }

        private static RelatorioFrequenciaTurmaAnoDto ObterAno(RelatorioFrequenciaUeDto ue, RelatorioFrequenciaTurmaAnoDto ano)
        {
            var anoSelecionado = ue.TurmasAnos.FirstOrDefault(c => c.Nome == ano.Nome);
            if (anoSelecionado != null)
                ano = anoSelecionado;
            else
                ue.TurmasAnos.Add(ano);
            return ano;
        }

        private static RelatorioFrequenciaUeDto ObterUe(RelatorioFrequenciaDreDto dre, RelatorioFrequenciaUeDto ue)
        {
            var ueSelecionada = dre.Ues.FirstOrDefault(c => c.CodigoUe == ue.CodigoUe);
            if (ueSelecionada != null)
                ue = ueSelecionada;
            else
            {
                ue.NomeUe = $"{ue.TipoUe.ShortName()} {ue.NomeUe}";
                dre.Ues.Add(ue);
            }


            return ue;
        }

        private static RelatorioFrequenciaDreDto ObterDre(RelatorioFrequenciaDreDto dre, List<RelatorioFrequenciaDreDto> dres)
        {
            var dreSelecionada = dres.FirstOrDefault(c => c.CodigoDre == dre.CodigoDre);
            if (dreSelecionada != null)
                dre = dreSelecionada;
            else
                dres.Add(dre);
            return dre;
        }
    }
}
