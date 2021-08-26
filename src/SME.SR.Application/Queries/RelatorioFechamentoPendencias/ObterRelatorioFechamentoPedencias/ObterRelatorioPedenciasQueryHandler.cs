using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioPedenciasQueryHandler : IRequestHandler<ObterRelatorioPedenciasQuery, RelatorioPendenciasDto>
    {
        private readonly IMediator mediator;
        private readonly IPendenciaRepository fechamentoPendenciaRepository;

        public ObterRelatorioPedenciasQueryHandler(IMediator mediator, IPendenciaRepository fechamentoPendenciaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.fechamentoPendenciaRepository = fechamentoPendenciaRepository ?? throw new ArgumentNullException(nameof(fechamentoPendenciaRepository));
        }
        private dynamic ObterModalidade(int modalidadeCodigo)
        {
            return Enum.GetValues(typeof(Modalidade))
                            .Cast<Modalidade>()
                            .Where(d => (int)d == modalidadeCodigo)
                            .Select(d => new { name = d.Name(), shortName = d.ShortName() }).FirstOrDefault();
        }
        public async Task<RelatorioPendenciasDto> Handle(ObterRelatorioPedenciasQuery request, CancellationToken cancellationToken)
        {
            var filtros = request.FiltroRelatorioPendencias;

            filtros.ExibirDetalhamento = true;

            var resultadoQuery = await fechamentoPendenciaRepository.ObterPendencias(filtros.AnoLetivo, filtros.DreCodigo, filtros.UeCodigo,
                (int)filtros.Modalidade, filtros.Semestre, filtros.TurmasCodigo, filtros.ComponentesCurriculares, filtros.Bimestre, filtros.ExibirPendenciasResolvidas, filtros.TipoPendenciaGrupo, filtros.UsuarioRf, filtros.ExibirHistorico);

            if (resultadoQuery == null || !resultadoQuery.Any())
                throw new NegocioException("Não foram localizadas pendências com os filtros selecionados.");


            //Obter as disciplinas do EOL por código\\
            var componentesCurricularesIds = resultadoQuery.Select(a => a.DisciplinaId).Distinct().ToArray();
            var componentesCurricularesDescricoes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesCurricularesIds));

            if (componentesCurricularesDescricoes == null || !componentesCurricularesDescricoes.Any())
                throw new NegocioException("Não foram localizadas descrições dos componentes curriculares no EOL.");

            var retorno = new RelatorioPendenciasDto();
            var retornoLinearParaCabecalho = resultadoQuery.FirstOrDefault();
            retorno.UsuarioLogadoNome = filtros.UsuarioLogadoNome;
            retorno.UsuarioLogadoRf = filtros.UsuarioLogadoRf;
            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");

            retorno.UeNome = string.IsNullOrEmpty(retornoLinearParaCabecalho.UeNome) ? "Todas" : retornoLinearParaCabecalho.UeNome;
            retorno.DreNome = retornoLinearParaCabecalho.DreNome;
            var qtdModalidades = resultadoQuery?.GroupBy(c => c.ModalidadeCodigo).Count();

            var modalidade = ObterModalidade(retornoLinearParaCabecalho.ModalidadeCodigo);

            if (qtdModalidades == 1)
                retorno.Modalidade = ((Modalidade)retornoLinearParaCabecalho.ModalidadeCodigo).GetAttribute<DisplayAttribute>().Name;

            retorno.Usuario = filtros.UsuarioNome;
            retorno.RF = filtros.UsuarioRf;
            retorno.ExibeDetalhamento = filtros.ExibirDetalhamento;
            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");
            retorno.Semestre = filtros.Semestre.ToString();
            retorno.Ano = filtros.AnoLetivo.ToString();



            if (filtros.TurmasCodigo.Any(t => t != "-99" && t != null))
                retorno.TurmaNome = modalidade.name.ToUpper() + " - " + retornoLinearParaCabecalho.TurmaNome.ToUpper();
            else retorno.TurmaNome = "Todas";

            if (filtros.ComponentesCurriculares?.Count() == 1)
                retorno.ComponenteCurricular = componentesCurricularesDescricoes.FirstOrDefault(a => a.CodDisciplina == filtros.ComponentesCurriculares.FirstOrDefault())?.Disciplina;
            else retorno.ComponenteCurricular = "Todos";

            if (filtros.Bimestre > 0)
                retorno.Bimestre = filtros.Bimestre.ToString();
            else retorno.Bimestre = "Todos";

            retorno.Dre = new RelatorioPendenciasDreDto()
            {
                Codigo = filtros.DreCodigo,
                Nome = retornoLinearParaCabecalho.DreNome
            };

            retorno.Dre.Ue = new RelatorioPendenciasUeDto()
            {
                Codigo = filtros.UeCodigo,
                Nome = retornoLinearParaCabecalho.UeNome
            };

            var turmasCodigos = resultadoQuery.Select(a => a.TurmaCodigo).Distinct();

            foreach (var turmaCodigo in turmasCodigos)
            {
                var turma = new RelatorioPendenciasTurmaDto();

                var bimestresDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo).Select(a => a.Bimestre).Distinct();
                var bimestresCodigoModalidade = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo).Select(a => a.ModalidadeCodigo).Distinct();
                var bimestresNomeModalidade = ObterModalidade(bimestresCodigoModalidade.FirstOrDefault());
                turma.Nome = bimestresNomeModalidade.name.ToUpper() + " - " + resultadoQuery.FirstOrDefault(a => a.TurmaCodigo == turmaCodigo).TurmaNome.ToUpper();
                
                foreach (var bimestreDaTurma in bimestresDaTurma)
                {
                    var bimestreParaAdicionar = new RelatorioPendenciasBimestreDto();

                    if (retornoLinearParaCabecalho.ModalidadeCodigo != (int)Modalidade.Infantil)
                    {
                        bimestreParaAdicionar.NomeBimestre = bimestreDaTurma.ToString() + "º BIMESTRE";
                        bimestreParaAdicionar.NomeModalidade = bimestresNomeModalidade.shortName.ToUpper();

                    }
                    var componentesDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo && a.Bimestre == bimestreDaTurma).Select(a => a.DisciplinaId).Distinct();

                    foreach (var componenteDaTurma in componentesDaTurma)
                    {
                        var componenteParaAdicionar = new RelatorioPendenciasComponenteDto();
                        componenteParaAdicionar.CodigoComponente = componenteDaTurma.ToString();
                        componenteParaAdicionar.NomeComponente = componentesCurricularesDescricoes?.FirstOrDefault(a => a.CodDisciplina == componenteDaTurma)?.Disciplina.ToUpper();

                        var pendenciasDoComponenteDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo && a.Bimestre == bimestreDaTurma && a.DisciplinaId == componenteDaTurma);
                        var pendenciasDoComponenteDaTurmaOrdenado = pendenciasDoComponenteDaTurma.OrderBy(p => p.Criador).OrderBy(p => p.TipoPendencia);
                        foreach (var pendenciaDoComponenteDaTurma in pendenciasDoComponenteDaTurmaOrdenado)
                        {
                            var pendenciaParaAdicionar = new RelatorioPendenciasPendenciaDto();

                            pendenciaParaAdicionar.CodigoUsuarioAprovacaoRf = pendenciaDoComponenteDaTurma.AprovadorRf;
                            pendenciaParaAdicionar.CodigoUsuarioRf = pendenciaDoComponenteDaTurma.CriadorRf;
                            pendenciaParaAdicionar.DescricaoPendencia = pendenciaDoComponenteDaTurma.Titulo;
                            pendenciaParaAdicionar.TipoPendencia = pendenciaDoComponenteDaTurma.TipoPendencia;

                            if (filtros.ExibirDetalhamento)
                            {
                                pendenciaParaAdicionar.DetalhamentoPendencia = UtilRegex.RemoverTagsHtml(pendenciaDoComponenteDaTurma.Detalhe);
                                pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar.DetalhamentoPendencia.Replace("Clique aqui para acessar o plano.", "");
                                pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar.DetalhamentoPendencia.Replace("Clique aqui para acessar o plano e atribuir", "Para resolver esta pendência você precisa atribuir");

                                pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar.DetalhamentoPendencia.Replace("Clique aqui para acessar o encaminhamento.", "");
                                pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar.DetalhamentoPendencia.Replace("Clique aqui para acessar o plano e registrar a devolutiva.", "");
                                pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar.DetalhamentoPendencia.Replace("Clique aqui para acessar o plano e atribuir um PAAI para analisar e realizar a devolutiva.", "");

                            }

                            if (pendenciaDoComponenteDaTurma.TipoPendencia == TipoPendenciaGrupo.Fechamento.Name() && (SituacaoPendencia)pendenciaDoComponenteDaTurma.Situacao == SituacaoPendencia.Aprovada && !String.IsNullOrEmpty(pendenciaDoComponenteDaTurma.Aprovador))
                                pendenciaParaAdicionar.ExibirAprovacao = true;

                            if (!String.IsNullOrEmpty(pendenciaDoComponenteDaTurma.CriadorRf) && pendenciaDoComponenteDaTurma.CriadorRf != "0")
                                pendenciaParaAdicionar.NomeUsuario = pendenciaDoComponenteDaTurma.Criador + $" ({pendenciaDoComponenteDaTurma.CriadorRf})";
                            else
                                pendenciaParaAdicionar.NomeUsuario = pendenciaDoComponenteDaTurma.Criador;

                            if (!String.IsNullOrEmpty(pendenciaDoComponenteDaTurma.AprovadorRf) && pendenciaDoComponenteDaTurma.AprovadorRf != "0")
                                pendenciaParaAdicionar.NomeUsuarioAprovacao = pendenciaDoComponenteDaTurma.Aprovador + $" ({pendenciaDoComponenteDaTurma.AprovadorRf})";
                            else
                                pendenciaParaAdicionar.NomeUsuarioAprovacao = pendenciaDoComponenteDaTurma.Aprovador;

                            pendenciaParaAdicionar.Situacao = ((SituacaoPendencia)pendenciaDoComponenteDaTurma.Situacao).ToString();

                            componenteParaAdicionar.Pendencias.Add(pendenciaParaAdicionar);
                        }


                        bimestreParaAdicionar.Componentes.Add(componenteParaAdicionar);
                    }


                    turma.Bimestres.Add(bimestreParaAdicionar);
                }

                turma.Bimestres.OrderBy(x => x.NomeModalidade).OrderBy(x => x.NomeBimestre);
                retorno.Dre.Ue.Turmas.Add(turma);
            }

            return await Task.FromResult(retorno);
        }
    }
}
