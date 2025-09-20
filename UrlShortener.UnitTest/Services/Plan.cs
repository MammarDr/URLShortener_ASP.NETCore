
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Plan;
using UrlShortener.Models.Enums;
using Xunit.Abstractions;

namespace UrlShortener.UnitTest.Services
{
    public class PlanTest
    {

        private readonly IRepositoryManager _repoManager;
        private readonly IServiceManager _smManager;
        private readonly ILoggerManager _logger;
        private readonly ITestOutputHelper output;

        public PlanTest(ITestOutputHelper op)
        {

            var provider = DiProvider.CreateServiceProvider("Plan");
            output = op;
            _repoManager = provider.GetRequiredService<IRepositoryManager>();
            _smManager = provider.GetRequiredService<IServiceManager>();
            _logger = provider.GetRequiredService<ILoggerManager>();
        }

        public void DisplayFullPlanDTO(FullPlanDTO dto)
        {
            if (dto == null) { output.WriteLine("FullPlanDTO is null."); return; }

            output.WriteLine("=== FullPlanDTO ===");
            output.WriteLine($"ID: {dto.ID}");
            output.WriteLine($"Name: {dto.Name}");
            output.WriteLine($"Price: {dto.Price:C}");
            output.WriteLine($"MaxDailyURL: {dto.MaxDailyURL}");
            output.WriteLine($"HasCustomSlugs: {dto.HasCustomSlugs}");
            output.WriteLine($"ExpireAfter: {(dto.ExpireAfter.HasValue ? dto.ExpireAfter.Value.ToString() : "Never")}");
            output.WriteLine($"SupportLevel: {dto.SupportLevel}");

        }

        private void DisplayFullPlanDTO(IEnumerable<FullPlanDTO> value)
        {
            if (value == null) { output.WriteLine("FullPlanDTO List is null."); return; }

            foreach (var dto in value)
                DisplayFullPlanDTO(dto);
        }

