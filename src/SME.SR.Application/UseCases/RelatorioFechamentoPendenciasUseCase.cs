using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFechamentoPendenciasUseCase : IRelatorioFechamentoPendenciasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFechamentoPendenciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioPendenciasFechamentoDto>();

            var resultado = await mediator.Send(new ObterRelatorioFechamentoPedenciasQuery() { filtroRelatorioPendenciasFechamentoDto = filtros });
            
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFechamentoPendencias", resultado, request.CodigoCorrelacao));

        }

        private static RelatorioFechamentoPendenciasDto GeraVariasPendencias2Componentes2Turmas()
        {
            var model = new RelatorioFechamentoPendenciasDto();

            model.DreNome = "DRE 001";
            model.UeNome = "UE 001";
            model.TurmaNome = "";
            model.Ano = "1987";
            model.Bimestre = "";
            model.ComponenteCurricular = "";
            model.Usuario = "ADMIN";
            model.Modalidade = "Fundamental";
            model.Semestre = "1";
            //model.Modalidade = "Fundamental";
            model.RF = "123123123";
            model.ExibeDetalhamento = true;
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.Dre = new RelatorioFechamentoPendenciasDreDto
            {
                Codigo = "123",
                Nome = "DRE 01",
                Ue = new RelatorioFechamentoPendenciasUeDto
                {
                    Nome = "UE 01",
                    Codigo = "456",
                    Turmas = new List<RelatorioFechamentoPendenciasTurmaDto>() {
                         new RelatorioFechamentoPendenciasTurmaDto() {
                          Nome = "TURMA 01",
                          Bimestres =  new List<RelatorioFechamentoPendenciasBimestreDto>
                                {
                                    new RelatorioFechamentoPendenciasBimestreDto
                                    {
                                         Nome="1º BIMESTRE",
                                         Componentes = new List<RelatorioFechamentoPendenciasComponenteDto>
                                         {
                                               new RelatorioFechamentoPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "001",
                                                     NomeComponente = "Matemática",
                                                      Pendencias = new List<RelatorioFechamentoPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           }


                                                      }
                                               },
                                               new RelatorioFechamentoPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "002",
                                                     NomeComponente = "Ciências",
                                                      Pendencias = new List<RelatorioFechamentoPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendenciadetalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           }


                                                      }
                                               }

                                         }
                                    }
                        }
                         },
                         new RelatorioFechamentoPendenciasTurmaDto() {
                          Nome = "TURMA 02",
                          Bimestres =  new List<RelatorioFechamentoPendenciasBimestreDto>
                                {
                                    new RelatorioFechamentoPendenciasBimestreDto
                                    {
                                         Nome="1º BIMESTRE",
                                         Componentes = new List<RelatorioFechamentoPendenciasComponenteDto>
                                         {
                                               new RelatorioFechamentoPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "001",
                                                     NomeComponente = "Matemática",
                                                      Pendencias = new List<RelatorioFechamentoPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           }


                                                      }
                                               },
                                               new RelatorioFechamentoPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "002",
                                                     NomeComponente = "Ciências",
                                                      Pendencias = new List<RelatorioFechamentoPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioFechamentoPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           }


                                                      }
                                               }

                                         }
                                    }
                        }
                         }
                        },


                }
            };
            return model;
        }

    }
}
