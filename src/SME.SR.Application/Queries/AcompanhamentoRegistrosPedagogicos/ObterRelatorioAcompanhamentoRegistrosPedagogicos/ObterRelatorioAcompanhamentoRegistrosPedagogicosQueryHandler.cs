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
    public class ObterRelatorioAcompanhamentoRegistrosPedagogicosQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery, RelatorioAcompanhamentoRegistrosPedagogicosDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioAcompanhamentoRegistrosPedagogicosQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<RelatorioAcompanhamentoRegistrosPedagogicosDto> Handle(ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery request, CancellationToken cancellationToken)
        {
            Dre dre = null;
            Ue ue = null;

            if (!string.IsNullOrEmpty(request.DreCodigo))
                dre = await ObterDrePorCodigo(request.DreCodigo);

            if (!string.IsNullOrEmpty(request.UeCodigo))
                ue = await ObterUePorCodigo(request.UeCodigo);

            int[] bimestres = request.Bimestres?.ToArray();

            //return await mediator.Send(new MontarRelatorioAcompanhamentoFechamentoQuery(dre, ue, request.Turmas?.ToArray(), turmas, componentesCurriculares, bimestres, consolidadoFechamento, consolidadoConselhosClasse, request.ListarPendencias, pendencias, request.Usuario));
            return null;
        }
        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorioPorSituacaoConsolidacao(string[] turmasCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres)
        {
            try
            {
                return await mediator.Send(new ObterTurmasRelatorioAcompanhamentoFechamentoQuery()
                {
                    CodigosTurma = turmasCodigo,
                    CodigoUe = ueCodigo,
                    Modalidade = modalidade,
                    AnoLetivo = anoLetivo,
                    Semestre = semestre,
                    Usuario = usuario,
                    ConsideraHistorico = consideraHistorico,
                    SituacaoConselhoClasse = situacaoConselhoClasse,
                    SituacaoFechamento = situacaoFechamento,
                    Bimestres = bimestres

                });
            }
            catch (NegocioException)
            {
                throw new NegocioException("As turmas selecionadas não possuem fechamento.");
            }
        }

        private async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidado(string[] turmasId)
        {
            return await mediator.Send(new ObterConselhosClasseConsolidadoPorTurmasQuery(turmasId));
        }

        private async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentosConsolidado(string[] turmasId)
        {
            return await mediator.Send(new ObterFechamentoConsolidadoPorTurmasQuery(turmasId));
        }
    }
}
