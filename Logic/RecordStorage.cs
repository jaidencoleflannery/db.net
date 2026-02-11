namespace db.net.RecordStorage;

public class RecordStorage : IRecordStorage {

    public uint Id { get; private set; }

    public byte[] Find() {
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

    // attempt to pull record, if it doesnt exist, create it?
    public uint Create(Func<uint, byte[]> generator) {
        return generator(uint);
    }

}
