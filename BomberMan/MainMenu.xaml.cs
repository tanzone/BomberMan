using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace BomberMan
{
	/// <summary>
	/// Logica di interazione per MainMenu.xaml
	/// </summary>
	public partial class MainMenu : Window
	{
		private cDati xmlDati;
		private Point m_start;

		public MainMenu()
		{
			UdpController.StartGame += new UdpController.StartGameNotify(RichiestaAccettata);

			if (!UdpController.bindRun)
			{
				Constant.PERSONE_ON = 1;
				UdpController.Bind();
				ValoriFromXml();
			}

			InitializeComponent();
			SetValoriCampi();
		}

		#region Vari settaggi di variabili

		private void ValoriFromXml()
		{
			if (File.Exists(Directory.GetCurrentDirectory() + "\\" + Constant.FILENAME))
				XmlController.Deserializzazione_Xml2Dati();
		}

		private void SetValoriCampi()
		{
			xmlDati = new cDati();
			myAddress.Content = GetLocalIp();
			myPort.Content = Constant.UDP_PORT_DEFAULT;
			myUser.Text = Constant.USERNAME_MYPLAYER;
			username.Content = Constant.USERNAME_MYPLAYER;

			remoteIp.Text = UdpController.ipDestinatario;
			remotePort.Text = UdpController.portaDestinatario.ToString();
			remoteIpNow.Content = UdpController.ipDestinatario;
			remotePortNow.Content = UdpController.portaDestinatario.ToString();
		}

		private string GetLocalIp()
		{
			string strIpDotted = "";

			try
			{
				IPAddress[] IPs = Dns.GetHostAddresses(Dns.GetHostName());
				if (IPs.Length > 0)
					for (int i = 0; i < IPs.Length; i++)
						if (IPs[i].AddressFamily == AddressFamily.InterNetwork)
						{
							strIpDotted = IPs[i].ToString();
							break;
						}
				return strIpDotted;
			}
			catch (Exception ex) { return "127.0.0.1"; }
		}

		#endregion

		#region Metodi utili per la connessione e il gioco

		private void RichiestaAccettata(int numPlayer, int indexMyPlayer, string enemyUser)
		{
			UdpController.StartGame -= new UdpController.StartGameNotify(RichiestaAccettata);

			List<string> ip = new List<string>() { myAddress.Content.ToString(), UdpController.ipDestinatario };
			List<string> user = new List<string>() { username.Content.ToString(), enemyUser };
			new GameField(numPlayer, indexMyPlayer, ip, user, true).Show();

			this.Close();
			MessageBox.Show("Il gioco inizia contro -" + enemyUser + "-");
		}

		#endregion

		#region Eventi Bottoni Finestra Principale

		private void MySettings_Click(object sender, RoutedEventArgs e)
		{
			mainGrid.Visibility = Visibility.Hidden;
			mySettingGrid.Visibility = Visibility.Visible;
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			mainGrid.Visibility = Visibility.Hidden;
			settingGrid.Visibility = Visibility.Visible;
		}

		private void PlayTest_Click(object sender, RoutedEventArgs e)
		{
			UdpController.StartGame -= new UdpController.StartGameNotify(RichiestaAccettata);
			Constant.PERSONE_ON--;

			List<string> ip = new List<string>() { "", "", "", "" };
			List<string> user = new List<string>() { "", "", "", "" };
			new GameField(4, 0, ip, user, false).Show();
			this.Close();
		}

		private void SendRequest_Click(object sender, RoutedEventArgs e)
		{
			UdpController.Send(Constant.LINK + username.Content.ToString());
		}

		private void Quit_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Vuoi davvero uscire ?", "CHIUSURA MENU", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				ChiusuraMenu();
				this.Close();
			}
		}

		private void ChiusuraMenu()
		{
			xmlDati.myUser = Constant.USERNAME_MYPLAYER;
			xmlDati.ipDestinatario = UdpController.ipDestinatario;
			xmlDati.portDestinatario = UdpController.portaDestinatario;
			XmlController.Serializzazione_Dati2Xml(xmlDati);
		}

		#region Chiusra form forzata
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ChiusuraMenu();
		}
		#endregion
		#endregion

		#region Eventi Bottoni Enemy Settings

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			EnemySetVuoto();

			settingGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
		}

		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			EnemySetVuoto();
			UdpController.Inizializza(Int32.Parse(remotePort.Text.ToString()), remoteIp.Text.ToString());
			remoteIpNow.Content = UdpController.ipDestinatario;
			remotePortNow.Content = UdpController.portaDestinatario.ToString();
		}

		private bool ValidatePort(string nPort)
		{
			try
			{
				return (Int32.Parse(nPort) >= Constant.NUM_MIN_PORT && Int32.Parse(nPort) <= Constant.NUM_MAX_PORT);
			}
			catch (Exception e) { return false; }
		}

		private void EnemySetVuoto()
		{
			if (string.IsNullOrWhiteSpace(remoteIp.Text.ToString()))
				remoteIp.Text = UdpController.ipDestinatario;
			if (string.IsNullOrWhiteSpace(remotePort.Text.ToString()) || !ValidatePort(remotePort.Text.ToString()))
				remotePort.Text = UdpController.portaDestinatario.ToString();
		}

		#endregion

		#region Eventi Bottoni My Settings

		private void BackMy_Click(object sender, RoutedEventArgs e)
		{
			MySetVuoto();
			mySettingGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
		}

		private void ConfirmMy_Click(object sender, RoutedEventArgs e)
		{
			MySetVuoto();
			Constant.USERNAME_MYPLAYER = myUser.Text.ToString();
			myUser.Text = Constant.USERNAME_MYPLAYER;
			username.Content = Constant.USERNAME_MYPLAYER;
		}

		private void MySetVuoto()
		{
			if (string.IsNullOrWhiteSpace(myUser.Text.ToString()))
				myUser.Text = Constant.USERNAME_MYPLAYER;
		}

		#endregion

		#region Spostamento Schermata Principale

		private void RootGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.m_start = e.GetPosition(this);
			this.m_start.Y = Convert.ToInt16(this.Top) + this.m_start.Y;
			this.m_start.X = Convert.ToInt16(this.Left) + this.m_start.X;
			rootGrid.CaptureMouse();
		}

		private void RootGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (rootGrid.IsMouseCaptured)
			{
				rootWindow.Opacity = 0.8;
				Point MousePosition = e.GetPosition(this);
				Point MousePositionAbs = new Point();
				MousePositionAbs.X = Convert.ToInt16(this.Left) + MousePosition.X;
				MousePositionAbs.Y = Convert.ToInt16(this.Top) + MousePosition.Y;
				this.Left = this.Left + (MousePositionAbs.X - this.m_start.X);
				this.Top = this.Top + (MousePositionAbs.Y - this.m_start.Y);
				this.m_start = MousePositionAbs;
			}
		}

		private void RootGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			rootWindow.Opacity = 1;
			rootGrid.ReleaseMouseCapture();
		}
		
		#endregion
	}
}