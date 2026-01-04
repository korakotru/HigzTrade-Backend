using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Xunit;

public class ArchitectureTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(HigzTrade.Domain.Entities.Order).Assembly,                 // Domain
            typeof(HigzTrade.Application.Interfaces.IOrderRepository).Assembly, // Application
            typeof(HigzTrade.Infrastructure.Data.HigzTradeDbContext).Assembly, // Infrastructure
            typeof(HigzTrade.TradeApi.Controllers.Customer.CustomerController).Assembly)  // TradeApi
        .Build();

    [Fact]
    public void Controllers_Should_Not_Depend_On_Domain_Or_Infrastructure()
    {
        var rule = Classes()
            .That()
            .ResideInNamespace("HigzTrade.TradeApi.Controllers") // ปรับตาม namespace Controller ของคุณ
            .Should()
            .NotDependOnAnyTypesThat()
            .ResideInNamespace("HigzTrade.Domain")
            .AndShould()
            .NotDependOnAnyTypesThat()
            .ResideInNamespace("HigzTrade.Infrastructure")
            .Because("Presentation layer ห้าม depend บน Domain หรือ Infrastructure โดยตรง เพื่อรักษา Clean Architecture");

        rule.Check(Architecture);

        //// ถ้า fail จะบอกชัดเจนว่าผิดที่ class ไหน
        //if (!result.HasNoViolations())
        //{
        //    var failingTypes = rule.GetFailingTypes(Architecture);
        //    var message = $"พบ {failingTypes.Count()} class ใน Controllers ที่ละเมิดกฎ:\n";
        //    foreach (var failingType in failingTypes)
        //    {
        //        message += $"- {failingType.FullName}\n";
        //    }
        //    Assert.Fail(message);
        //}
    }

    // เพิ่ม rule อื่น ๆ ได้เช่นกัน
}