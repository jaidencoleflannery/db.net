using db.net.Blocks;

namespace db.net.Records;

public class Record : IRecord, IEnumerable<Record> {
    
    private IBlock[] _blocks { get; set; }
    private int cursor { get; set; }

    public Record(int numBlocks) {
        if(numBlocks < 1)
            throw new ArgumentException("Record size must be at least 1 block.");
        this._blocks = new IBlock[numBlocks];
        this.cursor = 0;
    }

    public void Append(IBlock block) { 
        if(cursor == _blocks.Length)
            throw new IndexOutOfRangeException("Record out of space."); 
        if(block == null)
            throw new ArgumentNullException("Block cannot be null.");
        this._blocks[cursor] = block; 
        cursor++;
    }

    public IEnumerator<IBlock> GetEnumerator() {
        foreach (var block in _blocks)
            yield return block;
    }
}
