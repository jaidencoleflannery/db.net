namespace db.net.RecordService;

public class RecordService : IRecordService {

    new HashSet<BlockService>

    public uint Id { get; private set; }

    public byte[] Find(uint id) {
        return new byte[] {};
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

    // generator should be a function that takes the data and turns it into a byte[] for storage
    public uint Create(Func<uint, byte[]> generator) {
        return 1;
    }

}
