using MediatR;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioPlanoAulaUseCase : IRelatorioPlanoAulaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanoAulaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<ObterPlanoAulaFiltroQuery>();

            var planoAula = await mediator.Send(new ObterPlanoAulaQuery(filtros.PlanoAulaId));

            var turma = await mediator.Send(new ObterTurmaQuery() { CodigoTurma = planoAula.TurmaCodigo });

            if (planoAula == null)
                throw new NegocioException("Plano de aula não encontrado.");


            var regencia = await mediator.Send(new ObterComponentesCurricularesRegenciaQuery()
            {
                CdComponenteCurricular = planoAula.ComponenteCurricularId,
                Usuario = filtros.Usuario,
                Turma = turma
            });
            if (regencia != null)
                planoAula.ComponenteCurricular = "Regência de Classe";

            planoAula.Objetivos = await mediator.Send(new ObterPlanoAulaObjetivoAprendizagemQuery(filtros.PlanoAulaId));
            planoAula.Usuario = filtros.Usuario.Nome;
            planoAula.RF = filtros.Usuario.CodigoRf;

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPlanoAula", planoAula, request.CodigoCorrelacao));
        }
    }
}
