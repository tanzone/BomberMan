using System.Collections.Generic;

namespace BomberMan
{
	public class Player
	{
		private int index;
		private int precX;
		private int precY;
		private int x;
		private int y;
		private string ip;
		private string username;
		private int bomb;
		private List<Bomb> bombPiazzate;

		#region SET e GET delle varie Variabili
		public int Index
		{ get { return this.index; } }

		public int PrecX
		{ get { return this.precX; } }

		public int PrecY
		{ get { return this.precY; } }

		public int X
		{
			get { return this.x; }
			set { this.x = value; }
		}

		public int Y
		{
			get { return this.y; }
			set { this.y = value; }
		}

		public string Ip
		{ get { return ip; } }

		public string Username
		{ get { return this.username; } }

		public int Bomb
		{
			set { bomb = value; }
			get { return this.bomb; }
		}

		public List<Bomb> BombPiazzate
		{ get { return this.bombPiazzate; } }

		#endregion

		public Player(int index, int x, int y, string ip, string username)
		{
			this.index = index;
			this.precX = x;
			this.precY = y;
			this.x = x;
			this.y = y;
			this.ip = ip;
			this.username = username;
			this.Bomb = 10; //POSSIBILITA' DI PIAZZARE PIU BOMBE
			this.bombPiazzate = new List<Bomb>();
		}

		public string Mossa(string mossa)
		{
			if (Constant.MOV_RIGHT.Equals(mossa) && (Constant.field.CampoGioco[x][y + 1].Equals(Constant.NO_WALL) || Constant.field.CampoGioco[x][y + 1].Equals(Constant.EXPLOSION)))
				return EseguiMovimento(0, 1);

			if (Constant.MOV_LEFT.Equals(mossa) && (Constant.field.CampoGioco[x][y - 1].Equals(Constant.NO_WALL) || Constant.field.CampoGioco[x][y - 1].Equals(Constant.EXPLOSION)))
				return EseguiMovimento(0, -1);

			if (Constant.MOV_UP.Equals(mossa) && (Constant.field.CampoGioco[x - 1][y].Equals(Constant.NO_WALL) || Constant.field.CampoGioco[x - 1][y].Equals(Constant.EXPLOSION)))
				return EseguiMovimento(-1, 0);

			if (Constant.MOV_DOWN.Equals(mossa) && (Constant.field.CampoGioco[x + 1][y].Equals(Constant.NO_WALL)) || Constant.field.CampoGioco[x + 1][y].Equals(Constant.EXPLOSION))
				return EseguiMovimento(1, 0);

			if (Constant.MOV_BOMB.Equals(mossa) && this.bomb > 0)
			{
				AggiungiBomba();
				return "BOMB";
			}
			return "NO";
		}

		private string EseguiMovimento(int nuovaX, int nuovaY)
		{
			this.precX = this.x; this.precY = this.y;
			this.x += nuovaX; this.y += nuovaY;

			if (Constant.field.CampoGioco[this.x][this.y].Equals(Constant.EXPLOSION))
				return "MORTO";
			return "OK";
		}

		private void AggiungiBomba()
		{
			bomb--;
			bombPiazzate.Add(new Bomb(this.index, this.x, this.y));
		}

		public void TogliBomba()
		{
			bomb++;
			bombPiazzate.RemoveAt(0);
		}

		public void UccidiPlayer()
		{
			this.x = -1;
			this.y = -1;
		}
	}
}
