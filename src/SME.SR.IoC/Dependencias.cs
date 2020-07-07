using DinkToPdf;
using DinkToPdf.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using SME.SR.Application;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Repositories.Sgp;
using SME.SR.Infra;
using SME.SR.JRSClient;
using SME.SR.JRSClient.Extensions;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;
using SME.SR.Workers.SGP;
using System;
using System.Net;

namespace SME.SR.IoC
{
    public static class Dependencias
    {
        public static void RegistrarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = AppDomain.CurrentDomain.Load("SME.SR.Application");

            services.AddMediatR(assembly);
            AddRabbitMQ(services, configuration);

            var urlJasper = configuration.GetSection("ConfiguracaoJasper:Hostname").Value;
            var usuarioJasper = configuration.GetSection("ConfiguracaoJasper:Username").Value;
            var senhaJasper = configuration.GetSection("ConfiguracaoJasper:Password").Value;

            var cookieContainer = new CookieContainer();
            var jasperCookieHandler = new JasperCookieHandler() { CookieContainer = cookieContainer };
            services.AddSingleton(jasperCookieHandler);

            services.AddHttpClient<IExecucaoRelatorioService, ExecucaoRelatorioService>(c =>
            {
                c.BaseAddress = new Uri(urlJasper);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new JasperCookieHandler() { CookieContainer = cookieContainer };
                });

            services.AddJasperClient(urlJasper, usuarioJasper, senhaJasper);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


            services.AddSingleton(new VariaveisAmbiente());
            RegistrarRepositorios(services);
            RegistrarUseCase(services);
            RegistrarServicos(services);
        }



        private static void RegistrarRepositorios(IServiceCollection services)
        {
            services.TryAddScoped<IExemploRepository, ExemploRepository>();
            services.TryAddScoped(typeof(IAlunoRepository), typeof(AlunoRepository));
            services.TryAddScoped(typeof(IAtribuicaoCJRepository), typeof(AtribuicaoCJRepository));
            services.TryAddScoped(typeof(IAulaRepository), typeof(AulaRepository));
            services.TryAddScoped(typeof(ICicloRepository), typeof(CicloRepository));
            services.TryAddScoped(typeof(IComponenteCurricularRepository), typeof(ComponenteCurricularRepository));
            services.TryAddScoped(typeof(IConselhoClasseRepository), typeof(ConselhoClasseRepository));
            services.TryAddScoped(typeof(IConselhoClasseAlunoRepository), typeof(ConselhoClasseAlunoRepository));
            services.TryAddScoped(typeof(IConselhoClasseNotaRepository), typeof(ConselhoClasseNotaRepository));
            services.TryAddScoped(typeof(IConselhoClasseRecomendacaoRepository), typeof(ConselhoClasseRecomendacaoRepository));
            services.TryAddScoped(typeof(IEolRepository), typeof(EolRepository));
            services.TryAddScoped(typeof(IFechamentoAlunoRepository), typeof(FechamentoAlunoRepository));
            services.TryAddScoped(typeof(IFechamentoNotaRepository), typeof(FechamentoNotaRepository));
            services.TryAddScoped(typeof(IFechamentoTurmaRepository), typeof(FechamentoTurmaRepository));
            services.TryAddScoped(typeof(IFrequenciaAlunoRepository), typeof(FrequenciaAlunoRepository));
            services.TryAddScoped(typeof(INotaTipoRepository), typeof(NotaTipoRepository));
            services.TryAddScoped(typeof(IParametroSistemaRepository), typeof(ParametroSistemaRepository));
            services.TryAddScoped(typeof(IPeriodoEscolarRepository), typeof(PeriodoEscolarRepository));
            services.TryAddScoped(typeof(IPeriodoFechamentoRepository), typeof(PeriodoFechamentoRepository));
            services.TryAddScoped(typeof(IPermissaoRepository), typeof(PermissaoRepository));
            services.TryAddScoped(typeof(ITipoCalendarioRepository), typeof(TipoCalendarioRepository));
            services.TryAddScoped(typeof(ITurmaRepository), typeof(TurmaRepository));
        }

        //private static void RegistrarCommands(IServiceCollection services)
        //{
        //    services.AddMediatR(typeof(GerarRelatorioAssincronoCommand).GetTypeInfo().Assembly);
        //}

        //private static void RegistrarQueries(IServiceCollection services)
        //{
        //    services.AddMediatR(typeof(ObterAlunosPorTurmaQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterAnotacoesAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterComponentesCurricularesPorTurmaQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteComNotaBimestreQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteComNotaFinalQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteSemNotaBimestreQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteSemNotaFinalQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDreUePorTurmaQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFrequenciaAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFrequenciaGlobalPorAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFrequenciaPorDisciplinaBimestresQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterNotasAlunoBimestreQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterNotasConselhoClasseAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterParametroSistemaPorTipoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterParecerConclusivoPorAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterRecomendacoesPorFechamentoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterRelatorioConselhoClasseAlunoQuery).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFechamentoTurmaPorIdQuery).GetTypeInfo().Assembly);
        //}

        //private static void RegistrarHandlers(IServiceCollection services)
        //{
        //    services.AddMediatR(typeof(ObterAlunosPorTurmaQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterAnotacoesAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterComponentesCurricularesPorTurmaQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteComNotaBimestreQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteComNotaFinalQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteSemNotaBimestreQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDadosComponenteSemNotaFinalQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterDreUePorTurmaQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFrequenciaGlobalPorAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFrequenciaPorDisciplinaBimestresQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFrequenciaAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterNotasAlunoBimestreQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterNotasConselhoClasseAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterParecerConclusivoPorAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterRecomendacoesPorFechamentoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterRelatorioConselhoClasseAlunoQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterFechamentoTurmaPorIdQueryHandler).GetTypeInfo().Assembly);
        //    services.AddMediatR(typeof(ObterParametroSistemaPorTipoQueryHandler).GetTypeInfo().Assembly);


        //}

        private static void RegistrarUseCase(IServiceCollection services)
        {
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
            services.TryAddScoped<IMonitorarStatusRelatorioUseCase, MonitorarStatusRelatorioUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAlunoUseCase, RelatorioConselhoClasseAlunoUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseTurmaUseCase, RelatorioConselhoClasseTurmaUseCase>();
            services.TryAddScoped<IRelatorioBoletimEscolarUseCase, RelatorioBoletimEscolarUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAtaFinalUseCase, RelatorioConselhoClasseAtaFinalUseCase>();
            services.TryAddScoped<IDownloadPdfRelatorioUseCase, DownloadPdfRelatorioUseCase>();
        }

        private static void RegistrarServicos(IServiceCollection services)
        {
            services.TryAddScoped<IServicoFila, FilaRabbit>();
        }


        public static void AddRabbitMQ(IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value
            };

            var conexaoRabbit = factory.CreateConnection();
            IModel channel = conexaoRabbit.CreateModel();

            services.AddSingleton(conexaoRabbit);
            services.AddSingleton(channel);
        }
    }
}
