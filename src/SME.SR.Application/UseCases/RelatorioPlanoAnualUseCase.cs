using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SME.SR.Data;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Workers.SGP
{
    public class RelatorioPlanoAnualUseCase : IRelatorioPlanoAnualUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanoAnualUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroPlanoAnualDto>();

            var planoAnualBimestreObjetivosDtos = await mediator.Send(new ObterPlanoAnualQuery(filtros.Id));

            var planoAnual = planoAnualBimestreObjetivosDtos.FirstOrDefault();
            var cabecalho = new RelatorioPlanoAnualDto()
            {
                AnoLetivo = planoAnual.AnoLetivo,
                DreNome = planoAnual.DreNome,
                UeNome = $"{planoAnual.UeCodigo} - {planoAnual.TipoEscola.ShortName()} {planoAnual.UeNome}",
                Turma = $"{planoAnual.ModalidadeTurma.ShortName()} - {planoAnual.TurmaNome}{planoAnual.TurmaTipoTurno.NomeTipoTurnoEol(" - ")}",
                ComponenteCurricular = planoAnual.ComponenteCurricular,
                ExibeObjetivos = !(planoAnual.ModalidadeTurma == Modalidade.Medio || planoAnual.ModalidadeTurma.EhSemestral()),
                Usuario = filtros.Usuario,
                DataImpressao = DateTimeExtension.HorarioBrasilia().Date.ToString("dd/MM/yyyy"),
                Bimestres = planoAnualBimestreObjetivosDtos
                    .GroupBy(s=> new { s.Bimestre, s.DescricaoPlanejamento})
                    .Select(a=> new BimestreDescricaoPlanejamentoDto 
                    { 
                        Bimestre = a.Key.Bimestre, 
                        DescricaoPlanejamento = UtilRegex.RemoverTagsHtml(UtilRegex.RemoverTagsHtmlMultiMidia(a.Key.DescricaoPlanejamento)),
                        Objetivos = a.Any(b=> !string.IsNullOrEmpty(b.ObjetivoCodigo)) ? a.Select(o=> new ObjetivoAprendizagemPlanoAnualDto
                        {
                            Codigo = o.ObjetivoCodigo, 
                            Descricao = o.ObjetivoDescricao
                        }).ToList() : Enumerable.Empty<ObjetivoAprendizagemPlanoAnualDto>() 
                    }).OrderBy(o=> o.Bimestre)
            };
            
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPlanoAnual", cabecalho, request.CodigoCorrelacao));
        }
    }
}
