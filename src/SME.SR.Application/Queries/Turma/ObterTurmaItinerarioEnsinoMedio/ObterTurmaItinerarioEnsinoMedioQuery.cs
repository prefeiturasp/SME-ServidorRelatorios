using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterTurmaItinerarioEnsinoMedioQuery : IRequest<IEnumerable<TurmaItinerarioEnsinoMedioDto>>
    {
        public ObterTurmaItinerarioEnsinoMedioQuery()
        {

        }
    }
}
