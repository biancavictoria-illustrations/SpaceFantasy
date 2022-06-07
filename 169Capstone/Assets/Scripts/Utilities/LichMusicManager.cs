using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichMusicManager : MonoBehaviour
{
    public Transform startOfHallway;
    public Transform endOfHallway;

    private Vector3 hallwayVector;
    private bool hasStartedFight;

    void Start()
    {
        AudioManager.Instance.playMusic(AudioManager.MusicTrack.LichMusic);

        if(startOfHallway != null && endOfHallway != null)
        {
            hallwayVector = endOfHallway.position - startOfHallway.position;
        }

        hasStartedFight = false;
    }

    void Update()
    {
        if(!hasStartedFight && hallwayVector != Vector3.zero)
        {
            hallwayVector = endOfHallway.position - startOfHallway.position;
            Vector3 playerVector = Player.instance.transform.position - startOfHallway.position;
            Vector3 playerProjected = Vector3.Project(playerVector, hallwayVector);

            AudioManager.Instance.setLichHallwayProgress(Mathf.InverseLerp(0, hallwayVector.magnitude, playerProjected.magnitude));
        }
    }

    public void PlayLichDialogueMusic()
    {
        AudioManager.Instance.setLichMusicStage(1);
    }

    public void PlayLichFightMusic()
    {
        AudioManager.Instance.setLichMusicStage(2);
    }

    public void PlayLichVictoryMusic()
    {
        AudioManager.Instance.setLichMusicStage(3);
    }
}
