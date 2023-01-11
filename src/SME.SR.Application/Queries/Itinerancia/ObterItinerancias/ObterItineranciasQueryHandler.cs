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
    public class ObterItineranciasQueryHandler : IRequestHandler<ObterItineranciasQuery, IEnumerable<RegistrosRegistroItineranciaDto>>
    {
        private readonly IItineranciaRepository itineranciaRepository;
        private readonly IMediator mediator;

        public ObterItineranciasQueryHandler(IItineranciaRepository itineranciaRepository, IMediator mediator)
        {
            this.itineranciaRepository = itineranciaRepository ?? throw new System.ArgumentNullException(nameof(itineranciaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<RegistrosRegistroItineranciaDto>> Handle(ObterItineranciasQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Itinerancias.ToArray();

            var itinerancias = await itineranciaRepository.ObterComUEDREPorIds(ids);
            var objetivos = await itineranciaRepository.ObterObjetivosPorItineranciaIds(ids);
            var questoesItinerancias = (await itineranciaRepository.ObterQuestoesPorItineranciaIds(ids)).Where(x => x.TipoQuestao != TipoQuestao.Upload);
            var alunosItinerancias = await itineranciaRepository.ObterAlunosPorItineranciaIds(ids);
            var nomesAlunos = await mediator.Send(new ObterNomesAlunosPorCodigosQuery(alunosItinerancias.Select(a => a.AlunoCodigo).ToArray()));

            return MapearParaDto(itinerancias, objetivos, questoesItinerancias, alunosItinerancias, nomesAlunos);
        }

        private IEnumerable<RegistrosRegistroItineranciaDto> MapearParaDto(IEnumerable<Itinerancia> itinerancias, IEnumerable<ItineranciaObjetivoDto> objetivos, IEnumerable<ItineranciaQuestaoDto> questoesItinerancias, IEnumerable<ItineranciaAlunoDto> alunosItinerancias, IEnumerable<AlunoNomeDto> nomesAlunos)
        {
            foreach (var itinerancia in itinerancias)
            {
                var dataRetorno = itinerancia.DataRetorno.HasValue ? $"{itinerancia.DataRetorno:dd/MM/yyyy}" : "";
                var itineranciaDto = new RegistrosRegistroItineranciaDto()
                {
                    Dre = itinerancia.Ue.Dre.Abreviacao,
                    Ue = itinerancia.Ue.NomeRelatorio,
                    DataVisita = $"{itinerancia.DataVisita:dd/MM/yyyy}",
                    DataRetorno = dataRetorno,
                    Objetivos = ObterObjetivosItinerancia(itinerancia.Id, objetivos),
                    Alunos = ObterAlunosItinerancia(itinerancia.Id, alunosItinerancias, nomesAlunos)
                };

                MapearQuestoesItinerancia(itinerancia.Id, itineranciaDto, questoesItinerancias);

                yield return itineranciaDto;
            }
        }

        private IEnumerable<AlunoRegistroItineranciaDto> ObterAlunosItinerancia(long id, IEnumerable<ItineranciaAlunoDto> alunosItinerancias, IEnumerable<AlunoNomeDto> nomesAlunos)
        {
            foreach(var alunoItinerancia in alunosItinerancias.Where(c => c.ItineranciaId == id))
            {
                var nomeAluno = nomesAlunos.FirstOrDefault(c => c.Codigo == alunoItinerancia.AlunoCodigo)?.Nome ?? "";

                yield return new AlunoRegistroItineranciaDto()
                {
                    Estudante = $"{nomeAluno} ({alunoItinerancia.AlunoCodigo})",
                    DescritivoEstudante = UtilRegex.RemoverTagsHtml(alunoItinerancia.Questoes.FirstOrDefault(c => c.Ordem == 0)?.Resposta),
                    AcompanhamentoSituacao = UtilRegex.RemoverTagsHtml(alunoItinerancia.Questoes.FirstOrDefault(c => c.Ordem == 1)?.Resposta),
                    Encaminhamentos = UtilRegex.RemoverTagsHtml(alunoItinerancia.Questoes.FirstOrDefault(c => c.Ordem == 2)?.Resposta)
                };
            }
        }

        private void MapearQuestoesItinerancia(long id, RegistrosRegistroItineranciaDto itineranciaDto, IEnumerable<ItineranciaQuestaoDto> questoesItinerancias)
        {
            var questoes = questoesItinerancias.Where(c => c.ItineranciaId == id);

            if (questoes != null && questoes.Any())
            {
                var questaoAcompanhamento = questoes.FirstOrDefault(c => c.Ordem == 0);
                var questaoEncaminhamento = questoes.FirstOrDefault(c => c.Ordem == 1);

                if (questaoAcompanhamento != null)
                    itineranciaDto.AcompanhamentoSituacao = UtilRegex.RemoverTagsHtml(questaoAcompanhamento.Resposta);

                if (questaoEncaminhamento != null)
                    itineranciaDto.Encaminhamentos = UtilRegex.RemoverTagsHtml(questaoEncaminhamento.Resposta);
            }
        }

        private IEnumerable<ObjetivosRegistroItineranciaDto> ObterObjetivosItinerancia(long id, IEnumerable<ItineranciaObjetivoDto> objetivos)
        {
            foreach (var objetivo in objetivos
                .Where(c => c.ItineranciaId == id)
                .OrderBy(a => a.Ordem))
            {
                var descricao = !string.IsNullOrEmpty(objetivo.Descricao) ? $": {objetivo.Descricao}" : "";
                yield return new ObjetivosRegistroItineranciaDto()
                {
                    NomeObjetivo = $"{objetivo.Nome}{descricao}"
                };
            }
        }
    }
}
