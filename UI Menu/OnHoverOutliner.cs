using LeTai.TrueShadow;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverOutliner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TrueShadow trueShadowComponent;

    private void Awake()
    {
        trueShadowComponent = GetComponent<TrueShadow>();
    }

    private void Start()
    {
        trueShadowComponent.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        trueShadowComponent.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        trueShadowComponent.enabled = false;
    }

    private void OnDisable()
    {
        trueShadowComponent.enabled = false;
    }
}
