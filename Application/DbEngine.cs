using System.IO;
using System.Threading.Tasks.Dataflow;
using db.net.RecordService;

namespace db.net.Application;

public class DbEngine : IDbEngine {

    private readonly string _storePath;
    private readonly string _treePath; // this is where we will store our hashes for our lookup tree

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
            using var storeStream = new FileStream(
                this._storePath,
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.Read
            );
            using var treeStream = new FileStream(
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
        var recordService = new RecordService();
    }

    public Stream Get() {

    }

}