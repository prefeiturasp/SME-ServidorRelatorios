using MediatR;
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
    public class MontarHistoricoEscolarQueryHandler : IRequestHandler<MontarHistoricoEscolarQuery, IEnumerable<HistoricoEscolarDTO>>
    {
        public MontarHistoricoEscolarQueryHandler()
        {

        }

        public async Task<IEnumerable<HistoricoEscolarDTO>> Handle(MontarHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<HistoricoEscolarDTO>();

            var alunosTurmas = request.AlunosTurmas.GroupBy(a => a.Aluno.Codigo);

            foreach (var aluno in alunosTurmas)
            {
                var alunoTurmasPorModalidade = aluno.SelectMany(a => a.Turmas).Where(t => string.IsNullOrEmpty(t.RegularCodigo)).GroupBy(t => t.ModalidadeCodigo);

                foreach (var agrupamentoTurmas in alunoTurmasPorModalidade)
                {
                    var componentesDaTurma = request.ComponentesCurricularesTurmas.Where(cc => agrupamentoTurmas.Select(c => c.Codigo).Contains(cc.Key)).SelectMany(ct => ct).Where(cc => string.IsNullOrEmpty(cc.CodigoTurmaAssociada) || aluno.SelectMany(a => a.Turmas).Any(at => at.Codigo == cc.CodigoTurmaAssociada)).DistinctBy(d => d.CodDisciplina);
                    var componentesPorGrupoMatriz = componentesDaTurma.Where(gm => gm.GrupoMatriz != null).GroupBy(cc => cc.GrupoMatriz);

                    //Obter grupo matriz
                    var baseNacionalComum = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 1)?.Select(b => b);
                    var diversificados = componentesPorGrupoMatriz.Where(cpm => cpm.Key.Id == 2 || cpm.Key.Id > 4)?.SelectMany(d => d);
                    var enriquecimentos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 3)?.Select(e => e);
                    var projetos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 4)?.Select(p => p);
                    //

                    var turmasHistorico = agrupamentoTurmas.Where(t => request.Transferencias == null || !request.Transferencias.Any(tt => tt.CodigoTurma == t.Codigo && tt.CodigoAluno == aluno.Key));

                    var notasAluno = request.Notas.Where(n => turmasHistorico.Select(t => t.Codigo).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Key && (w.PeriodoEscolar == null || w.PeriodoEscolar.Bimestre == 0));
                    var frequenciasAluno = request.Frequencias.Where(f => turmasHistorico.Select(t => t.Codigo).Contains(f.Key)).SelectMany(a => a).Where(a => a.CodigoAluno == aluno.Key);

                    var baseNacionalDto = ObterBaseNacionalComum(turmasHistorico, notasAluno, frequenciasAluno, request.MediasFrequencia, baseNacionalComum, request.AreasConhecimento, request.GrupoAreaOrdenacao);
                    var diversificadosDto = ObterGruposDiversificado(turmasHistorico, notasAluno, frequenciasAluno, request.MediasFrequencia, diversificados, request.AreasConhecimento, request.GrupoAreaOrdenacao);
                    var enriquecimentoDto = MontarComponentesNotasFrequencia(turmasHistorico, enriquecimentos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();
                    var projetosDto = MontarComponentesNotasFrequencia(turmasHistorico, projetos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();

                    var tiposNotaDto = MapearTiposNota(agrupamentoTurmas.Key, request.TiposNota);
                    var pareceresDto = MapearPareceres(turmasHistorico, notasAluno);

                    var responsaveisUe = ObterResponsaveisUe(request.ImprimirDadosResponsaveis, request.DadosDiretor, request.DadosSecretario);

                    var uesHistorico = request.HistoricoUes?.FirstOrDefault(ue => ue.Key.Item1.ToString() == aluno.Key && ue.Key.Item2 == agrupamentoTurmas.Key)?.ToList();

                    var estudosRealizados = ObterHistoricoUes(uesHistorico)?.ToList();

                    var historicoDto = new HistoricoEscolarDTO()
                    {
                        NomeDre = request.Dre.Nome,
                        Cabecalho = request.Cabecalho,
                        InformacoesAluno = aluno.Select(a => a.Aluno).FirstOrDefault(),
                        DadosHistorico = ObterDadosHistorico(diversificadosDto, baseNacionalDto, enriquecimentoDto, projetosDto, tiposNotaDto, pareceresDto),
                        Modalidade = agrupamentoTurmas.Key,
                        Legenda = ObterLegenda(notasAluno, request.Transferencias, request.Legenda),
                        DadosData = request.DadosData,
                        ResponsaveisUe = responsaveisUe,
                        EstudosRealizados = estudosRealizados.Count > 0 ? estudosRealizados : null,
                        DadosTransferencia = ObterDadosTransferencia(request.Transferencias, aluno.Key),
                        ObservacaoComplementar = request.ObservacaoComplementar
                    };

                    listaRetorno.Add(historicoDto);
                }
            }

            listaRetorno = listaRetorno.OrderBy(a => a.InformacoesAluno.Nome).ToList();

            return await Task.FromResult(listaRetorno);
        }

        private LegendaDto ObterLegenda(IEnumerable<NotasAlunoBimestre> notasAluno, IEnumerable<TransferenciaDto> transferencias, LegendaDto legenda)
        {
            var legendaRetorno = new LegendaDto() { Texto = String.Empty };
            if ((notasAluno != null && notasAluno.Any(c => c.NotaConceito.ConceitoId != null)) ||
                (transferencias != null && transferencias.Any(x => x.Legenda.Texto.Contains("Satisfatório"))))
                legendaRetorno.Texto = legenda.TextoConceito;

            if ((notasAluno != null && notasAluno.Any(c => c.NotaConceito.Sintese != null)) ||
                (transferencias != null && transferencias.Any(x => x.Legenda.Texto.Contains("Frequente"))))
                legendaRetorno.Texto = legendaRetorno.Texto != String.Empty ? $"{legendaRetorno.Texto},{legenda.TextoSintese}" : legenda.TextoSintese;

            return legendaRetorno;
        }

        private IEnumerable<UeConclusaoDto> ObterHistoricoUes(List<UeConclusaoPorAlunoAno> uesHistorico)
        {
            if (uesHistorico != null && uesHistorico.Any())
            {
                foreach (var ue in uesHistorico)
                {
                    yield return new UeConclusaoDto()
                    {
                        Ano = ue.TurmaAno,
                        UeNome = ue.UeNome,
                        UeMunicipio = ue.UeMunicipio,
                        UeUf = ue.UeUF
                    };
                }
            }
        }

        private HistoricoEscolarNotasFrequenciaDto ObterDadosHistorico(List<GruposComponentesCurricularesDto> diversificadosDto, BaseNacionalComumDto baseNacionalDto, List<ComponenteCurricularHistoricoEscolarDto> enriquecimentoDto, List<ComponenteCurricularHistoricoEscolarDto> projetosDto, TiposNotaDto tiposNotaDto, ParecerConclusivoDto pareceresDto)
        {
            if ((diversificadosDto == null || !diversificadosDto.Any(d => d.PossuiNotaValida)) &&
                   (baseNacionalDto == null || baseNacionalDto.ObterComNotaValida == null) &&
                   (enriquecimentoDto == null || !enriquecimentoDto.Any(d => d.PossuiNotaValida)) &&
                   (projetosDto == null || !projetosDto.Any(d => d.PossuiNotaValida)))
                return null;
            else
                return new HistoricoEscolarNotasFrequenciaDto()
            {
                GruposComponentesCurriculares = diversificadosDto,
                BaseNacionalComum = baseNacionalDto,
                EnriquecimentoCurricular = enriquecimentoDto,
                ProjetosAtividadesComplementares = projetosDto,
                TipoNota = tiposNotaDto,
                ParecerConclusivo = pareceresDto
            };
        }

        private TransferenciaDto ObterDadosTransferencia(IEnumerable<TransferenciaDto> transferencias, string codigoAluno)
        {
            return transferencias?.FirstOrDefault(t => t.CodigoAluno == codigoAluno);
        }

        private ResponsaveisUeDto ObterResponsaveisUe(bool imprimirDadosResponsaveis, FuncionarioDto dadosDiretor, FuncionarioDto dadosSecretario)
        {
            if (imprimirDadosResponsaveis)
            {
                return new ResponsaveisUeDto()
                {
                    DocumentoDiretor = dadosDiretor?.DocumentoFormatado,
                    NomeDiretor = dadosDiretor?.NomeServidor,
                    DocumentoSecretario = dadosSecretario?.DocumentoFormatado,
                    NomeSecretario = dadosSecretario?.NomeServidor
                };
            }

            return null;
        }

        private TiposNotaDto MapearTiposNota(Modalidade modalidade, IEnumerable<TipoNotaCicloAno> tiposNota)
        {
            return new TiposNotaDto()
            {
                PrimeiroAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("1") && t.Modalidade == modalidade)?.TipoNota,
                SegundoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("2") && t.Modalidade == modalidade)?.TipoNota,
                TerceiroAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("3") && t.Modalidade == modalidade)?.TipoNota,
                QuartoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("4") && t.Modalidade == modalidade)?.TipoNota,
                QuintoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("5") && t.Modalidade == modalidade)?.TipoNota,
                SextoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("6") && t.Modalidade == modalidade)?.TipoNota,
                SetimoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("7") && t.Modalidade == modalidade)?.TipoNota,
                OitavoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("8") && t.Modalidade == modalidade)?.TipoNota,
                NonoAno = tiposNota.FirstOrDefault(t => t.Ano.Equals("9") && t.Modalidade == modalidade)?.TipoNota,
            };
        }

        private ParecerConclusivoDto MapearPareceres(IEnumerable<Turma> turmas, IEnumerable<NotasAlunoBimestre> notas)
        {
            ParecerConclusivoDto parecerConclusivoDto = new ParecerConclusivoDto();

            foreach (var turma in turmas)
            {
                if (notas.Any(n => n.CodigoTurma == turma.Codigo))
                {
                    if (turma.Ano == "1")
                        parecerConclusivoDto.PrimeiroAno = "Promovido";
                    else if (turma.Ano == "2")
                        parecerConclusivoDto.SegundoAno = "Promovido";
                    else if (turma.Ano == "3")
                        parecerConclusivoDto.TerceiroAno = "Promovido";
                    else if (turma.Ano == "4")
                        parecerConclusivoDto.QuartoAno = "Promovido";
                    else if (turma.Ano == "5")
                        parecerConclusivoDto.QuintoAno = "Promovido";
                    else if (turma.Ano == "6")
                        parecerConclusivoDto.SextoAno = "Promovido";
                    else if (turma.Ano == "7")
                        parecerConclusivoDto.SetimoAno = "Promovido";
                    else if (turma.Ano == "8")
                        parecerConclusivoDto.OitavoAno = "Promovido";
                    else if (turma.Ano == "9")
                        parecerConclusivoDto.NonoAno = "Promovido";
                }
            }

            return parecerConclusivoDto;
        }

        private List<GruposComponentesCurricularesDto> ObterGruposDiversificado(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                            IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz).OrderBy(g => g.Key.Id);

                foreach (var componentes in componentesPorGrupoMatriz)
                {
                    var areasConhecimento = MapearAreasDoConhecimento(componentes, areasDoConhecimentos, grupoAreaOrdenacao, componentes.Key.Id);

                    gruposComponentes.Add(new GruposComponentesCurricularesDto
                    {
                        Nome = componentes.Key.Nome,
                        AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoDto()
                        {
                            Nome = !string.IsNullOrEmpty(ac.Key.Nome) ? ac.Key.Nome : string.Empty,
                            ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                    ObterComponentesDasAreasDeConhecimento(componentes, ac),
                                                    notasAlunos, frequencias, mediasFrequencia, ac)?.OrderBy(o => o.Nome).ToList()
                        }).ToList()
                    });
                }
            }

            return gruposComponentes?.Select(gc => gc.ObterAreasComNotaValida)?.Where(gc => gc.AreasDeConhecimento.Any())?.ToList();
        }

        private BaseNacionalComumDto ObterBaseNacionalComum(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                            IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao)
        {
            BaseNacionalComumDto baseNacional = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos, grupoAreaOrdenacao, 1);

                baseNacional = new BaseNacionalComumDto()
                {
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoDto()
                    {
                        Nome = !string.IsNullOrEmpty(ac.Key.Nome) ? ac.Key.Nome : string.Empty,
                        ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                ObterComponentesDasAreasDeConhecimento(componentesCurricularesDaTurma, ac),
                                                notasAlunos, frequencias, mediasFrequencia, ac)?.OrderBy(o => o.Nome).ToList()
                    }).ToList()
                };
            }

            return baseNacional.ObterAreasComNotaValida;
        }

        private IEnumerable<ComponenteCurricularPorTurma> ObterComponentesDasAreasDeConhecimento(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                                IEnumerable<AreaDoConhecimento> areaDoConhecimento)
        {
            return componentesCurricularesDaTurma.Where(c => (!c.Regencia && areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Contains(c.CodDisciplina)) ||
                                                            (c.Regencia && areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Any(cr =>
                                                            c.ComponentesCurricularesRegencia.Select(cr => cr.CodDisciplina).Contains(cr)))).OrderBy(cc => cc.Disciplina);
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

        private IEnumerable<ComponenteCurricularHistoricoEscolarDto> MontarComponentesNotasFrequencia(IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var componentes = new List<ComponenteCurricularHistoricoEscolarDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {


                List<ComponenteCurricularHistoricoEscolarDto> componentesRegencia = new List<ComponenteCurricularHistoricoEscolarDto>();

                foreach (var componenteCurricular in componentesCurricularesDaTurma.Where(x => !x.Regencia))
                {
                    var regencia = componentesRegencia.FirstOrDefault(x => x.Codigo == componenteCurricular.CodDisciplina.ToString());
                    componentes.Add(new ComponenteCurricularHistoricoEscolarDto()
                    {
                        Codigo = componenteCurricular.CodDisciplina.ToString(),
                        Nome = componenteCurricular.Disciplina,
                        Frequencia = componenteCurricular.Frequencia,
                        Nota = componenteCurricular.LancaNota,
                        FrequenciaPrimeiroAno = regencia != null ? regencia.FrequenciaPrimeiroAno : ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaSegundoAno = regencia != null ? regencia.FrequenciaSegundoAno : ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaTerceiroAno = regencia != null ? regencia.FrequenciaTerceiroAno : ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaQuartoAno = regencia != null ? regencia.FrequenciaQuartoAno : ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaQuintoAno = regencia != null ? regencia.FrequenciaQuintoAno : ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "5"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaSextoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "6"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaSetimoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "7"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaOitavoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "8"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        FrequenciaNonoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "9"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                        NotaConceitoPrimeiroAno = regencia != null ? regencia.NotaConceitoPrimeiroAno : ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoSegundoAno = regencia != null ? regencia.NotaConceitoSegundoAno : ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoTerceiroAno = regencia != null ? regencia.NotaConceitoTerceiroAno : ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoQuartoAno = regencia != null ? regencia.NotaConceitoQuartoAno : ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoQuintoAno = regencia != null ? regencia.NotaConceitoQuintoAno : ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "5"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoSextoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "6"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoSetimoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "7"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoOitavoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "8"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoNonoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "9"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    });
                }

                var componentesRegenciaFiltro = componentesCurricularesDaTurma.Where(x => x.Regencia);
                foreach (var componenteCurricular in componentesRegenciaFiltro)
                {
                    MapearComponentesRegencia(componenteCurricular.CodDisciplina.ToString(),
                                              turmas,
                                              componenteCurricular.ComponentesCurricularesRegencia.Where(r => areasDoConhecimentos.Select(a => a.CodigoComponenteCurricular).Contains(r.CodDisciplina)),
                                              notas,
                                              frequencia.Where(f => f.DisciplinaId == componenteCurricular.CodDisciplina.ToString()),
                                              mediasFrequencia,
                                              componentes);
                }

            }

            return componentes;
        }

        private string ObterFrequenciaComponentePorTurma(Turma turma, string codigoComponente, IEnumerable<FrequenciaAluno> frequenciaAlunos)
        {
            if (turma != null)
            {
                var frequenciasAlunoParaTratar = frequenciaAlunos.Where(a => a.DisciplinaId == codigoComponente);
                FrequenciaAluno frequenciaAluno;

                if (frequenciasAlunoParaTratar == null || !frequenciasAlunoParaTratar.Any())
                {
                    return string.Empty;
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

                return frequenciaAluno.TotalAulas > 0 ? frequenciaAluno?.PercentualFrequencia.ToString() ?? "100" : "100";
            }
            else
                return null;
        }

        private string ObterNotaComponentePorTurma(Turma turma, string codigoComponente, bool regencia, bool lancaNota, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<NotasAlunoBimestre> notasAluno, IEnumerable<MediaFrequencia> mediasFrequencias)
        {
            if (turma != null)
            {
                if (lancaNota)
                    return notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turma.Codigo)?.NotaConceito?.NotaConceito;
                else
                    return ObterSintese(frequenciaAlunos.Where(f => f.PeriodoEscolarId != null), mediasFrequencias, regencia, !lancaNota);
            }
            else
                return null;
        }

        private void MapearComponentesRegencia(string codigoComponenteRegencia, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurmaRegencia> componentesRegencia, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, List<ComponenteCurricularHistoricoEscolarDto> componentes)
        {
            var frequenciaAno1 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1"), codigoComponenteRegencia, frequencia);
            var frequenciaAno2 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2"), codigoComponenteRegencia, frequencia);
            var frequenciaAno3 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3"), codigoComponenteRegencia, frequencia);
            var frequenciaAno4 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4"), codigoComponenteRegencia, frequencia);
            var frequenciaAno5 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "5"), codigoComponenteRegencia, frequencia);
            var frequenciaAno6 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "6"), codigoComponenteRegencia, frequencia);
            var frequenciaAno7 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "7"), codigoComponenteRegencia, frequencia);
            var frequenciaAno8 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "8"), codigoComponenteRegencia, frequencia);
            var frequenciaAno9 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "9"), codigoComponenteRegencia, frequencia);

            foreach (var componenteCurricular in componentesRegencia)
            {
                if (componentes != null && componentes.Any(c => c.Codigo == componenteCurricular.CodDisciplina.ToString()))
                {
                    var componente = componentes.FirstOrDefault(c => c.Codigo == componenteCurricular.CodDisciplina.ToString());

                    componente.Frequencia = componenteCurricular.Frequencia;
                    componente.Nota = componenteCurricular.LancaNota;
                    componente.FrequenciaPrimeiroAno = frequenciaAno1 ?? componente.FrequenciaPrimeiroAno;
                    componente.FrequenciaSegundoAno = frequenciaAno2 ?? componente.FrequenciaSegundoAno;
                    componente.FrequenciaTerceiroAno = frequenciaAno3 ?? componente.FrequenciaTerceiroAno;
                    componente.FrequenciaQuartoAno = frequenciaAno4 ?? componente.FrequenciaQuartoAno;
                    componente.FrequenciaQuintoAno = frequenciaAno5 ?? componente.FrequenciaQuintoAno;
                    componente.FrequenciaSextoAno = frequenciaAno6 ?? componente.FrequenciaSextoAno;
                    componente.FrequenciaSetimoAno = frequenciaAno7 ?? componente.FrequenciaSetimoAno;
                    componente.FrequenciaOitavoAno = frequenciaAno8 ?? componente.FrequenciaOitavoAno;
                    componente.FrequenciaNonoAno = frequenciaAno9 ?? componente.FrequenciaNonoAno;
                    componente.NotaConceitoPrimeiroAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoPrimeiroAno;
                    componente.NotaConceitoSegundoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoSegundoAno;
                    componente.NotaConceitoTerceiroAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoTerceiroAno;
                    componente.NotaConceitoQuartoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoQuartoAno;
                    componente.NotaConceitoQuintoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "5"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoQuintoAno;
                    componente.NotaConceitoSextoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "6"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoSextoAno;
                    componente.NotaConceitoSetimoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "7"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoSetimoAno;
                    componente.NotaConceitoOitavoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "8"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoOitavoAno;
                    componente.NotaConceitoNonoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "9"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia) ?? componente.NotaConceitoNonoAno;
                }
                else
                {
                    componentes.Add(new ComponenteCurricularHistoricoEscolarDto()
                    {
                        Codigo = componenteCurricular.CodDisciplina.ToString(),
                        Nome = componenteCurricular.Disciplina,
                        Frequencia = componenteCurricular.Frequencia,
                        Nota = componenteCurricular.LancaNota,
                        FrequenciaPrimeiroAno = frequenciaAno1,
                        FrequenciaSegundoAno = frequenciaAno2,
                        FrequenciaTerceiroAno = frequenciaAno3,
                        FrequenciaQuartoAno = frequenciaAno4,
                        FrequenciaQuintoAno = frequenciaAno5,
                        FrequenciaSextoAno = frequenciaAno6,
                        FrequenciaSetimoAno = frequenciaAno7,
                        FrequenciaOitavoAno = frequenciaAno8,
                        FrequenciaNonoAno = frequenciaAno9,
                        NotaConceitoPrimeiroAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoSegundoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoTerceiroAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoQuartoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoQuintoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "5"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoSextoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "6"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoSetimoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "7"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoOitavoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "8"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                        NotaConceitoNonoAno = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "9"), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    });
                }
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
