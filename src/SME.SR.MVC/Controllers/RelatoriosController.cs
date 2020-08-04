using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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


        [HttpGet("recuperacao-paralela")]
        public IActionResult RelatorioRecuperacaoParalela([FromServices] IMediator mediator)
        {
            RelatorioRecuperacaoParalelaDto model = new RelatorioRecuperacaoParalelaDto("Todas", "Todas", "ALANA FERREIRA DE OLIVEIRA", "1234567", DateTime.Today.ToString("dd/MM/yyyy"), 2020, 1);
            model.Alunos.Add(new RelatorioRecuperacaoParalelaAlunoDto("JOSÉ AUGUSTO CLEMENTE", "RF", "16/03/1985", "4241513", "RF", "MATRICULADO EM 04/02/2019",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies. Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies..",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies."));

            model.Alunos.Add(new RelatorioRecuperacaoParalelaAlunoDto("RONALDO CLEMENTE AUGUSTO", "RF", "16/03/1985", "4241513", "RF", "MATRICULADO EM 04/02/2019",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies."));

            model.Alunos.Add(new RelatorioRecuperacaoParalelaAlunoDto("PEREIRA CLEMENTE AUGUSTO", "RF", "16/03/1985", "4241513", "RF", "MATRICULADO EM 04/02/2019",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies."));

            model.Alunos.Add(new RelatorioRecuperacaoParalelaAlunoDto("AUGUSTA CLEMENTE AUGUSTO", "RF", "16/03/1985", "4241513", "RF", "MATRICULADO EM 04/02/2019",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies.",
                 "Donec. Aliquam pretium adipiscing consequat sollicitudin sed penatibus sit accumsan. Nostra hendrerit nascetur leo consectetuer ultrices mauris venenatis sodales massa, tellus vulputate condimentum nostra. Metus hendrerit luctus felis morbi. Libero condimentum primis, mattis condimentum fames urna. Pretium. Mauris et tincidunt viverra montes. Nascetur. Ante nisl vel convallis porta felis. Ad hymenaeos placerat. Phasellus. Odio magnis condimentum Enim hendrerit blandit, odio. Cursus lacus. Odio vulputate. At fames nam. Bibendum ut dis risus cursus aliquet hendrerit. Scelerisque eget penatibus nunc sit tellus pharetra pharetra porta condimentum cras Rutrum Fermentum vel proin feugiat molestie suscipit felis orci purus, nec tempus dapibus lacinia ultricies."));



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
        private static RelatorioFechamentoPendenciasDto GeraVariasPendencias2Componentes()
        {
            var model = new RelatorioFechamentoPendenciasDto();

            model.DreNome = "DRE 001";
            model.UeNome = "UE 001";
            model.TurmaNome = "1F - 01";
            model.Ano = "1987";
            model.Bimestre = "1º";
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
                          Nome = "Turma de Teste",
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
                        }

                }
            };
            return model;
        }
        private static RelatorioFechamentoPendenciasDto GeraVariasPendencias()
        {
            var model = new RelatorioFechamentoPendenciasDto();

            model.DreNome = "DRE 001";
            model.UeNome = "UE 001";
            model.TurmaNome = "1F - 01";
            model.Ano = "1987";
            model.Bimestre = "1º";
            model.ComponenteCurricular = "Matemática";
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
                          Nome = "Turma de Teste",
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
                        }

                }
            };
            return model;
        }
        private static RelatorioFechamentoPendenciasDto Gera1PendenciaBasica()
        {
            var model = new RelatorioFechamentoPendenciasDto();

            model.DreNome = "DRE 001";
            model.UeNome = "UE 001";
            model.TurmaNome = "1F - 01";
            model.Ano = "1987";
            model.Bimestre = "1º";
            model.ComponenteCurricular = "Matemática";
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
                          Nome = "Turma de Teste",
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
