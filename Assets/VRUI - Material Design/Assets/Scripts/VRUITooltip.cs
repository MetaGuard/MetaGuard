using UnityEngine;

// VRUI Tooltip Component
namespace SpaceBear.VRUI
{
    [ExecuteAlways]
    public class VRUITooltip : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] Transform anchor;
        [SerializeField] LineRenderer link;

        private void Start()
        {
            link.positionCount = 2;
        }
        // Update is called once per frame
        void Update()
        {
            Vector3[] points = new Vector3[link.positionCount];
            points[0] = container.position;
            points[1] = anchor.position;
            link.SetPositions(points);
        }
    }
}
