﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevIO.Api.Extensions
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        #region Private Fields

        readonly string _connection;

        #endregion Private Fields

        #region Public Constructors

        public SqlServerHealthCheck(string connection)
        {
            _connection = connection;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    await connection.OpenAsync(cancellationToken);

                    SqlCommand _command = connection.CreateCommand();
                    _command.CommandText = "select count(id) from produtos";

                    return Convert.ToInt32(await _command.ExecuteScalarAsync(cancellationToken)) > 0 ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        #endregion Public Methods
    }
}