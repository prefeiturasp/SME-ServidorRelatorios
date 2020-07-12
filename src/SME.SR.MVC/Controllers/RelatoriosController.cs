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
            var model = await mediator.Send(new ObterRelatorioFaltasFrequenciaPdfQuery(new FiltroRelatorioFaltasFrequenciasDto()));
            //mock
            model.ExibeFaltas = true;
            model.ExibeFrequencia = false;
            model.Dre = "DRE 01";
            model.Ue = "UE EMEF MÁXIMO DE MOURA 01";
            model.Ano = "001";
            model.Bimestre = "1º";
            model.ComponenteCurricular = "Matemática";
            model.Usuario = "ADMIN";
            model.Modalidade = "Fundamental";
            model.RF = "123123123";
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.Dres.Add(new RelatorioFaltaFrequenciaDreDto
            {
                Codigo = "123",
                Nome = "DRE 01",
                Ues = new List<RelatorioFaltaFrequenciaUeDto>
                {
                    new RelatorioFaltaFrequenciaUeDto
                    {
                        Nome="UE 01",
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
                                        Nome="1º Bim",
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
                                        Nome="2º Bim",
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
                                        Nome="3º Bim",
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
                                        Nome="4º Bim",
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
                            },
                            new RelatorioFaltaFrequenciaAnoDto
                            {
                                Nome="2º ano",
                                Bimestres= new List<RelatorioFaltaFrequenciaBimestreDto>
                                {
                                    new RelatorioFaltaFrequenciaBimestreDto
                                    {
                                        Nome="1º Bim",
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
                                        Nome="2º Bim",
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
                                        Nome="3º Bim",
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
                                        Nome="4º Bim",
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
                            },
                        }
                    }
                }
            });
            return View(model);
        }
    }
}
