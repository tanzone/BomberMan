using System;
using System.Windows.Media;

namespace BomberMan
{
	public class Constant
	{
		#region Variabii Finestra di Gioco per Renderla Dinamica
		/// <summary>
		/// DIMENSIONI FINESTRA IN MODO DINAMICO
		/// </summary>
		public static int NUM_CELLS = 11;
		public static int NUM_CELLS_WALL = 2;
		public static int GRID_WIDTH = ((NUM_CELLS + NUM_CELLS_WALL) * 100) / 2;
		public static int GRID_HEIGHT = ((NUM_CELLS + NUM_CELLS_WALL) * 100) / 2;
		public static int GRID_CELL_LENGHT = GRID_HEIGHT / (NUM_CELLS + NUM_CELLS_WALL);

		public static int ROOT_MARGIN_WIDTH = GRID_WIDTH + 17;
		public static int ROOT_MARGIN_HEIGHT = GRID_HEIGHT + 40;

		#endregion

		#region Costruzione, Tempistiche Bomba, Colorazioni e Radius del campo di gioco 
		/// <summary>
		/// COSTRUZIONE DEL CAMPO DA GIOCO
		/// </summary>
		public static string UNBREAKABLE_WALL = "+";
		public static string BREAKABLE_WALL = "^";
		public static string NO_WALL = "=";
		public static string EXPLOSION = "E";

		public static Color COLORE_MURO_HARD = Colors.PowderBlue; //Colors.Azure; Colors.Turquoise
		public static Color COLORE_MURO_SOFT = Colors.LightPink;
		public static Color COLORE_MURO_NO = Colors.MintCream;
		public static Color COLORE_BOMB = Colors.Black;
		public static Color COLORE_EXPLOSION = Colors.PaleVioletRed;

		public static int TIME_EXPLOSION = 3000;
		public static int TIME_EXPLOSION_VIS = TIME_EXPLOSION/10;

		public static int BORDER_RADIUS_PLAYER = 15;
		public static int BORDER_RADIUS_BOMB = 100;

		#endregion

		#region Settaggi Giocatori
		/// <summary>
		/// IMPOSTAZIONI GIOCATORI DINAMICI
		/// </summary>
		public static string PLAYER_ONE = "0";
		public static string PLAYER_TWO = "1";
		public static string PLAYER_THREE = "2";
		public static string PLAYER_FOUR = "3";

		public static Color COLORE_PLAYER_ONE = Colors.Green;
		public static Color COLORE_PLAYER_TWO = Colors.Red;
		public static Color COLORE_PLAYER_THREE = Colors.Violet;
		public static Color COLORE_PLAYER_FOUR = Colors.Orange;


		public static int[] X = new int[] { 1, NUM_CELLS, 1, NUM_CELLS };
		public static int[] Y = new int[] { 1, NUM_CELLS, NUM_CELLS, 1 };
		public static string[] NAME_PLAYER = new string[] { PLAYER_ONE, PLAYER_TWO, PLAYER_THREE, PLAYER_FOUR };
		public static Color[] COLOR_PLAYER = new Color[] { COLORE_PLAYER_ONE, COLORE_PLAYER_TWO, COLORE_PLAYER_THREE, COLORE_PLAYER_FOUR };

		#endregion

		#region Variabili Utili per il programmatore
		/// <summary>
		/// VARIABILI UTILI
		/// </summary>
		public static string USERNAME_MYPLAYER = Environment.UserName;
		public static int INDEX_MY_PLAYER = 0;

		public static int UDP_PORT_DEFAULT = 5000;
		public static int NUM_MAX_PORT = 65535;
		public static int NUM_MIN_PORT = 1000;
		public static int PERSONE_ON = 1;

		public static string FILENAME = "xmlDati.xml";

		#endregion

		#region Protocolli per i Pacchetti
		/// <summary>
		/// PACCHETTI DA INVIARE
		/// </summary>
		public const string LINK = "N";
		public const string RIFIUTO = "NOK";
		public const string CONFERMA = "OK";
		public const string RIGHT = "R";
		public const string LEFT = "L";
		public const string UP = "U";
		public const string DOWN = "D";
		public const string BOMB = "B";
		public const string END = "F";
		public const string WIN = "1";
		public const string LOSE = "2";
		public const string DRAW = "0";

		#endregion

		#region Input Tastiera per i vari Movimenti
		/// <summary>
		/// MOVIMENTI DI INPUT
		/// </summary>
		public const string MOV_RIGHT = "Right";
		public const string MOV_LEFT = "Left";
		public const string MOV_UP = "Up";
		public const string MOV_DOWN = "Down";
		public const string MOV_BOMB = "Space";

		#endregion

		#region Campo di Gioco, public static Field, Funzioni utili per il campo
		/// <summary>
		/// CAMPO DA GIOCO
		/// </summary>
		public static Field field;

		public static void CreaCampoGioco(int numPlayer)
		{
			field = new Field(numPlayer);
		}

		public static void ChiudiCampo()
		{
			field = null;
		}

		#endregion
	}
}
