namespace db.net.Application;

public interface IDbEngine {

    Stream Get(uint id);

    void Post(Stream value);

    void Put(Stream value);

    void Delete(uint id);
}