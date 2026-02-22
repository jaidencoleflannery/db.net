using db.net.StorageConstants;

namespace db.net.Blocks;

/*
	Block is the model for a single 4kb section of space which we store our metadata headers and related data into.
*/

public class Block : IBlock {

    public readonly uint Id;

    private const int _size = Storage.BlockSize;
    private int _numHeaders;
	private readonly Stream _stream;
    private readonly BlockService _service;
	private readonly Dictionary<int, BlockHeader> _headers;

    // constructors are for forming blocks as we pull them from disk
    public Block(BlockService service, Stream stream, uint id) {
		if(stream == null) 
			throw new ArgumentNullException(nameof(_stream));
        if(service == null)
            throw new ArgumentNullException(nameof(_service));
		this._stream = stream;
        this._service = service;

        this.Id = id;

        this._headers = new Dictionary<int, BlockHeader>();
        this._numHeaders = 0;
    }

    // CreateBlock is for the generation and storing to disk of a new block from data
    public static IBlock Create(BlockService service, Stream stream, uint Id, Span<byte> data) {
        var block = new Block(service, stream, id);
        
        this.AddHeader(data.Length, 0);
        this.Write(data, count: data.Length);

        return block;
    }

    public IBlock Create(BlockService service, Stream stream, byte[] data) {
        var block = new Block(service, stream);
        
        this.AddHeader(data.Length, 0);
        this.Write(data, count: data.Length);

        return block;
    }

    // headers have their own contained IDs
	public BlockHeader GetHeader(int id) {
        if(id < 0)
			throw new ArgumentException(nameof(id));
		if(_stream == null)
			throw new ArgumentNullException(nameof(_stream));
        
        // if its cached, give it back
        if(_headers.TryGetValue(id, out BlockHeader header))
            return header;

        // if the header is not cached, we have to iterate through every header in the block and push the cursor forward to find our header
        // alternatively, you could push all headers into a saved section at the top, but then you're sacrificing a set section of your block
        // we are opting for the space efficient option over the time efficient option here
         
		// find disk position of desired header
        this.CrawlHeaders(id);  

		byte[] headerBuffer = new byte[Storage.HeaderSize];

		// from our found position, read a full blockheader's worth of bytes into the headerbuffer, and then convert it into a BlockHeader object
		this.stream.ReadExactly(headerBuffer, 0, Storage.HeaderSize);
		_headers.Add(id, BlockHeader.ToHeader(headerBuffer));
        return header[id];
	}

	public void AddHeader(int dataSize, int offset) {
        // get end of used block space (position is number of bytes)
		if(!CrawlHeaders(out position)) 
            throw new InvalidOperationException("Not enough space in block to append header.");

        // generate a unique header based on the current block's num of headers
        BlockHeader header = new BlockHeader((_numHeaders + 1), (position + offset), 0, dataSize); 

		// write the header to a buffer and push it to the stream
        byte[] buffer = new byte[Storage.HeaderSize];
		header.ToBuffer(buffer);
        // our stream pointer should be at the end of the used block space after CrawlHeader()
		this.stream.Write(buffer, offset, service.HeaderSize);
    }

	public void Read(byte[] buffer, int offset, int count) {	
		if((count + offset) > Storage.ContentSize) 
            throw new ArgumentOutOfRangeException($"Index ({nameof(count)} + {nameof(offset)}) location exceeds bounds of block content.");
		if((Storage.ContentSize - offset) > buffer.Length)
            throw new ArgumentOutOfRangeException("Target buffer not large enough to fit contents.");
        SeekStartOfBlock();
		// read from block, starting at provided offset
		this.stream.ReadExactly(buffer, offset, count);
	}

	public void Write(byte[] data) {
		if((data.Length + offset) > service.ContentSize) 
            throw new ArgumentOutOfRangeException("Size of contents (count + offset) exceeds bounds of block content.");
		this.stream.Write(data, 0, data.Length);
	}

    // go to start of block
    public void SeekStartOfBlock(int offset = 0) {
        int blockPosition = (this.Id * Size) + offset;
        this.stream.Seek(blockPosition, SeekOrigin.Begin);
    }

    // bring stream pointer to the end of the block's data, or a specific header if the header id is provided
    private bool CrawlHeaders(uint? id, out int readBytes) {
        // find disk position for the block, and then move forward 1 uint (Block Id) to reach the first header
        SeekStartOfBlock();
        this.stream.Seek(sizeof(uint), SeekOrigin.Current);
        int readBytes = sizeof(uint);
        int headerCounter = 0;
        byte[] buffer = new byte[4];
        
        while(readBytes + Storage.HeaderSize < Storage.BlockSize && headerCounter < _numHeaders) { 
            // + headersize because we need to make sure we have at least enough room to read a header
            // check if the current header Id is the one we're looking for 
            this.stream.ReadExactly(buffer, 0, sizeof(uint)); // pushes the stream forward 4 bytes
            if(id != null && BinaryPrimitives.ReadUInt32LittleEndian(buffer) == id) {
                this.stream.Seek(-sizeof(uint), SeekOrigin.Current);
                break;
            }

            // we have to grab the UsedLength from the header so we can crawl
            this.stream.Seek((sizeof(uint) * 2), SeekOrigin.Current); // skip the next two uints to get to UsedLength
            this.stream.ReadExactly(buffer, 0, sizeof(uint));
            uint UsedLength = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
            
            // at this point, we have read the entire header
            readBytes += Storage.HeaderSize;

            if(readBytes + UsedLength < Storage.BlockSize) {
                this.stream.Seek(UsedLength, SeekOrigin.Current); // we are already at the end of the header, so we skip the usedlength 
                readBytes += UsedLength;
            } else {
                return false;
            }
        }
        return true;
    }

}
