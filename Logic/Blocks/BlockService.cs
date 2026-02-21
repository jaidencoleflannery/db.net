using System;
using System.Collections.Generic;
using System.IO;
using System.Buffers.Binary;
using db.net.Data;
using db.net.StorageConstants;

namespace db.net.Blocks;

public class BlockService : IBlockService {
    readonly Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();
    readonly Stream stream;

    readonly int blockSize = Storage.BlockSize;
    readonly int headerSize = Storage.HeaderSize;
    readonly int contentSize = Storage.BlockSize - Storage.HeaderSize;
    readonly int unitOfWork = Storage.UnitOfWork;

    public int UnitOfWork => unitOfWork;
    public int BlockSize => blockSize; 
    public int ContentSize => contentSize;
    public int HeaderSize => headerSize;

    public BlockService(Stream stream) {
        if(stream == null)
            throw new ArgumentException($"Parameter {nameof(stream)} is null.");
        if(blockSize <= headerSize)
            throw new ArgumentException($"Parameter {nameof(blockSize)} cannot be less than or equal to {nameof(headerSize)}, since {nameof(headerSize)} is contained within the block.");
        if(blockSize < 128)
            throw new ArgumentException($"Parameter {nameof(blockSize)} cannot be less than 128 bytes."); 

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

        // add our block in memory and cache it for future use
        block = new Block(this, id, this.stream, firstSector);
        OnBlockInitialized(block);
        return block;
    }

    public IBlock Create(byte[]? data) {
        // if the length of stream isnt an exact multiple of blockSize, our position is funky
        if((this.stream.Length % blockSize) != 0)
            throw new DataMisalignedException($"Unexpected length, stream is misaligned: {this.stream.Length}");

        // this service is just spinning up the 4kb blocks of data - recordservice handles the actual data handling and partitioning
        if(data.Length != contentSize)
            throw new ArgumentException($"{nameof(data)}'s size is not the same as the expected content size.");

        // our blocks are indexed by position, so we can find the block by iterating through n blocks
        var id = (uint)Math.Ceiling((double)this.stream.Length / (double)this.blockSize);

        // set our file to be 1 block longer than its current total length to make room
        this.stream.SetLength((long)((id * blockSize) + blockSize));
        // save everything in the stream and clear it
        this.stream.Flush();

        // setup block header first
        var header = new BlockHeader(id, 0, (uint)data.Length);
        
        // create our in memory block and cache it
        Block block = new Block(this, id, this.stream, data, header);
        OnBlockInitialized(block);
        return block;
    }

    private void OnBlockInitialized(Block block) {
        // cache the block
        blocks[block.Id] = block;
        // subscribe to the dispose event
        block.Disposed += HandleBlockDisposed;
    }

    protected virtual void HandleBlockDisposed(object? sender, EventArgs e)
    {
        if (sender is not Block block) return;
        block.Disposed -= HandleBlockDisposed;
        blocks.Remove(block.Id);
    }
}
