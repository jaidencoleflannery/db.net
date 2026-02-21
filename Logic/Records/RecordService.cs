using db.net.Blocks;
using db.net.Data;

namespace db.net.Records;

public class RecordService : IRecordService {

    public static RecordService Instance => _instance ?? throw new InvalidOperationException("RecordService is not initialized.");
    private static Boolean _initialized = false;

    private static RecordService? _instance;

    public static RecordService Initialize(BlockService blockService) {
        if(blockService == null)
            throw new ArgumentNullException("blockService cannot be null.");
        if(_instance != null)
            throw new InvalidOperationException("RecordService has already been initialized.");
        _initialized = true;
        _instance = new RecordService(blockService);
        return _instance;
    }

    private RecordService(BlockService blockService) {
        if(blockService == null)
            throw new ArgumentNullException("BlockService cannot be null.");
        this._blockService = blockService;
    }

    private BlockService _blockService;

    // each hashed record is the head link in the chain of that item.
    public HashSet<Record> records = new();

    public uint Id { get; private set; }

    public byte[] Find(uint id) {
        return new byte[] {};
    }

    // create a new record with blocks made from data
    public uint Create(byte[] data) {
        // partition our data into chunks that will fit inside each block
        List<byte[]> sections = DataService.Partition(data);
        
        var record = new Record(numBlocks: sections.Count);
 
        // loop through sections and append to the array of blocks in record
        for(int cursor = 0; cursor < sections.Count; cursor++) {
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
