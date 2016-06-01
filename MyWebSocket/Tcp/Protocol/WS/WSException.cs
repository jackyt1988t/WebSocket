﻿using System;
using System.Net.Sockets;

namespace MyWebSocket.Tcp.Protocol.WS
{
    [Serializable]
    class WSException : Exception
    {
		public int Number
        {
            get;
            private set;
        }
		public string Errors
		{
			get;
			private set;
		}
		public WSClose Closes
		{
			get;
			private set;
		}
		public WSException() :
            base()
			{

			}
        public WSException(string message) :
            base(message)
			{

			}
        public WSException(string message, int num) :
            base(message)
			{
				Number = num;
			}
		public WSException(string message, WsError num, WSClose close) :
			base( message )
			{
				Closes = close;
				Number = (int)num;
				Errors = WSErrorMsg.Error(num);
			}
		public WSException(string message, SocketError num, WSClose close) :
			base( message )
			{
				Closes = close;
				Number = (int)num;				
				Errors = WSErrorMsg.Error(num);
			}
			
		public override string ToString()
		{
			return "Код ошибки: " + Number + "("+ Closes.ToString() +"). Сообщение об ошибке: " + Errors;	
		}
    }
}