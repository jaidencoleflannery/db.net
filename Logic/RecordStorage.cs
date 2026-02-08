namespace db.net.RecordStorage;

public class RecordStorage : IRecordStorage {

    public uint Id { get; private set; }

    public byte[] Find() {
    }

    public void Update(uint id, byte[] data) {
    }

    public void Delete(uint id) {
    }

    uint Create(Func<uint, byte[]> generate) {
    }

}
