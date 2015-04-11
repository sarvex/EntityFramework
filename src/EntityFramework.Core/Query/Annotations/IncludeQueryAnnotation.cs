// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Remotion.Linq.Clauses;

namespace Microsoft.Data.Entity.Query.Annotations
{
    public class IncludeQueryAnnotation : QueryAnnotation
    {
        public IncludeQueryAnnotation([NotNull] ResultOperatorBase resultOperator)
           : base(resultOperator)
        {
        }
    }
}
