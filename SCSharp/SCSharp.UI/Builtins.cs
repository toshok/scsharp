//
// SCSharp.UI.Builtins
//
// Author:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace SCSharp.UI
{
	public static class Builtins
	{
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
		public static string rez_GluMainBin = "rez\\gluMain.bin";

		public static string rez_GluGameModeBin = "rez\\gluGameMode.bin";

		/* Campaign screen */
		public static string rez_GluCmpgnBin = "rez\\glucmpgn.bin"; // original
		public static string rez_GluExpcmpgnBin = "rez\\gluexpcmpgn.bin"; // broodwar

		/* Play custom screen */
		public static string rez_GluCustmBin = "rez\\gluCustm.bin";

		public static string rez_GluCreatBin = "rez\\gluCreat.bin";

		/* load saved screen */
		public static string rez_GluLoadBin = "rez\\gluLoad.bin";

		/* Login screen */
		public static string rez_GluLoginBin = "rez\\gluLogin.bin";
		public static string rez_GluPEditBin = "rez\\gluPEdit.bin";
		public static string rez_GluPOkBin = "rez\\gluPOk.bin";
		public static string rez_GluPOkCancelBin = "rez\\gluPOkCancel.bin";

		/* Ready room */
		public static string rez_GluRdyPBin = "rez\\glurdyp.bin";
		public static string rez_GluRdyTBin = "rez\\glurdyt.bin";

		/* Connection screen */
		public static string rez_GluConnBin = "rez\\gluConn.bin";

		/* Score screen */
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

		/* game menus */
		public static string rez_GameMenuBin = "rez\\gamemenu.bin";
		public static string rez_OptionsBin = "rez\\options.bin";
		public static string rez_SndDlgBin = "rez\\snd_dlg.bin";
		public static string rez_SpdDlgBin = "rez\\spd_dlg.bin";
		public static string rez_VideoBin = "rez\\video.bin";
		public static string rez_NetDlgBin = "rez\\netdlg.bin";
		public static string rez_ObjctDlgBin = "rez\\objctdlg.bin";
		public static string rez_AbrtMenuBin = "rez\\abrtmenu.bin";
		public static string rez_RestartBin = "rez\\restart.bin";
		public static string rez_QuitBin = "rez\\quit.bin";
		public static string rez_Quit2MnuBin = "rez\\quit2mnu.bin";
		public static string rez_HelpMenuBin = "rez\\helpmenu.bin";
		public static string rez_HelpBin = "rez\\help.bin";

		public static string rez_HelpTxtTbl = "rez\\help_txt.tbl";

		public static string rez_MinimapBin = "rez\\minimap.bin";
	}
}
