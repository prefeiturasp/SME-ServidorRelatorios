using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterOrdensSondagemPorAnoProficienciaQueryHandler : IRequestHandler<ObterOrdensSondagemPorAnoProficienciaQuery, IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>>
    {
        public async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> Handle(ObterOrdensSondagemPorAnoProficienciaQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>();
            
            if (request.Proficiencia == ProficienciaSondagemEnum.Numeros)
            {
                AdicionarOrdensNumeros(listaRetorno);
            }
            else
            {
                switch (request.Ano)
                {
                    case "1":
                        AdicionarOrdensAno1(listaRetorno);
                        break;
                    case "2":
                        AdicionarOrdensAno2(listaRetorno, request.Proficiencia);
                        break;
                    case "3":
                        AdicionarOrdensAno3(listaRetorno, request.Proficiencia);
                        break;
                    case "4":
                        AdicionarOrdensAno4(listaRetorno, request.Proficiencia);
                        break;
                    case "5":
                    case "6":
                        AdicionarOrdensAno5(listaRetorno, request.Proficiencia);
                        break;
                    case "7":
                    case "8":
                    case "9":
                        AdicionarOrdensAnos789(listaRetorno);
                        break;
                    default:
                        break;
                }
            }

            return await Task.FromResult(listaRetorno);


        }

        private void AdicionarOrdensAno5(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno, ProficienciaSondagemEnum proficiencia)
        {
            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.CampoAditivo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 1 - COMPOSIÇÃO",
                        Id = 1
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 2 - TRANSFORMAÇÃO",
                        Id = 2
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 3 - COMPOSIÇÃO DE TRANSF.",
                        Id = 3
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 4 - COMPARAÇÃO",
                        Id = 4
                    });
                    break;
                case ProficienciaSondagemEnum.CampoMultiplicativo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 5 - COMBINATÓRIA",
                        Id = 5
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 6 - CONFIGURAÇÃO RETANGULAR",
                        Id = 6
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 7 - PROPORCIONALIDADE",
                        Id = 7
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 8 - MULTIPLICAÇÃO COMPARATIVA",
                        Id = 8
                    });

                    break;
                case ProficienciaSondagemEnum.Numeros:
                    break;
                case ProficienciaSondagemEnum.Leitura:
                    break;
                case ProficienciaSondagemEnum.Escrita:
                    break;
                default:
                    break;
            }
        }

        private void AdicionarOrdensAno4(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno, ProficienciaSondagemEnum proficiencia)
        {
            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.CampoAditivo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 1 - COMPOSIÇÃO",
                        Id = 1
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 2 - TRANSFORMAÇÃO",
                        Id = 2
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 3 - COMPOSIÇÃO DE TRANSF.",
                        Id = 3
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 4 - COMPARAÇÃO",
                        Id = 4
                    });
                    break;
                case ProficienciaSondagemEnum.CampoMultiplicativo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 5 - CONFIGURAÇÃO RETANGULAR",
                        Id = 5
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 6 - PROPORCIONALIDADE",
                        Id = 6
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 7 - COMBINATÓRIA",
                        Id = 7
                    });
                    break;
                case ProficienciaSondagemEnum.Numeros:
                    break;
                case ProficienciaSondagemEnum.Leitura:
                    break;
                case ProficienciaSondagemEnum.Escrita:
                    break;
                default:
                    break;
            }
        }

        private void AdicionarOrdensAno3(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno, ProficienciaSondagemEnum proficiencia)
        {
            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.CampoAditivo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 1 - COMPOSIÇÃO",
                        Id = 1
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 2 - TRANSFORMAÇÃO",
                        Id = 2
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 3 - COMPARAÇÃO",
                        Id = 3
                    });
                    break;
                case ProficienciaSondagemEnum.CampoMultiplicativo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 4 - CONFIGURAÇÃO RETANGULAR",
                        Id = 4
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 5 - PROPORCIONALIDADE",
                        Id = 5
                    });
                    break;
                case ProficienciaSondagemEnum.Numeros:
                    break;
                case ProficienciaSondagemEnum.Leitura:
                    break;
                case ProficienciaSondagemEnum.Escrita:
                    break;
                default:
                    break;
            }
        }

        private void AdicionarOrdensAno2(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno, ProficienciaSondagemEnum proficiencia)
        {
            switch (proficiencia)
            {
                case ProficienciaSondagemEnum.CampoAditivo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 1 - COMPOSIÇÃO",
                        Id = 1
                    });
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 2 - TRANSFORMAÇÃO",
                        Id = 2
                    });
                    break;
                case ProficienciaSondagemEnum.CampoMultiplicativo:
                    listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
                    {
                        Descricao = "ORDEM 3 - PROPORCIONALIDADE",
                        Id = 3
                    });
                    break;
                case ProficienciaSondagemEnum.Numeros:
                    break;
                case ProficienciaSondagemEnum.Leitura:
                    break;
                case ProficienciaSondagemEnum.Escrita:
                    break;
                default:
                    break;
            }
        }

        private static void AdicionarOrdensAno1(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno)
        {
            listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
            {
                Descricao = "ORDEM 1 - COMPOSIÇÃO",
                Id = 1
            });
            listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
            {
                Descricao = "ORDEM 2 - COMPOSIÇÃO",
                Id = 2
            });
            listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
            {
                Descricao = "ORDEM 3 - COMPOSIÇÃO",
                Id = 3
            });
        }

        private static void AdicionarOrdensAnos789(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno)
        {
            listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
            {
                Descricao = "ORDEM AUTORAL",
                Id = 0
            });
        }

        private static void AdicionarOrdensNumeros(List<RelatorioSondagemComponentesPorTurmaOrdemDto> listaRetorno)
        {
            listaRetorno.Add(new RelatorioSondagemComponentesPorTurmaOrdemDto()
            {
                Descricao = "ORDEM NUMEROS",
                Id = 0
            });
        }

    }
}
