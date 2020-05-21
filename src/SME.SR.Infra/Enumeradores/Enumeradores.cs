using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SME.SR.Infra.Enumeradores
{
    public static class Enumeradores
    {
        public enum FormatoEnum
        {
            [Display(Name = "pdf")]
            Pdf = 1,
            [Display(Name = "html")]
            Html = 2,
            [Display(Name = "xls")]
            Xls = 3,
            [Display(Name = "xlsx")]
            Xlsx = 4,
            [Display(Name = "rtf")]
            Rtf = 5,
            [Display(Name = "csv")]
            Csv = 6,
            [Display(Name = "xml")]
            Xml = 7,
            [Display(Name = "docx")]
            Docx = 8,
            [Display(Name = "odt")]
            Odt = 9,
            [Display(Name = "ods")]
            Ods = 10,
            [Display(Name = "jrprint")]
            Jrprint = 11,
        }

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
