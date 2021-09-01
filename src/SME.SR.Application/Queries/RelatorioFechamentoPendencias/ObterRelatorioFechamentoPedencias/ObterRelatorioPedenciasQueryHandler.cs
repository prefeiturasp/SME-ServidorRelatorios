using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

            //ObterDescricaoPendenciaAula(resultadoQuery);
            //ObterDescricaoPendenciaEvento(resultadoQuery);
            //ObterDescricaoPendenciaAusenciaAvaliacaoCP(resultadoQuery);
            //ObterDescricaoPendenciaAusenciaRegistroIndividualAsync(resultadoQuery);

            if (resultadoQuery == null || !resultadoQuery.Any())
                throw new NegocioException("Não foram localizadas pendências com os filtros selecionados.");


            //Obter as disciplinas do EOL por código\\
            var componentesCurricularesIds = resultadoQuery.Select(a => a.DisciplinaId).Distinct().ToArray();
            var componentesCurricularesDescricoes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesCurricularesIds));

            if (componentesCurricularesDescricoes == null || !componentesCurricularesDescricoes.Any())
                throw new NegocioException("Não foram localizadas descrições dos componentes curriculares no EOL.");

            var retorno = new RelatorioPendenciasDto();
            var retornoLinearParaCabecalho = resultadoQuery.Where(x => x.DreNome.Length > 0 && x.UeNome.Length > 0).FirstOrDefault();
            retorno.UsuarioLogadoNome = filtros.UsuarioLogadoNome;
            retorno.UsuarioLogadoRf = filtros.UsuarioLogadoRf;
            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");

            retorno.UeNome = string.IsNullOrEmpty(retornoLinearParaCabecalho.UeNome) ? "Todas" : retornoLinearParaCabecalho.UeNome;
            retorno.DreNome = retornoLinearParaCabecalho.DreNome;
            var qtdModalidades = resultadoQuery?.Where(c => c.ModalidadeCodigo > 0).GroupBy(c => c.ModalidadeCodigo).Count();

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

            var turmasCodigos = resultadoQuery.Where(x => x.OutrasPendencias == false).Select(a => a.TurmaCodigo).Distinct();

            foreach (var turmaCodigo in turmasCodigos)
            {
                var turma = new RelatorioPendenciasTurmaDto();

                if (!string.IsNullOrWhiteSpace(turmaCodigo))
                {
                    var bimestresDaTurma = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo).Select(a => a.Bimestre).Distinct();
                    var semestreDaTurma = resultadoQuery.FirstOrDefault(a => a.TurmaCodigo == turmaCodigo).Semestre.ToUpper();
                    var bimestresCodigoModalidade = resultadoQuery.Where(a => a.TurmaCodigo == turmaCodigo).Select(a => a.ModalidadeCodigo).Distinct();
                    var bimestresNomeModalidade = ObterModalidade(bimestresCodigoModalidade.FirstOrDefault());
                    turma.Nome = bimestresNomeModalidade?.name.ToUpper() + " - " + resultadoQuery.FirstOrDefault(a => a.TurmaCodigo == turmaCodigo).TurmaNome.ToUpper();


                    foreach (var bimestreDaTurma in bimestresDaTurma)
                    {
                        var bimestreParaAdicionar = new RelatorioPendenciasBimestreDto();

                        if (retornoLinearParaCabecalho.ModalidadeCodigo != (int)Modalidade.Infantil)
                        {
                            bimestreParaAdicionar.NomeBimestre = bimestreDaTurma.ToString() + "º BIMESTRE";
                            if (qtdModalidades > 1)
                                bimestreParaAdicionar.NomeModalidade = bimestresNomeModalidade.name.ToUpper();
                            if (bimestreParaAdicionar.NomeModalidade == "EJA" && semestreDaTurma != "0")
                                bimestreParaAdicionar.SemestreTurma = semestreDaTurma + "º SEMESTRE";

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
                                pendenciaParaAdicionar.OutrasPendencias = pendenciaDoComponenteDaTurma.OutrasPendencias;

                                if (filtros.ExibirDetalhamento)
                                {
                                    
                                    //pendenciaParaAdicionar.DetalhamentoPendencia = UtilRegex.RemoverTagsHtml(pendenciaDoComponenteDaTurma.Detalhe);
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
                }
                turma.Bimestres.OrderBy(x => x.NomeModalidade).OrderBy(x => x.NomeBimestre);
                retorno.Dre.Ue.Turmas.Add(turma);
            }

            var outrasPendencias = resultadoQuery.Where(a => a.OutrasPendencias).OrderBy(p => p.TipoPendencia).OrderBy(p => p.Criador).ToList();

            retorno.Dre.Ue.OutrasPendencias = RetornarOutrasPendencias(outrasPendencias, filtros.ExibirDetalhamento);

            return await Task.FromResult(retorno);
        }

        private List<RelatorioPendenciasPendenciaDto> RetornarOutrasPendencias(List<RelatorioPendenciasQueryRetornoDto> outrasPendencias, bool exibirDetalhamento)
        {
            var listaOutrasPendencias = new List<RelatorioPendenciasPendenciaDto>();

            foreach (var item in outrasPendencias)
            {
                var pendenciaParaAdicionar = new RelatorioPendenciasPendenciaDto();

                pendenciaParaAdicionar.CodigoUsuarioAprovacaoRf = item.AprovadorRf;
                pendenciaParaAdicionar.CodigoUsuarioRf = item.CriadorRf;
                pendenciaParaAdicionar.DescricaoPendencia = item.Titulo;
                pendenciaParaAdicionar.TipoPendencia = item.TipoPendencia;
                pendenciaParaAdicionar.OutrasPendencias = item.OutrasPendencias;

                if (exibirDetalhamento)
                {
                    //pendenciaParaAdicionar.DetalhamentoPendencia = UtilRegex.RemoverTagsHtml(item.Detalhes);
                    pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar?.DetalhamentoPendencia?.Replace("Clique aqui para acessar o plano.", "");
                    pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar?.DetalhamentoPendencia?.Replace("Clique aqui para acessar o plano e atribuir", "Para resolver esta pendência você precisa atribuir");

                    pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar?.DetalhamentoPendencia?.Replace("Clique aqui para acessar o encaminhamento.", "");
                    pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar?.DetalhamentoPendencia?.Replace("Clique aqui para acessar o plano e registrar a devolutiva.", "");
                    pendenciaParaAdicionar.DetalhamentoPendencia = pendenciaParaAdicionar?.DetalhamentoPendencia?.Replace("Clique aqui para acessar o plano e atribuir um PAAI para analisar e realizar a devolutiva.", "");

                }

                if (item.TipoPendencia == TipoPendenciaGrupo.Fechamento.Name() && (SituacaoPendencia)item.Situacao == SituacaoPendencia.Aprovada && !String.IsNullOrEmpty(item.Aprovador))
                    pendenciaParaAdicionar.ExibirAprovacao = true;

                if (!String.IsNullOrEmpty(item.CriadorRf) && item.CriadorRf != "0")
                    pendenciaParaAdicionar.NomeUsuario = item.Criador + $" ({item.CriadorRf})";
                else
                    pendenciaParaAdicionar.NomeUsuario = item.Criador;

                if (!String.IsNullOrEmpty(item.AprovadorRf) && item.AprovadorRf != "0")
                    pendenciaParaAdicionar.NomeUsuarioAprovacao = item.Aprovador + $" ({item.AprovadorRf})";
                else
                    pendenciaParaAdicionar.NomeUsuarioAprovacao = item.Aprovador;

                pendenciaParaAdicionar.Situacao = ((SituacaoPendencia)item.Situacao).ToString();

                listaOutrasPendencias.Add(pendenciaParaAdicionar);
            }

            return listaOutrasPendencias;
        }
    }
}
