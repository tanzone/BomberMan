using System.Collections.Generic;

namespace BomberMan
{
	public class Bomb
	{
		private int indexPlayer;
		private int x;
		private int y;
		private int raggioExp;

		#region SET e GET delle varie Variabili

		public int IndexPlayer
		{ get { return this.indexPlayer; } }

		public int X
		{ get { return this.x; } }

		public int Y
		{ get { return this.y; } }

		public int RaggioExp
		{ get { return this.raggioExp; } }

		#endregion

		public Bomb(int indexPlayer, int x, int y)
		{
			this.indexPlayer = indexPlayer;
			this.x = x;
			this.y = y;
			this.raggioExp = Constant.NUM_CELLS;
		}

		/// <summary>
		/// Controlla l'esplosione nella 4 direzioni ovviamente si puo dimensionare l'esplosione cambiando una variabile
		/// </summary>
		/// <param name="xBomb"></param>
		/// <param name="yBomb"></param>
		/// <param name="players"></param>
		/// <returns>La lista dei player che sono morti</returns>
		public static List<int> ControllaEsplosione(int xBomb, int yBomb, List<Player> players)
		{
			List<int> playerMorti = new List<int>();

			for (int i = xBomb; i <= Constant.NUM_CELLS; i++)
			{
				if (Constant.field.CampoGioco[i][yBomb] == Constant.UNBREAKABLE_WALL) break;
				for (int j = 0; j < players.Count; j++)
					if (i == players[j].X && yBomb == players[j].Y)
						playerMorti.Add(j);
			}

			for (int i = xBomb - 1; i <= Constant.NUM_CELLS; i--)
			{
				if (Constant.field.CampoGioco[i][yBomb] == Constant.UNBREAKABLE_WALL) break;
				for (int j = 0; j < players.Count; j++)
					if (i == players[j].X && yBomb == players[j].Y)
						playerMorti.Add(j);
			}

			for (int i = yBomb + 1; i <= Constant.NUM_CELLS; i++)
			{
				if (Constant.field.CampoGioco[xBomb][i] == Constant.UNBREAKABLE_WALL) break;
				for (int j = 0; j < players.Count; j++)
					if (i == players[j].Y && xBomb == players[j].X)
						playerMorti.Add(j);
			}

			for (int i = yBomb - 1; i <= Constant.NUM_CELLS; i--)
			{
				if (Constant.field.CampoGioco[xBomb][i] == Constant.UNBREAKABLE_WALL) break;
				for (int j = 0; j < players.Count; j++)
					if (i == players[j].Y && xBomb == players[j].X)
						playerMorti.Add(j);
			}

			return playerMorti;
		}
	}
}
