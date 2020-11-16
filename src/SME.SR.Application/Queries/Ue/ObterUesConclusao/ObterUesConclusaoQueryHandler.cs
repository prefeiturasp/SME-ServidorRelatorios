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
    public class ObterUesConclusaoQueryHandler : IRequestHandler<ObterUesConclusaoQuery, IEnumerable<IGrouping<long, UeConclusaoPorAlunoAno>>>
    {
        public IMediator mediator;

        public ObterUesConclusaoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IEnumerable<IGrouping<long, UeConclusaoPorAlunoAno>>> Handle(ObterUesConclusaoQuery request, CancellationToken cancellationToken)
        {

            var pareceresConclusivosIds = await mediator.Send(new ObterPareceresConclusivosPorTipoAprovacaoQuery(true));
            if (!pareceresConclusivosIds.Any())
                throw new NegocioException("Não foi possível localizar os pareceres conclusivos.");

            //Obter informações dos alunos
            var informacoesDosAlunos = await ObterInformacoesDosAlunos(request.CodigosAlunos);
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possível obter a informação dos alunos.");

            //Obter as turmas dos Alunos
            var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigosAlunos, pareceresConclusivosIds.ToArray()));

            if (!turmasDosAlunos.Any())
                throw new NegocioException("Não foi possível obter os dados das turmas dos alunos.");

            var ues = await mediator.Send(new ObterUePorCodigosQuery(informacoesDosAlunos.Select(u => u.CodigoEscola).Distinct().ToArray()));

            var uesConclusao = new List<UeConclusaoPorAlunoAno>();

            foreach (var turmaAluno in turmasDosAlunos)
            {
                var turmaAlunoEol = informacoesDosAlunos.FirstOrDefault(a => a.CodigoAluno == turmaAluno.AlunoCodigo &&
                                                                             a.CodigoTurma.ToString() == turmaAluno.TurmaCodigo &&
                                                                             a.ParecerConclusivo == turmaAluno.ParecerConclusivo);

                if (turmaAlunoEol != null)
                {
                    var ue = ues.FirstOrDefault(ue => ue.Codigo == turmaAlunoEol.CodigoEscola);

                    uesConclusao.Add(new UeConclusaoPorAlunoAno()
                    {
                        AlunoCodigo = turmaAluno.AlunoCodigo,
                        TurmaAno = turmaAluno.Ano,
                        UeCodigo = ue.Codigo,
                        UeNome = ue.NomeComTipoEscola
                    });
                }
            }

            return uesConclusao.GroupBy(g => g.AlunoCodigo);

        }

        private async Task<IEnumerable<AlunoHistoricoEscolar>> ObterInformacoesDosAlunos(long[] codigoAlunos)
        {
            var informacoesDosAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(codigoAlunos));
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possíve obter os dados dos alunos");

            informacoesDosAlunos = informacoesDosAlunos.GroupBy(d => d.CodigoAluno)
                                  .SelectMany(g => g.OrderByDescending(d => d.AnoLetivo)
                                                    .ThenByDescending(m => m.DataSituacao)
                                                    .Take(1));

            return informacoesDosAlunos;
        }
    }
}
