using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform entryTemplate;

    private int height = 60;
    private const int MaxRows = 8;

    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh()
    {
        LeaderboardStatsStore.EnsureFileExists();
        ClearEntryRows();

        List<LeaderboardEntry> top = LeaderboardStatsStore.GetTopEntries(MaxRows);

        entryTemplate.gameObject.SetActive(false);
        for (int i = 0; i < MaxRows; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, container);
            RectTransform rectTransform = entryTransform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -height * i);
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("Rank").GetComponent<TMP_Text>().text = (i + 1).ToString();

            if (i < top.Count)
            {
                LeaderboardEntry e = top[i];
                entryTransform.Find("Player").GetComponent<TMP_Text>().text = e.playerName;
                entryTransform.Find("Score").GetComponent<TMP_Text>().text = e.round.ToString();
            }
            else
            {
                entryTransform.Find("Player").GetComponent<TMP_Text>().text = "—";
                entryTransform.Find("Score").GetComponent<TMP_Text>().text = "—";
            }
        }
    }

    private void ClearEntryRows()
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            if (child != entryTemplate)
                Destroy(child.gameObject);
        }
    }
}
