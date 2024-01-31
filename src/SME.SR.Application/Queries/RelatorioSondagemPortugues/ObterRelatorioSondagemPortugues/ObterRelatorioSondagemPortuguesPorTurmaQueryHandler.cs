using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesPorTurmaQuery, IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>>
    {
        private readonly IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemPortuguesPorTurmaQueryHandler(
            IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository,
            IMediator mediator)
        {
            this.relatorioSondagemPortuguesPorTurmaRepository = relatorioSondagemPortuguesPorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesPorTurmaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> Handle(ObterRelatorioSondagemPortuguesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            if (request.Proficiencia != ProficienciaSondagemEnum.Leitura &&
                request.Proficiencia != ProficienciaSondagemEnum.Escrita &&
                (request.Proficiencia != ProficienciaSondagemEnum.Autoral && request.Grupo != GrupoSondagemEnum.LeituraVozAlta))
                throw new NegocioException($"{ request.Proficiencia } fora do esperado.");

            string nomeColunaBimestre = ObterNomeColunaBimestre(request.Bimestre, request.Proficiencia);

            if (nomeColunaBimestre == String.Empty && request.Proficiencia != ProficienciaSondagemEnum.Autoral)
                throw new NegocioException($"Nome da coluna do bimestre não pode ser vazio.");

            return await relatorioSondagemPortuguesPorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.UeCodigo, request.TurmaCodigo, request.AnoLetivo, request.AnoTurma, request.Bimestre, request.Proficiencia, nomeColunaBimestre, request.Grupo, request.Semestre);
        }

        private String ObterNomeColunaBimestre(int bimestre, ProficienciaSondagemEnum proficiencia)
        {
            string nomeColunaBimestre = String.Empty;

            if (bimestre == 0) bimestre = 1;

            if (proficiencia == ProficienciaSondagemEnum.Leitura)
                nomeColunaBimestre = $"reading{bimestre}B";

            if (proficiencia == ProficienciaSondagemEnum.Escrita)
                nomeColunaBimestre = $"writing{bimestre}B";

            return nomeColunaBimestre;
        }
    }
}
