using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterProfessorTitularComponenteCurricularPorCodigosRfQueryHandler : IRequestHandler<ObterProfessorTitularComponenteCurricularPorCodigosRfQuery, IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        private readonly IProfessorRepository professorRepository;
        private readonly IFuncionarioRepository funcionarioRepository;
        private readonly IMediator mediator;

        public ObterProfessorTitularComponenteCurricularPorCodigosRfQueryHandler(IProfessorRepository professorRepository, IMediator mediator, IFuncionarioRepository funcionarioRepository)
        {
            this.professorRepository = professorRepository ?? throw new ArgumentNullException(nameof(professorRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> Handle(ObterProfessorTitularComponenteCurricularPorCodigosRfQuery request, CancellationToken cancellationToken)
        {
            var lstProfessores = (await professorRepository.BuscarProfessorTitularComponenteCurricularPorCodigosRf(request.CodigosRf)).ToList();
            lstProfessores.AddRange(await ObterProfessorTitularComponenteCurricularAgrupamentoTerritorioSaberPorCodigosRfQuery(request.CodigosRf));
            return lstProfessores;
        }

        private async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> ObterProfessorTitularComponenteCurricularAgrupamentoTerritorioSaberPorCodigosRfQuery(string[] codigosRf)
        {
            var lstProfessores = (await professorRepository.BuscarProfessorTitularComponenteCurricularAgrupamentoTerritorioSaberPorCodigosRf(codigosRf)).ToList();
            if (lstProfessores.Any())
            {
                var informacoesComponentesCurriculares = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(lstProfessores.Select(p => long.Parse(p.ComponenteCurricularId)).ToArray()));
                var informacoesProfessores = await funcionarioRepository.ObterNomesServidoresPorRfs(codigosRf);
                foreach (var profTitular in lstProfessores)
                {
                    profTitular.ComponenteCurricular = informacoesComponentesCurriculares.FirstOrDefault(cc => cc.CodDisciplina.ToString() == profTitular.ComponenteCurricularId)?.Disciplina;
                    profTitular.NomeProfessor = informacoesProfessores.FirstOrDefault(f => f.CodigoRF.ToString() == profTitular.ProfessorRf)?.NomeServidor;
                }
            }
            return lstProfessores;
        }
    }
}
