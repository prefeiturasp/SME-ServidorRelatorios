using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;

namespace SME.SR.Application.UseCases
{
    public class RelatorioFrequenciaControleMensalUseCase : IRelatorioFrequenciaControleMensalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFrequenciaControleMensalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var retorno = new List<ControleFrequenciaMensalDto>();

            var filtro = request.ObterObjetoFiltro<FiltroRelatorioControleFrenquenciaMensalDto>();
            var ueComDre = await ObterUeComDrePorCodigo(filtro.CodigoUe);
            var dadosTurma = await mediator.Send(new ObterTurmaPorCodigoQuery(filtro.CodigoTurma));
   
            var controFrequenciaMensal = new ControleFrequenciaMensalDto
            {
                Ano = filtro.AnoLetivo,
                Usuario = $"{filtro.NomeUsuario}(${filtro.CodigoRf})",
                Dre = ueComDre.Dre.Abreviacao,
                Ue = ueComDre.NomeRelatorio,
                Turma = dadosTurma.NomeRelatorio,
            };
            /*
             * Cabecalho
             * Nome Dre
             * Nome Ue
             * Criança/Estudante
             * CodigoEOL
             * Turma
             * Mês
             */
        }

        public async Task<Ue> ObterUeComDrePorCodigo(string codigoUe)
        {
            return await mediator.Send(new ObterUeComDrePorCodigoUeQuery(codigoUe));
        }
    }
}