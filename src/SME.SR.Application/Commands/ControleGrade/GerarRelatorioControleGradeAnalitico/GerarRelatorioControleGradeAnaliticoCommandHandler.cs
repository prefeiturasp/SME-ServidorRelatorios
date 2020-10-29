using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
            var periodosEscolares = await ObterPeriodosEscolares(request.Filtros.Bimestres, tipoCalendarioId);
            var componentesCurriculares = await ObterComponentesCurriculares(request.Filtros.ComponentesCurriculares);
            var turmas = await ObterTurmas(request.Filtros.Turmas);
            foreach (long turmaId in request.Filtros.Turmas)
            {
                var aulasPrevistasTurma = new List<AulaPrevistaBimestreQuantidade>();
                foreach (long componenteCurricularId in request.Filtros.ComponentesCurriculares)
                {
                    var aulasPrevistasBimestresComponente = await mediator.Send(new ObterAulasPrevistasDadasQuery(turmaId, componenteCurricularId));

                    if (aulasPrevistasBimestresComponente.Any())
                    {
                        aulasPrevistasTurma.AddRange(aulasPrevistasBimestresComponente);
                    }
                    else
                    {
                        foreach (var periodoEscolar in periodosEscolares)
                        {

                            aulasPrevistasTurma.Add(new AulaPrevistaBimestreQuantidade()
                            {

                                ComponenteCurricularId = componenteCurricularId,
                                ComponenteCurricularNome = componentesCurriculares.FirstOrDefault(a => a.CodDisciplina == componenteCurricularId)?.Disciplina,
                                TurmaNome = turmas.FirstOrDefault(a => a.Codigo == turmaId.ToString()).Nome,
                                Bimestre = periodoEscolar.Bimestre,
                                DataInicio = periodoEscolar.PeriodoInicio,
                                DataFim = periodoEscolar.PeriodoFim
                            }); ;
                        }
                    }
                }

                if (aulasPrevistasTurma.Any())
                    dto.Turmas.Add(await MapearParaTurmaDto(aulasPrevistasTurma, request.Filtros.Bimestres, turmaId, tipoCalendarioId, request.Filtros.ModalidadeTurma));
            }

            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGradeAnalitico", dto, request.CodigoCorrelacao, "", "Relatório Controle de Grade Sintético")));
        }

        private async Task<IEnumerable<Turma>> ObterTurmas(IEnumerable<long> turmasIds)
        {
            var turmas = await mediator.Send(new ObterTurmasPorIdsQuery(turmasIds.ToArray()));
            return turmas;
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(IEnumerable<long> componentesCurriculares)
        {
            var componentes = await mediator.Send(new ObterComponentesCurricularesPorIdsQuery(componentesCurriculares.ToArray()));
            return componentes;
        }

        private async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolares(IEnumerable<int> bimestres, long tipoCalendarioId)
        {
            var periodos = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));
            return periodos.Where(a => bimestres.Contains(a.Bimestre));
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

            var dataInicio = aulasPrevistasBimestre.FirstOrDefault().DataInicio;
            var dataFim = aulasPrevistasBimestre.FirstOrDefault().DataFim;

            var bimestreDto = new BimestreControleGradeDto()
            {
                Descricao = $"{bimestre}º Bimestre - {dataInicio.ToString("dd/MM")} À {dataFim.ToString("dd/MM")}"
            };

            foreach (var aulasPrevistasComponente in aulasPrevistasBimestre.OrderBy(c => c.ComponenteCurricularNome))
            {
                bimestreDto.ComponentesCurriculares.Add(await MapearParaComponenteDto(aulasPrevistasComponente, turmaId, tipoCalendarioId, modalidadeTurma, bimestre, dataInicio, dataFim));
            }

            return bimestreDto;
        }

        private async Task<ComponenteCurricularControleGradeDto> MapearParaComponenteDto(AulaPrevistaBimestreQuantidade aulasPrevistasComponente, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma, int bimestre, DateTime dataInicio, DateTime dataFim)
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
            componenteDto.VisaoSemanal = await ObterVisaoSemanal(dataInicio, dataFim, turmaId, aulasPrevistasComponente.ComponenteCurricularId, tipoCalendarioId);
            detalhamentoDivergencias.AulasDiasNaoLetivos = await ObterAulasDiasNaoLetivos(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre);

            return componenteDto;
        } 
        
        private async Task<List<AulaDiasNaoLetivosControleGradeDto>> ObterAulasDiasNaoLetivos(long turmaId, long tipoCalendarioId, long componenteCurricularCodigo, int bimestre, bool professorCJ = false)
        {
            var data = DateTime.Today;

            var diasComEvento = await mediator.Send(new ObterDiasPorPeriodosEscolaresComEventosLetivosENaoLetivosQuery(tipoCalendarioId));

            var aulas = await mediator.Send(new ObterAulaReduzidaPorTurmaComponenteEBimestreQuery(turmaId, tipoCalendarioId, componenteCurricularCodigo, bimestre));

            var diasComEventosNaoLetivos = diasComEvento.Where(e => e.EhNaoLetivo);

            var aulasDiasNaoLetivos = new List<AulaDiasNaoLetivosControleGradeDto>();

            aulas.Where(a => diasComEventosNaoLetivos.Any(d => d.Data == a.Data)).ToList()
                .ForEach(aula =>
                {
                    foreach(var eventoNaoLetivo in diasComEventosNaoLetivos.Where(d => d.Data == aula.Data))
                    {
                        aulasDiasNaoLetivos.Add(new AulaDiasNaoLetivosControleGradeDto()
                        {
                            Data = aula.Data.ToString("dd/MM/yyyy"),
                            Professor = $"{aula.Professor} ({aula.ProfessorRf})",
                            QuantidadeAulas = aula.Quantidade,
                            Motivo = eventoNaoLetivo.Motivo
                        });
                    }
                });

        private async Task<IEnumerable<VisaoSemanalControleGradeSinteticoDto>> ObterVisaoSemanal(DateTime dataInicio, DateTime dataFim, long turmaId, long componenteCurricularId, long tipoCalendarioId)
        {
            var visaoSemanal = new List<VisaoSemanalControleGradeSinteticoDto>();
            var quantidadeGrade = await mediator.Send(new ObterQuantidadeDeAulaGradeQuery(turmaId, componenteCurricularId));
            var eventosCadastrados = await mediator.Send(new ObterEventosPorTipoCalendarioIdEPeriodoInicioEFimQuery(tipoCalendarioId, dataInicio, dataFim));

            for (var data = dataInicio.AddDays(1); data <= dataFim; data = data.AddDays(1))
            {
                if (VerificaDataEhSegunda(data))
                {
                    var dataFimSemana = data.AddDays(6) > dataFim ? dataFim : data.AddDays(6);
                    var aulasCriadas = await mediator.Send(new ObterDiasAulaCriadasPeriodoInicioEFimQuery(turmaId, componenteCurricularId, data, dataFimSemana));
                    var diasLetivos = ObtemDiasLetivos(data, dataFimSemana, eventosCadastrados);

                    visaoSemanal.Add(new VisaoSemanalControleGradeSinteticoDto()
                    {
                        Data = data.ToString(),
                        QuantidadeGrade = quantidadeGrade,
                        DiasLetivo = diasLetivos,
                        AulasCriadas = aulasCriadas,
                        Diferenca = PossuiDiferencaDias(quantidadeGrade,diasLetivos,aulasCriadas)
                    });
                }
            }
            return visaoSemanal;
        }

        private string PossuiDiferencaDias(int quantidadeGrade, int diasLetivos, int aulasCriadas)
        {
            if(quantidadeGrade > 0)
            {
                if (quantidadeGrade != diasLetivos)
                    return "Sim";

                if (quantidadeGrade != aulasCriadas)
                    return "Sim";

                if (diasLetivos != aulasCriadas)
                    return "Sim";

                return "Não";
            }
            return "Sim";
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

        private async Task<List<AulaTitularCJDataControleGradeDto>> ObterAulasTitularCJ(AulaPrevistaBimestreQuantidade aulasPrevistasComponente, long turmaId, long tipoCalendarioId, int bimestre)
        {
            var aulasTitular = await mediator.Send(new ObterAulaReduzidaPorTurmaComponenteEBimestreQuery(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre));
            var aulasCJ = await mediator.Send(new ObterAulaReduzidaPorTurmaComponenteEBimestreQuery(turmaId, tipoCalendarioId, aulasPrevistasComponente.ComponenteCurricularId, bimestre, true));

            var aulasTitularCJData = new List<AulaTitularCJDataControleGradeDto>();
            foreach (var aulaTitular in aulasTitular)
            {
                var aulasRepetidas = aulasCJ.Where(a => a.Data == aulaTitular.Data);
                if (aulasRepetidas.Any())
                {
                    var aulaTitularCJData = new AulaTitularCJDataControleGradeDto()
                    {
                        Data = aulaTitular.Data.ToString("dd/MM/yyyy")
                    };

                    var divergencias = new List<AulaTitularCJControleGradeDto>();

                    foreach (var aula in aulasRepetidas)
                    {
                        divergencias.Add(new AulaTitularCJControleGradeDto()
                        {
                            QuantidadeAulas = aula.Quantidade + aulaTitular.Quantidade,
                            ProfessorCJ = aula.Professor,
                            ProfessorTitular = aulaTitular.Professor
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
                var verificaAulaCriadaProfessor = await mediator.Send(new VerificarAulasNormaisCriadasProfessorRegenciaQuery(componenteCurricularId.ToString(), bimestre, tipoCalendarioId));
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
            dto.Filtro.Ue = $"{turma.Ue.CodigoUe} - {turma.Ue.Nome}";
            dto.Filtro.Turma = filtros.Turmas.Count() > 1 ? "Todas" : $"{turma.Modalidade.ShortName()} - {turma.Nome}";
            dto.Filtro.Bimestre = filtros.Bimestres.Count() == QuantidadePeriodosPorModalidade(turma.Modalidade) ?
                                    "Todos" : string.Join(",", filtros.Bimestres);
            dto.Filtro.ComponenteCurricular = ObterNomeComponente(filtros, dto);
            dto.Filtro.Usuario = filtros.UsuarioNome;
            dto.Filtro.RF = filtros.UsuarioRf;
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
