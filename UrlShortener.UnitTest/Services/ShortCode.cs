using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Core.Services.Implementations;
using UrlShortener.Core.Services.Implementations;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data;
using UrlShortener.Data.Repositories;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.DTOs.Plan;
using UrlShortener.Models.Enums;
using UrlShortener.Models.Utility;
using Xunit.Abstractions;

namespace UrlShortener.UnitTest.Services
{
    public class ShortCodeTest 
    {
        private readonly IRepositoryManager _repoManager;
        private readonly IServiceManager _smManager;
        private readonly ILoggerManager _logger;
        private readonly ITestOutputHelper output;

        public ShortCodeTest(ITestOutputHelper op)
        {

            var provider = DiProvider.CreateServiceProvider("ShortCode");
            output = op;
            _repoManager = provider.GetRequiredService<IRepositoryManager>();
            _smManager   = provider.GetRequiredService<IServiceManager>();
            _logger      = provider.GetRequiredService<ILoggerManager>();
        }

        [Fact]
            public async Task ShortCode_Success_ReturnFiveCharCode()
            {
                  
                // Arrange

                // Act
                var result = await _smManager.ShortCodeService.Generate();

                // Assert
                output.WriteLine(result.Value);
                    Assert.Equal(ShortCodeGenerator.Length, result.Value.Length);
                }
            }
    
}
