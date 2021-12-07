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
using System.Security.Claims;

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
            public const string NULL_REQ = "The request was null";

            public const string NA = "N/A";
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
                
                public int? ReturnHash { get; set; }
            };

            public class Event_spec1 : Event_spec // includes owner user-name
            {
                public string OwnerUserName { get; set; } // username
            }

            public class GetEvents_requ
            {
                public long? From { get; set; }
                public long? To { get; set; }

                public int? ReturnHash { get; set; }
            }

            public class GetOneEvent_requ
            {
                public long Id { get; set; }

                public int? ReturnHash { get; set; }
            }

            public class DeleteEvent_requ
            {
                public long Id { get; set; }

                public int? ReturnHash { get; set; }
            }

            public class CreateEvent_requ
            {
                public Event_spec Spec { get; set; }
                public int? ReturnHash { get; set; }
            }

            public class EditEvent_requ
            {
                public long Id { get; set; }
                public Event_spec NewSpec { get; set; }

                public int? ReturnHash { get; set; }
            }


        }

        public class _Respose {
            public int ResponseType { get; set; }
            public bool Success { get; set; }

            public string FailReason { get; set; }

            public string PayLoad { get; set; }

            public int? ReturnHash { get; set; }
        }
        private class RequestProcessUtil
        {
            #region setup
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;
            private ClaimsPrincipal _user;
            public RequestProcessUtil(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ClaimsPrincipal user)
            {
                _context = context;
                _userManager = userManager;
                _user = user;
            }
            #endregion

            private ApplicationUser GetAppUser() {
                Task<ApplicationUser> task = _userManager.GetUserAsync(_user);
                task.Wait();
                return task.Result;
            }

            public _Respose Process_GE(_Request.GetEvents_requ req)
            {
                // check if request was null
                if (req == null) return new _Respose
                {
                    Success = false,
                    ResponseType = Qtype.GET_EVENTS,
                    FailReason = FailReason.NULL_REQ,
                    ReturnHash = -1,
                    PayLoad = ""
                };
                // get the current-user
                ApplicationUser user = GetAppUser();
                if (user == null) return new _Respose
                {
                    Success = false,
                    ResponseType = Qtype.GET_EVENTS,
                    FailReason = FailReason.NEED_LOGIN,
                    ReturnHash = req.ReturnHash,
                    PayLoad = ""
                };

                // get events for events 
                IQueryable<Event> eventSet = _context.Event.Where(e => e.Owner.Id.Equals(user.Id));
                // filter by date
                if (req.From != null) eventSet.Where(e => e.StartDateTime.Ticks >= req.From);
                if(req.To != null) eventSet.Where(e => e.StartDateTime.Ticks <= req.To);
                // remove sensitive user-info
                eventSet.Select(e => new _Request.Event_spec1 {
                    Title = e.Title,
                    Description = e.Description,
                    Start = e.StartDateTime.Ticks,
                    End = e.EndDateTime.Ticks,
                    OwnerUserName = user.UserName
                });

                // serialize-result
                string _payload = JsonSerializer.Serialize(eventSet.ToArray());

                return new _Respose {
                    Success = true,
                    ResponseType = Qtype.GET_EVENTS,
                    FailReason = FailReason.NA,
                    ReturnHash = req.ReturnHash,
                    PayLoad = _payload
                };
            }
        }


        [Route("GetEvents")]
        public _Respose GetEvents(_Request.GetEvents_requ req) {
            var rpu = new RequestProcessUtil(_context, _userManager, User);
            return rpu.Process_GE(req);
        }


    }
}
