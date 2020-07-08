using System.Collections.Generic;

namespace SME.SR.Data
{
   public class AreaDoConhecimento
    {
        public long Id { get; set; }

        public string Nome { get; set; }

        public IEnumerable<long> ComponentesCurricularesId { get; set; }
    }
}
