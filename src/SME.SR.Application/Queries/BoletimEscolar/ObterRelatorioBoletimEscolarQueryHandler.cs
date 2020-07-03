using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, RelatorioBoletimEscolarDto>
    {
        private IMediator _mediator;

        public ObterRelatorioBoletimEscolarQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<RelatorioBoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            var dre = await ObterDrePorCodigo(request.DreCodigo);
            var ue = await ObterUePorCodigo(request.UeCodigo);
            var turmas = await ObterTurmasRelatorio(request.TurmaCodigo, request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario);

            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();

            var alunosPorTurma = await ObterAlunosPorTurmasRelatorio(codigosTurma, request.AlunosCodigo);

            string[] codigosAlunos = alunosPorTurma.SelectMany(t => t.Select(t => t.CodigoAluno.ToString())).ToArray();

            var notasFrequencia = await ObterNotasFrequenciaAlunos(codigosTurma, codigosAlunos);

            return new RelatorioBoletimEscolarDto(new BoletimEscolarDto());
        }

        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await _mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await _mediator.Send(new ObterUePorCodigoQuery()
            {
                UeCodigo = ueCodigo
            });
        }

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorio(string turmaCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario)
        {
            return await _mediator.Send(new ObterTurmasRelatorioBoletimQuery()
            {
                CodigoTurma = turmaCodigo,
                CodigoUe = ueCodigo,
                Modalidade = modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Usuario = usuario
            });
        }

        private async Task<IEnumerable<IGrouping<int, Aluno>>> ObterAlunosPorTurmasRelatorio(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await _mediator.Send(new ObterAlunosTurmasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<IEnumerable<NotasFrequenciaAlunoBimestre>> ObterNotasFrequenciaAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await _mediator.Send(new ObterNotasFrequenciaRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }
    }
}
