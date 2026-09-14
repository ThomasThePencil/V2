using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace V2.UI.DialogueStyles
{
	public class Divine : DialogueStyle
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string DisplayName => "Divine";

		public override string Description => "A special sort of style, reserved for only the treasured inhabitants of the Divine Realm.";

		public override bool CanBeSelected() => false;

		public override Texture2D DialogueBoxTileSheet => ModContent.Request<Texture2D>("V2/UI/DialogueStyles/Divine_MainBox", AssetRequestMode.ImmediateLoad).Value;
	}
}
