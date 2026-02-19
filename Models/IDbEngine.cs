namespace db.net.Application;

public interface IDbEngine {

    byte[] Get(uint id);

    void Post(byte[] data);

    void Update(uint id, byte[] data);

    void Delete(uint id);
}