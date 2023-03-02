using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLocker : MonoBehaviour
{
	[SerializeField] ChapterManager manager;

	// Update is called once per frame
	void Update()
    {
        for(int i = 0; i < manager.chapters.Count; i++)
		{
			if (i <= PlayerPrefs.GetInt("LevelLock")) {
				if (manager.chapters[i].defaultState == ChapterManager.ChapterState.Locked)
				{
					ChapterManager.SetUnlocked(manager.chapters[i].chapterID);
					manager.identifiers[i].SetUnlocked();
				}
			}
			else
			{
				if (manager.chapters[i].defaultState == ChapterManager.ChapterState.Unlocked)
				{
					manager.chapters[i].defaultState = ChapterManager.ChapterState.Locked;
					ChapterManager.SetLocked(manager.chapters[i].chapterID);
				}
			}
		}
	}
}
