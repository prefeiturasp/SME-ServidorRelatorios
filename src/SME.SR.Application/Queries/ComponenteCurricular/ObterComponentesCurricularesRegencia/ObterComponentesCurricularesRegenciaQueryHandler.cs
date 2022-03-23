using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesRegenciaQueryHandler : IRequestHandler<ObterComponentesCurricularesRegenciaQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private static readonly long[] IDS_COMPONENTES_REGENCIA = { 2, 7, 8, 89, 138 };
        private readonly IMediator mediator;
        private readonly IAtribuicaoCJRepository atribuicaoCJRepository;

        public ObterComponentesCurricularesRegenciaQueryHandler(IMediator mediator, IAtribuicaoCJRepository atribuicaoCJRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.atribuicaoCJRepository = atribuicaoCJRepository ?? throw new ArgumentNullException(nameof(atribuicaoCJRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesRegenciaQuery request, CancellationToken cancellationToken)
        {
            Turma turma = request.Turma;

            if (request.Usuario.EhProfessorCj())
                return await ObterComponentesCJ(turma.ModalidadeCodigo, turma.Codigo, turma.Ue.Codigo, request.CdComponenteCurricular, request.Usuario.CodigoRf);
            else
            {
                var componentesCurriculares = await mediator.Send(new
                    ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery()
                {
                    CodigoTurma = turma.Codigo,
                    Usuario = request.Usuario
                });


                return componentesCurriculares.Where(c => c.Regencia);
            }
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCJ(Modalidade modalidade, string codigoTurma, string ueId, long codigoDisciplina, string rf, bool ignorarDeParaRegencia = false)
        {
            IEnumerable<ComponenteCurricularPorTurma> componentes = null;
            var atribuicoes = await atribuicaoCJRepository.ObterPorFiltros(modalidade,
                codigoTurma,
                ueId,
                codigoDisciplina,
                rf,
                string.Empty,
                true);

            if (atribuicoes == null || !atribuicoes.Any())
                return null;

            var turmasCodigo = atribuicoes.Select(a => a.Turma.Codigo).Distinct().ToArray();

            var disciplinasEol = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery() { ComponentesCurricularesIds = atribuicoes.Select(a => a.ComponenteCurricularId).Distinct().ToArray(), TurmasId = turmasCodigo });

            var componenteRegencia = disciplinasEol?.FirstOrDefault(c => c.Regencia);
            if (componenteRegencia == null || ignorarDeParaRegencia)
                return disciplinasEol;

            var componentesRegencia = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery() { ComponentesCurricularesIds = IDS_COMPONENTES_REGENCIA, TurmasId = turmasCodigo });
            if (componentesRegencia != null)
                return componentesRegencia;

            return componentes;
        }
    }
}
