using System.Collections.Generic;
using System.IO;
using System.Buffers.Binary;

namespace db.net.BlockStorage;

// Block is just the core mechanics of a block of memory.
public class Block : IBlock {
	public uint Id { get; private set; }
	private readonly Stream stream;
	private readonly long[] headers = new long[5];
	private readonly BlockStorage storage;
	private readonly byte[] firstSector;

	public bool isDisposed { get; private set; } = false;
	private bool isFirstSectorDirty = false;

	public event EventHandler disposed;

	public Block(BlockStorage storage, uint id, Stream stream, byte[] firstSector) {
		if(stream == null) 
			throw new ArgumentNullException(nameof(stream));
		if(firstSector.Length != storage.DiskSectorSize)
			throw new ArgumentException($"firstSector must be of length {storage.DiskSectorSize}"); 

		this.storage = storage;
		this.Id = id;
		this.stream = stream;
		this.firstSector = firstSector;
	}

	public long GetHeader(int id) {
		if(id <= 0)
			throw new ArgumentException(nameof(id));
		if(stream == null)
			throw new ArgumentNullException(nameof(stream));
	
		// each header is a long type, which is 8 bytes of data 
		int position = (id * 8);
		byte[] buffer = new byte[8];

		this.stream.ReadExactly(buffer, 0, position);
		return BinaryPrimitives.ReadInt64LittleEndian(buffer);
	}

	public void SetHeader(uint id, long value) {
		// each header is a long type, which is 8 bytes of data
		int position = (int)id * 8;
		byte[] buffer = new byte[8]; 

		BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
		this.stream.Write(buffer, position, 8); // header size == 8 bytes
	}

	public void Read(byte[] buffer, int offset, int headerOffset, uint count) {
		if(isDisposed) 
            throw new ObjectDisposedException("Block");
		if((count + headerOffset) > storage.ContentSize) 
            throw new ArgumentOutOfRangeException("Index (count + srcOffset) location exceeds bounds of block content.");
		if((storage.ContentSize - (count + headerOffset)) > buffer.Length) 
            throw new ArgumentOutOfRangeException("Target byte array not large enough to fit contents.");

		this.stream.ReadExactly(buffer, offset + headerOffset, (int)count);
	}

	public void Write(byte[] buffer, int offset, int headerOffset, int count) {
		if(isDisposed) 
            throw new ObjectDisposedException("Block");
		if((count + headerOffset + offset) > storage.ContentSize) 
            throw new ArgumentOutOfRangeException("Size of contents (count + offset) exceeds bounds of block content.");

		this.stream.Write(buffer, offset + headerOffset, count);
	}

	public void Dispose() {
		Dispose();
		GC.SuppressFinalize(this);
	}

}
