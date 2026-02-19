using db.net.Blocks;

namespace db.net.Records;

public class RecordService : IRecordService {

    private BlockService _blockService;
    public RecordService(BlockService blockService) {
        this._blockService = blockService;
    }

    // each hashed record is the head link in the chain of that item.
    public HashSet<Record> records = new();

    public uint Id { get; private set; }

    public byte[] Find(uint id) {
        return new byte[] {};
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

    public uint Create(byte[] data) {
        var record = new Record();
        return 1;
    }

}
