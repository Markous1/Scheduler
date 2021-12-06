using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scheduler.Data;
using Microsoft.AspNetCore.Identity;
using Scheduler.Models;
using System.Text.Json;

namespace Scheduler.Controllers
{
    [Route("api/v0.0")]
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
        #region test
        // |        [Route("test")]
        // |        public string Foo()
        // |        {
        // |            var list = _context.Model.GetEntityTypes().ToList();
        // |
        // |
        // |
        // |            return string.Format("size=={0}",list.Count);
        // |
        // |        }
        // |
        // |        [Route("test/{ind}")]
        // |        public string Foo(int ind)
        // |        {
        // |            var list = _context.Model.GetEntityTypes().ToList();
        // |            if (ind < 0 || ind >= list.Count) return "INDEX OUT OF RANGE";
        // |            var item = list.ElementAt(ind);
        // |
        // |            
        // |            //                         Name =
        // |            return string.Format("{2}\nIndex={0}\nName ={1}",ind,item.Name,item);
        // |
        // |        }
        // |
        // |        [Route("test/all")]
        // |        public string Phi()
        // |        {
        // |            const string SEP = "\n----------------------------\n";
        // |            string format_item(Microsoft.EntityFrameworkCore.Metadata.IEntityType item, int ind)
        // |            {
        // |                return string.Format("{2}\nIndex={0}\nName ={1}", ind, item.Name, item);
        // |            }
        // |            IEnumerable<string> EnuMech<T>( Func<T,int,string> func, IEnumerable<T> enu )
        // |            {
        // |                int ind = 0;
        // |                foreach (T item in enu)
        // |                {
        // |                    yield return func(item, ind++);
        // |                }
        // |            }
        // |            var enu0 = _context.Model.GetEntityTypes();
        // |            var enu = EnuMech(format_item, enu0);
        // |
        // |            return string.Join(SEP, enu);
        // |        }
        #endregion



        #region test0
        // |        public class Helper {
        // |            private readonly ApplicationDbContext _context;
        // |            private readonly UserManager<ApplicationUser> _userManager;
        // |
        // |            public Helper(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        // |            {
        // |                _context = context;
        // |                _userManager = userManager;
        // |            }
        // |            public Event[] GetEvents(IdentityUser user) 
        // |            {
        // |                return _context.Event.Where(e => e.Owner.Id.Equals(user.Id)).ToArray();
        // |            }
        // |
        // |            public class AugUser /* augmented-user */
        // |            {
        // |                public IdentityUser user { get; set; }
        // |                public Event[] events {get; set;}
        // |            }
        // |
        // |            public AugUser GetAugUser(IdentityUser _user)
        // |            {
        // |                Event[] _events = GetEvents(_user);
        // |                return new AugUser { user=_user, events=_events  };
        // |            }
        // |
        // |            
        // |        }
        // |        [Route("test")]
        // |        public string Foo()
        // |        {
        // |            Helper helper = new Helper(_context,_userManager);
        // |            var users = _context.Users.ToList();
        // |            var AugUsers = users.Select(helper.GetAugUser).ToArray();
        // |            var result = JsonSerializer.Serialize(AugUsers);
        // |            return result;
        // |        }
        #endregion
        #endregion

        /* API : v0.0
         *     - get-events          {from , to}
         *     - get SPECIFIC event {id}
         *     - delete-event      {id, hash?}
         *     - create-event       {..data..}
         *     - edit-event         {id, hash?, new_data}
         *     
         *     - responses : 
         *        { 
         *           responseType 
         *           request-hash
         *           bool success
         *           failReason?
         *          .. data ..
         *        }
         *        
         *        get-response : 
         *          payLoad?
         *        
         *        deleate-response : 
         *          ..
         *        
         *        create-event-response : 
         *          ..
         *          
         *        edit-event-response : 
         *          ..
         *          
         *        
         */

        public class FailReason {
            public const string NEED_LOGIN = "You are not logged in";
            public const string INVALID_JSON = "Your request could not be parsed";
        }
        public class Qtype
        {
            public const int GET_EVENTS = 0;
            public const int GET_ONE_EVENT = 1;
            public const int DELETE_EVENT = 2;
            public const int CREATE_EVENT = 3;
            public const int EDIT_EVENT = 4;

            
        }
        public class _Request
        {
            public const string QTYPE_PROPERTY_NAME = "Qtype";
            public class Event_spec
            {
                public string Title { get; set; }
                public string Description { get; set; }
                public long Start { get; set; } // measured in ticks
                public long End { get; set; }   // measured in ticks
            };

            public class GetEvents_requ
            {
                public long? From { get; set; }
                public long? To { get; set; }
            }

            public class GetOneEvent_requ
            {
                public long Id { get; set; }
            }

            public class DeleteEvent_requ
            {
                public long Id { get; set; }
            }

            public class CreateEvent_requ
            {
                public Event_spec Spec { get; set; }
            }

            public class EditEvent_requ
            {
                public long Id { get; set; }
                public Event_spec NewSpec { get; set; }
            }


        }

        



    }
}
