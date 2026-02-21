using System.Collections;
using db.net.Blocks;

namespace db.net.Records;

public class Record : IRecord, IEnumerable<IBlock> {

    public uint Id { get; set; } // id should be the id of the first stored block in the chain

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
        if(cursor == 0)
            this.Id = block.headersId;
        headers
        this._blocks[cursor] = block; 
        cursor++;
    }

    public IEnumerator<IBlock> GetEnumerator() {
        foreach (var block in _blocks)
            yield return block;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
