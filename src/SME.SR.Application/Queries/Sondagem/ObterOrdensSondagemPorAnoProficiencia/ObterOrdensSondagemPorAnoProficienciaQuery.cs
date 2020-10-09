using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterOrdensSondagemPorAnoProficienciaQuery : IRequest<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>>
    {
        public ObterOrdensSondagemPorAnoProficienciaQuery(string ano, ProficienciaSondagemEnum proficiencia)
        {
            Ano = ano;
            Proficiencia = proficiencia;
        }

        public string Ano { get; set; }
        public ProficienciaSondagemEnum Proficiencia { get; set; }
    }
}
