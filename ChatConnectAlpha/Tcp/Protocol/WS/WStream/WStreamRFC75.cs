﻿using System;
using System.IO;

namespace ChatConnect.Tcp.Protocol.WS
{
	class WStreamRFC75 : IWStream
	{
		
		public int Length
		{
			get;
			private set;
		}
		public int Position
		{
			get;
			set;
		}
		public byte[] __Buffer
		{
			get;
			set;
		}

		public WStreamRFC75(byte[] buffer)
		{
			Position = 0;
			__Buffer = buffer;
			  Length = buffer.Length;
		}
		public WStreamRFC75(byte[] buffer, int pos)
		{
			Position = pos;
			__Buffer = buffer;
			  Length = buffer.Length;
		}
		public WStreamRFC75(byte[] buffer, int pos, int length)
		{
			Position = pos;
			__Buffer = buffer;
					   Length = length;
		}

		public int ReadByte()
		{
			if (__Buffer == null)
				throw new ArgumentNullException("DataBuffer");
			if (__Buffer.Length <= Position)
				return -1;

			return __Buffer[Position++];
		}
		unsafe public int ReadExtn(ref IWSFrame Frame)
		{

		}
		unsafe public int ReadBody(ref IWSFrame Frame)
		{
			int _read = 0;

			if (Frame.DataBody == null)
				Frame.DataBody = new byte[Frame.LengBody];

			fixed (byte* sourse = __Buffer, target = Frame.DataBody)
			{
				byte* ps = sourse + Position;
				byte* pt = target + Frame.PartBody;

				while (Position < Length)
				{
					if (Frame.MaskPos > 3)
						Frame.MaskPos = 0;

					*pt = *ps;
					ps++;
					pt++;

					_read++;
					Position++;

					if (++Frame.PartBody == Frame.LengBody)
					{
						Frame.GetsBody = true;
						return _read;
					}
				}
			}
			_read = -1;
			return _read;
		}
		public int ReadHeader(ref IWSFrame Frame)
		{
			int _read = 0;
			int _byte = -1;

			while ((_byte = ReadByte()) > -1)
			{
				switch (Frame.Handler)
				{
					case 0:
						/*       FIN - доставка сообщения     */
						Frame.BitFind = (int)((uint)_byte >> 7);
						/*      RCV1 - устанавливается сервером.     */
						Frame.BitRsv1 = (int)((uint)_byte << 25 >> 31);
						/*      RCV2 - устанавливается сервером.     */
						Frame.BitRsv2 = (int)((uint)_byte << 26 >> 31);
						/*      RCV3 - устанавливается сервером.     */
						Frame.BitRsv3 = (int)((uint)_byte << 27 >> 31);
						/*      Опкод-хранит информацию о данных     */
						Frame.BitPcod = (int)((uint)_byte << 28 >> 28);

						/* Общая длинна  */
						Frame.LengHead = 2;
						/* Длинна ответа */
						Frame.PartHead = 0;
						/* Длинна ответа */
						Frame.PartBody = 0;
						/*  Обработчик.  */
						Frame.Handler += 1;

						break;
					case 1:
						/*      Бит маски тела сообщения      */
						Frame.BitMask = (int)((uint)_byte >> 7);
						/*     Длинна полученного тела сообщения     */
						Frame.BitLeng = (int)((uint)_byte << 25 >> 25);

						if (Frame.BitLeng == 127)
						{
							Frame.Handler += 1;
							Frame.LengHead += 8;
						}
						else if (Frame.BitLeng == 126)
						{
							Frame.Handler += 7;
							Frame.LengHead += 2;
						}
						else if (Frame.BitLeng <= 125)
						{
							Frame.Handler += 9;
							Frame.LengBody = Frame.BitLeng;
						}

						break;
					case 2:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)_byte << 56);
						break;
					case 3:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 48);
						break;
					case 4:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 40);
						break;
					case 5:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 32);
						break;
					case 6:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 24);
						break;
					case 7:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 16);
						break;
					case 8:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 08);
						break;
					case 9:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengBody = (int)((uint)Frame.LengBody | (uint)_byte << 00);
						break;
					case 10:
						/*  Обработчик.  */
						Frame.Handler += 1;
						Frame.LengExtn = (int)((uint)Frame.LengExtn | (uint)_byte << 08);
						break;
					case 11:
						Frame.LengExtn = (int)((uint)Frame.LengExtn | (uint)_byte << 00);
						break;
				}

				_read++;
				Frame.PartHead++;

				if (Frame.PartHead == Frame.LengHead)
				{
					Frame.GetsHead = true;
					return _read;
				}
			}

			_read = -1;
			return _read;
		}
	}
}