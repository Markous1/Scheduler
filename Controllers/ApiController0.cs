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
    /* Controller class handles requests to this application's api (version 0.0)
     */
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

        /* This class contains string constants with human-readable explanations for why I a query to the api failed
         */
        public class FailReason {
            public const string NEED_LOGIN = "You are not logged in";
            public const string INVALID_JSON = "Your request could not be parsed";
            public const string NULL_REQ = "The request was null";
            public const string EVENT_NOT_FOUND_OR_INACCESSABLE = "Event not found, OR you do not have permission to view it.";
            public const string INVALID_ARG = "Invalid arguments";
            public const string NA = "N/A"; // Not-Applicable : for when the query did NOT fail
        }

        /*  This class contains int constants that encode the types of query this api handles
         */
        public class Qtype
        {
            public const int GET_EVENTS = 0;     // Get all events in a given time-range 
            public const int GET_ONE_EVENT = 1;  // Get one event by its id
            public const int DELETE_EVENT = 2;   // delete one event by its id
            public const int CREATE_EVENT = 3;   // create a new event
            public const int EDIT_EVENT = 4;     // edit an already existing event
        }

        /* This class exposes subclasses representing requests to the api.
         * The class-name starts with and underscore, so as not to conflict with existing class name 'Request'
         */
        public class _Request
        {
            
            public const string QTYPE_PROPERTY_NAME = "Qtype";

            /* Represents an 'specification', for creating a new event, or Editing an existing event. 
             * This has properties corresponding to the Event Model, but does NOT contain fields for 
             * for Id or the Owner, because the Id is generated by the database, and the Owner is 
             * whoever is whoever created the event.
             */
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

                public Event_spec(Event @event) // constructor copies data from an existing Event
                {
                    this.Title = @event.Title;
                    this.Description = @event.Description;
                    this.Start = @event.StartDateTime.Ticks;
                    this.End = @event.EndDateTime.Ticks;
                }
            };
            /* This extension of Event_spec includes the Event-Id and the Owner-Username.
             * This is meant to be used in responses to GetEvent requests.
             * NOTE : this class only contains the UserName of the owner, and not the entire
             * Owner model, so as not expose sensitive data such as password hashes.
             */
            public class Event_spec1 : Event_spec 
            {
                public string OwnerUserName { get; set; } 
                public long? Id { get; set; }
                public Event_spec1() : base()
                {
                    OwnerUserName = "";
                    Id = null;
                }
                public Event_spec1(Event @event) : base(@event) // constructor copies data from an existing Event
                {
                    if (@event.Owner == null)
                    {
                        this.OwnerUserName = null;
                    }
                    else
                    {
                        this.OwnerUserName = @event.Owner.UserName;
                    }
                    this.Id = @event.Id;
                }
            }

            /* This class represents a 'GetEvents' request.
             */
            public class GetEvents_requ
            {
                public long? From { get; set; }  // earliest event date (in ticks)
                public long? To { get; set; }    // latest event date (in ticks)

                public int? ReturnHash { get; set; } // optional parameter that will be copied into the response.
            }

            /* This class represents a 'GetOneEvent' request
             */
            public class GetOneEvent_requ
            {
                public long Id { get; set; }  // Id of the event to retrieve

                public int? ReturnHash { get; set; } // optional parameter that will be copied into the response.
            }


            /* This class represents a 'DeleteEvent' request
             */
            public class DeleteEvent_requ
            {
                public long id { get; set; } // Id of the event to retrieve

                public int? returnhash { get; set; } // optional parameter that will be copied into the response.
            }

            /* This class represents a 'CreateEvent' request
            */
            public class CreateEvent_requ
            {
                public Event_spec Spec { get; set; } // Blueprint for the event to create
                public int? ReturnHash { get; set; } // optional parameter that will be copied into the response.
            }

            /* This class represents a 'CreateEvent' request
             */
            public class EditEvent_requ
            {
                public long Id { get; set; }             // Id of the event to Edit
                public Event_spec NewSpec { get; set; }  // New Blueprint for the event

                public int? ReturnHash { get; set; } // optional parameter that will be copied into the response.
            }


        }

        /* This class represents a response to an api request.
         * ( the name starts with an underscore for the same reason as '_Request' class above )
         * 
         *    type-parameter Tpayload : the type for data returned by this response
         */
        public class _Response<Tpayload> {
            public int ResponseType { get; set; }  // The type of query this is a response to (see 'Qtype' class above)
            public bool Success { get; set; }      // Indicates whether the query was successful

            public string FailReason { get; set; } // If the query failed, this field contains a Human-readable reason why the query failed

            public Tpayload PayLoad { get; set; }  // data returned by this response

            public int? ReturnHash { get; set; }   // copy of the 'ReturnHash' included with the request
        }
 
        /*  This class contains helper-functions for processing calls to this api.
         */
        private class RequestProcessUtil
        {
            #region setup
            private readonly ApplicationDbContext _context;              // current-context
            private readonly UserManager<ApplicationUser> _userManager;  // user-manager
            private ClaimsPrincipal _user;                               // claims-principal for current-user
            public RequestProcessUtil(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ClaimsPrincipal user)
            {
                _context = context;
                _userManager = userManager;
                _user = user;
            }
            #endregion
            /* Return the current ApplicationUser (will return null if the user is not logged in)
             */
            private ApplicationUser GetAppUser()
            {
                Task<ApplicationUser> task = _userManager.GetUserAsync(_user);
                task.Wait();
                return task.Result;
            }
            /* This section contains helper-methods for generating responses to failed querries.
             * ( to reduce repetitive-code ).
             */
            #region -- standard responses

            /*   Converts a 'generic' response ( with payload-type 'object' ) to a 
             *   response appropriate for a given Qtype. ( the output is returned as an object )
             *   
             *   parameters : 
             *      int               qtype : the 'Qtype' for a querry
             *      _Response<object> resp  : response to convert
             *   returns : 
             *       an object that can be cast to type _Response<T>, where T is an appropriote payload type for 
             *       for the qtype.
             *       
             *   May throw exception if : 
             *        * qtype is NOT a constant in Qtype
             *        * resp.PayLoad  cannot be cast as the needed type
             */
            private object convertResponse(int qtype, _Response<object> resp)
            {
                /*  local method copies the properties of a given _Response<object> r, and casts the payload to a given type T
                 */
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
                // use the Make<T> method above to convert resp appropriately for qtype
                switch (qtype)
                {
                    case Qtype.CREATE_EVENT:  // response to 'CreateEvent' will have a copy of the new event
                    case Qtype.DELETE_EVENT:  // response to 'DeleateEvent' will have a copy of the deleted event
                    case Qtype.GET_ONE_EVENT: // response to 'GetOneEvent' will have a copy of the retrieved event
                        return (object)make<_Request.Event_spec1>(resp);
                    case Qtype.GET_EVENTS:    // response to 'GetEvents' will have an array of retrived events
                    case Qtype.EDIT_EVENT:    // response to 'EditEvent' will have an array of events [old,new] to show the changes
                        return (object)make<_Request.Event_spec1[]>(resp);
                    default: throw new Exception("INVALID TYPE-CODE"); // throw exception if qtype is not supported
                }
            }

            /*  Generate a response to a request where the parameter was null.
             *  ( In fairness, I don't think the middleware lets you send a request with a null paramater, but this is cover 
             *    our bases ).
             *    
             *    parameteres : 
             *       int qtype : the Qtype for the request
             *    returns : 
             *        and object that can be cast to type _Response<Tpayload>, where Tpayload is appropriote for qtype.
             *    dependencies : convertResponse method above
             */
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

            /* Generate a response to a request that refers to an event that does not exist 
             * ( or one that does exist, but that the user does not own ).
             * 
             *    parameteres : 
             *       int qtype : the Qtype for the request
             *    returns : a response
             */
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

            /* Generate a response to a request that could not be properly parsed
             * 
             *    parameteres : 
             *       int qtype : the Qtype for the request
             *    returns : 
             *        and object that can be cast to type _Response<Tpayload>, where Tpayload is appropriote for qtype.
             *    dependencies : convertResponse method above
             */
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

            /* Generate a response to a request where the user is not logged in
             * 
             *    parameteres : 
             *       int qtype    : the Qtype for the request
             *       int? retHash : the return-hash included with the request
             *       
             *    returns : 
             *        and object that can be cast to type _Response<Tpayload>, where Tpayload is appropriote for qtype.
             *    dependencies : convertResponse method above
             */
            private object GenerateNeedLoginResponse(int qtype, int? retHash)
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
            /* This method acts an 'adapter' for methods that handle queries in 'object'-form so they can be used to process 'string'-queries
             *
             *    type-parameters : 
             *         Tpayload : the payload-type to be used in a response
             *         Trequest : the request-type to be processed
             *    parameters : 
             *         int                                qtyp   : the 'Qtype' that corresponds to the querry being processed
             *         string                             reqStr : a json string that encodes the request-arguments
             *         Func<Trequest,_Response<Tpayload>> func   : a method that processes a Trequest, and returns a _Response<Tpayload> 
             *    
             *    returns : 
             *         a _Response<Tpayload> response to the request encoded by reqStr.
             *    effects : 
             *        func is run on the request, which may update the database accordingly
             */
            private _Response<Tpayload> Adaptr<Tpayload, Trequest>(int qtyp, string reqStr, Func<Trequest, _Response<Tpayload>> func)
            {
                // check if the input request-string was null.
                if (reqStr == null) return (_Response<Tpayload>)GenerateNullResponse(qtyp);
                Trequest req;
                try/* try to parse the request-string */
                {
                    req = JsonSerializer.Deserialize<Trequest>(reqStr);
                }
                catch /* If the string could not be parsed, then the query fails */
                {
                    return (_Response<Tpayload>)GenerateInvalidResponse(qtyp);
                }
                // run the query and return the result
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
                // remove sensitive user-info, and order by date
                var payload = eventSet.OrderBy(e => e.StartDateTime)              /* remove sensitive user-data            */
                    .Select(e => new _Request.Event_spec1(e))                     /* order-by-startDate                    */
                    .ToArray();                                                   /* convert to (Json-friendly) array form */

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
                if (user == null) return (_Response<_Request.Event_spec1>)GenerateNeedLoginResponse(Qtype.GET_ONE_EVENT, req.ReturnHash);

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
                if (user == null) return (_Response<_Request.Event_spec1>)GenerateNeedLoginResponse(Qtype.CREATE_EVENT, req.ReturnHash);

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
                if (user == null) return (_Response<_Request.Event_spec1>)GenerateNeedLoginResponse(qtyp, req.returnhash);

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
                 const int qtyp = Qtype.EDIT_EVENT;
                // check if request was null
                if (req == null) return new _Response<_Request.Event_spec1[]> { 
                    Success = false,
                    FailReason = FailReason.NULL_REQ,
                    ResponseType = qtyp,
                    PayLoad = null,
                    ReturnHash = -1
                };
                
                 // get the current-user
                 ApplicationUser user = GetAppUser();
                 if (user == null) return (_Response<_Request.Event_spec1[]>)GenerateNeedLoginResponse(qtyp, req.ReturnHash);
                
                 // try to get the requested event
                 IQueryable<Event> resultSet = _context.Event
                        .Where(x => x.Id.Equals(req.Id) && x.Owner.Id.Equals(user.Id));

                // check if we found it
                if (!resultSet.Any()) return new _Response<_Request.Event_spec1[]> {
                    Success = false,
                    FailReason = FailReason.EVENT_NOT_FOUND_OR_INACCESSABLE,
                    PayLoad = null,
                    ResponseType = qtyp,
                    ReturnHash = req.ReturnHash
                 };
                
                 // extract the event
                 Event toEdit = resultSet.First();

                // validate the proposed edit
                if (req.NewSpec.End < req.NewSpec.Start) return (_Response<_Request.Event_spec1[]>)GenerateInvalidResponse(qtyp);

                
                 // save the details of the original-version
                 _Request.Event_spec1 originalEvent = new _Request.Event_spec1(toEdit);

                // edit the event
                toEdit.Title = req.NewSpec.Title;
                toEdit.Description = req.NewSpec.Description;
                toEdit.StartDateTime = new DateTime(ticks: req.NewSpec.Start);
                toEdit.EndDateTime = new DateTime(ticks: req.NewSpec.End);

                // save details of the new-version
                _Request.Event_spec1 newEvent = new _Request.Event_spec1(toEdit);

                // update the database
                _context.SaveChanges();

                // return confirmation
                return new _Response<_Request.Event_spec1[]>
                {
                    Success = true,
                    FailReason = FailReason.NA,
                    ResponseType = qtyp,
                    PayLoad = new _Request.Event_spec1[] { originalEvent, newEvent },
                    ReturnHash = req.ReturnHash
                };

            }
            public _Response<_Request.Event_spec1[]> Process_EDI(string reqStr)
            {
                return Adaptr<_Request.Event_spec1[], _Request.EditEvent_requ>(Qtype.EDIT_EVENT, reqStr, Process_EDI);
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

        [Route("EditEvent/{reqStr}")]
        public string EditEVent(string reqStr)
        {
            var rpu = new RequestProcessUtil(_context, _userManager, User);
            var resp = rpu.Process_EDI(reqStr);
            var respStr = JsonSerializer.Serialize<_Response<_Request.Event_spec1[]>>(resp);
            return respStr;
        }


        
    }
}
