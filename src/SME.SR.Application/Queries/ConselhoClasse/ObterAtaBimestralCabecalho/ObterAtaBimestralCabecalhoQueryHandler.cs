using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAtaBimestralCabecalhoQueryHandler : IRequestHandler<ObterAtaBimestralCabecalhoQuery, ConselhoClasseAtaBimestralCabecalhoDto>
    {
        private readonly IMediator mediator;

        public ObterAtaBimestralCabecalhoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ConselhoClasseAtaBimestralCabecalhoDto> Handle(ObterAtaBimestralCabecalhoQuery request, CancellationToken cancellationToken)
        {
            var turma = await ObterTurma(request.TurmaCodigo);
            var dreUe = await ObterDreUe(request.TurmaCodigo);

            return new ConselhoClasseAtaBimestralCabecalhoDto()
            {
                Dre = dreUe?.DreNome,
                Ue = dreUe?.UeNome,
                Turma = turma.Nome,
                ModalidadeResumida = turma.ModalidadeCodigo.ShortName(),
                AnoLetivo = turma.AnoLetivo                  
            };
        }        

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
