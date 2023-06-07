using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterListagemItineranciasQueryHandler : IRequestHandler<ObterListagemItineranciasQuery, IEnumerable<ListagemItineranciaDto>>
    {
        private readonly IItineranciaRepository itineranciaRepository;
        private readonly IMediator mediator;

        public ObterListagemItineranciasQueryHandler(IItineranciaRepository itineranciaRepository, IMediator mediator)
        {
            this.itineranciaRepository = itineranciaRepository ?? throw new System.ArgumentNullException(nameof(itineranciaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<ListagemItineranciaDto>> Handle(ObterListagemItineranciasQuery request, CancellationToken cancellationToken)
        {
            
            var itinerancias = await itineranciaRepository.ObterItinerancias(request.filtro);
            var nomesAlunos = await mediator.Send(new ObterNomesAlunosPorCodigosQuery(itinerancias.SelectMany(itinerancia => itinerancia.Alunos).Select(aluno => aluno.Codigo).Distinct().ToArray()));
            return MapearParaDto(itinerancias.ToList(), nomesAlunos.ToList());
        }

        private IEnumerable<ListagemItineranciaDto> MapearParaDto (List<ListagemItineranciaDto> itinerancias, List<AlunoNomeDto> alunos)
        {
            itinerancias.ForEach(itinerancia => itinerancia.Alunos.ForEach(aluno => aluno.Nome = alunos.Where(a => a.Codigo == aluno.Codigo).FirstOrDefault()?.Nome));
            return itinerancias;
        }

    }
}
