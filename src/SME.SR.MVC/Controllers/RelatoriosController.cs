using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Application;
using SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.MVC.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(ILogger<RelatoriosController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("faltas-frequencia")]
        public async Task<IActionResult> RelatorioFaltasFrequencias([FromServices] IMediator mediator)
        {
            var model = await mediator.Send(new ObterRelatorioFaltasFrequenciaPdfQuery(new ObterRelatorioFaltasFrequenciasQuery()));
            //mock
            model.Dre = "DR JT";
            model.Ue = "UE EMEF MÁXIMO DE MOURA";
            model.Ano = "9";
            model.Bimestre = "2º";
            model.ComponenteCurricular = "Matemática";
            model.Usuario = "ADMIN";
            model.RF = "123123123";
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.Dres.Add(new RelatorioFaltaFrequenciaDreDto
            {
                Codigo = "123",
                Nome = "Nome da dre 123",
                Ues = new List<RelatorioFaltaFrequenciaUeDto>
                {
                    new RelatorioFaltaFrequenciaUeDto
                    {
                        Nome="Nome da ue 456",
                        Codigo="456",
                        Anos= new List<RelatorioFaltaFrequenciaAnoDto>
                        {
                            new RelatorioFaltaFrequenciaAnoDto
                            {
                                Nome="1º ano",
                                Bimestres= new List<RelatorioFaltaFrequenciaBimestreDto>
                                {
                                    new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="1º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 01",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 02",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                             new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 03",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 04",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 05",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                        }
                                    },
                                    new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="2º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                        new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="3º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                            new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="4º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
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
            });
            return View(model);
        }

          [HttpGet("fechamentos-pendencias")]
        public async Task<IActionResult> RelatorioFechamentoPendencia([FromServices] IMediator mediator)
        {
            var model = new RelatorioFaltasFrequenciaDto(); //await mediator.Send(new ObterRelatorioFaltasFrequenciaPdfQuery(new ObterRelatorioFaltasFrequenciasQuery()));
            //mock
            model.Dre = "DR JT";
            model.Ue = "UE EMEF MÁXIMO DE MOURA";
            model.Ano = "9";
            model.Bimestre = "2º";
            model.ComponenteCurricular = "Matemática";
            model.Usuario = "ADMIN";
            model.RF = "123123123";
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.Dres.Add(new RelatorioFaltaFrequenciaDreDto
            {
                Codigo = "123",
                Nome = "Nome da dre 123",
                Ues = new List<RelatorioFaltaFrequenciaUeDto>
                {
                    new RelatorioFaltaFrequenciaUeDto
                    {
                        Nome="Nome da ue 456",
                        Codigo="456",
                        Anos= new List<RelatorioFaltaFrequenciaAnoDto>
                        {
                            new RelatorioFaltaFrequenciaAnoDto
                            {
                                Nome="1º ano",
                                Bimestres= new List<RelatorioFaltaFrequenciaBimestreDto>
                                {
                                    new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="1º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 01",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 02",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                             new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 03",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 04",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português 05",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                        }
                                    },
                                    new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="2º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                        new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="3º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                            new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="4º",
                                        Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
                                        {
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Matemática",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    }
                                                }
                                            },
                                            new RelatorioFaltaFrequenciaComponenteDto
                                            {
                                                Nome="Português",
                                                Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
                                                {
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
                                                    },
                                                    new RelatorioFaltaFrequenciaAlunoDto
                                                    {
                                                        Nome="José da silva",
                                                        Faltas="10",
                                                        Frequencia=50,
                                                        NomeTurma="1A",
                                                        Numero="1"
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
            });
            return View("RelatorioFechamentoPendencia", model);
        }

    }
}
