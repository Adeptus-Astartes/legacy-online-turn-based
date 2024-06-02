using System;
using UnityEngine;

public class Utility
{
	/// <summary>
	/// Cleans the path for texture in resources
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="val">Value.</param>
	public static string CleanPath( string val )
	{
		var res = val.Replace( "Assets/Resources/","" );
		return res.Replace( ".png","" );
	}
    public static string CleanPath(string val, string extension)
    {
        var res = val.Replace("Assets/Resources/", "");
        return res.Replace(extension, "");
    }
	public static string GetAssetPath( string fullPath )
	{
		return fullPath.Replace( Application.dataPath,"Assets" );
	}
	
	public enum ETextProperty
	{
		None,
		Upper,
		Lower,
		Trim
	}
	public static string StringProperty( string val, params ETextProperty[] textProperties )
	{
		if( textProperties != null )
		{
			foreach( var textProterty in textProperties )
			{
				switch( textProterty )
				{
				case ETextProperty.Upper:
					val = val.ToUpper();
					break;
				case ETextProperty.Lower:
					val = val.ToLower();
					break;
				case ETextProperty.Trim:
					val = val.Trim();
					break;
				}
			}
		}
		return val;
	}
	public static void SetValueText( UnityEngine.UI.Text obj, object val )
	{
		if( obj != null )
		{
			if( val != null )
			{
				obj.text = val.ToString();
			}
			else
			{
				obj.text = "";
			}
		}
		else
		{
			Debug.LogWarning( "Utility::SetValueText obj - NULL\n"+Environment.StackTrace );
		}
	}
	public static void SetValueImage( UnityEngine.UI.Image obj, Sprite val )
	{
		if( obj != null )
		{
			obj.sprite = val;
		}
		else
		{
			Debug.LogWarning( "Utility::SetValueImage obj - NULL\n"+Environment.StackTrace );
		}
	}
	public static void SetValueImage( UnityEngine.UI.RawImage obj, Texture val )
	{
		if( obj != null )
		{
			obj.texture = val;
		}
		else
		{
			Debug.LogWarning( "Utility::SetValueImage obj - NULL\n"+Environment.StackTrace );
		}
	}
	public static void SetSliderValue( UnityEngine.UI.Slider slider, float procent01 )
	{
		if( slider != null )
		{
			slider.value = procent01;
		}
		else
		{
			Debug.LogWarning( "Utility::SetValueImage slider - NULL\n"+Environment.StackTrace );
		}
	}
	
	public class TimeValuesString
	{
		public TimeValuesString() {}
		public string None = "None";
		public string D = "d";
		public string H = "h";
		public string M = "m";
		public string S = "s";
		public string MS = "ms";
		public string Delimeter = ".";
	}
	public static string CovertTimeFromSecond( double time, bool useMS = false, TimeValuesString stringval = null )
	{
		if (stringval == null)
		{
			stringval = new TimeValuesString();
		}
		if( time == 0 )
		{
			return stringval.None;
		}

		string resultString = "";

		TimeSpan timeSpan = TimeSpan.FromSeconds( time );
		int days = timeSpan.Days;
		int hours = timeSpan.Hours;
		int minutes = timeSpan.Minutes;
		int seconds = timeSpan.Seconds;
		int ms = timeSpan.Milliseconds;
		if( days > 0 )
		{
			resultString = days + stringval.D + " ";
		}
		if( hours > 0 )
		{
			resultString += hours + stringval.H + " ";
		}
		if( minutes > 0 )
		{
			resultString += minutes + stringval.M + " ";
		}
		if( seconds > 0 )
		{
			resultString += seconds + stringval.S;
		}
		if (useMS)
		{
			if (ms > 0)
			{
				resultString += stringval.Delimeter + ms + stringval.MS;
			}
		}
		return resultString;
	}

	public static void ActivateGameObject(GameObject go)
	{
		if (go != null)
		{
			go.SetActive(true);
		}
		else
		{
			Debug.LogWarning( "Utility::ActivateGameObject go - NULL\n"+Environment.StackTrace );
		}
	}

	public static void DeActivateGameObject(GameObject go)
	{
		if (go != null)
		{
			go.SetActive(false);
		}
		else
		{
			Debug.LogWarning( "Utility::DeActivateGameObject go - NULL\n"+Environment.StackTrace );
		}
	}

	public static class SwitchPosition
	{
		/// <summary>
		/// Switch the position by x between two transforms
		/// </summary>
		/// <param name="fromGo">From transform.</param>
		/// <param name="toGo">To transform.</param>
		public static void ByX( Transform fromGo, Transform toGo )
		{
			var vf = fromGo.localPosition.x;
			var v3 = fromGo.localPosition;
			v3.x = toGo.localPosition.x;
			fromGo.localPosition = v3;
			v3 = toGo.localPosition;
			v3.x = vf;
			toGo.localPosition = v3;
		}

