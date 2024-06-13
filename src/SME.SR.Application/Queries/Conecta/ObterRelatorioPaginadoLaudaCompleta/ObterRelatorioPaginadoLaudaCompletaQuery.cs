using MediatR;
using SME.SR.Data.Models.Conecta;
using SME.SR.Infra.Dtos.Relatorios.Conecta;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterRelatorioPaginadoLaudaCompletaQuery : IRequest<RelatorioPaginadoLaudaCompletaDto>
    {
        public ObterRelatorioPaginadoLaudaCompletaQuery(PropostaCompleta propostaCompleta)
        {
            PropostaCompleta = propostaCompleta;
        }

        public PropostaCompleta PropostaCompleta { get; set; }
    }
}
