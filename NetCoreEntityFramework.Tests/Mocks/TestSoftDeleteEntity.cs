using Nodes.NetCore.EntityFramework.Models;

namespace Nodes.NetCore.EntityFramework.Tests.Mocks
{
    public class TestSoftDeleteEntity : EntitySoftDeleteBase
    {
        public string Property { get; set; }
    }
}
