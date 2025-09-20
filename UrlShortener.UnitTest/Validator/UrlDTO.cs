using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Validators;
using UrlShortener.Models.DTOs.Url;

namespace UrlShortener.UnitTest.Validator;

public class UrlDtoValidatior
{
    [Fact]
    public void CreateDTO_Success_WhenSourceIsValid()
    {
        // Arrange
        var dto = new CreateUrlDTO
        {
            Source = "https://www.google.com",
            Title = "Title",
            isActive = true
        };

        // Act
        var result = dto.Validate();
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void CreateDTO_Fail_WhenSourceIsInvalid()
    {
        // Arrange
        var dto = new CreateUrlDTO
        {
            Source = "notwebsite",
            isActive = true
        };

        // Act
        var result = dto.Validate();

        // Assert
        Assert.False(result.IsSuccess);

        if(result.Error is ValidationError err)
        {
           var errors = Assert.Single(err.Errors);

            Assert.Equal(nameof(dto.Source), errors.Key);
        }

    }


}