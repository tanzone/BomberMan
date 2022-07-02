using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BomberMan
{
	/// <summary>
	/// Logica di interazione per MainWindow.xaml
	/// </summary>
	public partial class GameField : Window
	{
		private Grid dynamicGrid;
		private List<Player> players;
		private int indexMyPlayer;
		private int numPlayer;
		private bool online;
		private bool end;

		public GameField(int numPlayer, int indexMyPlayer, List<string> ip, List<string> user, bool online)
		{
			UdpController.AzioneMovimento += new UdpController.AzioneMovimentoNotify(ControlloMovimento);
			UdpController.AzioneFinePartita += new UdpController.AzionePartitaNotify(AzioniFinePartita);

			this.numPlayer = numPlayer;
			this.indexMyPlayer = indexMyPlayer;
			this.online = online;
			this.end = false;

			InitializeVariable(numPlayer, ip, user);
			InitializeComponent();

			SettaggiFormPrincipale();

			CreateDynamicWPFGrid();
		}

		#region Settaggi Iniziali
		private void InitializeVariable(int numPlayer, List<string> ip, List<string> user)
		{
			Constant.CreaCampoGioco(numPlayer);
			players = new List<Player>();

			for (int i = 0; i < numPlayer; i++)
				players.Add(new Player(i, Constant.X[i], Constant.Y[i], ip[i], user[i]));
		}

		private void SettaggiFormPrincipale()
		{
			rootWindow.Height = Constant.ROOT_MARGIN_HEIGHT;
			rootWindow.Width = Constant.ROOT_MARGIN_WIDTH;

			this.Left = 0;
			this.Top = 0;
		}
		#endregion

		#region Grafica del programma iniziale per la creazione del terreno

		private void CreateDynamicWPFGrid()
		{
			//Creo la griglia
			dynamicGrid = new Grid();
			dynamicGrid.Width = Constant.GRID_WIDTH;
			dynamicGrid.Height = Constant.GRID_HEIGHT;
			dynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
			dynamicGrid.VerticalAlignment = VerticalAlignment.Top;
			dynamicGrid.Background = new SolidColorBrush(Constant.COLORE_MURO_NO);

			CreaGriglia();
			CreaColorazioniStandard();

			//Aggiungo la griglia dinamica al root principale
			rootWindow.Content = dynamicGrid;
		}

		private void CreaGriglia()
		{
			for (int i = 0; i < Constant.NUM_CELLS + Constant.NUM_CELLS_WALL; i++)
			{
				ColumnDefinition colX = new ColumnDefinition();
				RowDefinition rowX = new RowDefinition();

				colX.Width = new GridLength(Constant.GRID_CELL_LENGHT);
				rowX.Height = new GridLength(Constant.GRID_CELL_LENGHT);

				dynamicGrid.ColumnDefinitions.Add(colX);
				dynamicGrid.RowDefinitions.Add(rowX);
			}
		}

		private void CreaColorazioniStandard()
		{
			Border[][] backGround = new Border[Constant.NUM_CELLS + Constant.NUM_CELLS_WALL][];

			//Varie colorazioni dipendentemente da cosa trovo in quella cella
			for (int i = 0; i < Constant.NUM_CELLS + Constant.NUM_CELLS_WALL; i++)
			{
				backGround[i] = new Border[Constant.NUM_CELLS + Constant.NUM_CELLS_WALL];
				for (int j = 0; j < Constant.NUM_CELLS + Constant.NUM_CELLS_WALL; j++)
				{
					backGround[i][j] = new Border();

					for (int k = 0; k < Constant.NAME_PLAYER.Length; k++)
						if (Constant.field.CampoGioco[i][j].Equals(Constant.NAME_PLAYER[k]))
						{
							backGround[i][j].Background = new SolidColorBrush(Constant.COLOR_PLAYER[k]);
							backGround[i][j].CornerRadius = new CornerRadius(Constant.BORDER_RADIUS_PLAYER);
						}

					if (Constant.field.CampoGioco[i][j].Equals(Constant.UNBREAKABLE_WALL))
						backGround[i][j].Background = new SolidColorBrush(Constant.COLORE_MURO_HARD);

					else if (Constant.field.CampoGioco[i][j].Equals(Constant.BREAKABLE_WALL))
						backGround[i][j].Background = new SolidColorBrush(Constant.COLORE_MURO_SOFT);
					else if (Constant.field.CampoGioco[i][j].Equals(Constant.NO_WALL))
						backGround[i][j].Background = new SolidColorBrush(Constant.COLORE_MURO_NO);

					Grid.SetRow(backGround[i][j], i);
					Grid.SetColumn(backGround[i][j], j);

					dynamicGrid.Children.Add(backGround[i][j]);
				}
			}
		}

		#endregion

		#region Input della tastiera

		private void InputTastiera(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (indexMyPlayer != -1 && numPlayer > 1)
				ControlloMovimento(e.Key.ToString(), indexMyPlayer);
		}

		private void ControlloMovimento(string mossa, int indexPlayer)
		{
			string ritorno = players[indexPlayer].Mossa(mossa);

			if (indexPlayer == indexMyPlayer && !ritorno.Equals("NO") && online)
				InviaMossa(mossa, indexPlayer);

			if (ritorno.Equals("OK"))
				ColorazioneMovimento(indexPlayer, Constant.COLOR_PLAYER[indexPlayer]);
			else if (ritorno.Equals("BOMB"))
				ColorazioneBomba(indexPlayer);
			else if (ritorno.Equals("MORTO"))
			{
				ColorazioneMovimento(indexPlayer, Constant.COLORE_EXPLOSION);
				AzionePlayerMorto(indexPlayer);
				AzioneDopoMorte();
			}
		}

		private void InviaMossa(string mossa, int indexPlayer)
		{
			switch (mossa)
			{
				case Constant.MOV_RIGHT: UdpController.Send(Constant.RIGHT + (players[indexPlayer].Y - 1) + (players[indexPlayer].X - 1)); break;
				case Constant.MOV_LEFT: UdpController.Send(Constant.LEFT + (players[indexPlayer].Y - 1) + (players[indexPlayer].X - 1)); break;
				case Constant.MOV_UP: UdpController.Send(Constant.UP + (players[indexPlayer].Y - 1) + (players[indexPlayer].X - 1)); break;
				case Constant.MOV_DOWN: UdpController.Send(Constant.DOWN + (players[indexPlayer].Y - 1) + (players[indexPlayer].X - 1)); break;
				case Constant.MOV_BOMB: UdpController.Send(Constant.BOMB + (players[indexPlayer].Y - 1) + (players[indexPlayer].X - 1)); break;
			}
		}

		#endregion

		#region Colorazione grafica del movimento

		private void ColorazioneMovimento(int indexPlayer, Color colorazione)
		{
			if (!(Constant.field.CampoGioco[players[indexPlayer].PrecX][players[indexPlayer].PrecY].Equals(Constant.BOMB)))
			{
				Constant.field.CampoGioco[players[indexPlayer].PrecX][players[indexPlayer].PrecY] = Constant.NO_WALL;
				((Border)dynamicGrid.Children[(players[indexPlayer].PrecX * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + players[indexPlayer].PrecY]).Background = new SolidColorBrush(Constant.COLORE_MURO_NO);
				((Border)dynamicGrid.Children[(players[indexPlayer].PrecX * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + players[indexPlayer].PrecY]).CornerRadius = new CornerRadius(0);
			}

			Constant.field.CampoGioco[players[indexPlayer].X][players[indexPlayer].Y] = Constant.NAME_PLAYER[indexPlayer];
			((Border)dynamicGrid.Children[(players[indexPlayer].X * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + players[indexPlayer].Y]).Background = new SolidColorBrush(colorazione);
			((Border)dynamicGrid.Children[(players[indexPlayer].X * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + players[indexPlayer].Y]).CornerRadius = new CornerRadius(Constant.BORDER_RADIUS_PLAYER);
		}

		private void ColorazioneBomba(int indexPlayer)
		{
			Constant.field.CampoGioco[players[indexPlayer].X][players[indexPlayer].Y] = Constant.BOMB;
			((Border)dynamicGrid.Children[(players[indexPlayer].X * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + players[indexPlayer].Y]).CornerRadius = new CornerRadius(Constant.BORDER_RADIUS_BOMB);
			((Border)dynamicGrid.Children[(players[indexPlayer].X * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + players[indexPlayer].Y]).Background = new SolidColorBrush(Constant.COLORE_BOMB);
			StartTimerBomb(indexPlayer, players[indexPlayer].BombPiazzate[players[indexPlayer].BombPiazzate.Count - 1].X, players[indexPlayer].BombPiazzate[players[indexPlayer].BombPiazzate.Count - 1].Y);
		}

		#endregion

		#region Controlli sulla Bomba e Varie Azioni

		public void StartTimerBomb(int indexPlayer, int xBomb, int yBomb)
		{
			new Thread(this.FaiQualcosa).Start(new List<int>() { indexPlayer, xBomb, yBomb });
		}

		public void FaiQualcosa(object data)
		{
			try
			{
				Thread.Sleep(Constant.TIME_EXPLOSION);
				EsplosioneBomb(((List<int>)data)[0], ((List<int>)data)[1], ((List<int>)data)[2]);
			}
			catch (Exception e) { }
		}

		public void EsplosioneBomb(int indexPlayer, int xBomb, int yBomb)
		{
			try
			{
				Constant.field.CampoGioco[xBomb][yBomb] = Constant.NO_WALL;
				players[indexPlayer].TogliBomba();
				ColorazioneEsplosione(xBomb, yBomb);
			}
			catch (Exception e) { }
		}

		#region Colorazione della bomba e dell'animazione dell'esplosione

		private void ColorazioneEsplosione(int xBomb, int yBomb)
		{
			try
			{
				RaggiEsplosivi(xBomb, yBomb, Constant.EXPLOSION, Constant.COLORE_EXPLOSION);
				ElaboraEsplosione(xBomb, yBomb);
				Thread.Sleep(Constant.TIME_EXPLOSION_VIS);
				RaggiEsplosivi(xBomb, yBomb, Constant.NO_WALL, Constant.COLORE_MURO_NO);			
			}
			catch (Exception e) { }
		}

		private void RaggiEsplosivi(int xBomb, int yBomb, string azione, Color colorazione)
		{
			try
			{
				Application.Current.Dispatcher.Invoke((Action)(() =>
				{
					((Border)dynamicGrid.Children[(xBomb * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + yBomb]).CornerRadius = new CornerRadius(0);
				}));

				for (int i = xBomb; i <= Constant.NUM_CELLS; i++)
				{
					if (Constant.field.CampoGioco[i][yBomb] == Constant.UNBREAKABLE_WALL) break;

					Application.Current.Dispatcher.Invoke((Action)(() =>
					{
						Constant.field.CampoGioco[i][yBomb] = azione;
						((Border)dynamicGrid.Children[(i * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + yBomb]).Background = new SolidColorBrush(colorazione);
					}));
				}

				for (int i = xBomb; i <= Constant.NUM_CELLS; i--)
				{
					if (Constant.field.CampoGioco[i][yBomb] == Constant.UNBREAKABLE_WALL) break;

					Application.Current.Dispatcher.Invoke((Action)(() =>
					{
						Constant.field.CampoGioco[i][yBomb] = azione;
						((Border)dynamicGrid.Children[(i * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + yBomb]).Background = new SolidColorBrush(colorazione);
					}));
				}

				for (int i = yBomb; i <= Constant.NUM_CELLS; i++)
				{
					if (Constant.field.CampoGioco[xBomb][i] == Constant.UNBREAKABLE_WALL) break;

					Application.Current.Dispatcher.Invoke((Action)(() =>
					{
						Constant.field.CampoGioco[xBomb][i] = azione;
						((Border)dynamicGrid.Children[(xBomb * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + i]).Background = new SolidColorBrush(colorazione);
					}));
				}

				for (int i = yBomb; i <= Constant.NUM_CELLS; i--)
				{
					if (Constant.field.CampoGioco[xBomb][i] == Constant.UNBREAKABLE_WALL) break;

					Application.Current.Dispatcher.Invoke((Action)(() =>
					{
						Constant.field.CampoGioco[xBomb][i] = azione;
						((Border)dynamicGrid.Children[(xBomb * (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL)) + i]).Background = new SolidColorBrush(colorazione);
					}));
				}
			}
			catch (Exception e) { }
		}

		#endregion

		#region Azioni per la morte di un giocatore

		private void ElaboraEsplosione(int x, int y)
		{
			try
			{
				List<int> playerMorto = Bomb.ControllaEsplosione(x, y, players);

				if (playerMorto.Count != 0)
				{
					foreach (int control in playerMorto)
						AzionePlayerMorto(control);

					AzioneDopoMorte();
				}
			}
			catch (Exception e) { }
		}

		private void AzionePlayerMorto(int indexPlayer)
		{
			if (indexPlayer == indexMyPlayer) indexMyPlayer = -1;
			Constant.field.CampoGioco[players[indexPlayer].X][players[indexPlayer].Y] = Constant.NO_WALL;
			players[indexPlayer].UccidiPlayer();
			numPlayer -= 1;
		}

		private void AzioneDopoMorte()
		{
			if (numPlayer == 0)
				AzioniFinePartita("PAREGGIO", Constant.DRAW, true);
			else if (indexMyPlayer == -1)
				AzioniFinePartita("PERSO", Constant.LOSE, true);
			else if (numPlayer == 1)
				AzioniFinePartita("VINTO", Constant.WIN, true);
		}

		private void AzioniFinePartita(string evento, string pacchetto, bool invia)
		{
			if (!this.end)
			{
				this.end = true;
				if (online && invia)
					Application.Current.Dispatcher.Invoke((Action)(() => { UdpController.Send(Constant.END + pacchetto); }));
				MessageBox.Show(evento);
				Application.Current.Dispatcher.Invoke((Action)(() => { this.Close(); }));
			}
		}

		#endregion

		#endregion

		#region Chiusura della schermata di gioco

		private void RootWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (online && !end)
				UdpController.Send(Constant.END + Constant.LOSE);

			Chiusura();
		}

		private void Chiusura()
		{
			UdpController.AzioneMovimento -= new UdpController.AzioneMovimentoNotify(ControlloMovimento);
			UdpController.AzioneFinePartita -= new UdpController.AzionePartitaNotify(AzioniFinePartita);

			UdpController.Chiusura();
			Constant.ChiudiCampo();
			new MainMenu().Show();
		}

		#endregion
	}
}
