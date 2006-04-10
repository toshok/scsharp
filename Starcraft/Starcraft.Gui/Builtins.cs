
namespace Starcraft {
	public static class Builtins {
		/* title screen */
		public static string rez_TitleDlgBin = "rez\\titledlg.bin";
#if RELEASE
		public static string TitlePcx = "glue\\title\\title.pcx";
#else
		public static string TitlePcx = "glue\\title\\title-beta.pcx";
#endif

		/* UI strings */
		public static string rez_GluAllTbl = "rez\\gluAll.tbl";

		/* Main menu */
		public static string Palmm_BackgroundPcx = "glue\\palmm\\Backgnd.pcx";
		public static string Palmm_ArrowGrp = "glue\\palmm\\arrow.grp";
		public static string rez_GluMainBin = "rez\\gluMain.bin";

		public static string rez_GluGameModeBin = "rez\\gluGameMode.bin";

		/* Login screen */
		public static string PalNl_BackgroundPcx = "glue\\palnl\\Backgnd.pcx";
		public static string PalNl_ArrowGrp = "glue\\palnl\\arrow.grp";
		public static string rez_GluLoginBin = "rez\\gluLogin.bin";
		public static string rez_GluPEditBin = "rez\\gluPEdit.bin";
		public static string rez_GluPOkBin = "rez\\gluPOk.bin";
		public static string rez_GluPOkCancelBin = "rez\\gluPOkCancel.bin";

		/* Connection screen */
		public static string rez_GluConnBin = "rez\\gluConn.bin";

		/* Score screen */
		public static string Palv_BackgroundPcx = "glue\\pal{0}v\\Backgnd.pcx";
		public static string Palv_ArrowGrp = "glue\\pal{0}v\\arrow.grp";
		public static string Pald_BackgroundPcx = "glue\\pal{0}d\\Backgnd.pcx";
		public static string Pald_ArrowGrp = "glue\\pal{0}d\\arrow.grp";
		public static string rez_GluScoreBin = "rez\\gluScore.bin";

		public static string Scorev_pMainPcx = "glue\\score{0}v\\pMain.pcx";
		public static string Scored_pMainPcx = "glue\\score{0}d\\pMain.pcx";


		public static string Game_ConsolePcx = "game\\{0}console.pcx";


		/* scripts */
		public static string IScriptBin = "scripts\\iscript.bin";
		public static string AIScriptBin = "scripts\\aiscript.bin";

		/* arr files */
		public static string FlingyDat = "arr\\flingy.dat";
		public static string FlingyTbl = "arr\\flingy.tbl";
		public static string ImagesDat = "arr\\images.dat";
		public static string ImagesTbl = "arr\\images.tbl";
		public static string MapdataDat = "arr\\mapdata.dat";
		public static string MapdataTbl = "arr\\mapdata.tbl";
		public static string OrdersDat = "arr\\orders.dat";
		public static string OrdersTbl = "arr\\orders.tbl";
		public static string PortdataDat = "arr\\portdata.dat";
		public static string PortdataTbl = "arr\\portdata.tbl";
		public static string SfxDataDat = "arr\\sfxdata.dat";
		public static string SfxDataTbl = "arr\\sfxdata.tbl";
		public static string SpritesDat = "arr\\sprites.dat";
		public static string SpritesTbl = "arr\\sprites.tbl";
		public static string TechdataDat = "arr\\techdata.dat";
		public static string TechdataTbl = "arr\\techdata.tbl";
		public static string UnitsDat = "arr\\units.dat";
		public static string UnitsTbl = "arr\\units.tbl";
		public static string UpgradesDat = "arr\\upgrades.dat";
		public static string UpgradesTbl = "arr\\upgrades.tbl";
		public static string WeaponsDat = "arr\\weapons.dat";
		public static string WeaponsTbl = "arr\\weapons.tbl";

		/* sounds */
		public static string MouseoverWav = "sound\\glue\\mouseover.wav";
		public static string Mousedown2Wav = "sound\\glue\\mousedown2.wav";
		public static string SwishinWav = "sound\\glue\\swishin.wav";
		public static string SwishoutWav = "sound\\glue\\swishout.wav";

		/* credits */
		public static string RezCrdtexpTxt = "rez\\crdt_exp.txt";
		public static string RezCrdtlistTxt = "rez\\crdt_lst.txt";

		/* music */
		public static string music_titleWav = "music\\title.wav";
	}
}
