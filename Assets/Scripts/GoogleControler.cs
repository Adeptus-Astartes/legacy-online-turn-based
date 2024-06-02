using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using GooglePlayGames;
[System.Serializable]
public struct GPUserData
{
	public string id;
	public string username;
	public string avatarUrl;
	public Texture2D avatar;
	public string score;
	public int rank;
}
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
public class GoogleControler : MonoBehaviour 
{

	public static GoogleControler Instance;

	public InputField inputScore;
	public Text outputScore;

	public List<GPUserData> Users;
	public static GPUserData LocalUser;
	public Transform root;
	public GameObject prefab;


	void Awake()
	{
		Instance = this;
	}
	void Start () 
	{
		// Select the Google Play Games platform as our social platform implementation
		GooglePlayGames.PlayGamesPlatform.Activate();
	}

	public bool IsLogged()
	{
		return Social.localUser.authenticated;
	}

	public void Login()
	{
		Social.localUser.Authenticate((bool success) => {
			if (success) 
			{
				//mStatusText = "Welcome " + Social.localUser.userName;
				/*string token = GooglePlayGames.PlayGamesPlatform.Instance.GetToken();
				Debug.Log(token);*/
			}
		});

		Social.LoadScores (GPGSIds.leaderboard_globalleaderboard, scores => {

		});
	}

	public static void PostScore(int score)
	{
		Social.ReportScore(score, GPGSIds.leaderboard_globalleaderboard, (bool success) => {
			
		});

	}

	public void PlayerWin()
	{
		if(LocalUser.score == "")
		{
			//Seems Like we have no scores in leaderboards or score loading error, so we post 1 poin, google will catch this,
			//and if this is loading error and player have more points, then scores staying without changes;

			Social.ReportScore(1, GPGSIds.leaderboard_globalleaderboard, (bool success) => {
				if(success)
				{
					DebugGUI.Instance.Text = "\nFirst score report success";
				}
			});
		}
		else
		{
			long newScore = long.Parse(LocalUser.score) + 1;
			Social.ReportScore(newScore, GPGSIds.leaderboard_globalleaderboard, (bool success) => {
				if(success)
				{
					DebugGUI.Instance.Text = "\nScores reported : " + newScore.ToString();
					LocalUser.score = newScore.ToString();
				}
			});
		}

	}

	public void LoadScore()
	{
		if (Social.localUser.authenticated)
		{
			
			int x = -1;
			DebugGUI.Instance.Text += "\nBEGIN LOADING";
			Social.LoadScores (GPGSIds.leaderboard_globalleaderboard, scores => {
				x = scores.Length;
				List<string> users = new List<string>();

				for (int a = 0; a < scores.Length; a++) 
				{
					IScore score = scores[a];

					GPUserData _user = new GPUserData();
					_user.id = score.userID;
					_user.score = score.formattedValue;
					_user.rank = score.rank;
					Users.Add(_user);
					users.Add(_user.id);
					if(score.userID == Social.Active.localUser.id)
					{
						LocalUser = _user;
					}
				}
				StartCoroutine(WaitOnScores()); 
				Social.LoadUsers (users.ToArray(),(IUserProfile [] profiles) => 
					{
						for(int i = 0; i<profiles.Length;i++)
						{
							PlayGamesUserProfile profile = profiles[i] as PlayGamesUserProfile;
							GPUserData _user = Users[i];
							_user.username = profile.userName;
							_user.avatarUrl = profile.AvatarURL;
							StartCoroutine(LoadAvatar(i,_user.avatarUrl)); 
							Users[i] = _user;
						}
					});
			});

		}
	}

	public IEnumerator LoadAvatar(int userId, string url)
	{
		WWW www = new WWW(url);

		// Wait for download to complete
		yield return www;
		GPUserData _user = Users[userId];
		_user.avatar = www.texture;
		Users[userId] = _user;

		DebugGUI.Instance.Text += "\nFINISH";
	}

	public IEnumerator WaitOnScores()
	{
		DebugGUI.Instance.Text += "\nWAITING ON SCORES";
		yield return new WaitUntil(() => Users[0].avatar != null);
		LeaderboadLoader.Instance.Load(Users.ToArray());

	}


}
#endif
