using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scheduler.Data;
using Microsoft.AspNetCore.Identity;
using Scheduler.Models;

namespace Scheduler.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController0 : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiController0(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region experiments
        /* This code is for testing only, and should NOT be included in final product
         */
        [Route("test")]
        public string Foo()
        {
            var list = _context.Model.GetEntityTypes().ToList();



            return string.Format("size=={0}",list.Count);

        }

        [Route("test/{ind}")]
        public string Foo(int ind)
        {
            var list = _context.Model.GetEntityTypes().ToList();
            if (ind < 0 || ind >= list.Count) return "INDEX OUT OF RANGE";
            var item = list.ElementAt(ind);

            
            //                         Name =
            return string.Format("{2}\nIndex={0}\nName ={1}",ind,item.Name,item);

        }

        [Route("test/all")]
        public string Phi()
        {
            const string SEP = "\n----------------------------\n";
            string format_item(Microsoft.EntityFrameworkCore.Metadata.IEntityType item, int ind)
            {
                return string.Format("{2}\nIndex={0}\nName ={1}", ind, item.Name, item);
            }
            IEnumerable<string> EnuMech<T>( Func<T,int,string> func, IEnumerable<T> enu )
            {
                int ind = 0;
                foreach (T item in enu)
                {
                    yield return func(item, ind++);
                }
            }
            var enu0 = _context.Model.GetEntityTypes();
            var enu = EnuMech(format_item, enu0);

            return string.Join(SEP, enu);
        }
        #endregion
    }
}