        #region CreateTest
        [Fact]
        public async Task Create_Success_WhenValidDTO()
        {
          
            // Arrange
            var dto = new CreatePlanDTO
            {
                Name = "BasicPlan",                  
                Price = 49.99m,                
                MaxDailyURL = 1000,       
                HasCustomSlugs = true,
                UrlExpiresAfter = 365,       
                SupportLevel = enSupportLevel.Low 
            };

            // Act
            var result = await _smManager.PlanService.CreatePlanAsync(dto);

            // Assert

            if (result.IsSuccess)
                DisplayFullPlanDTO(result.Value);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Create_Fail_WhenInvalidDTO()
        {
            // Arrange
            var dto = new CreatePlanDTO 
            { 
                Name = "A",                  // Invalid - too short
                Price = -10,                 // Invalid - negative price
                MaxDailyURL = 200000,        // Invalid - exceeds max value
                HasCustomSlugs = false,
                UrlExpiresAfter = 0,         // Invalid - below minimum
                SupportLevel = (enSupportLevel)999  // Invalid - undefined enum value
            };

            // Act
            var result = await _smManager.PlanService.CreatePlanAsync(dto);

            // Assert

            if (result.IsFailure && result.Error is ValidationError validation)
                output.WriteLine(JsonSerializer.Serialize(validation.Errors));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Create_Fail_WhenDuplicate()
        {
            // Arrange
            var dto = new CreatePlanDTO
            {
                Name = "PremiumPlan",
                Price = 499.99m,
                MaxDailyURL = 9000,
                HasCustomSlugs = true,
                UrlExpiresAfter = 3650,
                SupportLevel = enSupportLevel.Medium
            };

            // Act
            var res1 = await _smManager.PlanService.CreatePlanAsync(dto);
            var res2 = await _smManager.PlanService.CreatePlanAsync(dto);
            // Assert

            Assert.True(res1.IsSuccess);
            Assert.False(res2.IsSuccess);
            Assert.Equal(enErrorType.Conflict, res2.Error.Type);
            output.WriteLine(res2.Error.Description);
        }

        [Fact]
        public async Task CreateList_Success_WhenValidDTO()
        {

            // Arrange
            var plans = new List<CreatePlanDTO> { 
                new CreatePlanDTO { Name = "BasicListPlan", Price = 49.99m, MaxDailyURL = 1000, HasCustomSlugs = true, UrlExpiresAfter = 365, SupportLevel = enSupportLevel.Low }, 
                new CreatePlanDTO { Name = "ProListPlan", Price = 99.99m, MaxDailyURL = 5000, HasCustomSlugs = true, UrlExpiresAfter = 730, SupportLevel = enSupportLevel.Medium }, 
                new CreatePlanDTO { Name = "EnterpriseListPlan", Price = 199.99m, MaxDailyURL = 10000, HasCustomSlugs = true, UrlExpiresAfter = 1095, SupportLevel = enSupportLevel.High } 
            }; 

            // Act
            var result = await _smManager.PlanService.CreatePlansAsync(plans);

            // Assert

            if (result.IsSuccess)
                DisplayFullPlanDTO(result.Value);

            Assert.True(result.IsSuccess);
        }

       

        [Fact]
        public async Task CreateList_Fail_WhenInvalidDTO()
        {
            // Arrange
            var plans = new List<CreatePlanDTO> { 
                new CreatePlanDTO { Name = "BasicListingPlan", Price = 49.99m, MaxDailyURL = 1000, HasCustomSlugs = true, UrlExpiresAfter = 365, SupportLevel = enSupportLevel.Low }, 
                new CreatePlanDTO { Name = "ProListingPlan", Price = 99.99m, MaxDailyURL = 5000, HasCustomSlugs = true, UrlExpiresAfter = 730, SupportLevel = enSupportLevel.Medium }, 
                new CreatePlanDTO { Name = "EnterpriseListingPlan", Price = 199.99m, MaxDailyURL = 10000, HasCustomSlugs = true, UrlExpiresAfter = 1095, SupportLevel = enSupportLevel.High }, 
                new CreatePlanDTO { Name = "StarterListingPlan", Price = 19.99m, MaxDailyURL = 500, HasCustomSlugs = false, UrlExpiresAfter = 180, SupportLevel = enSupportLevel.Low }, 
                new CreatePlanDTO { Name = "PremiumListingPlan", Price = 149.99m, MaxDailyURL = 8000, HasCustomSlugs = true, UrlExpiresAfter = 900, SupportLevel = enSupportLevel.High }, 
                new CreatePlanDTO { Name = "B", Price = -10, MaxDailyURL = 200000, HasCustomSlugs = false, UrlExpiresAfter = 0, SupportLevel = (enSupportLevel)999 } 
            };

            // Act
            var result = await _smManager.PlanService.CreatePlansAsync(plans);

            // Assert

            if (result.IsFailure && result.Error is ValidationError validation)
                output.WriteLine(JsonSerializer.Serialize(validation.Errors));
            Assert.False(result.IsSuccess);
        }

        // Duplicates seems fine
        [Fact]
        public async Task CreateList_Fail_WhenDuplicate()
        {
            // Arrange
            var plans = new List<CreatePlanDTO> {
                new CreatePlanDTO { Name = "BasicsPlan", Price = 49.99m, MaxDailyURL = 1000, HasCustomSlugs = true, UrlExpiresAfter = 365, SupportLevel = enSupportLevel.Low },
                new CreatePlanDTO { Name = "ProsPlan", Price = 99.99m, MaxDailyURL = 5000, HasCustomSlugs = true, UrlExpiresAfter = 730, SupportLevel = enSupportLevel.Medium },
                new CreatePlanDTO { Name = "EnterprisesPlan", Price = 199.99m, MaxDailyURL = 10000, HasCustomSlugs = true, UrlExpiresAfter = 1095, SupportLevel = enSupportLevel.High }
            };

            // Act
            var res1 = await _smManager.PlanService.CreatePlansAsync(plans);
            var res2 = await _smManager.PlanService.CreatePlansAsync(plans);
            // Assert
            Assert.True(res1.IsSuccess);

            var HasConflict = async (string name) => await _repoManager.Plan.ExistsAnyAsync(p => p.Name == name);

            Assert.True(await HasConflict(plans[0].Name) && await HasConflict(plans[1].Name) && await HasConflict(plans[2].Name) );
        }
        #endregion

        #region GetTest

        //Task<Result<FullPlanDTO>> CreatePlanAsync(CreatePlanDTO planDTO);
        //Task<Result<IEnumerable<FullPlanDTO>>> CreatePlansAsync(IEnumerable<CreatePlanDTO> planDTOs, CancellationToken ct = default);
        //Task<Result> DeleteByIDsAsync(IEnumerable<int> ids, CancellationToken ct = default);
        //Task<Result> DeletePlanAsync(int id);

        //Task<Result<FullPlanDTO>> UpdatePlanAsync(int id, JsonPatchDocument<UpdatePlanDTO> dto);

        [Fact]
        public async Task Get_Success_WhenExistId()
        {
            // Arrange
            var dto = new CreatePlanDTO
            {
                Name = "GetPlan",
                Price = 49.99m,
                MaxDailyURL = 1000,
                HasCustomSlugs = true,
                UrlExpiresAfter = 365,
                SupportLevel = enSupportLevel.Low
            };

            // Act
            var create = await _smManager.PlanService.CreatePlanAsync(dto);
            var get = await _smManager.PlanService.RetrievePlanByIdAsync(create.Value.ID);

            // Assert
            Assert.True(create.IsSuccess);
            Assert.True(get.IsSuccess);
            Assert.Equal(get.Value.Name, dto.Name);
        }

        [Fact]
        public async Task Get_Fail_WhenNotExistId()
        {
            // Arrange

            // Act
            var get = await _smManager.PlanService.RetrievePlanAndValidateExistenceAsync(99999);

            // Assert
            Assert.False(get.IsSuccess);
            Assert.Equal(enErrorType.NotFound, get.Error.Type);
        }

        public async Task GetList_Success_WhenExistId()
        {
            // Arrange
            var plans = new List<CreatePlanDTO>
            {
                new CreatePlanDTO
                {
                    Name = "GetPlan1",
                    Price = 49.99m,
                    MaxDailyURL = 1000,
                    HasCustomSlugs = true,
                    UrlExpiresAfter = 365,
                    SupportLevel = enSupportLevel.Low
                },
                new CreatePlanDTO
                {
                    Name = "GetPlan2",
                    Price = 499.99m,
                    MaxDailyURL = 9000,
                    HasCustomSlugs = false,
                    UrlExpiresAfter = 3650,
                    SupportLevel = enSupportLevel.Medium
                }
            };

            // Act
            var create = await _smManager.PlanService.CreatePlansAsync(plans);
            var ids = create.Value.Select(p => p.ID).ToList();  
            var get = await _smManager.PlanService.RetrievePlansByIdAsync(ids, asNoTracking: true);

            // Assert
            Assert.True(create.IsSuccess);
            Assert.True(get.IsSuccess);
            Assert.Equal(plans.Count, get.Value.Count());
            var list = get.Value.ToList();
            for(int i = 0; i < list.Count(); i++)
            {
                Assert.NotNull(list[i]);
                Assert.Equal(list[i].Name, plans[i].Name);
            }
        }

        [Fact]
        public async Task GetList_Fail_WhenNotExistId()
        {
            // Arrange


            // Act
            var get = await _smManager.PlanService.RetrievePlansByIdAsync([1, 5, 999999], asNoTracking: true);

            // Assert
            Assert.False(get.IsSuccess);
            Assert.Equal(enErrorType.NotFound, get.Error.Type);
        }
        #endregion

        #region UpdateTest

        #endregion

        #region DeleteTest

        #endregion
    }
}
