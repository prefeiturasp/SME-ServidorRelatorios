using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RelatorioFaltasFrequenciaRepository : IRelatorioFaltasFrequenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioFaltasFrequenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioFaltaFrequenciaDreDto>> ObterFaltasFrequenciaPorAno(int anoLetivo,
                                                                                                   string dreId,
                                                                                                   string ueId,
                                                                                                   Modalidade modalidade,
                                                                                                   IEnumerable<string> anosEscolares,
                                                                                                   IEnumerable<string> componentesCurriculares,
                                                                                                   IEnumerable<int> bimestres)
        {
            var query = new StringBuilder(@"
                select
	                d.abreviacao as NomeDre,
	                d.dre_id as CodigoDre,
	                u.ue_id as CodigoUe,
	                u.nome as NomeUe,
                    t.ano as NomeAno,
                    fa.bimestre Numero, 
                    fa.bimestre NomeBimestre, 
                    fa.disciplina_id CodigoComponente,
	                fa.codigo_aluno as CodigoAluno,
	                fa.total_ausencias as TotalAusencias,
	                fa.total_compensacoes as TotalCompensacoes,
	                fa.total_aulas as TotalAulas,
                    t.nome as NomeTurma,
                    t.turma_id as CodigoTurma
                from
	                frequencia_aluno fa
                inner join turma t on
	                fa.turma_id = t.turma_id
                inner join ue u on
	                t.ue_id = u.id
                inner join dre d on
	                u.dre_id = d.id
                where
	                not fa.excluido and t.ano_letivo = @anoLetivo
                    and t.modalidade_codigo = @modalidade ");

            if (!string.IsNullOrWhiteSpace(dreId))
            {
                query.AppendLine("and d.dre_id = @dreId");
                if (!string.IsNullOrWhiteSpace(ueId))
                    query.AppendLine("and u.ue_id = @ueId");
            }

            if (anosEscolares != null && anosEscolares.Any())
                query.AppendLine("and t.ano = any(@anosEscolares)");

            if (componentesCurriculares != null && componentesCurriculares.Any())
                query.AppendLine("and disciplina_id = any(@componentesCurriculares)");
            else query.AppendLine("and fa.disciplina_id is not null and fa.disciplina_id <> ''");

            if (bimestres != null && bimestres.Any(c => c != 0))
                query.AppendLine("and bimestre = any(@bimestres)");

            query.AppendLine("order by t.ano, fa.bimestre");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                var dres = new List<RelatorioFaltaFrequenciaDreDto>();

                var arrayComponentes = componentesCurriculares = componentesCurriculares.Any() ? componentesCurriculares.ToArray() : new string[0] { };

                await conexao.QueryAsync(query.ToString(), (Func<RelatorioFaltaFrequenciaDreDto, RelatorioFaltaFrequenciaUeDto, RelatorioFaltaFrequenciaAnoDto, RelatorioFaltaFrequenciaBimestreDto, RelatorioFaltaFrequenciaComponenteDto, RelatorioFaltaFrequenciaAlunoDto, RelatorioFaltaFrequenciaDreDto>)((dre, ue, ano, bimestre, componente, aluno) =>
                {
                    dre = ObterDre(dre, dres);
                    ue = ObterUe(dre, ue);
                    ano = ObterAno(ue, ano);
                    bimestre = ObterBimestre(ano, bimestre);
                    componente = ObterComponente(bimestre, componente);
                    componente.Alunos.Add(aluno);

                    return dre;
                }), splitOn: "CodigoDre, CodigoUe, NomeAno,NomeBimestre, CodigoComponente,CodigoAluno",
                param: new
                {
                    anoLetivo,
                    dreId,
                    ueId,
                    modalidade,
                    anosEscolares = anosEscolares.ToArray(),
                    componentesCurriculares = arrayComponentes,
                    bimestres = bimestres.ToArray()
                });

                return dres;
            };
        }

        private static RelatorioFaltaFrequenciaComponenteDto ObterComponente(RelatorioFaltaFrequenciaBimestreDto bimestre, RelatorioFaltaFrequenciaComponenteDto componente)
        {
            var componenteSelecionado = bimestre.Componentes.FirstOrDefault(c => c.CodigoComponente == componente.CodigoComponente);
            if (componenteSelecionado != null)
                componente = componenteSelecionado;
            else
                bimestre.Componentes.Add(componente);
            return componente;
        }

        private static RelatorioFaltaFrequenciaBimestreDto ObterBimestre(RelatorioFaltaFrequenciaAnoDto ano, RelatorioFaltaFrequenciaBimestreDto bimestre)
        {
            var bimestreSelecionado = ano.Bimestres.FirstOrDefault(c => c.NomeBimestre == bimestre.NomeBimestre);
            if (bimestreSelecionado != null)
                bimestre = bimestreSelecionado;
            else
                ano.Bimestres.Add(bimestre);
            return bimestre;
        }

        private static RelatorioFaltaFrequenciaAnoDto ObterAno(RelatorioFaltaFrequenciaUeDto ue, RelatorioFaltaFrequenciaAnoDto ano)
        {
            var anoSelecionado = ue.Anos.FirstOrDefault(c => c.NomeAno == ano.NomeAno);
            if (anoSelecionado != null)
                ano = anoSelecionado;
            else
                ue.Anos.Add(ano);
            return ano;
        }

        private static RelatorioFaltaFrequenciaUeDto ObterUe(RelatorioFaltaFrequenciaDreDto dre, RelatorioFaltaFrequenciaUeDto ue)
        {
            var ueSelecionada = dre.Ues.FirstOrDefault(c => c.CodigoUe == ue.CodigoUe);
            if (ueSelecionada != null)
                ue = ueSelecionada;
            else
                dre.Ues.Add(ue);
            return ue;
        }

        private static RelatorioFaltaFrequenciaDreDto ObterDre(RelatorioFaltaFrequenciaDreDto dre, List<RelatorioFaltaFrequenciaDreDto> dres)
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
