using ConsoleApp.NonQueryOperations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueryingData.QueryingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    //public static class HostExtensions
    //{
    //    public static void UseAppServices(this IHost app,
    //        out SingleValueQueryManager singleValueQueryManager,
    //        out SingleRowQueryManager singleRowQueryManager,
    //        out MultipleQueryRowManager multipleRowsQueryManager,
    //        out DataInsertManager insertDataManager,
    //        out DataDeleteManager deleteDataManager,
    //        out DataUpdateManager updateDataManager)
    //    {
    //        singleValueQueryManager = app.Services.GetRequiredService<SingleValueQueryManager>();
    //        singleRowQueryManager = app.Services.GetRequiredService<SingleRowQueryManager>();
    //        multipleRowsQueryManager = app.Services.GetRequiredService<MultipleQueryRowManager>();
    //        insertDataManager = app.Services.GetRequiredService<DataInsertManager>();
    //        deleteDataManager = app.Services.GetRequiredService<DataDeleteManager>();
    //        updateDataManager = app.Services.GetRequiredService<DataUpdateManager>();
    //    }
    //}
}
