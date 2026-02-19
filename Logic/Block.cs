using System.Collections.Generic;
using System.IO;
using System.Buffers.Binary;

namespace db.net.Blocks;

/*
	Block is the model for a single 4kb section of space which we store our data into.
	For context, Block is utilized by Record, which creates a string of Blocks as a Linked List so that more than 4kb can be stored per item.
*/

public class Block : IBlock {
	public uint Id { get; private set; }
	private readonly Stream stream;
	private readonly BlockHeader[] headers = new BlockHeader[8];
	private readonly BlockService service;
	private readonly byte[] firstSector;

	public bool isDisposed { get; private set; } = false;
	private bool isFirstSectorDirty = false;

	public event EventHandler Disposed;

	public Block(BlockService service, uint id, Stream stream, byte[] firstSector) {
		if(stream == null) 
			throw new ArgumentNullException(nameof(stream));
		if(firstSector.Length != service.DiskSectorSize)
			throw new ArgumentException($"firstSector must be of length {service.DiskSectorSize}"); 

		this.service = service;
		this.Id = id;
		this.stream = stream;
		this.firstSector = firstSector;
	}

	public BlockHeader GetHeader(int id) {
		if(id <= 0)
			throw new ArgumentException(nameof(id));
		if(stream == null)
			throw new ArgumentNullException(nameof(stream));
	
		// find in memory position (index of id * the size of each header)
		int position = id * service.HeaderSize;
		// buffer for storing header
		byte[] buffer = new byte[service.HeaderSize];

		// from the index of the id, read a full blockheader's worth of bytes into the buffer, and then convert it into a BlockHeader object
		this.stream.ReadExactly(buffer, position, service.HeaderSize);
		return BlockHeader.ToHeader(buffer);
	}

	public void SetHeader(uint id, BlockHeader header) {
		// each header is 3 uint fields, which is 12 bytes of data
		int position = (int)id * 12;
		byte[] buffer = new byte[service.HeaderSize];

		// write the header to a buffer and push it to the stream
		header.ToBuffer(buffer);
		this.stream.Write(buffer, position, service.HeaderSize); // header size == 12 bytes
	}

	public void Read(byte[] buffer, int offset, int count) {
		if(isDisposed) 
            throw new ObjectDisposedException("Block");
		if((count + service.HeaderSize) > service.ContentSize) 
            throw new ArgumentOutOfRangeException($"Index ({nameof(count)} + {nameof(offset)}) location exceeds bounds of block content.");
		if((service.ContentSize - (count + service.HeaderSize)) > buffer.Length) 
            throw new ArgumentOutOfRangeException("Target buffer not large enough to fit contents.");
		long blockStart = (long)Id * service.BlockSize;
		// reset the stream position to the beginning of this block
		this.stream.Seek(blockStart + service.HeaderSize, SeekOrigin.Begin);
		// read from block, starting at provided offset
		this.stream.ReadExactly(buffer, offset, count);
	}

	public void Write(byte[] buffer, int offset, int headerOffset, int count) {
		if(isDisposed) 
            throw new ObjectDisposedException("Block");
		if((count + headerOffset + offset) > service.ContentSize) 
            throw new ArgumentOutOfRangeException("Size of contents (count + offset) exceeds bounds of block content.");

		this.stream.Write(buffer, offset + headerOffset, count);
	}

	public void Dispose() {
		if (isDisposed) return;
		isDisposed = true;
		Disposed?.Invoke(this, EventArgs.Empty);
		GC.SuppressFinalize(this);
	}

}
