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
    public class MontarHistoricoEscolarEJAQueryHandler : IRequestHandler<MontarHistoricoEscolarEJAQuery, IEnumerable<HistoricoEscolarEJADto>>
    {
        public MontarHistoricoEscolarEJAQueryHandler()
        {

        }

        public async Task<IEnumerable<HistoricoEscolarEJADto>> Handle(MontarHistoricoEscolarEJAQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<HistoricoEscolarEJADto>();

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

                    var notasAluno = request.Notas.Where(n => agrupamentoTurmas.Select(t => t.Codigo).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Aluno.Codigo && w.PeriodoEscolar == null);
                    var frequenciasAluno = request.Frequencias.Where(f => agrupamentoTurmas.Select(t => t.Codigo).Contains(f.Key)).SelectMany(a => a).Where(a => a.CodigoAluno == aluno.Aluno.Codigo);

                    var baseNacionalDto = ObterBaseNacionalComum(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, baseNacionalComum, request.AreasConhecimento);
                    var diversificadosDto = ObterGruposDiversificado(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, diversificados, request.AreasConhecimento);
                    var enriquecimentoDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, enriquecimentos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();
                    var projetosDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, projetos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();

                    var tiposNotaDto = MapearTiposNota(agrupamentoTurmas.Key, request.TiposNota);
                    var pareceresDto = MapearPareceres(agrupamentoTurmas, notasAluno);

                    var responsaveisUe = ObterResponsaveisUe(request.ImprimirDadosResponsaveis, request.DadosDiretor, request.DadosSecretario);

                    var historicoDto = new HistoricoEscolarEJADto()
                    {
                        NomeDre = request.Dre.Nome,
                        Cabecalho = request.Cabecalho,
                        InformacoesAluno = aluno.Aluno,
                        GruposComponentesCurriculares = diversificadosDto,
                        BaseNacionalComum = baseNacionalDto,
                        EnriquecimentoCurricular = enriquecimentoDto,
                        ProjetosAtividadesComplementares = projetosDto,
                        Modalidade = agrupamentoTurmas.Key,
                        TipoNota = tiposNotaDto,
                        ParecerConclusivo = pareceresDto,
                        Legenda = request.Legenda,
                        DadosData = request.DadosData,
                        ResponsaveisUe = responsaveisUe
                    };

                    listaRetorno.Add(historicoDto);
                }
            }

            return await Task.FromResult(listaRetorno);
        }

        private ResponsaveisUeDto ObterResponsaveisUe(bool imprimirDadosResponsaveis, FuncionarioDto dadosDiretor, FuncionarioDto dadosSecretario)
        {
            if (imprimirDadosResponsaveis)
            {
                return new ResponsaveisUeDto()
                {
                    NomeDiretor = dadosDiretor.NomeServidor,
                    NomeSecretario = dadosSecretario.NomeServidor
                };
            }

            return null;
        }

        private TiposNotaEJADto MapearTiposNota(Modalidade modalidade, IEnumerable<TipoNotaCicloAno> tiposNota)
        {
            return new TiposNotaEJADto()
            {
                PrimeiraEtapaCiclo1 = tiposNota.FirstOrDefault(t => t.Ano == 1 && t.Modalidade == modalidade)?.TipoNota,
                SegundaEtapaCiclo1 = tiposNota.FirstOrDefault(t => t.Ano == 1 && t.Modalidade == modalidade)?.TipoNota,
                PrimeiraEtapaCiclo2 = tiposNota.FirstOrDefault(t => t.Ano == 2 && t.Modalidade == modalidade)?.TipoNota,
                SegundaEtapaCiclo2 = tiposNota.FirstOrDefault(t => t.Ano == 2 && t.Modalidade == modalidade)?.TipoNota,
                PrimeiraEtapaCiclo3 = tiposNota.FirstOrDefault(t => t.Ano == 3 && t.Modalidade == modalidade)?.TipoNota,
                SegundaEtapaCiclo3 = tiposNota.FirstOrDefault(t => t.Ano == 3 && t.Modalidade == modalidade)?.TipoNota,
                PrimeiraEtapaCiclo4 = tiposNota.FirstOrDefault(t => t.Ano == 4 && t.Modalidade == modalidade)?.TipoNota,
                SegundaEtapaCiclo4 = tiposNota.FirstOrDefault(t => t.Ano == 4 && t.Modalidade == modalidade)?.TipoNota,
            };
        }

        private ParecerConclusivoEJADto MapearPareceres(IEnumerable<Turma> turmas, IEnumerable<NotasAlunoBimestre> notas)
        {
            ParecerConclusivoEJADto parecerConclusivoDto = new ParecerConclusivoEJADto();

            foreach (var turma in turmas)
            {
                if (notas.Any(n => n.CodigoTurma == turma.Codigo))
                {
                    if (turma.Ano == "1" && turma.EtapaEJA == 1)
                        parecerConclusivoDto.PrimeiraEtapaCiclo1 = "Promovido";
                    else if (turma.Ano == "2" && turma.EtapaEJA == 1)
                        parecerConclusivoDto.PrimeiraEtapaCiclo2 = "Promovido";
                    else if (turma.Ano == "3" && turma.EtapaEJA == 1)
                        parecerConclusivoDto.PrimeiraEtapaCiclo3 = "Promovido";
                    else if (turma.Ano == "4" && turma.EtapaEJA == 1)
                        parecerConclusivoDto.PrimeiraEtapaCiclo4 = "Promovido";
                    else if (turma.Ano == "1" && turma.EtapaEJA == 2)
                        parecerConclusivoDto.SegundaEtapaCiclo1 = "Promovido";
                    else if (turma.Ano == "2" && turma.EtapaEJA == 2)
                        parecerConclusivoDto.SegundaEtapaCiclo2 = "Promovido";
                    else if (turma.Ano == "3" && turma.EtapaEJA == 2)
                        parecerConclusivoDto.SegundaEtapaCiclo3 = "Promovido";
                    else if (turma.Ano == "4" && turma.EtapaEJA == 2)
                        parecerConclusivoDto.SegundaEtapaCiclo4 = "Promovido";
                }
            }

            return parecerConclusivoDto;
        }

        private List<GruposComponentesCurricularesEJADto> ObterGruposDiversificado(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesEJADto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos);

                gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesEJADto
                {
                    Nome = gp.Key.Nome,
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoEJADto()
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

        private BaseNacionalComumEJADto ObterBaseNacionalComum(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            BaseNacionalComumEJADto baseNacional = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos);

                baseNacional = new BaseNacionalComumEJADto()
                {
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoEJADto()
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

        private IEnumerable<ComponenteCurricularHistoricoEscolarEJADto> MontarComponentesNotasFrequencia(IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            List<ComponenteCurricularHistoricoEscolarEJADto> componentes = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                componentes = new List<ComponenteCurricularHistoricoEscolarEJADto>();

                foreach (var componenteCurricular in componentesCurricularesDaTurma)
                {
                    if (componenteCurricular.Regencia)
                    {
                        componentes.AddRange(MapearComponentesRegencia(componenteCurricular.CodDisciplina.ToString(), turmas, componenteCurricular.ComponentesCurricularesRegencia.Where(r => areasDoConhecimentos.Select(a => a.CodigoComponenteCurricular).Contains(r.CodDisciplina)), notas, frequencia.Where(f => f.DisciplinaId == componenteCurricular.CodDisciplina.ToString()), mediasFrequencia));
                    }
                    else
                    {
                        componentes.Add(new ComponenteCurricularHistoricoEscolarEJADto()
                        {
                            Codigo = componenteCurricular.CodDisciplina.ToString(),
                            Nome = componenteCurricular.Disciplina,
                            Frequencia = componenteCurricular.Frequencia,
                            Nota = componenteCurricular.LancaNota,
                            FrequenciaPrimeiraEtapaCiclo1 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaPrimeiraEtapaCiclo2 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaPrimeiraEtapaCiclo3 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaPrimeiraEtapaCiclo4 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSegundaEtapaCiclo1 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSegundaEtapaCiclo2 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSegundaEtapaCiclo3 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            FrequenciaSegundaEtapaCiclo4 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), frequencia),
                            NotaConceitoPrimeiraEtapaCiclo1 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoPrimeiraEtapaCiclo2 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoPrimeiraEtapaCiclo3 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoPrimeiraEtapaCiclo4 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoSegundaEtapaCiclo1 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoSegundaEtapaCiclo2 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoSegundaEtapaCiclo3 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                            NotaConceitoSegundaEtapaCiclo4 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
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

        private IEnumerable<ComponenteCurricularHistoricoEscolarEJADto> MapearComponentesRegencia(string codigoComponenteRegencia, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurmaRegencia> componentesRegencia, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            var componentes = new List<ComponenteCurricularHistoricoEscolarEJADto>();

            var frequenciaPrimeiraEtapaCiclo1 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 1), codigoComponenteRegencia, frequencia);
            var frequenciaPrimeiraEtapaCiclo2 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 1), codigoComponenteRegencia, frequencia);
            var frequenciaPrimeiraEtapaCiclo3 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 1), codigoComponenteRegencia, frequencia);
            var frequenciaPrimeiraEtapaCiclo4 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 1), codigoComponenteRegencia, frequencia);
            var frequenciaSegundaEtapaCiclo1 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 2), codigoComponenteRegencia, frequencia);
            var frequenciaSegundaEtapaCiclo2 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 2), codigoComponenteRegencia, frequencia);
            var frequenciaSegundaEtapaCiclo3 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 2), codigoComponenteRegencia, frequencia);
            var frequenciaSegundaEtapaCiclo4 = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 2), codigoComponenteRegencia, frequencia);

            foreach (var componenteCurricular in componentesRegencia)
            {
                componentes.Add(new ComponenteCurricularHistoricoEscolarEJADto()
                {
                    Codigo = componenteCurricular.CodDisciplina.ToString(),
                    Nome = componenteCurricular.Disciplina,
                    Frequencia = componenteCurricular.Frequencia,
                    Nota = componenteCurricular.LancaNota,
                    FrequenciaPrimeiraEtapaCiclo1 = frequenciaPrimeiraEtapaCiclo1,
                    FrequenciaPrimeiraEtapaCiclo2 = frequenciaPrimeiraEtapaCiclo2,
                    FrequenciaPrimeiraEtapaCiclo3 = frequenciaPrimeiraEtapaCiclo3,
                    FrequenciaPrimeiraEtapaCiclo4 = frequenciaPrimeiraEtapaCiclo4,
                    FrequenciaSegundaEtapaCiclo1 = frequenciaSegundaEtapaCiclo1,
                    FrequenciaSegundaEtapaCiclo2 = frequenciaSegundaEtapaCiclo2,
                    FrequenciaSegundaEtapaCiclo3 = frequenciaSegundaEtapaCiclo3,
                    FrequenciaSegundaEtapaCiclo4 = frequenciaSegundaEtapaCiclo4,
                    NotaConceitoPrimeiraEtapaCiclo1 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoPrimeiraEtapaCiclo2 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoPrimeiraEtapaCiclo3 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoPrimeiraEtapaCiclo4 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 1), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoSegundaEtapaCiclo1 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "1" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoSegundaEtapaCiclo2 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "2" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoSegundaEtapaCiclo3 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "3" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia),
                    NotaConceitoSegundaEtapaCiclo4 = ObterNotaComponentePorTurma(turmas.FirstOrDefault(f => f.Ano == "4" && f.EtapaEJA == 2), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia)
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
