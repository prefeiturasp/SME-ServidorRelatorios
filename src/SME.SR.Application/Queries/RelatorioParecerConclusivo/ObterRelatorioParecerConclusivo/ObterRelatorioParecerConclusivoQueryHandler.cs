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
                request.filtroRelatorioParecerConclusivoDto.Semestre, request.filtroRelatorioParecerConclusivoDto.CicloId, request.filtroRelatorioParecerConclusivoDto.Anos,
                request.filtroRelatorioParecerConclusivoDto.ParecerConclusivoId);

            await MontaCabecalho(request, retorno, parecesParaTratar);

            var modalidadeId = request.filtroRelatorioParecerConclusivoDto.Modalidade.HasValue ? (int)request.filtroRelatorioParecerConclusivoDto.Modalidade.Value : 0;

            await MontaSecoes(retorno, parecesParaTratar, request.filtroRelatorioParecerConclusivoDto.DreCodigo,
                request.filtroRelatorioParecerConclusivoDto.UeCodigo, request.filtroRelatorioParecerConclusivoDto.CicloId, modalidadeId,
                request.filtroRelatorioParecerConclusivoDto.Semestre, request.filtroRelatorioParecerConclusivoDto.Anos);

            return await Task.FromResult(retorno);
        }

        private async Task MontaSecoes(RelatorioParecerConclusivoDto retorno, IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, string dreCodigoEnviado,
            string ueCodigoEnviado, long cicloIdEnviado, int modalidadeId, int? semestre, string[] anos)
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

                    await TrataUes(parecesParaTratar, dreParaAdicionar, dre.Id, ueCodigoEnviado, cicloIdEnviado, modalidadeId, semestre, anos);
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
                    await TrataUes(parecesParaTratar, dreParaAdicionar, dreUnicaParaAdicionar.Id, "", cicloIdEnviado, modalidadeId, semestre, anos);
                else await TrataUes(parecesParaTratar, dreParaAdicionar, dreUnicaParaAdicionar.Id, ueCodigoEnviado, cicloIdEnviado, modalidadeId, semestre, anos);

                retorno.Dres.Add(dreParaAdicionar);
            }
        }

        private async Task TrataUes(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, RelatorioParecerConclusivoDreDto dreParaAdicionar, long dreId,
            string ueCodigoEnviado, long cicloIdEnviado, int modalidadeId, int? semestre, string[] anos)
        {

            List<Ue> uesDaDre = new List<Ue>();

            if (string.IsNullOrEmpty(ueCodigoEnviado))
                uesDaDre = (await mediator.Send(new ObterUesPorDreSemestreModadalidadeAnoIdQuery(dreId, semestre, modalidadeId, anos))).ToList();
            else
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigoEnviado));
                uesDaDre.Add(ue);
            }

            foreach (var ueDaDre in uesDaDre)
            {
                var ueParaAdicionar = new RelatorioParecerConclusivoUeDto();
                ueParaAdicionar.Nome = ueDaDre.TipoEscola + " - " + ueDaDre.Nome;
                ueParaAdicionar.Codigo = ueDaDre.Codigo;

                await TrataCiclosDaUe(parecesParaTratar, ueDaDre.Id, ueParaAdicionar, cicloIdEnviado, anos);

                dreParaAdicionar.Ues.Add(ueParaAdicionar);
            }

        }

        private async Task TrataCiclosDaUe(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, long ueId, RelatorioParecerConclusivoUeDto ueParaAdicionar, long cicloIdEnviado, 
            string[] anosEnviado)
        {
            var ciclosDaUe = await mediator.Send(new ObterCiclosPorUeIdQuery(ueId));

            if (cicloIdEnviado > 0)
                ciclosDaUe = ciclosDaUe.Where(a => a.Id == cicloIdEnviado).ToList();

            if (anosEnviado != null && anosEnviado.Length > 0)
                ciclosDaUe = ciclosDaUe.Where(a => anosEnviado.Contains(a.Ano.ToString())).ToList();                

            foreach (var cicloDaUeAgrupado in ciclosDaUe.GroupBy(a => a.Descricao))
            {
                var cicloParaAdicionar = new RelatorioParecerConclusivoCicloDto();
                cicloParaAdicionar.Codigo = "";
                cicloParaAdicionar.Nome = cicloDaUeAgrupado.Key;

                foreach (var cicloAgrupado in cicloDaUeAgrupado.ToList())
                {
                    var anoParaIncluir = new RelatorioParecerConclusivoAnoDto();

                    anoParaIncluir.Nome = cicloAgrupado.Ano + "º Ano";

                    var turmasFiltradas = await mediator.Send(new ObterTurmasPorUeCicloAnoQuery(cicloAgrupado.Id, cicloAgrupado.Ano.ToString(), ueId));
                    var alunosDasTurmas = await turmaRepository.ObterAlunosPorTurmas(turmasFiltradas.Select(a => long.Parse(a.Codigo)));


                    foreach (var turma in turmasFiltradas)
                    {

                        foreach (var alunoDaTurma in alunosDasTurmas.Where(a => a.CodigoTurma == int.Parse(turma.Codigo)).OrderBy(a => a.ObterNomeFinal()))
                        {
                            var parecerParaIncluir = new RelatorioParecerConclusivoAlunoDto();
                            parecerParaIncluir.AlunoCodigo = alunoDaTurma.CodigoAluno.ToString();
                            parecerParaIncluir.AlunoNomeCompleto = alunoDaTurma.ObterNomeFinal();
                            parecerParaIncluir.AlunoNumeroChamada = alunoDaTurma.NumeroAlunoChamada ?? "";
                            parecerParaIncluir.TurmaNome = turma.Nome;

                            var parecerFiltradoParaIncluir = parecesParaTratar.FirstOrDefault(a => a.TurmaId == turma.Id
                                                             && a.AlunoCodigo == alunoDaTurma.CodigoAluno && a.Ano == cicloAgrupado.Ano.ToString()
                                                             && a.CicloId == cicloAgrupado.Id);

                            if (parecerFiltradoParaIncluir == null)
                                parecerParaIncluir.ParecerConclusivoDescricao = "Sem Parecer";
                            else parecerParaIncluir.ParecerConclusivoDescricao = parecerFiltradoParaIncluir.ParecerConclusivo;

                            anoParaIncluir.PareceresConclusivos.Add(parecerParaIncluir);
                        }
                    }
                    cicloParaAdicionar.Anos.Add(anoParaIncluir);
                }

                ueParaAdicionar.Ciclos.Add(cicloParaAdicionar);
            }
        }
        private async Task MontaCabecalho(ObterRelatorioParecerConclusivoQuery request, RelatorioParecerConclusivoDto retorno, System.Collections.Generic.IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar)
        {
            retorno.Ano = (request.filtroRelatorioParecerConclusivoDto.Anos != null && request.filtroRelatorioParecerConclusivoDto.Anos.Length > 0) ? request.filtroRelatorioParecerConclusivoDto.Anos[0] : "Todos";

            if (request.filtroRelatorioParecerConclusivoDto.CicloId == 0)
                retorno.Ciclo = "Todos";
            else
                retorno.Ciclo = parecesParaTratar.FirstOrDefault(a => a.CicloId == request.filtroRelatorioParecerConclusivoDto.CicloId).Ciclo;

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
                retorno.UeNome = ueDoCabecalho.TipoEscola + " - " + ueDoCabecalho.Nome;
            }

            retorno.RF = request.UsuarioRf;
            retorno.Usuario = request.filtroRelatorioParecerConclusivoDto.UsuarioNome;
        }
    }
}
