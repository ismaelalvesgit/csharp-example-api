using Example.Domain.Models;
using Example.Domain.Enums;
using Xunit;

namespace Example.unitTests.Domain.Model
{
    public class WhereOptionsTests
    {

        [Fact]
        public void IntanceByAtributes_ShouldSuccess()
        {
            var instance = new WhereOptions(
                filterBy: "Name",
                value: "Ismael",
                operation: WhereOperator.Equal
            );

            Assert.NotNull(instance);
            Assert.IsType<WhereOptions>(instance);
        }
    }
}
