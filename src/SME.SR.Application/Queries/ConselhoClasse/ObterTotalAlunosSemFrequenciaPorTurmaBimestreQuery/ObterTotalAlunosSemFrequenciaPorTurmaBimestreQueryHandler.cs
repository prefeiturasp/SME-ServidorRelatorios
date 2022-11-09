using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterTotalAlunosSemFrequenciaPorTurmaBimestreQueryHandler : IRequestHandler<ObterTotalAlunosSemFrequenciaPorTurmaBimestreQuery, IEnumerable<TotalAulasTurmaDisciplinaDto>>
    {
        private readonly IConselhoClasseRepository repositorioConselhoClasse;
        public ObterTotalAlunosSemFrequenciaPorTurmaBimestreQueryHandler(IConselhoClasseRepository repositorioConselhoClasse)
        {
            this.repositorioConselhoClasse = repositorioConselhoClasse;
        }
        public async Task<IEnumerable<TotalAulasTurmaDisciplinaDto>> Handle(ObterTotalAlunosSemFrequenciaPorTurmaBimestreQuery request, CancellationToken cancellationToken)
        {
            return await repositorioConselhoClasse.ObterTotalAulasSemFrequenciaPorTurmaBismetre(request.DisciplinaId, request.CodigoTurma, request.Bimestre);
        }
    }
}
