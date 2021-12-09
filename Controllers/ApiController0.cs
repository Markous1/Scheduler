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
            public const string EVENT_NOT_FOUND_OR_INACCESSABLE = "Event not found, OR you do not have permission to view it.";
            public const string INVALID_ARG = "Invalid arguments";
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

                public Event_spec() {
                    Title = "";
                    Description = "";
                    Start = -1;
                    End = -1;
                }

                public Event_spec(Event @event)
                {
                    this.Title = @event.Title;
                    this.Description = @event.Description;
                    this.Start = @event.StartDateTime.Ticks;
                    this.End = @event.EndDateTime.Ticks;
                }
            };

            public class Event_spec1 : Event_spec // includes owner user-name
            {
                public string OwnerUserName { get; set; } // username
                public long? Id { get; set; }
                public Event_spec1() : base()
                {
                    OwnerUserName = "";
                    Id = null;
                }
                public Event_spec1(Event @event) : base(@event)
                {
                    if (@event.Owner == null)
                    {
                        this.OwnerUserName = null;
                        this.Id = null;
                    }
                    else
                    {
                        this.OwnerUserName = @event.Owner.UserName;
                        this.Id = @event.Id;
                    }
                }
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
                public long id { get; set; }

                public int? returnhash { get; set; }
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

        public class _Response<Tpayload> {
            public int ResponseType { get; set; }
            public bool Success { get; set; }

            public string FailReason { get; set; }

            public Tpayload PayLoad { get; set; }

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

            private ApplicationUser GetAppUser()
            {
                Task<ApplicationUser> task = _userManager.GetUserAsync(_user);
                task.Wait();
                return task.Result;
            }

            #region -- standard responses
            private object convertResponse(int qtype, _Response<object> resp)
            {
                _Response<T> make<T>(_Response<object> r)
                {
                    return new _Response<T>
                    {
                        ResponseType = r.ResponseType,
                        Success = r.Success,
                        FailReason = r.FailReason,
                        PayLoad = (T)r.PayLoad,
                        ReturnHash = r.ReturnHash
                    };
                }
                switch (qtype)
                {
                    case Qtype.CREATE_EVENT:
                    case Qtype.DELETE_EVENT:
                    case Qtype.EDIT_EVENT:
                    case Qtype.GET_ONE_EVENT:
                        return (object)make<_Request.Event_spec1>(resp);
                    case Qtype.GET_EVENTS:
                        return (object)make<_Request.Event_spec1[]>(resp);
                    default: throw new Exception("INVALID TYPE-CODE");
                }
            }
            private object GenerateNullResponse(int qtype)
            {

                _Response<object> resp = new _Response<object>
                {
                    ResponseType = qtype,
                    Success = false,
                    FailReason = FailReason.NULL_REQ,
                    PayLoad = null,
                    ReturnHash = -1
                };
                return convertResponse(qtype, resp);
            }

            private _Response<_Request.Event_spec1> GenerateNotFoundResponse(int qtype, int? retHash)
            {
                return new _Response<_Request.Event_spec1>
                {
                    ResponseType = qtype,
                    Success = false,
                    FailReason = FailReason.EVENT_NOT_FOUND_OR_INACCESSABLE,
                    PayLoad = null,
                    ReturnHash = retHash
                };
            }

            private object GenerateInvalidResponse(int qtype)
            {

                _Response<object> resp = new _Response<object>
                {
                    ResponseType = qtype,
                    Success = false,
                    FailReason = FailReason.INVALID_JSON,
                    PayLoad = null,
                    ReturnHash = -1
                };
                return convertResponse(qtype, resp);
            }

            private object GenerateNeedLoginResponse<T>(int qtype, int? retHash)
            {
                _Response<object> resp = new _Response<object>
                {
                    ResponseType = qtype,
                    Success = false,
                    FailReason = FailReason.NEED_LOGIN,
                    PayLoad = null,
                    ReturnHash = retHash
                };
                return convertResponse(qtype, resp);
            }

            private _Response<Tpayload> Adaptr<Tpayload, Trequest>(int qtyp, string reqStr, Func<Trequest, _Response<Tpayload>> func)
            {
                if (reqStr == null) return (_Response<Tpayload>)GenerateNullResponse(qtyp);
                Trequest req;
                try
                {
                    req = JsonSerializer.Deserialize<Trequest>(reqStr);
                }
                catch
                {
                    return (_Response<Tpayload>)GenerateInvalidResponse(qtyp);
                }
                return func(req);
            }
            #endregion





            /* Process GetEvents request
             */
            public _Response<_Request.Event_spec1[]> Process_GE(_Request.GetEvents_requ req)
            {
                // check if request was null
                if (req == null) return new _Response<_Request.Event_spec1[]>
                {
                    Success = false,
                    ResponseType = Qtype.GET_EVENTS,
                    FailReason = FailReason.NULL_REQ,
                    ReturnHash = -1,
                    PayLoad = null
                };
                // get the current-user
                ApplicationUser user = GetAppUser();
                if (user == null) return new _Response<_Request.Event_spec1[]>
                {
                    Success = false,
                    ResponseType = Qtype.GET_EVENTS,
                    FailReason = FailReason.NEED_LOGIN,
                    ReturnHash = req.ReturnHash,
                    PayLoad = null
                };

                // get events for events 
                IQueryable<Event> eventSet = _context.Event.Where(e => e.Owner.Id.Equals(user.Id));
                // filter by date
                if (req.From != null) eventSet = eventSet.Where(e => e.StartDateTime.Ticks >= req.From);
                if (req.To != null) eventSet = eventSet.Where(e => e.StartDateTime.Ticks <= req.To);
                // remove sensitive user-info
                var payload = eventSet.Select(e => new _Request.Event_spec1(e)).ToArray();

                return new _Response<_Request.Event_spec1[]>
                {
                    Success = true,
                    ResponseType = Qtype.GET_EVENTS,
                    FailReason = FailReason.NA,
                    ReturnHash = req.ReturnHash,
                    PayLoad = payload
                };
            }

            public _Response<_Request.Event_spec1[]> Process_GE(string reqStr)
            {
                return Adaptr<_Request.Event_spec1[], _Request.GetEvents_requ>(Qtype.GET_EVENTS, reqStr, Process_GE);
            }

            /* Process GetOneEvent request
             */
            public _Response<_Request.Event_spec1> Process_GOE(_Request.GetOneEvent_requ req)
            {
                // check if request was null
                if (req == null) return (_Response<_Request.Event_spec1>)GenerateNullResponse(Qtype.GET_ONE_EVENT);

                // get the current-user
                ApplicationUser user = GetAppUser();
                if (user == null) return (_Response<_Request.Event_spec1>)GenerateNeedLoginResponse<_Request.Event_spec1>(Qtype.GET_ONE_EVENT, req.ReturnHash);

                // try to get the requested event
                IQueryable<_Request.Event_spec1> resultSet = _context.Event
                       .Where(x => x.Id.Equals(req.Id) && x.Owner.Id.Equals(user.Id))
                       .Select(x => new _Request.Event_spec1(x));

                // check if we found it
                if (!resultSet.Any()) return (_Response<_Request.Event_spec1>)GenerateNotFoundResponse(Qtype.GET_ONE_EVENT, req.ReturnHash);

                // extract the event
                _Request.Event_spec1 payload = resultSet.First();

                // return the event
                return new _Response<_Request.Event_spec1>
                {
                    Success = true,
                    ResponseType = Qtype.GET_ONE_EVENT,
                    FailReason = FailReason.NA,
                    ReturnHash = req.ReturnHash,
                    PayLoad = payload
                };
            }

            public _Response<_Request.Event_spec1> Process_GOE(string reqStr)
            {
                return Adaptr<_Request.Event_spec1, _Request.GetOneEvent_requ>(Qtype.GET_ONE_EVENT, reqStr, Process_GOE);
            }

            /* Process CreateEvent request
             */
            public _Response<_Request.Event_spec1> Process_CRE(_Request.CreateEvent_requ req)
            {
                // check if request was null
                if (req == null) return (_Response<_Request.Event_spec1>)GenerateNullResponse(Qtype.CREATE_EVENT);

                // get the current-user
                ApplicationUser user = GetAppUser();
                if (user == null) return (_Response<_Request.Event_spec1>)GenerateNeedLoginResponse<_Request.Event_spec1>(Qtype.CREATE_EVENT, req.ReturnHash);

                // check for valid start/stop :
                if (req.Spec.End < req.Spec.Start)
                    return new _Response<_Request.Event_spec1>
                    {
                        Success = false,
                        ResponseType = Qtype.CREATE_EVENT,
                        FailReason = FailReason.INVALID_ARG + "|An event cannot end before it begins!",
                        ReturnHash = req.ReturnHash,
                        PayLoad = null
                    };

                // extract the specification
                var spec = req.Spec;

                // build Event-model
                Event @event = new Event
                {
                    Description = spec.Description,
                    EndDateTime = new DateTime(ticks: spec.End),
                    StartDateTime = new DateTime(ticks: spec.Start),
                    Owner = user,
                    Title = spec.Title
                };

                // put new request in database
                _context.Event.Add(@event);

                // update-database
                _context.SaveChanges();


                // confirm-creation
                return new _Response<_Request.Event_spec1>
                {
                    Success = true,
                    FailReason = FailReason.NA,
                    PayLoad = new _Request.Event_spec1(@event),
                    ResponseType = Qtype.CREATE_EVENT,
                    ReturnHash = req.ReturnHash
                };
            }

            public _Response<_Request.Event_spec1> Process_CRE(string reqStr)
            {
                return Adaptr<_Request.Event_spec1, _Request.CreateEvent_requ>(Qtype.CREATE_EVENT, reqStr, Process_CRE);
            }


            /* Process DeleateEvent request
             */
            public _Response<_Request.Event_spec1> Process_DEL(_Request.DeleteEvent_requ req)
            {
                const int qtyp = Qtype.DELETE_EVENT;
                // check if request was null
                if (req == null) return (_Response<_Request.Event_spec1>)GenerateNullResponse(qtyp);

                // get the current-user
                ApplicationUser user = GetAppUser();
                if (user == null) return (_Response<_Request.Event_spec1>)GenerateNeedLoginResponse<_Request.Event_spec1>(qtyp, req.returnhash);

                // try to get the requested event
                IQueryable<Event> resultSet = _context.Event
                       .Where(x => x.Id.Equals(req.id) && x.Owner.Id.Equals(user.Id));

                // check if we found it
                if (!resultSet.Any()) return (_Response<_Request.Event_spec1>)GenerateNotFoundResponse(qtyp, req.returnhash);

                // extract the event
                Event toDelete = resultSet.First();

                // save the details
                _Request.Event_spec1 payload = new _Request.Event_spec1(toDelete);

                // delete the event
                _context.Remove(toDelete);

                // update db
                _context.SaveChanges();

                // return confirmation
                return new _Response<_Request.Event_spec1>
                {
                    Success = true,
                    FailReason = FailReason.NA,
                    ResponseType = qtyp,
                    PayLoad = payload,
                    ReturnHash = req.returnhash
                };
            }

            public _Response<_Request.Event_spec1> Process_DEL(string reqStr)
            {
                return Adaptr<_Request.Event_spec1, _Request.DeleteEvent_requ>(Qtype.DELETE_EVENT, reqStr, Process_DEL);
            }

            /* Process EditEvent request 
             */
            public _Response<_Request.Event_spec1[]> Process_EDI(_Request.EditEvent_requ req)
            {
                /* const int qtyp = Qtype.EDIT_EVENT;
                 // check if request was null
                 if (req == null) return (_Response<_Request.Event_spec1[]>)GenerateNullResponse(qtyp);

                 // get the current-user
                 ApplicationUser user = GetAppUser();
                 if (user == null) return (_Response<_Request.Event_spec1[]>)GenerateNeedLoginResponse<_Request.Event_spec1>(qtyp, req.ReturnHash);

                 // try to get the requested event
                 IQueryable<Event> resultSet = _context.Event
                        .Where(x => x.Id.Equals(req.id) && x.Owner.Id.Equals(user.Id));

                 // check if we found it
                 if (!resultSet.Any()) return (_Response<_Request.Event_spec1>)GenerateNotFoundResponse(qtyp, req.r);

                 // extract the event
                 Event toDelete = resultSet.First();

                 // save the details
                 _Request.Event_spec1 payload = new _Request.Event_spec1(toDelete); */

                /* TODO */

                return new _Response<_Request.Event_spec1[]>
                {
                    Success = false,
                    FailReason = "NOT-IMPLEMENTED",
                    ResponseType = Qtype.EDIT_EVENT,
                    PayLoad = null,
                    ReturnHash = -1
                };


                //return null;
            }
        }



        [Route("GetEvents/{reqStr}")]
        public string GetEvents(string reqStr) {
            var rpu = new RequestProcessUtil(_context, _userManager, User);
            var resp = rpu.Process_GE(reqStr);
            var respStr = JsonSerializer.Serialize<_Response<_Request.Event_spec1[]>>(resp);
            return respStr;
        }

        [Route("GetOneEvent/{reqStr}")]
        public string GetOneEvent(string reqStr) {
            var rpu = new RequestProcessUtil(_context, _userManager, User);
            var resp = rpu.Process_GOE(reqStr);
            var respStr = JsonSerializer.Serialize<_Response<_Request.Event_spec1>>(resp);
            return respStr;
        }

        [Route("CreateEvent/{reqStr}")]
        public string CreateEvent(string reqStr) 
        {
            var rpu = new RequestProcessUtil(_context, _userManager, User);
            var resp = rpu.Process_CRE(reqStr);
            var respStr = JsonSerializer.Serialize<_Response<_Request.Event_spec1>>(resp);
            return respStr;
        }

        [Route("DeleteEvent/{reqStr}")]
        public string DeleteEvent(string reqStr)
        {
            var rpu = new RequestProcessUtil(_context, _userManager, User);
            var resp = rpu.Process_DEL(reqStr);
            var respStr = JsonSerializer.Serialize<_Response<_Request.Event_spec1>>(resp);
            return respStr;
        }


        
    }
}
