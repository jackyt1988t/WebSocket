﻿using System;
using System.Net.Sockets;

namespace MyWebSocket.Tcp.Protocol
{
	class BaseProtocol : IProtocol, IDisposable
	{
		/// <summary>
		/// tcp/ip соединение
		/// </summary>
		public Socket Tcp
		{
			get;
			set;
		}
		public virtual States State
		{
			get;
			protected set;
		}
		public virtual StreamS Reader
		{
			get;
			protected set;
		}
		public virtual StreamS Writer
		{
			get;
			protected set;
		}
		public virtual IHeader Request
		{
			get;
			protected set;
		}
		public virtual IHeader Response
		{
			get;
			protected set;
		}

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public virtual void Dispose(bool disposing)
		{
			
			if (disposing)
			{
				if (Tcp != null)
					Tcp.Dispose();
				if (Writer != null)
					Writer.Dispose();
				if (Reader != null)
					Reader.Dispose();
			}
		}
		public virtual TaskResult TaskLoopHandlerProtocol()
		{
			throw new NotSupportedException("Не поддерживается");
		}

		protected SocketError Read()
		{
			SocketError error = SocketError.Success;
			int count = 8000;
			int start =
			   (int)Reader.PointW;
			byte[] buffer =
					Reader.Buffer;
			
			if (Reader.Count - start < count)
				count =
					  (int)(Reader.Count - start);
			
			int length = Tcp.Receive(buffer, start, count, SocketFlags.None, out error);
			if (length > 0)
			{
				if (Reader.Clear < length)
					error = SocketError.SocketError;
					else
						Reader.SetLength(length);
			}
			return error;
		}
		protected SocketError Send()
		{
			SocketError error = SocketError.Success;
			int start =
				(int)Writer.PointR;
			int write =
				(int)Writer.Length;
			if (write > 16000)
				write = 16000;
			byte[] buffer =
					Writer.Buffer;
			
			if (Writer.Count - start < write)
				write =
				  (int)(Writer.Count - start);
			int length  =  Tcp.Send(buffer, start, write, SocketFlags.None, out error);
			if (length > 0)
				Writer.Position = length;
			return error;
		}
		protected SocketError Write(byte[] buffer, int start, int write)
		{
			SocketError error = SocketError.Success;
			if (Writer.Empty)
				start  =  Tcp.Send(buffer, start, write, SocketFlags.None, out error);

			int length = write - start;
			if (length > 0)
			{
				if (Writer.Clear < length)
					error = SocketError.SocketError;
				else
				{
					Writer.Write(buffer, start, length);
							   Reader.SetLength(length);
				}
			}
			return error;
		}
	}
}
