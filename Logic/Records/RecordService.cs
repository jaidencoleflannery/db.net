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

    public uint Create(byte[] data, int contentSize) {
        // partition our data into chunks that will fit inside each block
        List<byte[]> sections = DataService.Partition(data, contentSize);
        
        // create the record
        var record = new Record(numBlocks: sections.Count);
 
        // loop through sections and append to the array of blocks in record
        for(int cursor = 1; cursor < sections.Count; cursor++) {  // make sure to add record to cache
            record.Append(_blockService.Create(sections[cursor]));
        }

        // cache record for later user
        records.Add(record);
        return record.Id;
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

}
