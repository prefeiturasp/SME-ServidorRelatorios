using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioHistoricoEscolarUseCase : IRelatorioHistoricoEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioHistoricoEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            var filtros = request.ObterObjetoFiltro<FiltroHistoricoEscolarDto>();
                        
            var cabecalho = await MontarCabecalho(filtros);

            //Obter Alunos e Turmas
            var alunosTurmas = await MontarAlunosTurmas(filtros);


            //var jsonString = "";

            //if (relatorioHistoricoEscolar != null)
            //{
            //    jsonString = JsonConvert.SerializeObject(relatorioHistoricoEscolar);
            //}

            //  await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarFundamental/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao));
        }

        private async Task<IEnumerable<AlunoTurmasNotasFrequenciasDto>> MontarAlunosTurmas(FiltroHistoricoEscolarDto filtros)
        {

            var alunosCodigos = filtros.AlunosCodigo.Select(long.Parse).ToArray();

            var turmaCodigo = string.IsNullOrEmpty(filtros.TurmaCodigo) ? 0 : long.Parse(filtros.TurmaCodigo);

            return await mediator.Send(new ObterNotasEFrequenciasDosAlunosQuery(turmaCodigo, alunosCodigos));            
        }

        private async Task<CabecalhoDto> MontarCabecalho(FiltroHistoricoEscolarDto filtros)
        {
            var enderecosEAtos = await mediator.Send(new ObterEnderecoEAtosDaUeQuery(filtros.UeCodigo));
            if (!enderecosEAtos.Any())
                throw new NegocioException("Não foi possível obter os dados de endereço e atos da UE.");

            return MontaCabecalhoComBaseNoEnderecoEAtosDaUe(enderecosEAtos);
        }

        private static CabecalhoDto MontaCabecalhoComBaseNoEnderecoEAtosDaUe(IEnumerable<EnderecoEAtosDaUeDto> enderecosEAtos)
        {
            if (enderecosEAtos.Any())
            {
                var cabecalho = new CabecalhoDto();
                cabecalho.NomeUe = enderecosEAtos?.FirstOrDefault()?.NomeUe;
                cabecalho.Endereco = enderecosEAtos?.FirstOrDefault()?.Endereco;
                cabecalho.AtoCriacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "1")?.Atos;
                cabecalho.AtoAutorizacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "7")?.Atos;
                return cabecalho;
            }
            else return default;
        }
    }
}
