using Framework;

namespace Framework
{
	public class SpriteOrderFromY : Behaviour
	{
		public override void OnUpdate()
		{
			SpriteRenderer sprite = entity.sprite;
			if(sprite != null)
			{
				float ts = world.TS;
				float y = sprite.position.Y;
				y += sprite.scale.Y * (float)sprite.textureRect.Height;
				entity.sprite.drawOrder = (int)y;
			}
		}
	}
}
