using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Linq;
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

            if (planoAula == null)
                throw new NegocioException("Plano de aula não encontrado.");

            var componenteCurricular = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(planoAula.ComponenteCurricularId));

            planoAula.ComponenteCurricular = componenteCurricular.FirstOrDefault().Disciplina;
            planoAula.Objetivos = await mediator.Send(new ObterPlanoAulaObjetivoAprendizagemQuery(filtros.PlanoAulaId));
            planoAula.Descricao = planoAula.Descricao != null ? planoAula.Descricao : "";
            planoAula.LicaoCasa = planoAula.LicaoCasa != null ? planoAula.LicaoCasa : "";
            planoAula.Recuperacao = planoAula.Recuperacao != null ? planoAula.Recuperacao : "";

            planoAula.Usuario = filtros.Usuario.Nome;
            planoAula.RF = filtros.Usuario.CodigoRf;

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPlanoAula", planoAula, request.CodigoCorrelacao));
        }
    }
}
