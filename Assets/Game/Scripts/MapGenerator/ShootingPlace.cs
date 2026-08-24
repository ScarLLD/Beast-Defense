using UnityEngine;

namespace MapGenerator
{
    public class ShootingPlace : MonoBehaviour
    {
        public bool IsEmpty { get; private set; } = true;

        public void ChangeEmptyStatus(bool isEmpty)
        {
            IsEmpty = isEmpty;
        }
    }
}