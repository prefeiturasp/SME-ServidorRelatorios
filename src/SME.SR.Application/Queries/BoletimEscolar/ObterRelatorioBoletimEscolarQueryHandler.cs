using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, RelatorioBoletimEscolarDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioBoletimEscolarQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<RelatorioBoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            var dre = await ObterDrePorCodigo(request.DreCodigo);
            var ue = await ObterUePorCodigo(request.UeCodigo);
            var turmas = await ObterTurmasRelatorio(request.TurmaCodigo, request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario);

            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(turmas.Select(t => t.Codigo).ToArray(), request.UeCodigo, request.Modalidade, request.Usuario);

            var alunosPorTurma = await ObterAlunosPorTurmasRelatorio(codigosTurma, request.AlunosCodigo);

            string[] codigosAlunos = alunosPorTurma.SelectMany(t => t.Select(t => t.CodigoAluno.ToString())).ToArray();

            var notasFrequencia = await ObterNotasFrequenciaAlunos(codigosTurma, codigosAlunos);

            var boletins = await MontarBoletins(dre, ue, turmas, componentesCurriculares, alunosPorTurma, notasFrequencia);

            return new RelatorioBoletimEscolarDto(boletins);
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
            return await mediator.Send(new ObterUePorCodigoQuery()
            {
                UeCodigo = ueCodigo
            });
        }

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorio(string turmaCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario)
        {
            return await mediator.Send(new ObterTurmasRelatorioBoletimQuery()
            {
                CodigoTurma = turmaCodigo,
                CodigoUe = ueCodigo,
                Modalidade = modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Usuario = usuario
            });
        }

        private async Task<IEnumerable<IGrouping<string, Aluno>>> ObterAlunosPorTurmasRelatorio(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterAlunosTurmasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade, Usuario usuario)
        {
            return await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioBoletimQuery()
            {
                CodigosTurma = turmaCodigo,
                CodigoUe = codigoUe,
                Modalidade = modalidade,
                Usuario = usuario
            });
        }

        private async Task<IEnumerable<IGrouping<string, NotasFrequenciaAlunoBimestre>>> ObterNotasFrequenciaAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterNotasFrequenciaRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<BoletimEscolarDto> MontarBoletins(Dre dre, Ue ue, IEnumerable<Turma> turmas, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesPorTurma,
                                                             IEnumerable<IGrouping<string, Aluno>> alunosPorTurma, IEnumerable<IGrouping<string, NotasFrequenciaAlunoBimestre>> notasFrequenciaAlunos)
        {
            return await mediator.Send(new MontarBoletinsQuery()
            {
                Dre = dre,
                Ue = ue,
                Turmas = turmas,
                ComponentesCurricularesPorTurma = componentesCurricularesPorTurma,
                AlunosPorTuma = alunosPorTurma,
                NotasFrequencia = notasFrequenciaAlunos
            });
        }
    }
}
