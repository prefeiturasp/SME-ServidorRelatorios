using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaPdfQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciaPdfQuery, RelatorioFaltasFrequenciaDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFaltasFrequenciaPdfQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFaltasFrequenciaDto> Handle(ObterRelatorioFaltasFrequenciaPdfQuery request, CancellationToken cancellationToken)
        {
            var alunos = await ObterAlunosPorAno(request.Filtro.AnoLetivo, request.Filtro.AnosEscolares);

            var model = new RelatorioFaltasFrequenciaDto();
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
            
            return await Task.FromResult(model);
        }

        private async Task<IEnumerable<RelatorioFaltaFrequenciaAlunoDto>> ObterAlunosPorAno(int anoLetivo, IEnumerable<string> anosEscolares)
        {
            var alunos = await mediator.Send(new ObterAlunosPorAnoQuery(anoLetivo, anosEscolares));
            return alunos.Select(a => new RelatorioFaltaFrequenciaAlunoDto()
            {
                Nome = a.Nome,
                NomeTurma = a.Turma,
                Numero = a.NumeroChamada
            });
        }
    }
}
