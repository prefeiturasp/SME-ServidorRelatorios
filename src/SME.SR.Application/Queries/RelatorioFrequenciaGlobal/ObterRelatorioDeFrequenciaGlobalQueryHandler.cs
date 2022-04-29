using MediatR;
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
    public class ObterRelatorioDeFrequenciaGlobalQueryHandler : IRequestHandler<ObterRelatorioDeFrequenciaGlobalQuery, List<FrequenciaGlobalDto>>
    {
        private readonly IFrequenciaAlunoRepository _frequenciaAlunoRepository;
        private readonly IMediator _mediator;

        public ObterRelatorioDeFrequenciaGlobalQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository,
            IMediator mediator)
        {
            _frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<FrequenciaGlobalDto>> Handle(ObterRelatorioDeFrequenciaGlobalQuery request, CancellationToken cancellationToken)
        {
            var retornoQuery = await _frequenciaAlunoRepository.ObterFrequenciaAlunoMensal(request.Filtro.ExibirHistorico, request.Filtro.AnoLetivo,
                request.Filtro.CodigoDre, request.Filtro.CodigoUe, request.Filtro.Modalidade, request.Filtro.Semestre, request.Filtro.CodigosTurmas.Select(c => c).ToArray(),
                request.Filtro.MesesReferencias.Select(c => Convert.ToInt32(c)).ToArray(), request.Filtro.ApenasAlunosPercentualAbaixoDe);

            return await MapearRetornoQuery(request.Filtro, retornoQuery);
        }

        private async Task<List<FrequenciaGlobalDto>> MapearRetornoQuery(FiltroFrequenciaGlobalDto filtro,
            IEnumerable<FrequenciaAlunoMensalConsolidadoDto> retornoQuery)
        {

            var retornoMapeado = new List<FrequenciaGlobalDto>();
            var alunosEscola = await _mediator.Send(new ObterDadosAlunosEscolaQuery(filtro.CodigoUe, filtro.AnoLetivo, retornoQuery.Select(c => c.CodigoEol).ToArray()));

            foreach (var item in retornoQuery)
            {
                var aluno = alunosEscola.Select(c => new { c.CodigoAluno, c.NomeAluno, c.NomeSocialAluno })
                    .FirstOrDefault(c => c.CodigoAluno.ToString() == item.CodigoEol);

                var estudante = string.IsNullOrEmpty(aluno.NomeSocialAluno) ? aluno.NomeAluno : aluno.NomeSocialAluno;

                retornoMapeado.Add(new FrequenciaGlobalDto()
                {
                    CodigoDre = item.DreSigla,
                    CodigoUe = string.Concat(item.UeNome, " - ", item.DescricaoTipoEscola),
                    Mes = item.Mes,
                    Turma = string.Concat(ObterModalidade(item.ModalidadeCodigo).ShortName(), " - ", item.TurmaNome),
                    CodigoEOL = item.CodigoEol,
                    Estudante = estudante,
                    PercentualFrequencia = item.Percentual
                });
            }

            var retornoOrdenado = retornoMapeado.OrderBy(c => c.CodigoDre)
                .ThenBy(c => c.CodigoUe)
                .ThenBy(c => c.Mes)
                .ThenBy(c => c.Turma)
                .ThenBy(c => c.Estudante);

            return retornoOrdenado.ToList();
        }

        private Modalidade ObterModalidade(int modalidadeCodigo)
        {
            return Enum.GetValues(typeof(Modalidade))
                .Cast<Modalidade>().FirstOrDefault(x => (int)x == modalidadeCodigo);
        }
    }
}
