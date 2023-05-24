using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioAtaFinalQueryHandler : IRequestHandler<ObterNotasRelatorioAtaFinalQuery, IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        private INotaConceitoRepository notasConceitoRepository;
        private IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;
        private const int ANO_LETIVO_TURMAS_ED_FISICA_2020 = 2020;

        public ObterNotasRelatorioAtaFinalQueryHandler(INotaConceitoRepository notasConceitoRepository, IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentException(nameof(conselhoClasseAlunoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioAtaFinalQuery request, CancellationToken cancellationToken)
        {
            int[] modalidadesAtaFinal = VerificaModalidadeRelatorioAtaFinal(request.AnoLetivo, request.TiposTurma, request.Modalidade);

            var notas = await notasConceitoRepository.ObterNotasTurmasAlunosParaAtaFinalAsync(request.CodigosAlunos, request.AnoLetivo, modalidadesAtaFinal, request.Semestre, request.TiposTurma);
            if (notas == null || !notas.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");
            notas = notas.Where(x => x.CodigoTurma == request.CodigoTurma && (x.NotaConceito.Nota != null && x.NotaConceito.NotaConceito != ""));
            await CarregarConselhoDeClasseAlunoId(notas);

            return notas.GroupBy(nf => nf.CodigoTurma);
        }

        public int[] VerificaModalidadeRelatorioAtaFinal(int anoLetivo, int[] tiposTurmas, int modalidade)
        {
            if (anoLetivo == ANO_LETIVO_TURMAS_ED_FISICA_2020 && tiposTurmas.Contains((int)TipoTurma.EdFisica) && (modalidade == (int)Modalidade.EJA || modalidade == (int)Modalidade.Medio))
                return new int[] { (int)Modalidade.Fundamental, modalidade};
            
            return new int[] { modalidade };
        }

        private async Task CarregarConselhoDeClasseAlunoId(IEnumerable<NotasAlunoBimestre> notasAluno)
        {
            var turmaIds = notasAluno.Select(nota => nota.IdTurma).Distinct().ToArray();
            var alunosCodigos = notasAluno.Select(nota => nota.CodigoAluno).Distinct().ToArray();
            var conselhos = await conselhoClasseAlunoRepository.ObterConselhoDeClasseAlunoId(turmaIds, alunosCodigos);

            if (conselhos.Any())
            {
                foreach(var nota in notasAluno)
                {
                    nota.ConselhoClasseAlunoId = conselhos.FirstOrDefault(conselho => conselho.TurmaId == nota.IdTurma && 
                                                                                      conselho.AlunoCodigo == nota.CodigoAluno)?.ConselhoClasseAlunoId ?? 0;
                }
            }
        }
    }
}
