using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class RelatorioRecuperacaoParalelaUseCase : IRelatorioRecuperacaoParalelaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioRecuperacaoParalelaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioRecuperacaoParalelaDto>();

            var alunos = new List<Aluno>();

            if (!string.IsNullOrEmpty(filtros.AlunoCodigo))
            {
                var aluno = await mediator.Send(new ObterDadosAlunoQuery()
                {
                    CodigoAluno = filtros.AlunoCodigo,
                    CodigoTurma = filtros.TurmaCodigo
                });

                if (aluno == null)
                    throw new NegocioException($"Não foi possível localizar dados do aluno {filtros.AlunoCodigo}");

                alunos.Add(aluno);
            }
            else
            {
                alunos = (await mediator.Send(new ObterAlunosPorTurmaQuery()
                {
                    TurmaCodigo = filtros.TurmaCodigo
                })).ToList();
            }

            //var resultado = await mediator.Send(new ObterRelatorioFechamentoPedenciasQuery() { filtroRelatorioPendenciasFechamentoDto = filtros });

            //await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFechamentoPendencias", resultado, request.CodigoCorrelacao));

        }
    }
}