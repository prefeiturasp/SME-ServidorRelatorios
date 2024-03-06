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
    public class ObterRelatorioParecerConclusivoQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoQuery, RelatorioParecerConclusivoDto>
    {
        private readonly IParecerConclusivoRepository parecerConclusivoRepository;
        private readonly ITurmaRepository turmaRepository;
        private readonly IMediator mediator;

        public ObterRelatorioParecerConclusivoQueryHandler(IParecerConclusivoRepository parecerConclusivoRepository,
            ITurmaRepository turmaRepository, IMediator mediator)
        {
            this.parecerConclusivoRepository = parecerConclusivoRepository ?? throw new System.ArgumentNullException(nameof(parecerConclusivoRepository));
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<RelatorioParecerConclusivoDto> Handle(ObterRelatorioParecerConclusivoQuery request, CancellationToken cancellationToken)
        {

            var retorno = new RelatorioParecerConclusivoDto();

            var parecesParaTratar = await parecerConclusivoRepository.ObterPareceresFinais(request.filtroRelatorioParecerConclusivoDto.AnoLetivo,
                request.filtroRelatorioParecerConclusivoDto.DreCodigo, request.filtroRelatorioParecerConclusivoDto.UeCodigo, request.filtroRelatorioParecerConclusivoDto.Modalidade,
                request.filtroRelatorioParecerConclusivoDto.Semestre, request.filtroRelatorioParecerConclusivoDto.Ciclo, null, request.filtroRelatorioParecerConclusivoDto.Anos,
                request.filtroRelatorioParecerConclusivoDto.ParecerConclusivoId);

            if (!parecesParaTratar.Any())
                throw new NegocioException("Não foi possível emitir o relatório de parecer conclusivo, pois não contém nenhuma informação com o filtro de tipo de parecer conclusivo selecionado.");

            await MontaCabecalho(request, retorno, parecesParaTratar);

            var modalidadeId = request.filtroRelatorioParecerConclusivoDto.Modalidade.HasValue ? (int)request.filtroRelatorioParecerConclusivoDto.Modalidade.Value : 0;

            await MontaSecoes(retorno, parecesParaTratar, request.filtroRelatorioParecerConclusivoDto.AnoLetivo, request.filtroRelatorioParecerConclusivoDto.DreCodigo,
                request.filtroRelatorioParecerConclusivoDto.UeCodigo, request.filtroRelatorioParecerConclusivoDto.Ciclo, modalidadeId,
                request.filtroRelatorioParecerConclusivoDto.Semestre, request.filtroRelatorioParecerConclusivoDto.Anos,
                request.filtroRelatorioParecerConclusivoDto.ParecerConclusivoId, request.filtroRelatorioParecerConclusivoDto.Historico);

            return await Task.FromResult(retorno);
        }

        private async Task MontaSecoes(RelatorioParecerConclusivoDto retorno, IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, int anoLetivo, string dreCodigoEnviado,
            string ueCodigoEnviado, long cicloIdEnviado, int modalidadeId, int? semestre, string[] anos, long parecerConclusivoId, bool consideraHistorico)
        {
            IEnumerable<string> dresCodigos;

            if (retorno.DreNome == "Todas")
            {
                var dresRetorno = await mediator.Send(new ObterTodasDresQuery());
                dresCodigos = dresRetorno.Select(a => a.Codigo);

                foreach (var dre in dresRetorno)
                {
                    var dreParaAdicionar = new RelatorioParecerConclusivoDreDto();
                    dreParaAdicionar.Codigo = dre.Codigo;
                    dreParaAdicionar.Nome = dre.Abreviacao;

                    await TrataUes(parecesParaTratar, dreParaAdicionar, anoLetivo, dre.Id, ueCodigoEnviado, cicloIdEnviado, modalidadeId, semestre, anos, parecerConclusivoId, consideraHistorico);
                    retorno.Dres.Add(dreParaAdicionar);
                }
            }
            else
            {
                var dreUnicaParaAdicionar = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = dreCodigoEnviado });
                var dreParaAdicionar = new RelatorioParecerConclusivoDreDto();
                dreParaAdicionar.Codigo = dreUnicaParaAdicionar.Codigo;
                dreParaAdicionar.Nome = dreUnicaParaAdicionar.Abreviacao;

                if (retorno.UeNome == "Todas")
                    await TrataUes(parecesParaTratar, dreParaAdicionar, anoLetivo, dreUnicaParaAdicionar.Id, "", cicloIdEnviado, modalidadeId, semestre, anos, parecerConclusivoId, consideraHistorico);
                else await TrataUes(parecesParaTratar, dreParaAdicionar, anoLetivo, dreUnicaParaAdicionar.Id, ueCodigoEnviado, cicloIdEnviado, modalidadeId, semestre, anos, parecerConclusivoId, consideraHistorico);

                retorno.Dres.Add(dreParaAdicionar);
            }
        }

        private async Task TrataUes(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, RelatorioParecerConclusivoDreDto dreParaAdicionar, int anoLetivo,
long dreId, string ueCodigoEnviado, long cicloIdEnviado, int modalidadeId, int? semestre, string[] anos, long parecerConclusivoId, bool consideraHistorico)
        {

            List<Ue> uesDaDre = new List<Ue>();

            if (string.IsNullOrEmpty(ueCodigoEnviado))
            {
                var tipoEscolas = new List<int> { (int)TipoEscola.EMEF, (int)TipoEscola.EMEFM, (int)TipoEscola.EMEBS, (int)TipoEscola.CEUEMEF };
                uesDaDre = (await mediator.Send(new ObterUesPorDreSemestreModadalidadeAnoIdQuery(dreId, semestre, modalidadeId, anos, tipoEscolas))).ToList();
            }
            else
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigoEnviado));
                uesDaDre.Add(ue);
            }

            foreach (var ueDaDre in uesDaDre)
            {
                var ueParaAdicionar = new RelatorioParecerConclusivoUeDto();
                ueParaAdicionar.Nome = ueDaDre.NomeComTipoEscola;
                ueParaAdicionar.Codigo = ueDaDre.Codigo;

                await TrataCiclosDaUe(parecesParaTratar, ueDaDre.Id, ueParaAdicionar, anoLetivo, modalidadeId, cicloIdEnviado, anos, parecerConclusivoId, semestre, consideraHistorico);

                dreParaAdicionar.Ues.Add(ueParaAdicionar);
            }

        }

        private async Task TrataCiclosDaUe(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, long ueId, RelatorioParecerConclusivoUeDto ueParaAdicionar, int anoLetivo, int modalidadeId, long cicloIdEnviado,
            string[] anosEnviado, long parecerConclusivoId, int? semestre, bool consideraHistorico)
        {
            var ciclosDaUe = await mediator.Send(new ObterCiclosPorUeIdQuery(ueId));

            if (cicloIdEnviado > 0)
                ciclosDaUe = ciclosDaUe.Where(a => a.Id == cicloIdEnviado).ToList();

            if (anosEnviado != null && anosEnviado.Length > 0)
                ciclosDaUe = ciclosDaUe.Where(a => anosEnviado.Contains(a.Ano.ToString())).ToList();

            if (modalidadeId > 0)
                ciclosDaUe = ciclosDaUe.Where(a => (int)a.Modalidade == modalidadeId).ToList();

            foreach (var cicloDaUeAgrupado in ciclosDaUe.OrderBy(a => a.Id).GroupBy(a => a.Descricao))
            {
                var cicloParaAdicionar = new RelatorioParecerConclusivoCicloDto();
                cicloParaAdicionar.Codigo = "";
                cicloParaAdicionar.Nome = cicloDaUeAgrupado.Key;

                foreach (var cicloAgrupado in cicloDaUeAgrupado.ToList())
                {
                    var anoParaIncluir = new RelatorioParecerConclusivoAnoDto();

                    anoParaIncluir.Nome = cicloAgrupado.Ano + "º Ano";

                    var turmasFiltradas = await mediator.Send(new ObterTurmasPorUeCicloAnoQuery(cicloAgrupado.Id, cicloAgrupado.Ano, ueId, anoLetivo, semestre));

                    IEnumerable<AlunoDaTurmaDto> alunosDasTurmas = new List<AlunoDaTurmaDto>();

                    if (consideraHistorico)
                        alunosDasTurmas = await mediator.Send(new ObterAlunosPorTurmasAnosAnterioresQuery(turmasFiltradas.Select(a => long.Parse(a.Codigo))));
                    else
                        alunosDasTurmas = await mediator.Send(new ObterAlunosPorTurmasQuery(turmasFiltradas.Select(a => long.Parse(a.Codigo))));

                    foreach (var turma in turmasFiltradas)
                    {
                        var alunos = alunosDasTurmas.Where(a => a.CodigoTurma == int.Parse(turma.Codigo)).OrderBy(a => a.ObterNomeFinal());

                        var codigosAlunos = alunos?.Select(a => a.CodigoAluno).Distinct();

                        codigosAlunos = parecesParaTratar.Any() && parecerConclusivoId < 0 
                            ? parecesParaTratar.Where(p => codigosAlunos.Any(c => c == p.AlunoCodigo)).Select(p => p.AlunoCodigo).Distinct() 
                            : codigosAlunos;

                        if (codigosAlunos != null && codigosAlunos.Any())
                        {
                            foreach (var alunoDaTurma in alunos.Where(a => codigosAlunos.Any(c => c == a.CodigoAluno))?.ToList())
                            {
                                var parecerParaIncluir = new RelatorioParecerConclusivoAlunoDto();
                                var numeroAlunoChamadaConvertido = String.IsNullOrEmpty(alunoDaTurma.NumeroAlunoChamada) ? "" : Int32.Parse(alunoDaTurma.NumeroAlunoChamada).ToString();
                                parecerParaIncluir.AlunoCodigo = alunoDaTurma.CodigoAluno.ToString();
                                parecerParaIncluir.AlunoNomeCompleto = alunoDaTurma.ObterNomeFinal();
                                parecerParaIncluir.AlunoNumeroChamada = numeroAlunoChamadaConvertido;
                                parecerParaIncluir.TurmaNome = turma.Nome;

                                var parecerFiltradoParaIncluir = parecesParaTratar.FirstOrDefault(a => a.TurmaId.ToString() == turma.Codigo
                                                                 && a.AlunoCodigo == alunoDaTurma.CodigoAluno && a.Ano == cicloAgrupado.Ano.ToString()
                                                                 && a.CicloId == cicloAgrupado.Id);

                                if (parecerFiltradoParaIncluir != null || parecerConclusivoId == 0 ||
                                   (parecerFiltradoParaIncluir == null && parecerConclusivoId < 0))
                                {
                                    parecerParaIncluir.ParecerConclusivoDescricao = parecerFiltradoParaIncluir == null ?
                                                                                        "Sem Parecer" : parecerFiltradoParaIncluir.ParecerConclusivo;
                                    await VerificarParecerEmAprovacao(parecerParaIncluir, anoLetivo);
                                    anoParaIncluir.PareceresConclusivos.Add(parecerParaIncluir);
                                }
                            }
                        }

                    }

                    if (anoParaIncluir.PareceresConclusivos.Any())
                        cicloParaAdicionar.Anos.Add(anoParaIncluir);
                }

                if (cicloParaAdicionar.Anos.Any())
                    ueParaAdicionar.Ciclos.Add(cicloParaAdicionar);
            }
        }

        private async Task VerificarParecerEmAprovacao(RelatorioParecerConclusivoAlunoDto relatorioParecerAluno, int ano)
        {
            string parecerConclusivoDescricao = await mediator.Send(new ObterDescricaoParecerEmAprovacaoQuery(relatorioParecerAluno.AlunoCodigo, ano));
            if (parecerConclusivoDescricao != null)
            {
                relatorioParecerAluno.ParecerConclusivoDescricao = $"{parecerConclusivoDescricao}*";
                relatorioParecerAluno.EmAprovacao = true;
            }
            else
            {
                relatorioParecerAluno.EmAprovacao = false;
            }
        }
        private async Task MontaCabecalho(ObterRelatorioParecerConclusivoQuery request, RelatorioParecerConclusivoDto retorno, System.Collections.Generic.IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar)
        {
            retorno.Ano = (request.filtroRelatorioParecerConclusivoDto.Anos != null && request.filtroRelatorioParecerConclusivoDto.Anos.Length > 0) ? request.filtroRelatorioParecerConclusivoDto.Anos[0] : "Todos";
            retorno.AnoLetivo = parecesParaTratar.Any() ? parecesParaTratar.FirstOrDefault().AnoLetivo : request.filtroRelatorioParecerConclusivoDto.AnoLetivo.ToString();
            if (request.filtroRelatorioParecerConclusivoDto.Ciclo == 0)
                retorno.Ciclo = "Todos";
            else
                retorno.Ciclo = parecesParaTratar.FirstOrDefault(a => a.CicloId == request.filtroRelatorioParecerConclusivoDto.Ciclo)?.Ciclo;

            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");

            if (string.IsNullOrEmpty(request.filtroRelatorioParecerConclusivoDto.DreCodigo))
                retorno.DreNome = "Todas";
            else
            {
                var dreDoCabecalho = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = request.filtroRelatorioParecerConclusivoDto.DreCodigo });
                retorno.DreNome = dreDoCabecalho.Abreviacao;
            }


            if (string.IsNullOrEmpty(request.filtroRelatorioParecerConclusivoDto.UeCodigo))
                retorno.UeNome = "Todas";
            else
            {
                var ueDoCabecalho = await mediator.Send(new ObterUePorCodigoQuery(request.filtroRelatorioParecerConclusivoDto.UeCodigo));
                retorno.UeNome = ueDoCabecalho.NomeComTipoEscola;
            }

            retorno.RF = request.UsuarioRf;
            retorno.Usuario = request.filtroRelatorioParecerConclusivoDto.UsuarioNome;
        }
    }
}
