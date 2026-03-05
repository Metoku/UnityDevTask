using System;
using System.Collections.Generic;
using System.Linq;
using TestTask.NonEditable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        [Serializable]
        public struct MonsterVisualConfig //used a struct to pair monster names and their sprite
        {
            public MonsterNames type;
            public Sprite monsterSprite;
        }

        [Header("Monster UI References")]
        [SerializeField] private TextMeshProUGUI monsterName;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image currentMonsterSprite;

        [Header("Monster Visual Assets")]
        [SerializeField] private List<MonsterVisualConfig> monsterSprites;

        public int CurrentMonsterId { get; private set; }

        public void UpdateMonster(int monsterID, int MonsterType, float maxHP, float currentHP)
        {
            CurrentMonsterId = monsterID;// update monster ID

            //get monster name from enum and assign it to the text
            MonsterNames currentMonster = (MonsterNames)MonsterType;
            monsterName.text = currentMonster.ToFriendlyString();

            //assign monster HP Values
            healthBar.maxValue = maxHP;
            healthBar.value = currentHP;

            //setup and assign monster sprite to the image
            var config = monsterSprites.FirstOrDefault(visualConfig => visualConfig.type == currentMonster);
            if (config.monsterSprite != null)
            {
                currentMonsterSprite.sprite = config.monsterSprite;
            }
            else
            {
                Debug.LogWarning($"No sprite assigned for {currentMonster}!");
                return;
            }
            Debug.Log($"New Monster Spawned: ID:{monsterID} {currentMonster} with {maxHP} HP");
        }

        public void OnDamageMonsterButtonClicked()
        {
            ClientPacketsHandler.SendDamageRequest(CurrentMonsterId);

        }

        public void UpdateHealthBar(float currentHP)
        {
            healthBar.value = currentHP;
            Debug.Log($"Monster HP updated to: {currentHP}");
        }
    }
}