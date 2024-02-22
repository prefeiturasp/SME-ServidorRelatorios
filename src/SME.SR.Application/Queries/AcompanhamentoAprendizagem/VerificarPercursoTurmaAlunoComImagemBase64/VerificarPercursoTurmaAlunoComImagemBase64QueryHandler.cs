using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class VerificarPercursoTurmaAlunoComImagemBase64QueryHandler : IRequestHandler<VerificarPercursoTurmaAlunoComImagemBase64Query, IEnumerable<AcompanhamentoTurmaAlunoImagemBase64Dto>>
    {
        private readonly IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository;

        public VerificarPercursoTurmaAlunoComImagemBase64QueryHandler(IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository)
        {
            this.acompanhamentoAprendizagemRepository = acompanhamentoAprendizagemRepository ?? throw new ArgumentNullException(nameof(acompanhamentoAprendizagemRepository));
        }

        public async Task<IEnumerable<AcompanhamentoTurmaAlunoImagemBase64Dto>> Handle(VerificarPercursoTurmaAlunoComImagemBase64Query request, CancellationToken cancellationToken)
        {
            return await acompanhamentoAprendizagemRepository
                .ObterInformacoesAcompanhamentoComImagemBase64TurmaAlunos(request.TurmaId, request.Semestre, request.CodigoAluno, request.TagsImagemConsideradas);
        }
    }
}
