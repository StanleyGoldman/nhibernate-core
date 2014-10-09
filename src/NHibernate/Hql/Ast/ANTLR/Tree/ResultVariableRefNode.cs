using System;
using System.Text;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.SqlCommand;


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

        public override SqlString RenderText(ISessionFactoryImplementor sessionFactory)
        {
            int scalarColumnIndex = _selectExpression.ScalarColumn;

            string sql = sessionFactory.Dialect.RequiresCastingOfParametersInSelectClause ?
                GetColumnPositionsString(scalarColumnIndex) :
                GetColumnNamesString(scalarColumnIndex);

            return new SqlString(sql);

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
