using db.net.Blocks;
using db.net.Data;

namespace db.net.Records;

public class RecordService : IRecordService {

    private BlockService _blockService;

    public RecordService(BlockService blockService) {
        if(blockService == null)
            throw new ArgumentNullException("BlockService cannot be null.");
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

        // partition our data into chunks that will fit inside each block
        List<byte[]> sections = DataService.Partition(data, contentSize);

        // create record to fit our blocks 
        var current = new Record(_blockService.Create(sections[0]));

        // loop through remaining sections and append to the linked list (record) of blocks
        for(int cursor= 1; cursor < sections.Count; cursor++) {  // make sure to add record to cache
            var record = new Record(_blockService.Create(sections[cursor]));
            current.Append(record);
            records.Add(current);
            current = record;
        }
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

}
