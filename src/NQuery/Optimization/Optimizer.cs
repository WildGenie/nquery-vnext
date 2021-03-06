﻿using System.Collections.Generic;

using NQuery.Binding;

namespace NQuery.Optimization
{
    internal static class Optimizer
    {
        public static BoundQuery Optimize(BoundQuery query)
        {
            var optmizedRelation = Optimize(query.Relation);
            return new BoundQuery(optmizedRelation, query.OutputColumns);
        }

        private static BoundRelation Optimize(BoundRelation relation)
        {
            foreach (var step in GetOptimizationSteps())
                relation = step.RewriteRelation(relation);

            return relation;
        }

        public static IEnumerable<BoundTreeRewriter> GetOptimizationSteps()
        {
            // Instantiate CTEs
            yield return new CommonTableExpressionInstantiator();

            // TODO: This shouldn't be necessary
            yield return new DerivedTableRemover();

            // Expand full outer joins
            yield return new FullOuterJoinExpander();

            // Remove unused values
            yield return new UnusedValueSlotRemover();

            // Expand subqueries
            yield return new SubqueryExpander();

            // TODO: semi join simplification
            // TODO: decorrelation

            yield return new OuterJoinRemover();

            // selection pushing
            yield return new SelectionPusher();

            // join linearization
            yield return new JoinLinearizer();

            // TODO: outer join reordering
            // TODO: join order optimization

            yield return new JoinOrderer();

            // after the join ordering, we need to push selections again
            yield return new SelectionPusher();

            // TODO: at most one row reordering
            // TODO: push computations

            // physical op choosing
            yield return new HashMatchPhysicalOperatorChooser();
            yield return new AggregationPhysicalOperatorChooser();
            yield return new ConcatenationPhysicalOperatorChooser();
            yield return new ExceptIntersectPhysicalOperatorChooser();

            // TODO: null scan optimization
        }
    }
}