		/// <summary>
		/// Switch the position by y between two transforms
		/// </summary>
		/// <param name="fromGo">From transform.</param>
		/// <param name="toGo">To transform.</param>
		public static void ByY( Transform fromGo, Transform toGo )
		{
			var vf = fromGo.localPosition.y;
			var v3 = fromGo.localPosition;
			v3.y = toGo.localPosition.y;
			fromGo.localPosition = v3;
			v3 = toGo.localPosition;
			v3.y = vf;
			toGo.localPosition = v3;
		}

		/// <summary>
		/// Switch the position by z between two transforms
		/// </summary>
		/// <param name="fromGo">From transform.</param>
		/// <param name="toGo">To transform.</param>
		public static void ByZ( Transform fromGo, Transform toGo )
		{
			var vf = fromGo.localPosition.z;
			var v3 = fromGo.localPosition;
			v3.z = toGo.localPosition.z;
			fromGo.localPosition = v3;
			v3 = toGo.localPosition;
			v3.z = vf;
			toGo.localPosition = v3;
		}
	}
}
public static class TransformExtension
{
	public static void LocalScaleX( this Transform trans, float x )
	{
		var v3 = trans.localScale;
		v3.x = x;
		trans.localScale = v3;
	}
	/// <summary>
	/// Return vector plus float variable (don't change source vector)
	/// </summary>
	/// <param name="vect">Vector3</param>
	/// <param name="val">float</param>
	public static Vector3 Plus (this Vector3 vect, float val)
	{
		return vect + new Vector3(val, val, val);
	}
}
public static class RectTransformExtensions
{
	public static void SetDefaultScale( this RectTransform trans )
	{
		trans.localScale = new Vector3( 1, 1, 1 );
	}
	public static void SetPivotAndAnchors( this RectTransform trans, Vector2 aVec )
	{
		trans.pivot = aVec;
		trans.anchorMin = aVec;
		trans.anchorMax = aVec;
	}

	public static Vector2 GetSize( this RectTransform trans )
	{
		return trans.rect.size;
	}
	public static float GetWidth( this RectTransform trans )
	{
		return trans.rect.width;
	}
	public static float GetHeight( this RectTransform trans )
	{
		return trans.rect.height;
	}

	public static void SetPositionOfPivot( this RectTransform trans, Vector2 newPos )
	{
		trans.localPosition = new Vector3( newPos.x, newPos.y, trans.localPosition.z );
	}

	public static void SetLeftBottomPosition( this RectTransform trans, Vector2 newPos )
	{
		trans.localPosition = new Vector3( newPos.x + ( trans.pivot.x * trans.rect.width ), newPos.y + ( trans.pivot.y * trans.rect.height ), trans.localPosition.z );
	}
	public static void SetLeftTopPosition( this RectTransform trans, Vector2 newPos )
	{
		trans.localPosition = new Vector3( newPos.x + ( trans.pivot.x * trans.rect.width ), newPos.y - ( ( 1f - trans.pivot.y ) * trans.rect.height ), trans.localPosition.z );
	}
	public static void SetRightBottomPosition( this RectTransform trans, Vector2 newPos )
	{
		trans.localPosition = new Vector3( newPos.x - ( ( 1f - trans.pivot.x ) * trans.rect.width ), newPos.y + ( trans.pivot.y * trans.rect.height ), trans.localPosition.z );
	}
	public static void SetRightTopPosition( this RectTransform trans, Vector2 newPos )
	{
		trans.localPosition = new Vector3( newPos.x - ( ( 1f - trans.pivot.x ) * trans.rect.width ), newPos.y - ( ( 1f - trans.pivot.y ) * trans.rect.height ), trans.localPosition.z );
	}

	public static void SetSize( this RectTransform trans, Vector2 newSize )
	{
		Vector2 oldSize = trans.rect.size;
		Vector2 deltaSize = newSize - oldSize;

		var minOffset = trans.offsetMin - new Vector2( deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y );
		var maxOffset = trans.offsetMax + new Vector2( deltaSize.x * ( 1f - trans.pivot.x ), deltaSize.y * ( 1f - trans.pivot.y ) );

		trans.offsetMin = minOffset;
		trans.offsetMax = maxOffset;
	}
	public static void SetWidth( this RectTransform trans, float newSize )
	{
		SetSize( trans, new Vector2( newSize, trans.rect.size.y ) );
	}
	public static void SetHeight( this RectTransform trans, float newSize )
	{
		SetSize( trans, new Vector2( trans.rect.size.x, newSize ) );
	}
}
