using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//#define EDITORMODE

[System.Serializable]
public class CAspectCams
{
	public Vector3			CamPos;
	public Vector3			CamScales;
	public Vector3			CamRotate;
	
	public Camera		CamTransform;
	
	public AnimationClip	ForCamClip;
	public Animation		AnimationsCam;
	public List<Vector2>	Aspects;
	public float			FOV;
	
	public bool Activate = false;
	
	public CAspectCams( Camera settings )
	{
		CamTransform = settings;
		
		CamPos = settings.transform.position;
		CamScales = settings.transform.localScale;
		CamRotate = settings.transform.eulerAngles;
		FOV = settings.fieldOfView;
	}
	
	public void Update()
	{
		if( Activate )	
		{
			Activate = false;
			SetupSettings();
		}
	}
	
	public void SetupSettings()
	{
		if (CamTransform != null)
		{
			CamTransform.transform.position = CamPos;
			CamTransform.transform.localScale = CamScales;
			CamTransform.transform.eulerAngles = CamRotate;
			CamTransform.fieldOfView = FOV;
		}
		if (AnimationsCam != null && ForCamClip != null)
		{
			AnimationsCam.clip = ForCamClip;
			AnimationsCam.Play();
		}
	}
}
#if EDITORMODE
[ExecuteInEditMode]
#endif
public class AspectRationSceneChecker : Singleton<AspectRationSceneChecker>
{
	public Camera CamSettings;
	public List<CAspectCams> CamsAspects;
    public bool SimulateResolution;
    public Vector2 Resolution;

    public CAspectCams CurrentAspectSettings { get; private set; }
    public Vector2 CurrentAspect { get; private set; }

	#if UNITY_EDITOR
	public bool Save;
	void Update()
	{
		if (Save)
		{
			Save = false;
			if (CamsAspects == null)
			{
				CamsAspects = new List<CAspectCams>();
			}
			var cmSet = new CAspectCams(CamSettings);
			CamsAspects.Add( cmSet );
		}
		foreach( var s in CamsAspects )
		{
			s.Update();	
		}
	}
	#endif
    
	void LoadAspectsRation()
	{
        CurrentAspect = Vector2.zero;
	    if (SimulateResolution && (Application.platform == RuntimePlatform.WindowsEditor
	                               || Application.platform == RuntimePlatform.OSXEditor))
	    {
	        CurrentAspect = AspectRatio.GetAspectRatio(Resolution, true);
	    }
	    else
	    {
	        CurrentAspect = AspectRatio.GetAspectRatio(Screen.width, Screen.height, true);
	    }
        CurrentAspectSettings = CamsAspects.FirstOrDefault(e => e.Aspects.Contains(CurrentAspect));

        if (CurrentAspectSettings != null)
		{
			CurrentAspectSettings.SetupSettings();
		}
		else
		{
			Debug.LogWarning("Place debug this place!!! Aspect is "+CurrentAspect);
		}
	}
	
	void Start()
	{
		LoadAspectsRation();
	}
}