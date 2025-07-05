using ConsoleApp.BulkOperations;
using ConsoleApp.NonQueryOperations;
using ConsoleApp.QueryingData;
using ConsoleApp.Relationships;
using Microsoft.Extensions.DependencyInjection;
using QueryingData.QueryingData;


namespace ConsoleApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddTransient<SingleValueQueryManager>();
            services.AddTransient<SingleRowQueryManager>();
            services.AddTransient<MultipleQueryRowManager>();
            services.AddTransient<DataInsertManager>();
            services.AddTransient<DataDeleteManager>();
            services.AddTransient<DataUpdateManager>();
            services.AddTransient<StoreProcedureManager>();
            services.AddTransient<ReaderManager>();
            services.AddTransient<RelationalQueryManager>();
            services.AddTransient<BulkInsertManager>();
            services.AddTransient<BulkUpdateManager>();
            services.AddTransient<BulkDeleteManager>();
            services.AddTransient<BulkUpsertManager>();

            return services;
        }
    }
}
