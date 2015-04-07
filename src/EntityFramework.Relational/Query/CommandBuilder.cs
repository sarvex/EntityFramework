// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Relational.Query
{
    public class CommandBuilder
    {
        private readonly Func<ISqlQueryGenerator> _sqlGeneratorFunc;

        public CommandBuilder([NotNull] Func<ISqlQueryGenerator> sqlGeneratorFunc)
        {
            Check.NotNull(sqlGeneratorFunc, nameof(sqlGeneratorFunc));

            _sqlGeneratorFunc = sqlGeneratorFunc;
        }

        public virtual DbCommand Build(
            [NotNull] IRelationalConnection connection,
            [NotNull] IDictionary<string, object> parameterValues)
        {
            Check.NotNull(connection, nameof(connection));

            // TODO: Cache command...

            var command = connection.DbConnection.CreateCommand();

            if (connection.Transaction != null)
            {
                command.Transaction = connection.Transaction.DbTransaction;
            }

            if (connection.CommandTimeout != null)
            {
                command.CommandTimeout = (int)connection.CommandTimeout;
            }

            var sqlGenerator = _sqlGeneratorFunc();

            command.CommandText = sqlGenerator.GenerateSql(parameterValues);

            foreach (var parameterValue in sqlGenerator.Parameters)
            {
                var parameter = command.CreateParameter();

                parameter.ParameterName = parameterValue.Key;
                parameter.Value = parameterValue.Value;

                // TODO: Parameter facets?

                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}
