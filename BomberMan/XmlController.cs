using System.Xml.Serialization;
using System.IO;

namespace BomberMan
{
	public class XmlController
	{
		public static void Serializzazione_Dati2Xml(cDati dati)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(cDati));
			FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\" + Constant.FILENAME, FileMode.Create);
			serializer.Serialize(file, dati);
			file.Close();
		}

		public static void Deserializzazione_Xml2Dati()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(cDati));
			FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\" + Constant.FILENAME, FileMode.Open);
			SalvaDati(serializer.Deserialize(file) as cDati);
			file.Close();
		}

		private static void SalvaDati(cDati dati)
		{
			Constant.USERNAME_MYPLAYER = dati.myUser;
			UdpController.ipDestinatario = dati.ipDestinatario;
			UdpController.portaDestinatario = dati.portDestinatario;
		}
	}
}
