using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;

namespace SME.SR.Application.Queries
{
    public class BuscarTurmasSrmERegularDoAlunoQueryHandler : IRequestHandler<BuscarTurmasSrmERegularDoAlunoQuery, IEnumerable<TurmasDoAlunoDTO>>
    {
        const int COMPONENTE_CURRICULAR_ID_SRM = 1030;
        const int TIPO_TURMA_REGULAR = 1;
        const int TIPO_TURMA_PROGRAMA = 3;


        private readonly IRepositorioElasticTurma repositorioElasticTurma;

        public BuscarTurmasSrmERegularDoAlunoQueryHandler(IRepositorioElasticTurma repositorioElasticTurma)
        {
            this.repositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        }

        public async Task<IEnumerable<TurmasDoAlunoDTO>> Handle(BuscarTurmasSrmERegularDoAlunoQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<TurmasDoAlunoDTO>();
            var matriculas = await repositorioElasticTurma.ObterMatriculasTurmaDoAlunoAsync(request.CodigoAluno.ToString(), DateTime.Now, DateTime.Now.Year);
            var codigoTurmas = matriculas.Select(t => t.CodigoTurma).ToArray();

            var turmas = await repositorioElasticTurma.ObterTurmasAsync(codigoTurmas);

            //-> adiciona turma regular no retorno
            retorno.AddRange(turmas.Where(t => t.TipoTurma == TIPO_TURMA_REGULAR).Select(s =>
            {
                var matriculaTurma = matriculas.FirstOrDefault(t => t.CodigoTurma == s.CodigoTurma);
                return MapearEntidades(matriculaTurma, s);
            }));

            //-> adiciona turmas de SRM no retorno
            foreach (var turma in turmas.Where(t => t.TipoTurma == TIPO_TURMA_PROGRAMA))
            {
                if (turma.Componentes.Any(c => c.ComponenteCurricularCodigo == COMPONENTE_CURRICULAR_ID_SRM))
                {
                    var matriculaTurma = matriculas.FirstOrDefault(t => t.CodigoTurma == turma.CodigoTurma);
                    retorno.Add(MapearEntidades(matriculaTurma, turma));
                }
            }

            return retorno;
        }

        private static TurmasDoAlunoDTO MapearEntidades(AlunoNaTurmaDTO matricula, TurmaComponentesDto turma)
        {
            return new TurmasDoAlunoDTO()
            {
                CodigoAluno = matricula?.CodigoAluno ?? 0,
                TipoTurno = int.Parse(turma.Turno),
                AnoLetivo = turma.Ano,
                NomeAluno = matricula?.NomeAluno,
                NomeSocialAluno = matricula?.NomeSocialAluno,
                CodigoSituacaoMatricula = matricula?.CodigoSituacaoMatricula ?? 0,
                SituacaoMatricula = matricula?.SituacaoMatricula,
                DataSituacao = matricula?.DataSituacao ?? DateTime.MinValue,
                DataNascimento = matricula?.DataMatricula ?? DateTime.MinValue,
                NumeroAlunoChamada = matricula?.NumeroAlunoChamada,
                CodigoTurma = turma.CodigoTurma,
                NomeResponsavel = matricula?.NomeResponsavel,
                TipoResponsavel = matricula?.TipoResponsavel?.ToString(),
                CelularResponsavel = matricula?.CelularResponsavel,
                DataAtualizacaoContato = matricula?.DataAtualizacaoContato ?? DateTime.MinValue,
                CodigoTipoTurma = turma.TipoTurma,
                TurmaNome = turma.NomeTurma
            };
        }
    }
}
