using db.net.BlockStorage;

namespace db.net.RecordStorage;

class Record : IRecord {
    
    public IBlock block { get; private set; }
    public IBlock? next { get; private set; }
    public IBlock? prev { get; private set; }

    public Record(Block node) {
        if(node == null) 
            throw new ArgumentException("Initialization block cannot be null.");
        this.block = node;    
    }

    public void Append(IBlock node) {
        if(this.next != null) 
            throw new ArgumentException($"{nameof(node)}.next is already linked. Use .Break() to clear forwards.");
        else
            this.next = node;
    }

    public void Break() {
        this.next = null;
    }

}
