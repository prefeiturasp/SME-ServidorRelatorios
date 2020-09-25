using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME.SR.Application;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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
            model.Ano = 3;
            model.Turma = "5A";
            model.Periodo = "ACOMPANHAMENTO 1º SEMESTRE";
            model.UsuarioNome = "TESTE USUÁRIO";
            model.UsuarioRF = "123456789";
            model.Data = "21/10/2020";
            model.EhEncaminhamento = true;

            ResumoPAPTotalEstudantesDto totalEstudantes = new ResumoPAPTotalEstudantesDto();
            totalEstudantes.PorcentagemTotal = 100;
            totalEstudantes.QuantidadeTotal = 90;

            var anosTotalEstudantes = new List<ResumoPAPTotalAnoDto>();
            
            for(var i = 0; i < 7; i++)
            {
                anosTotalEstudantes.Add(new ResumoPAPTotalAnoDto
                {
                    AnoDescricao = i + 3,
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
                        DescricaoAno = (j + 3).ToString()+'°',
                        Porcentagem = j + i + 7.3,
                        Quantidade = j + i + 10,
                        TotalQuantidade = i + 12,
                        TotalPorcentagem = i + 13,
                    });

                }

                listaLinhas.Add(new ResumoPAPFrequenciaDto()
                {
                    QuantidadeTotalFrequencia = i + 10 + listaAno.Count,
                    PorcentagemTotalFrequencia =  i + 11 + listaAno.Count,
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

            model.FrequenciaDto = listaFrequencia;

            ResumoPAPTotalResultadoDto resultados = new ResumoPAPTotalResultadoDto()
             {
                EixoDescricao = "SONDAGEM"

             };

            var objetivosResultados = new ResumoPAPResultadoObjetivoDto()
            {
                ObjetivoDescricao = "Hipotese de escrita",
            };

            var totalResultados = new ResumoPAPResultadoRespostaDto() {
                Porcentagem = 9,
                Quantidade = 20,
                RespostaDescricao = null,
                TotalPorcentagem = 100,
                TotalQuantidade = 19
            };

            var anosResultados = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = 4,
            };

            var anosResultados1 = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = 8,
            };

            var respostaResultados = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 11,
                Quantidade = 10,
                RespostaDescricao = "Pré silábico",
                TotalPorcentagem = 100,
                TotalQuantidade = 19,
            };

            var respostaResultados1 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 21,
                Quantidade = 20,
                RespostaDescricao = "Silábico",
                TotalPorcentagem = 100,
                TotalQuantidade = 19,
            };


            var listaRespostasResultados = new List<ResumoPAPResultadoRespostaDto>();
            listaRespostasResultados.Add(respostaResultados);
            listaRespostasResultados.Add(respostaResultados);
            listaRespostasResultados.Add(respostaResultados);
            listaRespostasResultados.Add(respostaResultados);
            listaRespostasResultados.Add(respostaResultados);
            anosResultados.Respostas = listaRespostasResultados;

            var listaRespostasResultados1 = new List<ResumoPAPResultadoRespostaDto>();
            listaRespostasResultados1.Add(respostaResultados1);
            listaRespostasResultados1.Add(respostaResultados1);
            anosResultados1.Respostas = listaRespostasResultados1;

            var listaTotalResultados = new List<ResumoPAPResultadoRespostaDto>();
            listaTotalResultados.Add(totalResultados);

            var listaAnosResultados = new List<ResumoPAPResultadoAnoDto>();
            listaAnosResultados.Add(anosResultados);
            listaAnosResultados.Add(anosResultados1);

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
                AnoDescricao = 3,
            };

            var anosResultados3 = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = 7,
            };

            var anosResultados4 = new ResumoPAPResultadoAnoDto()
            {
                AnoDescricao = 8,
            };

            var totalResultados2 = new ResumoPAPResultadoRespostaDto()
            {
                Porcentagem = 9,
                Quantidade = 20,
                RespostaDescricao = null,
                TotalPorcentagem = 100,
                TotalQuantidade = 19
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

            var listaRespostasResultados2 = new List<ResumoPAPResultadoRespostaDto>();
            var listaRespostasResultados3 = new List<ResumoPAPResultadoRespostaDto>();
            listaRespostasResultados2.Add(respostaResultados2);
            listaRespostasResultados2.Add(respostaResultados3);
            listaRespostasResultados3.Add(respostaResultados3);
            anosResultados2.Respostas = listaRespostasResultados2;
            anosResultados3.Respostas = listaRespostasResultados2;
            anosResultados4.Respostas = listaRespostasResultados3;

            var listaTotalResultados2 = new List<ResumoPAPResultadoRespostaDto>();
            listaTotalResultados2.Add(totalResultados2);

            var listaAnosResultados2 = new List<ResumoPAPResultadoAnoDto>();
            listaAnosResultados2.Add(anosResultados2);
            listaAnosResultados2.Add(anosResultados3);
            listaAnosResultados2.Add(anosResultados4);

            var listaObjetivosResultados2 = new List<ResumoPAPResultadoObjetivoDto>();
            objetivosResultados2.Anos = listaAnosResultados2;
            objetivosResultados2.Total = listaTotalResultados2;
            listaObjetivosResultados2.Add(objetivosResultados2);
            resultados2.Objetivos = listaObjetivosResultados2;
            
            listaResultados.Add(resultados2);


            model.ResultadoDto = listaResultados;


            var listaEncaminhamento = new List<ResumoPAPTotalResultadoDto>();
            var listaObjetivos= new List<ResumoPAPResultadoObjetivoDto>();

            for (var i = 0; i < 3; i++)
            {
                var listaAnosEnca = new List<ResumoPAPResultadoAnoDto>();
                var listaTotal = new List<ResumoPAPResultadoRespostaDto>();
                var listaRepostas = new List<ResumoPAPResultadoRespostaDto>();
                var listaRepostas1 = new List<ResumoPAPResultadoRespostaDto>();

                listaTotal.Add(new ResumoPAPResultadoRespostaDto()
                {
                    Porcentagem = 0,
                    Quantidade = 0,
                    RespostaDescricao = null,
                    TotalQuantidade = 30 +  i,
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
                        AnoDescricao = 4 + j,
                        Respostas = listaRepostas
                    });
                }
                if(i == 0)
                {

                    listaRepostas1.Add(new ResumoPAPResultadoRespostaDto()
                    {
                        Porcentagem = 71,
                        Quantidade = 70,
                        RespostaDescricao = "Nao",
                        TotalQuantidade = 0,
                        TotalPorcentagem = 0
                    });

                

                    listaAnosEnca.Add(new ResumoPAPResultadoAnoDto()
                    {
                        AnoDescricao = 6,
                        Respostas = listaRepostas1
                    });
                }


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
    }
}
