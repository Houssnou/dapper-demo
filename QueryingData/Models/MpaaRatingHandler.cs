using Dapper;
using Npgsql;
using NpgsqlTypes;
using QueryingData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Models
{
    public class MpaaRatingTypeHandler : SqlMapper.TypeHandler<MpaaRating>
    {
        public override void SetValue(IDbDataParameter parameter, MpaaRating value)
        {
            // Store as string, matching PostgreSQL enum text
            parameter.Value = value.ToString().Replace("PG13", "PG-13").Replace("NC17", "NC-17");
            parameter.DbType = System.Data.DbType.String;
        }

        public override MpaaRating Parse(object value)
        {
            var str = value.ToString();
            return str switch
            {
                "PG-13" => MpaaRating.PG13,
                "NC-17" => MpaaRating.NC17,
                _ => Enum.Parse<MpaaRating>(str)
            };
        }
    }
}
