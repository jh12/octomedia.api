using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace OctoMedia.Api.Controllers
{
    [ApiController]
    [Route("v1")]
    public class HealthController : ControllerBase
    {
        [HttpGet("version")]
        public string GetVersion()
        {
            string? informationalVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            return informationalVersion ?? "Unknown";
        }
    }
}