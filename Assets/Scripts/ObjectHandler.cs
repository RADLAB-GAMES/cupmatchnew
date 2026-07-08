using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectHandler : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isSwapping) return;

        if (GameManager.Instance.clickedOn.Count == 0)
        {
            GameManager.Instance.clickedOn.Add(this.gameObject);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.25f);
        }
        else
        {
            // check to see if item already exists in list
            if (GameManager.Instance.clickedOn.Contains(this.gameObject))
            {
                // remove item from list to be swapped and reset position
                GameManager.Instance.clickedOn.Remove(this.gameObject);
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.25f);
            }
            // doesn't exist in list so push to list and swap
            else
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.25f);
                GameManager.Instance.clickedOn.Add(this.gameObject);
                GameManager.Instance.UpdateGameState(GameState.Swap);
            }
        }
    }
}
