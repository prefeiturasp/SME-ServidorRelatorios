using MediatR;
using Newtonsoft.Json;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarTransferenciaQueryHandler : IRequestHandler<MontarHistoricoEscolarTransferenciaQuery, IEnumerable<TransferenciaDto>>
    {
        public MontarHistoricoEscolarTransferenciaQueryHandler()
        {

        }

        public async Task<IEnumerable<TransferenciaDto>> Handle(MontarHistoricoEscolarTransferenciaQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<TransferenciaDto>();

            foreach (var aluno in request.AlunosTurmas)
            {
                var alunoTurmasPorModalidade = aluno.Turmas.Where(t => string.IsNullOrEmpty(t.RegularCodigo)).GroupBy(t => t.ModalidadeCodigo);

                foreach (var agrupamentoTurmas in alunoTurmasPorModalidade)
                {
                    var componentesDaTurma = request.ComponentesCurricularesTurmas.Where(cc => agrupamentoTurmas.Select(c => c.Codigo).Contains(cc.Key)).SelectMany(ct => ct).Where(cc => string.IsNullOrEmpty(cc.CodigoTurmaAssociada) || aluno.Turmas.Any(at => at.Codigo == cc.CodigoTurmaAssociada)).DistinctBy(d => d.CodDisciplina);
                    var componentesPorGrupoMatriz = componentesDaTurma.Where(gm => gm.GrupoMatriz != null).GroupBy(cc => cc.GrupoMatriz);

                    //Obter grupo matriz
                    var baseNacionalComum = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 1)?.Select(b => b);
                    var diversificados = componentesPorGrupoMatriz.Where(cpm => cpm.Key.Id == 2 || cpm.Key.Id > 4)?.SelectMany(d => d);
                    var enriquecimentos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 3)?.Select(e => e);
                    var projetos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 4)?.Select(p => p);
                    //

                    var notasAluno = request.Notas.Where(n => agrupamentoTurmas.Select(t => t.Codigo).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Aluno.Codigo && w.PeriodoEscolar != null);
                    var frequenciasAluno = request.Frequencias.Where(f => agrupamentoTurmas.Select(t => t.Codigo).Contains(f.Key)).SelectMany(a => a).Where(a => a.CodigoAluno == aluno.Aluno.Codigo);

                    var baseNacionalDto = ObterBaseNacionalComum(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, baseNacionalComum, request.AreasConhecimento, request.GrupoAreaOrdenacao, request.RegistroFrequenciasAlunos, aluno, request.BimestreAtual);
                    var diversificadosDto = ObterGruposDiversificado(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, diversificados, request.AreasConhecimento, request.GrupoAreaOrdenacao, request.RegistroFrequenciasAlunos, aluno, request.BimestreAtual);
                    var enriquecimentoDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, enriquecimentos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento, request.RegistroFrequenciasAlunos, aluno, request.BimestreAtual)?.ToList();
                    var projetosDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, projetos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento, request.RegistroFrequenciasAlunos, aluno, request.BimestreAtual)?.ToList();

                    var turma = aluno.Turmas.FirstOrDefault(x => x.TipoTurma == TipoTurma.Regular);
                    var tiposNotaDto = MapearTiposNota(agrupamentoTurmas.Key, request.TiposNota, turma);

                    var transferenciaDto = new TransferenciaDto()
                    {
                        CodigoAluno = aluno.Aluno.Codigo,
                        CodigoTurma = turma?.Codigo,
                        Modalidade = turma.ModalidadeCodigo,
                        TipoNota = tiposNotaDto,
                        Data = aluno.Aluno.DataSituacaoFormatada,
                        Descricao = turma?.DescricaoRelatorioTransferencia,
                        Rodape = turma?.RodapeRelatorioTransferencia,
                        GruposComponentesCurriculares = diversificadosDto,
                        BaseNacionalComum = baseNacionalDto,
                        EnriquecimentoCurricular = enriquecimentoDto,
                        ProjetosAtividadesComplementares = projetosDto,
                        Legenda = ObterLegenda(notasAluno, enriquecimentoDto, request.Legenda)
                    };

                    listaRetorno.Add(transferenciaDto);
                }
            }

            return await Task.FromResult(listaRetorno);
        }

        private LegendaDto ObterLegenda(IEnumerable<NotasAlunoBimestre> notasAluno, List<ComponenteCurricularHistoricoEscolarTransferenciaDto> enriquecimentoDto, LegendaDto legenda)
        {
            var legendaRetorno = new LegendaDto() { Texto = String.Empty };
            if (notasAluno.Any(c => c.NotaConceito.ConceitoId != null))
                legendaRetorno.Texto = legenda.TextoConceito;

            if (notasAluno.Any(c => c.NotaConceito.Sintese != null) || (enriquecimentoDto != null && !enriquecimentoDto.Any(c => c.Nota)))
                legendaRetorno.Texto = legendaRetorno.Texto != String.Empty ? $"{legendaRetorno.Texto},{legenda.TextoSintese}" : legenda.TextoSintese;

            return legendaRetorno;
        }

        private string MapearTiposNota(Modalidade modalidade, IEnumerable<TipoNotaCicloAno> tiposNota, Turma turma)
        {
            return tiposNota.FirstOrDefault(t => t.Ano.ToString() == turma.Ano && t.Modalidade == modalidade)?.TipoNota;
        }

        private List<GruposComponentesCurricularesTransferenciaDto> ObterGruposDiversificado(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                            IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                                            IEnumerable<RegistroFrequenciaAlunoDto> registroFrequenciasAlunos,
                                                            AlunoTurmasHistoricoEscolarDto aluno,
                                                            int bimestreAtual)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesTransferenciaDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz).OrderBy(g => g.Key.Id);

                foreach (var componentes in componentesPorGrupoMatriz)
                {
                    var areasConhecimento = MapearAreasDoConhecimento(componentes, areasDoConhecimentos, grupoAreaOrdenacao, componentes.Key.Id);

                    gruposComponentes.Add(new GruposComponentesCurricularesTransferenciaDto
                    {
                        Nome = componentes.Key.Nome,
                        AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoTransferenciaDto()
                        {
                            Nome = ac.Key.Nome,
                            ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                    ObterComponentesDasAreasDeConhecimento(componentes, ac),
                                                    notasAlunos, frequencias, mediasFrequencia, ac, registroFrequenciasAlunos, aluno, bimestreAtual)?.OrderBy(o => o.Nome).ToList()
                        }).ToList()
                    });
                }
            }

            return gruposComponentes;
        }

        private BaseNacionalComumTransferenciaDto ObterBaseNacionalComum(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                            IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                                            IEnumerable<RegistroFrequenciaAlunoDto> registroFrequenciasAlunos,
                                                            AlunoTurmasHistoricoEscolarDto aluno,
                                                            int bimestreAtual)
        {
            BaseNacionalComumTransferenciaDto baseNacional = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos, grupoAreaOrdenacao, 1);

                baseNacional = new BaseNacionalComumTransferenciaDto()
                {
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoTransferenciaDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                ObterComponentesDasAreasDeConhecimento(componentesCurricularesDaTurma, ac),
                                                notasAlunos, frequencias, mediasFrequencia, ac, registroFrequenciasAlunos, aluno, bimestreAtual)?.OrderBy(o => o.Nome).ToList()
                    }).ToList()
                };
            }

            return baseNacional;
        }

        private IEnumerable<ComponenteCurricularPorTurma> ObterComponentesDasAreasDeConhecimento(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                                IEnumerable<AreaDoConhecimento> areaDoConhecimento)
        {
            return componentesCurricularesDaTurma.Where(c => (!c.Regencia && areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Contains(c.CodDisciplina)) ||
                                                            (c.Regencia && areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Any(cr =>
                                                            c.ComponentesCurricularesRegencia.Select(cr => cr.CodDisciplina).Contains(cr))));
        }

        private IEnumerable<IGrouping<(string Nome, int? Ordem, long Id), AreaDoConhecimento>> MapearAreasDoConhecimento(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                                              IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                                                                              IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                                                                                              long grupoMatrizId)
        {

            return areasDoConhecimentos.Where(a => ((componentesCurricularesDaTurma.Where(cc => !cc.Regencia).Select(cc => cc.CodDisciplina).Contains(a.CodigoComponenteCurricular)) ||
                                                                    (componentesCurricularesDaTurma.Any(cc => cc.Regencia) &&
                                                                     componentesCurricularesDaTurma.Where(cc => cc.Regencia).SelectMany(cc => cc.ComponentesCurricularesRegencia).
                                                                     Select(r => r.CodDisciplina).Contains(a.CodigoComponenteCurricular)))).
                                                                     Select(a => { a.DefinirOrdem(grupoAreaOrdenacao, grupoMatrizId); return a; }).GroupBy(g => (g.Nome, g.Ordem, g.Id)).
                                                                     OrderByDescending(c => c.Key.Id > 0 && !string.IsNullOrEmpty(c.Key.Nome))
                                                                     .ThenByDescending(c => c.Key.Ordem.HasValue).ThenBy(c => c.Key.Ordem)
                                                                     .ThenBy(c => c.Key.Nome, StringComparer.OrdinalIgnoreCase);
        }

        private IEnumerable<ComponenteCurricularHistoricoEscolarTransferenciaDto> MontarComponentesNotasFrequencia(IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<AreaDoConhecimento> areasDoConhecimentos, IEnumerable<RegistroFrequenciaAlunoDto> registroFrequenciasAlunos, AlunoTurmasHistoricoEscolarDto aluno, int bimestreAtual)
        {
            List<ComponenteCurricularHistoricoEscolarTransferenciaDto> componentes = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var turmaRegular = turmas.FirstOrDefault(x => x.TipoTurma == TipoTurma.Regular);
                componentes = new List<ComponenteCurricularHistoricoEscolarTransferenciaDto>();

                foreach (var componenteCurricular in componentesCurricularesDaTurma)
                {
                    if (componenteCurricular.Regencia)
                    {
                        componentes.AddRange(MapearComponentesRegencia(componenteCurricular.CodDisciplina.ToString(), turmas, componenteCurricular.ComponentesCurricularesRegencia.Where(r => areasDoConhecimentos.Select(a => a.CodigoComponenteCurricular).Contains(r.CodDisciplina)), notas, frequencia.Where(f => f.DisciplinaId == componenteCurricular.CodDisciplina.ToString()), mediasFrequencia, registroFrequenciasAlunos, bimestreAtual));
                    }
                    else
                    {
                        var turmaDoComponente = turmas.FirstOrDefault(x => x.Codigo == componenteCurricular.CodigoTurma);

                        componentes.Add(new ComponenteCurricularHistoricoEscolarTransferenciaDto()
                        {
                            Codigo = componenteCurricular.CodDisciplina.ToString(),
                            Nome = componenteCurricular.Disciplina,
                            Frequencia = componenteCurricular.Frequencia,
                            Nota = componenteCurricular.LancaNota,
                            FrequenciaPrimeiroBimestre = ObterFrequenciaComponentePorTurma(turmaDoComponente, componenteCurricular.CodDisciplina.ToString(), frequencia, 1, registroFrequenciasAlunos.Where(a => a.CodigoAluno.ToString() == aluno.Aluno.Codigo), bimestreAtual),
                            FrequenciaSegundoBimestre = ObterFrequenciaComponentePorTurma(turmaDoComponente, componenteCurricular.CodDisciplina.ToString(), frequencia, 2, registroFrequenciasAlunos.Where(a => a.CodigoAluno.ToString() == aluno.Aluno.Codigo), bimestreAtual),
                            FrequenciaTerceiroBimestre = ObterFrequenciaComponentePorTurma(turmaDoComponente, componenteCurricular.CodDisciplina.ToString(), frequencia, 3, registroFrequenciasAlunos.Where(a => a.CodigoAluno.ToString() == aluno.Aluno.Codigo), bimestreAtual),
                            FrequenciaQuartoBimestre = ObterFrequenciaComponentePorTurma(turmaDoComponente, componenteCurricular.CodDisciplina.ToString(), frequencia, 4, registroFrequenciasAlunos.Where(a => a.CodigoAluno.ToString() == aluno.Aluno.Codigo), bimestreAtual),
                            NotaConceitoPrimeiroBimestre = ObterNotaComponentePorTurma(turmaDoComponente, turmaRegular, componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 1, bimestreAtual),
                            NotaConceitoSegundoBimestre = ObterNotaComponentePorTurma(turmaDoComponente, turmaRegular, componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 2, bimestreAtual),
                            NotaConceitoTerceiroBimestre = ObterNotaComponentePorTurma(turmaDoComponente, turmaRegular, componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 3, bimestreAtual),
                            NotaConceitoQuartoBimestre = ObterNotaComponentePorTurma(turmaDoComponente, turmaRegular, componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 4, bimestreAtual)
                        });
                    }
                }
            }

            return componentes;
        }

        private string ObterFrequenciaComponentePorTurma(Turma turma, string codigoComponente, IEnumerable<FrequenciaAluno> frequenciaAlunos, int bimestre, IEnumerable<RegistroFrequenciaAlunoDto> registroFrequenciasAlunos, int bimestreAtual)
        {
            if (turma != null)
            {
                var frequenciasAlunoParaTratar = frequenciaAlunos.Where(a => a.DisciplinaId == codigoComponente && a.Bimestre == bimestre);
                FrequenciaAluno frequenciaAluno;
                var registroFrequenciaAluno = registroFrequenciasAlunos.Where(a => a.ComponenteCurricularId.ToString() == codigoComponente && a.Bimestre == bimestre && a.TurmaCodigo == turma.Codigo);

                if (frequenciasAlunoParaTratar == null || !frequenciasAlunoParaTratar.Any())
                {
                    if (registroFrequenciaAluno.Any())
                        return FrequenciaAluno.FormatarPercentual(100);
                    if (bimestre <= bimestreAtual)
                        return string.Empty;
                    return "-";
                }
                else if (frequenciasAlunoParaTratar.Count() == 1)
                {
                    frequenciaAluno = frequenciasAlunoParaTratar.FirstOrDefault();
                }
                else
                {
                    frequenciaAluno = new FrequenciaAluno()
                    {
                        DisciplinaId = codigoComponente,
                        CodigoAluno = frequenciasAlunoParaTratar.FirstOrDefault().CodigoAluno
                    };


                    frequenciaAluno.TotalAulas = frequenciasAlunoParaTratar.Sum(a => a.TotalAulas);
                    frequenciaAluno.TotalAusencias = frequenciasAlunoParaTratar.Sum(a => a.TotalAusencias);
                    frequenciaAluno.TotalCompensacoes = frequenciasAlunoParaTratar.Sum(a => a.TotalCompensacoes);
                }

                return frequenciaAluno.TotalAulas > 0 ? frequenciaAluno?.TotalAusencias.ToString() ?? "0" : "0";
            }
            else
                return null;
        }

        private string ObterNotaComponentePorTurma(Turma turma, Turma turmaRegular, string codigoComponente, bool regencia, bool lancaNota, IEnumerable<FrequenciaAluno> frequenciaAlunos,
            IEnumerable<NotasAlunoBimestre> notasAluno, IEnumerable<MediaFrequencia> mediasFrequencias, int bimestre, int bimestreAtual)
        {
            if (turma is null)
                return null;

            if (!lancaNota)
                return ObterSintese(frequenciaAlunos.Where(f => f.PeriodoEscolarId != null), mediasFrequencias, regencia, !lancaNota);

            if (turmaRegular is null)
                return notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turma.Codigo && n.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;

            var nota = "";
            if (turma.Codigo == turmaRegular.Codigo)
            {
                nota = notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turma.Codigo && n.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;
                return ChecarNota(nota, bimestre, bimestreAtual);
            }

            nota = notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turmaRegular.Codigo && n.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito
                ?? notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turma.Codigo && n.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;

            return ChecarNota(nota, bimestre, bimestreAtual); ;
        }

        private string ChecarNota(string valor, int bimestre, int bimestreAtual)
        {
            if (valor == null)
                if (bimestre <= bimestreAtual)
                    valor = "";
                else
                    valor = "-";

            return valor;
        }

        private string ObterNotaComponentePorTurmaRegencia(Turma turma, string codigoComponente, bool regencia, bool lancaNota, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<NotasAlunoBimestre> notasAluno, IEnumerable<MediaFrequencia> mediasFrequencias, int bimestre)
        {
            if (turma is null)
                return null;

            if (!lancaNota)
                return ObterSintese(frequenciaAlunos.Where(f => f.PeriodoEscolarId != null), mediasFrequencias, regencia, !lancaNota);

            return notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turma.Codigo && n.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;
        }

        private IEnumerable<ComponenteCurricularHistoricoEscolarTransferenciaDto> MapearComponentesRegencia(string codigoComponenteRegencia, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurmaRegencia> componentesRegencia, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<RegistroFrequenciaAlunoDto> registroFrequenciasAlunos, int bimestreAtual)
        {
            var componentes = new List<ComponenteCurricularHistoricoEscolarTransferenciaDto>();

            var frequencia1Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 1, registroFrequenciasAlunos, bimestreAtual);
            var frequencia2Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 2, registroFrequenciasAlunos, bimestreAtual);
            var frequencia3Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 3, registroFrequenciasAlunos, bimestreAtual);
            var frequencia4Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 4, registroFrequenciasAlunos, bimestreAtual);

            foreach (var componenteCurricular in componentesRegencia)
            {
                componentes.Add(new ComponenteCurricularHistoricoEscolarTransferenciaDto()
                {
                    Codigo = componenteCurricular.CodDisciplina.ToString(),
                    Nome = componenteCurricular.Disciplina,
                    Frequencia = componenteCurricular.Frequencia,
                    Nota = componenteCurricular.LancaNota,
                    FrequenciaPrimeiroBimestre = frequencia1Bimestre,
                    FrequenciaSegundoBimestre = frequencia2Bimestre,
                    FrequenciaTerceiroBimestre = frequencia3Bimestre,
                    FrequenciaQuartoBimestre = frequencia4Bimestre,
                    NotaConceitoPrimeiroBimestre = ObterNotaComponentePorTurmaRegencia(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 1),
                    NotaConceitoSegundoBimestre = ObterNotaComponentePorTurmaRegencia(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 2),
                    NotaConceitoTerceiroBimestre = ObterNotaComponentePorTurmaRegencia(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 3),
                    NotaConceitoQuartoBimestre = ObterNotaComponentePorTurmaRegencia(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 4),
                });
            }

            return componentes;
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
