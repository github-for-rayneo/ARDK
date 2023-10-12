using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleCubeCtrl : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{


    private bool m_update = false;
    private bool m_rotDirection = false;

    //public void OnPointerEnter()
    //{
    //    //Debug.Log("[MercuryX2] Cube:OnPointerEnter");
    //}

    //public void OnPointerExit()
    //{
    //    m_update = false;
    //    Debug.Log("[MercuryX2] Cube:OnPointerExit");
    //}

    //public void OnPointerClick()
    //{
    //    Debug.Log("[MercuryX2] Cube:OnPointerClick");
    //}

    private void Update()
    {
        if (!m_update)
        {
            transform.Rotate(Vector3.forward * (m_rotDirection ? 1 : -1)*0.1f, Space.World);

            return;
        }

        transform.Rotate(Vector3.forward * (m_rotDirection ? 1 : -1), Space.World);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_update = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_update = false;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_rotDirection = !m_rotDirection;

    }
}