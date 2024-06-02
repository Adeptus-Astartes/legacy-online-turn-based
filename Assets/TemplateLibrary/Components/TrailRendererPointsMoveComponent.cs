using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererPointsMoveComponent : RectTransformPointsMoveComponent
{
	[SerializeField]	TrailRenderer	_trail;
	[SerializeField]	float			_timeForOutFromFade = 1f;
	
	float	_curentOutFadeTime;
	bool	_setedMaxColor;
	
	void UpdateMaterialAlpha( float a01)
	{
		if (_trail != null)
		{
			if (_trail.materials.Length > 0)
			{
				var color = _trail.materials[0].GetColor("_TintColor");
				color.a = a01;
				_trail.material.SetColor("_TintColor", color);
			}
		}
	}
	void Start()
	{
		StartMoveFromFirstpoint();
	}
	public override void StartMoveFromFirstpoint()
	{
		if (_trail != null)
		{
			_trail.enabled = true;
		}
		base.StartMoveFromFirstpoint();
	}
	protected override void OnStartAction()
	{
		_curentOutFadeTime = 0;
		_setedMaxColor = false;
		UpdateMaterialAlpha(0);
		if (_trail != null)
		{
			_trail.Reset(this);
		}
		base.OnStartAction();
	}
	protected override void Update()
	{
		_curentOutFadeTime += Time.deltaTime;
		if (_curentOutFadeTime <= _timeForOutFromFade)
		{
			UpdateMaterialAlpha(Mathf.Lerp(0, 1, _curentOutFadeTime / _timeForOutFromFade));
		}
		else
		{
			if (!_setedMaxColor)
			{
				_setedMaxColor = true;
				UpdateMaterialAlpha(1);
			}
		}
//		if (ComponentState == EComponentState.Enabled)
//		{
//			if (_trail != null)
//			{
//				_trail.enabled = true;
//			}
//		}
		base.Update();
	} 
}