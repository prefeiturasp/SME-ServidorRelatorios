using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
using SME.SR.Workers.SGP.Configuracoes;
using SME.SR.Workers.SGP.Services;
using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace SME.SR.Workers.SGP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });
            var assembly = AppDomain.CurrentDomain.Load("SME.SR.Application");
            services.AddMediatR(assembly);

            services.AddControllers();
            services.AddMvc().AddControllersAsServices();
            services.AddRabbitMQ(Configuration);
            services.AddHostedService<RabbitBackgroundListener>();

            //TODO: Informa�oes do arquivo de configura��o

            var cookieContainer = new CookieContainer();
            var jasperCookieHandler = new JasperCookieHandler() { CookieContainer = cookieContainer };

            services.AddSingleton(jasperCookieHandler);

            var urlJasper = Configuration.GetValue<string>("ConfiguracaoJasper:Hostname");
            var usuarioJasper = Configuration.GetValue<string>("ConfiguracaoJasper:Username");
            var senhaJasper = Configuration.GetValue<string>("ConfiguracaoJasper:Password");


            services.AddHttpClient<IExecucaoRelatorioService, ExecucaoRelatorioService>(c =>
            {
                c.BaseAddress = new Uri(urlJasper);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new JasperCookieHandler() { CookieContainer = cookieContainer }; 
                });

            services.AddJasperClient(urlJasper, usuarioJasper, senhaJasper);

            services.AddSingleton(new VariaveisAmbiente());

            // TODO: Criar arquivo especficio para as inje��es
            RegistrarRepositorios(services);
            RegistrarCommands(services);
            RegistrarUseCase(services);
            RegistrarServicos(services);
        }

        private void RegistrarRepositorios(IServiceCollection services)
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
            services.TryAddScoped(typeof(IDreRepository), typeof(DreRepository));
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
            services.TryAddScoped(typeof(IUeRepository), typeof(UeRepository));
        }

        private void RegistrarCommands(IServiceCollection services)
        {
            services.AddMediatR(typeof(GerarRelatorioAssincronoCommand).GetTypeInfo().Assembly);
        }
		
        private void RegistrarUseCase(IServiceCollection services)
        {
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
            services.TryAddScoped<IMonitorarStatusRelatorioUseCase, MonitorarStatusRelatorioUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAlunoUseCase, RelatorioConselhoClasseAlunoUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseTurmaUseCase, RelatorioConselhoClasseTurmaUseCase>();
            services.TryAddScoped<IRelatorioBoletimEscolarUseCase, RelatorioBoletimEscolarUseCase>();
            services.TryAddScoped<IRelatorioConselhoClasseAtaFinalUseCase, RelatorioConselhoClasseAtaFinalUseCase>();
        }

        private void RegistrarServicos(IServiceCollection services)
        {
            services.TryAddScoped<IServicoFila, FilaRabbit>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseRouting()
                .UseAuthorization()                
                .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
