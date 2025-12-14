using System;
using System.Collections.Generic;
using System.IO;

namespace BlockStorage;

public class BlockStorage {
    readonly Stream stream;
    readonly int blockSize;
    readonly int headerSize;
    readonly int contentSize;
    readonly int unitOfWork;
    readonly Dictionary<uint, Block> blocks = new Dictionary<uint, Block>()

    public int DiskSectorSize {
        get {
            return unitOfWork;
        }
    }

    public int BlockSize {
        get {
            return blockSize;
        }
    }

    public int ContentSize {
        get {
            return contentSize;
        }
    }

    public int HeaderSize {
        get {
            return headerSize;
        }
    }

    public BlockStorage(Stream stream, blockSize = 40960, headerSize = 48) {
        if(stream == null) {
            return ArgumentException("BlockStorage parameter {stream} is null.");
        }
        if(blockSize <= headerSize) {
            return ArgumentException("BlockStorage parameter {blockSize} cannot be less than or equal to {headerSize}, since {headerSize} is contained within the block.");
        }
        if(blockSize < 128) {
            return ArgumentException("BlockStorage parameter {blockSize} cannot be less than 128 bytes.");
        }

        this.unitOfWork = (blockSize >= 4096) ? 4096 : 128;
        this.blockSize = blockSize;
        this.headerSize = headerSize;
        this.contentSize = blockSize - headerSize;
        this.stream = stream;
    }

    public IBlock Find(uint id) {
        // we dynamically create each block as we find it - if we have found it before, just return it from our cache
        if(blocks.TryGetValue(id, out Block block)) return block;

        // {blockId} is ordered, so we can find our position by scaling off {blockSize}
        int position = blockId * blockSize;
        // we are attempting to read a {blockSize} worth of data from stream, so if there is less than between position and end of stream, we will be hitting memory that is not ours
        if((position + blockSize) > this.stream.Length) {
            return null;
        }

        // just grab the first "sector" of data to construct our new block
        byte[] firstSector = new byte[DiskSectorSize];
        this.stream.position = position;
        int readBytes = this.stream.Read(firstSector, 0, DiskSectorSize);

        Block block = new Block(this, id, firstSector, this.stream);
        OnBlockInitialized(block);

        return block;
    }

    public IBlock Create() {
        if((this.stream.Length % blockSize) != 0) {
            throw new DataMisalignedException($"Unexpected length, stream is misaligned: {this.stream.Length}");
        }

        var id = (uint)Math.Ceiling((double)this.stream.Length / (double)this.blockSize);

        this.stream.SetLength((long)((blockId * blockSize) blockSize));
        this.stream.Flush()

        var block = new Block(this, id, new byte[DiskSectorSize], this.stream);

        OnBlockInitialized(block);
        return block;
    }

    private OnBlockInitialized(Block block) {
        blocks[id] = block;

        block.Disposed += HandleBlockDisposed;
    }

    protected virtual void HandleBlockDisposed (object sender, EventArgs e)
		{
			var block = (Block)sender;
			block.Disposed -= HandleBlockDisposed;

			blocks.Remove (block.Id);
		}
}
