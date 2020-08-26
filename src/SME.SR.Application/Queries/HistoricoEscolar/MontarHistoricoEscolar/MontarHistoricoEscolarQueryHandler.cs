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

            foreach (var aluno in request.AlunosTurmas)
            {
                var alunoTurmasPorModalidade = aluno.Turmas.GroupBy(t => t.ModalidadeCodigo);

                foreach (var agrupamentoTurmas in alunoTurmasPorModalidade)
                {
                    var componentesDaTurma = request.ComponentesCurricularesTurmas.Where(cc => agrupamentoTurmas.Select(c => c.Codigo).Contains(cc.Key)).SelectMany(ct => ct).DistinctBy(d => d.CodDisciplina);
                    var componentesPorGrupoMatriz = componentesDaTurma.Where(gm => gm.GrupoMatriz != null).GroupBy(cc => cc.GrupoMatriz);

                    //Obter grupo matriz
                    var baseNacionalComum = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 1)?.Select(b => b);
                    var diversificados = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 2)?.Select(d => d);
                    var enriquecimentos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 3)?.Select(e => e);
                    var projetos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 4)?.Select(p => p);
                    //

                    var notasAluno = request.Notas.Where(n => agrupamentoTurmas.Select(t => t.Codigo).Contains(n.Key) && !request.Transferencias.Select(t => t.CodigoTurma).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Aluno.Codigo && w.PeriodoEscolar == null);
                    var frequenciasAluno = request.Frequencias.Where(f => agrupamentoTurmas.Select(t => t.Codigo).Contains(f.Key) && !request.Transferencias.Select(t => t.CodigoTurma).Contains(f.Key)).SelectMany(a => a).Where(a => a.CodigoAluno == aluno.Aluno.Codigo);

                    var baseNacionalDto = ObterBaseNacionalComum(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, baseNacionalComum, request.AreasConhecimento);
                    var diversificadosDto = ObterGruposDiversificado(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, diversificados, request.AreasConhecimento);
                    var enriquecimentoDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, enriquecimentos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();
                    var projetosDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, projetos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();

                    var tiposNotaDto = MapearTiposNota(agrupamentoTurmas.Key, request.TiposNota);
                    var pareceresDto = MapearPareceres(agrupamentoTurmas, notasAluno);

                    var responsaveisUe = ObterResponsaveisUe(request.ImprimirDadosResponsaveis, request.DadosDiretor, request.DadosSecretario);

                    var historicoDto = new HistoricoEscolarDTO()
                    {
                        NomeDre = request.Dre.Nome,
                        Cabecalho = request.Cabecalho,
                        InformacoesAluno = aluno.Aluno,
                        GruposComponentesCurriculares = diversificadosDto.Any(d => d.PossuiNotaValida) ? diversificadosDto : null,
                        BaseNacionalComum = baseNacionalDto.ObterComNotaValida,
                        EnriquecimentoCurricular = enriquecimentoDto.Any(d => d.PossuiNotaValida) ? enriquecimentoDto : null,
                        ProjetosAtividadesComplementares = projetosDto.Any(d => d.PossuiNotaValida) ? projetosDto : null,
                        Modalidade = agrupamentoTurmas.Key,
                        TipoNota = tiposNotaDto,
                        ParecerConclusivo = pareceresDto,
                        Legenda = request.Legenda,
                        DadosData = request.DadosData,
                        ResponsaveisUe = responsaveisUe,
                        DadosTransferencia = ObterDadosTransferencia(request.Transferencias, aluno.Aluno.Codigo)
                    };

                    listaRetorno.Add(historicoDto);
                }
            }

            return await Task.FromResult(listaRetorno);
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
                PrimeiroAno = tiposNota.FirstOrDefault(t => t.Ano == 1 && t.Modalidade == modalidade)?.TipoNota,
                SegundoAno = tiposNota.FirstOrDefault(t => t.Ano == 2 && t.Modalidade == modalidade)?.TipoNota,
                TerceiroAno = tiposNota.FirstOrDefault(t => t.Ano == 3 && t.Modalidade == modalidade)?.TipoNota,
                QuartoAno = tiposNota.FirstOrDefault(t => t.Ano == 4 && t.Modalidade == modalidade)?.TipoNota,
                QuintoAno = tiposNota.FirstOrDefault(t => t.Ano == 5 && t.Modalidade == modalidade)?.TipoNota,
                SextoAno = tiposNota.FirstOrDefault(t => t.Ano == 6 && t.Modalidade == modalidade)?.TipoNota,
                SetimoAno = tiposNota.FirstOrDefault(t => t.Ano == 7 && t.Modalidade == modalidade)?.TipoNota,
                OitavoAno = tiposNota.FirstOrDefault(t => t.Ano == 8 && t.Modalidade == modalidade)?.TipoNota,
                NonoAno = tiposNota.FirstOrDefault(t => t.Ano == 9 && t.Modalidade == modalidade)?.TipoNota,
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
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos);

                gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesDto
                {
                    Nome = gp.Key.Nome,
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                ObterComponentesDasAreasDeConhecimento(componentesCurricularesDaTurma, ac),
                                                notasAlunos, frequencias, mediasFrequencia, ac)?.ToList()
                    }).ToList()
                }));
            }

            return gruposComponentes;
        }

        private BaseNacionalComumDto ObterBaseNacionalComum(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            BaseNacionalComumDto baseNacional = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos);

                baseNacional = new BaseNacionalComumDto()
                {
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                ObterComponentesDasAreasDeConhecimento(componentesCurricularesDaTurma, ac),
                                                notasAlunos, frequencias, mediasFrequencia, ac)?.ToList()
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

        private IEnumerable<IGrouping<(string Nome, long Id), AreaDoConhecimento>> MapearAreasDoConhecimento(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                                             IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {

            return areasDoConhecimentos.Where(a => ((componentesCurricularesDaTurma.Where(cc => !cc.Regencia).Select(cc => cc.CodDisciplina).Contains(a.CodigoComponenteCurricular)) ||
                                                                    (componentesCurricularesDaTurma.Any(cc => cc.Regencia) &&
                                                                     componentesCurricularesDaTurma.Where(cc => cc.Regencia).SelectMany(cc => cc.ComponentesCurricularesRegencia).
                                                                     Select(r => r.CodDisciplina).Contains(a.CodigoComponenteCurricular)))).GroupBy(g => (g.Nome, g.Id));
        }

        private IEnumerable<ComponenteCurricularHistoricoEscolarDto> MontarComponentesNotasFrequencia(IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            List<ComponenteCurricularHistoricoEscolarDto> componentes = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                componentes = new List<ComponenteCurricularHistoricoEscolarDto>();

                foreach (var componenteCurricular in componentesCurricularesDaTurma)
                {
                    if (componenteCurricular.Regencia)
                    {
                        componentes.AddRange(MapearComponentesRegencia(componenteCurricular.CodDisciplina.ToString(), turmas, componenteCurricular.ComponentesCurricularesRegencia.Where(r => areasDoConhecimentos.Select(a => a.CodigoComponenteCurricular).Contains(r.CodDisciplina)), notas, frequencia.Where(f => f.DisciplinaId == componenteCurricular.CodDisciplina.ToString()), mediasFrequencia));
                    }
                    else
                    {
                        componentes.Add(new ComponenteCurricularHistoricoEscolarDto()
                        {
                            Codigo = componenteCurricular.CodDisciplina.ToString(),
                            Nome = componenteCurricular.Disciplina,
                            Frequencia = componenteCurricular.Frequencia,
                            Nota = componenteCurricular.LancaNota,
                            FrequenciaPrimeiroAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSegundoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaTerceiroAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaQuartoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaQuintoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "5"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSextoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "6"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSetimoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "7"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaOitavoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "8"), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaNonoAno = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "9"), componenteCurricular.CodDisciplina.ToString(), frequencia),
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

            return componentes;
        }

        private string ObterFrequenciaComponentePorTurma(Turma turma, string codigoComponente, IEnumerable<FrequenciaAluno> frequenciaAlunos)
        {
            if (turma != null)
                return frequenciaAlunos.FirstOrDefault(f => f.DisciplinaId == codigoComponente && f.TurmaId == turma.Codigo)?.PercentualFrequencia.ToString() ?? "100";
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

        private IEnumerable<ComponenteCurricularHistoricoEscolarDto> MapearComponentesRegencia(string codigoComponenteRegencia, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurmaRegencia> componentesRegencia, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            var componentes = new List<ComponenteCurricularHistoricoEscolarDto>();

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
