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
using SME.SR.Data.Repositories.Cache;
using SME.SR.Data.Repositories.Sgp;
using SME.SR.Data.Repositories.Sondagem;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.JRSClient;
using SME.SR.JRSClient.Extensions;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;
using SME.SR.Workers.SGP;
using System;
using System.IO;
using System.Net;

namespace SME.SR.IoC
{
    public static class Dependencias
    {
        public static void AddRabbitMQ(IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value,
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value
            };

            var conexaoRabbit = factory.CreateConnection();
            IModel channel = conexaoRabbit.CreateModel();

            services.AddSingleton(conexaoRabbit);
            services.AddSingleton(channel);
        }

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

            services.AddHttpClient(name: "jasperServer", c =>
            {
                c.BaseAddress = new Uri(urlJasper);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new JasperCookieHandler() { CookieContainer = cookieContainer };
                });

            services.AddJasperClient(urlJasper, usuarioJasper, senhaJasper);

            var context = new CustomAssemblyLoadContext();
            var nomeBliblioteca = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ? "libwkhtmltox.dll" : "libwkhtmltox.so";
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), nomeBliblioteca));

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddScoped<IHtmlHelper, HtmlHelper>();

            services.AddSingleton(new VariaveisAmbiente(configuration));
            RegistrarRepositorios(services);
            RegistrarUseCase(services);
            RegistrarServicos(services);
            RegistrarOptions(services, configuration);
        }

        private static void RegistrarRepositorios(IServiceCollection services)
        {
            services.TryAddScoped<IExemploRepository, ExemploRepository>();
            services.TryAddScoped(typeof(IAreaDoConhecimentoRepository), typeof(AreaDoConhecimentoRepository));
            services.TryAddScoped(typeof(IAlunoRepository), typeof(AlunoRepository));
            services.TryAddScoped(typeof(IRepositorioCache), typeof(RepositorioCache));
            services.TryAddScoped(typeof(IAtribuicaoCJRepository), typeof(AtribuicaoCJRepository));
            services.TryAddScoped(typeof(IAulaRepository), typeof(AulaRepository));
            services.TryAddScoped(typeof(ICicloRepository), typeof(CicloRepository));
            services.TryAddScoped(typeof(IComponenteCurricularRepository), typeof(ComponenteCurricularRepository));
            services.TryAddScoped(typeof(IComponenteCurricularGrupoAreaOrdenacaoRepository), typeof(ComponenteCurricularGrupoAreaOrdenacaoRepository));
            services.TryAddScoped(typeof(IConselhoClasseRepository), typeof(ConselhoClasseRepository));
            services.TryAddScoped(typeof(IConselhoClasseAlunoRepository), typeof(ConselhoClasseAlunoRepository));
            services.TryAddScoped(typeof(IConselhoClasseNotaRepository), typeof(ConselhoClasseNotaRepository));
            services.TryAddScoped(typeof(IConselhoClasseRecomendacaoRepository), typeof(ConselhoClasseRecomendacaoRepository));
            services.TryAddScoped(typeof(IConselhoClasseAlunoTurmaComplementarRepository), typeof(ConselhoClasseAlunoTurmaComplementarRepository));
            services.TryAddScoped(typeof(IEolRepository), typeof(EolRepository));
            services.TryAddScoped(typeof(IFechamentoAlunoRepository), typeof(FechamentoAlunoRepository));
            services.TryAddScoped(typeof(IFechamentoNotaRepository), typeof(FechamentoNotaRepository));
            services.TryAddScoped(typeof(IFechamentoTurmaRepository), typeof(FechamentoTurmaRepository));
            services.TryAddScoped(typeof(IFrequenciaAlunoRepository), typeof(FrequenciaAlunoRepository));
            services.TryAddScoped(typeof(IRegistroFrequenciaRepository), typeof(RegistroFrequenciaRepository));
            services.TryAddScoped(typeof(INotaTipoRepository), typeof(NotaTipoRepository));
            services.TryAddScoped(typeof(IFuncionarioRepository), typeof(FuncionarioRepository));
            services.TryAddScoped(typeof(IParametroSistemaRepository), typeof(ParametroSistemaRepository));
            services.TryAddScoped(typeof(IPeriodoEscolarRepository), typeof(PeriodoEscolarRepository));
            services.TryAddScoped(typeof(IPeriodoFechamentoRepository), typeof(PeriodoFechamentoRepository));
            services.TryAddScoped(typeof(IPermissaoRepository), typeof(PermissaoRepository));
            services.TryAddScoped(typeof(ITipoCalendarioRepository), typeof(TipoCalendarioRepository));
            services.TryAddScoped(typeof(ITurmaRepository), typeof(TurmaRepository));
            services.TryAddScoped(typeof(IDreRepository), typeof(DreRepository));
            services.TryAddScoped(typeof(INotaConceitoRepository), typeof(NotaConceitoRepository));
            services.TryAddScoped(typeof(IUeRepository), typeof(UeRepository));
            services.TryAddScoped(typeof(IObterCabecalhoHistoricoEscolarRepository), typeof(ObterCabecalhoHistoricoEscolarRepository));
            services.TryAddScoped(typeof(IObterEnderecoeAtosDaUeRepository), typeof(ObterEnderecoeAtosDaUeRepository));
            services.TryAddScoped(typeof(IRelatorioFrequenciaRepository), typeof(RelatorioFrequenciaRepository));
            services.TryAddScoped(typeof(IConceitoValoresRepository), typeof(ConceitoValoresRepository));
            services.TryAddScoped(typeof(IPendenciaRepository), typeof(PendenciaRepository));
            services.TryAddScoped(typeof(IRelatorioRecuperacaoParalelaRepository), typeof(RelatorioRecuperacaoParalelaRepository));
            services.TryAddScoped(typeof(IParecerConclusivoRepository), typeof(ParecerConclusivoRepository));
            services.TryAddScoped(typeof(ICompensacaoAusenciaRepository), typeof(CompensacaoAusenciaRepository));
            services.TryAddScoped(typeof(ICalendarioEventoRepository), typeof(CalendarioEventoRepository));
            services.TryAddScoped(typeof(IRecuperacaoParalelaRepository), typeof(RecuperacaoParalelaRepository));
            services.TryAddScoped(typeof(IPlanoAulaRepository), typeof(PlanoAulaRepository));
            services.TryAddScoped(typeof(IRelatorioSondagemComponentePorTurmaRepository), typeof(RelatorioSondagemComponentePorTurmaRepository));
            services.TryAddScoped(typeof(IRelatorioSondagemPortuguesPorTurmaRepository), typeof(RelatorioSondagemPortuguesPorTurmaRepository));
            services.TryAddScoped(typeof(IRelatorioSondagemPortuguesConsolidadoRepository), typeof(RelatorioSondagemPortuguesConsolidadoRepository));
            services.TryAddScoped(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            services.TryAddScoped(typeof(ITurmaEolRepository), typeof(TurmaEolRepository));
            services.TryAddScoped(typeof(IMathPoolNumbersRepository), typeof(MathPoolNumbersRepository));
            services.TryAddScoped(typeof(IPerguntasAutoralRepository), typeof(PerguntasAutoralRepository));
            services.TryAddScoped(typeof(ISondagemAutoralRepository), typeof(SondagemAutoralRepository));
            services.TryAddScoped(typeof(IPerguntasAditMultiNumRepository), typeof(PerguntasAditMultiNumRepository));
            services.TryAddScoped(typeof(IPeriodoSondagemRepository), typeof(PeriodoSondagemRepository));
            services.TryAddScoped(typeof(IAulaPrevistaBimestreRepository), typeof(AulaPrevistaBimestreRepository));
            services.TryAddScoped(typeof(IAtribuicaoEsporadicaRepository), typeof(AtribuicaoEsporadicaRepository));
            services.TryAddScoped(typeof(ICargoRepository), typeof(CargoRepository));
            services.TryAddScoped(typeof(IProfessorRepository), typeof(ProfessorRepository));

            services.TryAddScoped(typeof(IMathPoolCARepository), typeof(MathPoolCARepository));
            services.TryAddScoped(typeof(IMathPoolCMRepository), typeof(MathPoolCMRepository));

            services.TryAddScoped(typeof(IRelatorioSondagemPortuguesPorTurmaRepository), typeof(RelatorioSondagemPortuguesPorTurmaRepository));
            services.TryAddScoped(typeof(ISondagemOrdemRepository), typeof(SondagemOrdemRepository));
            services.TryAddScoped(typeof(IEventoRepository), typeof(EventoRepository));
            services.TryAddScoped(typeof(IDiarioBordoRepository), typeof(DiarioBordoRepository));
            services.TryAddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));

            services.TryAddScoped(typeof(IDashboardAdesaoRepository), typeof(DashboardAdesaoRepository));
            services.TryAddScoped(typeof(IUsuarioAERepository), typeof(UsuarioAERepository));

            services.TryAddScoped(typeof(IComunicadosRepository), typeof(ComunicadosRepository));

            services.TryAddScoped(typeof(IDevolutivaRepository), typeof(DevolutivaRepository));
            services.TryAddScoped(typeof(IItineranciaRepository), typeof(ItineranciaRepository));
            services.TryAddScoped(typeof(IAcompanhamentoAprendizagemRepository), typeof(AcompanhamentoAprendizagemRepository));
            services.TryAddScoped(typeof(IRegistroIndividualRepository), typeof(RegistroIndividualRepository));
            services.TryAddScoped(typeof(Data.Interfaces.IOcorrenciaRepository), typeof(Data.OcorrenciaRepository));
            services.TryAddScoped(typeof(IUeEolRepository), typeof(UeEolRepository));
            services.TryAddScoped(typeof(IArquivoRepository), typeof(ArquivoRepository));

            services.TryAddScoped(typeof(IAlunoFotoRepository), typeof(AlunoFotoRepository));

            services.TryAddScoped(typeof(IConselhoClasseConsolidadoRepository), typeof(ConselhoClasseConsolidadoRepository));
            services.TryAddScoped(typeof(IFechamentoConsolidadoRepository), typeof(FechamentoConsolidadoRepository));
            services.TryAddScoped(typeof(IPendenciaFechamentoRepository), typeof(PendenciaFechamentoRepository));
            services.TryAddScoped(typeof(IRegistroFrequenciaAlunoRepository), typeof(RegistroFrequenciaAlunoRepository));

            services.TryAddScoped(typeof(IRegistrosPedagogicosRepository), typeof(RegistrosPedagogicosRepository));
        }

        private static void RegistrarServicos(IServiceCollection services)
        {
            services.TryAddScoped<IServicoFila, FilaRabbit>();
            services.TryAddScoped<IReportConverter, PdfGenerator>();
            services.TryAddScoped<IServicoTelemetria, ServicoTelemetria>();
        }

        private static void RegistrarUseCase(IServiceCollection services)
        {
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
            services.TryAddScoped<IMonitorarStatusRelatorioUseCase, MonitorarStatusRelatorioUseCase>();
            services.TryAddScoped<IRelatorioPlanoAulaUseCase, RelatorioPlanoAulaUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAlunoUseCase, RelatorioConselhoClasseAlunoUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseTurmaUseCase, RelatorioConselhoClasseTurmaUseCase>();
            services.TryAddScoped<IRelatorioBoletimEscolarUseCase, RelatorioBoletimEscolarUseCase>();
            services.TryAddScoped<IRelatorioBoletimEscolarDetalhadoUseCase, RelatorioBoletimEscolarDetalhadoUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAtaFinalUseCase, RelatorioConselhoClasseAtaFinalUseCase>();
            services.TryAddScoped<IDownloadRelatorioUseCase, DownloadRelatorioUseCase>();
            services.TryAddScoped<IRelatorioFrequenciasUseCase, RelatorioFrequenciasUseCase>();
            services.TryAddScoped<IRelatorioHistoricoEscolarUseCase, RelatorioHistoricoEscolarUseCase>();
            services.TryAddScoped<IRelatorioPendenciasUseCase, RelatorioPendenciasUseCase>();
            services.TryAddScoped<IRelatorioRecuperacaoParalelaUseCase, RelatorioRecuperacaoParalelaUseCase>();
            services.TryAddScoped<IRelatorioParecerConclusivoUseCase, RelatorioParecerConclusivoUseCase>();
            services.TryAddScoped<IRelatorioNotasEConceitosFinaisUseCase, RelatorioNotasEConceitosFinaisUseCase>();
            services.TryAddScoped<IRelatorioCompensacaoAusenciaUseCase, RelatorioCompensacaoAusenciaUseCase>();
            services.TryAddScoped<IRelatorioImpressaoCalendarioUseCase, RelatorioImpressaoCalendarioUseCase>();
            services.TryAddScoped<IRelatorioResumoPAPUseCase, RelatorioResumoPAPUseCase>();
            services.TryAddScoped<IRelatorioGraficoPAPUseCase, RelatorioGraficoPAPUseCase>();
            services.TryAddScoped<IRelatorioSondagemMatematicaConsolidadoUseCase, RelatorioSondagemMatematicaConsolidadoUseCase>();
            services.TryAddScoped<IRelatorioSondagemComponentesPorTurmaUseCase, RelatorioSondagemComponentesPorTurmaUseCase>();
            services.TryAddScoped<IRelatorioControleGradeUseCase, RelatorioControleGradeUseCase>();
            services.TryAddScoped<IRelatorioSondagemMatConsolidadoAdtMultiUseCase, RelatorioSondagemMatConsolidadoAdtMultiUseCase>();
            services.TryAddScoped<IRelatorioSondagemPortuguesPorTurmaUseCase, RelatorioSondagemPortuguesPorTurmaUseCase>();
            services.TryAddScoped<IRelatorioSondagemPortuguesConsolidadoUseCase, RelatorioSondagemPortuguesConsolidadoUseCase>();
            services.TryAddScoped<IRelatorioSondagemPtPorTurmaCapLeituraUseCase, RelatorioSondagemPtPorTurmaCapLeituraUseCase>();
            services.TryAddScoped<IRelatorioSondagemPtConsolidadoLeitEscProdUseCase, RelatorioSondagemPtConsolidadoLeitEscProdUseCase>();
            services.TryAddScoped<IRelatorioNotificacaoUseCase, RelatorioNotificacaoUseCase>();
            services.TryAddScoped<IRelatorioUsuariosUseCase, RelatorioUsuariosUseCase>();
            services.TryAddScoped<IRelatorioAtribuicaoCJUseCase, RelatorioAtribuicaoCJUseCase>();
            services.TryAddScoped<IRelatorioAlteracaoNotasUseCase, RelatorioAlteracaoNotasUseCase>();
            services.TryAddScoped<IRelatorioLeituraComunicadosUseCase, RelatorioLeituraComunicadosUseCase>();
            services.TryAddScoped<IRelatorioAdesaoAppUseCase, RelatorioAdesaoAppUseCase>();
            services.TryAddScoped<IRelatorioPlanejamentoDiarioUseCase, RelatorioPlanejamentoDiarioUseCase>();
            services.TryAddScoped<IRelatorioDevolutivasUseCase, RelatorioDevolutivasUseCase>();
            services.TryAddScoped<IRelatorioDevolutivasSincronoUseCase, RelatorioDevolutivasSincronoUseCase>();
            services.TryAddScoped<IRelatorioItineranciasUseCase, RelatorioItineranciasUseCase>();
            services.TryAddScoped<IRelatorioRegistroIndividualUseCase, RelatorioRegistroIndividualUseCase>();
            services.TryAddScoped<IRelatorioAcompanhamentoAprendizagemUseCase, RelatorioAcompanhamentoAprendizagemUseCase>();
            services.TryAddScoped<IRelatorioAcompanhamentoFechamentoUseCase, RelatorioAcompanhamentoFechamentoUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAtaBimestralUseCase, RelatorioConselhoClasseAtaBimestralUseCase>();
            services.TryAddScoped<IRelatorioAcompanhamentoFrequenciaUseCase, RelatorioAcompanhamentoFrequenciaUseCase>();
            services.TryAddScoped<IRelatorioAcompanhamentoRegistrosPedagogicosUseCase, RelatorioAcompanhamentoRegistrosPedagogicosUseCase>();
            services.TryAddScoped<IRelatorioOcorrenciasUseCase, RelatorioOcorrenciasUseCase>();
            services.TryAddScoped<IRelatorioFrequenciaGlobalUseCase, RelatorioFrequenciaGlobalUseCase>();
        }

        private static void RegistrarOptions(IServiceCollection services, IConfiguration configuration)
        {
            var telemetriaOptions = new TelemetriaOptions();
            configuration.GetSection(TelemetriaOptions.Secao).Bind(telemetriaOptions, c => c.BindNonPublicProperties = true);
            services.AddSingleton(telemetriaOptions);
        }
    }
}