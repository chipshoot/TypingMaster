using Amazon.Lambda.AspNetCoreServer;

namespace TypingMaster.Server;

/// <summary>
/// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the
/// actual Lambda function entry point. The Lambda handler field should be set to
///
/// YourProject.API::YourProject.API.LambdaEntryPoint::FunctionHandlerAsync
/// </summary>
public class LambdaEntryPoint : APIGatewayProxyFunction
{
    /// <summary>
    /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
    /// needs to be configured in this method using the UseStartup<>() method.
    /// </summary>
    /// <param name="builder">The IWebHostBuilder to configure.</param>
    protected override void Init(IWebHostBuilder builder)
    {
        // Point to your application's Startup class or configuration method
        // For .NET 6+ minimal APIs, you might configure services/pipeline directly here
        // or point to your Program.cs setup logic if refactored appropriately.
        // If using Startup.cs:
        // builder.UseStartup<Startup>();

        // If using .NET 6+ minimal APIs (Program.cs), ensure Program.cs can be
        // referenced or its setup logic invoked correctly from here. Often, Program.cs
        // is modified slightly to accommodate this.
        builder.UseStartup<LambdaStartup>(); // Assuming you still use Startup.cs pattern
        // Adjust if using minimal APIs in Program.cs directly
    }

    /// <summary>
    /// Use this override to customize the services registered with the IHostBuilder.
    /// It is recommended not to call ConfigureWebHostDefaults to avoid overwriting settings configured by the base class.
    /// </summary>
    /// <param name="builder">The IHostBuilder to configure.</param>
    protected override void Init(IHostBuilder builder)
    {
        // Add any additional host configuration here if needed
    }
}