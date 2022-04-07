using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using SME.SR.Infra.RelatorioPaginado.Preparador;
using SME.SR.Infra.Utilitarios;
using SME.SR.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.MVC.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly ILogger<RelatoriosController> _logger;
        private readonly IMediator mediator;
        private Random rnd = new Random();

        public RelatoriosController(ILogger<RelatoriosController> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("graficos")]
        public async Task<IActionResult> RelatorioGraficosTeste([FromServices] IMediator mediator)
        {


            //await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("", new GraficoBarrasVerticalDto(10), Guid.NewGuid(), envioPorRabbit : false));

            var grafico = new GraficoBarrasVerticalDto(500, "");

            grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(10, "Banana"));
            grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(20, "Laranja"));

            grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(500, "Frutas", 100, 5);


            return View("RelatorioGraficoBarrasTeste", grafico);
        }
        [HttpGet("frequencia")]
        public async Task<IActionResult> RelatorioFaltasFrequencias([FromServices] IMediator mediator)
        {
            var model = new RelatorioFrequenciaDto();

            var alunos = new List<RelatorioFrequenciaAlunoDto>();
            for (var i = 0; i < 2; i++)
            {
                var aluno = new RelatorioFrequenciaAlunoDto()
                {
                    NumeroChamada = (i + 1).ToString(),
                    CodigoAluno = 001,
                    NomeAluno = "Marcos Almeida Machado" + i,
                    NomeTurma = "Turma 001",
                    CodigoTurma = "001",
                    TotalAusencias = 1,
                    TotalRemoto = 10,
                    TotalCompensacoes = 1,
                    TotalPresenca = 19,
                    TotalAulas = 20,
                };

                var aluno1 = new RelatorioFrequenciaAlunoDto()
                {
                    NumeroChamada = (i + 2).ToString(),
                    CodigoAluno = 002,
                    NomeAluno = "Antonio Castro Santana",
                    NomeTurma = "Turma 001",
                    CodigoTurma = "001",
                    TotalAusencias = 3,
                    TotalRemoto = 10,
                    TotalCompensacoes = 2,
                    TotalPresenca = 17,
                    TotalAulas = 20,
                };
                alunos.Add(aluno);
                alunos.Add(aluno1);
            }

            var componentes = new List<RelatorioFrequenciaComponenteDto>();
            for (var i = 0; i < 2; i++)
            {
                var componente = new RelatorioFrequenciaComponenteDto()
                {
                    NomeComponente = "Arte",
                    CodigoComponente = "001",
                    Alunos = alunos,
                };
                var componente1 = new RelatorioFrequenciaComponenteDto()
                {
                    NomeComponente = "Matematica",
                    CodigoComponente = "002",
                    Alunos = alunos,
                };

                componentes.Add(componente);
                componentes.Add(componente1);
            }

            var bimestres = new List<RelatorioFrequenciaBimestreDto>();
            for (var i = 0; i < 2; i++)
            {
                var bimestre = new RelatorioFrequenciaBimestreDto()
                {
                    NomeBimestre = "1º BIMESTRE",
                    Numero = "1",
                    Componentes = componentes,
                };
                var bimestre1 = new RelatorioFrequenciaBimestreDto()
                {
                    NomeBimestre = "2º BIMESTRE",
                    Numero = "1",
                    Componentes = componentes
                };
                bimestres.Add(bimestre);
                bimestres.Add(bimestre1);
            }

            var turmaAnos = new List<RelatorioFrequenciaTurmaAnoDto>();
            for (var i = 0; i < 2; i++)
            {
                var turmaAno = new RelatorioFrequenciaTurmaAnoDto()
                {
                    Nome = "EF-1A-1ºAno",
                    Bimestres = bimestres,
                    EhExibirTurma = false,
                };
                var turmaAno1 = new RelatorioFrequenciaTurmaAnoDto()
                {
                    Nome = "EF-2A-2ºAno",
                    Bimestres = bimestres,
                    EhExibirTurma = false,
                };
                turmaAnos.Add(turmaAno);
                turmaAnos.Add(turmaAno1);
            }

            var ues = new List<RelatorioFrequenciaUeDto>();
            for (var i = 0; i < 2; i++)
            {
                var ue = new RelatorioFrequenciaUeDto()
                {
                    CodigoUe = "1",
                    NomeUe = "CEU EMEF BUTANTA",
                    TipoUe = TipoEscola.CEMEI,
                    TurmasAnos = turmaAnos
                };
                var ue1 = new RelatorioFrequenciaUeDto()
                {
                    CodigoUe = "1",
                    NomeUe = "CEU EMEI BUTANTA",
                    TipoUe = TipoEscola.CEMEI,
                    TurmasAnos = turmaAnos
                };
                ues.Add(ue);
                ues.Add(ue1);
            }

            var dres = new List<RelatorioFrequenciaDreDto>();
            for (var i = 0; i < 2; i++)
            {
                var dre = new RelatorioFrequenciaDreDto()
                {
                    CodigoDre = "1",
                    NomeDre = "DRE-BT",
                    Ues = ues
                };
                var dre1 = new RelatorioFrequenciaDreDto()
                {
                    CodigoDre = "1",
                    NomeDre = "DRE-JT",
                    Ues = ues
                };
                dres.Add(dre);
                dres.Add(dre1);
            }

            model.Dres = dres;
            model.Cabecalho = new RelatorioFrequenciaCabecalhoDto()
            {
                Dre = "TODAS",
                Ue = "TODAS",
                Ano = "TODOS",
                Bimestre = "TODOS",
                ComponenteCurricular = "TODOS",
                Usuario = "JULIA FERREIRA DE OLIVEIRA ",
                Turma = "TODOS",
                RF = "1234567",
            };
            return View("RelatorioFrequencias", model);
        }

        [HttpGet("fechamentos-pendencias")]
        public IActionResult RelatorioFechamentoPendencia([FromServices] IMediator mediator)
        {
            RelatorioPendenciasDto model = GeraVariasPendencias2Componentes2Turmas();

            return View("RelatorioFechamentoPendencias", model);
        }
        [HttpGet("pareceres-conclusivos")]
        public async Task<IActionResult> RelatorioParecesConclusivos([FromServices] IMediator mediator)
        {
            var model = await mediator.Send(new ObterRelatorioParecerConclusivoQuery()
            {
                filtroRelatorioParecerConclusivoDto = new FiltroRelatorioParecerConclusivoDto()
                {
                    UsuarioNome = "Ronaldo Avante",
                    DreCodigo = "108900",
                    UeCodigo = "094099",
                    Anos = new string[] { "8", "9" }
                },
                UsuarioRf = "123"
            });

            var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            return View("RelatorioParecerConclusivo", model);
        }

        [HttpGet("recuperacao-paralela")]
        public IActionResult RelatorioRecuperacaoParalela([FromServices] IMediator mediator)
        {
            RelatorioRecuperacaoParalelaDto model = new RelatorioRecuperacaoParalelaDto("Todas", "Todas", "ALANA FERREIRA DE OLIVEIRA", "1234567", DateTime.Today.ToString("dd/MM/yyyy"), 2020, 1);

            var aluno = new RelatorioRecuperacaoParalelaAlunoDto("JOSÉ AUGUSTO CLEMENTE", "RF", "16/03/1985", "4241513", "RF", "MATRICULADO EM 04/02/2019");

            var secaoH = new RelatorioRecuperacaoParalelaAlunoSecaoDto("Histórico do Estudante", "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.");
            var secaoD = new RelatorioRecuperacaoParalelaAlunoSecaoDto("Dificuldades", "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.");
            var secaoE = new RelatorioRecuperacaoParalelaAlunoSecaoDto("Encaminhamentos", "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.");
            var secaoA = new RelatorioRecuperacaoParalelaAlunoSecaoDto("Avanços", "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.");
            var secaoO = new RelatorioRecuperacaoParalelaAlunoSecaoDto("Outros", "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.");

            aluno.Secoes.Add(secaoH);
            aluno.Secoes.Add(secaoD);
            aluno.Secoes.Add(secaoE);
            aluno.Secoes.Add(secaoA);
            aluno.Secoes.Add(secaoO);

            model.Alunos.Add(aluno);

            return View("RelatorioRecuperacaoParalela", model);
        }

        private static RelatorioPendenciasDto GeraVariasPendencias2Componentes2Turmas()
        {
            var model = new RelatorioPendenciasDto();

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
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.Dre = new RelatorioPendenciasDreDto
            {
                Codigo = "123",
                Nome = "DRE 01",
                Ue = new RelatorioPendenciasUeDto
                {
                    Nome = "UE 01",
                    Codigo = "456",
                    Turmas = new List<RelatorioPendenciasTurmaDto>() {
                         new RelatorioPendenciasTurmaDto() {
                          Nome = "TURMA 01",
                          Bimestres =  new List<RelatorioPendenciasBimestreDto>
                                {
                                    new RelatorioPendenciasBimestreDto
                                    {
                                         NomeBimestre="1º BIMESTRE",
                                         Componentes = new List<RelatorioPendenciasComponenteDto>
                                         {
                                               new RelatorioPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "001",
                                                     NomeComponente = "Matemática",
                                                      Pendencias = new List<RelatorioPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
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
                                               new RelatorioPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "002",
                                                     NomeComponente = "Ciências",
                                                      Pendencias = new List<RelatorioPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
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
                         },
                         new RelatorioPendenciasTurmaDto() {
                          Nome = "TURMA 02",
                          Bimestres =  new List<RelatorioPendenciasBimestreDto>
                                {
                                    new RelatorioPendenciasBimestreDto
                                    {
                                         NomeBimestre="1º BIMESTRE",
                                         Componentes = new List<RelatorioPendenciasComponenteDto>
                                         {
                                               new RelatorioPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "001",
                                                     NomeComponente = "Matemática",
                                                      Pendencias = new List<RelatorioPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
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
                                               new RelatorioPendenciasComponenteDto()
                                               {
                                                    CodigoComponente = "002",
                                                     NomeComponente = "Ciências",
                                                      Pendencias = new List<RelatorioPendenciasPendenciaDto>
                                                      {
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
                                                            CodigoUsuarioAprovacaoRf  = "teste",
                                                            CodigoUsuarioRf = "123",
                                                            DescricaoPendencia = "descrição da pendencia",
                                                            DetalhamentoPendencia = "detalhamento da pendencia",
                                                            NomeUsuario = "nome do usuário",
                                                            NomeUsuarioAprovacao = "nome usuário aprovação",
                                                            Situacao = "situação do aluno"
                                                           },
                                                           new RelatorioPendenciasPendenciaDto() {
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

        [HttpGet("notas-conceitos-finais")]
        public IActionResult RelatorioNotasConceitosFinais([FromServices] IMediator mediator)
        {
            RelatorioNotasEConceitosFinaisDto model = new RelatorioNotasEConceitosFinaisDto();
            model.DreNome = "Todas";
            model.UeNome = "Todas";
            model.Ano = "Todos";
            model.ComponenteCurricular = "Todos";
            model.Bimestre = "Todos";
            model.UsuarioNome = "Ronaldo Alves";
            model.UsuarioRF = "1234564";

            var dreDto = new RelatorioNotasEConceitosFinaisDreDto() { Codigo = "10001", Nome = "Dre de Teste" };

            var ueDto = new RelatorioNotasEConceitosFinaisUeDto() { Codigo = "321", Nome = "Ue de Teste" };

            var ano = new RelatorioNotasEConceitosFinaisAnoDto("1");

            var bimestre = new RelatorioNotasEConceitosFinaisBimestreDto("Bimestre 1");

            var componenteCurricular = new RelatorioNotasEConceitosFinaisComponenteCurricularDto() { Nome = "Matemática" };

            var notaConceitoAluno = new RelatorioNotasEConceitosFinaisDoAlunoDto("Turma ABC", 1, "Antolino Neves", "10", "");

            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            componenteCurricular.NotaConceitoAlunos.Add(notaConceitoAluno);
            bimestre.ComponentesCurriculares.Add(componenteCurricular);

            ano.Bimestres.Add(bimestre);
            ueDto.Anos.Add(ano);
            ueDto.Anos.Add(ano);
            dreDto.Ues.Add(ueDto);
            model.Dres.Add(dreDto);
            model.Dres.Add(dreDto);
            model.Dres.Add(dreDto);


            return View("RelatorioNotasEConceitosFinais", model);
        }

        [HttpGet("compensacao-ausencia")]
        public IActionResult RelatorioCompensacaoAusencia([FromServices] IMediator mediator)
        {
            RelatorioCompensacaoAusenciaDto model = GeraCompensacoesAusencia();

            return View("RelatorioCompensacaoAusencia", model);
        }

        [HttpGet("impressao-calendario")]
        public IActionResult RelatorioImpressaoCalendario()
        {
            RelatorioImpressaoCalendarioDto model = new RelatorioImpressaoCalendarioDto();
            model.DreNome = "DRE - JT";
            model.UeNome = "EMEFM DERVILLE ALEGRETTI, PROF.";
            model.TipoCalendarioNome = "CALENDÁRIO ESCOLAR FUNDAMENTAL/MÉDIO 2020";

            var listaMeses = new List<RelatorioImpressaoCalendarioMesDto>();
            var mes1 = new RelatorioImpressaoCalendarioMesDto()
            {
                MesDescricao = "JANEIRO",
                MesNumero = 1
            };

            var mes2 = new RelatorioImpressaoCalendarioMesDto()
            {
                MesDescricao = "FEVEREIRO",
                MesNumero = 2
            };

            var listaEventos = new List<RelatorioImpressaoCalendarioEventoDto>();
            var evento1 = new RelatorioImpressaoCalendarioEventoDto()
            {
                Dia = "01",
                DiaSemana = "Seg",
                EventoTipo = "SME",
                Evento = "Inicio das Férias Escolares"
            };

            var evento2 = new RelatorioImpressaoCalendarioEventoDto()
            {
                Dia = "15",
                DiaSemana = "Qua",
                EventoTipo = "SME",
                Evento = "Feriado"
            };

            listaEventos.Add(evento1);
            listaEventos.Add(evento2);
            listaEventos.Add(evento1);
            listaEventos.Add(evento2);
            listaEventos.Add(evento1);
            mes1.Eventos = listaEventos;
            mes2.Eventos = listaEventos;

            listaMeses.Add(mes1);
            listaMeses.Add(mes2);
            listaMeses.Add(mes1);
            listaMeses.Add(mes2);
            listaMeses.Add(mes1);
            listaMeses.Add(mes2);
            listaMeses.Add(mes1);
            listaMeses.Add(mes2);
            listaMeses.Add(mes1);
            listaMeses.Add(mes2);
            model.Meses = listaMeses;

            return View("RelatorioImpressaoCalendario", model);
        }

        [HttpGet("resumos-pap")]
        public IActionResult RelatorioResumosPAP()
        {
            ResumoPAPDto model = new ResumoPAPDto();
            model.DreNome = "DRE - JT";
            model.UeNome = "EMEFM DERVILLE ALEGRETTI, PROF.";
            model.AnoLetivo = 2020;
            model.Ciclo = "INTERDISCIPLINAR";
            model.Ano = "3";
            model.Turma = "5A";
            model.Periodo = "ACOMPANHAMENTO 1º SEMESTRE";
            model.UsuarioNome = "TESTE USUÁRIO";
            model.UsuarioRF = "123456789";
            model.Data = "21/10/2020";
            model.EhEncaminhamento = false;

            ResumoPAPTotalEstudantesDto totalEstudantes = new ResumoPAPTotalEstudantesDto();
            totalEstudantes.PorcentagemTotal = 100;
            totalEstudantes.QuantidadeTotal = 90;

            var anosTotalEstudantes = new List<ResumoPAPTotalAnoDto>();

            for (var i = 0; i < 7; i++)
            {
                anosTotalEstudantes.Add(new ResumoPAPTotalAnoDto
                {
                    AnoDescricao = (i + 3).ToString(),
                    Quantidade = i + 8,
                    Porcentagem = i + 33.3,
                });
            }

            totalEstudantes.Anos = anosTotalEstudantes;
            model.TotalEstudantesDto = totalEstudantes;


            var listaFrequencia = new List<ResumoPAPTotalEstudanteFrequenciaDto>();

            for (var i = 0; i < 4; i++)
            {
                var listaAno = new List<ResumoPAPTotalFrequenciaAnoDto>();
                var listaLinhas = new List<ResumoPAPFrequenciaDto>();

                for (var j = 0; j < 7; j++)
                {
                    listaAno.Add(new ResumoPAPTotalFrequenciaAnoDto()
                    {
                        DescricaoAno = (j + 3).ToString() + '°',
                        Porcentagem = j + i + 7.3,
                        Quantidade = j + i + 10,
                        TotalQuantidade = i + 12,
                        TotalPorcentagem = i + 13,
                    });

                }

                listaLinhas.Add(new ResumoPAPFrequenciaDto()
                {
                    QuantidadeTotalFrequencia = i + 10 + listaAno.Count,
                    PorcentagemTotalFrequencia = i + 11 + listaAno.Count,
                    Anos = listaAno
                });

                var desc = "";
                switch (i)
                {
                    case 0:
                        desc = "Frequente";
                        break;
                    case 1:
                        desc = " Pouco frequente";
                        break;
                    case 2:
                        desc = "Não comparece";
                        break;
                    default:
                        desc = "Total";
                        break;
                }

                listaFrequencia.Add(new ResumoPAPTotalEstudanteFrequenciaDto()
                {
                    PorcentagemTotalFrequencia = 0,
                    QuantidadeTotalFrequencia = 0,
                    FrequenciaDescricao = desc,
                    Linhas = listaLinhas
                });
            }

            //   model.FrequenciaDto = listaFrequencia;

            ResumoPAPTotalResultadoDto resultados = new ResumoPAPTotalResultadoDto()
            {
                EixoDescricao = "SONDAGEM"

            };

            var objetivosResultados = new ResumoPAPResultadoObjetivoDto()
            {
                ObjetivoDescricao = "Hipotese de escrita",
            };


            var listaTotalResultados = new List<ResumoPAPResultadoRespostaDto>();
            var listaRespostasResultados = new List<ResumoPAPResultadoRespostaDto>();
            var listaAnosResultados = new List<ResumoPAPResultadoAnoDto>();

            //anos
            for (var i = 0; i < 3; i++)
            {
                //respostas
                for (var j = 0; j < 2; j++)
                {
                    var desc = j % 2 == 0 ? "Pré silábico" : "Silabico";
                    listaRespostasResultados.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 11 + i + j,
                        Quantidade = 10 + i + j,
                        RespostaDescricao = desc,
                        TotalPorcentagem = 80 + i,
                        TotalQuantidade = 70 + i,
                    }
                    );
                }

                listaTotalResultados.Add(new ResumoPAPResultadoRespostaDto()
                {
                    Porcentagem = 41 + i,
                    Quantidade = 50 + i,
                    RespostaDescricao = null,
                    TotalPorcentagem = 100,
                    TotalQuantidade = 19
                });
                listaAnosResultados.Add(new ResumoPAPResultadoAnoDto()
                {
                    AnoDescricao = (4 + i).ToString(),
                    Respostas = listaRespostasResultados
                }
                );
            }


            var listaObjetivosResultados = new List<ResumoPAPResultadoObjetivoDto>();
            objetivosResultados.Anos = listaAnosResultados;
            objetivosResultados.Total = listaTotalResultados;
            listaObjetivosResultados.Add(objetivosResultados);
            resultados.Objetivos = listaObjetivosResultados;
            var listaResultados = new List<ResumoPAPTotalResultadoDto>();
            listaResultados.Add(resultados);


            ////////////////////////////
            ResumoPAPTotalResultadoDto resultados2 = new ResumoPAPTotalResultadoDto()
            {
                EixoDescricao = "ANALISA, INTERPRETA E SOLUCIONA PROBLEMAS ENVOLVENDO..."

            };

            var objetivosResultados2 = new ResumoPAPResultadoObjetivoDto()
            {
                ObjetivoDescricao = "Significados do campo aditivo composição e transformação",
            };

            var anosResultados2 = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = "3",
            };

            var anosResultados3 = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = "7",
            };

            var anosResultados4 = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = "8",
            };

            var totalResultados2 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 9,
                Quantidade = 20,
                RespostaDescricao = null,
                TotalPorcentagem = 100,
                TotalQuantidade = 19
            };

            var totalResultadosA1 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 9,
                Quantidade = 20,
                RespostaDescricao = null,
                TotalPorcentagem = 73,
                TotalQuantidade = 33
            };

            var respostaResultados2 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 20,
                Quantidade = 10,
                RespostaDescricao = "Realizou Plenamente",
                TotalPorcentagem = 100,
                TotalQuantidade = 19,
            };

            var respostaResultados3 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 10,
                Quantidade = 5,
                RespostaDescricao = "Realizou",
                TotalPorcentagem = 100,
                TotalQuantidade = 19,
            };



            var respostaResultadosA1 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 16,
                Quantidade = 15,
                RespostaDescricao = "Realizou Plenamente",
                TotalPorcentagem = 100,
                TotalQuantidade = 19,
            };

            var respostaResultadosA2 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 18,
                Quantidade = 17,
                RespostaDescricao = "Realizou",
                TotalPorcentagem = 100,
                TotalQuantidade = 19,
            };

            var listaRespostasResultados2 = new List<ResumoPAPResultadoRespostaDto>();
            var listaRespostasResultados3 = new List<ResumoPAPResultadoRespostaDto>();
            var listaRespostasResultadosA1 = new List<ResumoPAPResultadoRespostaDto>();
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados2);
            listaRespostasResultados3.Add(respostaResultados3);
            listaRespostasResultadosA1.Add(respostaResultadosA1);
            listaRespostasResultadosA1.Add(respostaResultadosA2);
            listaRespostasResultadosA1.Add(respostaResultadosA2);

            anosResultados2.Respostas = listaRespostasResultados2;
            anosResultados3.Respostas = listaRespostasResultadosA1;
            anosResultados4.Respostas = listaRespostasResultados3;

            var listaTotalResultados2 = new List<ResumoPAPResultadoRespostaDto>();
            listaTotalResultados2.Add(totalResultados2);

            var listaTotalResultadosA1 = new List<ResumoPAPResultadoRespostaDto>();
            listaTotalResultadosA1.Add(totalResultadosA1);

            var listaAnosResultados2 = new List<ResumoPAPResultadoAnoDto>();
            listaAnosResultados2.Add(anosResultados2);
            listaAnosResultados2.Add(anosResultados3);
            listaAnosResultados2.Add(anosResultados4);

            var listaObjetivosResultados2 = new List<ResumoPAPResultadoObjetivoDto>();
            objetivosResultados2.Anos = listaAnosResultados2;
            objetivosResultados2.Total = listaTotalResultadosA1;
            listaObjetivosResultados2.Add(objetivosResultados2);
            resultados2.Objetivos = listaObjetivosResultados2;

            listaResultados.Add(resultados2);


            model.ResultadoDto = listaResultados;


            var listaEncaminhamento = new List<ResumoPAPTotalResultadoDto>();
            var listaObjetivos = new List<ResumoPAPResultadoObjetivoDto>();

            for (var i = 0; i < 3; i++)
            {
                var listaAnosEnca = new List<ResumoPAPResultadoAnoDto>();
                var listaTotal = new List<ResumoPAPResultadoRespostaDto>();
                var listaRepostas = new List<ResumoPAPResultadoRespostaDto>();

                listaTotal.Add(new ResumoPAPResultadoRespostaDto()
                {
                    Porcentagem = 0,
                    Quantidade = 0,
                    RespostaDescricao = null,
                    TotalQuantidade = 30 + i,
                    TotalPorcentagem = 31 + i
                });


                if (i == 2)
                {
                    listaRepostas.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 11 + i,
                        Quantidade = 10 + i,
                        RespostaDescricao = "Aprovado",
                        TotalQuantidade = 0,
                        TotalPorcentagem = 0
                    });

                    listaRepostas.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 21 + i,
                        Quantidade = 20 + i,
                        RespostaDescricao = "Aprovado pelo conselho",
                        TotalQuantidade = 0,
                        TotalPorcentagem = 0
                    });

                    listaRepostas.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 31 + i,
                        Quantidade = 30 + i,
                        RespostaDescricao = "Retido",
                        TotalQuantidade = 0,
                        TotalPorcentagem = 0
                    });
                }
                else
                {
                    listaRepostas.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 11 + i,
                        Quantidade = 10 + i,
                        RespostaDescricao = "Sim",
                        TotalQuantidade = 0,
                        TotalPorcentagem = 0
                    });

                    listaRepostas.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 21 + i,
                        Quantidade = 20 + i,
                        RespostaDescricao = "Nao",
                        TotalQuantidade = 0,
                        TotalPorcentagem = 0
                    });

                }

                for (var j = 0; j < 2; j++)
                {
                    listaAnosEnca.Add(new ResumoPAPResultadoAnoDto()
                    {
                        AnoDescricao = (4 + j).ToString(),
                        Respostas = listaRepostas
                    });
                }
                //if(i == 0)
                //{

                //    listaRepostas1.Add(new ResumoPAPResultadoRespostaDto()
                //    {
                //        Porcentagem = 71,
                //        Quantidade = 70,
                //        RespostaDescricao = "Nao",
                //        TotalQuantidade = 0,
                //        TotalPorcentagem = 0
                //    });



                //    listaAnosEnca.Add(new ResumoPAPResultadoAnoDto()
                //    {
                //        AnoDescricao = 6,
                //        Respostas = listaRepostas1
                //    });
                //}


                var obj = "";
                switch (i)
                {
                    case 0:
                        obj = "É atendido pelo AEE?";
                        break;
                    case 1:
                        obj = "É atendido pelo NAAPA?";
                        break;
                    default:
                        obj = "Parecer conclusivo do ano anterior";
                        break;
                }

                listaObjetivos.Add(new ResumoPAPResultadoObjetivoDto()
                {
                    ObjetivoDescricao = obj,
                    Anos = listaAnosEnca,
                    Total = listaTotal
                });

            }

            listaEncaminhamento.Add(new ResumoPAPTotalResultadoDto()
            {
                EixoDescricao = "Informações escolares",
                Objetivos = listaObjetivos

            });

            model.EncaminhamentoDto = listaEncaminhamento;

            model.ResultadoDto = listaResultados;
            return View("RelatorioResumosPAP", model);
        }


        private static RelatorioCompensacaoAusenciaDto GeraCompensacoesAusencia()
        {
            var model = new RelatorioCompensacaoAusenciaDto();

            var compensacaoAlunoExemplo = new RelatorioCompensacaoAusenciaCompensacaoAlunoDto()
            {
                NomeAluno = "Aline Leal",
                NumeroChamada = "01",
                TotalAulas = 10,
                TotalAusencias = 3,
                TotalCompensacoes = 1
            };
            var compensacaoAlunoExemplo2 = new RelatorioCompensacaoAusenciaCompensacaoAlunoDto()
            {
                NomeAluno = "Teste",
                NumeroChamada = "01",
                TotalAulas = 10,
                TotalAusencias = 3,
                TotalCompensacoes = 1
            };

            model.DreNome = "DRE 001";
            model.UeNome = "UE 001";
            model.TurmaNome = "";
            model.Bimestre = "";
            model.ComponenteCurricular = "";
            model.Usuario = "ADMIN";
            model.Modalidade = "Fundamental";
            model.RF = "123123123";
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.Dre = new RelatorioCompensacaoAusenciaDreDto
            {
                Codigo = "123",
                Nome = "DRE 01",
                Ue = new RelatorioCompensacaoAusenciaUeDto
                {
                    Nome = "UE 01",
                    Codigo = "456",
                    Turmas = new List<RelatorioCompensacaoAusenciaTurmaDto>()
                    {
                        new RelatorioCompensacaoAusenciaTurmaDto()
                        {
                            Nome = "TURMA 01",
                            Bimestres = new List<RelatorioCompensacaoAusenciaBimestreDto>
                            {
                                new RelatorioCompensacaoAusenciaBimestreDto
                                {
                                    Nome = "1º BIMESTRE",
                                    Componentes = new List<RelatorioCompensacaoAusenciaComponenteDto>
                                    {
                                        new RelatorioCompensacaoAusenciaComponenteDto()
                                        {
                                            CodigoComponente = "001",
                                            NomeComponente = "Matemática",
                                            Atividades = new List<RelatorioCompensacaoAusenciaAtividadeDto>
                                            {
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 01",
                                                    CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                },
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 02",CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo2,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo2,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo2
                                                    }
                                                }
                                            }
                                        },
                                        new RelatorioCompensacaoAusenciaComponenteDto()
                                        {
                                            CodigoComponente = "002",
                                            NomeComponente = "Ciências",
                                            Atividades = new List<RelatorioCompensacaoAusenciaAtividadeDto>
                                            {
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 01",
                                                    CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                },
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 02",CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        },
                        new RelatorioCompensacaoAusenciaTurmaDto()
                        {
                            Nome = "TURMA 02",
                            Bimestres = new List<RelatorioCompensacaoAusenciaBimestreDto>
                            {
                                new RelatorioCompensacaoAusenciaBimestreDto
                                {
                                    Nome = "1º BIMESTRE",
                                    Componentes = new List<RelatorioCompensacaoAusenciaComponenteDto>
                                    {
                                        new RelatorioCompensacaoAusenciaComponenteDto()
                                        {
                                            CodigoComponente = "001",
                                            NomeComponente = "Matemática",
                                            Atividades = new List<RelatorioCompensacaoAusenciaAtividadeDto>
                                            {
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 01",
                                                    CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                },
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 02",CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                }
                                            }
                                        },
                                        new RelatorioCompensacaoAusenciaComponenteDto()
                                        {
                                            CodigoComponente = "002",
                                            NomeComponente = "Ciências",
                                            Atividades = new List<RelatorioCompensacaoAusenciaAtividadeDto>
                                            {
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 01",
                                                    CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                },
                                                new RelatorioCompensacaoAusenciaAtividadeDto()
                                                {
                                                    Nome ="Atividade 02",CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>(){
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo,
                                                        compensacaoAlunoExemplo
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            };
            return model;

        }

        [HttpGet("plano-aula")]
        public IActionResult RelatorioPlanoAula()

        {

            var model = new PlanoAulaDto()
            {
                DataPlanoAula = DateTime.Now,
                Id = 1,
                Descricao = @"Bloco 1The standard Lorem Ipsum passage, used since the 1500s Lorem
                             ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod 
                            tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim 
                            veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea 
                            commodo consequat. Duis aute irure dolor in reprehenderit in voluptate 
                            velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint 
                            occaecat cupidatat non proident, sunt in culpa qui officia deserunt 
                            mollit anim id est laborum. Section 1.10.32 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC Sed
                             ut perspiciatis unde omnis iste natus error sit voluptatem accusantium 
                            doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo 
                            inventore veritatis et quasi architecto beatae vitae dicta sunt 
                            explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut 
                            odit aut fugit, sed quia consequuntur magni dolores eos qui ratione 
                            voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum 
                            quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam 
                            eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat 
                            voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam 
                            corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?
                             Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse 
                            quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo 
                            voluptas nulla pariatur?
                            1914 translation by H. Rackham
                            But I must explain to you how all this mistaken idea of denouncing 
                            pleasure and praising pain was born and I will give you a complete 
                            account of the system, and expound the actual teachings of the great 
                            explorer of the truth, the master-builder of human happiness. No one 
                            rejects, dislikes, or avoids pleasure itself, because it is pleasure, 
                            but because those who do not know how to pursue pleasure rationally 
                            encounter consequences that are extremely painful. Nor again is there 
                            anyone who loves or pursues or desires to obtain pain of itself, because
                             it is pain, but because occasionally circumstances occur in which toil 
                            and pain can procure him some great pleasure. To take a trivial example,
                             which of us ever undertakes laborious physical exercise, except to 
                            obtain some advantage from it? But who has any right to find fault with a
                             man who chooses to enjoy a pleasure that has no annoying consequences, 
                            or one who avoids a pain that produces no resultant pleasure?
                            Section 1.10.33 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC
                            At vero eos et accusamus et iusto odio dignissimos ducimus qui 
                            blanditiis praesentium voluptatum deleniti atque corrupti quos dolores 
                            et quas molestias excepturi sint occaecati cupiditate non provident, 
                            similique sunt in culpa qui officia deserunt mollitia animi, id est 
                            laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita 
                            distinctio. Nam libero tempore, cum soluta nobis est eligendi optio 
                            cumque nihil impedit quo minus id quod maxime placeat facere possimus, 
                            omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem 
                            quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet
                             ut et voluptates repudiandae sint et molestiae non recusandae. Itaque 
                            earum rerum hic tenetur a sapiente delectus, ut aut reiciendis Bloco 2 -----------The standard Lorem Ipsum passage, used since the 1500s Lorem
                             ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod 
                            tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim 
                            veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea 
                            commodo consequat. Duis aute irure dolor in reprehenderit in voluptate 
                            velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint 
                            occaecat cupidatat non proident, sunt in culpa qui officia deserunt 
                            mollit anim id est laborum. Section 1.10.32 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC Sed
                             ut perspiciatis unde omnis iste natus error sit voluptatem accusantium 
                            doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo 
                            inventore veritatis et quasi architecto beatae vitae dicta sunt 
                            explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut 
                            odit aut fugit, sed quia consequuntur magni dolores eos qui ratione 
                            voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum 
                            quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam 
                            eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat 
                            voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam 
                            corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?
                             Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse 
                            quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo 
                            voluptas nulla pariatur?
                            1914 translation by H. Rackham
                            But I must explain to you how all this mistaken idea of denouncing 
                            pleasure and praising pain was born and I will give you a complete 
                            account of the system, and expound the actual teachings of the great 
                            explorer of the truth, the master-builder of human happiness. No one 
                            rejects, dislikes, or avoids pleasure itself, because it is pleasure, 
                            but because those who do not know how to pursue pleasure rationally 
                            encounter consequences that are extremely painful. Nor again is there 
                            anyone who loves or pursues or desires to obtain pain of itself, because
                             it is pain, but because occasionally circumstances occur in which toil 
                            and pain can procure him some great pleasure. To take a trivial example,
                             which of us ever undertakes laborious physical exercise, except to 
                            obtain some advantage from it? But who has any right to find fault with a
                             man who chooses to enjoy a pleasure that has no annoying consequences, 
                            or one who avoids a pain that produces no resultant pleasure?
                            Section 1.10.33 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC
                            At vero eos et accusamus et iusto odio dignissimos ducimus qui 
                            blanditiis praesentium voluptatum deleniti atque corrupti quos dolores 
                            et quas molestias excepturi sint occaecati cupiditate non provident, 
                            similique sunt in culpa qui officia deserunt mollitia animi, id est 
                            laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita 
                            distinctio. Nam libero tempore, cum soluta nobis est eligendi optio 
                            cumque nihil impedit quo minus id quod maxime placeat facere possimus, 
                            omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem 
                            quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet
                             ut et voluptates repudiandae sint et molestiae non recusandae. Itaque 
                            earum rerum hic tenetur a sapiente delectus, ut aut reiciendis 
                            voluptatibus maiores alias consequatur aut perferendis doloribus 
                            asperiores repellat. Bloco 3 ----The standard Lorem Ipsum passage, used since the 1500s Lorem
                             ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod 
                            tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim 
                            veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea 
                            commodo consequat. Duis aute irure dolor in reprehenderit in voluptate 
                            velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint 
                            occaecat cupidatat non proident, sunt in culpa qui officia deserunt 
                            mollit anim id est laborum. Section 1.10.32 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC Sed
                             ut perspiciatis unde omnis iste natus error sit voluptatem accusantium 
                            doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo 
                            inventore veritatis et quasi architecto beatae vitae dicta sunt 
                            explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut 
                            odit aut fugit, sed quia consequuntur magni dolores eos qui ratione 
                            voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum 
                            quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam 
                            eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat 
                            voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam 
                            corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?
                             Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse 
                            quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo 
                            voluptas nulla pariatur?
                            1914 translation by H. Rackham
                            But I must explain to you how all this mistaken idea of denouncing 
                            pleasure and praising pain was born and I will give you a complete 
                            account of the system, and expound the actual teachings of the great 
                            explorer of the truth, the master-builder of human happiness. No one 
                            rejects, dislikes, or avoids pleasure itself, because it is pleasure, 
                            but because those who do not know how to pursue pleasure rationally 
                            encounter consequences that are extremely painful. Nor again is there 
                            anyone who loves or pursues or desires to obtain pain of itself, because
                             it is pain, but because occasionally circumstances occur in which toil 
                            and pain can procure him some great pleasure. To take a trivial example,
                             which of us ever undertakes laborious physical exercise, except to 
                            obtain some advantage from it? But who has any right to find fault with a
                             man who chooses to enjoy a pleasure that has no annoying consequences, 
                            or one who avoids a pain that produces no resultant pleasure?
                            Section 1.10.33 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC
                            At vero eos et accusamus et iusto odio dignissimos ducimus qui 
                            blanditiis praesentium voluptatum deleniti atque corrupti quos dolores 
                            et quas molestias excepturi sint occaecati cupiditate non provident, 
                            similique sunt in culpa qui officia deserunt mollitia animi, id est 
                            laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita 
                            distinctio. Nam libero tempore, cum soluta nobis est eligendi optio 
                            cumque nihil impedit quo minus id quod maxime placeat facere possimus, 
                            omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem 
                            quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet
                             ut et voluptates repudiandae sint et molestiae non recusandae. Itaque 
                            earum rerum hic tenetur a sapiente delectus, ut aut reiciendis 
                            voluptatibus maiores alias consequatur aut perferendis doloribus 
                            asperiores repellat. 
                            voluptatibus maiores alias consequatur aut perferendis doloribus 
                            asperiores repellat.",
                Recuperacao = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a purus consectetur ante tristique fringilla id
                ut purus.Pellentesque lobortis eu sem facilisis ullamcorper.Integer congue ante et nibh aliquet gravida.Donec
                accumsan nisi pulvinar dignissim molestie.Suspendisse a libero suscipit,
                                pharetra sem semper,
                                sagittis
                turpis.Sed vulputate vel lacus in commodo.Pellentesque nisi quam,
                                consectetur in eros ac,
                                semper porta velit.
                Donec sapien ante,
                commodo in neque eu,
                faucibus tincidunt erat.Duis a felis aliquet,
                vulputate lectus vitae,
                elementum metus.Sed massa nulla,
                pretium euismod massa eu,
                volutpat auctor mi.
                Mauris vestibulum dictum odio a auctor.Fusce ullamcorper,
                                nibh sed sollicitudin porta,
                                lectus velit gravida
                tellus,
                                vel pellentesque metus quam at magna.Sed laoreet metus massa,
                                et sollicitudin lacus elementum vitae.
                Vestibulum in quam tincidunt,
                vestibulum eros non,
                imperdiet justo.Aenean suscipit felis ipsum,
                sit amet
                vulputate metus sollicitudin non.Curabitur a dapibus nibh.Nullam non lorem a felis mattis bibendum.Vivamus
                sit amet posuere orci,
                                a sodales ipsum.Curabitur viverra euismod urna.
                Ut sed porttitor eros.Nullam eget convallis mi.Nam luctus erat a sem malesuada auctor.Aliquam nec pulvinar
                risus.Nullam tincidunt maximus lectus nec dignissim.Nunc porta dolor quis nisl imperdiet cursus.Aliquam
                convallis,
                dui a aliquam bibendum,
                nunc nisi commodo ipsum,
                quis vestibulum lacus risus non nisl.Quisque in
                sapien neque.Suspendisse potenti.
                Nullam id nisl vel ipsum ultrices rutrum.Curabitur consequat tempor nunc,
                a condimentum eros iaculis ac.
                Integer risus lorem,
                commodo non felis euismod,
                finibus ultrices libero.Duis posuere magna ante,
                id auctor
                turpis pulvinar molestie.Morbi mattis purus eget turpis imperdiet pulvinar.Quisque vehicula euismod justo quis
                ullamcorper.In hac habitasse platea dictumst.Pellentesque quis elementum dolor, in sagittis neque.
                Maecenas blandit tristique vestibulum.
                Aliquam rhoncus dui odio,
                id posuere ante ullamcorper in. Nam odio libero,
                pharetra vitae interdum non,
                fringilla ut sem.Sed aliquam urna tortor,
                eu congue justo semper in. Nullam enim nisl,
                laoreet quis arcu quis,
                semper dignissim tortor.Phasellus sit amet massa ullamcorper,
                iaculis diam vel,
                vulputate sem.Quisque quis
                massa ut risus congue maximus at vitae leo.Etiam scelerisque lectus a tempor efficitur",
                LicaoCasa = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a purus consectetur ante tristique fringilla id
                ut purus.Pellentesque lobortis eu sem facilisis ullamcorper.Integer congue ante et nibh aliquet gravida.Donec
                accumsan nisi pulvinar dignissim molestie.Suspendisse a libero suscipit,
                pharetra sem semper,
                sagittis
                turpis.Sed vulputate vel lacus in commodo.Pellentesque nisi quam,
                consectetur in eros ac,
                semper porta velit.
                Donec sapien ante,
                commodo in neque eu,
                faucibus tincidunt erat.Duis a felis aliquet,
                vulputate lectus vitae,
                elementum metus.Sed massa nulla,
                pretium euismod massa eu,
                volutpat auctor mi.
                Mauris vestibulum dictum odio a auctor.Fusce ullamcorper,
                nibh sed sollicitudin porta,
                lectus velit gravida
                tellus,
                vel pellentesque metus quam at magna.Sed laoreet metus massa,
                et sollicitudin lacus elementum vitae.
                Vestibulum in quam tincidunt,
                vestibulum eros non,
                imperdiet justo.Aenean suscipit felis ipsum,
                sit amet
                vulputate metus sollicitudin non.Curabitur a dapibus nibh.Nullam non lorem a felis mattis bibendum.Vivamus
                sit amet posuere orci,
                                a sodales ipsum.Curabitur viverra euismod urna.
                Ut sed porttitor eros.Nullam eget convallis mi.Nam luctus erat a sem malesuada auctor.Aliquam nec pulvinar
                risus.Nullam tincidunt maximus lectus nec dignissim.Nunc porta dolor quis nisl imperdiet cursus.Aliquam
                convallis,
                dui a aliquam bibendum,
                nunc nisi commodo ipsum,
                quis vestibulum lacus risus non nisl.Quisque in
                sapien neque.Suspendisse potenti.
                Nullam id nisl vel ipsum ultrices rutrum.Curabitur consequat tempor nunc,
                a condimentum eros iaculis ac.
                Integer risus lorem,
                commodo non felis euismod,
                finibus ultrices libero.Duis posuere magna ante,
                id auctor
                turpis pulvinar molestie.Morbi mattis purus eget turpis imperdiet pulvinar.Quisque vehicula euismod justo quis
                ullamcorper.In hac habitasse platea dictumst.Pellentesque quis elementum dolor, in sagittis neque.
                Maecenas blandit tristique vestibulum.
                Aliquam rhoncus dui odio,
                id posuere ante ullamcorper in. Nam odio libero,
                pharetra vitae interdum non,
                fringilla ut sem.Sed aliquam urna tortor,
                eu congue justo semper in. Nullam enim nisl,
                laoreet quis arcu quis,
                semper dignissim tortor.Phasellus sit amet massa ullamcorper,
                iaculis diam vel,
                vulputate sem.Quisque quis
                m'assa ut risus congue maximus at vitae leo.Etiam scelerisque lectus a tempor efficitur",
                Dre = "DRE - JT",
                Ue = "EMEF MAXIMO DE MOURA SANTOS, PROF. ",
                Turma = "TURMA:EF - 6A",
                ComponenteCurricular = "Língua Portuguesa",
                Usuario = "JORGE ELIAS DE ALMEIDA",
                RF = "8243719",
                Objetivos = new List<ObjetivoAprendizagemDto>() {
                    new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M01",
                        Descricao = "Explorar números no contexto diário como indicadores de quantidade, ordem, medida e código; ler e produzir escritas numéricas, identificando algumas regularidades do sistema de numeração decimal"
                    },
                        new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M02",
                        Descricao = "Compor e decompor números naturais de diversas maneiras"
                        },
                        new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M02",
                        Descricao = "Compor e decompor números naturais de diversas maneiras"
                        },
                        new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M02",
                        Descricao = "Compor e decompor números naturais de diversas maneiras"
                        },
                        new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M02",
                        Descricao = "Compor e decompor números naturais de diversas maneiras"
                        },
                        new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M02",
                        Descricao = "Compor e decompor números naturais de diversas maneiras"
                        },
                        new ObjetivoAprendizagemDto() {
                        Codigo = "EF02M02",
                        Descricao = "Compor e decompor números naturais de diversas maneiras"
                        }
                }
            };
            return View("RelatorioPlanoAula", model);
        }

        [HttpGet("sondagem-numeros")]
        public async Task<IActionResult> SondagemComponentesNumeros([FromServices] IMediator mediator)
        {
            var linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            for (var i = 0; i < 30; i++)
            {
                linhas.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                        {
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "",
                                PerguntaId = 1
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Escreve Convencionalmente",
                                PerguntaId = 2
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Escreve Convencionalmente",
                                PerguntaId = 3
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Escreve Convencionalmente",
                                PerguntaId = 4
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Escreve Convencionalmente",
                                PerguntaId = 5
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "",
                                PerguntaId = 6
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Escreve Convencionalmente",
                                PerguntaId = 7
                            },
                        }
                });
                linhas.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6195479,
                        Nome = "ALICE SILVA RIBEIRO",
                        SituacaoMatricula = SituacaoMatriculaAluno.Desistente.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                            {
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "Não Escreve Convencionalmente",
                                    PerguntaId = 1
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "",
                                    PerguntaId = 2
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "Escreve Convencionalmente",
                                    PerguntaId = 3
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "Não Escreve Convencionalmente",
                                    PerguntaId = 4
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "Não Escreve Convencionalmente",
                                    PerguntaId = 5
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "Não Escreve Convencionalmente",
                                    PerguntaId = 6
                                },
                                new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                    OrdemId = 0,
                                    Resposta = "",
                                    PerguntaId = 7
                                },
                            },
                });
                linhas.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "AMANDA ALBUQUERQUE",
                        SituacaoMatricula = SituacaoMatriculaAluno.NaoCompareceu.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                        {
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Não Escreve Convencionalmente",
                                PerguntaId = 1
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Não Escreve Convencionalmente",
                                PerguntaId = 2
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Não Escreve Convencionalmente",
                                PerguntaId = 3
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "",
                                PerguntaId = 4
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "",
                                PerguntaId = 5
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Não Escreve Convencionalmente",
                                PerguntaId = 6
                            },
                            new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                                OrdemId = 0,
                                Resposta = "Escreve Convencionalmente",
                                PerguntaId = 7
                            },
                    },
                });
            }

            var model = new RelatorioSondagemComponentesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
                {
                    Ano = 5.ToString(),
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/YYYY"),
                    Dre = "DRE - BT",
                    Periodo = "1º Semestre",
                    Proficiencia = "Números",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 0,
                            Descricao = "ORDEM 1 - COMPOSIÇÃO"
                        },
                    },
                    Perguntas = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 1,
                            Nome = "Familiares ou Frequentes"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 2,
                            Nome = "Opacos"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 3,
                            Nome = "Transparentes"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 4,
                            Nome = "Terminam em Zero"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 5,
                            Nome = "Algarismos Iguais"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 6,
                            Nome = "Processos de Generalização"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 7,
                            Nome = "Zeros Intercalados"
                        }
                    },
                },
                Planilha = new RelatorioSondagemComponentesPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };

            foreach (var pergunta in model.Cabecalho.Perguntas)
            {
                var grafico = new GraficoBarrasVerticalDto(400, pergunta.Nome);

                var respostas = model.Planilha.Linhas
                    .SelectMany(l => l.OrdensRespostas.Where(or => or.PerguntaId == pergunta.Id)).GroupBy(b => b.Resposta);

                foreach (var resposta in respostas)
                {
                    var qntRespostas = resposta.Count();
                    grafico.EixosX.Add(new GraficoBarrasVerticalEixoXDto(qntRespostas, string.IsNullOrEmpty(resposta.Key) ? "Não Respondeu" : resposta.Key));
                }

                var valorMaximoEixo = respostas.Max(a => a.Count());
                grafico.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", valorMaximoEixo, 6);

                model.GraficosBarras.Add(grafico);
            }

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("relatorios/RelatorioSondagemComponentesPorTurma", model, Guid.NewGuid(), envioPorRabbit: false));

            return View("RelatorioSondagemComponentesPorTurma", model);
        }

        [HttpGet("sondagem-componentes-aditivos")]
        public IActionResult SondagemComponentesAditivos()
        {
            var linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();
            for (var i = 0; i < 20; i++)
            {
                linhas.Add(
                new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA ALEXIA FERNANDES LIMA ALEXIA FERNANDES LIMA",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                    },
                });
                linhas.Add(
                new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 4650630,
                        Nome = "MATHEUS GUILHERME NASCIMENTO DA SILVA (RECLASSIFICADO SAÍDA EM 11/04/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Desistente.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                    },
                });
                linhas.Add(
                new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "AMANDA ALBUQUERQUE",
                        SituacaoMatricula = SituacaoMatriculaAluno.NaoCompareceu.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Errou",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Acertou",
                            PerguntaId = 2
                        },
                    },
                });

            }

            var model = new RelatorioSondagemComponentesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
                {
                    Ano = 5.ToString(),
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/YYYY"),
                    Dre = "DRE - BT",
                    Periodo = "1º Semestre",
                    Proficiencia = "Campo Aditivo",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 1,
                            Descricao = "ORDEM 1 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 2,
                            Descricao = "ORDEM 2 - COMPOSIÇÃO"
                        },
                        new RelatorioSondagemComponentesPorTurmaOrdemDto()
                        {
                            Id = 3,
                            Descricao = "ORDEM 3 - COMPOSIÇÃO"
                        },
                    },
                    Perguntas = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 1,
                            Nome = "Ideia"
                        },
                        new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                        {
                            Id = 2,
                            Nome = "Resultado"
                        }
                    },
                },
                Planilha = new RelatorioSondagemComponentesPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };
            model.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            var graficoBarras1 = new GraficoBarrasVerticalDto(600, "Teste - gráfico de matemática");

            graficoBarras1.Legendas = new List<GraficoBarrasLegendaDto>() {
                new GraficoBarrasLegendaDto()
                {
                    Chave="A",
                    Valor= "Não conseguiu ou não quis ler aaaa nmnnnn kkkk ssss"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="B",
                    Valor= "Leu com muita dificuldade"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="C",
                    Valor= "Leu com alguma fluencia"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="D",
                    Valor= "Leu com fluencia"
                },
                new GraficoBarrasLegendaDto()
                {
                    Chave="E",
                    Valor= "Sem preenchimento"
                },
            };

            graficoBarras1.EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", 24, 10);

            graficoBarras1.EixosX = new List<GraficoBarrasVerticalEixoXDto>()
            {
                new GraficoBarrasVerticalEixoXDto(2, "A"),
                new GraficoBarrasVerticalEixoXDto(2, "B"),
                new GraficoBarrasVerticalEixoXDto(2, "C"),
                new GraficoBarrasVerticalEixoXDto(1, "D"),
                new GraficoBarrasVerticalEixoXDto(24, "E"),
            };

            model.GraficosBarras.Add(graficoBarras1);

            return View("RelatorioSondagemComponentesPorTurma", model);
        }

        [HttpGet("sondagem-consolidado-matematica-aditivo")]
        public IActionResult RelatorioSondagemConsolidadoMatematicaNumeros()
        {

            var model = new RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto();
            model.Dre = "DRE-JT";
            model.Ue = "EMEF - Máximo de Moura";
            model.AnoLetivo = 2020;
            model.Ano = "9";
            model.Turma = "Todas";
            model.ComponenteCurricular = "Matemática";
            model.Proficiencia = "Números";
            model.Periodo = "1º semestre";
            model.Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque";
            model.RF = "7777710";
            model.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
            Random randNum = new Random();

            var perguntas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto>();
            perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto() { Descricao = "Ideia", Id = 1 });
            perguntas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto() { Descricao = "Resultado", Id = 2 });
            model.Perguntas = perguntas;


            model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
            {
                Ordem = "ORDEM 1 - COMPOSIÇÃO",
                Respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>() {
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 60, AlunosQuantidade = randNum.Next(99999), Resposta = "Acertou" , PerguntaId = 1 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 30, AlunosQuantidade = randNum.Next(99999), Resposta = "Errou", PerguntaId = 1 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Não resolveu", PerguntaId = 1 },
                        new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 60, AlunosQuantidade = randNum.Next(99999), Resposta = "Acertou" , PerguntaId = 2 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 30, AlunosQuantidade = randNum.Next(99999), Resposta = "Errou", PerguntaId = 2 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Não resolveu", PerguntaId = 2 },
                }
            });

            model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
            {
                Ordem = "ORDEM 2 - COMPOSIÇÃO",
                Respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>() {
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 60, AlunosQuantidade = randNum.Next(99999), Resposta = "Acertou" , PerguntaId = 1 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 30, AlunosQuantidade = randNum.Next(99999), Resposta = "Errou", PerguntaId = 1 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Não resolveu", PerguntaId = 1 },
                        new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 60, AlunosQuantidade = randNum.Next(99999), Resposta = "Acertou" , PerguntaId = 2 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 30, AlunosQuantidade = randNum.Next(99999), Resposta = "Errou", PerguntaId = 2 },
                    new RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Não resolveu", PerguntaId = 2 },
                }
            });


            return View("RelatorioSondagemComponentesMatematicaAditivoMultiplicativoConsolidado", model);
        }

        [HttpGet("controle-grade-sintetico")]
        public IActionResult RelatorioControleGradeSintetico([FromQuery] bool analitico)
        {

            var controleGrade = new ControleGradeDto()
            {
                Filtro = new FiltroControleGrade()
                {
                    Dre = "DRE - BT",
                    Ue = "CEU EMEF BUTANTA",
                    Turma = "Todas",
                    Bimestre = "Todos",
                    ComponenteCurricular = "Todos",
                    RF = "9879878",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                }
            };

            controleGrade.Turmas = new List<TurmaControleGradeDto>()
            {
                new TurmaControleGradeDto()
               {
                   Nome="1F",
                   Bimestres = new List<BimestreControleGradeDto>()
                   {
                       new BimestreControleGradeDto()
                       {
                           Descricao = "1° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "MATEMÁTICA",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "CIÊNCIAS",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               }
                           }
                       },
                       new BimestreControleGradeDto()
                       {
                           Descricao = "2° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "MATEMÁTICA",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "CIÊNCIAS",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               }
                           }
                       },
                       new BimestreControleGradeDto()
                       {
                           Descricao = "3° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               }
                           }
                       },
                       new BimestreControleGradeDto()
                       {
                           Descricao = "4° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               }
                           }
                       }

                   }
               },
                new TurmaControleGradeDto()
               {
                   Nome="2F",
                   Bimestres = new List<BimestreControleGradeDto>()
                   {
                       new BimestreControleGradeDto()
                       {
                           Descricao = "1° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Sim",
                                   DetalhamentoDivergencias = analitico ? new DetalhamentoDivergenciasControleGradeSinteticoDto()
                                   {
                                       AulasNormaisExcedido = new List<AulaNormalExcedidoControleGradeDto>()
                                       {
                                           new AulaNormalExcedidoControleGradeDto()
                                           {
                                               Data = "26/10/2020",
                                               QuantidadeAulas = 10,
                                               Professor = "SADAKO MORITA (3135535)"
                                           },
                                           new AulaNormalExcedidoControleGradeDto()
                                           {
                                               Data = "26/10/2020",
                                               QuantidadeAulas = 5,
                                               Professor = "SADAKO MORITA (3135535)"
                                           }
                                       },
                                       AulasTitularCJ = new List<AulaTitularCJDataControleGradeDto>()
                                       {
                                           new AulaTitularCJDataControleGradeDto() {
                                               Data = "26/10/2020",
                                               Divergencias = new List<AulaTitularCJControleGradeDto>()
                                               {
                                                  new AulaTitularCJControleGradeDto()
                                                  {
                                                      QuantidadeAulas = 2,
                                                      ProfessorTitular =  "SADAKO MORITA (3135535)",
                                                  },
                                                  new AulaTitularCJControleGradeDto()
                                                  {
                                                      QuantidadeAulas = 1,
                                                      ProfessorCJ =  "CARLA REGIANE (8027129)",
                                                  },
                                                  new AulaTitularCJControleGradeDto()
                                                  {
                                                      QuantidadeAulas = 1,
                                                      ProfessorCJ =  "ANA CRISTINA (7777710)",
                                                  }
                                               }
                                           }
                                       },
                                       AulasDiasNaoLetivos = new List<AulaDiasNaoLetivosControleGradeDto>()
                                       {
                                           new AulaDiasNaoLetivosControleGradeDto()
                                           {
                                                Data = "21/03/2020",
                                                Professor = "SADAKO MORITA (3135535)",
                                                Motivo  = "SUSPENSÃO DE ATIVIDADE",
                                                QuantidadeAulas = 2
                                           }
                                       },
                                       AulasDuplicadas = new List<AulaDuplicadaControleGradeDto>()
                                       {
                                           new AulaDuplicadaControleGradeDto()
                                           {
                                               Data = "21/03/2020",
                                               QuantidadeDuplicado = 1,
                                               Professor = "SADAKO MORITA (3135535)"
                                           }
                                       },
                                   } : null
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "MATEMÁTICA",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "MATEMÁTICA",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "MATEMÁTICA",
                                   AulasPrevistas = 15,
                                   AulasCriadasProfessorTitular = 14,
                                   AulasCriadasProfessorSubstituto = 1,
                                   AulasDadasProfessorTitular = 14,
                                   AulasDadasProfessorSubstituto = 1,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                           }
                       },
                       new BimestreControleGradeDto()
                       {
                           Descricao = "2° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               }
                           }
                       },
                       new BimestreControleGradeDto()
                       {
                           Descricao = "3° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 10,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 20,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               }
                           }
                       },
                       new BimestreControleGradeDto()
                       {
                           Descricao = "4° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>()
                           {
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "INGLÊS",
                                   AulasPrevistas = 12,
                                   AulasCriadasProfessorTitular = 8,
                                   AulasCriadasProfessorSubstituto = 2,
                                   AulasDadasProfessorTitular = 8,
                                   AulasDadasProfessorSubstituto = 2,
                                   Repostas = 0,
                                   Divergencias = "Não"
                               },
                               new ComponenteCurricularControleGradeDto()
                               {
                                   Nome = "PORTUGUÊS",
                                   AulasPrevistas = 19,
                                   AulasCriadasProfessorTitular = 16,
                                   AulasCriadasProfessorSubstituto = 4,
                                   AulasDadasProfessorTitular = 19,
                                   AulasDadasProfessorSubstituto = 4,
                                   Repostas = 3,
                                   Divergencias = "Não"
                               }
                           }
                       }

                   }
               }
            };

            return View("RelatorioControleGradeSintetico", controleGrade);

        }

        [HttpGet("sondagem-portugues-leitura")]
        public IActionResult SondagemPortuguesLeitura()
        {
            var linhas = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();

            for (var i = 0; i < 11; i++)
            {
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Nível1",
                            PerguntaId = "1"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 5727636,
                        Nome = "BRENDHA VITORIA DOMINGUES DE ALMEIDA)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Nível3",
                            PerguntaId = "1"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 7334362,
                        Nome = "ALESSANDRA RIKELY BENTO SANTOS",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Nível2",
                            PerguntaId = "1"
                        },
                    }
                });
            }

            var model = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
                {
                    AnoTurma = 5,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Português",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/YYYY"),
                    Dre = "DRE - BT",
                    Periodo = "1º Bimestre",
                    Proficiencia = "Leitura",
                    Rf = "9879878",
                    Turma = "EMEF - 1A",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Perguntas = new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "1",
                            Nome = "Leitura"
                        },
                    },
                },
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };

            model.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            model.GraficosBarras.Add(new GraficoBarrasVerticalDto(800, "Português - Leitura")
            {
                EixosX = new List<GraficoBarrasVerticalEixoXDto>()
              {
                  new GraficoBarrasVerticalEixoXDto(5, "A"),
                  new GraficoBarrasVerticalEixoXDto(1, "B"),
                  new GraficoBarrasVerticalEixoXDto(3, "C"),
                  new GraficoBarrasVerticalEixoXDto(2, "D")
              },
                Legendas = new List<GraficoBarrasLegendaDto>()
              {
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "A",
                      Valor = "Nivel 1"
                  },
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "B",
                      Valor = "Nivel 2"
                  },
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "C",
                      Valor = "Nivel 3"
                  },
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "D",
                      Valor = "Sem preenchimento"
                  }
              },
                EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", 5, 10)
            });

            return View("RelatorioSondagemPortuguesPorTurma", model);
        }

        [HttpGet("sondagem-portugues-escrita")]
        public IActionResult SondagemPortuguesEscrita()
        {
            var linhas = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();

            for (var i = 0; i < 6; i++)
            {
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Pré-silábico",
                            PerguntaId = "1"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "JOÃO MARIA LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Silábico sem valor sonoro",
                            PerguntaId = "1"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "MARCOS ALBERTO FIGUEIRA",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Silábico com valor sonoro",
                            PerguntaId = "1"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "FERNANDO PIRES",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Silábico alfabético",
                            PerguntaId = "1"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "JOÃO PEDRO FARIAS",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "Alfabético",
                            PerguntaId = "1"
                        },
                    }
                });
            }

            var model = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
                {
                    AnoTurma = 5,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "1º Bimestre",
                    Proficiencia = "Escrita",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Perguntas = new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "1",
                            Nome = "Escrita"
                        },
                    },
                },
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };

            return View("RelatorioSondagemPortuguesPorTurma", model);
        }

        [HttpGet("sondagem-portugues-capacidade-leitura")]
        public IActionResult SondagemPortuguesCapacidadeLeitura()
        {
            var linhas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto>();
            for (var i = 0; i < 20; i++)
            {
                linhas.Add(
                new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA ALEXIA FERNANDES LIMA ALEXIA FERNANDES LIMA",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Adequada",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Inadequada",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Adequada",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Inadequada",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Não resolveu",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Não resolveu",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "3",
                            Resposta = "Inadequada",
                            PerguntaId = "1"
                        },
                        //new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                        //    OrdemId = "3",
                        //    Resposta = "Não resolveu",
                        //    PerguntaId = "2"
                        //},
                        //new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                        //    OrdemId = "3",
                        //    Resposta = "Não resolveu",
                        //    PerguntaId = "3"
                        //},
                    },
                });
                linhas.Add(
                new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaAlunoDto()
                    {
                        Codigo = 4650630,
                        Nome = "MATHEUS GUILHERME NASCIMENTO DA SILVA (RECLASSIFICADO SAÍDA EM 11/04/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Desistente.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Adequada",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Inadequada",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Adequada",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Inadequada",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Não resolveu",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Não resolveu",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "3",
                            Resposta = "Inadequada",
                            PerguntaId = "1"
                        },
                    //    new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                    //        OrdemId = "3",
                    //        Resposta = "Não resolveu",
                    //        PerguntaId = "2"
                    //    },
                    //    new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                    //        OrdemId = "3",
                    //        Resposta = "Não resolveu",
                    //        PerguntaId = "3"
                    //    },
                    },
                });
                linhas.Add(
                new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "AMANDA ALBUQUERQUE",
                        SituacaoMatricula = SituacaoMatriculaAluno.NaoCompareceu.ToString(),
                    },
                    OrdensRespostas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Adequada",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Inadequada",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "1",
                            Resposta = "Adequada",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Inadequada",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = "2",
                            Resposta = "Não resolveu",
                            PerguntaId = "2"
                        },
                        //new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                        //    OrdemId = "2",
                        //    Resposta = "Não resolveu",
                        //    PerguntaId = "3"
                        //},
                        //new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                        //    OrdemId = "3",
                        //    Resposta = "Inadequada",
                        //    PerguntaId = "1"
                        //},
                        //new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                        //    OrdemId = "3",
                        //    Resposta = "Não resolveu",
                        //    PerguntaId = "2"
                        //},
                        //new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                        //    OrdemId = "3",
                        //    Resposta = "Não resolveu",
                        //    PerguntaId = "3"
                        //},
                    },
                });
            }

            var model = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaCabecalhoDto()
                {
                    Ano = 5.ToString(),
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/YYYY"),
                    Dre = "DRE - BT",
                    Periodo = "1º Semestre",
                    Proficiencia = "Campo Aditivo",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Ordens = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
                        {
                            Id = "1",
                            Descricao = "ORDEM NO NARRAR"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
                        {
                            Id = "2",
                            Descricao = "ORDEM DO RELATAR"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
                        {
                            Id = "3",
                            Descricao = "ORDEM DO ARGUMENTAR"
                        },
                    },
                    Perguntas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
                        {
                            Id = "1",
                            Nome = "Inferência"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
                        {
                            Id = "2",
                            Nome = "Localização "
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
                        {
                            Id = "3",
                            Nome = "Reflexão"
                        }
                    },
                },
                Planilha = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };
            model.GraficosBarras = new List<GraficoBarrasVerticalDto>();

            model.GraficosBarras.Add(new GraficoBarrasVerticalDto(800, "Português - Leitura")
            {
                EixosX = new List<GraficoBarrasVerticalEixoXDto>()
              {
                  new GraficoBarrasVerticalEixoXDto(50, "A"),
                  new GraficoBarrasVerticalEixoXDto(10, "B"),
                  new GraficoBarrasVerticalEixoXDto(30, "C"),
                  new GraficoBarrasVerticalEixoXDto(20, "D")
              },
                Legendas = new List<GraficoBarrasLegendaDto>()
              {
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "A",
                      Valor = "Nivel 1"
                  },
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "B",
                      Valor = "Nivel 2"
                  },
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "C",
                      Valor = "Nivel 3"
                  },
                  new GraficoBarrasLegendaDto()
                  {
                      Chave = "D",
                      Valor = "Sem preenchimento"
                  }
              },
                EixoYConfiguracao = new GraficoBarrasVerticalEixoYDto(350, "Quantidade Alunos", 50, 10)
            });

            return View("RelatorioSondagemPortuguesCapacidadeLeituraPorTurma", model);
        }

        [HttpGet("sondagem-portugues-leitura-voz-alta")]
        public IActionResult SondagemPortuguesLeituraVozAlta()
        {
            var linhas = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();

            for (var i = 0; i < 6; i++)
            {
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "4"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "JOÃO MARIA LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "4"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "MARCOS ALBERTO FIGUEIRA",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "4"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "FERNANDO PIRES",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                                {
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "1"
                                    },
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "2"
                                    },
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "3"
                                    },
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "4"
                                    },
                                }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "JOÃO PEDRO FARIAS",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                                {
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "1"
                                    },
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "2"
                                    },
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "N",
                                        PerguntaId = "3"
                                    },
                                    new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                                        Resposta = "S",
                                        PerguntaId = "4"
                                    },
                                }
                });
            }

            var model = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
                {
                    AnoTurma = 5,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Matemática",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "1º Bimestre",
                    Proficiencia = "Leitura em voz alta",
                    Rf = "9879878",
                    Turma = "EMEF - 5A",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Perguntas = new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "1",
                            Nome = "Não conseguiu ou não quis ler"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "2",
                            Nome = "Leu com muita dificuldade"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "3",
                            Nome = "Leu com alguma fluência	"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "4",
                            Nome = "Leu com fluência"
                        },
                    },
                },
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };

            return View("RelatorioSondagemPortuguesPorTurma", model);
        }

        [HttpGet("sondagem-portugues-consolidado-leitura")]
        public IActionResult SondagemPortuguesConsolidadoLeitura()
        {
            var planilhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>();
            #region Monta dados
            var perguntas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto>();
            perguntas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
            {
                Pergunta = "Localização",
                Respostas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>()
                {
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Adequada",
                        Quantidade = 1,
                        Percentual = (decimal)11.23,
                        Total = 3
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Inadequada",
                        Quantidade = 2,
                        Percentual = (decimal)12.23,
                        Total = 3
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Não resolveu",
                        Quantidade = 3,
                        Percentual = (decimal)13.23,
                        Total = 3
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        Quantidade = 4,
                        Percentual = (decimal)14.23,
                        Total = 3
                    },
                }
            });
            perguntas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
            {
                Pergunta = "Inferência",
                Respostas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>()
                {
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Adequada",
                        Quantidade = 5,
                        Percentual = (decimal)15.23,
                        Total = 4
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Inadequada",
                        Quantidade = 6,
                        Percentual = (decimal)16.23,
                        Total = 4
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Não resolveu",
                        Quantidade = 7,
                        Percentual = (decimal)17.23,
                        Total = 4
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        Quantidade = 8,
                        Percentual = (decimal)18.23,
                        Total = 4
                    },
                }
            });
            perguntas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaPerguntaDto()
            {
                Pergunta = "Reflexão",
                Respostas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto>()
                {
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Adequada",
                        Quantidade = 9,
                        Percentual = (decimal)9.23,
                        Total = 5
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Inadequada",
                        Quantidade = 10,
                        Percentual = (decimal)20.23,
                        Total = 5
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Não resolveu",
                        Quantidade = 11,
                        Percentual = (decimal)21.23,
                        Total = 5
                    },
                    new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaRespostaDto()
                    {
                        Resposta = "Sem preenchimento",
                        Quantidade = 12,
                        Percentual = (decimal)22.23,
                        Total = 5
                    },
                }
            });
            #endregion

            for (var i = 0; i < 5; i++)
            {
                planilhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
                {
                    Ordem = "Ordem do argumentar " + (i + 1),
                    Perguntas = perguntas
                });
            }

            var model = new RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto()
                {
                    AnoTurma = 5,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Português",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "1º bimestre",
                    Proficiencia = "Capacidade de Leitura",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                },
                Planilhas = planilhas,
            };
            return View("RelatorioSondagemPortuguesConsolidadoCapacidadeLeitura", model);
        }

        [HttpGet("sondagem-producao-texto-turma")]
        public IActionResult SondagemProducaopTextoTurma()
        {
            var linhas = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>();

            for (var i = 0; i < 6; i++)
            {
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "ALEXIA FERNANDES LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "4"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "5"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "6"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "JOÃO MARIA LIMA (RECLASSIFICADO SAÍDA EM 23/09/2020)",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "4"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "5"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "6"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "MARCOS ALBERTO FIGUEIRA",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "4"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "5"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "6"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "FERNANDO PIRES",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "4"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "5"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "6"
                        },
                    }
                });
                linhas.Add(new RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
                {
                    Aluno = new RelatorioSondagemComponentesPorTurmaAlunoDto()
                    {
                        Codigo = 6197654,
                        Nome = "JOÃO PEDRO FARIAS",
                        SituacaoMatricula = SituacaoMatriculaAluno.Ativo.ToString(),
                    },
                    Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "1"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "2"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "3"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "4"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "S",
                            PerguntaId = "5"
                        },
                        new RelatorioSondagemPortuguesPorTurmaRespostaDto() {
                            Resposta = "N",
                            PerguntaId = "6"
                        },
                    }
                });
            }

            var model = new RelatorioSondagemPortuguesPorTurmaRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
                {
                    AnoTurma = 8,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Português",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "4º Bimestre",
                    Proficiencia = "Produção de texto",
                    Rf = "9879878",
                    Turma = "EMEF - 8A",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Perguntas = new List<RelatorioSondagemPortuguesPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "1",
                            Nome = "Não produziu/entregou em branco"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "2",
                            Nome = "Não apresentou dificuldades"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "3",
                            Nome = "Escrita não alfabética"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "4",
                            Nome = "Dificuldades com aspectos semânticos"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "5",
                            Nome = "Dificuldades com aspectos textuais"
                        },
                        new RelatorioSondagemPortuguesPorTurmaPerguntaDto()
                        {
                            Id = "6",
                            Nome = "Dificuldades com aspectos ortográficos e notacionais"
                        },
                    },
                },
                Planilha = new RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };

            return View("RelatorioSondagemPortuguesPorTurma", model);
        }

        [HttpGet("sondagem-portugues-consolidado")]
        public IActionResult SondagemPortuguesConsolidado()
        {
            #region Monta dados
            var respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>()
            {
                new RelatorioSondagemPortuguesConsolidadoRespostaDto(){
                    Id = "1",
                    Resposta = "Pré-Silábico",
                    Quantidade = 1,
                    Percentual = (decimal)11.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto(){
                    Resposta = "Silábico sem Valor",
                    Quantidade = 2,
                    Percentual = (decimal)12.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto(){
                    Resposta = "Silábico com Valor",
                    Quantidade = 3,
                    Percentual = (decimal)13.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto(){
                    Resposta = "Silábico Alfabético",
                    Quantidade = 4,
                    Percentual = (decimal)14.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto(){
                    Resposta = "Alfabético",
                    Quantidade = 5,
                    Percentual = (decimal)15.23,
                    Total = 3
                },
            };
            #endregion

            var model = new RelatorioSondagemPortuguesConsolidadoRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesConsolidadoCabecalhoDto()
                {
                    AnoTurma = 1,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Português",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "1º bimestre",
                    Proficiencia = "Leitura",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida ",
                    EhProducaoTexto = false,
                },
                Respostas = respostas,
            };
            return View("RelatorioSondagemPortuguesConsolidado", model);
        }

        [HttpGet("sondagem-portugues-consolidado-producao")]
        public IActionResult SondagemPortuguesConsolidadoProducao()
        {
            #region Monta dados
            var respostas = new List<RelatorioSondagemPortuguesConsolidadoRespostaDto>()
            {
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "1",
                    Resposta = "Escrita não alfabética",
                    Quantidade = 1,
                    Percentual = (decimal)11.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "2",
                    Resposta = "Dificuldades com aspectos semânticos",
                    Quantidade = 2,
                    Percentual = (decimal)12.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "3",
                    Resposta = "Dificuldades com aspectos textuais",
                    Quantidade = 3,
                    Percentual = (decimal)13.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "4",
                    Resposta = "Dificuldades com aspectos ortográficos e notacionais",
                    Quantidade = 4,
                    Percentual = (decimal)14.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "5",
                    Resposta = "Não produziu/entregou em branco",
                    Quantidade = 5,
                    Percentual = (decimal)15.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "6",
                    Resposta = "Não apresentou dificuldades",
                    Quantidade = 6,
                    Percentual = (decimal)16.23,
                    Total = 3
                },
                new RelatorioSondagemPortuguesConsolidadoRespostaDto()
                {
                    Id = "7",
                    Resposta = "Sem preenchimento",
                    Quantidade = 7,
                    Percentual = (decimal)17.23,
                    Total = 3
                },

            };
            #endregion

            var model = new RelatorioSondagemPortuguesConsolidadoRelatorioDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesConsolidadoCabecalhoDto()
                {
                    AnoTurma = 4,
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Português",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "3º bimestre",
                    Proficiencia = "Produção de texto",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza",
                    EhProducaoTexto = true,
                },
                Respostas = respostas,
            };
            return View("RelatorioSondagemPortuguesConsolidado", model);
        }

        [HttpGet("graficos-pap")]
        public IActionResult RelatorioGraficosPAP()
        {
            var graficosDto = new List<ResumoPAPGraficoDto>();

            for (var i = 0; i < 7; i++)
            {
                var grafico = new ResumoPAPGraficoDto()
                {
                    Titulo = "SONDAGEM: HIPÓTESE ESCRITA",
                    Graficos = new List<ResumoPAPGraficoAnoDto>()
                        {
                        new ResumoPAPGraficoAnoDto(395, "")
                        {
                            Titulo =  (3 + i) + "º Ano",
                            EixosX = new List<GraficoBarrasPAPVerticalEixoXDto>()
                            {
                                new GraficoBarrasPAPVerticalEixoXDto(8, 10, "A"),
                                new GraficoBarrasPAPVerticalEixoXDto(8, 10, "B"),
                                new GraficoBarrasPAPVerticalEixoXDto(8, 10, "C"),
                                new GraficoBarrasPAPVerticalEixoXDto(8, 10, "D"),
                                new GraficoBarrasPAPVerticalEixoXDto(8, 10, "E"),
                            },
                            Legendas = new List<GraficoBarrasLegendaDto>()
                            {
                                new GraficoBarrasLegendaDto()
                                {
                                    Chave = "A",
                                    Valor = "Alfabético"
                                },
                                new GraficoBarrasLegendaDto()
                                {
                                    Chave = "B",
                                    Valor = "Silábico com valor sonoro"
                                },
                                new GraficoBarrasLegendaDto()
                                {
                                    Chave = "C",
                                    Valor = "Pré silábico"
                                },
                                new GraficoBarrasLegendaDto()
                                {
                                    Chave = "D",
                                    Valor = "Silábico alfabético"
                                },
                                new GraficoBarrasLegendaDto()
                                {
                                    Chave = "E",
                                    Valor = "Silábico sem valor sonoro"
                                },
                            },
                            EixoYConfiguracao = new GraficoBarrasPAPVerticalEixoYDto(350, "Alunos", 50, 10)
                        }
                    }
                };

                graficosDto.Add(grafico);
            }

            var model = new GraficoPAPDto()
            {
                DreNome = "DRE - BT",
                AnoLetivo = 2020,
                Ciclo = "INTERDISCIPLINAR",
                Ano = "4",
                Turma = "Todas",
                Periodo = "Acompanhamento 2º Semestre",
                UeNome = "Todas",
                UsuarioNome = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                UsuarioRF = "9879878",
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                EhEncaminhamento = false,
                GraficosDto = graficosDto
            };

            return View("RelatorioGraficosPAP", model);
        }
        [HttpGet("Usuarios")]
        public IActionResult RelatorioUsuarios()
        {
            var model = new RelatorioUsuarioDto()
            {
                Filtro = new FiltroUsuarioDto()
                {
                    Dre = "DRE - BT",
                    Ue = "CEU EMEF BUTANTA",
                    RF = "9879878",
                    Usuario = "Alice Gonçalves de Almeida Souza Gonçalves de Almeida Souza",
                },
                DadosRelatorio = new DadosRelatorioUsuariosDto()
            };
            //[HttpGet("Usuarios")]
            //public IActionResult RelatorioUsuarios()
            //{
            //    var model = new RelatorioUsuarioDto()
            //    {
            //        Filtro = new FiltroUsuarioDto()
            //        {
            //            Dre = "DRE - BT",
            //            Ue = "CEU EMEF BUTANTA",
            //            RF = "9879878",
            //            Usuario = "Alice Gonçalves de Almeida Souza Gonçalves de Almeida Souza",
            //        }
            //    };

            model.DadosRelatorio.Dres = new List<DreUsuarioDto>()
            {
                new DreUsuarioDto()
                {
                    Nome = "DRE - BT",
                    Perfis = new List<PerfilUsuarioDto>()
                    {
                        new PerfilUsuarioDto()
                        {
                            Nome = "ADM DRE",
                            Usuarios = new List<UsuarioDto>()
                            {
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Alice Gonçalves de Almeida Souza de Freitas",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana lucia de Freitas",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana lucia de Freitas",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Jessica de Oliveira",
                                    Situacao = "5 - Expirado",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Jessica de Oliveira",
                                    Situacao = "5 - Expirado",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Jessica de Oliveira",
                                    Situacao = "5 - Expirado",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Jessica de Oliveira",
                                    Situacao = "5 - Expirado",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Jessica de Oliveira",
                                    Situacao = "5 - Expirado",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },

                            }
                        },
                        new PerfilUsuarioDto()
                        {
                            Nome = "DIPED",
                            Usuarios = new List<UsuarioDto>()
                            {
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Carmen Mendes",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana Luiza Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana Luiza Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana Luiza Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana Luiza Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana Luiza Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioDto()
                                {
                                    Login = "153726",
                                    Nome = "Ana Luiza Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                            }
                        }
                    },
                    Ues = new List<UePorPerfilUsuarioDto>()
                    {
                        new UePorPerfilUsuarioDto()
                        {
                            Nome = "CEU EMEF BUTANTA",
                            Perfis = new List<PerfilUsuarioDto>()
                            {
                                new PerfilUsuarioDto()
                                {
                                    Nome = "CP",
                                    Usuarios = new List<UsuarioDto>()
                                    {
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Jessica de Oliveira",
                                            Situacao = "5 - Expirado",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Jessica de Oliveira",
                                            Situacao = "5 - Expirado",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                    }
                                },
                                new PerfilUsuarioDto()
                                {
                                    Nome = "DIRETOR",
                                    Usuarios = new List<UsuarioDto>()
                                    {
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                         new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        }
                                    }
                                }
                            },
                            Professores = new List<UsuarioProfessorDto>()
                            {
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                }
                            }
                        },
                        new UePorPerfilUsuarioDto()
                        {
                            Nome = "CEU EMEF BUTANTA",
                            Perfis = new List<PerfilUsuarioDto>()
                            {
                                new PerfilUsuarioDto()
                                {
                                    Nome = "CP",
                                    Usuarios = new List<UsuarioDto>()
                                    {
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Jessica de Oliveira",
                                            Situacao = "5 - Expirado",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Jessica de Oliveira",
                                            Situacao = "5 - Expirado",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                    }
                                },
                                new PerfilUsuarioDto()
                                {
                                    Nome = "DIRETOR",
                                    Usuarios = new List<UsuarioDto>()
                                    {
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                         new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        }
                                    }
                                }
                            },
                            Professores = new List<UsuarioProfessorDto>()
                            {
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                }
                            }
                        },
                        new UePorPerfilUsuarioDto()
                        {
                            Nome = "CEU EMEF BUTANTA",
                            Perfis = new List<PerfilUsuarioDto>()
                            {
                                new PerfilUsuarioDto()
                                {
                                    Nome = "CP",
                                    Usuarios = new List<UsuarioDto>()
                                    {
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Jessica de Oliveira",
                                            Situacao = "5 - Expirado",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana lucia de Freitas",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Jessica de Oliveira",
                                            Situacao = "5 - Expirado",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                    }
                                },
                                new PerfilUsuarioDto()
                                {
                                    Nome = "DIRETOR",
                                    Usuarios = new List<UsuarioDto>()
                                    {
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Carmen Mendes",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                        new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        },
                                         new UsuarioDto()
                                        {
                                            Login = "153726",
                                            Nome = "Ana Luiza Souza",
                                            Situacao = "1 - Ativo",
                                            UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        }
                                    }
                                }
                            },
                            Professores = new List<UsuarioProfessorDto>()
                            {
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                },
                                new UsuarioProfessorDto()
                                {
                                    Login = "153726",
                                    Nome = "Alexandre Barros de Souza",
                                    Situacao = "1 - Ativo",
                                    UltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaAulaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimoPlanoAulaRegistrado = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    UltimaFrequenciaRegistrada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                }
                            }
                        }
                    },
                    HistoricoReinicioSenha = new List<HistoricoReinicioSenhaDto>()
                    {
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Não",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        },
                        new HistoricoReinicioSenhaDto()
                        {
                            Login = "153726",
                            Nome = "Ana de Souza",
                            Perfil = "CP",
                            SenhaReiniciada = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            SenhaReiniciadaPor = "Jordana Carvalho de Arruda",
                            UtilizaSenhaPadao = "Sim",
                        }
                    }
                }
            };
            return View("RelatorioUsuarios", model);
        }

        [HttpGet("notificacoes")]
        public IActionResult Notificacoes()
        {
            var model = new RelatorioNotificacaoDto
            {

                CabecalhoDRE = "TODAS",
                CabecalhoUE = "TODAS",
                CabecalhoUsuario = "ALANA FERREIRA DE OLIVEIRA",
                CabecalhoUsuarioRF = "1234567",
                DREs = new List<DreNotificacaoDto>()
                {
                    new DreNotificacaoDto()
                    {
                        Nome = "CAPELA DO SOCORRO",
                        UEs = new List<UeNotificacaoDto>()
                        {
                            new UeNotificacaoDto() {
                                Nome = "CEU EMEF JARDIM ELIANA",
                                Usuarios = new List<UsuarioNotificacaoDto>()
                                {
                                    new UsuarioNotificacaoDto()
                                    {
                                        Nome = "Irêne de Carvalho (1234567)", Notificacoes = new List<NotificacaoDto>()
                                        {
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            },
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            },
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            },
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            }
                                        }
                                    }
                                }
                            },
                            new UeNotificacaoDto() {
                                Nome = "CEU EMEF JARDIM ELIANA",
                                Usuarios = new List<UsuarioNotificacaoDto>()
                                {
                                    new UsuarioNotificacaoDto()
                                    {
                                        Nome = "Irêne de Carvalho (1234567)", Notificacoes = new List<NotificacaoDto>()
                                        {
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            },
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            },
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            },
                                            new NotificacaoDto()
                                            {
                                                Codigo = 153726,
                                                Titulo = "Relatório de Notas e Conceitos Finais",
                                                Categoria = NotificacaoCategoria.Aviso,
                                                Tipo = NotificacaoTipo.Relatorio,
                                                Situacao = NotificacaoStatus.Lida,
                                                DataRecebimento = DateTime.Now,
                                                DataLeitura = DateTime.Now
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return View("RelatorioNotificacoes", model);
        }

        [HttpGet("relatorio-atribuicoes-cj")]
        public IActionResult RelatorioAtribuioesCj()
        {
            var model = new RelatorioAtribuicaoCjDto()
            {
                DreNome = "CAPELA DO SOCORRO",
                UeNome = "CEU EMEF JARDIM ELIANA",
                Modalidade = Modalidade.Fundamental.Name(),
                Semestre = "1º",
                Turma = "TODAS",
                Professor = "ALANA FERRIRA OLIVEIRA",
                RfProfessor = "1234569789",
                Usuario = "RITA DE CASSIA FREITAS",
                RfUsuario = "789456310",
                AtribuicoesCjPorProfessor = new List<AtribuicaoCjPorProfessorDto>()
                    {
                        new AtribuicaoCjPorProfessorDto()
                        {
                            NomeProfessor = "PRISCILA MELLO DE ANDRADE(12345678) - ESPORÁDICO",
                            AtribuiicoesCjTurma = new List<AtribuicaoCjTurmaDto>()
                            {
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "24/03/2020",
                                    Aulas = new List<AtribuicaoCjAulaDto>()
                                    {
                                        new AtribuicaoCjAulaDto()
                                        {
                                            AulaDada = true,
                                            DataAula = "10/10/2020",
                                            Observacoes = "Sem observações askldjçlkj asdklj fçlkajsd fçlkjas dklj çlkasjdf çlkj açlksdj lkçj fasçldkj açlkjsd fçlkj  çlakkjsd fçlkja sdçfkj jç aksjd çlkj façskjd çlkj asd"
                                        },
                                        new AtribuicaoCjAulaDto()
                                        {
                                            AulaDada = true,
                                            DataAula = "10/10/2020",
                                            Observacoes = "Sem observações"
                                        },
                                        new AtribuicaoCjAulaDto()
                                        {
                                            AulaDada = false,
                                            DataAula = "10/10/2020",
                                            Observacoes = "Sem observações"
                                        },
                                        new AtribuicaoCjAulaDto()
                                        {
                                            AulaDada = true,
                                            DataAula = "10/10/2020",
                                            Observacoes = "Sem observações"
                                        },
                                        new AtribuicaoCjAulaDto()
                                        {
                                            AulaDada = true,
                                            DataAula = "10/10/2020",
                                            Observacoes = "Sem observações"
                                        }
                                    }
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                }
                            }
                        },
                        new AtribuicaoCjPorProfessorDto()
                        {
                            NomeProfessor = "JULIA ANDRADE(12345678) - ESPORÁDICO",
                            AtribuiicoesCjTurma = new List<AtribuicaoCjTurmaDto>()
                            {
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "24/03/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                },
                                new AtribuicaoCjTurmaDto()
                                {
                                    NomeTurma = "2B",
                                    ComponenteCurricular = "Português",
                                    NomeProfessorTitular = "ANA MARIA CARDOSO (98764531)",
                                    DataAtribuicao= "21/09/2020"
                                }
                            }
                        }
                    },
                AtribuicoesCjPorTurma = new List<AtribuicaoCjPorTurmaDto>()
                {
                    new AtribuicaoCjPorTurmaDto()
                    {
                        NomeTurma =  "1A",
                        AtribuicoesCjProfessor = new List<AtribuicaoCjProfessorDto>()
                        {
                            new AtribuicaoCjProfessorDto()
                            {
                                NomeProfessorCj = "Priscila Mello",
                                ComponenteCurricular = "Biologia",
                                NomeProfessorTitular = "Ana Paula Souza",
                                DataAtribuicao = "20/08/2020",
                                TipoProfessorCj = "Esporádico",
                                Aulas = new List<AtribuicaoCjAulaDto>()
                                {
                                    new AtribuicaoCjAulaDto()
                                    {
                                        DataAula = "23/08/2020",
                                        AulaDada = true,
                                        Observacoes = "Teste"
                                    },new AtribuicaoCjAulaDto()
                                    {
                                        DataAula = "23/08/2020",
                                        AulaDada = true,
                                        Observacoes = "Teste"
                                    },new AtribuicaoCjAulaDto()
                                    {
                                        DataAula = "23/08/2020",
                                        AulaDada = true,
                                        Observacoes = "Teste"
                                    }
                                }
                            },
                            new AtribuicaoCjProfessorDto()
                            {
                                NomeProfessorCj = "Jorge Abreu",
                                ComponenteCurricular = "Português",
                                NomeProfessorTitular = "Marcos Santos",
                                DataAtribuicao = "20/09/2020",
                                TipoProfessorCj = "Esporádico"
                            }
                        }
                    },
                    new AtribuicaoCjPorTurmaDto()
                    {
                        NomeTurma =  "1B",
                        AtribuicoesCjProfessor = new List<AtribuicaoCjProfessorDto>()
                        {
                            new AtribuicaoCjProfessorDto()
                            {
                                NomeProfessorCj = "Priscila Mello",
                                ComponenteCurricular = "Biologia",
                                NomeProfessorTitular = "Ana Paula Souza",
                                DataAtribuicao = "20/08/2020",
                                TipoProfessorCj = "Esporádico",
                                Aulas = new List<AtribuicaoCjAulaDto>()
                                {
                                    new AtribuicaoCjAulaDto()
                                    {
                                        DataAula = "23/08/2020",
                                        AulaDada = true,
                                        Observacoes = "Teste"
                                    },new AtribuicaoCjAulaDto()
                                    {
                                        DataAula = "23/08/2020",
                                        AulaDada = true,
                                        Observacoes = "Teste"
                                    },new AtribuicaoCjAulaDto()
                                    {
                                        DataAula = "23/08/2020",
                                        AulaDada = true,
                                        Observacoes = "Teste"
                                    }
                                }
                            },
                            new AtribuicaoCjProfessorDto()
                            {
                                NomeProfessorCj = "Jorge Abreu",
                                ComponenteCurricular = "Português",
                                NomeProfessorTitular = "Marcos Santos",
                                DataAtribuicao = "20/09/2020",
                                TipoProfessorCj = "Esporádico"
                            }
                        }
                    }
                },
                AtribuicoesEsporadicas = new List<AtribuicaoEsporadicaDto>()
                {
                    new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },
                    new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    },new AtribuicaoEsporadicaDto()
                    {
                        NomeUsuario = "Joana Lime do Carmo (1234569)",
                        Cargo = "Secretária (214123456)",
                        DataInicio = "18/06/2020",
                        DataFim = "20/12/2020",
                        AtribuidoPor= "Teste",
                        DataAtribuicao = "04/12/2020"
                    }
                }
            };

            return View("RelatorioHistoricoAlteracoesNotas", model);

        }

        [HttpGet("escola-aqui-leitura")]
        public IActionResult RelatorioEscolaAquiLeitura()
        {
            var model = new RelatorioLeituraComunicadosDto()
            {
                Filtro = new FiltroLeituraComunicadosDto()
                {
                    Dre = "DRE - BT",
                    Ue = "CEU EMEF BUTANTA",
                    RF = "9879878",
                    Usuario = "Anala Ferreira de Oliveira"
                },

                LeituraComunicadoDto = new List<LeituraComunicadoDto>()
                {
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020",
                        DataEnvio = DateTime.Now,
                        DataExpiracao = DateTime.Now.AddDays(5),
                        NaoInstalado = 523,
                        NaoVisualizado = 235,
                        Visualizado = 236,
                        LeituraComunicadoTurma = new List<LeituraComunicadoTurmaDto>()
                        {
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10,
                                LeituraComunicadoEstudantes = new List<LeituraComunicadoEstudanteDto>()
                                {
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "1",
                                        CodigoEstudante = "1256989",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    },
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "2",
                                        CodigoEstudante = "1256990",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    },
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "3",
                                        CodigoEstudante = "1256991",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Visualizada"
                                    }
                                }
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10,
                                LeituraComunicadoEstudantes = new List<LeituraComunicadoEstudanteDto>()
                                {
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "1",
                                        CodigoEstudante = "1256989",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    },
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "2",
                                        CodigoEstudante = "1256990",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    },
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "3",
                                        CodigoEstudante = "1256991",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Visualizada"
                                    }
                                }
                            }
                        }
                    },
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523,
                        NaoVisualizado = 235,
                        Visualizado = 236,
                        LeituraComunicadoTurma = new List<LeituraComunicadoTurmaDto>()
                        {
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                               TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10,
                                LeituraComunicadoEstudantes = new List<LeituraComunicadoEstudanteDto>()
                                {
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "1",
                                        CodigoEstudante = "1256989",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    },
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "1",
                                        CodigoEstudante = "1256989",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    },
                                    new LeituraComunicadoEstudanteDto()
                                    {
                                        NumeroChamada = "1",
                                        CodigoEstudante = "1256989",
                                        Estudante = "Alexandra Maria dos Santos",
                                        Responsavel = "Sarita Capiperibe",
                                        TipoResponsavel = " (Filiação 1)",
                                        ContatoResponsavel = "(11) 94585-0366",
                                        Situacao = "Não Visualizada"
                                    }
                                }
                            }

                        }},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto()
{
    Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523,
                        NaoVisualizado = 235,
                        Visualizado = 236,
                        LeituraComunicadoTurma = new List<LeituraComunicadoTurmaDto>()
                        {
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "2A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "3A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "4A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            }

                        }},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto()
{
    Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523,
                        NaoVisualizado = 235,
                        Visualizado = 236,
                        LeituraComunicadoTurma = new List<LeituraComunicadoTurmaDto>()
                        {
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "1A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "2A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "3A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            },
                            new LeituraComunicadoTurmaDto()
                            {
                                TurmaNome = "4A",
                                TurmaModalidade = Modalidade.Fundamental,
                                NaoInstalado = 2,
                                NaoVisualizado = 2,
                                Visualizado = 10
                            }

                        }},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina 2020", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 236},
                    new LeituraComunicadoDto() { Comunicado = "Festa junina Festa junina Festa junina 2099Festa junina Festa junina Festa junina 2099Festa junina Festa junina Festa junina 2099", DataEnvio = DateTime.Now, DataExpiracao = DateTime.Now.AddDays(5), NaoInstalado = 523, NaoVisualizado = 235, Visualizado = 244},

                }
            };

            return View("RelatorioEscolaAquiLeituraComunicados", model);
        }


        [HttpGet("controle-planejamento-diario")]
        public IActionResult RelatorioControlePlanejamentoDiario()
        {
            var model = new RelatorioControlePlanejamentoDiarioDto()
            {
                Filtro = new FiltroControlePlanejamentoDiarioDto()
                {
                    Dre = "DRE - BT",
                    Ue = "CEU EMEF BUTANTA",
                    Turma = "Todas",
                    ComponenteCurricular = "Lingua Portuguesa",
                    Bimestre = "Todos",
                    Usuario = "Anala Ferreira de Oliveira",
                    RF = "9879878",
                },

                Turmas = new List<TurmaPlanejamentoDiarioDto>()
                {
                    new TurmaPlanejamentoDiarioDto()
                    {
                        Nome = "EF - 1A",
                        Bimestres = new List<BimestrePlanejamentoDiarioDto>()
                        {
                            new BimestrePlanejamentoDiarioDto()
                            {
                                Nome = "1º Bimestre",
                                ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
                                {
                                    new ComponenteCurricularPlanejamentoDiarioDto()
                                    {
                                        Nome = "Lingua Portuguesa",
                                        PlanejamentoDiario = new List<PlanejamentoDiarioDto>()
                                        {
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = string.Empty

                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) -CJ",
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = string.Empty

                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                        }
                                    }
                                }
                            },
                            new BimestrePlanejamentoDiarioDto()
                            {
                                Nome = "2º Bimestre",
                                ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
                                {
                                    new ComponenteCurricularPlanejamentoDiarioDto()
                                    {
                                        Nome = "Lingua Portuguesa",
                                        PlanejamentoDiario = new List<PlanejamentoDiarioDto>()
                                        {
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = string.Empty

                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) -CJ",
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = string.Empty

                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                        }
                                    }
                                }
                            },
                            new BimestrePlanejamentoDiarioDto()
                            {
                                Nome = "3º Bimestre",
                                ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
                                {
                                    new ComponenteCurricularPlanejamentoDiarioDto()
                                    {
                                        Nome = "Lingua Portuguesa",
                                        PlanejamentoDiario = new List<PlanejamentoDiarioDto>()
                                        {
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"bloco 1the standard lorem ipsum passage, used since the 1500s lorem ipsum dolor sit amet, consectetur
                                                                                adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ut enim ad minim veniam, quis
                                                                                nostrud exercitation ullamco laboris nisi ut aliquip exea commodo consequat. duis aute irure dolor in reprehenderit
                                                                                in voluptate velit esse cillum dolore eu fugiat nulla pariatur. excepteur sint occaecat cupidatat non proident, sunt in
                                                                                culpa qui officia deserunt mollit anim id est laborum. section 1.10.32 ofde finibus bonorum et malorum,written
                                                                                bycicero in 45 bc sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque
                                                                                laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta
                                                                                sunt explicabo. nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur
                                                                                magni dolores eos qui ratione voluptatem sequi nesciunt. neque porro quisquam est, qui dolorem ipsum quia
                                                                                dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore
                                                                                magnam aliquam quaerat voluptatem. ut enim ad minima veniam, quis nostrum exercitationem ullam corporis
                                                                                suscipit laboriosam, nisi ut aliquid exea commodi consequatur? quis autem vel eum iure reprehenderit qui in ea
                                                                                voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla
                                                                                pariatur? 1914 translation byh. rackham but i must explain to you howall this mistaken idea of denouncing
                                                                                pleasure and praising pain was born and i will give you a complete account of the system, and expound the actual
                                                                                teachings of the great explorer of the truth, the master-builder of human happiness. no one rejects, dislikes, or
                                                                                avoids pleasure itself, because it is pleasure, but because those who do not knowhowto pursue pleasure
                                                                                rationallyencounter consequences that are extremelypainful. nor again is there anyone who loves or pursues or
                                                                                desires to obtain pain of itself, because it is pain, but because occasionallycircumstances occur in which toil and
                                                                                pain can procure him some great pleasure. to take a trivial example,which of us ever undertakes laborious physical
                                                                                exercise, except to obtain some advantage from it? butwho has anyright to find faultwith a man who chooses to
                                                                                enjoya pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant
                                                                                pleasure? section 1.10.33 ofde finibus bonorum et malorum,written bycicero in 45 bc at vero eos et
                                                                                accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos
                                                                                dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia
                                                                                deserunt mollitia animi, id est laborum et dolorum fuga. et harum quidem rerum facilis est et expedita distinctio.
                                                                                nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat
                                                                                facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. temporibus autem quibusdam et aut
                                                                                officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non
                                                                                recusandae. itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis bloco 2 -----------the standard
                                                                                lorem ipsum passage, used since the 1500s lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
                                                                                eiusmod tempor incididunt ut labore et dolore magna aliqua. ut enim ad minim veniam, quis nostrud exercitation
                                                                                ullamco laboris nisi ut aliquip exea commodo consequat. duis aute irure dolor in reprehenderit in voluptate velit
                                                                                esse cillum dolore eu fugiat nulla pariatur. excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia
                                                                                deserunt mollit anim id est laborum. section 1.10.32 ofde finibus bonorum et malorum,written bycicero in 45 bc
                                                                                sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem
                                                                                aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. nemo
                                                                                enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores e 
                                                                                consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam
                                                                                aliquam quaerat voluptatem. ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit
                                                                                laboriosam, nisi ut aliquid exea commodi consequatur? quis autem vel eum iure reprehenderit qui in ea voluptate
                                                                                velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? 1914
                                                                                translation byh. rackham but i must explain to you howall this mistaken idea of denouncing pleasure and praising
                                                                                pain was born and i will give you a complete account of the system, and expound the actual teachings of the great
                                                                                explorer of the truth, the master-builder of human happiness. no one rejects, dislikes, or avoids pleasure itself,
                                                                                because it is pleasure, but because those who do not knowhowto pursue pleasure rationallyencounter
                                                                                consequences that are extremelypainful. nor again is there anyone who loves or pursues or desires to obtain pain
                                                                                of itself, because it is pain, but because occasionallycircumstances occur in which toil and pain can procure him
                                                                                some great pleasure. to take a trivial example,which of us ever undertakes laborious physical exercise, except to
                                                                                obtain some advantage from it? butwho has anyright to find faultwith a man who chooses to enjoya pleasure that
                                                                                has no annoying consequences, or one who avoids a pain that produces no resultant pleasure? section 1.10.33 of
                                                                                de finibus bonorum et malorum,written bycicero in 45 bc at vero eos et accusamus et iusto odio dignissimos
                                                                                ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi
                                                                                sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et
                                                                                dolorum fuga. et harum quidem rerum facilis est et expedita distinctio. nam libero tempore, cum soluta nobis est
                                                                                eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas
                                                                                assumenda est, omnis dolor repellendus. temporibus autem quibusdam et aut officiis debitis aut rerum
                                                                                necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. itaque earum rerum
                                                                                hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus
                                                                                asperiores repellat. bloco 3 ----the standard lorem ipsum passage, used since the 1500s lorem ipsum dolor sit
                                                                                amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ut enim ad
                                                                                minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip exea commodo consequat. duis aute irure
                                                                                dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. excepteur sint occaecat cupidatat
                                                                                non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. section 1.10.32 ofde finibus bonorum
                                                                                et malorum,written bycicero in 45 bc sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium
                                                                                doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae
                                                                                vitae dicta sunt explicabo. nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia
                                                                                consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. neque porro quisquam est, qui dolorem
                                                                                ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore
                                                                                et dolore magnam aliquam quaerat voluptatem. ut enim ad minima veniam, quis nostrum exercitationem ullam
                                                                                corporis suscipit laboriosam, nisi ut aliquid exea commodi consequatur? quis autem vel eum iure reprehenderit
                                                                                qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla
                                                                                pariatur? 1914 translation byh. rackham but i must explain to you howall this mistaken idea of denouncing
                                                                                pleasure and praising pain was born and i will give you a complete account of the system, and expound the actual
                                                                                teachings of the great explorer of the truth, the master-builder of human happiness. no one rejects, dislikes, or
                                                                                avoids pleasure itself, because it is pleasure, but because those who do not knowhowto pursue pleasure
                                                                                rationallyencounter consequences that are extremelypainful. nor again is there anyone who loves or pursues or
                                                                                desires to obtain pain of itself, because it is pain, but because occasionallycircumstances occur in which toil and
                                                                                pain can procure him some great pleasure. to take a trivial example,which of us ever undertakes laborious physical
                                                                                exercise, except to obtain some advantage from it? butwho has anyright to find faultwith a man who chooses to
                                                                                enjoya pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant
                                                                                pleasure? section 1.10.33 ofde finibus bonorum et malorum,written bycicero in 45 bc at vero eos et
                                                                                accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos
                                                                                dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia
                                                                                deserunt mollitia animi, id est laborum et dolorum fuga. et harum quidem rerum facilis est et expedita distinctio.
                                                                                nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat
                                                                                facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. temporibus autem quibusdam et aut
                                                                                officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non
                                                                                recusandae. itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias
                                                                                consequatur aut perferendis doloribus asperiores repellat. voluptatibus maiores alias consequatur aut
                                                                                perferendis doloribus asperiores repellat.",

                                            },
                                        }
                                    }
                                }
                            },
                            new BimestrePlanejamentoDiarioDto()
                            {
                                Nome = "4º Bimestre",
                                ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
                                {
                                    new ComponenteCurricularPlanejamentoDiarioDto()
                                    {
                                        Nome = "Lingua Portuguesa",
                                        PlanejamentoDiario = new List<PlanejamentoDiarioDto>()
                                        {
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = "Compreender a diversidade de modalidades esportivas e paradesportivas e as formas de prática presentes no contexto regional.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) -CJ",
                                                SecoesPreenchidas = "&nbsp; - Meus Objetivos específicos <br> &nbsp; - Desenvolvimento da aula <br> &nbsp; - Lição de casa",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                              Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Relacionar a influência de diferentes ritmos a cada tipo de dança.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) - CJ",
                                                SecoesPreenchidas = "&nbsp; - Meus Objetivos específicos <br> &nbsp; - Desenvolvimento da aula <br> &nbsp; - Lição de casa",
                                                ObjetivosSelecionados = "EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                             Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                             Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                             Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) - CJ",
                                                SecoesPreenchidas = "&nbsp; - Meus Objetivos específicos <br> &nbsp; - Desenvolvimento da aula <br> &nbsp; - Lição de casa",
                                                ObjetivosSelecionados = "EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = "Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = "Compreender a diversidade de modalidades esportivas e paradesportivas e as formas de prática presentes no contexto regional.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) -CJ",
                                                SecoesPreenchidas = "&nbsp; - Meus Objetivos específicos <br> &nbsp; - Desenvolvimento da aula <br> &nbsp; - Lição de casa",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                              Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Relacionar a influência de diferentes ritmos a cada tipo de dança.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999) - CJ",
                                                SecoesPreenchidas = "&nbsp; - Meus Objetivos específicos <br> &nbsp; - Desenvolvimento da aula <br> &nbsp; - Lição de casa",
                                                ObjetivosSelecionados = "EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                             Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                             Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Objetivos de Aprendizagem e Desenvolvimento 2 objetivos selecionados <br> &nbsp; - Meus Objetivos Especificos <br> &nbsp; - Desenvolvimento da aula",
                                                ObjetivosSelecionados = " EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = @"Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.
                                                                             Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano de aula.Texto que o professor vai digitar no respectivo campo do plano.",
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = false,
                                                DateRegistro = string.Empty,
                                                Usuario = string.Empty,
                                                SecoesPreenchidas = string.Empty
                                            },
                                            new PlanejamentoDiarioDto()
                                            {
                                                DataAula = DateTime.Now.AddDays(9).ToString("dd/MM/yyyy"),
                                                QuantidadeAulas = 2,
                                                PlanejamentoRealizado = true,
                                                DateRegistro = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy HH:mm"),
                                                Usuario = "Maria da Silva (999999)",
                                                SecoesPreenchidas = "&nbsp; - Meus Objetivos específicos <br> &nbsp; - Desenvolvimento da aula <br> &nbsp; - Lição de casa",
                                                ObjetivosSelecionados = "EF06LP03 <br> EF06LP08 <br> EF06LP09",
                                                MeusObjetivosEspecificos = "Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula. Texto que o professor vai digitar no respectivo campo do plano de aula.",
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                },
            };

            return View("RelatorioControlePlanejamentoDiario", model);
        }

        [HttpGet("controle-planejamento-diario-infantil")]
        public IActionResult RelatorioControlePlanejamentoDiarioInfantil()
        {
            return Ok();
            //var model = new RelatorioControlePlanejamentoDiarioDto()
            //{
            //    Filtro = new FiltroControlePlanejamentoDiarioDto()
            //    {
            //        Dre = "DRE - BT",
            //        Ue = "CEU EMEF BUTANTA",
            //        Turma = "Todas",
            //        ComponenteCurricular = "Todos",
            //        Bimestre = "Todos",
            //        Usuario = "Anala Ferreira de Oliveira",
            //        RF = "9879878",
            //    },

            //    Turmas = new List<TurmaPlanejamentoDiarioDto>()
            //    {
            //        new TurmaPlanejamentoDiarioDto()
            //        {
            //            Nome = "EF - 1A",
            //            Bimestres = new List<BimestrePlanejamentoDiarioDto>()
            //            {
            //                new BimestrePlanejamentoDiarioDto()
            //                {
            //                    Nome = "1º Bimestre",
            //                    ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
            //                    {
            //                        new ComponenteCurricularPlanejamentoDiarioDto()
            //                        {
            //                            Nome = "Lingua Portuguesa",
            //                            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>()
            //                            {
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),                                                
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),                                                
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Devolutiva",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),                                                
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Devolutiva",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Devolutiva",
            //                                    Planejamento = @"Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo. Texto que o professor vai digitar no respectivo campo."
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(13).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                }
            //                            }
            //                        }
            //                    }
            //                },
            //                new BimestrePlanejamentoDiarioDto()
            //                {
            //                    Nome = "2º Bimestre",
            //                    ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
            //                    {
            //                        new ComponenteCurricularPlanejamentoDiarioDto()
            //                        {
            //                            Nome = "Lingua Portuguesa",
            //                            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>()
            //                            {
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Devolutiva"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Devolutiva"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = false,
            //                                    DateRegistro = "",
            //                                    Usuario = "",
            //                                    SecoesPreenchidas = ""
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Devolutiva"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                },
            //                                new PlanejamentoDiarioInfantilDto()
            //                                {
            //                                    DataAula = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
            //                                    PlanejamentoRealizado = true,
            //                                    DateRegistro = DateTime.Now.AddDays(13).ToString("dd/MM/yyyy HH:mm"),
            //                                    Usuario = "Maria da Silva (999999)",
            //                                    SecoesPreenchidas = "&nbsp; - Planejamento <br> &nbsp; - Reflexões e replanejamento"
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    },
            //};

            //return View("RelatorioControlePlanejamentoDiarioInfantil", model);
        }

        [HttpGet("controle-planejamento-diario-infantil-componente")]
        public IActionResult RelatorioControlePlanejamentoDiarioInfantilComComponente()
        {
            var retorno = new RelatorioControlePlanejamentoDiarioDto
            {
                Filtro = new FiltroControlePlanejamentoDiarioDto
                {
                    Dre = "DRE - CL",
                    Ue = "019493 - CEI - CAPAO REDONDO",
                    Turma = "EI - 5A",
                    ComponenteCurricular = "Regência de Classe Infantil",
                    Bimestre = "Todos",
                    Usuario = "TATIANE GONCALVES FERREIRA SOARES",
                    RF = "7851493",
                },
                Turmas = new List<TurmaPlanejamentoDiarioDto>()
                    {
                        new TurmaPlanejamentoDiarioDto()
                        {
                            Nome = "EF - 1A",
                            Bimestres = new List<BimestrePlanejamentoDiarioDto>()
                            {
                                new BimestrePlanejamentoDiarioDto()
                                {
                                    Nome = "1º Bimestre",
                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                    ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
                                    {
                                        new ComponenteCurricularPlanejamentoDiarioDto()
                                        {
                                            Nome = "REGÊNCIA INFANTIL EMEI 4H",
                                            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>()
                                            {
                                                new PlanejamentoDiarioInfantilDto
                                                {
                                                    AulaId  = 1000,
                                                    AulaCJ  = false,
                                                    DataAula  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    PlanejamentoRealizado  = true,
                                                    DateRegistro  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    Usuario  = "Levi Gael Kevin Ribeiro",
                                                    SecoesPreenchidas  = "- Planejamento",
                                                    Planejamento  = @"At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati 
                                                                      cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. 
                                                                      Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. 
                                                                      Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente 
                                                                      delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.1914 translation by H. Rackham On the other hand, we denounce with righteous indignation 
                                                                      and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee the pain and trouble that are bound to ensue; and equal blame 
                                                                      belongs to those who fail in their duty through weakness of will, which is the same as saying through shrinking from toil and pain. These cases are perfectly simple and easy to distinguish. In a free hour, 
                                                                      when our power of choice is untrammelled and when nothing prevents our being able to do what we like best, every pleasure is to be welcomed and every pain avoided. But in certain circumstances and owing to 
                                                                      the claims of duty or the obligations of business it will frequently occur that pleasures have to be repudiated and annoyances accepted. The wise man therefore always holds in these matters to this principle 
                                                                      of selection: he rejects pleasures to secure other greater pleasures, or else he endures pains to avoid worse pains.",
                                                }
                                            }
                                        },
                                       new ComponenteCurricularPlanejamentoDiarioDto()
                                        {
                                            Nome = "REGÊNCIA INFANTIL EMEI 2H",
                                            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>()
                                            {
                                                new PlanejamentoDiarioInfantilDto
                                                {
                                                    AulaId  = 1012,
                                                    AulaCJ  = false,
                                                    DataAula  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    PlanejamentoRealizado  = true,
                                                    DateRegistro  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    Usuario  = "Levi Gael Kevin Ribeiro",
                                                    SecoesPreenchidas  = "- Planejamento",
                                                    Planejamento  = @"At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati 
                                                                      cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. 
                                                                      Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. 
                                                                      Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente 
                                                                      delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.1914 translation by H. Rackham On the other hand, we denounce with righteous indignation 
                                                                      and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee the pain and trouble that are bound to ensue; and equal blame 
                                                                      belongs to those who fail in their duty through weakness of will, which is the same as saying through shrinking from toil and pain. These cases are perfectly simple and easy to distinguish. In a free hour, 
                                                                      when our power of choice is untrammelled and when nothing prevents our being able to do what we like best, every pleasure is to be welcomed and every pain avoided. But in certain circumstances and owing to 
                                                                      the claims of duty or the obligations of business it will frequently occur that pleasures have to be repudiated and annoyances accepted. The wise man therefore always holds in these matters to this principle 
                                                                      of selection: he rejects pleasures to secure other greater pleasures, or else he endures pains to avoid worse pains.",
                                                }
                                            }
                                        }
                                    }
                                },
                                new BimestrePlanejamentoDiarioDto()
                                {
                                    Nome = "2º Bimestre",
                                    DataAula = DateTime.Now.ToString("dd/MM/yyyy"),
                                    ComponentesCurriculares = new List<ComponenteCurricularPlanejamentoDiarioDto>()
                                    {
                                        new ComponenteCurricularPlanejamentoDiarioDto()
                                        {
                                            Nome = "REGÊNCIA INFANTIL EMEI 2H",
                                            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>()
                                            {
                                                new PlanejamentoDiarioInfantilDto
                                                {
                                                    AulaId  = 3000,
                                                    AulaCJ  = false,
                                                    DataAula  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    PlanejamentoRealizado  = true,
                                                    DateRegistro  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    Usuario  = "Levi Gael Kevin Ribeiro",
                                                    SecoesPreenchidas  = "- Planejamento",
                                                    Planejamento  = @"At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati 
                                                                      cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. 
                                                                      Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. 
                                                                      Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente 
                                                                      delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.1914 translation by H. Rackham On the other hand, we denounce with righteous indignation 
                                                                      and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee the pain and trouble that are bound to ensue; and equal blame 
                                                                      belongs to those who fail in their duty through weakness of will, which is the same as saying through shrinking from toil and pain. These cases are perfectly simple and easy to distinguish. In a free hour, 
                                                                      when our power of choice is untrammelled and when nothing prevents our being able to do what we like best, every pleasure is to be welcomed and every pain avoided. But in certain circumstances and owing to 
                                                                      the claims of duty or the obligations of business it will frequently occur that pleasures have to be repudiated and annoyances accepted. The wise man therefore always holds in these matters to this principle 
                                                                      of selection: he rejects pleasures to secure other greater pleasures, or else he endures pains to avoid worse pains.",
                                                }
                                            }
                                        },
                                      new ComponenteCurricularPlanejamentoDiarioDto()
                                        {
                                            Nome = "REGÊNCIA INFANTIL EMEI 4H",
                                            PlanejamentoDiarioInfantil = new List<PlanejamentoDiarioInfantilDto>()
                                            {
                                                new PlanejamentoDiarioInfantilDto
                                                {
                                                    AulaId  = 3023,
                                                    AulaCJ  = false,
                                                    DataAula  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    PlanejamentoRealizado  = true,
                                                    DateRegistro  = DateTime.Now.ToString("dd/MM/yyyy"),
                                                    Usuario  = "Levi Gael Kevin Ribeiro",
                                                    SecoesPreenchidas  = "- Planejamento",
                                                    Planejamento  = @"At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati 
                                                                      cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. 
                                                                      Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. 
                                                                      Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente 
                                                                      delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.1914 translation by H. Rackham On the other hand, we denounce with righteous indignation 
                                                                      and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee the pain and trouble that are bound to ensue; and equal blame 
                                                                      belongs to those who fail in their duty through weakness of will, which is the same as saying through shrinking from toil and pain. These cases are perfectly simple and easy to distinguish. In a free hour, 
                                                                      when our power of choice is untrammelled and when nothing prevents our being able to do what we like best, every pleasure is to be welcomed and every pain avoided. But in certain circumstances and owing to 
                                                                      the claims of duty or the obligations of business it will frequently occur that pleasures have to be repudiated and annoyances accepted. The wise man therefore always holds in these matters to this principle 
                                                                      of selection: he rejects pleasures to secure other greater pleasures, or else he endures pains to avoid worse pains.",
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
            };
            return View("RelatorioControlePlanejamentoDiarioInfantilComComponente", retorno);
        }

        [HttpGet("relatorio-devolutivas")]
        public IActionResult RelatorioDevolutivas()
        {
            var turmas = new List<TurmasDevolutivasDto>();
            var devolutivas1 = new List<DevolutivaRelatorioDto>();
            var devolutivas2 = new List<DevolutivaRelatorioDto>();

            for (var i = 0; i < 3; i++)
            {
                var DiasIntervalos1 = new List<String>();
                for (var j = 0; j < 3; j++)
                {
                    DiasIntervalos1.Add(DateTime.Now.AddDays(j - 1).ToString("dd/MM"));
                }

                DevolutivaRelatorioDto valoresDevolutivas1 = new DevolutivaRelatorioDto()
                {
                    IntervaloDatas = DateTime.Now.AddDays(i).ToString("dd/MM/yyyy") + " até " + DateTime.Now.AddDays(i + 5).ToString("dd/MM/yyyy"),
                    DiasIntervalo = String.Join(", ", DiasIntervalos1.ToArray()),
                    DataRegistro = DateTime.Now.AddDays(i + 10).ToString("dd/MM/yyyy"),
                    ResgistradoPor = "REGINA DA SILVA CAVALCANTE (2547458)",
                    Descricao = "77777777777777777777777777777777777777777777777777777777777777778888888888888888888888888888899999999999999999999999999999999999999999998888888888888888888888888888999999999999999999999997777777777777777777798888888"
                };
                devolutivas1.Add(valoresDevolutivas1);
            }

            for (var i = 0; i < 6; i++)
            {
                var DiasIntervalos2 = new List<String>();
                for (var j = 0; j < 4; j++)
                {
                    DiasIntervalos2.Add(DateTime.Now.AddDays(j - 20).ToString("dd/MM"));
                }

                DevolutivaRelatorioDto valoresDevolutivas2 = new DevolutivaRelatorioDto()
                {
                    IntervaloDatas = DateTime.Now.AddDays(i).ToString("dd/MM/yyyy") + " até " + DateTime.Now.AddDays(i - 20).ToString("dd/MM/yyyy"),
                    DiasIntervalo = String.Join(", ", DiasIntervalos2.ToArray()),
                    DataRegistro = DateTime.Now.AddDays(i - 10).ToString("dd/MM/yyyy"),
                    ResgistradoPor = "REGINA DA SILVA CAVALCANTE (2547458)",
                    Descricao = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled. "
                };
                devolutivas2.Add(valoresDevolutivas2);
            }

            for (var i = 0; i < 1; i++)
            {

                var turmasDevolutivasDto1 = new TurmasDevolutivasDto()
                {
                    NomeTurma = i + 1 + "A",
                    BimestresComponentesCurriculares = new List<BimestresComponentesCurricularesDevolutivasDto>()
                    {
                        new BimestresComponentesCurricularesDevolutivasDto()
                        {
                            NomeBimestreComponenteCurricular = "1º Bimestre (02/02/2020 à 29/04/2020)",
                            Devolutivas = devolutivas1
                        },

                    }
                };
                turmas.Add(turmasDevolutivasDto1);
            }

            for (var i = 0; i < 1; i++)
            {

                var turmasDevolutivasDto2 = new TurmasDevolutivasDto()
                {
                    NomeTurma = i + 1 + "B",
                    BimestresComponentesCurriculares = new List<BimestresComponentesCurricularesDevolutivasDto>()
                    {
                        new BimestresComponentesCurricularesDevolutivasDto()
                        {
                            NomeBimestreComponenteCurricular = "1º Bimestre (02/02/2020 à 29/04/2020)",
                            Devolutivas = devolutivas2
                        },
                        // new BimestresDevolutivasDto()
                        //{
                        //    NomeBimestre = "2º Bimestre (02/02/2020 à 29/04/2020)",
                        //    Devolutivas = devolutivas1
                        //}
                    }
                };
                turmas.Add(turmasDevolutivasDto2);
            }

            var model = new RelatorioDevolutivasDto()
            {
                Dre = "DRE - BT",
                Ue = "CEU EMEF BUTANTA",
                Turma = "Todas",
                Bimestre = "Todos",
                Usuario = "Anala Ferreira de Oliveira",
                RF = "9879878",
                //DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                ExibeConteudoDevolutivas = true,
                Turmas = turmas
            };

            return View("RelatorioDevolutivas", model);
        }

        [HttpGet("relatorio-registro-itinerancia")]
        public IActionResult RelatorioRegistroItinerancia()
        {
            var registros = new List<RegistrosRegistroItineranciaDto>();
            var objetivos1 = new List<ObjetivosRegistroItineranciaDto>();

            for (var i = 0; i < 1; i++)
            {
                var objetivo1 = new ObjetivosRegistroItineranciaDto()
                {
                    NomeObjetivo = "Mapeamento dos estudantes público da educação especial " + i,
                };
                var objetivo2 = new ObjetivosRegistroItineranciaDto()
                {
                    NomeObjetivo = "Reunião - Discussão sobre melhorias da SRM " + i,
                };
                objetivos1.Add(objetivo1);
                objetivos1.Add(objetivo2);
            }

            var alunos1 = new List<AlunoRegistroItineranciaDto>();

            for (var i = 0; i < 4; i++)
            {
                var aluno1 = new AlunoRegistroItineranciaDto()
                {
                    Estudante = "ALANA FERREIRA DE OLIVEIRA (1234567) - EF-" + i + "A",
                    DescritivoEstudante = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                    AcompanhamentoSituacao = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                    Encaminhamentos = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                };
                var aluno2 = new AlunoRegistroItineranciaDto()
                {
                    Estudante = "FERNANDO DOS SANTOS (1234567) - EF-" + i + "B",
                    DescritivoEstudante = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                    AcompanhamentoSituacao = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                    Encaminhamentos = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                };
                alunos1.Add(aluno1);
                alunos1.Add(aluno2);
            }

            RegistrosRegistroItineranciaDto registro1 = new RegistrosRegistroItineranciaDto()
            {
                Dre = "DRE - BT",
                Ue = "CEU EMEF BUTANTA",
                DataVisita = DateTime.Now.ToString("dd/MM/yyyy"),
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy"),
                Objetivos = objetivos1,
                Alunos = alunos1,
            };

            RegistrosRegistroItineranciaDto registro2 = new RegistrosRegistroItineranciaDto()
            {
                Dre = "DRE - BT",
                Ue = "CEU EMEF BUTANTA",
                DataVisita = DateTime.Now.ToString("dd/MM/yyyy"),
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy"),
                Objetivos = objetivos1,
                AcompanhamentoSituacao = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                Encaminhamentos = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
            };
            registros.Add(registro1);
            registros.Add(registro2);

            var model = new RelatorioRegistroItineranciaDto()
            {
                Usuario = "Catia Pereira de Souza",
                RF = "9879878",
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Registros = registros
            };

            return View("RelatorioRegistroItinerancia", model);
        }

        [HttpGet("relatorio-ata-final")]
        public async Task<IActionResult> RelatorioAtaFinal()
        {

            var filtro = new FiltroConselhoClasseAtaFinalDto()
            {
                AnoLetivo = 2021,
                TurmasCodigos = new[] { "2317820" },
                Visualizacao = AtaFinalTipoVisualizacao.Estudantes
            };

            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = await mediator.Send(new ObterRelatorioConselhoClasseAtaFinalPdfQuery(filtro));

            //var rel = relatoriosTurmas.Where(a => a.GruposMatriz)
            return View("RelatorioAtasComColunaFinal", relatoriosTurmas[2]);
        }

        [HttpGet("registro-individual")]
        public async Task<IActionResult> RegistroIndividual([FromServices] IRelatorioRegistroIndividualUseCase useCase)
        {
            var mensagem = JsonConvert.SerializeObject(new FiltroRelatorioRegistroIndividualDto() { TurmaId = 615813, DataInicio = DateTime.Now.AddDays(-90), DataFim = DateTime.Now, UsuarioNome = "ALANA FERREIRA DE OLIVEIRA", UsuarioRF = "1234567" }, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await useCase.Executar(new FiltroRelatorioDto() { Mensagem = mensagem, CodigoCorrelacao = Guid.NewGuid() });

            return default;
        }

        [HttpGet("acompanhamento-aprendizagem")]
        public async Task<IActionResult> AcompanhamentoAprendizagem([FromServices] IRelatorioAcompanhamentoAprendizagemUseCase useCase)
        {
            try
            {
                var mensagem = JsonConvert.SerializeObject(new FiltroRelatorioAcompanhamentoAprendizagemDto() { Semestre = 1, TurmaId = 615813 }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                await useCase.Executar(new FiltroRelatorioDto() { Mensagem = mensagem });
                //  var model = await mediator.Send(new ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(615822, "6731135", 1));
                return View("RelatorioAcompanhamentoAprendizagem", null);
            }
            catch (Exception ex)
            {
                throw ex;
                throw ex;
            }
        }

        [HttpGet("boletim-escolar-detalhado")]
        public async Task<IActionResult> RelatorioBoletimEscolarDetalhado([FromServices] IRelatorioAcompanhamentoAprendizagemUseCase useCase)
        {

            var boletimEscolarDetalhadoDto = new BoletimEscolarDetalhadoDto();

            var aluno01 = new BoletimEscolarDetalhadoAlunoDto()
            {
                Cabecalho = new BoletimEscolarDetalhadoCabecalhoDto()
                {
                    NomeDre = "DIRETORIA REGIONAL DE EDUCAÇÃO CAMPO LIMPO",
                    NomeUe = "CEU EMEF PARAISOPOLIS",
                    NomeTurma = "EM-3A",
                    Aluno = "Emerson Ferreira e Silva",
                    CodigoEol = "1234567",
                    Data = "01/06/2021",
                    FrequenciaGlobal = "100%",
                    Foto = "https://via.placeholder.com/80",
                    Ciclo = "Médio"
                },
                ParecerConclusivo = "Retido",
                RecomendacoesEstudante = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
                eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
                blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
                vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
                feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
                nibh sed pulvinar. Ut consequat semper viverra nam libero justo
                laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
                lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
                Duis at tellus at urna condimentum mattis.",

                RecomendacoesFamilia = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
                eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
                blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
                vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
                feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
                nibh sed pulvinar. Ut consequat semper viverra nam libero justo
                laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
                lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
                Duis at tellus at urna condimentum mattis.",
            };
            boletimEscolarDetalhadoDto.Boletins.Add(aluno01);

            var aluno02 = new BoletimEscolarDetalhadoAlunoDto()
            {
                Cabecalho = new BoletimEscolarDetalhadoCabecalhoDto()
                {
                    NomeDre = "DIRETORIA REGIONAL DE EDUCAÇÃO CAMPO LIMPO",
                    NomeUe = "CEU EMEF PARAISOPOLIS",
                    NomeTurma = "EM-3A",
                    Aluno = "Maria Ferreira e Silva",
                    CodigoEol = "1234568",
                    Data = "01/06/2021",
                    FrequenciaGlobal = "100%",
                    Foto = "https://via.placeholder.com/80",
                    Ciclo = "Médio"
                },

                ParecerConclusivo = "",
                RecomendacoesEstudante = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
                eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
                blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
                vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
                feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
                nibh sed pulvinar. Ut consequat semper viverra nam libero justo
                laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
                lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
                Duis at tellus at urna condimentum mattis.",

                RecomendacoesFamilia = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
                eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
                blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
                vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
                feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
                nibh sed pulvinar. Ut consequat semper viverra nam libero justo
                laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
                lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
                Duis at tellus at urna condimentum mattis.",
            };
            boletimEscolarDetalhadoDto.Boletins.Add(aluno02);


            var model = new BoletimEscolarDetalhadoEscolaAquiDto(boletimEscolarDetalhadoDto);

            return View("RelatorioBoletimEscolarDetalhado", model);

        }

        //[HttpGet("boletim-escolar-detalhado")]
        //public async Task<IActionResult> RelatorioBoletimEscolarDetalhado([FromServices] IRelatorioAcompanhamentoAprendizagemUseCase useCase)
        //{

        //    var boletimEscolarDetalhadoDto = new BoletimEscolarDetalhadoDto();

        //    var aluno01 = new BoletimEscolarDetalhadoAlunoDto()
        //    {
        //        Cabecalho = new BoletimEscolarDetalhadoCabecalhoDto()
        //        {
        //            NomeDre = "DIRETORIA REGIONAL DE EDUCAÇÃO CAMPO LIMPO",
        //            NomeUe = "CEU EMEF PARAISOPOLIS",
        //            NomeTurma = "EM-3A",
        //            Aluno = "Emerson Ferreira e Silva",
        //            CodigoEol = "1234567",
        //            Data = "01/06/2021",
        //            FrequenciaGlobal = "100%",
        //            Foto = "https://via.placeholder.com/80",
        //            Ciclo = "Médio"
        //        },
        //        ParecerConclusivo = "Retido",
        //        RecomendacoesEstudante = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
        //        eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
        //        blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
        //        vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
        //        feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
        //        nibh sed pulvinar. Ut consequat semper viverra nam libero justo
        //        laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
        //        lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
        //        Duis at tellus at urna condimentum mattis.",

        //        RecomendacoesFamilia = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
        //        eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
        //        blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
        //        vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
        //        feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
        //        nibh sed pulvinar. Ut consequat semper viverra nam libero justo
        //        laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
        //        lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
        //        Duis at tellus at urna condimentum mattis.",
        //    };
        //    boletimEscolarDetalhadoDto.Boletins.Add(aluno01);

        //    var aluno02 = new BoletimEscolarDetalhadoAlunoDto()
        //    {
        //        Cabecalho = new BoletimEscolarDetalhadoCabecalhoDto()
        //        {
        //            NomeDre = "DIRETORIA REGIONAL DE EDUCAÇÃO CAMPO LIMPO",
        //            NomeUe = "CEU EMEF PARAISOPOLIS",
        //            NomeTurma = "EM-3A",
        //            Aluno = "Maria Ferreira e Silva",
        //            CodigoEol = "1234568",
        //            Data = "01/06/2021",
        //            FrequenciaGlobal = "100%",
        //            Foto = "https://via.placeholder.com/80",
        //            Ciclo = "Médio"
        //        },

        //        ParecerConclusivo = "",
        //        RecomendacoesEstudante = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
        //        eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
        //        blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
        //        vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
        //        feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
        //        nibh sed pulvinar. Ut consequat semper viverra nam libero justo
        //        laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
        //        lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
        //        Duis at tellus at urna condimentum mattis.",

        //        RecomendacoesFamilia = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
        //        eiusmod tempor incididunt ut labore et dolore magna aliqua. Non
        //        blandit massa enim nec dui nunc mattis enim ut. Nunc mi ipsum faucibus
        //        vitae aliquet. Semper quis lectus nulla at volutpat diam. Molestie ac
        //        feugiat sed lectus vestibulum. Nec tincidunt praesent semper feugiat
        //        nibh sed pulvinar. Ut consequat semper viverra nam libero justo
        //        laoreet sit amet. Est sit amet facilisis magna etiam tempor orci eu
        //        lobortis. Massa placerat duis ultricies lacus sed turpis tincidunt.
        //        Duis at tellus at urna condimentum mattis.",
        //    };
        //    boletimEscolarDetalhadoDto.Boletins.Add(aluno02);


        //    var model = new RelatorioBoletimEscolarDetalhadoDto(boletimEscolarDetalhadoDto);

        //    return View("RelatorioBoletimEscolarDetalhado", model);

        //}


        [HttpGet("acompanhamento-fechamento")]
        public async Task<IActionResult> AcompanhamentoFechamento([FromServices] IRelatorioAcompanhamentoFechamentoUseCase useCase)
        {
            try
            {
                var mensagem = JsonConvert.SerializeObject(new RelatorioAcompanhamentoFechamentoPorUeDto() { }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return default;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //    [HttpGet("acompanhamento-fechamento-ue")]
        //    public async Task<IActionResult> AcompanhamentoFechamentoConsolidadoUe([FromServices] IRelatorioAcompanhamentoFechamentoUseCase useCase)
        //    {
        //        var fechamento= new RelatorioConsolidadoFechamento()
        //        {
        //            NaoIniciado = 3,
        //            ProcessadoComPendencia = 2,
        //            ProcessadoComSucesso = 5
        //        };
        //        var conselhoDeClasse= new RelatorioConsolidadoConselhoClasse()
        //        {
        //            NaoIniciado = 4,
        //            EmAndamento = 8,
        //            Concluido = 7,
        //        };

        //        var fechamentoConselhoClasseConsolidados = new List<RelatorioAcompanhamentoFechamentoConselhoClasseDto>();
        //        var fechamentoConselhoClasseConsolidado = new RelatorioAcompanhamentoFechamentoConselhoClasseDto("EF -1A")
        //        {
        //            FechamentoConsolidado = fechamento,
        //            ConselhoDeClasseConsolidado =  conselhoDeClasse ,
        //        };
        //        fechamentoConselhoClasseConsolidados.Add(fechamentoConselhoClasseConsolidado);

        //        var bimestres = new List<RelatorioAcompanhamentoFechamentoBimestresDto>();
        //        var bimestre = new RelatorioAcompanhamentoFechamentoBimestresDto("1º Bimestre","0123")
        //        {                
        //            FechamentoConselhoClasseConsolidado = fechamentoConselhoClasseConsolidados
        //        };
        //        bimestres.Add(bimestre);            

        //        var ues = new List<RelatorioAcompanhamentoFechamentoUesDto>();
        //        var ue = new RelatorioAcompanhamentoFechamentoUesDto("CEU EMEF BUTANTA")
        //        {
        //           Bimestres = bimestres,
        //        };
        //        ues.Add(ue);

        //        var model = new RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto()
        //        {
        //            DreNome = "DRE-BT",
        //            UeNome = "TODAS",
        //            Turma = "TODAS",
        //            Bimestre = "TODOS",
        //            Usuario = "JULIA FERREIRA DE OLIVEIRA",
        //            RF = "1234567",
        //            Data = "13/09/2021",
        //            Ues = ues,
        //        };

        //        return View("RelatorioAcompanhamentoFechamentoConsolidadoPorUe", model);
        //    }

        [HttpGet("frequencia-individual")]
        public async Task<IActionResult> FrequenciaIndividual()
        {
            var justificativas = new List<RelatorioFrequenciaIndividualJustificativasDto>();
            for (var i = 0; i < 34; i++)
            {
                var justificativa = new RelatorioFrequenciaIndividualJustificativasDto()
                {
                    DataAusencia = "21/01/2020",
                    MotivoAusencia = "Atestado médico da criança " + (i + 1),
                    //MotivoAusencia = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. " + (i + 1),
                };
                justificativas.Add(justificativa);
            };

            var dadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto()
            {
                TotalAulasDadas = 30,
                TotalPresencas = 20,
                TotalRemoto = 5,
                TotalAusencias = 3,
                TotalCompensacoes = 2,
                TotalPercentualFrequenciaFormatado = "96,67%",
            };


            var bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
            for (var i = 0; i < 2; i++)
            {
                var bimestre = new RelatorioFrequenciaIndividualBimestresDto()
                {
                    NomeBimestre = (i + 1) + "º bimestre - 2020",
                    DadosFrequencia = dadosFrequencia,
                    Justificativas = justificativas,
                };
                bimestres.Add(bimestre);
            }

            var alunos = new List<RelatorioFrequenciaIndividualAlunosDto>();

            for (var i = 0; i < 10; i++)
            {
                var aluno = new RelatorioFrequenciaIndividualAlunosDto()
                {
                    NomeAluno = "Antônio CarLos dos santos " + (i + 1) + " (1234567)",
                    Bimestres = bimestres,
                };
                alunos.Add(aluno);
            }

            var model = new RelatorioFrequenciaIndividualDto()
            {
                DreNome = "DRE - BT",
                UeNome = "CEU EMEF BUTANTA",
                Usuario = "JULIA FERREIRA DE OLIVEIRA",
                RF = "1234567",
                ehInfantil = true,
                Alunos = alunos,
            };

            return View("RelatorioFrequenciaIndividual", model);
        }

        [HttpGet("registro-ocorrencias")]
        public async Task<IActionResult> RegistroOcorrencias()
        {
            var ocorrencias = new List<RelatorioOcorrenciasDto>();

            for (var i = 0; i < 3; i++)
            {
                var ocorrencia = new RelatorioOcorrenciasDto()
                {
                    CriancaNome = "ALANA FERREIRA DE OLIVEIRA (12345678) " + (i + 1),
                    Turma = "EI - " + (i + 1) + "A",
                    DataOcorrencia = "19/12/2020",
                    TipoOcorrencia = "Brica com colega de sala",
                    TituloOcorrencia = "Atrito com o coleaga após discussão",
                    DescricaoOcorrencia = "Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially(5;7) unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
                };
                ocorrencias.Add(ocorrencia);
            }

            var model = new RelatorioRegistroOcorrenciasDto()
            {
                DreNome = "DRE - BT",
                UeNome = "CEU EMEF BUTANTA",
                Endereco = "Av. Eng. Heitor Antônio Eiras García, 1870 - Jardim Esmeralda",
                Contato = "(11) 3732 - 4520",
                UsuarioNome = "ELIENE SOUZA MATOS",
                UsuarioRF = "1234567",
                Ocorrencias = ocorrencias,
            };
            return View("RelatorioRegistroOcorrencias", model);
        }

        [HttpGet("sondagem-componentes-aditivos-paginado")]
        public IActionResult SondagemComponentesAditivosPaginado()
        {
            var relatorio = new CriadorDeMockRelatorioPaginadoSondagemTurmaModel();
            var preparo = new PreparadorDeRelatorioPaginadoSondagemPorTurmaMatematica(relatorio.ObtenhaSondagemComponente());
            var dto = preparo.ObtenhaRelatorioPaginadoDto();

            return View("RelatorioPaginado/Index", dto);
        }
    }
}

