// using Application.AppServices.User.UserAppService;
using Application.AppServices.User;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web;

namespace AppServicesTests;

public class UserAppServiceTests
{
    [Fact]
    public async Task GetUserById_Success_Test()
    {
        // prepare 

        // TODO: make extension method or smth like
        var services = new ServiceCollection();
        services.AddMappings();
        services.AddHttpContextAccessor();
        services.AddUsersServices();
        services.AddDbContext<FlitchDbContext>
        (optionsBuilder =>
        {
            optionsBuilder.UseInMemoryDatabase("test");
        });
        services.AddUsersServices();
        var serviceProvider = services.BuildServiceProvider(true);
        // ENDTODO

        await using var scope = serviceProvider.CreateAsyncScope();
        var guid = Guid.NewGuid();

        var context = scope.ServiceProvider.GetRequiredService<FlitchDbContext>();
        context.Users.Add(new Domain.Entities.User()
        {
            Id = guid,
            DisplayName = "Test DisplayName",
            FullName = "Test FullName"
        });
        await context.SaveChangesAsync();
        var userAppService = scope.ServiceProvider.GetRequiredService<IUserAppService>();
        //act
        var result = await userAppService.GetUserById(guid.ToString());

        // assert
        Assert.Equal(guid.ToString(), result.Id);

    }
}