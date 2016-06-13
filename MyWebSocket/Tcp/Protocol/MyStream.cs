﻿using System;
using System.IO;

namespace MyWebSocket.Tcp.Protocol
{
	/// <summary>
	/// Кольцевой поток данных
	/// </summary>
	class Mytream : Stream
	{
		public long Count
		{
			get
			{
				return _len;
			}
		}
		public bool Empty
		{
			get
			{
				return (_p_r == _p_w);
			}
		}
		public long Clear
		{
			get
			{
				if (_p_w < _p_r)
					return (_p_r - _p_w) - 1;
				else
					return (_len - _p_w) + _p_r;
			}
		}		
		long _p_w;
		public long PointR
		{
			get
			{
				return _p_r;
			}
			protected set
			{
				if (value < _len)
					_p_r = value;
				else
					_p_r = 0;
			}
		}
		long _p_r;
		public long PointW
		{
			get
			{
				return _p_w;
			}
			protected set
			{
				if (value < _len)
					_p_w = value;
				else
					_p_w = 0;
			}
		}
		public object __Sync
		{
			get;
			private set;
		}
		public byte[] Buffer
		{
			get
			{
				return _buffer;
			}
		}
		public override long Length
		{
			get
			{
				if (_p_w < _p_r)
					return (_len - _p_r) + _p_w;					
				else
					return (_p_w - _p_r);
			}
		}
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}
		public override long Position
		{
			get
			{
				return 0;
			}

			set
			{
				if (value > 0)
				{
					if (value > Length)
						throw new IOException();
					if (value + _p_r < Count)
						_p_r = value + _p_r;
					else
						_p_r = value - (Count - _p_r);
				}
			}
		}
		protected long _len;
		protected byte[] _buffer;
		
		public Mytream(int length) : 
			base()
		{
			_len = length;
			__Sync = new object();
			_buffer = new byte[length];
		}
#region virtual
		/// <summary>
		/// сбрасывает поток в начальное положение
		/// </summary>
		public virtual void Reset()
		{
			_p_r = 0;
			_p_w = 0;
		}
		/// <summary>
		/// увеличивает вместимость кольцевого потока
		/// </summary>
		/// <param name="length"></param>
		public virtual void Resize(int length)
		{
			byte[] buffer = new byte[length];
			Read(buffer, 0, (int)_len);

			_p_r    = 0;
			_p_w    = (int)_len;
			_len    = length;
			_buffer = buffer;

		}
#endregion

#region ovveride
		public override void Flush()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// увеличивает текущую длинну кольцевого потока, 
		/// не может выходить за рамки длинны буффера потока
		/// </summary>
		/// <param name="value">количетво на которое необходимо увеличить длинну потока</param>
						
		public override void SetLength(long value)
		{
			if (value > Clear)
				throw new IOException();
			if (PointW + value < _len)
				PointW = value + PointW;
			else
				PointW = value - (Count - PointW);
		}
		#endregion

#region read write 
		/// <summary>
		/// считывает тело сообщения
		/// </summary>
		/// <returns>количество прочитанных байт</returns>
		public virtual int ReadBody()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// считывает заголовки сообщения
		/// </summary>
		/// <returns>количество прочитанных байт</returns>
		public virtual int ReadHead()
		{
			throw new NotImplementedException();
		}
		
		public override  int ReadByte()
		{
			if (Empty)
				return -1;
			return _buffer[_p_r++];
		}
		/// <summary>
		/// Записывает данные в поток
		/// </summary>
		/// <param name="buffer">буффер данных для записи</param>
		/// <param name="pos">начальная позиция</param>
		/// <param name="len">количество которое необходимо записать</param>
		/// <returns></returns>
		unsafe public override int Read(byte[] buffer, int pos, int len)
		{
			int i;
			if (Empty)
				return -1;
			fixed(byte* source = buffer, target = _buffer)
			{
				
				byte* ps = source + pos;
				for (  i = 0; i < len; i++  )
				{
					byte* pt = target + PointR;
					
					*ps = *pt;
					ps++;
					PointR++;
					if (_p_r == _p_w)
						;
					if (Empty)
						return i;
				}
			}
			return i;
		}
		/// <summary>
		/// Не поодерживается данной реализацией
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		unsafe public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Читает данные из потока
		/// </summary>
		/// <param name="buffer">буффер в который будут записаны данные</param>
		/// <param name="pos">начальная позиция</param>
		/// <param name="len">количество которое необходимо прочитать</param>
		unsafe public override void Write(byte[] buffer, int pos, int len)
		{
			int i;
			fixed(byte* source = _buffer, target = buffer)
			{
				byte* pt = target + pos;
				for (  i = 0; i < len; i++  )
				{
					byte* ps = source + PointW;

					*ps = *pt;					
					pt++;
					PointW++;
					if (Clear == 0)
						throw new IOException();
				}
			}
		}
#endregion
	}
}
