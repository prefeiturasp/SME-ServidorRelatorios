using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.JRSClient;
using SME.SR.Workers.SGP.Services;
using System;
using System.Linq;

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
            services.AddHostedService<RabbitBackgroundListener>();

            //TODO: Informaçoes do arquivo de configuração
            services.AddJasperClient("http://127.0.0.1:8080", "user", "bitnami");

            services.AddSingleton(new VariaveisAmbiente());

            // TODO: Criar arquivo especficio para as injeções
            RegistrarRepositorios(services);
			RegistrarQueries(services);
			RegistrarCommands(services);
            RegistrarUseCase(services);
        }

        private void RegistrarRepositorios(IServiceCollection services)
        {
            services.TryAddScoped<IExemploRepository, ExemploRepository>();
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
			services.TryAddScoped(typeof(IAlunoRepository), typeof(AlunoRepository));
            services.TryAddScoped(typeof(IAulaRepository), typeof(AulaRepository));
            services.TryAddScoped(typeof(IComponenteCurricularRepository), typeof(ComponenteCurricularRepository));
            services.TryAddScoped(typeof(IConselhoClasseAlunoRepository), typeof(ConselhoClasseAlunoRepository));
            services.TryAddScoped(typeof(IConselhoClasseNotaRepository), typeof(ConselhoClasseNotaRepository));
            services.TryAddScoped(typeof(IConselhoClasseRecomendacaoRepository), typeof(ConselhoClasseRecomendacaoRepository));
            services.TryAddScoped(typeof(IEolRepository), typeof(EolRepository));
            services.TryAddScoped(typeof(IFechamentoAlunoRepository), typeof(FechamentoAlunoRepository));
            services.TryAddScoped(typeof(IFechamentoNotaRepository), typeof(FechamentoNotaRepository));
            services.TryAddScoped(typeof(IFechamentoTurmaRepository), typeof(FechamentoTurmaRepository));
            services.TryAddScoped(typeof(IFrequenciaAlunoRepository), typeof(FrequenciaAlunoRepository));
            services.TryAddScoped(typeof(IParametroSistemaRepository), typeof(ParametroSistemaRepository));
            services.TryAddScoped(typeof(IPeriodoEscolarRepository), typeof(PeriodoEscolarRepository));
            services.TryAddScoped(typeof(ITipoCalendarioRepository), typeof(TipoCalendarioRepository));
            services.TryAddScoped(typeof(ITurmaRepository), typeof(TurmaRepository));
        }

        private void RegistrarCommands(IServiceCollection services)
        {
            services.AddMediatR(typeof(RelatorioDadosAlunoCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(RelatorioDadosAlunoCommandHandler).GetTypeInfo().Assembly);
		}
		
        private void RegistrarQueries(IServiceCollection services)
        {
            services.AddMediatR(typeof(ObterAlunosPorTurmaQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterAnotacoesAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterComponentesCurricularesPorTurmaQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteComNotaBimestreQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteComNotaFinalQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteSemNotaBimestreQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteSemNotaFinalQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDreUePorTurmaQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterFrequenciaAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterFrequenciaGlobalPorAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterFrequenciaPorDisciplinaBimestresQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterNotasAlunoBimestreQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterNotasConselhoClasseAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterParametroSistemaPorTipoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterParecerConclusivoPorAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterRecomendacoesPorFechamentoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterRelatorioConselhoClasseAlunoQuery).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterTurmaPeriodoFechamentoPorIdQuery).GetTypeInfo().Assembly);
		}
		
        private void RegistrarHandlers(IServiceCollection services)
        {
            services.AddMediatR(typeof(ObterAlunosPorTurmaQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterAnotacoesAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterComponentesCurricularesPorTurmaQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteComNotaBimestreQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteComNotaFinalQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteSemNotaBimestreQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDadosComponenteSemNotaFinalQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterDreUePorTurmaQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterFrequenciaGlobalPorAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterFrequenciaPorDisciplinaBimestresQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterFrequenciaAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterNotasAlunoBimestreQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterNotasConselhoClasseAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterParecerConclusivoPorAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterRecomendacoesPorFechamentoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterRelatorioConselhoClasseAlunoQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterTurmaPeriodoFechamentoPorIdQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterParametroSistemaPorTipoQueryHandler).GetTypeInfo().Assembly);
		}
		
        private void RegistrarUseCase(IServiceCollection services)
        {
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
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
