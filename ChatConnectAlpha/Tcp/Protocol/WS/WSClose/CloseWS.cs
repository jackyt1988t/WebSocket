﻿using System;
using System.Collections.Generic;

namespace ChatConnect.Tcp.Protocol.WS
{
	class CloseWS
	{
		/// <summary>
		/// Инициатор закрытия
		/// </summary>
		public string Host
		{
			get;
			set;
		}
		/// <summary>
		/// Информация о закрытии
		/// </summary>
		public string CloseMsg
		{
			get;
			set;
		}
		/// <summary>
		/// Код закрытия WebSocket
		/// </summary>
		public WSClose CloseCode
		{
			get;
			set;
		}
		/// <summary>
		/// Вермя закрытия соединения
		/// </summary>	 
		public DateTime CloseTime
		{
			get;
			set;
		}
		/// <summary>
		/// Вермя прошедшее после закрытия
		/// </summary>
		public TimeSpan AwaitTime
		{
			get
			{
				return DateTime.Now - CloseTime;
			}
		}
		/// <summary>
		/// Содержит описание завершения подключения
		/// </summary>
		public static Dictionary<WSClose, string> Message;
		
		public CloseWS(string host, WSClose code)
		{
			Host	  = host;
			CloseMsg  = Message[code];
			CloseCode = code;
		}
		static CloseWS()
		{
			Message = new Dictionary<WSClose, string>();

			Message.Add(WSClose.Normal, "Соединение было закрыто чисто");
			Message.Add(WSClose.GoingAway, "Был выполнен переход");
			Message.Add(WSClose.ProtocolError, "Произошла ошибка протокола");
			Message.Add(WSClose.UnsupportedData, "Данные не поддерживаются текущей версией");
			Message.Add(WSClose.Reserved, "Первышено время ожидания");
			Message.Add(WSClose.NoStatusRcvd, "Код статуса не получен");
			Message.Add(WSClose.Abnormal, "Соединение было закрыто неправильно");
			Message.Add(WSClose.InvalidFrame, "Неверный формат данных");
			Message.Add(WSClose.PolicyViolation, "нарушена политика безопасности");
			Message.Add(WSClose.BigMessage, "Слишком большое сообщение");
			Message.Add(WSClose.Mandatory, "Не возвращен список поддерживаемых расширений");
			Message.Add(WSClose.ServerError, "Произошла ошибка сервера");
			Message.Add(WSClose.TLSHandshake, "не удалось совершить рукопожатие");
		}
		/// <summary>
		/// Возвращает информацию о том каким образом было закрыто соединение
		/// </summary>
		/// <returns>строка с информацие о закрытом подключении</returns>
		public override string ToString()
		{
			return "Инициатор "  +  Host + ". "  +  CloseCode.ToString()  +  ": " + CloseMsg;
		}
	}
}
