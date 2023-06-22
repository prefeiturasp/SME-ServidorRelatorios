using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioNotasEConceitosFinaisPdfQueryHandler : IRequestHandler<ObterRelatorioNotasEConceitosFinaisPdfQuery, RelatorioNotasEConceitosFinaisDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioNotasEConceitosFinaisPdfQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioNotasEConceitosFinaisDto> Handle(ObterRelatorioNotasEConceitosFinaisPdfQuery request, CancellationToken cancellationToken)
        {
            var relatorioNotasEConceitosFinaisDto = new RelatorioNotasEConceitosFinaisDto();
            var filtros = request.FiltroRelatorioNotasEConceitosFinais;

            // Dres
            List<Dre> dres = await AplicarFiltroPorDre(filtros);
            var dresCodigos = dres.Select(d => d.Codigo).ToArray();

            // Ues
            string[] uesCodigos;
            if (!string.IsNullOrEmpty(filtros.UeCodigo))
            {
                var ues = await mediator.Send(new ObterPorDresIdQuery(dres.Select(d => d.Id).ToArray()));
                uesCodigos = await AplicarFiltroPorUe(filtros, ues.OrderBy(u => u.TipoEscola));
            }
            else uesCodigos = new string[0];

            // Filtrar notas
            var notasPorTurmas = await mediator.Send(new ObterNotasFinaisRelatorioNotasConceitosFinaisQuery(dresCodigos, uesCodigos, filtros.Semestre, (int)filtros.Modalidade, filtros.Anos, filtros.AnoLetivo, filtros.Bimestres.ToArray(), filtros.ComponentesCurriculares.ToArray()));

            // Aplicar filtro por condições e valores
            notasPorTurmas = AplicarFiltroPorCondicoesEValores(filtros, notasPorTurmas);

            if (!notasPorTurmas.Any())
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            if (notasPorTurmas.Any(n => n.ConselhoClasseAlunoId.HasValue && n.PossuiTurmaAssociada))
                notasPorTurmas = await ObterTurmasAssociadas(notasPorTurmas);

            await ConverteTipoNotaEJAEdFisica(notasPorTurmas);

            // Componentes curriculares
            var componentesCurriculares = await ObterComponentesCurriculares(notasPorTurmas, filtros.TipoNota);

            // Cabeçalho
            MontarCabecalho(filtros, relatorioNotasEConceitosFinaisDto, componentesCurriculares);

            // Alunos
            var alunos = await ObterAlunos(notasPorTurmas);

            // Dres
            var dresParaAdicionar = notasPorTurmas.Select(a => new { a.DreCodigo, a.DreNome, a.DreAbreviacao }).Distinct();

            bool possuiNotaFechamento = false;

            foreach (var dreParaAdicionar in dresParaAdicionar.OrderBy(b => b.DreAbreviacao))
            {
                var dreNova = new RelatorioNotasEConceitosFinaisDreDto
                {
                    Codigo = dreParaAdicionar.DreCodigo,
                    Nome = dreParaAdicionar.DreNome
                };

                var uesParaAdicionar = notasPorTurmas.Where(a => a.DreCodigo == dreParaAdicionar.DreCodigo).Select(a => new { a.UeCodigo, a.UeNomeComTipoEscola }).Distinct();

                foreach (var ueParaAdicionar in uesParaAdicionar)
                {
                    var ueNova = new RelatorioNotasEConceitosFinaisUeDto
                    {
                        Codigo = ueParaAdicionar.UeCodigo,
                        Nome = ueParaAdicionar.UeNomeComTipoEscola
                    };

                    var anosParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo).Select(a => a.Ano).Distinct();

                    foreach (var anoParaAdicionar in anosParaAdicionar)
                    {
                        var anoNovo = new RelatorioNotasEConceitosFinaisAnoDto($"{anoParaAdicionar} ANO");

                        var bimestresParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo && a.Ano == anoParaAdicionar).Select(a => a.Bimestre).Distinct();

                        foreach (var bimestreParaAdicionar in bimestresParaAdicionar)
                        {
                            var bimestreNovo = new RelatorioNotasEConceitosFinaisBimestreDto(bimestreParaAdicionar != null ? $"{bimestreParaAdicionar}º BIMESTRE" : $"FINAL");

                            var componentesParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo && a.Ano == anoParaAdicionar && a.Bimestre == bimestreParaAdicionar)
                                                                         .Select(a => a.ComponenteCurricularCodigo)
                                                                         .Distinct();

                            var componentesDetalhes = componentesCurriculares.Where(a => componentesParaAdicionar.Contains(a.CodDisciplina))
                                                                             .OrderBy(c => c.Regencia)
                                                                             .ThenBy(c => c.GrupoMatriz.Id)
                                                                             .ThenByDescending(c => c.AreaDoConhecimento != null && !string.IsNullOrEmpty(c.AreaDoConhecimento.Nome))
                                                                             .ThenBy(c => c.AreaDoConhecimento.Nome, StringComparer.OrdinalIgnoreCase)
                                                                             .ThenBy(c => c.Disciplina, StringComparer.OrdinalIgnoreCase);


                            foreach (var componenteParaAdicionar in componentesDetalhes)
                            {
                                var componenteNovo = new RelatorioNotasEConceitosFinaisComponenteCurricularDto
                                {
                                    Nome = componenteParaAdicionar?.Disciplina
                                };

                                var notasDosAlunosParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo && a.Ano == anoParaAdicionar
                                                                                       && a.Bimestre == bimestreParaAdicionar && a.ComponenteCurricularCodigo == componenteParaAdicionar?.CodDisciplina)
                                                                                .Select(a => new { a.AlunoCodigo, a.NotaConceitoFinal, a.Sintese, a.TurmaNome, a.EhNotaConceitoFechamento, a.ConselhoClasseAlunoId, a.NotaConceitoEmAprovacao, a.NotaConceitoPosConselhoEmAprovacao, a.ConselhoClasseNotaId })
                                                                                .Distinct();


                                foreach (var notaDosAlunosParaAdicionar in notasDosAlunosParaAdicionar)
                                {
                                    if (notaDosAlunosParaAdicionar.EhNotaConceitoFechamento)
                                        possuiNotaFechamento = true;

                                    var alunoNovo = alunos.FirstOrDefault(a => a.CodigoAluno == int.Parse(notaDosAlunosParaAdicionar.AlunoCodigo));
                                    var notaConceitoNovo = new RelatorioNotasEConceitosFinaisDoAlunoDto(notaDosAlunosParaAdicionar.TurmaNome, alunoNovo.CodigoAluno, alunoNovo?.NumeroAlunoChamada, alunoNovo?.ObterNomeFinal(), componenteParaAdicionar.LancaNota ? notaDosAlunosParaAdicionar.NotaConceitoFinal : notaDosAlunosParaAdicionar.Sintese, notaDosAlunosParaAdicionar.ConselhoClasseAlunoId);

                                    if (notaDosAlunosParaAdicionar.NotaConceitoPosConselhoEmAprovacao != null)
                                    {
                                        notaConceitoNovo.NotaConceito = $"{notaDosAlunosParaAdicionar.NotaConceitoPosConselhoEmAprovacao.Replace(".00", ".0")} **";
                                        notaConceitoNovo.EmAprovacao = true;
                                    }
                                    else if (notaDosAlunosParaAdicionar.NotaConceitoEmAprovacao != null && notaDosAlunosParaAdicionar.ConselhoClasseNotaId == null)
                                    {
                                        notaConceitoNovo.NotaConceito = $"{notaDosAlunosParaAdicionar.NotaConceitoEmAprovacao.Replace(".00", ".0")} **";
                                        notaConceitoNovo.EmAprovacao = true;
                                    }

                                    componenteNovo.NotaConceitoAlunos.Add(notaConceitoNovo);
                                }
                                componenteNovo.NotaConceitoAlunos = componenteNovo.NotaConceitoAlunos.OrderBy(t => t.TurmaNome).ThenBy(a => a.AlunoNomeCompleto).ToList();
                                bimestreNovo.ComponentesCurriculares.Add(componenteNovo);
                            }

                            anoNovo.Bimestres.Add(bimestreNovo);
                        }

                        anoNovo.Bimestres = anoNovo.Bimestres.OrderBy(a => a.Nome).ToList();
                        ueNova.Anos.Add(anoNovo);
                    }
                    ueNova.Anos = ueNova.Anos.OrderBy(a => a.Nome).ToList();
                    dreNova.Ues.Add(ueNova);
                }
                relatorioNotasEConceitosFinaisDto.Dres.Add(dreNova);
                relatorioNotasEConceitosFinaisDto.PossuiNotaFechamento = possuiNotaFechamento;
            }

            if (relatorioNotasEConceitosFinaisDto.Dres.Count == 0)
                throw new NegocioException("Não encontramos dados para geração do relatório!");

            return relatorioNotasEConceitosFinaisDto;
        }

        private async Task<IEnumerable<RetornoNotaConceitoBimestreComponenteDto>> ObterTurmasAssociadas(IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasPorTurmas)
        {
            var notasTurmasAssociadas = notasPorTurmas.Where(n => n.ConselhoClasseAlunoId.HasValue && n.PossuiTurmaAssociada);

            var conselhoClasseAlunoIds = notasTurmasAssociadas.Select(n => n.ConselhoClasseAlunoId.Value).ToArray();
            var turmasAssociadas = await mediator.Send(new ObterTurmasAssociadasConselhoClasseAlunoQuery(conselhoClasseAlunoIds));

            var turmasAssociadasIds = turmasAssociadas.Select(t => t.TurmaComplementarId).Distinct().ToArray();
            var turmaAssociadaCodigos = turmasAssociadas.Select(t => t.TurmaComplementarCodigo).Distinct().ToArray();
            var turmaRegularCodigos = turmasAssociadas.Select(t => t.TurmaRegularCodigo).Distinct().ToArray();

            var componentesDasTurmas = await mediator.Send(new ObterComponentesCurricularesPorTurmasQuery(turmaAssociadaCodigos));
            var turmasAssociadasObj = await mediator.Send(new ObterTurmasPorIdsQuery(turmasAssociadasIds));

            var notasComponentesTurmasAssociadas = notasTurmasAssociadas.Where(n => componentesDasTurmas.Any(c => c.CodDisciplina == n.ComponenteCurricularCodigo));

            foreach (var nota in notasComponentesTurmasAssociadas)
            {
                var associacao = turmasAssociadas.FirstOrDefault(t => t.ConselhoClasseAlunoId == nota.ConselhoClasseAlunoId);

                var turmaAssociada = turmasAssociadasObj.FirstOrDefault(t => t.Id == associacao.TurmaComplementarId);

                nota.TurmaCodigo = turmaAssociada.Codigo;
                nota.TurmaNome = turmaAssociada.Nome;
                nota.Ano = turmaAssociada.Ano;
            }

            var notasTurmasAssociadasConselho = notasComponentesTurmasAssociadas.Where(n => !n.EhNotaConceitoFechamento);

            foreach (var notaConselho in notasTurmasAssociadasConselho)
            {
                var notaFechamento = notasPorTurmas.FirstOrDefault(n => n.EhNotaConceitoFechamento &&
                                                                   n.TurmaCodigo == notaConselho.TurmaCodigo &&
                                                                   n.Ano == notaConselho.Ano &&
                                                                   n.Bimestre == notaConselho.Bimestre &&
                                                                   n.ComponenteCurricularCodigo == notaConselho.ComponenteCurricularCodigo &&
                                                                   n.AlunoCodigo == notaConselho.AlunoCodigo);

                if (notaFechamento != null)
                    notaFechamento.ExcluirNota = true;
            }

            return notasPorTurmas.Where(n => !n.ExcluirNota);
        }

        private async Task<IEnumerable<AlunoHistoricoEscolar>> ObterAlunos(IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasPorTurmas)
        {
            var alunosCodigos = notasPorTurmas.Select(a => long.Parse(a.AlunoCodigo)).Distinct();
            var alunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunosCodigos.ToArray()));
            if (alunos == null || !alunos.Any())
                throw new NegocioException("Não foi possível obter os alunos");
            return alunos;
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasPorTurmas, TipoNota filtro)
        {
            var componentesCurricularesCodigos = notasPorTurmas.Select(a => a.ComponenteCurricularCodigo).Distinct();
            var turmasCodigo = notasPorTurmas.Select(a => a.TurmaCodigo).Distinct().ToArray();
            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery() { ComponentesCurricularesIds = componentesCurricularesCodigos.ToArray(), TurmasId = turmasCodigo });
            if (componentesCurriculares == null || !componentesCurriculares.Any())
                throw new NegocioException("Não foi possível obter os componentes curriculares");
            return componentesCurriculares.Where(cc => filtro == TipoNota.Todas || (cc.LancaNota == (filtro != TipoNota.Sintese)));
        }

        private IEnumerable<RetornoNotaConceitoBimestreComponenteDto> AplicarFiltroPorCondicoesEValores(FiltroRelatorioNotasEConceitosFinaisDto filtros, IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notas)
        {

            if (filtros.TipoNota == TipoNota.Conceito)
                return notas.Where(a => a.ConceitoId == filtros.ValorCondicao && a.SinteseId == null).ToList();

            if (filtros.TipoNota == TipoNota.Sintese)
                return notas.Where(a => a.SinteseId == filtros.ValorCondicao).ToList();

            if (filtros.TipoNota != TipoNota.Todas)
            {
                switch (filtros.Condicao)
                {
                    case CondicoesRelatorioNotasEConceitosFinais.Igual:
                        return notas.Where(a => a.Nota == filtros.ValorCondicao && a.ConceitoId == null && a.SinteseId == null).ToList();
                    case CondicoesRelatorioNotasEConceitosFinais.Maior:
                        return notas.Where(a => a.Nota > filtros.ValorCondicao && a.ConceitoId == null && a.SinteseId == null).ToList();
                    case CondicoesRelatorioNotasEConceitosFinais.Menor:
                        return notas.Where(a => a.Nota < filtros.ValorCondicao && a.ConceitoId == null && a.SinteseId == null).ToList();
                    default:
                        break;
                }
            }

            return notas;
        }

        private async Task<List<Dre>> AplicarFiltroPorDre(FiltroRelatorioNotasEConceitosFinaisDto filtros)
        {
            var dres = new List<Dre>();

            if (!string.IsNullOrEmpty(filtros.DreCodigo))
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

        private async void MontarCabecalho(FiltroRelatorioNotasEConceitosFinaisDto filtros, RelatorioNotasEConceitosFinaisDto relatorioNotasEConceitosFinaisDto, IEnumerable<ComponenteCurricularPorTurma> componentes)
        {
            if (string.IsNullOrEmpty(filtros.DreCodigo))
            {
                relatorioNotasEConceitosFinaisDto.DreNome = "Todas";
            }
            else
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                relatorioNotasEConceitosFinaisDto.DreNome = dre.Abreviacao;
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
            else if (filtros.ComponentesCurriculares.Length == 1)
                relatorioNotasEConceitosFinaisDto.ComponenteCurricular = componentes.FirstOrDefault(a => a.CodDisciplina == filtros.ComponentesCurriculares[0])?.Disciplina;

            if (filtros.Bimestres == null || filtros.Bimestres.Count > 1)
                relatorioNotasEConceitosFinaisDto.Bimestre = "Todos";
            else if (filtros.Bimestres != null && filtros.Bimestres.Count == 1)
                relatorioNotasEConceitosFinaisDto.Bimestre = $"{(filtros.Bimestres[0] == 0 ? $"FINAL" : $"{filtros.Bimestres[0]}º")}";

            if (filtros.Anos == null || filtros.Anos.Length == 0)
                relatorioNotasEConceitosFinaisDto.Ano = "Todos";
            else if (filtros.Anos.Length == 1)
                relatorioNotasEConceitosFinaisDto.Ano = filtros.Anos[0];

            relatorioNotasEConceitosFinaisDto.UsuarioNome = filtros.UsuarioNome;
            relatorioNotasEConceitosFinaisDto.UsuarioRF = filtros.UsuarioRf;
        }

        private async Task ConverteTipoNotaEJAEdFisica(IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasConceitos)
        {
            const long ED_FISICA = 6;

            var notasConceitosEdFisica = notasConceitos.Where(nc => nc.ComponenteCurricularCodigo == ED_FISICA);
            var codigosTurmas = notasConceitosEdFisica?.Select(nc => nc.TurmaCodigo).Distinct().ToArray();

            if (codigosTurmas.Any())
            {
                var turmas = await mediator.Send(new ObterTurmasPorCodigoQuery(codigosTurmas));
                var turmasEja = turmas?.Where(turma => turma.EhEja && turma.TipoTurma == TipoTurma.EdFisica);

                foreach(var turmaEja in turmasEja)
                {
                    await ConverteValorNotaPorTurmaEja(turmaEja, notasConceitosEdFisica);
                }
            }
        }

        private async Task ConverteValorNotaPorTurmaEja(Turma turmaEja, IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasConceitosEdFisica)
        {
            var notasTurma = notasConceitosEdFisica.Where(nc => nc.TurmaCodigo == turmaEja.Codigo);

            if (notasTurma.Any())
            {
                var codigosAlunos = notasTurma.Select(nc => nc.AlunoCodigo)?.Distinct().ToArray();
                var tipoNotaAluno = await mediator.Send(new ObterTipoTurmaRegularParaEdFisicaQuery(turmaEja, codigosAlunos));

                foreach (var nota in notasTurma)
                {
                    nota.CarregaTipoNota(tipoNotaAluno[nota.AlunoCodigo]);
                }
            }
        }
    }
}
