using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace KeytiaWeb.UserInterface.DashboardLT
{
    public static class TransponerDataTable
    {
        private static void CreatingColumns(DataTable dtOld, DataTable dtNew)
        {
            DataColumn[] arr = new DataColumn[] { new DataColumn("Head") }
               .ToList()
                .Union<DataColumn>(dtOld.Rows.Cast<DataRow>()
                    .ToList()
                    .Select(row => new DataColumn(Convert.ToString(row[0])))
                    ).ToArray();

            dtNew.Columns.AddRange(arr);
        }

        private static void CreatingRows(DataTable dtOld, DataTable dtNew)
        {
            dtOld.Columns.Cast<DataColumn>()
                .ToList()
                .ForEach(a =>
                    dtNew.Rows.Add(new string[] { a.ColumnName }.Union(
                        dtOld.Rows.Cast<DataRow>()
                        .ToList()
                        .Select(row => row[a.ColumnName])).ToArray()));

        }

        public static DataTable Transpond(DataTable dtOld)
        {
            DataTable dtNew = new DataTable(dtOld.TableName);

            CreatingColumns(dtOld, dtNew);

            CreatingRows(dtOld, dtNew);

            return dtNew;
        }
    }
}
