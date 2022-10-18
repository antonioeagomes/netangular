using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
    
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
       
    }
