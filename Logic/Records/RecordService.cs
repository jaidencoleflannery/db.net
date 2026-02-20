using db.net.Blocks;
using db.net.Data;

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

    // TODO need to loop through our partitioned data and for each section, create the block, and then store the header in that block with the returned ID
    public uint Create(byte[] data, int contentSize) {
        List<byte[]> sections = DataService.Partition(data, contentSize);
        foreach(var section in sections) {
            var header = new BlockHeader();
            records.Add(new Record(_blockService.Create(section, header)));
        }
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

}
