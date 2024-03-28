using UnityEngine;
using RosMessageTypes.CtrlInterfaces;
using System.Collections;

public class FrankaSubscriber : MonoBehaviour
{
    // Array to hold the joint Articulation Bodies
    private ArticulationBody[] jointArticulationBodies;

    // public float topicHz = 200.0f; // Frequency to check for new messages
    private float jointAssignmentWait = 0.001f; // Time to wait after setting each joint position

    private RosConnector rosConnector;


    void Start()
    {
        rosConnector = FindObjectOfType<RosConnector>();
        jointArticulationBodies = new ArticulationBody[FrankaConstants.NumJoints];
        
        var linkName = string.Empty;
        for (var i = 0; i < FrankaConstants.NumJoints; i++)
        {
            linkName += FrankaConstants.LinkNames[i];
            jointArticulationBodies[i] = transform.Find(linkName).GetComponent<ArticulationBody>();
        }

        // Establish ROS connection and subscribe to the topic
        rosConnector.GetBridge().Subscribe<FrankaJointsMsg>(rosConnector.topicFrankaJoints, UpdateJointPositions);
    }

    
    void UpdateJointPositions(FrankaJointsMsg jointsMsg)
    {
        if (jointArticulationBodies.Length != jointsMsg.joints.Length)
        {
            Debug.LogWarning("Joint state message does not contain the expected number of joints.");
            return;
        }

        StartCoroutine(MoveJointsToTargetPositions(jointsMsg));
    }


    private IEnumerator MoveJointsToTargetPositions(FrankaJointsMsg jointsMsg)
    {
        for (int jointIndex = 0; jointIndex < FrankaConstants.NumJoints; jointIndex++)
        {
            if (jointIndex < jointArticulationBodies.Length)
            {
                var jointXDrive = jointArticulationBodies[jointIndex].xDrive;
                jointXDrive.target = (float)jointsMsg.joints[jointIndex] * Mathf.Rad2Deg; // Convert to degrees
                jointArticulationBodies[jointIndex].xDrive = jointXDrive;
            }
        }
        yield return new WaitForSeconds(jointAssignmentWait);
    }

    public void Unsubscribe()
    {
        rosConnector.GetBridge().Unsubscribe(rosConnector.topicFrankaJoints);
    }

}
