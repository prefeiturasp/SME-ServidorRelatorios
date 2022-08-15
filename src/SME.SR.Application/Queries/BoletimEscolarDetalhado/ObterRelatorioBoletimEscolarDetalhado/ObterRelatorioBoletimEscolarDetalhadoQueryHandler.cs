using MediatR;
using SME.SR.Application.Queries.BoletimEscolar;
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
    public class ObterRelatorioBoletimEscolarDetalhadoQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarDetalhadoQuery, BoletimEscolarDetalhadoEscolaAquiDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioBoletimEscolarDetalhadoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<BoletimEscolarDetalhadoEscolaAquiDto> Handle(ObterRelatorioBoletimEscolarDetalhadoQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.TurmaCodigo))
                await ObterFechamentoTurma(request.TurmaCodigo);

            var dre = await ObterDrePorCodigo(request.DreCodigo);
            var ue = await ObterUePorCodigo(request.UeCodigo);
            var ciclos = await ObterCiclosPorAnoModalidade(request.Modalidade);
            var turmas = await ObterTurmasRelatorio(request.TurmaCodigo, request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.ConsideraHistorico);
            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();

            var mediasFrequencia = await ObterMediasFrequencia();

            var alunosPorTurma = await ObterAlunosPorTurmasRelatorio(codigosTurma, request.AlunosCodigo, request.ConsideraInativo);

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(alunosPorTurma.SelectMany(t => t.Select(t => t.CodigoAluno)).Distinct().ToArray(), request.AnoLetivo, request.Semestre, request.UeCodigo, request.Modalidade, request.Usuario);
            var tiposNota = await ObterTiposNotaRelatorio(request.AnoLetivo, dre.Id, ue.Id, request.Semestre, request.Modalidade, turmas);

            string[] codigosAlunos = alunosPorTurma.SelectMany(t => t.Select(t => t.CodigoAluno.ToString())).ToArray();

            var ultimoBimestrePeriodoFechamento = await ObterUltimoBimestrePeriodoFechamento(request.AnoLetivo);

            var alunosFoto = await ObterFotosAlunos(alunosPorTurma.Select(a => a.Key)?.ToArray());

            var notas = await ObterNotasAlunos(codigosAlunos, codigosTurma, request.AnoLetivo, request.Modalidade, request.Semestre);
            var pareceresConclusivos = await ObterPareceresConclusivos(dre.Codigo, ue.Codigo, turmas, request.AnoLetivo, request.Modalidade, request.Semestre);
            var frequencias = await ObterFrequenciasAlunos(codigosAlunos, request.AnoLetivo, request.Modalidade, request.Semestre, request.TurmaCodigo);
            var frequenciaGlobal = await ObterFrequenciaGlobalAlunos(codigosAlunos, request.AnoLetivo, request.Modalidade);
            var recomendacoes = await ObterRecomendacoesAlunosTurma(codigosAlunos, codigosTurma, request.AnoLetivo, request.Modalidade, request.Semestre);

            var boletins = await MontarBoletins(dre, ue, ciclos, turmas, ultimoBimestrePeriodoFechamento, componentesCurriculares, alunosPorTurma, alunosFoto, notas, pareceresConclusivos, recomendacoes, frequencias, tiposNota, mediasFrequencia, frequenciaGlobal, request.AnoLetivo);

            return new BoletimEscolarDetalhadoEscolaAquiDto(boletins);
        }

        private async Task<int> ObterUltimoBimestrePeriodoFechamento(int anoLetivo)
        {
            return await mediator.Send(new ObterBimestrePeriodoFechamentoAtualQuery(anoLetivo));
        }

        private async Task<IEnumerable<RecomendacaoConselhoClasseAluno>> ObterRecomendacoesAlunosTurma(string[] codigosAlunos, string[] codigosTurma, int anoLetivo, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterRecomendacoesPorAlunosTurmasQuery(codigosAlunos, codigosTurma, anoLetivo, modalidade, semestre));
        }

        private async Task<IEnumerable<AlunoFotoArquivoDto>> ObterFotosAlunos(string[] codigosAluno)
        {
            return await mediator.Send(new ObterAlunosFotoPorCodigoQuery(codigosAluno));
        }

        private async Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> ObterPareceresConclusivos(string dreCodigo, string ueCodigo, IEnumerable<Turma> turmas, int anoLetivo, Modalidade modalidade, int semestre)
        {
            var turmasCodigo = turmas.Select(t => t.Codigo)?.ToArray();

            return await mediator.Send(new ObterPareceresFinaisQuery()
            {
                DreCodigo = dreCodigo,
                UeCodigo = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade,
                Semestre = semestre,
                TurmasCodigo = turmasCodigo
            });
        }

        private async Task<IEnumerable<TipoCiclo>> ObterCiclosPorAnoModalidade(Modalidade modalidade)
        {
            return await mediator.Send(new ObterCiclosPorModalidadeQuery(modalidade));
        }

        private async Task ObterFechamentoTurma(string codigoTurma)
        {
            var obterFechamentosPorCodigosTurmaQuery = new ObterFehamentoPorCodigoTurmaQuery()
            {
                CodigoTurma = codigoTurma
            };

            await mediator.Send(obterFechamentosPorCodigosTurmaQuery);
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
            try
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
            catch (NegocioException)
            {
                throw new NegocioException("As turmas selecionadas não possuem fechamento.");
            }
        }

        private async Task<IEnumerable<IGrouping<string, Aluno>>> ObterAlunosPorTurmasRelatorio(string[] turmasCodigo, string[] alunosCodigo, bool consideraInativo)
        {
            return await mediator.Send(new ObterAlunosTurmasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo,
                TrazerAlunosInativos = consideraInativo
            });
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(int[] codigosAlunos, int anoLetivo, int semestre, string codigoUe, Modalidade modalidade, Usuario usuario)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorAlunosQuery(codigosAlunos, anoLetivo, semestre, codigoUe, modalidade, usuario));
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

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, string[] codigosTurma, int anoLetivo, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterNotasRelatorioBoletimQuery(alunosCodigo, codigosTurma, anoLetivo, (int)modalidade, semestre));
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre,string turmaCodigo)
        {
            return await mediator.Send(new ObterFrequenciasRelatorioBoletimQuery(alunosCodigo, anoLetivo, modalidade, semestre, turmaCodigo));
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciaGlobalAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade)
        {
            return await mediator.Send(new ObterFrequenciaGlobalRelatorioBoletimQuery(alunosCodigo, anoLetivo, modalidade));
        }

        private async Task<BoletimEscolarDetalhadoDto> MontarBoletins(Dre dre, Ue ue, IEnumerable<TipoCiclo> ciclos, IEnumerable<Turma> turmas,
                                                             int ultimoBimestrePeriodoFechamento, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares,
                                                             IEnumerable<IGrouping<string, Aluno>> alunosPorTurma,
                                                             IEnumerable<AlunoFotoArquivoDto> alunosFoto, IEnumerable<IGrouping<string, NotasAlunoBimestre>> notasAlunos,
                                                             IEnumerable<RelatorioParecerConclusivoRetornoDto> pareceresConclusivos,
                                                             IEnumerable<RecomendacaoConselhoClasseAluno> recomendacoes,
                                                             IEnumerable<IGrouping<string, FrequenciaAluno>> frequenciasAlunos,
                                                             IDictionary<string, string> tiposNota, IEnumerable<MediaFrequencia> mediasFrequencias,
                                                             IEnumerable<IGrouping<string, FrequenciaAluno>> frequenciaGlobal,
                                                             int anoLetivo)
        {
            return await mediator.Send(new MontarBoletinsDetalhadosQuery()
            {
                Dre = dre,
                Ue = ue,
                UltimoBimestrePeriodoFechamento = ultimoBimestrePeriodoFechamento,
                TiposCiclo = ciclos,
                ComponentesCurriculares = componentesCurriculares,
                Turmas = turmas,
                AlunosPorTuma = alunosPorTurma,
                AlunosFoto = alunosFoto,
                Notas = notasAlunos,
                Frequencias = frequenciasAlunos,
                TiposNota = tiposNota,
                MediasFrequencia = mediasFrequencias,
                PareceresConclusivos = pareceresConclusivos,
                FrequenciasGlobal = frequenciaGlobal,
                RecomendacoesAlunos = recomendacoes,
                AnoLetivo = anoLetivo
            });
        }

        private async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            return await mediator.Send(new ObterParametrosMediaFrequenciaQuery());
        }
    }
}