using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(GetMaterialStandingOn), typeof(AudioSource))]
public class FootStepPlayer : MonoBehaviour
{
    AudioMixerGroup sfx;
    AudioSource footStepPlayer;
    GameObject player;
    PlayerMovement pm;
    GetMaterialStandingOn materialDetector;
    Dictionary<string, AudioClip> matsToAudio;

    [Range(0.1f, 1.0f)]
    public float footStepCoolDown = 0.5f;
    private float resetCoolDown;

    public void Awake()
    {
        resetCoolDown = footStepCoolDown;
        AudioMixer mixer = Resources.Load("Sounds/Mixer") as AudioMixer;
        footStepPlayer = GetComponent<AudioSource>();
        matsToAudio = new Dictionary<string, AudioClip>();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (System.IO.File.Exists("FootStepConfig.json"))
        {
            string data = File.ReadAllText("FootStepConfig.json");
            DictionarySerializer s = JsonUtility.FromJson<DictionarySerializer>(data);
            dict = s.toDictionary();
        }

        foreach (KeyValuePair<string, string> kvp in dict)
        {
            if(kvp.Value == null || kvp.Value.Length == 0)
            {
                continue;
            }
            string name = kvp.Value;
            int idx = name.IndexOf(".");
            if (idx > 0)
            {
                name = name.Substring(0, idx);
                AudioClip clip = Resources.Load<AudioClip>("Sounds/Footsteps/" + name);
                matsToAudio.Add(kvp.Key, clip);
            }
        }


        if (mixer != null)
        {
            //only one of each group will be found, it just likes to return an array
            sfx = mixer.FindMatchingGroups("SFX")[0];
        }

        player = GameObject.FindGameObjectWithTag("Player");
        materialDetector = GetComponent<GetMaterialStandingOn>();
    }

    private string getMatchingName(string name)
    {
       int idx = name.IndexOf("(");
       string justName = name.Substring(0, idx -1);
       return justName;

    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if(pm == null)
        {
            pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        }

        if (pm.OnGround)
        {
            Vector3 vel = pm.GetComponent<Rigidbody>().velocity;
            if (vel.x > 0 || vel.z > 0)
            {
                footStepCoolDown -= Time.deltaTime;
                if (footStepCoolDown <= 0)
                {
                    footStepCoolDown = resetCoolDown;


                    if (materialDetector.current_mat != null)
                    {
                        string current_name = materialDetector.current_mat.name;
                        current_name = getMatchingName(current_name);
                        // Debug.Log(current_name);
                        if (matsToAudio.ContainsKey(current_name))
                        {
                            AudioClip clip;
                            matsToAudio.TryGetValue(current_name, out clip);
                            footStepPlayer.clip = clip;
                            if (clip != null)
                                footStepPlayer.Play();
                        }
                    }

                }
            }


        }

    }
}
