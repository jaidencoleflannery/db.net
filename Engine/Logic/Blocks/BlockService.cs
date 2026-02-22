using System.Buffers.Binary;
using db.net.StorageConstants;

namespace db.net.Blocks;

public sealed class BlockService : IBlockService {

    public static BlockService Instance => _instance ?? throw new InvalidOperationException("BlockService is not initialized.");
    private static Boolean _initialized = false;

    private static BlockService? _instance;

    readonly Dictionary<uint, IBlock> blocks = new Dictionary<uint, IBlock>();
    readonly Stream stream;

    readonly int blockSize = Storage.BlockSize;
    readonly int headerSize = Storage.HeaderSize;
    readonly int contentSize = Storage.BlockSize - Storage.HeaderSize;
    readonly int unitOfWork = Storage.UnitOfWork;

    public int UnitOfWork => unitOfWork;
    public int BlockSize => blockSize; 
    public int ContentSize => contentSize;
    public int HeaderSize => headerSize;

    public static BlockService Initialize(Stream stream) {
        if(stream == null)
            throw new ArgumentNullException("Stream cannot be null.");
        if(_instance != null)
            throw new InvalidOperationException("BlockService has already been initialized.");
        _initialized = true;
        _instance = new BlockService(stream);
        return Instance;
    }

    private BlockService(Stream stream) {
        if(stream == null)
            throw new ArgumentException($"Parameter {nameof(stream)} is null.");
        if(Storage.BlockSize <= Storage.HeaderSize)
            throw new ArgumentException($"Parameter {nameof(Storage.BlockSize)} cannot be less than or equal to {nameof(Storage.HeaderSize)}, since {nameof(Storage.HeaderSize)} is contained within the block.");
        if(Storage.BlockSize < 128)
            throw new ArgumentException($"Parameter {nameof(Storage.BlockSize)} cannot be less than 128 bytes."); 

        this.stream = stream;
    }
 
    public IBlock Find(uint id) {
        // we dynamically create each block as we find it - if we have found it before, just return it from our cache
        if(blocks.TryGetValue(id, out IBlock block)) return block;

        // {blockId} is ordered, so we can find our position by scaling off {blockSize}
        int position = (int)(id * Storage.BlockSize);
        // we are attempting to read a {blockSize} worth of data from stream, so if there is less than between position and end of stream, we will be hitting memory that is not ours
        if((position + Storage.BlockSize) > this.stream.Length)
            throw new InvalidOperationException("Hit end of stream - provided Id is most likely invalid."); 
        SeekStartOfBlock(id);

        // grab the block
        byte[] buffer = new byte[Storage.BlockSize];
        this.stream.Read(buffer, 0, Storage.BlockSize);

        // add our block in memory and cache it for future use
        // get header values
        uint id = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(0, 4));
        uint offset = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(4, 4));
        uint nextBlockId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(8, 4));
        uint usedLength = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(12, 4));

        var header = new BlockHeader(id, offset, nextBlockId, usedLength);

        // section out our content from the header
        Span<byte> content = buffer.AsSpan(12);
        block = Block.Create(this, this.stream, content, header);
        OnBlockInitialized(block);
        return block;
    }

    public IBlock Create(byte[]? data) {
        // if the length of stream isnt an exact multiple of blockSize, our position is funky
        if((this.stream.Length % Storage.BlockSize) != 0)
            throw new DataMisalignedException($"Unexpected length, stream is misaligned: {this.stream.Length}");

        // this service is just spinning up the 4kb blocks of data - recordservice handles the actual data handling and partitioning
        if(data.Length > Storage.ContentSize)
            throw new ArgumentException($"{nameof(data)}'s size is larger than the expected content size.");

        // our blocks are indexed by position, so we can find the block by iterating through n blocks and assigning the length
        // this id is the database location of the block
        var id = (uint)Math.Ceiling((double)this.stream.Length / (double)this.blockSize); 

        // setup block header first
        // this id is the data's position within the block, so 0 to start
        var header = new BlockHeader(0, 0, 0, (uint)data.Length); 
        // create our in memory block, cache it, and save it to our file
        Block block = Block.Create(this, id, this.stream, data, header); 
        OnBlockInitialized(block);
        return block;
    }

    private void OnBlockInitialized(IBlock block) {
        // cache the block
        blocks[block.Id] = block;
        // subscribe to the dispose event
        block.Disposed += HandleBlockDisposed;
        WriteBlock(block);
    }

    private void WriteBlock(IBlock block) {
        // set our file to be 1 block longer than its current total length to make room
        this.stream.SetLength((long)((block.Id * blockSize) + blockSize));
        // save everything in the stream and clear it
        this.stream.Flush();
        // make sure the stream cursor is at the correct slot
        this.stream.Seek(block.Id, SeekOrigin.Begin);
    }

    public void SeekStartOfBlock(int id, int offset = 0) {
        int blockPosition = (id * Storage.BlockSize) + offset;
        this.stream.Seek(blockPosition, SeekOrigin.Begin);
    }

    protected void HandleBlockDisposed(object? sender, EventArgs e)
    {
        if (sender is not Block block) return;
        block.Disposed -= HandleBlockDisposed;
        blocks.Remove(block.Id);
    }
}
