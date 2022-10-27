using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterAlunosTurmasRelatorioBoletimQuery, IEnumerable<IGrouping<string, Aluno>>>
    {
        private readonly IAlunoRepository alunoRepository;
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosTurmasRelatorioBoletimQueryHandler(IAlunoRepository alunoRepository,
                                                             ITurmaRepository turmaRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<IGrouping<string, Aluno>>> Handle(ObterAlunosTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var alunos = Enumerable.Empty<Aluno>();
            var alunosOrdenadosPorSituacao = Enumerable.Empty<Aluno>();

            var turmasEdFisica = (await turmaRepository.ObterPorAlunosEParecerConclusivo(request.CodigosAlunos.Select(ca => long.Parse(ca)).ToArray(), null))
                    .Where(te => te.TipoTurma == TipoTurma.EdFisica && request.CodigosTurma.Contains(te.RegularCodigo))
                    .Distinct()
                    .ToArray();

            var codigosTurmaConsiderados = request.CodigosTurma
                .Concat(turmasEdFisica.Select(t => t.TurmaCodigo))
                .Distinct()
                .ToArray();

            alunos = request.CodigosAlunos?.Length > 0 ?
                await alunoRepository.ObterPorCodigosAlunoETurma(codigosTurmaConsiderados, request.CodigosAlunos) :
                await alunoRepository.ObterPorCodigosTurma(codigosTurmaConsiderados);

            alunosOrdenadosPorSituacao = ObterAlunosPorUltimaSituacao(alunos, request.ConsideraNovoEM, turmasEdFisica != null && turmasEdFisica.Any(), codigosTurmaConsiderados);

            if (!alunosOrdenadosPorSituacao.Any())
                throw new NegocioException("Não foi possível localizar os alunos");

            var resultadoAlunos = request.TrazerAlunosInativos ? alunosOrdenadosPorSituacao.OrderBy(a => a.ObterNomeFinal()).GroupBy(a => a.CodigoAluno.ToString())
                                                : alunosOrdenadosPorSituacao.Where(al => al.Ativo)
                                                    .OrderBy(a => a.ObterNomeFinal()).GroupBy(a => a.CodigoAluno.ToString());
            if (!resultadoAlunos.Any())
                throw new NegocioException("Não foi possível localizar alunos com os filtros definidos.");

            return resultadoAlunos;
        }

        private IEnumerable<Aluno> ObterAlunosPorUltimaSituacao(IEnumerable<Aluno> listaAlunos, bool consideraNovoEM, bool considerarComplementaresEdFisica = false, params string[] codigosTurmas)
        {
            var listaTemporaria = new List<Aluno>();
            var listaAlunosOrdenada = new List<Aluno>();

            foreach (var item in listaAlunos)
            {
                listaTemporaria = listaAlunos
                    .Where(x => x.CodigoAluno == item.CodigoAluno && codigosTurmas.ToList().Contains(x.CodigoTurma.ToString()))
                    .ToList();

                var listaTemporariaParaOrdenada = consideraNovoEM || considerarComplementaresEdFisica ?
                    listaTemporaria.OrderByDescending(x => x.DataSituacao) :
                    listaTemporaria.OrderByDescending(x => x.DataSituacao).Take(1);

                listaAlunosOrdenada.AddRange(listaTemporariaParaOrdenada);
                listaTemporaria.Clear();
            }

            return consideraNovoEM || considerarComplementaresEdFisica ? 
                listaAlunosOrdenada.Distinct() : listaAlunosOrdenada.DistinctBy(x => x.CodigoAluno);
        }
    }
}