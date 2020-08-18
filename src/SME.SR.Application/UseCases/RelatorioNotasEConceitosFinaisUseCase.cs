using MediatR;
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
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioNotasEConceitosFinaisDto>();
                var relatorioNotasEConceitosFinaisDto = new RelatorioNotasEConceitosFinaisDto();

                // Dres
                List<Dre> dres = await AplicarFiltroPorDre(filtros);
                var dresIds = dres.Select(d => d.Id).ToArray();

                // Ues
                var ues = await mediator.Send(new ObterPorDresIdQuery(dresIds));
                var uesCodigos = await AplicarFiltroPorUe(filtros, ues.OrderBy(u => u.TipoEscola));

                // Cabeçalho
                MontarCabecalho(filtros, relatorioNotasEConceitosFinaisDto);

                // Obter turmas por códigos da UE, Anos, Modalidade e semestre
                var turmas = await mediator.Send(new ObterTurmasPorUeAnosModalidadeSemestreQuery(uesCodigos, filtros.Anos, filtros.Modalidade, filtros.Semestre));

                if (!turmas.Any())
                    throw new NegocioException("Não foi possível localizar turmas para geração do relatório");

                // Obter dados das turmas
                foreach (var turma in turmas)
                {

                    // Obter alunos de uma turma
                    var alunosTurma = await mediator.Send(new ObterAlunosPorTurmaNotasConceitosQuery() { TurmaCodigo = turma.Codigo });

                    if (alunosTurma.Any() && alunosTurma != null)
                    {

                        // Obter componentes curriculares de uma turma 
                        // TODO: Não está retornando componentes quando passamos uma lista de componentes na consulta.
                        var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorCodigoETurmaQuery(turma.Codigo, filtros.ComponentesCurriculares));

                        if (componentesCurriculares != null && componentesCurriculares.Any())
                        {
                            //TODO: Aplicar filtro por critério de Condições e Valores

                            // Obter notas por turma, bimestres
                            var notasPorTurma = await mediator.Send(new ObterNotasFinaisPorTurmaBimestreQuery(turma.Codigo, filtros.Bimestres.ToArray()));

                            if (notasPorTurma != null && notasPorTurma.Any())
                            {
                                var ueDaTurma = ues.Where(a => a.Id == turma.UeId).FirstOrDefault();
                                if (ueDaTurma != null)
                                {
                                    var dreDaTurma = dres.Where(a => a.Id == ueDaTurma.DreId).FirstOrDefault();
                                    if (dreDaTurma != null)
                                    {
                                        ObterDadosBaseRelatorio(filtros, dreDaTurma, ueDaTurma, relatorioNotasEConceitosFinaisDto, out RelatorioNotasEConceitosFinaisBimestreDto relatorioNotasEConceitosFinaisBimestreDto);

                                        if (notasPorTurma != null && notasPorTurma.Any() && componentesCurriculares != null && componentesCurriculares.Any())
                                            ObterNotasConceitosFinais(filtros, turma, alunosTurma, componentesCurriculares, notasPorTurma, relatorioNotasEConceitosFinaisBimestreDto);
                                    }
                                }
                            }
                        }
                    }
                }

                if (relatorioNotasEConceitosFinaisDto.Dres.Count == 0)
                    throw new NegocioException("Não encontramos dados para geração do relatório!");

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotasEConceitosFinais", relatorioNotasEConceitosFinaisDto, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw new NegocioException("Não foi possível gerar o relatório");
            }
        }



        private async Task<List<Dre>> AplicarFiltroPorDre(FiltroRelatorioNotasEConceitosFinaisDto filtros)
        {
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

            return dres;
        }

        private async Task<string[]> AplicarFiltroPorUe(FiltroRelatorioNotasEConceitosFinaisDto filtros, IEnumerable<UePorDresIdResultDto> ues)
        {
            string[] uesCodigos;
            if (!string.IsNullOrEmpty(filtros.UeCodigo))
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                if (ue == null)
                    throw new NegocioException("Não foi possível obter a Ue.");

                var codigos = new List<string>
                    {
                        filtros.UeCodigo
                    };
                uesCodigos = codigos.ToArray();
            }
            else
            {
                uesCodigos = ues.Select(u => u.Codigo).ToArray();
            }
            return uesCodigos;
        }

        private static void ObterNotasConceitosFinais(FiltroRelatorioNotasEConceitosFinaisDto filtros, TurmaFiltradaUeCicloAnoDto turma, IEnumerable<Aluno> alunosTurma, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasPorTurma, RelatorioNotasEConceitosFinaisBimestreDto relatorioNotasEConceitosFinaisBimestreDto)
        {
            foreach (var componenteCurricular in componentesCurriculares.OrderBy(c => c.Disciplina))
            {
                if (!notasPorTurma.Any())
                    throw new NegocioException("Não foi possível localizar notas e conceitos para a turma");

                var relatorioNotasEConceitosFinaisComponenteCurricularDto = new RelatorioNotasEConceitosFinaisComponenteCurricularDto();

                foreach (var nota in notasPorTurma)
                {
                    foreach (var aluno in alunosTurma.OrderBy(n => n.NomeAluno))
                    {
                        var notasAluno = notasPorTurma.Where(n => n.AlunoCodigo == aluno.CodigoAluno.ToString() && n.ComponenteCurricularCodigo == componenteCurricular.CodDisciplina);

                        foreach (var notaAluno in notasAluno)
                        {
                            var nomeAluno = aluno.NomeSocialAluno == null ? aluno.NomeAluno : aluno.NomeSocialAluno;
                            var notaConceito = notaAluno.Conceito == null ? notaAluno.NotaConceito : notaAluno.Conceito;
                            if (notaConceito != null)
                            {
                                if (relatorioNotasEConceitosFinaisBimestreDto.ComponentesCurriculares.Count(c => c.Nome == componenteCurricular.Disciplina) == 0)
                                {
                                    relatorioNotasEConceitosFinaisComponenteCurricularDto.Nome = componenteCurricular.Disciplina;
                                    relatorioNotasEConceitosFinaisBimestreDto.ComponentesCurriculares.Add(relatorioNotasEConceitosFinaisComponenteCurricularDto);
                                    relatorioNotasEConceitosFinaisBimestreDto.Nome = notaAluno.Bimestre.ToString();
                                }

                                var relatorioNotasEConceitosFinaisDoAlunoDto = new RelatorioNotasEConceitosFinaisDoAlunoDto(turma.Nome, aluno.NumeroAlunoChamada, nomeAluno, notaConceito);
                                relatorioNotasEConceitosFinaisComponenteCurricularDto.NotaConceitoAlunos.Add(relatorioNotasEConceitosFinaisDoAlunoDto);
                            }
                        }
                    }
                }
            }
        }

        private void ObterDadosBaseRelatorio(FiltroRelatorioNotasEConceitosFinaisDto filtros, Dre dre, UePorDresIdResultDto ue, RelatorioNotasEConceitosFinaisDto relatorioNotasEConceitosFinaisDto, out RelatorioNotasEConceitosFinaisBimestreDto relatorioNotasEConceitosFinaisBimestreDto)
        {
            // Dre
            var relatorioNotasEConceitosFinaisDreDto = new RelatorioNotasEConceitosFinaisDreDto();
            if (relatorioNotasEConceitosFinaisDto.Dres.Count(d => d.Codigo == dre.Codigo) == 0)
            {
                relatorioNotasEConceitosFinaisDreDto.Codigo = dre.Codigo;
                relatorioNotasEConceitosFinaisDreDto.Nome = dre.Nome;
                relatorioNotasEConceitosFinaisDto.Dres.Add(relatorioNotasEConceitosFinaisDreDto);
            }

            // Ue
            var relatorioNotasEConceitosFinaisUeDto = new RelatorioNotasEConceitosFinaisUeDto();
            if (relatorioNotasEConceitosFinaisDreDto.Ues.Count(u => u.Codigo == ue.Codigo) == 0)
            {
                relatorioNotasEConceitosFinaisUeDto.Codigo = ue.Codigo;
                relatorioNotasEConceitosFinaisUeDto.Nome = ue.Nome;
                relatorioNotasEConceitosFinaisDreDto.Ues.Add(relatorioNotasEConceitosFinaisUeDto);
            }

            // Ano
            var relatorioNotasEConceitosFinaisAnoDto = new RelatorioNotasEConceitosFinaisAnoDto(filtros.AnoLetivo.ToString());
            relatorioNotasEConceitosFinaisUeDto.Anos.Add(relatorioNotasEConceitosFinaisAnoDto);

            // Bimestre
            relatorioNotasEConceitosFinaisBimestreDto = new RelatorioNotasEConceitosFinaisBimestreDto("");
            relatorioNotasEConceitosFinaisAnoDto.Bimestres.Add(relatorioNotasEConceitosFinaisBimestreDto);
        }

        private async void MontarCabecalho(FiltroRelatorioNotasEConceitosFinaisDto filtros, RelatorioNotasEConceitosFinaisDto relatorioNotasEConceitosFinaisDto)
        {
            if (string.IsNullOrEmpty(filtros.DreCodigo))
            {
                relatorioNotasEConceitosFinaisDto.DreNome = "Todas";
            }
            else
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                relatorioNotasEConceitosFinaisDto.DreNome = dre.Nome;
            }


            if (string.IsNullOrEmpty(filtros.UeCodigo))
            {
                relatorioNotasEConceitosFinaisDto.UeNome = "Todas";
            }
            else
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                relatorioNotasEConceitosFinaisDto.UeNome = ue.Nome;
            }

            if (filtros.ComponentesCurriculares == null || filtros.ComponentesCurriculares.Length == 0)
                relatorioNotasEConceitosFinaisDto.ComponenteCurricular = "Todos";

            if (filtros.Bimestres == null || filtros.Bimestres.Count > 1)
                relatorioNotasEConceitosFinaisDto.Bimestre = "Todos";

            if (filtros.Anos == null || filtros.Anos.Length == 0)
                relatorioNotasEConceitosFinaisDto.TurmaNome = "Todos";

            relatorioNotasEConceitosFinaisDto.UsuarioNome = filtros.UsuarioNome;
            relatorioNotasEConceitosFinaisDto.UsuarioRF = filtros.UsuarioRf;
        }
    }
}