using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioCompensacaoAusenciaObterResultadoFinalQueryHandler : IRequestHandler<RelatorioCompensacaoAusenciaObterResultadoFinalQuery, RelatorioCompensacaoAusenciaDto>
    {
        public Task<RelatorioCompensacaoAusenciaDto> Handle(RelatorioCompensacaoAusenciaObterResultadoFinalQuery request, CancellationToken cancellationToken)
        {
            var result = new RelatorioCompensacaoAusenciaDto();

            MontaCabecalho(request.Filtros, request.Ue, request.Dre, request.Compensacoes, request.ComponentesCurriculares, result);

            var dreParaAdicionar = new RelatorioCompensacaoAusenciaDreDto() { Codigo = request.Dre.Codigo, Nome = request.Dre.Nome };

            var ueParaAdicionar = new RelatorioCompensacaoAusenciaUeDto() { Codigo = request.Ue.Codigo, Nome = request.Ue.Nome };

            foreach (var compensacoesTurma in request.Compensacoes.GroupBy(a => a.TurmaNome))
            {
                var turmaParaAdicionar = new RelatorioCompensacaoAusenciaTurmaDto();
                turmaParaAdicionar.Nome = compensacoesTurma.Key;

                var bimestresDaTurma = compensacoesTurma.Select(a => a.Bimestre).Distinct();

                foreach (var bimestreDaTurma in bimestresDaTurma)
                {
                    var bimestreParaAdicionar = new RelatorioCompensacaoAusenciaBimestreDto();
                    bimestreParaAdicionar.Nome = $"{bimestreDaTurma}º BIMESTRE";

                    foreach (var componentesDaTurmaBimestre in compensacoesTurma.Where(a => a.Bimestre == bimestreDaTurma).Select(a => a.DisciplinaId).Distinct())
                    {
                        var componenteParaAdicionar = new RelatorioCompensacaoAusenciaComponenteDto();
                        componenteParaAdicionar.CodigoComponente = componentesDaTurmaBimestre.ToString();
                        componenteParaAdicionar.NomeComponente = request.ComponentesCurriculares.FirstOrDefault(a => a.CodDisciplina == componentesDaTurmaBimestre)?.Disciplina;

                        foreach (var atividadeDaTurmaBimestre in compensacoesTurma.Where(a => a.Bimestre == bimestreDaTurma && a.DisciplinaId == componentesDaTurmaBimestre).GroupBy(a => a.AtividadeNome))
                        {
                            var atividadeParaAdicionar = new RelatorioCompensacaoAusenciaAtividadeDto();
                            atividadeParaAdicionar.Nome = atividadeDaTurmaBimestre.Key.ToUpper();

                            foreach (var alunoAtividade in atividadeDaTurmaBimestre)
                            {
                                var alunoCompensacaoParaAdicionar = new RelatorioCompensacaoAusenciaCompensacaoAlunoDto();

                                var alunoParaAdicionar = request.Alunos.FirstOrDefault(a => a.CodigoAluno == alunoAtividade.AlunoCodigo);

                                alunoCompensacaoParaAdicionar.NomeAluno = alunoParaAdicionar?.ObterNomeFinal();
                                alunoCompensacaoParaAdicionar.NumeroChamada = alunoParaAdicionar?.NumeroAlunoChamada;

                                var frequenciaAluno = request.Frequencias.FirstOrDefault(a => a.CodigoAluno == alunoAtividade.AlunoCodigo.ToString() &&
                                                                a.TurmaId == alunoAtividade.TurmaCodigo && a.Bimestre == bimestreDaTurma &&
                                                                a.DisciplinaId == componentesDaTurmaBimestre.ToString());
                                if (frequenciaAluno != null)
                                {
                                    alunoCompensacaoParaAdicionar.TotalAulas = frequenciaAluno.TotalAulas;
                                    alunoCompensacaoParaAdicionar.TotalAusencias = frequenciaAluno.TotalAusencias;
                                    alunoCompensacaoParaAdicionar.TotalCompensacoes = frequenciaAluno.TotalCompensacoes;
                                }
                                atividadeParaAdicionar.CompensacoesAluno.Add(alunoCompensacaoParaAdicionar);
                            }
                            atividadeParaAdicionar.CompensacoesAluno = atividadeParaAdicionar.CompensacoesAluno.OrderBy(a => a.NomeAluno).ToList();

                            componenteParaAdicionar.Atividades.Add(atividadeParaAdicionar);
                        }
                        componenteParaAdicionar.Atividades = componenteParaAdicionar.Atividades.OrderBy(a => a.Nome).ToList();
                        bimestreParaAdicionar.Componentes.Add(componenteParaAdicionar);
                    }

                    bimestreParaAdicionar.Componentes = bimestreParaAdicionar.Componentes.OrderBy(a => a.NomeComponente).ToList();
                    turmaParaAdicionar.Bimestres.Add(bimestreParaAdicionar);
                }
                turmaParaAdicionar.Bimestres = turmaParaAdicionar.Bimestres.OrderBy(a => a.Nome).ToList();
                ueParaAdicionar.Turmas.Add(turmaParaAdicionar);
            }
            
            ueParaAdicionar.Turmas = ueParaAdicionar.Turmas.OrderBy(a => a.Nome).ToList();
            dreParaAdicionar.Ue = ueParaAdicionar;
            result.Dre = dreParaAdicionar;

            return Task.FromResult(result);
        }

        private static void MontaCabecalho(FiltroRelatorioCompensacaoAusenciaDto filtros, Data.Ue ue, Data.Dre dre, System.Collections.Generic.IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta> compensacoes, System.Collections.Generic.IEnumerable<Data.ComponenteCurricularPorTurma> componentesCurriculares, RelatorioCompensacaoAusenciaDto result)
        {
            result.Bimestre = filtros.Bimestre > 0 ? filtros.Bimestre.ToString() : "Todos";


            if (filtros.ComponentesCurriculares != null && filtros.ComponentesCurriculares.Any())
            {
                if (filtros.ComponentesCurriculares.Count() == 1)
                {
                    result.ComponenteCurricular = componentesCurriculares.FirstOrDefault(a => a.CodDisciplina == filtros.ComponentesCurriculares.FirstOrDefault())?.Disciplina;
                }
                else result.ComponenteCurricular = "";

            }
            else result.ComponenteCurricular = "Todos";

            result.Data = DateTime.Today.ToString("dd/MM/yyyy");
            result.Modalidade = filtros.Modalidade.Name();
            result.RF = filtros.UsuarioRf;

            if (filtros.TurmasCodigo != null && filtros.TurmasCodigo.Any())
                result.TurmaNome = $"{filtros.Modalidade.ShortName()} - {compensacoes.FirstOrDefault(a => a.TurmaCodigo == filtros.TurmasCodigo[0])?.TurmaNome}";
            else result.TurmaNome = "Todas"; 

            result.UeNome = ue.NomeComTipoEscola;
            result.DreNome = dre.Abreviacao;
            result.Usuario = filtros.UsuarioNome;
        }
    }
}
