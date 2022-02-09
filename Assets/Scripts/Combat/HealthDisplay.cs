using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTS.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject healthBarParent;
        [SerializeField] private Image healthBarImage;

        private void Awake()
        {
            health.ClientOnHealthUpdate += OnHealthUpdated;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthUpdate -= OnHealthUpdated;
        }

        private void OnHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / (float)maxHealth;
        }

        private void OnMouseEnter()
        {
            healthBarParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarParent.SetActive(false);
        }
    }
}