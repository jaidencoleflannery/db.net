namespace db.net.Application;

public interface IDbEngine {

    Stream Get(uint id);

    void Post(byte[] data);

    void Put(byte[] data);

    void Delete(uint id);
}