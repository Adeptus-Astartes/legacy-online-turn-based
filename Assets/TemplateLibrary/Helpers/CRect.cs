using UnityEngine;

public class CRect
{
    /// <summary>
    /// Gets the center of rect.
    /// </summary>
    /// <value>The center.</value>
    public Vector2 Center   { get; private set; }
    /// <summary>
    /// Gets the size of rect.
    /// </summary>
    /// <value>The size.</value>
    public Vector2 Size     { get; private set; }

    public float X
    {
        get
        {
            return Center.x;
        }
    }
    public float Y
    {
        get
        {
            return Center.y;
        }
    }
    public float W
    {
        get
        {
            return Size.x;
        }
    }
    public float H
    {
        get
        {
            return Size.y;
        }
    }

    public float T  { get; set; }
    public float D  { get; set; }
    public float L  { get; set; }
    public float R  { get; set; }

    public float HalfW;
    public float HalfH;

    public CRect( Vector2 center, Vector2 size )
    {
        Center = center;
        Size = size;

        HalfW = size.x / 2f;
        HalfH = size.y / 2f;

        UpdateBoundPints();
    }
    public CRect( Vector2 center, float w, float h )
    {
        Center = center;
        Size = new Vector2( w, h );

        HalfW = w / 2f;
        HalfH = h / 2f;

        UpdateBoundPints();
    }
    void UpdateBoundPints()
    {
        T = Y + HalfH;
        D = Y - HalfH;

        R = X + HalfW;
        L = X - HalfW;
    }
    /// <summary>
    /// Водящий прямоугольник лежит в этом прямоугольнике
    /// </summary>
    public bool IsEntered(CRect r)
    {
        if( 
            R >= r.R 
            && L <= r.L
            && T >= r.T
            && D <= r.D
        )
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Этот прямоугольник лежит во входящем прямоугольнике
    /// </summary>
    public bool IsPartOfRect(CRect r)
    {
        if( 
            R <= r.R 
            && L >= r.L
            && T <= r.T
            && D >= r.D
        )
        {
            return true;
        }
        return false;

    }
    /// <summary>
    /// Текущий и входящий прямоугольники пересекаються 
    /// </summary>
    public bool IsIntersected(CRect r)
    {
        return L < r.R && R > r.L && T > r.D && D < r.T;
    }
    public override string ToString()
    {
        return string.Format("[CRect: Center={0}, Size={1}, X={2}, Y={3}, W={4}, H={5}, T={6}, D={7}, L={8}, R={9}]", Center, Size, X, Y, W, H, T, D, L, R);
    }
}