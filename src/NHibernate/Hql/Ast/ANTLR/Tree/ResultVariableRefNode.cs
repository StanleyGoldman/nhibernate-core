using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
    public class ResultVariableRefNode : HqlSqlWalkerNode
    {
        private ISelectExpression _selectExpression;
        
        public ResultVariableRefNode(IToken token)
            : base(token)
        {
        }

        public void SetSelectExpression(ISelectExpression selectExpression)
        {
            if (selectExpression == null || selectExpression.Alias == null)
            {
                throw new SemanticException("A ResultVariableRefNode must refer to a non-null alias.");
            }
            _selectExpression = selectExpression;
        }

        public String GetRenderText(ISessionFactoryImplementor sessionFactory)
        {
            int scalarColumnIndex = _selectExpression.ScalarColumn;
            if (scalarColumnIndex < 0)
            {
                throw new QueryException("selectExpression.getScalarColumnIndex() must be >= 0; actual = " + scalarColumnIndex);
            }
            
            return sessionFactory.Dialect.RequiresCastingOfParametersInSelectClause ?
                GetColumnPositionsString(scalarColumnIndex) :
                GetColumnNamesString(scalarColumnIndex);

        }

        private String GetColumnPositionsString(int scalarColumnIndex)
        {
            int startPosition = Walker.SelectClause.GetColumnNamesStartPosition(scalarColumnIndex);
            StringBuilder buf = new StringBuilder();
            int nColumns = Walker.SelectClause.ColumnNames[scalarColumnIndex].Length;
            for (int i = startPosition; i < startPosition + nColumns; i++)
            {
                if (i > startPosition)
                {
                    buf.Append(", ");
                }
                buf.Append(i);
            }
            return buf.ToString();
        }

        private String GetColumnNamesString(int scalarColumnIndex)
        {
            return StringHelper.Join(", ", Walker.SelectClause.ColumnNames[scalarColumnIndex]);
        }
    }
}
