using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaPdfQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciaPdfQuery, RelatorioFaltasFrequenciaDto>
    {
        private readonly IRelatorioFaltasFrequenciaRepository relatorioFaltasFrequenciaRepository;
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterRelatorioFaltasFrequenciaPdfQueryHandler(IRelatorioFaltasFrequenciaRepository relatorioFaltasFrequenciaRepository,
                                                             IComponenteCurricularRepository componenteCurricularRepository,
                                                             IMediator mediator)
        {
            this.relatorioFaltasFrequenciaRepository = relatorioFaltasFrequenciaRepository ?? throw new ArgumentNullException(nameof(relatorioFaltasFrequenciaRepository));
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFaltasFrequenciaDto> Handle(ObterRelatorioFaltasFrequenciaPdfQuery request, CancellationToken cancellationToken)
        {
            var model = new RelatorioFaltasFrequenciaDto();
            var filtro = request.Filtro;
            var dres = await relatorioFaltasFrequenciaRepository.ObterFaltasFrequenciaPorAno(filtro.AnoLetivo,
                                                                                          filtro.CodigoDre,
                                                                                          filtro.CodigoUe,
                                                                                          filtro.Modalidade,
                                                                                          filtro.AnosEscolares,
                                                                                          filtro.ComponentesCurriculares,
                                                                                          filtro.Bimestres);

            if (dres != null)
            {
                var componentes = await componenteCurricularRepository.ListarComponentes();

                var codigosTurmas = dres.SelectMany(d => d.Ues)
                    .SelectMany(u => u.Anos)
                    .SelectMany(u => u.Bimestres)
                    .SelectMany(u => u.Componentes)
                    .SelectMany(u => u.Alunos)
                    .Select(u => u.CodigoTurma);

                var alunos = await mediator.Send(new ObterAlunosPorAnoQuery(codigosTurmas));

                var frequencias = await mediator.Send(new ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery());

                foreach (var dre in dres)
                {
                    foreach (var ue in dre.Ues)
                    {
                        foreach (var ano in ue.Anos)
                        {
                            foreach (var bimestre in ano.Bimestres)
                            {
                                foreach (var componente in bimestre.Componentes)
                                {
                                    var componenteAtual = componentes.FirstOrDefault(c => c.Codigo == componente.CodigoComponente);
                                    if (componenteAtual != null)
                                        componente.NomeComponente = componenteAtual.Descricao;
                                    
                                    foreach (var aluno in componente.Alunos)
                                    {
                                        var alunoAtual = alunos.SingleOrDefault(c => c.CodigoAluno == aluno.CodigoAluno && c.TurmaCodigo == aluno.CodigoTurma);
                                        if (alunoAtual != null)
                                        {
                                            aluno.NomeAluno = alunoAtual.Nome;
                                            aluno.NumeroChamada = alunoAtual.NumeroChamada;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var selecionouTodasDres = filtro.CodigoDre == "-99";
                var selecionouTodasUes = filtro.CodigoDre == "-99";
                model.Dres = dres.ToList();
                model.Dre = selecionouTodasDres ? "Todas" : dres.FirstOrDefault().NomeDre;
                model.Ue = selecionouTodasUes ? "Todas" : dres.FirstOrDefault().Ues.FirstOrDefault().NomeUe;
                model.Ano = filtro.AnosEscolares.Count() > 1 ? string.Empty : filtro.AnosEscolares.FirstOrDefault();
                model.Bimestre = filtro.Bimestres.Count() > 1 ? string.Empty : $"{filtro.Bimestres.FirstOrDefault()}º Bimestre";
                model.ComponenteCurricular = filtro.ComponentesCurriculares.Count() > 1 ? string.Empty : "ObterNomeDoComponente";
                model.Usuario = request.Filtro.NomeUsuario;
                model.RF = request.Filtro.CodigoRf;
                model.Data = DateTime.Now.ToString("dd/MM/yyyy");
                model.ExibeFaltas = filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Faltas || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos;
                model.ExibeFrequencia = filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Frequencia || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos;
            }

            return await Task.FromResult(model);
        }


        private async Task<IEnumerable<FrequenciaAluno>> ObterFaltasEFrequencias(IEnumerable<string> turmas, IEnumerable<int> bimestresFiltro, IEnumerable<long> componentesCurriculares)
        {
            var faltasFrequenciasAlunos = new List<FrequenciaAluno>();
            
            // Obter bimestres diferentes de "Final"
            var bimestres = bimestresFiltro.Where(c => c != 0);
            if (bimestres != null && bimestres.Any())
            {
                faltasFrequenciasAlunos.AddRange(await mediator.Send(new ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(turmas, bimestres, componentesCurriculares)));
            }

            // Verifica se foi solicitado bimestre final
            if (bimestresFiltro.Any(c => c == 0))
            {
                faltasFrequenciasAlunos.AddRange(await mediator.Send(new ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery(turmas, componentesCurriculares)));
            }

            return faltasFrequenciasAlunos;
        }
    }
}
