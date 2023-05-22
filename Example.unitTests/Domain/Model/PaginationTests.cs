using Example.Domain.Entitys;
using Example.Domain.Exceptions;
using Example.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Example.unitTests.Domain.Model
{
    public class PaginationTests
    {

        [Fact]
        public void ShouldThrowException()
        {
            var exception = Record.Exception(() => new Pagination<Category>(
                items: Array.Empty<Category>(),
                totalCount: 10,
                page: 0
            ));

            Assert.NotNull(exception);
            Assert.IsType<BadRequestException>(exception);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void IntanceByAtributes_ShouldSuccess(int page)
        {
            var instance = new Pagination<Category>(
                items: Array.Empty<Category>(),
                totalCount: 10,
                page: page,
                limit: 2,
                applyPageAndLimitToItems: true
            );

            Assert.NotNull(instance);
            Assert.IsType<Pagination<Category>>(instance);
        }
    }
}
 