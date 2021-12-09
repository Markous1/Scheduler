using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class Holidays
    {

        public class Rootobject
        {
            public Pagination pagination { get; set; }
            public Result[] results { get; set; }
        }

        public class Pagination
        {
            public int page { get; set; }
            public object prevPage { get; set; }
            public int nextPage { get; set; }
            public int totalPages { get; set; }
            public int size { get; set; }
            public int total { get; set; }
        }

        public class Result
        {
            public string name { get; set; }
            public string date { get; set; }
            public int day { get; set; }
            public int month { get; set; }
            public string year { get; set; }
            public int weekday { get; set; }
            public string weekdayName { get; set; }
            public string country { get; set; }
            public string countryName { get; set; }
            public Type[] types { get; set; }
            public Region[] regions { get; set; }
        }

        public class Type
        {
            public string name { get; set; }
            public string code { get; set; }
            public bool isReligious { get; set; }
            public bool isObservance { get; set; }
            public bool isLocal { get; set; }
        }

        public class Region
        {
            public int id { get; set; }
            public string abbrev { get; set; }
            public string name { get; set; }
            public object exception { get; set; }
            public string iso { get; set; }
        }

    }
}
