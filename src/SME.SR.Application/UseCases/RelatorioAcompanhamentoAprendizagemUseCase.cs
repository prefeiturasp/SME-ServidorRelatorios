using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoAprendizagemUseCase : IRelatorioAcompanhamentoAprendizagemUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoAprendizagemUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto filtro)
        {
            if (filtro.Action == "relatorios/acompanhamento-aprendizagem-escolaaqui" ||
               filtro.Action == "relatorios/boletimescolardetalhadoescolaaqui")
                filtro.RelatorioEscolaAqui = true;

            var parametros = filtro.ObterObjetoFiltro<FiltroRelatorioAcompanhamentoAprendizagemDto>();
           
            var turma = await mediator.Send(new ObterComDreUePorTurmaIdQuery(parametros.TurmaId));

            if (turma == null)
                throw new NegocioException("Turma não encontrada");

            var turmaCodigo = new string[1] { turma.Codigo };
            var professores = (await mediator.Send(new ObterProfessorTitularComponenteCurricularPorTurmaQuery(turmaCodigo))).ToList();

            var uesIndiretas = new TipoEscola[] { TipoEscola.CRPCONV, TipoEscola.CEIINDIR };
            if (uesIndiretas.Contains(turma.Ue.TipoEscola))
            {
                professores.RemoveAll(prof => prof.NomeProfessor == "Não há professor titular.");
                var professoresAtribuicaoExterna = await mediator.Send(new ObterProfessorTitularExternoComponenteCurricularPorTurmaQuery(turmaCodigo));
                professores.AddRange(professoresAtribuicaoExterna);
            }


            var alunosEol = await mediator.Send(new ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(turma.Codigo, parametros.AlunoCodigo, turma.AnoLetivo));
            if (alunosEol == null || !alunosEol.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var acompanhmentosAlunos = await mediator.Send(new ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), parametros.Semestre));

            var bimestres = ObterBimestresPorSemestre(parametros.Semestre);

            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre));

            var periodoInicioFim = await ObterInicioFimPeriodo(tipoCalendarioId, bimestres, parametros.Semestre);

            var quantidadeAulasDadas = await mediator.Send(new ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery(turma.Codigo, tipoCalendarioId, bimestres));

            var frequenciaAlunos = await mediator.Send(new ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), bimestres));

            var ocorrencias = await mediator.Send(new ObterOcorenciasPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo, periodoInicioFim.DataInicio, periodoInicioFim.DataFim));

            if (filtro.RelatorioEscolaAqui)
            {
                var mensagemdados = await MapearDadosParaGerarMensagem(filtro);
                var relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(turma, alunosEol, professores, acompanhmentosAlunos, frequenciaAlunos, ocorrencias, parametros, quantidadeAulasDadas, periodoInicioFim.Id, relatorioEscolaAqui: true));
                filtro.RotaProcessando = RotasRabbitSR.RotaRelatoriosSolicitadosRaaEscolaAqui;
                filtro.RotaErro = RotasRabbitSGP.RotaRelatorioComErro;
                await mediator.Send(new GerarRelatorioHtmlCommand("RelatorioAcompanhamentoAprendizagem", relatorioDto, filtro.CodigoCorrelacao, nomeFila: RotasRabbitSGP.RotaRelatoriosProntosApp, modalidade: turma.ModalidadeCodigo, mensagemDados: mensagemdados));
            }
            else
            {
                var mensagemTitulo = $"Relatório do Acompanhamento da Aprendizagem - {turma.NomeRelatorio}";
                var relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(turma, alunosEol, professores, acompanhmentosAlunos, frequenciaAlunos, ocorrencias, parametros, quantidadeAulasDadas, periodoInicioFim.Id, relatorioEscolaAqui: false));
                await mediator.Send(new GerarRelatorioHtmlCommand("RelatorioAcompanhamentoAprendizagem", relatorioDto, filtro.CodigoCorrelacao, mensagemTitulo: mensagemTitulo));
            }
        }

        private async Task<string> MapearDadosParaGerarMensagem(FiltroRelatorioDto filtro)
        {
            var dados = filtro.ObterObjetoFiltro<ObterDadosMensagemEscolaAquiQuery>();
            dados.Modalidade = ObterModalidade(dados.ModalidadeCodigo);
            dados.Usuario = await ObterUsuarioLogado(filtro.UsuarioLogadoRF);
            dados.CodigoArquivo = filtro.CodigoCorrelacao;
            return UtilJson.ConverterApenasCamposNaoNulos(dados);
        }
        private Modalidade ObterModalidade(int modalidadeId)
        {
            return Enum.GetValues(typeof(Modalidade))
            .Cast<Modalidade>().Where(x => (int)x == modalidadeId).FirstOrDefault();
        }
        private async Task<Usuario> ObterUsuarioLogado(string usuarioLogadorf)
        {
            return await mediator.Send(new ObterUsuarioPorCodigoRfQuery(usuarioLogadorf));
        }
        private async Task<PeriodoEscolarDto> ObterInicioFimPeriodo(long tipoCalendarioId, int[] bimestres, int semestre)
        {
            var periodosEscolares = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));
            int ano = periodosEscolares.FirstOrDefault().PeriodoInicio.Year;

            if (semestre == 1)
            {
                var periodoEscolar = periodosEscolares.FirstOrDefault(p => p.Bimestre == bimestres.Last());
                return new PeriodoEscolarDto()
                {
                    Id = periodoEscolar.Id,
                    DataInicio = new DateTime(ano, 1, 1),
                    DataFim = periodoEscolar.PeriodoFim
                };
            }
            else
            {
                var periodoEscolar = periodosEscolares.FirstOrDefault(p => p.Bimestre == bimestres.First());
                return new PeriodoEscolarDto()
                {
                    Id = periodoEscolar.Id,
                    DataInicio = periodoEscolar.PeriodoInicio,
                    DataFim = new DateTime(ano, 12, 31)
                };
            }
        }

        private static int[] ObterBimestresPorSemestre(int semestre)
        {
            if (semestre == 1)
                return new int[] { 1, 2 };
            else return new int[] { 3, 4 };
        }
    }
}
