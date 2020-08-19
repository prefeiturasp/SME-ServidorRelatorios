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
                var dresCodigos = dres.Select(d => d.Codigo).ToArray();

                // Ues
                var ues = await mediator.Send(new ObterPorDresIdQuery(dres.Select(d => d.Id).ToArray()));
                var uesCodigos = await AplicarFiltroPorUe(filtros, ues.OrderBy(u => u.TipoEscola));

                // Cabeçalho
                MontarCabecalho(filtros, relatorioNotasEConceitosFinaisDto);

                // Obter turmas por códigos da UE, Anos, Modalidade e semestre
                var turmas = await mediator.Send(new ObterTurmasPorUeAnosModalidadeSemestreQuery(uesCodigos, filtros.Anos, filtros.Modalidade, filtros.Semestre));
                if (!turmas.Any())
                    throw new NegocioException("Não foi possível localizar turmas para geração do relatório");

                // Filtrar notas
                var notasPorTurmas = await mediator.Send(new ObterNotasFinaisPorTurmasBimestresComponentesQuery(turmas.Select(t => t.Id).ToArray(), dresCodigos, uesCodigos, filtros.Semestre, (int)filtros.Modalidade, filtros.Anos, filtros.AnoLetivo, filtros.Bimestres.ToArray(), filtros.ComponentesCurriculares.ToArray()));

                // Aplicar filtro por condições e valores
                notasPorTurmas = AplicarFiltroPorCondicoesEValores(filtros, notasPorTurmas);

                // Componentes curriculares
                var componentesCurriculares = await ObterComponentesCurriculares(notasPorTurmas);

                // Alunos
                var alunos = await ObterAlunos(notasPorTurmas);

                // Dres
                var dresParaAdicionar = notasPorTurmas.Select(a => new { a.DreCodigo, a.DreNome, a.DreAbreviacao }).Distinct();

                foreach (var dreParaAdicionar in dresParaAdicionar.OrderBy(b => b.DreAbreviacao))
                {
                    var dreNova = new RelatorioNotasEConceitosFinaisDreDto
                    {
                        Codigo = dreParaAdicionar.DreCodigo,
                        Nome = dreParaAdicionar.DreNome
                    };

                    var uesParaAdicionar = notasPorTurmas.Where(a => a.DreCodigo == dreParaAdicionar.DreCodigo).Select(a => new { a.UeCodigo, a.UeNome }).Distinct();

                    foreach (var ueParaAdicionar in uesParaAdicionar)
                    {
                        var ueNova = new RelatorioNotasEConceitosFinaisUeDto
                        {
                            Codigo = ueParaAdicionar.UeCodigo,
                            Nome = ueParaAdicionar.UeNome
                        };

                        var anosParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo).Select(a => a.Ano).Distinct();

                        foreach (var anoParaAdicionar in anosParaAdicionar)
                        {
                            var anoNovo = new RelatorioNotasEConceitosFinaisAnoDto(anoParaAdicionar);

                            var bimestresParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo && a.Ano == anoParaAdicionar).Select(a => a.Bimestre).Distinct();

                            foreach (var bimestreParaAdicionar in bimestresParaAdicionar)
                            {
                                var bimestreNovo = new RelatorioNotasEConceitosFinaisBimestreDto(bimestreParaAdicionar.ToString());

                                var componentesParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo && a.Ano == anoParaAdicionar && a.Bimestre == bimestreParaAdicionar)
                                                                             .Select(a => a.ComponenteCurricularCodigo)
                                                                             .Distinct();


                                foreach (var componenteParaAdicionar in componentesParaAdicionar)
                                {
                                    var componenteNovo = new RelatorioNotasEConceitosFinaisComponenteCurricularDto
                                    {
                                        Nome = componentesCurriculares.FirstOrDefault(a => a.CodDisciplina == componenteParaAdicionar)?.Disciplina
                                    };

                                    var notasDosAlunosParaAdicionar = notasPorTurmas.Where(a => a.UeCodigo == ueParaAdicionar.UeCodigo && a.Ano == anoParaAdicionar
                                                                                           && a.Bimestre == bimestreParaAdicionar && a.ComponenteCurricularCodigo == componenteParaAdicionar)
                                                                                    .Select(a => new { a.AlunoCodigo, a.NotaConceito, a.TurmaNome })
                                                                                    .Distinct();


                                    foreach (var notaDosAlunosParaAdicionar in notasDosAlunosParaAdicionar)
                                    {
                                        var alunoNovo = alunos.FirstOrDefault(a => a.CodigoAluno == int.Parse(notaDosAlunosParaAdicionar.AlunoCodigo));
                                        var notaConceitoNovo = new RelatorioNotasEConceitosFinaisDoAlunoDto(notaDosAlunosParaAdicionar.TurmaNome, alunoNovo?.NumeroAlunoChamada, alunoNovo?.ObterNomeFinal(), notaDosAlunosParaAdicionar.NotaConceito);
                                        componenteNovo.NotaConceitoAlunos.Add(notaConceitoNovo);
                                    }

                                    bimestreNovo.ComponentesCurriculares.Add(componenteNovo);
                                }

                                anoNovo.Bimestres.Add(bimestreNovo);
                            }

                            ueNova.Anos.Add(anoNovo);
                        }
                        dreNova.Ues.Add(ueNova);
                    }
                    relatorioNotasEConceitosFinaisDto.Dres.Add(dreNova);
                }

                if (relatorioNotasEConceitosFinaisDto.Dres.Count == 0)
                    throw new NegocioException("Não encontramos dados para geração do relatório!");

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotasEConceitosFinais", relatorioNotasEConceitosFinaisDto, request.CodigoCorrelacao));
            }
            catch (Exception)
            {
                throw new NegocioException("Não foi possível gerar o relatório");
            }
        }

        private async Task<IEnumerable<AlunoHistoricoEscolar>> ObterAlunos(IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasPorTurmas)
        {
            var alunosCodigos = notasPorTurmas.Select(a => long.Parse(a.AlunoCodigo)).Distinct();
            var alunos = await mediator.Send(new ObterDadosAlunosHistoricoEscolarQuery() { CodigosAluno = alunosCodigos.ToArray() });
            if (alunos == null || !alunos.Any())
                throw new NegocioException("Não foi possível obter os alunos");
            return alunos;
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notasPorTurmas)
        {
            var componentesCurricularesCodigos = notasPorTurmas.Select(a => a.ComponenteCurricularCodigo).Distinct();
            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorIdsQuery() { ComponentesCurricularesIds = componentesCurricularesCodigos.ToArray() });
            if (componentesCurriculares == null || !componentesCurriculares.Any())
                throw new NegocioException("Não foi possível obter os componentes curriculares");
            return componentesCurriculares;
        }

        private IEnumerable<RetornoNotaConceitoBimestreComponenteDto> AplicarFiltroPorCondicoesEValores(FiltroRelatorioNotasEConceitosFinaisDto filtros, IEnumerable<RetornoNotaConceitoBimestreComponenteDto> notas)
        {
            var operacao = new Dictionary<CondicoesRelatorioNotasEConceitosFinais, Func<double, double, bool>>
            {
                { CondicoesRelatorioNotasEConceitosFinais.Igual, (valor, valorFiltro) => valor == valorFiltro },
                { CondicoesRelatorioNotasEConceitosFinais.Maior, (valor, valorFiltro) => valor > valorFiltro },
                { CondicoesRelatorioNotasEConceitosFinais.Menor, (valor, valorFiltro) => valor < valorFiltro }
            };

            return notas.Where(n => operacao[filtros.Condicao](n.Nota, filtros.ValorCondicao)).ToList();
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
                relatorioNotasEConceitosFinaisDto.Ano = "Todos";

            relatorioNotasEConceitosFinaisDto.UsuarioNome = filtros.UsuarioNome;
            relatorioNotasEConceitosFinaisDto.UsuarioRF = filtros.UsuarioRf;
        }
    }
}