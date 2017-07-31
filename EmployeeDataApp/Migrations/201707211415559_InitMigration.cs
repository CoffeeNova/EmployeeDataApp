using System.Text;

namespace EmployeeDataApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeModels",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    FirstName = c.String(maxLength: 20),
                    LastName = c.String(maxLength: 20),
                    Age = c.Int(nullable: false),
                    Gender = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.FirstName, t.LastName, t.Age }, unique: true, name: "UniqueEmployeeInd");

            CreateTable(
                "dbo.ProfessionModels",
                c => new
                {
                    ProfessionName = c.String(nullable: false, maxLength: 20),
                    EmployeeModelId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.ProfessionName, t.EmployeeModelId })
                .ForeignKey("dbo.EmployeeModels", t => t.EmployeeModelId, cascadeDelete: true)
                .Index(t => t.ProfessionName)
                .Index(t => t.EmployeeModelId);

            //CreateStoredProcedure(
            //    "FindEmployeeProcedure",
            //    p => new
            //    {
            //        firstName = p.String(20),
            //        lastName = p.String(20),
            //        age = p.Int(),
            //        gender = p.String(20)
            //    },
            //    "CREATE TABLE #TempTable(Id int) " +
            //    "INSERT INTO #TempTable(Id) " +
            //    "SELECT Id " +
            //    "FROM EmployeeModels " +
            //    "LEFT JOIN ProfessionModels ON EmployeeModels.Id=ProfessionModels.EmployeeModelId " +
            //    "WHERE   (FirstName = @firstName OR @firstName IS NULL) " +
            //    "AND (LastName = @lastName OR @lastName IS NULL) " +
            //    "AND (Gender = @gender OR @gender IS NULL) " +
            //    "AND (Age = @age OR @age = 0) " +
            //    "AND (ProfessionName IN  (SELECT ProfessionName FROM @professions) OR (SELECT ProfessionName FROM @professions) IS NULL) " +
            //    "SELECT * " +
            //    "FROM EmployeeModels " +
            //    "LEFT JOIN ProfessionModels ON EmployeeModels.Id=ProfessionModels.EmployeeModelId " +
            //    "WHERE Id IN (SELECT Id FROM #TempTable) " +
            //    "DROP TABLE #TempTable"
            //);
            var type = new StringBuilder();
            type.AppendLine("CREATE TYPE dbo.ProfessionList AS TABLE");
            type.AppendLine("(");
            type.AppendLine("ProfessionName nvarchar(20)");
            type.AppendLine(")");
            Sql(type.ToString());
            #region big tored procedure
            var proc = new StringBuilder();
            proc.AppendLine("CREATE PROCEDURE dbo.FindEmployeeProcedure");
            proc.AppendLine("@firstName nvarchar(20) = NULL,");
            proc.AppendLine("@lastName nvarchar(20) = NULL,");
            proc.AppendLine("@age int = 0,");
            proc.AppendLine("@gender nvarchar(20) = NULL,");
            proc.AppendLine("@professions AS dbo.ProfessionList READONLY");
            proc.AppendLine();
            proc.AppendLine("AS");
            proc.AppendLine();
            proc.AppendLine("CREATE TABLE #TempEmployee(Id int)");
            proc.AppendLine("CREATE TABLE #TempProfessions(Id int, ProfessionsCount int)");
            proc.AppendLine("DECLARE @professionsCount int");
            proc.AppendLine();
            proc.AppendLine("SET @professionsCount = (SELECT COUNT(*) FROM @professions)");
            proc.AppendLine();
            proc.AppendLine("INSERT INTO #TempProfessions");
            proc.AppendLine("SELECT Id, Count(*) AS ProfessionsCount");
            proc.AppendLine("FROM EmployeeModels");
            proc.AppendLine("LEFT JOIN ProfessionModels ON EmployeeModels.Id=ProfessionModels.EmployeeModelId");
            proc.AppendLine("GROUP BY Id");
            proc.AppendLine("HAVING COUNT(*) >= @professionsCount");
            proc.AppendLine();
            proc.AppendLine("INSERT INTO #TempEmployee(Id)");
            proc.AppendLine("SELECT Id");
            proc.AppendLine("FROM EmployeeModels");
            proc.AppendLine("LEFT JOIN ProfessionModels ON EmployeeModels.Id=ProfessionModels.EmployeeModelId");
            proc.AppendLine("WHERE   (FirstName LIKE @firstName OR @firstName IS NULL)");
            proc.AppendLine("AND (LastName LIKE @lastName OR @lastName IS NULL)");
            proc.AppendLine("AND (Gender LIKE @gender OR @gender IS NULL)");
            proc.AppendLine("AND (Age LIKE @age OR @age = 0)");
            proc.AppendLine("AND ((ProfessionName IN (SELECT ProfessionName FROM @professions))  OR (SELECT ProfessionName FROM @professions) IS NULL)");
            proc.AppendLine("AND (Id IN (Select Id FROM #TempProfessions))");
            proc.AppendLine();
            proc.AppendLine("SELECT *");
            proc.AppendLine("FROM EmployeeModels ");
            proc.AppendLine("LEFT JOIN ProfessionModels ON EmployeeModels.Id=ProfessionModels.EmployeeModelId");
            proc.AppendLine("WHERE Id IN (SELECT Id FROM #TempEmployee)");
            proc.AppendLine();
            proc.AppendLine("DROP TABLE #TempEmployee");
            proc.Append("DROP TABLE #TempProfessions");
            #endregion
            Sql(proc.ToString());
        }

        public override void Down()
        {
            DropForeignKey("dbo.ProfessionModels", "EmployeeModelId", "dbo.EmployeeModels");
            DropIndex("dbo.ProfessionModels", new[] { "EmployeeModelId" });
            DropIndex("dbo.ProfessionModels", new[] { "ProfessionName" });
            DropIndex("dbo.EmployeeModels", "UniqueEmployeeInd");
            DropTable("dbo.ProfessionModels");
            DropTable("dbo.EmployeeModels");

            DropStoredProcedure("FindEmployeeProcedure");
        }
    }
}
