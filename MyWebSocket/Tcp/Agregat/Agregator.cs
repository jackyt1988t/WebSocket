﻿using System;
using System.Net.Sockets;
	using System.Threading;
using System.Collections.Concurrent;

using MyWebSocket.Tcp.Protocol;
using MyWebSocket.Tcp.Protocol.WS;
using MyWebSocket.Tcp.Protocol.HTTP;

namespace MyWebSocket.Tcp
{
	class Agregator
	{
		public IProtocol Protocol;		
		private static ConcurrentQueue<Agregator> Container;

		static Agregator()
		{
			Container = new ConcurrentQueue<Agregator>();
		}
		public Agregator(Socket tcp)
		{
			Protocol = new HTTPProtocol(tcp);
			Container.Enqueue(this);
		}
 static public void Loop()
		{
			short loop = 0;
			while ( true )
			{
				Agregator agregator = null;
				try
				{
					
					if (!Container.TryDequeue(out agregator))
						Thread.Sleep(1);
					else
					{
						agregator.TaskLoopHandler();
						if (loop++ > 500)
						{
							loop = 0;
							Thread.Sleep(1);
						}
					}
				}
				catch ( FieldAccessException exc )
				{
					Console.WriteLine("Обработчик: " + exc.Message);
				}
			}
		}
		private void TaskLoopHandler()
		{
			TaskResult TaskResult = Protocol.TaskLoopHandlerProtocol();
			switch (TaskResult.Option)
			{
				case TaskOption.Loop:
					Container.Enqueue(this);
					break;
				case TaskOption.Protocol:
					if (TaskResult.Protocol == TaskProtocol.WSN13)
					{
						Protocol = new WSProtocolN13(Protocol);
						Container.Enqueue(this);
					}
					break;
				case TaskOption.Threading:
						Thread thr = new Thread( TaskLoopThreading );
							   thr.IsBackground = true;
							   thr.Start();
						break;
			}
		}
		private void TaskLoopThreading()
		{
					while (true)
					{
						TaskResult TaskResult = Protocol.TaskLoopHandlerProtocol();
						switch (TaskResult.Option)
						{
							case TaskOption.Loop:
								break;
							case TaskOption.Delete:
								return;
							default:
								throw new ArgumentException("TaskResult");
						}
						Thread.Sleep(1);
					}
		} 
	}
}