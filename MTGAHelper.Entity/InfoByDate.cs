using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class InfoByDate<T>
    {
        public DateTime DateTime;
        public T Info;

        public InfoByDate(DateTime datetime, T info)
        {
            DateTime = datetime;
            Info = info;
        }
    }

}
