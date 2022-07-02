namespace BomberMan
{
	public class Field
	{
		private string[][] campoGioco;

		public string[][] CampoGioco
		{
			get { return campoGioco; }
		}

		public Field(int numPlayer)
		{
			campoGioco = new string[Constant.NUM_CELLS + Constant.NUM_CELLS_WALL][];
			CreaCampo();
			SetPlayer(numPlayer);
		}

		private void CreaCampo()
		{
			for (int i = 0; i < Constant.NUM_CELLS + Constant.NUM_CELLS_WALL; i++)
			{
				campoGioco[i] = new string[Constant.NUM_CELLS + Constant.NUM_CELLS_WALL];
				for (int j = 0; j < Constant.NUM_CELLS + Constant.NUM_CELLS_WALL; j++)
					if ((i % 2 == 0) && (j % 2 == 0))
						campoGioco[i][j] = Constant.UNBREAKABLE_WALL;
					else if (((i % (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL - 1) == 0) || (j % (Constant.NUM_CELLS + Constant.NUM_CELLS_WALL - 1) == 0)) && Constant.NUM_CELLS_WALL == 2)
						campoGioco[i][j] = Constant.UNBREAKABLE_WALL;
					else
						campoGioco[i][j] = Constant.NO_WALL;
			}
		}

		private void SetPlayer(int numPlayer)
		{
			for (int i = 0; i < numPlayer; i++)
				if (numPlayer >= i)
					campoGioco[Constant.X[i]][Constant.Y[i]] = Constant.NAME_PLAYER[i];
		}
	}
}
