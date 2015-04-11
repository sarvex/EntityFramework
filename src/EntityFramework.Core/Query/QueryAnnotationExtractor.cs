// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Annotations;
using Microsoft.Data.Entity.Query.ResultOperators;
using Microsoft.Data.Entity.Utilities;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;

namespace Microsoft.Data.Entity.Query
{
    public class QueryAnnotationExtractor
    {
        public virtual ICollection<QueryAnnotation> ExtractQueryAnnotations([NotNull] QueryModel queryModel)
        {
            Check.NotNull(queryModel, nameof(queryModel));

            var queryAnnotations = new List<QueryAnnotation>();

            ExtractQueryAnnotations(queryModel, queryAnnotations);

            return queryAnnotations;
        }

        private static void ExtractQueryAnnotations(
            QueryModel queryModel, ICollection<QueryAnnotation> queryAnnotations)
        {
            foreach (var resultOperator
                in queryModel.ResultOperators
                    .OfType<QueryAnnotationResultOperator>()
                    .ToList())
            {
                resultOperator.Annotation.QueryModel = queryModel;
                resultOperator.Annotation.QuerySource = queryModel.MainFromClause;
                queryAnnotations.Add(resultOperator.Annotation);
                queryModel.ResultOperators.Remove(resultOperator);
            }

            foreach (var resultOperator
                in queryModel.ResultOperators
                    .OfType<IncludeResultOperator>()
                    .ToList())
            {
                queryAnnotations.Add(
                    new IncludeQueryAnnotation(resultOperator)
                    {
                        QueryModel = queryModel,
                        QuerySource = queryModel.MainFromClause
                    });

                queryModel.ResultOperators.Remove(resultOperator);
            }

            queryModel.MainFromClause
                .TransformExpressions(e =>
                    ExtractQueryAnnotations(e, queryAnnotations));

            foreach (var bodyClause in queryModel.BodyClauses)
            {
                bodyClause
                    .TransformExpressions(e =>
                        ExtractQueryAnnotations(e, queryAnnotations));
            }
        }

        private static Expression ExtractQueryAnnotations(
            Expression expression, ICollection<QueryAnnotation> queryAnnotations)
        {
            var subQueryExpression = expression as SubQueryExpression;

            if (subQueryExpression != null)
            {
                ExtractQueryAnnotations(subQueryExpression.QueryModel, queryAnnotations);
            }

            return expression;
        }
    }
}
