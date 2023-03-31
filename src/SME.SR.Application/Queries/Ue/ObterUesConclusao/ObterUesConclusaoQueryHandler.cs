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
    public class ObterUesConclusaoQueryHandler : IRequestHandler<ObterUesConclusaoQuery, IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>>>
    {
        private readonly IMediator mediator;

        public ObterUesConclusaoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>>> Handle(ObterUesConclusaoQuery request, CancellationToken cancellationToken)
        {
            var pareceresConclusivosIds = await mediator.Send(new ObterPareceresConclusivosPorTipoAprovacaoQuery(true));
            if (!pareceresConclusivosIds.Any())
                throw new NegocioException("Não foi possível localizar os pareceres conclusivos.");

            //Obter informações dos alunos
            var informacoesDosAlunos = await ObterInformacoesDosAlunos(request.CodigosAlunos);
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possível obter a informação dos alunos.");

            var codigosTurma = informacoesDosAlunos.Select(x => Convert.ToInt64(x.CodigoTurma)).Distinct().ToArray();

            var turmasDetalhe = await mediator.Send(new ObterTurmasDetalhePorCodigoQuery(codigosTurma));
            //Obter as turmas dos Alunos
            var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigosAlunos, pareceresConclusivosIds.ToArray()));
            var turmasRegularesDosAlunos = turmasDosAlunos?.Where(x => x.TipoTurma == TipoTurma.Regular).ToList();
            if (turmasRegularesDosAlunos == null || !turmasRegularesDosAlunos.Any())
                return new List<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>>();

            var ues = await mediator.Send(new ObterUePorCodigosQuery(informacoesDosAlunos.Select(u => u.CodigoEscola).Distinct().ToArray()));

            var uesConclusao = new List<UeConclusaoPorAlunoAno>();

            var ciclos = await mediator.Send(new ObterCiclosPorModalidadeQuery(request.Modalidade));

            foreach (var turmaRegularAluno in turmasRegularesDosAlunos.Where(tra => tra.Modalidade == request.Modalidade))
            {
                var turmaAlunoEol = informacoesDosAlunos.FirstOrDefault(a => a.CodigoAluno == turmaRegularAluno.AlunoCodigo &&
                                                                             a.CodigoTurma.ToString() == turmaRegularAluno.TurmaCodigo &&
                                                                             ((a.AnoLetivo < DateTime.Now.Year && a.CodigoSituacaoMatricula == SituacaoMatriculaAluno.Concluido) ||
                                                                             (a.AnoLetivo == DateTime.Now.Year && a.EstaAtivo(DateTime.Now))));

                if (turmaAlunoEol != null)
                {
                    var ue = ues.FirstOrDefault(ue => ue.Codigo == turmaAlunoEol.CodigoEscola);
                    var turmaDetalhe = turmasDetalhe.FirstOrDefault(t => t.Codigo == turmaAlunoEol.CodigoTurma.ToString());

                    foreach (var ciclo in ciclos)
                    {
                        var etapaEja = turmaRegularAluno.EtapaEJA == 1 ? "I" : "II";
                        var descricaoAno = request.Modalidade == Modalidade.EJA ? $"{ciclo.Descricao} - {etapaEja}" : $"{ciclo.Ano}º ano";
                        var existeRegistro = uesConclusao.FirstOrDefault(u => u.TurmaAno == descricaoAno && u.AlunoCodigo == turmaRegularAluno.AlunoCodigo) != null;
                        if (!existeRegistro)
                        {
                            if (request.Modalidade == Modalidade.EJA)
                            {
                                descricaoAno = $"{ciclo.Descricao} - I";
                                uesConclusao.Add(new UeConclusaoPorAlunoAno()
                                {
                                    AlunoCodigo = turmaRegularAluno.AlunoCodigo,
                                    TurmaAno = descricaoAno,
                                    UeCodigo = null,
                                    UeNome = null,
                                });
                                descricaoAno = $"{ciclo.Descricao} - II";
                                uesConclusao.Add(new UeConclusaoPorAlunoAno()
                                {
                                    AlunoCodigo = turmaRegularAluno.AlunoCodigo,
                                    TurmaAno = descricaoAno,
                                    UeCodigo = null,
                                    UeNome = null,
                                });
                            }
                            else
                            {
                                if (request.Modalidade != Modalidade.Medio || (int.TryParse(ciclo.Ano, out int ano) && (ano < 4 || (ano > 3 && turmaDetalhe.EhTurmaMagisterio))))
                                {
                                    uesConclusao.Add(new UeConclusaoPorAlunoAno()
                                    {
                                        AlunoCodigo = turmaRegularAluno.AlunoCodigo,
                                        TurmaAno = descricaoAno,
                                        UeCodigo = null,
                                        UeNome = null,
                                    });
                                }
                            }
                        }
                    }

                    var ueConclusao = uesConclusao.FirstOrDefault(u => u.TurmaAno == turmaRegularAluno.DescricaoAno && u.AlunoCodigo == turmaRegularAluno.AlunoCodigo);

                    if (ueConclusao != null)
                    {
                        ueConclusao.AlunoCodigo = turmaRegularAluno.AlunoCodigo;
                        ueConclusao.TurmaAno = turmaRegularAluno.DescricaoAno;
                        ueConclusao.UeCodigo = ue.Codigo;
                        ueConclusao.UeNome = ue.NomeComTipoEscolaEDre;
                        ueConclusao.UeMunicipio = "São Paulo";
                        ueConclusao.UeUF = "SP";
                    }
                    else
                    {
                        uesConclusao.Add(new UeConclusaoPorAlunoAno()
                        {
                            AlunoCodigo = turmaRegularAluno.AlunoCodigo,
                            TurmaAno = turmaRegularAluno.DescricaoAno,
                            UeCodigo = ue.Codigo,
                            UeNome = ue.NomeComTipoEscolaEDre,
                            UeMunicipio = "São Paulo",
                            UeUF = "SP"
                        });
                    }
                }
            }
            var alunosAgrupados = uesConclusao.GroupBy(g => (g.AlunoCodigo, request.Modalidade));
            return alunosAgrupados;

        }

        private async Task<IEnumerable<AlunoHistoricoEscolar>> ObterInformacoesDosAlunos(long[] codigoAlunos)
        {
            var informacoesDosAlunos = await mediator.Send(new ObterDadosHistoricoAlunosPorCodigosQuery(codigoAlunos));
            if (!informacoesDosAlunos.Any())
                throw new NegocioException("Não foi possível obter os dados dos alunos");

            informacoesDosAlunos = informacoesDosAlunos.GroupBy(d => d.CodigoAluno)
                                  .SelectMany(g => g.OrderByDescending(d => d.AnoLetivo)
                                                    .ThenByDescending(m => m.DataSituacao));

            return informacoesDosAlunos;
        }
    }
}
