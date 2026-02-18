using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.API.Controllers.Base;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController<T> : ControllerBase where T : BaseController<T>;
