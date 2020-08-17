using MediatR;
using RazorEngine.Compilation.ImpromptuInterface.Optimization;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioNotasEConceitosFinaisUseCase : IRelatorioNotasEConceitosFinaisUseCase
    {
        private readonly IMediator mediator;

        public RelatorioNotasEConceitosFinaisUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioNotasEConceitosFinaisDto>();

            var dres = new List<Dre>();

            if (string.IsNullOrEmpty(filtros.DreCodigo))
            {
                var dresRetorno = await mediator.Send(new ObterTodasDresQuery());
                if (dresRetorno == null || !dresRetorno.Any())
                    throw new NegocioException("Não foi possível obter as Dres.");

                dres = dresRetorno.ToList();
            }
            else
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                if (dre == null)
                    throw new NegocioException("Não foi possível obter a Dre.");

                dres.Add(dre);
            }

            var dresIds = dres.Select(d => d.Id).ToArray();

            var ues = await mediator.Send(new ObterPorDresIdQuery(dresIds));


            RelatorioNotasEConceitosFinaisDto relatorioNotasEConceitosFinaisDto = new RelatorioNotasEConceitosFinaisDto();

            MontarCabecalho(filtros, relatorioNotasEConceitosFinaisDto);

            var turmas = await mediator.Send(new ObterTurmasPorUeAnosModalidadeSemestreQuery(filtros.UeCodigo, filtros.Anos, filtros.Modalidade, filtros.Semestre));

            if (!turmas.Any())
                throw new NegocioException("Não foi possível localizar turmas para geração do relatório");

            // Obter dados das turmas
            foreach (var turma in turmas)
            {
                var alunosTurma = await mediator.Send(new ObterAlunosPorTurmaNotasConceitosQuery() { TurmaCodigo = turma.Codigo });

                if (alunosTurma.Any() && alunosTurma != null)
                {
                    var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorCodigoETurmaQuery(turma.Codigo, filtros.ComponentesCurriculares));

                    var notasPorTurma = await mediator.Send(new ObterNotasFinaisPorTurmaBimestreQuery(turma.Codigo, filtros.Bimestres.ToArray()));

                    if (notasPorTurma != null && notasPorTurma.Any())
                    {
                        var ueDaTurma = ues.FirstOrDefault(a => a.Id == turma.UeId);

                        if (ueDaTurma == null)
                            throw new NegocioException("Não foi possível encontrar uma UE para a turma");

                        var dreDaTurma = dres.FirstOrDefault(a => a.Id == ueDaTurma.DreId);

                        if (dreDaTurma == null)
                            throw new NegocioException("Não foi possível encontrar uma DRE para a turma");

                        ObterDadosBaseRelatorio(filtros, dreDaTurma, ueDaTurma, turma, relatorioNotasEConceitosFinaisDto, out RelatorioNotasEConceitosFinaisBimestreDto relatorioNotasEConceitosFinaisBimestreDto);

                        if (notasPorTurma.Any())
                        {
                            ObterNotasConceitosFinais(turma, alunosTurma, componentesCurriculares, notasPorTurma, relatorioNotasEConceitosFinaisBimestreDto);
                        }
                    }
                }
            }

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotasEConceitosFinais", relatorioNotasEConceitosFinaisDto, request.CodigoCorrelacao));
        }

        private static void MontarCabecalho(FiltroRelatorioNotasEConceitosFinaisDto filtros, RelatorioNotasEConceitosFinaisDto relatorioNotasEConceitosFinaisDto)
        {
            if (string.IsNullOrEmpty(filtros.DreCodigo))
                relatorioNotasEConceitosFinaisDto.DreNome = "Todas";

            if (string.IsNullOrEmpty(filtros.UeCodigo))
                relatorioNotasEConceitosFinaisDto.UeNome = "Todas";

            if (filtros.ComponentesCurriculares == null || filtros.ComponentesCurriculares.Length == 0)
                relatorioNotasEConceitosFinaisDto.ComponenteCurricular = "Todos";

            if (filtros.Bimestres == null || !filtros.Bimestres.Any())
                relatorioNotasEConceitosFinaisDto.Bimestre = "Todos";

            if (filtros.Anos == null || filtros.Anos.Length == 0)
                relatorioNotasEConceitosFinaisDto.TurmaNome = "Todos";

            relatorioNotasEConceitosFinaisDto.UsuarioNome = filtros.UsuarioNome;
            relatorioNotasEConceitosFinaisDto.UsuarioRF = filtros.UsuarioRf;            
        }

        private static void ObterNotasConceitosFinais(TurmaFiltradaUeCicloAnoDto turma, IEnumerable<Aluno> alunosTurma, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasPorTurma, RelatorioNotasEConceitosFinaisBimestreDto relatorioNotasEConceitosFinaisBimestreDto)
        {
            foreach (var componenteCurricular in componentesCurriculares.OrderBy(c => c.Disciplina))
            {
                if (!notasPorTurma.Any())
                    throw new NegocioException("Não foi possível localizar notas e conceitos para a turma");

                var relatorioNotasEConceitosFinaisComponenteCurricularDto = new RelatorioNotasEConceitosFinaisComponenteCurricularDto(componenteCurricular.Disciplina);
                relatorioNotasEConceitosFinaisBimestreDto.ComponentesCurriculares.Add(relatorioNotasEConceitosFinaisComponenteCurricularDto);

                foreach (var nota in notasPorTurma)
                {
                    foreach (var aluno in alunosTurma.OrderBy(n => n.NomeAluno))
                    {
                        var notasAluno = notasPorTurma.Where(n => n.AlunoCodigo == aluno.CodigoAluno.ToString() && n.ComponenteCurricularCodigo == componenteCurricular.CodDisciplina);
                        foreach (var notaAluno in notasAluno)
                        {
                            var nomeAluno = aluno.NomeSocialAluno == null ? aluno.NomeAluno : aluno.NomeSocialAluno;
                            var notaConceito = notaAluno.Conceito == null ? notaAluno.NotaConceito : notaAluno.Conceito;
                            var relatorioNotasEConceitosFinaisDoAlunoDto = new RelatorioNotasEConceitosFinaisDoAlunoDto(turma.Nome, aluno.NumeroAlunoChamada, nomeAluno, notaConceito);
                            relatorioNotasEConceitosFinaisComponenteCurricularDto.NotaConceitoAlunos.Add(relatorioNotasEConceitosFinaisDoAlunoDto);
                        }
                    }
                }
            }
        }

        private static void ObterDadosBaseRelatorio(FiltroRelatorioNotasEConceitosFinaisDto filtros, Dre dre, UePorDresIdResultDto ue, TurmaFiltradaUeCicloAnoDto turma, RelatorioNotasEConceitosFinaisDto relatorioNotasEConceitosFinaisDto, out RelatorioNotasEConceitosFinaisBimestreDto relatorioNotasEConceitosFinaisBimestreDto)
        {
            var relatorioNotasEConceitosFinaisDreDto = new RelatorioNotasEConceitosFinaisDreDto(dre.Codigo, dre.Nome);
            relatorioNotasEConceitosFinaisDto.Dres.Add(relatorioNotasEConceitosFinaisDreDto);

            var relatorioNotasEConceitosFinaisUeDto = new RelatorioNotasEConceitosFinaisUeDto(ue.Codigo, ue.Nome);
            relatorioNotasEConceitosFinaisDreDto.Ues.Add(relatorioNotasEConceitosFinaisUeDto);

            var relatorioNotasEConceitosFinaisAnoDto = new RelatorioNotasEConceitosFinaisAnoDto(filtros.AnoLetivo.ToString());
            relatorioNotasEConceitosFinaisUeDto.Anos.Add(relatorioNotasEConceitosFinaisAnoDto);

            relatorioNotasEConceitosFinaisBimestreDto = new RelatorioNotasEConceitosFinaisBimestreDto("Preencher");
            relatorioNotasEConceitosFinaisAnoDto.Bimestres.Add(relatorioNotasEConceitosFinaisBimestreDto);
        }
    }
}