// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Relational.Query.Sql
{
    public class RawSqlQueryGenerator : ISqlQueryGenerator
    {
        private readonly Dictionary<string, object> _parameters;
        private readonly string _rawSql;
        private readonly object[] _rawParameters;

        public RawSqlQueryGenerator([NotNull] string rawSql, [NotNull] object[] rawParameters)
        {
            Check.NotNull(rawSql, nameof(rawSql));
            Check.NotNull(rawParameters, nameof(rawParameters));

            _rawSql = rawSql;
            _rawParameters = rawParameters;

            _parameters = new Dictionary<string, object>();
        }

        public virtual IDictionary<string, object> Parameters => _parameters;

        protected virtual string ParameterPrefix => "@";

        public virtual string GenerateSql([NotNull]IDictionary<string, object> parameterValues)
        {
            Check.NotNull(parameterValues, nameof(parameterValues));

            var substitutions = new string[_rawParameters.Length];

            for (var index = 0; index < _rawParameters.Length; index++)
            {
                var parameterName = "p" + index;

                _parameters.Add(parameterName, _rawParameters[index]);
                substitutions[index] = ParameterPrefix + parameterName;
            }

            return string.Format(_rawSql, substitutions);
        }
    }
}
