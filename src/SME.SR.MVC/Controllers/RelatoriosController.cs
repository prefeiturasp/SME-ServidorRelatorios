using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Application;
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
            //model.Dres.Add(new RelatorioFaltaFrequenciaDreDto
            //{
            //    CodigoDre = "123",
            //    NomeDre = "DRE 01",
            //    Ues = new List<RelatorioFaltaFrequenciaUeDto>
            //    {
            //        new RelatorioFaltaFrequenciaUeDto
            //        {
            //            NomeUe="UE 01",
            //            CodigoUe="456",
            //            Anos= new List<RelatorioFaltaFrequenciaAnoDto>
            //            {
            //                new RelatorioFaltaFrequenciaAnoDto
            //                {
            //                    NomeAno="1º ano",
            //                    Bimestres= new List<RelatorioFaltaFrequenciaBimestreDto>
            //                    {
            //                        new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="1º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 01",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 02",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                 new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 03",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 04",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 05",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                            }
            //                        },
            //                        new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="2º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        },
            //                            new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="3º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        },
            //                                new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="4º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }

            //                    }
            //                },
            //                new RelatorioFaltaFrequenciaAnoDto
            //                {
            //                    NomeAno="2º ano",
            //                    Bimestres= new List<RelatorioFaltaFrequenciaBimestreDto>
            //                    {
            //                        new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="1º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 01",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 02",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                 new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 03",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 04",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português 05",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                            }
            //                        },
            //                        new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="2º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        },
            //                            new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="3º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        },
            //                                new RelatorioFaltaFrequenciaBimestreDto
            //                        {
            //                            NomeBimestre="4º Bim",
            //                            Componentes= new List<RelatorioFaltaFrequenciaComponenteDto>
            //                            {
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Matemática",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                },
            //                                new RelatorioFaltaFrequenciaComponenteDto
            //                                {
            //                                    NomeComponente="Português",
            //                                    Alunos= new List<RelatorioFaltaFrequenciaAlunoDto>
            //                                    {
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        },
            //                                        new RelatorioFaltaFrequenciaAlunoDto
            //                                        {
            //                                            NomeAluno="José da silva",
            //                                            Faltas="10",
            //                                            Frequencia=50,
            //                                            NomeTurma="1A",
            //                                            Numero="1"
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }

            //                    }
            //                },
            //            }
            //        }
            //    }
            //});

            return View(model);
        }

        [HttpGet("fechamentos-pendencias")]
        public IActionResult RelatorioFechamentoPendencia([FromServices] IMediator mediator)
        {
            RelatorioFechamentoPendenciasDto model = GeraVariasPendencias2Componentes2Turmas();

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

        //[HttpGet("sondagem-consolidado-matematica-numeros")]
        //public IActionResult RelatorioSondagemConsolidadoMatematicaNumeros()
        //{

        //    var model = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto();
        //    model.Dre = "DRE-JT";
        //    model.Ue = "EMEF - Máximo de Moura";
        //    model.AnoLetivo = 2020;
        //    model.Ano = "9";
        //    model.Turma = "Todas";
        //    model.ComponenteCurricular = "Matemática";
        //    model.Proficiencia = "Números";
        //    model.Periodo = "1º semestre";
        //    model.Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque";
        //    model.RF = "7777710";
        //    model.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
        //    Random randNum = new Random();

        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    { 
        //        Pergunta = "Familiares ou frequentes", Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() { 
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 82, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 18, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        } 
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Opacos",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 73, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 27, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Transparentes",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 56, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 44, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Terminam em zero",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 53, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 47, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Algarismos iguais",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 50, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 50, AlunosQuantidade = randNum.Next(99999) , Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Processo de generalização",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 43, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 57, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Zero intercalado",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 33, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 67, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Zero intercalado1",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 33, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 67, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Zero intercalado2",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 33, AlunosQuantidade = randNum.Next(99999), Resposta = "Escreve convencionalmente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 67, AlunosQuantidade = randNum.Next(99999), Resposta = "Não escreve convencionalmente" },
        //        }
        //    });

        //    return View("RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidado", model);
        //}

        //[HttpGet("sondagem-consolidado-matematica-autoral")]
        //public IActionResult RelatorioSondagemConsolidadoMatematicaAutoral()
        //{

        //    var model = new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto();
        //    model.Dre = "DRE-JT";
        //    model.Ue = "EMEF - Máximo de Moura";
        //    model.AnoLetivo = 2020;
        //    model.Ano = "9";
        //    model.Turma = "Todas";
        //    model.ComponenteCurricular = "Matemática";
        //    model.Proficiencia = "";
        //    model.Periodo = "1º semestre";
        //    model.Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque";
        //    model.RF = "7777710";
        //    model.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
        //    Random randNum = new Random();

        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Problema de lógica",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 20, AlunosQuantidade = randNum.Next(99999), Resposta = "Resolveu corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 15, AlunosQuantidade = randNum.Next(99999), Resposta = "Resolveu uma parte do problema corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 25, AlunosQuantidade = randNum.Next(99999), Resposta = "Não registrou" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 40, AlunosQuantidade = randNum.Next(99999), Resposta = "Sem preenchimento" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Área e perímetro",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Resolveu corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 23, AlunosQuantidade = randNum.Next(99999), Resposta = "Compreende o que é área, mas não compreende o que é perímetro" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 27, AlunosQuantidade = randNum.Next(99999), Resposta = "Compreende o que é perímetro, mas não compreende o que é área" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 25, AlunosQuantidade = randNum.Next(99999), Resposta = "Não registrou" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 15, AlunosQuantidade = randNum.Next(99999), Resposta = "Sem preenchimento" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Sólidos geométricos",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Resolveu corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 26, AlunosQuantidade = randNum.Next(99999), Resposta = "Identificou os nomes das figuras e não determinou elementos de poliedros corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 24, AlunosQuantidade = randNum.Next(99999), Resposta = "Não identificou nomes de figuras e não determinou elementos de poliedros corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 25, AlunosQuantidade = randNum.Next(99999), Resposta = "Não registrou" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 15, AlunosQuantidade = randNum.Next(99999), Resposta = "Sem preenchimento" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Regularidade e generalização",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Resolveu corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 23, AlunosQuantidade = randNum.Next(99999), Resposta = "Percebeu a regularidade, mas não expressou a generalização por meio de uma expressão algébrica" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 32, AlunosQuantidade = randNum.Next(99999), Resposta = "Não percebeu a regularidade e nem expressou a generalização por meio de uma expressão algébrica" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 20, AlunosQuantidade = randNum.Next(99999), Resposta = "Não registrou" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 15, AlunosQuantidade = 32275, Resposta = "Sem preenchimento" },
        //        }
        //    });
        //    model.PerguntasRespostas.Add(new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        //    {
        //        Pergunta = "Probabilidade",
        //        Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>() {
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 10, AlunosQuantidade = randNum.Next(99999), Resposta = "Resolveu corretamente" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 30, AlunosQuantidade = randNum.Next(99999), Resposta = "Representou corretamente a probabilidade na forma fracionária, mas errou as formas decimal e/ou percentual" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 15, AlunosQuantidade = randNum.Next(99999) , Resposta = "Não identificou a probabilidade" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 25, AlunosQuantidade = randNum.Next(99999), Resposta = "Não registrou" },
        //            new RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto() { AlunosPercentual = 20, AlunosQuantidade = randNum.Next(99999), Resposta = "Sem preenchimento" },
        //        }
        //    });

        //    return View("RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidado", model);
        //}

        [HttpGet("plano-aula")]
        public IActionResult RelatorioPlanoAula()

        {

            var model = new PlanoAulaDto()
            {
                DataPlanoAula = DateTime.Now,
                Id = 1,
                Descricao = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a purus consectetur ante tristique fringilla id
ut purus. Pellentesque lobortis eu sem facilisis ullamcorper. Integer congue ante et nibh aliquet gravida. Donec
accumsan nisi pulvinar dignissim molestie. Suspendisse a libero suscipit, pharetra sem semper, sagittis
turpis. Sed vulputate vel lacus in commodo. Pellentesque nisi quam, consectetur in eros ac, semper porta velit.
Donec sapien ante, commodo in neque eu, faucibus tincidunt erat. Duis a felis aliquet, vulputate lectus vitae,
elementum metus. Sed massa nulla, pretium euismod massa eu, volutpat auctor mi.
Mauris vestibulum dictum odio a auctor. Fusce ullamcorper, nibh sed sollicitudin porta, lectus velit gravida
tellus, vel pellentesque metus quam at magna. Sed laoreet metus massa, et sollicitudin lacus elementum vitae.
Vestibulum in quam tincidunt, vestibulum eros non, imperdiet justo. Aenean suscipit felis ipsum, sit amet
vulputate metus sollicitudin non. Curabitur a dapibus nibh. Nullam non lorem a felis mattis bibendum. Vivamus
sit amet posuere orci, a sodales ipsum. Curabitur viverra euismod urna.
Ut sed porttitor eros. Nullam eget convallis mi. Nam luctus erat a sem malesuada auctor. Aliquam nec pulvinar
risus. Nullam tincidunt maximus lectus nec dignissim. Nunc porta dolor quis nisl imperdiet cursus. Aliquam
convallis, dui a aliquam bibendum, nunc nisi commodo ipsum, quis vestibulum lacus risus non nisl. Quisque in
sapien neque. Suspendisse potenti.
Nullam id nisl vel ipsum ultrices rutrum. Curabitur consequat tempor nunc, a condimentum eros iaculis ac.
Integer risus lorem, commodo non felis euismod, finibus ultrices libero. Duis posuere magna ante, id auctor
turpis pulvinar molestie. Morbi mattis purus eget turpis imperdiet pulvinar. Quisque vehicula euismod justo quis
ullamcorper. In hac habitasse platea dictumst. Pellentesque quis elementum dolor, in sagittis neque.
Maecenas blandit tristique vestibulum.
Aliquam rhoncus dui odio, id posuere ante ullamcorper in. Nam odio libero, pharetra vitae interdum non,
fringilla ut sem. Sed aliquam urna tortor, eu congue justo semper in. Nullam enim nisl, laoreet quis arcu quis,
semper dignissim tortor. Phasellus sit amet massa ullamcorper, iaculis diam vel, vulputate sem. Quisque quis
massa ut risus congue maximus at vitae leo. Etiam scelerisque lectus a tempor efficitur",
                DesenvolvimentoAula = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a purus consectetur ante tristique fringilla id
ut purus. Pellentesque lobortis eu sem facilisis ullamcorper. Integer congue ante et nibh aliquet gravida. Donec
accumsan nisi pulvinar dignissim molestie. Suspendisse a libero suscipit, pharetra sem semper, sagittis
turpis. Sed vulputate vel lacus in commodo. Pellentesque nisi quam, consectetur in eros ac, semper porta velit.
Donec sapien ante, commodo in neque eu, faucibus tincidunt erat. Duis a felis aliquet, vulputate lectus vitae,
elementum metus. Sed massa nulla, pretium euismod massa eu, volutpat auctor mi.
Mauris vestibulum dictum odio a auctor. Fusce ullamcorper, nibh sed sollicitudin porta, lectus velit gravida
tellus, vel pellentesque metus quam at magna. Sed laoreet metus massa, et sollicitudin lacus elementum vitae.
Vestibulum in quam tincidunt, vestibulum eros non, imperdiet justo. Aenean suscipit felis ipsum, sit amet
vulputate metus sollicitudin non. Curabitur a dapibus nibh. Nullam non lorem a felis mattis bibendum. Vivamus
sit amet posuere orci, a sodales ipsum. Curabitur viverra euismod urna.
Ut sed porttitor eros. Nullam eget convallis mi. Nam luctus erat a sem malesuada auctor. Aliquam nec pulvinar
risus. Nullam tincidunt maximus lectus nec dignissim. Nunc porta dolor quis nisl imperdiet cursus. Aliquam
convallis, dui a aliquam bibendum, nunc nisi commodo ipsum, quis vestibulum lacus risus non nisl. Quisque in
sapien neque. Suspendisse potenti.
Nullam id nisl vel ipsum ultrices rutrum. Curabitur consequat tempor nunc, a condimentum eros iaculis ac.
Integer risus lorem, commodo non felis euismod, finibus ultrices libero. Duis posuere magna ante, id auctor
turpis pulvinar molestie. Morbi mattis purus eget turpis imperdiet pulvinar. Quisque vehicula euismod justo quis
ullamcorper. In hac habitasse platea dictumst. Pellentesque quis elementum dolor, in sagittis neque.
Maecenas blandit tristique vestibulum.
Aliquam rhoncus dui odio, id posuere ante ullamcorper in. Nam odio libero, pharetra vitae interdum non,
fringilla ut sem. Sed aliquam urna tortor, eu congue justo semper in. Nullam enim nisl, laoreet quis arcu quis,
semper dignissim tortor. Phasellus sit amet massa ullamcorper, iaculis diam vel, vulputate sem. Quisque quis
massa ut risus congue maximus at vitae leo. Etiam scelerisque lectus a tempor efficitur.",
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
massa ut risus congue maximus at vitae leo.Etiam scelerisque lectus a tempor efficitur",
                Dre = "DRE 1",
                Ue = "UE 1",
                Turma = "1A",
                ComponenteCurricular = "3",
                Usuario = "Usuario X",
                RF = "2266334",
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

        [HttpGet("sondagem-componentes-numeros")]
        public IActionResult SondagemComponentesNumeros()
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
        public IActionResult RelatorioControleGradeSintetico()
        {

            var controleGrade = new ControleGradeSinteticoDto()
            {
                Filtro = new FiltroGradeSintetico()
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

            controleGrade.Turmas = new List<TurmaControleGradeSinteticoDto>()
            {
               new TurmaControleGradeSinteticoDto()
               {
                   Nome="1F",
                   Bimestres = new List<BimestreControleGradeSinteticoDto>()
                   {
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "1° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "2° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "3° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "4° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
             new TurmaControleGradeSinteticoDto()
               {
                   Nome="2F",
                   Bimestres = new List<BimestreControleGradeSinteticoDto>()
                   {
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "1° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
                               {
                                   Nome = "MATEMÁTICA",
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
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "2° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "3° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                       new BimestreControleGradeSinteticoDto()
                       {
                           Descricao = "4° BIMESTRE - 20/03 À 25/04",
                           ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>()
                           {
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                               new ComponenteCurricularControleGradeSinteticoDto()
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
                            OrdemId = 1,
                            Resposta = "Adequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Inadequada",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Adequada",
                            PerguntaId = 3
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Inadequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 3
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Inadequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Não resolveu",
                            PerguntaId = 3
                        },
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
                            OrdemId = 1,
                            Resposta = "Adequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Inadequada",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Adequada",
                            PerguntaId = 3
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Inadequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 3
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Inadequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Não resolveu",
                            PerguntaId = 3
                        },
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
                            OrdemId = 1,
                            Resposta = "Adequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Inadequada",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 1,
                            Resposta = "Adequada",
                            PerguntaId = 3
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Inadequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 2,
                            Resposta = "Não resolveu",
                            PerguntaId = 3
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Inadequada",
                            PerguntaId = 1
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Não resolveu",
                            PerguntaId = 2
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto() {
                            OrdemId = 3,
                            Resposta = "Não resolveu",
                            PerguntaId = 3
                        },
                    },
                });
            }

            var model = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto()
            {
                Cabecalho = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaCabecalhoDto()
                {
                    Ano = 5.ToString(),
                    AnoLetivo = 2020,
                    ComponenteCurricular = "Língua portuguesa",
                    DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                    Dre = "DRE - BT",
                    Periodo = "1º Bimestre",
                    Proficiencia = "Capacidade leitura",
                    Rf = "9879878",
                    Turma = "Todas",
                    Ue = "CEU EMEF BUTANTA",
                    Usuario = "Alice Gonçalves de Almeida Souza Nascimento da Silva Albuquerque",
                    Ordens = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
                        {
                            Id = 1,
                            Descricao = "ORDEM NO NARRAR"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
                        {
                            Id = 2,
                            Descricao = "ORDEM DO RELATAR"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
                        {
                            Id = 3,
                            Descricao = "ORDEM DO ARGUMENTAR"
                        },
                    },
                    Perguntas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto>()
                    {
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
                        {
                            Id = 1,
                            Nome = "Inferência"
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
                        {
                            Id = 2,
                            Nome = "Localização "
                        },
                        new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto()
                        {
                            Id = 3,
                            Nome = "Reflexão"
                        }
                    },
                },
                Planilha = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto()
                {
                    Linhas = linhas
                },
            };

            return View("RelatorioSondagemPortuguesCapacidadeLeituraPorTurma", model);
        }
    }
}
