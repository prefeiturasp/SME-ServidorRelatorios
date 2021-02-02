using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioFechamentoPedenciasQueryHandler : IRequestHandler<ObterRelatorioFechamentoPedenciasQuery, RelatorioFechamentoPendenciasDto>
    {
        private readonly IMediator mediator;
        private readonly IFechamentoPendenciaRepository fechamentoPendenciaRepository;        

        public ObterRelatorioFechamentoPedenciasQueryHandler(IMediator mediator, IFechamentoPendenciaRepository fechamentoPendenciaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.fechamentoPendenciaRepository = fechamentoPendenciaRepository ?? throw new ArgumentNullException(nameof(fechamentoPendenciaRepository));
        }

        public async Task<RelatorioFechamentoPendenciasDto> Handle(ObterRelatorioFechamentoPedenciasQuery request, CancellationToken cancellationToken)
        {
            var filtros = request.filtroRelatorioPendenciasFechamentoDto;

            var resultadoQuery = await fechamentoPendenciaRepository.ObterPendencias(filtros.AnoLetivo, filtros.DreCodigo, filtros.UeCodigo, 
                (int)filtros.Modalidade, filtros.Semestre, filtros.TurmasCodigo, filtros.ComponentesCurriculares, filtros.Bimestre);

            if (!resultadoQuery.Any())
                throw new NegocioException("Não foram localizadas pendências com os filtros selecionados.");


            //Obter as disciplinas do EOL por código\\
            var componentesCurricularesIds = resultadoQuery.Select(a => a.DisciplinaId).Distinct().ToArray();
            var componentesCurricularesDescricoes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery() { ComponentesCurricularesIds = componentesCurricularesIds });
            
            if (!componentesCurricularesDescricoes.Any())
                throw new NegocioException("Não foram localizadas descrições dos componentes curriculares no EOL.");

            var retorno = new RelatorioFechamentoPendenciasDto();
            var retornoLinearParaCabecalho = resultadoQuery.FirstOrDefault();


            retorno.UeNome = string.IsNullOrEmpty(retornoLinearParaCabecalho.UeNome) ? "Todas" : retornoLinearParaCabecalho.UeNome;
            retorno.DreNome = retornoLinearParaCabecalho.DreNome;
            retorno.Modalidade = ((Modalidade)retornoLinearParaCabecalho.ModalidadeCodigo).GetAttribute<DisplayAttribute>().Name;
            retorno.Usuario = request.filtroRelatorioPendenciasFechamentoDto.UsuarioNome;
            retorno.RF = request.filtroRelatorioPendenciasFechamentoDto.UsuarioRf;
            retorno.ExibeDetalhamento = filtros.ExibirDetalhamento;
            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");
            retorno.Semestre = filtros.Semestre.ToString();
            retorno.Ano = filtros.AnoLetivo.ToString();

            if (filtros.TurmasCodigo.Count() == 1)
                retorno.TurmaNome = retornoLinearParaCabecalho.TurmaNome;
            else retorno.TurmaNome = "Todas";

            if (filtros.ComponentesCurriculares.Count() == 1)
                retorno.ComponenteCurricular = componentesCurricularesDescricoes.FirstOrDefault(a => a.CodDisciplina == filtros.ComponentesCurriculares.FirstOrDefault())?.Disciplina;
            else retorno.ComponenteCurricular = "Todos";

            if (filtros.Bimestre > 0)
                retorno.Bimestre = filtros.Bimestre.ToString();
            else retorno.Bimestre = "Todos";

            retorno.Dre = new RelatorioFechamentoPendenciasDreDto()
            {
                Codigo = filtros.DreCodigo,
                Nome = retornoLinearParaCabecalho.DreNome
            };

            retorno.Dre.Ue = new RelatorioFechamentoPendenciasUeDto() { 
             Codigo = filtros.UeCodigo,
              Nome = retornoLinearParaCabecalho.UeNome            
            };

            var turmasCodigos = resultadoQuery.Select(a => a.TurmaCodigo).Distinct();

            foreach (var turmaCodigo in turmasCodigos)
            {
                var turma = new RelatorioFechamentoPendenciasTurmaDto();
                turma.Nome = resultadoQuery.FirstOrDefault(a => a.TurmaCodigo == turmaCodigo).TurmaNome;

                var bimestresDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo).Select(a => a.Bimestre).Distinct();

                foreach (var bimestreDaTurma in bimestresDaTurma)
                {
                    var bimestreParaAdicionar = new RelatorioFechamentoPendenciasBimestreDto();
                    bimestreParaAdicionar.Nome = bimestreDaTurma.ToString() + "º BIMESTRE";

                    var componentesDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo && a.Bimestre == bimestreDaTurma).Select(a => a.DisciplinaId).Distinct();

                    foreach (var componenteDaTurma in componentesDaTurma)
                    {
                        var componenteParaAdicionar = new RelatorioFechamentoPendenciasComponenteDto();
                        componenteParaAdicionar.CodigoComponente = componenteDaTurma.ToString();
                        componenteParaAdicionar.NomeComponente = componentesCurricularesDescricoes.FirstOrDefault(a => a.CodDisciplina == componenteDaTurma).Disciplina;

                        var pendenciasDoComponenteDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo && a.Bimestre == bimestreDaTurma && a.DisciplinaId == componenteDaTurma);
                        
                        foreach (var pendenciaDoComponenteDaTurma in pendenciasDoComponenteDaTurma)
                        {
                            var pendenciaParaAdicionar = new RelatorioFechamentoPendenciasPendenciaDto();

                            pendenciaParaAdicionar.CodigoUsuarioAprovacaoRf = pendenciaDoComponenteDaTurma.AprovadorRf;
                            pendenciaParaAdicionar.CodigoUsuarioRf = pendenciaDoComponenteDaTurma.CriadorRf;
                            pendenciaParaAdicionar.DescricaoPendencia = pendenciaDoComponenteDaTurma.Titulo;
                            
                            if (filtros.ExibirDetalhamento)
                                pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaDoComponenteDaTurma.Detalhe;
                            
                            pendenciaParaAdicionar.NomeUsuario = pendenciaDoComponenteDaTurma.Criador;
                            pendenciaParaAdicionar.NomeUsuarioAprovacao = pendenciaDoComponenteDaTurma.Aprovador;
                            
                            pendenciaParaAdicionar.Situacao = ((SituacaoPendencia)pendenciaDoComponenteDaTurma.Situacao).ToString();                            

                            componenteParaAdicionar.Pendencias.Add(pendenciaParaAdicionar);
                        }


                        bimestreParaAdicionar.Componentes.Add(componenteParaAdicionar);
                    }


                    turma.Bimestres.Add(bimestreParaAdicionar);
                }

                
                retorno.Dre.Ue.Turmas.Add(turma);
            }



            return await Task.FromResult(retorno);
        }
    }
}
