using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace V2
{
	public class V2ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Header("$Mods.V2.Configs.Server.ContentLoading.Header")]

		[LabelKey("$Mods.V2.Configs.Server.ContentLoading.BasicMode.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.ContentLoading.BasicMode.Tooltip")]
		[ReloadRequired]
		[DefaultValue(false)]
		public bool BasicMode { get; set; }

		[Header("$Mods.V2.Configs.Server.Insight.Header")]

		[LabelKey("$Mods.V2.Configs.Server.Insight.DebugChatMessages.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.Insight.DebugChatMessages.Tooltip")]
		[DefaultValue(false)]
		public bool DebugChatMessages { get; set; }

		[Header("$Mods.V2.Configs.Server.Personalization.Header")]

		[LabelKey("$Mods.V2.Configs.Server.Personalization.RandomGulpsAgainstPlayers.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.Personalization.RandomGulpsAgainstPlayers.Tooltip")]
		[DefaultValue(false)]
		public bool RandomGulpsAgainstPlayers { get; set; }

		[Header("$Mods.V2.Configs.Server.JustForFun.Header")]

		[LabelKey("$Mods.V2.Configs.Server.JustForFun.DefenseInDigestionCalcs.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.JustForFun.DefenseInDigestionCalcs.Tooltip")]
		[DefaultValue(true)]
		public bool DefenseInDigestionCalcs { get; set; }

		[LabelKey("$Mods.V2.Configs.Server.JustForFun.EasilyEdibleEmpress.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.JustForFun.EasilyEdibleEmpress.Tooltip")]
		[DefaultValue(false)]
		public bool EasilyEdibleEmpress { get; set; }

		[LabelKey("$Mods.V2.Configs.Server.JustForFun.PermaChurnableEquipment.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.JustForFun.PermaChurnableEquipment.Tooltip")]
		[DefaultValue(false)]
		public bool PermaChurnableEquipment { get; set; }

		[LabelKey("$Mods.V2.Configs.Server.JustForFun.FatAssesBreakTiles.Label")]
		[TooltipKey("$Mods.V2.Configs.Server.JustForFun.FatAssesBreakTiles.Tooltip")]
		[DefaultValue(true)]
		public bool FatAssesBreakTiles { get; set; }

	}
}
