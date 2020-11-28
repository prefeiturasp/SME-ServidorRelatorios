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
    public class ObterUesConclusaoQueryHandler : IRequestHandler<ObterUesConclusaoQuery, IEnumerable<IGrouping<long, UeConclusaoPorAlunoAno>>>
    {
        public IMediator mediator;

        public ObterUesConclusaoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IEnumerable<IGrouping<long, UeConclusaoPorAlunoAno>>> Handle(ObterUesConclusaoQuery request, CancellationToken cancellationToken)
        {

            var pareceresConclusivosIds = await mediator.Send(new ObterPareceresConclusivosPorTipoAprovacaoQuery(true));
            if (!pareceresConclusivosIds.Any())
                throw new NegocioException("Não foi possível localizar os pareceres conclusivos.");

            //Obter informações dos alunos
            var informacoesDosAlunos = await ObterInformacoesDosAlunos(request.CodigosAlunos);
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possível obter a informação dos alunos.");

            //Obter as turmas dos Alunos
            var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigosAlunos, pareceresConclusivosIds.ToArray()));

            if (!turmasDosAlunos.Any())
                throw new NegocioException("Não foi possível obter os dados das turmas dos alunos.");

            var ues = await mediator.Send(new ObterUePorCodigosQuery(informacoesDosAlunos.Select(u => u.CodigoEscola).Distinct().ToArray()));

            var uesConclusao = new List<UeConclusaoPorAlunoAno>();

            var ciclos = await mediator.Send(new ObterCiclosPorModalidadeQuery(request.Modalidade));

            foreach (var turmaAluno in turmasDosAlunos)
            {
                var turmaAlunoEol = informacoesDosAlunos.FirstOrDefault(a => a.CodigoAluno == turmaAluno.AlunoCodigo &&
                                                                             a.CodigoTurma.ToString() == turmaAluno.TurmaCodigo &&
                                                                             ((a.AnoLetivo < DateTime.Now.Year && a.CodigoSituacaoMatricula == SituacaoMatriculaAluno.Concluido) ||
                                                                             (a.AnoLetivo == DateTime.Now.Year && a.EstaAtivo(DateTime.Now))));

                if (turmaAlunoEol != null)
                {
                    var ue = ues.FirstOrDefault(ue => ue.Codigo == turmaAlunoEol.CodigoEscola);

                    foreach (var ciclo in ciclos)
                    {
                        var descricaoAno = request.Modalidade == Modalidade.EJA ? $"{ciclo.Descricao} - {(turmaAluno.EtapaEJA == 1 ? "I" : "II")}" : $"{ciclo.Ano}º ano";
                        var existeRegistro = uesConclusao.FirstOrDefault(u => u.TurmaAno == descricaoAno && u.AlunoCodigo == turmaAluno.AlunoCodigo) != null;
                        if (!existeRegistro)
                        {
                            if (request.Modalidade == Modalidade.EJA)
                            {
                                descricaoAno = $"{ciclo.Descricao} - I";
                                uesConclusao.Add(new UeConclusaoPorAlunoAno()
                                {
                                    AlunoCodigo = turmaAluno.AlunoCodigo,
                                    TurmaAno = descricaoAno,
                                    UeCodigo = null,
                                    UeNome = null,
                                });
                                descricaoAno = $"{ciclo.Descricao} - II";
                                uesConclusao.Add(new UeConclusaoPorAlunoAno()
                                {
                                    AlunoCodigo = turmaAluno.AlunoCodigo,
                                    TurmaAno = descricaoAno,
                                    UeCodigo = null,
                                    UeNome = null,
                                });
                            }
                            else
                            {
                                uesConclusao.Add(new UeConclusaoPorAlunoAno()
                                {
                                    AlunoCodigo = turmaAluno.AlunoCodigo,
                                    TurmaAno = descricaoAno,
                                    UeCodigo = null,
                                    UeNome = null,
                                });
                            }
                        }
                    }

                    var ueConclusao = uesConclusao.FirstOrDefault(u => u.TurmaAno == turmaAluno.DescricaoAno && u.AlunoCodigo == turmaAluno.AlunoCodigo);

                    if (ueConclusao != null)
                    {
                        ueConclusao.AlunoCodigo = turmaAluno.AlunoCodigo;
                        ueConclusao.TurmaAno = turmaAluno.DescricaoAno;
                        ueConclusao.UeCodigo = ue.Codigo;
                        ueConclusao.UeNome = ue.NomeComTipoEscolaEDre;
                        ueConclusao.UeMunicipio = "São Paulo";
                        ueConclusao.UeUF = "SP";
                    }
                    else
                    {
                        uesConclusao.Add(new UeConclusaoPorAlunoAno()
                        {
                            AlunoCodigo = turmaAluno.AlunoCodigo,
                            TurmaAno = turmaAluno.DescricaoAno,
                            UeCodigo = ue.Codigo,
                            UeNome = ue.NomeComTipoEscolaEDre,
                            UeMunicipio = "São Paulo",
                            UeUF = "SP"
                        });
                    }
                }
            }
            var alunosAgrupados = uesConclusao.GroupBy(g => g.AlunoCodigo);
            return alunosAgrupados;

        }

        private async Task<IEnumerable<AlunoHistoricoEscolar>> ObterInformacoesDosAlunos(long[] codigoAlunos)
        {
            var informacoesDosAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(codigoAlunos));
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possíve obter os dados dos alunos");

            informacoesDosAlunos = informacoesDosAlunos.GroupBy(d => d.CodigoAluno)
                                  .SelectMany(g => g.OrderByDescending(d => d.AnoLetivo)
                                                    .ThenByDescending(m => m.DataSituacao));

            return informacoesDosAlunos;
        }
    }
}
