using UnityEngine;
using System.Collections;
using Assets.TemplateLibrary.Helpers;

public class UICoreBase : MonoBehaviour
{
	public	GameObject		RootObject;
	
	public bool Showed
	{
		get
		{
			return RootObject != null && RootObject.activeSelf;
		}
	}
	
	public	Animation		AnimationComponent;
	public	AnimationClip	ClipHide;
	public	AnimationClip	ClipShow;
	
	public System.Action<bool> AnimationClose;
	
	public ActionWrapper OnClosedAction = new ActionWrapper();
	public ActionWrapper OnOpenedAction = new ActionWrapper();
	
	public AudioSource	SourceToPlay;
	public AudioClip	OpenAudioClip;
	public AudioClip	CloseAudioClip;
	
	public virtual void UIHide( bool useAniimation = false )
	{
		if (!Showed)
		{
			return;
		}
		if (RootObject != null && useAniimation)
		{
			if (AnimationComponent != null && ClipHide != null)
			{
				if (AnimationClose != null)
				{
					AnimationClose(false);
				}
				AnimationComponent.clip = ClipHide;
				AnimationComponent.Play(ClipHide.name);
				
				if (SourceToPlay != null)
				{
					if (CloseAudioClip != null)
					{
						SourceToPlay.clip = CloseAudioClip;
						SourceToPlay.Play();
					}
				}
				
				StartCoroutine(AnimationComponent.WhilePlaying(()=>
					{
						if (AnimationClose != null)
						{
							AnimationClose(true);
						}
						if( OnClosedAction != null)
						{
							OnClosedAction.Dispatch();
						}
						RootObject.SetActive(false);
					}));
			}
			else
			{
				RootObject.SetActive(false);
				if( OnClosedAction != null)
				{
					OnClosedAction.Dispatch();
				}
			}
		} 
		else
		{
			RootObject.SetActive(false);
			if( OnClosedAction != null)
			{
				OnClosedAction.Dispatch();
			}
		}
	}
	public virtual void UIShowAnimation()
	{
		if (AnimationComponent != null && ClipShow != null)
		{
			if (AnimationClose != null)
			{
				AnimationClose(false);
			}
			AnimationComponent.clip = ClipShow;
			AnimationComponent.Play(ClipShow.name);
			StartCoroutine(AnimationComponent.WhilePlaying(()=>
				{
					if (AnimationClose != null)
					{
						AnimationClose(true);
					}
					if( OnOpenedAction != null)
					{
						OnOpenedAction.Dispatch();
					}
				}));
		}
	}
	public virtual void UIShow( bool useAniimation = false )
	{
		if (Showed)
		{
			return;
		}
		if (RootObject != null && useAniimation)
		{
			if (AnimationComponent != null && ClipShow != null)
			{
				if (AnimationClose != null)
				{
					AnimationClose(false);
				}
				AnimationComponent.clip = ClipShow;
				AnimationComponent.Play(ClipShow.name);
				
				RootObject.SetActive(true);
				if (SourceToPlay != null)
				{
					if (OpenAudioClip != null)
					{
						SourceToPlay.clip = OpenAudioClip;
						SourceToPlay.Play();
					}
				}
				StartCoroutine(AnimationComponent.WhilePlaying(()=>
					{
						if (AnimationClose != null)
						{
							AnimationClose(true);
						}
						if( OnOpenedAction != null)
						{
							OnOpenedAction.Dispatch();
						}
					}));
			}
			else
			{
				RootObject.SetActive(true);
				if( OnOpenedAction != null)
				{
					OnOpenedAction.Dispatch();
				}
			}
		}
		else
		{
			RootObject.SetActive(true);
			if( OnOpenedAction != null)
			{
				OnOpenedAction.Dispatch();
			}
		}
	}
}
public static class AnimationExtensions
{
	public static IEnumerator WhilePlaying( this Animation animation, System.Action calback )
	{
		do
		{
			yield return null;
		} 
		while ( animation.isPlaying );
		if (calback != null)
		{
			calback();
		}
	}
}