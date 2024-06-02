using UnityEngine;
using System.Collections.Generic;

public class RectTransformPointsMoveComponent : MonoBehaviour
{
	public enum EComponentState
	{
		Enabled,
		Disabled
	}
	
	[SerializeField] protected float				_minStartDelay	= 1f;
	[SerializeField] protected float				_maxStartDelay	= 3f;
	[SerializeField] protected float				_moveSpeed		= 35f;
	[SerializeField] protected List<RectTransform>	_movePoints		= new List<RectTransform>();
	[SerializeField] protected bool					_selfInit		= true;
	
	public RectTransform	Body;
	public EComponentState	ComponentState = EComponentState.Disabled;
	public bool				IsLoop;
	public bool				UsePreset		= true;
	public System.Action	OnEndWay;
	public bool				MoveForward		= true;
	
	int			_currentPointIndex;
	BaseTimer	_timer;
	
	public void InitComponent()
	{
		_timer = new BaseTimer(OnStartAction);
		if (Body == null)
		{
			Body = GetComponent<RectTransform>();
		}
	}
	
	public virtual void StartMoveFromLastPoint()
	{
		_currentPointIndex = _movePoints.Count -1;
		if (_timer != null)
		{
			_timer.Start(Random.Range(_minStartDelay, _maxStartDelay));
		}
	}
	public virtual void StartMoveFromFirstpoint()
	{
		_currentPointIndex = 1;
		if (_timer != null)
		{
			_timer.Start(Random.Range(_minStartDelay, _maxStartDelay));
		}
	}
	public virtual void StartMoveFromPoint( int index )
	{
		_currentPointIndex = Mathf.Clamp(index,0,_movePoints.Count-1);
		
//		Debug.LogError("move input "+index+" clamp "+_currentPointIndex + " move to "+_movePoints.Count);
		OnStartAction();
	}
	public virtual void Stop()
	{
		ComponentState = EComponentState.Disabled;
	}
	public void UpdateSpeed( float speed )
	{
		_moveSpeed = speed;
	}
	public void SetupMovePoints( List<RectTransform> movePoints)
	{
		_movePoints = movePoints;
	}
	
	void Awake()
	{
		if (_selfInit)
		{
			InitComponent();
		}
	}
	protected virtual void OnStartAction()
	{
		if (UsePreset)
		{
			if (_movePoints.Count > 0 && Body != null)
			{
				Body.position = _movePoints[0].position;
			}
		}
		ComponentState = EComponentState.Enabled;
	}
	protected virtual void Update()
	{
		if (_timer != null)
		{
			if (_timer.IsEnabled)
			{
				_timer.Update(Time.deltaTime);
			}
		}
		
		if (ComponentState == EComponentState.Enabled)
		{
			if (Body != null)
			{
                Body.position = Vector3.MoveTowards(Body.position, _movePoints[_currentPointIndex].position, Time.deltaTime * _moveSpeed);

                if ((Body.position - _movePoints[_currentPointIndex].position).magnitude <= 0.001f)
				{
					if (MoveForward)
					{
						_currentPointIndex++;
						if (_currentPointIndex >= _movePoints.Count)
						{
							ComponentState = EComponentState.Disabled;
							if (OnEndWay != null)
							{
								OnEndWay();
							}
							if (IsLoop)
							{
								StartMoveFromFirstpoint();
							}
						}
					}
					else
					{
						_currentPointIndex--;
						if (_currentPointIndex < 0)
						{
							ComponentState = EComponentState.Disabled;
							if (OnEndWay != null)
							{
								OnEndWay();
							}
							if (IsLoop)
							{
								StartMoveFromLastPoint();
							}
						}
					}
				}
			}
		}
	}
	
}