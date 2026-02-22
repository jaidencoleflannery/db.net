using db.net.Records;
using db.net.Blocks;
using Xunit;

namespace db.net.RecordsTests; 

public class RecordServiceTests {

    private readonly FileStream _storeStream = new FileStream(
        Directory.GetCurrentDirectory(),
        FileMode.OpenOrCreate,
        FileAccess.ReadWrite,
        FileShare.Read
    );

    [Fact]
    public void RecordServiceIsASingleton() {
        RecordService.Initialize(BlockService.Initialize(_storeStream));
        var firstInstance = RecordService.Instance;
        var secondInstance = RecordService.Instance;
        Assert.Same(firstInstance, secondInstance);
    }
}
