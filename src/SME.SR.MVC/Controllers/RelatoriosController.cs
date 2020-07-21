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

            var model = new RelatorioFechamentoPendenciasDto();
            model.Cabecalho = new RelatorioFechamentoPendenciaCabecalhoDto()
            {
                AnoLetivo = "2020",
                Bimestre = "1",
                Data = DateTime.Today.ToString("dd/MM/yyyy"),
                Modalidade = Modalidade.Fundamental.ToString(),
                NomeComponenteCurricular = "Matemática",
                NomeDre = "Nome da Dre",
                NomeProfessor = "Girafalles",
                NomeUe = "Nome da UE",
                NumeroRf = "0014",
                Turma = "123"
            };
            model.Bimestres.Add(new RelatorioFechamentoPendenciaBimestreDto() { 
             
            
            });



            return View("RelatorioFechamentoPendencia", model);
        }

    }
}
