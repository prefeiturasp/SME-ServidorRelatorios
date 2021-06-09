using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarBoletinsDetalhadosQueryHandler : IRequestHandler<MontarBoletinsDetalhadosQuery, BoletimEscolarDetalhadoDto>
    {
        private const string FREQUENCIA_100 = "100";
        private readonly IMediator mediator;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;

        public MontarBoletinsDetalhadosQueryHandler(IMediator mediator,
                                          ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
        }

        public async Task<BoletimEscolarDetalhadoDto> Handle(MontarBoletinsDetalhadosQuery request, CancellationToken cancellationToken)
        {
            var turmas = request.Turmas;
            var dre = request.Dre;
            var ue = request.Ue;
            var ciclos = request.TiposCiclo;
            var componentesCurriculares = request.ComponentesCurriculares;
            var alunos = request.AlunosPorTuma;
            var fotos = request.AlunosFoto;
            var notas = request.Notas;
            var frequencia = request.Frequencias;
            var frequenciasGlobal = request.FrequenciasGlobal;
            var tiposNota = request.TiposNota;
            var pareceresConclusivos = request.PareceresConclusivos;
            var mediasFrequencia = request.MediasFrequencia;
            var recomendacoes = request.RecomendacoesAlunos;

            var boletinsAlunos = new List<BoletimEscolarDetalhadoAlunoDto>();

            foreach (var aluno in alunos)
            {
                // verifica PossuiConselho bimestre
                var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                var conselhoClassBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre));

                if (conselhoClassBimestres != null && conselhoClassBimestres.Any())
                {
                    var tipoNota = tiposNota[turma.Codigo];
                    var boletimEscolarAlunoDto = new BoletimEscolarDetalhadoAlunoDto()
                    {
                        TipoNota = tipoNota
                    };

                    var componentesAluno = componentesCurriculares.First(c => c.Key == aluno.Key);
                    foreach (var turmaAluno in aluno)
                    {
                        await MapearGruposEComponentes(componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.CodigoTurma.ToString()), boletimEscolarAlunoDto);
                    }

                    var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                    var frequenciasAluno = frequencia?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                    if (notasAluno != null && notasAluno.Any())
                        SetarNotasFrequencia(boletimEscolarAlunoDto, notasAluno, frequenciasAluno, mediasFrequencia, conselhoClassBimestres);

                    var frequeciaGlobal = frequenciasGlobal?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                    var percentualFrequenciaGlobal = frequeciaGlobal != null ? frequeciaGlobal.First().PercentualFrequencia : 100;
                    var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);
                    var recomendacao = recomendacoes?.FirstOrDefault(r => r.TurmaCodigo == turma.Codigo && r.AlunoCodigo == aluno.Key);
                    var ciclo = ciclos.FirstOrDefault(c => c.Modalidade == turma.ModalidadeCodigo && c.Ano == Convert.ToInt32(turma.Ano));
                    var foto = fotos.FirstOrDefault(c => c.CodigoAluno.ToString() == aluno.Key);

                    boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, ciclo, turma, aluno.Key, foto, aluno.FirstOrDefault().NomeRelatorio, $"{percentualFrequenciaGlobal}%");
                    boletimEscolarAlunoDto.ParecerConclusivo = parecerConclusivo?.ParecerConclusivo ?? "";
                    boletimEscolarAlunoDto.RecomendacoesEstudante = recomendacao?.RecomendacoesAluno;
                    boletimEscolarAlunoDto.RecomendacoesFamilia = recomendacao?.RecomendacoesFamilia;
                    boletinsAlunos.Add(boletimEscolarAlunoDto);
                }
            }

            return await Task.FromResult(new BoletimEscolarDetalhadoDto(boletinsAlunos));
        }

        private BoletimEscolarDetalhadoCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, TipoCiclo ciclo, Turma turma, string alunoCodigo, AlunoFotoArquivoDto foto, string nome, string frequenciaGlobal)
        {
            return new BoletimEscolarDetalhadoCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Nome,
                NomeUe = ue.TituloTipoEscolaNome,
                NomeTurma = turma.NomeRelatorio,
                CodigoEol = alunoCodigo,
                Aluno = nome,
                FrequenciaGlobal = frequenciaGlobal,
                Ciclo = ciclo.Descricao,
                Foto = foto?.FotoBase64
            };
        }

        private async Task MapearGruposEComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma, BoletimEscolarDetalhadoAlunoDto boletim)
        {
            var componentesOrdenados = await mediator.Send(new OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery(componentesCurricularesPorTurma));

            var gruposAreas = componentesOrdenados.GroupBy(cc => new { GrupoMatrizId = cc.GrupoMatriz?.Id, AreaConhecimentoId = cc.AreaDoConhecimento?.Id }).ToList();

            var areasRetorno = new List<AreaConhecimentoComponenteCurricularDto>();

            foreach (var grupoArea in gruposAreas)
            {
                AreaConhecimentoComponenteCurricularDto area = null;

                if (boletim.AreasConhecimento.Any(g => g.Id == grupoArea.Key.AreaConhecimentoId && g.GrupoMatrizId == grupoArea.Key.GrupoMatrizId))
                    area = boletim.AreasConhecimento.FirstOrDefault(g => g.Id == grupoArea.Key.AreaConhecimentoId && g.GrupoMatrizId == grupoArea.Key.GrupoMatrizId);
                else
                {
                    area = new AreaConhecimentoComponenteCurricularDto()
                    {
                        Id = (int)grupoArea.Key.AreaConhecimentoId,
                        GrupoMatrizId = (int)grupoArea.Key.GrupoMatrizId,
                        Nome = $"GRUPO {areasRetorno.Count() + 1}",
                    };

                    boletim.AreasConhecimento.Add(area);
                }

                foreach (var componente in grupoArea.OrderBy(a => a.Disciplina))
                {
                    if (componente.Regencia && componente.ComponentesCurricularesRegencia != null && componente.ComponentesCurricularesRegencia.Any())
                    {
                        boletim.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto()
                        {
                            Codigo = componente.CodDisciplina.ToString(),
                            Frequencia = componente.Frequencia
                        };

                        foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia.OrderBy(a => a.Disciplina))
                        {
                            if (!boletim.ComponenteCurricularRegencia.ComponentesCurriculares.Any(g => g.Codigo == componenteRegencia.CodDisciplina.ToString()))
                                boletim.ComponenteCurricularRegencia.ComponentesCurriculares.Add(
                                    new ComponenteCurricularRegenciaNotaDto()
                                    {
                                        Codigo = componenteRegencia.CodDisciplina.ToString(),
                                        Nome = componenteRegencia.Disciplina,
                                        Nota = componenteRegencia.LancaNota
                                    });
                        }
                    }
                    else if (!componente.Regencia)
                    {
                        if (area.ComponentesCurriculares == null)
                            area.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                        if (!area.ComponentesCurriculares.Any(g => g.Codigo == componente.CodDisciplina.ToString()))
                            area.ComponentesCurriculares.Add(
                                new ComponenteCurricularDto()
                                {
                                    Codigo = componente.CodDisciplina.ToString(),
                                    Nome = componente.Disciplina,
                                    Nota = componente.LancaNota,
                                    Frequencia = componente.Frequencia
                                });
                    }
                }
                if (boletim.ComponenteCurricularRegencia != null)
                    boletim.ComponenteCurricularRegencia.ComponentesCurriculares = boletim.ComponenteCurricularRegencia.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();

                if (area.ComponentesCurriculares != null && area.ComponentesCurriculares.Any())
                    area.ComponentesCurriculares = area.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();
            }

            if (boletim.AreasConhecimento != null && boletim.AreasConhecimento.Any())
                boletim.AreasConhecimento = boletim.AreasConhecimento.Where(b => b.ComponentesCurriculares != null && b.ComponentesCurriculares.Any()).ToList();
        }

        private void SetarNotasFrequencia(BoletimEscolarDetalhadoAlunoDto boletimEscolar, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres)
        {
            if (boletimEscolar.ComponenteCurricularRegencia != null)
            {
                if (boletimEscolar.ComponenteCurricularRegencia.Frequencia)
                {
                    var frequenciasRegencia = frequencia?.Where(f => f.DisciplinaId == boletimEscolar.ComponenteCurricularRegencia.Codigo);

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 1);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 2);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 3);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 4);

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasRegencia, conselhoClasseBimestres);
                }

                foreach (var componenteCurricular in boletimEscolar.ComponenteCurricularRegencia.ComponentesCurriculares)
                {
                    if (componenteCurricular.Nota)
                    {
                        var notaFrequenciaComponente = notas?.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(notaFrequenciaComponente, 1);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(notaFrequenciaComponente, 2);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(notaFrequenciaComponente, 3);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(notaFrequenciaComponente, 4);

                        componenteCurricular.NotaFinal = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                    }
                }
            }

            foreach (var areaConhecimento in boletimEscolar.AreasConhecimento)
            {
                if (areaConhecimento.ComponentesCurriculares != null && areaConhecimento.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in areaConhecimento.ComponentesCurriculares)
                    {
                        var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                        if (componenteCurricular.Nota)
                        {
                            var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(notasComponente, 1);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(notasComponente, 2);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(notasComponente, 3);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(notasComponente, 4);

                            componenteCurricular.NotaFinal = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        }
                        else
                            componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);


                        if (componenteCurricular.Frequencia)
                        {

                            componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 1);
                            componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 2);
                            componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 3);
                            componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 4);

                            componenteCurricular.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasComponente, conselhoClasseBimestres); ;

                            if (!componenteCurricular.Nota)
                                componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);
                        }
                    }
                }
            }
        }

        private string ObterNotaBimestre(IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre)
        {
            var nota = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;
            return !string.IsNullOrEmpty(nota) ? nota : "-";
        }

        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClassBimestres, int bimestre)
        {
            return conselhoClassBimestres.Any(a => a == bimestre);
        }

        private string ObterFrequenciaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<FrequenciaAluno> frequenciasComponente, int bimestre)
        {
            var frequencia = !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequencia.ToString() ?? FREQUENCIA_100;
            return !string.IsNullOrEmpty(frequencia) ? frequencia + "%" : frequencia;

        }

        private string ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequencias, IEnumerable<int> conselhoClassBimestres)
        {
            if (!conselhoClassBimestres.Any(a => a == 0))
                return "";
            else if (frequencias == null || !frequencias.Any())
                return FREQUENCIA_100;
            else if (frequencias.FirstOrDefault(nf => nf.PeriodoEscolarId == null) != null)
                return frequencias.FirstOrDefault(nf => nf.PeriodoEscolarId == null).PercentualFrequencia.ToString();
            else
            {
                var frequenciaFinal = new FrequenciaAluno()
                {
                    TotalAulas = frequencias.Sum(f => f.TotalAulas),
                    TotalAusencias = frequencias.Sum(f => f.TotalAusencias),
                    TotalCompensacoes = frequencias.Sum(f => f.TotalCompensacoes)
                };

                //Particularidade de cálculo de frequência para 2020.
                if (frequencias.First().AnoTurma.Equals(2020))
                {
                    var idTipoCalendario = tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(frequencias.First().AnoTurma, frequencias.First().ModalidadeTurma).Result;
                    var periodos = mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(idTipoCalendario)).Result;

                    periodos.ToList().ForEach(p =>
                    {
                        var frequencia = frequencias.SingleOrDefault(f => f.Bimestre.Equals(p.Bimestre));
                        frequenciaFinal.AdicionarFrequenciaBimestre(p.Bimestre, frequencia != null ? frequencia.PercentualFrequencia : 100);
                    });

                    return frequenciaFinal.PercentualFrequenciaFinal.ToString();
                }

                return frequenciaFinal.PercentualFrequencia.ToString();
            }

        }

        private string ObterSintese(IEnumerable<FrequenciaAluno> frequenciasComponente, IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            var percentualFrequencia = ObterPercentualDeFrequencia(frequenciasComponente);

            var sintese = percentualFrequencia >= ObterFrequenciaMedia(mediaFrequencias, regencia, lancaNota) ?
                          "F" : "NF";

            return sintese;
        }

        private double ObterPercentualDeFrequencia(IEnumerable<FrequenciaAluno> frequenciaDisciplina)
        {
            return frequenciaDisciplina != null && frequenciaDisciplina.Any() ? frequenciaDisciplina.Sum(x => x.PercentualFrequencia) / frequenciaDisciplina.Count() : 100;
        }

        private double ObterFrequenciaMedia(IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            if (regencia || !lancaNota)
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse).Media;
            else
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualFund2).Media;
        }
    }
}
