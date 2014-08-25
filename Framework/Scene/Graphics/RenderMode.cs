using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public enum RenderMode : int
	{
		/// <summary>
		/// Basic visual : color, texture
		/// </summary>
		BASE = 0,

		/// <summary>
		/// Light filter rendering :
		/// colors correspond to how much light can go through the objects.
		/// The resulting render will be used by the shadow pass.
		/// </summary>
		LIGHT_FILTER = 1,

		/// <summary>
		/// Rendering on the lightmap.
		/// Allows custom rendering.
		/// </summary>
		LIGHT_MAP = 2,

		/// <summary>
		/// Render on the glow layer.
		/// This layer will be blurred and rendered with an additive blending.
		/// </summary>
		GLOW_MAP = 3,

		COUNT = 4
	}
}
