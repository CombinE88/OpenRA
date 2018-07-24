#region Copyright & License Information
/*
 * Copyright 2007-2018 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.IO;
using OpenRA.FileFormats;
using OpenRA.Mods.Common.FileFormats;

namespace OpenRA.Mods.Common.UtilityCommands
{
	public class PngSheetExportMetadataCommand : IUtilityCommand
	{
		string IUtilityCommand.Name { get { return "--PngSheetExport"; } }

		bool IUtilityCommand.ValidateArguments(string[] args)
		{
			return args.Length == 3;
		}

		[Desc("PNGFILE YAMLFILE", "Export png metadata to yaml")]
		void IUtilityCommand.Run(Utility utility, string[] args)
		{
			var s = File.OpenRead(args[1]);
			var png = new Png(s);
			s.Close();
			var yaml = "";

			foreach (var entry in png.Meta)
				yaml += entry.Key + ": " + entry.Value + "\n";

			File.WriteAllText(args[2], yaml);
		}
	}
}
