using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Tile
{
    Vector2 pos;
    Texture2D tex;
    Rectangle sourceRect;
    public Tile(Vector2 a_pos, Texture2D a_tex, Rectangle a_sourceRect)
    {
        pos = a_pos;
        tex = a_tex;
        sourceRect = a_sourceRect;
    }

    public void Draw(SpriteBatch a_sb)
    {
        a_sb.Draw(tex, pos, null, sourceRect, null, 0f, null, null, SpriteEffects.None, 0f);
    }
}
