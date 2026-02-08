using db.net.Block;

namespace db.net.RecordStorage;

class Record : IRecord {
    
    public Block block { get; private set; }
    public Block next { get; private set; }
    public Block prev { get; private set; }

    public Record(Block node) {
        if(Block == null) 
            throw new ArgumentException("Block cannot be null.");
        this.block = node;    
    }

    public void AppendBlock(Block node) {
        if(Block.next != null) 
            throw new ArgumentException($"nameof({node}).next is already linked. Use .Break() to clear forwards.");
        else
            this.block.next = node;
    }

    public void PushBlock(Block node) {
        if(Block.prev != null) 
            throw new ArgumentException($"nameof({node}).next is already linked. Use .Relink() to free the current block from it's parents.");
        else
            this.block.prev = node;
    }

}
