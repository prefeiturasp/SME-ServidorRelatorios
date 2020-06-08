using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra.Enumeradores
{
    public static partial class Enumeradores
    {
        public enum OrdenacaoEnum
        {
            [Display(Name = "NONE")]
            NONE = 0,
            [Display(Name = "SORTBY_JOBID")]
            SORTBY_JOBID = 1,
            [Display(Name = "SORTBY_JOBNAME")]
            SORTBY_JOBNAME = 2,
            [Display(Name = "SORTBY_REPORTURI")]
            SORTBY_REPORTURI = 3,
            [Display(Name = "SORTBY_REPORTNAME")]
            SORTBY_REPORTNAME = 4,
            [Display(Name = "SORTBY_REPORTFOLDER")]
            SORTBY_REPORTFOLDER = 5,
            [Display(Name = "SORTBY_OWNER")]
            SORTBY_OWNER = 6,
            [Display(Name = "SORTBY_STATUS")]
            SORTBY_STATUS = 7,
            [Display(Name = "SORTBY_LASTRUN")]
            SORTBY_LASTRUN = 8,
            [Display(Name = "SORTBY_NEXTRUN")]
            SORTBY_NEXTRUN = 9
        }
    }
}
