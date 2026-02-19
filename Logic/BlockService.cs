using System;
using System.Collections.Generic;
using System.IO;
using System.Buffers.Binary;

namespace db.net.Blocks;

public class BlockService : IBlockService {
    readonly Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();
    readonly Stream stream;
    readonly int blockSize;
    readonly int headerSize;
    readonly int contentSize;
    readonly int unitOfWork; 

    public int UnitOfWork => unitOfWork;
    public int BlockSize => blockSize; 
    public int ContentSize => contentSize;
    public int HeaderSize => headerSize;

    public BlockService(Stream stream, int blockSize = Block.Size, int headerSize = BlockHeader.Size) {
        if(stream == null)
            throw new ArgumentException($"Parameter {nameof(stream)} is null.");
        if(blockSize <= headerSize)
            throw new ArgumentException($"Parameter {nameof(blockSize)} cannot be less than or equal to {nameof(headerSize)}, since {nameof(headerSize)} is contained within the block.");
        if(blockSize < 128)
            throw new ArgumentException($"Parameter {nameof(blockSize)} cannot be less than 128 bytes."); 

        this.blockSize = blockSize;
        this.unitOfWork = (blockSize >= 4096) ? 4096 : 128;
        this.headerSize = headerSize;
        this.contentSize = blockSize - headerSize;
        this.stream = stream;
    }
 
    public IBlock Find(uint id) {
        // we dynamically create each block as we find it - if we have found it before, just return it from our cache
        
        if(blocks.TryGetValue(id, out Block block)) return block;

        // {blockId} is ordered, so we can find our position by scaling off {blockSize}
        int position = (int)(id * blockSize);
        // we are attempting to read a {blockSize} worth of data from stream, so if there is less than between position and end of stream, we will be hitting memory that is not ours
        if((position + blockSize) > this.stream.Length)
            return null; 

        // just grab the first "sector" of data to construct our new block
        byte[] firstSector = new byte[UnitOfWork];
        int readBytes = this.stream.Read(firstSector, position, UnitOfWork);

        block = new Block(this, id, this.stream, firstSector);
        OnBlockInitialized(block);

        return block;
    }

    public IBlock Create(byte[]? data) {
        if((this.stream.Length % blockSize) != 0)
            throw new DataMisalignedException($"Unexpected length, stream is misaligned: {this.stream.Length}");

        var id = (uint)Math.Ceiling((double)this.stream.Length / (double)this.blockSize);

        this.stream.SetLength((long)((id * blockSize) + blockSize));
        this.stream.Flush();

        Block block;

        // if data was provided, build structure and store blocks
        if(data != null) {
            List<byte[]> sections = Partition(data);
            block = new Block(this, id, this.stream, sections[0]);
        } else {
            block = new Block(this, id, this.stream, new byte[UnitOfWork]);
        }

        OnBlockInitialized(block);
        return block;
    }

    // section data into a list of (4kb - headersize) sections
    public List<byte[]> Partition(byte[] data) {
        var sections = new List<byte[]>();
        int offset = 0;
        while(offset < data.Length) {
            int size = Math.Min(ContentSize, (data.Length - offset));
            var chunk = new byte[ContentSize];
            Buffer.BlockCopy(data, offset, chunk, 0, size);
            sections.Add(chunk);
            offset += size;
        }
        return sections;
    }

    private void OnBlockInitialized(Block block) {
        blocks[block.Id] = block;
        block.Disposed += HandleBlockDisposed;
    }

    protected virtual void HandleBlockDisposed(object? sender, EventArgs e)
    {
        if (sender is not Block block) return;
        block.Disposed -= HandleBlockDisposed;
        blocks.Remove(block.Id);
    }
}
