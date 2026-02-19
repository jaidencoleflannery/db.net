using System.Collections.Generic;
using System.IO;
using System.Buffers.Binary;

namespace db.net.BlockService;

public class Block : IBlock {
	public uint Id { get; private set; }
	private readonly Stream stream;
	private readonly long[] headers = new long[8];
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
		if((count + headerOffset) > service.ContentSize) 
            throw new ArgumentOutOfRangeException("Index (count + srcOffset) location exceeds bounds of block content.");
		if((service.ContentSize - (count + headerOffset)) > buffer.Length) 
            throw new ArgumentOutOfRangeException("Target byte array not large enough to fit contents.");

		this.stream.ReadExactly(buffer, offset + headerOffset, (int)count);
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
