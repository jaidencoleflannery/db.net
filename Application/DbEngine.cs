using System.IO;
using System.Threading.Tasks.Dataflow;
using db.net.Blocks;
using db.net.Records;

namespace db.net.Application;

public class DbEngine : IDbEngine, IDisposable {

    private readonly RecordService _recordService;
    private readonly string _storePath;
    private readonly FileStream _storeStream;
    private readonly string _treePath; // this is where we will store our hashes for our lookup tree
    private readonly FileStream _treeStream;

    public DbEngine(string storePath, string treePath = "") {
        if(storePath == null || string.IsNullOrEmpty(storePath.Trim()))
            throw new ArgumentException("Invalid path for data storage.");
        
        this._storePath = Path.GetFullPath(storePath);
        if(string.IsNullOrWhiteSpace(treePath))
            this._treePath = this._storePath; 
        else    
            this._treePath = Path.GetFullPath(treePath);

        this._storePath = Path.Combine(_storePath, "db.data");
        this._treePath = Path.Combine(_treePath, "tree.data");

        // create file(s)
        try{
            // make sure to dispose these on shutdown - they last for the lifetime of our database
            _storeStream = new FileStream(
                this._storePath,
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.Read
            );
            _treeStream = new FileStream(
                this._treePath,
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.Read
            );
        } catch(Exception e) {
            Console.WriteLine(e);
            throw;
        }

        // spin up singleton for managing records
        _recordService = new RecordService(new BlockService(_storeStream));
    }

    public byte[] Get(uint id) =>
        _recordService.Find(id);

    public void Post(byte[] data) {
        // TODO
    }

    public void Put(byte[] data) {
        // TODO
    }

    public void Delete(uint id) {
        // TODO
    }

    public void Dispose() {
        _storeStream?.Dispose();
        _treeStream?.Dispose();
    }

}