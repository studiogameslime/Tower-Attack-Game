using UnityEngine;
using UnityEngine.UI;

public class MonstersHealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthbarsprite;
    [SerializeField] private float _reduceSpeed = 2;
    private float _target = 1;
    private Camera _cam;


    void Start()
    {
        _cam = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;
        UpdateColor(_target);
    }

    private void Update()
    {
        transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward,_cam.transform.rotation * Vector3.up);
        _healthbarsprite.fillAmount = Mathf.MoveTowards(_healthbarsprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);
    }

    private void UpdateColor(float percent)
    {
        Color green = Color.green;
        Color yellow = Color.yellow;
        Color red = Color.red;

        if (percent > 0.5f)
        {
            float t = (percent - 0.5f) / 0.5f; // 0.5–1 becomes 0–1
            _healthbarsprite.color = Color.Lerp(yellow, green, t);
        }
        else
        {
       
            float t = percent / 0.5f; // 0–0.5 becomes 0–1
            _healthbarsprite.color = Color.Lerp(red, yellow, t);
        }
    }

}
