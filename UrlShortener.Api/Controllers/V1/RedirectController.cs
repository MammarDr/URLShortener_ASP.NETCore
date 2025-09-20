using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Models.DTOs;
using UrlShortener.Models.DTOs.Url;

namespace UrlShortener.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")] 
    [Route("")]

    public class RedirectController(IServiceManager _sm, ILoggerManager _repo) : Controller
    {

        [HttpGet("{shortcode}")]
        [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSourceAndRedirect([FromRoute] string shortcode)
        {
            var res = await _sm.UrlService.RetrieveUrlByShortCode(shortcode);
            if(res.IsFailure || res.Value == null)
                return RedirectPermanent("https://localhost:7254/page/404");

            string ip = HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip == "::1")
                ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[4].ToString();

            await _sm.UrlService.RegisterView(res.Value, Request.Headers.UserAgent, ip); // Learn Fire and Run
            // Current issue, if return response, the context will be disposed, meaning it will be impossible.


            return RedirectPermanent("https://www." + res.Value.Source);
        }
    }
}
