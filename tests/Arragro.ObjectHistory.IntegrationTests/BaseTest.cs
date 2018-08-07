using System;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
namespace Arragro.ObjectHistory.IntegrationTests
{
    class BaseTest
    {
        protected readonly Guid UserId = Guid.NewGuid();

        protected BaseTest()
        {
            
        }
    }
}
