using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioControleGradeAnaliticoCommandHandler : IRequestHandler<GerarRelatorioControleGradeAnaliticoCommand, bool>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleGradeAnaliticoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarRelatorioControleGradeAnaliticoCommand request, CancellationToken cancellationToken)
        {
            var dto = new ControleGradeDto();

            var modalidadeCalendario = request.Filtros.ModalidadeTurma == Modalidade.EJA ?
                                                ModalidadeTipoCalendario.EJA : request.Filtros.ModalidadeTurma == Modalidade.Infantil ?
                                                    ModalidadeTipoCalendario.Infantil : ModalidadeTipoCalendario.FundamentalMedio;
            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(request.Filtros.AnoLetivo, modalidadeCalendario, request.Filtros.Semestre));

            foreach (long turmaId in request.Filtros.Turmas)
            {
                var aulasPrevistasTurma = new List<AulaPrevistaBimestreQuantidade>();
                if (request.Filtros.ComponentesCurriculares.Any())
                {
                    foreach (long componenteCurricularId in request.Filtros.ComponentesCurriculares)
                    {
                        var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));
                        var componentesCurriculares = await ObterComponentesCurriculares(request.Filtros.ComponentesCurriculares, turma.Codigo);
                        var aulasPrevistasBimestresComponente = await mediator.Send(new ObterAulasPrevistasDadasQuery(turmaId, componenteCurricularId, tipoCalendarioId));

                        if (aulasPrevistasBimestresComponente.Any())
                        {
                            foreach (var aula in aulasPrevistasBimestresComponente)
                            {
                                var componenteAula = componentesCurriculares.Where(c => c.CodDisciplina == aula.ComponenteCurricularId).FirstOrDefault();
                                aula.ComponenteCurricularNome = componenteAula.Disciplina;
                                aulasPrevistasTurma.Add(aula);
                            }
                        }
                    }
                }
                else
                {
                    var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));
                    var componentesDaTurma = await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery(turma.Codigo));

                    foreach (long componenteCurricularId in componentesDaTurma.Select(c => c.CodDisciplina))
                    {
                        var aulasPrevistasBimestresComponente = await mediator.Send(new ObterAulasPrevistasDadasQuery(turmaId, componenteCurricularId, tipoCalendarioId));

                        if (aulasPrevistasBimestresComponente.Any())
                        {
                            foreach (var aula in aulasPrevistasBimestresComponente)
                            {
                                var componenteAula = componentesDaTurma.Where(c => c.CodDisciplina == aula.ComponenteCurricularId).FirstOrDefault();
                                aula.ComponenteCurricularNome = componenteAula.Disciplina;
                                aulasPrevistasTurma.Add(aula);
                            }
                        }
                    }
                }

                if (aulasPrevistasTurma.Any())
                    dto.Turmas.Add(await MapearParaTurmaDto(aulasPrevistasTurma, request.Filtros.Bimestres, turmaId, tipoCalendarioId, request.Filtros.ModalidadeTurma));
            }

            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGradeAnalitico", dto, request.CodigoCorrelacao, "", "Relatório Controle de Grade Analítico", true, "RELATÓRIO CONTROLE DE GRADE ANALÍTICO")));
        }

        private async Task<TurmaControleGradeDto> MapearParaTurmaDto(List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, IEnumerable<int> bimestres, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var turmaDto = new TurmaControleGradeDto()
            {
                Nome = aulasPrevistasTurma.FirstOrDefault().TurmaNome
            };

            foreach (var bimestre in bimestres.OrderBy(o => o))
            {
                turmaDto.Bimestres.Add(await MapearParaBimestreDto(bimestre, aulasPrevistasTurma, turmaId, tipoCalendarioId, modalidadeTurma));
            }

            return turmaDto;
        }

        private async Task<BimestreControleGradeDto> MapearParaBimestreDto(int bimestre, List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var aulasPrevistasBimestre = aulasPrevistasTurma.Where(c => c.Bimestre == bimestre);
            bool ehEja = modalidadeTurma == Modalidade.EJA;

            var dataInicio = aulasPrevistasBimestre.FirstOrDefault().DataInicio;
            var dataFim = aulasPrevistasBimestre.FirstOrDefault().DataFim;

            var bimestreDto = new BimestreControleGradeDto()
            {
                Descricao = $"{bimestre}º Bimestre - {dataInicio:dd/MM} À {dataFim:dd/MM}"
            };

            foreach (var aulasPrevistasComponente in aulasPrevistasBimestre.OrderBy(c => c.ComponenteCurricularNome))
            {
                bimestreDto.ComponentesCurriculares.Add(await MapearParaComponenteDto(aulasPrevistasComponente, turmaId, tipoCalendarioId, modalidadeTurma, bimestre, dataInicio, dataFim, ehEja));
            }

            return bimestreDto;
        }

        private async Task<ComponenteCurricularControleGradeDto> MapearParaComponenteDto(AulaPrevistaBimestreQuantidade aulasPrevistasComponente, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma, int bimestre, DateTime dataInicio, DateTime dataFim, bool ehEja)
        {
            var componenteDto = new ComponenteCurricularControleGradeDto();

            componenteDto.Nome = aulasPrevistasComponente.ComponenteCurricularNome;
            componenteDto.AulasPrevistas = aulasPrevistasComponente.Previstas;
            componenteDto.AulasCriadasProfessorTitular = aulasPrevistasComponente.CriadasTitular;
            componenteDto.AulasCriadasProfessorSubstituto = aulasPrevistasComponente.CriadasCJ;
            componenteDto.AulasDadasProfessorTitular = aulasPrevistasComponente.CumpridasTitular;
            componenteDto.AulasDadasProfessorSubstituto = aulasPrevistasComponente.CumpridasCj;
            componenteDto.Repostas = aulasPrevistasComponente.Reposicoes;
            componenteDto.Divergencias = await VerificarDivergencias(turmaId, aulasPrevistasComponente.Bimestre, aulasPrevistasComponente.ComponenteCurricularId, aulasPrevistasComponente.Regencia, modalidadeTurma,
                componenteDto.AulasPrevistas != (aulasPrevistasComponente.CumpridasTitular + aulasPrevistasComponente.CumpridasCj), tipoCalendarioId, componenteDto.AulasPrevistas == 0) ? "Sim" : "Não";

            var detalhamentoDivergencias = new DetalhamentoDivergenciasControleGradeSinteticoDto();

            detalhamentoDivergencias.AulasNormaisExcedido = await mediator.Send(new ObterAulasNormaisExcedidasQuery(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre));
            detalhamentoDivergencias.AulasTitularCJ = await ObterAulasTitularCJ(aulasPrevistasComponente, turmaId, tipoCalendarioId, bimestre);
            detalhamentoDivergencias.AulasDuplicadas = await mediator.Send(new DetalharAulasDuplicadasPorTurmaComponenteEBimestreQuery(turmaId, aulasPrevistasComponente.ComponenteCurricularId, tipoCalendarioId, aulasPrevistasComponente.Bimestre));
            componenteDto.DetalhamentoDivergencias = detalhamentoDivergencias;
            componenteDto.VisaoSemanal = await ObterSessaoVisaoSemanal(dataInicio, dataFim, turmaId, aulasPrevistasComponente.ComponenteCurricularId, tipoCalendarioId, aulasPrevistasComponente.Regencia, ehEja);
            detalhamentoDivergencias.AulasDiasNaoLetivos = await ObterAulasDiasNaoLetivos(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre, dataInicio, dataFim);

            return componenteDto;
        }

        private async Task<List<AulaDiasNaoLetivosControleGradeDto>> ObterAulasDiasNaoLetivos(long turmaId, long tipoCalendarioId, long componenteCurricularCodigo, int bimestre, DateTime dataInicio, DateTime dataFim, bool professorCJ = false)
        {
            var data = DateTime.Today;

            var diasComEvento = await mediator.Send(new ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery(tipoCalendarioId, dataInicio, dataFim, turmaId));

            var aulas = await mediator.Send(new ObterAulaReduzidaPorTurmaComponenteEBimestreQuery(turmaId, tipoCalendarioId, componenteCurricularCodigo, bimestre));

            var diasComEventosNaoLetivos = diasComEvento.Where(e => e.EhNaoLetivo);

            var aulasDiasNaoLetivos = new List<AulaDiasNaoLetivosControleGradeDto>();
            if (aulas != null)
            {
                aulas.Where(a => diasComEventosNaoLetivos.Any(d => d.Data == a.Data)).ToList()
                    .ForEach(aula =>
                    {
                        foreach (var eventoNaoLetivo in diasComEventosNaoLetivos.Where(d => d.Data == aula.Data))
                        {
                            aulasDiasNaoLetivos.Add(new AulaDiasNaoLetivosControleGradeDto()
                            {
                                Data = $"{aula.Data:dd/MM/yyyy}",
                                Professor = $"{aula.Professor} ({aula.ProfessorRf})",
                                QuantidadeAulas = aula.Quantidade,
                                Motivo = eventoNaoLetivo.Motivo
                            });
                        }
                    });
            }
            return aulasDiasNaoLetivos;
        }

        private async Task<IEnumerable<VisaoSemanalControleGradeSinteticoDto>> ObterSessaoVisaoSemanal(DateTime dataInicio, DateTime dataFim, long turmaId, long componenteCurricularId, long tipoCalendarioId, bool regencia, bool ehEja)
        {
            var visaoSemanal = new List<VisaoSemanalControleGradeSinteticoDto>();
            var quantidadeGrade = await mediator.Send(new ObterQuantidadeDeAulaGradeQuery(turmaId, componenteCurricularId));

            if (quantidadeGrade == 0 && regencia)
                quantidadeGrade = 5;
            
            if (quantidadeGrade == 0 && ehEja)
                quantidadeGrade = 25;

            var eventosCadastrados = await mediator.Send(new ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQuery(tipoCalendarioId, dataInicio, dataFim, turmaId));

            var dataSegundaFeira = ObterUltimaSegundaFeira(dataInicio);
            visaoSemanal.Add(await ObterVisaoSemanal(dataSegundaFeira, quantidadeGrade, turmaId, componenteCurricularId, eventosCadastrados, dataSegundaFeira));

            for (var data = dataSegundaFeira.AddDays(7); data <= dataFim; data = data.AddDays(7))
            {
                visaoSemanal.Add(await ObterVisaoSemanal(data, quantidadeGrade, turmaId, componenteCurricularId, eventosCadastrados));
            }

            return visaoSemanal;
        }

        private DateTime ObterUltimaSegundaFeira(DateTime data)
        {
            switch (data.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return data.AddDays(1);
                case DayOfWeek.Monday:
                    return data;
                default:
                    return data.AddDays(((int)data.DayOfWeek - 1) * -1);
            }
        }

        private async Task<VisaoSemanalControleGradeSinteticoDto> ObterVisaoSemanal(DateTime dataInicioSemana, int quantidadeGrade, long turmaId, long componenteCurricularId, IEnumerable<Evento> eventosCadastrados, DateTime? dataParaExibicao = null)
        {
            var ultimoDomingo = dataInicioSemana.AddDays(-1);
            var dataFimSemana = ultimoDomingo.AddDays(6);
            var aulasCriadas = await mediator.Send(new ObterDiasAulaCriadasPeriodoInicioEFimQuery(turmaId, componenteCurricularId, ultimoDomingo, dataFimSemana));
            var diasLetivos = ObtemDiasLetivos(ultimoDomingo, dataFimSemana, eventosCadastrados);

            var dataParaVisao = dataParaExibicao.HasValue ? dataParaExibicao : dataInicioSemana;
            return new VisaoSemanalControleGradeSinteticoDto()
            {
                Data = $"{dataParaVisao:dd/MM/yyyy}",
                QuantidadeGrade = quantidadeGrade,
                DiasLetivo = diasLetivos,
                AulasCriadas = aulasCriadas,
                Diferenca = quantidadeGrade != aulasCriadas ? "Sim" : "Não"
            };
        }

        private int ObtemDiasLetivos(DateTime dataInicio, DateTime dataFim, IEnumerable<Evento> eventosCadastrados)
        {
            int diasLetivos = 0;
            for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
            {
                if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (eventosCadastrados.FirstOrDefault(a => a.DataInicio == data && a.EhEventoLetivo()) != null)
                        diasLetivos += 1;
                }
                else
                {
                    diasLetivos += 1;
                    if (eventosCadastrados.FirstOrDefault(a => a.DataInicio == data && a.EhEventoNaoLetivo()) != null)
                        diasLetivos -= 1;
                }
            }


            return diasLetivos;
        }

        private bool VerificaDataEhSegunda(DateTime data)
        {
            return data.DayOfWeek == DayOfWeek.Monday;

        }
        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(IEnumerable<long> componentesCurriculares, string turmaId)
        {
            string[] turmaCodigo = { turmaId };
            var componentes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesCurriculares.ToArray(), turmaCodigo));
            return componentes;
        }

        private async Task<List<AulaTitularCJDataControleGradeDto>> ObterAulasTitularCJ(AulaPrevistaBimestreQuantidade aulasPrevistasComponente, long turmaId, long tipoCalendarioId, int bimestre)
        {
            var aulasTitular = await mediator.Send(new ObterAulaReduzidaPorTurmaComponenteEBimestreQuery(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre));
            var aulasCJ = await mediator.Send(new ObterAulaReduzidaPorTurmaComponenteEBimestreQuery(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre, true));

            var aulasTitularCJData = new List<AulaTitularCJDataControleGradeDto>();

            var datas = aulasTitular.Select(a => a.Data);
            datas.Union(aulasCJ.Select(a => a.Data));

            foreach (var data in datas.Distinct())
            {
                if(aulasTitular.Where(a => a.Data == data).Any() && aulasCJ.Where(a => a.Data == data).Any())
                {
                    var aulaTitularCJData = new AulaTitularCJDataControleGradeDto()
                    {
                        Data = data.ToString("dd/MM/yyyy")
                    };

                    var divergencias = new List<AulaTitularCJControleGradeDto>();
                    foreach (var aula in aulasTitular.Where(a => a.Data == data))
                    {
                        divergencias.Add(new AulaTitularCJControleGradeDto()
                        {
                            QuantidadeAulas = aula.Quantidade,
                            ProfessorTitular = $"{aula.Professor} ({aula.ProfessorRf})"
                        });
                    }

                    foreach (var aula in aulasCJ.Where(a => a.Data == data))
                    {
                        divergencias.Add(new AulaTitularCJControleGradeDto()
                        {
                            QuantidadeAulas = aula.Quantidade,
                            ProfessorCJ = $"{aula.Professor} ({aula.ProfessorRf})"
                        });
                    }

                    aulaTitularCJData.Divergencias = divergencias;
                    aulasTitularCJData.Add(aulaTitularCJData);
                }
            }
            

            return aulasTitularCJData;
        }

        private async Task<bool> VerificarDivergencias(long turmaId, int bimestre, long componenteCurricularId, bool regencia, Modalidade modalidadeTurma, bool divergenciaNumeroAulas, long tipoCalendarioId, bool aulasPrevistasvZero)
        {
            //Não existe aulas previstas cadastradas.
            if (aulasPrevistasvZero)
                return true;

            //Diferença entre aulas previstas X aulas dadas.
            if (divergenciaNumeroAulas)
                return true;

            //Não há nenhuma aula criada para o componente curricular.
            var verificaExisteAula = await mediator.Send(new VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery(turmaId, componenteCurricularId.ToString(), bimestre, tipoCalendarioId));
            if (!verificaExisteAula)
                return true;

            //2 ou mais aulas normais criadas no mesmo dia por um professor de regência de classe do fundamental na mesma turma
            if (regencia)
            {
                var verificaAulaCriadaProfessor = await mediator.Send(new VerificarAulasNormaisCriadasProfessorRegenciaQuery(turmaId, componenteCurricularId.ToString(), bimestre, tipoCalendarioId));
                if (!verificaAulaCriadaProfessor)
                    return true;
            }

            //Mais de um registro de aula do mesmo professor, componente curricular e turma no mesmo dia.           
            var verificaExisteMaisAula = await mediator.Send(new VerificaExisteMaisAulaCadastradaNoDiaQuery(turmaId, componenteCurricularId.ToString(), tipoCalendarioId, bimestre));
            if (verificaExisteMaisAula)
                return true;

            if (await mediator.Send(new VerificaExisteAulaTitularECjQuery(turmaId, componenteCurricularId, tipoCalendarioId, bimestre)))
                return true;

            if (await VerificaAulasMesmoDiaEProfessor(turmaId, componenteCurricularId, tipoCalendarioId, bimestre, regencia, modalidadeTurma))
                return true;

            return false;
        }

        private async Task<bool> VerificaAulasMesmoDiaEProfessor(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre, bool regencia, Modalidade modalidadeTurma)
        {
            if (!regencia && !(modalidadeTurma == Modalidade.EJA) &&
                await mediator.Send(new VerificaExisteAulasMemoDiaEProfessorQuery(turmaId, componenteCurricularId, tipoCalendarioId, bimestre)))
                return true;

            return false;
        }

        private async Task MontarCabecalhoRelatorioDto(ControleGradeDto dto, RelatorioControleGradeFiltroDto filtros)
        {
            var turmaId = filtros.Turmas.First();
            var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));

            dto.Filtro.Dre = turma.Ue.Dre.Abreviacao;
            dto.Filtro.Ue = $"{turma.Ue.CodigoUe} - {turma.Ue.TipoEscola.ShortName()} {turma.Ue.Nome}";
            dto.Filtro.Turma = filtros.Turmas.Count() > 1 ? "Todas" : $"{turma.Modalidade.ShortName()} - {turma.Nome}";
            dto.Filtro.Bimestre = filtros.Bimestres.Count() == QuantidadePeriodosPorModalidade(turma.Modalidade) ?
                                    "Todos" : string.Join(",", filtros.Bimestres);
            dto.Filtro.ComponenteCurricular = ObterNomeComponente(filtros, dto);
            dto.Filtro.Usuario = filtros.UsuarioNome;
            dto.Filtro.RF = filtros.UsuarioRf;
            dto.Filtro.EhEducacaoInfantil = filtros.ModalidadeTurma == Modalidade.Infantil;  
    }

        private int QuantidadePeriodosPorModalidade(Modalidade modalidade)
            => modalidade == Modalidade.EJA ? 2 : 4;

        private string ObterNomeComponente(RelatorioControleGradeFiltroDto filtros, ControleGradeDto dto)
        {
            return filtros.ComponentesCurriculares.Count() > 1 ?
                            "Todos" : !dto.Turmas.Any()
                                   || !dto.Turmas.First().Bimestres.Any()
                                   || !dto.Turmas.First().Bimestres.First().ComponentesCurriculares.Any() ?
                                "" : dto.Turmas.First().Bimestres.First().ComponentesCurriculares.First().Nome;
        }
    }
}
