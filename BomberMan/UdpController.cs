using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace BomberMan
{
	public class UdpController
	{
		#region Variabili
		internal static event StartGameNotify StartGame;
		internal delegate void StartGameNotify(int numPlayer, int indexMyPlayer, string enemyUser);

		internal static event AzioneMovimentoNotify AzioneMovimento;
		internal delegate void AzioneMovimentoNotify(string mossa, int indexPlayer);

		internal static event AzionePartitaNotify AzioneFinePartita;
		internal delegate void AzionePartitaNotify(string evento, string pacchetto, bool invia);

		public static bool bindRun = false;
		public static int portaDestinatario = 5000;
		public static string ipDestinatario = "127.0.0.1";

		public static Socket mySocket;
		public static EndPoint endPoint;
		public static byte[] bufferRice = new byte[1024];
		public static byte[] bufferSped = new byte[1024];
		#endregion

		public static void Inizializza(int enemyPort, string ipDest)
		{
			portaDestinatario = enemyPort;
			ipDestinatario = ipDest;
		}

		#region Connessione P2P UDP
		public static void Bind()
		{
			try
			{
				IPEndPoint ipEP = new IPEndPoint(IPAddress.Any, Constant.UDP_PORT_DEFAULT);

				mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				mySocket.Bind(ipEP);

				endPoint = (EndPoint)ipEP;

				mySocket.BeginReceiveFrom(bufferRice, 0, bufferRice.Length, SocketFlags.None, ref endPoint, new AsyncCallback(OnReceive), endPoint);
				bindRun = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Impossibile eseguire il bind sulla porta : " + Constant.UDP_PORT_DEFAULT);
				Constant.UDP_PORT_DEFAULT++;
				mySocket.Shutdown(SocketShutdown.Both);
				mySocket.Close();
				mySocket = null;
				Bind();
			}
		}

		#endregion

		#region Ricezione del messaggio
		private static void OnReceive(IAsyncResult ar)
		{
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				try
				{
					if (mySocket == null)
						return;

					string strReceived;
					int idx;

					endPoint = ((EndPoint)new IPEndPoint(IPAddress.Any, 0));
					mySocket.EndReceiveFrom(ar, ref endPoint);

					string[] astr = endPoint.ToString().Split(':');

					strReceived = Encoding.UTF8.GetString(bufferRice);

					idx = strReceived.IndexOf((char)0);
					if (idx > -1)
						ProtocolloRicevuto(strReceived.Substring(0, idx), astr[0], astr[1]);

					bufferRice = new byte[bufferRice.Length];
					mySocket.BeginReceiveFrom(bufferRice, 0, bufferRice.Length, SocketFlags.None, ref endPoint, new AsyncCallback(OnReceive), endPoint);

				}
				catch (Exception ex)
				{
					MessageBox.Show("L'avversario non e' connesso !!\n\n" + ex.Message);
					mySocket.Shutdown(SocketShutdown.Both);
					mySocket.Close();
					mySocket = null;
					Bind();
				}
			}));
		}

		private static void ProtocolloRicevuto(string messaggio, string ip, string port)
		{
			if (!ControlProtocolGame(messaggio, ip, port))
				ControlProtocolMenu(messaggio, ip, port);
		}

		private static bool ControlProtocolGame(string messaggio, string ip, string port)
		{
			if (ip.Equals(ipDestinatario) && port.Equals(portaDestinatario.ToString()))
			{
				switch (messaggio[0] + "")
				{
					case Constant.RIGHT: AzioneMovimento(Constant.MOV_LEFT, 1); return true;
					case Constant.LEFT: AzioneMovimento(Constant.MOV_RIGHT, 1); return true;
					case Constant.UP: AzioneMovimento(Constant.MOV_DOWN, 1); return true;
					case Constant.DOWN: AzioneMovimento(Constant.MOV_UP, 1); return true;
					case Constant.BOMB: AzioneMovimento(Constant.MOV_BOMB, 1); return true;
					case Constant.END: ControlloPacchettoEnd(messaggio[1] + ""); return true;
				}
			}
			return false;
		}

		private static void ControlloPacchettoEnd(string messaggio)
		{
			switch (messaggio)
			{
				case Constant.WIN: AzioneFinePartita("Hai Perso Nella schermata Nemica", "", false); break;
				case Constant.LOSE: AzioneFinePartita("Hai Vinto Nella schermata Nemica", "", false); break;
				case Constant.DRAW: AzioneFinePartita("Hai Pareggiato Nella schermata Nemica", "", false); break;
			}
		}

		private static void ControlProtocolMenu(string messaggio, string ip, string port)
		{
			if (!messaggio.StartsWith(Constant.RIFIUTO))
			{
				if (messaggio.StartsWith(Constant.LINK))
				{
					if (Constant.PERSONE_ON > 0)
						if (MessageBox.Show("VUOI GIOCARE CON -" + messaggio.Substring(1, messaggio.Length - 1) + "-?", "RICHIESTA DI GIOCO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							Constant.PERSONE_ON--;
							Inizializza(Int32.Parse(port), ip);
							Send(Constant.CONFERMA + Constant.USERNAME_MYPLAYER);
							StartGame(2, Constant.INDEX_MY_PLAYER, messaggio.Substring(1, messaggio.Length - 1));
						}
						else Send(Constant.RIFIUTO + Constant.USERNAME_MYPLAYER);
					else Send(Constant.RIFIUTO + Constant.USERNAME_MYPLAYER);
				}
				else if (messaggio.StartsWith(Constant.CONFERMA))
				{
					if (Constant.PERSONE_ON > 0)
					{
						Constant.PERSONE_ON--;
						StartGame(2, Constant.INDEX_MY_PLAYER, messaggio.Substring(2, messaggio.Length - 2));
					}
				}
			}
		}
		#endregion

		#region Send messaggio

		public static void Send(string messaggio)
		{
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				IPEndPoint ipEP = new IPEndPoint(IPAddress.Parse(ipDestinatario), portaDestinatario);
				endPoint = (EndPoint)ipEP;
				try
				{
					bufferSped = Encoding.UTF8.GetBytes(messaggio);
					mySocket.BeginSendTo(bufferSped, 0, messaggio.Length, SocketFlags.None, endPoint, new AsyncCallback(OnSend), null);
				}
				catch (Exception ex) { MessageBox.Show("Send(): eccezione Exception\n" + ex.Message); }
			}));
		}

		private static void OnSend(IAsyncResult ar)
		{
			try { mySocket.EndSend(ar); } catch (Exception ex) { MessageBox.Show("OnSend(): Eccezione Exception\n" + ex.Message); }
		}

		#endregion

		#region Chiusura
		public static void Chiusura()
		{
			Constant.PERSONE_ON++;
		}
		#endregion
	}
}
