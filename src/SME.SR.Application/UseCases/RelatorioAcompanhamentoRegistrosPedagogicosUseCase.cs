using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosUseCase : IRelatorioAcompanhamentoRegistrosPedagogicosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoRegistrosPedagogicosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroRelatorioAcompanhamentoRegistrosPedagogicos;

            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery>();

            if (relatorioQuery.Modalidade == Modalidade.Infantil)
            {

                // TODO NOVO - 2022 PARAMETRO
                if (relatorioQuery.AnoLetivo == 2022)
                {
                    //var relatorioQueryInfantil = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery>();
                    //var relatorioInfantilDto = await mediator.Send(relatorioQueryInfantil);

                    var cabecalho = new RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto()
                    {
                        Dre = "BT",
                        Ue = "019262 - CEU EMEF BUTANTA",
                        Turma = "Todas",
                        Bimestre = "1",
                        UsuarioNome = "JULIA FERREIRA DE OLIVEIRA",
                        UsuarioRF = "1234567",
                    };

                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto();
                    var bimestre2 = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto();
                    var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>();                    
                    var turmasInfantilComponente = new List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto>(); 

                    for (var i = 0; i < 50; i++)
                    {
                        var turmaInfantilComponente1 = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto();
                        turmaInfantilComponente1.Nome = "EI - 5A";
                        turmaInfantilComponente1.Aulas = 201;
                        turmaInfantilComponente1.FrequenciasPendentes = 3;
                        turmaInfantilComponente1.DataUltimoRegistroFrequencia = "10/06/2021";

                        var turmaInfantilComponente2 = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto();
                        turmaInfantilComponente2.Nome = "EI - 5B";
                        turmaInfantilComponente2.Aulas = 234;
                        turmaInfantilComponente2.FrequenciasPendentes = 5;
                        turmaInfantilComponente2.DataUltimoRegistroFrequencia = "23/06/2021";

                        turmasInfantilComponente.Add(turmaInfantilComponente1);
                        turmasInfantilComponente.Add(turmaInfantilComponente2); 
                    }

                    var turmasDiarios= new List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto>();
                    var componentes = new List<RelatorioAcompanhamentoRegistrosPedagogicosDiarioBordoComponenteDto>();

                    var componente1 = new RelatorioAcompanhamentoRegistrosPedagogicosDiarioBordoComponenteDto();
                    componente1.NomeComponente = "REGÊNCIA DE CLASSE EMEI 4H";
                    componente1.DiarioBordoPendentes = 123;
                    componente1.DataUltimoRegistroDiarioBordo = "01/01/2000";

                    var componente2 = new RelatorioAcompanhamentoRegistrosPedagogicosDiarioBordoComponenteDto();
                    componente2.NomeComponente = "REGÊNCIA DE CLASSE EMEI 2H";
                    componente2.DiarioBordoPendentes = 333;
                    componente2.DataUltimoRegistroDiarioBordo = "01/01/2005";

                    componentes.Add(componente1);
                    componentes.Add(componente2);

                    for (var i = 0; i < 20; i++)
                    {
                        var turmaDiario1 = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto();
                        turmaDiario1.NomeTurma = "EI - 5C";
                        turmaDiario1.Aulas = 111;
                        turmaDiario1.Componentes = componentes;

                        var turmaDiario2 = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto();
                        turmaDiario2.NomeTurma = "EI - 5D";
                        turmaDiario2.Aulas = 222;
                        turmaDiario2.Componentes = componentes;

                        turmasDiarios.Add(turmaDiario1);
                        turmasDiarios.Add(turmaDiario2);
                    }

                    bimestre.Bimestre = "1º BIMESTRE";
                    bimestre.TurmasInfantilComponente = turmasInfantilComponente;
                    bimestre.TurmasInfantilDiarioBordoComponente = turmasDiarios;
                    bimestres.Add(bimestre);

                    bimestre2.Bimestre = "2º BIMESTRE";
                    bimestre2.TurmasInfantilComponente = turmasInfantilComponente;
                    bimestre2.TurmasInfantilDiarioBordoComponente = turmasDiarios;
                    bimestres.Add(bimestre2);

                    var mock = new RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto()
                    {
                        Cabecalho = cabecalho,
                        Bimestres = bimestres,
                    };


                    if (mock.Bimestres.Any())
                        await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponente", mock, request.CodigoCorrelacao, "", "Relatório de Acompanhamento de registros pedagógicos", true));
                    else
                        throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
                }
                else
                {
               
                    var relatorioQueryInfantil = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery>();
                    var relatorioInfantilDto = await mediator.Send(relatorioQueryInfantil);
                    if (relatorioInfantilDto.Bimestre.Any())
                        await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicosInfantil", relatorioInfantilDto, request.CodigoCorrelacao, "", "Relatório de Acompanhamento de registros pedagógicos", true));
                    else    
                        throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");     
                }
            }
            else
            {
                var relatorioDto = await mediator.Send(relatorioQuery);
                if (relatorioDto.Bimestre.Any())
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicos", relatorioDto, request.CodigoCorrelacao,"", "Relatório de Acompanhamento de registros pedagógicos",true));
                else
                    throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
            }
        }
    }
}
