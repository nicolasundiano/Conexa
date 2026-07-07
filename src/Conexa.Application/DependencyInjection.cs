using System.Reflection;
using Conexa.Application.Common.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Conexa.Application;

public static class DependencyInjection
{
    private static readonly Assembly ApplicationAssembly = Assembly.GetExecutingAssembly();

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidation();
        services.AddCqrs();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(ApplicationAssembly);

        return services;
    }

    private static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(ApplicationAssembly);
            config.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        return services;
    }
}
