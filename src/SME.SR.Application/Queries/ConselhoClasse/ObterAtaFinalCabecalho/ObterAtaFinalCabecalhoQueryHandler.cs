using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Application
{
    public class ObterAtaFinalCabecalhoQueryHandler : IRequestHandler<ObterAtaFinalCabecalhoQuery, ConselhoClasseAtaFinalCabecalhoDto>
    {
        private readonly IMediator mediator;

        public ObterAtaFinalCabecalhoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ConselhoClasseAtaFinalCabecalhoDto> Handle(ObterAtaFinalCabecalhoQuery request, CancellationToken cancellationToken)
        {
            var turma = await ObterTurma(request.TurmaCodigo);
            var dreUe = await ObterDreUe(request.TurmaCodigo);
            var ciclo = await ObterCiclo(request.TurmaCodigo);

            return new ConselhoClasseAtaFinalCabecalhoDto()
            {
                Dre = dreUe?.DreNome,
                Ue = dreUe?.UeNome,
                Ciclo = ciclo,
                Turma = $"{turma.ModalidadeCodigo.ShortName()} - {turma.Nome}",
                AnoLetivo = turma.AnoLetivo,
                Data = DateTime.Now.ToString("dd/MM/yyyy")
            };
        }

        private async Task<string> ObterCiclo(string turmaCodigo)
            => await mediator.Send(new ObterCicloAprendizagemTurmaQuery(turmaCodigo));

        private async Task<DreUe> ObterDreUe(string turmaCodigo)
        {
            var dreUe = await mediator.Send(new ObterDreUePorTurmaQuery(turmaCodigo));

            return dreUe;
        }

        private async Task<Turma> ObterTurma(string turmaCodigo)
        {
            var turma = await mediator.Send(new ObterTurmaQuery(turmaCodigo));
            if (turma == null)
                throw new Exception("Turma não localizada");

            return turma;
        }
    }
}
