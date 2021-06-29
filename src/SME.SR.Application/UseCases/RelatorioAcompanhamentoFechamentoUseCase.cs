using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFechamentoUseCase : IRelatorioAcompanhamentoFechamentoUseCase
    {
        public async Task<RelatorioAcompanhamentoFechamentoPorUeDto> Executar(FiltroRelatorioDto filtro)
        {
            return await Task.FromResult(new RelatorioAcompanhamentoFechamentoPorUeDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                DreNome = "DIRETORIA REGIONAL DE EDUCAÇÃO BUTANTA",
                RF = "1234567",
                Turma = "TODAS",
                UeNome = "CEU EMEF BUTANTA",
                Usuario = "JULIA FERREIRA DE OLIVEIRA",
                Turmas = new List<RelatorioAcompanhamentoFechamentoTurmaDto>()
                {
                    new RelatorioAcompanhamentoFechamentoTurmaDto()
                    {
                          TurmaDescricao = "EF - 6A",
                          Bimestres = new List<RelatorioAcompanhamentoFechamentoBimestreDto>()
                          {
                              new RelatorioAcompanhamentoFechamentoBimestreDto()
                              {
                                   Bimestre = 1,
                                   FechamentosComponente = new List<RelatorioAcompanhamentoFechamentoComponenteDto>()
                                   {
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Arte",
                                           Status = "Não Iniciado"
                                      },
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Ciência",
                                           Status = "Em Andamento"
                                      },
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Educação Física",
                                           Status = "Processado com Pendência",
                                           Pendencias = new List<string>()
                                           {
                                               "Aulas sem frequência registrada",
                                               "Aulas sem plano de aula registrado"
                                           }
                                      },
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Geografia",
                                           Status = "Processado",
                                           Pendencias = new List<string>()
                                           {
                                               "Avaliação sem notas/conceitos lançados"
                                           }
                                      }
                                  },
                                  ConselhosClasse = new List<RelatorioAcompanhamentoFechamentoConselhoDto>()
                                  {
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                           Status = "Não iniciado",
                                           Quantidade = 2
                                      },
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                            Status = "Em andamento",
                                            Quantidade = 5
                                      },
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                           Status = "Concluído",
                                           Quantidade = 10
                                      }
                                  }
                              },
                              new RelatorioAcompanhamentoFechamentoBimestreDto()
                              {
                                   Bimestre = 2,
                                   FechamentosComponente = new List<RelatorioAcompanhamentoFechamentoComponenteDto>()
                                   {
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Ciência",
                                           Status = "Em Andamento"
                                      },
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Educação Física",
                                           Status = "Processado com Pendência",
                                           Pendencias = new List<string>()
                                           {
                                               "Aulas sem frequência registrada",
                                               "Aulas sem plano de aula registrado"
                                           }
                                      },
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Geografia",
                                           Status = "Processado",
                                           Pendencias = new List<string>()
                                           {
                                               "Avaliação sem notas/conceitos lançados"
                                           }
                                      }
                                  },
                                  ConselhosClasse = new List<RelatorioAcompanhamentoFechamentoConselhoDto>()
                                  {
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                           Status = "Não iniciado",
                                           Quantidade = 2
                                      },
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                           Status = "Concluído",
                                           Quantidade = 10
                                      }
                                  }
                              }
                          },
                    },
                    new RelatorioAcompanhamentoFechamentoTurmaDto()
                    {
                          TurmaDescricao = "EF - 7A",
                          Bimestres = new List<RelatorioAcompanhamentoFechamentoBimestreDto>()
                          {
                              new RelatorioAcompanhamentoFechamentoBimestreDto()
                              {
                                   Bimestre = 1,
                                   FechamentosComponente = new List<RelatorioAcompanhamentoFechamentoComponenteDto>()
                                   {
                                      new RelatorioAcompanhamentoFechamentoComponenteDto()
                                      {
                                           Componente = "Arte",
                                           Status = "Não Iniciado"
                                      },
                                  },
                                  ConselhosClasse = new List<RelatorioAcompanhamentoFechamentoConselhoDto>()
                                  {
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                           Status = "Não iniciado",
                                           Quantidade = 2
                                      },
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                            Status = "Em andamento",
                                            Quantidade = 5
                                      },
                                      new RelatorioAcompanhamentoFechamentoConselhoDto()
                                      {
                                           Status = "Concluído",
                                           Quantidade = 10
                                      }
                                  }
                              }
                          },
                    }
                }
            });
        }
    }
}
