using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
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
            var turmas = await ObterTurmasRelatorio(request.TurmaCodigo, request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.ConsideraHistorico);
            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();

            var mediasFrequencia = await ObterMediasFrequencia();

            var alunosPorTurma = await ObterAlunosPorTurmasRelatorio(codigosTurma, request.AlunosCodigo);

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(codigosTurma, request.UeCodigo, request.Modalidade, request.Usuario);
            var tiposNota = await ObterTiposNotaRelatorio(request.AnoLetivo, dre.Id, ue.Id, request.Semestre, request.Modalidade, turmas);

            string[] codigosAlunos = alunosPorTurma.SelectMany(t => t.Select(t => t.CodigoAluno.ToString())).ToArray();

            var notas = await ObterNotasAlunos(codigosTurma, codigosAlunos);
            var frequencias = await ObterFrequenciasAlunos(codigosTurma, codigosAlunos);

            var boletins = await MontarBoletins(dre, ue, turmas, componentesCurriculares, alunosPorTurma, notas, frequencias, tiposNota, mediasFrequencia);

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
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorio(string turmaCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico)
        {
            return await mediator.Send(new ObterTurmasRelatorioBoletimQuery()
            {
                CodigoTurma = turmaCodigo,
                CodigoUe = ueCodigo,
                Modalidade = modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Usuario = usuario,
                ConsideraHistorico = consideraHistorico
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

        private async Task<IDictionary<string, string>> ObterTiposNotaRelatorio(int anoLetivo, long dreId, long ueId, int semestre, Modalidade modalidade, IEnumerable<Turma> turmas)
        {
            return await mediator.Send(new ObterTiposNotaRelatorioBoletimQuery()
            {
                AnoLetivo = anoLetivo,
                DreId = dreId,
                UeId = ueId,
                Semestre = semestre,
                Modalidade = modalidade,
                Turmas = turmas
            });
        }

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterNotasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciasAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterFrequenciasRelatorioBoletimQuery()
            {
                CodigosAluno = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<BoletimEscolarDto> MontarBoletins(Dre dre, Ue ue, IEnumerable<Turma> turmas, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesPorTurma,
                                                             IEnumerable<IGrouping<string, Aluno>> alunosPorTurma, IEnumerable<IGrouping<string, NotasAlunoBimestre>> notasAlunos,
                                                             IEnumerable<IGrouping<string, FrequenciaAluno>> frequenciasAlunos, IDictionary<string, string> tiposNota,
                                                             IEnumerable<MediaFrequencia> mediasFrequencias)
        {
            return await mediator.Send(new MontarBoletinsQuery()
            {
                Dre = dre,
                Ue = ue,
                Turmas = turmas,
                ComponentesCurricularesPorTurma = componentesCurricularesPorTurma,
                AlunosPorTuma = alunosPorTurma,
                Notas = notasAlunos,
                Frequencias = frequenciasAlunos,
                TiposNota = tiposNota,
                MediasFrequencia = mediasFrequencias
            });
        }

        private async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            return await mediator.Send(new ObterParametrosMediaFrequenciaQuery());
        }
    }
}
