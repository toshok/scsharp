
namespace Starcraft {
	public static class Builtins {
		/* splash screen */
#if RELEASE
		public static string TitlePcx = "glue\\title\\title.pcx";
#else
		public static string TitlePcx = "glue\\title\\title-beta.pcx";
#endif

		public static string Game_ConsolePcx = "game\\{0}console.pcx";

		/* Main menu */
		public static string Palmm_BackgroundPcx = "glue\\palmm\\Backgnd.pcx";
		public static string Palmm_ArrowGrp = "glue\\palmm\\arrow.grp";
		public static string rez_GluMainBin = "rez\\gluMain.bin";

		/* Score screen */
		public static string Palv_BackgroundPcx = "glue\\pal{0}v\\Backgnd.pcx";
		public static string Palv_ArrowGrp = "glue\\pal{0}v\\arrow.grp";
		public static string rez_GluScoreBin = "rez\\gluScore.bin";

		public static string Scorev_pMainPcx = "glue\\score{0}v\\pMain.pcx";
	}
}
