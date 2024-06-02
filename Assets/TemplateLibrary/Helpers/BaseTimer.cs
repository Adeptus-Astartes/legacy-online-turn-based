using System;
using UnityEngine;

[Serializable]
public class BaseTimer
{
	public	bool	IsEnabled	= false;
	public	bool	IsLoop		= false;
	public	bool	IsReverse	= false;

	public	float	TickTime;
	public	float	CurrentTime
	{
		get
		{
			return _curTime;
		}
	}
	public	float	Percent;

	private	Action	_callbackTimerEnd;
	private	Action	_callbackEachTick;
	private	float	_curTime;

	public BaseTimer()
	{
		IsEnabled	= false;
	}

	public BaseTimer( Action callback )
	{
		_callbackTimerEnd = callback;
		IsEnabled	= false;
	}

	public void AddCallbackOfEachTick( Action callbackEachTick )
	{
		_callbackEachTick = callbackEachTick;
	}

	public void Start( float time, bool isReverse = false )
	{
		IsReverse	= isReverse;
		TickTime	= time;
		IsEnabled	= TickTime > 0;
		_curTime	= 0.0f;
	}

	public void Stop( bool withCallback = false )
	{
		IsEnabled = false;
		if( withCallback )
		{
			if( _callbackTimerEnd != null )
			{
				_callbackTimerEnd();
			}
		}
	}

	public void Update( float dt )
	{
		if( IsEnabled )
		{
			_curTime += dt;
			Percent = _curTime / TickTime;

			if( IsReverse )
			{
				Percent = 1.0f - Percent;
			}

			if( _curTime >= TickTime )
			{
				Percent = 1.0f;
				if( IsReverse )
				{
					Percent = 0.0f;
				}
			}

			if( _callbackEachTick != null )
			{
				_callbackEachTick();
			}

			if( _curTime >= TickTime )
			{
				_curTime = 0.0f;
				IsEnabled = IsLoop;

				if( _callbackTimerEnd != null )
				{
					_callbackTimerEnd();
				}
			}
		}
	}
	
	public void FixedUpdate( float dt )
	{
		if( IsEnabled )
		{
			_curTime += dt;
			Percent = _curTime / TickTime;

			if( IsReverse )
			{
				Percent = 1.0f - Percent;
			}

			if( _curTime >= TickTime )
			{
				Percent = 1.0f;
				if( IsReverse )
				{
					Percent = 0.0f;
				}
			}

			if( _callbackEachTick != null )
			{
				_callbackEachTick();
			}

			if( _curTime >= TickTime )
			{
				_curTime = 0.0f;
				IsEnabled = IsLoop;

				if( _callbackTimerEnd != null )
				{
					_callbackTimerEnd();
				}
			}
		}
	}
}