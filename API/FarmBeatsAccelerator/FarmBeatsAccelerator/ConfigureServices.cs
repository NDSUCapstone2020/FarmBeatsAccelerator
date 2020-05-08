using FluentAssertions.Common;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
namespace FarmBeatsAccelerator
{
    public class ConfigureServices
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Region { get; set; }
    }
}